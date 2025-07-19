using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using PadTai.Classes;
using System.Data.SQLite;
using System.Windows.Forms;
using PadTai.Classes.Others;
using PadTai.Classes.Databaselink;
using PadTai.Sec_daryfolders.Others;
using PadTai.Sec_daryfolders.Others_Forms;
using PadTai.Sec_daryfolders.Updaters.FoodUpdater;
using System.Collections.Generic;


namespace PadTai.Sec_daryfolders.Grossmanager
{
    public partial class Modifyproduct : UserControl
    {
        private Clavieroverlay clavieroverlay;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;
        private DataTable dataTable;
        private string currentState;

        public Modifyproduct()
        {
            InitializeComponent();
            InitializeControlResizer();

            this.DoubleBuffered = true;
            crudDatabase = new CrudDatabase();

            LocalizeControls();
            LoadTables();
            ApplyTheme();

            dataGridView1.GridColor = this.BackColor;
        }

        private void InitializeControlResizer()
        {
            roundedPanel3.Visible = false;

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(label2);
            resizer.RegisterControl(label3);
            resizer.RegisterControl(label4);
            resizer.RegisterControl(label18);
            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(rjButton2);
            resizer.RegisterControl(pictureBox1);
            resizer.RegisterControl(roundedPanel1);
            resizer.RegisterControl(roundedPanel2);
            resizer.RegisterControl(roundedPanel3);
            resizer.RegisterControl(dataGridView1);
            resizer.RegisterControl(roundedTextbox1);
            resizer.RegisterControl(roundedTextbox2);
            resizer.RegisterControl(roundedTextbox3);
            resizer.RegisterControl(roundedTextbox4);

            if (!Properties.Settings.Default.showKeyboard)
            {
                dataGridView1.MouseEnter += Control_MouseEnter;
                dataGridView1.MouseDown += Control_MouseDown;
            }
        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (resizer != null)
            {
                resizer.ResizeControls(this);

                subgroupDGVColumns();
            }
        }


        public void LoadTables()
        {
            string query = "SELECT ProductID, ProductName, ProductPrice FROM GrossProducts";

            dataTable = crudDatabase.FetchDataFromDatabase(query);

            if (dataTable.Rows.Count > 0)
            {
                // Add Update and Delete columns
                dataTable.Columns.Add("Update", typeof(string));
                dataTable.Columns.Add("Delete", typeof(string));

                foreach (DataRow row in dataTable.Rows)
                {
                    row["Update"] = "✏️";
                    row["Delete"] = "❌";
                }

                dataGridView1.AutoGenerateColumns = true;
                dataGridView1.DataSource = dataTable;

                subgroupDGVColumns();
            }
            else
            {
                // Handle the case where no data is found
                label18.Text = "No tables found.";
                label18.ForeColor = Color.Tomato;
            }
        }

        private void subgroupDGVColumns()
        {
            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[1].DefaultCellStyle.Padding = new Padding(20, 0, 0, 0);
                dataGridView1.Columns[0].Width = (int)(dataGridView1.Width * 0.10);
                dataGridView1.Columns[1].Width = (int)(dataGridView1.Width * 0.60);
                dataGridView1.Columns[2].Width = (int)(dataGridView1.Width * 0.14);
                dataGridView1.Columns[3].Width = (int)(dataGridView1.Width * 0.08);
                dataGridView1.Columns[4].Width = (int)(dataGridView1.Width * 0.08);
                dataGridView1.RowTemplate.Height = 30;

                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                if (dataGridView1.Columns.Count > 0)
                {
                    dataGridView1.Columns[0].HeaderText = "ID";
                    dataGridView1.Columns[1].HeaderText = "Name";
                    dataGridView1.Columns[2].HeaderText = "Price";
                    dataGridView1.Columns["Update"].HeaderText = "Update";
                    dataGridView1.Columns["Delete"].HeaderText = "Delete";
                }

                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    dataGridView1.ColumnHeadersHeight = 30;
                    dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.DarkGray;
                    dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                    dataGridView1.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.DarkGray;
                    dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = dataGridView1.BackgroundColor;
                    dataGridView1.Columns[i].HeaderCell.Style.Font = new Font("Arial", 10, FontStyle.Bold);
                    dataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = dataGridView1.BackColor;
                    dataGridView1.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                }
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Check if the current cell is in the TotalPrice column
            if (dataGridView1.Columns[e.ColumnIndex].Name == "ProductPrice" && e.Value != null)
            {
                // Format the value as a decimal and append the currency symbol
                if (decimal.TryParse(e.Value.ToString(), out decimal price))
                {
                    e.Value = price.ToString("N2") + $" {CurrencyService.Instance.GetCurrencySymbol()}";
                    e.FormattingApplied = true; // Indicate that formatting has been applied
                }
            }
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Update")
            {
                e.CellStyle.ForeColor = Color.DodgerBlue;
                e.CellStyle.SelectionForeColor = Color.DeepSkyBlue;
                e.CellStyle.Font = new Font("Century Gothic", 12, FontStyle.Bold);
            }

