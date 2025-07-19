using System;
using System.Linq;
using System.Data;
using PadTai.Classes;
using System.Drawing;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Data.SqlClient;
using PadTai.Classes.Others;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using PadTai.Classes.Controlsdesign;
using PadTai.Sec_daryfolders.Tablereservation;


namespace PadTai.Sec_daryfolders.Tablereserve
{
    public partial class Modifyreservation : UserControl
    {
        private Reservationform reservationform;
        private CrudDatabase crudDatabase;  
        private FontResizer fontResizer;
        private ControlResizer resizer;
        public string currentState;

        public Modifyreservation(Reservationform reserveform)
        {
            InitializeComponent();        
            InitializeControlResizer();
            this.reservationform = reserveform;
            LocalizeControls();
            loadClientID();
            ApplyTheme();
        }


        private void InitializeControlResizer()
        {
            currentState = "Add";
            button1.Enabled = false;
            checkBox1.Visible = false;
            rjButton2.Visible = false;
            label6.Visible = false;
            roundedTextbox1.Visible = false;

            crudDatabase = new CrudDatabase();
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);
            resizer.RegisterControl(label1);
            resizer.RegisterControl(label2);
            resizer.RegisterControl(label3);
            resizer.RegisterControl(label4);
            resizer.RegisterControl(label5);
            resizer.RegisterControl(label6);
            resizer.RegisterControl(label7);
            resizer.RegisterControl(label8);
            resizer.RegisterControl(label9);
            resizer.RegisterControl(label10);
            resizer.RegisterControl(button1);
            resizer.RegisterControl(textBox2);
            resizer.RegisterControl(textBox5);
            resizer.RegisterControl(textBox7);
            resizer.RegisterControl(textBox1);            
            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(rjButton2);
            resizer.RegisterControl(checkBox1);
            resizer.RegisterControl(customBox1);
            resizer.RegisterControl(customBox2);
            resizer.RegisterControl(roundedPanel1);
            resizer.RegisterControl(roundedPanel2);
            resizer.RegisterControl(roundedTextbox1);
            resizer.RegisterControl(customDateTimePicker1);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
   
            if (resizer != null)
            {
                resizer.ResizeControls(this);
            }
        }

        private void loadClientID()
        {
            customBox1.Items.Clear();
            customBox2.Items.Clear();

            if (Properties.Settings.Default.isHeadquarterMode)
            {
                crudDatabase.LoadClientIDs();
                customBox1.DataSource = crudDatabase.Clients;
                customBox1.DisplayMember = "ClientName"; 
                customBox1.ValueMember = "ClientID"; 

                if (!string.IsNullOrEmpty(Properties.Settings.Default.SelectedClientId))
                {
                    if (int.TryParse(Properties.Settings.Default.SelectedClientId, out int selectedClientId))
                    {
                        customBox1.SelectedValue = selectedClientId; 
                    }
                    else
                    {                     
                    }
                }
            }
            else
            {
                loadTheClientID();
            }

            customBox2.Items.Add(LanguageManager.Instance.GetString("GenderF"));
            customBox2.Items.Add(LanguageManager.Instance.GetString("GenderH"));
            customBox2.Items.Add(LanguageManager.Instance.GetString("GenderO"));
            customBox2.SelectedIndex = 0;   
        }


        private void loadTheClientID()
        {
            if (int.TryParse(Properties.Settings.Default.SelectedClientId, out int clientId))
            {
                string idQuery = @"SELECT ClientName FROM Clients WHERE ClientID = @ClientID;";

                var parameters = new Dictionary<string, object>
                {
                    { "@ClientID", clientId }
                };

                DataTable idTable = crudDatabase.FetchDataFromDatabase(idQuery, parameters);

                if (idTable != null && idTable.Rows.Count > 0)
                {
                    customBox1.Text = idTable.Rows[0]["ClientName"].ToString();
                }
                else
                {
                }
            }
            else
            {
            }
        }

        private bool TableNumberExists(string tablenumber)
        {
            // Prepare the query and parameters
            string sql = "SELECT COUNT(*) AS Count FROM Tablenumber WHERE Thetablenumber = @Thetablenumber";
            var parameters = new Dictionary<string, object>
            {
                { "@Thetablenumber", tablenumber }
            };

            // Fetch data from the database
            DataTable resultTable = crudDatabase.FetchDataFromDatabase(sql, parameters);

            // Check if any rows were returned and return the count
            if (resultTable != null && resultTable.Rows.Count > 0)
            {
                int count = Convert.ToInt32(resultTable.Rows[0]["Count"]);
                return count > 0; // Returns true if a row with matching table number exists
            }

            return false; // Return false if no rows were found
        }


