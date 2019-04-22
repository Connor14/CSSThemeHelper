using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Drawing;

namespace CSSThemeHelper
{
    public class CSSFileHelper
    {
        // Source: https://stackoverflow.com/questions/1636350/how-to-identify-a-given-string-is-hex-color-format
        public const string HexColorRegex = "#(?:[0-9a-fA-F]{3}){1,2}";

        private readonly Color _primaryColor;
        private readonly Color _defaultPrimaryColor;

        private List<CSSFile> _defaultFilesContents;
        private List<CSSFile> _alternativeFilesContents;
        private List<CSSFile> _newFilesContents;

        // This should be a CSSFile like the other files
        private string _demoTenantFile;

        public CSSFileHelper(Color primaryColor, 
            Color defaultPrimaryColor, 
            string[] defaultFiles, 
            string[] alternativeFiles, 
            string[] outputFiles,
            string demoTenantFile)
        {
            _demoTenantFile = demoTenantFile;

            bool sameFileCount = defaultFiles.Length == alternativeFiles.Length && defaultFiles.Length == outputFiles.Length;
            if (!sameFileCount)
            {
                throw new Exception("The number of default, alternative, and output files are not the same.");
            }

            _primaryColor = primaryColor;
            _defaultPrimaryColor = defaultPrimaryColor;

            _defaultFilesContents = GetFiles(defaultFiles);
            _alternativeFilesContents = GetFiles(alternativeFiles);

            // assume file names are the same, but paths are different
            _newFilesContents = new List<CSSFile>();
            foreach(string file in outputFiles)
            {
                // find the default file that has the same name as this output file
                CSSFile coorelatedDefaultFile = _defaultFilesContents.First(cssFile => cssFile.FileName == Path.GetFileName(file));
                _newFilesContents.Add(new CSSFile(file, coorelatedDefaultFile.Contents));
            }
        }

        // relativeColors: Should color variables use a standard HUE and saturation and lightness values?
        public void UpdateThemeFiles(bool relativeColors = false)
        {
            HashSet<string> customColors = this.Harvest();

            string colorVariables = "\n:root {\n";

            // create a variable for the HUE of the color to act as the foundation for every other color
            if (relativeColors)
            {
                colorVariables += string.Format("\t--branding-color-hue: {0}; /* The HUE of the tenant's branding color */\n", (int)_primaryColor.GetHue());
            }

            int counter = 1; // counter to label variable names
            foreach(string hexString in customColors)
            {
                Color oldColor = ColorTranslator.FromHtml(hexString);
                HSLColor oldHslColor = new HSLColor(oldColor);

                HSLColor newHslColor = null;
                if (hexString == ColorToHex(_defaultPrimaryColor))
                {
                    // don't do any hue or saturation operations
                    newHslColor = new HSLColor(_primaryColor);
                }
                else
                {
                    newHslColor = new HSLColor(_primaryColor.GetHue(), oldHslColor.Saturation, oldHslColor.Luminosity);
                }

                // new variable for the current hex string
                string variableName = string.Format("--branding-color-{0}", counter);
                string variableReference = string.Format("var({0})", variableName);

                if (relativeColors)
                {
                    string relativeColor = string.Format("hsl({0}, {1}%, {2}%)", "var(--branding-color-hue)", (int)(newHslColor.Saturation / HSLColor.SCALE * 100.0), (int)(newHslColor.Luminosity / HSLColor.SCALE * 100));
                    colorVariables += string.Format("\t{0}: {1};\n", variableName, relativeColor);
                }
                else
                {
                    string newHexString = ColorToHex(newHslColor);
                    colorVariables += string.Format("\t{0}: {1};\n", variableName, newHexString);
                }
                
                foreach (CSSFile file in _newFilesContents)
                {
                    file.Contents = file.Contents.Replace(hexString, variableReference);
                }

                counter++;
            }

            colorVariables += "}\n\n";

            // write to disk
            foreach (CSSFile file in _newFilesContents)
            {
                if(file.FileName == "metronic-customize.css")
                {
                    file.Contents = colorVariables + file.Contents;

                    // also write to the demo tenant.css file
                    File.WriteAllText(_demoTenantFile, colorVariables);
                }

                File.WriteAllText(file.FilePath, file.Contents);
            }
        }

        // Returns a HashSet containing the colors that are different betweeen the sets of files 
        // Colors in the HashSet are unique - no duplicates
        private HashSet<string> Harvest()
        {
            HashSet<string> customColors = new HashSet<string>();

            // add all colors from the input files to the hash set
            foreach (CSSFile file in _defaultFilesContents)
            {
                MatchCollection matches = Regex.Matches(file.Contents, HexColorRegex);

                foreach (Match match in matches)
                {
                    customColors.Add(match.Value);
                }
            }

            // remove colors that were common between the two (we assume these are standard colors)
            foreach (CSSFile file in _alternativeFilesContents)
            {
                MatchCollection matches = Regex.Matches(file.Contents, HexColorRegex);

                foreach (Match match in matches)
                {
                    customColors.Remove(match.Value);
                }
            }

            return customColors;
        }

        private static List<CSSFile> GetFiles(string[] filePaths)
        {
            List<CSSFile> files = new List<CSSFile>();

            foreach (string file in filePaths)
            {                
                string fileContents = File.ReadAllText(file);
                files.Add(new CSSFile(file, fileContents));
            }

            return files;
        }

        // Source https://stackoverflow.com/questions/2395438/convert-system-drawing-color-to-rgb-and-hex-value
        private static string ColorToHex(Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
    }
}
