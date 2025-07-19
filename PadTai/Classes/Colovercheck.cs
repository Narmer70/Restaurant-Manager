using System;
using System.Drawing;
using System.Windows.Forms;

namespace PadTai.Classes
{
    internal class Colovercheck : CheckBox
    {
        public Color BorderColor { get; set; } = Color.DodgerBlue;
        public string Identifier { get; set; }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Pen pen = new Pen(BorderColor))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate(); // Redraw the control
        }
    }
}
