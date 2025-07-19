using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using PadTai.Classes;
using System.Data.SQLite;
using System.Windows.Forms;
using PadTai.Classes.Others;
using System.Collections.Generic;
using PadTai.Sec_daryfolders.Others;
using PadTai.Sec_daryfolders.Updates;
using PadTai.Classes.Databaselink;


namespace PadTai.Fastcheckfiles
{
    public partial class IndividualDiscount : UserControl
    {
        private Dictionary<int, DiscountConfig> _discountConfigurations = new Dictionary<int, DiscountConfig>();
        private DataGridViewScroller scroller;
        private Clavieroverlay clavieroverlay;
        private Receiptdetails receiptdetails;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;
        private int _selectedFoodID;
        Fastcheck FCH;

        public IndividualDiscount(Fastcheck fCH)
        {
            InitializeComponent();

            crudDatabase = new CrudDatabase();
            rjButton2.Visible = false;
            rjButton1.Enabled = false;
            rjButton3.Enabled = false;  

            this.FCH = fCH;

            ApplyTheme();
            LocalizeControls();
            InitializeControlResizer();
            dataGridView1.GridColor = this.BackColor;
        }


        private void InitializeControlResizer()
        {
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(label2);
            resizer.RegisterControl(label3);
            resizer.RegisterControl(label4);
            resizer.RegisterControl(label5);
            resizer.RegisterControl(textBox1);
            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(rjButton2);
            resizer.RegisterControl(rjButton3);
            resizer.RegisterControl(customBox1);
            resizer.RegisterControl(customBox2);
            resizer.RegisterControl(customBox3);
            resizer.RegisterControl(dataGridView1);
            resizer.RegisterControl(roundedPanel1);

            LoadComboBoxes();
            LoadFoodItems();
            LoadDiscountConfigurations();
            scroller = new DataGridViewScroller(this, null, dataGridView1);
            rjButton1.Enabled = rjButton3.Enabled = Properties.Settings.Default.enableIndvButton;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (resizer != null)
            {
                resizer.ResizeControls(this);
            }
        }