            else if (dataGridView1.Columns[e.ColumnIndex].Name == "Delete")
            {
                e.CellStyle.ForeColor = Color.Tomato;
                e.CellStyle.SelectionForeColor = Color.Orange;
                e.CellStyle.Font = new Font("Century Gothic", 12, FontStyle.Bold);
            }
        }

        private void roundedTextbox1_TextChanged(object sender, EventArgs e)
        {
            string filterExpression = roundedTextbox1.Text.Trim();

            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);

                if (!string.IsNullOrEmpty(filterExpression) && filterExpression != roundedTextbox1.WaterMark)
                {
                    dataView.RowFilter = $"ProductName LIKE '%{filterExpression}%'";
                }
                else
                {
                    dataView.RowFilter = string.Empty;
                }
                dataGridView1.DataSource = dataView;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == dataGridView1.Columns["Update"].Index)
                {
                    var selectedId = dataGridView1.Rows[e.RowIndex].Cells[0].Value;

                    if (selectedId != null)
                    {
                        currentState = "Update";
                        rjButton1.Text = "Back";
                        label18.Text = string.Empty;
                        roundedPanel1.Enabled = false;
                        roundedTextbox2.ReadOnly = true;
                        roundedTextbox3.ReadOnly = false;
                        roundedTextbox4.ReadOnly = false;
                        rjButton2.Text = "Update the product";
                        rjButton2.BackColor = Color.DodgerBlue;
                        loadtablesbyID(selectedId.ToString());
                        rjButton1.BackColor = roundedPanel1.BackColor;
                        roundedTextbox2.WaterMark = "Optional";
                        roundedPanel3.Visible = true;
                    }
                }
                else if (e.ColumnIndex == dataGridView1.Columns["Delete"].Index)
                {
                    var selectedId = dataGridView1.Rows[e.RowIndex].Cells[0].Value;

                    if (selectedId != null)
                    {
                        currentState = "Delete";
                        rjButton1.Text = "Back";
                        label18.Text = string.Empty;
                        roundedPanel1.Enabled = false;
                        roundedTextbox4.ReadOnly = true;
                        roundedTextbox2.ReadOnly = true;
                        roundedTextbox3.ReadOnly = true;
                        rjButton2.BackColor = Color.Crimson;
                        rjButton2.Text = "Delete the product";
                        loadtablesbyID(selectedId.ToString());
                        rjButton1.BackColor = roundedPanel1.BackColor;
                        roundedTextbox2.WaterMark = "Optional";
                        roundedPanel3.Visible = true;
                    }
                }
            }
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
            if (roundedPanel3.Visible == false)
            {
                currentState = "Add";
                rjButton1.Text = "Back";
                label18.Text = string.Empty;
                roundedPanel3.Visible = true;
                roundedPanel1.Enabled = false;
                rjButton2.Text = "Add a product";
                roundedTextbox2.ReadOnly = true;
                roundedTextbox3.ReadOnly = false;
                roundedTextbox4.ReadOnly = false;
                rjButton2.BackColor = Color.Green;
                rjButton1.BackColor = roundedPanel1.BackColor;
                roundedTextbox2.WaterMark = "Optional";
            }
            else if (roundedPanel3.Visible == true)
            {
                clearControls();
                roundedPanel3.Visible = false;
                roundedPanel1.Enabled = true;
                rjButton1.Text = "Add product";
                rjButton1.BackColor = SystemColors.Highlight;
            }
        }

        private void loadtablesbyID(string productId)
        {
            DataTable Details = crudDatabase.FetchDataFromDatabase($"SELECT ProductID, ProductName, ProductPrice FROM GrossProducts WHERE ProductID = {productId}");

            if (Details.Rows.Count > 0)
            {
                roundedTextbox2.Text = Details.Rows[0]["ProductID"].ToString();
                roundedTextbox3.Text = Details.Rows[0]["ProductName"].ToString();

                var productPrice = Details.Rows[0]["ProductPrice"];
                roundedTextbox4.Text = productPrice != DBNull.Value ? productPrice.ToString() : "0";
            }
            else
            {
                roundedTextbox4.Text = "No details found.";
                roundedTextbox2.Text = "No details found.";
                roundedTextbox3.Text = "No details found.";
            }
        }

        public void InsertTable()
        {
            if (string.IsNullOrWhiteSpace(roundedTextbox3.Text))
            {
                label18.Text = "Please enter a product";
                label18.ForeColor = Color.Tomato;
                return;
            }

            if (decimal.TryParse(roundedTextbox4.Text, out decimal price))
            {
                label18.Text = "Please enter a valid price";
                label18.ForeColor = Color.Tomato;
                return;
            }
            else
            {
                try
                {
                    // If TableID is empty, do not include it in the insert command
                    string insertCommand = @"INSERT INTO GrossProducts (ProductName, ProductPrice) VALUES (@ProductName, @ProductPrice)";

                    // Prepare parameters
                    var parameters = new Dictionary<string, object>
                    {
                        { "@ProductName", roundedTextbox3.Text.Trim() },
                        { "@ProductPrice", price }
                    };

                    // Execute the command using the CRUDDatabase class
                    bool isInserted = crudDatabase.ExecuteNonQuery(insertCommand, parameters);

                    if (isInserted)
                    {
                        LoadTables();
                        clearControls();
                        label18.Text = string.Empty;
                        this.Alert("Product added!", Alertform.enmType.Success);
                        label18.Text = "Table added successfully!";
                        label18.ForeColor = Color.LawnGreen;
                    }
                    else
                    {
                        label18.Text = "Failed to insert the product.";
                        label18.ForeColor = Color.Tomato;
                    }
                }
                catch (Exception ex)
                {
                    label18.Text = string.Empty;
                    label18.Text += ex.Message;
                    label18.ForeColor = Color.Tomato;
                }
            }
        }


        public void UpdateTablenumber()
        {
            // Validate Subgroup name input
            if (string.IsNullOrWhiteSpace(roundedTextbox3.Text))
            {
                label18.Text = "Please enter a table.";
                label18.ForeColor = Color.Tomato;
                return;
            }

            // Check if any row is selected in the DataGridView
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Get the selected row
                var selectedRow = dataGridView1.SelectedRows[0];
                var selectedId = selectedRow.Cells[0].Value;

                if (selectedId != null)
                {
                    string updateQuery = "UPDATE GrossProducts SET ProductName = @ProductName, ProductPrice = @ProductPrice WHERE ProductID = @selectedId";

                    // Prepare parameters
                    var parameters = new Dictionary<string, object>
                    {
                        { "@ProductName", roundedTextbox3.Text.Trim() }
                    };

                    // Validate and add Seatsamount
                    if (decimal.TryParse(roundedTextbox4.Text.Trim(), out decimal seatsAmount))
                    {
                        parameters.Add("@ProductPrice", seatsAmount);
                    }
                    else
                    {
                        label18.Text = "Please enter a valid price.";
                        label18.ForeColor = Color.Tomato;
                        return;
                    }

                    parameters.Add("@selectedId", selectedId);

                    // Execute the update command using the CRUDDatabase class
                    bool isUpdated = crudDatabase.ExecuteNonQuery(updateQuery, parameters);


                    if (isUpdated)
                    {
                        label18.Text = "Product updated successfully!";
                        label18.ForeColor = Color.LawnGreen;
                        LoadTables();
                        clearControls();
                        rjButton1.Text = "Add product";
                        roundedPanel1.Enabled = true;
                        roundedPanel3.Visible = false;
                        rjButton1.BackColor = SystemColors.Highlight;
                        this.Alert("Product updated successfully!", Alertform.enmType.Success);
                    }
                    else
                    {
                        label18.Text = "No item found to update.";
                        label18.ForeColor = Color.Tomato;
                    }

                }
                else
                {
                    label18.Text = "Please select an item to update.";
                    label18.ForeColor = Color.Tomato;
                }
            }
        }


        public void DeleteSubgroup()
        {
            // Check if any row is selected in the DataGridView
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Get the selected row
                var selectedRow = dataGridView1.SelectedRows[0];
                var selectedId = selectedRow.Cells[0].Value;

                if (selectedId != null && int.TryParse(selectedId.ToString(), out int productId))
                {
                    // Prepare the delete query
                    string deleteQuery = $"DELETE FROM GrossProducts WHERE ProductID = @id";
                 
                    var parameters = new Dictionary<string, object>
                    {
                        { "@id", productId }
                    };

                    // Execute the delete command using ExecuteNonQuery
                    if (crudDatabase.ExecuteNonQuery(deleteQuery, parameters))
                    {
                        label18.Text = "Product deleted successfully!";
                        label18.ForeColor = Color.LawnGreen;
                        LoadTables();
                        clearControls();
                        rjButton1.Text = "Add product";
                        roundedPanel1.Enabled = true;
                        roundedPanel3.Visible = false;
                        rjButton1.BackColor = SystemColors.Highlight;
                        this.Alert("Product deleted successfully!", Alertform.enmType.Success);
                    }
                    else
                    {
                        label18.Text = "Failed to delete product.";
                        label18.ForeColor = Color.Tomato;
                    }
                }
                else
                {
                    label18.Text = "Invalid product selected.";
                    label18.ForeColor = Color.Tomato;
                }
            }
            else
            {
                label18.Text = "Please select a product to delete.";
                label18.ForeColor = Color.Tomato;
            }
        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            if (currentState == "Update")
            {
                UpdateTablenumber();
            }
            else if (currentState == "Delete")
            {
                using (Deletedishconfirm DDC = new Deletedishconfirm())
                {
                    FormHelper.ShowFormWithOverlay(this.FindForm(), DDC);

                    // Check the DialogResult after the dialog is closed
                    if (DDC.DialogResult == DialogResult.OK)
                    {
                        DeleteSubgroup();
                        DDC.Close();
                    }
                    else if (DDC.DialogResult == DialogResult.Cancel)
                    {
                        DDC.Close();
                    }
                }
            }
            else if (currentState == "Add")
            {
                InsertTable();
            }
            else
            {
                roundedPanel3.Visible = false;
            }
        }

        private void clearControls()
        {
            roundedTextbox2.Text = string.Empty;
            roundedTextbox3.Text = string.Empty;
            roundedTextbox4.Text = string.Empty;

            if (currentState == "Add")
            {
                roundedTextbox2.WaterMark = "Optional";
            }
        }

        private void Alert(string msg, Alertform.enmType type)
        {
            Alertform alertform = new Alertform();
            alertform.showAlert(msg, type);
        }

        private void roundedTextbox1_Enter(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.showKeyboard)
            {
                clavieroverlay = new Clavieroverlay(roundedTextbox1);
                clavieroverlay.boardLocationBottom();
                clavieroverlay.Show();
            }
        }

        private void roundedTextbox1_Leave(object sender, EventArgs e)
        {
            clavieroverlay?.Hide();
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


        public void LocalizeControls()
        {
            //button1.Text = LanguageManager.Instance.GetString("MF-btn1");
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
            //button4.Text = LanguageManager.Instance.GetString("MF-btn4");

        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            pictureBox1.Image = colors.Image1;
            roundedPanel1.BackColor = colors.Color3;
            roundedPanel2.BackColor = colors.Color3;
            roundedPanel3.BackColor = colors.Color3;
            roundedTextbox1.ForeColor = colors.Color2;
            roundedTextbox2.ForeColor = colors.Color2;
            roundedTextbox3.ForeColor = colors.Color2;
            roundedTextbox4.ForeColor = colors.Color2;
            roundedTextbox2.BackColorRounded = colors.Color1;
            roundedTextbox3.BackColorRounded = colors.Color1;
            roundedTextbox4.BackColorRounded = colors.Color1;

            rjButton1.ForeColor = this.ForeColor;
            dataGridView1.ForeColor = this.ForeColor;
            pictureBox1.BackColor = roundedPanel1.BackColor;
            roundedTextbox1.BackColorRounded = this.BackColor;
            roundedPanel1.BorderColor = roundedPanel1.BackColor;
            roundedTextbox1.BorderColor = roundedPanel1.BackColor;
            dataGridView1.BackgroundColor = roundedPanel3.BackColor;
            dataGridView1.DefaultCellStyle.ForeColor = this.ForeColor;
            dataGridView1.DefaultCellStyle.BackColor = roundedPanel3.BackColor;
            dataGridView1.DefaultCellStyle.SelectionForeColor = this.ForeColor;
        }
    }
}
