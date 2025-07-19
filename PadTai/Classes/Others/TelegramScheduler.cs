using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Timers;
using System.Windows;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using System.Net.NetworkInformation;


namespace PadTai.Classes.Others
{
    public class TelegramScheduler
    {
        private readonly string _botToken = Properties.Settings.Default.telegramToken; 
        private readonly string _chatId = Properties.Settings.Default.chatID; 
        private static readonly HttpClient client = new HttpClient();
        private readonly CrudDatabase crudDatabase;
        private static Timer timer;

        public TelegramScheduler()
        {
            crudDatabase = new CrudDatabase();
            ScheduleRun();
        }

        public void ScheduleRun()
        {
            DateTime now = DateTime.Now; 
            DateTime nextRun = now.Date.AddHours(Properties.Settings.Default.TGtime); 

            if (now > nextRun)
            {
                nextRun = nextRun.AddDays(1); 
            }

            double interval = (nextRun - now).TotalMilliseconds;

            // Set up the timer
            timer = new Timer(interval);
            timer.Elapsed += (sender, e) =>
            {
                timer.Stop(); // Stop the timer to prevent multiple triggers
                SendWorkSchedule();
            };
            timer.Start();

            //MessageBox.Show("Scheduler started. Next run at: " + nextRun);
        }

        public async void SendWorkSchedule()
        {
            var message = GetWorkScheduleMessage();

            if (!string.IsNullOrEmpty(message) && IsInternetAvailable())
            {
               await SendMessage(message);
            }
            else
            {
               // MessageBox.Show("No internet connection or no message to send.");
            }
            ScheduleRun();
        }

        public string GetWorkScheduleMessage()
        {
            // Get tomorrow's date
            DateTime tomorrow = DateTime.Now.AddDays(1);
            string tomorrowDateString = tomorrow.ToString("dddd, MMMM dd, yyyy", LanguageManager.Instance.CurrentCulture);
            string message = $"Work Schedule for Tomorrow \n ({tomorrowDateString}):\n\n";

            int monthDay = tomorrow.Day;   

            // Retrieve the ClientID from settings
            int clientId;
            if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                return "Error getting Schedule \n";
            }

            // Update the SQL query to filter by ClientID
            string query = @" SELECT e.Name, e.PersonalType, t.WorkHours FROM TimetableMap t 
                                    INNER JOIN Employees e ON t.EmployeeId = e.ID WHERE t.Monthday = @Monthday AND e.ClientID = @ClientID";

            var parameters = new Dictionary<string, object>
            {
                { "@Monthday", monthDay },
                { "@ClientID", clientId } 
            };

            DataTable dataTable = crudDatabase.FetchDataFromDatabase(query, parameters);

            var groupedData = new Dictionary<string, List<string>>();

            foreach (DataRow row in dataTable.Rows)
            {
                string name = row["Name"].ToString();
                string personalType = row["PersonalType"].ToString();
                double workHours = Convert.ToDouble(row["WorkHours"]);

                if (!groupedData.ContainsKey(personalType))
                {
                    groupedData[personalType] = new List<string>();
                }
                groupedData[personalType].Add($"{name}: {workHours} {LanguageManager.Instance.GetString("Hour")}");
            }

            foreach (var group in groupedData)
            {
                message += $"{group.Key}:\n" + string.Join("\n", group.Value) + "\n\n";
            }

            return message;
        }

        private  async Task SendMessage(string message)
        {
            var url = $"https://api.telegram.org/bot{_botToken}/sendMessage?chat_id={_chatId}&text={Uri.EscapeDataString(message)}";

            var response = await client.GetAsync(url);
          
            if (response.IsSuccessStatusCode)
            {
               // MessageBox.Show("Message sent successfully!");
            }
            else
            {
               // MessageBox.Show("Error sending message: " + response.StatusCode);
            }
        }

        private bool IsInternetAvailable()
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = ping.Send("8.8.8.8", 1000); 
                    return reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false; 
            }
        }
    }
}
