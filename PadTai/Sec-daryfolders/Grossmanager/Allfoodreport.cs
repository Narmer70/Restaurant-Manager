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
using PadTai.Classes.Controlsdesign;


namespace PadTai.Sec_daryfolders.Others
{
    public partial class Allfoodreport : UserControl
    {
        ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();
        private DataGridViewScroller scroller;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;
        private int clientId;

        public Allfoodreport()
        {
            InitializeComponent();
            InitializeControlResizer(); 
            dataGridView1.GridColor = colors.Color1;
            dataGridView2.GridColor = colors.Color1;            

            LocalizeControls();;
            ApplyTheme();
        }

        private void InitializeControlResizer()
        {
            crudDatabase = new CrudDatabase();

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);            
            
            resizer = new ControlResizer(this.Size);
          
            resizer.RegisterControl(label1);
            resizer.RegisterControl(label2);
            resizer.RegisterControl(label3);
            resizer.RegisterControl(label4);
            resizer.RegisterControl(label5);
            resizer.RegisterControl(label6);
            resizer.RegisterControl(label7);
            resizer.RegisterControl(label8);
            resizer.RegisterControl(label9);
            resizer.RegisterControl(label10);
            resizer.RegisterControl(label11);
            resizer.RegisterControl(label12);
            resizer.RegisterControl(label13);
            resizer.RegisterControl(label14);
            resizer.RegisterControl(label15);
            resizer.RegisterControl(label16);
            resizer.RegisterControl(label17);
            resizer.RegisterControl(label18);
            resizer.RegisterControl(label19);
            resizer.RegisterControl(label20);
            resizer.RegisterControl(label21);
            resizer.RegisterControl(label22);
            resizer.RegisterControl(pictureBox1);
            resizer.RegisterControl(pictureBox2);
            resizer.RegisterControl(dataGridView1);
            resizer.RegisterControl(dataGridView2);
            resizer.RegisterControl(roundedPanel1);
            resizer.RegisterControl(roundedPanel2);
            resizer.RegisterControl(roundedPanel3);
            resizer.RegisterControl(roundedPanel4);
            resizer.RegisterControl(roundedPanel5);
            resizer.RegisterControl(roundedPanel6);
            resizer.RegisterControl(roundedPanel7);
            scroller = new DataGridViewScroller(this, null, dataGridView1, dataGridView2);

            if (int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                LoadData();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (resizer != null)
            {
                resizer.ResizeControls(this);
                dgv1ColumnsWidth();             
            }

            if (dataGridView1.Rows.Count > 0)
            {
                string cellValue = dataGridView1.Rows[0].Cells[0].Value.ToString().Trim();
                if (!string.IsNullOrEmpty(cellValue))
                {
                    var grossProducts = GetGrossProductsByFoodID(cellValue);

                    if (grossProducts.Count > 0)
                    {
                        dataGridView2.DataSource = grossProducts.Select(g => new
                        {
                            g.ProductName,
                            ProductPriceFormatted = g.ProductPriceFormatted,
                            AmountInGramsWithSuffix = g.AmountInGramsWithSuffix,
                            TotalUsedWithSuffix = g.TotalUsedWithSuffix
                        }).ToList();
                        dgv2ColumnsWidth();
                    }
                    else
                    {
                        dataGridView2.DataSource = null;
                    }
                }
            }
        }

        public void LoadData()
        {
            DataTable receiptsTable = GetReceiptsByClientId(clientId);

            var foodSummaryService = new FoodSummaryService();
            IEnumerable<FoodPriceSummaryResult> summaryResults = foodSummaryService.GetFoodPriceSummary(receiptsTable);

            DataTable dt = ConvertToDataTable(summaryResults);
            dataGridView1.DataSource = dt;
           
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            dataGridView1.Columns["FoodItems"].HeaderText = "Name";
            dataGridView1.Columns["TotalAmount"].HeaderText = "Price";
            dataGridView1.Columns["CountOccurrences"].HeaderText = "Qte";
            dataGridView1.Columns["FoodID"].Visible = false;

            string currencySymbol = CurrencyService.Instance.GetCurrencySymbol();

            if (string.IsNullOrEmpty(currencySymbol))
            {
                currencySymbol = " ";
            }

            if (dt.Rows.Count > 0)
            {
                decimal overallTotalAmount = 0;
                int totalReceiptsCount = 0;

                foreach (DataRow row in dt.Rows)
                {
                    decimal price = Convert.ToDecimal(row["TotalAmount"].ToString().Replace(currencySymbol, "").Trim());
                    int quantity = Convert.ToInt32(row["CountOccurrences"]);

                    overallTotalAmount += price * quantity;
                    totalReceiptsCount += quantity;
                }
                dgv1ColumnsWidth();
                label2.Text = $"{overallTotalAmount} {currencySymbol}";
                label3.Text = $"{totalReceiptsCount}";
            }
            else
            {
                label2.Text = $"0.00 {currencySymbol}";
                label3.Text = "0";
            }
        }

