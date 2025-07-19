using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using PadTai.Classes;
using System.Data.SQLite;
using System.Windows.Forms;
using PadTai.Classes.Others;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using PadTai.Sec_daryfolders.Others;
using PadTai.Sec_daryfolders.Others_Forms;
using PadTai.Sec_daryfolders.Updaters.FoodUpdater;


namespace PadTai.Sec_daryfolders.Updaters.Otherupdates
{
    public partial class Modifypaygroups : UserControl
    {
        ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();
        private Clavieroverlay clavieroverlay;
        private DataGridViewScroller scroller;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;
        private DataTable dataTable;
        private string currentState;

        public Modifypaygroups()
        {
            InitializeComponent();

            InitializeControlResizer();
            this.DoubleBuffered = true;
            crudDatabase = new CrudDatabase();
            dataGridView1.GridColor = colors.Color1;

            LoadDiscountsTable();
            LocalizeControls();
            ApplyTheme();
        }

        private void InitializeControlResizer()
        {
            roundedPanel3.Visible = false;

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(label2);
            resizer.RegisterControl(label3);
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
            scroller = new DataGridViewScroller(this, null, dataGridView1);
        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (resizer != null)
            {
                resizer.ResizeControls(this);
                DGVColumns();
            }
        }

        public void LoadDiscountsTable()
        {
            string query = "SELECT * FROM PaymentGroups";

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

                DGVColumns();
            }
            else
            {
                // Handle the case where no data is found
                label18.Text = "No payment types found.";
                label18.ForeColor = Color.Tomato;
            }
        }

        private void DGVColumns()
        {
            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[1].DefaultCellStyle.Padding = new Padding(20, 0, 0, 0);
                dataGridView1.Columns[0].Width = (int)(dataGridView1.Width * 0.14);
                dataGridView1.Columns[1].Width = (int)(dataGridView1.Width * 0.70);
                dataGridView1.Columns[2].Width = (int)(dataGridView1.Width * 0.08);
                dataGridView1.Columns[3].Width = (int)(dataGridView1.Width * 0.08);
                dataGridView1.RowTemplate.Height = 30;

                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                if (dataGridView1.Columns.Count > 0)
                {
                    dataGridView1.Columns[0].HeaderText = "ID";
                    dataGridView1.Columns[1].HeaderText = "Name";
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
                    dataView.RowFilter = $"PaymentGroupName LIKE '%{filterExpression}%'";
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
                        rjButton2.Text = "Update the discount";
                        rjButton2.BackColor = Color.DodgerBlue;
                        loadpaymentgroups(selectedId.ToString());
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
                        roundedTextbox2.ReadOnly = true;
                        roundedTextbox3.ReadOnly = true;
                        rjButton2.BackColor = Color.Crimson;
                        rjButton2.Text = "Delete the discount";
                        loadpaymentgroups(selectedId.ToString());
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
                rjButton2.Text = "Add a discount";
                roundedTextbox2.ReadOnly = false;
                roundedTextbox3.ReadOnly = false;
                rjButton2.BackColor = Color.Green;
                rjButton1.BackColor = roundedPanel1.BackColor;
                roundedTextbox2.WaterMark = "Optional";
            }
            else if (roundedPanel3.Visible == true)
            {
                clearControls();
                roundedPanel3.Visible = false;
                roundedPanel1.Enabled = true;
                rjButton1.Text = "Add discount";
                rjButton1.BackColor = SystemColors.Highlight;
            }
        }

        private void loadpaymentgroups(string groupId)
        {
            DataTable groupDetails = crudDatabase.FetchDataFromDatabase($"SELECT PaymentgroupID, PaymentGroupName FROM PaymentGroups WHERE PaymentgroupID = '{groupId}'");

            if (groupDetails.Rows.Count > 0)
            {
                roundedTextbox2.Text = groupDetails.Rows[0]["PaymentgroupID"]?.ToString() ?? "N/A";
                roundedTextbox3.Text = groupDetails.Rows[0]["PaymentGroupName"]?.ToString() ?? "N/A";
            }
            else
            {
                roundedTextbox3.Text = "No details found.";
                roundedTextbox2.Text = "No details found.";
            }
        }

        private void InsertPaymentGroup()
        {
            if (string.IsNullOrWhiteSpace(roundedTextbox3.Text))
            {
                label18.Text = "Please enter a GroupName.";
                label18.ForeColor = Color.Tomato;
                return;
            }

            string query = "INSERT INTO PaymentGroups (PaymentGroupName) VALUES (@name)";
           
            var parameters = new Dictionary<string, object>
            {
                { "@name", roundedTextbox3.Text }
            };
         
            if (crudDatabase.ExecuteNonQuery(query, parameters))
            {
                label18.Text = "Payment Group inserted successfully!";
                label18.ForeColor = Color.LawnGreen;
                LoadDiscountsTable();
                clearControls();
                this.Alert("Discount added. Success!", Alertform.enmType.Success);
            }
            else
            {
                label18.Text = "Failed to insert Payment Group.";
                label18.ForeColor = Color.Tomato;
            }
        }


