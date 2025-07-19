using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;


namespace PadTai.Classes
{
    internal class FontResizer
    {
        private readonly float _minFontSize;
        private readonly float _maxFontSize;
        private readonly float _sizeFactor;

        public FontResizer(float minFontSize = 10f, float maxFontSize = 10f, float sizeFactor = 15f)
        {
            _minFontSize = minFontSize;
            _maxFontSize = maxFontSize;
            _sizeFactor = sizeFactor;
        }

        public void AdjustFont(Control control)
        {
            if (control == null) return;

            // Calculate a new font size based on both the control's height and width
            float heightFactor = control.Height / _sizeFactor;
            float widthFactor = control.Width / _sizeFactor;

            // Use the smaller of the two to ensure the font fits within the control
            float newFontSize = Math.Max(_minFontSize, Math.Min(Math.Min(heightFactor, widthFactor), _maxFontSize));
            Font newFont = new Font(control.Font.FontFamily, newFontSize, control.Font.Style);
            // Adjust the font for the control itself
            control.Font = newFont;

            // Special handling for DataGridView headers
            if (control is DataGridView dgv)
            {
                // Store the previous width and height of the DataGridView
                int previousWidth = dgv.Width;
                int previousHeight = dgv.Height;

                // Adjust font sizes
                dgv.DefaultCellStyle.Font = newFont; // Resize cell font
                dgv.RowTemplate.Height = (int)(newFontSize * 1.5f); // Adjust row height based on font size

                // Calculate new width and height ratios
                float widthRatio = (float)dgv.Width / previousWidth;
                float heightRatio = (float)dgv.Height / previousHeight;

                // Resize header font and adjust column widths based on font size
                foreach (DataGridViewColumn column in dgv.Columns)
                {
                    column.HeaderCell.Style.Font = newFont; // Resize header font
                    column.Width = (int)(newFontSize * 10 * widthRatio); // Adjust width based on font size and previous width
                }

                // Ensure each row has the same height
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    row.Height = (int)(dgv.RowTemplate.Height * heightRatio); // Adjust row height based on previous height
                }

                dgv.Refresh(); // Refresh to apply changes
            }

            
            /*
            // Recursively adjust the font for child controls
            foreach (Control childControl in control.Controls)
            {
                AdjustFont(childControl);
            }
            */
            
        }

    }
}
