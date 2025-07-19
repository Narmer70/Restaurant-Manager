using System;
using PadTai.Classes;
using System.Drawing;
using System.Windows.Forms;
using PadTai.Classes.Others;
using PadTai.Sec_daryfolders.Update;
using PadTai.Sec_daryfolders.Allreports.Otdelreports;

namespace PadTai.Sec_daryfolders.Departmentdata
{
    public partial class Departementsplitter : Form
    {
        private DraggableForm draggableForm;
        private FontResizer fontResizer;
        private ControlResizer resizer;


        public Departementsplitter()
        {
            InitializeComponent();

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);
            draggableForm = new DraggableForm();
            draggableForm.EnableDragging(this, this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(rjButton2);
            resizer.RegisterControl(rjButton3);
            resizer.RegisterControl(rjButton4);
            resizer.RegisterControl(rjButton5);
            resizer.RegisterControl(rjButton6);
            resizer.RegisterControl(rjButton7);
            resizer.RegisterControl(rjButton8);
            resizer.RegisterControl(rjButton9);
            resizer.RegisterControl(rjButton10);
            resizer.RegisterControl(roundedPanel1);

            LocalizeControls();
            ApplyTheme();
        }

        private void rjButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Departementsplitter_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void Departementsplitter_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
            using (Departcharts DPT = new Departcharts())
            {
                FormHelper.ShowFormWithOverlay(this.FindForm(), DPT);
            }
        }

        private void rjButton8_Click(object sender, EventArgs e)
        {
            using (Companywebsite CWS = new Companywebsite())
            {
                FormHelper.ShowFormWithOverlay(this.FindForm(), CWS);
            }
        }

        private void rjButton5_Click(object sender, EventArgs e)
        {

        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            using (Allotdelreports CWS = new Allotdelreports())
            {
                FormHelper.ShowFormWithOverlay(this.FindForm(), CWS);
            }
        }

        public void LocalizeControls()
        {
            //button1.Text = LanguageManager.Instance.GetString("MF-btn1");
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
            //button4.Text = LanguageManager.Instance.GetString("MF-btn4");
            //button5.Text = LanguageManager.Instance.GetString("MF-btn5");
            //button6.Text = LanguageManager.Instance.GetString("MF-btn6");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            rjButton3.ForeColor = colors.Color2;
            rjButton3.BackColor = colors.Color3;
            roundedPanel1.GradientBottomColor = colors.Color3;
        }
    }
}
