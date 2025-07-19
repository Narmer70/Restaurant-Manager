namespace PadTai
{
    partial class Login
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button14 = new PadTai.Classes.RJButton();
            this.button13 = new PadTai.Classes.RJButton();
            this.PasswordText = new PadTai.Classes.Controlsdesign.RoundedTextbox();
            this.UsernameText = new PadTai.Classes.Controlsdesign.RoundedTextbox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label1.Location = new System.Drawing.Point(105, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "Логин";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label2.Location = new System.Drawing.Point(105, 162);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 25);
            this.label2.TabIndex = 4;
            this.label2.Text = "Пароль";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(139, 411);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(171, 20);
            this.label3.TabIndex = 12;
            this.label3.Text = "Forgot your password?";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(315, 411);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 20);
            this.label4.TabIndex = 13;
            this.label4.Text = "Click here";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            this.label4.MouseEnter += new System.EventHandler(this.label4_MouseEnter);
            this.label4.MouseLeave += new System.EventHandler(this.label4_MouseLeave);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Century Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(51, 498);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(0, 23);
            this.label5.TabIndex = 12;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.BackColor = System.Drawing.Color.Transparent;
            this.checkBox1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.checkBox1.Location = new System.Drawing.Point(109, 238);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(169, 24);
            this.checkBox1.TabIndex = 7;
            this.checkBox1.Text = "Скрывать пароль";
            this.checkBox1.UseVisualStyleBackColor = false;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // button14
            // 
            this.button14.BackColor = System.Drawing.Color.White;
            this.button14.BackgroundColor = System.Drawing.Color.White;
            this.button14.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.button14.BorderRadius = 8;
            this.button14.BorderSize = 0;
            this.button14.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button14.FlatAppearance.BorderSize = 0;
            this.button14.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button14.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button14.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button14.Location = new System.Drawing.Point(460, 11);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(75, 47);
            this.button14.TabIndex = 9;
            this.button14.TabStop = false;
            this.button14.Text = "❌";
            this.button14.TextColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button14.UseVisualStyleBackColor = false;
            this.button14.Click += new System.EventHandler(this.button14_Click);
            // 
            // button13
            // 
            this.button13.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button13.BackgroundColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button13.BorderColor = System.Drawing.Color.Black;
            this.button13.BorderRadius = 8;
            this.button13.BorderSize = 3;
            this.button13.FlatAppearance.BorderSize = 0;
            this.button13.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(24)))), ((int)(((byte)(26)))));
            this.button13.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.WindowText;
            this.button13.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button13.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button13.ForeColor = System.Drawing.Color.White;
            this.button13.Location = new System.Drawing.Point(109, 336);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(334, 70);
            this.button13.TabIndex = 10;
            this.button13.TabStop = false;
            this.button13.Text = "Потвердите";
            this.button13.TextColor = System.Drawing.Color.White;
            this.button13.UseVisualStyleBackColor = false;
            this.button13.Click += new System.EventHandler(this.button13_Click);
            // 
            // PasswordText
            // 
            this.PasswordText.BackColor = System.Drawing.Color.Transparent;
            this.PasswordText.BackColorRounded = System.Drawing.Color.LightGray;
            this.PasswordText.BorderColor = System.Drawing.Color.Transparent;
            this.PasswordText.BorderSize = 0;
            this.PasswordText.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PasswordText.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.PasswordText.Location = new System.Drawing.Point(109, 188);
            this.PasswordText.Name = "PasswordText";
            this.PasswordText.PasswordChar = '\0';
            this.PasswordText.Radius = 10;
            this.PasswordText.ReadOnly = false;
            this.PasswordText.SelectionLength = 0;
            this.PasswordText.SelectionStart = 0;
            this.PasswordText.Size = new System.Drawing.Size(334, 45);
            this.PasswordText.TabIndex = 2;
            this.PasswordText.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            this.PasswordText.UseSystemPasswordChar = false;
            this.PasswordText.WaterMark = null;
            this.PasswordText.WaterMarkColor = System.Drawing.Color.Gray;
            this.PasswordText.Enter += new System.EventHandler(this.PasswordText_Enter);
            this.PasswordText.Leave += new System.EventHandler(this.PasswordText_Leave);
            // 
            // UsernameText
            // 
            this.UsernameText.BackColor = System.Drawing.Color.Transparent;
            this.UsernameText.BackColorRounded = System.Drawing.Color.LightGray;
            this.UsernameText.BorderColor = System.Drawing.Color.Transparent;
            this.UsernameText.BorderSize = 0;
            this.UsernameText.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UsernameText.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.UsernameText.Location = new System.Drawing.Point(109, 99);
            this.UsernameText.Name = "UsernameText";
            this.UsernameText.PasswordChar = '\0';
            this.UsernameText.Radius = 10;
            this.UsernameText.ReadOnly = false;
            this.UsernameText.SelectionLength = 0;
            this.UsernameText.SelectionStart = 0;
            this.UsernameText.Size = new System.Drawing.Size(334, 45);
            this.UsernameText.TabIndex = 1;
            this.UsernameText.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            this.UsernameText.UseSystemPasswordChar = false;
            this.UsernameText.WaterMark = null;
            this.UsernameText.WaterMarkColor = System.Drawing.Color.Gray;
            this.UsernameText.Enter += new System.EventHandler(this.UsernameText_Enter);
            this.UsernameText.Leave += new System.EventHandler(this.UsernameText_Leave);
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(548, 497);
            this.ControlBox = false;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button14);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button13);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PasswordText);
            this.Controls.Add(this.UsernameText);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Login_Load);
            this.Shown += new System.EventHandler(this.Login_Shown);
            this.Click += new System.EventHandler(this.Login_Click);
            this.Resize += new System.EventHandler(this.Login_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Classes.RJButton button13;
        private Classes.RJButton button14;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;        
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox checkBox1;
        private Classes.Controlsdesign.RoundedTextbox UsernameText;
        private Classes.Controlsdesign.RoundedTextbox PasswordText;    
    }
}

