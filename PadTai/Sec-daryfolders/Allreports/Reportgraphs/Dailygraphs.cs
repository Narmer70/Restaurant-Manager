using System;
using System.Linq;
using PadTai.Classes;
using System.Drawing;
using System.Collections;
using System.Data.SQLite;
using System.Globalization;
using System.Windows.Forms;
using System.Data.SqlClient;
using PadTai.Classes.Others;
using System.Collections.Generic;
using System.Windows.Forms.DataVisualization.Charting;


namespace PadTai.Sec_daryfolders.Allreports.Reportgraphs
{
    public partial class Dailygraphs : Form
    {
        private BusinessInfo businessInfo;
        private FontResizer fontResizer;
        private ControlResizer resizer;


        public Dailygraphs()
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
            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(rjButton2);
            resizer.RegisterControl(rjButton3);
            resizer.RegisterControl(rjButton4);
            resizer.RegisterControl(rjButton5);
            resizer.RegisterControl(rjButton6);
        }

        public void AddUserControl(UserControl UserControl)
        {
            UserControl.Dock = DockStyle.Fill;
            panel1.Controls.Clear();
            panel1.Controls.Add(UserControl);
            UserControl.BringToFront();
        }

        private void Dailygraphs_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }

            Receipthourchart RPOG = new Receipthourchart();
            AddUserControl(RPOG);
        }

        private void Dailygraphs_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
            this.Close();   
        }

        private void rjButton3_Click(object sender, EventArgs e)
        {
            Foodgroupchart FGC = new Foodgroupchart();
            AddUserControl(FGC);
        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            Paychart paygraph = new Paychart();
            AddUserControl(paygraph);
        }

        private void rjButton4_Click(object sender, EventArgs e)
        {
            Dailydiscountchart daylydiscountchart = new Dailydiscountchart();   
            AddUserControl(daylydiscountchart); 
        }

        private void rjButton5_Click(object sender, EventArgs e)
        {
            Dailydeliverychart daylydeliverychart = new Dailydeliverychart();
            AddUserControl(daylydeliverychart);
        }

        private void rjButton6_Click(object sender, EventArgs e)
        {
            Receipthourchart RPOG = new Receipthourchart();
            AddUserControl(RPOG);
        }

        public void LocalizeControls()
        {
            rjButton1.Text = LanguageManager.Instance.GetString("Btn-close");
            rjButton5.Text = LanguageManager.Instance.GetString("FCK-btn36");
            rjButton2.Text = LanguageManager.Instance.GetString("Dailypay-btn");
            rjButton3.Text = LanguageManager.Instance.GetString("Addbtns-btn18");
            rjButton4.Text = LanguageManager.Instance.GetString("Addbtns-btn17");
            rjButton6.Text = LanguageManager.Instance.GetString("Staffdash-lbl1");
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
