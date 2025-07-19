using PadTai.Classes;
using PadTai.Sec_daryfolders.Reports;
using System;
using System.Data.SqlClient;
using System.Reflection.Emit;
using System.Windows.Forms;
using Label = System.Windows.Forms.Label;

namespace PadTai.Sec_daryfolders.Others
{
    public partial class Deltypecateport : UserControl
    {
        private Resizeuser resizer;
        private Reportviewer reportviewer;
        string connectionString = DatabaseConnection.GetConnection().ConnectionString;
        private FontResizer fontResizer;

        public Deltypecateport(Reportviewer report)
        {
            InitializeComponent();
            this.reportviewer = report;
            InitializeControlResizer();

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);
        }
        private void InitializeControlResizer()
        {
            resizer = new Resizeuser(this.Size);

            resizer.RegisterControl(label26);
            resizer.RegisterControl(label29);
            resizer.RegisterControl(label3);
            resizer.RegisterControl(label25);
            resizer.RegisterControl(label28);
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

        public void LoadDeliverySummary()
        {
            int clientId;

            // Validate Client ID from settings
            if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                throw new Exception("Invalid Client ID. Please ensure a valid Client ID is selected.");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // SQL query to get delivery summary, sorted by ClientID
                string query = @"
                SELECT 
                    LTRIM(RTRIM(PlacetoEatName)) AS Placetoeattype,
                    COUNT(*) AS TotalOccurrences,
                    SUM(TotalPrice) AS TotalRevenue
                FROM 
                    Receipts
                WHERE 
                    ClientID = @ClientID
                GROUP BY 
                    LTRIM(RTRIM(PlacetoEatName))
                ORDER BY 
                    Placetoeattype;"; // You can change the order by clause as needed

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientID", clientId);
                    using (SqlDataReader reader = command.ExecuteReader())
                        {
                            int index = 0; // To keep track of the discount type rows
                            decimal totalOverallPrice = 0m; // Variable to store the total overall price
                            int totalOccurrences = 0; // Variable to store the total occurrences

                            while (reader.Read() && index < 100) // Limit to the first 5 discount types
                            {
                                // Assuming you have the following labels on your form
                                string discountTypeLabel = $"labelFoodType{index + 1}";
                                string totalOccurrencesLabel = $"labelReceiptCount{index + 1}";
                                string totalPriceLabel = $"labelTotalAmount{index + 1}";

                                // Use reflection to get the labels by name
                                Label discountType = this.panel1.Controls[discountTypeLabel] as Label;
                                Label occurrences = this.panel1.Controls[totalOccurrencesLabel] as Label;
                                Label price = this.panel1.Controls[totalPriceLabel] as Label;

                                // Assign values to the labels
                                if (discountType != null && occurrences != null && price != null)
                                {
                                    discountType.Text = reader["Placetoeattype"].ToString();
                                    int occurrencesValue = Convert.ToInt32(reader["TotalOccurrences"]);
                                    decimal priceValue = Convert.ToDecimal(reader["TotalRevenue"]);

                                    occurrences.Text = occurrencesValue.ToString();
                                    price.Text = priceValue.ToString("C"); // Format as currency

                                    // Accumulate totals
                                    totalOverallPrice += priceValue;
                                    totalOccurrences += occurrencesValue;

                                    index++; // Move to the next set of labels
                                }
                            }

                            // After processing the first 5 records, you can display overall totals
                            label26.Text = totalOverallPrice.ToString("C"); // Display total overall price
                            label25.Text = totalOccurrences.ToString(); // Display total occurrences
                        }
                    }
                    
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Deliveryreport dr = new Deliveryreport();
            reportviewer.AddUserControl(dr);
        }
    }
}
