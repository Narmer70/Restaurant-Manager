using System;
using PadTai.Classes;
using System.Drawing;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Globalization;
using System.Data.SqlClient;
using PadTai.Classes.Others;
using PadTai.Sec_daryfolders.Departmentdata;


namespace PadTai.Sec_daryfolders.Staffmanager
{
    public partial class Staffviewer : Form
    {
        private BusinessInfo businessInfo;
        private FontResizer fontResizer;
        private ControlResizer resizer;


        public Staffviewer()
        {
            InitializeComponent();
            initialiseControlsResizing();
            businessInfo = new BusinessInfo(label2, label1);

            LocalizeControls();
            ApplyTheme();
        }

        private void initialiseControlsResizing()
        {
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(panel2);
            resizer.RegisterControl(label1);
            resizer.RegisterControl(label2);
            resizer.RegisterControl(button4);
            resizer.RegisterControl(button5);
            resizer.RegisterControl(rjButton1);
        }


        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void AdduserControl(UserControl UserControl)
        {
            UserControl.Dock = DockStyle.Fill;
            panel2.Controls.Clear();
            panel2.Controls.Add(UserControl);
            UserControl.BringToFront();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Modifystaff addstaff = new Modifystaff();
            AdduserControl(addstaff);
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
            Employeereward employeereward = new Employeereward();
            AdduserControl(employeereward);
        }

        private void Staffviewer_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
            Modifystaff addstaff = new Modifystaff();
            AdduserControl(addstaff);
        }

        private void Staffviewer_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }


        public void LocalizeControls()
        {
            button5.Text = LanguageManager.Instance.GetString("Btn-close");
            //button1.Text = LanguageManager.Instance.GetString("MF-btn1");
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            label1.ForeColor = this.ForeColor;
            label2.ForeColor = this.ForeColor;
        }
    }
}
