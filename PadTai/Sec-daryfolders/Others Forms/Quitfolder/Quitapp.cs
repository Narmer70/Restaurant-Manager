using System;
using System.Data;
using PadTai.Classes;
using System.Drawing;
using System.Data.SQLite;
using System.Windows.Forms;
using PadTai.Classes.Others;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;


namespace PadTai.Sec_daryfolders.Quitfolder
{
    public partial class Quitapp : Form
    {
        private CrudDatabase crudDatabase;
        private FormResizer formResizer;
        private ControlResizer resizer;

        public Quitapp()
        {
            InitializeComponent();

            crudDatabase = new CrudDatabase();
            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(label1);
            resizer.RegisterControl(button1);
            resizer.RegisterControl(button2);

            LocalizeControls();
            ApplyTheme();
        }


        private void Quitapp_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                CenterLabel();
            }
        }

        private void Quitapp_Load(object sender, EventArgs e)
        {
            formResizer = new FormResizer(this);
            formResizer.Resize(this);
            CenterLabel();
        }


        public void ResetreceiptId()
        {
            int clientId;

            // Try to parse the Client ID from settings
            if (int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                // Step 1: Retrieve the rows to be archived
                string selectQuery = "SELECT * FROM Receipts WHERE ClientID = @ClientID";
              
                var parameters = new Dictionary<string, object> 
                { 
                    { "@ClientID", clientId } 
                };

                DataTable receiptsToArchive = crudDatabase.FetchDataFromDatabase(selectQuery, parameters);

                // Step 2: Insert the rows into the ReceiptsArchive table
                if (receiptsToArchive.Rows.Count > 0)
                {
                    foreach (DataRow row in receiptsToArchive.Rows)
                    {
                        string insertQuery = "INSERT INTO ReceiptsArchive (ReceiptId, FoodName, Foodprice, OrderDateTime, PaymenttypeName, PlacetoEatName, Thediscount, TotalPrice, FooditemtypeID, FoodID, ClientID) " +
                                             "VALUES (@ReceiptId, @FoodName, @Foodprice, @OrderDateTime, @PaymenttypeName, @PlacetoEatName, @Thediscount, @TotalPrice, @FooditemtypeID, @FoodID, @ClientID)";

                        var insertParameters = new Dictionary<string, object>
                        {
                             { "@ReceiptId", row["ReceiptId"] },
                             { "@FoodName", row["FoodName"] },
                             { "@Foodprice", row["Foodprice"] },
                             { "@OrderDateTime", row["OrderDateTime"] },
                             { "@PaymenttypeName", row["PaymenttypeName"] },
                             { "@PlacetoEatName", row["PlacetoEatName"] },
                             { "@Thediscount", row["Thediscount"] },
                             { "@TotalPrice", row["TotalPrice"] },
                             { "@FooditemtypeID", row["FooditemtypeID"] },
                             { "@FoodID", row["FoodID"] },
                             { "@ClientID", row["ClientID"] }
                       };

                        crudDatabase.ExecuteNonQuery(insertQuery, insertParameters);
                    }

                    // Step 3: Delete the rows from the original Receipts table
                    string deleteReceiptsQuery = "DELETE FROM Receipts WHERE ClientID = @ClientID";
                    crudDatabase.ExecuteNonQuery(deleteReceiptsQuery, parameters);

                    // Step 4: Update the row from Receiptcount
                    string deleteCountsQuery = "UPDATE Receiptcount SET LastReceiptId = 0 WHERE ClientID = @ClientID";
                    crudDatabase.ExecuteNonQuery(deleteCountsQuery, parameters);

                    // Delete reservations
                    string deleteReservationsQuery = "DELETE FROM Reservetable WHERE ClientID = @ClientID";
                    crudDatabase.ExecuteNonQuery(deleteReservationsQuery, parameters);

                    // Update tablet counts
                    string deleteTabletCountsQuery = "UPDATE Tabletcount SET LastReserveId = 0 WHERE ClientID = @ClientID";
                    crudDatabase.ExecuteNonQuery(deleteTabletCountsQuery, parameters);

                    // Update time of stopping
                    string updateTimeOfStop = @"UPDATE Employeelogin SET TimeOfStoping = @NewTimeOfStopping WHERE ID = (SELECT ID FROM Employeelogin ORDER BY ID DESC LIMIT 1);";
                  
                    var timeParameters = new Dictionary<string, object>
                    {
                        { "@NewTimeOfStopping", DateTime.Now.TimeOfDay }
                    };
                   
                    crudDatabase.ExecuteNonQuery(updateTimeOfStop, timeParameters);

                    // Update session state
                    Properties.Settings.Default.IsSessionOpened = false;
                    Properties.Settings.Default.Save();

                    this.Hide();

                    using (Whenclosed WC = new Whenclosed())
                    {
                        FormHelper.ShowFormWithOverlay(this.FindForm(), WC);
                    }

                    // MessageBox.Show("Receipts archived and deleted successfully, along with the Receiptcount.");
                }
                else
                {
                    MessageBox.Show("No sift data for this location. Can't close an empty shift.");
                }
            }
            else
            {
                // MessageBox.Show("Invalid ClientID.");
            }
        }

        private void CenterLabel()
        {
            label1.Left = (this.ClientSize.Width - label1.Width) / 2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ResetreceiptId();   
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void LocalizeControls()
        {
            button1.Text = LanguageManager.Instance.GetString("Yesbutton");
            button2.Text = LanguageManager.Instance.GetString("Nobutton");
            label1.Text = LanguageManager.Instance.GetString("Closesession");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;
            label1.ForeColor = colors.Color2;
        }
    }
}
