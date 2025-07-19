using System;
using System.Net;
using System.Data;
using PadTai.Classes;
using System.Drawing;
using System.Net.Mail;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Data.SqlClient;
using PadTai.Classes.Others;
using PadTai.Classes.Databaselink;
using System.Net.NetworkInformation;
using PadTai.Classes.Controlsdesign;
using PadTai.Sec_daryfolders.Others;
using PadTai.Sec_daryfolders.Others_Forms;
using System.Collections.Generic;


namespace PadTai
{
    public partial class Login : Form
    {
        private bool isForgotPasswordPage = true;
        private Clavieroverlay clavieroverlay;
        private DraggableForm draggableForm;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;
        private string generatedCode;
        private MainPage mainPage;

        public Login(MainPage Page)
        {
            InitializeComponent();
            InitializeControlResizer();
            crudDatabase = new CrudDatabase();
            PasswordText.UseSystemPasswordChar = true;

            this.mainPage = Page;
            LocalizeControls();
            ApplyTheme();
        }

        private void InitializeControlResizer()
        {
            label1.Visible = false;
            UsernameText.Visible = false;

            draggableForm = new DraggableForm();
            draggableForm.EnableDragging(this, this);
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);
            resizer.RegisterControl(label1);
            resizer.RegisterControl(label2);
            resizer.RegisterControl(label3);
            resizer.RegisterControl(label4);
            resizer.RegisterControl(label5);
            resizer.RegisterControl(button13);
            resizer.RegisterControl(button14);
            resizer.RegisterControl(checkBox1);
            resizer.RegisterControl(UsernameText);
            resizer.RegisterControl(PasswordText);
        }


        private bool ValidateUser(string password)
        {
            string query = "SELECT COUNT(*) AS UserCount FROM Appaccesscode WHERE Password = @password";

            var parameters = new Dictionary<string, object>
            {
                { "@password", password }
            };

            DataTable resultTable = crudDatabase.FetchDataFromDatabase(query, parameters);

            if (resultTable != null && resultTable.Rows.Count > 0)
            {
                int count = Convert.ToInt32(resultTable.Rows[0]["UserCount"]);
                return count > 0;
            }

            return false;
        }

