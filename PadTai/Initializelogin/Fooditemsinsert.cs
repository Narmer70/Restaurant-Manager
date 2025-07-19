using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using PadTai.Classes;
using ExcelDataReader;
using System.Data.SQLite;
using System.Windows.Forms;
using PadTai.Classes.Others;
using System.Threading.Tasks;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using PadTai.Sec_daryfolders.Initialize_DB_APP;
using System.Windows.Forms.DataVisualization.Charting;


namespace PadTai.Sec_daryfolders.DB_Appinitialize
{
    public partial class Fooditemsinsert : Form
    {
        private string sqliteConnectionString = DatabaseConnection.GetSQLiteConnectionString();
        ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();
        private DataGridViewScroller scroller;
        private DraggableForm draggableForm;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public Fooditemsinsert()
        {
            InitializeComponent();

            crudDatabase = new CrudDatabase();
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);
           
            draggableForm = new DraggableForm();
            draggableForm.EnableDragging(this, this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(label1);
            resizer.RegisterControl(label2);
            resizer.RegisterControl(label4);
            resizer.RegisterControl(button1);
            resizer.RegisterControl(button2);
            resizer.RegisterControl(button3);
            resizer.RegisterControl(textBox1);
            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(linkLabel2);
            resizer.RegisterControl(customBox1);
            resizer.RegisterControl(dataGridView1);
            dataGridView1.GridColor = colors.Color1;

            scroller = new DataGridViewScroller(this, null, dataGridView1);

            LocalizeControls();
            CenterLabel();
            ApplyTheme();
        }

        private void CenterLabel()
        {
            label4.Left = (this.ClientSize.Width - label4.Width) / 2;
            dataGridView1.Left = (this.ClientSize.Width - dataGridView1.Width) / 2;
        }

