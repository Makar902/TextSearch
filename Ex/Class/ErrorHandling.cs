using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ex.Class
{
    internal static class ErrorHandling
    {
        private static object logLock = new object();
        public static string LogTXT = "log.txt";


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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task CatchExToLog(Exception error)
        {
            await CatchExToLog(error, null);
        }

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
