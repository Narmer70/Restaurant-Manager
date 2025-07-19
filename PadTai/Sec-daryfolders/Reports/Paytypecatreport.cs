using PadTai.Classes;
using PadTai.Sec_daryfolders.Reports;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PadTai.Sec_daryfolders.Quitfolder
{
    public partial class Paytypecatreport : UserControl
    {            
        private Resizeuser resizer;
        private Reportviewer reportviewer;
        string connectionString = DatabaseConnection.GetConnection().ConnectionString;
        private FontResizer fontResizer;
        public Paytypecatreport(Reportviewer report)
        {
            InitializeComponent();      
            fontResizer = new FontResizer();      
            fontResizer.AdjustFont(this);

            this.reportviewer = report;

            InitializeControlResizer();
            // draggableForm = new DraggableForm();
            //draggableForm.EnableDragging(this);
          
        }

        private void InitializeControlResizer()
        {

        resizer = new Resizeuser(this.Size);
            resizer.RegisterControl(label21);
            resizer.RegisterControl(label23);
            resizer.RegisterControl(label25);
            resizer.RegisterControl(label26);
            resizer.RegisterControl(label3);
            resizer.RegisterControl(label4);
            resizer.RegisterControl(label5);
            resizer.RegisterControl(labelFoodType1);
            resizer.RegisterControl(labelFoodType2);
            resizer.RegisterControl(labelFoodType3);
            resizer.RegisterControl(labelFoodType4);
            resizer.RegisterControl(labelFoodType5);
            resizer.RegisterControl(labelReceiptCount1);
            resizer.RegisterControl(labelReceiptCount2);
            resizer.RegisterControl(labelReceiptCount3);
            resizer.RegisterControl(labelReceiptCount4);
            resizer.RegisterControl(labelReceiptCount5);
            resizer.RegisterControl(labelTotalAmount1);
            resizer.RegisterControl(labelTotalAmount2);
            resizer.RegisterControl(labelTotalAmount3);
            resizer.RegisterControl(labelTotalAmount4);
            resizer.RegisterControl(labelTotalAmount5);
            resizer.RegisterControl(linkLabel1);
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

        public void LoadPaymentTypeSalesReport()
        {
            int clientId;
            if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                throw new Exception("Invalid Client ID. Please ensure a valid Client ID is selected.");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // SQL query to group by PaymenttypeName and calculate total occurrences and sum of TotalPrice
                string query = @"SELECT 
                    PaymenttypeName, 
                    COUNT(*) AS Occurrences, 
                    SUM(TotalPrice) AS TotalPriceSum
                    FROM 
                    Receipts 
                    WHERE 
                    ClientID = @ClientID 
                    GROUP BY 
                    PaymenttypeName 
                    ORDER BY 
                    PaymenttypeName"; // You can change the order by clause as needed

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClientID", clientId);
                    using (SqlDataReader reader = command.ExecuteReader())
                        {
                            int index = 0; // To keep track of the payment type rows
                            decimal totalOverallAmount = 0m; // Variable to store the total overall amount
                            int totalReceiptCount = 0; // Variable to store the total receipt count

                            while (reader.Read() && index < 5) // Limit to the first 5 payment types
                            {
                                // Assuming you have the following labels on your form
                                string paymentTypeLabel = $"labelFoodType{index + 1}";
                                string totalAmountLabel = $"labelTotalAmount{index + 1}";
                                string receiptCountLabel = $"labelReceiptCount{index + 1}";

                                // Use reflection to get the labels by name
                                Label paymentType = this.panel3.Controls[paymentTypeLabel] as Label;
                                Label totalAmount = this.panel3.Controls[totalAmountLabel] as Label;
                                Label receiptCount = this.panel3.Controls[receiptCountLabel] as Label;

                                // Assign values to the labels
                                if (paymentType != null && totalAmount != null && receiptCount != null)
                                {
                                    paymentType.Text = reader["PaymenttypeName"].ToString();
                                    decimal amount = Convert.ToDecimal(reader["TotalPriceSum"]);
                                    int receiptCountValue = Convert.ToInt32(reader["Occurrences"]);

                                    totalAmount.Text = amount.ToString("C"); // Format as currency
                                    receiptCount.Text = receiptCountValue.ToString();

                                    // Accumulate totals
                                    totalOverallAmount += amount;
                                    totalReceiptCount += receiptCountValue;

                                    index++; // Move to the next set of labels
                                }
                            }

                            // After processing the first 5 records, read the overall totals
                            if (reader.Read()) // Assuming there is a record for totals
                            {
                                label26.Text = reader["TotalOverallAmount"].ToString(); // Overall total amount
                                label25.Text = reader["TotalReceiptCount"].ToString(); // Total receipt count
                                                                                       // If you have a TotalReceiptId, uncomment the line below
                                                                                       // label6.Text = reader["TotalReceiptId"].ToString(); 
                            }

                            // Set the accumulated totals in the appropriate labels
                            label26.Text = totalOverallAmount.ToString("C"); // Display total overall amount
                            label25.Text = totalReceiptCount.ToString(); // Display total receipt count
                        
                    }
                    
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Paytypereport dr = new Paytypereport();
            reportviewer.AddUserControl(dr);
        }
    }
}
