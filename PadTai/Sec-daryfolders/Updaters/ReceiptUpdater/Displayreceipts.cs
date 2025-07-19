using System;
using System.Data;
using System.Linq;
using PadTai.Classes;
using System.Drawing;
using System.Data.SQLite;
using System.Globalization;
using System.Windows.Forms;
using System.Data.SqlClient;
using PadTai.Classes.Others;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using PadTai.Classes.Controlsdesign;
using PadTai.Sec_daryfolders.Quitfolder;


namespace PadTai.Sec_daryfolders.Updates
{
    public partial class Displayreceipts : Form
    {
        ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

        private CrudDatabase crudDatabase;
        private BusinessInfo businessInfo;
        private FontResizer fontResizer;
        private ControlResizer resizer;

        private const int ItemsPerPageFirstPage = 8; 
        private const int ItemsPerPageOtherPages = 7;
        private int _currentPage = 1;
        private int _totalReceipts = 0;


        public Displayreceipts()
        {
            InitializeComponent();
            initialiseResizeControls();

            crudDatabase = new CrudDatabase();
            businessInfo = new BusinessInfo(label2, label1);

            UpdateControlVisibility();
            LocalizeControls();
            ApplyTheme();
            LoadData();
        }

        private void initialiseResizeControls() 
        {
            panel2.Visible = false;
            tableLayoutPanel1.Visible = false;

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(panel2);
            resizer.RegisterControl(label1);
            resizer.RegisterControl(label2);
            resizer.RegisterControl(label3);
            resizer.RegisterControl(button1);
            resizer.RegisterControl(pictureBox1);
            resizer.RegisterControl(tableLayoutPanel1);
        }


        public (bool HasMoreThanOne, int ReceiptCount) HasMoreThanOneClient()
        {
            int clientId;
            if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                _totalReceipts = 0;
                return (false, 0);
            }

            string query = "SELECT COUNT(*) AS ClientCount FROM Receipts WHERE ClientID = @ClientID";
            var parameters = new Dictionary<string, object>
            {
                { "@ClientID", clientId }
            };

            DataTable resultTable = crudDatabase.FetchDataFromDatabase(query, parameters);

            if (resultTable != null && resultTable.Rows.Count > 0)
            {
                long count = Convert.ToInt64(resultTable.Rows[0]["ClientCount"]);
                _totalReceipts = Convert.ToInt32(count);

                return (count > 0, _totalReceipts); // Return true if count is greater than 0
            }
            else
            {
                _totalReceipts = 0;
                return (false, 0);
            }
        }

        public void UpdateControlVisibility()
        {
            // Unpack the tuple and ignore the ReceiptCount
            var (hasMoreThanOneClient, _) = HasMoreThanOneClient();

            panel2.Visible = !hasMoreThanOneClient;
            tableLayoutPanel1.Visible = hasMoreThanOneClient;
        }

        private void CenterLabel()
        {
            panel2.Left = (this.ClientSize.Width - panel2.Width) / 2;
            //label1.Left = (this.ClientSize.Width - label1.Width) / 2;
            //label2.Left = (this.ClientSize.Width - label2.Width) / 2;
            label3.Left = (panel2.ClientSize.Width - label3.Width) / 2;
        }


