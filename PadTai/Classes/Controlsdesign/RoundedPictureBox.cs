using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;


namespace PadTai.Classes.Controlsdesign
{
    public class RoundedPictureBox : PictureBox
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            GraphicsPath g = new GraphicsPath();
            g.AddEllipse(0, 0, ClientSize.Width, ClientSize.Height);
            this.Region = new System.Drawing.Region(g);
            base.OnPaint(e);
        }
    }
}
