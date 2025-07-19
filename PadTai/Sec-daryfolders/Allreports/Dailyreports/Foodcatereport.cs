using System;
using System.Linq;
using System.Data;
using System.Drawing;
using PadTai.Classes;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Globalization;
using System.Data.SqlClient;
using PadTai.Classes.Others;
using System.Reflection.Emit;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;


namespace PadTai.Sec_daryfolders
{
    public partial class Foodcatereport : UserControl
    {
        ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();
        private Dictionary<int, string> foodItemNames;
        private DataGridViewScroller scroller;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;       
        private ControlResizer resizer;
        private int clientId;


        public Foodcatereport()
        {
            InitializeComponent();
            InitializeControlResizer();
            crudDatabase = new CrudDatabase();
            dataGridView1.GridColor = colors.Color1;
            dataGridView5.GridColor = colors.Color1;          
      
            LoadFoodItems();

            if (int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                LoadFoodSalesReport();
            }
 
            LoadfirstRowData();
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

        private void LoadfirstRowData()
        {
            if (dataGridView1.Rows.Count > 0)
            {
                string cellValueString = dataGridView1.Rows[0].Cells[3].Value.ToString().Trim();
                if (int.TryParse(cellValueString, out int cellValue))
                {
                    LoadFoodData(cellValue);
                }
            }
        }

        private void LoadFoodItems()
        {
            foodItemNames = new Dictionary<int, string>();
            string query = "SELECT FooditemtypeID, FooditemtypeName FROM FoodItemsTypes";

            try
            {
                DataTable foodItemTypesData = crudDatabase.FetchDataFromDatabase(query);
                foreach (DataRow row in foodItemTypesData.Rows)
                {
                    // Use Convert.ToInt32 to safely convert the value to int
                    int id = Convert.ToInt32(row["FooditemtypeID"]);
                    string name = row.Field<string>("FooditemtypeName");
                    foodItemNames[id] = name; // Populate the dictionary
                }
            }
            catch 
            {
            }
        }

        public void LoadFoodSalesReport()
        {
            DataTable receiptsTable = GetReceiptsByClientId(clientId);
            var foodSummary = GetFoodItemTypeSalesReport(receiptsTable);

            decimal totalOverallRevenue = 0m;
            int totalOccurrences = 0;

            if (dataGridView1.Columns.Count == 0)
            {
                dataGridView1.Columns.Add("FoodItemType", "     " + LanguageManager.Instance.GetString("Dailyreport-foodtype"));
                dataGridView1.Columns.Add("TotalOccurrences", LanguageManager.Instance.GetString("Dailyreport-qte"));
                dataGridView1.Columns.Add("TotalRevenue", LanguageManager.Instance.GetString("Dailyreport-sum"));
                dataGridView1.Columns.Add("Fooditemtypeid", "ID");
            }

            foreach (var item in foodSummary)
            {
                string foodNamespaced = "     " + item.FoodTypeName;
                string priceValue = item.TotalAmount.ToString("N2") + $" {CurrencyService.Instance.GetCurrencySymbol()}";

                dataGridView1.Rows.Add(foodNamespaced, item.TotalCount, priceValue, item.FoodItemTypeID);

                totalOccurrences += item.TotalCount;
                totalOverallRevenue += item.TotalAmount;
            }
            dataGridView1.Columns[3].Visible = false;

            label1.Text = totalOccurrences.ToString();
            label2.Text = totalOverallRevenue.ToString("N2") + $" {CurrencyService.Instance.GetCurrencySymbol()}";
        }

        public DataTable GetReceiptsByClientId(int clientId)
        {
            string query = "SELECT * FROM Receipts WHERE ClientID = @ClientID";
            var parameters = new Dictionary<string, object>
            {
                { "@ClientID", clientId }
            };

            return crudDatabase.FetchDataFromDatabase(query, parameters);
        }

        private IEnumerable<FoodSummaryResult> GetFoodItemTypeSalesReport(DataTable receiptsTable)
        {
            var combinedFoodData = from r in receiptsTable.AsEnumerable()
                                   let foodItemTypes = r.Field<string>("FooditemtypeID")?.Split(',')
                                   let foodNames = r.Field<string>("FoodName")?.Split(',')
                                   let foodPrices = r.Field<string>("Foodprice")?.Split(',')
                                   select foodItemTypes.Zip(foodNames, (itemType, name) => new { itemType, name })
                                                       .Zip(foodPrices, (combined, price) => new
                                                       {
                                                           FoodItemTypeIDValue = combined.itemType.Trim(),
                                                           FoodNameValue = combined.name.Trim(),
                                                           FoodPriceValues = price.Trim().Split(',').Select(p =>
                                                           decimal.TryParse(p.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal priceValue) ? priceValue : 0)
                                                       });

            var foodSummary = combinedFoodData.SelectMany(x => x)
                .SelectMany(f => f.FoodPriceValues.Select(priceValue => new
                {
                    f.FoodItemTypeIDValue,
                    f.FoodNameValue,
                    FoodPriceValue = priceValue
                }))
                .GroupBy(f => f.FoodItemTypeIDValue)
                .Select(g =>
                {
                    return new FoodSummaryResult
                    {
                        FoodTypeName = foodItemNames.TryGetValue(int.Parse(g.Key), out string foodTypeName) ? foodTypeName : "NULL",
                        TotalCount = g.Count(),
                        TotalAmount = g.Sum(f => f.FoodPriceValue),
                        FoodItemTypeID = int.Parse(g.Key)
                    };
                })
                .OrderBy(f => f.FoodTypeName)
                .ToList();

            return foodSummary;
        }

        public void LoadFoodData(int foodItemTypeId)
        {
            int clientId;
            if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                return;
            }

            string query = "SELECT FooditemtypeID, Foodprice, FoodName FROM Receipts WHERE ClientID = @ClientID";
            var parameters = new Dictionary<string, object>
            {
               { "@ClientID", clientId }
            };

            DataTable dataTable = crudDatabase.FetchDataFromDatabase(query, parameters);
            var flattenedData = dataTable.AsEnumerable()
                .SelectMany(row =>
                    row.Field<string>("FooditemtypeID").Split(',')
                    .Select((id, index) => new
                    {
                        FoodItemTypeIDValue = id.Trim(),
                        FoodNameValue = row.Field<string>("FoodName").Split(',')[index].Trim(),
                        FoodPriceValue = decimal.TryParse(row.Field<string>("Foodprice").Split(',')[index].Trim(),
                        NumberStyles.Any, CultureInfo.InvariantCulture, out decimal priceValue) ? priceValue : 0
                    }))
                .ToList();

            var groupedData = flattenedData
                .Where(item => item.FoodItemTypeIDValue == foodItemTypeId.ToString())
                .GroupBy(item => item.FoodItemTypeIDValue)
                .Select(g => new
                {
                    FoodItemTypeIDValue = g.Key,
                    Items = g.GroupBy(item => new { item.FoodNameValue, item.FoodPriceValue })
                              .Select(ig => new
                              {
                                  ig.Key.FoodNameValue,
                                  ig.Key.FoodPriceValue,
                                  CountOccurrences = ig.Count()
                              })
                              .OrderByDescending(x => x.CountOccurrences)
                              .ToList()
                })
                .ToList();

            dataGridView5.Rows.Clear();
            dataGridView5.Columns.Clear();
            dataGridView5.Columns.Add("FoodName", "     " + LanguageManager.Instance.GetString("Dailyreport-dname"));
            dataGridView5.Columns.Add("FoodPrice", LanguageManager.Instance.GetString("Dailyreport-price"));
            dataGridView5.Columns.Add("CountOccurrences", LanguageManager.Instance.GetString("Dailyreport-qte"));
            dataGridView5.Columns.Add("TotalValue", LanguageManager.Instance.GetString("Dailyreport-total"));

            foreach (var group in groupedData)
            {
                foreach (var item in group.Items)
                {
                    string foodNameSpaced = "    " + item.FoodNameValue;
                    string priceCurrency = item.FoodPriceValue.ToString("N2") + $" {CurrencyService.Instance.GetCurrencySymbol()}";

                    decimal totalValue = item.FoodPriceValue * item.CountOccurrences;
                    string totalValueCurrency = totalValue.ToString("N2") + $" {CurrencyService.Instance.GetCurrencySymbol()}";

                    dataGridView5.Rows.Add(foodNameSpaced, priceCurrency, item.CountOccurrences, totalValueCurrency);
                    SetDataGridView5ColumnWidths();
                }
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

            dataGridView5.Columns[0].Width = (int)(dataGridView5.Width * 0.60);
            dataGridView5.Columns[1].Width = (int)(dataGridView5.Width * 0.10);
            dataGridView5.Columns[2].Width = (int)(dataGridView5.Width * 0.15);
            dataGridView5.Columns[3].Width = (int)(dataGridView5.Width * 0.15);
            dataGridView5.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView5.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView5.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView5.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView5.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView5.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
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
                int cellValue = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString().Trim());
                LoadFoodData(cellValue);
            }
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            int lastRowIndex = e.RowIndex + e.RowCount - 1;
            dataGridView1.Rows[lastRowIndex].Selected = true;
            dataGridView1.CurrentCell = dataGridView1.Rows[lastRowIndex].Cells[0];
            dataGridView1.FirstDisplayedScrollingRowIndex = lastRowIndex;
        }

