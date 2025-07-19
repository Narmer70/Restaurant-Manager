using System;
using QRCoder;
using System.IO;
using System.Data;
using System.Drawing;
using PadTai.Classes;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Data.SqlClient;
using PadTai.Classes.Others;
using System.Drawing.Printing;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using PadTai.Classes.Controlsdesign;
using PadTai.Sec_daryfolders.Updates;
using PadTai.Sec_daryfolders.Tablereserve;
using PadTai.Sec_daryfolders.Updaters.FoodUpdater;


namespace PadTai.Sec_daryfolders.Tablereservation
{
    public partial class Reservationdetails : Form
    {
        private Reservationform reservationform;
        public int ReservationId { get; set; }
        private DataGridViewScroller scroller;
        private DraggableForm draggableForm;
        private CrudDatabase crudDatabase;
        private bool isKeyPressed = false;
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public Reservationdetails(Reservationform reserve)
        {
            InitializeComponent();
            initiateResieControls();
            this.reservationform = reserve;
            crudDatabase = new CrudDatabase();

            LocalizeControls();
            ApplyTheme();
        }

        private void initiateResieControls() 
        {
            draggableForm = new DraggableForm();
            draggableForm.EnableDragging(this, this);
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(button1);
            resizer.RegisterControl(button2);
            resizer.RegisterControl(button3);
            resizer.RegisterControl(button4);
            resizer.RegisterControl(button5);
            resizer.RegisterControl(button6);
            resizer.RegisterControl(button7);
            resizer.RegisterControl(dataGridView1);
            resizer.RegisterControl(dataGridView2);
            resizer.RegisterControl(dataGridView3);

            scroller = new DataGridViewScroller(this, null, dataGridView1, dataGridView2, dataGridView3);
            this.KeyDown += new KeyEventHandler(Reservationdetails_KeyDown);
            this.KeyUp += new KeyEventHandler(Reservationdetails_KeyUp);
            this.KeyPreview = true;
        }


        public void LoadReceiptDetails(int reservationId)
        {
            Reservation reservation = LoadReservationDetails(reservationId);
          
            if (reservation == null)
            {
                return; // Exit if no reservation was found
            }

            // Clear existing rows in the DataGridViews
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();

            // Populate DataGridViews with reservation details
            dataGridView1.Rows.Add(reservation.ReservationID.ToString());
            dataGridView1.Rows.Add(reservation.OrderTime);
            dataGridView2.Rows.Add(reservation.TableNumber);
            dataGridView3.Rows.Add("    " + LanguageManager.Instance.GetString("Printtableguests"), reservation.NumberOfGuests.ToString());
            dataGridView3.Rows.Add("    " + LanguageManager.Instance.GetString("Printtabletime"), reservation.ReservationTime);
            dataGridView3.Rows.Add("    " + LanguageManager.Instance.GetString("Printtableguestgender"), reservation.GuestGender);
            dataGridView3.Rows.Add("    " + LanguageManager.Instance.GetString("Printtableguestname"), reservation.GuestName);
            dataGridView3.Rows.Add("    " + LanguageManager.Instance.GetString("Printtableguestcontact"), reservation.GuestContact);
            dataGridView3.DefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);

            // Set status based on reservation details
            if (reservation.Status == LanguageManager.Instance.GetString("Tablecompleted"))
            {
                dataGridView3.Rows.Add("    " + LanguageManager.Instance.GetString("Printtablestatus"), LanguageManager.Instance.GetString("Tablecompleted"));
            }
            else if (reservation.Status == LanguageManager.Instance.GetString("Tablecancelled"))
            {
                dataGridView3.Rows.Add("    " + LanguageManager.Instance.GetString("Printtablestatus"), LanguageManager.Instance.GetString("Tablecancelled"));
            }

            resizeDgvscolumms();
        }

