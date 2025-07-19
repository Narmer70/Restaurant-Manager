using System;
using System.Drawing;
using System.Windows.Forms;

namespace PadTai.Classes
{
    public class FormResizer
    {
        private readonly Size originalSize;

        public FormResizer(Form form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            // Store the original size of the form
            this.originalSize = form.ClientSize; // Use ClientSize to get the usable area of the form
        }

        public void Resize(Form form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            // Get the working area of the screen (excluding taskbar)
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;

            // Calculate the width and height ratios
            float widthRatio = (float)workingArea.Width / originalSize.Width;
            float heightRatio = (float)workingArea.Height / originalSize.Height;

            // Use the smaller ratio to maintain aspect ratio
            float scaleFactor = Math.Min(widthRatio, heightRatio);

            // Calculate new size
            int newWidth = (int)(originalSize.Width * scaleFactor);
            int newHeight = (int)(originalSize.Height * scaleFactor);

            // Set the new size
            form.Size = new Size(newWidth, newHeight);

            // Center the form within the working area
            form.StartPosition = FormStartPosition.Manual; // Set to Manual to allow custom positioning
            form.Location = new Point(
                (workingArea.Width - newWidth) / 2 + workingArea.Left,
                (workingArea.Height - newHeight) / 2 + workingArea.Top);
        }
    }
}