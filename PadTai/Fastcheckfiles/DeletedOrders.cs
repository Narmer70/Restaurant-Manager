using System;
using System.Data;
using System.Linq;
using System.Text;
using PadTai.Classes;
using System.Drawing;
using System.Windows.Forms;
using PadTai.Classes.Others;
using PadTai.Classes.Databaselink;


namespace PadTai.Fastcheckfiles
{
    public partial class DeletedOrders : UserControl
    {
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;

        public DeletedOrders()
        {
            InitializeComponent();
            crudDatabase = new CrudDatabase();

            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            LocalizeControls();
            ApplyTheme();
        }  
        
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (resizer != null)
            {
                resizer.ResizeControls(this);
            }
        }

        public void LocalizeControls()
        {
            //button1.Text = LanguageManager.Instance.GetString("MF-btn1");
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;
        }
    }
}
