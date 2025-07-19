using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Drawing;
using PadTai.Classes;
using System.Diagnostics; 
using System.Data.SQLite;
using System.Windows.Forms;
using PadTai.Fastcheckfiles;
using PadTai.Classes.Others;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using PadTai.Sec_daryfolders.Updates;
using PadTai.Sec_daryfolders.Tablereserve;


namespace PadTai.Sec_daryfolders.Tablereservation
{
    public partial class TableStructure : UserControl
    {
        private Reservationform reservationform;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;


        private DataTable dataTable;
        private List<RJButton> buttons;
        private bool isDragging = false;
        private int draggingButtonIndex = -1;
        private Point dragStartPoint = new Point(0, 0);
        private bool isResizing = false;
        private Point resizeStartPoint;
        private ResizeCorner resizeCorner;

        private Point startPoint;
        private bool isSelecting = false;
        private Rectangle selectionRectangle;
        private List<RJButton> selectedButtons = new List<RJButton>();
        private bool isMoving = false;
        private Point moveOffset;

        public TableStructure(Reservationform reserve)
        {
            InitializeComponent();      
            InitializeControlResizer();

            crudDatabase = new CrudDatabase();
            buttons = new List<RJButton>();
            EnsureDataGridViewFocus();

            this.reservationform = reserve;           
            roundedPanel1.Visible = false;
            DoubleBuffered = true; 
     
           
            toggleSwitch1.IsOn = Properties.Settings.Default.tablebtnslocked;
            LocalizeControls();
            ApplyTheme();
            LoadData();
            dataGridView1.GridColor = this.BackColor;
        }

        #region Controls Resizers
        private void InitializeControlResizer()
        {
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);
            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(label1);
            resizer.RegisterControl(panel1);
            resizer.RegisterControl(textBox6);
            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(rjButton2);
            resizer.RegisterControl(rjButton3);
            resizer.RegisterControl(rjButton4);
            resizer.RegisterControl(pictureBox1);
            resizer.RegisterControl(toggleSwitch1);
            resizer.RegisterControl(dataGridView1);
            resizer.RegisterControl(roundedPanel1);

            if (!Properties.Settings.Default.tablebtnslocked)
            {
                dataGridView1.MouseDown += Control_MouseDown;
                dataGridView1.MouseEnter += Control_MouseEnter;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (resizer != null)
            {
                resizer.ResizeControls(this);
                SetupDataGridViewColumns();
            }
        }
        #endregion

        #region Move and redraw buttons

        public void EnsureDataGridViewFocus()
        {
            LoadButtonPositions();
            panel1.Visible = false;
            pictureBox1.Visible = true;
            rjButton1.Visible = !Properties.Settings.Default.tablebtnslocked;
            rjButton3.Visible = !Properties.Settings.Default.tablebtnslocked;

            if (!Properties.Settings.Default.tablebtnslocked)
            {
                pictureBox1.Paint += pictureBox1_Paint;
                pictureBox1.MouseUp += pictureBox1_MouseUp;
                pictureBox1.MouseDown += pictureBox1_MouseDown;
                pictureBox1.MouseMove += pictureBox1_MouseMove;
            }
        }

        private void toggleSwitch1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.tablebtnslocked = toggleSwitch1.IsOn;
            Properties.Settings.Default.Save();
            reloadPage();
        }

