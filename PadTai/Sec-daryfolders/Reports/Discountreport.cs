using System;
using System.Linq;
using System.Data;
using PadTai.Classes;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Globalization;

namespace PadTai.Sec_daryfolders.Others
{
    public partial class Discountreport : UserControl
    {
        string connectionString = DatabaseConnection.GetConnection().ConnectionString;
        private Resizeuser resizer;
        private FontResizer fontResizer;
        
        
        public Discountreport()
        {
            InitializeComponent();
            InitializeControlResizer();
            LoadDiscounts();
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);
        }

        private void InitializeControlResizer()
        {
            resizer = new Resizeuser(this.Size);
           // resizer.RegisterControl(comboBox1);
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

        private void LoadDiscounts()
        {
            int clientId;
            if (int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT DISTINCT TheDiscount FROM Receipts WHERE ClientID = @ClientID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ClientID", clientId);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    var discounts = new List<string>();
                    while (reader.Read())
                    {
                        discounts.Add(reader["TheDiscount"].ToString());
                    }

                    comboBox1.DataSource = discounts; // Populate the ComboBox
                }
            }
            else
            {
                MessageBox.Show("Invalid ClientId.");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Load data based on selected discount
            LoadData(comboBox1.SelectedItem?.ToString());
        }

        private void LoadData(string selectedDiscount)
        {
            DataTable receiptTable = new DataTable();

            // Add columns to the DataTable
            receiptTable.Columns.Add("ReceiptId", typeof(int));
            receiptTable.Columns.Add("FoodName", typeof(string)); // Original FoodName column
            receiptTable.Columns.Add("TotalFoodPrice", typeof(decimal));
            receiptTable.Columns.Add("TotalPrice", typeof(decimal));
            receiptTable.Columns.Add("DiscountedTotalPrice", typeof(decimal));
            receiptTable.Columns.Add("FormattedFoodName", typeof(string)); // New FormattedFoodName column

            int clientId;
            if (int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Query to get data for the specified ClientID and optional discount filter
                    string query = @"
                SELECT ReceiptId, FoodName, FoodPrice, TotalPrice 
                FROM Receipts 
                WHERE ClientID = @ClientID" +
                        (string.IsNullOrEmpty(selectedDiscount) ? "" : " AND TheDiscount = @SelectedDiscount");

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ClientID", clientId);
                    if (!string.IsNullOrEmpty(selectedDiscount))
                    {
                        command.Parameters.AddWithValue("@SelectedDiscount", selectedDiscount);
                    }

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    // Read data from the SqlDataReader and populate the DataTable
                    while (reader.Read())
                    {
                        // Parse the FoodPrice and calculate the total food price
                        var foodPricesString = reader.GetString(reader.GetOrdinal("FoodPrice"));
                        var foodPricesArray = foodPricesString
                            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries) // Split by comma
                            .Select(p => decimal.TryParse(p.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal priceValue) ? priceValue : 0)
                            .ToArray();

                        var totalFoodPrice = foodPricesArray.Sum();
                        var totalPrice = reader.GetDecimal(reader.GetOrdinal("TotalPrice"));
                        var discountedTotalPrice = totalPrice - totalFoodPrice;

                        // Create a new DataRow and populate it
                        DataRow row = receiptTable.NewRow();
                        row["ReceiptId"] = reader.GetInt32(reader.GetOrdinal("ReceiptId"));
                        row["FoodName"] = reader.GetString(reader.GetOrdinal("FoodName")); // Original FoodName
                        row["TotalFoodPrice"] = totalFoodPrice;
                        row["TotalPrice"] = totalPrice;
                        row["DiscountedTotalPrice"] = discountedTotalPrice;

                        // Format the FoodName for the new column
                        string formattedFoodName = string.Join(", ", row["FoodName"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(name => name.Trim()));
                        row["FormattedFoodName"] = formattedFoodName;

                        // Add the populated DataRow to the DataTable
                        receiptTable.Rows.Add(row);
                    }
                }

                // Set the DataSource for the DataGridView
                   dataGridView1.DataSource = receiptTable;

                // Hide the original FoodName column
                   dataGridView1.Columns["FoodName"].Visible = false;

                // Optionally set column titles and widths
                dataGridView1.Columns["ReceiptId"].HeaderText = "ID";
                    dataGridView1.Columns["ReceiptId"].Width = 95;

                    dataGridView1.Columns["FormattedFoodName"].HeaderText = "Имя";
                    dataGridView1.Columns["FormattedFoodName"].Width = 315;

                    dataGridView1.Columns["TotalFoodPrice"].HeaderText = "Total Food Price";
                    dataGridView1.Columns["TotalFoodPrice"].Width = 95;

                    dataGridView1.Columns["TotalPrice"].HeaderText = "Total Price";
                    dataGridView1.Columns["TotalPrice"].Width = 95;

                    dataGridView1.Columns["DiscountedTotalPrice"].HeaderText = "Discounted Total";
                    dataGridView1.Columns["DiscountedTotalPrice"].Width = 95;

                    dataGridView1.Columns["ReceiptId"].DisplayIndex = 0; 
                    dataGridView1.Columns["FormattedFoodName"].DisplayIndex = 1; 
                    dataGridView1.Columns["TotalPrice"].DisplayIndex = 2; 
                    dataGridView1.Columns["TotalFoodPrice"].DisplayIndex = 3;
                    dataGridView1.Columns["DiscountedTotalPrice"].DisplayIndex = 4;


                
            }
            else
            {
                MessageBox.Show("Invalid ClientId.");
            }
        }
        public class ReceiptDisplay
        {
            public int ReceiptId { get; set; }
            public string FoodName { get; set; }
            public decimal TotalFoodPrice { get; set; }
            public decimal TotalPrice { get; set; }
            public decimal DiscountedTotalPrice { get; set; } // This represents the discount
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }
}
