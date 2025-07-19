using System;
using PadTai.Classes;
using System.Drawing;
using System.Windows.Forms;
using PadTai.Classes.Others;


namespace PadTai.Sec_daryfolders.Quitfolder
{
    public partial class Whenclosed : Form
    {
        private FormResizer formResizer;
        private ControlResizer resizer;

        public Whenclosed()
        {
            InitializeComponent();

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(label4);
            resizer.RegisterControl(label5);
            resizer.RegisterControl(label6);
            resizer.RegisterControl(label7);
            resizer.RegisterControl(button1);
            resizer.RegisterControl(button2);

            LocalizeControls();
            ApplyTheme();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _timer.Tick += timer_Tick;
            _timer.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void Whenclosed_Load(object sender, EventArgs e)
        {
            timer1.Start();
            var currentCulture = LanguageManager.Instance.CurrentCulture;

            string currentDateTime = DateTime.Now.ToString("f", currentCulture).ToUpper(); 
            label5.Text = $"{currentDateTime}";

            formResizer = new FormResizer(this);
            formResizer.Resize(this);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            var currentCulture = LanguageManager.Instance.CurrentCulture;
            label6.Text = $"{now.ToString("d", currentCulture)}   {now.ToString("T", currentCulture)}"; 
        }
        System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer { Interval = 1000 };

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Whenclosed_Resize(object sender, EventArgs e)
        {
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
        }
    }
}

