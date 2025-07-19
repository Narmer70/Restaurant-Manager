using System;
using System.Linq;
using System.Data;
using PadTai.Classes;
using System.Drawing;
using System.Data.SQLite;
using System.Windows.Forms;
using PadTai.Classes.Others;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using PadTai.Classes.Controlsdesign;
using PadTai.Sec_daryfolders.Tablereservation;
using PadTai.Sec_daryfolders.Updaters.Otherupdates;


namespace PadTai.Sec_daryfolders.Tablereserve
{
    public partial class Reservationform : Form
    {
        ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();
        private const int ItemsPerPageOtherPages = 10; 
        private const int ItemsPerPageFirstPage = 11; 
        private int _totalReservations = 0;
        private CrudDatabase crudDatabase;
        private RJButton buttonPrevious;
        private FontResizer fontResizer;
        private ControlResizer resizer;        
        private int _currentPage = 1;
        private RJButton buttonNext;
        
        public Reservationform()
        {
            InitializeComponent();
            initialaiseControlResizing();

            LocalizeControls();
            ApplyTheme();
        }

        private void initialaiseControlResizing() 
        {
            panel2.Visible = false;
            tableLayoutPanel1.Visible = false;

            crudDatabase = new CrudDatabase();
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(panel1);
            resizer.RegisterControl(panel2);
            resizer.RegisterControl(label1);
            resizer.RegisterControl(button1);
            resizer.RegisterControl(button2);
            resizer.RegisterControl(button3);
            resizer.RegisterControl(button5);
            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(pictureBox1);
            resizer.RegisterControl(tableLayoutPanel1);

            UpdateControlVisibility();
        }

        public bool HasMoreThanOneClient()
        {
            int clientId;

            if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                _totalReservations = 0; 
                return false;
            }

            string query = "SELECT COUNT(*) FROM Reservetable WHERE ClientID = @ClientID";
            var parameters = new Dictionary<string, object>
            {
                { "@ClientID", clientId }
            };

            DataTable resultTable = crudDatabase.FetchDataFromDatabase(query, parameters);

            if (resultTable.Rows.Count > 0)
            {
                long count = Convert.ToInt64(resultTable.Rows[0][0]);
                _totalReservations = Convert.ToInt32(count);

                return count > 0; // Return true if count is greater than 0
            }
            else 
            {
                _totalReservations = 0;
                return false;
            }
        }

        public void UpdateControlVisibility()
        {
            bool hasMoreThanOneClient = HasMoreThanOneClient();

            panel2.Visible = !hasMoreThanOneClient;
            tableLayoutPanel1.Visible = hasMoreThanOneClient;
        }

        private void CenterLabel()
        {
            panel2.Left = (this.ClientSize.Width - panel2.Width) / 2;
            label1.Left = (panel2.ClientSize.Width - label1.Width) / 2;
        }

