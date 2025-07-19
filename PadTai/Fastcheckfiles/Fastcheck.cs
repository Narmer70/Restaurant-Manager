using System;
using System.Linq;
using System.Data;
using System.Timers;
using System.Drawing;
using PadTai.Classes;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Globalization;
using System.Data.SqlClient;
using PadTai.Fastcheckfiles;
using PadTai.Classes.Others;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using PadTai.Sec_daryfolders.Others;
using PadTai.Classes.Controlsdesign;
using PadTai.Sec_daryfolders.Updates;
using PadTai.Classes.Fastcheckmodifiers;
using PadTai.Sec_daryfolders.Quitfolder;
using PadTai.Sec_daryfolders.Updaters.Staffupdater;
using static PadTai.Fastcheckfiles.DishGroupControl;
using PadTai.Sec_daryfolders.Updaters.ReceiptUpdater;


namespace PadTai
{
    public partial class Fastcheck : Form
    {
        private System.Timers.Timer checkNewReceiptTimer;
        public IndividualDiscount individualDiscount; 
        private DiscountControls discountcontrols;  
        private DishGroupControl dishGroupControl;
        private FoodItemManager _foodItemManager;
        private System.Timers.Timer clearTimer;
        private bool isDiscountUpdated = false;
        private Receiptdetails receiptdetails;
        private Clavieroverlay clavieroverlay;
        private Additionalbtns additionalbtns;
        public DataGridViewScroller scroller;
        private CrudDatabase crudDatabase;
        public int UpdreceiptId { get; set; }
        private FontResizer fontResizer;
        private ControlResizer resizer;
        private int lastReceiptId = 0;

        private int _totalItemCount;
        private int _grouppageNumber = 0;
        private const int ItemsPerPage = 6; 
        private bool selectionChanged = false;


        public Fastcheck(Receiptdetails details)
        {
            InitializeComponent();

            this.discountcontrols = new DiscountControls(this);
            this.individualDiscount = new IndividualDiscount(this);
            this.dishGroupControl = new DishGroupControl(this, null);
            crudDatabase = new CrudDatabase();
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            UpdateButtonState();        
            LoadGroupButtons();
            InitializeTimer();
            resizeControls();

            _foodItemManager = new FoodItemManager();
            _foodItemManager.TotalPriceUpdated += OnTotalPriceUpdated;

            label11.Visible = false;            
            label12.Visible = false;
            button1.Visible = false;
            button17.Visible = false;
            button36.Enabled = false;
            dataGridView3.Visible = false;
            this.receiptdetails = details;

            LocalizeControls();
            ApplyTheme();
        }



        private void resizeControls()
        {
            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(panel3);
            resizer.RegisterControl(panel4);
            resizer.RegisterControl(panel5);
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
            resizer.RegisterControl(label11);
            resizer.RegisterControl(label12);
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
            resizer.RegisterControl(btnequal);
            resizer.RegisterControl(textBox1);
            resizer.RegisterControl(textBox2);
            resizer.RegisterControl(button36);
            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(roundedPanel1);
            resizer.RegisterControl(dataGridView3);
            resizer.RegisterControl(dataGridView1);
            resizer.RegisterControl(dataGridView2);
            resizer.RegisterControl(dataGridView3);
            resizer.RegisterControl(Panelreceptacle);
            resizer.RegisterControl(tableLayoutPanel1);
            resizer.RegisterControl(tableLayoutPanel2);
            resizer.RegisterControl(tableLayoutPanel3);
            scroller = new DataGridViewScroller(this, null, dataGridView1, dataGridView2, dataGridView3);
           
            additionalbtns = new Additionalbtns(this);
            additionalbtns.Dock = DockStyle.Fill;
            panel3.Controls.Add(additionalbtns);
        }

        public void EnsureDataGridViewRows()
        {
            while (dataGridView2.Rows.Count < 6)
            {
                dataGridView2.Rows.Add();
            }
        }

        public void UpdateLabel(decimal newPrice, decimal discountAmount)
        {
            label7.Text = newPrice.ToString() + $" {CurrencyService.Instance.GetCurrencySymbol()}";
            label6.Text = "-" + discountAmount.ToString() + $" {CurrencyService.Instance.GetCurrencySymbol()}";
            button1.Text = LanguageManager.Instance.GetString("FCK-btn1") + Environment.NewLine + Environment.NewLine + newPrice.ToString() + $" {CurrencyService.Instance.GetCurrencySymbol()}";
            button3.Text = LanguageManager.Instance.GetString("FCK-btn3") + Environment.NewLine + Environment.NewLine + newPrice.ToString() + $" {CurrencyService.Instance.GetCurrencySymbol()}";
            isDiscountUpdated = true;
        }

        private void OnTotalPriceUpdated(decimal totalPrice)
        {
            if (isDiscountUpdated)
            {
                isDiscountUpdated = false;
                return;
            }
            label7.Text = totalPrice.ToString() + $" {CurrencyService.Instance.GetCurrencySymbol()}";
            button1.Text = LanguageManager.Instance.GetString("FCK-btn1") + Environment.NewLine + Environment.NewLine + totalPrice.ToString() + $" {CurrencyService.Instance.GetCurrencySymbol()}";
            button3.Text = LanguageManager.Instance.GetString("FCK-btn3") + Environment.NewLine + Environment.NewLine + totalPrice.ToString() + $" {CurrencyService.Instance.GetCurrencySymbol()}";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _timer.Tick += timer_Tick;
            _timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            label3.Text = $"{now.ToShortDateString()}  {now.ToLongTimeString()}";
            string currentDateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        }
        System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer { Interval = 1000 };

