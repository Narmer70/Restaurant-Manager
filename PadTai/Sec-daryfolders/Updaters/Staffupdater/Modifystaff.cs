using System;
using System.IO;
using System.Linq;
using System.Data;
using System.Drawing;
using PadTai.Classes;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Data.SqlClient;
using PadTai.Classes.Others;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using PadTai.Sec_daryfolders.Others;
using PadTai.Classes.Controlsdesign;
using PadTai.Sec_daryfolders.Others_Forms;
using PadTai.Sec_daryfolders.Updaters.FoodUpdater;


namespace PadTai.Sec_daryfolders.Staffmanager
{
    public partial class Modifystaff : UserControl
    {
        ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();
        private string Oldemployeename = null;
        private DataGridViewScroller scroller;
        private Clavieroverlay clavieroverlay;
        private string Oldpassword = null;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;
        private DataTable dataTable;
        private string currentState;


        public Modifystaff()
        {
            InitializeComponent();
            initialiseControlResizing();
            crudDatabase = new CrudDatabase();
            dataGridView1.GridColor = colors.Color1;

            LoadPersonalTypes();
            LocalizeControls();
            LoadGenders();
            ApplyTheme();
            Fetchdata();
        }


        private void initialiseControlResizing()
        {
            roundedPanel1.Visible = false;
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
            resizer.RegisterControl(label9);
            resizer.RegisterControl(label10);
            resizer.RegisterControl(label15);
            resizer.RegisterControl(button1);
            resizer.RegisterControl(textBox1);
            resizer.RegisterControl(textBox2);
            resizer.RegisterControl(textBox5);
            resizer.RegisterControl(textBox7);
            resizer.RegisterControl(checkBox1);
            resizer.RegisterControl(comboBox3);
            resizer.RegisterControl(comboBox2);
            resizer.RegisterControl(comboBox1);
            resizer.RegisterControl(rjButton2);
            resizer.RegisterControl(customBox1);
            resizer.RegisterControl(customBox2);         
            resizer.RegisterControl(pictureBox1);
            resizer.RegisterControl(pictureBox2);
            resizer.RegisterControl(dataGridView1);
            resizer.RegisterControl(roundedPanel1);
            resizer.RegisterControl(roundedPanel2);
            resizer.RegisterControl(roundedPanel3);
            resizer.RegisterControl(dateTimePicker1);
            scroller = new DataGridViewScroller(this, null, dataGridView1);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (resizer != null)
            {
                resizer.ResizeControls(this);
                setColumnWidth();
            }
        }

        private void Alert(string msg, Alertform.enmType type)
        {
            Alertform alertform = new Alertform();
            alertform.showAlert(msg, type);
        }


        private void LoadComboBoxData(string query, ComboBox comboBox, string defaultItem)
        {
            DataTable dataTable = crudDatabase.FetchDataFromDatabase(query);

            comboBox.Items.Clear(); // Clear existing items
            comboBox.Items.Add(defaultItem); // Add the default item at the first position

            foreach (DataRow row in dataTable.Rows)
            {
                comboBox.Items.Add(row[0].ToString());
            }

            comboBox.SelectedIndex = 0; // Set the default item as selected
        }

        private void LoadGenders()
        {
            string query = "SELECT DISTINCT Gender FROM Employees";
            LoadComboBoxData(query, customBox1, LanguageManager.Instance.GetString("Filterbygender"));
            LoadComboBoxData(query, comboBox2, " -");
            comboBox2.SelectedIndex = -1;
        }

        private void LoadPersonalTypes()
        {
            string query = "SELECT DISTINCT PersonalType FROM Employees";
            LoadComboBoxData(query, customBox2, LanguageManager.Instance.GetString("Filterbyjob"));
            LoadComboBoxData(query, comboBox1, " -");
            comboBox1.SelectedIndex = -1;
        }

        private void FilterEmployees()
        {
            string filter = "";

            // Get the search text
            string searchText = roundedTextbox1.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                filter += $"(Name LIKE '%{searchText}%' OR Password LIKE '%{searchText}%' OR Contact LIKE '%{searchText}%')";
            }

