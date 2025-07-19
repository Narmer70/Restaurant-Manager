using System;
using System.IO;
using System.Data;
using System.Drawing;
using PadTai.Classes;
using System.Data.SQLite;
using System.Windows.Forms;
using PadTai.Fastcheckfiles;
using System.Data.SqlClient;
using PadTai.Classes.Others;
using PadTai.Sec_daryfolders;
using System.Collections.Generic;
using PadTai.Classes.Databaselink;
using PadTai.Sec_daryfolders.Others;
using PadTai.Sec_daryfolders.Update;
using PadTai.Sec_daryfolders.Updates;
using PadTai.Sec_daryfolders.Updaters;
using PadTai.Sec_daryfolders.Quitfolder;
using PadTai.Sec_daryfolders.Staffmanager;
using PadTai.Sec_daryfolders.Tablereserve;
using PadTai.Sec_daryfolders.Departmentdata;
using PadTai.Sec_daryfolders.DB_Appinitialize;
using PadTai.Sec_daryfolders.Updaters.Otherupdates;
using PadTai.Sec_daryfolders.Allreports.Reportgraphs;
using PadTai.Sec_daryfolders.Staffmanager.Stafftimetable;


namespace PadTai
{
    public partial class MainPage : Form
    {
        private Receiptdetails receiptdetails;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public MainPage()
        {
            InitializeComponent();

            crudDatabase = new CrudDatabase();
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(panel1);
            resizer.RegisterControl(label1);
            resizer.RegisterControl(label2);
            resizer.RegisterControl(label3);
            resizer.RegisterControl(label4);
            resizer.RegisterControl(label5);
            resizer.RegisterControl(label6);
            resizer.RegisterControl(label7);
            resizer.RegisterControl(label8);
            resizer.RegisterControl(button1);
            resizer.RegisterControl(button2);
            resizer.RegisterControl(button3);
            resizer.RegisterControl(button4);
            resizer.RegisterControl(button5);
            resizer.RegisterControl(button6);
            resizer.RegisterControl(button7);
            resizer.RegisterControl(button8);
            resizer.RegisterControl(button9);
            resizer.RegisterControl(button10);
            resizer.RegisterControl(button11);
            resizer.RegisterControl(button12);
            resizer.RegisterControl(button13);
            resizer.RegisterControl(button14);
            resizer.RegisterControl(button15);
            resizer.RegisterControl(button16);
            resizer.RegisterControl(button17);
            resizer.RegisterControl(button18);
            resizer.RegisterControl(button19);
            resizer.RegisterControl(button20);
        
            CheckSessionAndShowForm();
            buttonsIamageResize();
            initiateManagerMode();
         
            LanguageManager.Instance.LanguageChanged += HandleLanguageChange;
            ThemeManager.ThemeChanged += ApplyTheme;
            LocalizeControls();
            ApplyTheme();
        }

        private void initiateManagerMode() 
        {
            if (Properties.Settings.Default.lockManagers)
            {
                panel1.Enabled = false;
                label8.Enabled = false;
            }
            else
            {
                panel1.Enabled = true;
                label7.Enabled = false;
                panel1.Enabled = Properties.Settings.Default.isMangerMode;
                label8.Enabled = Properties.Settings.Default.isMangerMode;
                label7.Enabled = !Properties.Settings.Default.isMangerMode;          
            }        
        }

        public void CheckSessionAndShowForm()
        {
            if (Properties.Settings.Default.AccountFormShown)
            {
                using(Databasecreate DBC = new Databasecreate()) 
                {
                    string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Localdatabase.db");

                    using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                    {
                        connection.Open();
                        
                        if (!File.Exists(dbPath))
                        {
                            SQLiteConnection.CreateFile(dbPath);
                            DBC.CreateSQLiteTables(connection);
                        } 
                    }
                    DBC.ShowDialog();
                    this.Hide();
                }
            }

            if (!Properties.Settings.Default.IsSessionOpened && EmployeesTableCount())
            {
                using (Opening opening = new Opening())
                {
                    opening.ShowDialog();
                    this.Hide();
                }
            }
        }