        private List<Receipt> allReceipts = new List<Receipt>();
        public void LoadData()
        {
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowStyles.Clear();

            int rowIndex = 0;
            int itemsPerPage = _currentPage == 1 ? ItemsPerPageFirstPage : ItemsPerPageOtherPages;
            int startIndex = (_currentPage - 1) * itemsPerPage;

            RJButton buttonPrevious = new RJButton
            {
                Text = "◀",
                Width = 145,
                Height = 75,
                BorderRadius = 0,
                Dock = DockStyle.Fill,
                Margin = new Padding(4),
                Visible = _currentPage > 1,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            resizer.RegisterControl(buttonPrevious);
            buttonPrevious.Click += buttonPrevious_Click;

            RJButton buttonNext = new RJButton
            {
                Text = "▶",
                Width = 145,
                Height = 75,
                BorderRadius = 0,
                Dock = DockStyle.Fill,
                Margin = new Padding(4),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Visible = (_currentPage * itemsPerPage) < _totalReceipts
            };
            resizer.RegisterControl(buttonNext);
            buttonNext.Click += buttonNext_Click;

            try
            {
                int clientId;
                if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
                {
                    MessageBox.Show("Invalid Client ID format.");
                    return;
                }

                string query = "SELECT * FROM Receipts WHERE ClientID = @ClientID ORDER BY ReceiptId DESC";
                var parameters = new Dictionary<string, object>
                {
                    { "@ClientID", clientId }
                };

                DataTable resultTable = crudDatabase.FetchDataFromDatabase(query, parameters);
                allReceipts.Clear();

                if (resultTable != null)
                {
                    foreach (DataRow row in resultTable.Rows)
                    {
                        Receipt receipt = new Receipt
                        {
                            BuyerID = row["BuyerID"].ToString(),
                            FoodName = row["FoodName"].ToString(),
                            Foodprice = row["Foodprice"].ToString(),
                            Ordertable = row["Ordertable"].ToString(),
                            ReceiptId = Convert.ToInt32(row["ReceiptId"]),
                            PaymenttypeName = row["PaymenttypeName"].ToString(),
                            OrderDateTime = Convert.ToDateTime(row["OrderDateTime"]),
                            TotalPrice = row.IsNull("TotalPrice") ? 0 : Convert.ToDouble(row["TotalPrice"])
                        };

                        allReceipts.Add(receipt);
                    }
                }

                List<Receipt> currentPageReceipts = allReceipts.Skip(startIndex).Take(itemsPerPage).ToList();

                float[] rowsPercentages = new float[3] { 33.3F, 33.3F, 33.3F };

                foreach (float percentage in rowsPercentages)
                {
                    tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, percentage));
                }

                foreach (Receipt receipt in currentPageReceipts)
                {
                    RoundedPanel panel = new RoundedPanel
                    {
                        BorderRadius = 0,
                        Dock = DockStyle.Fill,
                        Margin = new Padding(4),
                        BackColor = Color.Transparent,
                        GradientTopColor = Color.Transparent,
                        GradientBottomColor = Color.Transparent
                    };
                    resizer.RegisterControl(panel);

                    DataGridView dgv = new DataGridView
                    {
                        ReadOnly = true,
                        Dock = DockStyle.Fill,
                        Margin = new Padding(4),
                        RowHeadersVisible = false,
                        AllowUserToAddRows = false,
                        ColumnHeadersVisible = false,
                        ScrollBars = ScrollBars.None,
                        AllowUserToDeleteRows = false,
                        AllowUserToResizeRows = false,
                        BorderStyle = BorderStyle.None,
                        AllowUserToOrderColumns = false,
                        AllowUserToResizeColumns = false,
                        EnableHeadersVisualStyles = true,
                        Tag = receipt.ReceiptId.ToString(),
                        CellBorderStyle = DataGridViewCellBorderStyle.None,
                        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    };
                    resizer.RegisterControl(dgv);

                    dgv.CellFormatting += Dgv_CellFormatting;
                    dgv.MouseEnter += Dgv_MouseEnter;
                    dgv.MouseLeave += Dgv_MouseLeave;                    
                    dgv.CellClick += Dgv_CellClick;
                    dgv.Resize += Dgv_Resize;

                    dgv.RowTemplate.Height = 22;

                    dgv.ColumnCount = 2;
                    dgv.Columns[0].Width = (int)(dgv.Width * 0.80);
                    dgv.Columns[1].Width = (int)(dgv.Width * 0.20);

                    dgv.Rows.Add(); // Row 0
                    dgv.Rows.Add(); // Row 1
                    dgv.Rows.Add(); // Row 2
                    dgv.Rows.Add(); // Row 3
                    dgv.Rows.Add(); // Row 4
                    dgv.Rows.Add(); // Row 5
                    dgv.Rows.Add(); // Row 6
                    dgv.Rows.Add(); // Row 7
                    dgv.Rows.Add(); // Row 8

                    dgv[0, 0].Value = $"              {receipt.ReceiptId}";
                    dgv[0, 1].Value = $"     {receipt.OrderDateTime.ToString("g")}";
                    dgv[1, 1].Value = $"     {receipt.Ordertable.ToString()}";

                    string[] foodNames = receipt.FoodName.Split(',');
                    string[] foodPrices = receipt.Foodprice.Split(',');


                    int numberOfSpaces = 5;
                    string spacePrefix = new string(' ', numberOfSpaces);

                    for (int i = 0; i < Math.Min(6, foodNames.Length); i++)
                    {
                        dgv[0, i + 2].Value = spacePrefix + foodNames[i].Trim();

                        if (foodPrices.Length > i)

                            dgv[1, i + 2].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dgv[1, i + 2].Value = foodPrices[i].Trim() + $" {CurrencyService.Instance.GetCurrencySymbol()}";
                    }

                    dgv[0, 8].Value = $"     {receipt.PaymenttypeName}";
                    dgv[1, 8].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dgv[1, 8].Value = receipt.TotalPrice.ToString("N2") + $" {CurrencyService.Instance.GetCurrencySymbol()}";

                    panel.Controls.Add(dgv);
                    tableLayoutPanel1.Controls.Add(panel, rowIndex % 3, rowIndex / 3);
                  
                    rowIndex++;
                  
                    if (rowIndex % 3 == 0)
                    {
                        tableLayoutPanel1.RowCount++;
                    }
                    tableLayoutPanel1.Controls.Add(buttonPrevious, 0, 0);
                    tableLayoutPanel1.Controls.Add(buttonNext, 2, 2);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
            finally
            {

            }

            ApplyTheme();
        }

        private void Dgv_Resize(object sender, EventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
           
            if (dgv != null)
            {
                int totalHeight = (int)(dgv.ClientSize.Height * 0.98); 
                int numberOfRows = dgv.Rows.Count;
                int newRowHeight = totalHeight / numberOfRows;

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    row.Height = newRowHeight;
                }

                dgv.Columns[0].Width = (int)(dgv.Width * 0.80);
                dgv.Columns[1].Width = (int)(dgv.Width * 0.20);
            }
        }

