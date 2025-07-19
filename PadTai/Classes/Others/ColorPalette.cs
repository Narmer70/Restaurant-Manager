using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections.Generic;


namespace PadTai.Classes.Others
{
    public static class ColorPalette
    {
        // Define a class to hold a set of colors and images for a theme
        public class ColorTrio
        {
            public string Identifier { get; }
            public Color Color1 { get; }
            public Color Color2 { get; }
            public Color Color3 { get; }
            public Color Color4 { get; }
            public Color Color5 { get; }
            public Image Image1 { get; }
            public Image Image2 { get; }
            public Image Image3 { get; }
            public Boolean Bool1 { get; }

            public ColorTrio(string identifier, Color color1, Color color2, Color color3, Color color4, Color color5, Image image1, Image image2, Image image3, Boolean bool1)
            {
                Identifier = identifier;
                Color1 = color1;
                Color2 = color2;
                Color3 = color3;
                Color4 = color4;
                Color5 = color5;
                Image1 = image1;
                Image2 = image2;
                Image3 = image3;
                Bool1 = bool1;
            }
        }

        // List of color trios for each theme
        private static readonly List<ColorTrio> colorTrios = new List<ColorTrio>
        {
             new ColorTrio
             (
                  "Light",
                  Color.White,
                  Color.Black,
                  Color.FromArgb(240, 240, 240),
                  Color.LightGray,
                  Color.DarkGray,
                  Properties.Resources.search_12227561, 
                  Properties.Resources.calendarDark, 
                  Properties.Resources.search,   
                  true
             ),

             new ColorTrio
             (
                  "Dark",
                  Color.FromArgb(30, 30, 30),
                  Color.GhostWhite,
                  Color.FromArgb(40, 40, 40),
                  Color.FromArgb(23, 24, 26),
                  Color.FromArgb(100, 100, 100),
                  Properties.Resources.search,
                  Properties.Resources.calendarWhite,
                  Properties.Resources.search,
                  false
             ),
          
            new ColorTrio
             (
                 "Blue",
                 Color.FromArgb(2, 40, 71),
                 Color.GhostWhite,
                 Color.FromArgb(2, 50, 90),
                 Color.FromArgb(0, 50, 120),
                 Color.FromArgb(0, 150, 255),
                 Properties.Resources.search, 
                 Properties.Resources.calendarWhite, 
                 Properties.Resources.search,  
                 false
             )
    };

        // Method to get the color trio based on the current theme
        public static ColorTrio GetColorTrio()
        {
            string currentTheme = ThemeManager.CurrentTheme; 
            return colorTrios.Find(trio => trio.Identifier == currentTheme) ?? colorTrios[0]; 
        }
    }
}
