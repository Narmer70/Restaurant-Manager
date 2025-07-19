using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace PadTai.Classes.Controlsdesign
{
    public class RoundedTextbox : Control
    {
        private int borderSize = 0;
        private int radius = 50;
        private Color borderColor = Color.Transparent;
        private Color backColor;
        private Color waterMarkColor = Color.Gray;
        private Color mColor = Color.Transparent;
        private string waterMark;
        private GraphicsPath shape;
        private GraphicsPath innerRect;
        public TextBox textbox = new TextBox();
        private HorizontalAlignment textAlignment = HorizontalAlignment.Left;


        public RoundedTextbox()
        {
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.ResizeRedraw, true);

            base.Controls.Add(textbox);
            base.Size = new Size(200, 50);
            textbox.Parent = this;
            textbox.BorderStyle = BorderStyle.None;
            textbox.BackColor = backColor;
            BackColor = Color.Transparent;
            backColor = Color.White;
            Font = new Font("Century Gothic", 12F);
            DoubleBuffered = true;
            textbox.TextAlign = textAlignment; 

            textbox.KeyDown += new KeyEventHandler(textbox_KeyDown);
            textbox.TextChanged += new EventHandler(textbox_TextChanged);
            textbox.MouseDoubleClick += new MouseEventHandler(textbox_MouseDoubleClick);
            textbox.MouseClick += new MouseEventHandler(textbox_MouseClick);
            textbox.Leave += new EventHandler(textbox_Leave);
            textbox.KeyPress += new KeyPressEventHandler(textbox_KeyPress);

        }

        private void textbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
        }

        private void textbox_Leave(object sender, EventArgs e)
        {
            if (Text == waterMark || Text == string.Empty)
            {
                Text = waterMark;
                textbox.ForeColor = waterMarkColor;
            }
            else
            {
                textbox.ForeColor = mColor;
            }
        }

        private void textbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                textbox.SelectAll();
            }
        }

        private void textbox_MouseClick(object sender, MouseEventArgs e)
        {
            if (Text == waterMark || Text == string.Empty)
            {
                Text = string.Empty;
                textbox.ForeColor = mColor;
            }
        }

        private void textbox_TextChanged(object sender, EventArgs e)
        {
            Text = textbox.Text;
        }

        private void textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.A))
            {
                textbox.SelectionStart = 0;
                textbox.SelectionLength = Text.Length;
            }
        }

        public HorizontalAlignment TextAlignment
        {
            get { return textAlignment; }
            set
            {
                textAlignment = value;
                textbox.TextAlign = textAlignment; 
                Invalidate(); 
            }
        }

        public bool ReadOnly
        {
            get { return textbox.ReadOnly; }
            set { textbox.ReadOnly = value; }
        }

        public char PasswordChar
        {
            get { return textbox.PasswordChar; }
            set { textbox.PasswordChar = value; base.Invalidate(); }
        }

        public bool UseSystemPasswordChar
        {
            get { return textbox.UseSystemPasswordChar; }
            set { textbox.UseSystemPasswordChar = value; base.Invalidate(); }
        }

        public Color BackColorRounded
        {
            get { return backColor; }
            set
            {
                backColor = value;
                if (backColor != Color.Transparent) { textbox.BackColor = backColor; }
                base.Invalidate();
            }
        }

        public override Color BackColor
        {
            get => base.BackColor;
            set => base.BackColor = Color.Transparent;
        }

        public Color BorderColor
        {
            get => borderColor;
            set { borderColor = value; Invalidate(); }
        }

        public Color WaterMarkColor
        {
            get => waterMarkColor;
            set { waterMarkColor = value; Invalidate(); }
        }

        public int Radius
        {
            get => radius;
            set { radius = value; Invalidate(); }
        }
        public int BorderSize
        {
            get => borderSize;
            set { borderSize = value; Invalidate(); }
        }

        public string WaterMark
        {
            get => waterMark;
            set
            {
                waterMark = value;
                if (Text == waterMark || Text == string.Empty)
                {
                    Text = waterMark;
                    textbox.ForeColor = waterMarkColor;
                }
                else textbox.ForeColor = mColor;
                Invalidate();
            }
        }

        public int SelectionStart
        {
            get { return textbox.SelectionStart; }
            set { textbox.SelectionStart = value; }
        }

        public int SelectionLength
        {
            get { return textbox.SelectionLength; }
            set { textbox.SelectionLength = value; }
        }

        public void AppendText(string text)
        {
            if (textbox != null)
            {
                int selectionStart = textbox.SelectionStart;
                textbox.Text = textbox.Text.Insert(selectionStart, text);
                textbox.SelectionStart = selectionStart + text.Length;
            }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            textbox.Font = Font;
            base.Invalidate();
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            textbox.ForeColor = mColor = ForeColor;
            base.Invalidate();
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            textbox.Text = Text;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Validate dimensions
            if (base.Width <= 0 || base.Height <= 0)
                return; // Prevent painting if dimensions are invalid

            // Ensure radius is a valid value
            if (radius < 0)
                radius = 0; // Set to zero if negative

            try
            {
                shape = new MyRectangle((float)base.Width, (float)base.Height, (float)radius / 2, 0F, 0F).path;
                innerRect = new MyRectangle(base.Width - 0.5F, base.Height - 0.5F, (float)radius / 2, 0.5F, 0.5F).path;
                Pen pen = new Pen(BorderColor, BorderSize);

                if (textbox.Height >= (base.Height - 4))
                    base.Height = textbox.Height + 4;

                textbox.Location = new Point(Radius - 5, (base.Height / 2) - (textbox.Font.Height / 2));
                textbox.Width = base.Width - ((int)(radius * 1.5));
                e.Graphics.SmoothingMode = (SmoothingMode.HighQuality);
                e.Graphics.DrawPath(pen, shape);

                using (SolidBrush brush = new SolidBrush(backColor))
                {
                    e.Graphics.FillPath((Brush)brush, innerRect);
                }

                Trans.MakeTransparent(this, e.Graphics);
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            base.OnPaint(e);
        }
    }
}