        public bool EmployeesTableCount()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Employees";

                DataTable resultTable = crudDatabase.FetchDataFromDatabase(query);

                if (resultTable != null && resultTable.Rows.Count > 0)
                {
                    long count = Convert.ToInt64(resultTable.Rows[0][0]);

                    return count > 0; 
                }
                else
                {
                    return false; 
                }
            }
            catch
            {
                return false;
            }
        }

        public void EnablePanel()
        {
            panel1.Enabled = true;
            label8.Enabled = true;
            label7.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (Fastcheck FCK = new Fastcheck(receiptdetails))
            {
                FormHelper.ShowFormWithOverlay(this, FCK);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (Reportviewer RVR = new Reportviewer())
            {
                FormHelper.ShowFormWithOverlay(this, RVR);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            using (Reservationform RF = new Reservationform())
            {
                FormHelper.ShowFormWithOverlay(this, RF);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            using (Dailygraphs DG = new Dailygraphs())
            {
                FormHelper.ShowFormWithOverlay(this, DG);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            using (Quitapp QA = new Quitapp())
            {
                FormHelper.ShowFormWithOverlay(this, QA);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (Displayreceipts DR = new Displayreceipts())
            {
                FormHelper.ShowFormWithOverlay(this, DR);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (UpdateFood UF = new UpdateFood())
            {
                FormHelper.ShowFormWithOverlay(this.FindForm(), UF);
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            using (Staffviewer SV = new Staffviewer())
            {
                FormHelper.ShowFormWithOverlay(this, SV);
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Application.Exit();
            notifyIcon1.Dispose();
        }

        private void показатьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Maximized;
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void выйтиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MainPage_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void MainPage_Load(object sender, EventArgs e)
        {        
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            using (Otdelupdater OTR = new Otdelupdater())
            {
                FormHelper.ShowFormWithOverlay(this, OTR);
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            using (Emplotimetable ETT = new Emplotimetable())
            {
                FormHelper.ShowFormWithOverlay(this, ETT);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            using (Daylyspendings DLSS = new Daylyspendings())
            {
                FormHelper.ShowFormWithOverlay(this, DLSS);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            using (Butttondisabler BD = new Butttondisabler())
            {
                FormHelper.ShowFormWithOverlay(this, BD);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (Savereceipes SRS = new Savereceipes())
            {
                FormHelper.ShowFormWithOverlay(this, SRS);
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            using (Departementsplitter DPSTR = new Departementsplitter())
            {
                FormHelper.ShowFormWithOverlay(this, DPSTR);
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            using (Deliverysystem DS = new Deliverysystem())
            {
                FormHelper.ShowFormWithOverlay(this, DS);
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            using (Managegross MG = new Managegross())
            {
                FormHelper.ShowFormWithOverlay(this, MG);
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            using (Clientloyalplan CLP = new Clientloyalplan())
            {
                FormHelper.ShowFormWithOverlay(this, CLP);
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            using (Customisablesform UF = new Customisablesform())
            {
                FormHelper.ShowFormWithOverlay(this.FindForm(), UF);
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {
            using (Login LG = new Login(this))
            {
                FormHelper.ShowFormWithOverlay(this, LG);
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {
            panel1.Enabled = false;
            label7.Enabled = true;
            label8.Enabled = false;
            Properties.Settings.Default.isMangerMode = false;
            Properties.Settings.Default.Save();
        }

        private void label7_MouseEnter(object sender, EventArgs e)
        {
            label7.Font = new Font(label7.Font, FontStyle.Underline);
            Cursor = Cursors.Hand;
        }

        private void label7_MouseLeave(object sender, EventArgs e)
        {
            label7.Font = new Font(label7.Font, FontStyle.Bold);
            Cursor = Cursors.Default;
        }

        private void label8_MouseLeave(object sender, EventArgs e)
        {
            label8.Font = new Font(label8.Font, FontStyle.Bold);
            Cursor = Cursors.Default;
        }

        private void label8_MouseEnter(object sender, EventArgs e)
        {
            label8.Font = new Font(label8.Font, FontStyle.Underline);
            Cursor = Cursors.Hand;
        }


        private void HandleLanguageChange()
        {
            LocalizeControls();
        }

        public void LocalizeControls()
        {
            button1.Text = LanguageManager.Instance.GetString("MF-btn1");
            button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            button3.Text = LanguageManager.Instance.GetString("MF-btn3");
            button4.Text = LanguageManager.Instance.GetString("MF-btn4");
            button5.Text = LanguageManager.Instance.GetString("MF-btn5");
            button6.Text = LanguageManager.Instance.GetString("MF-btn6");
            button7.Text = LanguageManager.Instance.GetString("MF-btn7");
            button8.Text = LanguageManager.Instance.GetString("MF-btn8");
            button9.Text = LanguageManager.Instance.GetString("MF-btn9");
            button10.Text = LanguageManager.Instance.GetString("MF-btn10");
            button11.Text = LanguageManager.Instance.GetString("MF-btn11");
            button13.Text = LanguageManager.Instance.GetString("MF-btn13");
            button14.Text = LanguageManager.Instance.GetString("MF-btn14");
            button15.Text = LanguageManager.Instance.GetString("MF-btn15");
            button16.Text = LanguageManager.Instance.GetString("MF-btn16");
            button17.Text = LanguageManager.Instance.GetString("MF-btn17");
            button18.Text = LanguageManager.Instance.GetString("MF-btn18");
            button19.Text = LanguageManager.Instance.GetString("MF-btn19");
            label1.Text = LanguageManager.Instance.GetString("MF-label1");
            label2.Text = LanguageManager.Instance.GetString("MF-label2");
            label3.Text = LanguageManager.Instance.GetString("MF-label3");
            label4.Text = LanguageManager.Instance.GetString("MF-label4");
            label5.Text = LanguageManager.Instance.GetString("MF-label5");
            label6.Text = LanguageManager.Instance.GetString("MF-label6");
            label7.Text = LanguageManager.Instance.GetString("MF-linklbl1");
            label8.Text = LanguageManager.Instance.GetString("MF-linklbl2");
            StripMenu1.Text = LanguageManager.Instance.GetString("MF-ctxtMenuShow");
            StripMenu2.Text = LanguageManager.Instance.GetString("MF-ctxtMenuQuit");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            label7.ForeColor = this.ForeColor;
            label8.ForeColor = this.ForeColor;
            button1.BackColor = colors.Color3;
            button1.ForeColor = colors.Color2;
            button2.BackColor = colors.Color3;
            button2.ForeColor = colors.Color2;
            button3.BackColor = colors.Color3;
            button3.ForeColor = colors.Color2;
            button4.BackColor = colors.Color3;
            button4.ForeColor = colors.Color2;
            button5.BackColor = colors.Color3;
            button5.ForeColor = colors.Color2;
            button6.BackColor = colors.Color3;  
            button6.ForeColor = colors.Color2;
            button7.BackColor = colors.Color3;
            button7.ForeColor = colors.Color2;
            button8.BackColor = colors.Color3;
            button8.ForeColor = colors.Color2;
            button9.BackColor = colors.Color3;
            button9.ForeColor = colors.Color2;
            button10.BackColor = colors.Color3;
            button10.ForeColor = colors.Color2;
            button11.BackColor = colors.Color3;
            button11.ForeColor = colors.Color2;
            button13.BackColor = colors.Color3;
            button13.ForeColor = colors.Color2;
            button14.BackColor = colors.Color3;
            button14.ForeColor = colors.Color2;
            button15.BackColor = colors.Color3;
            button15.ForeColor = colors.Color2;
            button16.BackColor = colors.Color3;
            button16.ForeColor = colors.Color2;
            button17.BackColor = colors.Color3;
            button17.ForeColor = colors.Color2;
            button18.BackColor = colors.Color3;
            button18.ForeColor = colors.Color2;
            button19.BackColor = colors.Color3;
            button19.ForeColor = colors.Color2;
            button12.ForeColor = this.ForeColor;
            button12.BackColor = this.BackColor;
            button20.ForeColor = this.ForeColor;
            button20.BackColor = this.BackColor;
            button1.BorderColor = colors.Color3;
            button2.BorderColor = colors.Color3;
            button3.BorderColor = colors.Color3;
            button4.BorderColor = colors.Color3;
            button5.BorderColor = colors.Color3;
            button6.BorderColor = colors.Color3;
            button7.BorderColor = colors.Color3;
            button8.BorderColor = colors.Color3;
            button9.BorderColor = colors.Color3;
            button10.BorderColor = colors.Color3;
            button11.BorderColor = colors.Color3;
            button12.BorderColor = colors.Color1;
            button13.BorderColor = colors.Color3;
            button14.BorderColor = colors.Color3;
            button15.BorderColor = colors.Color3;
            button16.BorderColor = colors.Color3;
            button17.BorderColor = colors.Color3;
            button18.BorderColor = colors.Color3;
            button19.BorderColor = colors.Color3;
            button20.BorderColor = colors.Color1;
            StripMenu1.ForeColor = this.ForeColor;
            StripMenu2.ForeColor = this.ForeColor;
            StripMenu1.BackColor = this.BackColor;
            StripMenu2.BackColor = this.BackColor;
            contextMenuStrip1.BackColor = this.BackColor;
        }

        public void buttonsIamageResize()
        {
            Image resizedImage1 = ImageResizer.ResizeImage(Properties.Resources.bar_chart_15173309, button7.Width * 2 / 3, button7.Height * 2 / 3);
            button7.Image = resizedImage1;
            button7.ImageAlign = ContentAlignment.BottomCenter;
            button7.TextImageRelation = TextImageRelation.ImageAboveText;

            Image resizedImage2 = ImageResizer.ResizeImage(Properties.Resources.clipboard_10419423, button2.Width * 2 / 3, button2.Height * 2 / 3);
            button2.Image = resizedImage2;
            button2.ImageAlign = ContentAlignment.BottomCenter;
            button2.TextImageRelation = TextImageRelation.ImageAboveText;

            Image resizedImage3 = ImageResizer.ResizeImage(Properties.Resources.settings, button11.Width * 2 / 3, button11.Height * 2 / 3);
            button11.Image = resizedImage3;
            button11.ImageAlign = ContentAlignment.BottomCenter;
            button11.TextImageRelation = TextImageRelation.ImageAboveText;

            Image resizedImage4 = ImageResizer.ResizeImage(Properties.Resources.wallet_11928102, button8.Width * 2 / 3, button8.Height * 2 / 3);
            button8.Image = resizedImage4;
            button8.ImageAlign = ContentAlignment.BottomCenter;
            button8.TextImageRelation = TextImageRelation.ImageAboveText;

            Image resizedImage5 = ImageResizer.ResizeImage(Properties.Resources.closed_17847902, button6.Width * 2 / 3, button6.Height * 2 / 3);
            button6.Image = resizedImage5;
            button6.ImageAlign = ContentAlignment.BottomCenter;
            button6.TextImageRelation = TextImageRelation.ImageAboveText;

            Image resizedImage6 = ImageResizer.ResizeImage(Properties.Resources.scooter_1337182, button16.Width * 2 / 3, button16.Height * 2 / 3);
            button16.Image = resizedImage6;
            button16.ImageAlign = ContentAlignment.BottomCenter;
            button16.TextImageRelation = TextImageRelation.ImageAboveText;
         
            Image resizedImage7 = ImageResizer.ResizeImage(Properties.Resources.delivery_truck_4931581, button17.Width * 2 / 3, button17.Height * 2 / 3);
            button17.Image = resizedImage7;
            button17.ImageAlign = ContentAlignment.BottomCenter;
            button17.TextImageRelation = TextImageRelation.ImageAboveText;
           
            Image resizedImage8 = ImageResizer.ResizeImage(Properties.Resources.favorite_6952950, button18.Width * 2 / 3, button18.Height * 2 / 3);
            button18.Image = resizedImage8;
            button18.ImageAlign = ContentAlignment.BottomCenter;
            button18.TextImageRelation = TextImageRelation.ImageAboveText;        
          
            Image resizedImage9 = ImageResizer.ResizeImage(Properties.Resources.food_14705524, button4.Width * 2 / 3, button4.Height * 2 / 3);
            button4.Image = resizedImage9;
            button4.ImageAlign = ContentAlignment.BottomCenter;
            button4.TextImageRelation = TextImageRelation.ImageAboveText;

            Image resizedImage10 = ImageResizer.ResizeImage(Properties.Resources.pin_12066913, button15.Width * 2 / 3, button15.Height * 2 / 3);
            button15.Image = resizedImage10;
            button15.ImageAlign = ContentAlignment.BottomCenter;
            button15.TextImageRelation = TextImageRelation.ImageAboveText;

            Image resizedImage11 = ImageResizer.ResizeImage(Properties.Resources.calculation, button1.Width * 2 / 3, button1.Height * 2 / 3);
            button1.Image = resizedImage11;
            button1.ImageAlign = ContentAlignment.BottomCenter;
            button1.TextImageRelation = TextImageRelation.ImageAboveText;

            Image resizedImage12 = ImageResizer.ResizeImage(Properties.Resources.checklist_5190705, button3.Width * 2 / 3, button3.Height * 2 / 3);
            button3.Image = resizedImage12;
            button3.ImageAlign = ContentAlignment.BottomCenter;
            button3.TextImageRelation = TextImageRelation.ImageAboveText;

            Image resizedImage13 = ImageResizer.ResizeImage(Properties.Resources.disaster_recovery__1_, button5.Width * 2 / 3, button5.Height * 2 / 3);
            button5.Image = resizedImage13;
            button5.ImageAlign = ContentAlignment.BottomCenter;
            button5.TextImageRelation = TextImageRelation.ImageAboveText;

            Image resizedImage14 = ImageResizer.ResizeImage(Properties.Resources.waiting_room__2_, button9.Width * 2 / 3, button9.Height * 2 / 3);
            button9.Image = resizedImage14;
            button9.ImageAlign = ContentAlignment.BottomCenter;
            button9.TextImageRelation = TextImageRelation.ImageAboveText;

            Image resizedImage15 = ImageResizer.ResizeImage(Properties.Resources.dish, button10.Width * 2 / 3, button10.Height * 2 / 3);
            button10.Image = resizedImage15;
            button10.ImageAlign = ContentAlignment.BottomCenter;
            button10.TextImageRelation = TextImageRelation.ImageAboveText;

            Image resizedImage16 = ImageResizer.ResizeImage(Properties.Resources.support_17824757, button13.Width * 2 / 3, button13.Height * 2 / 3);
            button13.Image = resizedImage16;
            button13.ImageAlign = ContentAlignment.BottomCenter;
            button13.TextImageRelation = TextImageRelation.ImageAboveText;

            Image resizedImage17 = ImageResizer.ResizeImage(Properties.Resources.calendar_8567661, button14.Width * 2 / 3, button14.Height * 2 / 3);
            button14.Image = resizedImage17;
            button14.ImageAlign = ContentAlignment.BottomCenter;
            button14.TextImageRelation = TextImageRelation.ImageAboveText;

            Image resizedImage18 = ImageResizer.ResizeImage(Properties.Resources.folder_9782493, button19.Width * 2 / 3, button19.Height * 2 / 3);
            button19.Image = resizedImage18;
            button19.ImageAlign = ContentAlignment.BottomCenter;
            button19.TextImageRelation = TextImageRelation.ImageAboveText;
        }
    }
}