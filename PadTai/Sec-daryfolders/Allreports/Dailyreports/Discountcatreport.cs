using System;
using System.Data;
using System.Linq;
using System.Drawing;
using PadTai.Classes;
using System.Data.SQLite;
using System.Globalization;
using System.Windows.Forms;
using System.Data.SqlClient;
using PadTai.Classes.Others;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;


namespace PadTai.Sec_daryfolders.Others
{
    public partial class Discountcatreport : UserControl
    {
        ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();
        private DataGridViewScroller scroller;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public Discountcatreport()
        {
            InitializeComponent();

            crudDatabase = new CrudDatabase();
            dataGridView1.GridColor = colors.Color1;
            dataGridView5.GridColor = colors.Color1;
       
            InitializeControlResizer();   
            LoadDiscountSummary();
            LoadfirstLineData();
            LocalizeControls();
            ApplyTheme();
        }

        private void InitializeControlResizer()
        {
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(label1);
            resizer.RegisterControl(label2);
            resizer.RegisterControl(label3);
            resizer.RegisterControl(label4);
            resizer.RegisterControl(label5);
            resizer.RegisterControl(label6);
            resizer.RegisterControl(pictureBox1);
            resizer.RegisterControl(pictureBox2);
            resizer.RegisterControl(pictureBox3);
            resizer.RegisterControl(pictureBox4);
            resizer.RegisterControl(roundedPanel1);
            resizer.RegisterControl(roundedPanel2);
            resizer.RegisterControl(roundedPanel3);
            resizer.RegisterControl(roundedPanel4);
            resizer.RegisterControl(roundedPanel5);
            resizer.RegisterControl(roundedPanel6);
            resizer.RegisterControl(roundedPanel7);
            resizer.RegisterControl(dataGridView1);
            resizer.RegisterControl(dataGridView5);
            resizer.RegisterControl(circularProgressbar1);
            resizer.RegisterControl(circularProgressbar2);
            resizer.RegisterControl(circularProgressbar3);
            scroller = new DataGridViewScroller(this, null, dataGridView1, dataGridView5);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                try
                {
                    SetDataGridView1ColumnWidths();
                    SetDataGridView5ColumnWidths();
                }
                catch 
                {
                }
            }
        }



        private void LoadfirstLineData()
        {
            if (dataGridView1.Rows.Count > 0)
            {
                string cellValue = dataGridView1.Rows[0].Cells[0].Value.ToString().Trim();
                if (!string.IsNullOrEmpty(cellValue))
                {
                    string valueWithoutPercent = cellValue.Substring(0, cellValue.Length - 1).Trim();
                    LoadData(valueWithoutPercent);
                }
            }
        }

        public void LoadDiscountSummary()
        {
            int clientId;

            if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                return;
            }

            string query = @" SELECT LTRIM(RTRIM(Thediscount)) AS DiscountType, COUNT(*) AS TotalOccurrences, SUM(TotalPrice) AS TotalRevenue
                               FROM Receipts WHERE ClientID = @ClientID GROUP BY LTRIM(RTRIM(Thediscount)) ORDER BY DiscountType";

            var parameters = new Dictionary<string, object>
            {
               { "@ClientID", clientId }
            };

