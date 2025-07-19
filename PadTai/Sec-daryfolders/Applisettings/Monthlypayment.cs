using System;
using System.Drawing;
using PadTai.Classes;
using System.Windows.Forms;
using PadTai.Classes.Others;
using PadTai.Classes.Controlsdesign;


namespace PadTai.Sec_daryfolders.Deptmmodifier
{
    public partial class Monthlypayment : UserControl
    {
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public Monthlypayment()
        {
            InitializeComponent();
            InitializeControlResizer();

            LocalizeControls();
            ApplyTheme();
        }

        private void InitializeControlResizer()
        {
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            //resizer.RegisterControl(label1);
            //resizer.RegisterControl(label2);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (resizer != null)
            {
                resizer.ResizeControls(this);
            }
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
        }
    }
}
