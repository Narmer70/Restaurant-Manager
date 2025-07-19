using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using PadTai.Classes;
using System.Windows.Forms;
using System.ComponentModel;
using PadTai.Classes.Others;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using PadTai.Sec_daryfolders.Others;
using PadTai.Classes.Controlsdesign;
using PadTai.Sec_daryfolders.Others_Forms;
using PadTai.Sec_daryfolders.Updaters.FoodUpdater;


namespace PadTai.Sec_daryfolders.Clientsloyalty
{
    public partial class Modifyclientsdata : UserControl
    {
        ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();
        private DataGridViewScroller scroller;
        private Clavieroverlay clavieroverlay;
        private Random random = new Random();
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;
        private DataTable dataTable;
        private string currentState;

        public Modifyclientsdata()
        {
            InitializeComponent();
            InitializeControlResizer();
            crudDatabase = new CrudDatabase();
            dataGridView1.GridColor = colors.Color1;
            this.DoubleBuffered = true;

            LoadSubgroupsTable();
            LocalizeControls();
            CenterLabel();
            ApplyTheme();

        }

        private void InitializeControlResizer()
        {
            panel1.Visible = false;
            checkBox1.Visible = false;
            roundedPanel3.Visible = false;

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(label1);
            resizer.RegisterControl(label2);
            resizer.RegisterControl(label3);
            resizer.RegisterControl(label4);
            resizer.RegisterControl(label5);
            resizer.RegisterControl(label6);
            resizer.RegisterControl(label7);
            resizer.RegisterControl(label8);
            resizer.RegisterControl(label18);
            resizer.RegisterControl(checkBox1);
            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(rjButton2);
            resizer.RegisterControl(customBox1);
            resizer.RegisterControl(pictureBox1);
            resizer.RegisterControl(pictureBox2);
            resizer.RegisterControl(roundedPanel1);
            resizer.RegisterControl(roundedPanel2);
            resizer.RegisterControl(roundedPanel3);
            resizer.RegisterControl(dataGridView1);
            resizer.RegisterControl(roundedTextbox1);
            resizer.RegisterControl(roundedTextbox2);
            resizer.RegisterControl(roundedTextbox3);
            resizer.RegisterControl(dateTimePicker1);
            resizer.RegisterControl(roundedTextbox5);
            resizer.RegisterControl(roundedTextbox6);
            scroller = new DataGridViewScroller(this, null, dataGridView1);
        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (resizer != null)
            {
                resizer.ResizeControls(this);
                subgroupDGVColumns();
                CenterLabel();
            }
        }

        private void CenterLabel()
        {
            label7.Left = (panel1.ClientSize.Width - label7.Width) / 2;
        }

        public void LoadSubgroupsTable()
        {
            try
            {
                string query = "SELECT * FROM Userdata";

                dataTable = crudDatabase.FetchDataFromDatabase(query);

                if (dataTable.Rows.Count > 0)
                {
                    // Add Update and Delete columns
                    dataTable.Columns.Add("Update", typeof(string));
                    dataTable.Columns.Add("Delete", typeof(string));
                    dataTable.Columns.Add("Loyalty", typeof(string));

                    foreach (DataRow row in dataTable.Rows)
                    {
                        row["Update"] = "✏️";
                        row["Delete"] = "❌";
                        row["Loyalty"] = "⚡";
                    }

                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = dataTable;
                    subgroupDGVColumns();
                }
                else
                {
                    // Handle the case where no data is found
                    label18.Text = "No client found.";
                    label18.ForeColor = Color.Tomato;
                }
            }
            catch
            {
            }
            customBox1.Items.Clear();
            customBox1.Items.Add(LanguageManager.Instance.GetString("GenderF"));
            customBox1.Items.Add(LanguageManager.Instance.GetString("GenderH"));
            customBox1.Items.Add(LanguageManager.Instance.GetString("GenderO"));
            customBox1.SelectedIndex = 0;
        }

