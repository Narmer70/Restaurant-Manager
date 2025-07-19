using System;
using System.Data;
using System.Linq;
using System.Text;
using PadTai.Classes;
using System.Drawing;
using System.Windows.Forms;
using PadTai.Classes.Others;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using PadTai.Classes.Controlsdesign;
using PadTai.Sec_daryfolders.Others;


namespace PadTai.Fastcheckfiles
{
    public partial class Clientdiscount : UserControl
    {
        private Clavieroverlay clavieroverlay;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public Clientdiscount()
        {
            InitializeComponent();

            ApplyTheme();
            LocalizeControls();
            initialiseControlResize();
        }

        private void initialiseControlResize() 
        {
            crudDatabase = new CrudDatabase();
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);
            resizer.RegisterControl(label1);
            resizer.RegisterControl(roundedPanel1);
            resizer.RegisterControl(dataGridView1);
            resizer.RegisterControl(roundedTextbox4);
            dataGridView1.GridColor = dataGridView1.BackgroundColor;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (resizer != null)
            {
                resizer.ResizeControls(this);
            }
        }


        public void LoadUserData(int userId)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("0", "0");
            dataGridView1.Columns.Add("1", "1");
            dataGridView1.Columns[0].Width = (int)(dataGridView1.Width * 0.30);
            dataGridView1.Columns[1].Width = (int)(dataGridView1.Width * 0.70);
            dataGridView1.Columns[0].DefaultCellStyle.Padding = new Padding(20, 0, 0, 0);
            dataGridView1.Columns[1].DefaultCellStyle.Padding = new Padding(20, 0, 0, 0);

            string query = "SELECT * FROM Userdata WHERE UserID = @UserID";
          
            var parameters = new Dictionary<string, object>
            {
                { "@UserID", userId }
            };

            DataTable dataTable = crudDatabase.FetchDataFromDatabase(query, parameters);

            if (dataTable.Rows.Count > 0)
            {
                // Assuming the first row contains the user data
                DataRow row = dataTable.Rows[0];

                // Add rows to the DataGridView with the corresponding field names and values
                dataGridView1.Rows.Add("Username", row["Username"] != DBNull.Value ? row["Username"].ToString() : "-");
                dataGridView1.Rows.Add("Secondname", row["Secondname"] != DBNull.Value ? row["Secondname"].ToString() : "-");
                dataGridView1.Rows.Add("Gender", row["Gender"] != DBNull.Value ? row["Gender"].ToString() : "-");
                dataGridView1.Rows.Add("UserID", row["UserID"] != DBNull.Value ? row["UserID"].ToString() : "-");
                dataGridView1.Rows.Add("Phonenumber", row["Phonenumber"] != DBNull.Value ? row["Phonenumber"].ToString() : "-");
                dataGridView1.Rows.Add("Birthday", row["Birthday"] != DBNull.Value ? row["Birthday"].ToString() : "-");
                dataGridView1.Rows.Add("Email", row["Email"] != DBNull.Value ? row["Email"].ToString() : "-");
            }
            else
            {
              //  MessageBox.Show("No user found with the specified ID.");
            }
        }


        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            dataGridView1.DefaultCellStyle.Font = new Font("Century Gothic", 10, FontStyle.Bold);
           
            if (e.ColumnIndex == 0)
            {
                e.CellStyle.ForeColor = Color.DarkGray;
                e.CellStyle.SelectionForeColor = Color.DarkGray;
            }
        }

        private void roundedTextbox4_Enter(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.showKeyboard)
            {
                clavieroverlay = new Clavieroverlay(roundedTextbox4);
                clavieroverlay.boardLocationBottom();
                clavieroverlay.Show();
            }
        }

        private void roundedTextbox4_Leave(object sender, EventArgs e)
        {
            clavieroverlay?.Hide();
        }

        public void LocalizeControls()
        {
            //button1.Text = LanguageManager.Instance.GetString("MF-btn1");
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            label1.BackColor = colors.Color3;
            dataGridView1.ForeColor = colors.Color2;
            roundedPanel1.BackColor = colors.Color3;
            roundedTextbox4.ForeColor = colors.Color2;
            dataGridView1.BackgroundColor = colors.Color3;
            roundedTextbox4.BackColorRounded = colors.Color1;
            dataGridView1.DefaultCellStyle.ForeColor = colors.Color2;
            dataGridView1.DefaultCellStyle.BackColor = colors.Color3;
            dataGridView1.DefaultCellStyle.SelectionForeColor = colors.Color2;
            dataGridView1.DefaultCellStyle.SelectionBackColor = colors.Color3;
        }
    }
}
