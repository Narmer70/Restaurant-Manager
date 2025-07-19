using System;
using PadTai.Classes;
using System.Drawing;
using System.Windows.Forms;
using PadTai.Classes.Others;
using PadTai.Sec_daryfolders.Updaters.FoodUpdater.Delete;


namespace PadTai.Sec_daryfolders.Updaters.FoodUpdater
{
    public partial class Deletedishconfirm : Form
    {
        private FormResizer formResizer;
        private ControlResizer resizer;

        public Deletedishconfirm()
        {
            InitializeComponent();

            button1.DialogResult = DialogResult.OK;
            button2.DialogResult = DialogResult.Cancel;

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(label1);
            resizer.RegisterControl(button1);
            resizer.RegisterControl(button2);

            LocalizeControls();
            ApplyTheme();
        }

        private void Deletedishconfirm_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                CenterLabel();
            }

            formResizer = new FormResizer(this);
            formResizer.Resize(this);
        }

        private void Deletedishconfirm_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                CenterLabel();
            }
        }

        private void CenterLabel()
        {
            label1.Left = (this.ClientSize.Width - label1.Width) / 2;
        }


        public void LocalizeControls()
        {
            button1.Text = LanguageManager.Instance.GetString("Btnconfirm");
            button2.Text = LanguageManager.Instance.GetString("Btnannuler");
            label1.Text = LanguageManager.Instance.GetString("Delete-lbl1");
        }

        public void ApplyTheme()
        {
           
        }
    }
}
