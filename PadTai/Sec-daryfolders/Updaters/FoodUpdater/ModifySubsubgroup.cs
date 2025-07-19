using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Text;
using PadTai.Classes;
using System.Drawing;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Data.SqlClient;
using PadTai.Classes.Others;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using PadTai.Classes.Controlsdesign;
using PadTai.Sec_daryfolders.Others;
using PadTai.Sec_daryfolders.Others_Forms;


namespace PadTai.Sec_daryfolders.Updaters.FoodUpdater.Delete
{
    public partial class ModifySubsubgroup : UserControl
    {
        ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();
        private Clavieroverlay clavieroverlay;
        private DataGridViewScroller scroller;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;
        private DataTable dataTable;
        private string currentState;

        public ModifySubsubgroup()
        {
            InitializeComponent();
            InitializeControlResizer();
            crudDatabase = new CrudDatabase();
            dataGridView1.GridColor = colors.Color1;
           
            LoadComboBoxes(customBox1);
            LoadSubgroupsTable();
            LocalizeControls();
            ApplyTheme();
        }


        private void InitializeControlResizer()
        {
            roundedPanel3.Visible = false;

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(label1);
            resizer.RegisterControl(label2);
            resizer.RegisterControl(label3);
            resizer.RegisterControl(label4);
            resizer.RegisterControl(label18);
            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(rjButton2);
            resizer.RegisterControl(customBox1);
            resizer.RegisterControl(pictureBox1);
            resizer.RegisterControl(roundedPanel1);
            resizer.RegisterControl(roundedPanel2);
            resizer.RegisterControl(roundedPanel3);
            resizer.RegisterControl(dataGridView1);
            resizer.RegisterControl(roundedTextbox1);
            resizer.RegisterControl(roundedTextbox2);
            resizer.RegisterControl(roundedTextbox3);
            resizer.RegisterControl(roundedTextbox4);
            scroller = new DataGridViewScroller(this, null, dataGridView1);
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


        public void LoadComboBoxes(ComboBox customBox1)
        {
            try
            {
                crudDatabase.LoadSubgroups();
                customBox1.DataSource = crudDatabase.Subgroups;
                customBox1.DisplayMember = "SubgroupName";
                customBox1.ValueMember = "SubgroupID";
                customBox1.SelectedValue = -1;
                roundedTextbox4.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadSubgroupsTable()
        {
            string query = "SELECT * FROM Subsubgroups";

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
                label18.Text = "No groups found.";
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
                    dataGridView1.Columns[2].HeaderText = "Subroup";
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
                    dataView.RowFilter = $"SubsubgroupName LIKE '%{filterExpression}%'";
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
                        customBox1.Enabled = true;
                        label18.Text = string.Empty;
                        roundedPanel1.Enabled = false;
                        roundedTextbox2.ReadOnly = true;
                        roundedTextbox3.ReadOnly = false;
                        rjButton2.Text = "Update the group";
                        rjButton2.BackColor = Color.DodgerBlue;
                        LoadSubsubgroupDetails(selectedId.ToString());
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
                        customBox1.Enabled = false;
                        label18.Text = string.Empty;
                        roundedPanel1.Enabled = false;
                        roundedTextbox2.ReadOnly = true;
                        roundedTextbox3.ReadOnly = true;
                        rjButton2.BackColor = Color.Crimson;
                        rjButton2.Text = "Delete the group";
                        LoadSubsubgroupDetails(selectedId.ToString());
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
                customBox1.Enabled = true;
                label18.Text = string.Empty;
                roundedPanel3.Visible = true;
                roundedPanel1.Enabled = false;
                rjButton2.Text = "Add a dish";
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
                rjButton1.Text = "Add dish";
                rjButton1.BackColor = SystemColors.Highlight;
            }
        }

        private void LoadSubsubgroupDetails(string subsubgroupId)
        {
            DataTable subsubgroupDetails = crudDatabase.FetchDataFromDatabase($"SELECT SubsubgroupID, SubsubgroupName, SubgroupID FROM Subsubgroups WHERE SubsubgroupID = '{subsubgroupId}'");

            if (subsubgroupDetails.Rows.Count > 0)
            {
                roundedTextbox2.Text = subsubgroupDetails.Rows[0]["SubsubgroupID"]?.ToString() ?? "N/A";
                roundedTextbox3.Text = subsubgroupDetails.Rows[0]["SubsubgroupName"]?.ToString() ?? "N/A";

                int subgroupId = Convert.ToInt32(subsubgroupDetails.Rows[0]["SubgroupID"]);
                customBox1.SelectedValue = subgroupId;
            }
            else
            {
                roundedTextbox2.Text = "No details found.";
                roundedTextbox3.Text = "No details found.";
                customBox1.Text = "No details found.";
            }
        }

        public void InsertSubsubgroup()
        {
            // Validate Subgroup name input
            if (string.IsNullOrWhiteSpace(roundedTextbox3.Text))
            {
                label18.Text = "Please enter a Subsubgroup name.";
                label18.ForeColor = Color.Tomato;
                return;
            }

            if (customBox1.SelectedValue == null)
            {
                label18.Text = "Please select a parent subgroup for your subsubgroup.";
                label18.ForeColor = Color.Tomato;
                return;
            }


            // Get the selected group from the ComboBox
            if (customBox1.SelectedItem is CrudDatabase.Subgroup selectedGroup)
            {
                int groupId = selectedGroup.SubgroupID;

                // Prepare the insert query
                string insertQuery = "INSERT INTO Subsubgroups (SubsubgroupName, SubgroupID) VALUES (@name, @groupId)";

                // Prepare parameters for the insert
                var parameters = new Dictionary<string, object>
                {
                    { "@name", roundedTextbox3.Text },
                    { "@groupId", groupId }
                };

                // Execute the insert command using ExecuteNonQuery
                if (crudDatabase.ExecuteNonQuery(insertQuery, parameters))
                {
                    this.Alert("Subsubgroup added. Success!", Alertform.enmType.Success);
                    label18.Text = "Subsubgroup inserted successfully!";
                    label18.ForeColor = Color.LawnGreen;
                    LoadSubgroupsTable();
                    clearControls();
                }
                else
                {
                    label18.Text = "Failed to insert subgroup.";
                    label18.ForeColor = Color.Tomato;
                }
            }
            else
            {
                label18.Text = "Please select a valid group.";
                label18.ForeColor = Color.Tomato;
            }
        }


        public void UpdateSubsubgroup()
        {
            // Validate Subgroup name input
            if (string.IsNullOrWhiteSpace(roundedTextbox3.Text))
            {
                label18.Text = "Please enter a Subsubgroup name.";
                label18.ForeColor = Color.Tomato;
                return;
            }

            // Check if any row is selected in the DataGridView
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Get the selected row
                var selectedRow = dataGridView1.SelectedRows[0];
                var selectedSubgroupId = selectedRow.Cells[0].Value; // Assuming SubgroupID is in the first cell

                if (selectedSubgroupId != null && int.TryParse(selectedSubgroupId.ToString(), out int subgroupId))
                {
                    // Check if a group is selected in the ComboBox
                    if (customBox1.SelectedItem is CrudDatabase.Subgroup selectedGroup)
                    {
                        int groupId = selectedGroup.SubgroupID; // Get the GroupID from the selected group

                        // Prepare the update query
                        string updateQuery = "UPDATE Subsubgroups SET SubsubgroupName = @name, SubgroupID = @groupId WHERE SubsubgroupID = @id";
                        var parameters = new Dictionary<string, object>
                        {
                            { "@id", subgroupId },
                            { "@name", roundedTextbox3.Text },
                            { "@groupId", groupId }
                        };

                        // Execute the update command using ExecuteNonQuery
                        if (crudDatabase.ExecuteNonQuery(updateQuery, parameters))
                        {
                            label18.Text = "Subsubgroup updated successfully!";
                            label18.ForeColor = Color.LawnGreen;
                            LoadSubgroupsTable();
                            clearControls();
                            rjButton1.Text = "Add dish";
                            roundedPanel1.Enabled = true;
                            roundedPanel3.Visible = false;
                            rjButton1.BackColor = SystemColors.Highlight;
                            this.Alert("Subsubgroup updated successfully!", Alertform.enmType.Success);
                        }
                        else
                        {
                            label18.Text = "Failed to update subgroup.";
                            label18.ForeColor = Color.Tomato;
                        }
                    }
                    else
                    {
                        label18.Text = "Please select a valid group.";
                        label18.ForeColor = Color.Tomato;
                    }
                }
                else
                {
                    label18.Text = "Invalid SubgroupID selected.";
                    label18.ForeColor = Color.Tomato;
                }
            }
            else
            {
                label18.Text = "Please select a subgroup to update.";
                label18.ForeColor = Color.Tomato;
            }
        }


        public void DeleteSubsubgroup()
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
                    string deleteQuery = "DELETE FROM Subsubgroups WHERE SubsubgroupID = @id";
                    var parameters = new Dictionary<string, object>
                    {
                        { "@id", groupId }
                    };

                    // Execute the delete command using ExecuteNonQuery
                    if (crudDatabase.ExecuteNonQuery(deleteQuery, parameters))
                    {
                        label18.Text = "Subsubgroup deleted successfully!";
                        label18.ForeColor = Color.LawnGreen;
                        LoadSubgroupsTable();
                        clearControls();
                        rjButton1.Text = "Add Subgroup";
                        roundedPanel1.Enabled = true;
                        roundedPanel3.Visible = false;
                        rjButton1.BackColor = SystemColors.Highlight;
                        this.Alert("Subgroup deleted successfully!", Alertform.enmType.Success);
                    }
                    else
                    {
                        label18.Text = "Failed to delete group.";
                        label18.ForeColor = Color.Tomato;
                    }
                }
                else
                {
                    label18.Text = "Invalid GroupID selected.";
                    label18.ForeColor = Color.Tomato;
                }
            }
            else
            {
                label18.Text = "Please select a group to delete.";
                label18.ForeColor = Color.Tomato;
            }
        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            if (currentState == "Update")
            {
                UpdateSubsubgroup();
            }
            else if (currentState == "Delete")
            {
                using (Deletedishconfirm DDC = new Deletedishconfirm())
                {
                    FormHelper.ShowFormWithOverlay(this.FindForm(), DDC);

                    // Check the DialogResult after the dialog is closed
                    if (DDC.DialogResult == DialogResult.OK)
                    {
                        DeleteSubsubgroup();
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
                InsertSubsubgroup();
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
            customBox1.Text = string.Empty;
            customBox1.SelectedValue = -1;

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

        private void customBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (customBox1.SelectedItem != null)
            {
                roundedTextbox4.Text = customBox1.Text;
            }
        }

        private void customBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
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
        }

        public void ApplyTheme()
        {
            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            pictureBox1.Image = colors.Image1;
            customBox1.BackColor = colors.Color1;
            customBox1.ForeColor = colors.Color2;
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
            customBox1.ArrowColor = this.ForeColor;
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