        private void rjButton4_Click(object sender, EventArgs e)
        {
            if (panel1.Visible == false)
            {
                pictureBox1.Visible = false;
                panel1.Visible = true;
            }
            else if (panel1.Visible == true)
            {
                pictureBox1.Visible = true;
                panel1.Visible = false;
            }

            if (roundedPanel1.Visible == true)
            {
                roundedPanel1.Visible = false;
                updateButtonStatus();
                reloadPage();
            }
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (roundedPanel1.Visible == true)
            {
                updateButtonStatus();
                roundedPanel1.Visible = false;
                reloadPage();
            }

            if (panel1.Visible == true)
            {
                pictureBox1.Visible = true;
                panel1.Visible = false;
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                startPoint = e.Location;
                isSelecting = true;
                isMoving = false;
                selectionRectangle = new Rectangle(startPoint, new Size(0, 0));
                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSelecting)
            {
                selectionRectangle = new Rectangle(
                    Math.Min(startPoint.X, e.X),
                    Math.Min(startPoint.Y, e.Y),
                    Math.Abs(startPoint.X - e.X),
                    Math.Abs(startPoint.Y - e.Y)
                );
                pictureBox1.Invalidate();
            }
            if (isMoving && selectedButtons.Count > 0)
            {
                int deltaX = e.X - moveOffset.X;
                int deltaY = e.Y - moveOffset.Y;

                foreach (var button in selectedButtons)
                {
                    button.Left += deltaX;
                    button.Top += deltaY;
                }

                moveOffset = e.Location;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (isSelecting)
            {
                isSelecting = false;

                selectedButtons.Clear();

                foreach (var button in buttons)
                {
                    if (selectionRectangle.IntersectsWith(button.Bounds))
                    {
                        selectedButtons.Add(button);
                    }
                }

                if (selectedButtons.Count > 0)
                {
                    isMoving = true;
                    moveOffset = e.Location;
                }

                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (isSelecting)
            {
                using (var brush = new SolidBrush(Color.FromArgb(51, Color.DodgerBlue)))
                {
                    e.Graphics.FillRectangle(brush, selectionRectangle);
                }

                using (var pen = new Pen(Color.DodgerBlue, 2))
                {
                    e.Graphics.DrawRectangle(pen, selectionRectangle);
                }
            }
        }

        #endregion

        #region DGV Fetch-Display-Update     

        private void LoadData()
        {
            string query = "SELECT TableID, ThetableNumber, IsAvailable FROM Tablenumber";

            dataTable = crudDatabase.FetchDataFromDatabase(query);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = dataTable;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[1].Value != null && !string.IsNullOrEmpty(row.Cells[1].Value.ToString()))
                    {
                        row.Cells[1].Value = "     N° " + row.Cells[1].Value.ToString();
                    }
                }

                SetupDataGridViewColumns();
            }
            else
            {
            }
        }

        private void SetupDataGridViewColumns()
        {
            if (dataGridView1.Columns.Count > 0)
            {
                // Modify the FoodID column
                var foodIdColumn = dataGridView1.Columns["TableID"];
                if (foodIdColumn != null)
                {
                    foodIdColumn.HeaderText = " ";
                    foodIdColumn.Width = 50;
                    foodIdColumn.ReadOnly = true;
                    foodIdColumn.DisplayIndex = 0;
                    foodIdColumn.Visible = false;
                }

                // Modify the FoodName column
                var foodNameColumn = dataGridView1.Columns["ThetableNumber"];
                if (foodNameColumn != null)
                {
                    foodNameColumn.HeaderText = "ThetableNumber";
                    foodNameColumn.Width = (int)(dataGridView1.Width * 0.75);
                    foodNameColumn.ReadOnly = true;
                    foodNameColumn.DisplayIndex = 1;
                }

                // Modify the IsChecked column
                var isCheckedColumn = dataGridView1.Columns["IsAvailable"];
                if (isCheckedColumn != null)
                {
                    isCheckedColumn.HeaderText = "IsAvailable";
                    isCheckedColumn.Width = 40;
                    isCheckedColumn.DisplayIndex = 2;

                    // Convert the IsChecked column to a checkbox column
                    DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn
                    {
                        DataPropertyName = "IsAvailable",
                        Name = "IsAvailable",
                        HeaderText = "IsAvailable",
                        Width = (int)(dataGridView1.Width * 0.249),
                        DisplayIndex = 2
                    };

                    // Remove the old column and add the checkbox column
                    dataGridView1.Columns.Remove(isCheckedColumn);
                    dataGridView1.Columns.Add(checkBoxColumn);
                }
            }
        }


        private void updateButtonStatus()
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        // Ensure TableID is not null and is of the correct type
                        if (row.Cells["TableID"].Value != DBNull.Value)
                        {
                            int tableID = Convert.ToInt32(row.Cells["TableID"].Value);

                            // Check if IsAvailable is not null and convert it to boolean
                            bool newIsAvailable = row.Cells["IsAvailable"].Value != DBNull.Value && Convert.ToBoolean(row.Cells["IsAvailable"].Value);

                            // Retrieve the current IsAvailable status from the database
                            string query = "SELECT IsAvailable FROM Tablenumber WHERE TableID = @TableID";
                            var parameters = new Dictionary<string, object>
                            {
                                { "@TableID", tableID }
                            };

                            object result = crudDatabase.FetchDataFromDatabase(query, parameters).Rows[0]["IsAvailable"];
                            bool currentIsAvailable = result != null && Convert.ToBoolean(result);

                            // Only update if the new value is different from the current value
                            if (newIsAvailable != currentIsAvailable)
                            {
                                string updateQuery = "UPDATE Tablenumber SET IsAvailable = @IsAvailable WHERE TableID = @TableID";
                                var updateParameters = new Dictionary<string, object>
                                {
                                    { "@IsAvailable", newIsAvailable ? 1 : 0 },
                                    { "@TableID", tableID }
                                };

                                // Execute the update
                                crudDatabase.ExecuteNonQuery(updateQuery, updateParameters);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void reloadPage() 
        {
            TableStructure tableStructure = new TableStructure(reservationform);
            reservationform.AdduserControl(tableStructure);
        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            if (panel1.Visible == true)
            {
                pictureBox1.Visible = true;
                panel1.Visible = false;
            }           
            
            if (roundedPanel1.Visible == true)
            {
                roundedPanel1.Visible = false;
                updateButtonStatus();
                reloadPage();
            }
            else if (roundedPanel1.Visible == false) 
            { 
                roundedPanel1.Visible = true;
            }

          
        }

        private void FilterDataGridView(string searchText)
        {

            if (string.IsNullOrEmpty(searchText))
            {
                LoadData(); // Restore all data
                return;
            }

            var filter = searchText.ToLower();

            DataView dataView = new DataView(dataTable);
            dataView.RowFilter = $"ThetableNumber LIKE '%{filter}%'";
            dataGridView1.DataSource = dataView;
        }


        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            FilterDataGridView(textBox6.Text);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (dataGridView1.Focused)
            {
                int newIndex = dataGridView1.FirstDisplayedScrollingRowIndex + (e.Delta > 0 ? -1 : 1);
                if (newIndex >= 0 && newIndex < dataGridView1.Rows.Count)
                {
                    try
                    {
                        dataGridView1.FirstDisplayedScrollingRowIndex = newIndex;
                    }
                    catch (InvalidOperationException)
                    {

                    }
                }
            }
        }

        private void Control_MouseEnter(object sender, EventArgs e)
        {
            // Set focus to the control that the mouse entered
            if (sender is Control control)
            {
                control.Focus();
            }
        }

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            // Set focus to the control that was touched or clicked
            if (sender is Control control)
            {
                control.Focus();
            }
        }
        #endregion

        #region Buttons Update - Resize
            
        private enum ResizeCorner
        {
            None,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        private void LoadButtonPositions()
        {
            string query = "SELECT TableID, ThetableNumber, Seatsamount, PositionX, PositionY, Width, Height, IsAvailable FROM Tablenumber";

            DataTable dataTable = crudDatabase.FetchDataFromDatabase(query);

            foreach (DataRow row in dataTable.Rows)
            {
                var buttonID = row.IsNull("TableID") ? 0 : Convert.ToInt32(row["TableID"]);
                var buttonName = row.IsNull("ThetableNumber") ? string.Empty : row["ThetableNumber"].ToString();
                int seatsAmount = row.IsNull("Seatsamount") ? 0 : Convert.ToInt32(row["Seatsamount"]);
                var positionX = row.IsNull("PositionX") ? 0 : Convert.ToInt32(row["PositionX"]);
                var positionY = row.IsNull("PositionY") ? 0 : Convert.ToInt32(row["PositionY"]);
                var width = row.IsNull("Width") ? 75 : Convert.ToInt32(row["Width"]);
                var height = row.IsNull("Height") ? 75 : Convert.ToInt32(row["Height"]);
                var isAvailable = row.IsNull("IsAvailable") ? false : Convert.ToInt32(row["IsAvailable"]) == 0;

                var button = new RJButton
                {
                    Tag = buttonID,
                    BorderRadius = 5,
                    Name = buttonName,
                    Size = new Size(width, height),
                    Location = new Point(positionX, positionY),
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    BackColor = isAvailable ? Color.DodgerBlue : Color.Chocolate,
                    Text = GetButtonText(buttonName, seatsAmount)
                };

                if (!Properties.Settings.Default.tablebtnslocked)
                {
                    button.MouseDown += Button_MouseDown;
                    button.MouseMove += Button_MouseMove;
                    button.MouseUp += Button_MouseUp;
                }

                button.Click += Button_Click;
                buttons.Add(button);
                pictureBox1.Controls.Add(button);
            }
        }

        private string GetButtonText(string buttonName, int seatsAmount)
        {
            if (seatsAmount == 0)
            {
                return buttonName;
            }
            else if (seatsAmount == 1)
            {
                return $"{buttonName}{Environment.NewLine}({seatsAmount} {LanguageManager.Instance.GetString("TBS-Place")})";
            }
            else
            {
                return $"{buttonName}{Environment.NewLine}({seatsAmount} {LanguageManager.Instance.GetString("TBS-Places")})";
            }
        }

        private void Button_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is RJButton button)
            {
                if (e.Button == MouseButtons.Left)
                {
                    // Check if the click is near the corners for resizing
                    int cornerSize = 10; // Adjust corner size as needed
                    if (IsMouseInResizeCorner(button, e.Location, cornerSize, out resizeCorner))
                    {
                        isResizing = true;
                        resizeStartPoint = e.Location;
                        button.Capture = true; //Capture the button
                    }
                    else
                    {
                        isDragging = true;

                        dragStartPoint = new Point(e.X, e.Y);
                        draggingButtonIndex = buttons.IndexOf(sender as RJButton);
                        button.Capture = true;
                    }
                }
            }
        }

        private bool IsMouseInResizeCorner(RJButton button, Point mouseLocation, int cornerSize, out ResizeCorner corner)
        {
            corner = ResizeCorner.None;

            if (mouseLocation.X <= cornerSize && mouseLocation.Y <= cornerSize)
            {
                corner = ResizeCorner.TopLeft;
                return true;
            }
            if (mouseLocation.X >= button.Width - cornerSize && mouseLocation.Y <= cornerSize)
            {
                corner = ResizeCorner.TopRight;
                return true;
            }
            if (mouseLocation.X <= cornerSize && mouseLocation.Y >= button.Height - cornerSize)
            {
                corner = ResizeCorner.BottomLeft;
                return true;
            }
            if (mouseLocation.X >= button.Width - cornerSize && mouseLocation.Y >= button.Height - cornerSize)
            {
                corner = ResizeCorner.BottomRight;
                return true;
            }
            return false;
        }

        private void Button_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is RJButton button)
            {
                if (isResizing)
                {
                    int widthChange = e.X - resizeStartPoint.X;
                    int heightChange = e.Y - resizeStartPoint.Y;

                    if (resizeCorner == ResizeCorner.TopLeft)
                    {
                        button.Location = new Point(button.Location.X + widthChange, button.Location.Y + heightChange);
                        button.Size = new Size(button.Width - widthChange, button.Height - heightChange);
                    }

                    if (resizeCorner == ResizeCorner.TopRight)
                    {
                        button.Location = new Point(button.Location.X, button.Location.Y + heightChange);
                        button.Size = new Size(button.Width + widthChange, button.Height - heightChange);

                    }
                    if (resizeCorner == ResizeCorner.BottomLeft)
                    {
                        button.Location = new Point(button.Location.X + widthChange, button.Location.Y);
                        button.Size = new Size(button.Width - widthChange, button.Height + heightChange);

                    }
                    if (resizeCorner == ResizeCorner.BottomRight)
                    {
                        button.Size = new Size(button.Width + widthChange, button.Height + heightChange);
                    }

                    resizeStartPoint = e.Location;

                }
                else if (isDragging && draggingButtonIndex >= 0)
                {
                    var btn = buttons[draggingButtonIndex];

                    Point newLocation = this.PointToClient(MousePosition);
                    newLocation.Offset(-dragStartPoint.X, -dragStartPoint.Y);
                    btn.Location = newLocation;
                }
            }
        }

        private void Button_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && sender is RJButton button)
            {
                if (isDragging && draggingButtonIndex >= 0)
                {
                    isDragging = false;
                    draggingButtonIndex = -1;
                    button.Capture = false;
                }
                else if (isResizing)
                {
                    isResizing = false;
                    button.Capture = false;
                }
            }
        }

        Receiptdetails receiptdetails;
        private void Button_Click(object sender, EventArgs e)
        {              
            if (isMoving)
            {
                isMoving = false; 
                return; 
            }
            if (roundedPanel1.Visible == true)
            {
                roundedPanel1.Visible = false;
                updateButtonStatus();
                reloadPage();
            }
            else if (roundedPanel1.Visible == false)
            {
                if (sender is RJButton button && button.BackColor == Color.DodgerBlue && Properties.Settings.Default.tablebtnslocked)
                {
                    Fastcheck fastcheck = new Fastcheck(receiptdetails);
                    fastcheck.Show();
                    fastcheck.label8.Text = button.Name;
                }
            }       
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
            if (roundedPanel1.Visible == true)
            {
                roundedPanel1.Visible = false;
                updateButtonStatus();
                reloadPage();
            }

            if (panel1.Visible == true)
            {
                pictureBox1.Visible = true;
                panel1.Visible = false;
            }

            UpdateTableLocation();
        }

        private void UpdateTableLocation()
        {
            List<RJButton> buttons = pictureBox1.Controls.OfType<RJButton>().ToList();
            if (buttons.Count == 0) return;

            string query = "UPDATE Tablenumber SET PositionX = @PositionX, PositionY = @PositionY, Width = @Width, Height = @Height WHERE TableID = @TableID";

            foreach (RJButton button in buttons)
            {
                var parameters = new Dictionary<string, object>
                {
                     { "@TableID", button.Tag },
                     { "@PositionX", button.Location.X },
                     { "@PositionY", button.Location.Y },
                     { "@Width", button.Width },
                     { "@Height", button.Height }
                };

                // Execute the update for each button
                crudDatabase.ExecuteNonQuery(query, parameters);
            }
        }

        private void rjButton3_Click(object sender, EventArgs e)
        {
            if (roundedPanel1.Visible == true)
            {
                roundedPanel1.Visible = false;
                updateButtonStatus();
                reloadPage();
            }

            if (panel1.Visible == true)
            {
                pictureBox1.Visible = true;
                panel1.Visible = false;
            }

            ArrangeButtonsRect();
        }

        private const int padding = 10;
        private const int ButtonWidth = 75;
        private const int ButtonHeight = 75;

        private void ArrangeButtonsRect()
        {
            try
            {
                // Fetch button IDs from the database
                string query = "SELECT TableID FROM Tablenumber ORDER BY TableID";
                DataTable buttonData = crudDatabase.FetchDataFromDatabase(query);

                List<int> buttonIds = new List<int>();

                // Populate buttonIds from the DataTable
                foreach (DataRow row in buttonData.Rows)
                {
                    // Use safe casting
                    object tableIdValue = row["TableID"];
                    if (tableIdValue != DBNull.Value)
                    {
                        // Check if the value is of type int or can be converted to int
                        if (tableIdValue is int tableId)
                        {
                            buttonIds.Add(tableId);
                        }
                        else if (tableIdValue is long longId)
                        {
                            buttonIds.Add(Convert.ToInt32(longId)); // Convert long to int
                        }
                        else if (tableIdValue is string strId && int.TryParse(strId, out int parsedId))
                        {
                            buttonIds.Add(parsedId); // Parse string to int
                        }
                        else
                        {
                            // Handle unexpected types if necessary
                            MessageBox.Show($"Unexpected type for TableID: {tableIdValue.GetType()}");
                        }
                    }
                }

                // The rest of your code remains unchanged...

                // Get the current width and height of the user control
                int userControlWidth = this.Width;
                int userControlHeight = this.Height;

                // Calculate the number of buttons per row based on user control width
                int buttonsPerRow = (userControlWidth + padding) / (ButtonWidth + padding);

                // Calculate total space needed for all buttons
                int totalWidthNeeded = 0;
                int totalHeightNeeded = 0;

                if (buttonIds.Count > 0)
                {
                    int rows = (int)Math.Ceiling((double)buttonIds.Count / buttonsPerRow);
                    totalWidthNeeded = (buttonsPerRow * (ButtonWidth + padding)) - padding;
                    totalHeightNeeded = (rows * (ButtonHeight + padding)) - padding;
                }

                // Calculate the available space in the user control
                int availableSpace = userControlHeight - totalHeightNeeded;

                // Calculate the starting Y position with equal spacing above and below
                int topBottomPadding = availableSpace / 2;
                int startY = topBottomPadding;

                // Calculate the starting X position (centered)
                int centerX = userControlWidth / 2;
                int startX = centerX - totalWidthNeeded / 2;

                for (int i = 0; i < buttonIds.Count; i++)
                {
                    int row = i / buttonsPerRow;
                    int column = i % buttonsPerRow;

                    // Calculate button's x and y offset from the starting position
                    int xOffset = column * (ButtonWidth + padding);
                    int yOffset = row * (ButtonHeight + padding);

                    // Apply offset to starting position to calculate button location
                    int xPosition = startX + xOffset;
                    int yPosition = startY + yOffset;

                    if (xPosition + ButtonWidth > userControlWidth || yPosition + ButtonHeight > userControlHeight)
                    {
                        // Optionally handle the case where buttons exceed the user control bounds
                        return;
                    }

                    // Find and relocate the button
                    RJButton button = pictureBox1.Controls.Find(buttonIds[i].ToString(), true).FirstOrDefault() as RJButton;

                    if (button != null)
                    {
                        button.Location = new Point(xPosition, yPosition);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region App theme and language
        public void LocalizeControls()
        {
            //button1.Text = LanguageManager.Instance.GetString("MF-btn1");
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            textBox6.ForeColor = this.ForeColor;
      
            toggleSwitch1.OffColor = colors.Color3;
            roundedPanel1.BackColor = colors.Color3;
            dataGridView1.ForeColor = this.ForeColor;
            textBox6.BackColorRounded = this.BackColor;            
            dataGridView1.BackgroundColor = colors.Color3;
            dataGridView1.DefaultCellStyle.BackColor = colors.Color3;
            dataGridView1.DefaultCellStyle.ForeColor = this.ForeColor;
            dataGridView1.DefaultCellStyle.SelectionForeColor = this.ForeColor;

            label1.ForeColor = this.ForeColor;
            pictureBox1.BackColor = this.BackColor;
            //toggleSwitch1.OffColor = this.BackColor;
            toggleSwitch1.LabelColor = this.ForeColor;
        }
        #endregion
    }  
}

