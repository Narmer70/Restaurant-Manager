using System;
using System.Data;
using System.Linq;
using System.Text;
using PadTai.Classes;
using System.Drawing;
using System.Data.SQLite;
using System.Windows.Forms;
using System.ComponentModel;
using PadTai.Classes.Others;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;


namespace PadTai.Fastcheckfiles
{
    public partial class PayupdateControls : UserControl
    {
        private ButtonClickHandler _buttonClickHandler;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;
        Fastcheck FCH;


        public PayupdateControls(Fastcheck fCH)
        {
            InitializeComponent();
            this.FCH = fCH;

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);
            crudDatabase = new CrudDatabase();
            _buttonClickHandler = new ButtonClickHandler(this);
            
            LoadPaymentTypes();
            LocalizeControls();
            ApplyTheme();
        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (resizer != null)
            {
                resizer.ResizeControls(this);
            }
        }


        private void LoadPaymentTypes()
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

            // Re-add the back button
            RJButton backButton = new RJButton
            {
                Text = "◀",
                Width = 145,
                Height = 75,
                Dock = DockStyle.Fill,
                Margin = new Padding(2),
                BackColor = Color.DodgerBlue,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            resizer.RegisterControl(backButton);
            backButton.Click += BackButton_Click;
            tableLayoutPanel.Controls.Add(backButton, 0, 0);
            tableLayoutPanel.SetColumnSpan(backButton, 1);

            // Load all payment types
            List<UpdatePaytypes> paymentTypes = GetAllPaymentTypes();

            // Load and add PaymentType Buttons
            foreach (var paymentType in paymentTypes)
            {
                RJButton paymentTypeButton = new RJButton
                {
                    Text = paymentType.PaymenttypeName,
                    Tag = paymentType.PaymentypeID,
                    Width = 145,
                    Height = 75,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(2),
                    BackColor = Color.DodgerBlue,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular)
                };
                resizer.RegisterControl(paymentTypeButton);
                paymentTypeButton.Click += PaymentTypeButton_Click;
                tableLayoutPanel.Controls.Add(paymentTypeButton);
                tableLayoutPanel.RowCount++;
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            DishGroupControl MCL = new DishGroupControl(FCH, null);
            FCH.AddUserControl(MCL);
        }

        private void PaymentTypeButton_Click(object sender, EventArgs e)
        {
            if (sender is RJButton button && button.Tag is int paymentTypeId)
            {
                _buttonClickHandler.HandleButtonClickP(sender);
            }
        }

        private List<UpdatePaytypes> GetAllPaymentTypes()
        {
            string query = "SELECT PaymentypeID, PaymenttypeName FROM PaymentTypes";

            return crudDatabase.FetchDataToList(query, reader => new UpdatePaytypes
            {
                PaymentypeID = reader.GetInt32(0),
                PaymenttypeName = reader.GetString(1)
            });
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

    // Model class for PaymentType
    public class UpdatePaytypes
    {
        public int PaymentypeID { get; set; }
        public string PaymenttypeName { get; set; }
    }
}

