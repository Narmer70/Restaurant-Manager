using System;
using System.Data;
using System.Linq;
using System.Text;
using PadTai.Classes;
using System.Windows.Forms;
using PadTai.Classes.Others;
using PadTai.Classes.Databaselink;
using PadTai.Sec_daryfolders.Others;
using PadTai.Classes.Controlsdesign;


namespace PadTai.Sec_daryfolders.Allreports.Otdelreports
{
    public partial class OtdelReturntable : UserControl
    {
        private Clavieroverlay clavieroverlay;
        private CrudDatabase crudDatabase;
        private FontResizer fontResizer;
        private ControlResizer resizer;
        private DataTable dataTable;
        private string currentState;

        public OtdelReturntable()
        {
            InitializeComponent();
            InitializeControlResizer();

            LocalizeControls(); 
            ApplyTheme();
            dataGridView1.GridColor = this.BackColor;
        }

        private void InitializeControlResizer()
        {
            fontResizer = new FontResizer();
            fontResizer.AdjustFont(this);

            resizer = new ControlResizer(this.Size);

            resizer.RegisterControl(label1);
            resizer.RegisterControl(label2);
            resizer.RegisterControl(label3);
            resizer.RegisterControl(label4);
            resizer.RegisterControl(dataGridView1);
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
            //button2.Text = LanguageManager.Instance.GetString("MF-btn2");
            //button3.Text = LanguageManager.Instance.GetString("MF-btn3");
        }

        public void ApplyTheme()
        {
            ColorPalette.ColorTrio colors = ColorPalette.GetColorTrio();

            this.BackColor = colors.Color1;
            this.ForeColor = colors.Color2;

            dataGridView1.ForeColor = colors.Color2;
            dataGridView1.BackgroundColor = colors.Color1;
        }
    }
}
