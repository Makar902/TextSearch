using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ex.Class 
{
    // Comment: This struct represents information about a file item, including its path, size, creation date, modification date,
    // file extension, and MIME type.
    public struct ItemInfo
    {
        public string FilePath { get; set; } // Comment: Gets or sets the path of the file.
        public long FileSizeBytes { get; set; } // Comment: Gets or sets the size of the file in bytes.
        public DateTime CreationDate { get; set; } // Comment: Gets or sets the creation date of the file.
        public DateTime LastModifiedDate { get; set; } // Comment: Gets or sets the last modified date of the file.
        public string FileExtension { get; set; } // Comment: Gets or sets the file extension.
        public string MimeType { get; set; } // Comment: Gets or sets the MIME type of the file.

        // Comment: This method retrieves the MIME type of a file based on its extension.
        public static async Task<string> GetMimeType(string filePath)
        {
            try
            {
                await Task.Run(() =>
                {
                    if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                    {
                        return null; // Comment: If the file path is empty or the file doesn't exist, return null.
                    }

                    string extension = Path.GetExtension(filePath).ToLower();
                    string mimeType = Registry.ClassesRoot.OpenSubKey(extension)?.GetValue("Content Type") as string;

                    return mimeType; // Comment: Return the retrieved MIME type.
                });
            }
            catch (Exception error)
            {
                await ErrorHandling.CatchExToLog(error);
            }

            return null; // Comment: Return null if an exception occurs.
        }
    }

}
