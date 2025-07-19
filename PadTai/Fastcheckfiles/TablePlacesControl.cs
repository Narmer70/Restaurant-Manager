using System;
using System.Data;
using System.Linq;
using System.Text;
using PadTai.Classes;
using System.Drawing;
using System.Data.SQLite;
using System.Windows.Forms;
using System.ComponentModel;
using PadTai.Classes.Others;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;


namespace PadTai.Fastcheckfiles
{
    public partial class TablePlacesControl : UserControl
    {
        private ButtonClickHandler _buttonClickHandler;
        private const int ItemsPerPage = 19;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;
        private int _pageNumber = 0;
        private int _totalItemCount;
        private Fastcheck FCH;

        public TablePlacesControl(Fastcheck fCH, int pageNumber = 0)
        {
            InitializeComponent();

            _buttonClickHandler = new ButtonClickHandler(this);
            crudDatabase = new CrudDatabase();
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);
           
            this.FCH = fCH;
            this._pageNumber = pageNumber; 
            LoadTableNumbers();
            LocalizeControls();
            ApplyTheme();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (resizer != null)
            {
                resizer.ResizeControls(this);
            }
        }

        private void LoadTableNumbers()
        {
            // Clear existing buttons and reset the row
            this.Controls.Clear();
            resizer = new ControlResizer(this.Size);
            int currentRow = 0;
            const int maxRowCount = 19;


            // Create a TableLayoutPanel for better button structure
            TableLayoutPanel tableLayoutPanel = new TableLayoutPanel
            {
                //BackColor = this.BackColor,
                Dock = DockStyle.Fill,
                AutoScroll = false,
                AutoSize = false,
                ColumnCount = 3,
                RowCount = 7 
            };

            float[] rowPercentages = new float[7] { 14F, 14F, 14F, 14F, 14F, 14F, 14F };

            foreach (float percentage in rowPercentages)
            {
                tableLayoutPanel.RowStyles.Add(new ColumnStyle(SizeType.Percent, percentage));
            }

            float[] columnPercentages = new float[3] { 33.3F, 33.3F, 33.3F };

            foreach (float percentage in columnPercentages)
            {
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, percentage));
            }
            resizer.RegisterControl(tableLayoutPanel);
            this.Controls.Add(tableLayoutPanel);


            // Add back button
            RJButton backButton = new RJButton
            {
                Text = "◀",
                Width = 145,
                Height = 75,
                Visible = true,
                //BorderRadius = 5,
                Dock = DockStyle.Fill,
                Margin = new Padding(2),
                BackColor = Color.DodgerBlue,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            resizer.RegisterControl(backButton);
            backButton.Click += BackButton_Click;
            tableLayoutPanel.Controls.Add(backButton, 0, 0);
            tableLayoutPanel.SetColumnSpan(backButton, 1);


            // Load table numbers
            List<TableNumber> tableNumbers = GetAllTableNumbers();
            _totalItemCount = tableNumbers.Count;
            tableNumbers = tableNumbers.Skip(_pageNumber * ItemsPerPage).Take(ItemsPerPage).ToList();

            foreach (var tableNumber in tableNumbers)
            {
                bool isChecked = GetIsCheckedStatus(tableNumber.TableID); 

                if (currentRow >= maxRowCount) break;

                RJButton tableNumberButton = new RJButton
                {
                    Name = tableNumber.Thetablenumber,
                    Text = "N° " + tableNumber.Thetablenumber,
                    Tag = tableNumber.TableID,
                    Width = 145,
                    Height = 75,
                    //BorderRadius = 5,
                    Enabled = !isChecked,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(2),
                    BackColor = Color.DodgerBlue,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Century Gothic", 10, FontStyle.Regular)
                };
                resizer.RegisterControl(tableNumberButton);
                tableNumberButton.Click += TableNumberButton_Click;
                tableLayoutPanel.Controls.Add(tableNumberButton);
                tableLayoutPanel.RowCount++;
                currentRow++;
            }

            //Add next Button
            if ((_pageNumber + 1) * ItemsPerPage < _totalItemCount)
            {
                RJButton nextPageButton = new RJButton
                {
                    Text = "▶",
                    Width = 145,
                    Height = 75,
                    //BorderRadius = 5,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(2),
                    BackColor = Color.DodgerBlue,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };
                resizer.RegisterControl(nextPageButton);
                nextPageButton.Click += NextPageButton_Click;
                tableLayoutPanel.Controls.Add(nextPageButton);

            }
        }
        private void NextPageButton_Click(object sender, EventArgs e)
        {
            if ((_pageNumber + 1) * ItemsPerPage < _totalItemCount)
            {
                _pageNumber++;
                LoadTablePlacesControl(_pageNumber); 
            }
        }


        private void BackButton_Click(object sender, EventArgs e)
        {
            if (_pageNumber > 0)
            {
                _pageNumber--;
                LoadTablePlacesControl(_pageNumber); 
            }
            else
            {
                DishGroupControl MCL = new DishGroupControl(FCH, null);
                FCH.AddUserControl(MCL);
            }
        }

        private void TableNumberButton_Click(object sender, EventArgs e)
        {
            if (sender is RJButton button && button.Tag is int tableId)
            {
                // Handle table number button click here
                _buttonClickHandler.AddTablenumber(sender);
            }
        }

        private List<TableNumber> GetAllTableNumbers()
        {
            string query = "SELECT TableID, Thetablenumber FROM Tablenumber";

            return crudDatabase.FetchDataToList(query, reader => new TableNumber
            {
                TableID = reader.GetInt32(0),
                Thetablenumber = reader.GetString(1)
            });
        }

        private bool GetIsCheckedStatus(int TableID)
        {
            string query = $"SELECT IsAvailable FROM Tablenumber WHERE TableID = {TableID}";

            // Fetch data using the existing method
            DataTable resultTable = crudDatabase.FetchDataFromDatabase(query);

            // Check if any rows were returned and return the IsChecked status
            if (resultTable != null && resultTable.Rows.Count > 0)
            {
                return Convert.ToBoolean(resultTable.Rows[0]["IsAvailable"]);
            }

            return false; // Default return value if no data is found
        }

        public void LoadTablePlacesControl(int pageNumber)
        {
            TablePlacesControl tablePlacesControl = new TablePlacesControl(FCH, pageNumber); 
            FCH.Panelreceptacle.Controls.Clear(); 
            tablePlacesControl.Dock = DockStyle.Fill; 
            FCH.Panelreceptacle.Controls.Add(tablePlacesControl); 
        }

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
        }
    }


    // Model class for TableNumber
    public class TableNumber
    {
        public int TableID { get; set; }
        public string Thetablenumber { get; set; }
    }
}

