using System;
using PadTai.Classes;
using System.Drawing;
using System.Windows.Forms;
using PadTai.Classes.Others;
using PadTai.Sec_daryfolders.Others;


namespace PadTai.Fastcheckfiles
{
    public partial class Additionalbtns : UserControl
    {
        Fastcheck FCH;
        private ControlResizer resizer;

        public Additionalbtns(Fastcheck fCH)
        {
            InitializeComponent();

            button17.Enabled = false;
            button19.Enabled = false;  

            InitializeControlResizer();
            LocalizeControls();
            ApplyTheme();
            this.FCH = fCH;
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

        private void button24_Click(object sender, EventArgs e)
        {
            Comingsoon comingsoon = new Comingsoon();
            FCH.AddUserControl(comingsoon);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            Calculatrice calculatrice = new Calculatrice(FCH);
            FCH.AddUserControl(calculatrice);
        }


        private void button18_Click(object sender, EventArgs e)
        {
            DishGroupControl MCL = new DishGroupControl(FCH, null);
            FCH.AddUserControl(MCL);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            DiscountControls DCS = new DiscountControls(FCH);
            FCH.AddUserControl(DCS);

        }

        private void button19_Click(object sender, EventArgs e)
        {
            TablePlacesControl TPC = new TablePlacesControl(FCH);
            FCH.AddUserControl(TPC);
        }

        private void button23_Click(object sender, EventArgs e)
        {
            IndividualDiscount IND = new IndividualDiscount(FCH);
            FCH.AddUserControl(IND);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            Addit_btnsmore ABM = new Addit_btnsmore(FCH, this);
            FCH.AdditionalControls(ABM);
        }

        private void button22_Click(object sender, EventArgs e)
        {
            Clientslistdisplay CLD = new Clientslistdisplay(FCH);
            FCH.AddUserControl(CLD);
        }

        public void LocalizeControls()
        {
            button17.Text = LanguageManager.Instance.GetString("Addbtns-btn17");
            button18.Text = LanguageManager.Instance.GetString("Addbtns-btn18");
            button19.Text = LanguageManager.Instance.GetString("Addbtns-btn19");
            button21.Text = LanguageManager.Instance.GetString("Addbtns-btn21");
            button22.Text = LanguageManager.Instance.GetString("Addbtns-btn22");
            button23.Text = LanguageManager.Instance.GetString("Addbtns-btn23");
            button24.Text = LanguageManager.Instance.GetString("Addbtns-btn24");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;
        }
    }
}
