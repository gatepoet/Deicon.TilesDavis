using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;

namespace TilesDavis.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        //private bool IsRunAsAdministrator()
        //{
        //    var wi = WindowsIdentity.GetCurrent();
        //    var wp = new WindowsPrincipal(wi);

        //    return wp.IsInRole(WindowsBuiltInRole.Administrator);
        //}
        
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            EventLog.WriteEntry("TilesDavis", e.ToString(), EventLogEntryType.Error);
        }
        //    if (!IsRunAsAdministrator())
        //    {
        //        // It is not possible to launch a ClickOnce app as administrator directly, so instead we launch the
        //        // app as administrator in a new process.
        //        var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);

        //        // The following properties run the new process as administrator
        //        processInfo.UseShellExecute = true;
        //        processInfo.Verb = "runas";

        //        // Start the new process
        //        try
        //        {
        //            Process.Start(processInfo);
        //        }
        //        catch (Exception)
        //        {
        //            // The user did not allow the application to run as administrator
        //            MessageBox.Show("Sorry, this application must be run as Administrator.");
        //        }

        //        // Shut down the current process
        //        Application.Current.Shutdown();
        //    }
        //    else
        //    {
        //        // We are running as administrator

        //        // Do normal startup stuff...
        //    }
        //}
    }
}
