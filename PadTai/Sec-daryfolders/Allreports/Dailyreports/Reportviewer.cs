using System;
using PadTai.Classes;
using System.Drawing;
using System.Data.SQLite;
using System.Globalization;
using System.Windows.Forms;
using PadTai.Classes.Others;
using PadTai.Sec_daryfolders.Others;
using PadTai.Sec_daryfolders.Quitfolder;
using PadTai.Sec_daryfolders.Allreports.Dailyreports;


namespace PadTai.Sec_daryfolders
{

    public partial class Reportviewer : Form
    {
        private BusinessInfo businessInfo;
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public Reportviewer()
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

            resizer.RegisterControl(panel1);
            resizer.RegisterControl(label1);
            resizer.RegisterControl(label2);
            resizer.RegisterControl(button3);
            resizer.RegisterControl(button2);
            resizer.RegisterControl(button4);
            resizer.RegisterControl(button5);
            resizer.RegisterControl(button8);
            resizer.RegisterControl(rjButton1);
        }

        private void Reportviewer_Load(object sender, EventArgs e)
        {         
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }

            Paytypecatreport PTR = new Paytypecatreport();
            AddUserControl(PTR);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Paytypecatreport PTR = new Paytypecatreport();
            AddUserControl(PTR);
        }

        public void AddUserControl(UserControl UserControl)
        {
            UserControl.Dock = DockStyle.Fill;
            panel1.Controls.Clear();
            panel1.Controls.Add(UserControl);
            UserControl.BringToFront();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Foodcatereport FCR = new Foodcatereport();
            AddUserControl(FCR);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Discountcatreport DCR = new Discountcatreport();
            AddUserControl(DCR);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Deltypecateport TOD = new Deltypecateport();
            AddUserControl(TOD);
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
            Dailyprints dailyprints = new Dailyprints();
            AddUserControl(dailyprints);
        }

        private void Reportviewer_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        public void LocalizeControls()
        {
            button8.Text = LanguageManager.Instance.GetString("Btn-close");
            button5.Text = LanguageManager.Instance.GetString("FCK-btn36");
            button2.Text = LanguageManager.Instance.GetString("Dailypay-btn");
            button3.Text = LanguageManager.Instance.GetString("Addbtns-btn18");
            button4.Text = LanguageManager.Instance.GetString("Addbtns-btn17");
            rjButton1.Text = LanguageManager.Instance.GetString("Dailyprints-btn");
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
