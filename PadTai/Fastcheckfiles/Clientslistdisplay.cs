using System;
using System.Data;
using System.Linq;
using System.Text;
using PadTai.Classes;
using System.Drawing;
using System.Data.SQLite;
using System.Windows.Forms;
using PadTai.Classes.Others;
using PadTai.Classes.Databaselink;
using PadTai.Classes.Controlsdesign;
using PadTai.Sec_daryfolders.Others;


namespace PadTai.Fastcheckfiles
{
    public partial class Clientslistdisplay : UserControl
    {
        private Clavieroverlay clavieroverlay;
        private DataGridViewScroller scroller;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;
        private DataTable dataTable;
        Fastcheck FCH;

        public Clientslistdisplay(Fastcheck fCH)
        {
            InitializeComponent();
            InitializeControlResizer();
            crudDatabase = new CrudDatabase();

            ApplyTheme();
            this.FCH = fCH;
            LoadClientsData();
            LocalizeControls();
            dataGridView1.GridColor = this.BackColor;
        }

        private void InitializeControlResizer()
        {
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(dataGridView1);
            resizer.RegisterControl(roundedTextbox1);
            scroller = new DataGridViewScroller(this, null, dataGridView1);
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



        private void LoadClientsData()
        {
            try
            {
                // Define the query to fetch user data
                string query = "SELECT UserID, COALESCE(Username, '') || ' ' || COALESCE(Secondname, '') AS FullName, Phonenumber FROM Userdata";

                // Fetch data using the existing method
                dataTable = crudDatabase.FetchDataFromDatabase(query);

                // Check if data was retrieved
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    dataGridView1.DataSource = dataTable;
                    dgv1ColumnsWidth();
                }
                else
                {
                    MessageBox.Show("No data found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void dgv1ColumnsWidth()
        {
            if (dataGridView1.Columns.Count > 0 && dataGridView1.Rows.Count > 0)
            {
                dataGridView1.RowTemplate.Height = 25;
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[1].HeaderText = "FullName";
                dataGridView1.Columns[2].HeaderText = "Phonenumber";
                dataGridView1.Columns[1].Width = (int)(dataGridView1.Width * 0.65);
                dataGridView1.Columns[2].Width = (int)(dataGridView1.Width * 0.35);
                dataGridView1.DefaultCellStyle.ForeColor = dataGridView1.ForeColor;
                dataGridView1.DefaultCellStyle.BackColor = roundedTextbox1.BackColorRounded;
                dataGridView1.DefaultCellStyle.SelectionForeColor = dataGridView1.ForeColor;
                dataGridView1.Columns[1].DefaultCellStyle.Padding = new Padding(20, 0, 0, 0);
                dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    dataGridView1.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridView1.Columns[i].HeaderCell.Style.Font = new Font("Century Gothic", 10, FontStyle.Bold);
                }
                dataGridView1.ColumnHeadersHeight = 35;
                dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = dataGridView1.ForeColor;
                dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = roundedTextbox1.BackColorRounded;
                dataGridView1.ColumnHeadersDefaultCellStyle.SelectionForeColor = dataGridView1.ForeColor;
                dataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor = roundedTextbox1.BackColorRounded;
                dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            }
        }

        private void Clientslistdisplay_Load(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void roundedTextbox1_TextChanged(object sender, EventArgs e)
        {
            string filterExpression = roundedTextbox1.Text.Trim();

            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);

                if (!string.IsNullOrEmpty(filterExpression))
                {
                    dataView.RowFilter = $"FullName LIKE '%{filterExpression}%' OR Phonenumber LIKE '%{filterExpression}%' ";
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
                string userName = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                FCH.label1.Text = userName;
                string userID = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                FCH.label1.Tag = userID;

            }
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
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;
            
            //label1.ForeColor = colors.Color2;
            //panel1.BackColor = colors.Color3;
            dataGridView1.ForeColor = colors.Color2;
            roundedTextbox1.ForeColor = colors.Color2;
            dataGridView1.BackgroundColor = colors.Color1;
            roundedTextbox1.BackColorRounded = colors.Color3;
            if (colors.Bool1 && Properties.Settings.Default.showScrollbars) showScrollBars();
        }

        private void showScrollBars()
        {
            dataGridView1.ScrollBars = ScrollBars.Vertical;
        }
    }
}
