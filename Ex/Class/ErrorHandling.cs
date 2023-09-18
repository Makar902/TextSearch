using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ex.Class
{
    // Comment: This internal class handles error logging and provides methods to log exceptions.
    internal static class ErrorHandling
    {
        private static object logLock = new object();
        public static string LogTXT = "log.txt"; // Comment: Specifies the name of the log file.

        // Comment: Logs a custom error message to the log file.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task CatchExToLog(string errorText)
        {
            await Task.Run(() => {
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {errorText}";

                lock (logLock)
                {
                    using (StreamWriter log = new StreamWriter(LogTXT, true))
                    {
                        log.WriteLine(logMessage);
                    }
                }
            });
        }

        // Comment: Logs an exception to the log file.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task CatchExToLog(Exception error)
        {
            await CatchExToLog(error, null);
        }

        // Comment: Logs an exception along with additional text to the log file.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task CatchExToLog(Exception error, string? text)
        {
            await Task.Run(() =>
            {
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {(text != null ? text : "")}{error}";

                lock (logLock)
                {
                    using (StreamWriter log = new StreamWriter(LogTXT, true))
                    {
                        log.WriteLine(logMessage);
                    }
                }
            });
        }
    }

}
