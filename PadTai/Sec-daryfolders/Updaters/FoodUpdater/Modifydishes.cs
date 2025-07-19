using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Drawing;
using PadTai.Classes;
using System.Data.SQLite;
using System.Windows.Forms;
using PadTai.Classes.Others;
using System.Data.SqlClient;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using PadTai.Sec_daryfolders.Others;
using PadTai.Classes.Controlsdesign;
using PadTai.Sec_daryfolders.Others_Forms;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;


namespace PadTai.Sec_daryfolders.Updaters.FoodUpdater
{
    public partial class Modifydishes : UserControl
    {
        ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();
        private Clavieroverlay clavieroverlay;
        private DataGridViewScroller scroller;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;
        private DataTable dataTable;
        private string currentState;


        public Modifydishes()
        {
            InitializeComponent();
            InitializeControlResizer();

            crudDatabase = new CrudDatabase();
            dataGridView1.GridColor = colors.Color1;
            LoadComboBoxes(customBox1, customBox2, customBox3, customBox4);
            LocalizeControls();
            ApplyTheme();
            Fetchdata();
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
            resizer.RegisterControl(label6);
            resizer.RegisterControl(label8);
            resizer.RegisterControl(label9);
            resizer.RegisterControl(label12);
            resizer.RegisterControl(label15);
            resizer.RegisterControl(label18);
            resizer.RegisterControl(button1);
            resizer.RegisterControl(SaveWord);
            resizer.RegisterControl(checkBox1);
            resizer.RegisterControl(rjButton1);            
            resizer.RegisterControl(rjButton2);
            resizer.RegisterControl(customBox1);
            resizer.RegisterControl(customBox2);
            resizer.RegisterControl(customBox3);
            resizer.RegisterControl(customBox4);
            resizer.RegisterControl(pictureBox1);
            resizer.RegisterControl(pictureBox2);
            resizer.RegisterControl(roundedPanel1);
            resizer.RegisterControl(roundedPanel2);
            resizer.RegisterControl(roundedPanel3);
            resizer.RegisterControl(dataGridView1);
            resizer.RegisterControl(roundedTextbox1);
            resizer.RegisterControl(roundedTextbox2);
            resizer.RegisterControl(roundedTextbox3);
            resizer.RegisterControl(roundedTextbox4);
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

                if (dataGridView1.ColumnCount > 0) 
                {
                    setColumnWidth();
                }
            }
        }


        private void Alert(string msg, Alertform.enmType type)
        {
            Alertform alertform = new Alertform();
            alertform.showAlert(msg, type);
        }