            // Get the selected gender
            string selectedGender = customBox1.SelectedItem?.ToString();
            if (selectedGender != null && selectedGender != LanguageManager.Instance.GetString("Filterbygender")) // Check for default item
            {
                if (!string.IsNullOrEmpty(filter)) filter += " AND ";
                filter += $"Gender = '{selectedGender}'";
            }

            // Get the selected personal type
            string selectedPersonalType = customBox2.SelectedItem?.ToString();
            if (selectedPersonalType != null && selectedPersonalType != LanguageManager.Instance.GetString("Filterbyjob")) // Check for default item
            {
                if (!string.IsNullOrEmpty(filter)) filter += " AND ";
                filter += $"PersonalType = '{selectedPersonalType}'";
            }

            // Apply the filter to the DataView
            DataView dv = new DataView(dataTable);
            dv.RowFilter = filter;

            // Bind the filtered DataView to the DataGridView
            dataGridView1.DataSource = dv;
        }

        private void Fetchdata()
        {
            var clients = crudDatabase.LoadClientIDs();
           
            comboBox3.DataSource = crudDatabase.Clients;
            comboBox3.DisplayMember = "ClientName";
            comboBox3.ValueMember = "ClientID";
            comboBox3.SelectedIndex = -1;

            string query = "SELECT ID, Name, PersonalType, Gender, Password, Contact, Salary, ClientID FROM Employees";

            dataTable = crudDatabase.FetchDataFromDatabase(query);

            if (dataTable.Rows.Count > 0)
            {
                dataTable.Columns.Add("ClientName", typeof(string));
             
                foreach (DataRow row in dataTable.Rows)
                {
                    if (row["ClientID"] is long clientId)
                    {
                        if (clients.TryGetValue((int)clientId, out string clientName))
                        {
                            row["ClientName"] = clientName;
                        }
                        else
                        {
                            row["ClientName"] = "Unknown";
                        }
                    }
                    else
                    {
                        //  row["ClientName"] = "Invalid"; 
                    }
                }

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
        
                dataGridView1.Columns["ID"].Visible = false;    
                dataGridView1.Columns["ClientID"].Visible = false;    

                if (dataGridView1.Columns.Count > 0)
                {
                    setColumnWidth();
                    RenameDataGridViewColumns();
                }
            }
            else
            {
                // Handle the case where no data is found
                label10.Text = "No groups found.";
                label10.ForeColor = Color.Tomato;
            }
        }

