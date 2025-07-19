using System;
using System.IO;
using System.Data;
using System.Media;
using System.Drawing;
using System.Windows.Forms;
using PadTai.Classes.Others;


namespace PadTai.Sec_daryfolders.Others_Forms
{
    public partial class Alertform : Form
    {
        public Alertform()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += ApplyTheme;
            ApplyTheme();
        }

        public enum enmAction
        {
            wait,
            start,
            close
        }

        public enum enmType
        {
            Success,
            Warning,
            Error,
            Info
        }

        private Alertform.enmAction action;

        private int x, y;

        private void rjButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (this.action) 
            {
                case enmAction.wait:
                    timer1.Interval = 5000;
                    action = enmAction.close;
                     break;
                case Alertform.enmAction.start:
                    this.timer1.Interval = 1;
                    this.Opacity += 0.1;
                    if (this.x <this.Location.X) 
                    {
                        this.Left--;
                    }
                    else 
                    {
                        if(this.Opacity == 1.0) 
                        {
                            action = Alertform.enmAction.wait;
                        }
                    }
                    break;
                case enmAction.close:
                    timer1.Interval = 1;
                    this.Opacity -= 0.1;

                    this.Left -= 3;
                    if (this.Opacity == 0.0) 
                    {
                        this.Hide();
                        timer1.Stop();
                    }
                    break;

            }
        }

        public void showAlert(string msg, enmType type) 
        {
            this.Opacity = 0.0;
            this.StartPosition = FormStartPosition.Manual;
            string fname;

            for (int i = 1; i < 100000; i++) 
            {
                fname = "Alert" + i.ToString();
                Alertform alertform = (Alertform)Application.OpenForms[fname];

                if (alertform == null) 
                {
                    this.Name = fname;
                    this.x = Screen.PrimaryScreen.WorkingArea.Width - this.Width + 15;
                    this.y = Screen.PrimaryScreen.WorkingArea.Height - this.Height + 20 /** i -5 * i*/;
                    this.Location = new Point(this.x, this.y);
                    break;
                }
            }
            this.x = Screen.PrimaryScreen.WorkingArea.Width - base.Width /*- 5*/;

            switch (type)
            {
                case enmType.Success:
                    this.pictureBox1.Image = Properties.Resources.guarantee_5290087;
                    this.panel1.BackColor = Color.LimeGreen;

                    if (Properties.Settings.Default.playSoundNotification) 
                    {
                          PlaySound(Properties.Resources.bell_congratulations_epic_stock_media_1_00_01); 
                    }
                    break;

                case enmType.Error:
                    this.pictureBox1.Image = Properties.Resources.exclamation;
                    this.panel1.BackColor = Color.Red;

                    if (Properties.Settings.Default.playSoundNotification)
                    {
                         PlaySound(Properties.Resources.ui_negative_answer_om_fx_1_00_03); 
                    }
                    break;

                case enmType.Warning:
                    this.pictureBox1.Image = Properties.Resources.attention_12465580;
                    this.panel1.BackColor = Color.Orange;

                    if (Properties.Settings.Default.playSoundNotification)
                    {
                          PlaySound(Properties.Resources.ui_bell_ding_om_fx_2_2_00_04); 
                    }
                    break;

                case enmType.Info:
                    this.pictureBox1.Image = Properties.Resources.edit_8866040;
                    this.panel1.BackColor = Color.DeepSkyBlue;

                    if (Properties.Settings.Default.playSoundNotification)
                    {
                          PlaySound(Properties.Resources.ui_bell_ding_om_fx_2_2_00_04); 
                    }
                    break;
            }


            this.label1.Text = msg;
            this.Show();
            this.action = enmAction.start;
            this.timer1.Interval = 1;
            this.timer1.Start();
        }

        private void PlaySound(System.IO.Stream soundStream)
        {
            try
            {
                using (SoundPlayer player = new SoundPlayer(soundStream))
                {
                    player.Play(); // Play the sound asynchronously
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error playing sound: {ex.Message}");
            }
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            label1.ForeColor = colors.Color2;
            rjButton1.BackColor = this.BackColor;   
            rjButton1.ForeColor = this.ForeColor;
        }
    }
}
