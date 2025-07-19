using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using PadTai.Classes;
using System.Data.SQLite;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading.Tasks;
using PadTai.Classes.Others;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;

namespace PadTai.Fastcheckfiles
{
    public partial class DeliveryControls : UserControl
    {
        private ButtonClickHandler _buttonClickHandler;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;
        Fastcheck FCH;

        public DeliveryControls(Fastcheck fCH)
        {
            InitializeComponent();

            _buttonClickHandler = new ButtonClickHandler(this);
            crudDatabase = new CrudDatabase();
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            ApplyTheme();
            this.FCH = fCH;
            LoadDeliveries();
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

        private void LoadDeliveries()
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
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            resizer.RegisterControl(backButton);
            backButton.Click += BackButton_Click;
            tableLayoutPanel.Controls.Add(backButton, 0, 0);
            tableLayoutPanel.SetColumnSpan(backButton, 1);


            List<Delivery> deliveries = GetAllDeliveries();
            foreach (var delivery in deliveries)
            {
                RJButton deliveryButton = new RJButton
                {
                    Text = delivery.Thedelivery,
                    Tag = delivery.DeliveryID,
                    Width = 145,
                    Height = 75,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(2),
                    BackColor = Color.DodgerBlue,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular)
                };
                resizer.RegisterControl(deliveryButton);
                deliveryButton.Click += DeliveryButton_Click;
                tableLayoutPanel.Controls.Add(deliveryButton);
                tableLayoutPanel.RowCount++;
            }

        }


        private void BackButton_Click(object sender, EventArgs e)
        {
            DishGroupControl MCL = new DishGroupControl(FCH, null);
            FCH.AddUserControl(MCL);
        }

        private void DeliveryButton_Click(object sender, EventArgs e)
        {
            if (sender is RJButton button && button.Tag is int deliveryId)
            {
                _buttonClickHandler.HandleButtonClickWTE(sender);            
            }
        }

        private List<Delivery> GetAllDeliveries()
        {
            string query = "SELECT DeliveryID, Thedelivery FROM Delivery";

            // Use FetchDataToList to retrieve the list of deliveries
            return crudDatabase.FetchDataToList(query, reader => new Delivery
            {
                DeliveryID = reader.GetInt32(0),
                Thedelivery = reader.GetString(1)
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

    // Model classes
    public class Delivery
    {
        public int DeliveryID { get; set; }
        public string Thedelivery { get; set; }
    }
}

