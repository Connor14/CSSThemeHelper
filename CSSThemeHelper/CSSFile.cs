using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSThemeHelper
{
    public class CSSFile
    {
        public string FilePath { get; set; }
        public string Contents { get; set; }

        public string FileName
        {
            get
            {
                return Path.GetFileName(FilePath);
            }
        }

        public CSSFile(string filePath, string contents)
        {
            this.FilePath = filePath;
            this.Contents = contents;
        }
    }
}
