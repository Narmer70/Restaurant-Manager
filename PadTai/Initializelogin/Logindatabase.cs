using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using PadTai.Classes;
using System.Windows.Forms;
using System.ComponentModel;
using PadTai.Classes.Others;
using System.Threading.Tasks;
using System.Collections.Generic;
using PadTai.Classes.Controlsdesign;
using PadTai.Sec_daryfolders.DB_Appinitialize;


namespace PadTai.Sec_daryfolders.App_DBInitializer
{
    public partial class Logindatabase : Form
    {
        private DraggableForm draggableForm;
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public Logindatabase()
        {
            InitializeComponent();

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);
            draggableForm = new DraggableForm();
            draggableForm.EnableDragging(this, this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(label1);
            resizer.RegisterControl(label2);
            resizer.RegisterControl(label3);
            resizer.RegisterControl(label4);
            resizer.RegisterControl(label5);
            resizer.RegisterControl(label6);
            resizer.RegisterControl(label7);
            resizer.RegisterControl(label8);
            resizer.RegisterControl(label9);
            resizer.RegisterControl(label10);
            //resizer.RegisterControl(Emailbox);
            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(rjButton2);
            resizer.RegisterControl(rjButton4);
            resizer.RegisterControl(rjButton5);
            resizer.RegisterControl(linkLabel1);
            resizer.RegisterControl(lblMessage);
            //resizer.RegisterControl(pictureBox2);
            //resizer.RegisterControl(pictureBox3);
            //resizer.RegisterControl(pictureBox4);
            resizer.RegisterControl(Passwordbox);
            resizer.RegisterControl(roundedPanel1);
            resizer.RegisterControl(roundedPanel2);
            //resizer.RegisterControl(roundedPanel3);
            //resizer.RegisterControl(roundedPanel4);
            //resizer.RegisterControl(roundedPanel5);
            resizer.RegisterControl(Databasenamebox);

            LanguageManager.Instance.LanguageChanged += HandleLanguageChange;
            ThemeManager.ThemeChanged += ApplyTheme;
            ThemeManager.LoadTheme();
            LocalizeControls();
            ApplyTheme();
        }

        private void Logindatabase_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void Logindatabase_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            Databasecreate databasecreate = new Databasecreate();
            databasecreate.ShowDialog();
        }

        private void HandleLanguageChange()
        {
            LocalizeControls();
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

            label1.ForeColor = colors.Color2;
            label2.ForeColor = colors.Color2;
            label3.ForeColor = colors.Color2;
            label6.ForeColor = colors.Color2;
            label8.ForeColor = colors.Color2;
            label9.ForeColor = colors.Color2;
            customBox1.ForeColor = colors.Color2;
            customBox2.ForeColor = colors.Color2;
            customBox1.BackColor = colors.Color3;
            customBox2.BackColor = colors.Color3;
            customBox1.ArrowColor = colors.Color2;
            customBox2.ArrowColor = colors.Color2;
            Passwordbox.ForeColor = colors.Color2;
            Passwordbox.BackColor = colors.Color3;
            Databasenamebox.ForeColor = colors.Color2;
            Databasenamebox.BackColor = colors.Color3;
            roundedPanel1.GradientTopColor = colors.Color1;
            roundedPanel2.GradientTopColor = colors.Color1;
            roundedPanel1.GradientBottomColor = colors.Color3;
            roundedPanel2.GradientBottomColor = colors.Color3;
        }
    }
}
