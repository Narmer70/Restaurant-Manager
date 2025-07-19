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
using System.ComponentModel;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using PadTai.Sec_daryfolders.DB_Appinitialize;


namespace PadTai.Sec_daryfolders.Initialize_DB_APP
{
    public partial class Fooditemtypeinsert : Form
    {
        ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();
        private DataGridViewScroller scroller;
        private DraggableForm draggableForm;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public Fooditemtypeinsert()
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
            ApplyTheme();
        }


        private void CenterLabel()
        {
            label4.Left = (this.ClientSize.Width - label4.Width) / 2;
            dataGridView1.Left = (this.ClientSize.Width - dataGridView1.Width) / 2;
        }

        private void Fooditemtypeinsert_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
                CenterLabel();
            }
        }

        private void Fooditemtypeinsert_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
                CenterLabel();
            }
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

        private void customBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = tableCollection[customBox1.SelectedItem.ToString()];
            dataGridView1.DataSource = dt;

            if (dataGridView1.Columns.Count > 0)
            {
                try 
                {
                    if (dataGridView1.Columns.Count > 0)
                    {
                        dataGridView1.Columns[0].Width = 300;
                    }

                    if (dataGridView1.Columns.Count > 1)
                    {
                        dataGridView1.Columns[1].Width = 548;
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

        private void InsertDataGridViewData()
        {
            if (dataGridView1.Rows.Count == 0)
            {
                textBox1.Text = LanguageManager.Instance.GetString("Exceldata");
                textBox1.ForeColor = Color.Tomato;
                return;
            }

            try
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow) continue; // Skip the new row at the end of the dgv

                    // Validate the data in the first and second columns
                    if (row.Cells[0].Value == null || row.Cells[1].Value == null)
                    {
                        textBox1.Text = LanguageManager.Instance.GetString("Failedinsert");
                        textBox1.ForeColor = Color.Tomato;
                        return;
                    }

                    if (!int.TryParse(row.Cells[0].Value.ToString(), out int fooditemtypeID))
                    {
                        textBox1.Text = LanguageManager.Instance.GetString("FGI-Colinteger");
                        textBox1.ForeColor = Color.Tomato;
                        return;
                    }
                    string fooditemtypeName = row.Cells[1].Value?.ToString() ?? "";

                    // Prepare the parameters for the query
                    var parameters = new Dictionary<string, object>
                    {
                        { "@FooditemtypeID", fooditemtypeID },
                        { "@FooditemtypeName", fooditemtypeName }
                    };

                    // Use the ExecuteNonQuery method to insert the data
                    string query = "INSERT INTO FoodItemsTypes (FooditemtypeID, FooditemtypeName) VALUES (@FooditemtypeID, @FooditemtypeName);";
                    if (!crudDatabase.ExecuteNonQuery(query, parameters))
                    {
                        textBox1.Text = LanguageManager.Instance.GetString("Failedinsert");
                        textBox1.ForeColor = Color.Tomato;
                        return;
                    }
                }

                textBox1.Text = LanguageManager.Instance.GetString("Insertsucess");
                textBox1.ForeColor = Color.LawnGreen;
                rjButton1.Visible = true;
                button2.Enabled = true;
            }
            catch (Exception ex)
            {
                textBox1.Text = ex.Message;
                textBox1.ForeColor = Color.Tomato;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            InsertDataGridViewData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Fooditemsinsert fooditemsinsert = new Fooditemsinsert();
            fooditemsinsert.ShowDialog();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Application.Restart();
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
            string query = "DELETE FROM FoodItemsTypes;";
          
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
            label4.Text = LanguageManager.Instance.GetString("FIIT-lbl4");
            label2.Text = LanguageManager.Instance.GetString("Sheets");
        }

        public void ApplyTheme()
        {
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
