using System;
using System.Data;
using System.Linq;
using System.Text;
using PadTai.Classes;
using System.Drawing;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Globalization;
using PadTai.Classes.Others;


namespace PadTai.Sec_daryfolders.Updaters.Otherupdates
{
    public partial class Customisablesform : Form
    {
        private BusinessInfo businessInfo;
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public Customisablesform()
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
            resizer.RegisterControl(rjButton2);
            resizer.RegisterControl(rjButton3);
            resizer.RegisterControl(rjButton4);
        }

        public void AdduserControl(UserControl UserControl)
        {
            UserControl.Dock = DockStyle.Fill;
            panel1.Controls.Clear();
            panel1.Controls.Add(UserControl);
            UserControl.BringToFront();
        }
    
        private void Customisablesform_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
            Modifytables modifytables = new Modifytables();
            AdduserControl(modifytables);
        }

        private void Customisablesform_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            Modifydiscounts modifydiscounts = new Modifydiscounts();
            AdduserControl(modifydiscounts);
        }

        private void rjButton3_Click(object sender, EventArgs e)
        {
            Modifypayments modifypayments = new Modifypayments();
            AdduserControl(modifypayments);
        }

        private void rjButton4_Click(object sender, EventArgs e)
        {
            Modifypaygroups modifypaygroups = new Modifypaygroups();
            AdduserControl(modifypaygroups);
        }

        public void LocalizeControls()
        {
            button3.Text = LanguageManager.Instance.GetString("Btn-close");
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
