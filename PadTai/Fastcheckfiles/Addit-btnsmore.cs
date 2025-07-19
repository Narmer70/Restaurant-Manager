using System;
using System.Data;
using System.Linq;
using System.Text;
using PadTai.Classes;
using System.Drawing;
using System.Windows.Forms;
using PadTai.Classes.Others;
using PadTai.Sec_daryfolders.Others;
using PadTai.Sec_daryfolders.Updaters.Staffupdater;


namespace PadTai.Fastcheckfiles
{
    public partial class Addit_btnsmore : UserControl
    {
        private Additionalbtns additionalbtns;
        private ControlResizer resizer;
        Fastcheck FCH;

        public Addit_btnsmore(Fastcheck fCH, Additionalbtns existing)
        {
            InitializeComponent();
            this.FCH = fCH;

            InitializeControlResizer();
            this.additionalbtns = existing; 
            LocalizeControls();
            ApplyTheme();
        }

        private void InitializeControlResizer()
        {
            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(button17);
            resizer.RegisterControl(button18);
            resizer.RegisterControl(button19);
            resizer.RegisterControl(button20);
            resizer.RegisterControl(button21);
            resizer.RegisterControl(button22);
            resizer.RegisterControl(button23);
            resizer.RegisterControl(button24);
            resizer.RegisterControl(tableLayoutPanel1);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (resizer != null)
            {
                resizer.ResizeControls(this);
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            FCH.AdditionalControls(additionalbtns);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            Messaging messaging = new Messaging();
            FCH.AddUserControl(messaging);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            Comingsoon comingsoon = new Comingsoon();
            FCH.AddUserControl(comingsoon);
        }

        private void button23_Click(object sender, EventArgs e)
        {
            Staffdash staffdash = new Staffdash();
            FCH.AddUserControl(staffdash);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            Fastchecksettings fastchecksettings = new Fastchecksettings(FCH);
            FCH.AddUserControl(fastchecksettings);
        }

        private void button22_Click(object sender, EventArgs e)
        {
            Comingsoon comingsoon = new Comingsoon();
            FCH.AddUserControl(comingsoon);
        }

        private void button24_Click(object sender, EventArgs e)
        {
            Comingsoon comingsoon = new Comingsoon();
            FCH.AddUserControl(comingsoon);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            Comingsoon comingsoon = new Comingsoon();
            FCH.AddUserControl(comingsoon);
        }

        public void LocalizeControls()
        {
            button21.Text = LanguageManager.Instance.GetString("MF-btn11");
            button18.Text = LanguageManager.Instance.GetString("Btn-morebtn18");
            button19.Text = LanguageManager.Instance.GetString("Btn-morebtn19");
            button20.Text = LanguageManager.Instance.GetString("Btn-morebtn20");
            button22.Text = LanguageManager.Instance.GetString("Btn-morebtn22");
            button23.Text = LanguageManager.Instance.GetString("Btn-morebtn23");
            button24.Text = LanguageManager.Instance.GetString("Btn-morebtn24");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;
        }
    }
}
