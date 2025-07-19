using PadTai.Classes;
using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection.Emit;
using System.Windows.Forms;
using System.Configuration;
using PadTai.Sec_daryfolders.Others;
using PadTai.Sec_daryfolders.Quitfolder;

namespace PadTai.Sec_daryfolders
{

    public partial class Reportviewer : Form
    {
        private FontResizer fontResizer;
        private ControlResizer resizer;
        private DraggableForm draggableForm;
        string connectionString = DatabaseConnection.GetConnection().ConnectionString;

        public Reportviewer()
        {
            InitializeComponent();
            timer1.Interval = 1000; // Set timer interval to 1 second
            timer1.Tick += Timer1_Tick; // Subscribe to the Tick event
            timer1.Start(); // Start the timer

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);
            draggableForm = new DraggableForm();
            draggableForm.EnableDragging(this);
            LoadClient();

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(label1);
            resizer.RegisterControl(label2);
            resizer.RegisterControl(label3);
            resizer.RegisterControl(button3);
            resizer.RegisterControl(button2);
            resizer.RegisterControl(button4);
            resizer.RegisterControl(button5);
            resizer.RegisterControl(button8);
            resizer.RegisterControl(panel1);
            resizer.RegisterControl(panel2);

            this.Resize += Reportviewer_Resize;
            this.Load += Reportviewer_Load; 

        }
        private void Timer1_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            string formattedDateTime = $"{now.ToString("dd MMMM yyyyГ", new CultureInfo("ru-RU"))} " +
                                        $"{now.ToString("HH:mm:ss", new CultureInfo("ru-RU"))}";
            formattedDateTime = formattedDateTime.ToUpper();
            label2.Text = formattedDateTime;
        }

        private void LoadClient()
        {
            // Retrieve the UserId from application settings
            string clientIdString = Properties.Settings.Default.SelectedClientId;

            // Check if the setting is not empty
            if (int.TryParse(clientIdString, out int clientId))
            {
                User user = GetUserById(clientId); // Fetch user by the ID from settings

                if (user != null)
                {
                    // Set the button and label text
                   label1.Text = $" КАССА: {user.Name}";
                }
                else
                {
                    MessageBox.Show("User not found.");
                }
            }
            else
            {
                MessageBox.Show("Invalid User ID in settings.");
            }
        }

        // Your existing method to fetch user by ID
        public User GetUserById(int clientId)
        {
            User user = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT ClientID, ClientName FROM Clients WHERE ClientID = @ClientID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClientID", clientId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    user = new User
                    {
                        Id = (int)reader["ClientId"],
                        Name = reader["ClientName"].ToString()
                    };
                }
            }

            return user;
        }

        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private void Reportviewer_Load(object sender, EventArgs e)
        {
            Paytypecatreport PTR = new Paytypecatreport(this);
            AddUserControl(PTR);
            PTR.LoadPaymentTypeSalesReport();

            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Paytypecatreport PTR = new Paytypecatreport(this);
            AddUserControl(PTR);
            PTR.LoadPaymentTypeSalesReport();
        }

        public void AddUserControl(UserControl UserControl)
        {
            UserControl.Dock = DockStyle.Fill;
            panel1.Controls.Clear();
            panel1.Controls.Add(UserControl);
            UserControl.BringToFront();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Foodcatereport FCR = new Foodcatereport(this);
            AddUserControl(FCR);
            FCR.LoadFoodSalesReport();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Discountcatreport DCR = new Discountcatreport(this);
            AddUserControl(DCR);
            DCR.LoadDiscountSummary();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Deltypecateport TOD = new Deltypecateport(this);
            AddUserControl(TOD);
            TOD.LoadDeliverySummary();
        }

        private void Reportviewer_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }

        }
    }
}
