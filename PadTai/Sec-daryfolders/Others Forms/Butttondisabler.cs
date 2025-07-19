using System;
using System.Linq;
using System.Data;
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
    public partial class Butttondisabler : Form
    {
        ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();
        private Dictionary<int, string> foodItemNames;
        private Clavieroverlay clavieroverlay;
        private DataGridViewScroller scroller;
        private CrudDatabase crudDatabase;
        private List<FoodItem> foodItems;
        private FontResizer fontResizer;
        private ControlResizer resizer;


        public Butttondisabler()
        {
            InitializeComponent();
            initialiseControlResize();
            dataGridView1.GridColor = colors.Color1;   

            LocalizeControls();
            ApplyTheme();
        }

        private void initialiseControlResize()
        {
            crudDatabase = new CrudDatabase();
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(button1);
            resizer.RegisterControl(button2);
            resizer.RegisterControl(textBox1);
            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(customBox1);
            resizer.RegisterControl(pictureBox1);
            resizer.RegisterControl(roundedPanel1);
            resizer.RegisterControl(roundedPanel2);
            resizer.RegisterControl(dataGridView1);
            scroller = new DataGridViewScroller(this, null, dataGridView1);

            LoadData();
            LoadFoodItemTypes();
        }

        private void LoadData()
        {
            string query = "SELECT FoodID, FoodName, FooditemtypeID, Price, IsChecked FROM FoodItems";

            try
            {
                DataTable dataTable = crudDatabase.FetchDataFromDatabase(query);
                foodItems = new List<FoodItem>();

                foreach (DataRow row in dataTable.Rows)
                {
                    foodItems.Add(new FoodItem
                    {
                        FoodID = Convert.ToInt32(row["FoodID"]),
                        FoodName = "     " + row["FoodName"].ToString(),
                        FooditemtypeID = Convert.ToInt32(row["FooditemtypeID"]),
                        Price = Convert.ToDecimal(row["Price"]).ToString("N2") + $" {CurrencyService.Instance.GetCurrencySymbol()}",
                        IsChecked = Convert.ToBoolean(row["IsChecked"])
                    });
                }

                dataGridView1.DataSource = foodItems;
                SetupDataGridViewColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading data: " + ex.Message);
            }
        }

        private void SetupDataGridViewColumns()
        {
            if (dataGridView1.Columns.Count > 0)
            {
                // Modify the FoodName column
                var foodNameColumn = dataGridView1.Columns["FoodName"];
                if (foodNameColumn != null)
                {
                    foodNameColumn.Width = (int)(dataGridView1.Width * 0.75); 
                    foodNameColumn.HeaderText = "FoodName";
                    foodNameColumn.DisplayIndex = 0;
                    foodNameColumn.ReadOnly = true;
                }

                // Modify the FoodName column
                var priceColumn = dataGridView1.Columns["Price"];
                if (priceColumn != null)
                {
                    priceColumn.ReadOnly = true;
                    priceColumn.DisplayIndex = 1;
                    priceColumn.HeaderText = "Price";
                    priceColumn.DefaultCellStyle.Format = "F2";
                    priceColumn.Width = (int)(dataGridView1.Width * 0.15); 
                    priceColumn.DefaultCellStyle.FormatProvider = CultureInfo.InvariantCulture;
                    priceColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // Modify the IsChecked column
                var isCheckedColumn = dataGridView1.Columns["IsChecked"];
                if (isCheckedColumn != null)
                {
                    isCheckedColumn.Width = (int)(dataGridView1.Width * 0.10); 
                    isCheckedColumn.HeaderText = "Is Checked";
                    isCheckedColumn.DisplayIndex = 2;
                }

                // Modify the FoodID column
                var foodIdColumn = dataGridView1.Columns["FoodID"];
                if (foodIdColumn != null)
                {
                    foodIdColumn.HeaderText = " ";
                    foodIdColumn.ReadOnly = true;
                    foodIdColumn.DefaultCellStyle.BackColor = dataGridView1.BackColor;
                    foodIdColumn.DefaultCellStyle.ForeColor = dataGridView1.BackColor;
                    foodIdColumn.HeaderCell.Style.ForeColor = dataGridView1.BackColor;
                    foodIdColumn.HeaderCell.Style.BackColor = dataGridView1.BackColor;
                    foodIdColumn.DefaultCellStyle.SelectionBackColor = dataGridView1.BackColor;
                    foodIdColumn.DefaultCellStyle.SelectionForeColor = dataGridView1.BackColor;                    
                    foodIdColumn.DisplayIndex = 3;
                    foodIdColumn.Visible = false;
                }

                // Modify the IsChecked column
                var fooditemtype = dataGridView1.Columns["FooditemtypeID"];
                if (fooditemtype != null)
                {
                    fooditemtype.HeaderText = "TypeID";
                    fooditemtype.DisplayIndex = 4;
                    fooditemtype.Visible = false;
                }
            }

            dataGridView1.Columns[1].HeaderText = "     Dish name";
            dataGridView1.Columns[3].HeaderText = "Dish price";
            dataGridView1.Columns[4].HeaderText = "Un-restrict";

            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

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

        private void FilterDataGridView(string searchText)
        {

            if (string.IsNullOrEmpty(searchText))
            {
                LoadData();  
                return;
            }

            var filter = searchText.ToLower();
            var filteredItems = foodItems
                .Where(item => item.FoodName.ToLower().Contains(filter))
                .ToList();

            dataGridView1.DataSource = filteredItems;
        }

        private void LoadFoodItemTypes()
        {
            foodItemNames = new Dictionary<int, string>();

            string foodItemTypesQuery = "SELECT FooditemtypeID, FooditemtypeName FROM FoodItemsTypes";

            try
            {
                DataTable foodItemTypesTable = crudDatabase.FetchDataFromDatabase(foodItemTypesQuery);

                foreach (DataRow row in foodItemTypesTable.Rows)
                {
                    int id = Convert.ToInt32(row["FooditemtypeID"]);
                    string name = row["FooditemtypeName"].ToString();
                    foodItemNames[id] = name; // Populate the dictionary
                }

                // Populate the ComboBox with food item types
                customBox1.Items.Clear();
                foreach (var kvp in foodItemNames)
                {
                    customBox1.Items.Add(new { ID = kvp.Key, Name = kvp.Value });
                }

                customBox1.DisplayMember = "Name";
                customBox1.ValueMember = "ID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading food item types: " + ex.Message);
            }
        }

        private void saveCheckedChanges() 
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        // Ensure FoodID is not null and is of the correct type
                        if (row.Cells["FoodID"].Value != DBNull.Value)
                        {
                            int foodID = Convert.ToInt32(row.Cells["FoodID"].Value);

                            // Check if IsChecked is not null and convert it to boolean
                            bool newIsChecked = row.Cells["IsChecked"].Value != DBNull.Value && Convert.ToBoolean(row.Cells["IsChecked"].Value);

                            // Retrieve the current IsChecked status from the database
                            string currentIsCheckedQuery = "SELECT IsChecked FROM FoodItems WHERE FoodID = @FoodID";
                            var parameters = new Dictionary<string, object>
                            {
                                { "@FoodID", foodID }
                            };
                            var currentIsCheckedTable = crudDatabase.FetchDataFromDatabase(currentIsCheckedQuery, parameters);
                            bool currentIsChecked = currentIsCheckedTable.Rows.Count > 0 && Convert.ToBoolean(currentIsCheckedTable.Rows[0]["IsChecked"]);

                            // Only update if the new value is different from the current value
                            if (newIsChecked != currentIsChecked)
                            {
                                string updateQuery = "UPDATE FoodItems SET IsChecked = @IsChecked WHERE FoodID = @FoodID";
                                var updateParameters = new Dictionary<string, object>
                                {
                                    { "@IsChecked", newIsChecked ? 1 : 0 },
                                    { "@FoodID", foodID }
                                };
                                crudDatabase.ExecuteNonQuery(updateQuery, updateParameters);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
            saveCheckedChanges();
        }

        private void Butttondisabler_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void Butttondisabler_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
               
                if (dataGridView1.RowCount > 0) 
                {
                    SetupDataGridViewColumns();
                }
            }
        }

        private void Butttondisabler_FormClosing(object sender, FormClosingEventArgs e)
        {
            //saveCheckedChanges();
        }

     

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            FilterDataGridView(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var currentDataSource = dataGridView1.DataSource as List<FoodItem>;

            if (currentDataSource != null && currentDataSource.All(item => item.IsChecked))
            {
                LoadData();
            }
            else
            {
                var checkedItems = foodItems.Where(item => item.IsChecked).ToList();
                dataGridView1.DataSource = checkedItems;
            }
        }

        private void customBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (customBox1.SelectedItem != null)
            {
                dynamic selectedItem = customBox1.SelectedItem;
                int selectedFoodItemTypeID = selectedItem.ID;

                var filteredItems = foodItems
                    .Where(item => item.FooditemtypeID == selectedFoodItemTypeID)
                    .ToList();

                dataGridView1.DataSource = filteredItems;
            }
            else
            {
                LoadData();
            }
        }

        private void customBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.showKeyboard)
            {
                clavieroverlay = new Clavieroverlay(textBox1);
                clavieroverlay.boardLocationBottom();
                clavieroverlay.Show();
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            clavieroverlay?.Hide();
        }

        private void Butttondisabler_Click(object sender, EventArgs e)
        {
            if (clavieroverlay != null && clavieroverlay.Visible == true)
            {
                clavieroverlay.Visible = false;
            }
            dataGridView1.Focus();
        }


        public class FoodItem
        {
            public int FoodID { get; set; }
            public string FoodName { get; set; }
            public int FooditemtypeID { get; set; }
            public string Price { get; set; }
            public bool IsChecked { get; set; }
        }

        public void LocalizeControls()
        {
            button1.Text = LanguageManager.Instance.GetString("Btn-close");
            rjButton1.Text = LanguageManager.Instance.GetString("Btn-save");
            button2.Text = LanguageManager.Instance.GetString("BTNDIS-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
            //button4.Text = LanguageManager.Instance.GetString("MF-btn4");
            //button5.Text = LanguageManager.Instance.GetString("MF-btn5");
        }

        public void ApplyTheme()
        {
            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            pictureBox1.Image = colors.Image1;
            textBox1.ForeColor = colors.Color2;
            customBox1.BackColor = colors.Color3;
            customBox1.ForeColor = colors.Color2;
            customBox1.ArrowColor = colors.Color2;
            customBox1.BorderColor = colors.Color5;
            roundedPanel1.BackColor = colors.Color3;
            textBox1.BackColorRounded = colors.Color3;
            customBox1.BorderFocusColor = colors.Color5;

            button2.BackColor = colors.Color3;
            button2.ForeColor = this.ForeColor;
            button2.BorderColor = colors.Color3;    
            textBox1.BorderColor = colors.Color3;
            pictureBox1.BackColor = colors.Color3;
            roundedPanel2.BackColor = colors.Color3;
            roundedPanel1.ForeColor = this.ForeColor;
            roundedPanel2.ForeColor = this.ForeColor;
            dataGridView1.ForeColor = this.ForeColor;
            roundedPanel1.BorderColor = colors.Color3;
            textBox1.BackColorRounded = this.BackColor;
            dataGridView1.BackgroundColor = colors.Color3;
            dataGridView1.DefaultCellStyle.BackColor = colors.Color3;
            dataGridView1.DefaultCellStyle.ForeColor = this.ForeColor;
            dataGridView1.DefaultCellStyle.SelectionForeColor = this.ForeColor;
            if (colors.Bool1 && Properties.Settings.Default.showScrollbars) showScrollBars();
        }

        private void showScrollBars()
        {
            dataGridView1.ScrollBars = ScrollBars.Vertical;
        }
    }
}
