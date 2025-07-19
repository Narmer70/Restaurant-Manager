using System;
using System.Windows.Forms;

namespace PadTai.Classes
{
    public class Charcharacter : TextBox
    {
        private string _password = string.Empty;

        public Charcharacter()
        {
            this.Multiline = true;
            this.PasswordChar = '*'; // Character to display
            this.TextChanged += CharcharacterTextBox_TextChanged;
            this.Leave += CharcharacterTextBox_Leave;
            this.Enter += CharcharacterTextBox_Enter;
        }

        private void CharcharacterTextBox_TextChanged(object sender, EventArgs e)
        {
            // Store the actual password
            _password = this.Text;

            // Replace the text with asterisks
            this.Text = new string(PasswordChar, _password.Length);
            this.SelectionStart = this.Text.Length; // Keep the cursor at the end
        }

        private void CharcharacterTextBox_Leave(object sender, EventArgs e)
        {
            // Optionally, you can clear the text when the control loses focus
            // this.Text = string.Empty;
        }

        private void CharcharacterTextBox_Enter(object sender, EventArgs e)
        {
            // Optionally, you can show the actual password when the control gains focus
            // this.Text = _password;
        }

        public string GetPassword()
        {
            return _password; // Method to retrieve the actual password
        }
    }
}
