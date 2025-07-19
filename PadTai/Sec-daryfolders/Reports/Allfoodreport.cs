using PadTai.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using static PadTai.Sec_daryfolders.Clientform;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PadTai.Sec_daryfolders.Others
{
    public partial class Allfoodreport : UserControl
    {
        private FontResizer fontResizer;
        string connectionString = DatabaseConnection.GetConnection().ConnectionString;
        private Resizeuser resizer;
        private int clientId;

        public Allfoodreport()
        {
            InitializeComponent();
            InitializeControlResizer();

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            if (int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                LoadData();
            }
        }
        private void InitializeControlResizer()
        {
            resizer = new Resizeuser(this.Size);
          //  resizer.RegisterControl(comboBox1);
            resizer.RegisterControl(dataGridView1);
            resizer.RegisterControl(panel3);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // Call ResizeControls to resize registered controls
            if (resizer != null)
            {
                resizer.ResizeControls(this);
            }
        }

        public void LoadData()
        {
            DataTable receiptsTable = GetReceiptsByClientId(clientId);

            var foodSummaryService = new FoodSummaryService();
            IEnumerable<FoodPriceSummaryResult> summaryResults = foodSummaryService.GetFoodPriceSummary(receiptsTable);

            // Convert the results to a DataTable
            DataTable dt = ConvertToDataTable(summaryResults);

            // Bind data to DataGridView
            dataGridView1.DataSource = dt;

            // Set column headers
            dataGridView1.Columns["FoodItems"].HeaderText = "Name";
            dataGridView1.Columns["TotalAmount"].HeaderText = "Price";
            dataGridView1.Columns["CountOccurrences"].HeaderText = "Qte";

            // Set specific column widths
            dataGridView1.Columns["FoodItems"].Width = 500;
            dataGridView1.Columns["CountOccurrences"].Width = 85;
            dataGridView1.Columns["TotalAmount"].Width = 110;

            // Calculate overall totals
            if (dt.Rows.Count > 0)
            {
                decimal overallTotalAmount = 0;
                int totalReceiptsCount = dt.Rows.Count; // Count of unique food items

                foreach (DataRow row in dt.Rows)
                {
                    overallTotalAmount += Convert.ToDecimal(row["TotalAmount"]);
                }
            }
        }

        public class FoodSummaryService
        {
            public IEnumerable<FoodPriceSummaryResult> GetFoodPriceSummary(DataTable receiptsTable)
            {
                var combinedFoodData = from r in receiptsTable.AsEnumerable()
                                       let foodNames = r.Field<string>("FoodName")?.Split(',')
                                       let foodPrices = r.Field<string>("FoodPrice")?.Split(',')
                                       select foodNames.Zip(foodPrices, (name, price) => new
                                       {
                                           FoodNameValue = name.Trim(),
                                           FoodPriceValue = decimal.TryParse(price.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal priceValue) ? priceValue : 0
                                       });

                var foodPriceSummary = combinedFoodData.SelectMany(x => x)
                    .GroupBy(f => new { f.FoodNameValue, f.FoodPriceValue })
                    .Select(g => new FoodPriceSummaryResult
                    {
                        FoodName = g.Key.FoodNameValue,
                        FoodPrice = g.Key.FoodPriceValue,
                        CountOccurrences = g.Count()
                    })
                    .OrderBy(f => f.FoodName) // Sort by FoodName or any other criteria you prefer
                    .ToList();

                return foodPriceSummary;
            }
        }

        public DataTable GetReceiptsByClientId(int clientId)
        {
            DataTable receiptsTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Receipts WHERE ClientID = @ClientID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientID", clientId);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(receiptsTable);
                }
            }

            return receiptsTable;
        }

        private DataTable ConvertToDataTable(IEnumerable<FoodPriceSummaryResult> summaryResults)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("FoodItems", typeof(string));
            dt.Columns.Add("TotalAmount", typeof(decimal));
            dt.Columns.Add("CountOccurrences", typeof(int));

            foreach (var result in summaryResults)
            {
                dt.Rows.Add(result.FoodName, result.FoodPrice, result.CountOccurrences);
            }

            return dt;
        }
            public class FoodPriceSummaryResult
            {
                public string FoodName { get; set; }
                public decimal FoodPrice { get; set; }
                public int CountOccurrences { get; set; }
            }

    }
}
