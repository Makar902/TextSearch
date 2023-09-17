﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ex
{
    internal static class ErorHandling
    {
        private static object logLock = new object();
        public static string LogTXT = "log.txt";


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CatchExToLog(string errorText)
        {
            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {errorText}";

            lock (logLock)
            {
                using (StreamWriter log = new StreamWriter(LogTXT, true))
                {
                    log.WriteLine(logMessage);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CatchExToLog(Exception error)
        {
            CatchExToLog(error, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CatchExToLog(Exception error, string? text)
        {
            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {(text != null ? text : "")}{error}";

            lock (logLock)
            {
                using (StreamWriter log = new StreamWriter(LogTXT, true))
                {
                    log.WriteLine(logMessage);
                }
            }
        }

    }
}