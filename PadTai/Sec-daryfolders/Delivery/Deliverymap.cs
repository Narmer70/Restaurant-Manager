using System;
using System.IO;
using PadTai.Classes;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Globalization;
using PadTai.Classes.Others;
using System.Device.Location;
using PadTai.Classes.Controlsdesign;
using PadTai.Sec_daryfolders.Others;


namespace PadTai.Sec_daryfolders.Delivery
{
    public partial class Deliverymap : Form
    {
        private Clavieroverlay clavieroverlay;
        private DraggableForm draggableForm;
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public Deliverymap()
        {
            InitializeComponent();

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);
            draggableForm = new DraggableForm();
            draggableForm.EnableDragging(this, this);
            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(textBox1);
            resizer.RegisterControl(rjButton2);
            resizer.RegisterControl(rjButton1);

            LocalizeControls();
            ApplyTheme();
        }

        private void Deliverymap_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void Deliverymap_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {

        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void textBox1_Enter(object sender, EventArgs e)
        {            
            if (Properties.Settings.Default.showKeyboard)
            {
                clavieroverlay = new Clavieroverlay(textBox1);
                clavieroverlay.boardLocationBottom();
                clavieroverlay.Show();
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            clavieroverlay?.Hide();
        }

        public void LocalizeControls()
        {
            rjButton2.Text = LanguageManager.Instance.GetString("Btn-close");
            //button1.Text = LanguageManager.Instance.GetString("MF-btn1");
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;
            textBox1.BackColorRounded = colors.Color3;

            textBox1.ForeColor = this.ForeColor;
        }
    }   
}
  