        private void subgroupDGVColumns()
        {
            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[0].Width = (int)(dataGridView1.Width * 0.00);
                dataGridView1.Columns[1].Width = (int)(dataGridView1.Width * 0.15);
                dataGridView1.Columns[2].Width = (int)(dataGridView1.Width * 0.14);
                dataGridView1.Columns[3].Width = (int)(dataGridView1.Width * 0.15);
                dataGridView1.Columns[4].Width = (int)(dataGridView1.Width * 0.00);
                dataGridView1.Columns[5].Width = (int)(dataGridView1.Width * 0.00);
                dataGridView1.Columns[6].Width = (int)(dataGridView1.Width * 0.16);
                dataGridView1.Columns[7].Width = (int)(dataGridView1.Width * 0.16);
                dataGridView1.Columns["Update"].Width = (int)(dataGridView1.Width * 0.08);
                dataGridView1.Columns["Delete"].Width = (int)(dataGridView1.Width * 0.08);
                dataGridView1.Columns["Loyalty"].Width = (int)(dataGridView1.Width * 0.08);
                dataGridView1.RowTemplate.Height = 32;

                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[4].Visible = false;
                dataGridView1.Columns[5].Visible = false;
                dataGridView1.Columns[0].HeaderText = "ID";
                dataGridView1.Columns[1].HeaderText = "Clientname";
                dataGridView1.Columns[2].HeaderText = "2ndname";
                dataGridView1.Columns[3].HeaderText = "Number";
                dataGridView1.Columns[4].HeaderText = "Email";
                dataGridView1.Columns[5].HeaderText = "Picture";
                dataGridView1.Columns[6].HeaderText = "Gender";
                dataGridView1.Columns[7].HeaderText = "Birthday";
                dataGridView1.Columns["Update"].HeaderText = "Update";
                dataGridView1.Columns["Delete"].HeaderText = "Delete";
                dataGridView1.Columns["Loyalty"].HeaderText = "Loyalty";

                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    dataGridView1.ColumnHeadersHeight = 35;
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
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                e.CellStyle.Font = new Font("Century Gothic", 12, FontStyle.Bold);
            }

            if (dataGridView1.Columns[e.ColumnIndex].Name == "Delete")
            {
                e.CellStyle.ForeColor = Color.Tomato;
                e.CellStyle.SelectionForeColor = Color.Orange;
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                e.CellStyle.Font = new Font("Century Gothic", 12, FontStyle.Bold);
            }

            if (dataGridView1.Columns[e.ColumnIndex].Name == "Loyalty")
            {
                e.CellStyle.ForeColor = Color.Green;
                e.CellStyle.SelectionForeColor = Color.LawnGreen;
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
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
                    dataView.RowFilter = $"Username LIKE '%{filterExpression}%' OR Secondname LIKE '%{filterExpression}%' OR Phonenumber LIKE '%{filterExpression}%'";
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
                        dateTimePicker1.Enabled = true;
                        roundedTextbox2.ReadOnly = false;
                        roundedTextbox3.ReadOnly = false;
                        rjButton2.Text = "Update the data";
                        rjButton2.BackColor = Color.DodgerBlue;
                        LoadSubsubgroupDetails(selectedId.ToString());
                        rjButton1.BackColor = roundedPanel1.BackColor;
                        roundedTextbox5.WaterMark = "Optional";
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
                        dateTimePicker1.Enabled = false;
                        rjButton2.BackColor = Color.Crimson;
                        rjButton2.Text = "Delete the data";
                        LoadSubsubgroupDetails(selectedId.ToString());
                        rjButton1.BackColor = roundedPanel1.BackColor;
                        roundedTextbox5.WaterMark = "Optional";
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
                dateTimePicker1.Enabled= true;
                rjButton2.Text = "Add a client";
                roundedTextbox2.ReadOnly = false;
                roundedTextbox3.ReadOnly = false;
                rjButton2.BackColor = Color.Green;
                rjButton1.BackColor = roundedPanel1.BackColor;
                roundedTextbox5.WaterMark = "Optional";
            }
            else if (roundedPanel3.Visible == true)
            {
                clearControls();
                roundedPanel3.Visible = false;
                roundedPanel1.Enabled = true;
                rjButton1.Text = "Add client";
                rjButton1.BackColor = SystemColors.Highlight;
            }
        }