        private void setColumnWidth()
        {
            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.RowTemplate.Height = 35;
                dataGridView1.Columns[1].Width = (int)(dataGridView1.Width * 0.20);
                dataGridView1.Columns[2].Width = (int)(dataGridView1.Width * 0.12);
                dataGridView1.Columns[3].Width = (int)(dataGridView1.Width * 0.10);
                dataGridView1.Columns[4].Width = (int)(dataGridView1.Width * 0.10);
                dataGridView1.Columns[5].Width = (int)(dataGridView1.Width * 0.14);
                dataGridView1.Columns[6].Width = (int)(dataGridView1.Width * 0.08);
                dataGridView1.Columns["Update"].Width = (int)(dataGridView1.Width * 0.08);
                dataGridView1.Columns["Delete"].Width = (int)(dataGridView1.Width * 0.08);
                dataGridView1.Columns[1].DefaultCellStyle.Padding = new Padding(20, 0, 0, 0);
                dataGridView1.Columns["ClientName"].Width = (int)(dataGridView1.Width * 0.10);
                dataGridView1.Columns["Update"].DefaultCellStyle.Padding = new Padding(2, 2, 2, 2);
                dataGridView1.Columns["Delete"].DefaultCellStyle.Padding = new Padding(2, 2, 2, 2);
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["Update"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["Delete"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["ClientName"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void RenameDataGridViewColumns()
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            if (dataGridView1.Columns.Count > 0)
            {
                dataGridView1.Columns[0].HeaderText = "ID";
                dataGridView1.Columns[1].HeaderText = "Имя";
                dataGridView1.Columns[2].HeaderText = "Должность";
                dataGridView1.Columns[3].HeaderText = "Пол";
                dataGridView1.Columns[4].HeaderText = "Password";
                dataGridView1.Columns[5].HeaderText = "Телефон";
                dataGridView1.Columns[6].HeaderText = "Salary/h";
                dataGridView1.Columns["Update"].HeaderText = "Update";
                dataGridView1.Columns["Delete"].HeaderText = "Delete";
                dataGridView1.Columns["ClientName"].HeaderText = "Отдел";

                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    dataGridView1.ColumnHeadersHeight = 40;
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
                        button1.Text = "Back";
                        checkBox1.Visible = true;
                        comboBox1.Enabled = true;
                        comboBox2.Enabled = true;
                        comboBox3.Enabled = true;
                        checkBox1.Checked = false;
                        customBox1.Visible = false;
                        customBox2.Visible = false;
                        roundedPanel3.Enabled = false;
                        rjButton2.Text = "Update data";
                        rjButton2.BackColor = Color.DodgerBlue;
                        button1.BackColor = roundedPanel1.BackColor;
                        SearchEmployeeById(selectedId.ToString());
                        roundedPanel1.Visible = true;
                    }
                }
                else if (e.ColumnIndex == dataGridView1.Columns["Delete"].Index)
                {
                    var selectedId = dataGridView1.Rows[e.RowIndex].Cells[0].Value;

                    if (selectedId != null)
                    {
                        currentState = "Delete";
                        button1.Text = "Back";
                        checkBox1.Visible = false;
                        checkBox1.Checked = false;
                        customBox1.Visible = false;
                        customBox2.Visible = false;
                        roundedPanel3.Enabled = false;
                        rjButton2.BackColor = Color.Crimson;
                        rjButton2.Text = "Delete employee";
                        button1.BackColor = roundedPanel1.BackColor;
                        SearchEmployeeById(selectedId.ToString());
                        roundedPanel1.Visible = true;
                    }
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (roundedPanel1.Visible == false)
            {
                currentState = "Add";
                button1.Text = "Back";
                checkBox1.Visible = false;
                customBox1.Visible = false;
                customBox2.Visible = false;
                roundedPanel1.Visible = true;
                roundedPanel3.Enabled = false;
                rjButton2.Text = "Add Employee";
                rjButton2.BackColor = Color.Green;
                button1.BackColor = roundedPanel1.BackColor;
                pictureBox1.Image = Properties.Resources.follower__1_;
            }
            else if (roundedPanel1.Visible == true)
            {
                ClearFields();
                roundedPanel1.Visible = false;
                roundedPanel3.Enabled = true;
                customBox1.Visible = true;
                customBox2.Visible = true;
                button1.Text = "Add dish";
                button1.BackColor = SystemColors.Highlight;
            }
        }


        private void rjButton2_Click(object sender, EventArgs e)
        {
            if (currentState == "Update")
            {
                UpdateEmployee();
            }
            else if (currentState == "Delete")
            {
                using (Deletedishconfirm DDC = new Deletedishconfirm())
                {
                    FormHelper.ShowFormWithOverlay(this.FindForm(), DDC);

                    // Check the DialogResult after the dialog is closed
                    if (DDC.DialogResult == DialogResult.OK)
                    {
                        DeleteSelectedEmployee();
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
                initialiseInsert();
            }
            else
            {
                roundedPanel1.Visible = false;
            }
        }


        private void SearchEmployeeById(string id)
        {
            string query = "SELECT Name, PersonalType, Gender, Contact, Password, DateOfStarting, Salary, ClientID, Picture FROM Employees WHERE ID = @ID";

            // Create a parameterized query to prevent SQL injection
            var parameters = new Dictionary<string, object>
            {
               { "@ID", id }
            };

            // Fetch data from the database
            DataTable employeeData = crudDatabase.FetchDataFromDatabase(query, parameters);

            if (employeeData.Rows.Count > 0)
            {
                DataRow row = employeeData.Rows[0];

                // Clear previous values
                ClearFields();

                // Populate fields with data from the DataRow
                if (row["Name"] != DBNull.Value)
                {
                    Oldemployeename = row["Name"].ToString();
                }

                textBox1.Text = row["Name"].ToString();         
                
                textBox2.Text = row["Salary"].ToString();
                comboBox2.Text = row["Gender"].ToString();
                textBox5.Text = row["Contact"].ToString();

                if (row["Password"] != DBNull.Value)
                {
                    Oldpassword = row["Password"].ToString();
                }

                textBox7.Text = row["Password"].ToString();
                comboBox1.Text = row["PersonalType"].ToString();
                dateTimePicker1.Text = DateTime.Parse(row["DateOfStarting"].ToString()).ToString("yyyy-MM-dd");

                int clientId = Convert.ToInt32(employeeData.Rows[0]["ClientID"]);
                comboBox3.SelectedValue = clientId;

                // Load the image into the PictureBox
                if (row["Picture"] != DBNull.Value)
                {
                    byte[] imageBytes = (byte[])row["Picture"];
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        pictureBox1.Image = Image.FromStream(ms);
                    }
                }
                else
                {
                    if (currentState != null && currentState == "Delete")
                    {
                        pictureBox1.Image = Properties.Resources.follower__2_;
                    }
                    else if (currentState != null && currentState == "Update")
                    {
                        pictureBox1.Image = Properties.Resources.follower;
                        checkBox1.Visible = false;
                    }
                }
            }
            else
            {
                label10.Text = "No employee found with the provided ID.";
                label10.ForeColor = Color.Tomato;
            }
        }


        private void InsertEmployee(decimal? salary, string personalType, string gender, string contact, string name, string password, DateTime? dateOfStarting, byte[] picture, int? clientId)
        {
            try
            {
                string query = "INSERT INTO Employees (Salary, PersonalType, Gender, Contact, Name, Password, DateOfStarting, Picture, ClientID) " +
                               "VALUES (@Salary, @PersonalType, @Gender, @Contact, @Name, @Password, @DateOfStarting, @Picture, @ClientID)";

                var parameters = new Dictionary<string, object>
                {
                     { "@Salary", (object)salary ?? DBNull.Value },
                     { "@PersonalType", string.IsNullOrEmpty(personalType) ? DBNull.Value : (object)personalType },
                     { "@Gender", string.IsNullOrEmpty(gender) ? DBNull.Value : (object)gender },
                     { "@Contact", string.IsNullOrEmpty(contact) ? DBNull.Value : (object)contact },
                     { "@Name", string.IsNullOrEmpty(name) ? DBNull.Value : (object)name },
                     { "@Password", string.IsNullOrEmpty(password) ? DBNull.Value : (object)password },
                     { "@DateOfStarting", dateOfStarting ?? (object)DBNull.Value },
                     { "@Picture", picture != null && picture.Length > 0 ? (object)picture : DBNull.Value },
                     { "@ClientID", (object)clientId ?? DBNull.Value }
                };

                if (crudDatabase.ExecuteNonQuery(query, parameters))
                {
                    ClearFields();
                    Fetchdata();
                    customBox1.Visible = true;
                    customBox2.Visible = true;
                    label10.Text = string.Empty;
                    label10.ForeColor = Color.LawnGreen;
                    label10.Text = "Employee inserted successfully.";
                    this.Alert("Success!!", Alertform.enmType.Success);
                }
                else
                {
                    label10.Text = "Error inserting employee.";
                    label10.ForeColor = Color.Red;
                }
            }
            catch 
            {
                label10.Text = "Error inserting employee.";
                label10.ForeColor = Color.Red;
            }
        }


        private void initialiseInsert()
        {
            // Clear previous messages
            label10.Text = string.Empty;

            // Gather data from input controls
            decimal? salary = null; // Initialize salary to null
            string personalType = comboBox1.Text;
            string gender = comboBox2.Text;
            string contact = textBox5.Text;
            string password = null;
            string name = null;
            DateTime? dateOfStarting = dateTimePicker1.Value;

            byte[] picture = null;

            // Convert the image from PictureBox to byte array
            if (pictureBox1.Image != null && !crudDatabase.IsSpecificImage(pictureBox1.Image, Properties.Resources.follower__1_))
            {
                using (var ms = new MemoryStream())
                {
                    pictureBox1.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    picture = ms.ToArray();
                }
            }

            int? clientId = null;

            // Validate name input
            if (string.IsNullOrEmpty(textBox1.Text.ToString().Trim()))
            {
                label10.Text += "Name is required. ";
                label10.ForeColor = Color.Tomato;
                return; // Return immediately after adding the error
            }

            // Check if the name is already taken
            name = textBox1.Text.ToString().Trim();
            if (crudDatabase.CheckIfElementExists("Employees", "Name", name))
            {
                label10.Text += "This name already exists. ";
                label10.ForeColor = Color.Tomato;
                return; // Return immediately after adding the error
            }

            // Validate salary input
            if (!string.IsNullOrEmpty(textBox2.Text))
            {
                if (!decimal.TryParse(textBox2.Text, out decimal parsedSalary))
                {
                    label10.Text += "Salary must be a valid number.";
                    label10.ForeColor = Color.Tomato;
                    return; // Return immediately after adding the error
                }
                salary = parsedSalary; // Assign parsed salary if valid
            }

            // Validate password input
            if (!string.IsNullOrEmpty(textBox7.Text.Trim()))
            {
                if (crudDatabase.CheckIfElementExists("Employees", "Password", textBox7.Text.Trim()))
                {
                    label10.Text += "This password already exists. ";
                    label10.ForeColor = Color.Tomato;
                    return; // Return immediately after adding the error
                }
                password = textBox7.Text.Trim();
            }

            if (string.IsNullOrEmpty(contact))
            {
                label10.Text += "Contact is required. ";
                label10.ForeColor = Color.Tomato;
                return; // Return immediately after adding the error
            }

            // Validate other fields
            if (string.IsNullOrEmpty(personalType))
            {
                label10.Text += "Personal Type is required. ";
                label10.ForeColor = Color.Tomato;
                return; // Return immediately after adding the error
            }

            if (string.IsNullOrEmpty(gender))
            {
                label10.Text += "Gender is required. ";
                label10.ForeColor = Color.Tomato;
                return; // Return immediately after adding the error
            }

            if (comboBox3.SelectedItem == null)
            {
                label10.Text += "Client ID is required. ";
                label10.ForeColor = Color.Tomato;
                return; // Return immediately after adding the error
            }
            else
            {
                clientId = (int)((CrudDatabase.Client)comboBox3.SelectedItem).ClientID;
            }

            if (dateOfStarting == null)
            {
                label10.Text += "Date of Starting is required. ";
                label10.ForeColor = Color.Tomato;
                return; // Return immediately after adding the error
            }

            // If all validations pass, proceed to insert the employee
            InsertEmployee(salary, personalType, gender, contact, name, password, dateOfStarting, picture, clientId);
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (currentState != "Delete")
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "Image Files (*.jpg, *.jpeg, *.png, *.gif)|*.jpg;*.jpeg;*.png;*.gif";
                openFileDialog1.Title = "Select an Image";

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    // Get the image file path
                    string imagePath = openFileDialog1.FileName;

                    // Load the image into the PictureBox
                    pictureBox1.Image = Image.FromFile(imagePath);
                }
            }
        }



        private void UpdateEmployee()
        {
            label10.Text = string.Empty;

            string name = null;
            decimal? salary = null;
            string password = null;
            string personalType = comboBox1.Text;
            string gender = comboBox2.Text.Trim();
            string contact = textBox5.Text.Trim();
            DateTime? dateOfStarting = dateTimePicker1.Value;

            int? clientId = null;

            // Validate name input
            if (string.IsNullOrEmpty(textBox1.Text.Trim()))
            {
                label10.Text += "Name is required. ";
                label10.ForeColor = Color.Tomato;
                return; // Return immediately after adding the error
            }

            // Check if the name is already taken or if it's the same as the old name
            name = textBox1.Text.Trim();
            if (!crudDatabase.CheckIfElementExists("Employees", "Name", name) ||
                (name == Oldemployeename && Oldemployeename != null))
            {
                // Valid name
            }
            else
            {
                label10.Text += "This name already exists. ";
                label10.ForeColor = Color.Tomato;
                return; // Return immediately after adding the error
            }

            // Validate salary input
            if (!string.IsNullOrEmpty(textBox2.Text))
            {
                if (!decimal.TryParse(textBox2.Text, out decimal parsedSalary))
                {
                    label10.Text += "Salary must be a valid number.";
                    label10.ForeColor = Color.Tomato;
                    return; // Return immediately after adding the error
                }
                salary = parsedSalary; // Assign the parsed value to the nullable salary
            }
            else
            {
                salary = null; // If the text box is empty, set salary to null
            }

            // Validate contact
            if (string.IsNullOrEmpty(contact))
            {
                label10.Text += "Contact is required. ";
                label10.ForeColor = Color.Tomato;
                return; // Return immediately after adding the error
            }

            // Validate personal type
            if (string.IsNullOrEmpty(personalType))
            {
                label10.Text += "Personal Type is required. ";
                label10.ForeColor = Color.Tomato;
                return; // Return immediately after adding the error
            }

            // Validate gender
            if (string.IsNullOrEmpty(gender))
            {
                label10.Text += "Gender is required. ";
                label10.ForeColor = Color.Tomato;
                return; // Return immediately after adding the error
            }

            // Validate password input
            if (!string.IsNullOrEmpty(textBox7.Text.Trim()))
            {
                password = textBox7.Text.Trim();
                if (crudDatabase.CheckIfElementExists("Employees", "Password", password) &&
                    !(password == Oldpassword && Oldpassword != null))
                {
                    label10.Text += "This password already exists. ";
                    label10.ForeColor = Color.Tomato;
                    return; // Return immediately after adding the error
                }
            }

            // Validate date of starting
            if (dateOfStarting == null)
            {
                label10.Text += "Date of Starting is required. ";
                label10.ForeColor = Color.Tomato;
                return; // Return immediately after adding the error
            }

            // Validate client ID
            if (comboBox3.SelectedItem == null)
            {
                label10.Text += "Client ID is required. ";
                label10.ForeColor = Color.Tomato;
                return; // Return immediately after adding the error
            }
            else
            {
                clientId = (int)((CrudDatabase.Client)comboBox3.SelectedItem).ClientID;
            }


            byte[] imageBytes = null;

            if (pictureBox1.Image != null && !crudDatabase.IsSpecificImage(pictureBox1.Image, Properties.Resources.follower))
            {
                using (var ms = new MemoryStream())
                {
                    pictureBox1.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    imageBytes = ms.ToArray();
                }
            }

            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Get the selected row
                var selectedRow = dataGridView1.SelectedRows[0];
                var selectedId = selectedRow.Cells[0].Value;

                if (selectedId != null)
                {
                    // Convert the selected ID to an integer
                    if (int.TryParse(selectedId.ToString(), out int employeeId))
                    {
                        string query = @"UPDATE Employees SET Name = @Name, PersonalType = @PersonalType, Gender = @Gender,
                                                               Contact = @Contact, Password = @Password, DateOfStarting = @DateOfStarting,
                                                                   Salary = @salary, ClientID = @ClientID, Picture = @Picture WHERE ID = @ID";

                        var parameters = new Dictionary<string, object>
                        {
                             { "@ID", employeeId },
                             { "@Name", name },
                             { "@Gender", gender },
                             { "@Contact", contact },
                             { "@Password", password },
                             { "@ClientID", clientId },
                             { "@PersonalType", personalType },
                             { "@DateOfStarting", dateOfStarting },
                             { "@salary", (object)salary ?? DBNull.Value},
                             { "@Picture", (object)imageBytes ?? DBNull.Value } 
                        };

                        try
                        {
                            bool rowsAffected = crudDatabase.ExecuteNonQuery(query, parameters);
                            if (rowsAffected)
                            {
                                label10.Text = "Employee updated successfully.";
                                label10.ForeColor = Color.LawnGreen;
                                ClearFields();
                                Fetchdata();
                                roundedPanel1.Visible = false;
                                button1.Text = "Add Employee";
                                roundedPanel3.Enabled = true;
                                customBox1.Visible = true;
                                customBox2.Visible = true;
                                button1.BackColor = SystemColors.Highlight;
                                this.Alert("Employee updated successfully!", Alertform.enmType.Success);
                            }
                            else
                            {
                                label10.Text = "No employee found with the provided ID.";
                                label10.ForeColor = Color.Tomato;
                            }
                        }
                        catch (Exception ex)
                        {
                            label10.Text = ex.Message;
                            label10.ForeColor = Color.Tomato;
                        }
                    }
                }
            }
        }


        private void DeleteSelectedEmployee()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView1.SelectedRows[0];
                var selectedId = selectedRow.Cells[0].Value;

                if (selectedId != null)
                {
                    if (int.TryParse(selectedId.ToString(), out int employeeId))
                    {
                        string query = "DELETE FROM Employees WHERE ID = @ID";
                     
                        var parameters = new Dictionary<string, object>
                        {
                            { "@ID", employeeId }
                        };

                        try
                        {
                            bool isDeleted = crudDatabase.ExecuteNonQuery(query, parameters);
                            if (isDeleted)
                            {
                                ClearFields();
                                Fetchdata();
                                label10.Text = "Employee deleted successfully.";
                                label10.ForeColor = Color.LawnGreen;
                                roundedPanel1.Visible = false;
                                button1.Text = "Add Employee";
                                roundedPanel3.Enabled = true;
                                customBox1.Visible = true;
                                customBox2.Visible = true;
                                button1.BackColor = SystemColors.Highlight;
                                this.Alert("Employee deleted successfully!", Alertform.enmType.Success);
                            }
                            else
                            {
                                label10.Text = "No employee found with the provided ID.";
                                label10.ForeColor = Color.Tomato;
                            }
                        }
                        catch
                        {
                            label10.Text = "An error occurred";
                            label10.ForeColor = Color.Tomato;
                        }
                    }
                    else
                    {
                        label10.Text = "Invalid employee ID.";
                        label10.ForeColor = Color.Tomato;
                    }
                }
            }
            else
            {
                label10.Text = "Please select an employee to delete.";
                label10.ForeColor = Color.Tomato;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                DeletePic();
            }
        }

        private void DeletePic()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Get the selected row
                var selectedRow = dataGridView1.SelectedRows[0];
                var selectedId = selectedRow.Cells[0].Value;

                if (selectedId != null)
                {
                    // Construct the SQL query to update the FoodPicture
                    string query = $"UPDATE Employees SET Picture = NULL WHERE ID = {selectedId}";

                    if (crudDatabase.ExecuteNonQuery(query))
                    {
                        // Disable buttons or perform other UI updates
                        pictureBox1.Image = Properties.Resources.follower;
                        this.Alert("Picture deleted. Success!", Alertform.enmType.Success);
                        checkBox1.Visible = false;
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete the picture.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete the picture.");
            }
        }

        private void ClearFields()
        {
            label10.Text = string.Empty;
            comboBox2.SelectedIndex = -1;
            comboBox1.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox5.Text = string.Empty;
            textBox7.Text = string.Empty;
            dateTimePicker1.Text = string.Empty;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (comboBox3.SelectedItem != null)
            //{
            //    roundedTextbox1.Text = comboBox3.Text;
            //}
        }

        private void customBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterEmployees();
        }

        private void roundedTextbox1_TextChanged(object sender, EventArgs e)
        {
            FilterEmployees();
        }

        public void LocalizeControls()
        {
            //button1.Text = LanguageManager.Instance.GetString("MF-btn1");
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
            //button4.Text = LanguageManager.Instance.GetString("MF-btn4");
            //button5.Text = LanguageManager.Instance.GetString("MF-btn5");
            //button6.Text = LanguageManager.Instance.GetString("MF-btn6");
            //button7.Text = LanguageManager.Instance.GetString("MF-btn7");
            //button8.Text = LanguageManager.Instance.GetString("MF-btn8");
            //button9.Text = LanguageManager.Instance.GetString("MF-btn9");
        }

        public void ApplyTheme()
        {
            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;
            
            pictureBox2.Image = colors.Image1;
            button1.ForeColor = colors.Color2;
            textBox1.ForeColor = colors.Color2;
            textBox2.ForeColor = colors.Color2;
            textBox5.ForeColor = colors.Color2;
            textBox7.ForeColor = colors.Color2;
            comboBox1.ForeColor = colors.Color2;
            comboBox2.ForeColor = colors.Color2;
            comboBox3.ForeColor = colors.Color2;
            comboBox1.BackColor = colors.Color1;
            comboBox2.BackColor = colors.Color1;
            comboBox3.BackColor = colors.Color1;            
            comboBox1.ArrowColor = colors.Color2;
            comboBox2.ArrowColor = colors.Color2;
            comboBox3.ArrowColor = colors.Color2;
            customBox1.ForeColor = colors.Color2;
            customBox2.ForeColor = colors.Color2;
            customBox1.BackColor = colors.Color3;
            customBox2.BackColor = colors.Color3;
            pictureBox1.BackColor = colors.Color3;
            pictureBox2.BackColor = colors.Color3;
            comboBox1.BorderColor = colors.Color5;
            comboBox2.BorderColor = colors.Color5;
            comboBox3.BorderColor = colors.Color5;
            customBox1.ArrowColor = colors.Color2;
            customBox2.ArrowColor = colors.Color2;
            customBox1.BorderColor = colors.Color5;
            customBox2.BorderColor = colors.Color5;
            roundedPanel1.ForeColor = colors.Color2;
            roundedPanel2.ForeColor = colors.Color2;
            roundedPanel3.ForeColor = colors.Color2;
            dataGridView1.ForeColor = colors.Color2;
            roundedPanel1.BackColor = colors.Color3;
            roundedPanel2.BackColor = colors.Color3;
            roundedPanel3.BackColor = colors.Color3;
            roundedPanel3.BorderColor = colors.Color4;
            roundedTextbox1.ForeColor = colors.Color2;
            dateTimePicker1.TextColor = colors.Color2;
            textBox1.BackColorRounded = colors.Color1;
            textBox2.BackColorRounded = colors.Color1;
            textBox5.BackColorRounded = colors.Color1;
            textBox7.BackColorRounded = colors.Color1;
            comboBox1.BorderFocusColor = colors.Color5;
            comboBox2.BorderFocusColor = colors.Color5;
            comboBox3.BorderFocusColor = colors.Color5;           
            dateTimePicker1.BorderColor = colors.Color5;
            roundedTextbox1.BorderColor = colors.Color3;
            customBox1.BorderFocusColor = colors.Color5;
            customBox2.BorderFocusColor = colors.Color5;
            dateTimePicker1.CalendarIcon = colors.Image2;
            dataGridView1.BackgroundColor = colors.Color3;
            dateTimePicker1.BackColorCustom = colors.Color1;
            dateTimePicker1.BorderFocusColor = colors.Color5;
            roundedTextbox1.BackColorRounded = colors.Color1;
            dataGridView1.DefaultCellStyle.BackColor = colors.Color3;   
            dataGridView1.DefaultCellStyle.ForeColor = colors.Color2;
            if (colors.Bool1 && Properties.Settings.Default.showScrollbars) showScrollBars();
        }

        private void showScrollBars()
        {
            dataGridView1.ScrollBars = ScrollBars.Vertical;
        }
    }
}
