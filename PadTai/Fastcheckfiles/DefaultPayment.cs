using System;
using System.Data;
using System.Linq;
using System.Text;
using PadTai.Classes;
using System.Drawing;
using System.Data.SQLite;
using System.Windows.Forms;
using PadTai.Classes.Others;
using PadTai.Classes.Databaselink;


namespace PadTai.Fastcheckfiles
{
    public partial class DefaultPayment : Form
    {
        private CrudDatabase crudDatabase;
        private FormResizer formResizer;
        private ControlResizer resizer;
        Fastcheck FCH;

        public DefaultPayment(Fastcheck fCH)
        {
            InitializeComponent();

            crudDatabase = new CrudDatabase();
            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(label1);
            resizer.RegisterControl(button1);
            resizer.RegisterControl(button2);
            FCH = fCH;

            GetFirstPaymentTypeName();
            LocalizeControls();
            ApplyTheme();
        }

        private string GetFirstPaymentTypeName()
        {
            string query = "SELECT PaymenttypeName FROM PaymentTypes LIMIT 1";
            DataTable paymentTypesTable = crudDatabase.FetchDataFromDatabase(query);

            if (paymentTypesTable.Rows.Count > 0)
            {
                string paymentTypeName = paymentTypesTable.Rows[0]["PaymenttypeName"].ToString();
                label1.Text = LanguageManager.Instance.GetString("Popdefaultpay") + " '" + paymentTypeName + "'" + " ?";
                return paymentTypeName;
            }

            return string.Empty; // Return empty if no payment types are found
        }

        private void DefaultPayment_Load(object sender, EventArgs e)
        {
            formResizer = new FormResizer(this);
            formResizer.Resize(this);
            CenterLabel();
        }

        private void DefaultPayment_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                CenterLabel();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PaymentControls PCS = new PaymentControls(FCH, null);
            FCH.AddUserControl(PCS);
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
