using System;
using System.Data;
using PadTai.Classes;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using PadTai.Classes.Others;
using PadTai.Classes.Databaselink;
using System.Net.NetworkInformation;


namespace PadTai.Sec_daryfolders.Others
{
    public partial class Messaging : UserControl
    {
        public event EventHandler UserControlClosing;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public Messaging()
        {
            InitializeComponent();
            InitializeControlResizer();
            crudDatabase = new CrudDatabase();
            CheckInternetAvailability();
            LocalizeControls();
            CenterLabel();
            ApplyTheme();
        }

        private void InitializeControlResizer()
        {
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(panel1);
            resizer.RegisterControl(panel2);
            resizer.RegisterControl(panel3);
            resizer.RegisterControl(label1);
            resizer.RegisterControl(label2);
            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(rjButton2);
            resizer.RegisterControl(pictureBox1);
            resizer.RegisterControl(pictureBox2);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (resizer != null)
            {
                resizer.ResizeControls(this);
                CenterLabel();
            }
        }


        private void CenterLabel()
        {
            panel1.Left = (this.ClientSize.Width - panel1.Width) / 2;
            panel2.Left = (this.ClientSize.Width - panel2.Width) / 2;
            label1.Left = (panel1.ClientSize.Width - label1.Width) / 2;
            label2.Left = (panel2.ClientSize.Width - label2.Width) / 2;
            label3.Left = (panel3.ClientSize.Width - label3.Width) / 2;
            rjButton1.Left = (panel1.ClientSize.Width - rjButton1.Width) / 2;
            rjButton2.Left = (panel2.ClientSize.Width - rjButton2.Width) / 2;        
            pictureBox1.Left = (panel1.ClientSize.Width - pictureBox1.Width) / 2;
            pictureBox2.Left = (panel2.ClientSize.Width - pictureBox2.Width) / 2;          
        }


        #region Check Internet Connection Availability
        private void NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            CheckInternetAvailability();
        }

        private void NetworkAddressChanged(object sender, EventArgs e)
        {
            CheckInternetAvailability();
        }

        private void CheckInternetAvailability()
        {
            NetworkChange.NetworkAddressChanged += NetworkAddressChanged;
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkAvailabilityChanged);
            bool isConnected = crudDatabase.IsInternetAvailable();
            UpdateUI(isConnected);
        }


        private void UpdateUI(bool isConnected)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateUI(isConnected)));
                return;
            }

            panel1.Visible = !isConnected; 
            panel2.Visible = isConnected /*&& hasNoSubscription*/;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            NetworkChange.NetworkAvailabilityChanged -= NetworkAvailabilityChanged;
            NetworkChange.NetworkAddressChanged -= NetworkAddressChanged; 
            base.OnHandleDestroyed(e);
        }
        #endregion

        #region Check Account and Subscription Availability
        private void rjButton1_Click(object sender, EventArgs e)
        {
            CheckInternetAvailability();
        }
        #endregion
        

        private void rjButton2_Click(object sender, EventArgs e)
        {
            panel3.Visible = !panel3.Visible;
        }        
        
        public void LocalizeControls()
        {
            rjButton1.Text = LanguageManager.Instance.GetString("Retry");
            label2.Text = LanguageManager.Instance.GetString("Noaccount");
            label1.Text = LanguageManager.Instance.GetString("Nointernet");
            label3.Text = LanguageManager.Instance.GetString("Comingsoon");
            rjButton2.Text = LanguageManager.Instance.GetString("Makeaccount");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            label1.ForeColor = this.ForeColor;
            label2.ForeColor = this.ForeColor;
            label3.ForeColor = this.ForeColor;
            panel3.GradientBottomColor = colors.Color3;
        }
    }
}
            

