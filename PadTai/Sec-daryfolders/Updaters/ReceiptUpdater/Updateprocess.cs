using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using PadTai.Classes;
using System.Windows.Forms;
using PadTai.Fastcheckfiles;
using PadTai.Classes.Others;


namespace PadTai.Sec_daryfolders.Updaters.ReceiptUpdater
{
    public partial class Updateprocess : Form
    {
        private FormResizer formResizer;
        private ControlResizer resizer;
        private Fastcheck fastchecks;

        public Updateprocess(Fastcheck fastcheck)
        {
            InitializeComponent();

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(label1);
            resizer.RegisterControl(button1);
            resizer.RegisterControl(button2);
            this.fastchecks = fastcheck;
;
            LocalizeControls();
            ApplyTheme();
        }

        private void Updateprocess_Load(object sender, EventArgs e)
        {
            formResizer = new FormResizer(this);
            formResizer.Resize(this);
            CenterLabel();
        }

        private void Updateprocess_Resize(object sender, EventArgs e)
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

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fastchecks.updatereceiptcall();
            fastchecks.Close();
            this.Close();
        }

        public void LocalizeControls()
        {
            button1.Text = LanguageManager.Instance.GetString("Yesbutton");
            button2.Text = LanguageManager.Instance.GetString("Nobutton");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
        }

        public void ApplyTheme()
        {

        }
    }
}
