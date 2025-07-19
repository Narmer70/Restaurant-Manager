using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PadTai.Classes.Others
{
    public static class ThemeManager
    {
        public static string CurrentTheme { get; private set; }

        // Event to notify when the theme changes
        public static event Action ThemeChanged;

        public static void SwitchTheme(string themeName)
        {
            CurrentTheme = themeName;
            ThemeChanged?.Invoke(); 
        }

        public static void LoadTheme()
        {
            CurrentTheme = Properties.Settings.Default.SelectedTheme ?? "Light";
            SwitchTheme(CurrentTheme);
        }

        public static void SaveTheme(string themeName)
        {
            Properties.Settings.Default.SelectedTheme = themeName;
            Properties.Settings.Default.Save();
        }
    }
}
