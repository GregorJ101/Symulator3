#region Using statements
using System;
using System.Management;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
#endregion

namespace Symulator3GUI
{
    /// <summary>
    /// Interaction logic for CAboutBox.xaml
    /// </summary>
    public partial class CAboutBox : Window
    {
        private uint   m_uiMaxClockSpeed     = 0;
        private uint   m_uiCurrentClockSpeed = 0;
        private string m_strCpuType          = "";
        public CAboutBox (string strAssemblyTitle, string strAssemblyVersion, string strCompanyName, string strTargetNetVersion,
                          string strConfiguration, string strCurrentVersions, DateTime dtBuildDate)
        {
            InitializeComponent ();
            GetCPUData ();

            x_txbAppName.Text          = strAssemblyTitle + " " + strAssemblyVersion + ((IntPtr.Size == 8) ? " (64-bit)" : " (32-bit)") + " (" + strConfiguration + ")";
            x_txbCopyright.Text        = "Copyright " + dtBuildDate.Year.ToString () + " (C) " + strCompanyName;
            x_txbBuildDate.Text        = "Build Timestamp: " + dtBuildDate.ToString ();
            x_txbMscVer.Text           = strCurrentVersions;
            x_txbTargetNetVersion.Text = "Target Net Version: " + strTargetNetVersion;
            x_txbNetVersion.Text       = ".Net Version: " + LoadSetting (@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\", "Version");
            x_txbCPU.Text              =  m_strCpuType + " (" + m_uiCurrentClockSpeed.ToString () + "MHz)";
            KeyDown += OnKeyDown;
        }

        void GetCPUData ()
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher ("select MaxClockSpeed from Win32_Processor");
            foreach (var item in mos.Get ())
            {
                m_uiMaxClockSpeed = (uint)item["MaxClockSpeed"];
            }

            mos = new ManagementObjectSearcher ("select CurrentClockSpeed from Win32_Processor");
            foreach (var item in mos.Get ())
            {
                m_uiCurrentClockSpeed = (uint)item["CurrentClockSpeed"];
            }

            mos = new ManagementObjectSearcher ("select Name from Win32_Processor");
            foreach (var item in mos.Get ())
            {
                m_strCpuType = item["Name"].ToString (); // "Intel(R) Core(TM) i7-3820 CPU @ 3.60GHz"
            }
        }

        void OnKeyDown (object sender, EventArgs e)
        {
            // Escape closes the window
            if ((Keyboard.GetKeyStates (Key.Escape) & KeyStates.Down) > 0)
            {
                Close ();
            }
        }

        private string LoadSetting (string strRegistryPath, string strValueName)
        {
            RegistryKey rkPathKey  = Registry.LocalMachine.OpenSubKey (strRegistryPath, false);
            if (rkPathKey == null)
            {
                return "";
            }

            object objRegValue = rkPathKey.GetValue (strValueName);
            try
            {
                // Treat all types as string
                return objRegValue.ToString ();
            }
            catch (Exception)
            {
                // Fail silently
            }

            return "";
        }

        private void LinkToGitHub (object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start (e.Uri.AbsoluteUri);
        }
    }
}
