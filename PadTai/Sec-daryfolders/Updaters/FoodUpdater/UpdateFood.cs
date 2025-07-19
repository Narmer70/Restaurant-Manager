using System;
using PadTai.Classes;
using System.Drawing;
using System.Data.SQLite;
using System.Globalization;
using System.Windows.Forms;
using System.Data.SqlClient;
using PadTai.Classes.Others;
using PadTai.Sec_daryfolders.Quitfolder;
using PadTai.Sec_daryfolders.Staffmanager;
using PadTai.Sec_daryfolders.Deptmmodifier;
using PadTai.Sec_daryfolders.Updaters.FoodUpdater;
using PadTai.Sec_daryfolders.Updaters.Otherupdates;
using PadTai.Sec_daryfolders.Updaters.FoodUpdater.Create;
using PadTai.Sec_daryfolders.Updaters.FoodUpdater.Delete;
using PadTai.Sec_daryfolders.Updaters.FoodUpdater.Updates;


namespace PadTai.Sec_daryfolders.Update
{
    public partial class UpdateFood : Form
    {
        private BusinessInfo businessInfo;
        private FontResizer fontResizer;
        private ControlResizer resizer;
       
        public UpdateFood()
        {
            InitializeComponent();
            initialiseControlsResizing();         

            businessInfo = new BusinessInfo(label2, label1); 
            LocalizeControls();
            ApplyTheme();
        }

        private void initialiseControlsResizing()
        {

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(panel1);
            resizer.RegisterControl(label1);
            resizer.RegisterControl(label2);
            resizer.RegisterControl(button3);
            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(rjButton2);
            resizer.RegisterControl(rjButton3);
            resizer.RegisterControl(rjButton4);
            resizer.RegisterControl(rjButton5);
            resizer.RegisterControl(rjButton6);    
        }

        public void AdduserControl(UserControl UserControl)
        {
            UserControl.Dock = DockStyle.Fill;
            panel1.Controls.Clear();
            panel1.Controls.Add(UserControl);
            UserControl.BringToFront();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UpdateFood_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
            Modifydishes modifydishes = new Modifydishes();
            AdduserControl(modifydishes);
        }

        private void UpdateFood_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
            Modifygroups modifygroups = new Modifygroups();
            AdduserControl(modifygroups);
        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            Modifydishes modifydishes = new Modifydishes();
            AdduserControl(modifydishes);
        }

        private void rjButton3_Click(object sender, EventArgs e)
        {
            Modifysubgroup modifysubgroup = new Modifysubgroup();
            AdduserControl(modifysubgroup);
        }

        private void rjButton4_Click(object sender, EventArgs e)
        {
            ModifySubsubgroup modifySubsubgroup = new ModifySubsubgroup();
            AdduserControl(modifySubsubgroup);
        }

        private void rjButton5_Click(object sender, EventArgs e)
        {
            Modifytypes modifytypes = new Modifytypes();
            AdduserControl(modifytypes);
        }

        private void rjButton6_Click(object sender, EventArgs e)
        {
            Foodsettings foodsettings = new Foodsettings();
            AdduserControl(foodsettings);
        }

        public void LocalizeControls()
        {
            button3.Text = LanguageManager.Instance.GetString("Btn-close");
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

            label1.ForeColor =  this.ForeColor;
            label2.ForeColor = this.ForeColor;
        }
    }
}
