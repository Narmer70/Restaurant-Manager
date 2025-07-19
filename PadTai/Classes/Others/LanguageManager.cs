using System;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using System.Globalization;
using System.Collections.Generic;


namespace PadTai.Classes.Others
{
    public class LanguageManager
    {
        private static LanguageManager _instance;
        private ResourceManager _resourceManager;
        private CultureInfo _currentCulture;
        public event Action LanguageChanged;

        private LanguageManager()
        {
            LoadUserLanguage();
            LoadResources(); 
        }

        public static LanguageManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LanguageManager();
                }
                return _instance;
            }
        }

        public string GetString(string key, Form form = null)
        {
            return _resourceManager.GetString(key);
        }

        public CultureInfo CurrentCulture => _currentCulture;

        public void ChangeLanguage(string cultureCode)
        {
            _currentCulture = new CultureInfo(cultureCode);
            LoadResources();
            SaveUserLanguage(cultureCode);
            LanguageChanged?.Invoke(); // Trigger the event
        }

        private void LoadResources()
        {
            Thread.CurrentThread.CurrentUICulture = _currentCulture;
            _resourceManager = new ResourceManager("PadTai.Resources.Strings", typeof(LanguageManager).Assembly);
        }

        private void LoadUserLanguage()
        {
            string cultureCode = Properties.Settings.Default.Language;
            _currentCulture = string.IsNullOrEmpty(cultureCode) ? CultureInfo.CurrentCulture : new CultureInfo(cultureCode);
        }

        private void SaveUserLanguage(string cultureCode)
        {
            Properties.Settings.Default.Language = cultureCode;
            Properties.Settings.Default.Save();
        }
    }
}