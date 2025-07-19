using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using PadTai.Classes;
using System.Data.SQLite;
using System.Windows.Forms;
using PadTai.Classes.Others;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;


namespace PadTai.Fastcheckfiles
{
    public partial class DiscountControls : UserControl
    {
        private ButtonClickHandler _buttonClickHandler;
        private FoodItemManager _foodItemManager;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;
        Fastcheck FCH;

        public DiscountControls(Fastcheck fCH)
        {
            InitializeComponent();

            _buttonClickHandler = new ButtonClickHandler(this);
            _foodItemManager = new FoodItemManager();
            crudDatabase = new CrudDatabase();
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            this.FCH = fCH;

            ApplyTheme();
            LoadDiscounts();
            LocalizeControls();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
          
            if (resizer != null)
            {
                resizer.ResizeControls(this);
            }
        }

        private void LoadDiscounts()
        {
            this.Controls.Clear();
            resizer = new ControlResizer(this.Size);

            TableLayoutPanel tableLayoutPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = false,
                AutoSize = false,
                ColumnCount = 3,
                RowCount = 7 
            };

            float[] rowPercentages = new float[7] { 14F, 14F, 14F, 14F, 14F, 14F, 14F };

            foreach (float percentage in rowPercentages)
            {
                tableLayoutPanel.RowStyles.Add(new ColumnStyle(SizeType.Percent, percentage));
            }

            float[] columnPercentages = new float[3] { 33.3F, 33.3F, 33.3F };

            foreach (float percentage in columnPercentages)
            {
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, percentage));
            }
            resizer.RegisterControl(tableLayoutPanel);
            this.Controls.Add(tableLayoutPanel);


            // Add back button
            RJButton backButton = new RJButton
            {
                Text = "◀",
                Width = 145,
                Height = 75,
                Dock = DockStyle.Fill,
                Margin = new Padding(2),
                BackColor = Color.DodgerBlue,
                Visible = true, // Always visible in this control
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            resizer.RegisterControl(backButton);
            backButton.Click += BackButton_Click;
            tableLayoutPanel.Controls.Add(backButton, 0, 0);
            tableLayoutPanel.SetColumnSpan(backButton, 1);


            // Load and add Discount Buttons
            List<Discount> discounts = GetAllDiscounts();
            foreach (var discount in discounts)
            {
                RJButton discountButton = new RJButton
                {
                    Name = discount.Thediscount,
                    Text = "-" + discount.Thediscount +"%",
                    Tag = discount.DiscountID,
                    Width = 145,
                    Height = 75,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(2),
                    BackColor = Color.DodgerBlue,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular)
                };
                resizer.RegisterControl(discountButton);
                discountButton.Click += DiscountButton_Click;
                tableLayoutPanel.Controls.Add(discountButton);
                tableLayoutPanel.RowCount++;

            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            DishGroupControl MCL = new DishGroupControl(FCH, null);
            FCH.AddUserControl(MCL);
        }

        private void DiscountButton_Click(object sender, EventArgs e)
        {
            if (sender is RJButton button)
            {
                if (button.Tag is int discountId && button.Name != "0")
                {
                    _buttonClickHandler.HandleDiscount(sender);
                    _foodItemManager.UpdateTotalPrice(FCH.dataGridView1, FCH.dataGridView2);

                    if (int.TryParse(button.Name, out int discountValue))
                    {
                        ApplyDiscount(discountValue);
                    }
                    else
                    {
                        MessageBox.Show("Invalid discount value. Please enter a valid number.");
                    }
                }
                else if (button.Name == "0")
                {
                    _buttonClickHandler.HandleDiscount(sender);
                    _foodItemManager.UpdateTotalPrice(FCH.dataGridView1, FCH.dataGridView2);
                   
                    if (int.TryParse(button.Name, out int discountValue))
                    {
                        ApplyDiscount(discountValue);
                        RemoveRowsFromDataGridView();
                    }
                    else
                    {
                        MessageBox.Show("Invalid discount value. Please enter a valid number.");
                    }
                }
            }
        }


        private List<Discount> GetAllDiscounts()
        {
            string query = "SELECT DiscountID, Thediscount FROM Discounts";

            // Use FetchDataToList to retrieve the list of discounts
            return crudDatabase.FetchDataToList(query, reader => new Discount
            {
                DiscountID = reader.GetInt32(0),
                Thediscount = reader.GetString(1)
            });
        }

        public void ApplyDiscount(decimal discountPercentage)
        {
            var parentForm = this.FindForm() as Fastcheck;
            if (parentForm != null)
            {
                parentForm.EnsureDataGridViewRows();

                decimal totalPrice = Convert.ToDecimal(parentForm.dataGridView2.Rows[0].Cells[1].Value);

                decimal discountAmount = (discountPercentage / 100m) * totalPrice;

                decimal newPrice = totalPrice - discountAmount;
                discountAmount = Math.Round(discountAmount, 2);
                newPrice = Math.Round(newPrice, 2);

                parentForm.dataGridView2.Rows[4].Cells[1].Value = discountAmount;
                parentForm.dataGridView2.Rows[4].Cells[0].Value = LanguageManager.Instance.GetString("Sumdiscount"); 

                parentForm.dataGridView2.Rows[5].Cells[1].Value = newPrice;
                parentForm.dataGridView2.Rows[5].Cells[0].Value = LanguageManager.Instance.GetString("Newprice");

                parentForm.UpdateLabel(newPrice, discountAmount);
            }
        }        
        
        public void RemoveRowsFromDataGridView()
        {
            var parentForm = this.FindForm() as Fastcheck;
            if (parentForm != null)
            {
                if (parentForm.dataGridView2.Rows.Count > 4)
                {
                    parentForm.dataGridView2.Rows.RemoveAt(3);
                    parentForm.dataGridView2.Rows.RemoveAt(4);
                    parentForm.dataGridView2.Rows.RemoveAt(parentForm.dataGridView2.Rows.Count - 1);
                }
                else
                {
                    MessageBox.Show("Not enough rows to remove.");
                }
            }
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
        }
    }

    // Model class for Discount
    public class Discount
    {
        public int DiscountID { get; set; }
        public string Thediscount { get; set; }
    }
}

