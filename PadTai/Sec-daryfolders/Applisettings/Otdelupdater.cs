using System;
using System.Data;
using PadTai.Classes;
using System.Drawing;
using System.Windows.Forms;
using PadTai.Classes.Others;
using PadTai.Sec_daryfolders.Applisettings;
using PadTai.Sec_daryfolders.Deptmmodifier;


namespace PadTai.Sec_daryfolders.Staffmanager
{
    public partial class Otdelupdater : Form
    {
        private BusinessInfo businessInfo;
        private FontResizer fontResizer;
        private ControlResizer resizer;
    
        public Otdelupdater()
        {
            InitializeComponent();

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(panel1);
            resizer.RegisterControl(label1);
            resizer.RegisterControl(label2);
            resizer.RegisterControl(button1);
            resizer.RegisterControl(button2);
            resizer.RegisterControl(button5);
            resizer.RegisterControl(button6);
            resizer.RegisterControl(rjButton1);

            businessInfo = new BusinessInfo(label2, label1);
            LanguageManager.Instance.LanguageChanged += HandleLanguageChange;
            ThemeManager.ThemeChanged += ApplyTheme;
            LocalizeControls();
            ApplyTheme();
        }


        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void AddUserControl(UserControl UserControl)
        {
            UserControl.Dock = DockStyle.Fill;
            panel1.Controls.Clear();
            panel1.Controls.Add(UserControl);
            UserControl.BringToFront();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Printersettings QCG = new Printersettings();
            AddUserControl(QCG);
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
            Monthlypayment MP = new Monthlypayment();
            AddUserControl(MP);
        }

        private void Otdelupdater_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
            Generalsettings generalsettings = new Generalsettings();
            AddUserControl(generalsettings);
        }

        private void Otdelupdater_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
           Otdelrenamer otdelrenamer = new Otdelrenamer();  
            AddUserControl(otdelrenamer);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Generalsettings generalsettings = new Generalsettings();
            AddUserControl(generalsettings);
        }

        private void HandleLanguageChange()
        {
            LocalizeControls();
        }

        public void LocalizeControls()
        {
            button1.Text = LanguageManager.Instance.GetString("SET-btn1");
            button2.Text = LanguageManager.Instance.GetString("SET-btn2");
            button6.Text = LanguageManager.Instance.GetString("SET-btn6");
            button5.Text = LanguageManager.Instance.GetString("Btn-close");
            rjButton1.Text = LanguageManager.Instance.GetString("SET-rbtn1");
            //button6.Text = LanguageManager.Instance.GetString("MF-btn6");
            //button7.Text = LanguageManager.Instance.GetString("MF-btn7");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            label1.ForeColor = colors.Color2;
            label2.ForeColor = colors.Color2;
        }
    }
}
