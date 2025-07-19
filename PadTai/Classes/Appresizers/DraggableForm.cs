using System;
using System.Drawing;
using System.Windows.Forms;

namespace PadTai.Classes
{
    public class DraggableForm
    {
        private const int RESIZE_BORDER = 10; // Define the resize border area
        private bool dragging = false; // To track dragging state
        private bool resizing = false; // To track resizing state
        private Point dragCursorPoint; // To store the cursor position during dragging
        private Size dragFormSize; // To store the form's size during resizing
        private Point dragFormLocation; // To store the form's location during dragging

        public void EnableDragging(Control control, Form form)
        {
            // Attach mouse event handlers to the control
            control.MouseDown += (s, e) => Control_MouseDown(s, e, form);
            control.MouseMove += (s, e) => Control_MouseMove(s, e, form);
            control.MouseUp += Control_MouseUp;
        }

        private void Control_MouseDown(object sender, MouseEventArgs e, Form form)
        {
            // Check if the left mouse button is pressed
            if (e.Button == MouseButtons.Left)
            {
                if (IsInResizeArea(e.Location, sender as Control))
                {
                    resizing = true; // Set resizing to true
                    dragCursorPoint = Cursor.Position; // Get the current cursor position
                    dragFormSize = form.Size; // Get the current form size
                }
                else
                {
                    dragging = true; // Set dragging to true
                    dragCursorPoint = Cursor.Position; // Get the current cursor position
                    dragFormLocation = form.Location; // Get the current form location
                }
            }
        }

        private void Control_MouseMove(object sender, MouseEventArgs e, Form form)
        {
            Control control = sender as Control;

            // If resizing is true, resize the form
            if (resizing)
            {
                Point cursorDifference = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                int newWidth = Math.Max(100, dragFormSize.Width + cursorDifference.X); // Minimum width
                int newHeight = Math.Max(100, dragFormSize.Height + cursorDifference.Y); // Minimum height
                form.Size = new Size(newWidth, newHeight); // Update form size
            }
            else if (dragging)
            {
                Point cursorDifference = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                form.Location = Point.Add(dragFormLocation, new Size(cursorDifference)); // Update form location
            }
            else if (IsInResizeArea(e.Location, control))
            {
                // Change the cursor to indicate resizing
                control.Cursor = Cursors.SizeNWSE; // Change to appropriate cursor
            }
            else
            {
                control.Cursor = Cursors.Default; // Reset cursor
            }
        }

        private bool IsInResizeArea(Point mousePosition, Control control)
        {
            // Check if the mouse position is within the resize border
            return mousePosition.X <= RESIZE_BORDER ||
                   mousePosition.X >= control.Width - RESIZE_BORDER ||
                   mousePosition.Y <= RESIZE_BORDER ||
                   mousePosition.Y >= control.Height - RESIZE_BORDER;
        }

        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
            // Stop dragging or resizing when the mouse button is released
            if (e.Button == MouseButtons.Left)
            {
                dragging = false; // Set dragging to false
                resizing = false; // Set resizing to false
            }
        }
    }
}


