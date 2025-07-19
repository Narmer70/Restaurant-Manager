using System;
using System.Drawing;
using System.Windows.Forms;
using PadTai.Classes.Others;
using System.Threading.Tasks;


namespace PadTai.Sec_daryfolders.Others_Forms
{
    public partial class Waitform : Form
    {
        public Action Waiting {  get; set; }

        public Waitform(Action waiting)
        {
            InitializeComponent();

            if (Waiting == null) 
            {
                throw new ArgumentNullException();
            }
            Waiting = waiting;

            LocalizeControls();
            ApplyTheme();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e); 
            Task.Factory.StartNew(Waiting).ContinueWith(s => {this.Close(); }, TaskScheduler.FromCurrentSynchronizationContext());
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
