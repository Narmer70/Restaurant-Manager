namespace PadTai.Fastcheckfiles
{
    partial class Clientslistdisplay
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

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.roundedTextbox1 = new PadTai.Classes.Controlsdesign.RoundedTextbox();
            this.eclipseControl1 = new PadTai.Classes.Controlsdesign.EclipseControl();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(40)))), ((int)(((byte)(71)))));
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeight = 34;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(50)))), ((int)(((byte)(90)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.ControlDarkDark;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.EnableHeadersVisualStyles = false;
            this.dataGridView1.Location = new System.Drawing.Point(11, 60);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 62;
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(502, 662);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // roundedTextbox1
            // 
            this.roundedTextbox1.BackColor = System.Drawing.Color.Transparent;
            this.roundedTextbox1.BackColorRounded = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(50)))), ((int)(((byte)(90)))));
            this.roundedTextbox1.BorderColor = System.Drawing.Color.Transparent;
            this.roundedTextbox1.BorderSize = 0;
            this.roundedTextbox1.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roundedTextbox1.ForeColor = System.Drawing.Color.White;
            this.roundedTextbox1.Location = new System.Drawing.Point(105, 9);
            this.roundedTextbox1.Name = "roundedTextbox1";
            this.roundedTextbox1.PasswordChar = '\0';
            this.roundedTextbox1.Radius = 20;
            this.roundedTextbox1.ReadOnly = false;
            this.roundedTextbox1.SelectionLength = 0;
            this.roundedTextbox1.SelectionStart = 0;
            this.roundedTextbox1.Size = new System.Drawing.Size(313, 40);
            this.roundedTextbox1.TabIndex = 0;
            this.roundedTextbox1.TextAlignment = System.Windows.Forms.HorizontalAlignment.Left;
            this.roundedTextbox1.UseSystemPasswordChar = false;
            this.roundedTextbox1.WaterMark = null;
            this.roundedTextbox1.WaterMarkColor = System.Drawing.Color.Gray;
            this.roundedTextbox1.TextChanged += new System.EventHandler(this.roundedTextbox1_TextChanged);
            this.roundedTextbox1.Enter += new System.EventHandler(this.roundedTextbox1_Enter);
            this.roundedTextbox1.Leave += new System.EventHandler(this.roundedTextbox1_Leave);
            // 
            // eclipseControl1
            // 
            this.eclipseControl1.CornerRadius = 20;
            this.eclipseControl1.TargetControl = this.dataGridView1;
            // 
            // Clientslistdisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(40)))), ((int)(((byte)(71)))));
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.roundedTextbox1);
            this.Name = "Clientslistdisplay";
            this.Size = new System.Drawing.Size(520, 730);
            this.Load += new System.EventHandler(this.Clientslistdisplay_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Classes.Controlsdesign.RoundedTextbox roundedTextbox1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private Classes.Controlsdesign.EclipseControl eclipseControl1;
    }
}
