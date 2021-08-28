using System;
using System.Windows;
using System.Data;
using System.Xml;
using System.Configuration;

using Microsoft.Win32;                // PowerModeChangedEventArgs

namespace Symulator3GUI
{
    /// <summary>
    /// Interaction logic for MyApp.xaml
    /// </summary>

    public partial class App : Application
    {
        private void AppStartup (object obj, StartupEventArgs e)
        {
            if (!DotNet45Found ())
            {
                Application.Current.Shutdown ();
                MessageBox.Show ("Symulator3 requires .NET Framework 4.5 or later.", "Symulator/3",
                                 MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        // v0.1.0.00  2019-05-28  Windows Apps\WpfClock
        public static bool DotNet45Found ()
        {
            int iKeyreleaseKey = 0;

            RegistryKey regKey = RegistryKey.OpenBaseKey (RegistryHive.LocalMachine,
                                                          RegistryView.Registry32).OpenSubKey ("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\");

            if (regKey == null)
            {
                return false;
            }

            try
            {
                iKeyreleaseKey = Convert.ToInt32 (regKey.GetValue ("Release"));
            }
            catch (Exception)
            {
                // Fail silently
                return false;
            }

            if (iKeyreleaseKey >= 393273)
            {
                return true; // 4.6 RC or later
            }

            if (iKeyreleaseKey >= 379893)
            {
                return true; // 4.5.2 or later
            }

            if (iKeyreleaseKey >= 378675)
            {
                return true; // 4.5.1 or later
            }

            if (iKeyreleaseKey >= 378389)
            {
                return true; // 4.5 or later
            }

            // This line should never execute. A non-null release key should mean that 4.5 or later is installed. 
            return false; // No 4.5 or later version detected
        }
    }
}