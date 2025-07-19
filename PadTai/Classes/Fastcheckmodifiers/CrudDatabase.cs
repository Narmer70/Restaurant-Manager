using System;
using System.IO;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Net.NetworkInformation;



namespace PadTai.Classes.Databaselink
{
    public class CrudDatabase
    {
        // Use the connection string from DatabaseConnection
        private string sqliteConnectionString = DatabaseConnection.GetSQLiteConnectionString();
  
        // Lists to hold data for the application
        public List<Group> Groups { get; private set; } = new List<Group>();
        public List<Client> Clients { get; private set; } = new List<Client>();
        public List<Subgroup> Subgroups { get; private set; } = new List<Subgroup>();
        public List<Subsubgroup> Subsubgroups { get; private set; } = new List<Subsubgroup>();
        public List<FoodItemType> FoodItemTypes { get; private set; } = new List<FoodItemType>();
        public List<PaymentGroups> Paymentgroups { get; private set; } = new List<PaymentGroups>();


        // General method to fetch data
        public DataTable FetchDataFromDatabase(string query, Dictionary<string, object> parameters = null)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(sqliteConnectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        // Add parameters to the command if provided
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                command.Parameters.AddWithValue(param.Key, param.Value);
                            }
                        }

                        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (SQLiteException sqlEx)
            {
                // Handle SQLite specific exceptions
                MessageBox.Show($"SQLite Error: {sqlEx.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                // Handle general exceptions
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dataTable;
        }



        // General method to execute an insert, update, or delete command
        public bool ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(sqliteConnectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        // Add parameters to the command if provided
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                command.Parameters.AddWithValue(param.Key, param.Value);
                            }
                        }

                        int affectedRows = command.ExecuteNonQuery();
                        return affectedRows > 0; // Return true if at least one row was affected
                    }
                }
            }
            catch (SQLiteException sqlEx)
            {
                MessageBox.Show($"SQLite Error: {sqlEx.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        // General method to fetch data and return a list of a specified type
        public List<T> FetchDataToList<T>(string query, Func<IDataRecord, T> mapFunction, Dictionary<string, object> parameters = null)
        {
            List<T> resultList = new List<T>();

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(sqliteConnectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        // Add parameters to the command if provided
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                command.Parameters.AddWithValue(param.Key, param.Value);
                            }
                        }

                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultList.Add(mapFunction(reader));
                            }
                        }
                    }
                }
            }
            catch (SQLiteException sqlEx)
            {
                MessageBox.Show($"SQLite Error: {sqlEx.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return resultList;
        }


        public bool CheckIfElementExists(string tableName, string columnName, string valueToCheck)
        {
            // Construct the SQL query to check for existence
            string query = $"SELECT 1 FROM {tableName} WHERE {columnName} = @ValueToCheck";

            // Create parameters dictionary
            var parameters = new Dictionary<string, object>
            {
                { "@ValueToCheck", valueToCheck }
            };

            try
            {
                // Execute the query and fetch the result
                DataTable result = FetchDataFromDatabase(query, parameters);

                // Check if any rows were returned
                return result.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool IsSpecificImage(Image imgToCheck, Image specificImage)
        {
            using (var ms1 = new MemoryStream())
            using (var ms2 = new MemoryStream())
            {
                specificImage.Save(ms1, System.Drawing.Imaging.ImageFormat.Png);
                imgToCheck.Save(ms2, System.Drawing.Imaging.ImageFormat.Png);

                byte[] specificImageBytes = ms1.ToArray();
                byte[] currentImageBytes = ms2.ToArray();

                return specificImageBytes.SequenceEqual(currentImageBytes);
            }
        }

        public bool IsInternetAvailable()
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

        // Method to load Groups
        public void LoadGroups()
        {
            string groupQuery = "SELECT GroupID, GroupName FROM Groups";
            Groups = FetchDataToList(groupQuery, reader => new Group
            {
                GroupID = reader.GetInt32(0),
                GroupName = reader.GetString(1)
            });
        }

        // Method to load Food Item Types
        public void LoadFoodItemTypes()
        {
            string foodItemTypeQuery = "SELECT FooditemtypeID, FooditemtypeName FROM FoodItemsTypes";
            FoodItemTypes = FetchDataToList(foodItemTypeQuery, reader => new FoodItemType
            {
                FooditemtypeID = reader.GetInt32(0),
                FooditemtypeName = reader.GetString(1)
            });
        }

        // Method to load Subgroups
        public void LoadSubgroups()
        {
            string subgroupQuery = "SELECT SubgroupID, SubgroupName FROM Subgroups";
            Subgroups = FetchDataToList(subgroupQuery, reader => new Subgroup
            {
                SubgroupID = reader.GetInt32(0),
                SubgroupName = reader.GetString(1)
            });
        }

        // Method to load Subsubgroups
        public void LoadSubsubgroups()
        {
            string subsubgroupQuery = "SELECT SubsubgroupID, SubsubgroupName FROM Subsubgroups";
            Subsubgroups = FetchDataToList(subsubgroupQuery, reader => new Subsubgroup
            {
                SubsubgroupID = reader.GetInt32(0),
                SubsubgroupName = reader.GetString(1)
            });
        }

        public void LoadPaymentGroups()
        {
            string subgroupQuery = "SELECT PaymentgroupID, PaymentGroupName FROM PaymentGroups";
            Paymentgroups = FetchDataToList(subgroupQuery, reader => new PaymentGroups
            {
                PaymentgroupID = reader.GetInt32(0),
                PaymentGroupName = reader.GetString(1)
            });
        }

        public Dictionary<int, string> LoadClientIDs()
        {
            var clients = new Dictionary<int, string>();
            string clientQuery = "SELECT ClientID, ClientName FROM Clients";
            Clients = FetchDataToList(clientQuery, reader => new Client
            {
                ClientID = reader.GetInt32(0),
                ClientName = reader.GetString(1)
            });
            
            foreach (var client in Clients)
            {
                clients[client.ClientID] = client.ClientName;
            }
            return clients;
        }

        public class Group
        {
            public int GroupID { get; set; }
            public string GroupName { get; set; }
        }

        public class FoodItemType
        {
            public int FooditemtypeID { get; set; }
            public string FooditemtypeName { get; set; }
        }

        public class Subgroup
        {
            public int SubgroupID { get; set; }
            public string SubgroupName { get; set; }
        }

        public class Subsubgroup
        {
            public int SubsubgroupID { get; set; }
            public string SubsubgroupName { get; set; }
        }

        public class PaymentGroups
        {
            public int PaymentgroupID { get; set; }
            public string PaymentGroupName { get; set; }
        }

        public class Client
        {
            public int ClientID { get; set; }
            public string ClientName { get; set; }

            public override string ToString()
            {
                return ClientName;
            }
        }
    }
}
