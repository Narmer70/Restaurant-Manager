using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;


namespace PadTai.Classes.Others
{
    public class ImageResizer
    {
        public static Image ResizeImage(Image img, int width, int height)
        {
            // Calculate the new dimensions while maintaining the aspect ratio
            double ratioX = (double)width / img.Width;
            double ratioY = (double)height / img.Height;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(img.Width * ratio);
            int newHeight = (int)(img.Height * ratio);

            // Create a new bitmap with the new dimensions
            Bitmap newImage = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(newImage))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(img, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }
    }
}