        private void InsertFoodItemsDataGridViewData()
        {
            if (dataGridView1.Rows.Count == 0)
            {
                textBox1.Text = LanguageManager.Instance.GetString("Exceldata");
                textBox1.ForeColor = Color.Tomato;
                return;
            }

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(sqliteConnectionString))
                {
                    connection.Open();

                    using (SQLiteCommand command = new SQLiteCommand(@"INSERT INTO FoodItems (FoodID, FoodName, FooditemtypeID, Price, GroupID, SubgroupID, SubsubgroupID, FoodPicture, IsChecked) 
                        VALUES (@FoodID, @FoodName, @FooditemtypeID, @Price, @GroupID, @SubgroupID, @SubsubgroupID, @FoodPicture, @IsChecked);", connection))
                    {

                        using (SQLiteTransaction transaction = connection.BeginTransaction())
                        {
                            command.Transaction = transaction;
                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                if (row.IsNewRow) continue; // Skip the new row at the end of the dgv

                                // --- Data Validation ---
                                // Handle null values
                                if (row.Cells[0].Value == null || row.Cells[1].Value == null ||
                                    row.Cells[2].Value == null || row.Cells[3].Value == null)
                                {
                                    textBox1.Text = LanguageManager.Instance.GetString("Notenoughdata");
                                    textBox1.ForeColor = Color.Tomato;
                                    return;
                                }

                                // Column 0: FoodID (INTEGER PRIMARY KEY) - Should be convertible to integer.  
                                if (!int.TryParse(row.Cells[0].Value.ToString(), out int foodId))
                                {
                                    textBox1.Text = LanguageManager.Instance.GetString("FGI-Colinteger");
                                    textBox1.ForeColor = Color.Tomato;
                                    return;
                                }


                                // Column 1: FoodName (TEXT NOT NULL)
                                string foodName = row.Cells[1].Value?.ToString()?.Trim() ?? "";


                                // Column 2: FooditemtypeID (INTEGER NOT NULL) - Should be convertible to integer.
                                int foodItemTypeId = 0; // Default value
                                if (row.Cells[2].Value != null && !string.IsNullOrWhiteSpace(row.Cells[2].Value.ToString()))
                                {
                                    // Try to parse the value from the cell
                                    if (!int.TryParse(row.Cells[2].Value.ToString(), out foodItemTypeId))
                                    {
                                        textBox1.Text = LanguageManager.Instance.GetString("InvalidFoodID");
                                        textBox1.ForeColor = Color.Tomato;
                                        return;
                                    }
                                }


                                // Column 3: Price (DECIMAL) -  Should be convertible to decimal.
                                if (!decimal.TryParse(row.Cells[3].Value.ToString(), out decimal price))
                                {
                                    textBox1.Text = LanguageManager.Instance.GetString("Invalidprice");
                                    textBox1.ForeColor = Color.Tomato;
                                    return;
                                }

                                // Column 4: GroupID (INTEGER, nullable)
                                int? groupId = null;
                                if (row.Cells[4].Value != null)
                                {
                                    string groupValue = row.Cells[4].Value.ToString().Trim();
                                    if (!string.IsNullOrEmpty(groupValue) && int.TryParse(groupValue, out int parsedGroupId))
                                    {
                                        groupId = parsedGroupId;
                                    }
                                }


                                // Column 5: SubgroupID (INTEGER, nullable)
                                int? subGroupId = null;
                                if (row.Cells[5].Value != null)
                                {
                                    string subGroupValue = row.Cells[5].Value.ToString().Trim();
                                    if (!string.IsNullOrEmpty(subGroupValue) && int.TryParse(subGroupValue, out int parsedSubGroupId))
                                    {
                                        subGroupId = parsedSubGroupId;
                                    }
                                }


                                // Column 6: SubsubgroupID (INTEGER, nullable)
                                int? subSubGroupId = null;
                                if (row.Cells[6].Value != null)
                                {
                                    string subSubGroupValue = row.Cells[6].Value.ToString().Trim();
                                    if (!string.IsNullOrEmpty(subSubGroupValue) && int.TryParse(subSubGroupValue, out int parsedSubSubGroupId))
                                    {
                                        subSubGroupId = parsedSubSubGroupId;
                                    }
                                }
                                // Column 7: FoodPicture (BLOB, nullable) - Placeholder for now.
                                byte[] foodPicture = null; // Assuming you may need to handle images in the future

                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@FoodID", foodId);
                                command.Parameters.AddWithValue("@FoodName", foodName);
                                command.Parameters.AddWithValue("@FooditemtypeID", foodItemTypeId);
                                command.Parameters.AddWithValue("@Price", price);
                                command.Parameters.AddWithValue("@GroupID", groupId == null ? (object)DBNull.Value : groupId);
                                command.Parameters.AddWithValue("@SubgroupID", subGroupId == null ? (object)DBNull.Value : subGroupId);
                                command.Parameters.AddWithValue("@SubsubgroupID", subSubGroupId == null ? (object)DBNull.Value : subSubGroupId);
                                command.Parameters.AddWithValue("@FoodPicture", foodPicture == null ? (object)DBNull.Value : foodPicture);
                                command.Parameters.AddWithValue("@IsChecked", 0);

                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                    }
                    textBox1.Text = LanguageManager.Instance.GetString("Insertsucess");
                    textBox1.ForeColor = Color.LawnGreen;
                    rjButton1.Visible = true;
                    button2.Enabled = true;
                }
            }
            catch (SQLiteException ex)
            {
                textBox1.Text = ex.Message;
                textBox1.ForeColor = Color.Tomato;
            }
            catch (Exception ex)
            {
                textBox1.Text = ex.Message;
                textBox1.ForeColor = Color.Tomato;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void customBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = tableCollection[customBox1.SelectedItem.ToString()];
            dataGridView1.DataSource = dt;

            if (dataGridView1.Columns.Count > 0)
            {
                try
                {
                    // Set widths for existing columns safely
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                dataGridView1.Columns[i].Width = 83;
                            break;
                            case 1:
                                dataGridView1.Columns[i].Width = 350;
                            break;
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                                dataGridView1.Columns[i].Width = 83;
                            break;
                            case 7:
                                if (dataGridView1.Columns.Count > 7) 
                                {
                                    dataGridView1.Columns[i].Width = 72;
                                }
                            break;
                        }
                    }

                    textBox1.Text = LanguageManager.Instance.GetString("Checkdata");
                    textBox1.ForeColor = Color.Tomato;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    // Handle the exception
                    textBox1.Text = LanguageManager.Instance.GetString("Unexpectederror") + ex.Message;
                    textBox1.ForeColor = Color.Tomato;
                }
                catch (Exception ex)
                {
                    // Handle any other exceptions
                    textBox1.Text = LanguageManager.Instance.GetString("Unexpectederror") + ex.Message;
                    textBox1.ForeColor = Color.Tomato;
                }
            }   
        }

