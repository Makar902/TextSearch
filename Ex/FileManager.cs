using FluentCommand.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Ex
{
    public class FileManager
    {
        public static bool haveAdminRights;
       // public static Form1 form;
        public static void AdminRightsInitiate()
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
                ErorHandling.CatchExToLog(error);
            }
        }


    }
}