        public void LoadData()
        {
            buttonPrevious = new RJButton
            {
                Text = "◀",
                Width = 145,
                Height = 75,
                Visible = false,
                Dock = DockStyle.Fill,
                Margin = new Padding(4),
                BackColor = Color.DodgerBlue,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            buttonPrevious.Click += buttonPrevious_Click;
            tableLayoutPanel1.Controls.Add(buttonPrevious, 0, 0);
            tableLayoutPanel1.SetColumnSpan(buttonPrevious, 1);


            buttonNext = new RJButton
            {
                Text = "▶",
                Width = 145,
                Height = 75,
                Visible = false,
                Dock = DockStyle.Fill,
                Margin = new Padding(4),
                BackColor = Color.DodgerBlue,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)

            };
            buttonNext.Click += buttonNext_Click;


            int clientId;

            // Try to parse the ClientID from settings
            if (int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                string query = "SELECT * FROM Reservetable WHERE ClientID = @ClientID ORDER BY ReservationID DESC";
                var parameters = new Dictionary<string, object>
                {
                    { "@ClientID", clientId }
                };

                try
                {
                    // Fetch data from the database
                    DataTable reservationsTable = crudDatabase.FetchDataFromDatabase(query, parameters);
                  
                    if (reservationsTable == null || reservationsTable.Rows.Count == 0)
                    {
                        return; 
                  
                    }
                    tableLayoutPanel1.Controls.Clear();
                    tableLayoutPanel1.RowStyles.Clear();

                    float[] rowsPercentages = new float[3] { 33.3F, 33.3F, 33.3F };
                    foreach (float percentage in rowsPercentages)
                    {
                        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, percentage));
                    }

                    int itemsPerPage = _currentPage == 1 ? ItemsPerPageFirstPage : ItemsPerPageOtherPages;
                    int offset = (_currentPage - 1) * itemsPerPage;

                    // Apply pagination
                    DataView view = new DataView(reservationsTable);
                    view.Sort = "ReservationID DESC";
                   
                    if (view.Count == 0)
                    {
                        return; 
                    }
                    DataTable paginatedTable = view.ToTable().AsEnumerable().Skip(offset).Take(itemsPerPage).CopyToDataTable();

                    int rowIndex = 0;             

                    foreach (DataRow row in paginatedTable.Rows)
                    {
                        RoundedPanel panel = new RoundedPanel
                        {
                            BorderRadius = 0,
                            Dock = DockStyle.Fill,
                            Margin = new Padding(4),
                            BackColor = Color.Transparent,
                            GradientTopColor = Color.Transparent,
                            GradientBottomColor = Color.Transparent
                        };
                        resizer.RegisterControl(panel);

                        DataGridView dgv = new DataGridView
                        {
                            ReadOnly = true,
                            Dock = DockStyle.Fill,
                            Margin = new Padding(4),
                            RowHeadersVisible = false,
                            AllowUserToAddRows = false,
                            ColumnHeadersVisible = false,
                            ScrollBars = ScrollBars.None,
                            AllowUserToDeleteRows = false,
                            AllowUserToResizeRows = false,
                            BorderStyle = BorderStyle.None,
                            AllowUserToOrderColumns = false,
                            AllowUserToResizeColumns = false,
                            CellBorderStyle = DataGridViewCellBorderStyle.None,
                            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                            Tag = row["ReservationID"].ToString(),
                        };
                        resizer.RegisterControl(dgv);
                        dgv.CellFormatting += Dgv_CellFormatting;
                        dgv.MouseEnter += Dgv_MouseEnter;
                        dgv.MouseLeave += Dgv_MouseLeave;
                        dgv.CellClick += Dgv_CellClick;
                        dgv.Resize += Dgv_Resize;

                        // Set up columns
                        dgv.ColumnCount = 2;
                        dgv.Columns[0].Width = (int)(dgv.Width * 0.50);
                        dgv.Columns[1].Width = (int)(dgv.Width * 0.50);

                        // Add rows and set values
                        dgv.Rows.Add(); // Row 1
                        dgv.Rows.Add(); // Row 2
                        dgv.Rows.Add(); // Row 3

                        // Set values
                        dgv[0, 0].Value = $"  {row["ReservationID"]}";

                        dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

                        string combinedText = $"    {row["Guestgender"]}\n" +
                                              $"    {row["GuestName"]}\n" +
                                              $"    {row["GuestContact"]}";

                        dgv[0, 1].Value = combinedText;

                        dgv[0, 2].Value = $"     {row["Guesttime"]}";
                        dgv[1, 1].Value = $" {row["Tablenumber"]}";

                        // Check for completed and cancelled conditions
                        if (row["Completed"] != DBNull.Value)
                        {
                            dgv[1, 2].Value = $"{LanguageManager.Instance.GetString("Tablecompleted")}     ";
                            dgv[1, 2].Style.ForeColor = Color.LimeGreen;
                            dgv[1, 2].Style.SelectionForeColor = Color.LimeGreen;
                        }
                        else if (row["Cancelled"] != DBNull.Value)
                        {
                            dgv[1, 2].Value = $"{LanguageManager.Instance.GetString("Tablecancelled")}     ";
                            dgv[1, 2].Style.ForeColor = Color.Tomato;
                            dgv[1, 2].Style.SelectionForeColor = Color.Tomato;
                        }

                        // Add the cell panel to the TableLayout Panel
                        panel.Controls.Add(dgv);
                        tableLayoutPanel1.Controls.Add(panel, rowIndex % 4, rowIndex / 4);
                        rowIndex++;

                        // Adjust the row count if necessary
                        if (rowIndex % 4 == 0)
                        {
                            tableLayoutPanel1.RowCount++;
                        }
                    }

                    // Add navigation buttons
                    tableLayoutPanel1.Controls.Add(buttonPrevious, 0, 0);
                    tableLayoutPanel1.Controls.Add(buttonNext, 3, 2);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }

