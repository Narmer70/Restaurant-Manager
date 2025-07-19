using System;
using System.Data;
using System.Linq;
using System.Text;
using PadTai.Classes;
using System.Drawing;
using PadTai.Classes.Others;
using System.Windows.Forms;
using PadTai.Classes.Controlsdesign;


namespace PadTai.Sec_daryfolders.Updaters.FoodUpdater
{
    public partial class Foodsettings : UserControl
    {
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public Foodsettings()
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
            //resizer.RegisterControl(label3);
        }

        public void LocalizeControls()
        {
            //button1.Text = LanguageManager.Instance.GetString("MF-btn1");
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
            //button4.Text = LanguageManager.Instance.GetString("MF-btn4");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;
        }
    }
}