        private void Fooditemsinsert_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void Fooditemsinsert_Resize(object sender, EventArgs e)
        {

            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
                CenterLabel();
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Application.Restart();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            InsertFoodItemsDataGridViewData();
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                if (dataGridView1.CurrentCell.RowIndex > 0)
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex - 1].Cells[dataGridView1.CurrentCell.ColumnIndex];
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (dataGridView1.CurrentCell.RowIndex < dataGridView1.Rows.Count - 1)
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex + 1].Cells[dataGridView1.CurrentCell.ColumnIndex];
                }
                e.Handled = true;
            }
        }


        DataTableCollection tableCollection;
        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "Excel Files|*.xls; *.xlsx; *.xlsm" })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = openFileDialog.FileName;
                   
                    try
                    {
                        using (var stream = File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                        {
                            using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                            {
                                DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                                {
                                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = false }
                                });

                                tableCollection = result.Tables;
                                customBox1.Items.Clear();
                                foreach (DataTable table in tableCollection)
                                    customBox1.Items.Add(table.TableName);
                            }
                        }
                    }
                    catch (IOException ex)
                    {
                        textBox1.Text = ex.Message;
                        textBox1.Text = LanguageManager.Instance.GetString("Closefile");
                        textBox1.ForeColor = Color.Tomato;
                    }
                    catch (Exception ex)
                    {
                        textBox1.Text = ex.Message;
                        textBox1.Text = LanguageManager.Instance.GetString("Unexpectederror");
                        textBox1.ForeColor = Color.Tomato;
                    }
                }
            }
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
            string query = "DELETE FROM FoodItems;";

            if (crudDatabase.ExecuteNonQuery(query))
            {
                textBox1.Text = LanguageManager.Instance.GetString("Rowsdeleted");
                rjButton1.Visible = false;
                button2.Enabled = false;
            }
            else
            {
                textBox1.Text = LanguageManager.Instance.GetString("Rowsnotdeleted");
            }
        }

        public void LocalizeControls()
        {
            button1.Text = LanguageManager.Instance.GetString("Parcourir");
            rjButton1.Text = LanguageManager.Instance.GetString("Delete");
            button2.Text = LanguageManager.Instance.GetString("DC-rbtn2");
            button3.Text = LanguageManager.Instance.GetString("Inserer");
            linkLabel2.Text = LanguageManager.Instance.GetString("Skip");
            label1.Text = LanguageManager.Instance.GetString("Filename");
            label4.Text = LanguageManager.Instance.GetString("FII-lbl4");
            label2.Text = LanguageManager.Instance.GetString("Sheets");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;


            label1.ForeColor = colors.Color2;
            label2.ForeColor = colors.Color2;
            label4.ForeColor = colors.Color2;
            textBox1.ForeColor = colors.Color2;
            customBox1.ForeColor = colors.Color2;
            customBox1.BackColor = colors.Color3;
            customBox1.ArrowColor = colors.Color2;
            customBox1.BorderColor = colors.Color5;
            dataGridView1.ForeColor = colors.Color2;
            dataGridView1.BackColor = colors.Color3;
            textBox1.BackColorRounded = colors.Color3;
            dataGridView1.BackgroundColor = colors.Color3;
            dataGridView1.DefaultCellStyle.ForeColor = colors.Color2;
            dataGridView1.DefaultCellStyle.BackColor = colors.Color3;
            dataGridView1.DefaultCellStyle.SelectionForeColor = colors.Color2;
            dataGridView1.DefaultCellStyle.SelectionBackColor = colors.Color3;
        }
    }
}
