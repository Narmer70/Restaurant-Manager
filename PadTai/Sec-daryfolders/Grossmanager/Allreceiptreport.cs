using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using PadTai.Classes;
using System.Data.SQLite;
using System.Windows.Forms;
using PadTai.Classes.Others;
using PadTai.Classes.Controlsdesign;
using PadTai.Sec_daryfolders.Others;
using PadTai.Classes.Databaselink;
using System.Collections.Generic;


namespace PadTai.Sec_daryfolders.Grossmanager
{
    public partial class Allreceiptreport : UserControl
    {
        ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();
        private DataGridViewScroller scroller;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;
        public Allreceiptreport()
        {
            InitializeComponent();
            InitializeControlResizer();
            dataGridView1.GridColor = colors.Color1;
            dataGridView2.GridColor = colors.Color1;
            LocalizeControls();
            ApplyTheme();
        }

        private void InitializeControlResizer()
        {
            crudDatabase = new CrudDatabase();
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
            resizer.RegisterControl(label9);
            resizer.RegisterControl(label10);
            resizer.RegisterControl(label11);
            resizer.RegisterControl(label12);
            resizer.RegisterControl(label13);
            resizer.RegisterControl(label14);
            resizer.RegisterControl(label15);
            resizer.RegisterControl(label16);
            resizer.RegisterControl(label17);
            resizer.RegisterControl(label18);
            resizer.RegisterControl(label19);
            resizer.RegisterControl(label20);
            resizer.RegisterControl(label21);
            resizer.RegisterControl(label22);
            resizer.RegisterControl(pictureBox1);
            resizer.RegisterControl(pictureBox2);
            resizer.RegisterControl(dataGridView1);
            resizer.RegisterControl(dataGridView2);
            resizer.RegisterControl(roundedPanel1);
            resizer.RegisterControl(roundedPanel2);
            resizer.RegisterControl(roundedPanel3);
            resizer.RegisterControl(roundedPanel4);
            resizer.RegisterControl(roundedPanel5);
            resizer.RegisterControl(roundedPanel6);
            resizer.RegisterControl(roundedPanel7);
            scroller = new DataGridViewScroller(this, null, dataGridView1, dataGridView2);

            LoadData();
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


        public void LoadData()
        {
            int clientId;

            if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                return;
            }

            string sql = "SELECT ReceiptID, FoodName FROM Receipts WHERE ClientID = @ClientID ORDER BY ReceiptID DESC";
            var parameters = new Dictionary<string, object>
            {
                { "@ClientID", clientId }
            };

            try
            {
                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                DataTable dataTable = crudDatabase.FetchDataFromDatabase(sql, parameters);
                dataGridView2.DataSource = dataTable;

                if (dataGridView2.Columns.Count > 0)
                {
                    setColumnWidth();
                }
            }
            catch 
            {

            }
        }

        private void setColumnWidth()
        {
            dataGridView2.ColumnHeadersHeight = 30;
            dataGridView2.Columns[0].HeaderText = "ID";
            dataGridView2.Columns[1].HeaderText = "Receipt dishes";
            dataGridView2.Columns[0].Width = (int)(dataGridView2.Width * 0.08);
            dataGridView2.Columns[1].Width = (int)(dataGridView2.Width * 0.92);
            dataGridView2.Columns[1].HeaderCell.Style.Padding = new Padding(20, 0, 0, 0);
            dataGridView2.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = dataGridView2.ForeColor;
            dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = dataGridView2.BackgroundColor;
            dataGridView2.ColumnHeadersDefaultCellStyle.SelectionForeColor = dataGridView2.ForeColor;
            dataGridView2.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView2.ColumnHeadersDefaultCellStyle.SelectionBackColor = dataGridView2.BackgroundColor;
            dataGridView2.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.ColumnHeadersDefaultCellStyle.Font = new Font("Century Gothic", 10, FontStyle.Bold);
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
        }

        public void LocalizeControls()
        {
            //button1.Text = LanguageManager.Instance.GetString("MF-btn1");
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
            //button4.Text = LanguageManager.Instance.GetString("MF-btn4");
            //button5.Text = LanguageManager.Instance.GetString("MF-btn5");
            //button6.Text = LanguageManager.Instance.GetString("MF-btn6");
        }

        public void ApplyTheme()
        {
            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            dataGridView1.ForeColor = colors.Color2;
            dataGridView2.ForeColor = colors.Color2;
            roundedPanel1.BackColor = colors.Color3;
            roundedPanel2.BackColor = colors.Color3;
            roundedPanel3.BackColor = colors.Color3;
            roundedPanel4.BackColor = colors.Color3;
            roundedPanel5.BackColor = colors.Color3;
            roundedPanel6.BackColor = colors.Color3;
            roundedPanel7.BackColor = colors.Color3;
            dataGridView1.BackgroundColor = colors.Color3;
            dataGridView2.BackgroundColor = colors.Color3;
            dataGridView1.DefaultCellStyle.ForeColor = colors.Color2;
            dataGridView2.DefaultCellStyle.ForeColor = colors.Color2;
            dataGridView1.DefaultCellStyle.BackColor = colors.Color3;
            dataGridView2.DefaultCellStyle.BackColor = colors.Color3;
            dataGridView1.DefaultCellStyle.SelectionForeColor = colors.Color2;
            dataGridView2.DefaultCellStyle.SelectionForeColor = colors.Color2;
            dataGridView2.DefaultCellStyle.SelectionBackColor = colors.Color3;
            if (colors.Bool1 && Properties.Settings.Default.showScrollbars) showScrollBars();
        }

        private void showScrollBars()
        {
            dataGridView1.ScrollBars = ScrollBars.Vertical;
            dataGridView2.ScrollBars = ScrollBars.Vertical;
        }
    }
}
