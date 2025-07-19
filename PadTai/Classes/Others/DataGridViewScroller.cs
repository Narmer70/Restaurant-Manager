using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace PadTai.Classes.Others
{
    public class DataGridViewScroller
    {
        private DataGridView[] dataGridViews;
        private Control[] focusableControls;
        private Control parentControl;

        public DataGridViewScroller(Control parent, Control[] focusableControls = null, params DataGridView[] dataGridViews)
        {
            this.parentControl = parent;
            this.dataGridViews = dataGridViews;
            this.focusableControls = focusableControls ?? Array.Empty<Control>();
           
            // Subscribe to the parent control's MouseWheel event
            parentControl.MouseWheel += ParentControl_MouseWheel;

            // Update event subscriptions based on the current setting
            UpdateEventSubscriptions();
        }

        private void UpdateEventSubscriptions()
        {
            // Subscribe to MouseEnter and MouseDown events for DataGridViews
            foreach (var dgv in dataGridViews)
            {
                if (!Properties.Settings.Default.showKeyboard)
                {
                    dgv.MouseEnter += Control_MouseEnter;
                    dgv.MouseDown += Control_MouseDown;
                }
                else
                {
                    dgv.MouseEnter -= Control_MouseEnter;
                    dgv.MouseDown -= Control_MouseDown;
                }
            }

            // Subscribe to MouseEnter and MouseDown events for additional controls
            foreach (var control in focusableControls)
            {
                if (!Properties.Settings.Default.showKeyboard)
                {
                    control.MouseEnter += Control_MouseEnter;
                    control.MouseDown += Control_MouseDown;
                }
                else
                {
                    control.MouseEnter -= Control_MouseEnter;
                    control.MouseDown -= Control_MouseDown;
                }
            }
        }

        // Method to update the focus behavior based on the toggle switch
        public void UpdateFocusBehavior(bool showKeyboard)
        {
            Properties.Settings.Default.showKeyboard = showKeyboard;
            Properties.Settings.Default.Save();
            UpdateEventSubscriptions();
        }

        private void ParentControl_MouseWheel(object sender, MouseEventArgs e)
        {
            OnMouseWheel(e);
        }

        protected void OnMouseWheel(MouseEventArgs e)
        {
            foreach (var dgv in dataGridViews)
            {
                if (dgv.Focused)
                {
                    int newIndex = dgv.FirstDisplayedScrollingRowIndex + (e.Delta > 0 ? -1 : 1);
                    if (newIndex >= 0 && newIndex < dgv.Rows.Count)
                    {
                        try
                        {
                            dgv.FirstDisplayedScrollingRowIndex = newIndex;
                        }
                        catch (InvalidOperationException)
                        {
                            // Handle exception if needed
                        }
                    }
                    break; // Exit after handling the focused DataGridView
                }
            }
        }

        private void Control_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Control control)
            {
                control.Focus();
            }
        }

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is Control control)
            {
                control.Focus();
            }
        }
    }
}