        public void UpdateButtonState()
        {
            bool hasNonEmptyRow = false;
            bool pricenotnull = false;

            if (dataGridView2.Rows.Count > 0 && dataGridView2.Rows[0].Cells[1].Value != null 
               && !string.IsNullOrWhiteSpace(dataGridView2.Rows[0].Cells[1].Value.ToString()))
            {      
                pricenotnull = true; 
            }
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    bool isRowEmpty = true;
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value != null && !string.IsNullOrWhiteSpace(cell.Value.ToString()))
                        {
                            isRowEmpty = false;
                            break;
                        }
                    }
                    if (!isRowEmpty)
                    {
                        hasNonEmptyRow = true;
                        break;
                    }
                }
            }

            button1.Enabled = hasNonEmptyRow && pricenotnull;
            button3.Enabled = hasNonEmptyRow && pricenotnull;
            button36.Enabled = hasNonEmptyRow && pricenotnull;
            button17.Enabled = hasNonEmptyRow && pricenotnull;
            
            if (additionalbtns != null) 
            {
                additionalbtns.button17.Enabled = hasNonEmptyRow && pricenotnull;
                additionalbtns.button19.Enabled = hasNonEmptyRow && pricenotnull;
            }
        }

        public void DeleteSelectedRow()
        {
            if (dataGridView1.SelectedRows.Count >= 1)
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    if (!row.IsNewRow)
                    {
                        dataGridView1.Rows.Remove(row);
                    }
                }
            }
            else
            {
                return;
            }
        }

        public void AddUserControl(UserControl UserControl)
        {
            UserControl.Dock = DockStyle.Fill;
            Panelreceptacle.Controls.Clear();
            Panelreceptacle.Controls.Add(UserControl);
            UserControl.BringToFront();
        }

        public void AdditionalControls(UserControl UserControl)
        {
            UserControl.Dock = DockStyle.Fill;
            panel3.Controls.Clear();
            panel3.Controls.Add(UserControl);
            UserControl.BringToFront();
        }

        private void InitializeTimer()
        {
            clearTimer = new System.Timers.Timer(600000);
            clearTimer.Elapsed += Cleardgv3;
            clearTimer.AutoReset = false;
        }

        private void InitializeCheckNewReceiptTimer()
        {
            checkNewReceiptTimer = new System.Timers.Timer(30000); 
            checkNewReceiptTimer.Elapsed += CheckForNewReceipts;
            checkNewReceiptTimer.AutoReset = true; // Repeat the check
            checkNewReceiptTimer.Start();
        }

        private void Cleardgv3(object sender, ElapsedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Cleardgv3(null, null)));
                return;
            }
            dataGridView3.Rows.Clear();
            dataGridView3.Visible = false;
        }

        public void LoadReceipt(int receiptId)
        {
            int clientId;
            if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                return;
            }

            string query = "SELECT FoodName FROM Receipts WHERE ReceiptId = @ReceiptId AND ClientID = @ClientID";
            var parameters = new Dictionary<string, object>
            {
                { "@ReceiptId", receiptId },
                { "@ClientID", clientId }
            };

            DataTable receiptData = crudDatabase.FetchDataFromDatabase(query, parameters);

            if (receiptData.Rows.Count > 0)
            {
                dataGridView3.Visible = true;
                dataGridView3.Rows.Clear();
                dataGridView3.Columns.Clear();
                dataGridView3.RowTemplate.Height = 18;
                dataGridView3.Columns.Add("FoodItem", "Food Item");
                dataGridView3.Columns[0].Width = dataGridView3.Width;
                dataGridView3.DefaultCellStyle.Font = new Font("Century Gothic", 9, FontStyle.Regular);
                dataGridView3.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView3.Rows.Add($"..........{receiptId}..........");

                string foodNames = receiptData.Rows[0]["FoodName"].ToString();
                var foodItems = foodNames.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(item => item.Trim());
                foreach (var foodItem in foodItems)
                {
                    dataGridView3.Rows.Add(foodItem);
                }
            }

            dataGridView3.ClearSelection();
            lastReceiptId = receiptId;
            clearTimer.Stop();
            clearTimer.Start();
            InitializeCheckNewReceiptTimer();
        }

        public void CheckForNewReceipts(object sender, ElapsedEventArgs e)
        {
            int clientId;
            if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                return;
            }

            string query = "SELECT MAX(ReceiptId) FROM Receipts WHERE ClientID = @ClientID";
            var parameters = new Dictionary<string, object>
            {
               { "@ClientID", clientId }
            };

            DataTable resultData = crudDatabase.FetchDataFromDatabase(query, parameters);
            if (resultData.Rows.Count > 0)
            {
                object result = resultData.Rows[0][0];
                if (result != null && result != DBNull.Value)
                {
                    int newReceiptId = Convert.ToInt32(result);
                    if (newReceiptId > lastReceiptId)
                    {
                        lastReceiptId = newReceiptId;
                        LoadReceipt(newReceiptId);
                    }
                }
            }
        }

        private void Fastcheck_Load(object sender, EventArgs e)
        {
            DishGroupControl MCL = new DishGroupControl(this, null);
            Panelreceptacle.Controls.Clear();
            MCL.Dock = DockStyle.Fill;
            Panelreceptacle.Controls.Add(MCL);

            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }

            if (Properties.Settings.Default.saveReceiptdraft && !Properties.Settings.Default.isUpdatepage) 
            {
                DraftReceiptSerializer.LoadDraft(dataGridView1, dataGridView2, label1, label8, textBox1, "draft_receipt.dat");

                if (dataGridView2.RowCount >= 4) 
                {
                    continiousDiscount();
                }
            }
        }

        private void Fastcheck_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
           
            if (Properties.Settings.Default.saveReceiptdraft && !Properties.Settings.Default.isUpdatepage)
            {
                DraftReceiptSerializer.SaveDraft(dataGridView1, dataGridView2, label1, label8, textBox1, "draft_receipt.dat");
            }
            else if (Properties.Settings.Default.saveReceiptdraft && Properties.Settings.Default.isUpdatepage)
            {
                Properties.Settings.Default.isUpdatepage = false;
                Properties.Settings.Default.Save();
            }
            this.Close();
        }

        private void button36_Click(object sender, EventArgs e)
        {
            DeliveryControls DL = new DeliveryControls(this);
            AddUserControl(DL);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            GetFirstPaymentTypeName();
           
            using (DefaultPayment DP = new DefaultPayment(this))
            {
                FormHelper.ShowFormWithOverlay(this, DP);
            }
        }

        private string GetFirstPaymentTypeName()
        {
            string paymentTypeName = string.Empty;

            string query = "SELECT PaymenttypeName FROM PaymentTypes LIMIT 1";
            DataTable paymentTypeData = crudDatabase.FetchDataFromDatabase(query);

            if (paymentTypeData.Rows.Count > 0)
            {
                paymentTypeName = paymentTypeData.Rows[0]["PaymenttypeName"].ToString();
            }

            // Ensure there are at least two rows in dataGridView2
            while (dataGridView2.Rows.Count < 2)
            {
                dataGridView2.Rows.Add();
            }

            // Set the values in the DataGridView
            dataGridView2.Rows[1].Cells[0].Value = LanguageManager.Instance.GetString("FIM-paytype");
            dataGridView2.Rows[1].Cells[1].Value = paymentTypeName;

            return paymentTypeName;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            RJButton btn = (RJButton)sender; 
            textBox2.Text += btn.Text;
            textBox2.Focus();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox2.Text.Length > 0) 
                { 
                    textBox2.Text = textBox2.Text.Substring(0, textBox2.Text.Length - 1); 
                }
                else 
                {
                    return; 
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private void btnequal_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                return;
            }

            if (dataGridView2.Rows.Count == 0 || dataGridView2.Rows[0].Cells.Count <= 1)
            {
                return;
            }

            // Try to get the value from row 5, cell 1 first
            double num2;

            if (dataGridView2.Rows.Count > 5 &&
                !string.IsNullOrWhiteSpace(dataGridView2.Rows[5].Cells[1].Value?.ToString()) &&
                double.TryParse(dataGridView2.Rows[5].Cells[1].Value.ToString(), out num2))
            {
                // If we successfully parsed the value from row 5, cell 1, proceed with subtraction
            }
            else if (!string.IsNullOrWhiteSpace(dataGridView2.Rows[0].Cells[1].Value?.ToString()) &&
                     double.TryParse(dataGridView2.Rows[0].Cells[1].Value.ToString(), out num2))
            {
                // If row 5, cell 1 is empty, try row 0, cell 1
            }
            else
            {
                // If both cells are empty or cannot be parsed, exit
                return;
            }

            // Now we can safely parse textBox2
            if (!double.TryParse(textBox2.Text, out double num1))
            {
                return;
            }

            try
            {
                double subtract = num1 - num2;
                textBox2.Text = subtract.ToString();
            }
            catch (Exception)
            {
              
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!selectionChanged)
            {
                dataGridView1.ClearSelection();
                selectionChanged = true;
            }
            else
            {
                selectionChanged = false;
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try 
            {
                if (e.RowIndex >= 0)
                {

                   using (Deleterow DR = new Deleterow(this, _foodItemManager))
                   {
                        FormHelper.ShowFormWithOverlay(this, DR);
                   }                   
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            selectionChanged = true;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {      
            UpdateButtonState();

            if (e.RowIndex >= 0 && e.ColumnIndex == 1) 
            {
                if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null 
                    && !string.IsNullOrWhiteSpace(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()))
                {
                    _foodItemManager.UpdateTotalPrice(dataGridView1, dataGridView2);
                  
                    if (dataGridView2.Rows.Count >= 4 && Properties.Settings.Default.contDiscountAnabled)
                    {
                        continiousDiscount();
                    }
                }
                else
                {
                }            
            }
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            UpdateButtonState();
            _foodItemManager.UpdateTotalPrice(dataGridView1, dataGridView2);

            if (dataGridView2.Rows.Count >= 4 && Properties.Settings.Default.contDiscountAnabled)
            {
                continiousDiscount();
            }
            else if (dataGridView2.Rows.Count >= 4 && !Properties.Settings.Default.contDiscountAnabled)
            {
                discountcontrols.ApplyDiscount(0);
                dataGridView2.Rows.RemoveAt(3);
                dataGridView2.Rows.RemoveAt(4);
                dataGridView2.Rows.RemoveAt(dataGridView2.Rows.Count - 1);
                label6.Text = "0.00";
            }

            int lastRowIndex = e.RowIndex + e.RowCount - 1;
            dataGridView1.Rows[lastRowIndex].Selected = true;
            dataGridView1.CurrentCell = dataGridView1.Rows[lastRowIndex].Cells[0];
            dataGridView1.FirstDisplayedScrollingRowIndex = lastRowIndex;
        }

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            UpdateButtonState();
            _foodItemManager.UpdateTotalPrice(dataGridView1, dataGridView2);
            ApplyIndividualDiscounts();

            if (dataGridView2.Rows.Count >= 4 && Properties.Settings.Default.contDiscountAnabled)
            {
                continiousDiscount();
            }
            else if (dataGridView2.Rows.Count >= 4 && !Properties.Settings.Default.contDiscountAnabled)
            {
                discountcontrols.ApplyDiscount(0);
                dataGridView2.Rows.RemoveAt(3);
                dataGridView2.Rows.RemoveAt(4);
                dataGridView2.Rows.RemoveAt(dataGridView2.Rows.Count - 1);
                label6.Text = "0.00";
            }

            if (dataGridView1.Rows.Count < 1)
            {
                dataGridView2.Rows.Clear();
                label6.Text = "0.00";
                label7.Text = "0.00";
                button1.Text = LanguageManager.Instance.GetString("FCK-btn1") + Environment.NewLine + Environment.NewLine + "0.00" + $" {CurrencyService.Instance.GetCurrencySymbol()}";
                button3.Text = LanguageManager.Instance.GetString("FCK-btn3") + Environment.NewLine + Environment.NewLine + "0.00" + $" {CurrencyService.Instance.GetCurrencySymbol()}";
            }
        }


        public void continiousDiscount()
        {
            decimal discount = 0.00m;
            decimal newPrice = 0.00m;
            decimal totalPrice = 0.00m;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow && row.Cells[1].Value != null)
                {
                    if (decimal.TryParse(row.Cells[1].Value.ToString(), out decimal cellValue))
                    {
                        totalPrice += cellValue;
                    }
                    else
                    {
                    }
                }
            }

            if (dataGridView2.Rows.Count >=4 && dataGridView2.Rows[3].Cells[1].Value != null)
            {
                if (decimal.TryParse(dataGridView2.Rows[3].Cells[1].Value.ToString(), out decimal discountPercentage))
                {
                    discount = (discountPercentage / 100m) * totalPrice;
                    newPrice = totalPrice - discount;
                }
                else
                {
                }
            }
            label7.Text = newPrice.ToString("F2") + $" {CurrencyService.Instance.GetCurrencySymbol()}";
            label6.Text = "-" + discount.ToString("F2") + $" {CurrencyService.Instance.GetCurrencySymbol()}";
            button1.Text = LanguageManager.Instance.GetString("FCK-btn1") + Environment.NewLine + Environment.NewLine + newPrice.ToString("F2") + $" {CurrencyService.Instance.GetCurrencySymbol()}";
            button3.Text = LanguageManager.Instance.GetString("FCK-btn3") + Environment.NewLine + Environment.NewLine + newPrice.ToString("F2") + $" {CurrencyService.Instance.GetCurrencySymbol()}";
            dataGridView2.Rows[4].Cells[0].Value = LanguageManager.Instance.GetString("Sumdiscount");
            dataGridView2.Rows[5].Cells[0].Value = LanguageManager.Instance.GetString("Newprice");
            dataGridView2.Rows[0].Cells[1].Value = totalPrice;
            dataGridView2.Rows[4].Cells[1].Value = discount;
            dataGridView2.Rows[5].Cells[1].Value = newPrice;

        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex == 0 && e.ColumnIndex == 0)
            {
                //e.CellStyle.ForeColor = System.Drawing.Color.DarkGray;
                e.CellStyle.Font = new System.Drawing.Font("Century Gothic", 10, FontStyle.Bold);
            }
            else if (e.RowIndex == 0 && e.ColumnIndex == 1)
            {
                //e.CellStyle.ForeColor = System.Drawing.Color.DarkGray;
                e.CellStyle.Font = new System.Drawing.Font("Century Gothic", 10, FontStyle.Bold);
            }
            else if (e.RowIndex == 1 && e.ColumnIndex == 0)
            {
                //e.CellStyle.ForeColor = System.Drawing.Color.DarkGray;
                e.CellStyle.Font = new System.Drawing.Font("Century Gothic", 10, FontStyle.Italic);
            }
            else if (e.RowIndex == 2 && e.ColumnIndex == 0)
            {
                //e.CellStyle.ForeColor = System.Drawing.Color.DarkGray;
                e.CellStyle.Font = new System.Drawing.Font("Century Gothic", 10, FontStyle.Italic);
            }
            else if (e.RowIndex == 3 && e.ColumnIndex == 0)
            {
                //e.CellStyle.ForeColor = System.Drawing.Color.DarkGray;
                e.CellStyle.Font = new System.Drawing.Font("Century Gothic", 10, FontStyle.Italic);

            }
            else if (e.RowIndex == 4 && e.ColumnIndex == 0)
            {
                //e.CellStyle.ForeColor = System.Drawing.Color.DarkGray;
                e.CellStyle.Font = new System.Drawing.Font("Century Gothic", 10, FontStyle.Italic);
            }
            else if (e.RowIndex == 5 && e.ColumnIndex == 0)
            {
                //e.CellStyle.ForeColor = System.Drawing.Color.DarkGray;
                e.CellStyle.Font = new System.Drawing.Font("Century Gothic", 10, FontStyle.Italic);
            }
            else if (e.RowIndex == 6 && e.ColumnIndex == 0)
            {
                //e.CellStyle.ForeColor = System.Drawing.Color.DarkGray;
                e.CellStyle.Font = new System.Drawing.Font("Century Gothic", 10, FontStyle.Italic);
            }
        }

        public void ApplyIndividualDiscounts()
        {
            var discounts = individualDiscount.GetDiscountConfigurations();
            _foodItemManager.ApplyIndividualDiscounts(dataGridView1, discounts);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Check if label1.Tag is not null
            if (label1.Tag != null)
            {
                if (int.TryParse(label1.Tag.ToString(), out int userId))
                {
                    Clientdiscount clientdiscount = new Clientdiscount();
                    clientdiscount.LoadUserData(userId); 
                    AddUserControl(clientdiscount);
                }
                else
                {
                }
            }
            else
            {
            }
        }



        public void insertreceipt()
        {
            Loaddatatodvgs(UpdreceiptId);
        }

        public void disablecontrols()
        {
            UpdateButtonState();
            button3.Visible = false;
            button1.Visible = true;
            button17.Visible = true;
            button1.Text = LanguageManager.Instance.GetString("FCK-btn1") + Environment.NewLine + Environment.NewLine + dataGridView2.Rows[0].Cells[1].Value.ToString() + $" {CurrencyService.Instance.GetCurrencySymbol()}";
            label7.Text = dataGridView2.Rows[0].Cells[1].Value.ToString() + $" {CurrencyService.Instance.GetCurrencySymbol()}";
            calculateDiscount();
        }

        private void calculateDiscount()
        {
            decimal discount = 0.00m;
            decimal totalPrice = 0.00m;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow && row.Cells[1].Value != null)
                {
                    if (decimal.TryParse(row.Cells[1].Value.ToString(), out decimal cellValue))
                    {
                        totalPrice += cellValue;
                    }
                    else
                    {
                    }
                }
            }

            if (dataGridView2.Rows.Count > 0 && dataGridView2.Rows[0].Cells[1].Value != null)
            {
                if (decimal.TryParse(dataGridView2.Rows[0].Cells[1].Value.ToString(), out decimal discountValue))
                {
                    discount = totalPrice - discountValue;
                }
                else
                {
                }
            }

            label6.Text = "-" + discount + $" {CurrencyService.Instance.GetCurrencySymbol()}";

            if (dataGridView2.Rows.Count > 3 && dataGridView2.Rows[3].Cells[1].Value != null &&
                decimal.TryParse(dataGridView2.Rows[3].Cells[1].Value.ToString(), out decimal Value) && Value != 0)
            {
                dataGridView2.Rows[5].Cells[1].Value = dataGridView2.Rows[0].Cells[1].Value.ToString();
                dataGridView2.Rows[4].Cells[0].Value = LanguageManager.Instance.GetString("Sumdiscount");
                dataGridView2.Rows[5].Cells[0].Value = LanguageManager.Instance.GetString("Newprice");
                dataGridView2.Rows[0].Cells[1].Value = totalPrice;
                dataGridView2.Rows[4].Cells[1].Value = discount; 
            }
        }

        private void Loaddatatodvgs(int receiptId)
        {
            int clientId;
            if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                return;
            }

            // Query to fetch details for the specific ReceiptId
            string query = "SELECT FoodName, Foodprice, FooditemtypeID, FoodID, Ordertable, BuyerID FROM Receipts WHERE ReceiptId = @ReceiptId AND ClientID = @ClientID";
            var parameters = new Dictionary<string, object>
            {
               { "@ReceiptId", receiptId },
               { "@ClientID", clientId }
            };

            DataTable receiptData = crudDatabase.FetchDataFromDatabase(query, parameters);

            if (receiptData.Rows.Count > 0)
            {
                var row = receiptData.Rows[0];
                string orderTable = row["Ordertable"].ToString();
                string foodIdString = row["FoodID"].ToString();
                string buyerID = row["BuyerID"].ToString();

                // Split the data into arrays
                string[] foodNames = row["FoodName"].ToString().Split(',');
                string[] foodPrices = row["Foodprice"].ToString().Split(',');
                string[] foodItemTypes = row["FooditemtypeID"].ToString().Split(',');
                string[] foodIDs = foodIdString.Split(',');

                // Determine the maximum number of elements to process
                int maxRows = Math.Min(10000, foodNames.Length);

                for (int i = 0; i < maxRows; i++)
                {
                    // Add a new row to the DataGridView
                    if (dataGridView1.Rows.Count < 10000)
                    {
                        int rowIndex = dataGridView1.Rows.Add();

                        // Set the values for the new row
                        dataGridView1.Rows[rowIndex].Cells[0].Value = i < foodNames.Length ? $"{new string(' ', 4)}{foodNames[i].Trim()}" : string.Empty;

                        if (i < foodPrices.Length)
                        {
                            string priceWithComma = foodPrices[i].Trim().Replace('.', ',');
                            dataGridView1.Rows[rowIndex].Cells[1].Value = priceWithComma;
                        }
                        else
                        {
                            dataGridView1.Rows[rowIndex].Cells[1].Value = string.Empty;
                        }

                        dataGridView1.Rows[rowIndex].Cells[2].Value = i < foodItemTypes.Length ? foodItemTypes[i].Trim() : string.Empty;

                        if (i < foodIDs.Length)
                        {
                            if (int.TryParse(foodIDs[i].Trim(), out int foodIDValue))
                            {
                                dataGridView1.Rows[rowIndex].Cells[3].Value = foodIDValue;
                            }
                            else
                            {
                                dataGridView1.Rows[rowIndex].Cells[3].Value = string.Empty;
                            }
                        }
                        else
                        {
                            dataGridView1.Rows[rowIndex].Cells[3].Value = string.Empty;
                        }
                    }
                }

                label8.Text = string.Empty;
                label1.Text = string.Empty;

                if (!string.IsNullOrEmpty(buyerID))
                {
                    string userName = receiptdetails.GetUsernameById(buyerID);
                    label1.Text = userName;
                    label1.Tag = buyerID;
                }

                if (!string.IsNullOrEmpty(orderTable))
                {
                    label8.Text = orderTable;
                }
            }

            // Now fetch additional information
            string additionalQuery = "SELECT TotalPrice, PaymenttypeName, PlacetoEatName, Thediscount FROM Receipts WHERE ReceiptId = @ReceiptId AND ClientID = @ClientID";
            DataTable additionalData = crudDatabase.FetchDataFromDatabase(additionalQuery, parameters);

            if (additionalData.Rows.Count > 0)
            {
                var additionalRow = additionalData.Rows[0];
                EnsureDataGridViewRows();

                object formattedPrice;
                if (!additionalRow.IsNull("TotalPrice"))
                {
                    if (double.TryParse(additionalRow["TotalPrice"].ToString(), out double totalPriceValue))
                    {
                        formattedPrice = totalPriceValue.ToString("F2");
                    }
                    else
                    {
                        formattedPrice = "Invalid Price";
                    }
                }
                else
                {
                    formattedPrice = "Price is null";
                }

                dataGridView2.Rows[0].Cells[1].Value = formattedPrice;
                dataGridView2.Rows[1].Cells[1].Value = additionalRow["PaymenttypeName"];
                dataGridView2.Rows[2].Cells[1].Value = additionalRow["PlacetoEatName"];
                dataGridView2.Rows[3].Cells[1].Value = additionalRow["Thediscount"];
                dataGridView2.Rows[0].Cells[0].Value = LanguageManager.Instance.GetString("FIM-totalprice");
                dataGridView2.Rows[1].Cells[0].Value = LanguageManager.Instance.GetString("FIM-paytype");
                dataGridView2.Rows[2].Cells[0].Value = LanguageManager.Instance.GetString("FIM-delivery");
                dataGridView2.Rows[3].Cells[0].Value = LanguageManager.Instance.GetString("FIM-discount");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (Updateprocess UPDP = new Updateprocess(this))
            {
                FormHelper.ShowFormWithOverlay(this, UPDP);
            }
        }

        public void updatereceiptcall()
        {
            UpdateReceiptData(UpdreceiptId);

            if (Properties.Settings.Default.saveReceiptdraft && Properties.Settings.Default.isUpdatepage)
            {
                Properties.Settings.Default.isUpdatepage = false;
                Properties.Settings.Default.Save();
            }
        }

        private void UpdateReceiptData(int receiptId)
        {
            decimal totalPrice = 0.0m; // Initialize totalPrice

            List<string> foodID = new List<string>();
            List<string> foodNames = new List<string>();
            List<string> foodPrices = new List<string>();
            List<string> foodItemTypes = new List<string>();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                foodNames.Add(row.Cells[0].Value?.ToString() ?? string.Empty);

                if (decimal.TryParse(row.Cells[1].Value?.ToString(), out decimal price))
                {
                    foodPrices.Add(price.ToString("F2", CultureInfo.InvariantCulture));
                }

                foodItemTypes.Add(row.Cells[2].Value?.ToString() ?? string.Empty);

                if (int.TryParse(row.Cells[3].Value?.ToString(), out int id))
                {
                    foodID.Add(id.ToString());
                }
            }

            string foodIDConcatenated = string.Join(", ", foodID);
            string foodNamesConcatenated = string.Join(", ", foodNames);
            string foodPricesConcatenated = string.Join(", ", foodPrices);
            string foodItemTypesConcatenated = string.Join(", ", foodItemTypes);

            // Determine total price from dataGridView2
            if (dataGridView2.Rows.Count >= 6 && dataGridView2.Rows[5].Cells.Count > 1 && dataGridView2.Rows[5].Cells[1].Value != null)
            {
                if (decimal.TryParse(dataGridView2.Rows[5].Cells[1].Value.ToString(), out decimal parsedPrice))
                {
                    totalPrice = parsedPrice;
                }
            }
            else if (dataGridView2.Rows.Count >= 1 && dataGridView2.Rows[0].Cells.Count > 1 && dataGridView2.Rows[0].Cells[1].Value != null)
            {
                if (decimal.TryParse(dataGridView2.Rows[0].Cells[1].Value.ToString(), out decimal parsedPrice))
                {
                    totalPrice = parsedPrice;
                }
            }
            else
            {
                totalPrice = 0.0m;
            }

            string paymentTypeName = dataGridView2.Rows[1].Cells[1].Value?.ToString() ?? string.Empty;
            string placeToEatName = dataGridView2.Rows[2].Cells[1].Value?.ToString() ?? string.Empty;

            string discount = dataGridView2.Rows.Count > 3 && !string.IsNullOrEmpty(dataGridView2.Rows[3].Cells[1].Value?.ToString())
                ? dataGridView2.Rows[3].Cells[1].Value.ToString()
                : "0";

            string updateQuery = @"UPDATE Receipts SET FoodName = @FoodName, Foodprice = @FoodPrice, 
                            FooditemtypeID = @FoodItemTypeID, TotalPrice = @TotalPrice, 
                            PaymenttypeName = @PaymentTypeName, PlacetoEatName = @PlaceToEatName, 
                            Thediscount = @Discount, FoodID = @FoodID, Ordertable = @Ordertable,
                            BuyerID = @BuyerID WHERE ReceiptId = @ReceiptId AND ClientID = @ClientID";

            var parameters = new Dictionary<string, object>
            {
                { "@FoodName", foodNamesConcatenated },
                { "@FoodPrice", foodPricesConcatenated },
                { "@FoodItemTypeID", foodItemTypesConcatenated },
                { "@TotalPrice", totalPrice },
                { "@PaymentTypeName", paymentTypeName },
                { "@PlaceToEatName", placeToEatName },
                { "@Discount", discount },
                { "@FoodID", foodIDConcatenated },
                { "@ReceiptId", receiptId },
                { "@ClientID", Convert.ToInt32(Properties.Settings.Default.SelectedClientId) },
                { "@Ordertable", string.IsNullOrEmpty(label8.Text) ? (object)DBNull.Value : label8.Text },
                { "@BuyerID", string.IsNullOrEmpty(label1.Text) ? (object)DBNull.Value : label1.Tag }
            };

            try
            {
                // Execute the update command using the crudDatabase class
                crudDatabase.ExecuteNonQuery(updateQuery, parameters);
                receiptdetails.updateforms();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            PayupdateControls payupdateControls = new PayupdateControls(this);
            AddUserControl(payupdateControls);
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            ActiveControl = (Control)sender;
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



        private void LoadGroupButtons()
        {
            // Clear existing controls
            tableLayoutPanel1.Controls.Clear();
            resizer = new ControlResizer(this.Size);
            resizer.RegisterControl(tableLayoutPanel1);

            // Load Groups
            List<Group> groups = dishGroupControl.GetAllGroups();
            _totalItemCount = groups.Count;
            groups = groups.Skip(_grouppageNumber * ItemsPerPage).Take(ItemsPerPage).ToList();

            // Set the number of columns to 8
            tableLayoutPanel1.ColumnCount = 8;

            // Clear existing column styles
            tableLayoutPanel1.ColumnStyles.Clear();

            // Set column styles
            float[] columnPercentages = new float[8] { 15F, 15F, 15F, 15F, 15F, 15F, 5F, 5F };

            foreach (float percentage in columnPercentages)
            {
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, percentage));
            }

            // Add group buttons (up to 6)
            for (int i = 0; i < Math.Min(groups.Count, 6); i++)
            {
                RJButton groupButton = new RJButton
                {
                    Text = groups[i].GroupName,
                    Tag = groups[i].GroupID,
                    Width = 145,
                    Height = 35,
                    BorderRadius = 3,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(2),
                    BackColor = Color.DodgerBlue,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                };
                resizer.RegisterControl(groupButton);
                groupButton.Click += GroupButton_Click;
                tableLayoutPanel1.Controls.Add(groupButton, i, 0); // Add to the current column
            }

            // Add previous Button
            if (_grouppageNumber > 0)
            {
                RJButton previousPageButton = new RJButton
                {
                    Text = "◀",
                    Width = 52,
                    Height = 35,
                    BorderRadius = 3,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(2),
                    BackColor = Color.DodgerBlue,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                };
                resizer.RegisterControl(previousPageButton);
                previousPageButton.Click += PreviousPageButton_Click;
                tableLayoutPanel1.Controls.Add(previousPageButton, 7, 0); 
            }

            // Add next Button
            if ((_grouppageNumber + 1) * ItemsPerPage < _totalItemCount)
            {
                RJButton nextPageButton = new RJButton
                {
                    Text = "▶",
                    Width = 52,
                    Height = 40,
                    BorderRadius = 3,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(2),
                    BackColor = Color.DodgerBlue,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                };
                resizer.RegisterControl(nextPageButton);
                nextPageButton.Click += NextPageButton_Click;
                tableLayoutPanel1.Controls.Add(nextPageButton, 6, 0); 
            }
        }

        private void PreviousPageButton_Click(object sender, EventArgs e)
        {
            if (_grouppageNumber > 0)
            {
                _grouppageNumber--;
                LoadGroupButtons();
            }
        }

        private void NextPageButton_Click(object sender, EventArgs e)
        {
            if ((_grouppageNumber + 1) * ItemsPerPage < _totalItemCount)
            {
                _grouppageNumber++;
                LoadGroupButtons();
            }
        }

        private void GroupButton_Click(object sender, EventArgs e)
        {
            RJButton button = sender as RJButton;
            int groupId = (int)button.Tag;

            if (dishGroupControl._navigationStack.Count >= 5)
            {
               dishGroupControl._navigationStack.Pop();
            }
            dishGroupControl._navigationStack.Push(new NavigationState { GroupId = dishGroupControl._currentGroupId = null, SubgroupId = dishGroupControl._currentSubgroupId = null, 
            SubsubgroupId = dishGroupControl._currentSubsubgroupId = null, PageNumber = dishGroupControl._pageNumber =0});
           
            dishGroupControl.LoadDishGroupControl(groupId);           
        }

        private void label11_Click(object sender, EventArgs e)
        {
            label8.Text = string.Empty; 
        }

        private void label12_Click(object sender, EventArgs e)
        {
            label1.Text = string.Empty; 
            label1.Tag = null;
            DishGroupControl MCL = new DishGroupControl(this, null);
            AddUserControl(MCL);
        }

        private void label8_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(label8.Text))
            {
                label11.Visible = true;
            }
            else
            {
                label11.Visible = false;
            }
        }

        private void label1_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(label1.Text))
            {
                label12.Visible = true;
            }
            else 
            {
                label12.Visible = false;
            }
        }

        private void label11_MouseHover(object sender, EventArgs e)
        {
            label11.ForeColor = Color.Crimson;
            Cursor = Cursors.Hand;
        }

        private void label11_MouseLeave(object sender, EventArgs e)
        {
            label11.ForeColor = Color.Tomato;
            Cursor = Cursors.Default;
        }

        private void label12_MouseHover(object sender, EventArgs e)
        {
            label12.ForeColor = Color.Crimson;
            Cursor = Cursors.Hand;
        }

        private void label12_MouseLeave(object sender, EventArgs e)
        {
            label12.ForeColor = Color.Tomato;
            Cursor = Cursors.Default;
        }

        public void LocalizeControls()
        {
            label9.Text = LanguageManager.Instance.GetString("FCH-lbl9");
            label2.Text = LanguageManager.Instance.GetString("FCH-Lbl2");
            label4.Text = LanguageManager.Instance.GetString("FCK-lbl4");
            label5.Text = LanguageManager.Instance.GetString("FCK-lbl5");
            button1.Text = LanguageManager.Instance.GetString("FCK-btn1");
            button2.Text = LanguageManager.Instance.GetString("FCK-btn2");
            button3.Text = LanguageManager.Instance.GetString("FCK-btn3");
            button4.Text = LanguageManager.Instance.GetString("Btn-close");
            button36.Text = LanguageManager.Instance.GetString("FCK-btn36");
            button17.Text = LanguageManager.Instance.GetString("FCK-btn17");
            textBox1.WaterMark = LanguageManager.Instance.GetString("FCK-txt1Watermark");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;
            panel4.BackColor = colors.Color3;
            panel5.BackColor = colors.Color3;
            button5.BackColor = colors.Color3;
            button6.BackColor = colors.Color3;
            button7.BackColor = colors.Color3;
            button8.BackColor = colors.Color3;
            button9.BackColor = colors.Color3;
            button10.BackColor = colors.Color3;
            button11.BackColor = colors.Color3;
            button13.BackColor = colors.Color3;
            button14.BackColor = colors.Color3;
            button15.BackColor = colors.Color3;
            button16.BackColor = colors.Color3;
            rjButton1.BackColor = colors.Color4;
            roundedPanel1.BackColor = colors.Color3;
            textBox1.BackColorRounded = colors.Color3;
            textBox2.BackColorRounded = colors.Color3;
            dataGridView1.BackgroundColor = colors.Color3;
            dataGridView2.BackgroundColor = colors.Color3;
            dataGridView3.BackgroundColor = colors.Color3;
            dataGridView1.DefaultCellStyle.BackColor = colors.Color3;
            dataGridView2.DefaultCellStyle.BackColor = colors.Color3;
            dataGridView3.DefaultCellStyle.BackColor = colors.Color3;
            dataGridView2.DefaultCellStyle.SelectionBackColor = colors.Color3;
            dataGridView3.DefaultCellStyle.SelectionBackColor = colors.Color3;

            label1.ForeColor = this.ForeColor;
            label2.ForeColor = this.ForeColor;
            label4.ForeColor = this.ForeColor;
            label5.ForeColor = this.ForeColor;
            label6.ForeColor = this.ForeColor;
            label7.ForeColor = this.ForeColor;
            label8.ForeColor = this.ForeColor;
            label9.ForeColor = this.ForeColor;
            button5.ForeColor = this.ForeColor;
            button6.ForeColor = this.ForeColor;
            button7.ForeColor = this.ForeColor;
            button8.ForeColor = this.ForeColor;
            button9.ForeColor = this.ForeColor;
            button10.ForeColor = this.ForeColor;
            button11.ForeColor = this.ForeColor;
            button13.ForeColor = this.ForeColor;
            button14.ForeColor = this.ForeColor;
            button15.ForeColor = this.ForeColor;
            button16.ForeColor = this.ForeColor;
            textBox1.ForeColor = this.ForeColor;
            textBox2.ForeColor = this.ForeColor;
            rjButton1.ForeColor = this.ForeColor;
            dataGridView1.ForeColor = this.ForeColor;
            dataGridView2.ForeColor = this.ForeColor;
            dataGridView3.ForeColor = this.ForeColor;
            dataGridView1.DefaultCellStyle.ForeColor = this.ForeColor;
            dataGridView2.DefaultCellStyle.ForeColor = this.ForeColor;
            dataGridView3.DefaultCellStyle.ForeColor = this.ForeColor;
            dataGridView1.DefaultCellStyle.SelectionForeColor = this.ForeColor;
            dataGridView2.DefaultCellStyle.SelectionForeColor = this.ForeColor;
            dataGridView3.DefaultCellStyle.SelectionForeColor = this.ForeColor;
            if (colors.Bool1 && Properties.Settings.Default.showScrollbars) showScrollBars();
        }

        private void showScrollBars()
        {
            dataGridView1.ScrollBars = ScrollBars.Vertical;
            dataGridView2.ScrollBars = ScrollBars.Vertical;
            dataGridView3.ScrollBars = ScrollBars.Vertical;
        }
    }

    public class DiscountConfig
    {
        public int DiscountPercentage { get; set; }
        public int OccurrencesRequired { get; set; }
    }
}

