using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSThemeHelper
{
    class Program
    {
        static readonly Color PrimaryColor = ColorTranslator.FromHtml("#500000"); // Base color the the theme (branding color)
        static readonly Color DefaultPrimaryColor = ColorTranslator.FromHtml("#5867dd"); // The primary color from the default theme

        //NOTE: there should be 69 colors (as there are 69 colors in ASP.NET Zero's metronic customization tool
        // I have 62 - I might be missing a style sheet

        public static readonly string[] DefaultThemeFiles = FilePaths.DefaultThemeFiles; // Default theme CSS files
        public static readonly string[] AlternativeThemeFiles = FilePaths.AlternativeThemeFiles; // An alternative theme for comparison
        public static readonly string[] OutputThemeFiles = FilePaths.OutputThemeFiles;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting theme creation");

            CSSFileHelper fileHelper = new CSSFileHelper(PrimaryColor, DefaultPrimaryColor, DefaultThemeFiles, AlternativeThemeFiles, OutputThemeFiles);

            Console.WriteLine("Done");
            //Console.WriteLine("Press any key to continue...");
            //Console.ReadLine();
        }

        
    }
}
