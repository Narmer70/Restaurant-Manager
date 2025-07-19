using System;
using System.Data;
using PadTai.Classes;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using PadTai.Classes.Others;
using PadTai.Classes.Databaselink;


namespace PadTai.Sec_daryfolders.Allreports.Otdelreports
{
    public partial class Allotdelreports : Form
    {
        private BusinessInfo businessInfo;
        private FontResizer fontResizer;
        private ControlResizer resizer;
        
        
        public Allotdelreports()
        {
            InitializeComponent();
            initialiseControlResizing();
            businessInfo = new BusinessInfo(label2, label1);

            LocalizeControls();
            ApplyTheme();
        }

        private void initialiseControlResizing()
        {
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);
            resizer.RegisterControl(panel1);
            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(rjButton2);
        }


        public void AddUserControl(UserControl UserControl)
        {
            UserControl.Dock = DockStyle.Fill;
            panel1.Controls.Clear();
            panel1.Controls.Add(UserControl);
            UserControl.BringToFront();
        }


        private void rjButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            OtdelReturntable FCR = new OtdelReturntable();
            AddUserControl(FCR);
        }

        private void Fullotdelreports_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void Fullotdelreports_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        public void LocalizeControls()
        {
            rjButton1.Text = LanguageManager.Instance.GetString("Btn-close");
            //button1.Text = LanguageManager.Instance.GetString("MF-btn1");
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;
        }
    }
}
