using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ex.Class 
{
    internal struct ItemInfo
    {
        public string FilePath { get; set; }
        public long FileSizeBytes { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string FileExtension { get; set; }
        public string MimeType { get; set; }

        public static async Task<string> GetMimeType(string filePath)
        {
            try
            {
                await Task.Run(() => {
                    if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                    {
#pragma warning disable CS8603
                        return null;
#pragma warning disable CS8603
                    }

                    string extension = Path.GetExtension(filePath).ToLower();
#pragma warning disable CS8600
                    string mimeType = Registry.ClassesRoot.OpenSubKey(extension)?.GetValue("Content Type") as string;
#pragma warning restore CS8600
#pragma warning disable CS8603
                    return mimeType;
#pragma warning disable CS8603
                });

                }
            catch (Exception error)
            {

                await ErrorHandling.CatchExToLog(error);
            }
#pragma warning disable CS8603 
            return null;
#pragma warning restore CS8603 
        }

    }
}