        private void Dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (sender is DataGridView dgv)
            {
                dgv.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Century Gothic", 10, FontStyle.Bold);

                if (e.RowIndex == 0 && e.ColumnIndex == 0)
                {
                    e.CellStyle.Font = new System.Drawing.Font("Century Gothic", 15, FontStyle.Bold);
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }              
                else if (e.RowIndex == 1 && e.ColumnIndex == 0)
                {
                    e.CellStyle.ForeColor = System.Drawing.Color.DodgerBlue;
                    e.CellStyle.SelectionForeColor = System.Drawing.Color.DodgerBlue;
                    e.CellStyle.Font = new System.Drawing.Font("Century Gothic", 9, FontStyle.Regular);
                }
                else if (e.RowIndex == 1 && e.ColumnIndex == 1)
                {
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    e.CellStyle.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
                    e.CellStyle.SelectionForeColor = System.Drawing.SystemColors.ControlDarkDark;
                    e.CellStyle.Font = new System.Drawing.Font("Century Gothic", 11, FontStyle.Bold);
                }
                else if (e.RowIndex == 8 && e.ColumnIndex == 0)
                {
                    e.CellStyle.ForeColor = System.Drawing.Color.SaddleBrown;
                    e.CellStyle.SelectionForeColor = System.Drawing.Color.SaddleBrown;
                    e.CellStyle.Font = new System.Drawing.Font("Segoe UI", 9, FontStyle.Regular);
                }
                else if (e.RowIndex == 8 && e.ColumnIndex == 1)
                {
                    e.CellStyle.ForeColor = System.Drawing.Color.Green;
                    e.CellStyle.SelectionForeColor = System.Drawing.Color.Green;
                    e.CellStyle.Font = new System.Drawing.Font("Century Gothic", 12, FontStyle.Bold);

                }
            }
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (sender is DataGridView clickedDgv && clickedDgv.Tag != null)
            {
                if (int.TryParse(clickedDgv.Tag.ToString(), out int receiptId))
                {
                    Receiptdetails receiptDetailsForm = new Receiptdetails(this)
                    {
                        ReceiptId = receiptId // Set the ReceiptId property
                    };

                    using (Receiptdetails RC = new Receiptdetails(this))
                    {
                        FormHelper.ShowFormWithOverlay(this.FindForm(), receiptDetailsForm);
                    }
                }
            }
        }

        private void Dgv_MouseEnter(object sender, EventArgs e)
        {
            DataGridView dgv = sender as DataGridView;

            if (dgv != null)
            {
                dgv.BackgroundColor = colors.Color4;
                dgv.DefaultCellStyle.BackColor = colors.Color4;
                dgv.DefaultCellStyle.SelectionBackColor = colors.Color4;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = colors.Color4;
                dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = colors.Color4;
            }
        }

        private void Dgv_MouseLeave(object sender, EventArgs e)
        {
            DataGridView dgv = sender as DataGridView;

            if (dgv != null)
            {
                dgv.BackgroundColor = colors.Color3;
                dgv.DefaultCellStyle.BackColor = colors.Color3;
                dgv.DefaultCellStyle.SelectionBackColor = colors.Color3;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = colors.Color3;
                dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = colors.Color3;
            }
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadData();
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            int itemsPerPage = _currentPage == 1 ? ItemsPerPageFirstPage : ItemsPerPageOtherPages;
            if ((_currentPage * itemsPerPage) < _totalReceipts)
            {
                _currentPage++;
                LoadData();
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Alterreceipts_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
                CenterLabel();
            }
        }

        private void Alterreceipts_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
                CenterLabel();
            }
        }


        public void LocalizeControls()
        {
            label3.Text = LanguageManager.Instance.GetString("DR-lbl3");
            button1.Text = LanguageManager.Instance.GetString("Btn-close");
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
        }

        public void ApplyTheme()
        {
            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            label1.ForeColor = colors.Color2;
            label3.ForeColor = colors.Color2;
            tableLayoutPanel1.BackColor = colors.Color1;

            // Apply button colors
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                if (control is RJButton button)
                {
                    button.BackColor = colors.Color3;
                    button.ForeColor = colors.Color2;
                    button.FlatAppearance.MouseOverBackColor = colors.Color4;
                }

                if (control is RoundedPanel panel)
                {
                    panel.BackColor = colors.Color3;
                    panel.ForeColor = colors.Color2;

                    foreach (Control controls in panel.Controls)
                    {
                        if (controls is DataGridView dgv)
                        {
                            dgv.BackgroundColor = colors.Color3;
                            dgv.DefaultCellStyle.BackColor = colors.Color3;
                            dgv.DefaultCellStyle.ForeColor = colors.Color2;
                            dgv.DefaultCellStyle.SelectionBackColor = colors.Color3;
                            dgv.DefaultCellStyle.SelectionForeColor = colors.Color2;
                        }
                    }
                } 
            }          
        }
    }

    public class Receipt
    {
        public int ReceiptId { get; set; }
        public DateTime OrderDateTime { get; set; }
        public string FoodName { get; set; }
        public string Foodprice { get; set; }
        public string PaymenttypeName { get; set; }
        public double TotalPrice { get; set; }
        public string Ordertable { get; set; }
        public string BuyerID { get; set; }
    }
}