using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Collections.Generic;


namespace PadTai.Classes.Controlsdesign
{
    public class RoundedPanel : Panel
    {
        private int borderRadius = 30;
        private float gradientAngle = 90F;
        private Color borderColor = Color.Gray; 
        private Color gradientTopColor = Color.DodgerBlue;
        private Color gradientBottomColor = Color.CadetBlue;

        public RoundedPanel() 
        {
           this.BackColor = Color.White;
           this.ForeColor = Color.Black;
           this.Size = new Size(350,200);          
        }

        public int BorderRadius 
        { 
            get => borderRadius;
            set {borderRadius = value; this.Invalidate();} 
        }

        public float GradientAngle 
        {
            get => gradientAngle;
            set {gradientAngle = value; this.Invalidate(); }
        }

        public Color GradientTopColor 
        {
            get => gradientTopColor;
            set {gradientTopColor = value; this.Invalidate(); }
        }

        public Color GradientBottomColor 
        {
            get => gradientBottomColor;
            set {gradientBottomColor = value; this.Invalidate(); }
        }

        public Color BorderColor 
        {
            get => borderColor;
            set { borderColor = value; this.Invalidate(); }
        }

        private GraphicsPath GetRoundedPath(RectangleF rectangle,float radius) 
        {
            GraphicsPath graphicsPath = new GraphicsPath();
            graphicsPath.StartFigure();
            graphicsPath.AddArc(rectangle.Width - radius, rectangle.Height - radius,radius,radius,0,90);
            graphicsPath.AddArc(rectangle.X, rectangle.Height - radius, radius, radius, 90, 90);
            graphicsPath.AddArc(rectangle.X, rectangle.Y, radius, radius, 180, 90);
            graphicsPath.AddArc(rectangle.Width - radius, rectangle.Y, radius, radius, 270, 90);
            graphicsPath.CloseFigure();
            return graphicsPath;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
          
            // Check if the width and height are greater than zero
            if (this.Width > 0 && this.Height > 0)
            {
                LinearGradientBrush Roundedbrush = new LinearGradientBrush(this.ClientRectangle, this.GradientTopColor, this.gradientBottomColor, this.gradientAngle);
                Graphics Roundedgraphics = e.Graphics;
                Roundedgraphics.FillRectangle(Roundedbrush, ClientRectangle);

                RectangleF rectangleF = new RectangleF(0, 0, this.Width, this.Height);

                if (borderRadius > 2)
                {
                    using (GraphicsPath graphicsPath = GetRoundedPath(rectangleF, borderRadius))
                    using (Pen pen = new Pen(this.BorderColor, 2))
                    {
                        this.Region = new Region(graphicsPath);
                        e.Graphics.DrawPath(pen, graphicsPath);
                    }
                }
                else
                {
                    this.Region = new Region(rectangleF);
                }
            }
        }
    }
}
