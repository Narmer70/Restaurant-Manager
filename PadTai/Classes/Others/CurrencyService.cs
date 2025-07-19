using System;
using System.Linq;
using System.Text;

namespace PadTai.Classes.Others
{
    public class CurrencyService
    {
        private static CurrencyService _instance;

        // Private constructor to prevent instantiation from outside
        private CurrencyService()
        {
            // Load the default currency from settings
            SelectedCurrency = Properties.Settings.Default.SelectedCurrency ?? "USD"; 
        }

        // Public property to access the singleton instance
        public static CurrencyService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CurrencyService();
                }
                return _instance;
            }
        }

        public string SelectedCurrency { get; set; }

        public string GetCurrencySymbol()
        {
            switch (SelectedCurrency)
            {
                case "United States Dollar (USD)":
                    return "$"; // United States Dollar
                case "Euro (EUR)":
                    return "€"; // Euro
                case "Russian Ruble (RUB)":
                    return "₽"; // Russian Ruble
                case "Belarusian Ruble (BYN)":
                    return "Br"; // Belarusian Ruble
                case "Canadian Dollar (CAD)":
                    return "C$"; // Canadian Dollar
                case "Australian Dollar (AUD)":
                    return "A$"; // Australian Dollar
                case "British Pound Sterling (GBP)":
                    return "£"; // British Pound Sterling
                case "West African CFA Franc (XOF)":
                    return "CFA"; // West African CFA Franc
                case "Central African CFA Franc (XAF)":
                    return "CFA"; // Central African CFA Franc
                case "Kenyan Shilling (KES)":
                    return "Ksh"; // Kenyan Shilling
                case "South African Rand (ZAR)":
                    return "R"; // South African Rand
                case "Congolese Franc (CDF)":
                    return "FC"; // Congolese Franc
                case "Null":
                    return ""; // No currency symbol
                default:
                    return "$"; // Default to USD if not recognized
            }
        }

        public void SaveCurrency()
        {
            Properties.Settings.Default.SelectedCurrency = SelectedCurrency;
            Properties.Settings.Default.Save();
        }
    }
}
