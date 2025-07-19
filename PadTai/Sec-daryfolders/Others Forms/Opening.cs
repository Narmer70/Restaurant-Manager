using System;
using System.Data;
using PadTai.Classes;
using System.Drawing;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Data.SqlClient;
using PadTai.Classes.Others;
using PadTai.Sec_daryfolders;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using PadTai.Classes.Controlsdesign;
using PadTai.Sec_daryfolders.Others;
using PadTai.Sec_daryfolders.DB_Appinitialize;


namespace PadTai
{
    public partial class Opening : Form
    {
        private Clavieroverlay clavieroverlay;
        private DraggableForm draggableForm;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;
     
        public Opening()
        {
            InitializeComponent();
            initiateControlResizing();
            crudDatabase = new CrudDatabase();

            LocalizeControls();
            ApplyTheme();
        }


        private void initiateControlResizing() 
        {
            draggableForm = new DraggableForm();
            draggableForm.EnableDragging(this, this);
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(label1);
            resizer.RegisterControl(label3);
            resizer.RegisterControl(label4);
            resizer.RegisterControl(textBox2);
            resizer.RegisterControl(button13);
            resizer.RegisterControl(rjButton1);
        }


        private void Clavieroverlay_EnterButtonClicked(object sender, EventArgs e)
        {
            verifyInsertedCode();
        }


        private void Opening_Load(object sender, EventArgs e)
        { 
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void Opening_Shown(object sender, EventArgs e)
        {
            label1.Focus();
        }

        private void Opening_Click(object sender, EventArgs e)
        {
            label1.Focus();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            label1.Focus();
        }

        private void Opening_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            verifyInsertedCode();
        }

        private void verifyInsertedCode() 
        {
            string password = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Invalid password.");
                return;
            }

            if (!ValidateUser(password))
            {
                MessageBox.Show("User  validation failed.");
                return;
            }

            string selectQuery = "SELECT ID, Name, Contact, PersonalType, ClientID FROM Employees WHERE Password = @Password";
            var selectParams = new Dictionary<string, object>
            {
                { "@Password", password },
            };

            DataTable userTable = crudDatabase.FetchDataFromDatabase(selectQuery, selectParams);

            if (userTable == null || userTable.Rows.Count == 0)
            {
                MessageBox.Show("Invalid credentials.");
                return;
            }

            DataRow userRow = userTable.Rows[0];

            string userName = userRow["Name"].ToString();
            string contact = userRow["Contact"].ToString();
            string personalType = userRow["PersonalType"].ToString();
            int userId = userRow["ID"] != DBNull.Value ? Convert.ToInt32(userRow["ID"]) : 0;
            int clientId = userRow["ClientID"] != DBNull.Value ? Convert.ToInt32(userRow["ClientID"]) : 0;

            string insertQuery = @" INSERT INTO Employeelogin (EmployeeID, PersonalType, UserName, Thedaydate, TimeOfStarting, Contact, ClientID) 
                                      VALUES (@ID, @PersonalType, @UserName, @Thedaydate, @TimeOfStarting, @Contact, @ClientID)";

            var insertParams = new Dictionary<string, object>
            {
                { "@ID", userId },
                { "@PersonalType", personalType },
                { "@UserName", userName },
                { "@Thedaydate", DateTime.Now.Date },
                { "@TimeOfStarting", DateTime.Now.TimeOfDay },
                { "@Contact", contact },
                { "@ClientID", clientId }
            };

            bool insertSuccess = crudDatabase.ExecuteNonQuery(insertQuery, insertParams);

            if (insertSuccess)
            {
                Properties.Settings.Default.IsSessionOpened = true;
                Properties.Settings.Default.Save();
                Application.Restart();
            }
            else
            {
                MessageBox.Show("Failed to log employee login.");
            }
        }


        private bool ValidateUser(string password)
        {
            string query = "SELECT COUNT(*) AS UserCount FROM Employees WHERE Password = @Password";

            var parameters = new Dictionary<string, object>
            {
                { "@Password", password },
            };

            DataTable resultTable = crudDatabase.FetchDataFromDatabase(query, parameters);

            if (resultTable != null && resultTable.Rows.Count > 0)
            {
                int count = Convert.ToInt32(resultTable.Rows[0]["UserCount"]);
                return count > 0;
            }

            return false;
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.showKeyboard)
            {
                clavieroverlay = new Clavieroverlay(textBox2);
                clavieroverlay.EnterButtonClicked += Clavieroverlay_EnterButtonClicked;
                clavieroverlay.boardLocationBottom();
                clavieroverlay.Visible = true;
                clavieroverlay.Show();

            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (clavieroverlay != null && clavieroverlay.Visible == true)
            {
                clavieroverlay.Visible = false;
            }
            clavieroverlay?.Hide();
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label4_MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Hand;
            label4.ForeColor = SystemColors.Highlight;
        }

        private void label4_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            label4.ForeColor = Color.DodgerBlue;
        }

        public void LocalizeControls()
        {
            label1.Text = LanguageManager.Instance.GetString("LGN-lbl2");
            label3.Text = LanguageManager.Instance.GetString("LGN-lbl3");
            label4.Text = LanguageManager.Instance.GetString("LGN-lbl4");
            button13.Text = LanguageManager.Instance.GetString("Btn-Enter");
            rjButton1.Text = LanguageManager.Instance.GetString("Btn-close");
            //button5.Text = LanguageManager.Instance.GetString("MF-btn5");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            label3.ForeColor = this.ForeColor;
            label4.ForeColor = this.ForeColor;
            textBox2.ForeColor = colors.Color2;
            label4.ForeColor = Color.DodgerBlue;
            pictureBox1.BackColor = colors.Color3;
            textBox2.BackColorRounded = colors.Color3;
            label3.Font = new Font("Century Gothic", 10, FontStyle.Regular);
            label4.Font = new Font("Century Gothic", 10, FontStyle.Underline);
        }
    }
}