        private DataTable ConvertToDataTable(IEnumerable<FoodPriceSummaryResult> summaryResults)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("FoodID", typeof(string)); 
            dt.Columns.Add("FoodItems", typeof(string));
            dt.Columns.Add("TotalAmount", typeof(string));
            dt.Columns.Add("CountOccurrences", typeof(int));
            string currencySymbol = CurrencyService.Instance.GetCurrencySymbol();

            if (string.IsNullOrEmpty(currencySymbol))
            {
                currencySymbol = " ";
            }

            foreach (var result in summaryResults)
            {
                string foodNameSpaced = "    " + result.FoodName.ToString();
                string formattedPrice = $"{result.FoodPrice} {currencySymbol}";
                dt.Rows.Add(result.FoodID, foodNameSpaced, formattedPrice, result.CountOccurrences);
            }
            return dt;
        }

        public class FoodSummaryService
        {
            public IEnumerable<FoodPriceSummaryResult> GetFoodPriceSummary(DataTable receiptsTable)
            {
                var combinedFoodData = from r in receiptsTable.AsEnumerable()
                                       let foodNames = r.Field<string>("FoodName")?.Split(',')
                                       let foodPrices = r.Field<string>("FoodPrice")?.Split(',')
                                       let foodIDs = r.Field<string>("FoodID")?.Split(',')
                                       let count = Math.Min(foodNames.Length, Math.Min(foodPrices.Length, foodIDs.Length)) 
                                       select Enumerable.Range(0, count).Select(i => new
                                       {
                                           FoodNameValue = foodNames[i].Trim(),
                                           FoodPriceValue = decimal.TryParse(foodPrices[i].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal priceValue) ? priceValue : 0,
                                           FoodIDValue = foodIDs[i].Trim()
                                       });

                var foodPriceSummary = combinedFoodData.SelectMany(x => x)
                    .GroupBy(f => new { f.FoodNameValue, f.FoodPriceValue, f.FoodIDValue })
                    .Select(g => new FoodPriceSummaryResult
                    {
                        FoodName = g.Key.FoodNameValue,
                        FoodPrice = g.Key.FoodPriceValue,
                        CountOccurrences = g.Count(),
                        FoodID = g.First().FoodIDValue 
                    })
                    .OrderByDescending(f => f.CountOccurrences)
                    .ToList();

                return foodPriceSummary;
            }
        }

        public DataTable GetReceiptsByClientId(int clientId)
        {
            string query = "SELECT * FROM Receipts WHERE ClientID = @ClientID";
            var parameters = new Dictionary<string, object>
            {
                { "@ClientID", clientId }
            };

            try
            {
                return crudDatabase.FetchDataFromDatabase(query, parameters);
            }
            catch 
            {
                return new DataTable(); // Return an empty DataTable in case of error
            }
        }


        private List<GrossProductSummary> GetGrossProductsByFoodID(string foodID)
        {
            var grossProductSummaries = new List<GrossProductSummary>();

            if (!int.TryParse(foodID, out int dishId))
            {
                return grossProductSummaries;
            }

            string query = @"SELECT gp.ProductName, gp.ProductPrice, im.AmountInGrams, im.AmountInGrams * @CountOccurrences AS TotalUsed
                     FROM IngredientMap im JOIN GrossProducts gp ON im.ProductID = gp.ProductID WHERE im.DishID = @DishID";

            var parameters = new Dictionary<string, object>
            {
                { "@DishID", dishId },
                { "@CountOccurrences", 0 } // Placeholder for countOccurrences
            };

            // Get the countOccurrences from the selected row if available
            if (dataGridView1.SelectedRows.Count > 0)
            {
                parameters["@CountOccurrences"] = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["CountOccurrences"].Value);
            }

