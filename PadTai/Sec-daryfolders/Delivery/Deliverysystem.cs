using System;
using System.Drawing;
using PadTai.Classes;
using System.Data.SQLite;
using System.Globalization;
using System.Windows.Forms;
using System.Data.SqlClient;
using PadTai.Classes.Others;
using PadTai.Classes.Databaselink;
using PadTai.Sec_daryfolders.Delivery;


namespace PadTai.Sec_daryfolders
{
    public partial class Deliverysystem : Form
    {
        private BusinessInfo businessInfo;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public Deliverysystem()
        {
            InitializeComponent();
            initialiseControlsResizing();
            crudDatabase = new CrudDatabase();
            businessInfo = new BusinessInfo(label1, label2); 

            LocalizeControls();
            ApplyTheme();
        }

        private void initialiseControlsResizing()
        {
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);
            resizer.RegisterControl(label1);
            resizer.RegisterControl(label2);
            resizer.RegisterControl(panel1);
            resizer.RegisterControl(button1);
            resizer.RegisterControl(button2);
            resizer.RegisterControl(button3);
            resizer.RegisterControl(button4);
            resizer.RegisterControl(button5);
            resizer.RegisterControl(button6);
            resizer.RegisterControl(button7);
            resizer.RegisterControl(button8);
            resizer.RegisterControl(button9);
            resizer.RegisterControl(button10);
            resizer.RegisterControl(button11);
            resizer.RegisterControl(button12);
            resizer.RegisterControl(button13);
            resizer.RegisterControl(button14);
            resizer.RegisterControl(button15);
            resizer.RegisterControl(button16);
            resizer.RegisterControl(button17);
            resizer.RegisterControl(button18);
            resizer.RegisterControl(tableLayoutPanel1);
        }


        private void Deliverysystem_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void Deliverysystem_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            
        }

        private void button11_Click(object sender, EventArgs e)
        {
            using (Deliverymap DM = new Deliverymap())
            {
                FormHelper.ShowFormWithOverlay(this.FindForm(), DM);
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {

        }

        private void button14_Click(object sender, EventArgs e)
        {

        }

        private void button15_Click(object sender, EventArgs e)
        {

        }

        private void button16_Click(object sender, EventArgs e)
        {

        }

        private void button17_Click(object sender, EventArgs e)
        {

        }

        private void button18_Click(object sender, EventArgs e)
        {

        }

        public void LocalizeControls()
        {
            button1.Text = LanguageManager.Instance.GetString("Btn-close");
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
            panel1.BackColor = colors.Color3;
            tableLayoutPanel1.BackColor = colors.Color1;

            label1.ForeColor = this.ForeColor;
            label2.ForeColor= this.ForeColor;
            tableLayoutPanel1.ForeColor = this.ForeColor;
        }
    }
}
