using System;
using QRCoder;
using System.IO;
using System.Net;
using System.Data;
using System.Linq;
using System.Text;
using PadTai.Classes;
using System.Drawing;
using System.Net.Mail;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Globalization;
using System.Data.SqlClient;
using PadTai.Classes.Others;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using PadTai.Sec_daryfolders.Others;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using PadTai.Sec_daryfolders.Others_Forms;
using PadTai.Sec_daryfolders.Applisettings;


namespace PadTai.Sec_daryfolders.Deptmmodifier
{
    public partial class Otdelrenamer : UserControl
    {
        public event EventHandler ImageChanged;
        private Clavieroverlay clavieroverlay;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;
        private string generatedCode;

        public Otdelrenamer()
        {
            InitializeComponent();
            InitializeControlResizer();

            LoadAppscodeData();
            GenerateQRCode();
            LoadClientIDs();
            CheckForChanges(); 

            ImageChanged += pictureBoxImageChanged;
            LocalizeControls();
            ApplyTheme();
        }

        private void InitializeControlResizer()
        {
            crudDatabase = new CrudDatabase();

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);    
            
            label5.Visible = false;  
            button2.Visible = false;
            comboBox1.Visible = false;
            rjButton6.Enabled = false;
            rjButton4.Visible = false;
            roundedTextbox5.Visible = false;

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(panel1);
            resizer.RegisterControl(panel2);
            resizer.RegisterControl(label1);
            resizer.RegisterControl(label3);
            resizer.RegisterControl(label4);
            resizer.RegisterControl(label5);
            resizer.RegisterControl(label6);
            resizer.RegisterControl(label7);
            resizer.RegisterControl(label8);
            resizer.RegisterControl(label9);
            resizer.RegisterControl(label10);
            resizer.RegisterControl(label11);
            resizer.RegisterControl(label12);
            resizer.RegisterControl(button1);
            resizer.RegisterControl(button2);
            resizer.RegisterControl(textBox1);
            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(rjButton2);
            resizer.RegisterControl(rjButton3);
            resizer.RegisterControl(rjButton4);
            resizer.RegisterControl(rjButton5);
            resizer.RegisterControl(rjButton6);
            resizer.RegisterControl(rjButton7);
            resizer.RegisterControl(rjButton8);
            resizer.RegisterControl(comboBox1);
            resizer.RegisterControl(pictureBox1);
            resizer.RegisterControl(pictureBox2);
            resizer.RegisterControl(roundedPanel1);
            resizer.RegisterControl(roundedTextbox1);
            resizer.RegisterControl(roundedTextbox2);
            resizer.RegisterControl(roundedTextbox3);
            resizer.RegisterControl(roundedTextbox4);
            resizer.RegisterControl(roundedTextbox5);

            label5.Visible = comboBox1.Visible = button2.Visible = HasMoreThanOneClient();
            CenterLabel();
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
            label4.Left = (panel1.ClientSize.Width - label4.Width) / 2;
            label3.Left = (panel2.ClientSize.Width - label3.Width) / 2;
            label10.Left = (panel1.ClientSize.Width - label10.Width) / 2;
            pictureBox1.Left = (panel1.ClientSize.Width - pictureBox1.Width) / 2;
            pictureBox2.Left = (panel1.ClientSize.Width - pictureBox2.Width) / 2;
        }

        public bool HasMoreThanOneClient()
        {
            string query = "SELECT COUNT(*) FROM Clients";

            try
            {
                long count = (long)crudDatabase.FetchDataFromDatabase(query).Rows[0][0];
                return count > 1; // Return true if count is greater than 1
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
                return false; // Return false in case of an error
            }
        }