        private void resizeDgvscolumms() 
        {
            dataGridView2.RowTemplate.Height = 220;
            dataGridView1.Columns[0].Width = (int)(dataGridView1.Width * 1.00);
            dataGridView2.Columns[0].Width = (int)(dataGridView2.Width * 1.00);
            dataGridView3.Columns[0].Width = (int)(dataGridView3.Width * 0.40);
            dataGridView3.Columns[1].Width = (int)(dataGridView1.Width * 0.60);
           
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells[0].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                row.Cells[0].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            foreach (DataGridViewRow row in dataGridView3.Rows)
            {
                row.Cells[1].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
        }

        private void deleteReservation()
        {
            int clientId;

            if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
            {
                return; 
            }

            string query = "DELETE FROM Reservetable WHERE ReservationID = @ReservationID AND ClientID = @ClientID";
          
            var parameters = new Dictionary<string, object>
            {
                { "@ReservationID", ReservationId },
                { "@ClientID", clientId }
            };

            try
            {
                bool rowsAffected = crudDatabase.ExecuteNonQuery(query, parameters);

                if (rowsAffected)
                {
                   // MessageBox.Show("Reservation deleted successfully.");
                      updateforms();
                }
                else
                {
                    MessageBox.Show("No reservation found to delete.");
                }

            }
            catch
            {
               
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex == 0)
            {
                e.CellStyle.Font = new Font("Arial", 16, FontStyle.Bold);
            }
            else if (e.RowIndex == 1)
            {
                e.CellStyle.ForeColor = Color.DodgerBlue;
                e.CellStyle.SelectionForeColor = Color.DodgerBlue;
                e.CellStyle.Font = new Font("Arial", 10, FontStyle.Regular);
            }
        }

        private void dataGridView2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex == 0 && e.ColumnIndex == 0)
            {
                e.CellStyle.ForeColor = Color.White;
                e.CellStyle.Font = new Font("Segoe UI", 80, FontStyle.Bold);
            }
        }

        private void dataGridView3_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex == 5 && e.ColumnIndex == 1)
            {
                int clientId;

                if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out clientId))
                {
                    return; 
                }

                string query = "SELECT * FROM Reservetable WHERE ReservationID = @ReservationID AND ClientID = @ClientID";
            
                var parameters = new Dictionary<string, object>
                {
                    { "@ReservationID", ReservationId },
                    { "@ClientID", clientId }
                };

                DataTable reservationTable = crudDatabase.FetchDataFromDatabase(query, parameters);

                if (reservationTable != null && reservationTable.Rows.Count > 0)
                {
                    DataRow row = reservationTable.Rows[0]; 

                    if (row["Completed"] != DBNull.Value)
                    {
                        e.CellStyle.ForeColor = Color.ForestGreen;
                        e.CellStyle.SelectionForeColor = Color.ForestGreen;
                        e.CellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
                    }
                    else if (row["Cancelled"] != DBNull.Value)
                    {
                        e.CellStyle.ForeColor = Color.Crimson;
                        e.CellStyle.SelectionForeColor = Color.Crimson;
                        e.CellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
                    }
                }
            }
        }

        public Reservation LoadReservationDetails(int reservationId)
        {
            // Validate Client ID
            if (!int.TryParse(Properties.Settings.Default.SelectedClientId, out int clientId))
            {
                return null; 
            }

            // Load reservation details
            string query = "SELECT * FROM Reservetable WHERE ReservationID = @ReservationID AND ClientID = @ClientID";
            var parameters = new Dictionary<string, object>
            {
                { "@ReservationID", reservationId },
                { "@ClientID", clientId }
            };

            // Fetch data from the database
            DataTable reservationTable = crudDatabase.FetchDataFromDatabase(query, parameters);

            if (reservationTable != null && reservationTable.Rows.Count > 0)
            {
                DataRow reader = reservationTable.Rows[0]; // Get the first row

                // Create and populate a new Reservation object
                var reservation = new Reservation
                {
                    ReservationID = Convert.ToInt32(reader["ReservationID"]),
                    OrderTime = reader["Ordertime"].ToString(),
                    TableNumber = reader["Tablenumber"].ToString(),
                    NumberOfGuests = Convert.ToInt32(reader["Clientsqte"]),
                    ReservationTime = reader["Guesttime"].ToString(),
                    GuestGender = reader["Guestgender"].ToString(),
                    GuestName = reader["GuestName"].ToString(),
                    GuestContact = reader["GuestContact"].ToString(),
                    Status = reader["Completed"] != DBNull.Value ? LanguageManager.Instance.GetString("Tablecompleted") :
                             reader["Cancelled"] != DBNull.Value ? LanguageManager.Instance.GetString("Tablecancelled") : ""
                };

                // Fetch restaurant and client details
                var detailsFetcher = new RestaurantDetails();
                var (restaurantName, restaurantLogo, clientName, clientAddress) = detailsFetcher.GetDetails();

                reservation.RestaurantName = restaurantName;
                reservation.RestaurantLogo = restaurantLogo;
                reservation.ClientName = clientName;
                reservation.ClientAddress = clientAddress;

                return reservation;
            }
            else
            {
                //MessageBox.Show("No reservation found.");
                return null;
            }
        }

        private void PrintReceipt(Reservation reservation)
        {
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += (sender, e) =>
            {
                // Set up the font and brush
                Brush brush = Brushes.Black;
                Font font = new Font("Arial", 10);
                Font thefont = new Font("Arial", 10, FontStyle.Italic);
                Font otherfont = new Font("Arial", 10, FontStyle.Bold);

                // Center the restaurant name and address
                float discountValueX = e.Graphics.VisibleClipBounds.Width - 120;
                float centerX = e.Graphics.VisibleClipBounds.Width / 2;
                float discountLabelX = 10;
                float yPos = 0; 

                // Draw reservation ID centered
                string reservationIdText = $"{reservation.ReservationID}";
                SizeF reservationIdSize = e.Graphics.MeasureString(reservationIdText, new Font("Century Gothic", 14, FontStyle.Bold));
                e.Graphics.DrawString(reservationIdText, new Font("Century Gothic", 14, FontStyle.Bold), brush, centerX - (reservationIdSize.Width / 2), yPos);
                yPos += 30;

                // Draw Order Time centered
                string orderTimeText = $"{reservation.OrderTime}";
                SizeF orderTimeSize = e.Graphics.MeasureString(orderTimeText, font);
                e.Graphics.DrawString(orderTimeText, font, brush, centerX - (orderTimeSize.Width / 2), yPos);
                yPos += 15;

                // Draw a line separator
                e.Graphics.DrawString("---------------------------------------------------", new Font("Century Gothic", 12, FontStyle.Bold), brush, new PointF(5, yPos));
                yPos += 20;

                // Draw Table Number
                string tableNumberText = LanguageManager.Instance.GetString("Printtablenumber");
                SizeF tableNumberSize = e.Graphics.MeasureString(tableNumberText, thefont);
                e.Graphics.DrawString(tableNumberText, thefont, brush, centerX - (tableNumberSize.Width / 2), yPos);
                yPos += 20;

                string NumberText = $"{reservation.TableNumber}";
                SizeF NumberSize = e.Graphics.MeasureString(NumberText, new Font("Century Gothic", 30, FontStyle.Bold));
                e.Graphics.DrawString(NumberText, new Font("Century Gothic", 30, FontStyle.Bold), brush, centerX - (NumberSize.Width / 2), yPos);
                yPos += 60;

                // Draw Number of Guests
                e.Graphics.DrawString(LanguageManager.Instance.GetString("Printtableguests"), thefont, brush, discountLabelX, yPos);
                e.Graphics.DrawString($"{reservation.NumberOfGuests}", font, brush, discountValueX, yPos);
                yPos += 20;

                // Draw Reservation Time
                e.Graphics.DrawString(LanguageManager.Instance.GetString("Printtabletime"), thefont, brush, discountLabelX, yPos);
                e.Graphics.DrawString(reservation.ReservationTime, otherfont, brush, discountValueX, yPos);
                yPos += 20;

                // Draw Guest Details
                e.Graphics.DrawString(LanguageManager.Instance.GetString("Printtableguestname"), thefont, brush, discountLabelX, yPos);
                e.Graphics.DrawString(reservation.GuestName, font, brush, discountValueX, yPos);
                yPos += 20;

                e.Graphics.DrawString(LanguageManager.Instance.GetString("Printtableguestgender"), thefont, brush, discountLabelX, yPos);
                e.Graphics.DrawString(reservation.GuestGender, font, brush, discountValueX, yPos);
                yPos += 20;

                if (reservation.Status != "") 
                {
                    // Draw Status
                    e.Graphics.DrawString(LanguageManager.Instance.GetString("Printtablestatus"), thefont, brush, discountLabelX, yPos);
                    e.Graphics.DrawString(reservation.Status, font, brush, discountValueX, yPos);
                    yPos += 20;
                }

                e.Graphics.DrawString(LanguageManager.Instance.GetString("Printtableguestcontact"), thefont, brush, discountLabelX, yPos);
                e.Graphics.DrawString(reservation.GuestContact, font, brush, discountValueX, yPos);
                yPos += 20;

                // Draw a line separator
                e.Graphics.DrawString("---------------------------------------------------", new Font("Century Gothic", 12, FontStyle.Bold), brush, new PointF(5, yPos));
                yPos += 20;
           
                // Draw a thank you message
                string thanksMessage = LanguageManager.Instance.GetString("Printtablethanks");
                SizeF thanksMessageSize = e.Graphics.MeasureString(thanksMessage, font);
                e.Graphics.DrawString(thanksMessage, font, brush, centerX - (thanksMessageSize.Width / 2), yPos);
                yPos += 20;

                // Draw the restaurant address
                SizeF restaurantAddressSize = e.Graphics.MeasureString(reservation.ClientAddress, font);
                e.Graphics.DrawString(reservation.ClientAddress, font, brush, centerX - (restaurantAddressSize.Width / 2), yPos);
                yPos += 20;

                // Draw the restaurant location
                string locationText = reservation.RestaurantName.ToUpper() + ":" + reservation.ClientName.ToUpper();
                SizeF locationSize = e.Graphics.MeasureString(locationText, font);
                e.Graphics.DrawString(locationText, font, brush, centerX - (locationSize.Width / 2), yPos);

                if (Properties.Settings.Default.printBrandLogo && !Properties.Settings.Default.printQRcode)
                {
                    // Draw the restaurant logo if available
                    if (reservation.RestaurantLogo != null)
                    {
                        yPos += 20;

                        using (MemoryStream ms = new MemoryStream(reservation.RestaurantLogo))
                        {
                            Image logo = Image.FromStream(ms);

                            // Define the desired width and height for the logo
                            int desiredWidth = 80; // Set your desired width
                            int desiredHeight = 80; // Set your desired height

                            // Calculate the aspect ratio
                            float aspectRatio = (float)logo.Width / logo.Height;

                            // Adjust dimensions to maintain aspect ratio
                            if (logo.Width > logo.Height)
                            {
                                desiredHeight = (int)(desiredWidth / aspectRatio);
                            }
                            else
                            {
                                desiredWidth = (int)(desiredHeight * aspectRatio);
                            }

                            // Calculate the position to center the logo
                            int centerPic = (int)(e.Graphics.VisibleClipBounds.Width - desiredWidth) / 2;

                            // Draw the logo with the specified dimensions
                            e.Graphics.DrawImage(logo, new Rectangle(centerPic, (int)yPos, desiredWidth, desiredHeight));
                        }
                    }
                }
                else if (Properties.Settings.Default.printBrandLogo && Properties.Settings.Default.printQRcode)
                {
                    // Draw the restaurant logo if available
                    if (reservation.RestaurantLogo != null)
                    {
                        yPos += 20;

                        using (MemoryStream ms = new MemoryStream(reservation.RestaurantLogo))
                        {
                            Image logo = Image.FromStream(ms);

                            // Define the desired width and height for the logo
                            int desiredWidth = 80; // Set your desired width
                            int desiredHeight = 80; // Set your desired height

                            // Calculate the aspect ratio
                            float aspectRatio = (float)logo.Width / logo.Height;

                            // Adjust dimensions to maintain aspect ratio
                            if (logo.Width > logo.Height)
                            {
                                desiredHeight = (int)(desiredWidth / aspectRatio);
                            }
                            else
                            {
                                desiredWidth = (int)(desiredHeight * aspectRatio);
                            }

                            // Draw the logo with the specified dimensions
                            e.Graphics.DrawImage(logo, new Rectangle(10, (int)yPos, desiredWidth, desiredHeight));
                        }
                    }

                    // Generate QR Code for the receipt
                    string qrText = LanguageManager.Instance.GetString("Receiptprintqrcode"); // Replace with actual data
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(20); // You can adjust the size here

                    // Draw the QR Code next to the logo
                    int qrCodeWidth = 80;
                    int qrCodeHeight = 80;
                    float qrCodeX = e.Graphics.VisibleClipBounds.Width - 100;
                    int qrCodeY = (int)yPos; // Align with the logo's y position

                    e.Graphics.DrawImage(qrCodeImage, new Rectangle((int)qrCodeX, qrCodeY, qrCodeWidth, qrCodeHeight));
                }
                else if (!Properties.Settings.Default.printBrandLogo && Properties.Settings.Default.printQRcode)
                {
                    yPos += 20;

                    // Generate QR Code for the receipt
                    string qrText = LanguageManager.Instance.GetString("Receiptprintqrcode"); // Replace with actual data
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(20); // You can adjust the size here

                    // Draw the QR Code next to the logo
                    int qrCodeWidth = 80;
                    int qrCodeHeight = 80;
                    int qrCodeX = (int)(e.Graphics.VisibleClipBounds.Width - qrCodeWidth) / 2;
                    int qrCodeY = (int)yPos; // Align with the logo's y position

                    e.Graphics.DrawImage(qrCodeImage, new Rectangle(qrCodeX, qrCodeY, qrCodeWidth, qrCodeHeight));
                }
                else
                {
                }
            };

            printDocument.Print();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Reservation reservation = LoadReservationDetails(ReservationId); 

            if (reservation != null)
            {
                PrintReceipt(reservation);
            }
            else
            {
                MessageBox.Show("No reservation found.");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();   
        }

        private void Reservationdetails_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
            if (ReservationId > 0)
            {
                LoadReceiptDetails(ReservationId);
            }
        }

        private void Reservationdetails_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
                resizeDgvscolumms();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (ReservationId > 0) 
            {
                ReservationId--; 
                LoadReceiptDetails(ReservationId); 
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (ReservationId > 0) 
            {
                ReservationId++; 
                LoadReceiptDetails(ReservationId); 
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ReservationId > 0)
            {
                using (Deletedishconfirm DDC = new Deletedishconfirm())
                {
                    FormHelper.ShowFormWithOverlay(this.FindForm(), DDC);

                    // Check the DialogResult after the dialog is closed
                    if (DDC.DialogResult == DialogResult.OK)
                    {
                        deleteReservation();
                        DDC.Close();
                    }
                    else if (DDC.DialogResult == DialogResult.Cancel)
                    {
                        DDC.Close();
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Modifyreservation updatereservation = new Modifyreservation(reservationform);
            reservationform.AdduserControl(updatereservation);
            updatereservation.label6.Visible = true;
            updatereservation.button1.Text = "Update";
            updatereservation.label10.Text = string.Empty;
            updatereservation.roundedTextbox1.Visible = true;
            updatereservation.roundedTextbox1.Enabled = false;
            updatereservation.label10.Text = ReservationId.ToString();
            updatereservation.selectReservationById();
            updatereservation.currentState = "Update";
            this.Close();
        }

        public void updateforms()
        {
            reservationform.LoadData();
            reservationform.UpdateControlVisibility();
            this.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Modifyreservation updatestatus = new Modifyreservation(reservationform);
            reservationform.AdduserControl(updatestatus);
            updatestatus.disableControls();
            updatestatus.label6.Visible = true;
            updatestatus.rjButton2.Visible = true;
            updatestatus.rjButton1.Visible = false;
            updatestatus.label10.Text = string.Empty;   
            updatestatus.roundedTextbox1.Visible = true;
            updatestatus.button1.BackColor = Color.Crimson;
            updatestatus.button1.BorderColor = Color.Crimson;
            updatestatus.label10.Text = ReservationId.ToString();
            updatestatus.selectReservationById();
            updatestatus.button1.Text = "Cancelled";
            updatestatus.currentState = "Cancel";
            updatestatus.checkBox1.Visible = false;
            this.Close();
        }

        private void Reservationdetails_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if the Up Arrow key is pressed
            if (e.KeyCode == Keys.Up && !isKeyPressed)
            {
                isKeyPressed = true;
                if (ReservationId > 0)
                {
                    ReservationId++;
                    LoadReceiptDetails(ReservationId);
                }
            }
            // Check if the Down Arrow key is pressed
            else if (e.KeyCode == Keys.Down && !isKeyPressed)
            {
                isKeyPressed = true;
                if (ReservationId > 0)
                {
                    ReservationId--;
                    LoadReceiptDetails(ReservationId);
                }
            }
        }

        private void Reservationdetails_KeyUp(object sender, KeyEventArgs e)
        {
            // Reset the flag when the key is released
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                isKeyPressed = false;
            }
        }

        public void LocalizeControls()
        {
            button5.Text = LanguageManager.Instance.GetString("Btn-close");
            //button1.Text = LanguageManager.Instance.GetString("MF-btn1");
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
            //button4.Text = LanguageManager.Instance.GetString("MF-btn4");
            //button5.Text = LanguageManager.Instance.GetString("MF-btn5");
            //button6.Text = LanguageManager.Instance.GetString("MF-btn6");
            //button7.Text = LanguageManager.Instance.GetString("MF-btn7");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            dataGridView2.BackgroundColor = colors.Color1;
            dataGridView1.BackgroundColor = colors.Color3;
            dataGridView3.BackgroundColor = colors.Color3;
            dataGridView2.DefaultCellStyle.BackColor = colors.Color1;
            dataGridView1.DefaultCellStyle.BackColor = colors.Color3;
            dataGridView3.DefaultCellStyle.BackColor = colors.Color3;
            dataGridView2.DefaultCellStyle.SelectionBackColor = colors.Color1;
            dataGridView1.DefaultCellStyle.SelectionBackColor = colors.Color3;
            dataGridView3.DefaultCellStyle.SelectionBackColor = colors.Color3;

            dataGridView1.ForeColor = this.ForeColor;
            dataGridView2.ForeColor = this.ForeColor;
            dataGridView3.ForeColor = this.ForeColor;
            dataGridView1.DefaultCellStyle.ForeColor = this.ForeColor;
            dataGridView2.DefaultCellStyle.ForeColor = this.ForeColor;
            dataGridView3.DefaultCellStyle.ForeColor = this.ForeColor;
            dataGridView1.DefaultCellStyle.SelectionForeColor = this.ForeColor;
            dataGridView2.DefaultCellStyle.SelectionForeColor = this.ForeColor;
            dataGridView3.DefaultCellStyle.SelectionForeColor = this.ForeColor;
        }
    }

    public class Reservation
    {
        public int ReservationID { get; set; }
        public string OrderTime { get; set; }
        public string TableNumber { get; set; }
        public int NumberOfGuests { get; set; }
        public string ReservationTime { get; set; }
        public string GuestGender { get; set; }
        public string GuestName { get; set; }
        public string GuestContact { get; set; }
        public string Status { get; set; } 
        public string ClientName { get; set; }
        public string ClientAddress { get; set; }
        public string RestaurantName { get; set; }
        public byte[] RestaurantLogo { get; set; }
    }
}
