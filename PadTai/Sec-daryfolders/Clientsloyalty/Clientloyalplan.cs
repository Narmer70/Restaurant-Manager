using System;
using System.Data;
using System.Linq;
using System.Text;
using PadTai.Classes;
using System.Drawing;
using System.Windows.Forms;
using PadTai.Classes.Others;
using PadTai.Sec_daryfolders.Clientsloyalty;


namespace PadTai.Sec_daryfolders.Departmentdata
{
    public partial class Clientloyalplan : Form
    {
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public Clientloyalplan()
        {
            InitializeComponent();
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(panel1);
            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(rjButton2);

            LocalizeControls();
            ApplyTheme();
        }


        private void AddUserControl(UserControl UserControl) 
        {
            UserControl.Dock = DockStyle.Fill;
            panel1.Controls.Clear();
            panel1.Controls.Add(UserControl);
            UserControl.BringToFront();
        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            Modifyclientsdata modifyclientsdata = new Modifyclientsdata();
            AddUserControl(modifyclientsdata);
        }

        private void Clientloyalplan_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
            Modifyclientsdata modifyclientsdata = new Modifyclientsdata();
            AddUserControl(modifyclientsdata);
        }

        private void Clientloyalplan_Resize(object sender, EventArgs e)
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