        private void dataGridView5_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            int lastRowIndex = e.RowIndex + e.RowCount - 1;
            dataGridView5.Rows[lastRowIndex].Selected = true;
            dataGridView5.CurrentCell = dataGridView5.Rows[lastRowIndex].Cells[0];
            dataGridView5.FirstDisplayedScrollingRowIndex = lastRowIndex;
        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //// Check if the current cell is in the first column (index 0)
            //if (e.ColumnIndex == 0 && e.RowIndex >= 0) // First cell in the row
            //{
            //    e.CellStyle.ForeColor = Color.Red; // Set the foreground color to Red
            //}
            //// Check if the current cell is in the second column (index 1)
            //else if (e.ColumnIndex == 1 && e.RowIndex >= 0) // Second cell in the row
            //{
            //    e.CellStyle.ForeColor = Color.Blue; // Set the foreground color to Blue
            //}
        }

        private void dataGridView5_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //if (e.RowIndex >= 0) 
            //{
            //    if (e.RowIndex % 2 == 0)
            //    {
            //        e.CellStyle.BackColor = Color.FromArgb(2, 40, 71); 
            //    }
            //    else
            //    {
            //        e.CellStyle.BackColor = Color.FromArgb(2, 50, 90); 
            //    }
            //}
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //if (e.RowIndex >= 0)
            //{
            //    if (e.RowIndex % 2 == 0)
            //    {
            //        e.CellStyle.BackColor = Color.FromArgb(2, 40, 71);
            //    }
            //    else
            //    {
            //        e.CellStyle.BackColor = Color.FromArgb(2, 50, 90);
            //    }
            //}
        }


        public void LocalizeControls()
        {
            label3.Text = LanguageManager.Instance.GetString("Dailyreport-dishes");
            label4.Text = LanguageManager.Instance.GetString("Dailyreport-lbl4") + "*";
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

    public class FoodSummaryResult
    {
         public string FoodTypeName { get; set; }
         public decimal TotalAmount { get; set; }
         public int FoodItemTypeID { get; set; }            
         public int TotalCount { get; set; }
    }
}


