using System;
using System.IO;
using System.Linq;
using System.Data;
using System.Text;
using PadTai.Classes;
using System.Drawing;
using System.Data.SQLite;
using System.Collections;
using System.Globalization;
using System.Windows.Forms;
using System.ComponentModel;
using PadTai.Classes.Others;
using System.Drawing.Printing;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using PadTai.Classes.Controlsdesign;
using PadTai.Sec_daryfolders.Others_Forms;
using PadTai.Sec_daryfolders.Departmentdata;


namespace PadTai.Sec_daryfolders.Staffmanager.Stafftimetable
{
    public partial class Emplotimetable : Form
    {
        private string sqliteConnectionString = DatabaseConnection.GetSQLiteConnectionString();
        ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();
        private List<int> personalTypeRowIndices = new List<int>();
        private DataGridViewScroller scroller;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;
        private Receipt currentReceipt;


        public Emplotimetable()
        {
            InitializeComponent();
            PopulateDaysOfMonth();
            InitializeControlResize();
            InitializeDataGridView();
            LoadEmployeeNamestoDgv();

            ApplyTheme();
            LocalizeControls();
            dataGridView1.GridColor = colors.Color3;
            dataGridView2.GridColor = colors.Color3;
        }

        private void InitializeControlResize()
        {
            crudDatabase = new CrudDatabase();
            fontResizer = new FontResizer();
            roundedPanel1.Visible = false;
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);
         
            resizer.RegisterControl(panel1);
            resizer.RegisterControl(label1);
            resizer.RegisterControl(panel1);
            resizer.RegisterControl(label1);
            resizer.RegisterControl(label2);
            resizer.RegisterControl(label3);
            resizer.RegisterControl(label4);
            resizer.RegisterControl(button1);
            resizer.RegisterControl(button6);            
            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(dataGridView1);            
            resizer.RegisterControl(dataGridView2);
            resizer.RegisterControl(roundedPanel1);

            scroller = new DataGridViewScroller(this, null, dataGridView1, dataGridView2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
                this.Close();
        }

