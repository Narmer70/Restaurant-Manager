using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using PadTai.Classes;
using System.Data.SQLite;
using System.Windows.Forms;
using PadTai.Classes.Others;
using System.Threading.Tasks;
using PadTai.Sec_daryfolders.Others;
using PadTai.Classes.Controlsdesign;
using PadTai.Sec_daryfolders.Updates;
using PadTai.Sec_daryfolders.Updaters.FoodUpdater;


namespace PadTai.Sec_daryfolders.Deptmmodifier
{
    public partial class Generalsettings : UserControl
    {
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public Generalsettings()
        {
            InitializeComponent();

            InitializeControlResizer();                
            PopulateLanguageComboBox();
            LoadCurrentLanguage();
            addcurrenciesinBox();
          
            LoadThemes();
            ApplyTheme();
            LocalizeControls();
            LoadSelectedTheme();
            ThemeManager.ThemeChanged += ApplyTheme;
            LanguageManager.Instance.LanguageChanged += HandleLanguageChange;
            LanguageManager.Instance.LanguageChanged += HandleControlLanguageChange;             
        }

        private void InitializeControlResizer()
        {
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
            resizer.RegisterControl(label9);
            resizer.RegisterControl(label10);
            resizer.RegisterControl(label11);
            resizer.RegisterControl(label12);
            resizer.RegisterControl(label13);
            resizer.RegisterControl(label14);
            resizer.RegisterControl(textBox1);
            resizer.RegisterControl(customBox1);
            resizer.RegisterControl(customBox2);
            resizer.RegisterControl(customBox3);
            resizer.RegisterControl(toggleSwitch1);
            resizer.RegisterControl(toggleSwitch2);
            resizer.RegisterControl(toggleSwitch3);
            resizer.RegisterControl(toggleSwitch4);
            resizer.RegisterControl(toggleSwitch5);
            resizer.RegisterControl(toggleSwitch8);
            resizer.RegisterControl(toggleSwitch9);
            resizer.RegisterControl(roundedPanel1);
            resizer.RegisterControl(roundedPanel2);
            resizer.RegisterControl(roundedPanel3);
            resizer.RegisterControl(roundedPanel4);
            resizer.RegisterControl(dateTimePicker1);

            double TGtime = Properties.Settings.Default.TGtime;
            int hours = (int)TGtime; 
            double fractionalPart = TGtime - hours; 
            int minutes = (int)(fractionalPart * 60);

            textBox1.Text = Properties.Settings.Default.chatID;
            toggleSwitch4.IsOn = Properties.Settings.Default.lockManagers;
            toggleSwitch3.IsOn = Properties.Settings.Default.showScrollbars;
            toggleSwitch1.IsOn = Properties.Settings.Default.enableIndvButton;
            toggleSwitch2.IsOn = Properties.Settings.Default.isHeadquarterMode;
            toggleSwitch9.IsOn = Properties.Settings.Default.isKeyboardFullScreen;
            toggleSwitch8.IsOn = Properties.Settings.Default.isReadOnlyIngredients;
            dateTimePicker1.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hours, minutes, 0);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (resizer != null)
            {
                resizer.ResizeControls(this);
            }
        }