        private void UpdateTableAvailability(string tablenumber)
        {
            string sql = @"UPDATE Tablenumber SET IsAvailable = 1 WHERE Thetablenumber = @Thetablenumber AND IsAvailable != 1;";
            var parameters = new Dictionary<string, object>
            {
                { "@Thetablenumber", tablenumber }
            };

            // Execute the update command
            crudDatabase.ExecuteNonQuery(sql, parameters);
        }

        private void insertReservation() 
        {
            label8.Text = string.Empty;
            string guestGender = null;
            string guestName = textBox5.Text;
            string clientSqte = textBox7.Text;
            string tablenumber = textBox1.Text;
            string guestContact = textBox2.Text;
            string guestTime = customDateTimePicker1.Text;

            if (string.IsNullOrEmpty(guestName))
            {
                label8.Text += "Name is required. ";
                label8.ForeColor = Color.Tomato;
            }
            else if (string.IsNullOrEmpty(guestContact))
            {
                label8.Text += "Contact is required. ";
                label8.ForeColor = Color.Tomato;
            }
            else if (string.IsNullOrEmpty(clientSqte))
            {
                label8.Text += "Guest number is required. ";
                label8.ForeColor = Color.Tomato;
            }
            else if (string.IsNullOrEmpty(tablenumber))
            {
                label8.Text += "Tablenumber is required. ";
                label8.ForeColor = Color.Tomato;
            }

            else if (string.IsNullOrEmpty(guestTime))
            {
                label8.Text += "Guest time is required. ";
                label8.ForeColor = Color.Tomato;
            }
            else if (customBox2.SelectedItem == null)
            {
                label8.Text += "Gender is required. ";
                label8.ForeColor = Color.Tomato;
            }
            else
            {
                guestGender = customBox2.SelectedItem.ToString();
            }
            if (!string.IsNullOrEmpty(label8.Text))
            {
                return;
            }

            int clientId;

            if (Properties.Settings.Default.isHeadquarterMode)
            {
                if (customBox1.SelectedValue is int selectedClientId)
                {
                    clientId = selectedClientId; 
                }
                else
                {
                    label8.Text = "Please select a valid location.";
                    label8.ForeColor = Color.Tomato;
                    return; 
                }
            }
            else 
            {
                if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
                {
                    label8.Text = "Invalid Client ID.";
                    label8.ForeColor = Color.Tomato;
                    return; 
                }
            }
         
            // Check if the table number exists
            if (!TableNumberExists(tablenumber))
            {
                label8.Text = ("The entered table number does not exist");
                label8.ForeColor = Color.Tomato;
                return;
            }

            int newReceiptId;

            try
            {
                // Step 1: Get the next ReceiptId for the ClientID
                string receiptIdQuery = @"SELECT LastReserveId FROM Tabletcount WHERE ClientID = @ClientID LIMIT 1;";
                var parameters = new Dictionary<string, object>
                {
                    { "@ClientID", clientId }
                };

                DataTable receiptIdTable = crudDatabase.FetchDataFromDatabase(receiptIdQuery, parameters);

                if (receiptIdTable != null && receiptIdTable.Rows.Count > 0)
                {
                    // If the record exists, update LastReserveId
                    int lastReserveId = Convert.ToInt32(receiptIdTable.Rows[0]["LastReserveId"]);
                    newReceiptId = lastReserveId + 1;

                    string updateQuery = @"UPDATE Tabletcount SET LastReserveId = @NewLastReserveId WHERE ClientID = @ClientID;";
                    var updateParameters = new Dictionary<string, object>
                    {
                         { "@NewLastReserveId", newReceiptId },
                         { "@ClientID", clientId }
                    };

                    // Execute the update command
                    bool updateSuccess = crudDatabase.ExecuteNonQuery(updateQuery, updateParameters);
                    if (!updateSuccess)
                    {
                        label8.Text = "Failed to update LastReserveId.";
                        label8.ForeColor = Color.Tomato;
                        return;
                    }
                }
                else
                {
                    // If the record does not exist, insert a new record
                    string insertQuery = @"INSERT INTO Tabletcount (ClientID, LastReserveId) VALUES (@ClientID, 1);";
                    var insertParameters = new Dictionary<string, object>
                    {
                         { "@ClientID", clientId }
                    };

                    // Execute the insert command
                    bool insertSuccess = crudDatabase.ExecuteNonQuery(insertQuery, insertParameters);
                    if (!insertSuccess)
                    {
                        label8.Text = "Failed to create new Tabletcount record.";
                        label8.ForeColor = Color.Tomato;
                        return;
                    }

                    newReceiptId = 1; // Set newReceiptId to 1 for the new record
                }

                // Step 2: Insert the reservation
                string sql = @"INSERT INTO Reservetable (ReservationID, Guestgender, GuestContact, GuestName, Guesttime, Ordertime, Clientsqte, Tablenumber, Cancelled, Completed, ClientID) 
                       VALUES (@ReservationID, @Guestgender, @GuestContact, @GuestName, @Guesttime, @Ordertime, @Clientsqte, @Tablenumber, NULL, NULL, @ClientID)";

                var reservationParameters = new Dictionary<string, object>
                {
                    { "@ReservationID", newReceiptId },
                    { "@Guestgender", guestGender },
                    { "@GuestContact", guestContact },
                    { "@GuestName", guestName },
                    { "@Guesttime", guestTime },
                    { "@Ordertime", DateTime.Now }, // Use DateTime directly
                    { "@Clientsqte", clientSqte },
                    { "@Tablenumber", tablenumber },
                    { "@ClientID", clientId }
                };

                // Execute the reservation insert command
                bool reservationSuccess = crudDatabase.ExecuteNonQuery(sql, reservationParameters);
                if (reservationSuccess)
                {
                    Clearcontrols();
                    label8.Text = "Reservation successfully created.";
                    label8.ForeColor = Color.LawnGreen;
                    UpdateTableAvailability(tablenumber);
                    reservationform.triggerbutton1click();
                }
                else
                {
                    label8.Text = "Failed to create reservation.";
                    label8.ForeColor = Color.Tomato;
                }
            }
            catch (SQLiteException ex)
            {
                if (ex.Message.Contains("UNIQUE constraint failed"))
                {
                    // Handle unique constraint violation
                    label8.Text = "A reservation with this ID already exists.";
                    label8.ForeColor = Color.Tomato;
                }
                else
                {
                    label8.Text = $"An error occurred. Try again!";
                    label8.ForeColor = Color.Tomato;
                }
            }
        }
  