            try
            {
                DataTable grossProductData = crudDatabase.FetchDataFromDatabase(query, parameters);

                foreach (DataRow row in grossProductData.Rows)
                {
                    var summary = new GrossProductSummary
                    {
                        ProductName = row["ProductName"].ToString(),
                        ProductPrice = Convert.ToDecimal(row["ProductPrice"]),
                        AmountInGrams = Convert.ToInt32(row["AmountInGrams"]),
                        TotalUsed = Convert.ToInt32(row["TotalUsed"])
                    };
                    grossProductSummaries.Add(summary);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while fetching gross products: " + ex.Message);
            }

            return grossProductSummaries;
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView1.SelectedRows[0];
                string foodID = selectedRow.Cells["FoodID"].Value.ToString();
                var grossProducts = GetGrossProductsByFoodID(foodID);

                if (grossProducts.Count > 0)
                {
                    // Create an anonymous type to bind to the DataGridView
                    dataGridView2.DataSource = grossProducts.Select(g => new
                    {
                        g.ProductName,
                        ProductPriceFormatted = g.ProductPriceFormatted,
                        AmountInGramsWithSuffix = g.AmountInGramsWithSuffix,
                        TotalUsedWithSuffix = g.TotalUsedWithSuffix
                    }).ToList();
                    dgv2ColumnsWidth();
                }
                else
                {
                    dataGridView2.DataSource = null; 
                }
            }
        }

        private void dgv1ColumnsWidth()
        {
            dataGridView1.Columns[1].Width = (int)(dataGridView1.Width * 0.65);
            dataGridView1.Columns[2].Width = (int)(dataGridView1.Width * 0.20);
            dataGridView1.Columns[3].Width = (int)(dataGridView1.Width * 0.15);
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[i].HeaderCell.Style.Font = new Font("Century Gothic", 10, FontStyle.Bold);
            }
            dataGridView1.ColumnHeadersHeight = 30;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = dataGridView1.ForeColor;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = dataGridView1.BackgroundColor;
            dataGridView1.ColumnHeadersDefaultCellStyle.SelectionForeColor = dataGridView1.ForeColor;
            dataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = dataGridView1.BackgroundColor;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
        }

        private void dgv2ColumnsWidth()
        {
            foreach (DataGridViewColumn column in dataGridView2.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            dataGridView2.Columns[0].HeaderText = "Name";
            dataGridView2.Columns[1].HeaderText = "Price/pc";
            dataGridView2.Columns[2].HeaderText = "Amount(g)";
            dataGridView2.Columns[3].HeaderText = "Total(g)";
            dataGridView2.Columns[0].Width = (int)(dataGridView2.Width * 0.50);
            dataGridView2.Columns[1].Width = (int)(dataGridView2.Width * 0.15);
            dataGridView2.Columns[2].Width = (int)(dataGridView2.Width * 0.15);
            dataGridView2.Columns[3].Width = (int)(dataGridView2.Width * 0.20);
            dataGridView2.Columns[0].DefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            for (int i = 0; i < dataGridView2.Columns.Count; i++)
            {
                if (i == 0)
                {
                    dataGridView2.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                }
                else
                {
                    dataGridView2.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                dataGridView2.Columns[i].HeaderCell.Style.Font = new Font("Century Gothic", 10, FontStyle.Bold);
            }
            dataGridView2.ColumnHeadersHeight = 30;
            dataGridView2.Columns[0].HeaderCell.Style.Padding = new Padding(10, 0, 0, 0);
            dataGridView2.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = dataGridView2.ForeColor;
            dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = dataGridView2.BackgroundColor;
            dataGridView2.ColumnHeadersDefaultCellStyle.SelectionForeColor = dataGridView2.ForeColor;
            dataGridView2.ColumnHeadersDefaultCellStyle.SelectionBackColor = dataGridView2.BackgroundColor;
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
        }

  
        public void LocalizeControls()
        {
            //button1.Text = LanguageManager.Instance.GetString("MF-btn1");
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
            //button4.Text = LanguageManager.Instance.GetString("MF-btn4");
            //button5.Text = LanguageManager.Instance.GetString("MF-btn5");
            //button6.Text = LanguageManager.Instance.GetString("MF-btn6");
        }

        public void ApplyTheme()
        {
            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            dataGridView1.ForeColor = colors.Color2;
            dataGridView2.ForeColor = colors.Color2;
            roundedPanel1.BackColor = colors.Color3;
            roundedPanel2.BackColor = colors.Color3;
            roundedPanel3.BackColor = colors.Color3;
            roundedPanel4.BackColor = colors.Color3;
            roundedPanel5.BackColor = colors.Color3;
            roundedPanel6.BackColor = colors.Color3;
            roundedPanel7.BackColor = colors.Color3;
            dataGridView1.BackgroundColor = colors.Color3;
            dataGridView2.BackgroundColor = colors.Color3; 
            dataGridView1.DefaultCellStyle.ForeColor = colors.Color2;
            dataGridView2.DefaultCellStyle.ForeColor = colors.Color2;
            dataGridView1.DefaultCellStyle.BackColor = colors.Color3;
            dataGridView2.DefaultCellStyle.BackColor = colors.Color3;
            dataGridView1.DefaultCellStyle.SelectionForeColor = colors.Color2;
            dataGridView2.DefaultCellStyle.SelectionForeColor = colors.Color2;
            dataGridView2.DefaultCellStyle.SelectionBackColor = colors.Color3;
            if (colors.Bool1 && Properties.Settings.Default.showScrollbars) showScrollBars();
        }

        private void showScrollBars()
        {
            dataGridView1.ScrollBars = ScrollBars.Vertical;
            dataGridView2.ScrollBars = ScrollBars.Vertical;
        }
    }

    public class FoodPriceSummaryResult
    {
        public string FoodName { get; set; }
        public decimal FoodPrice { get; set; }
        public int CountOccurrences { get; set; }
        public string FoodID { get; set; } 
    }

    public class GrossProductSummary
    {
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public int AmountInGrams { get; set; }
        public int TotalUsed { get; set; }
        public string TotalUsedWithSuffix => TotalUsed + " g";
        public string AmountInGramsWithSuffix => AmountInGrams + " g";
        public string ProductPriceFormatted => ProductPrice.ToString("F2") + $" {CurrencyService.Instance.GetCurrencySymbol()}";
    }
}
