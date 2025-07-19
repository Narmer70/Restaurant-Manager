using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;


namespace PadTai.Classes.Others
{
    public class RestaurantDetails
    {
        private CrudDatabase crudDatabase;

        public RestaurantDetails()
        {
            crudDatabase = new CrudDatabase();
        }

        public (string restaurantName, byte[] restaurantLogo, string clientName, string clientAddress) GetDetails()
        {
            string clientId = Properties.Settings.Default.SelectedClientId;
            string restaurantName = null;
            byte[] restaurantLogo = null;
            string clientName = null;
            string clientAddress = null;

            // Fetch Businessname and Brandlogo from Appaccesscode
            string appAccessQuery = "SELECT Businessname, Brandlogo FROM Appaccesscode;";
            DataTable appAccessData = crudDatabase.FetchDataFromDatabase(appAccessQuery);

            if (appAccessData.Rows.Count > 0)
            {
                restaurantName = appAccessData.Rows[0]["Businessname"].ToString();
                restaurantLogo = appAccessData.Rows[0]["Brandlogo"] as byte[];
            }


            // Fetch ClientName and Clientadress from Clients where ClientID = @ClientID
            string clientQuery = $"SELECT ClientName, Clientadress FROM Clients WHERE ClientID = '{clientId}';";
            DataTable clientData = crudDatabase.FetchDataFromDatabase(clientQuery);

            if (clientData.Rows.Count > 0)
            {
                clientName = clientData.Rows[0]["ClientName"].ToString();
                clientAddress = clientData.Rows[0]["Clientadress"].ToString();
            }

            return (restaurantName, restaurantLogo, clientName, clientAddress);
        }
    }
}
