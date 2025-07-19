using System;
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
using PadTai.Classes.Others;
using System.Threading.Tasks;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using PadTai.Sec_daryfolders.App_DBInitializer;
using PadTai.Sec_daryfolders.Others_Forms;


namespace PadTai.Sec_daryfolders.DB_Appinitialize
{
    public partial class Databasecreate : Form
    {
        private DraggableForm draggableForm;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private string confirmationCode; 
        private ControlResizer resizer;
        private string lastSentEmail;

        public Databasecreate()
        {
            InitializeComponent();

            ApplyTheme();
            LoadThemes();
            LoadSelectedTheme();
            ThemeManager.ThemeChanged += ApplyTheme;

            rjButton5.Enabled = false;
            rjButton6.Enabled = false;
            textBox1.Enabled = false;
            crudDatabase = new CrudDatabase();
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);
            draggableForm = new DraggableForm();
            draggableForm.EnableDragging(this, this);


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
            resizer.RegisterControl(label11);
            resizer.RegisterControl(label12);
            resizer.RegisterControl(Emailbox);
            resizer.RegisterControl(textBox1);
            resizer.RegisterControl(rjButton1);
            resizer.RegisterControl(rjButton2);
            resizer.RegisterControl(rjButton3);
            resizer.RegisterControl(rjButton5);
            resizer.RegisterControl(rjButton6);
            resizer.RegisterControl(rjButton7);
            resizer.RegisterControl(linkLabel1);
            resizer.RegisterControl(pictureBox1);
            resizer.RegisterControl(pictureBox2);
            resizer.RegisterControl(pictureBox3);
            resizer.RegisterControl(pictureBox4);
            resizer.RegisterControl(Passwordbox);
            resizer.RegisterControl(roundedPanel1);
            resizer.RegisterControl(roundedPanel2);
            resizer.RegisterControl(roundedPanel3);
            resizer.RegisterControl(roundedPanel4);
            resizer.RegisterControl(roundedPanel5);
            resizer.RegisterControl(Databasenamebox);

            PopulateLanguageComboBox();
            LoadCurrentLanguage();
            LocalizeControls();

            LanguageManager.Instance.LanguageChanged += HandleLanguageChange;
            customBox1.SelectedIndexChanged += customBox1_SelectedIndexChanged;
            LanguageManager.Instance.LanguageChanged += HandleControlLanguageChange;
        }

