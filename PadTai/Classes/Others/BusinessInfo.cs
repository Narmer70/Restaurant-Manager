using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;


namespace PadTai.Classes.Others
{
    public class BusinessInfo
    {
        private CrudDatabase crudDatabase;
        private Label labelTime;
        private Label labelUser;
        private Timer timer;

        public BusinessInfo(Label timeLabel, Label userLabel)
        {
            // Initialize labels
            labelTime = timeLabel;
            labelUser = userLabel;

            crudDatabase = new CrudDatabase();

            LoadClientFromSettings();

            // Set up the timer
            timer = new Timer();
            timer.Interval = 1000; 
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateTimeLabel();
        }

        private void UpdateTimeLabel()
        {
            DateTime now = DateTime.Now;
            string formattedDateTime = $"{now.ToString("dd MMMM yyyy", LanguageManager.Instance.CurrentCulture)} " +  $"{now.ToString("HH:mm:ss", LanguageManager.Instance.CurrentCulture)}";
            labelTime.Text = formattedDateTime.ToUpper();
        }

        private void LoadClientFromSettings()
        {
            string clientIdString = Properties.Settings.Default.SelectedClientId;
            LoadClient(clientIdString);
        }

        private void LoadClient(string clientIdString)
        {
            if (int.TryParse(clientIdString, out int clientId))
            {
                User user = GetUserById(clientId);
                if (user != null)
                {
                    string businessName = GetBusinessName();
                    labelUser.Text = $"{businessName.ToUpper()}-{user.Name.ToUpper()}";
                }
                else
                {
                    MessageBox.Show("User  not found.");
                }
            }
            else
            {
                MessageBox.Show("Invalid User ID.");
            }
        }

        private User GetUserById(int clientId)
        {
            try
            {
                User user = null;
                // Use string interpolation to create the query
                string query = $"SELECT ClientID, ClientName FROM Clients WHERE ClientID = '{clientId}'";

                // Fetch data using the existing method
                DataTable userData = crudDatabase.FetchDataFromDatabase(query);

                if (userData.Rows.Count > 0)
                {
                    user = new User
                    {
                        Id = Convert.ToInt32(userData.Rows[0]["ClientID"]),
                        Name = userData.Rows[0]["ClientName"].ToString()
                    };
                }

                return user;
            }
            catch
            {
                return null;
            }
        }

        private string GetBusinessName()
        {
            string businessName = string.Empty;
           
            // Use string interpolation to create the query
            string query = "SELECT Businessname FROM Appaccesscode";

            // Fetch data using the existing method
            DataTable businessData = crudDatabase.FetchDataFromDatabase(query);

            if (businessData.Rows.Count > 0)
            {
                businessName = businessData.Rows[0]["Businessname"].ToString();
            }

            return businessName;
        }

        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