            try
            {
                DataTable discountSummaryData = crudDatabase.FetchDataFromDatabase(query, parameters);
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                if (dataGridView1.Columns.Count == 0)
                {
                    dataGridView1.Columns.Add("DiscountType", LanguageManager.Instance.GetString("Dailyreport-discount"));
                    dataGridView1.Columns.Add("TotalOccurrences", LanguageManager.Instance.GetString("Dailyreport-qte"));
                    dataGridView1.Columns.Add("TotalRevenue", LanguageManager.Instance.GetString("Dailyreport-sum"));
                }

                decimal totalOverallRevenue = 0m;
                int totalOccurrences = 0;

                foreach (DataRow row in discountSummaryData.Rows)
                {
                    string discountTypeValue = row["DiscountType"].ToString();
                    int occurrencesValue = Convert.ToInt32(row["TotalOccurrences"]);
                    decimal priceValue = Convert.ToDecimal(row["TotalRevenue"]);

                    string discountTypespaced = "     " + discountTypeValue + "%";
                    string priceValuespaced = priceValue.ToString("N2") + $" {CurrencyService.Instance.GetCurrencySymbol()}";
                    dataGridView1.Rows.Add(discountTypespaced, occurrencesValue, priceValuespaced);

                    totalOccurrences += occurrencesValue;
                    totalOverallRevenue += priceValue;
                }

                label1.Text = totalOccurrences.ToString();
                label2.Text = totalOverallRevenue.ToString("N2") + $" {CurrencyService.Instance.GetCurrencySymbol()}";
            }
            catch 
            {
            }
        }

        public void LoadData(string selectedDiscount)
        {
            DataTable receiptTable = new DataTable();
            receiptTable.Columns.Add("ReceiptId", typeof(int));
            receiptTable.Columns.Add("FoodName", typeof(string));
            receiptTable.Columns.Add("TotalFoodPrice", typeof(decimal));
            receiptTable.Columns.Add("TotalPrice", typeof(decimal));
            receiptTable.Columns.Add("DiscountedTotalPrice", typeof(decimal));
            receiptTable.Columns.Add("FormattedFoodName", typeof(string));

            int clientId;
            if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                return;
            }

            string query = @"SELECT ReceiptId, FoodName, FoodPrice, TotalPrice FROM Receipts WHERE ClientID = @ClientID" +
                           (string.IsNullOrEmpty(selectedDiscount) ? "" : " AND TheDiscount = @SelectedDiscount");

            var parameters = new Dictionary<string, object>
            {
                { "@ClientID", clientId }
            };

            if (!string.IsNullOrEmpty(selectedDiscount))
            {
                parameters.Add("@SelectedDiscount", selectedDiscount);
            }

            try
            {
                DataTable receiptData = crudDatabase.FetchDataFromDatabase(query, parameters);

                foreach (DataRow reader in receiptData.Rows)
                {
                    var foodPricesString = reader["FoodPrice"].ToString();
                    var foodPricesArray = foodPricesString
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(p => decimal.TryParse(p.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal priceValue) ? priceValue : 0)
                        .ToArray();

                    var totalFoodPrice = foodPricesArray.Sum();
                    var totalPrice = Convert.ToDecimal(reader["TotalPrice"]);
                    var discountedTotalPrice = totalPrice - totalFoodPrice;

                    DataRow row = receiptTable.NewRow();
                    row["ReceiptId"] = Convert.ToInt32(reader["ReceiptId"]);
                    row["FoodName"] = reader["FoodName"].ToString();
                    row["TotalFoodPrice"] = totalFoodPrice;
                    row["TotalPrice"] = totalPrice;
                    row["DiscountedTotalPrice"] = discountedTotalPrice;

                    string formattedFoodName = string.Join(", ", row["FoodName"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(name => name.Trim()));
                    receiptTable.Rows.Add(row);
                }

                dataGridView5.DataSource = receiptTable;
                dataGridView5.Columns["FoodName"].HeaderText = "      " + LanguageManager.Instance.GetString("Dailyreport-reccontent");
                dataGridView5.Columns["ReceiptId"].HeaderText = LanguageManager.Instance.GetString("Dailyreport-Recid");
                dataGridView5.Columns["TotalFoodPrice"].HeaderText = LanguageManager.Instance.GetString("Dailyreport-price");
                dataGridView5.Columns["TotalPrice"].HeaderText = LanguageManager.Instance.GetString("Dailyreport-discounted");
                dataGridView5.Columns["DiscountedTotalPrice"].HeaderText = LanguageManager.Instance.GetString("Addbtns-btn17");

                dataGridView5.Columns["ReceiptId"].DisplayIndex = 0;
                dataGridView5.Columns["TotalPrice"].DisplayIndex = 2;
                dataGridView5.Columns["TotalFoodPrice"].DisplayIndex = 3;
                dataGridView5.Columns["FoodName"].DisplayIndex = 1;
                dataGridView5.Columns["DiscountedTotalPrice"].DisplayIndex = 4;
                SetDataGridView5ColumnWidths();
            }
            catch
            {
            }
        }

        private void SetDataGridView1ColumnWidths()
        {
            if (dataGridView1.Columns.Count == 0) return;

            dataGridView1.Columns[0].Width = (int)(dataGridView1.Width * 0.55);
            dataGridView1.Columns[1].Width = (int)(dataGridView1.Width * 0.20);
            dataGridView1.Columns[2].Width = (int)(dataGridView1.Width * 0.25);
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Century Gothic", 10, FontStyle.Bold);

            dataGridView1.ColumnHeadersHeight = 35;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.DarkGray;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridView1.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.DarkGray;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = dataGridView1.BackgroundColor;
            dataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = dataGridView1.BackColor;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
        }

        private void SetDataGridView5ColumnWidths()
        {
            if (dataGridView5.Columns.Count == 0) return;

            float[] columnWidths5 = new float[5] { 60F, 360F, 100F, 100F,100F };
            float totalDesiredWidth5 = columnWidths5.Sum();

            dataGridView5.Columns[0].Width = (int)(dataGridView5.Width * 0.10);
            dataGridView5.Columns[1].Width = (int)(dataGridView5.Width * 0.45);
            dataGridView5.Columns[2].Width = (int)(dataGridView5.Width * 0.15);
            dataGridView5.Columns[3].Width = (int)(dataGridView5.Width * 0.15);
            dataGridView5.Columns[4].Width = (int)(dataGridView5.Width * 0.15);           
            dataGridView5.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView5.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView5.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView5.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView5.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView5.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView5.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView5.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView5.ColumnHeadersDefaultCellStyle.Font = new Font("Century Gothic", 10, FontStyle.Bold);
          
            dataGridView5.ColumnHeadersHeight = 35;
            dataGridView5.ColumnHeadersDefaultCellStyle.ForeColor = Color.DarkGray;
            dataGridView5.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridView5.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.DarkGray;
            dataGridView5.ColumnHeadersDefaultCellStyle.BackColor = dataGridView5.BackgroundColor;
            dataGridView5.ColumnHeadersDefaultCellStyle.SelectionBackColor = dataGridView5.BackColor;
            dataGridView5.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string cellValue = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Trim();

                if (!string.IsNullOrEmpty(cellValue))
                {
                    string valueWithoutPercent = cellValue.Substring(0, cellValue.Length - 1).Trim();
                    LoadData(valueWithoutPercent);
                }    
            }
        }

        private void dataGridView5_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            int lastRowIndex = e.RowIndex + e.RowCount - 1;
            dataGridView5.Rows[lastRowIndex].Selected = true;
            dataGridView5.CurrentCell = dataGridView5.Rows[lastRowIndex].Cells[0];
            dataGridView5.FirstDisplayedScrollingRowIndex = lastRowIndex;
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            int lastRowIndex = e.RowIndex + e.RowCount - 1;
            dataGridView1.Rows[lastRowIndex].Selected = true;
            dataGridView1.CurrentCell = dataGridView1.Rows[lastRowIndex].Cells[0];
            dataGridView1.FirstDisplayedScrollingRowIndex = lastRowIndex;
        }

        private void dataGridView5_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Check if the current column is one of the price columns
            if (dataGridView5.Columns[e.ColumnIndex].Name == "TotalFoodPrice" ||
                dataGridView5.Columns[e.ColumnIndex].Name == "TotalPrice" ||
                dataGridView5.Columns[e.ColumnIndex].Name == "DiscountedTotalPrice"){
            
                if (e.Value != null && e.Value is decimal)
                {
                    // Format the value as currency
                    decimal value = (decimal)e.Value;
                    e.Value = value.ToString("N2") + " " + CurrencyService.Instance.GetCurrencySymbol();
                    e.FormattingApplied = true; // Indicate that formatting has been applied
                }
            }
        }

        public void LocalizeControls()
        {
            label3.Text = LanguageManager.Instance.GetString("Dailyreport-lbl3");
            label4.Text = LanguageManager.Instance.GetString("Dailyreport-lbl4");
            //button1.Text = LanguageManager.Instance.GetString("MF-btn1");
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
            //button4.Text = LanguageManager.Instance.GetString("MF-btn4");
            //button5.Text = LanguageManager.Instance.GetString("MF-btn5");
        }

        public void ApplyTheme()
        {
            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            roundedPanel1.BackColor = colors.Color3;
            roundedPanel2.BackColor = colors.Color3;
            roundedPanel3.BackColor = colors.Color3;
            roundedPanel4.BackColor = colors.Color3;
            roundedPanel5.BackColor = colors.Color3;
            roundedPanel6.BackColor = colors.Color3;
            roundedPanel7.BackColor = colors.Color3;
            dataGridView1.BackgroundColor = colors.Color3;
            dataGridView5.BackgroundColor = colors.Color3;
            dataGridView1.DefaultCellStyle.BackColor = colors.Color3;
            dataGridView5.DefaultCellStyle.BackColor = colors.Color3;
            dataGridView5.DefaultCellStyle.SelectionBackColor = colors.Color3;

            label1.ForeColor = this.ForeColor;
            label2.ForeColor = this.ForeColor;
            label3.ForeColor = this.ForeColor;
            label4.ForeColor = this.ForeColor;
            roundedPanel1.ForeColor = this.ForeColor;
            roundedPanel2.ForeColor = this.ForeColor;
            roundedPanel3.ForeColor = this.ForeColor;
            roundedPanel4.ForeColor = this.ForeColor;
            roundedPanel5.ForeColor = this.ForeColor;
            roundedPanel6.ForeColor = this.ForeColor;
            roundedPanel7.ForeColor = this.ForeColor;
            circularProgressbar1.ForeColor = this.ForeColor;
            circularProgressbar2.ForeColor = this.ForeColor;
            circularProgressbar3.ForeColor = this.ForeColor;
            circularProgressbar1.BackPenColor = this.BackColor;
            circularProgressbar2.BackPenColor = this.BackColor;
            circularProgressbar3.BackPenColor = this.BackColor;
            circularProgressbar1.MiddleCircleColor = this.ForeColor;
            circularProgressbar2.MiddleCircleColor = this.ForeColor;
            circularProgressbar3.MiddleCircleColor = this.ForeColor;
            dataGridView1.DefaultCellStyle.ForeColor = this.ForeColor;
            dataGridView5.DefaultCellStyle.ForeColor = this.ForeColor;
            dataGridView1.DefaultCellStyle.SelectionForeColor = this.ForeColor;
            dataGridView5.DefaultCellStyle.SelectionForeColor = this.ForeColor;
            if (colors.Bool1 && Properties.Settings.Default.showScrollbars) showScrollBars();
        }

        private void showScrollBars()
        {
            dataGridView1.ScrollBars = ScrollBars.Vertical;
            dataGridView5.ScrollBars = ScrollBars.Vertical;
        }
    }
}