        private void toggleSwitch1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.enableIndvButton = toggleSwitch1.IsOn;
            Properties.Settings.Default.Save();
        }

        private void toggleSwitch2_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.isHeadquarterMode = toggleSwitch2.IsOn;
            Properties.Settings.Default.Save();
        }

        private void toggleSwitch3_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.showScrollbars = toggleSwitch3.IsOn;
            Properties.Settings.Default.Save();
        }

        private void toggleSwitch4_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.lockManagers = toggleSwitch4.IsOn;
            Properties.Settings.Default.Save();
        }

        private void toggleSwitch5_Click(object sender, EventArgs e)
        {

        }

        private void toggleSwitch8_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.isReadOnlyIngredients = toggleSwitch8.IsOn;
            Properties.Settings.Default.Save();
        }

        private void toggleSwitch9_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.isKeyboardFullScreen = toggleSwitch9.IsOn;
            Properties.Settings.Default.Save();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                Properties.Settings.Default.chatID = textBox1.Text.Trim().ToString();
                Properties.Settings.Default.Save();
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DateTime selectedTime = dateTimePicker1.Value;

            // Convert the time to a double
            double hours = selectedTime.Hour;
            double minutes = selectedTime.Minute;
            double minutesAsFraction = minutes / 60.0;
            double TGtime = hours + minutesAsFraction;

            Properties.Settings.Default.TGtime = TGtime;
            Properties.Settings.Default.Save();
        }

        #region LANGUAGES
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

            if (currentLanguage == "en")
            {
                customBox1.SelectedItem = "English";
            }
            else if (currentLanguage == "fr")
            {
                customBox1.SelectedItem = "Français";
            }
            else if (currentLanguage == "ru")
            {
                customBox1.SelectedItem = "Русский";
            }
            else
            {
                customBox1.SelectedItem = "English";
            }
        }

        private void customBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
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
                cultureCode = "en"; 
            }

            LanguageManager.Instance.ChangeLanguage(cultureCode);
        }

        private void customBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void HandleControlLanguageChange()
        {
            LocalizeControls();
        }

        public void LocalizeControls()
        {
            label1.Text = LanguageManager.Instance.GetString("GENSET-lbl1");
            label2.Text = LanguageManager.Instance.GetString("GENSET-lbl2");
            label3.Text = LanguageManager.Instance.GetString("GENSET-lbl3");
            label4.Text = LanguageManager.Instance.GetString("GENSET-lbl4");
            label6.Text = LanguageManager.Instance.GetString("GENSET-lbl6");
            label7.Text = LanguageManager.Instance.GetString("GENSET-lbl7");
            label9.Text = LanguageManager.Instance.GetString("GENSET-lbl9");
            label12.Text = LanguageManager.Instance.GetString("GENSET-lbl12");
            label8.Text = "*" + LanguageManager.Instance.GetString("GENSET-lbl8");
            label5.Text = LanguageManager.Instance.GetString("GENSET-lbl5") + "*";
        }

        #endregion

        #region CURRENCY

        private void addcurrenciesinBox() 
        {
            string currentCurrency = CurrencyService.Instance.SelectedCurrency;
            // Adding currencies to customBox3
            customBox3.Items.Add("Null"); // Option to choose no currency
            customBox3.Items.Add("United States Dollar (USD)"); // United States Dollar
            customBox3.Items.Add("Euro (EUR)"); // Euro
            customBox3.Items.Add("Russian Ruble (RUB)"); // Russian Ruble
            customBox3.Items.Add("Belarusian Ruble (BYN)"); // Belarusian Ruble
            customBox3.Items.Add("Canadian Dollar (CAD)"); // Canadian Dollar
            customBox3.Items.Add("Australian Dollar (AUD)"); // Australian Dollar
            customBox3.Items.Add("British Pound Sterling (GBP)"); // British Pound Sterling
            customBox3.Items.Add("West African CFA Franc (XOF)"); // West African CFA Franc
            customBox3.Items.Add("Central African CFA Franc (XAF)"); // Central African CFA Franc
            customBox3.Items.Add("Kenyan Shilling (KES)"); // Kenyan Shilling
            customBox3.Items.Add("South African Rand (ZAR)"); // South African Rand
            customBox3.Items.Add("Congolese Franc (CDF)"); // Congolese Franc

            if (!string.IsNullOrEmpty(currentCurrency))
            {
                customBox3.SelectedItem = currentCurrency;
            }
            else
            {
                customBox3.SelectedIndex = 0; 
            }
        }

        private void customBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrencyService.Instance.SelectedCurrency = customBox3.SelectedItem.ToString();
            CurrencyService.Instance.SaveCurrency();
        }

        private void customBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        #endregion

        #region THEMES     
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


        private void customBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            panel1.BackColor = colors.Color3;
            textBox1.ForeColor = colors.Color2;
            roundedPanel1.BackColor = colors.Color3;
            roundedPanel2.BackColor = colors.Color3;
            roundedPanel3.BackColor = colors.Color3;
            roundedPanel4.BackColor = colors.Color3;
            textBox1.BackColorRounded = colors.Color1;
            customBox1.BorderFocusColor = colors.Color5;
            customBox2.BorderFocusColor = colors.Color5;
            customBox3.BorderFocusColor = colors.Color5;
            customBox3.BorderFocusColor = colors.Color5;
            customBox3.BorderFocusColor = colors.Color5;

            label1.ForeColor = this.ForeColor;
            label2.ForeColor = this.ForeColor;
            label3.ForeColor = this.ForeColor;
            label4.ForeColor = this.ForeColor;
            label5.ForeColor = this.ForeColor;
            label6.ForeColor = this.ForeColor;
            label7.ForeColor = this.ForeColor;
            label9.ForeColor = this.ForeColor;
            label10.ForeColor = this.ForeColor;
            label11.ForeColor = this.ForeColor;
            label12.ForeColor = this.ForeColor;
            label13.ForeColor = this.ForeColor;
            label14.ForeColor = this.ForeColor;
            customBox1.ForeColor = this.ForeColor;
            customBox2.ForeColor = this.ForeColor;
            customBox3.ForeColor = this.ForeColor;
            customBox1.BackColor = this.BackColor;
            customBox2.BackColor = this.BackColor;
            customBox3.BackColor = this.BackColor;
            customBox1.ArrowColor = this.ForeColor;
            customBox2.ArrowColor = this.ForeColor;
            customBox3.ArrowColor = this.ForeColor;
            toggleSwitch1.OffColor = this.BackColor;
            toggleSwitch2.OffColor = this.BackColor;        
            toggleSwitch3.OffColor = this.BackColor;     
            toggleSwitch4.OffColor = this.BackColor;
            toggleSwitch5.OffColor = this.BackColor;
            toggleSwitch8.OffColor = this.BackColor;
            toggleSwitch9.OffColor = this.BackColor;
            toggleSwitch1.LabelColor = this.ForeColor;
            toggleSwitch2.LabelColor = this.ForeColor;
            toggleSwitch3.LabelColor = this.ForeColor;
            toggleSwitch4.LabelColor = this.ForeColor;
            toggleSwitch5.LabelColor = this.ForeColor;
            toggleSwitch8.LabelColor = this.ForeColor;
            toggleSwitch9.LabelColor = this.ForeColor;
            toggleSwitch1.SwitchColor = this.ForeColor;
            toggleSwitch2.SwitchColor = this.ForeColor;
            toggleSwitch3.SwitchColor = this.ForeColor;
            toggleSwitch4.SwitchColor = this.ForeColor;
            toggleSwitch5.SwitchColor = this.ForeColor;
            toggleSwitch8.SwitchColor = this.ForeColor;
            toggleSwitch9.SwitchColor = this.ForeColor;
            dateTimePicker1.TextColor = this.ForeColor;
            dateTimePicker1.BorderColor = colors.Color5;
            dateTimePicker1.BackColorCustom = colors.Color1;
            dateTimePicker1.BorderFocusColor = colors.Color5;
            dateTimePicker1.CalendarForeColor = colors.Color2;
        }
        #endregion
    }
}
