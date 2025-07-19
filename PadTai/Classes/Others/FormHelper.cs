using System;
using System.Drawing;
using System.Windows.Forms;


namespace PadTai.Classes
{
    public class FormHelper
    {
        public static void ShowFormWithOverlay(Form owner, Form formToShow)
        {
            Form formBackground = new Form();
            try
            {
                formBackground.StartPosition = FormStartPosition.Manual;
                formBackground.WindowState = FormWindowState.Maximized;
                formBackground.FormBorderStyle = FormBorderStyle.None;
                formBackground.Location = owner.Location;
                formBackground.BackColor = Color.Black;
                formBackground.ShowInTaskbar = false;
                formBackground.Opacity = 0.50d;
                formBackground.TopMost = true;


                formBackground.Show();
                formToShow.ShowDialog();
                formBackground.Dispose();
                formToShow.Owner = formBackground;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                formBackground.Dispose();              
            }
        }
    }
}