                ApplyTheme();
            }
            else
            {
                return;
            }
        }

        private void Dgv_Resize(object sender, EventArgs e)
        {
            DataGridView dgv = sender as DataGridView;

            if (dgv != null)
            {
                int totalHeight = (int)(dgv.ClientSize.Height * 0.99); 
                int numberOfRows = dgv.Rows.Count;

                int row2Height = (int)(totalHeight * 0.70); 
                dgv.Rows[1].Height = row2Height; 

                int remainingHeight = totalHeight - row2Height;
                int remainingRowsCount = numberOfRows - 1; 

                int newRowHeight = remainingRowsCount > 0 ? remainingHeight / remainingRowsCount : 0;

                for (int i = 0; i < numberOfRows; i++)
                {
                    if (i != 1) // Skip the second row
                    {
                        dgv.Rows[i].Height = newRowHeight; 
                    }
                }

                dgv.Columns[0].Width = (int)(dgv.Width * 0.50); 
                dgv.Columns[1].Width = (int)(dgv.Width * 0.50);
            }
        }

        private void Dgv_CellClick(object sender, EventArgs e)
        {
            DataGridView dgv = sender as DataGridView;

            if (dgv != null)
            {
                // Retrieve the ReceiptId from the Tag property
                if (int.TryParse(dgv.Tag.ToString(), out int reservationId))
                {
                    Reservationdetails resdetailsForm = new Reservationdetails(this)
                    {
                        ReservationId = reservationId
                    };

                    using (Reservationdetails RD = new Reservationdetails(this))
                    {
                        FormHelper.ShowFormWithOverlay(this.FindForm(), resdetailsForm);
                    }
                }
            }
        }

        private void Dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (sender is DataGridView dgv)
            {
                dgv.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Century Gothic", 10, FontStyle.Regular);

                if (e.RowIndex == 0 && e.ColumnIndex == 0)
                {
                    //e.CellStyle.ForeColor = Color.Gray;
                    //e.CellStyle.SelectionForeColor = Color.Gray;
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    e.CellStyle.Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Regular);
                }
                else if (e.RowIndex == 1 && e.ColumnIndex == 0)
                {
                    e.CellStyle.ForeColor = Color.Gray;
                    e.CellStyle.SelectionForeColor = Color.Gray;                    
                    e.CellStyle.Font = new System.Drawing.Font("Century Gothic", 10, FontStyle.Regular);
                }
                else if (e.RowIndex == 1 && e.ColumnIndex == 1)
                {
                   
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    e.CellStyle.Font = new System.Drawing.Font("Century Gothic", 30, FontStyle.Bold);
                }
                else if (e.RowIndex == 2 && e.ColumnIndex == 1)
                {
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (e.RowIndex == 2 && e.ColumnIndex == 0)
                {
                    e.CellStyle.ForeColor = System.Drawing.Color.DodgerBlue;
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    e.CellStyle.SelectionForeColor = System.Drawing.Color.DodgerBlue;
                    e.CellStyle.Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold);
                }
            }
        }

        private void Dgv_MouseEnter(object sender, EventArgs e)
        {
            DataGridView dgv = sender as DataGridView;

            if (dgv != null)
            {
                dgv.BackgroundColor = colors.Color4;
                dgv.DefaultCellStyle.BackColor = colors.Color4;
                dgv.DefaultCellStyle.SelectionBackColor = colors.Color4;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = colors.Color4;
                dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = colors.Color4;
            }
        }

        private void Dgv_MouseLeave(object sender, EventArgs e)
        {
            DataGridView dgv = sender as DataGridView;

            if (dgv != null)
            {
                dgv.BackgroundColor = colors.Color3;
                dgv.DefaultCellStyle.BackColor = colors.Color3;
                dgv.DefaultCellStyle.SelectionBackColor = colors.Color3;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = colors.Color3;
                dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = colors.Color3;
            }
        }

        public void AdduserControl(UserControl UserControl)
        {
            UserControl.Dock = DockStyle.Fill;
            panel1.Controls.Clear();
            panel2.Visible = false;
            panel1.Controls.Add(UserControl);
            UserControl.BringToFront();
        }
    
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();   
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
            Modifytables modifytables = new Modifytables();
            AdduserControl(modifytables);
        }

        private void Reserveform_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
            CenterLabel();
            LoadData();
            UpdatePaginationButtons();
        }


        private void UpdatePaginationButtons()
        {
            buttonPrevious.Visible = _currentPage > 1;
            int itemsPerPage = _currentPage == 1 ? ItemsPerPageFirstPage : ItemsPerPageOtherPages;
            buttonNext.Visible = (_currentPage * itemsPerPage) < _totalReservations;
        }
        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadData();
                UpdatePaginationButtons();
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            int itemsPerPage = _currentPage == 1 ? ItemsPerPageFirstPage : ItemsPerPageOtherPages;
            if ((_currentPage * itemsPerPage) < _totalReservations)
            {
                _currentPage++;
                LoadData();
                UpdatePaginationButtons();
            }
        }

        private void Reserveform_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
                CenterLabel();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Modifyreservation addreservation = new Modifyreservation(this);
            AdduserControl(addreservation);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            LoadData();
            panel1.Controls.Add(panel2);
            panel1.Controls.Add(tableLayoutPanel1);
            UpdateControlVisibility();
        }

        public void triggerbutton1click()
        {
            button1.PerformClick();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TableStructure addreservation = new TableStructure(this);
            AdduserControl(addreservation);
        }

        public void LocalizeControls()
        {
            label1.Text = LanguageManager.Instance.GetString("RF-lbl1");
            button5.Text = LanguageManager.Instance.GetString("Btn-Add");
            button3.Text = LanguageManager.Instance.GetString("RF-btn3");
            button1.Text = LanguageManager.Instance.GetString("RF-btn1");
            button2.Text = LanguageManager.Instance.GetString("Btn-close");
            rjButton1.Text = LanguageManager.Instance.GetString("TR-rbtn1");
        }

        public void ApplyTheme()
        {
            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            label1.ForeColor = this.ForeColor;

            // Apply button colors
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                if (control is RJButton button)
                {
                    button.BackColor = colors.Color3;
                    button.ForeColor = colors.Color2;
                    button.FlatAppearance.MouseOverBackColor = colors.Color4;
                }

                if (control is RoundedPanel panel)
                {
                    panel.BackColor = colors.Color3;
                    panel.ForeColor = colors.Color2;

                    foreach (Control controls in panel.Controls)
                    {
                        if (controls is DataGridView dgv)
                        {
                            dgv.BackgroundColor = colors.Color3;
                            dgv.DefaultCellStyle.BackColor = colors.Color3;
                            dgv.DefaultCellStyle.ForeColor = colors.Color2;
                            dgv.DefaultCellStyle.SelectionBackColor = colors.Color3;
                            dgv.DefaultCellStyle.SelectionForeColor = colors.Color2;
                        }
                    }
                }
            }
        }
    }
}