        private void LoadSubsubgroupDetails(string userId)
        {
            try
            {
                DataTable subsubgroupDetails = crudDatabase.FetchDataFromDatabase($"SELECT UserID, Username, Secondname, Phonenumber, Email, Profilepicture, Gender,  Birthday FROM Userdata WHERE UserID = '{userId}'");

                if (subsubgroupDetails.Rows.Count > 0)
                {
                    customBox1.Text = subsubgroupDetails.Rows[0]["Gender"]?.ToString() ?? "N/A";
                    roundedTextbox5.Text = subsubgroupDetails.Rows[0]["Email"]?.ToString() ?? "N/A";
                    dateTimePicker1.Text = subsubgroupDetails.Rows[0]["Birthday"]?.ToString() ?? "N/A";
                    roundedTextbox2.Text = subsubgroupDetails.Rows[0]["Username"]?.ToString() ?? "N/A";
                    roundedTextbox6.Text = subsubgroupDetails.Rows[0]["Secondname"]?.ToString() ?? "N/A";
                    roundedTextbox3.Text = subsubgroupDetails.Rows[0]["Phonenumber"]?.ToString() ?? "N/A";

                    byte[] imageData = subsubgroupDetails.Rows[0]["Profilepicture"] as byte[];

                    if (imageData != null)
                    {
                        using (var ms = new MemoryStream(imageData))
                        {
                            pictureBox2.Image = Image.FromStream(ms);
                        }
                    }
                    else
                    {
                        if (currentState != null && currentState == "Delete")
                        {
                            pictureBox2.Image = Properties.Resources.image_add;
                        }
                        else if (currentState != null && currentState == "Update")
                        {
                            pictureBox2.Image = Properties.Resources.image_add__1_;
                            checkBox1.Visible = false;
                        }
                    }
                }
                else
                {
                    roundedTextbox2.Text = "No details found.";
                    roundedTextbox3.Text = "No details found.";
                    roundedTextbox5.Text = "No details found.";
                    roundedTextbox6.Text = "No details found.";                    
                    customBox1.Text = "No details found.";
                }
            }
            catch 
            {
            }
        }

