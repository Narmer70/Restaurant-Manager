using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using PadTai.Classes;
using System.Data.SQLite;
using System.Windows.Forms;
using PadTai.Classes.Others;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using PadTai.Classes.Controlsdesign;
using PadTai.Sec_daryfolders.Others;
using PadTai.Sec_daryfolders.Others_Forms;
using PadTai.Sec_daryfolders.Updaters.FoodUpdater;


namespace PadTai.Sec_daryfolders.Updaters.Otherupdates
{
    public partial class Modifytables : UserControl
    {
        ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();
        private Clavieroverlay clavieroverlay;
        private DataGridViewScroller scroller;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;
        private DataTable dataTable;
        private string currentState;

        public Modifytables()
        {
            InitializeComponent();
            InitializeControlResizer();

            this.DoubleBuffered = true;
            crudDatabase = new CrudDatabase();
            dataGridView1.GridColor = colors.Color1;

            LocalizeControls();
            LoadTables();
            ApplyTheme();

        }

        private void InitializeControlResizer()
        {
            roundedPanel3.Visible = false;

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(label2);
            resizer.RegisterControl(label3);
            resizer.RegisterControl(label4);
            resizer.RegisterControl(label18);
            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(rjButton2);
            resizer.RegisterControl(pictureBox1);
            resizer.RegisterControl(roundedPanel1);
            resizer.RegisterControl(roundedPanel2);
            resizer.RegisterControl(roundedPanel3);
            resizer.RegisterControl(dataGridView1);
            resizer.RegisterControl(roundedTextbox1);
            resizer.RegisterControl(roundedTextbox2);
            resizer.RegisterControl(roundedTextbox3);
            resizer.RegisterControl(roundedTextbox4);
            scroller = new DataGridViewScroller(this, null, dataGridView1);
        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (resizer != null)
            {
                resizer.ResizeControls(this);

                subgroupDGVColumns();
            }
        }


        public void LoadTables()
        {
            string query = "SELECT TableID, Thetablenumber, Seatsamount FROM Tablenumber";
           
            dataTable = crudDatabase.FetchDataFromDatabase(query);

            if (dataTable.Rows.Count > 0)
            {
                // Add Update and Delete columns
                dataTable.Columns.Add("Update", typeof(string));
                dataTable.Columns.Add("Delete", typeof(string));

                foreach (DataRow row in dataTable.Rows)
                {
                    row["Update"] = "✏️";
                    row["Delete"] = "❌";
                }

                dataGridView1.AutoGenerateColumns = true;
                dataGridView1.DataSource = dataTable;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[1].Value != null && !string.IsNullOrEmpty(row.Cells[1].Value.ToString()))
                    {
                        row.Cells[1].Value = "N° " + row.Cells[1].Value.ToString();
                    }
                    if (row.Cells[2].Value == null || string.IsNullOrWhiteSpace(row.Cells[2].Value.ToString()))
                    {
                        row.Cells[2].Value = "0";
                    }
                }

