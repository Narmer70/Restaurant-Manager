using System;
using System.Data;
using System.Linq;
using System.Text;
using PadTai.Classes;
using System.Drawing;
using System.Data.SQLite;
using System.Windows.Forms;
using PadTai.Classes.Others;
using System.Collections.Generic;
using PadTai.Sec_daryfolders.Updates;


namespace PadTai.Fastcheckfiles
{
    public partial class Billsplitter : UserControl
    {
        private FontResizer fontResizer;
        private ControlResizer resizer;
        Fastcheck FCH;

        public Billsplitter(Fastcheck fCH)
        {
            InitializeComponent();
            this.FCH = fCH;

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);
            resizer = new ControlResizer(this.Size);
            resizer.RegisterControl(tableLayoutPanel1);

            LocalizeControls();
            ApplyTheme();
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
            //button4.Text = LanguageManager.Instance.GetString("MF-btn4");
            //button5.Text = LanguageManager.Instance.GetString("MF-btn5");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            rjButton1.BackColor = colors.Color3;
            customBox1.BackColor = colors.Color3;
            customBox1.ForeColor = colors.Color2;
            customBox1.ArrowColor = colors.Color2;
            customBox1.BorderColor = colors.Color4;
            customBox1.BorderFocusColor = colors.Color5;
        }
    }
}
