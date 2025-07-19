using System;
using System.Linq;
using System.Data;
using PadTai.Classes;
using PadTai.Sec_daryfolders.Reports;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection.Emit;
using System.Windows.Forms;
using Label = System.Windows.Forms.Label;
using System.Globalization;

namespace PadTai.Sec_daryfolders
{
    public partial class Foodcatereport : UserControl
    {
        string connectionString = DatabaseConnection.GetConnection().ConnectionString;
        private Resizeuser resizer;
        private FontResizer fontResizer;
        private Reportviewer reportviewer;
        private int clientId;

        public Foodcatereport(Reportviewer report)
        {
            InitializeComponent();
            this.reportviewer = report;
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            InitializeControlResizer();
            // draggableForm = new DraggableForm();
            //draggableForm.EnableDragging(this);
            if (int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                LoadFoodSalesReport();
            }
        }

        private void InitializeControlResizer()
        {
            resizer = new Resizeuser(this.Size);

            resizer.RegisterControl(labelTotalReceiptCount);
            resizer.RegisterControl(labelTotalOverallAmount);
            resizer.RegisterControl(label3);
            resizer.RegisterControl(label28);
            resizer.RegisterControl(label25);
            resizer.RegisterControl(labelFoodType1);
            resizer.RegisterControl(labelFoodType2);
            resizer.RegisterControl(labelFoodType3);
            resizer.RegisterControl(labelFoodType4);
            resizer.RegisterControl(labelFoodType5);
            resizer.RegisterControl(labelFoodType6);
            resizer.RegisterControl(labelFoodType7);
            resizer.RegisterControl(labelReceiptCount1);
            resizer.RegisterControl(labelReceiptCount2);
            resizer.RegisterControl(labelReceiptCount3);
            resizer.RegisterControl(labelReceiptCount4);
            resizer.RegisterControl(labelReceiptCount5);
            resizer.RegisterControl(labelReceiptCount6);
            resizer.RegisterControl(labelReceiptCount7);
            resizer.RegisterControl(labelTotalAmount1);
            resizer.RegisterControl(labelTotalAmount2);
            resizer.RegisterControl(labelTotalAmount3);
            resizer.RegisterControl(labelTotalAmount4);
            resizer.RegisterControl(labelTotalAmount5);
            resizer.RegisterControl(labelTotalAmount6);
            resizer.RegisterControl(labelTotalAmount7);
            resizer.RegisterControl(linkLabel1);
            resizer.RegisterControl(panel1);

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
        public void LoadFoodSalesReport()
        {
            DataTable receiptsTable = GetReceiptsByClientId(clientId);
            var foodSummary = GetFoodItemTypeSalesReport(receiptsTable);
            // Here, you can bind foodSummary to a UI element like a DataGridView or similar

            // Clear previous values
            for (int i = 1; i <= 10; i++)
            {
                Label foodTypeLabel = this.panel1.Controls[$"labelFoodType{i}"] as Label;
                Label totalAmountLabel = this.panel1.Controls[$"labelTotalAmount{i}"] as Label;
                Label receiptCountLabel = this.panel1.Controls[$"labelReceiptCount{i}"] as Label;

                if (foodTypeLabel != null) foodTypeLabel.Text = string.Empty;
                if (totalAmountLabel != null) totalAmountLabel.Text = string.Empty;
                if (receiptCountLabel != null) receiptCountLabel.Text = string.Empty;
            }

            decimal totalOverallAmount = 0m; // Variable to store the total overall amount
            int totalReceiptCount = 0; // Variable to store the total receipt count

            int index = 0; // To keep track of the food item type rows
            foreach (var item in foodSummary)
            {
                if (index >= 10) break; // Limit to the first 10 food item types

                // Assuming you have the following labels on your form
                Label foodTypeLabel = this.panel1.Controls[$"labelFoodType{index + 1}"] as Label;
                Label totalAmountLabel = this.panel1.Controls[$"labelTotalAmount{index + 1}"] as Label;
                Label receiptCountLabel = this.panel1.Controls[$"labelReceiptCount{index + 1}"] as Label;

                // Assign values to the labels
                if (foodTypeLabel != null && totalAmountLabel != null && receiptCountLabel != null)
                {
                    foodTypeLabel.Text = item.FoodTypeName;
                    decimal amount = item.TotalAmount;
                    int receiptCountValue = item.TotalCount;

                    totalAmountLabel.Text = amount.ToString("C"); // Format as currency
                    receiptCountLabel.Text = receiptCountValue.ToString();

                    // Accumulate totals
                    totalOverallAmount += amount;
                    totalReceiptCount += receiptCountValue;

                    index++; // Move to the next set of labels
                }
            }

            // After processing the records, set the overall totals
            labelTotalOverallAmount.Text = totalOverallAmount.ToString("C"); // Display total overall amount
            labelTotalReceiptCount.Text = totalReceiptCount.ToString(); // Display total receipt count
        }

        public DataTable GetReceiptsByClientId(int ClientId)
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
        .Select(g => new FoodSummaryResult
        {
            FoodTypeName = GetFoodTypeName(g.Key),
            TotalCount = g.Count(),
            TotalAmount = g.Sum(f => f.FoodPriceValue)
        })
        .OrderBy(f => f.FoodTypeName) // Sort by FoodTypeName or any other criteria you prefer
        .ToList();

            return foodSummary;

        }
        private string GetFoodTypeName(string foodItemTypeID)
        {
            string foodTypeName;
            switch (foodItemTypeID)
            {
                case "1":
                    foodTypeName = "WOK";
                    break;
                case "2":
                    foodTypeName = "Поке";
                    break;
                case "3":
                    foodTypeName = "Супы";
                    break;
                case "4":
                    foodTypeName = "Комбо";
                    break;
                case "5":
                    foodTypeName = "Десерты";
                    break;
                case "6":
                    foodTypeName = "Бар";
                    break;
                case "7":
                    foodTypeName = "Закуски";
                    break;
                default:
                    foodTypeName = "Неизвестный тип еды";
                    break;
            }
            return foodTypeName;
        }
    

       public class FoodSummaryResult
        {
           public string FoodTypeName { get; set; }
           public int TotalCount { get; set; }
           public decimal TotalAmount { get; set; }
       }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Foodreport dr = new Foodreport(reportviewer);
            reportviewer.AddUserControl(dr); ;
        }

    }
}


