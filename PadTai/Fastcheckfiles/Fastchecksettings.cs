using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using PadTai.Classes;
using System.Windows.Forms;
using PadTai.Classes.Others;
using PadTai.Classes.Controlsdesign;


namespace PadTai.Fastcheckfiles
{
    public partial class Fastchecksettings : UserControl
    {
        private DataGridViewScroller scroller;
        private FontResizer fontResizer;
        private ControlResizer resizer;
        Fastcheck FCH;
        public Fastchecksettings(Fastcheck fastcheck)
        {
            InitializeComponent();
            InitializeControlResizer();
            this.FCH = fastcheck;
            scroller = new DataGridViewScroller(this);

            LocalizeControls();
            CenterLabel();
            ApplyTheme();
        }

        private void InitializeControlResizer()
        {
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);
            resizer.RegisterControl(label1);
            resizer.RegisterControl(label2);
            resizer.RegisterControl(label3);
            resizer.RegisterControl(label4);
            resizer.RegisterControl(label5);
            resizer.RegisterControl(toggleSwitch1);
            resizer.RegisterControl(toggleSwitch2);
            resizer.RegisterControl(toggleSwitch3);
            resizer.RegisterControl(toggleSwitch4);
            resizer.RegisterControl(toggleSwitch5);
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
            label1.Left = (this.ClientSize.Width - label1.Width) / 2;
            label2.Left = (this.ClientSize.Width - label2.Width) / 2;
            label3.Left = (this.ClientSize.Width - label3.Width) / 2;
            label4.Left = (this.ClientSize.Width - label4.Width) / 2;
            label5.Left = (this.ClientSize.Width - label5.Width) / 2;
            toggleSwitch1.Left = (this.ClientSize.Width - toggleSwitch1.Width) / 2;
            toggleSwitch2.Left = (this.ClientSize.Width - toggleSwitch2.Width) / 2;
            toggleSwitch3.Left = (this.ClientSize.Width - toggleSwitch3.Width) / 2;
            toggleSwitch4.Left = (this.ClientSize.Width - toggleSwitch4.Width) / 2;
            toggleSwitch5.Left = (this.ClientSize.Width - toggleSwitch5.Width) / 2;
        }

        private void Fastchecksettings_Load(object sender, EventArgs e)
        {
            toggleSwitch1.IsOn = Properties.Settings.Default.contDiscountAnabled;
            toggleSwitch2.IsOn = Properties.Settings.Default.dishpicEnabled;
            toggleSwitch3.IsOn = Properties.Settings.Default.playSoundNotification;
            toggleSwitch4.IsOn = Properties.Settings.Default.saveReceiptdraft;
            toggleSwitch5.IsOn = Properties.Settings.Default.showKeyboard;
        }

        private void toggleSwitch1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.contDiscountAnabled = toggleSwitch1.IsOn;
            Properties.Settings.Default.Save();
        }

        private void toggleSwitch2_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.dishpicEnabled = toggleSwitch2.IsOn;
            Properties.Settings.Default.Save();
        }

        private void toggleSwitch3_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.playSoundNotification = toggleSwitch3.IsOn;
            Properties.Settings.Default.Save();
        }

        private void toggleSwitch4_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.saveReceiptdraft = toggleSwitch4.IsOn;
            Properties.Settings.Default.Save();
        }

        private void toggleSwitch5_Click(object sender, EventArgs e)
        {
            scroller.UpdateFocusBehavior(toggleSwitch5.IsOn);
        }

        public void LocalizeControls()
        {
            label1.Text = LanguageManager.Instance.GetString("FSettings1");
            label2.Text = LanguageManager.Instance.GetString("FSettings2");
            label3.Text = LanguageManager.Instance.GetString("FSettings3");
            label4.Text = LanguageManager.Instance.GetString("FSettings4");
            label5.Text = LanguageManager.Instance.GetString("FSettings5");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;
            toggleSwitch1.OffColor = colors.Color3;
            toggleSwitch2.OffColor = colors.Color3;
            toggleSwitch3.OffColor = colors.Color3;
            toggleSwitch4.OffColor = colors.Color3;
            toggleSwitch5.OffColor = colors.Color3;
       
            label1.ForeColor = this.ForeColor;
            label2.ForeColor = this.ForeColor;
            label3.ForeColor = this.ForeColor;
            label4.ForeColor = this.ForeColor;
            label5.ForeColor = this.ForeColor;
            toggleSwitch1.LabelColor = this.ForeColor;
            toggleSwitch2.LabelColor = this.ForeColor;
            toggleSwitch3.LabelColor = this.ForeColor;
            toggleSwitch4.LabelColor = this.ForeColor;
            toggleSwitch5.LabelColor = this.ForeColor;
        }
    }
}