        private void GenerateQRCode()
        {
            // Get the connection string from the DatabaseConnection class
            string message = LanguageManager.Instance.GetString("QRCode-Msg");

            // Generate the QR code
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(message, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    // Get the bitmap of the QR code
                    Bitmap qrCodeImage = qrCode.GetGraphic(20);
                    // Display the QR code in the PictureBox
                    pictureBox2.Image = qrCodeImage;
                }
            }
        }

        private void SetPictureBoxImage(Image newImage)
        {
            if (pictureBox1.Image != newImage)
            {
                pictureBox1.Image = newImage;
                OnImageChanged(EventArgs.Empty); 
            }
        }

        // Method to raise the ImageChanged event
        protected virtual void OnImageChanged(EventArgs e)
        {
            ImageChanged?.Invoke(this, e);
        }

        private void pictureBoxImageChanged (object sender, EventArgs e)
        {
            // Check if the PictureBox has an image
            if (pictureBox1.Image == null)
            {
                rjButton6.Enabled = false; 
            }
            else
            {
                rjButton6.Enabled = rjButton7.Enabled = true;
            }
        }

        #region Load Account data
        private void LoadAppscodeData()
        {
            string query = "SELECT * FROM Appaccesscode";

            try
            {
                DataTable appAccessData = crudDatabase.FetchDataFromDatabase(query);

                if (appAccessData.Rows.Count > 0) // Check if there are any records
                {
                    DataRow reader = appAccessData.Rows[0]; // Read the first record
                    roundedTextbox2.Text = reader["Businessname"].ToString();
                    roundedTextbox4.Text = reader["Password"].ToString();
                    roundedTextbox3.Text = reader["Login"].ToString();

                    string location = LoadClient();

                    if (location != null)
                    {
                        label4.Text = reader["Businessname"].ToString().ToUpper() + ": " + location.ToUpper();
                    }

                    SaveDraftData();

                    // Load the logo from the BLOB
                    if (reader["Brandlogo"] != DBNull.Value)
                    {
                        byte[] logoData = (byte[])reader["Brandlogo"];
                        using (MemoryStream ms = new MemoryStream(logoData))
                        {
                            pictureBox1.Image = Image.FromStream(ms);
                        }
                    }
                    else
                    {
                        pictureBox1.Image = Properties.Resources.image_add_13434878;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void LoadClientIDs()
        {
            string query = "SELECT ClientID, ClientName FROM Clients";

            try
            {
                DataTable clientData = crudDatabase.FetchDataFromDatabase(query);
                List<Client> clients = new List<Client>();

                foreach (DataRow reader in clientData.Rows)
                {
                    var client = new Client
                    {
                        Id = reader["ClientID"].ToString(),
                        Name = reader["ClientName"].ToString()
                    };
                    clients.Add(client);
                }

                comboBox1.DataSource = clients;
                comboBox1.DisplayMember = "Name";
                comboBox1.ValueMember = "Id";
                comboBox1.SelectedValue = Properties.Settings.Default.SelectedClientId;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private string LoadClient()
        {
            string clientName = string.Empty;
            string clientAddress = string.Empty;

            try
            {
                string clientIdString = Properties.Settings.Default.SelectedClientId;

                if (int.TryParse(clientIdString, out int clientId))
                {
                    string query = "SELECT ClientID, ClientName, Clientadress FROM Clients WHERE ClientID = @ClientID";
                    var parameters = new Dictionary<string, object>
                    {
                        { "@ClientID", clientId }
                    };

                    DataTable clientData = crudDatabase.FetchDataFromDatabase(query, parameters);

                    if (clientData.Rows.Count > 0)
                    {
                        DataRow reader = clientData.Rows[0];
                        clientName = reader["ClientName"].ToString(); // Get the client name
                        clientAddress = reader["Clientadress"].ToString(); // Get the client address
                        textBox1.Text = clientName;
                        roundedTextbox1.Text = clientAddress;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }

            return clientName; // Return the client name
        }
        #endregion



        #region Update Account Data
        private void SaveSelectedClientId(string clientId)
        {
            // Save the selected client ID in user settings
            Properties.Settings.Default.SelectedClientId = clientId;
            Properties.Settings.Default.Save();

            this.Alert("Location changed.", Alertform.enmType.Success);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                Client selectedClient = (Client)comboBox1.SelectedItem;
                string selectedId = selectedClient.Id;
                SaveSelectedClientId(selectedId);
                LoadAppscodeData();
                rjButton3.Enabled = false;
                rjButton5.Enabled = false;
                rjButton4.Enabled = false;
                rjButton2.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = false;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Image Files (*.jpg, *.jpeg, *.png, *.gif)|*.jpg;*.jpeg;*.png;*.gif";
            openFileDialog1.Title = "Select an Image";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Get the image file path
                string imagePath = openFileDialog1.FileName;
               
                Image newImage = Image.FromFile(imagePath);
                SetPictureBoxImage(newImage);              
            }
        }

        private void UpdateClientName()
        {
            string query = "UPDATE Clients SET ClientName = @ClientName WHERE ClientID = @ClientID";
            var parameters = new Dictionary<string, object>
            {
                { "@ClientName", textBox1.Text.Trim() },
                { "@ClientID", Properties.Settings.Default.SelectedClientId }
            };

            try
            {
                crudDatabase.ExecuteNonQuery(query, parameters);
                MessageBox.Show("Client name updated successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void UpdateBusinessName(string newBusinessName)
        {
            string query = "UPDATE Appaccesscode SET Businessname = @Businessname";
            var parameters = new Dictionary<string, object>
            {
                { "@Businessname", newBusinessName }
            };

            try
            {
                crudDatabase.ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while updating the business name: " + ex.Message);
            }
        }

        private void UpdatePassword(string newPassword)
        {
            string query = "UPDATE Appaccesscode SET Password = @Password";
            var parameters = new Dictionary<string, object>
            {
                { "@Password", newPassword }
            };

            try
            {
                crudDatabase.ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while updating the password: " + ex.Message);
            }
        }

        private void UpdateLogin(string newLogin)
        {
            string query = "UPDATE Appaccesscode SET Login = @Login";
            var parameters = new Dictionary<string, object>
            {
                { "@Login", newLogin }
            };

            try
            {
                crudDatabase.ExecuteNonQuery(query, parameters);
                this.Alert("Email updated!", Alertform.enmType.Success);
                label11.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while updating the login: " + ex.Message);
            }
        }

        private void UpdateBrandLogo(Image logoImage)
        {
            if (logoImage != null)
            {
                byte[] logoData = ImageToByteArray(logoImage);
                string query = "UPDATE Appaccesscode SET Brandlogo = @Brandlogo";
                var parameters = new Dictionary<string, object>
                {
                    { "@Brandlogo", logoData }
                };

                try
                {
                    crudDatabase.ExecuteNonQuery(query, parameters);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while updating the brand logo: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("No image to update.");
            }
        }

        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png); 
                return ms.ToArray();
            }
        }

        private void UpdateClientName(string newLocation)
        {
            string query = "UPDATE Clients SET Clientadress = @Adress WHERE ClientID = @ClientID";
            var parameters = new Dictionary<string, object>
            {
                { "@Adress", newLocation },
                { "@ClientID", Properties.Settings.Default.SelectedClientId }
            };

            try
            {
                crudDatabase.ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void DeleteBrandPic()
        {
            string query = "UPDATE Appaccesscode SET Brandlogo = NULL";

            try
            {
                crudDatabase.ExecuteNonQuery(query);
                rjButton6.Enabled = false;
                rjButton7.Enabled = false;
                pictureBox1.Image = Properties.Resources.image_add_13434878;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateClientName();
            LoadClientIDs();
            LoadAppscodeData();
            rjButton3.Enabled = false;
            rjButton5.Enabled = false;
            rjButton4.Enabled = false;
            rjButton2.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
        }

        private void rjButton3_Click(object sender, EventArgs e)
        {
            UpdateBusinessName(roundedTextbox2.Text.Trim());
            LoadAppscodeData();
            rjButton3.Enabled = false; 
            rjButton5.Enabled = false; 
            rjButton4.Enabled = false; 
            rjButton2.Enabled = false; 
            button1.Enabled = false; 
            button2.Enabled = false;
        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            UpdateClientName(roundedTextbox1.Text.Trim());
            LoadAppscodeData();
            rjButton3.Enabled = false;
            rjButton5.Enabled = false;
            rjButton4.Enabled = false;
            rjButton2.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false; 
        }

        private void rjButton4_Click(object sender, EventArgs e)
        {
            iscodeMatch(roundedTextbox5.Text.Trim());
            LoadAppscodeData();
            rjButton8.Enabled = false;
            rjButton3.Enabled = false;
            rjButton5.Enabled = false;
            rjButton4.Enabled = false;
            rjButton2.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false; 
        }

        private void rjButton5_Click(object sender, EventArgs e)
        {
            UpdatePassword(roundedTextbox4.Text.Trim());
            LoadAppscodeData();
            rjButton3.Enabled = false;
            rjButton5.Enabled = false;
            rjButton4.Enabled = false;
            rjButton2.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false; 
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
        }

        private void rjButton6_Click(object sender, EventArgs e)
        {
            UpdateBrandLogo(pictureBox1.Image);
            rjButton6.Enabled = false;
            rjButton7.Enabled = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DeleteBrandPic();
        }


        #endregion

        private void SaveDraftData()
        {
            // Prepare the draft data with default values if controls are empty
            string draftData = $"{(string.IsNullOrWhiteSpace(roundedTextbox2.Text) ? string.Empty : roundedTextbox2.Text.Trim())}\n" +
                               $"{(string.IsNullOrWhiteSpace(roundedTextbox4.Text) ? string.Empty : roundedTextbox4.Text.Trim())}\n" +
                               $"{(string.IsNullOrWhiteSpace(roundedTextbox3.Text) ? string.Empty : roundedTextbox3.Text.Trim())}\n" +
                               $"{(string.IsNullOrWhiteSpace(textBox1.Text) ? string.Empty : textBox1.Text.Trim())}\n" +
                               $"{(string.IsNullOrWhiteSpace(roundedTextbox1.Text) ? string.Empty : roundedTextbox1.Text.Trim())}";

            // Write the draft data to the file
            File.WriteAllText("draft.dat", draftData);
        }

        private void CheckForChanges()
        {
            if (!File.Exists("draft.dat"))
            {
                SaveDraftData();
            }

            string[] draftData = File.ReadAllLines("draft.dat");

            // Initialize boolean variables with default values
            bool businessNameChanged = false;
            bool passwordChanged = false;
            bool loginChanged = false;
            bool clientNameChanged = false;
            bool clientAddressChanged = false;
            bool clientChoiceChanged = false;

            // Check for changes in each field, ensuring we don't access out of bounds
            if (draftData.Length > 0)
            {
                businessNameChanged = roundedTextbox2.Text.Trim() != draftData[0];
            }
            else
            {
                // Enable if there's new data in the textbox
                businessNameChanged = !string.IsNullOrWhiteSpace(roundedTextbox2.Text);
            }

            if (draftData.Length > 1)
            {
                passwordChanged = roundedTextbox4.Text.Trim() != draftData[1];
            }
            else
            {
                passwordChanged = !string.IsNullOrWhiteSpace(roundedTextbox4.Text);
            }

            if (draftData.Length > 2)
            {
                loginChanged = roundedTextbox3.Text.Trim() != draftData[2];
            }
            else
            {
                loginChanged = !string.IsNullOrWhiteSpace(roundedTextbox3.Text);
            }

            if (draftData.Length > 3)
            {
                clientNameChanged = textBox1.Text.Trim() != draftData[3];
            }
            else
            {
                clientNameChanged = !string.IsNullOrWhiteSpace(textBox1.Text);
            }

            if (draftData.Length > 4)
            {
                clientAddressChanged = roundedTextbox1.Text.Trim() != draftData[4];
            }
            else
            {
                clientAddressChanged = !string.IsNullOrWhiteSpace(roundedTextbox1.Text);
            }

            // Assuming comboBox1 is also compared to the client name
            if (draftData.Length > 3)
            {
                clientChoiceChanged = comboBox1.Text.Trim() != draftData[3];
            }
            else
            {
                clientChoiceChanged = !string.IsNullOrWhiteSpace(comboBox1.Text);
            }

            // Enable or disable buttons based on changes
            rjButton3.Enabled = businessNameChanged; // Update Business Name
            rjButton5.Enabled = passwordChanged; // Update Password
            rjButton4.Enabled = loginChanged; // Update Login
            rjButton2.Enabled = clientAddressChanged; // Update Client Address
            button1.Enabled = clientNameChanged; // Update Client Name
            button2.Enabled = clientChoiceChanged; // Update Client Choice
            rjButton8.Enabled = loginChanged; // Update Login
        }

        private void roundedTextbox2_TextChanged(object sender, EventArgs e)
        {
            CheckForChanges();

            if (string.IsNullOrWhiteSpace(roundedTextbox2.Text)) 
            {
                rjButton3.Enabled = false;
                label12.ForeColor = Color.Tomato;
                label12.Text = "Business name field cannot be empty.";
            }
            else 
            {
                label12.Text = string.Empty;
            }
        }

        private void roundedTextbox4_TextChanged(object sender, EventArgs e)
        {
            CheckForChanges();

            if (string.IsNullOrWhiteSpace(roundedTextbox4.Text))
            {
                rjButton5.Enabled = false;
            }

            string strength = CheckPasswordStrength(roundedTextbox4.Text);

            if (string.IsNullOrWhiteSpace(roundedTextbox4.Text))
            {
                label12.ForeColor = Color.Tomato;
                label12.Text = "Password field cannot be empty.";
            }
            else
            {
                label12.Text = strength;
                label12.ForeColor = Color.Tomato;
            }
        }

        private string CheckPasswordStrength(string password)
        {
            bool hasUpperCase = Regex.IsMatch(password, @"[A-Z]");
            bool hasLowerCase = Regex.IsMatch(password, @"[a-z]");
            bool hasDigits = Regex.IsMatch(password, @"[0-9]");
            bool hasSpecialChars = Regex.IsMatch(password, @"[!@#$%^&*(),.?""{}|<>]");
            bool isLongEnough = password.Length >= 8;

            if (isLongEnough && hasUpperCase && hasLowerCase && hasDigits && hasSpecialChars)
            {
                return " ";
            }
            else if (isLongEnough && (hasUpperCase || hasLowerCase) && (hasDigits || hasSpecialChars))
            {
                rjButton5.Enabled = false;
                return "Moderate password.";
            }
            else
            {
                rjButton5.Enabled = false;
                return "Weak password.";
            }
        }

        private bool IsValidEmail(string email)
        {
            string pattern = @"^(?=.{1,256})(?=.{1,64}@.{1,255}$)(?=.{1,64})(?=.{1,255})" + // Length constraints
                     @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"; // General email pattern

            return Regex.IsMatch(email, pattern);
        }

        private void roundedTextbox3_TextChanged(object sender, EventArgs e)
        {
            CheckForChanges();

            if (string.IsNullOrWhiteSpace(roundedTextbox3.Text))
            {
                rjButton4.Enabled = false;
                rjButton8.Enabled= false;
                label11.ForeColor = Color.Tomato;
                label11.Text = "Email field cannot be empty.";
            }
            else
            {
                label11.Text = string.Empty;
            }

            if (!IsValidEmail(roundedTextbox3.Text.Trim())) 
            {
                rjButton4.Enabled = false;
                rjButton8.Enabled = false;
                label11.ForeColor = Color.Tomato;
                label11.Text = "Invalid Email.";
            }
            else
            {
                label11.Text = string.Empty;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            CheckForChanges();

            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                button1.Enabled = false;
                label12.ForeColor = Color.Tomato;
                label12.Text = "Location name field cannot be empty.";
            }
            else
            {
                label12.Text = string.Empty;
            }
        }

        private void roundedTextbox1_TextChanged(object sender, EventArgs e)
        {
            CheckForChanges();

            if (string.IsNullOrWhiteSpace(roundedTextbox1.Text))
            {
                rjButton2.Enabled = false;
                label12.ForeColor = Color.Tomato;
                label12.Text = "Location address field cannot be empty.";
            }
            else
            {
                label12.Text = string.Empty;
            }
        }

        private void roundedTextbox5_TextChanged(object sender, EventArgs e)
        {
            if (roundedTextbox5.Text.Trim().Length < 6)
            {
                rjButton4.Visible = false;
            }
            else 
            {
                rjButton4.Visible = true;

                if (roundedTextbox5.Text.Trim() == generatedCode)
                {
                    label11.Text = "Match: Now save your email";
                    label11.ForeColor = Color.ForestGreen;  
                }
                else 
                {
                    label11.Text = "Confirmation code mismatch";
                    label11.ForeColor = Color.Tomato;
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckForChanges();
            rjButton8.Enabled = false;
            rjButton5.Enabled = false;        
        }

        private void rjButton8_Click(object sender, EventArgs e)
        {
            if (checkifEmailSent()) 
            {
                label11.Text = "Enter the sent code in the empty field above";
                label11.ForeColor = Color.ForestGreen;
                roundedTextbox5.Visible = true;
            }
            else 
            {
                label11.Text = "Error sending the code. Try again";
                label11.ForeColor = Color.Tomato;
            }
        }

        private void iscodeMatch(string text)
        {
            if (text == generatedCode)
            {
                UpdateLogin(roundedTextbox3.Text.Trim());
            }
            else 
            {
                label11.Text = "Confirmation code mismatch";
                label11.ForeColor = Color.Tomato;
            }
        }

        private bool checkifEmailSent()
        {
            string email = roundedTextbox3.Text.Trim();

            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            else
            {
                string code = GenerateConfirmationCode();

                generatedCode = code;

                return SendConfirmationEmail(email, code);
            }
        }

        private bool SendConfirmationEmail(string email, string confirmationCode)
        {
            if (crudDatabase.IsInternetAvailable())
            {
                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com");

                    mail.From = new MailAddress("murnarmail@gmail.com");
                    mail.To.Add(email);
                    mail.Subject = "Email Confirmation code";

                    string logoUrl = "";

                    mail.IsBodyHtml = true;
                    mail.Body = $@"
                                     <html>
                                     <body>
                                          <div style='text-align: center;'>
                                          <img src='{logoUrl}' alt='Company Logo' style='width: 200px; height: auto;' />
                                          <h2>Email Confirmation Code</h2>
                                          <p>Enter this code in the empty field to confirm your new email:</p>
                                          <h3 style='color: #007BFF;'>{confirmationCode}</h3>
                                          <p style='font-size: 0.9em; color: gray;'>Please do not reply to this email.</p>
                                          </div>
                                          <footer style='text-align: center; margin-top: 20px;'>
                                          <p>Best regards,</p>
                                          <p>MURNAR TECH<br />
                                          Your Curgonova 47 k5<br />
                                          Phone: +375 (80) 25 660-65-20<br />
                                         Email: murnarmail@gmail.com</p>
                                   </footer>
                                   </body>
                                   </html>";

                    smtp.Port = 587; // or your SMTP port
                    smtp.Credentials = new NetworkCredential("murnarmail@gmail.com", "fsnx ooho qqpq giqc");
                    smtp.EnableSsl = true;

                    smtp.Send(mail);

                    return true;
                }
                catch (FormatException)
                {
                    return false;
                }
                catch (SmtpException)
                {
                    return false;
                }
                catch (ArgumentException)
                {
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                MessageBox.Show("No internet ");
                return false;
            }
        }

        private string GenerateConfirmationCode()
        {
            // Generate a random confirmation code
            Random random = new Random();
            return random.Next(100000, 999999).ToString(); // 6-digit code
        }

        private void Alert(string msg, Alertform.enmType type)
        {
            Alertform alertform = new Alertform();
            alertform.showAlert(msg, type);
        }

        private void panel2_Click(object sender, EventArgs e)
        {
            if (clavieroverlay != null && clavieroverlay.Visible == true)
            {
                clavieroverlay.Visible = false;
                label3.Focus();
            }
        }

        private void roundedTextbox2_Enter(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.showKeyboard)
            {
                clavieroverlay = new Clavieroverlay(roundedTextbox2);
                clavieroverlay.boardLocationBottom();
                clavieroverlay.Show();
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.showKeyboard)
            {
                clavieroverlay = new Clavieroverlay(textBox1);
                clavieroverlay.boardLocationBottom();
                clavieroverlay.Show();
            }
        }

        private void roundedTextbox1_Enter(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.showKeyboard)
            {
                clavieroverlay = new Clavieroverlay(roundedTextbox1);
                clavieroverlay.boardLocationBottom();
                clavieroverlay.Show();
            }
        }

        private void roundedTextbox4_Enter(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.showKeyboard)
            {
                clavieroverlay = new Clavieroverlay(roundedTextbox4);
                clavieroverlay.boardLocationBottom();
                clavieroverlay.Show();
            }
        }

        private void roundedTextbox3_Enter(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.showKeyboard)
            {
                clavieroverlay = new Clavieroverlay(roundedTextbox3);
                clavieroverlay.boardLocationTop();
                clavieroverlay.Show();
            }
        }

        private void roundedTextbox5_Enter(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.showKeyboard)
            {
                clavieroverlay = new Clavieroverlay(roundedTextbox5);
                clavieroverlay.boardLocationTop();
                clavieroverlay.Show();
            }
        }

        private void roundedTextbox2_Leave(object sender, EventArgs e)
        {
            clavieroverlay?.Hide();
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            clavieroverlay?.Hide();
        }

        private void roundedTextbox1_Leave(object sender, EventArgs e)
        {
            clavieroverlay?.Hide();
        }

        private void roundedTextbox4_Leave(object sender, EventArgs e)
        {
            clavieroverlay?.Hide();
        }

        private void roundedTextbox3_Leave(object sender, EventArgs e)
        {
            clavieroverlay?.Hide();
        }

        private void roundedTextbox5_Leave(object sender, EventArgs e)
        {
            clavieroverlay?.Hide();
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        public void LocalizeControls()
        {
            label1.Text = LanguageManager.Instance.GetString("SETACC-lbl1");
            label3.Text = LanguageManager.Instance.GetString("SETACC-lbl3");
            label5.Text = LanguageManager.Instance.GetString("SETACC-lbl5");
            label6.Text = LanguageManager.Instance.GetString("SETACC-lbl6");
            label7.Text = LanguageManager.Instance.GetString("SETACC-lbl7");
            label8.Text = LanguageManager.Instance.GetString("SETACC-lbl8");
            label9.Text = LanguageManager.Instance.GetString("SETACC-lbl9");
            label10.Text = LanguageManager.Instance.GetString("SETACC-lbl10");
            button2.Text = LanguageManager.Instance.GetString("SETACC-btn2");
            rjButton1.Text = LanguageManager.Instance.GetString("SETACC-rbtn1");
            button1.Text = LanguageManager.Instance.GetString("SETACC-btnsACH");
            rjButton2.Text = LanguageManager.Instance.GetString("SETACC-btnsACH");
            rjButton3.Text = LanguageManager.Instance.GetString("SETACC-btnsACH");
            rjButton4.Text = LanguageManager.Instance.GetString("SETACC-btnsACH");
            rjButton5.Text = LanguageManager.Instance.GetString("SETACC-btnsACH");
            rjButton6.Text = LanguageManager.Instance.GetString("SETACC-rbtn6");
            rjButton7.Text = LanguageManager.Instance.GetString("SETACC-rbtn7");
            rjButton8.Text = LanguageManager.Instance.GetString("SETACC-rbtn8");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            panel1.BackColor = colors.Color3;
            panel2.BackColor = colors.Color3;
            comboBox1.BackColor = colors.Color1;
            comboBox1.BorderColor = colors.Color5;
            roundedPanel1.BackColor = colors.Color3;
            textBox1.BackColorRounded = colors.Color1;          
            comboBox1.BorderFocusColor = colors.Color5;

            label1.ForeColor = this.ForeColor;
            label3.ForeColor = this.ForeColor;
            label4.ForeColor = this.ForeColor;
            label5.ForeColor = this.ForeColor;
            label6.ForeColor = this.ForeColor;
            label7.ForeColor = this.ForeColor;
            label8.ForeColor = this.ForeColor;
            label9.ForeColor = this.ForeColor;
            label10.ForeColor = this.ForeColor;
            label11.ForeColor = this.ForeColor;
            label12.ForeColor = this.ForeColor;
            textBox1.ForeColor = this.ForeColor;
            comboBox1.ForeColor = this.ForeColor;
            comboBox1.ArrowColor = this.ForeColor;
            roundedTextbox1.ForeColor = this.ForeColor;
            roundedTextbox2.ForeColor = this.ForeColor;
            roundedTextbox3.ForeColor = this.ForeColor;
            roundedTextbox4.ForeColor = this.ForeColor;
            roundedTextbox5.ForeColor = this.ForeColor;           
            rjButton1.BorderColor = rjButton1.BackColor;
            roundedTextbox1.BackColorRounded = colors.Color1;
            roundedTextbox2.BackColorRounded = colors.Color1;
            roundedTextbox3.BackColorRounded = colors.Color1;
            roundedTextbox4.BackColorRounded = colors.Color1;
            roundedTextbox5.BackColorRounded = colors.Color1;
        }
    }

    public class Client
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}    

