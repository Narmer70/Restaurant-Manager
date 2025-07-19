using System;
using System.Data;
using System.Linq;
using System.Text;
using PadTai.Classes;
using System.Drawing;
using System.Windows.Forms;
using PadTai.Classes.Others;
using PadTai.Sec_daryfolders.Others;
using PadTai.Sec_daryfolders.Grossmanager;
using PadTai.Sec_daryfolders.Updaters.FoodUpdater.Create;


namespace PadTai.Sec_daryfolders.Departmentdata
{
    public partial class Managegross : Form
    {
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public Managegross()
        {
            InitializeComponent();
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(panel1);
            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(rjButton7);
            resizer.RegisterControl(rjButton8);

            LocalizeControls();
            ApplyTheme();
        }


        public void AdduserControl(UserControl UserControl)
        {
            UserControl.Dock = DockStyle.Fill;
            panel1.Controls.Clear();
            panel1.Controls.Add(UserControl);
            UserControl.BringToFront();
        }

        private void Recepeesaver_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
            Allfoodreport allfoodreport = new Allfoodreport();
            AdduserControl(allfoodreport);
        }

        private void Recepeesaver_Resize(object sender, EventArgs e)
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

        private void rjButton7_Click(object sender, EventArgs e)
        {
            Allfoodreport allfoodreport = new Allfoodreport();
            AdduserControl(allfoodreport);
        }

        private void rjButton8_Click(object sender, EventArgs e)
        {
            Allreceiptreport allreceiptreport = new Allreceiptreport();
            AdduserControl(allreceiptreport);
        }

        public void LocalizeControls()
        {
            rjButton1.Text = LanguageManager.Instance.GetString("Btn-close");
            //button1.Text = LanguageManager.Instance.GetString("MF-btn1");
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
            //button4.Text = LanguageManager.Instance.GetString("MF-btn4");
            //button5.Text = LanguageManager.Instance.GetString("MF-btn5");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;
        } 
    }
}