        private void button1_Click(object sender, EventArgs e)
        {
            if (currentState == "Add")
            {
                insertReservation();
            }
            else if (currentState == "Cancel")
            {
                updateStatusCancel();
            }
            else if (currentState == "Update")
            {
                updateReservation();
            }
            else 
            {
                return;
            }
        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            updateStatusCompleted();
        }

        private void Clearcontrols() 
        {
             textBox1.Text = "";
             textBox2.Text = "";
             textBox5.Text = "";
             textBox7.Text = "";
             customDateTimePicker1.Text = "";
             customBox2.Text = string.Empty;
             customBox1.Text = string.Empty;
        }

        public void disableControls()
        {
            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            textBox5.ReadOnly = true;
            textBox7.ReadOnly = true;
            customBox2.Enabled = false;
            customBox1.Enabled = false;      
            roundedTextbox1.ReadOnly = true;
            customDateTimePicker1.Enabled = false;
        }

        #region UPDATE STATUS
        private void updateStatusCancel()
        {
            if (int.TryParse(Properties.Settings.Default.SelectedClientId, out int clientId))
            {
                if (!string.IsNullOrWhiteSpace(label10.Text))
                {
                    string receiptId = label10.Text.Trim();

                    // Prepare the update query and parameters
                    string sql = @"UPDATE Reservetable SET Cancelled = @CancelledValue WHERE ClientID = @ClientID AND ReservationID = @ReservationID";
                   
                    var parameters = new Dictionary<string, object>
                    {
                        { "@CancelledValue", "iscancelled" },
                        { "@ClientID", clientId },
                        { "@ReservationID", receiptId }
                    };

                    try
                    {
                        // Execute the update command
                        bool success = crudDatabase.ExecuteNonQuery(sql, parameters);
                        if (success)
                        {
                            Clearcontrols();
                            reservationform.triggerbutton1click();
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        if (ex.ErrorCode == 19 && ex.Message.Contains("CHECK constraint failed"))
                        {
                            // Handle CHECK constraint failure
                        }
                    }
                }
            }
        }

        private void updateStatusCompleted()
        {
            if (int.TryParse(Properties.Settings.Default.SelectedClientId, out int clientId))
            {
                if (!string.IsNullOrWhiteSpace(label10.Text))
                {
                    string receiptId = label10.Text.Trim();

                    // Prepare the update query and parameters
                    string sql = @"UPDATE Reservetable SET Completed = @CompletedValue WHERE ClientID = @ClientID AND ReservationID = @ReservationID";
                    var parameters = new Dictionary<string, object>
                    {
                        { "@CompletedValue", "iscompleted" },
                        { "@ClientID", clientId },
                        { "@ReservationID", receiptId }
                    };

                    try
                    {
                        // Execute the update command
                        bool success = crudDatabase.ExecuteNonQuery(sql, parameters);
                        if (success)
                        {
                            Clearcontrols();
                            reservationform.triggerbutton1click();
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        if (ex.ErrorCode == 19 && ex.Message.Contains("CHECK constraint failed"))
                        {
                            // Handle CHECK constraint failure
                        }
                    }
                }
            }
        }
        #endregion

        #region UPDATE RESERVATION
        public void selectReservationById()
        {

            if (int.TryParse(Properties.Settings.Default.SelectedClientId, out int clientId) && !string.IsNullOrWhiteSpace(label10.Text))
            {
                string tableNumberValue = label10.Text.Trim();

                // Prepare the query and parameters
                string query = "SELECT * FROM Reservetable WHERE ReservationID = @ReservationID AND ClientID = @ClientID";
              
                var parameters = new Dictionary<string, object>
                {
                    { "@ReservationID", tableNumberValue },
                    { "@ClientID", clientId }
                };

                // Fetch data from the database
                DataTable data = crudDatabase.FetchDataFromDatabase(query, parameters);

                // Check if any rows were returned
                if (data != null && data.Rows.Count > 0)
                {
                    DataRow row = data.Rows[0];

                    // Set the status text and color
                    if (row["Completed"] != DBNull.Value)
                    {
                        roundedTextbox1.Text = LanguageManager.Instance.GetString("Tablecompleted");
                        roundedTextbox1.ForeColor = Color.ForestGreen;
                        checkBox1.Visible = true;
                    }
                    else if (row["Cancelled"] != DBNull.Value)
                    {
                        roundedTextbox1.Text = LanguageManager.Instance.GetString("Tablecancelled");
                        roundedTextbox1.ForeColor = Color.Tomato;
                        checkBox1.Visible = true;   
                    }
                    else
                    {
                        roundedTextbox1.Text = LanguageManager.Instance.GetString("Tablenostatus");
                        roundedTextbox1.ForeColor = Color.Gray;
                    }

                    // Populate other fields
                    customBox2.Text = row["Guestgender"].ToString();
                    textBox2.Text = row["GuestContact"].ToString();
                    textBox5.Text = row["GuestName"].ToString();
                    textBox7.Text = row["Clientsqte"].ToString();
                    textBox1.Text = row["Tablenumber"].ToString();
                    customDateTimePicker1.Text = row["Guesttime"].ToString();
                    //customBox1.Text = row["ClientID"].ToString();
                   
                    if (Properties.Settings.Default.isHeadquarterMode)
                    {
                        int theclientId = Convert.ToInt32(row["ClientID"]);
                        customBox1.SelectedValue = theclientId;
                    }
                    else 
                    {
                        loadTheClientID();
                    }
                }
            }
        }

        private void updateReservation()
        {
            label8.Text = string.Empty;
            string guestGender = null;
            string guestName = textBox5.Text;
            string clientSqte = textBox7.Text;
            string tablenumber = textBox1.Text;
            string guestContact = textBox2.Text;
            string guestTime = customDateTimePicker1.Text;


            if (string.IsNullOrEmpty(guestName))
            {
                label8.Text += "Name is required. ";
                label8.ForeColor = Color.Tomato;
            }
            else if (string.IsNullOrEmpty(guestContact))
            {
                label8.Text += "Contact is required. ";
                label8.ForeColor = Color.Tomato;
            }
            else if (string.IsNullOrEmpty(clientSqte))
            {
                label8.Text += "Guest number is required. ";
                label8.ForeColor = Color.Tomato;
            }
            else if (string.IsNullOrEmpty(tablenumber))
            {
                label8.Text += "Tablenumber is required. ";
                label8.ForeColor = Color.Tomato;
            }

            else if (string.IsNullOrEmpty(guestTime))
            {
                label8.Text += "Guest time is required. ";
                label8.ForeColor = Color.Tomato;
            }
            else if (customBox2.SelectedItem == null)
            {
                label8.Text += "Gender is required. ";
                label8.ForeColor = Color.Tomato;
            }
            else
            {
                guestGender = customBox2.SelectedItem.ToString();
            }
            if (!string.IsNullOrEmpty(label8.Text))
            {
                return;
            }
            if (!int.TryParse(label10.Text, out int reservationId))
            {
                label8.Text = ("Invalid Reservation ID.");
                return;
            }

            int clientId;

            if (Properties.Settings.Default.isHeadquarterMode)
            {
                if (customBox1.SelectedValue is int selectedClientId)
                {
                    clientId = selectedClientId;
                }
                else
                {
                    label8.Text = "Please select a valid location.";
                    label8.ForeColor = Color.Tomato;
                    return;
                }
            }
            else
            {
                if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
                {
                    label8.Text = "Invalid Client ID.";
                    label8.ForeColor = Color.Tomato;
                    return;
                }
            }

            if (!TableNumberExists(tablenumber))
            {
                label8.Text = ("The entered table number does not exist");
                return;
            }

            try
            {
                // Start building the SQL command
                string sql = @"UPDATE Reservetable SET Guestgender = @Guestgender, GuestContact = @GuestContact, GuestName = @GuestName,
                                       Guesttime = @Guesttime, Clientsqte = @Clientsqte, Tablenumber = @Tablenumber, ClientID = @ClientID";

                // Check if the checkbox is checked and modify the SQL command accordingly
                if (checkBox1.Checked)
                {
                    sql += @", Completed = @Completed, Cancelled = @Cancelled";
                }

                // Complete the SQL command with the WHERE clause
                sql += " WHERE ReservationID = @ReservationID";

                // Prepare parameters
                var parameters = new Dictionary<string, object>
                {
                     { "@Guestgender", guestGender },
                     { "@GuestContact", guestContact },
                     { "@GuestName", guestName },
                     { "@Guesttime", guestTime },
                     { "@Clientsqte", clientSqte },
                     { "@Tablenumber", tablenumber },
                     { "@ReservationID", reservationId },
                     {"@ClientID", clientId }
                };

                // Add Completed and Cancelled parameters if the checkbox is checked
                if (checkBox1.Checked)
                {
                    parameters.Add("@Completed", DBNull.Value); // Set to DBNull
                    parameters.Add("@Cancelled", DBNull.Value); // Set to DBNull
                }

                // Execute the update command
                bool success = crudDatabase.ExecuteNonQuery(sql, parameters);
                if (success)
                {
                    Clearcontrols();
                    reservationform.triggerbutton1click();
                }
            }
            catch (SQLiteException ex)
            {
                if (ex.ErrorCode == 2601)
                {
                    // Handle unique constraint violation
                    //using (Doubletablepop DTP = new Doubletablepop())
                    //{
                    //    FormHelper.ShowFormWithOverlay(this.FindForm(), DTP);
                    //}
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        private void customBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox5.Text))
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }
        }

        public void LocalizeControls()
        {
            //button1.Text = LanguageManager.Instance.GetString("MF-btn1");
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
            //button4.Text = LanguageManager.Instance.GetString("MF-btn4");
            //button5.Text = LanguageManager.Instance.GetString("MF-btn5");
            //button6.Text = LanguageManager.Instance.GetString("MF-btn6");
            //button7.Text = LanguageManager.Instance.GetString("MF-btn7");
            //button8.Text = LanguageManager.Instance.GetString("MF-btn8");
            //button9.Text = LanguageManager.Instance.GetString("MF-btn9");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            rjButton1.ForeColor = colors.Color2;
            rjButton1.BackColor = colors.Color3;
            customBox1.ArrowColor = colors.Color2;
            customBox2.ArrowColor = colors.Color2;
            customBox1.BackColor = colors.Color1;
            customBox2.BackColor = colors.Color1;
            customBox1.BorderColor = colors.Color5;
            customBox2.BorderColor = colors.Color5;
            roundedPanel1.BackColor = colors.Color3;
            roundedPanel2.BackColor = colors.Color1;
            textBox1.BackColorRounded = colors.Color1;
            textBox2.BackColorRounded = colors.Color1;
            textBox5.BackColorRounded = colors.Color1;
            textBox7.BackColorRounded = colors.Color1;
            roundedTextbox1.ForeColor = colors.Color2;
            customBox1.BorderFocusColor = colors.Color5;
            customBox2.BorderFocusColor = colors.Color5;

            textBox1.ForeColor = this.ForeColor;
            textBox2.ForeColor = this.ForeColor;
            textBox5.ForeColor = this.ForeColor;
            textBox7.ForeColor = this.ForeColor;
            customBox1.ForeColor = this.ForeColor;
            customBox2.ForeColor = this.ForeColor;
            roundedTextbox1.BackColorRounded = colors.Color1;
            customDateTimePicker1.TextColor = this.ForeColor;
            customDateTimePicker1.BorderColor = colors.Color5;
            customDateTimePicker1.BackColorCustom = colors.Color1;
        }
    }
}
