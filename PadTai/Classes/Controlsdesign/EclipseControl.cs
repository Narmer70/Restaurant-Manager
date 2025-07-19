using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace PadTai.Classes.Controlsdesign
{
    public class EclipseControl:Component
    {
        [DllImport("Gdi32.dll",EntryPoint = "CreateRoundRectRgn")]

        public static extern IntPtr CreateRoundRectRgn(int nL, int mT, int nR, int nB, int nWidthEllipse, int nHeightEclipse);

        private Control control;
        private int cornerRadius = 25;

        public Control TargetControl 
        {
            get {return control;} 
            set 
            {
                control = value; 
                control.SizeChanged += (sender, eventArgs) => control.Region = Region.FromHrgn(CreateRoundRectRgn(0,0,control.Width,control.Height,cornerRadius,cornerRadius));
            }
        }

        public int CornerRadius 
        {
            get { return cornerRadius; }
            set 
            {
                cornerRadius = value;
                if (control != null)
                control.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, control.Width, control.Height, cornerRadius, cornerRadius));
            }
        }
    }
}
