using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;


namespace PadTai.Classes
{
    public class ControlResizer
    {
        private Size originalFormSize;
        private Dictionary<Control, Rectangle> originalControlSizes;


        public ControlResizer(Size formSize)
        {
            originalFormSize = formSize;
            originalControlSizes = new Dictionary<Control, Rectangle>();
        }

        public void RegisterControl(Control control)
        {
            if (control == null) throw new ArgumentNullException(nameof(control));

            // Store the original size and location of the control
            originalControlSizes[control] = new Rectangle(control.Location, control.Size);           
        }

        public void ResizeControls(Control parentControl)
        {
            if (parentControl == null) throw new ArgumentNullException(nameof(parentControl));

            EnableDoubleBuffer();

            float xRatio = (float)(parentControl.Width) / (float)(originalFormSize.Width);
            float yRatio = (float)(parentControl.Height) / (float)(originalFormSize.Height);

            foreach (var entry in originalControlSizes)
            {
                Control control = entry.Key;
                Rectangle originalRect = entry.Value;

                int newX = (int)(originalRect.X * xRatio);
                int newY = (int)(originalRect.Y * yRatio);
                int newWidth = (int)(originalRect.Width * xRatio);
                int newHeight = (int)(originalRect.Height * yRatio);

                control.Location = new Point(newX, newY);
                control.Size = new Size(newWidth, newHeight);
            }
        }

        public void EnableDoubleBuffer()
        {
            foreach (var control in originalControlSizes.Keys)
            {
                SetDoubleBuffer(control, true);
            }
        }

        static void SetDoubleBuffer(Control control, bool DoubleBuffered) 
        {
            try 
            {
                typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, control, new object[] { DoubleBuffered });
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