        public void UpdateGroup()
        {
            // Validate GroupName input
            if (string.IsNullOrWhiteSpace(roundedTextbox3.Text))
            {
                label18.Text = "Please enter a discount.";
                label18.ForeColor = Color.Tomato;
                return;
            }

            // Check if any row is selected in the DataGridView
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Get the selected row
                var selectedRow = dataGridView1.SelectedRows[0];
                var selectedId = selectedRow.Cells[0].Value;

                if (selectedId != null && int.TryParse(selectedId.ToString(), out int groupId))
                {
                    // Prepare the update query
                    string updateQuery = "UPDATE PaymentGroups SET PaymentGroupName = @name WHERE PaymentgroupID = @id";
                 
                    var parameters = new Dictionary<string, object>
                    {
                        { "@id", groupId },
                        { "@name", roundedTextbox3.Text }
                    };

                    // Execute the update command using ExecuteNonQuery
                    if (crudDatabase.ExecuteNonQuery(updateQuery, parameters))
                    {
                        label18.Text = "Discount updated successfully!";
                        label18.ForeColor = Color.LawnGreen;
                        LoadDiscountsTable();
                        clearControls();
                        rjButton1.Text = "Add discount";
                        roundedPanel1.Enabled = true;
                        roundedPanel3.Visible = false;
                        rjButton1.BackColor = SystemColors.Highlight;
                        this.Alert("Discount updated successfully!", Alertform.enmType.Success);
                    }
                    else
                    {
                        label18.Text = "Failed to update discount.";
                        label18.ForeColor = Color.Tomato;
                    }
                }
                else
                {
                    label18.Text = "Invalid discount selected.";
                    label18.ForeColor = Color.Tomato;
                }
            }
            else
            {
                label18.Text = "Please select a discount to update.";
                label18.ForeColor = Color.Tomato;
            }
        }


        public void DeleteDiscount()
        {
            // Check if any row is selected in the DataGridView
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Get the selected row
                var selectedRow = dataGridView1.SelectedRows[0];
                var selectedId = selectedRow.Cells[0].Value;

                if (selectedId != null && int.TryParse(selectedId.ToString(), out int groupId))
                {
                    // Prepare the delete query
                    string deleteQuery = "DELETE FROM PaymentGroups WHERE PaymentgroupID = @id";
                   
                    var parameters = new Dictionary<string, object>
                    {
                        { "@id", groupId }
                    };

                    // Execute the delete command using ExecuteNonQuery
                    if (crudDatabase.ExecuteNonQuery(deleteQuery, parameters))
                    {
                        label18.Text = "Discount deleted successfully!";
                        label18.ForeColor = Color.LawnGreen;
                        LoadDiscountsTable();
                        clearControls();
                        rjButton1.Text = "Add discount";
                        roundedPanel1.Enabled = true;
                        roundedPanel3.Visible = false;
                        rjButton1.BackColor = SystemColors.Highlight;
                        this.Alert("Discount deleted successfully!", Alertform.enmType.Success);
                    }
                    else
                    {
                        label18.Text = "Failed to delete discount.";
                        label18.ForeColor = Color.Tomato;
                    }
                }
                else
                {
                    label18.Text = "Invalid discount selected.";
                    label18.ForeColor = Color.Tomato;
                }
            }
            else
            {
                label18.Text = "Please select a discount to delete.";
                label18.ForeColor = Color.Tomato;
            }
        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            if (currentState == "Update")
            {
                UpdateGroup();
            }
            else if (currentState == "Delete")
            {
                using (Deletedishconfirm DDC = new Deletedishconfirm())
                {
                    FormHelper.ShowFormWithOverlay(this.FindForm(), DDC);

                    // Check the DialogResult after the dialog is closed
                    if (DDC.DialogResult == DialogResult.OK)
                    {
                        DeleteDiscount();
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
                InsertPaymentGroup();
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

        public void LocalizeControls()
        {
            //button1.Text = LanguageManager.Instance.GetString("MF-btn1");
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
            //button4.Text = LanguageManager.Instance.GetString("MF-btn4");
            //button5.Text = LanguageManager.Instance.GetString("MF-btn5");
        }

        public void ApplyTheme()
        {
            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            roundedPanel1.BackColor = colors.Color3;
            roundedPanel2.BackColor = colors.Color3;
            roundedPanel3.BackColor = colors.Color3;

            roundedTextbox1.ForeColor = colors.Color2;
            roundedTextbox2.ForeColor = colors.Color2;
            roundedTextbox3.ForeColor = colors.Color2;

            roundedTextbox2.BackColorRounded = colors.Color1;
            roundedTextbox3.BackColorRounded = colors.Color1;

            pictureBox1.Image = colors.Image1;
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
            if (colors.Bool1 && Properties.Settings.Default.showScrollbars) showScrollBars();
        }

        private void showScrollBars()
        {
            dataGridView1.ScrollBars = ScrollBars.Vertical;
        }
    }
}
