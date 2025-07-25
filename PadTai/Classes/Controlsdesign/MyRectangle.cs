﻿using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;


namespace PadTai.Classes.Controlsdesign
{
    internal class MyRectangle
    {
        private float x,y,width,height,radius;
        private GraphicsPath graphicsPath;
        private Point location;


        public MyRectangle(float width, float height, float radius,float x=0F, float y=0F)
        {
            this.location = new Point(0,0);
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.radius = radius;
            graphicsPath = new GraphicsPath();

            if (radius <= 0F)
            {
                graphicsPath.AddRectangle(new RectangleF(x, y, width, height));
            }
            else 
            {
                graphicsPath.AddArc(new RectangleF(x, y, 2F * radius, 2F * radius),180F, 90F);
                graphicsPath.AddArc(new RectangleF(width -(2F*radius)-1F,x,2F * radius, 2F * radius), 270F, 90F);
                graphicsPath.AddArc(new RectangleF(width - (2F * radius) - 1F, height -(2F * radius)-1F, 2F * radius, 2F * radius),0F, 90F);
                graphicsPath.AddArc(new RectangleF(x,height - (2F * radius) - 1F, 2F * radius, 2F * radius), 90F, 90F);
                graphicsPath.CloseAllFigures();
            }
        }

        public GraphicsPath path => graphicsPath;

        public RectangleF Rect => new RectangleF(x, y, width,height);

        public float Radius
        {  
            get => radius; 
            set => radius = value;
        }
    }
}