        private void Empltimetable_Load(object sender, EventArgs e)
        {
            string currentMonth = DateTime.Now.ToString("MMMM yyyy", LanguageManager.Instance.CurrentCulture).ToUpper();
            label1.Text = currentMonth;            
        
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }    
            
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }        
        }

        private void Empltimetable_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }

            if (dataGridView1.RowCount > 0)
            {
                dgvColumnWidth();
            }
        }


        private void InitializeDataGridView()
        {
            // Set up the DataGridView
           dataGridView1.RowCount = 40;
           dataGridView1.ColumnCount = 40; 
           
            for (int i = 0; i < 40; i++)
            {
                dataGridView1.Rows[i].Height = 30;        
                dataGridView1.Columns[i].Width = 38;
                dataGridView1.Columns[31].Width = 60;
                dataGridView1.Columns[33].Width = 60;
                dataGridView1.Columns[36].Width = 60;
                dataGridView1.Columns[31].ReadOnly = true;
                dataGridView1.Columns[32].ReadOnly = true;
                dataGridView1.Columns[33].ReadOnly = true;
                dataGridView1.Columns[34].ReadOnly = true;
                dataGridView1.Columns[35].ReadOnly = true;
                dataGridView1.Columns[36].ReadOnly = true;
                dataGridView1.Columns[i].Resizable = DataGridViewTriState.True;
                dataGridView1.Columns[34].HeaderText = LanguageManager.Instance.GetString("EMPTT-bonus");
                dataGridView1.Columns[31].HeaderText = LanguageManager.Instance.GetString("EMPTT-sumhours");
                dataGridView1.Columns[32].HeaderText = LanguageManager.Instance.GetString("EMPTT-Salary/h");
                dataGridView1.Columns[35].HeaderText = LanguageManager.Instance.GetString("EMPTT-penalities");
                dataGridView1.Columns[33].HeaderText = LanguageManager.Instance.GetString("EMPTT-grosssalary");
                dataGridView1.Columns[36].HeaderText = LanguageManager.Instance.GetString("EMPTT-finalsalary");
                dataGridView1.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void UpdateRowSum(int rowIndex)
        {
            decimal sum = 0;

            // Loop through all cells in the specified row, excluding the last column (Sum)
            for (int i = 0; i < 31; i++) // Exclude the last column (Sum)
            {
                if (dataGridView1.Rows[rowIndex].Cells[i].Value != null &&
                    decimal.TryParse(dataGridView1.Rows[rowIndex].Cells[i].Value.ToString(), out decimal value))
                {
                    sum += value;
                }
            }

            // Update the 31st cell in the specified row with the calculated sum
            if (dataGridView1.Rows.Count > rowIndex) // Ensure the row exists
            {
                dataGridView1.Rows[rowIndex].Cells[31].Value = sum > 0 ? sum : (object)null; // Set to null if sum is 0

                // Retrieve the salary from cell 32 (index 32)
                if (decimal.TryParse(dataGridView1.Rows[rowIndex].Cells[32].Value?.ToString(), out decimal salary) && salary > 0)
                {
                    // Multiply the sum by the salary if salary is greater than 0
                    decimal total = sum * salary;

                    // Update the 33rd cell in the specified row with the total
                    dataGridView1.Rows[rowIndex].Cells[33].Value = total; // Assuming cell 32 is index 33
                }
                else
                {
                    // If salary is not greater than 0, set the total cell to null
                    dataGridView1.Rows[rowIndex].Cells[33].Value = null;
                }
            }
        }

        private void PopulateDaysOfMonth()
        {
            dataGridView1.Columns.Clear();

            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            CultureInfo culture = LanguageManager.Instance.CurrentCulture;

            // Populate the column with day names and day numbers
            for (int day = 1; day <= DateTime.DaysInMonth(firstDayOfMonth.Year, firstDayOfMonth.Month); day++)
            {
                // Get the day of the week
                string dayName = firstDayOfMonth.AddDays(day - 1).ToString("dddd", culture);
                dayName = char.ToUpper(dayName[0]) + dayName.Substring(1).ToLower();

                // Combine day name and day number
                string combinedDay = $"  {dayName},  {day}";

                dataGridView1.Columns.Add("Days", combinedDay);
            }         
        }

        private void LoadEmployeeNamestoDgv()
        {
            int clientId;

            // Try to parse the selected client ID from settings
            if (int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                // Fetch work hours from TimetableMap
                string timetableQuery = @" SELECT EmployeeId, Monthday, WorkHours FROM TimetableMap 
                                                   WHERE EmployeeId IN (SELECT ID FROM Employees WHERE ClientID = @ClientID)";

                var timetableParameters = new Dictionary<string, object>
                {
                   { "@ClientID", clientId }
                };

                // Fetch data from the database
                DataTable timetableTable = crudDatabase.FetchDataFromDatabase(timetableQuery, timetableParameters);

                // Create a dictionary to hold work hours by employee and day
                var workHours = new Dictionary<int, Dictionary<int, double>>();

                foreach (DataRow row in timetableTable.Rows)
                {
                    int employeeId = Convert.ToInt32(row["EmployeeId"]); // Use Convert.ToInt32
                    int monthDay = Convert.ToInt32(row["Monthday"]); // Use Convert.ToInt32
                    double workHoursValue = Convert.ToDouble(row["WorkHours"]); // Use Convert.ToDouble


                    if (!workHours.ContainsKey(employeeId))
                    {
                        workHours[employeeId] = new Dictionary<int, double>();
                    }

                    workHours[employeeId][monthDay] = workHoursValue; 
                }

                string query = "SELECT ID, Name, PersonalType, Salary FROM Employees WHERE ClientID = @ClientID ORDER BY PersonalType";
             
                var parameters = new Dictionary<string, object>
                {
                    { "@ClientID", clientId }
                };

                // Fetch data from the database
                DataTable employeesTable = crudDatabase.FetchDataFromDatabase(query, parameters);

                int rowIndex = 0; 

                // Group employees by PersonalType
                var groupedEmployees = employeesTable.AsEnumerable()
                    .GroupBy(row => row.Field<string>("PersonalType"));

                // Populate the DataGridView with employee names
                foreach (var group in groupedEmployees)
                {
                    // Insert an empty row for the PersonalType
                    if (rowIndex < dataGridView1.RowCount)
                    {
                        personalTypeRowIndices.Add(rowIndex); 
                        //dataGridView1.Rows[rowIndex].Frozen = true;
                        dataGridView1.Rows[rowIndex].Tag = group.Key; 
                        dataGridView1.Rows[rowIndex].ReadOnly = true; 
                        //dataGridView1.Rows[rowIndex].HeaderCell.Value = group.Key; 
                        rowIndex++;
                    }

                    // Add each employee in the group
                    foreach (var employee in group)
                    {
                        if (rowIndex < dataGridView1.RowCount)
                        {
                            int employeeId = Convert.ToInt32(employee["ID"]);
                            dataGridView1.Rows[rowIndex].HeaderCell.Value = employee["Name"].ToString();
                            double employeeSalary = employee["Salary"] != DBNull.Value ? Convert.ToDouble(employee["Salary"]) : 0; 
                            dataGridView1.Rows[rowIndex].Cells[32].Value = employeeSalary; 

                            for (int day = 1; day <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); day++)
                            {
                                if (workHours.ContainsKey(employeeId) && workHours[employeeId].ContainsKey(day))
                                {
                                    dataGridView1.Rows[rowIndex].Cells[day - 1].Value = workHours[employeeId][day]; // Adjust for zero-based index
                                }
                            }

                            rowIndex++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                dgvColumnWidth();
            }
        }

        private void dgvColumnWidth()
        {
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[i].HeaderCell.Style.Font = new Font("Century Gothic", 10, FontStyle.Regular);
            }
            dataGridView1.ColumnHeadersHeight = 150;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = dataGridView1.BackgroundColor;
            dataGridView1.ColumnHeadersDefaultCellStyle.SelectionForeColor = dataGridView1.ForeColor;
            dataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = dataGridView1.BackColor;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            dataGridView1.RowHeadersWidth = 160;
            dataGridView1.DefaultCellStyle.ForeColor = dataGridView1.ForeColor;
            dataGridView1.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView1.RowHeadersDefaultCellStyle.ForeColor = dataGridView1.ForeColor;
            dataGridView1.RowHeadersDefaultCellStyle.BackColor = dataGridView1.BackgroundColor;
            dataGridView1.RowHeadersDefaultCellStyle.SelectionForeColor = dataGridView1.ForeColor;
            dataGridView1.RowHeadersDefaultCellStyle.SelectionBackColor = dataGridView1.BackColor;
            dataGridView1.RowHeadersDefaultCellStyle.Font = new Font("Microsoft PhagsPa", 10, FontStyle.Regular);
        }

        private void SaveTimetableMap()
        {
            try
            {
                using (var connection = new SQLiteConnection(sqliteConnectionString))
                {
                    connection.Open();

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow) continue;
                      
                        if (row.HeaderCell != null && row.HeaderCell.Value != null)
                        {
                            // Get the employee name from the row header
                            var employeeName = row.HeaderCell.Value.ToString();
                            var employeeIdCommand = new SQLiteCommand("SELECT ID FROM Employees WHERE Name = @EmployeeName", connection);
                            employeeIdCommand.Parameters.AddWithValue("@EmployeeName", employeeName);
                            var employeeId = employeeIdCommand.ExecuteScalar();

                            // Iterate through each cell in the row (representing days of the month)
                            for (int day = 1; day <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); day++)
                            {
                                var cell = row.Cells[day - 1]; // Adjust for zero-based index

                                if (cell.Value != null)
                                {
                                    double workHours; 
                                    try
                                    {
                                        workHours = Convert.ToDouble(cell.Value); 
                                    }
                                    catch (FormatException)
                                    {
                                        continue;
                                    }
                                    catch (InvalidCastException)
                                    {
                                        continue;
                                    }

                                    // Upsert command for the TimetableMap
                                    var upsertCommand = new SQLiteCommand(@" INSERT INTO TimetableMap (EmployeeId, Monthday, WorkHours) VALUES (@EmployeeId, @Monthday, @WorkHours)
                                                                              ON CONFLICT(EmployeeId, Monthday) DO UPDATE SET WorkHours = @WorkHours", connection);

                                    upsertCommand.Parameters.AddWithValue("@EmployeeId", employeeId);
                                    upsertCommand.Parameters.AddWithValue("@Monthday", day);
                                    upsertCommand.Parameters.AddWithValue("@WorkHours", workHours); // Use double for REAL type
                                    upsertCommand.ExecuteNonQuery();
                                }
                                else
                                {
                                    // If the cell is empty, delete the corresponding record
                                    var deleteCommand = new SQLiteCommand(@" DELETE FROM TimetableMap WHERE EmployeeId = @EmployeeId AND Monthday = @Monthday", connection);
                                    deleteCommand.Parameters.AddWithValue("@EmployeeId", employeeId);
                                    deleteCommand.Parameters.AddWithValue("@Monthday", day);
                                    deleteCommand.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error)
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
            SaveTimetableMap();
        }

     
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex >= 0 && e.ColumnIndex <= 30) // Column header clicked
            {
                roundedPanel1.Visible = true;

                string columnHeader = dataGridView1.Columns[e.ColumnIndex].HeaderText ?? LanguageManager.Instance.GetString("Novalue");
                label2.Text = columnHeader;

                 currentReceipt = new Receipt { Header = columnHeader };

                // Retrieve all work hours associated with the selected day
                using (var connection = new SQLiteConnection(sqliteConnectionString))
                {
                    connection.Open();
                    int monthDay = e.ColumnIndex + 1; // Assuming columns represent days of the month

                    var workHoursCommand = new SQLiteCommand(@" SELECT Employees.Name, TimetableMap.WorkHours FROM TimetableMap 
                                                                       INNER JOIN Employees ON TimetableMap.EmployeeId = Employees.ID 
                                                                                  WHERE TimetableMap.Monthday = @Monthday", connection);
                    workHoursCommand.Parameters.AddWithValue("@Monthday", monthDay);
                    var workHoursReader = workHoursCommand.ExecuteReader();

                    // Clear previous data in dataGridView2
                    dataGridView2.Rows.Clear();
                    dataGridView2.Columns.Clear();
                    dataGridView2.Columns.Add("EmployeeName", "     " + LanguageManager.Instance.GetString("EMPTT-Empworking"));
                    dataGridView2.Columns.Add("WorkHours", LanguageManager.Instance.GetString("EMPTT-workhours"));
                    dgv2ColumnsWidth();

                    double totalWorkHours = 0; // Initialize total work hours
                    while (workHoursReader.Read())
                    {
                        string employeeName = workHoursReader["Name"].ToString();
                        double workHours = Convert.ToDouble(workHoursReader["WorkHours"]);
                        dataGridView2.Rows.Add(employeeName, workHours + " " + LanguageManager.Instance.GetString("Hour"));
                        totalWorkHours += workHours;

                        currentReceipt.Items.Add(new ReceiptItem { Name = employeeName, AmountInGrams = workHours }); // Adjust as needed
                    }
                    currentReceipt.TotalAmount = totalWorkHours;
                    label3.Text = totalWorkHours + " " + LanguageManager.Instance.GetString("Hour");
                }
            }
            else if (e.RowIndex >= 0 && e.ColumnIndex == -1) // Row header clicked
            {
                roundedPanel1.Visible = true;

                string rowHeader = dataGridView1.Rows[e.RowIndex].HeaderCell.Value?.ToString() ?? LanguageManager.Instance.GetString("Novalue");
                label2.Text = rowHeader;

                currentReceipt = new Receipt { Header = rowHeader };

                // Retrieve all work hours associated with the selected employee
                using (var connection = new SQLiteConnection(sqliteConnectionString))
                {
                    connection.Open();
                    var employeeIdCommand = new SQLiteCommand("SELECT ID FROM Employees WHERE Name = @EmployeeName", connection);
                    employeeIdCommand.Parameters.AddWithValue("@EmployeeName", rowHeader);
                    int employeeId = Convert.ToInt32(employeeIdCommand.ExecuteScalar());

                    var workHoursCommand = new SQLiteCommand(@" SELECT Monthday, WorkHours FROM TimetableMap WHERE EmployeeId = @EmployeeId", connection);
                    workHoursCommand.Parameters.AddWithValue("@EmployeeId", employeeId);
                    var workHoursReader = workHoursCommand.ExecuteReader();

                    // Clear previous data in dataGridView2
                    dataGridView2.Rows.Clear();
                    dataGridView2.Columns.Clear();
                    dataGridView2.Columns.Add("Day", "     " + LanguageManager.Instance.GetString("EMPTT-daysofmonth"));
                    dataGridView2.Columns.Add("WorkHours", LanguageManager.Instance.GetString("EMPTT-workhours"));
                    dgv2ColumnsWidth();

                    double totalWorkHours = 0;
                    while (workHoursReader.Read())
                    {
                        int monthDay = Convert.ToInt32(workHoursReader["Monthday"]);
                        double workHours = Convert.ToDouble(workHoursReader["WorkHours"]);
                        dataGridView2.Rows.Add(monthDay, workHours + " " + LanguageManager.Instance.GetString("Hour"));
                        totalWorkHours += workHours;

                        currentReceipt.Items.Add(new ReceiptItem { Name = monthDay.ToString(), AmountInGrams = workHours }); // Adjust as needed
                    }
                    currentReceipt.TotalAmount = totalWorkHours;
                    label3.Text = totalWorkHours + " " + LanguageManager.Instance.GetString("Hour");
                }
            }
            else if (e.RowIndex >= 0 && e.ColumnIndex >= 0) // Cell clicked
            {
                roundedPanel1.Visible = false;
            }
        }

        private void dgv2ColumnsWidth()
        {
            dataGridView2.Columns[0].Width = (int)(dataGridView2.Width * 0.70);
            dataGridView2.Columns[1].Width = (int)(dataGridView2.Width * 0.30);
            dataGridView2.Columns[0].DefaultCellStyle.Padding = new Padding(20, 0, 0, 0);
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            for (int i = 0; i < dataGridView2.Columns.Count; i++)
            {
                if (i == 0)
                {
                    dataGridView2.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                }
                else if (i == 1)
                {
                    dataGridView2.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                dataGridView2.Columns[i].HeaderCell.Style.Font = new Font("Century Gothic", 9, FontStyle.Bold);
            }
            dataGridView2.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = dataGridView2.ForeColor;
            dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = dataGridView2.BackgroundColor;
            dataGridView2.ColumnHeadersDefaultCellStyle.SelectionForeColor = dataGridView2.ForeColor;
            dataGridView2.ColumnHeadersDefaultCellStyle.SelectionBackColor = dataGridView2.BackColor;
        }


        private void button6_Click(object sender, EventArgs e)
        {
            SaveAsPhoto();
        }

        private void SaveAsPhoto()
        {
            // Determine the bounds for the image
            int totalWidth = dataGridView1.RowHeadersWidth; // Start with row header width
            int totalHeight = - dataGridView1.ColumnHeadersHeight + 30;

            // Calculate total width for all columns including the row header
            for (int col = 0; col <= 31; col++)
            {
                totalWidth += dataGridView1.Columns[col].Width;
            }

            // Calculate total height for all rows until two consecutive empty rows are found
            int rowCount = 0;
            for (int row = 0; row < dataGridView1.Rows.Count; row++)
            {
                // Check if the current row and the next row are both empty
                if (row < dataGridView1.Rows.Count - 1 && IsRowEmpty(dataGridView1.Rows[row]) && IsRowEmpty(dataGridView1.Rows[row + 1]))
                {
                    break; // Stop if we find two consecutive empty rows
                }

                totalHeight += dataGridView1.Rows[row].Height;
                rowCount++;
            }

            // Create a bitmap to hold the image
            Bitmap bitmap = new Bitmap(totalWidth, totalHeight + dataGridView1.ColumnHeadersHeight);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                int offsetY = 0;

                // Draw the numbering for columns 1 to 31 in the first row from right to left
                int offsetX = totalWidth; // Start from the right edge
                for (int col = 31; col >= 0; col--) // Iterate from last column to first
                {
                    offsetX -= dataGridView1.Columns[col].Width; // Move left for each column
                    Rectangle headerRect = new Rectangle(offsetX, offsetY, dataGridView1.Columns[col].Width, dataGridView1.Rows[0].Height);
                    g.FillRectangle(new SolidBrush(Color.LightGray), headerRect); // Background color for header
                    g.DrawRectangle(Pens.Gray, headerRect);
                    g.DrawString((col + 1).ToString(), dataGridView1.ColumnHeadersDefaultCellStyle.Font, Brushes.Black, headerRect);
                }

                // Move down for the next row
                offsetY += dataGridView1.Rows[0].Height;

                // Draw the DataGridView content including row headers
                for (int row = 0; row < rowCount; row++)
                {
                    offsetX = 0;

                    // Draw the row header
                    Rectangle rowHeaderRect = new Rectangle(0, offsetY, dataGridView1.RowHeadersWidth, dataGridView1.Rows[row].Height);
                    g.FillRectangle(new SolidBrush(Color.White), rowHeaderRect);
                    g.DrawRectangle(Pens.Gray, rowHeaderRect);
                    g.DrawString(dataGridView1.Rows[row].HeaderCell.Value?.ToString() ?? "", dataGridView1.RowHeadersDefaultCellStyle.Font, Brushes.Black, rowHeaderRect);

                    // Check if this row is a PersonalType row
                    if (personalTypeRowIndices.Contains(row))
                    {
                        // Draw the background for the entire row
                        using (Brush backBrush = new SolidBrush(Color.LightGray))
                        {
                            g.FillRectangle(backBrush, rowHeaderRect); // Fill row header
                        }
                        // Draw the text centered across the entire row
                        string text = dataGridView1.Rows[row].Tag?.ToString() ?? ""; // Use the Tag for PersonalType
                        TextRenderer.DrawText(g, text, dataGridView1.RowHeadersDefaultCellStyle.Font, rowHeaderRect, Color.Black, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                    }

                    // Draw the cells
                    for (int col = 0; col <= 31; col++)
                    {
                        DataGridViewCell cell = dataGridView1.Rows[row].Cells[col];
                        Rectangle cellRect = new Rectangle(offsetX + dataGridView1.RowHeadersWidth, offsetY, cell.Size.Width, cell.Size.Height);

                        // Fill the cell background for PersonalType rows
                        if (personalTypeRowIndices.Contains(row))
                        {
                            g.FillRectangle(new SolidBrush(Color.LightGray), cellRect); // Fill cell background
                        }
                        else
                        {
                            g.FillRectangle(new SolidBrush(Color.White), cellRect); // Fill normal cell background
                        }
                        g.DrawRectangle(Pens.Gray, cellRect);
                        g.DrawString(cell.Value?.ToString() ?? "", cell.InheritedStyle.Font, Brushes.Black, cellRect);
                        offsetX += cell.Size.Width;
                    }
                    offsetY += dataGridView1.Rows[row].Height; // Move down for the next row
                }
            }

            // Save the bitmap to a file
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
                saveFileDialog.Title = "Save DataGridView As Image";
                saveFileDialog.FileName = "Timetable Image";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    bitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    MessageBox.Show($"DataGridView saved as image at: {saveFileDialog.FileName}");
                }
            }
        }

        // Helper method to check if a row is empty
        private bool IsRowEmpty(DataGridViewRow row)
        {
            foreach (DataGridViewCell cell in row.Cells)
            {
                if (cell.Value != null && !string.IsNullOrWhiteSpace(cell.Value.ToString()))
                {
                    return false; // Found a non-empty cell
                }
            }
            return true; // All cells are empty
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                UpdateRowSum(e.RowIndex);
            }
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1 && (e.ColumnIndex >= 0))
            {
                e.PaintBackground(e.CellBounds, true);
                e.Graphics.TranslateTransform(e.CellBounds.Left, e.CellBounds.Bottom);
                e.Graphics.RotateTransform(270);

                using (Brush brush = new SolidBrush(this.ForeColor))
                {
                    e.Graphics.DrawString(e.FormattedValue.ToString(), e.CellStyle.Font, brush, 5, 5);
                }
                e.Graphics.ResetTransform();
                e.Handled = true;
            }


            DataGridView dgv = sender as DataGridView;

            // Check if we are painting a row that corresponds to a PersonalType
            if (e.RowIndex >= 0 && personalTypeRowIndices.Contains(e.RowIndex))
            {
                // Prevent the default painting
                e.Handled = true;

                // Calculate the rectangle for the entire row
                Rectangle rowBounds = dgv.GetRowDisplayRectangle(e.RowIndex, true);

                // Draw the background
                using (Brush backBrush = new SolidBrush(e.CellStyle.BackColor))
                {
                    e.Graphics.FillRectangle(backBrush, rowBounds);
                }
                // Draw the top border
                using (Pen borderPen = new Pen(dgv.GridColor))
                {
                  //  e.Graphics.DrawLine(borderPen, rowBounds.Left, rowBounds.Top, rowBounds.Right, rowBounds.Top);
                    e.Graphics.DrawLine(borderPen, rowBounds.Left, rowBounds.Bottom - 1, rowBounds.Right, rowBounds.Bottom - 1);
                }

                // Draw the text centered across the entire row
                string text = dgv.Rows[e.RowIndex].Tag?.ToString() ?? ""; // Use the Tag for PersonalType
                TextRenderer.DrawText(e.Graphics, text, e.CellStyle.Font, rowBounds, e.CellStyle.ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 31 )
            {
                e.CellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            }
            if (e.ColumnIndex == 33)
            {
                e.CellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            }
            if (e.ColumnIndex == 36)
            {
                e.CellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            }
        }

        private void PrintReceipt(Receipt receipt)
        {
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += (sender, e) =>
            {
                Brush brush = Brushes.Black;
                Font font = new Font("Arial", 10);
                Font hfont = new Font("Century Gothic", 12, FontStyle.Bold);
                float centerX = e.Graphics.VisibleClipBounds.Width / 2;
                float discountValueX = e.Graphics.VisibleClipBounds.Width - 70;
                float currentYPos = 0;

                // Define margins
                float leftMargin = 20;
                float rightMargin = 20;
                float maxWidth = e.Graphics.VisibleClipBounds.Width - leftMargin - rightMargin;

                // Print header
                string receiptIdText = $"{receipt.Header}";
                string[] words = receiptIdText.Split(' '); // Split the header into words for wrapping
                StringBuilder currentLine = new StringBuilder();
                float lineHeight = hfont.GetHeight(e.Graphics);

                foreach (var word in words)
                {
                    // Check if adding the next word exceeds the max width
                    string testLine = currentLine.Length > 0 ? currentLine + " " + word : word;
                    SizeF testLineSize = e.Graphics.MeasureString(testLine, hfont);

                    if (testLineSize.Width > maxWidth)
                    {
                        // Draw the current line
                        e.Graphics.DrawString(currentLine.ToString(), hfont, brush, centerX - (testLineSize.Width / 2), currentYPos);
                        currentYPos += lineHeight; // Move to the next line
                        currentLine.Clear(); // Clear the current line
                        currentLine.Append(word); // Start a new line with the current word
                    }
                    else
                    {
                        currentLine.Append(currentLine.Length > 0 ? " " + word : word);
                    }
                }

                // Draw the last line if there's any text left
                if (currentLine.Length > 0)
                {
                    SizeF lastLineSize = e.Graphics.MeasureString(currentLine.ToString(), hfont);
                    e.Graphics.DrawString(currentLine.ToString(), hfont, brush, centerX - (lastLineSize.Width / 2), currentYPos);
                }

                float yPos = currentYPos + 20;
                e.Graphics.DrawString("---------------------------------------------------", new Font("Century Gothic", 12, FontStyle.Bold), brush, new PointF(5, yPos));

                yPos += 20;
                // Print items
                foreach (var item in receipt.Items)
                {
                    e.Graphics.DrawString(item.Name, font, brush, new PointF(10, yPos));
                    e.Graphics.DrawString($"{item.AmountInGrams} {LanguageManager.Instance.GetString("Hour")}", font, brush, new PointF(discountValueX, yPos));
                    yPos += 20;
                }

                e.Graphics.DrawString("---------------------------------------------------", new Font("Century Gothic", 12, FontStyle.Bold), brush, new PointF(5, yPos));

                yPos += 20;
                // Print total amount
                e.Graphics.DrawString(LanguageManager.Instance.GetString("Receiptprinttotal"), new Font("Arial", 12, FontStyle.Bold), brush, new PointF(10, yPos));
                e.Graphics.DrawString($"{receipt.TotalAmount} {LanguageManager.Instance.GetString("Hour")}", new Font("Arial", 12, FontStyle.Bold), brush, new PointF(discountValueX, yPos));

                yPos += 20;
                e.Graphics.DrawString("---------------------------------------------------", new Font("Century Gothic", 12, FontStyle.Bold), brush, new PointF(5, yPos));

                yPos += 40;
                SizeF restaurantAddressSize = e.Graphics.MeasureString(receipt.ClientAddress, font);
                e.Graphics.DrawString(receipt.ClientAddress, font, brush, centerX - (restaurantAddressSize.Width / 2), yPos);

                yPos += 20;
                string Pointname = receipt.RestaurantName.ToUpper() + ":" + receipt.ClientName.ToUpper();
                SizeF restaurantNameSize = e.Graphics.MeasureString(Pointname, font);
                e.Graphics.DrawString(Pointname, font, brush, centerX - (restaurantNameSize.Width / 2), yPos);

                if (Properties.Settings.Default.printBrandLogo)
                {
                    // Draw the restaurant logo if available
                    if (receipt.RestaurantLogo != null)
                    {
                        yPos += 20;

                        using (MemoryStream ms = new MemoryStream(receipt.RestaurantLogo))
                        {
                            Image logo = Image.FromStream(ms);

                            // Define the desired width and height for the logo
                            int desiredWidth = 80; // Set your desired width
                            int desiredHeight = 80; // Set your desired height

                            // Calculate the aspect ratio
                            float aspectRatio = (float)logo.Width / logo.Height;

                            // Adjust dimensions to maintain aspect ratio
                            if (logo.Width > logo.Height)
                            {
                                desiredHeight = (int)(desiredWidth / aspectRatio);
                            }
                            else
                            {
                                desiredWidth = (int)(desiredHeight * aspectRatio);
                            }

                            // Calculate the position to center the logo
                            int centerPic = (int)(e.Graphics.VisibleClipBounds.Width - desiredWidth) / 2;

                            // Draw the logo with the specified dimensions
                            e.Graphics.DrawImage(logo, new Rectangle(centerPic, (int)yPos, desiredWidth, desiredHeight));
                        }
                    }
                }
            };

            printDocument.Print();
        }

        private void Alert(string msg, Alertform.enmType type)
        {
            Alertform alertform = new Alertform();
            alertform.showAlert(msg, type);
        }

        private void label4_Click(object sender, EventArgs e)
        {
            if (currentReceipt != null)
            {
                var detailsFetcher = new RestaurantDetails();
                var (restaurantName, restaurantLogo, clientName, clientAddress) = detailsFetcher.GetDetails();

                currentReceipt.RestaurantName = restaurantName;
                currentReceipt.RestaurantLogo = restaurantLogo;
                currentReceipt.ClientAddress = clientAddress;
                currentReceipt.ClientName = clientName;

                if (currentReceipt.Items.Count > 0)
                {
                    PrintReceipt(currentReceipt);
                }
                else
                {
                    this.Alert("No timetable to print", Alertform.enmType.Warning);
                }
            }
        }

        private void label4_MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Hand;
            label4.ForeColor = SystemColors.Highlight;
            label4.Font = new Font(label4.Font, FontStyle.Underline);
        }

        private void label4_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            label4.ForeColor = Color.DodgerBlue;
            label4.Font = new Font(label4.Font, FontStyle.Bold);
        }

        private void Emplotimetable_Click(object sender, EventArgs e)
        {
            roundedPanel1.Visible = false;
        }

        public void LocalizeControls()
        {
            label4.Text = LanguageManager.Instance.GetString("EMPTT-lbl4");
            button1.Text = LanguageManager.Instance.GetString("Btn-close");
            button6.Text = LanguageManager.Instance.GetString("Getimage");
            rjButton1.Text = LanguageManager.Instance.GetString("Btn-save");
            //button4.Text = LanguageManager.Instance.GetString("MF-btn4");
        }

        public void ApplyTheme()
        {
            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;
            label1.ForeColor = colors.Color2;
            roundedPanel1.BackColor = colors.Color3;
            dataGridView1.BackgroundColor = colors.Color1;
            dataGridView2.BackgroundColor = colors.Color3;
            dataGridView1.DefaultCellStyle.BackColor = colors.Color1;
            dataGridView2.DefaultCellStyle.BackColor = colors.Color3;

            roundedPanel1.ForeColor = this.ForeColor;
            dataGridView1.ForeColor = this.ForeColor;
            dataGridView2.ForeColor = this.ForeColor;
            dataGridView1.DefaultCellStyle.ForeColor = this.ForeColor;
            dataGridView2.DefaultCellStyle.ForeColor = this.ForeColor;
            label4.Font = new Font("Century Gothic", 10, FontStyle.Bold);
            if (colors.Bool1 && Properties.Settings.Default.showScrollbars) showScrollBars();
        }

        private void showScrollBars()
        {
            dataGridView1.ScrollBars = ScrollBars.Both;
            dataGridView2.ScrollBars = ScrollBars.Vertical;
        }

        public class Receipt
        {
            public string Header { get; set; }
            public List<ReceiptItem> Items { get; set; } = new List<ReceiptItem>();
            public double TotalAmount { get; set; }
            public string RestaurantName { get; set; }
            public byte[] RestaurantLogo { get; set; }
            public string ClientName { get; set; }
            public string ClientAddress { get; set; }
        }

        public class ReceiptItem
        {
            public string Name { get; set; }
            public double AmountInGrams { get; set; }
        }
    }
}