        public void InsertSubsubgroup()
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(roundedTextbox2.Text))
                {
                    label18.Text = "Please enter the Username";
                    label18.ForeColor = Color.Tomato;
                    return;
                }
                if (string.IsNullOrWhiteSpace(roundedTextbox3.Text))
                {
                    label18.Text = "Please enter the contact";
                    label18.ForeColor = Color.Tomato;
                    return;
                }
                if (string.IsNullOrWhiteSpace(customBox1.Text))
                {
                    label18.Text = "Please select a gender";
                    label18.ForeColor = Color.Tomato;
                    return;
                }

                // Prepare optional fields
                string userName = string.IsNullOrWhiteSpace(roundedTextbox2.Text) ? null : roundedTextbox2.Text;
                string secondName = string.IsNullOrWhiteSpace(roundedTextbox6.Text) ? null : roundedTextbox6.Text;
                string phoneNumber = string.IsNullOrWhiteSpace(roundedTextbox3.Text) ? null : roundedTextbox3.Text;
                string email = string.IsNullOrWhiteSpace(roundedTextbox5.Text) ? null : roundedTextbox5.Text;
                string gender = string.IsNullOrWhiteSpace(customBox1.Text) ? null : customBox1.Text;
                DateTime? birthday = dateTimePicker1.Value == DateTime.MinValue ? (DateTime?)null : dateTimePicker1.Value;

                byte[] profilePicture = null;

                if (pictureBox2.Image != null && !crudDatabase.IsSpecificImage(pictureBox2.Image, Properties.Resources.image_add_13434878))
                {
                    using (var ms = new MemoryStream())
                    {
                        pictureBox2.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        profilePicture = ms.ToArray();
                    }
                }

                // Prepare the insert query
                string insertQuery = "INSERT INTO Userdata (Username, Secondname, Phonenumber, Email, Profilepicture, Gender, Birthday) " +
                                     "VALUES (@Username, @Secondname, @Phonenumber, @Email, @Profilepicture, @Gender, @Birthday)";

                // Prepare parameters for the insert
                var parameters = new Dictionary<string, object>
                {
                    { "@Username", userName },
                    { "@Secondname", secondName ?? (object)DBNull.Value },
                    { "@Phonenumber", phoneNumber ?? (object)DBNull.Value },
                    { "@Email", email ?? (object)DBNull.Value },
                    { "@Profilepicture", profilePicture ?? (object)DBNull.Value },
                    { "@Gender", gender ?? (object)DBNull.Value },
                    { "@Birthday", birthday.HasValue ? (object)birthday.Value : DBNull.Value }
                };

                // Execute the insert command using ExecuteNonQuery
                if (crudDatabase.ExecuteNonQuery(insertQuery, parameters))
                {
                    clearControls();
                    LoadSubgroupsTable();
                    label18.ForeColor = Color.LawnGreen;
                    label18.Text = "User inserted successfully!";
                    this.Alert("User added successfully!", Alertform.enmType.Success);
                }
                else
                {
                    label18.Text = "Failed to insert user.";
                    label18.ForeColor = Color.Tomato;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error inserting user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public void UpdateUserData()
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(roundedTextbox2.Text))
                {
                    label18.Text = "Please enter the Username";
                    label18.ForeColor = Color.Tomato;
                    return;
                }
                if (string.IsNullOrWhiteSpace(roundedTextbox3.Text))
                {
                    label18.Text = "Please enter the contact";
                    label18.ForeColor = Color.Tomato;
                    return;
                }
                if (string.IsNullOrWhiteSpace(customBox1.Text))
                {
                    label18.Text = "Please select a gender";
                    label18.ForeColor = Color.Tomato;
                    return;
                }

                // Check if any row is selected in the DataGridView
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    // Get the selected row
                    var selectedRow = dataGridView1.SelectedRows[0];
                    var selectedUserId = selectedRow.Cells[0].Value; // Assuming UserID is in the first cell

                    if (selectedUserId != null && int.TryParse(selectedUserId.ToString(), out int userId))
                    {
                        // Prepare optional fields
                        string userName = string.IsNullOrWhiteSpace(roundedTextbox2.Text) ? null : roundedTextbox2.Text;
                        string secondName = string.IsNullOrWhiteSpace(roundedTextbox6.Text) ? null : roundedTextbox6.Text;
                        string phoneNumber = string.IsNullOrWhiteSpace(roundedTextbox3.Text) ? null : roundedTextbox3.Text;
                        string email = string.IsNullOrWhiteSpace(roundedTextbox5.Text) ? null : roundedTextbox5.Text;
                        string gender = string.IsNullOrWhiteSpace(customBox1.Text) ? null : customBox1.Text;
                        DateTime? birthday = dateTimePicker1.Value == DateTime.MinValue ? (DateTime?)null : dateTimePicker1.Value;

                        byte[] profilePicture = null;

                        if (pictureBox2.Image != null && !crudDatabase.IsSpecificImage(pictureBox2.Image, Properties.Resources.image_add__1_))
                        {
                            using (var ms = new MemoryStream())
                            {
                                pictureBox2.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                profilePicture = ms.ToArray();
                            }
                        }

                        // Prepare the update query
                        string updateQuery = "UPDATE Userdata SET Username = @Username, Secondname = @Secondname, Phonenumber = @Phonenumber, " +
                                             "Email = @Email, Profilepicture = @Profilepicture, Gender = @Gender, Birthday = @Birthday " +
                                             "WHERE UserID = @UserID";

                        // Prepare parameters for the update
                        var parameters = new Dictionary<string, object>
                        {
                            { "@UserID", userId },
                            { "@Username", userName},
                            { "@Secondname", secondName ?? (object)DBNull.Value },
                            { "@Phonenumber", phoneNumber ?? (object)DBNull.Value },
                            { "@Email", email ?? (object)DBNull.Value },
                            { "@Profilepicture", profilePicture ?? (object)DBNull.Value },
                            { "@Gender", gender ?? (object)DBNull.Value },
                            { "@Birthday", birthday.HasValue ? (object)birthday.Value : DBNull.Value }
                        };

                        // Execute the update command using ExecuteNonQuery
                        if (crudDatabase.ExecuteNonQuery(updateQuery, parameters))
                        {
                            clearControls(); 
                            LoadSubgroupsTable();
                            roundedPanel1.Enabled = true;
                            roundedPanel3.Visible = false;
                            rjButton1.Text = "Add client";
                            label18.ForeColor = Color.LawnGreen;
                            label18.Text = "User  updated successfully!";
                            rjButton1.BackColor = SystemColors.Highlight;
                            this.Alert("User updated. Success!", Alertform.enmType.Success);
                        }
                        else
                        {
                            label18.Text = "Failed to update user.";
                            label18.ForeColor = Color.Tomato;
                        }
                    }
                    else
                    {
                        label18.Text = "Invalid UserID selected.";
                        label18.ForeColor = Color.Tomato;
                    }
                }
                else
                {
                    label18.Text = "Please select a user to update.";
                    label18.ForeColor = Color.Tomato;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void DeleteSubsubgroup()
        {
            try
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
                        string deleteQuery = "DELETE FROM Userdata WHERE UserID = @id";
                        var parameters = new Dictionary<string, object>
                        {
                            { "@id", groupId }
                        };

                        // Execute the delete command using ExecuteNonQuery
                        if (crudDatabase.ExecuteNonQuery(deleteQuery, parameters))
                        {
                            label18.Text = "Client deleted successfully!";
                            label18.ForeColor = Color.LawnGreen;
                            LoadSubgroupsTable();
                            clearControls();
                            rjButton1.Text = "Add client";
                            roundedPanel1.Enabled = true;
                            roundedPanel3.Visible = false;
                            rjButton1.BackColor = SystemColors.Highlight;
                            this.Alert("Client deleted successfully!", Alertform.enmType.Success);
                        }
                        else
                        {
                            label18.Text = "Failed to delete client.";
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
            catch
            {
            }
        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            if (currentState == "Update")
            {
                UpdateUserData();
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
            roundedTextbox5.Text = string.Empty;
            roundedTextbox6.Text = string.Empty;
            customBox1.Text = string.Empty;
            customBox1.SelectedValue = -1;

            if (currentState == "Add")
            {
                roundedTextbox5.WaterMark = "Optional";
            }
        }

        private void Alert(string msg, Alertform.enmType type)
        {
            Alertform alertform = new Alertform();
            alertform.showAlert(msg, type);
        }

        private void Modifyclientsdata_Load(object sender, EventArgs e)
        {
            int randomValue = random.Next(0, 8);

            if (randomValue == 1)
            {
                panel1.Visible = true;
            }
            else
            {
                panel1.Visible = false;
            }
        }

        private void customBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (customBox1.SelectedItem != null)
            {
              // roundedTextbox4.Text = customBox1.Text;
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

            label7.ForeColor = colors.Color2;
            panel1.BackColor = colors.Color3;
            pictureBox1.Image = colors.Image1;
            customBox1.BackColor = colors.Color1;
            customBox1.ForeColor = colors.Color2;
            roundedPanel1.BackColor = colors.Color3;
            roundedPanel2.BackColor = colors.Color3;
            roundedPanel3.BackColor = colors.Color3;
            roundedTextbox1.ForeColor = colors.Color2;
            roundedTextbox2.ForeColor = colors.Color2;
            roundedTextbox3.ForeColor = colors.Color2;
            roundedTextbox5.ForeColor = colors.Color2;
            roundedTextbox6.ForeColor = colors.Color2;
            dateTimePicker1.BorderColor = colors.Color4;
            dateTimePicker1.CalendarIcon = colors.Image2;
            dateTimePicker1.BackColorCustom = colors.Color1;
            dateTimePicker1.BorderFocusColor = colors.Color5;
            roundedTextbox2.BackColorRounded = colors.Color1;
            roundedTextbox3.BackColorRounded = colors.Color1;
            roundedTextbox5.BackColorRounded = colors.Color1;
            roundedTextbox6.BackColorRounded = colors.Color1;

            rjButton1.ForeColor = this.ForeColor;
            customBox1.ArrowColor = this.ForeColor;
            dataGridView1.ForeColor = this.ForeColor;
            dateTimePicker1.TextColor = this.ForeColor;
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
