namespace PadTai.Fastcheckfiles
{
    partial class Billsplitter
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.rjButton1 = new PadTai.Classes.RJButton();
            this.customBox1 = new PadTai.Classes.Controlsdesign.customBox();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 61);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(520, 669);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // rjButton1
            // 
            this.rjButton1.BackColor = System.Drawing.Color.MediumSlateBlue;
            this.rjButton1.BackgroundColor = System.Drawing.Color.MediumSlateBlue;
            this.rjButton1.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.rjButton1.BorderRadius = 0;
            this.rjButton1.BorderSize = 0;
            this.rjButton1.FlatAppearance.BorderSize = 0;
            this.rjButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rjButton1.ForeColor = System.Drawing.Color.White;
            this.rjButton1.Location = new System.Drawing.Point(344, 11);
            this.rjButton1.Name = "rjButton1";
            this.rjButton1.Size = new System.Drawing.Size(87, 33);
            this.rjButton1.TabIndex = 1;
            this.rjButton1.TabStop = false;
            this.rjButton1.Text = "Start";
            this.rjButton1.TextColor = System.Drawing.Color.White;
            this.rjButton1.UseVisualStyleBackColor = false;
            // 
            // customBox1
            // 
            this.customBox1.ArrowColor = System.Drawing.Color.CornflowerBlue;
            this.customBox1.BorderColor = System.Drawing.Color.Gainsboro;
            this.customBox1.BorderFocusColor = System.Drawing.Color.CornflowerBlue;
            this.customBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.customBox1.FormattingEnabled = true;
            this.customBox1.Location = new System.Drawing.Point(112, 14);
            this.customBox1.Name = "customBox1";
            this.customBox1.Size = new System.Drawing.Size(184, 28);
            this.customBox1.TabIndex = 0;
            // 
            // Billsplitter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.rjButton1);
            this.Controls.Add(this.customBox1);
            this.Name = "Billsplitter";
            this.Size = new System.Drawing.Size(520, 730);
            this.ResumeLayout(false);

        }

        #endregion

        private Classes.Controlsdesign.customBox customBox1;
        private Classes.RJButton rjButton1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
