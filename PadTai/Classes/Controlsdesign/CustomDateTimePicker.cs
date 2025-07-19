using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace PadTai.Classes.Controlsdesign
{
    [ToolboxBitmap(typeof(DateTimePicker))]
    [Description("Displays a date and time picker with custom styling.")]
    public class CustomDateTimePicker : DateTimePicker
    {
        // Constants
        private const int WM_PAINT = 0x0F;

        private Color _BorderColor = Color.Gainsboro;
        private Color _BorderFocusColor = Color.CornflowerBlue;
        private Color _TextColor = Color.Black;
        private Color _BackColor = Color.White;
        private Image _CalendarIcon = Properties.Resources.calendarWhite; // Ensure you have this resource
        private bool droppedDown = false;
        private bool _IconVisible = true; // Default to true
        private int _selectedPart = -1; // 0: Hours, 1: Minutes, 2: Seconds

        // Parameterless constructor for design-time support
        public CustomDateTimePicker() : this(DateTimePickerFormat.Long) // Default to Long format
        {
        }

        // Constructor that allows setting the initial format
        public CustomDateTimePicker(DateTimePickerFormat format)
        {
            this.SuspendLayout();
            this.MinimumSize = new Size(0, 24);
            this.Format = format; // Set the format passed in

            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
           // SetStyle(ControlStyles.UserPaint, true); // Allow custom painting

            this.ResumeLayout();
        }

        [Category("MControl")]
        public Color BorderColor
        {
            get { return _BorderColor; }
            set
            {
                _BorderColor = value;
                Invalidate();
            }
        }

        [Category("MControl")]
        public Color BorderFocusColor
        {
            get { return _BorderFocusColor; }
            set
            {
                _BorderFocusColor = value;
                Invalidate();
            }
        }

        [Category("MControl")]
        public Color TextColor
        {
            get { return _TextColor; }
            set
            {
                _TextColor = value;
                Invalidate();
            }
        }

        [Category("MControl")]
        public Color BackColorCustom
        {
            get { return _BackColor; }
            set
            {
                _BackColor = value;
                Invalidate();
            }
        }

        [Category("MControl")]
        public Image CalendarIcon
        {
            get { return _CalendarIcon; }
            set
            {
                _CalendarIcon = value;
                Invalidate();
            }
        }

        // New property for icon visibility
        [Category("MControl")]
        public bool IconVisible
        {
            get { return _IconVisible; }
            set
            {
                _IconVisible = value;
                Invalidate(); // Redraw the control when the property changes
            }
        }


        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_PAINT)
            {
                using (Graphics g = CreateGraphics())
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    Rectangle clientArea = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

                    // Define padding values
                    int topPadding = 3;    // Adjust this value for top padding
                    int bottomPadding = 5; // Adjust this value for bottom padding
                    int iconHeight = 15;   // Height of the icon

                    // Adjust the icon area with padding
                    int yPosition = (this.Height - iconHeight - topPadding - bottomPadding) / 2 + topPadding;
                    Rectangle iconArea = new Rectangle(this.Width - 22, yPosition, 16, iconHeight);

                    // Draw background
                    using (SolidBrush backBrush = new SolidBrush(_BackColor))
                    {
                        g.FillRectangle(backBrush, clientArea);
                    }

                    // Draw Text aligned to the left
                    Point textLocation = new Point(5, (this.Height - this.Font.Height) / 2); // 5 pixels padding from the left
                    TextRenderer.DrawText(g, this.Value.ToString(this.CustomFormat), this.Font, textLocation, _TextColor);

                    // Draw calendar icon
                    if (_IconVisible && _CalendarIcon != null)
                    {
                        g.DrawImage(_CalendarIcon, iconArea);
                    }

                    // Draw border
                    using (Pen borderPen = new Pen(this.Focused ? _BorderFocusColor : _BorderColor))
                    {
                        g.DrawRectangle(borderPen, clientArea.X, clientArea.Y, clientArea.Width - 1, clientArea.Height - 1);
                    }

                    // Draw overlay when dropdown is open
                    if (droppedDown)
                    {
                        using (SolidBrush openIconBrush = new SolidBrush(Color.FromArgb(50, 64, 64, 64)))
                        {
                            g.FillRectangle(openIconBrush, iconArea);
                        }
                    }
                }
            }
        }

        protected override void OnDropDown(EventArgs eventargs)
        {
            base.OnDropDown(eventargs);
            droppedDown = true;
        }

        protected override void OnCloseUp(EventArgs eventargs)
        {
            base.OnCloseUp(eventargs);
            droppedDown = false;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            // Allow text input
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar))
            {
                if (e.KeyChar == (char)Keys.Back)
                {
                    // Handle backspace
                    if (this.Text.Length > 0)
                    {
                        this.Text = this.Text.Substring(0, this.Text.Length - 1);
                    }
                }
                else
                {
                    // Append the character to the text
                    if (_selectedPart == -1) return; // No part selected

                    // Modify the selected part based on the current selection
                    string[] timeParts = this.Text.Split(':');
                    if (timeParts.Length == 3)
                    {
                        if (_selectedPart == 0) // Hours
                        {
                            timeParts[0] += e.KeyChar;
                        }
                        else if (_selectedPart == 1) // Minutes
                        {
                            timeParts[1] += e.KeyChar;
                        }
                        else if (_selectedPart == 2) // Seconds
                        {
                            timeParts[2] += e.KeyChar;
                        }

                        // Reconstruct the time string
                        this.Text = string.Join(":", timeParts);
                    }
                }

                // Validate and update the DateTime value
                if (DateTime.TryParse(this.Text, out DateTime parsedDateTime))
                {
                    this.Value = parsedDateTime;
                }

                e.Handled = true; // Prevent further processing of the key press
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            // Determine which part of the time is clicked
            string[] timeParts = this.Text.Split(':');
            if (timeParts.Length == 3)
            {
                int hourWidth = TextRenderer.MeasureText(timeParts[0], this.Font).Width;
                int minuteWidth = TextRenderer.MeasureText(timeParts[1], this.Font).Width;

                if (e.X < hourWidth) // Clicked on hours
                {
                    _selectedPart = 0;
                }
                else if (e.X < hourWidth + minuteWidth + 2) // Clicked on minutes
                {
                    _selectedPart = 1;
                }
                else // Clicked on seconds
                {
                    _selectedPart = 2;
                }
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            // Validate the text and update the DateTime value
            if (DateTime.TryParse(this.Text, out DateTime parsedDateTime))
            {
                this.Value = parsedDateTime;
            }
        }


        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Prevent flickering
        }

        // Method to change the format of the DateTimePicker
        public void SetDateFormat(DateTimePickerFormat format)
        {
            this.Format = format;
            Invalidate();
        }
    }
}