        private void Fetchdata()
        {
            string query = "SELECT FoodID, FoodName, FooditemtypeID, Price, GroupID, SubgroupID, SubsubgroupID, FoodPicture FROM FoodItems";

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

                dataGridView1.Columns["FoodPicture"].Visible = false;

                if (dataGridView1.Columns.Count > 0)
                {
                    setColumnWidth();
                    RenameDataGridViewColumns();
                }
            }
            else
            {
                // Handle the case where no data is found
                label18.Text = "No groups found.";
                label18.ForeColor = Color.Tomato;
            }       
        }

        private void setColumnWidth()
        {
            dataGridView1.RowTemplate.Height = 30;
            dataGridView1.Columns[3].DefaultCellStyle.Format = "F2";
            dataGridView1.Columns[0].Width = (int)(dataGridView1.Width * 0.05);
            dataGridView1.Columns[1].Width = (int)(dataGridView1.Width * 0.37);
            dataGridView1.Columns[2].Width = (int)(dataGridView1.Width * 0.08);
            dataGridView1.Columns[3].Width = (int)(dataGridView1.Width * 0.10);
            dataGridView1.Columns[4].Width = (int)(dataGridView1.Width * 0.08);
            dataGridView1.Columns[5].Width = (int)(dataGridView1.Width * 0.08);
            dataGridView1.Columns[6].Width = (int)(dataGridView1.Width * 0.08);
            dataGridView1.Columns[7].Width = (int)(dataGridView1.Width * 0.00);
            dataGridView1.Columns[8].Width = (int)(dataGridView1.Width * 0.08); 
            dataGridView1.Columns[9].Width = (int)(dataGridView1.Width * 0.08); 
            dataGridView1.Columns[1].DefaultCellStyle.Padding = new Padding(20, 0, 0, 0);
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; 
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void RenameDataGridViewColumns()
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            if (dataGridView1.Columns.Count > 0)
            {
                //dataGridView1.Columns[0].Name = "ID";
                //dataGridView1.Columns[1].Name = "Name";
                //dataGridView1.Columns[2].Name = "Type";
                //dataGridView1.Columns[3].Name = "Price";
                //dataGridView1.Columns[4].Name = "Group";
                //dataGridView1.Columns[5].Name = "Sgroup";
                //dataGridView1.Columns[6].Name = "Ssgroup";
                dataGridView1.Columns[0].HeaderText = "ID";
                dataGridView1.Columns[1].HeaderText = "Name";
                dataGridView1.Columns[2].HeaderText = "Type";
                dataGridView1.Columns[3].HeaderText = "Price";
                dataGridView1.Columns[4].HeaderText = "Group";
                dataGridView1.Columns[5].HeaderText = "Sgroup";
                dataGridView1.Columns[6].HeaderText = "Ssgroup";
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

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == dataGridView1.Columns[4].Index)
            {
                var currentCellValue = dataGridView1.CurrentCell.Value;

                // Cast the editing control to a ComboBox
                customBox comboBox = e.Control as customBox;

                if (comboBox != null)
                {   
                    comboBox.Items.Clear();
                    comboBox.ForeColor = this.ForeColor;
                    comboBox.BackColor = this.BackColor;

                    crudDatabase.LoadGroups();
                    comboBox.DataSource = crudDatabase.Groups;
                    comboBox.DisplayMember = "GroupName";
                    comboBox.ValueMember = "GroupID";

                    if (currentCellValue != null && !string.IsNullOrEmpty(currentCellValue.ToString()))
                    {
                        comboBox.SelectedValue = currentCellValue;
                    }
                    else
                    {
                        comboBox.SelectedIndex = -1; 
                    }
                    comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                }
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
                    dataView.RowFilter = $"FoodName LIKE '%{filterExpression}%'";
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
                        button1.Visible = false;
                        SaveWord.Visible = false;
                        checkBox1.Visible = true;
                        checkBox1.Checked = false;
                        customBox1.Enabled = true;
                        customBox2.Enabled = true;
                        customBox3.Enabled = true;
                        customBox4.Enabled = true;
                        label18.Text = string.Empty;
                        roundedPanel1.Enabled = false;
                        roundedTextbox2.ReadOnly = true;
                        roundedTextbox3.ReadOnly = false;
                        roundedTextbox4.ReadOnly = false;
                        rjButton2.Text = "Update the dish";
                        rjButton2.BackColor = Color.DodgerBlue;
                        rjButton1.BackColor = roundedPanel1.BackColor;
                        roundedTextbox6.Text = string.Empty;
                        roundedTextbox5.Text = string.Empty;
                        roundedTextbox2.WaterMark = "Optional";
                        roundedTextbox6.WaterMark = "Dish type";
                        roundedTextbox5.WaterMark = "Classification";
                        LoadFoodItemDetails(selectedId.ToString());
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
                        button1.Visible = false;
                        SaveWord.Visible = false;
                        checkBox1.Visible = false;
                        customBox1.Enabled = false;
                        customBox2.Enabled = false;
                        customBox3.Enabled = false;
                        customBox4.Enabled = false;
                        label18.Text = string.Empty;
                        roundedPanel1.Enabled = false;
                        roundedTextbox2.ReadOnly = true;
                        roundedTextbox3.ReadOnly = true;
                        roundedTextbox4.ReadOnly = true;
                        rjButton2.BackColor = Color.Crimson;
                        rjButton2.Text = "Delete the dish";
                        rjButton1.BackColor = roundedPanel1.BackColor;
                        roundedTextbox6.Text = string.Empty;
                        roundedTextbox5.Text = string.Empty;
                        roundedTextbox2.WaterMark = "Optional";
                        roundedTextbox6.WaterMark = "Dish type";
                        roundedTextbox5.WaterMark = "Classification";
                        LoadFoodItemDetails(selectedId.ToString());
                        roundedPanel3.Visible = true;
                    }
                }
            }
        }


        public void Export_Data_To_Word(DataGridView DGV, string filename)
        {
            if (DGV.Rows.Count != 0)
            {
                int RowCount = DGV.Rows.Count;
                int ColumnCount = Math.Min(DGV.Columns.Count, 7);
                Object[,] DataArray = new object[RowCount, ColumnCount];

                for (int r = 0; r < RowCount; r++)
                {
                    for (int c = 0; c < ColumnCount; c++)
                    {
                        DataArray[r, c] = DGV.Rows[r].Cells[c].Value;
                    } 
                } 

                Word.Document oDoc = new Word.Document();
                oDoc.Application.Visible = true;

                oDoc.PageSetup.Orientation = Word.WdOrientation.wdOrientLandscape;

                dynamic oRange = oDoc.Content.Application.Selection.Range;
                string oTemp = "";
                for (int r = 0; r < RowCount; r++)
                {
                    for (int c = 0; c < ColumnCount; c++)
                    {
                        oTemp += DataArray[r, c] + "\t";
                    }
                }

                oRange.Text = oTemp;

                object Separator = Word.WdTableFieldSeparator.wdSeparateByTabs;
                object ApplyBorders = true;
                object AutoFit = true;
                object AutoFitBehavior = Word.WdAutoFitBehavior.wdAutoFitContent;

                oRange.ConvertToTable(ref Separator, ref RowCount, ref ColumnCount,
                                      Type.Missing, Type.Missing, ref ApplyBorders,
                                      Type.Missing, Type.Missing, Type.Missing,
                                      Type.Missing, Type.Missing, Type.Missing,
                                      Type.Missing, ref AutoFit, ref AutoFitBehavior, Type.Missing);

                System.Threading.Thread.Sleep(100); 

                oRange.Select();

                var table = oDoc.Application.ActiveDocument.Tables[1];
                table.Rows.AllowBreakAcrossPages = 0;
                table.Rows.Alignment = 0;

                try
                {
                    table.set_Style("Grid Table 5 Dark - Accent 2");

                    Word.Section section = oDoc.Application.ActiveDocument.Sections[1]; 
                    Word.Range headerRange = section.Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                    headerRange.Fields.Add(headerRange, Word.WdFieldType.wdFieldPage);
                    headerRange.Text = "Your Header Text";
                    headerRange.Font.Size = 16;
                    headerRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                    oDoc.SaveAs2(filename);
                    this.Alert("Saved to MS.word. Success!", Alertform.enmType.Success);

                }
                catch (Exception)
                {
                    this.Alert("Error saving to MS.word!", Alertform.enmType.Error);
                }
            }
            else
            {
                this.Alert("No record to export.", Alertform.enmType.Warning);
            }
        }
        
        
        private void SaveWord_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "Word Documents (*.docx)|*.docx";

            sfd.FileName = "Export.docx";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Export_Data_To_Word(dataGridView1, sfd.FileName);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel (.xlsx)|*.xlsx";
                sfd.FileName = "Output.xlsx";
                bool fileError = false;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(sfd.FileName))
                    {
                        try
                        {
                            File.Delete(sfd.FileName);
                        }
                        catch (IOException)
                        {
                            fileError = true;
                            this.Alert("An error occurred", Alertform.enmType.Error);
                        }
                    }
                    if (!fileError)
                    {
                        try
                        {
                            Excel.Application XcelApp = new Excel.Application();
                            Excel._Workbook workbook = XcelApp.Workbooks.Add(Type.Missing);
                            Excel._Worksheet worksheet = null;

                            worksheet = workbook.Sheets["Sheet1"];
                            worksheet = workbook.ActiveSheet;
                            worksheet.Name = "Output";
                            worksheet.Application.ActiveWindow.SplitRow = 1;
                            worksheet.Application.ActiveWindow.FreezePanes = true;

                            int columnCount = Math.Min(dataGridView1.Columns.Count, 7); // Limit to the first 7 columns

                            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                            {
                                for (int j = 0; j < columnCount; j++)
                                {
                                    if (dataGridView1.Rows[i].Cells[j].Value != null)
                                    {
                                        worksheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                                    }
                                    else
                                    {
                                        worksheet.Cells[i + 2, j + 1] = string.Empty; // Or any default value you prefer
                                    }
                                }

                            }

                            worksheet.Columns.AutoFit();
                            workbook.SaveAs(sfd.FileName);
                            XcelApp.Quit();

                            ReleaseObject(worksheet);
                            ReleaseObject(workbook);
                            ReleaseObject(XcelApp);

                            this.Alert("Saved to MS.Excel!", Alertform.enmType.Success);
                        }
                        catch (Exception)
                        {
                            this.Alert("Error saving to MS.Excel!", Alertform.enmType.Error);
                        }
                    }
                }
            }
            else
            {
                this.Alert("No record to export.", Alertform.enmType.Warning);
            }
        }

        private void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception)
            {
                obj = null;
                this.Alert("An error occurred", Alertform.enmType.Error);
            }
            finally
            {
                GC.Collect();
            }
        }


        #region ADD DISH
        public void LoadComboBoxes(ComboBox customBox1, ComboBox customBox2, ComboBox customBox3, ComboBox customBox4)
        {
            try
            {
                crudDatabase.LoadGroups();
                customBox4.DataSource = crudDatabase.Groups;
                customBox4.DisplayMember = "GroupName";
                customBox4.ValueMember = "GroupID";
                customBox4.SelectedIndex = -1;

                crudDatabase.LoadFoodItemTypes();
                customBox1.DataSource = crudDatabase.FoodItemTypes;
                customBox1.DisplayMember = "FooditemtypeName";
                customBox1.ValueMember = "FooditemtypeID";
                customBox1.SelectedIndex = -1;

                crudDatabase.LoadSubgroups();
                customBox2.DataSource = crudDatabase.Subgroups;
                customBox2.DisplayMember = "SubgroupName";
                customBox2.ValueMember = "SubgroupID";
                customBox2.SelectedIndex = -1;

                crudDatabase.LoadSubsubgroups();
                customBox3.DataSource = crudDatabase.Subsubgroups;
                customBox3.DisplayMember = "SubsubgroupName";
                customBox3.ValueMember = "SubsubgroupID";
                customBox3.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public void InsertFoodItem(int? foodID, string foodName, decimal price, ComboBox customBox1, ComboBox customBox2, ComboBox customBox3, ComboBox customBox4, PictureBox foodPictureBox)
        {
            // Input validation
            if (customBox1.SelectedValue == null)
            {
                label18.Text = "Please select a food item type.";
                label18.ForeColor = Color.Tomato;
                return;
            }
            if (customBox2.SelectedValue == null && customBox3.SelectedValue == null && customBox4.SelectedValue == null)
            {
                label18.Text = "Please select a classification (Group, Subgroup, or Subsubgroup).";
                label18.ForeColor = Color.Tomato;
                return;
            }

            try
            {
                // Convert the image in the PictureBox to a byte array
                byte[] foodPicture = null;

                if (foodPictureBox.Image != null && !crudDatabase.IsSpecificImage(foodPictureBox.Image, Properties.Resources.image_add_13434878))
                {
                    using (var ms = new MemoryStream())
                    {
                        foodPictureBox.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        foodPicture = ms.ToArray();
                    }
                }

                // Get the selected values from the ComboBoxes
                int? foodItemTypeId = customBox1.SelectedValue as int?;
                int? groupId = customBox4.SelectedValue as int?;
                int? subgroupId = customBox2.SelectedValue as int?;
                int? subsubgroupId = customBox3.SelectedValue as int?;

                // Prepare the SQL command to insert the FoodItem without foodID
                var sql = @"INSERT INTO FoodItems (FoodName, Price, FooditemtypeID, GroupID, SubgroupID, SubsubgroupID, FoodPicture, IsChecked) 
                             VALUES (@FoodName, @Price, @FooditemtypeID, @GroupID, @SubgroupID, @SubsubgroupID, @FoodPicture, 0)";

                // Prepare the parameters for the query
                var parameters = new Dictionary<string, object>
                {
                    { "@FoodName", foodName },
                    { "@Price", price },
                    { "@FooditemtypeID", foodItemTypeId.HasValue ? (object)foodItemTypeId.Value : DBNull.Value },
                    { "@GroupID", groupId.HasValue ? (object)groupId.Value : DBNull.Value },
                    { "@SubgroupID", subgroupId.HasValue ? (object)subgroupId.Value : DBNull.Value },
                    { "@SubsubgroupID", subsubgroupId.HasValue ? (object)subsubgroupId.Value : DBNull.Value },
                    { "@FoodPicture", foodPicture ?? (object)DBNull.Value }
                };

                // Call the InsertData method from CrudDatabase
                if (crudDatabase.ExecuteNonQuery(sql, parameters))
                {
                    Fetchdata();
                    clearControls();
                    label18.Text = "Dish added successfully";
                    label18.ForeColor = Color.LawnGreen;
                    this.Alert("Dish added. Success!", Alertform.enmType.Success);
                }
                else
                {
                    label18.Text = "Failed to add dish.";
                    label18.ForeColor = Color.Tomato;
                }
            }
            catch (Exception ex)
            {
                label18.Text = ex.Message;
                label18.ForeColor = Color.Tomato;
            }
        }

        private void addaDish() 
        {
            // Validate foodID input
            int? foodID = null;
            if (!string.IsNullOrWhiteSpace(roundedTextbox2.Text))
            {
                if (!int.TryParse(roundedTextbox2.Text, out int parsedFoodID)) { }

                foodID = parsedFoodID;
            }

            string foodName = roundedTextbox4.Text;
            if (string.IsNullOrWhiteSpace(foodName))
            {
                label18.Text = "Food name cannot be empty.";
                label18.ForeColor = Color.Tomato;
                return;
            }

            decimal price;
            if (!decimal.TryParse(roundedTextbox3.Text, out price))
            {
                label18.Text = "Please enter a valid price.";
                label18.ForeColor = Color.Tomato;
                return;
            }

            // Call InsertFoodItem with the updated ComboBox names
            InsertFoodItem(foodID, foodName, price, customBox1, customBox2, customBox3, customBox4, pictureBox2);
        }
        #endregion

        #region UPDATE DISH
        private void LoadFoodItemDetails(string foodId)
        {
            string query = $"SELECT FoodID, FoodName, FooditemtypeID, Price, GroupID, SubgroupID, SubsubgroupID, FoodPicture FROM FoodItems WHERE FoodID = {foodId}";

            DataTable foodItemDetails = crudDatabase.FetchDataFromDatabase(query);

            if (foodItemDetails.Rows.Count > 0)
            {
                roundedTextbox2.Text = foodItemDetails.Rows[0]["FoodID"].ToString();
                roundedTextbox4.Text = foodItemDetails.Rows[0]["FoodName"].ToString();

                int foodItemTypeId = Convert.ToInt32(foodItemDetails.Rows[0]["FooditemtypeID"]);
                customBox1.SelectedValue = foodItemTypeId;

                roundedTextbox3.Text = foodItemDetails.Rows[0]["Price"] != DBNull.Value ? Convert.ToDecimal(foodItemDetails.Rows[0]["Price"]).ToString("F2") : "0.00";

                if (foodItemDetails.Rows[0]["GroupID"] != DBNull.Value)
                {
                    int groupId = Convert.ToInt32(foodItemDetails.Rows[0]["GroupID"]);
                    customBox4.SelectedValue = groupId;
                }
                else if (foodItemDetails.Rows[0]["SubgroupID"] != DBNull.Value)
                {
                    int subgroupId = Convert.ToInt32(foodItemDetails.Rows[0]["SubgroupID"]);
                    customBox2.SelectedValue = subgroupId;
                }
                else if (foodItemDetails.Rows[0]["SubsubgroupID"] != DBNull.Value)
                {
                    int subsubgroupId = Convert.ToInt32(foodItemDetails.Rows[0]["SubsubgroupID"]);
                    customBox3.SelectedValue = subsubgroupId;
                }
                else
                {
                    customBox2.Text = string.Empty;
                    customBox3.Text = string.Empty;
                    customBox4.Text = string.Empty;
                }

                byte[] imageData = foodItemDetails.Rows[0]["FoodPicture"] as byte[];

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
                roundedTextbox1.Text = "No details found.";
                roundedTextbox2.Text = "No details found.";
                roundedTextbox3.Text = "No details found.";
                customBox1.Text = "No details found.";
                customBox2.Text = "No details found.";
                customBox3.Text = "No details found.";
                customBox4.Text = "No details found.";
                pictureBox1.Image = null;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) 
            {
                DeleteFoodPic();
            }
        }

        private void DeleteFoodPic()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Get the selected row
                var selectedRow = dataGridView1.SelectedRows[0];
                var selectedId = selectedRow.Cells[0].Value;

                if (selectedId != null)
                {
                    // Construct the SQL query to update the FoodPicture
                    string query = $"UPDATE FoodItems SET FoodPicture = NULL WHERE FoodID = {selectedId}";

                    if (crudDatabase.ExecuteNonQuery(query))
                    {
                        // Disable buttons or perform other UI updates
                        pictureBox2.Image = Properties.Resources.image_add__1_;
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

        public void UpdateSelectedFoodItem()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Get the selected row
                var selectedRow = dataGridView1.SelectedRows[0];
                var selectedId = selectedRow.Cells[0].Value;

                if (selectedId != null)
                {
                    // Prepare the update query
                    string updateQuery = @"UPDATE FoodItems SET FoodName = @foodName, FooditemtypeID = @foodItemTypeId, Price = @price, GroupID = @groupId, 
                                            SubgroupID = @subgroupId, SubsubgroupID = @subsubgroupId, FoodPicture = @foodPicture WHERE FoodID = @selectedId";

                    // Input validation
                    if (customBox1.SelectedValue == null)
                    {
                        label18.Text = "Please select a food item type.";
                        label18.ForeColor = Color.Tomato;
                        return;
                    }
                    if (customBox2.SelectedValue == null && customBox3.SelectedValue == null && customBox4.SelectedValue == null)
                    {
                        label18.Text = "Please select at least one classification (Group, Subgroup, or Subsubgroup).";
                        label18.ForeColor = Color.Tomato;
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(roundedTextbox4.Text))
                    {
                        label18.Text = "Food name cannot be empty.";
                        label18.ForeColor = Color.Tomato;
                        return;
                    }

                    // Get values from text boxes and combo boxes
                    string foodName = roundedTextbox4.Text.Trim();
                    int foodItemTypeId = (int)customBox1.SelectedValue;

                    int? groupId = customBox4.SelectedValue != null ? (int?)customBox4.SelectedValue : null;
                    int? subgroupId = customBox2.SelectedValue != null ? (int?)customBox2.SelectedValue : null;
                    int? subsubgroupId = customBox3.SelectedValue != null ? (int?)customBox3.SelectedValue : null;

                    byte[] foodPicture = null;

                    // Check if the PictureBox has an image
                    if (pictureBox2.Image != null && !crudDatabase.IsSpecificImage(pictureBox2.Image, Properties.Resources.image_add__1_))
                    {
                        using (var ms = new MemoryStream())
                        {
                            pictureBox2.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            foodPicture = ms.ToArray();
                        }
                    }

                    decimal? price = null;

                    // Validate price input
                    if (!string.IsNullOrWhiteSpace(roundedTextbox3.Text))
                    {
                        if (decimal.TryParse(roundedTextbox3.Text, out decimal parsedPrice))
                        {
                            price = parsedPrice;
                        }
                        else
                        {
                            label18.Text = "Please enter a valid number for price.";
                            label18.ForeColor = Color.Tomato;
                            return;
                        }
                    }
                    else
                    {
                        label18.Text = "Input cannot be empty. Please enter a price.";
                        label18.ForeColor = Color.Tomato;
                        return;
                    }

                    // Prepare parameters for the update
                    var parameters = new Dictionary<string, object>
                    {
                         { "@foodName", foodName },
                         { "@foodItemTypeId", foodItemTypeId },
                         { "@price", price },
                         { "@groupId", (object)groupId ?? DBNull.Value },
                         { "@subgroupId", (object)subgroupId ?? DBNull.Value },
                         { "@subsubgroupId", (object)subsubgroupId ?? DBNull.Value },
                         { "@foodPicture", (object)foodPicture ?? DBNull.Value },
                         { "@selectedId", selectedId }
                    };

                    // Execute the update command using ExecuteNonQuery
                    if (crudDatabase.ExecuteNonQuery(updateQuery, parameters))
                    {
                        Fetchdata();
                        clearControls();
                        button1.Visible = true;
                        SaveWord.Visible = true;
                        rjButton1.Text = "Add dish";
                        roundedPanel1.Enabled = true;
                        roundedPanel3.Visible = false;
                        rjButton1.BackColor = SystemColors.Highlight;
                        this.Alert("Dish updated successfully!", Alertform.enmType.Success);
                    }
                    else
                    {
                        label18.Text = "No item found to update.";
                        label18.ForeColor = Color.Tomato;
                    }
                }
            }
            else
            {
                label18.Text = "Please select an item to update.";
                label18.ForeColor = Color.Tomato;
            }
        }

        private void clearControls()
        {
            pictureBox2.Image = null;
            customBox1.SelectedValue = -1;
            customBox2.SelectedValue = -1;
            customBox3.SelectedValue = -1;
            customBox4.SelectedValue = -1;
            roundedTextbox5.Text = string.Empty;
            roundedTextbox2.Text = string.Empty;
            roundedTextbox3.Text = string.Empty;
            roundedTextbox4.Text = string.Empty;
            roundedTextbox6.Text = string.Empty;
          
            if (currentState == "Add") 
            {
                roundedTextbox2.WaterMark = "Optional";
                roundedTextbox6.WaterMark = "Dish type";
                roundedTextbox5.WaterMark = "Classification";
                pictureBox2.Image = Properties.Resources.image_add_13434878;
            }
        }
        #endregion

        #region DELETE DISH
        public void DeleteSelectedFoodItem()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Get the selected row
                var selectedRow = dataGridView1.SelectedRows[0];
                var selectedId = selectedRow.Cells[0].Value;

                if (selectedId != null)
                {
                    string deleteQuery = "DELETE FROM FoodItems WHERE FoodID = @selectedId";

                    var parameters = new Dictionary<string, object>
                    {
                        { "@selectedId", selectedId }
                    };

                    // Execute the delete command using ExecuteNonQuery
                    if (crudDatabase.ExecuteNonQuery(deleteQuery, parameters))
                    {
                        Fetchdata();
                        clearControls();
                        button1.Visible = true;
                        SaveWord.Visible = true;
                        roundedPanel3.Visible = false;
                        roundedPanel1.Enabled = true;
                        rjButton1.Text = "Add dish";
                        rjButton1.BackColor = SystemColors.Highlight;
                        this.Alert("Dish deleted successfully!", Alertform.enmType.Success);
                    }
                    else
                    {
                        label18.Text = "No item found to delete.";
                        label18.ForeColor = Color.Tomato;
                    }
                }
            }
            else
            {
                label18.Text = "Please select an item to delete.";
                label18.ForeColor = Color.Tomato;
            }
        }
        #endregion

        #region SHARED COMPONENTS 

        private void customBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (customBox4.SelectedItem != null)
            {
                customBox2.SelectedIndex = -1;
                customBox3.SelectedIndex = -1;
                roundedTextbox5.Text = customBox4.Text;
            }
        }

        private void customBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (customBox2.SelectedItem != null)
            {
                customBox3.SelectedIndex = -1;
                customBox4.SelectedIndex = -1;
                roundedTextbox5.Text = customBox2.Text;
            }
        }

        private void customBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (customBox3.SelectedItem != null)
            {
                customBox2.SelectedIndex = -1;
                customBox4.SelectedIndex = -1;
                roundedTextbox5.Text = customBox3.Text;
            }
        }

        private void customBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (customBox1.SelectedItem != null)
            {
                roundedTextbox6.Text = customBox1.Text;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (currentState != null && currentState != "Delete") 
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*";
                    openFileDialog.Title = "Select an Image";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            pictureBox2.Image = new Bitmap(openFileDialog.FileName);
                        }
                        catch (Exception ex)
                        {
                            label18.Text = "Error loading image: " + ex.Message;
                            label18.ForeColor = Color.Tomato;
                        }
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
                button1.Visible = false;
                SaveWord.Visible = false;
                checkBox1.Visible = false;
                customBox1.Enabled = true;
                customBox2.Enabled = true;
                customBox3.Enabled = true;
                customBox4.Enabled = true;
                label18.Text = string.Empty;
                roundedPanel3.Visible = true;
                roundedPanel1.Enabled = false;
                rjButton2.Text = "Add a dish";
                roundedTextbox2.ReadOnly = true;
                roundedTextbox3.ReadOnly = false;
                roundedTextbox4.ReadOnly = false;
                rjButton2.BackColor = Color.Green;
                rjButton1.BackColor = roundedPanel1.BackColor;
                pictureBox2.Image = Properties.Resources.image_add_13434878;
                roundedTextbox6.Text = string.Empty;
                roundedTextbox5.Text = string.Empty;
                roundedTextbox2.WaterMark = "Optional";
                roundedTextbox6.WaterMark = "Dish type";
                roundedTextbox5.WaterMark = "Classification";
            }
            else if (roundedPanel3.Visible == true)
            {
                clearControls();
                roundedPanel3.Visible = false;
                roundedPanel1.Enabled = true;
                button1.Visible = true;
                SaveWord.Visible = true;
                rjButton1.Text = "Add dish";
                rjButton1.BackColor = SystemColors.Highlight;
            }
        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            if (currentState == "Update")
            {
                UpdateSelectedFoodItem();
            }
            else if (currentState == "Delete")
            {
                using (Deletedishconfirm DDC = new Deletedishconfirm())
                {
                    FormHelper.ShowFormWithOverlay(this.FindForm(), DDC);

                    // Check the DialogResult after the dialog is closed
                    if (DDC.DialogResult == DialogResult.OK)
                    {
                        DeleteSelectedFoodItem();
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
                addaDish();
            }
            else
            {
                roundedPanel3.Visible = false;
            }
        }

        private void customBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
        #endregion

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

        private void Foodlistdisplay_Click(object sender, EventArgs e)
        {
            if (clavieroverlay != null && clavieroverlay.Visible == true)
            {
                clavieroverlay.Visible = false;
            }
            dataGridView1.Focus();
        }

    
        public void LocalizeControls()
        {
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
        }

        public void ApplyTheme()
        {
            this.BackColor = colors.Color1; 
            this.ForeColor = colors.Color2; 

            customBox1.BackColor = colors.Color1;
            customBox2.BackColor = colors.Color1;
            customBox3.BackColor = colors.Color1;
            customBox4.BackColor = colors.Color1;

            customBox1.ForeColor = colors.Color2;
            customBox2.ForeColor = colors.Color2;
            customBox3.ForeColor = colors.Color2;
            customBox4.ForeColor = colors.Color2;

            roundedPanel1.BackColor = colors.Color3;
            roundedPanel2.BackColor = colors.Color3;
            roundedPanel3.BackColor = colors.Color3;

            roundedTextbox1.ForeColor = colors.Color2;
            roundedTextbox2.ForeColor = colors.Color2;
            roundedTextbox3.ForeColor = colors.Color2;
            roundedTextbox4.ForeColor = colors.Color2;
            roundedTextbox5.ForeColor = colors.Color2;
            roundedTextbox6.ForeColor = colors.Color2;

            roundedTextbox2.BackColorRounded = colors.Color1;
            roundedTextbox3.BackColorRounded = colors.Color1;
            roundedTextbox4.BackColorRounded = colors.Color1;
            roundedTextbox5.BackColorRounded = colors.Color1;
            roundedTextbox6.BackColorRounded = colors.Color1;
  

            pictureBox1.Image = colors.Image1; 
            button1.BackColor = colors.Color3;
            SaveWord.BackColor = colors.Color3;
            button1.ForeColor = this.ForeColor;
            button1.BorderColor = colors.Color3;
            SaveWord.ForeColor = this.ForeColor;
            rjButton1.ForeColor = this.ForeColor;          
            SaveWord.BorderColor = colors.Color3;
            pictureBox1.BackColor = colors.Color3;
            customBox1.ArrowColor = this.ForeColor;
            customBox2.ArrowColor = this.ForeColor;
            customBox3.ArrowColor = this.ForeColor;
            customBox4.ArrowColor = this.ForeColor;
            dataGridView1.ForeColor = this.ForeColor;
            roundedPanel1.BorderColor = colors.Color3;
            roundedTextbox1.BorderColor = colors.Color3;
            dataGridView1.BackgroundColor = colors.Color3;
            roundedTextbox1.BackColorRounded = colors.Color1;
            dataGridView1.DefaultCellStyle.BackColor = colors.Color3;
            dataGridView1.DefaultCellStyle.ForeColor = this.ForeColor;
            dataGridView1.DefaultCellStyle.SelectionForeColor = this.ForeColor;
            if (colors.Bool1 && Properties.Settings.Default.showScrollbars) showScrollBars();

            //Image resizedImage1 = ImageResizer.ResizeImage(Properties.Resources.ods_9496661, SaveWord.Width, SaveWord.Height);
            //SaveWord.Image = resizedImage1;
            //SaveWord.ImageAlign = ContentAlignment.MiddleLeft;
            //SaveWord.TextImageRelation = TextImageRelation.Overlay;

            //Image resizedImage2 = ImageResizer.ResizeImage(Properties.Resources.xls_9496456, button1.Width, button1.Height);
            //button1.Image = resizedImage2;
            //button1.ImageAlign = ContentAlignment.MiddleRight;
            //button1.TextImageRelation = TextImageRelation.Overlay;
        }

        private void showScrollBars()
        {
            dataGridView1.ScrollBars = ScrollBars.Vertical;
        }
    }
}
