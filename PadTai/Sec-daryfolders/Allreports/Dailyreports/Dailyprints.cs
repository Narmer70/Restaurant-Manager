
using System;
using System.Data;
using System.Linq;
using System.Text;
using PadTai.Classes;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using PadTai.Classes.Others;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using PadTai.Classes.Controlsdesign;
using PadTai.Sec_daryfolders.Others;


namespace PadTai.Sec_daryfolders.Allreports.Dailyreports
{
    public partial class Dailyprints : UserControl
    {
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;
        public Dailyprints()
        {
            InitializeComponent();
            InitializeControlResizer();
            crudDatabase = new CrudDatabase();

            LocalizeControls();
            ApplyTheme();
        }

        private void InitializeControlResizer()
        {
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);
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
            //button6.Text = LanguageManager.Instance.GetString("MF-btn6");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

        }
    }
}