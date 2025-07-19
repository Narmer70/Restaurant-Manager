using System;
using System.IO;
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
using PadTai.Classes.Controlsdesign;


namespace PadTai.Sec_daryfolders.Updaters.Staffupdater
{
    public partial class Staffdash : UserControl
    {
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;


        public Staffdash()
        {
            InitializeComponent();
            InitializeControlResizer();
            crudDatabase = new CrudDatabase();
          
            ApplyTheme();
            GetEmployeeList();
            LocalizeControls();
            dataGridView1.GridColor = dataGridView1.BackgroundColor;
            LoadEmployeeFromLastLogin();
        }

        private void InitializeControlResizer()
        {
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
            resizer.RegisterControl(customBox1);
            resizer.RegisterControl(pictureBox1);
            resizer.RegisterControl(dataGridView1);
            resizer.RegisterControl(roundedPanel4);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                dgv1ColumnsWidth();
            }
        }

        public List<Employee> GetEmployeeList()
        {
            string query = "SELECT ID, Name FROM Employees";

            List<Employee> employees = crudDatabase.FetchDataToList(query, reader => new Employee
            {
                ID = Convert.ToInt32(reader["ID"]),
                Name = reader["Name"].ToString().ToUpper(),
            });

            // Set the DataSource, DisplayMember, and ValueMember for customBox1
            customBox1.DataSource = employees;
            customBox1.DisplayMember = "Name";
            customBox1.ValueMember = "ID";

            return employees;
        }

