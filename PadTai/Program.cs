using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Data.SQLite;
using System.Net.Sockets;
using System.Windows.Forms;
using PadTai.Classes.Others;
using PadTai.Sec_daryfolders;
using System.Threading.Tasks;
using System.Collections.Generic;
using PadTai.Sec_daryfolders.Quitfolder;
using PadTai.Sec_daryfolders.DB_Appinitialize;
using PadTai.Sec_daryfolders.Initialize_DB_APP;
using PadTai.Sec_daryfolders.App_DBInitializer;
using PadTai.Sec_daryfolders.Allreports.Reportgraphs;


namespace PadTai
{
    internal static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>

        [STAThread]
        static void Main(string[] args) 
        {           
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ThemeManager.LoadTheme();
            var scheduler = new TelegramScheduler();
            Application.Run(new MainPage());
        }
    }
}
