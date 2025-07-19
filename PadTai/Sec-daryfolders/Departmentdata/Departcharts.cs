using System;
using System.Data;
using System.Linq;
using System.Text;
using PadTai.Classes;
using System.Drawing;
using System.Windows.Forms;
using PadTai.Classes.Others;


namespace PadTai.Sec_daryfolders.Departmentdata
{
    public partial class Departcharts : Form
    {
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public Departcharts()
        {
            InitializeComponent();
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(rjButton1);

            LocalizeControls();
            ApplyTheme();
        }

        private void Dptmanager_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void Dptmanager_Resize(object sender, EventArgs e)
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