                subgroupDGVColumns();
            }
            else
            {
                // Handle the case where no data is found
                label18.Text = "No tables found.";
                label18.ForeColor = Color.Tomato;
            }
        }

        private void subgroupDGVColumns()
        {
            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[1].DefaultCellStyle.Padding = new Padding(20, 0, 0, 0);
                dataGridView1.Columns[0].Width = (int)(dataGridView1.Width * 0.10);
                dataGridView1.Columns[1].Width = (int)(dataGridView1.Width * 0.60);
                dataGridView1.Columns[2].Width = (int)(dataGridView1.Width * 0.14);
                dataGridView1.Columns[3].Width = (int)(dataGridView1.Width * 0.08);
                dataGridView1.Columns[4].Width = (int)(dataGridView1.Width * 0.08);
                dataGridView1.RowTemplate.Height = 30;

                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                if (dataGridView1.Columns.Count > 0)
                {
                    dataGridView1.Columns[0].HeaderText = "ID";
                    dataGridView1.Columns[1].HeaderText = "Table number";
                    dataGridView1.Columns[2].HeaderText = "Seats amount";
                    dataGridView1.Columns["Update"].HeaderText = "Update";
                    dataGridView1.Columns["Delete"].HeaderText = "Delete";
                }

                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    dataGridView1.ColumnHeadersHeight = 30;
                    dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.DarkGray;
                    dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                    dataGridView1.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.DarkGray;
                    dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = dataGridView1.BackgroundColor;
                    dataGridView1.Columns[i].HeaderCell.Style.Font = new Font("Arial", 10, FontStyle.Bold);
                    dataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = dataGridView1.BackColor;
                    dataGridView1.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                }
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Update")
            {
                e.CellStyle.ForeColor = Color.DodgerBlue;
                e.CellStyle.SelectionForeColor = Color.DeepSkyBlue;
                e.CellStyle.Font = new Font("Century Gothic", 12, FontStyle.Bold);
            }

            else if (dataGridView1.Columns[e.ColumnIndex].Name == "Delete")
            {
                e.CellStyle.ForeColor = Color.Tomato;
                e.CellStyle.SelectionForeColor = Color.Orange;
                e.CellStyle.Font = new Font("Century Gothic", 12, FontStyle.Bold);
            }
        }

        private void roundedTextbox1_TextChanged(object sender, EventArgs e)
        {
            string filterExpression = roundedTextbox1.Text.Trim();

            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);

                if (!string.IsNullOrEmpty(filterExpression) && filterExpression != roundedTextbox1.WaterMark)
                {
                    dataView.RowFilter = $"Thetablenumber LIKE '%{filterExpression}%'";
                }
                else
                {
                    dataView.RowFilter = string.Empty;
                }
                dataGridView1.DataSource = dataView;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == dataGridView1.Columns["Update"].Index)
                {
                    var selectedId = dataGridView1.Rows[e.RowIndex].Cells[0].Value;

                    if (selectedId != null)
                    {
                        currentState = "Update";
                        rjButton1.Text = "Back";
                        label18.Text = string.Empty;
                        roundedPanel1.Enabled = false;
                        roundedTextbox2.ReadOnly = true;
                        roundedTextbox3.ReadOnly = false;
                        roundedTextbox4.ReadOnly = false;
                        rjButton2.Text = "Update the table";
                        rjButton2.BackColor = Color.DodgerBlue;
                        loadtablesbyID(selectedId.ToString());
                        rjButton1.BackColor = roundedPanel1.BackColor;
                        roundedTextbox2.WaterMark = "Optional";
                        roundedPanel3.Visible = true;
                    }
                }
                else if (e.ColumnIndex == dataGridView1.Columns["Delete"].Index)
                {
                    var selectedId = dataGridView1.Rows[e.RowIndex].Cells[0].Value;

                    if (selectedId != null)
                    {
                        currentState = "Delete";
                        rjButton1.Text = "Back";
                        label18.Text = string.Empty;
                        roundedPanel1.Enabled = false;
                        roundedTextbox4.ReadOnly = true;
                        roundedTextbox2.ReadOnly = true;
                        roundedTextbox3.ReadOnly = true;
                        rjButton2.BackColor = Color.Crimson;
                        rjButton2.Text = "Delete the table";
                        loadtablesbyID(selectedId.ToString());
                        rjButton1.BackColor = roundedPanel1.BackColor;
                        roundedTextbox2.WaterMark = "Optional";
                        roundedPanel3.Visible = true;
                    }
                }
            }
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
            if (roundedPanel3.Visible == false)
            {
                currentState = "Add";
                rjButton1.Text = "Back";
                label18.Text = string.Empty;
                roundedPanel3.Visible = true;
                roundedPanel1.Enabled = false;
                rjButton2.Text = "Add a dish";
                roundedTextbox2.ReadOnly = true;
                roundedTextbox3.ReadOnly = false;
                roundedTextbox4.ReadOnly = false;
                rjButton2.BackColor = Color.Green;
                rjButton1.BackColor = roundedPanel1.BackColor;
                roundedTextbox2.WaterMark = "Optional";
            }
            else if (roundedPanel3.Visible == true)
            {
                clearControls();
                roundedPanel3.Visible = false;
                roundedPanel1.Enabled = true;
                rjButton1.Text = "Add dish";
                rjButton1.BackColor = SystemColors.Highlight;
            }
        }

        private void loadtablesbyID(string tableId)
        {
            DataTable tableDetails = crudDatabase.FetchDataFromDatabase($"SELECT TableID, Thetablenumber, Seatsamount FROM Tablenumber WHERE TableID = {tableId}");

            if (tableDetails.Rows.Count > 0)
            {
                roundedTextbox2.Text = tableDetails.Rows[0]["TableID"].ToString();
                roundedTextbox3.Text = tableDetails.Rows[0]["Thetablenumber"].ToString();

                var seatsAmount = tableDetails.Rows[0]["Seatsamount"];
                roundedTextbox4.Text = seatsAmount != DBNull.Value ? seatsAmount.ToString() : "0";
            }
            else
            {
                roundedTextbox4.Text = "No details found.";
                roundedTextbox2.Text = "No details found.";
                roundedTextbox3.Text = "No details found.";
            }
        }

        public void InsertTable()
        {
            if (string.IsNullOrWhiteSpace(roundedTextbox3.Text))
            {
                label18.Text = "Please enter a Table";
                label18.ForeColor = Color.Tomato;
                return;
            }
            if (crudDatabase.CheckIfElementExists("Tablenumber", "Thetablenumber", roundedTextbox3.Text.Trim())) 
            {
                label18.Text = "This table already exists!";
                label18.ForeColor = Color.Tomato;
                return;
            }
            else
            {
                try
                {
                    Random random = new Random();
                    int positionX = random.Next(1, 701);
                    int positionY = random.Next(1, 701);

                    int height = 75;
                    int width = 75;
                    int isAvailable = 0;

                    // If TableID is empty, do not include it in the insert command
                   string insertCommand = @"INSERT INTO Tablenumber (Thetablenumber, Seatsamount, PositionX, PositionY, Width, Height, IsAvailable) 
                              VALUES (@TableNumber, @SeatsAmount, @PositionX, @PositionY, @Width, @Height, @IsAvailable)";

                    // Prepare parameters
                    var parameters = new Dictionary<string, object>
                    {
                        { "@TableNumber", roundedTextbox3.Text.Trim() },
                        { "@SeatsAmount", int.TryParse(roundedTextbox4.Text.Trim(), out int seatsAmount) ? seatsAmount : throw new InvalidOperationException("Please enter a valid number for Seats amount.") },
                        { "@PositionX", positionX },
                        { "@PositionY", positionY },
                        { "@Width", width },
                        { "@Height", height },
                        { "@IsAvailable", isAvailable }
                    };

                    // Execute the command using the CRUDDatabase class
                    bool isInserted = crudDatabase.ExecuteNonQuery(insertCommand, parameters);

                    if (isInserted)
                    {
                        LoadTables();
                        clearControls();
                        label18.Text = string.Empty;
                        this.Alert("Table inserted!", Alertform.enmType.Success);
                        label18.Text = "Table inserted successfully!";
                        label18.ForeColor = Color.LawnGreen;
                    }
                    else
                    {
                        label18.Text = "Failed to insert the table.";
                        label18.ForeColor = Color.Tomato;
                    }
                }
                catch (Exception ex)
                {
                    label18.Text = string.Empty;
                    label18.Text += ex.Message;
                    label18.ForeColor = Color.Tomato;
                }
            }
        }


        public void UpdateTablenumber()
        {
            // Validate Subgroup name input
            if (string.IsNullOrWhiteSpace(roundedTextbox3.Text))
            {
                label18.Text = "Please enter a table.";
                label18.ForeColor = Color.Tomato;
                return;
            }

            // Check if any row is selected in the DataGridView
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Get the selected row
                var selectedRow = dataGridView1.SelectedRows[0];
                var selectedId = selectedRow.Cells[0].Value;

                if (selectedId != null)
                {
                    string updateQuery = "UPDATE Tablenumber SET Thetablenumber = @tablenumber, Seatsamount = @Seatsamount WHERE TableID = @selectedId";

                    // Prepare parameters
                    var parameters = new Dictionary<string, object>
                    {
                        { "@tablenumber", roundedTextbox3.Text.Trim() }
                    };

                    // Validate and add Seatsamount
                    if (int.TryParse(roundedTextbox4.Text.Trim(), out int seatsAmount))
                    {
                        parameters.Add("@Seatsamount", seatsAmount);
                    }
                    else
                    {
                        label18.Text = "Please enter a valid number for Seats amount.";
                        label18.ForeColor = Color.Tomato;
                        return;
                    }

                    parameters.Add("@selectedId", selectedId);

                    // Execute the update command using the CRUDDatabase class
                    bool isUpdated = crudDatabase.ExecuteNonQuery(updateQuery, parameters);

                       
                    if (isUpdated)
                    {
                        label18.Text = "Table updated successfully!";
                        label18.ForeColor = Color.LawnGreen;
                        LoadTables();
                        clearControls();
                        rjButton1.Text = "Add table";
                        roundedPanel1.Enabled = true;
                        roundedPanel3.Visible = false;
                        rjButton1.BackColor = SystemColors.Highlight;
                        this.Alert("Table updated successfully!", Alertform.enmType.Success);
                    }
                    else
                    {
                        label18.Text = "No item found to update.";
                        label18.ForeColor = Color.Tomato;
                    }

                }
                else
                {
                    label18.Text = "Please select an item to update.";
                    label18.ForeColor = Color.Tomato;
                }
            }
        }


        public void DeleteSubgroup()
        {
            // Check if any row is selected in the DataGridView
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Get the selected row
                var selectedRow = dataGridView1.SelectedRows[0];
                var selectedId = selectedRow.Cells[0].Value;

                if (selectedId != null && int.TryParse(selectedId.ToString(), out int groupId))
                {
                    // Prepare the delete query
                    string deleteQuery = $"DELETE FROM Tablenumber WHERE TableID = {selectedId}";
                    var parameters = new Dictionary<string, object>
                    {
                        { "@id", groupId }
                    };

                    // Execute the delete command using ExecuteNonQuery
                    if (crudDatabase.ExecuteNonQuery(deleteQuery, parameters))
                    {
                        label18.Text = "Subgroup deleted successfully!";
                        label18.ForeColor = Color.LawnGreen;
                        LoadTables();
                        clearControls();
                        rjButton1.Text = "Add Subgroup";
                        roundedPanel1.Enabled = true;
                        roundedPanel3.Visible = false;
                        rjButton1.BackColor = SystemColors.Highlight;
                        this.Alert("Subgroup deleted successfully!", Alertform.enmType.Success);
                    }
                    else
                    {
                        label18.Text = "Failed to delete group.";
                        label18.ForeColor = Color.Tomato;
                    }
                }
                else
                {
                    label18.Text = "Invalid GroupID selected.";
                    label18.ForeColor = Color.Tomato;
                }
            }
            else
            {
                label18.Text = "Please select a group to delete.";
                label18.ForeColor = Color.Tomato;
            }
        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            if (currentState == "Update")
            {
                UpdateTablenumber();
            }
            else if (currentState == "Delete")
            {
                using (Deletedishconfirm DDC = new Deletedishconfirm())
                {
                    FormHelper.ShowFormWithOverlay(this.FindForm(), DDC);

                    // Check the DialogResult after the dialog is closed
                    if (DDC.DialogResult == DialogResult.OK)
                    {
                        DeleteSubgroup();
                        DDC.Close();
                    }
                    else if (DDC.DialogResult == DialogResult.Cancel)
                    {
                        DDC.Close();
                    }
                }
            }
            else if (currentState == "Add")
            {
                InsertTable();
            }
            else
            {
                roundedPanel3.Visible = false;
            }
        }

        private void clearControls()
        {
            roundedTextbox2.Text = string.Empty;
            roundedTextbox3.Text = string.Empty;
            roundedTextbox4.Text = string.Empty;

            if (currentState == "Add")
            {
                roundedTextbox2.WaterMark = "Optional";
            }
        }

        private void Alert(string msg, Alertform.enmType type)
        {
            Alertform alertform = new Alertform();
            alertform.showAlert(msg, type);
        }

        private void roundedTextbox1_Enter(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.showKeyboard)
            {
                clavieroverlay = new Clavieroverlay(roundedTextbox1);
                clavieroverlay.boardLocationBottom();
                clavieroverlay.Show();
            }
        }

        private void roundedTextbox1_Leave(object sender, EventArgs e)
        {
            clavieroverlay?.Hide();
        }

        public void LocalizeControls()
        {
            //button1.Text = LanguageManager.Instance.GetString("MF-btn1");
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
            //button4.Text = LanguageManager.Instance.GetString("MF-btn4");

        }

        public void ApplyTheme()
        {
            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            pictureBox1.Image = colors.Image1;
            roundedPanel1.BackColor = colors.Color3;
            roundedPanel2.BackColor = colors.Color3;
            roundedPanel3.BackColor = colors.Color3;
            roundedTextbox1.ForeColor = colors.Color2;
            roundedTextbox2.ForeColor = colors.Color2;
            roundedTextbox3.ForeColor = colors.Color2;
            roundedTextbox4.ForeColor = colors.Color2;
            roundedTextbox2.BackColorRounded = colors.Color1;
            roundedTextbox3.BackColorRounded = colors.Color1;
            roundedTextbox4.BackColorRounded = colors.Color1;

            rjButton1.ForeColor = this.ForeColor;
            dataGridView1.ForeColor = this.ForeColor;
            pictureBox1.BackColor = roundedPanel1.BackColor;
            roundedTextbox1.BackColorRounded = this.BackColor;
            roundedPanel1.BorderColor = roundedPanel1.BackColor;
            roundedTextbox1.BorderColor = roundedPanel1.BackColor;
            dataGridView1.BackgroundColor = roundedPanel3.BackColor;
            dataGridView1.DefaultCellStyle.ForeColor = this.ForeColor;
            dataGridView1.DefaultCellStyle.BackColor = roundedPanel3.BackColor;
            dataGridView1.DefaultCellStyle.SelectionForeColor = this.ForeColor;
            if (colors.Bool1 && Properties.Settings.Default.showScrollbars) showScrollBars();
        }

        private void showScrollBars()
        {
            dataGridView1.ScrollBars = ScrollBars.Vertical;
        }
    }   
}
