using FluentCommand.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Ex.Class
{
    // Comment: This class provides file management and access control functionality.
    public class FileManager
    {
        public static bool haveAdminRights; // Comment: Indicates whether the program has administrator rights.

        // Comment: This method checks if the program is launched with administrator rights.
        public static async Task AdminRightsInitiate()
        {
            try
            {
                WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
                WindowsPrincipal currentPrincipal = new WindowsPrincipal(currentIdentity);

                if (currentPrincipal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    MessageBox.Show("The program was launched with the rights of an administrator.");
                    haveAdminRights = true;
                }
                else
                {
                    MessageBox.Show("The program is not launched with administrator rights. There may be some trouble with access.");
                    haveAdminRights = false;
                }
            }
            catch (Exception error)
            {
                await ErrorHandling.CatchExToLog(error); // Comment: Handle and log any exceptions that occur.
            }
        }

        // Comment: This method waits for the 'stopSearch' flag in Form1 to become false.
        public static async Task Wait()
        {
            while (Form1.stopSearch == true)
            {
                await Task.Delay(100); // Comment: Delay the execution of the method for 100 milliseconds.
            }
        }
    }



}