        private void customBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (customBox1.SelectedItem is Employee selectedEmployee)
            {
                int selectedEmployeeId = selectedEmployee.ID;
                LoadEmployeeData(selectedEmployeeId);
            }
        }

        public int? GetLastLoginRecord()
        {
            string query = @"SELECT EmployeeID FROM Employeelogin ORDER BY ID DESC LIMIT 1";
           
            DataTable resultTable = crudDatabase.FetchDataFromDatabase(query);

            if (resultTable.Rows.Count > 0)
            {
                // Check if the value is DBNull before casting
                if (resultTable.Rows[0]["EmployeeID"] != DBNull.Value)
                {
                    return Convert.ToInt32(resultTable.Rows[0]["EmployeeID"]);
                }
            }

            return null; 
        }


        public void LoadEmployeeFromLastLogin()
        {
            // Get the last login record ID
            int? lastLoginId = GetLastLoginRecord();

            // Check if a valid ID was found
            if (lastLoginId.HasValue)
            {
                // Load employee data using the found ID
                LoadEmployeeData(lastLoginId.Value);

                UpdateDateOfStartingLabel(lastLoginId.Value);

                CountMatchingOrderDates(lastLoginId.Value);

                //Optionally set the selected value in customBox1 if it has items
                if (customBox1.Items.Count > 0)
                {
                    customBox1.SelectedValue = lastLoginId.Value;
                }
            }
            else
            {
              //  MessageBox.Show("No login record found.");
            }
        }

        public void LoadEmployeeData(int employeeId)
        {
            string query = $"SELECT Name, Gender, Contact, PersonalType, DateOfStarting, Picture FROM Employees WHERE ID = {employeeId}";

            DataTable employeeData = crudDatabase.FetchDataFromDatabase(query);

            if (employeeData.Rows.Count > 0)
            {
                var row = employeeData.Rows[0];

                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                dataGridView1.Columns.Add("0", "0");
                dataGridView1.Columns.Add("1", "1");

                dataGridView1.Rows.Add(LanguageManager.Instance.GetString("Gender") + ":", row["Gender"].ToString());
                dataGridView1.Rows.Add(LanguageManager.Instance.GetString("Contact") + ":", row["Contact"].ToString());
                dataGridView1.Rows.Add(LanguageManager.Instance.GetString("Position") + ":", row["PersonalType"].ToString());
                dataGridView1.Rows.Add(LanguageManager.Instance.GetString("Day1") + ":", Convert.ToDateTime(row["DateOfStarting"]).ToString("d"));

                // Calculate the number of days worked
                DateTime dateOfStarting = Convert.ToDateTime(row["DateOfStarting"]);
                DateTime currentDate = DateTime.Now;
                TimeSpan timeWorked = currentDate - dateOfStarting;
                int daysWorked = (int)timeWorked.TotalDays;
                label4.Text = $"{FormatDays(daysWorked)}";

                dgv1ColumnsWidth();

                // Load picture if not null
                if (row["Picture"] != DBNull.Value)
                {
                    byte[] imageData = (byte[])row["Picture"];
                    using (MemoryStream ms = new MemoryStream(imageData))
                    {
                        pictureBox1.Image = Image.FromStream(ms);
                    }
                }
                else
                {
                    pictureBox1.Image = Properties.Resources.follower;
                }
            }
            else
            {
                // MessageBox.Show("Employee not found.");
            }
        }

        public void UpdateDateOfStartingLabel(int employeeId)
        {
            string query = $"SELECT Thedaydate, TimeOfStarting FROM Employeelogin WHERE EmployeeID = {employeeId} ORDER BY ID DESC LIMIT 1";

            DataTable sessionData = crudDatabase.FetchDataFromDatabase(query);

            if (sessionData.Rows.Count > 0)
            {
                var row = sessionData.Rows[0];

                object dateValue = row["Thedaydate"];
                object timeValue = row["TimeOfStarting"];

                if (timeValue != DBNull.Value && dateValue != DBNull.Value)
                {
                    // Convert the time value to a string
                    string dateString = dateValue.ToString();
                    string timeString = timeValue.ToString();

                    // Try to parse the DateTime
                    if (DateTime.TryParse(timeString, out DateTime timeOfStarting) && DateTime.TryParse(dateString, out DateTime dateOfStarting))
                    {
                        label7.Text = LanguageManager.Instance.GetString("Staffdash01") + " " + dateOfStarting.ToString(@"dd/MM/yyyy") + "\n" +
                                      LanguageManager.Instance.GetString("Staffdash02") + timeOfStarting.ToString(@"hh\:mm\:ss");
                    }
                    else
                    {
                        // label7.Text = "Invalid time format.";
                    }
                }
                else
                {
                    // label7.Text = "Time of starting not available.";
                }
            }
            else
            {
                // label7.Text = "No record found for the given EmployeeID.";
            }
        }


        private string FormatDays(int days)
        {
            if (days >= 1000)
            {
                // Format as "1k", "6.7k", etc.
                return (days / 1000.0).ToString("0.#") + "k"; // Use "0.#" to keep one decimal place if needed
            }
            return days.ToString(); // Return the number as is if less than 1000
        }

        public List<DateTime> GetThedaydateById(int id)
        {
            // Use string interpolation to construct the query
            string query = $"SELECT Thedaydate FROM Employeelogin WHERE EmployeeID = {id}";

            return crudDatabase.FetchDataToList(query, reader =>
            {
                // Ensure the value is not null before returning
                return reader.IsDBNull(0) ? default(DateTime) : reader.GetDateTime(0);
            }).Where(date => date != default(DateTime)).ToList();
        }

        public void CountMatchingOrderDates(int id)
        {
            List<DateTime> thedaydates = GetThedaydateById(id);

            decimal totalSum = 0;
            int matchingDateCount = 0;

            // If there are no dates found, set label to 0 and return
            if (thedaydates.Count == 0)
            {
                label2.Text = "0";
                return;
            }

            // Fetch the order dates and total prices from ReceiptsArchive
            string query = "SELECT OrderDateTime, TotalPrice FROM ReceiptsArchive";

            var receiptData = crudDatabase.FetchDataToList(query, reader =>
            {
                // Ensure the values are not null before returning
                return new
                {
                    OrderDateTime = reader.IsDBNull(0) ? default(DateTime) : reader.GetDateTime(0).Date,
                    TotalPrice = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1)
                };
            });

            foreach (var receipt in receiptData)
            {
                // Check if the date matches and add to the total sum
                if (thedaydates.Contains(receipt.OrderDateTime))
                {
                    totalSum += receipt.TotalPrice;
                    matchingDateCount++;
                }
            }

            label2.Text = FormatDays(matchingDateCount).ToString();
            label6.Text = FormatDays(Convert.ToInt32(totalSum)).ToString() + $" {CurrencyService.Instance.GetCurrencySymbol()}";
        }

        private void dgv1ColumnsWidth()
        {
            if(dataGridView1.RowCount > 0) 
            {
                dataGridView1.DefaultCellStyle.ForeColor = dataGridView1.ForeColor;
                dataGridView1.Columns[0].Width = (int)(dataGridView1.Width * 0.40);
                dataGridView1.Columns[1].Width = (int)(dataGridView1.Width * 0.60);
                dataGridView1.DefaultCellStyle.BackColor = dataGridView1.BackgroundColor;
                dataGridView1.DefaultCellStyle.SelectionForeColor = dataGridView1.ForeColor;
                dataGridView1.Columns[0].DefaultCellStyle.Padding = new Padding(40, 0, 0, 0);
                dataGridView1.Columns[1].DefaultCellStyle.Padding = new Padding(20, 0, 0, 0);
                dataGridView1.DefaultCellStyle.SelectionBackColor = dataGridView1.BackgroundColor;
                dataGridView1.DefaultCellStyle.Font = new Font("Century Gothic", 10, FontStyle.Bold);
                dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }          
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 0) 
            {
                e.CellStyle.ForeColor = Color.DarkGray;
                e.CellStyle.SelectionForeColor= Color.DarkGray;
            }
        }

        private void customBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        public void LocalizeControls()
        {
            label1.Text = LanguageManager.Instance.GetString("Staffdash-lbl1");
            label3.Text = LanguageManager.Instance.GetString("Staffdash-lbl3");
            label5.Text = LanguageManager.Instance.GetString("Staffdash-lbl5");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;
            roundedPanel4.BackColor = colors.Color3;
            dataGridView1.BackgroundColor = colors.Color3;

            label7.ForeColor = this.ForeColor;
            customBox1.BackColor = this.BackColor;
            customBox1.ForeColor = this.ForeColor;
            pictureBox1.BackColor = this.BackColor;
            customBox1.ArrowColor = this.ForeColor;
            customBox1.BorderColor = this.BackColor;
            dataGridView1.ForeColor = this.ForeColor;
            roundedPanel4.ForeColor = this.ForeColor;
            customBox1.BorderFocusColor = this.BackColor;
        }


        public class Employee
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }
    }
}
