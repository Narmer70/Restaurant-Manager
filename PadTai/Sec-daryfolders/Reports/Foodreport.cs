using PadTai.Classes;
using PadTai.Sec_daryfolders.Others;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Windows.Forms;

namespace PadTai.Sec_daryfolders.Reports
{
    public partial class Foodreport : UserControl
    {
        public Reportviewer reportviewer;
        private Resizeuser resizer;
        string connectionString = DatabaseConnection.GetConnection().ConnectionString;
        private FontResizer fontResizer;
        public Foodreport(Reportviewer report)
        {
            InitializeComponent();
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            this.reportviewer = report;
            InitializeControlResizer();
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
        }

        private void InitializeControlResizer()
        {
            resizer = new Resizeuser(this.Size);
            resizer.RegisterControl(comboBox1);
            resizer.RegisterControl(dataGridView1);
            resizer.RegisterControl(panel3);
            resizer.RegisterControl(linkLabel1);

            LoadFooditemsTypes();
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

        private void LoadFooditemsTypes()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                int clientId;
                if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
                {
                    throw new Exception("Invalid Client ID. Please ensure a valid Client ID is selected.");
                }

                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT FooditemtypeID FROM Receipts WHERE ClientID = @ClientID", connection);
                cmd.Parameters.AddWithValue("@ClientID", clientId);

                SqlDataReader reader = cmd.ExecuteReader();

                // Define a dictionary to map IDs to names
                Dictionary<int, string> foodItemNames = new Dictionary<int, string>
                 {
                 { 1, "WOK" },
                 { 2, "Поке" },
                 { 3, "Супы" },
                 { 4, "Комбо" },
                 { 5, "Десерты" },
                 { 6, "Бар" },
                 { 7, "Закуски" }
                 };

                HashSet<int> foodItemTypesSet = new HashSet<int>();

                while (reader.Read())
                {
                    // Split the FooditemtypeID by comma and add to the HashSet
                    string[] types = reader["FooditemtypeID"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var type in types)
                    {
                        if (int.TryParse(type.Trim(), out int id))
                        {
                            foodItemTypesSet.Add(id); // Add only valid integer IDs
                        }
                    }
                }

                // Convert HashSet to a sorted list
                var sortedFoodItemTypes = foodItemTypesSet.OrderBy(id => id).ToList();

                // Populate the ComboBox with user-friendly names and their corresponding IDs
                comboBox1.Items.Clear(); // Clear existing items if necessary
                foreach (var id in sortedFoodItemTypes)
                {
                    if (foodItemNames.TryGetValue(id, out string name))
                    {
                        comboBox1.Items.Add(new { ID = id, Name = name });
                    }
                }

                // Set the display and value members for the ComboBox
                comboBox1.DisplayMember = "Name"; // Use Name for display
                comboBox1.ValueMember = "ID";     // Use ID as value

                reader.Close();
                connection.Close(); ;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {   // Ensure the ComboBox has a selected item
            if (comboBox1.SelectedItem != null)
            {
                // Retrieve the selected item as a dynamic object
                dynamic selectedItem = comboBox1.SelectedItem;

                // Get the FooditemtypeID from the selected item
                int selectedFoodItemTypeID = selectedItem.ID;

                // Call LoadReceipts method with the selected FooditemtypeID
                LoadFoodData(selectedFoodItemTypeID);
            }
        }

        
        private void LoadFoodData(int foodItemTypeId)
        {
            int clientId;
            if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                throw new Exception("Invalid Client ID. Please ensure a valid Client ID is selected.");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // SQL query to select FooditemtypeID, Foodprice, FoodName where ClientID matches
                SqlCommand cmd = new SqlCommand("SELECT FooditemtypeID, Foodprice, FoodName FROM Receipts WHERE ClientID = @ClientID", connection);
                cmd.Parameters.AddWithValue("@ClientID", clientId); // Add parameter for ClientID

                SqlDataReader reader = cmd.ExecuteReader();

                // Create a DataTable to hold the results
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("FooditemtypeID", typeof(string));
                dataTable.Columns.Add("Foodprice", typeof(string));
                dataTable.Columns.Add("FoodName", typeof(string));

                while (reader.Read())
                {
                    // Read values from the reader
                    string foodItemTypeIDs = reader.GetString(reader.GetOrdinal("FooditemtypeID"));
                    string foodPrices = reader.GetString(reader.GetOrdinal("Foodprice"));
                    string foodNames = reader.GetString(reader.GetOrdinal("FoodName"));

                    // Add to DataTable
                    dataTable.Rows.Add(foodItemTypeIDs, foodPrices, foodNames);
                }

                // Flatten the data
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

                // Filter for the selected foodItemTypeId and group by FoodItemTypeID
                var groupedData = flattenedData
                    .Where(item => item.FoodItemTypeIDValue == foodItemTypeId.ToString())
                    .GroupBy(item => item.FoodItemTypeIDValue)
                    .SelectMany(g => g)
                // .Distinct() // Ensures we have unique entries
                    .ToList();

                // Clear existing rows and columns in DataGridView
                dataGridView1.Columns.Clear(); // Clear existing columns if necessary
                dataGridView1.Rows.Clear();     // Clear existing rows if necessary

                // Step 1: Define the columns
                dataGridView1.Columns.Add("FoodName", "Food Name"); 
                dataGridView1.Columns.Add("FoodPrice", "Food Price"); 

                // Step 2: Set column sizes (you can adjust these values as needed)
                dataGridView1.Columns["FoodName"].Width = 592; // Set width for Food Name column
                dataGridView1.Columns["FoodPrice"].Width = 100; // Set width for Food Price column
                // Populate the DataGridView with the grouped data
                foreach (var item in groupedData)
                {
                    dataGridView1.Rows.Add(item.FoodNameValue, item.FoodPriceValue);
                }
            }
        }

            private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
            {
                Allfoodreport FR = new Allfoodreport();
                reportviewer.AddUserControl(FR);
                FR.LoadData();
            }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }
}

