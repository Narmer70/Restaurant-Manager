using PadTai.Classes;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Emit;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Word = Microsoft.Office.Interop.Word;

namespace PadTai.Security
{
    public partial class FoodItemsSave : Form
    {
        string connectionString = DatabaseConnection.GetConnection().ConnectionString;
        private DraggableForm draggableForm;
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public FoodItemsSave()
        {
            InitializeComponent();

            fontResizer = new FontResizer();

            fontResizer.AdjustFont(this);
            draggableForm = new DraggableForm();
            draggableForm.EnableDragging(this);
            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(Fetch);
            resizer.RegisterControl(button1);
            resizer.RegisterControl(SaveWord);
            resizer.RegisterControl(dataGridView1);
    
            //  resizer.RegisterControl(button15);
            //   resizer.RegisterControl(button16);

            this.Resize += FoodItemsSave_Resize;
            this.Load += FoodItemsSave_Load;

        }

        private void Fetch_Click(object sender, EventArgs e)
        {
            string sql = "SELECT * FROM FoodItems";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter dataadapter = new SqlDataAdapter(sql, connection);
                DataSet ds = new DataSet();

                try
                {
                    connection.Open();
                    dataadapter.Fill(ds, "FoodItems_table");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message);
                    return; // Exit the method if an error occurs
                }
                finally
                {
                    connection.Close();
                }

                // Bind the DataSet to the DataGridView
                dataGridView1.DataSource = ds;
                dataGridView1.DataMember = "FoodItems_table";

                // Set specific widths for each column in the DataGridView
                // Adjust the widths according to your requirements
                if (dataGridView1.Columns.Count > 0) // Check if there are columns
                {
                    dataGridView1.Columns[0].Width = 120; // Width for the first column
                    dataGridView1.Columns[1].Width = 620;// Width for the seconpolumn
                    dataGridView1.Columns[2].Width = 145; // Width for the third column
                    dataGridView1.Columns[3].Width = 120; // Add more as needed for additional columns
                }
            }
        }

        public void Export_Data_To_Word(DataGridView DGV, string filename)
        {
            if (DGV.Rows.Count != 0)
            {
                int RowCount = DGV.Rows.Count;
                int ColumnCount = DGV.Columns.Count;
                Object[,] DataArray = new object[RowCount + 1, ColumnCount + 1];

                //add rows
                int r = 0;
                for (int c = 0; c <= ColumnCount - 1; c++)
                {
                    for (r = 0; r <= RowCount - 1; r++)
                    {
                        DataArray[r, c] = DGV.Rows[r].Cells[c].Value;
                    } //end row loop
                } //end column loop

                Word.Document oDoc = new Word.Document();
                oDoc.Application.Visible = true;

                //page orintation
                oDoc.PageSetup.Orientation = Word.WdOrientation.wdOrientLandscape;


                dynamic oRange = oDoc.Content.Application.Selection.Range;
                string oTemp = "";
                for (r = 0; r <= RowCount - 1; r++)
                {
                    for (int c = 0; c <= ColumnCount - 1; c++)
                    {
                        oTemp = oTemp + DataArray[r, c] + "\t";

                    }
                }

                //table format
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

                // Assuming oDoc is your Word document object and DGV is your DataGridView

                // Ensure Word is ready
                System.Threading.Thread.Sleep(100); // Wait for a short period

                // Select the range to work with
                oRange.Select();

                // Work directly with the table without excessive selection
                var table = oDoc.Application.ActiveDocument.Tables[1];
                table.Rows.AllowBreakAcrossPages = 0;
                table.Rows.Alignment = 0;

                // Add a new header row at the top
                Word.Row newHeaderRow = table.Rows.Add(table.Rows[1]); // Duplicate the first row

                // Set properties for the new header row
                newHeaderRow.Range.Bold = 1;
                newHeaderRow.Range.Font.Name = "Tahoma";
                newHeaderRow.Range.Font.Size = 14;

                // Populate the header row with column names
                for (int c = 0; c < DGV.Columns.Count; c++)
                {
                    newHeaderRow.Cells[c + 1].Range.Text = DGV.Columns[c].HeaderText;
                }

                // Set the style for the table
                table.set_Style("Grid Table 5 Dark - Accent 2");
                newHeaderRow.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                // Add header text only on the first page
                Word.Section section = oDoc.Application.ActiveDocument.Sections[1]; // Get the first section
                Word.Range headerRange = section.Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                headerRange.Fields.Add(headerRange, Word.WdFieldType.wdFieldPage);
                headerRange.Text = "Your Header Text";
                headerRange.Font.Size = 16;
                headerRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                // Clean up selections
                oRange.Select(); // If needed, but try to minimize this


                //save the file
                oDoc.SaveAs2(filename);
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
            this.Close();
        }

        private void FoodItemsSave_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void FoodItemsSave_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }
    }
}
