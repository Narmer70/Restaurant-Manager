using System;
using PadTai.Classes;
using System.Drawing;
using System.Windows.Forms;
using PadTai.Classes.Others;

namespace PadTai.Sec_daryfolders.Quitfolder
{
    public partial class Deleterow : Form
    {
        private FoodItemManager _foodItemManager;
        private FormResizer formResizer;
        private ControlResizer resizer;
        private Fastcheck FCH;


        public Deleterow(Fastcheck form, FoodItemManager foodanager)
        {
            InitializeComponent();
            this.FCH = form;
            _foodItemManager = foodanager;

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(label1);
            resizer.RegisterControl(button1);
            resizer.RegisterControl(button2);

            LocalizeControls();
            ApplyTheme();
        }


        private void Deleterow_Resize(object sender, EventArgs e)
        {

            if (resizer != null)
            {
                resizer.ResizeControls(this);
                CenterLabel();
            }
        }

        private void Deleterow_Load(object sender, EventArgs e)
        {
            formResizer = new FormResizer(this);
            formResizer.Resize(this);
            CenterLabel();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            FCH.DeleteSelectedRow();
            FCH.UpdateButtonState();
            _foodItemManager.UpdateTotalPrice(FCH.dataGridView1, FCH.dataGridView2);
          
            if (FCH.dataGridView2.Rows.Count >= 4 && Properties.Settings.Default.contDiscountAnabled)
            {
                FCH.continiousDiscount();
            }

            this.Close();
        }

        private void CenterLabel()
        {
            label1.Left = (this.ClientSize.Width - label1.Width) / 2;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void LocalizeControls()
        {
            button1.Text = LanguageManager.Instance.GetString("Yesbutton");
            button2.Text = LanguageManager.Instance.GetString("Btnannuler");
            label1.Text = LanguageManager.Instance.GetString("Popdeletedish");
        }

        public void ApplyTheme()
        {

        }
    }
}
