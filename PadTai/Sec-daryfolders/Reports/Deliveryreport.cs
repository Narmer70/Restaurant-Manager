using PadTai.Classes;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;
using static PadTai.Sec_daryfolders.Clientform;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace PadTai.Sec_daryfolders.Reports
{
    public partial class Deliveryreport : UserControl
    {
        private FontResizer fontResizer;
        string connectionString = DatabaseConnection.GetConnection().ConnectionString;
        private Resizeuser resizer;
        public Deliveryreport()
        {
            InitializeComponent();
            LoadDeliveryTypes();
            comboBox1.SelectedIndexChanged += comboBox1_IndexChanged;
            InitializeControlResizer();

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);
        }
        private void InitializeControlResizer()
        {
            resizer = new Resizeuser(this.Size);
            resizer.RegisterControl(comboBox1);
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

        private void LoadDeliveryTypes()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                int clientId;
                if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
                {
                    throw new Exception("Invalid Client ID. Please ensure a valid Client ID is selected.");
                }

                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT PlacetoEatName FROM Receipts WHERE ClientID = @ClientID", connection);
                cmd.Parameters.AddWithValue("@ClientID", clientId);

                SqlDataReader reader = cmd.ExecuteReader();
                 
                while (reader.Read())
                {
                    comboBox1.Items.Add(reader["PlacetoEatName"].ToString());
                }
            }
        }

        private void comboBox1_IndexChanged(object sender, EventArgs e)
        {
            string selectedPaymentType = comboBox1.SelectedItem.ToString();

            if (comboBox1.SelectedItem != null)
            {
                LoadReceipts(selectedPaymentType);
            }
        }

        private void LoadReceipts(string paymentType)
        {
            DataTable receiptTable = new DataTable();

            int clientId;
            if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                throw new Exception("Invalid Client ID. Please ensure a valid Client ID is selected.");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT ReceiptId, FoodName, TotalPrice FROM Receipts " +
                    "WHERE PlacetoEatName = @PlacetoEatName AND ClientID = @ClientID", connection);

                cmd.Parameters.AddWithValue("@PlacetoEatName", paymentType);
                cmd.Parameters.AddWithValue("@ClientID", clientId);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(receiptTable); // Fill the DataTable with data from the database

                // Add a new column for the formatted food names
                receiptTable.Columns.Add("FormattedFoodName", typeof(string));

                // Iterate through the rows to format the FoodName
                foreach (DataRow row in receiptTable.Rows)
                {
                    string foodName = row["FoodName"].ToString();
                    // Split the food names, trim whitespace, and concatenate
                    string[] foodNames = foodName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < foodNames.Length; i++)
                    {
                        foodNames[i] = foodNames[i].Trim(); // Trim whitespace
                    }
                    string formattedFoodName = string.Join(", ", foodNames);
                    row["FormattedFoodName"] = formattedFoodName; // Set the formatted food name
                }
            }

            // Set the DataSource for the DataGridView
            dataGridView1.DataSource = receiptTable;

            // Hide the original FoodName column
            dataGridView1.Columns["FoodName"].Visible = false;

            // Set header text for the columns
            dataGridView1.Columns["FormattedFoodName"].HeaderText = "Name";
            dataGridView1.Columns["TotalPrice"].HeaderText = "Price";
            dataGridView1.Columns["ReceiptId"].HeaderText = "Recid";

            // Set specific column widths
            dataGridView1.Columns["ReceiptId"].Width = 90;
            dataGridView1.Columns["FormattedFoodName"].Width = 510; // Changed from FoodName to FormattedFoodName
            dataGridView1.Columns["TotalPrice"].Width = 95;

            // Set the display order of the columns
            dataGridView1.Columns["ReceiptId"].DisplayIndex = 0; // First column
            dataGridView1.Columns["FormattedFoodName"].DisplayIndex = 1; // Second column
            dataGridView1.Columns["TotalPrice"].DisplayIndex = 2; // Third
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }
}

