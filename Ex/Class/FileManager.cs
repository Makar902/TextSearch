using FluentCommand.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Ex.Class
{
    public class FileManager
    {
        public static bool haveAdminRights;
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
                        MessageBox.Show("The program is not launched with administrator rights.May be some trouble with acces");
                        haveAdminRights = false;
                    }
                
            }
            catch (Exception error)
            {
                await ErrorHandling.CatchExToLog(error);
            }
        }
        public static async Task Wait()
        {
            while (Form1.stopSearch == true)
            {
                await Task.Delay(100); 
            }
        }

    }
}