        string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Localdatabase.db");

        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png); // Save as PNG or any other format
                return ms.ToArray();
            }
        }

        public void CreateSQLiteTables(SQLiteConnection connection)
        {
            string[] sqlCommands = new string[]
            {
                 @"CREATE TABLE IF NOT EXISTS Appaccesscode 
                 (
                    Businessname TEXT NOT NULL,                        
                    Password TEXT NOT NULL,
                    Login TEXT NOT NULL,
                    Brandlogo BLOB NULL,
                    Tableplanpic BLOB NULL
                );",


                @"CREATE TABLE IF NOT EXISTS Clients 
                (
                    ClientID INTEGER PRIMARY KEY,                        
                    ClientName TEXT NOT NULL,
                    Clientadress TEXT NOT NULL
                );",


                @"CREATE TABLE IF NOT EXISTS Delivery 
                (
                    DeliveryID INTEGER PRIMARY KEY,
                    Thedelivery TEXT NOT NULL
                );",


                @"CREATE TABLE IF NOT EXISTS Tablenumber 
                (
                    TableID INTEGER PRIMARY KEY,
                    Thetablenumber TEXT NOT NULL,
                    Seatsamount INTEGER,
                    PositionX INTEGER NOT NULL ,
                    PositionY INTEGER NOT NULL ,
                    Width INTEGER NOT NULL,
                    Height INTEGER NOT NULL,
                    IsAvailable INTEGER NOT NULL
                );",


                @"CREATE TABLE IF NOT EXISTS Employees 
                (
                    ID INTEGER PRIMARY KEY,                        
                    PersonalType TEXT NOT NULL,       
                    Gender TEXT NOT NULL,             
                    Contact TEXT NOT NULL,            
                    Name TEXT NOT NULL,              
                    Password TEXT,          
                    Picture BLOB NULL,   
                    Salary REAL NULL,
                    Reward REAL NULL,
                    Penalty REAL NULL,
                    ClientID INTEGER NOT NULL,                     
                    DateOfStarting DATETIME NOT NULL,         
                    FOREIGN KEY (ClientID) REFERENCES Clients(ClientID) 
                );",


                @"CREATE TABLE IF NOT EXISTS Employeelogin 
                (
                    ID INTEGER PRIMARY KEY,
                    EmployeeID INTEGER NOT NULL,
                    PersonalType TEXT NOT NULL,       
                    UserName TEXT NOT NULL,
                    Thedaydate DATE NOT NULL,         
                    TimeOfStarting TIME NOT NULL,
                    TimeOfStoping TIME,
                    Contact TEXT NOT NULL,            
                    ClientID INTEGER NOT NULL,   
                    FOREIGN KEY (ID) REFERENCES Employees(ID) ON DELETE CASCADE
                );",


                @"CREATE TABLE IF NOT EXISTS Reservetable 
                (
                    ReservationID INTEGER NOT NULL,                        
                    Guestgender TEXT NOT NULL,             
                    GuestContact TEXT NOT NULL,            
                    GuestName TEXT NOT NULL,  
                    Guesttime TEXT NOT NULL,         
                    Ordertime DATETIME NOT NULL,  
                    Clientsqte TEXT,
                    Tablenumber TEXT NOT NULL,
                    Cancelled TEXT,
                    Completed TEXT,
                    ClientID INTEGER NOT NULL,                     
                    FOREIGN KEY (ClientID) REFERENCES Clients(ClientID),
                    PRIMARY KEY (ClientID, ReservationID),
                    CHECK (NOT (Cancelled IS NOT NULL AND Completed IS NOT NULL))
                );",

                @"CREATE TABLE IF NOT EXISTS Tabletcount 
                (
                    ClientID INTEGER PRIMARY KEY,
                    LastReserveId INTEGER NOT NULL
                );",

                @"CREATE TABLE GrossProducts 
                (
                    ProductID INTEGER PRIMARY KEY,
                    ProductName TEXT NOT NULL,
                    ProductPrice REAL NOT NULL
                );",

                @"CREATE TABLE IngredientMap 
                (
                    MapID INTEGER PRIMARY KEY,
                    DishID INTEGER,
                    ProductID INTEGER,
                    AmountInGrams REAL,
                    FOREIGN KEY (DishID) REFERENCES FoodItems(FoodID) ON DELETE CASCADE,
                    FOREIGN KEY (ProductID) REFERENCES GrossProducts(ProductID) ON DELETE CASCADE,
                    UNIQUE (DishID, ProductID) 
                );",

                 @"CREATE TABLE TimetableMap 
                 (
                    MapID INTEGER PRIMARY KEY,
                    EmployeeId INTEGER,
                    Monthday INTEGER,
                    WorkHours REAL,
                    FOREIGN KEY (EmployeeId) REFERENCES Employees(ID) ON DELETE CASCADE,
                    UNIQUE (EmployeeId, Monthday)
                 );",

                @"CREATE TABLE IF NOT EXISTS FoodItems 
                (
                    FoodID INTEGER PRIMARY KEY,
                    FoodName TEXT NOT NULL,
                    FooditemtypeID INTEGER,
                    Price REAL NOT NULL,
                    GroupID INTEGER,
                    SubgroupID INTEGER,
                    SubsubgroupID INTEGER,
                    FoodPicture BLOB NULL,
                    IsChecked INTEGER, 
                    FOREIGN KEY (GroupID) REFERENCES Groups(GroupID) ON DELETE SET NULL,
                    FOREIGN KEY (SubgroupID) REFERENCES Subgroups(SubgroupID) ON DELETE SET NULL,
                    FOREIGN KEY (SubsubgroupID) REFERENCES Subsubgroups(SubsubgroupID) ON DELETE SET NULL
               );",


                @"CREATE TABLE IF NOT EXISTS FoodItemsTypes 
                (
                    FooditemtypeID INTEGER PRIMARY KEY,
                    FooditemtypeName TEXT NOT NULL
                );",


                @"CREATE TABLE IF NOT EXISTS Groups 
                (
                    GroupID INTEGER PRIMARY KEY,
                    GroupName TEXT NOT NULL
                );",


                @"CREATE TABLE IF NOT EXISTS Subgroups 
                (
                    SubgroupID INTEGER PRIMARY KEY,
                    SubgroupName TEXT NOT NULL,
                    GroupID INTEGER,
                    FOREIGN KEY(GroupID) REFERENCES Groups(GroupID) ON DELETE SET NULL
                );",


                @"CREATE TABLE IF NOT EXISTS Subsubgroups 
                (
                    SubsubgroupID INTEGER PRIMARY KEY,
                    SubsubgroupName TEXT NOT NULL,
                    SubgroupID INTEGER,
                    FOREIGN KEY(SubgroupID) REFERENCES Subgroups(SubgroupID) ON DELETE SET NULL
                );",


                @"CREATE TABLE IndividualDiscounts (
                     FoodID INTEGER PRIMARY KEY,
                     DiscountPercentage INTEGER NOT NULL,
                     OccurrencesRequired INTEGER NOT NULL,
                     FOREIGN KEY (FoodID) REFERENCES FoodItems(FoodID) ON DELETE CASCADE
                );",


                @"CREATE TABLE IF NOT EXISTS PaymentGroups 
                (
                    PaymentgroupID INTEGER PRIMARY KEY,
                    PaymentGroupName TEXT NOT NULL
                );",


                @"CREATE TABLE IF NOT EXISTS PaymentTypes 
                (
                    PaymentypeID INTEGER PRIMARY KEY,
                    PaymenttypeName TEXT NOT NULL,
                    PaymentgroupID INTEGER,
                    FOREIGN KEY (PaymentgroupID) REFERENCES PaymentGroups(PaymentgroupID) ON DELETE SET NULL
                );",


                @"CREATE TABLE IF NOT EXISTS OrderTypes 
                (
                    OrdertypeID INTEGER PRIMARY KEY,
                    OrdertypeName TEXT NOT NULL
                );",


                @"CREATE TABLE IF NOT EXISTS Discounts 
                (
                    DiscountID INTEGER PRIMARY KEY,
                    Thediscount TEXT NOT NULL
                );",


                @"CREATE TABLE IF NOT EXISTS Receipts 
                (
                    ReceiptId INTEGER NOT NULL,
                    FoodName TEXT NOT NULL,
                    Foodprice TEXT NOT NULL,
                    Quantity TEXT,
                    OrderDateTime DATETIME NOT NULL,
                    PaymenttypeName TEXT NOT NULL,
                    PlacetoEatName TEXT NOT NULL,
                    Thediscount TEXT NOT NULL,
                    TotalPrice REAL NOT NULL,
                    FooditemtypeID TEXT NOT NULL,
                    FoodID TEXT NOT NULL,
                    Ordertable TEXT, 
                    BuyerID TEXT,
                    Ordertype TEXT,
                    TaxedAmount TEXT,
                    BuyerDiscount TEXT,    
                    Commentary TEXT,
                    ClientID INTEGER NOT NULL,     
                    FOREIGN KEY (ClientID) REFERENCES Clients(ClientID),
                    PRIMARY KEY (ClientID, ReceiptId)
                );",


                @"CREATE TABLE IF NOT EXISTS Receiptcount 
                (
                    ClientID INTEGER PRIMARY KEY,
                    LastReceiptId INTEGER NOT NULL
                );",

                @"CREATE TABLE Userdata 
                (
                    UserID INTEGER PRIMARY KEY,
                    Username TEXT NOT NULL,
                    Secondname TEXT,
                    Phonenumber TEXT NOT NULL,
                    Email TEXT,
                    Profilepicture BLOB,
                    Gender TEXT,
                    Birthday DATE
                );",

                @"CREATE TABLE IF NOT EXISTS Weborders 
                (
                    WebreceiptID INTEGER NOT NULL,
                    CustomerName TEXT NOT NULL,
                    Contact TEXT NOT NULL,
                    OrderTime DATETIME NOT NULL,
                    PreparationTime DATETIME NOT NULL,
                    DeliveryAddress TEXT NOT NULL,
                    DeliveryType TEXT NOT NULL,
                    Orderstatus TEXT NOT NULL,
                    Ordertype TEXT NOT NULL,
                    ClientID INTEGER NOT NULL
                );",


                @"CREATE TABLE IF NOT EXISTS ReceiptsArchive 
                (
                    ReceiptId INTEGER NOT NULL,
                    FoodName TEXT NOT NULL,
                    Foodprice TEXT NOT NULL,
                    Quantity TEXT,
                    OrderDateTime DATETIME NOT NULL,
                    PaymenttypeName TEXT NOT NULL,
                    PlacetoEatName TEXT NOT NULL,
                    Thediscount TEXT NOT NULL,
                    TotalPrice REAL NOT NULL,
                    FooditemtypeID TEXT NOT NULL,
                    FoodID TEXT NOT NULL,
                    Ordertable TEXT, 
                    BuyerID TEXT,
                    Ordertype TEXT,
                    TaxedAmount TEXT,
                    BuyerDiscount TEXT,
                    Commentary TEXT,
                    ClientID INTEGER NOT NULL,    
                    FOREIGN KEY (ClientID) REFERENCES Clients(ClientID),
                    FOREIGN KEY (ReceiptID) REFERENCES Receipts(ReceiptID)
                );"
            };


            foreach (var command in sqlCommands)
            {
                using (var cmd = new SQLiteCommand(command, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }

 
           string createIndexSql = @" CREATE UNIQUE INDEX UQ_Tablenumber_Active ON Reservetable (Tablenumber) 
                                      WHERE Cancelled IS NULL AND Completed IS NULL;";

            using (var indexcmd = new SQLiteCommand(createIndexSql, connection))
            {
                try
                {
                    indexcmd.ExecuteNonQuery();
                }
                catch 
                {
                   label12.Text = LanguageManager.Instance.GetString("DC-IndError");
                }
            }
        }
       

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void Databasecreate_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void Databasecreate_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
            byte[] picture = null;

            if (pictureBox1.Image != null && !crudDatabase.IsSpecificImage(pictureBox1.Image, Properties.Resources.photo))
            {
                picture = ImageToByteArray(pictureBox1.Image);
            }

            string Businessname = Databasenamebox.Text.Trim();
            string Password = Passwordbox.Text.Trim();
            string Login = Emailbox.Text.Trim();

            if (crudDatabase.IsInternetAvailable())
            {
                try
                {
                    if (string.IsNullOrEmpty(Businessname))
                    {
                        label12.Text = LanguageManager.Instance.GetString("DC-try11");
                        label12.ForeColor = Color.Tomato;
                    }
                    else if (string.IsNullOrEmpty(Password))
                    {
                        label12.Text = LanguageManager.Instance.GetString("DC-try12");
                        label12.ForeColor = Color.Tomato;
                    }
                    else if (string.IsNullOrEmpty(Login))
                    {
                        label12.Text = LanguageManager.Instance.GetString("DC-try13");
                        label12.ForeColor = Color.Tomato;
                    }
                    else if (!string.IsNullOrEmpty(Login) && Login != lastSentEmail)
                    {
                        label12.Text = LanguageManager.Instance.GetString("DC-try14");
                    }
                    else if (File.Exists(dbPath))
                    {
                        using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                        {
                            connection.Open();

                            // 1. Insert into Appaccesscode
                            string insertCommand = @"INSERT INTO Appaccesscode (Businessname, Login, Password, Brandlogo) VALUES (@Businessname, @Login, @Password, @Picture);";

                            using (var insertCmd = new SQLiteCommand(insertCommand, connection))
                            {
                                insertCmd.Parameters.AddWithValue("@Businessname", Businessname);
                                insertCmd.Parameters.AddWithValue("@Password", Password);
                                insertCmd.Parameters.AddWithValue("@Login", Login);

                                if (picture != null && picture.Length > 0)
                                {
                                    insertCmd.Parameters.AddWithValue("@Picture", picture);
                                }
                                else
                                {
                                    insertCmd.Parameters.AddWithValue("@Picture", DBNull.Value);
                                }
                                try
                                {
                                    insertCmd.ExecuteNonQuery();
                                    label12.Text = LanguageManager.Instance.GetString("DC-try15");
                                    label12.ForeColor = Color.LawnGreen;
                                    rjButton2.Enabled = true;                                    
                                    rjButton7.Visible = true;

                                    // After the form is closed, set the flag to true
                                    Properties.Settings.Default.AccountFormShown = true;
                                    Properties.Settings.Default.Save(); // Save the setting
                                }
                                catch (Exception ex)
                                {
                                    label12.Text = LanguageManager.Instance.GetString("DC-try16") + $"{ex.Message}";
                                    return; 
                                }
                            }
                        }
                    }
                    else
                    {
                        label12.Text = LanguageManager.Instance.GetString("DC-try17");
                        label12.ForeColor = Color.Tomato;
                    }

                }
                catch (Exception ex)
                {
                    label12.Text = LanguageManager.Instance.GetString("DC-try16") + $" {ex.Message}";
                }
            }
            else
            {
                label12.Text = LanguageManager.Instance.GetString("Nointernet");
            }
        }

        private void rjButton2_Click(object sender, EventArgs e)
        {
            if (File.Exists(dbPath))
            {
                this.Hide();
                Foodgroupsinsert foodgroupsinsert = new Foodgroupsinsert();
                foodgroupsinsert.ShowDialog();
            }
            else 
            {
                label12.Text = LanguageManager.Instance.GetString("DC-try18");
            }
        }

        private void rjButton3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Image Files (*.jpg, *.jpeg, *.png, *.gif)|*.jpg;*.jpeg;*.png;*.gif";
            openFileDialog1.Title = LanguageManager.Instance.GetString("Sel-image");

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Get the image file path
                string imagePath = openFileDialog1.FileName;

                // Load the image into the PictureBox
                pictureBox1.Image = Image.FromFile(imagePath);
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if(Passwordbox.UseSystemPasswordChar == false) 
            {
                Passwordbox.UseSystemPasswordChar = true;
                pictureBox5.Image = Properties.Resources.eye;
            }
            else 
            {
                Passwordbox.UseSystemPasswordChar = false;
                pictureBox5.Image = Properties.Resources.invisible;
            }
        }

        private void HandleLanguageChange()
        {
            LoadCurrentLanguage();
        }

        private void PopulateLanguageComboBox()
        {
            customBox1.Items.Add("English");
            customBox1.Items.Add("Français");
            customBox1.Items.Add("Русский");
        }

        private void LoadCurrentLanguage()
        {
            string currentLanguage = LanguageManager.Instance.CurrentCulture.TwoLetterISOLanguageName;

            if (customBox1.Items.Count == 0)
            {
                return; 
            }

            switch (currentLanguage)
            {
                case "en":
                    customBox1.SelectedItem = "English";
                    break;
                case "fr":
                    customBox1.SelectedItem = "Français";
                    break;
                case "ru":
                    customBox1.SelectedItem = "Русский";
                    break;
                default:
                    customBox1.SelectedItem = "English"; 
                    break;
            }
        }   

        private void LoadThemes()
        {
            customBox2.Items.Add("Blue"); 
            customBox2.Items.Add("Light");
            customBox2.Items.Add("Dark");
        }

        private void LoadSelectedTheme()
        {
            string selectedTheme = Properties.Settings.Default.SelectedTheme;
            customBox2.SelectedItem = selectedTheme ?? "Light";
        }

        private void customBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTheme = customBox2.SelectedItem.ToString();
            ThemeManager.SwitchTheme(selectedTheme);
            ThemeManager.SaveTheme(selectedTheme);
        }

        private void customBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void customBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void customBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Save the selected language when the user changes the selection
            string selectedLanguage = customBox1.SelectedItem.ToString();
            string cultureCode;

            if (selectedLanguage == "English")
            {
                cultureCode = "en";
            }
            else if (selectedLanguage == "Français")
            {
                cultureCode = "fr";
            }
            else if (selectedLanguage == "Русский")
            {
                cultureCode = "ru";
            }
            else
            {
                cultureCode = "en"; // Default to English
            }

            LanguageManager.Instance.ChangeLanguage(cultureCode);
            // MessageBox.Show("Language changed to " + selectedLanguage);
        }

        private string GenerateConfirmationCode()
        {
                // Generate a random confirmation code
                Random random = new Random();
                return random.Next(100000, 999999).ToString(); // 6-digit code
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
                    mail.Subject = LanguageManager.Instance.GetString("Email-confirm");

                    string logoUrl = "";
                    mail.IsBodyHtml = true;
                    mail.Body = $@"
                                     <html>
                                     <body>
                                          <div style='text-align: center;'>
                                          <img src='{logoUrl}' alt='' style='width: 200px; height: auto;' />
                                          <h2>{LanguageManager.Instance.GetString("Email-confirm")}</h2>
                                          <p>{LanguageManager.Instance.GetString("Email-conftext")}:</p>
                                          <h3 style='color: #007BFF;'>{confirmationCode}</h3>
                                          <p style='font-size: 0.9em; color: gray;'> {LanguageManager.Instance.GetString("Email-noreply")} </p>
                                          </div>
                                          <footer style='text-align: center; margin-top: 20px;'>
                                          <p>{LanguageManager.Instance.GetString("Bestregards")},</p>
                                          <p>MURNAR TECH<br />
                                          Curgonova 47 k5<br />
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
                catch (Exception ex)
                {
                    MessageBox.Show(LanguageManager.Instance.GetString("DC-try16") + $" {ex.Message}");
                    return false;
                }
            }
            else
            {
                label12.Text = LanguageManager.Instance.GetString("Nointernet");
                return false; // Ensure a return value                             
            }
        }

       

        private void rjButton6_Click(object sender, EventArgs e)
        {
            string email = Emailbox.Text;
            confirmationCode = GenerateConfirmationCode(); // Generate and store the code

            if (SendConfirmationEmail(email, confirmationCode))
            {
                label12.Text = LanguageManager.Instance.GetString("DC-try19");
                label12.ForeColor = System.Drawing.Color.Green;
                lastSentEmail = confirmationCode;
                textBox1.Enabled = true;
            }
            else
            {
                label12.Text = LanguageManager.Instance.GetString("DC-try20");
                label12.ForeColor = System.Drawing.Color.Tomato;
                textBox1.Enabled = true;
            }
        }

        private void rjButton5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                label12.Text = LanguageManager.Instance.GetString("DC-try22");
                label12.ForeColor = System.Drawing.Color.Tomato;
            }
            else if (string.IsNullOrEmpty(lastSentEmail))
            {
                label12.Text = LanguageManager.Instance.GetString("DC-try14");
                label12.ForeColor = System.Drawing.Color.Tomato;
            }
            else if (textBox1.Text != lastSentEmail) 
            {
                label12.Text = LanguageManager.Instance.GetString("DC-try23");
                label12.ForeColor = System.Drawing.Color.Tomato;
            }
            else 
            {
                label12.Text = LanguageManager.Instance.GetString("DC-try24");
                label12.ForeColor = System.Drawing.Color.Green;
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
                return LanguageManager.Instance.GetString("M-password");
            }
            else
            {
                return LanguageManager.Instance.GetString("W-password");
            }
        }

        private void Passwordbox_TextChanged(object sender, EventArgs e)
        {
            string password = Passwordbox.Text;
            string strength = CheckPasswordStrength(password);

            if (string.IsNullOrWhiteSpace(password))
            {
                label12.Text = LanguageManager.Instance.GetString("Empty-password");
                label12.ForeColor = Color.Tomato;
            }
            else 
            {
                label12.Text = strength;
                label12.ForeColor = Color.Tomato;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(textBox1.Text.Length < 6) 
            {
                label12.Text = LanguageManager.Instance.GetString("DC-try21");
                label12.ForeColor = System.Drawing.Color.Tomato;
                rjButton5.Enabled = false;

            }
            else if (textBox1.Text.Length > 6)
            {
                label12.Text = LanguageManager.Instance.GetString("DC-try21");
                label12.ForeColor = System.Drawing.Color.Tomato;
                rjButton5.Enabled = false;
            }
            else 
            {
                rjButton5.Enabled = true;
                label12.Text = "";
            }
        }

        private bool IsValidEmail(string email)
        {
            // Regular expression for validating an Email
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        private void Emailbox_TextChanged(object sender, EventArgs e)
        {
            string Login = Emailbox.Text.Trim();

            if (string.IsNullOrWhiteSpace(Login))
            {
                label12.Text = LanguageManager.Instance.GetString("Empty-email");
                label12.ForeColor = Color.Tomato;
                rjButton5.Enabled = false;
                rjButton6.Enabled = false;
            }
            else if (!IsValidEmail(Login))
            {
                label12.Text = LanguageManager.Instance.GetString("Inv-Email");
                label12.ForeColor = Color.Tomato;
                rjButton5.Enabled = false;
                rjButton6.Enabled = false;
            }
            else 
            {
                label12.Text = " ";
                rjButton6.Enabled = true;
            }
        }

        private void rjButton7_Click(object sender, EventArgs e)
        {
            if (File.Exists(dbPath))
            {
                try
                {
                    string query = "DELETE FROM Appaccesscode;";

                    if (crudDatabase.ExecuteNonQuery(query))
                    {
                        label12.Text = LanguageManager.Instance.GetString("Del-account");
                        label12.ForeColor = Color.LawnGreen;
                        lastSentEmail = string.Empty;
                        rjButton2.Enabled = false;
                        rjButton7.Visible = false;
                    }
                    else
                    {
                        label12.Text = LanguageManager.Instance.GetString("ERRORDel-account");
                    }

                    Properties.Settings.Default.AccountFormShown = false;
                    Properties.Settings.Default.Save(); // Save the setting
                }
                catch (Exception ex)
                {
                   label12.Text = ex.Message;
                   label12.ForeColor = Color.Red;
                }
            }
            else
            {
                label12.Text = LanguageManager.Instance.GetString("Noaccountdel");    
                label12.ForeColor = Color.Tomato;
            }
        }

        private void HandleControlLanguageChange()
        {
            LocalizeControls();
        }

        public void LocalizeControls()
        {
             label1.Text = LanguageManager.Instance.GetString("DC-lbl1");
             label2.Text = LanguageManager.Instance.GetString("DC-lbl2");
             label3.Text = LanguageManager.Instance.GetString("DC-lbl3");
             label6.Text = LanguageManager.Instance.GetString("DC-lbl6");
             label7.Text = LanguageManager.Instance.GetString("DC-lbl7");
             label8.Text = LanguageManager.Instance.GetString("DC-lbl8");
             label9.Text = LanguageManager.Instance.GetString("DC-lbl9");
             label11.Text = LanguageManager.Instance.GetString("DC-lbl11");
             rjButton1.Text = LanguageManager.Instance.GetString("DC-rbtn1");
             rjButton2.Text = LanguageManager.Instance.GetString("DC-rbtn2");
             rjButton5.Text = LanguageManager.Instance.GetString("DC-rbtn5");
             rjButton6.Text = LanguageManager.Instance.GetString("DC-rbtn6");            
             rjButton7.Text = LanguageManager.Instance.GetString("DC-rbtn7");            
             rjButton3.Text = LanguageManager.Instance.GetString("Btn-close");
             linkLabel1.Text = LanguageManager.Instance.GetString("DC-linkbl1");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            label1.ForeColor = colors.Color2;
            label2.ForeColor = colors.Color2;
            label3.ForeColor = colors.Color2;
            label6.ForeColor = colors.Color2;
            label8.ForeColor = colors.Color2;
            label9.ForeColor = colors.Color2;
            label11.ForeColor = colors.Color2;
            textBox1.ForeColor = colors.Color2;
            textBox1.BackColor = colors.Color3;
            Emailbox.ForeColor = colors.Color2;
            Emailbox.BackColor = colors.Color3;
            customBox1.ForeColor = colors.Color2;
            customBox2.ForeColor = colors.Color2;
            customBox1.BackColor = colors.Color3;
            customBox2.BackColor = colors.Color3;           
            customBox1.ArrowColor = colors.Color2;
            customBox2.ArrowColor = colors.Color2;           
            Passwordbox.ForeColor = colors.Color2;
            Passwordbox.BackColor = colors.Color3;
            roundedPanel3.BackColor = colors.Color3;
            roundedPanel4.BackColor = colors.Color3;
            roundedPanel5.BackColor = colors.Color3;
            Databasenamebox.ForeColor = colors.Color2;
            Databasenamebox.BackColor = colors.Color3;
            roundedPanel1.GradientTopColor = colors.Color1;
            roundedPanel2.GradientTopColor = colors.Color1;
            roundedPanel1.GradientBottomColor = colors.Color3;
            roundedPanel2.GradientBottomColor = colors.Color3;        
        }
    }    
}

