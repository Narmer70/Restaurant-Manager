using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using PadTai.Classes;
using System.Data.SQLite;
using System.Windows.Forms;
using PadTai.Classes.Others;
using System.Drawing.Printing;
using PadTai.Classes.Databaselink;
using System.Collections.Generic;


namespace PadTai.Fastcheckfiles
{
    public partial class PayConfirmForm : Form
    {
        private CrudDatabase crudDatabase;
        private FormResizer formResizer;
        private ControlResizer resizer;
        Fastcheck FCH;

        private int _paymentTypeId;
        public int PaymentTypeId
        {
            get 
            {
                return _paymentTypeId; 
            }
            set 
            {   _paymentTypeId = value;
                label1.Text = LanguageManager.Instance.GetString("Popdefaultpay") + " '" +GetPaymentTypeName(_paymentTypeId) + "'" + " ?";
            }
        }


        public PayConfirmForm(Fastcheck fCH)
        {
            InitializeComponent();

            crudDatabase = new CrudDatabase();
            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(label1);
            resizer.RegisterControl(button1);
            resizer.RegisterControl(button2);

            FCH = fCH;

            LocalizeControls();
            ApplyTheme();
        }

        private void PayConfirmForm_Load(object sender, EventArgs e)
        {
            formResizer = new FormResizer(this);
            formResizer.Resize(this);
            CenterLabel();
        }

        private string GetPaymentTypeName(int paymentTypeId)
        {
            string query = "SELECT PaymenttypeName FROM PaymentTypes WHERE PaymentypeID = @PaymentTypeId";
           
            var parameters = new Dictionary<string, object>
            {
                { "@PaymentTypeId", paymentTypeId }
            };

            DataTable paymentTypesTable = crudDatabase.FetchDataFromDatabase(query, parameters);

            if (paymentTypesTable.Rows.Count > 0)
            {
                return paymentTypesTable.Rows[0]["PaymenttypeName"].ToString();
            }

            return string.Empty; // Return empty if no payment type is found
        }

        private void PayConfirmForm_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                CenterLabel();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Create an instance of FoodItemManager
                FoodItemManager managers = new FoodItemManager();

                // Save receipts using the SaveReceipts method
                managers.SaveReceipts(FCH.dataGridView1, FCH.dataGridView2, FCH.label8, FCH.label1);

                if (FCH != null && FCH.dataGridView1 != null)
                {
                    FCH.dataGridView1.Rows.Clear();
                }
                DishGroupControl dishGroupControl = new DishGroupControl(FCH, null);
                FCH.AddUserControl(dishGroupControl);

                timer1.Start();
                string currentDateTime = DateTime.Now.ToString("dd/MM/yyyy  HH:mm:ss");

                FCH.label6.Text = "0.00";
                FCH.label7.Text = "0.00";
                FCH.dataGridView2.Rows.Clear();
                FCH.label1.Text = string.Empty;
                FCH.label8.Text = string.Empty;
                FCH.textBox1.Text = string.Empty;
                FCH.CheckForNewReceipts(null, null);
                FCH.label10.Text = $"{currentDateTime}";
                FCH.textBox1.WaterMark = LanguageManager.Instance.GetString("FCK-txt1Watermark");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving receipts: {ex.Message}");
            }
         
            this.Close();
        }
        private void CenterLabel()
        {
            label1.Left = (this.ClientSize.Width - label1.Width) / 2;
        }

        public void LocalizeControls()
        {
            button1.Text = LanguageManager.Instance.GetString("Yesbutton");
            button2.Text = LanguageManager.Instance.GetString("Nobutton");
        }

        public void ApplyTheme()
        {
          
        }
    }
}
