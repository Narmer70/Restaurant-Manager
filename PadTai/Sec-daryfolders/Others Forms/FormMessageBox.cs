using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using PadTai.Classes;
using System.Windows.Forms;
using PadTai.Fastcheckfiles;
using PadTai.Sec_daryfolders.Updates;
using System.Runtime.InteropServices;


namespace PadTai.Sec_daryfolders.Others_Forms
{
    public partial class FormMessageBox : Form
    {
        private DraggableForm draggableForm;
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public FormMessageBox()
        {
            InitializeComponent();
        }

        //Fields
        private Color primaryColor = Color.DarkCyan;
        private int borderSize = 2;
        //Properties
        public Color PrimaryColor
        {
            get { return primaryColor; }
            set
            {
                primaryColor = value;
                this.panelTitleBar.BackColor = PrimaryColor;
            }
        }
        //Constructors
        public FormMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            InitializeComponent();
            initialiseResize();
            InitializeItems();
            SetIcon(icon);           
            this.labelMessage.Text = text;
            this.labelCaption.Text = caption;
            this.btnClose.BackColor = primaryColor;
        }
        //-> Private Methods
        private void InitializeItems()
        {
            this.Padding = new Padding(borderSize);
            this.button1.DialogResult = DialogResult.OK;
            this.FormBorderStyle = FormBorderStyle.None;
            this.btnClose.DialogResult = DialogResult.Cancel;
            button1.BackColor = button2.BackColor = primaryColor;
            this.button2.Visible = false;
        }
        private void initialiseResize()
        {
            draggableForm = new DraggableForm();
            draggableForm.EnableDragging(this, this);

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);
            resizer.RegisterControl(button1);
            resizer.RegisterControl(button2);
            resizer.RegisterControl(btnClose);
            resizer.RegisterControl(panelBody);
            resizer.RegisterControl(labelCaption);
            resizer.RegisterControl(labelMessage);
            resizer.RegisterControl(panelButtons);
            resizer.RegisterControl(panelTitleBar);
            resizer.RegisterControl(pictureBoxIcon);
        }
    
        private void SetIcon(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Error: //Error
                    this.pictureBoxIcon.Image = Properties.Resources.error;
                    PrimaryColor = Color.FromArgb(224, 79, 95);
                    break;
                case MessageBoxIcon.Information: //Information
                    this.pictureBoxIcon.Image = Properties.Resources.question;
                    PrimaryColor = Color.FromArgb(38, 191, 166);
                    break;
                case MessageBoxIcon.Question://Question
                    this.pictureBoxIcon.Image = Properties.Resources.question;
                    PrimaryColor = Color.FromArgb(10, 119, 232);
                    break;
                case MessageBoxIcon.Exclamation://Exclamation
                    this.pictureBoxIcon.Image = Properties.Resources.exclamation;
                    PrimaryColor = Color.FromArgb(255, 140, 0);
                    break;
                case MessageBoxIcon.None: //None
                    this.pictureBoxIcon.Image = Properties.Resources.chat;
                    PrimaryColor = Color.CornflowerBlue;
                    break;
            }
        }

        private void rjButton4_Click(object sender, EventArgs e)
        {
            this.Close();   
        }

        private void FormMessageBox_Load(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }

        private void FormMessageBox_Resize(object sender, EventArgs e)
        {
            if (resizer != null)
            {
                resizer.ResizeControls(this);
                fontResizer.AdjustFont(this);
            }
        }
    }
}


    //int widht = this.labelMessage.Width + this.pictureBoxIcon.Width + this.panelBody.Padding.Left;
    //int height = this.panelTitleBar.Height + this.labelMessage.Height + this.panelButtons.Height + this.panelBody.Padding.Top;
    //this.Size = new Size(widht, height);        