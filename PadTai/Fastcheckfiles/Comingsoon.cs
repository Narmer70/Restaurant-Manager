using System;
using System.Linq;
using System.Text;
using PadTai.Classes;
using System.Windows.Forms;
using PadTai.Classes.Others;
using PadTai.Classes.Controlsdesign;


namespace PadTai.Fastcheckfiles
{
    public partial class Comingsoon : UserControl
    {
        private FontResizer fontResizer;
        private ControlResizer resizer;
        public Comingsoon()
        {
            InitializeComponent();

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);
            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(panel2);
            resizer.RegisterControl(label2);

            LocalizeControls();
            CenterLabel();
            ApplyTheme();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (resizer != null)
            {
                resizer.ResizeControls(this);
                CenterLabel();
            }
        }

        private void CenterLabel()
        {
            panel2.Left = (this.ClientSize.Width - panel2.Width) / 2;
            label2.Left = (panel2.ClientSize.Width - label2.Width) / 2;
            pictureBox2.Left = (panel2.ClientSize.Width - pictureBox2.Width) / 2;
        }



        public void LocalizeControls()
        {
            label2.Text = LanguageManager.Instance.GetString("Comingsoon");
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

            label2.ForeColor = colors.Color2;
            //rjButton1.BackColor = colors.Color3;
            //customBox1.BackColor = colors.Color3;
            //customBox1.ForeColor = colors.Color2;
            //customBox1.ArrowColor = colors.Color2;
            //customBox1.BorderColor = colors.Color4;
            //customBox1.BorderFocusColor = colors.Color5;
        }
    }
}
