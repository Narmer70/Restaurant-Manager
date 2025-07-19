using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace PadTai.Classes.Controlsdesign
{

    [ToolboxBitmap(typeof(ComboBox))]
    [Description("Displays an editable text box and a drop-down list of permitted values.")]
    public class customBox: ComboBox
    {
        // Constants
        private const int WM_PAINT = 0x0F;

        private Color _BorderColor = Color.Gainsboro;
        private Color _ArrowColor = Color.CornflowerBlue;
        private Color _BorderFocusColor = Color.CornflowerBlue;

        public customBox()
        {
            this.SuspendLayout();
            this.MinimumSize = new Size(0, 23);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
           // this.DropDownStyle = ComboBoxStyle.DropDownList;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

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
        public Color ArrowColor
        {
            get { return _ArrowColor; }
            set
            {
                _ArrowColor = value;
                Invalidate();
            }
        }

        public void AppendText(string text)
        {
            int selectionStart = this.SelectionStart;
            this.Text = this.Text.Insert(selectionStart, text);
            this.SelectionStart = selectionStart + text.Length;
        }


        [Flags]
        private enum RedrawWindowFlags : uint
        {
            Invalidate = 0x1,
            InternalPaint = 0x2,
            Erase = 0x4,
            Validate = 0x8,
            NoInternalPaint = 0x10,
            NoErase = 0x20,
            NoChildren = 0x40,
            AllChildren = 0x80,
            UpdateNow = 0x100,
            EraseNow = 0x200,
            Frame = 0x400,
            NoFrame = 0x800
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        private static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, RedrawWindowFlags flags);

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            RedrawWindow(this.Handle, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.Frame | RedrawWindowFlags.UpdateNow | RedrawWindowFlags.Invalidate);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            SolidBrush skinBrush = new SolidBrush(BackColor);

            if (m.Msg == WM_PAINT)
            {
                IntPtr hDC = GetWindowDC(m.HWnd);
                using (Graphics g = Graphics.FromHdc(hDC))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    Rectangle clientArea = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                    Rectangle rectIcon = new Rectangle(this.Width - 16, (this.Height - 5) / 2, 10, 5);

                    // Draw surface
                    g.FillRectangle(skinBrush, clientArea);

                    // Draw Text
                    if (Enabled)
                    {
                        TextRenderer.DrawText(g, this.Text, this.Font, new Point(0, 3), this.ForeColor);
                    }
                    else
                    {
                        TextRenderer.DrawText(g, this.Text, this.Font, new Point(0, 3), SystemColors.GrayText);
                    }

                    using (GraphicsPath path = new GraphicsPath())
                    {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        path.AddLine(rectIcon.X, rectIcon.Y, rectIcon.X + (10.0F / 2), rectIcon.Bottom);
                        path.AddLine(rectIcon.X + (10.0F / 2), rectIcon.Bottom, rectIcon.Right, rectIcon.Y);
                        g.DrawPath(new Pen(_ArrowColor), path);
                    }

                    if (this.Focused)
                    {
                        g.DrawRectangle(new Pen(_BorderFocusColor), clientArea.X, clientArea.Y, clientArea.Width, clientArea.Height); // Focus color
                    }
                    else
                    {
                        g.DrawRectangle(new Pen(_BorderColor), clientArea.X, clientArea.Y, clientArea.Width, clientArea.Height); // Border
                    }
                }

                ReleaseDC(m.HWnd, hDC);
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                e.DrawBackground();
                e.Graphics.DrawString(this.Items[e.Index].ToString(), this.Font, Brushes.Black, e.Bounds, StringFormat.GenericDefault);
                e.DrawFocusRectangle();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // nothing
        }
    }
}