        private void tryValidateData()
        {
            string password = PasswordText.Text.Trim();

            if (ValidateUser(password))
            {
                Properties.Settings.Default.isMangerMode = true;
                Properties.Settings.Default.Save();
                mainPage.EnablePanel();
                this.Close();

            }
            else if (UsernameText.Text == string.Empty)
            {
                this.Alert("No Username", Alertform.enmType.Warning);
            }
            else
            {
                MessageBox.Show("Incorrect Username or Password", "Error!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            clavieroverlay?.Hide();
        }

        private void Alert(string msg, Alertform.enmType type)
        {
            Alertform alertform = new Alertform();
            alertform.showAlert(msg, type);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (!isForgotPasswordPage)
            {
                iscodeMatch(PasswordText.Text.Trim());
            }
            else if (isForgotPasswordPage)
            {
                tryValidateData();
            }
        }

        private void Clavieroverlay_EnterButtonClicked(object sender, EventArgs e)
        {
            if (!isForgotPasswordPage)
            {
                iscodeMatch(PasswordText.Text.Trim());
            }
            else if (isForgotPasswordPage)
            {
                tryValidateData();
            }
        }


        private void iscodeMatch(string text) 
        {
            if (text == generatedCode)
            {
                Properties.Settings.Default.isMangerMode = true;
                Properties.Settings.Default.Save();
                mainPage.EnablePanel();
                this.Close();
            }
            else if(string.IsNullOrEmpty(text))
            {
                this.Alert("Enter the code", Alertform.enmType.Warning);
            }
            else if(text.Length < 6)
            {
                this.Alert("6 digits code excepted", Alertform.enmType.Warning);
            }
            else
            {
                this.Alert("Confirmation code \n mismatch", Alertform.enmType.Warning);
            }
            clavieroverlay?.Hide();
        }

        private bool checkifEmailSent()
        {
            string email = fetchEmail(); 

            // Check if the email is null or empty
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            else 
            {
                UsernameText.Text = email;  
            }

            string code = GenerateConfirmationCode();

            UsernameText.Text = $"  {email}";
            generatedCode = code;

            return SendConfirmationEmail(email, code);
        }

        private string fetchEmail()
        {
            string query = "SELECT Login FROM Appaccesscode LIMIT 1"; // Limit 1 to get only one record

            try
            {
                DataTable dt = crudDatabase.FetchDataFromDatabase(query);
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["Login"].ToString();
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while fetching email: " + ex.Message);
                return null;
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
                                          <p>This is a one-time use code that will help you enter manager mode and look up or change your usual access code:</p>
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

                    this.Alert("Email sent", Alertform.enmType.Success);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                this.Alert(LanguageManager.Instance.GetString("Nointernet"), Alertform.enmType.Info);
                return false;                            
            }
        }

        private string GenerateConfirmationCode()
        {
            // Generate a random confirmation code
            Random random = new Random();
            return random.Next(100000, 999999).ToString(); // 6-digit code
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                PasswordText.UseSystemPasswordChar = false;
            }
            else
            {
                PasswordText.UseSystemPasswordChar = true;
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void Login_Shown(object sender, EventArgs e)
        {
            label3.Focus();
        }

        private void Login_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void Login_Click(object sender, EventArgs e)
        {
            if (clavieroverlay != null && clavieroverlay.Visible == true) 
            {
                 clavieroverlay.Visible = false;
            }           
            label3.Focus();
        }

        private void UsernameText_Enter(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.showKeyboard) 
            {
                clavieroverlay = new Clavieroverlay(UsernameText);
                clavieroverlay.EnterButtonClicked += Clavieroverlay_EnterButtonClicked;
                clavieroverlay.boardLocationBottom();
                clavieroverlay.Visible = true;
                clavieroverlay.Show();
            }
        }

        private void PasswordText_Enter(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.showKeyboard)
            {
                clavieroverlay = new Clavieroverlay(PasswordText);
                clavieroverlay.EnterButtonClicked += Clavieroverlay_EnterButtonClicked;
                clavieroverlay.boardLocationBottom();
                clavieroverlay.Visible = true;
                clavieroverlay.Show();

            }
        }     
        
        private void UsernameText_Leave(object sender, EventArgs e)
        {
            if (clavieroverlay != null && clavieroverlay.Visible == true) 
            {
                clavieroverlay.Visible = false;
            }
            clavieroverlay?.Hide();
        }

        private void PasswordText_Leave(object sender, EventArgs e)
        {
            if (clavieroverlay != null && clavieroverlay.Visible == true)
            {
                clavieroverlay.Visible = false;
            }
            clavieroverlay?.Hide();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            if (isForgotPasswordPage)
            {
                label1.Visible = true;
                checkBox1.Visible = false;
                label5.Text = string.Empty;
                UsernameText.Visible = true;
                isForgotPasswordPage = false;
                UsernameText.ReadOnly = true;
                UsernameText.Text = string.Empty;
                PasswordText.Text = string.Empty;
                PasswordText.UseSystemPasswordChar = false;
                label1.Text = LanguageManager.Instance.GetString("iFPP-lbl1");
                label2.Text = LanguageManager.Instance.GetString("iFPP-lbl2");
                label3.Text = LanguageManager.Instance.GetString("iFPP-lbl3");
                label4.Text = LanguageManager.Instance.GetString("iFPP-lbl4");
                checkifEmailSent();
            }
            else if (!isForgotPasswordPage)
            {
                label1.Visible= false;
                checkBox1.Visible = true;
                label5.Text = string.Empty;
                isForgotPasswordPage = true;
                UsernameText.Visible = false;
                UsernameText.ReadOnly = false;
                UsernameText.Text = string.Empty;
                PasswordText.Text = string.Empty;
                PasswordText.UseSystemPasswordChar = true;
                label1.Text = LanguageManager.Instance.GetString("LGN-lbl1");
                label2.Text = LanguageManager.Instance.GetString("LGN-lbl2");
                label3.Text = LanguageManager.Instance.GetString("LGN-lbl3");
                label4.Text = LanguageManager.Instance.GetString("LGN-lbl4");
            }
        }

        private void label4_MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Hand;
            label4.ForeColor = SystemColors.Highlight;
            //label4.Font = new Font(label4.Font, FontStyle.Underline);
        }

        private void label4_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            label4.ForeColor = Color.DodgerBlue;
            //label4.Font = new Font(label4.Font, FontStyle.Regular);
        }

        public void LocalizeControls()
        {
            label1.Text = LanguageManager.Instance.GetString("LGN-lbl1");
            label2.Text = LanguageManager.Instance.GetString("LGN-lbl2");
            label3.Text = LanguageManager.Instance.GetString("LGN-lbl3");
            label4.Text = LanguageManager.Instance.GetString("LGN-lbl4");
            button13.Text = LanguageManager.Instance.GetString("Btnconfirm");
            checkBox1.Text = LanguageManager.Instance.GetString("LGN-chbox1");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            button14.BackColor = colors.Color3;
            UsernameText.ForeColor = colors.Color2;
            PasswordText.ForeColor = colors.Color2;
            UsernameText.BackColorRounded = colors.Color3;
            PasswordText.BackColorRounded = colors.Color3;

            label3.ForeColor = this.ForeColor;
            label4.ForeColor = this.ForeColor;
            label4.ForeColor = Color.DodgerBlue;
            button14.ForeColor = this.ForeColor;
            button13.BorderColor = button13.BackColor;
            label3.Font = new Font("Century Gothic", 10, FontStyle.Regular);
            label4.Font = new Font("Century Gothic", 10, FontStyle.Underline);
        }
    }
}
