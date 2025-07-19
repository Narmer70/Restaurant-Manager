using System;
using System.Linq;
using PadTai.Classes;
using System.Drawing;
using System.Windows.Forms;
using PadTai.Classes.Others;
using System.Drawing.Printing;
using System.Collections.Generic;
using PadTai.Classes.Controlsdesign;


namespace PadTai.Sec_daryfolders.Applisettings
{
    public partial class Printersettings : UserControl
    {
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public Printersettings()
        {
            InitializeComponent();

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);
            InitializeControlResizer();

            LocalizeControls();
            CenterLabel();
            ApplyTheme();
        }


        private void InitializeControlResizer()
        {
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(panel1);
            resizer.RegisterControl(label1);
            resizer.RegisterControl(label2);
            resizer.RegisterControl(label5);
            resizer.RegisterControl(label8);
            resizer.RegisterControl(label9);
            resizer.RegisterControl(label10);
            resizer.RegisterControl(label11);
            resizer.RegisterControl(textBox1);
            resizer.RegisterControl(toggleSwitch3);
            resizer.RegisterControl(toggleSwitch4);
            resizer.RegisterControl(toggleSwitch5);
            resizer.RegisterControl(toggleSwitch6);
            resizer.RegisterControl(toggleSwitch7);

            toggleSwitch6.IsOn = Properties.Settings.Default.printQRcode;
            toggleSwitch3.IsOn = Properties.Settings.Default.Automaticprint;
            toggleSwitch4.IsOn = Properties.Settings.Default.printToNewLine;
            toggleSwitch5.IsOn = Properties.Settings.Default.printBrandLogo;
            toggleSwitch7.IsOn = Properties.Settings.Default.isPlustonewLine;
            textBox1.Text = Properties.Settings.Default.NumberOfCopies.ToString();
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
            label1.Left = (panel1.ClientSize.Width - label1.Width) / 2;
        }

        private void toggleSwitch3_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Automaticprint = toggleSwitch3.IsOn;
            Properties.Settings.Default.Save();
        }

        private void toggleSwitch4_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.printToNewLine = toggleSwitch4.IsOn;
            Properties.Settings.Default.Save();
        }

        private void toggleSwitch5_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.printBrandLogo = toggleSwitch5.IsOn;
            Properties.Settings.Default.Save();
        }

        private void toggleSwitch6_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.printQRcode = toggleSwitch6.IsOn;
            Properties.Settings.Default.Save();
        }

        private void toggleSwitch7_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.isPlustonewLine = toggleSwitch7.IsOn;
            Properties.Settings.Default.Save();
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out int numberOfCopies))
            {
                Properties.Settings.Default.NumberOfCopies = numberOfCopies;
                Properties.Settings.Default.Save(); 
            }          
        }

        public void LocalizeControls()
        {
            label1.Text = LanguageManager.Instance.GetString("SETPRINT-lbl1");
            label2.Text = LanguageManager.Instance.GetString("SETPRINT-lbl2");
            label5.Text = LanguageManager.Instance.GetString("SETPRINT-lbl5");
            label8.Text = LanguageManager.Instance.GetString("SETPRINT-lbl8");
            label9.Text = LanguageManager.Instance.GetString("SETPRINT-lbl9");
            label10.Text = LanguageManager.Instance.GetString("SETPRINT-lbl10");
            label11.Text = LanguageManager.Instance.GetString("SETPRINT-lbl11");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            panel1.BackColor = colors.Color3;
            label1.ForeColor = this.ForeColor;
            label2.ForeColor = this.ForeColor;
            label5.ForeColor = this.ForeColor;
            label8.ForeColor = this.ForeColor;
            label9.ForeColor = this.ForeColor;
            label10.ForeColor = this.ForeColor;
            label11.ForeColor = this.ForeColor;
            textBox1.ForeColor = this.ForeColor;
            toggleSwitch3.OffColor = this.BackColor;
            toggleSwitch4.OffColor = this.BackColor;
            toggleSwitch5.OffColor = this.BackColor;
            toggleSwitch6.OffColor = this.BackColor;
            toggleSwitch7.OffColor = this.BackColor;
            textBox1.BackColorRounded = colors.Color1;
            toggleSwitch3.LabelColor = this.ForeColor;
            toggleSwitch4.LabelColor = this.ForeColor;
            toggleSwitch5.LabelColor = this.ForeColor;
            toggleSwitch6.LabelColor = this.ForeColor;
            toggleSwitch7.LabelColor = this.ForeColor;
            toggleSwitch3.SwitchColor = this.ForeColor;
            toggleSwitch4.SwitchColor = this.ForeColor;
            toggleSwitch5.SwitchColor = this.ForeColor;
            toggleSwitch6.SwitchColor = this.ForeColor;
            toggleSwitch7.SwitchColor = this.ForeColor;
        }
    }
}





