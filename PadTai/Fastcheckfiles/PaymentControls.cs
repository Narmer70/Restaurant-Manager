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
using static PadTai.Fastcheckfiles.DishGroupControl;
using PadTai.Classes.Databaselink;

namespace PadTai.Fastcheckfiles
{
    public partial class PaymentControls : UserControl
    {
        private Stack<NavigationPay> _navigationStack = new Stack<NavigationPay>(); 
        private ButtonClickHandler _buttonClickHandler;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;
        private int? _currentGroupId;
        Fastcheck FCH;

        public PaymentControls(Fastcheck fCH, Stack<NavigationPay> navigationStack, int? groupId = null)
        {
            InitializeComponent();

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);
            crudDatabase = new CrudDatabase();
            _buttonClickHandler = new ButtonClickHandler(this);
            _navigationStack = navigationStack ?? new Stack<NavigationPay>();
            _currentGroupId = groupId; 
            LoadPaymentControls();
            this.FCH = fCH;

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

        private void LoadPaymentControls()
        {
            // Clear existing buttons and reset the row
            this.Controls.Clear();
            resizer = new ControlResizer(this.Size);

            // Create a TableLayoutPanel for better button structure
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


            // Add back button (visible if we are in a group)
            RJButton backButton = new RJButton
            {
                Text = "◀" , 
                Width = 145,
                Height = 75,
                Dock = DockStyle.Fill,
                Margin = new Padding(2),
                BackColor = Color.DodgerBlue,
               // Visible = (_navigationStack.Count > 0),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            resizer.RegisterControl(backButton);
            backButton.Click += BackButton_Click;
            tableLayoutPanel.Controls.Add(backButton, 0, 0);
            tableLayoutPanel.SetColumnSpan(backButton, 1);

            if (!_currentGroupId.HasValue)
            {
                // Load groups and null payment types
                List<object> items = new List<object>();
                List<PaymentType> nullPaymentTypes = GetNullPaymentTypes();
                List<Groups> groups = GetAllGroups();
                items.AddRange(nullPaymentTypes);
                items.AddRange(groups);

                // Create buttons for each item
                foreach (var item in items)
                {
                    if (item is Groups group)
                    {
                        RJButton groupButton = new RJButton
                        {
                            Text = group.GroupName,
                            Tag = group.GroupID,
                            Width = 145,
                            Height = 75,
                            Dock = DockStyle.Fill,
                            Margin = new Padding(2),
                            BackColor = Color.DodgerBlue,
                            TextAlign = ContentAlignment.MiddleCenter,
                            Font = new Font("Segoe UI", 10, FontStyle.Regular)
                        };
                        resizer.RegisterControl(groupButton);
                        groupButton.Click += GroupButton_Click;
                        tableLayoutPanel.Controls.Add(groupButton);
                    }
                    else if (item is PaymentType paymentType)
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
                    }
                    tableLayoutPanel.RowCount++;
                }
            }
            else if (_currentGroupId.HasValue && _currentGroupId.Value != 0)
            {
                if (_currentGroupId.HasValue) 
                {
                    List<PaymentType> paymentTypes = GetPaymentTypesForGroup(_currentGroupId.Value);
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
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            if (_navigationStack.Count > 0)
            {
                NavigationPay previousState = _navigationStack.Pop(); 
                _currentGroupId = previousState.GroupId;
                LoadControls(_currentGroupId); 
            }
            else 
            {
                DishGroupControl MCL = new DishGroupControl(FCH, null);
                FCH.AddUserControl(MCL);
            }
        }

        private void GroupButton_Click(object sender, EventArgs e)
        {
            if (sender is RJButton button && button.Tag is int groupId)
            {
                _navigationStack.Push(new NavigationPay { GroupId = _currentGroupId });
                _currentGroupId = groupId;
                LoadControls(_currentGroupId); 
            }
        }

        private void PaymentTypeButton_Click(object sender, EventArgs e)
        {
            if (sender is RJButton button && button.Tag is int paymentTypeId)
            {
                // Handle payment type button click here
                _buttonClickHandler.HandleButtonClickP(sender);

                using (PayConfirmForm PCF = new PayConfirmForm(FCH))
                {
                    PCF.PaymentTypeId = paymentTypeId;
                    FormHelper.ShowFormWithOverlay(this.FindForm(), PCF);
                }
            }
        }

        // Helper methods for database interaction
        private List<Groups> GetAllGroups()
        {
            string query = "SELECT PaymentgroupID, PaymentGroupName FROM PaymentGroups";

            return crudDatabase.FetchDataToList(query, reader => new Groups
            {
                GroupID = reader.GetInt32(0),
                GroupName = reader.GetString(1)
            });
        }

        private List<PaymentType> GetPaymentTypesForGroup(int groupId)
        {
            string query = $"SELECT PaymentypeID, PaymenttypeName FROM PaymentTypes WHERE PaymentgroupID = {groupId}";

            return crudDatabase.FetchDataToList(query, reader => new PaymentType
            {
                PaymentypeID = reader.GetInt32(0),
                PaymenttypeName = reader.GetString(1)
            });
        }

        private List<PaymentType> GetNullPaymentTypes()
        {
            string query = "SELECT PaymentypeID, PaymenttypeName FROM PaymentTypes WHERE PaymentgroupID IS NULL";

            return crudDatabase.FetchDataToList(query, reader => new PaymentType
            {
                PaymentypeID = reader.GetInt32(0),
                PaymenttypeName = reader.GetString(1)
            });
        }

        public void LoadControls(int? groupId)
        {
            PaymentControls paymentControls = new PaymentControls(FCH, _navigationStack, groupId);
            FCH.Panelreceptacle.Controls.Clear(); 
            paymentControls.Dock = DockStyle.Fill; 
            FCH.Panelreceptacle.Controls.Add(paymentControls);
            _currentGroupId = groupId;

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

    public class NavigationPay
    {
        public int? GroupId { get; set; }
    }

    // Model classes
    public class Groups
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; }
    }

    public class PaymentType
    {
        public int PaymentypeID { get; set; }
        public string PaymenttypeName { get; set; }
    }
}