        private void LoadComboBoxes()
        {
            customBox1.Items.AddRange(new object[] { 10, 15, 20, 25, 30, 35, 40, 45, 50 });
            customBox1.SelectedIndex = 0;

            customBox3.Items.AddRange(new object[] { 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            customBox3.SelectedIndex = 0;
        }

        private List<FoodItems> GetFoodItems()
        {
            string query = "SELECT FoodID, FoodName, Price FROM FoodItems";

            return crudDatabase.FetchDataToList(query, reader => new FoodItems
            {
                FoodID = Convert.ToInt32(reader["FoodID"]),
                FoodName = reader["FoodName"].ToString(),
                Price = Convert.ToDecimal(reader["Price"])
            });
        }


        private void LoadFoodItems()
        {
            var foodItems = GetFoodItems();

            // Set up your DataGridView columns first (if necessary)

            dataGridView1.DataSource = foodItems;
            dataGridView1.ForeColor = this.ForeColor;
            dataGridView1.Columns["FoodID"].Visible = false;
            dataGridView1.Columns["FoodName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns["FoodName"].DefaultCellStyle.Padding = new Padding(20, 0, 0, 0);
            dataGridView1.Columns["Price"].Visible = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                LoadFoodItems();
                return;
            }
            var filter = textBox1.Text.ToLower();
            var filteredItems = ((List<FoodItems>)dataGridView1.DataSource).
                Where(item => item.FoodName.ToLower().Contains(filter)).ToList();
                dataGridView1.DataSource = filteredItems;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var selectedRow = dataGridView1.Rows[e.RowIndex];
                if (selectedRow.DataBoundItem is FoodItems foodItem)
                {
                    textBox1.Text = foodItem.FoodName;
                    int selectedFoodID = foodItem.FoodID;

                    if (_discountConfigurations.ContainsKey(selectedFoodID))
                    {
                        var config = _discountConfigurations[selectedFoodID];
                        customBox1.Text = config.DiscountPercentage.ToString();
                        customBox3.Text = config.OccurrencesRequired.ToString();
                    }
                    else
                    {
                        customBox1.SelectedIndex = 0;
                        customBox3.SelectedIndex = 0;
                    }
                }
            }
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) || dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a food element from the table", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var selectedRow = dataGridView1.SelectedRows[0];
            if (selectedRow.DataBoundItem is FoodItems foodItem)
            {

                int selectedFoodID = foodItem.FoodID;
                int discountPercentage;
                if (!int.TryParse(customBox1.Text, out discountPercentage))
                {
                    MessageBox.Show("Invalid discount percentage", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                int occurrencesRequired;
                if (!int.TryParse(customBox3.Text, out occurrencesRequired))
                {
                    MessageBox.Show("Invalid occurrences number", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DiscountConfig config = new DiscountConfig
                {
                    DiscountPercentage = discountPercentage,
                    OccurrencesRequired = occurrencesRequired
                };

                _discountConfigurations[selectedFoodID] = config;
                SaveDiscountConfiguration(selectedFoodID, config);


                if (!customBox2.Items.Cast<string>().Any(x => x == textBox1.Text))
                    customBox2.Items.Add(textBox1.Text);

            }
            Fastcheck fastcheck = new Fastcheck(receiptdetails);
            fastcheck.Show();
            FCH.Close();
        }

        private void customBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (customBox2.SelectedIndex == -1) return;
            textBox1.Text = string.Empty;

            var selectedFoodName = customBox2.SelectedItem.ToString();
            var foodItems = (List<FoodItems>)dataGridView1.DataSource;
            var selectedItem = foodItems.FirstOrDefault(item => item.FoodName == selectedFoodName);

            if (selectedItem != null)
            {
                _selectedFoodID = selectedItem.FoodID;
                dataGridView1.ClearSelection();
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    var foodItem = (FoodItems)row.DataBoundItem;
                    if (foodItem.FoodID == selectedItem.FoodID)
                    {
                        row.Selected = true;
                        break;
                    }
                }
                textBox1.Text = selectedFoodName;
                if (_discountConfigurations.ContainsKey(selectedItem.FoodID))
                {
                    var config = _discountConfigurations[selectedItem.FoodID];
                    customBox1.Text = config.DiscountPercentage.ToString();
                    customBox3.Text = config.OccurrencesRequired.ToString();
                }
                else
                {
                    customBox1.SelectedIndex = 0;
                    customBox3.SelectedIndex = 0;
                }
            }
        }


        private void LoadDiscountConfigurations()
        {
            string query = @"SELECT id.FoodID, id.DiscountPercentage, id.OccurrencesRequired, fi.FoodName 
                              FROM IndividualDiscounts id INNER JOIN FoodItems fi ON id.FoodID = fi.FoodID";

            // Fetch data using the FetchDataFromDatabase method
            DataTable resultTable = crudDatabase.FetchDataFromDatabase(query);

            // Check if any rows were returned
            if (resultTable != null && resultTable.Rows.Count > 0)
            {
                foreach (DataRow row in resultTable.Rows)
                {
                    int foodID = Convert.ToInt32(row["FoodID"]);
                    DiscountConfig config = new DiscountConfig
                    {
                        DiscountPercentage = Convert.ToInt32(row["DiscountPercentage"]),
                        OccurrencesRequired = Convert.ToInt32(row["OccurrencesRequired"])
                    };
                    _discountConfigurations[foodID] = config;

                    string foodName = row["FoodName"].ToString();
                    if (!customBox2.Items.Cast<string>().Any(x => x == foodName))
                    {
                        customBox2.Items.Add(foodName);
                    }
                }
            }
        }

        private void SaveDiscountConfiguration(int foodID, DiscountConfig config)
        {
            string query = "INSERT OR REPLACE INTO IndividualDiscounts (FoodID, DiscountPercentage, OccurrencesRequired) " +
                           "VALUES (@FoodID, @DiscountPercentage, @OccurrencesRequired)";

            // Create a dictionary to hold the parameters
            var parameters = new Dictionary<string, object>
            {
                { "@FoodID", foodID },
                { "@DiscountPercentage", config.DiscountPercentage },
                { "@OccurrencesRequired", config.OccurrencesRequired }
            };

            // Execute the query using ExecuteNonQuery
            crudDatabase.ExecuteNonQuery(query, parameters);
        }

        private void rjButton3_Click(object sender, EventArgs e)
        {
            if (customBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Select a discount to delete", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DeleteDiscountConfiguration(_selectedFoodID);

            customBox2.Items.RemoveAt(customBox2.SelectedIndex);
            _discountConfigurations.Remove(_selectedFoodID);
            textBox1.Text = string.Empty;
            customBox1.Text = string.Empty;
            customBox2.Text = string.Empty;
            customBox3.Text = string.Empty;

            Fastcheck fastcheck = new Fastcheck(receiptdetails);
            fastcheck.Show();
            FCH.Close();
        }

        private void DeleteDiscountConfiguration(int foodID)
        {
            string query = $"DELETE FROM IndividualDiscounts WHERE FoodID = {foodID}";

            // Execute the query using ExecuteNonQuery
            crudDatabase.ExecuteNonQuery(query);
        }

        private void customBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            DishGroupControl dishGroupControl = new DishGroupControl(FCH, null);
            FCH.AddUserControl(dishGroupControl);
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

        private void customBox1_Leave(object sender, EventArgs e)
        {
            clavieroverlay?.Hide();
        }

        private void customBox1_Enter(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.showKeyboard)
            {
                clavieroverlay = new Clavieroverlay(customBox1);
                clavieroverlay.boardLocationBottom();
                clavieroverlay.Show();
            }
        }

        private void customBox3_Leave(object sender, EventArgs e)
        {
            clavieroverlay?.Hide();
        }

        private void customBox3_Enter(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.showKeyboard)
            {
                clavieroverlay = new Clavieroverlay(customBox3);
                clavieroverlay.boardLocationBottom();
                clavieroverlay.Show();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (dataGridView1.Focused)
            {
                int newIndex = dataGridView1.FirstDisplayedScrollingRowIndex + (e.Delta > 0 ? -1 : 1);
                if (newIndex >= 0 && newIndex < dataGridView1.Rows.Count)
                {
                    try
                    {
                        dataGridView1.FirstDisplayedScrollingRowIndex = newIndex;
                    }
                    catch (InvalidOperationException)
                    {

                    }
                }
            }
        }

        private void Control_MouseEnter(object sender, EventArgs e)
        {
            // Set focus to the control that the mouse entered
            if (sender is Control control)
            {
                control.Focus();
            }
        }

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            // Set focus to the control that was touched or clicked
            if (sender is Control control)
            {
                control.Focus();
            }
        }

        public Dictionary<int, DiscountConfig> GetDiscountConfigurations()
        {
            return _discountConfigurations;
        }

        public void LocalizeControls()
        {
            label2.Text = LanguageManager.Instance.GetString("IndivDisc-lbl2");
            label3.Text = LanguageManager.Instance.GetString("IndivDisc-lbl3");
            label4.Text = LanguageManager.Instance.GetString("IndivDisc-lbl4");
            label5.Text = LanguageManager.Instance.GetString("IndivDisc-lbl5");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            label2.ForeColor = colors.Color2;
            label3.ForeColor = colors.Color2;
            label4.ForeColor = colors.Color2;
            label5.ForeColor = colors.Color2;
            textBox1.ForeColor = colors.Color2;
            customBox1.ForeColor = colors.Color2;
            customBox2.ForeColor = colors.Color2;
            customBox3.ForeColor = colors.Color2;
            customBox1.BackColor = colors.Color3;
            customBox2.BackColor = colors.Color3;
            customBox3.BackColor = colors.Color3;
            customBox1.ArrowColor = colors.Color2;
            customBox2.ArrowColor = colors.Color2;
            customBox3.ArrowColor = colors.Color2;            
            customBox1.BorderColor = colors.Color5;
            customBox2.BorderColor = colors.Color5;
            customBox3.BorderColor = colors.Color5;
            roundedPanel1.BackColor = colors.Color3;
            roundedPanel1.ForeColor = colors.Color2;
            textBox1.BackColorRounded = colors.Color3;
            customBox1.BorderFocusColor = colors.Color5;
            customBox2.BorderFocusColor = colors.Color5;
            customBox3.BorderFocusColor = colors.Color5;
            dataGridView1.BackgroundColor = colors.Color3;
            dataGridView1.DefaultCellStyle.ForeColor = colors.Color2;
            dataGridView1.DefaultCellStyle.BackColor = colors.Color3;
            dataGridView1.DefaultCellStyle.SelectionForeColor = colors.Color2;
            if (colors.Bool1 && Properties.Settings.Default.showScrollbars) showScrollBars();
        }

        private void showScrollBars()
        {
            dataGridView1.ScrollBars = ScrollBars.Vertical;
        }

        public class FoodItems
        {
            public int FoodID { get; set; }
            public string FoodName { get; set; }
            public decimal Price { get; set; }
        }
    }
}




