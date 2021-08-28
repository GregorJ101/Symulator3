#region Using Statements
using Microsoft.Win32;                // RegistryKey, Registry
using System;                         // Exception
using System.Collections.Generic;     // IEnumerable
using System.ComponentModel;          // CancelEventArgs
using System.Diagnostics;             // Debug.Assert
using System.IO;                      // File
using System.Management;              // 
using System.Text;                    // StringBuilder
//using System.Threading;               // Sleep
using System.Threading.Tasks;         // Task, Task<>
using System.Windows;                 // Window, RoutedEventArgs
using System.Windows.Controls;        // ColumnDefinition
using System.Windows.Input;
using System.Windows.Media;           // SolidColorBrush
using System.Windows.Media.Imaging;   // BitmapImage

using EmulatorEngine;
using GetMscVersionLib;
#endregion

namespace Symulator3GUI
{
    public partial class CSymulator3GUI : Window
    {
        //private const string SOFTWARE_STRING            = "Software";
        //private const string SACRED_CAT_SOFTWARE_STRING = "Sacred Cat Software";
        //private const string SOFTWARE_PATH_STRING       = "Software";
        //private const string SACRED_CAT_PATH_STRING     = @"Software\Sacred Cat Software";
        private const string WPF_SYM3_PATH_STRING       = @"Software\Sacred Cat Software\Symulator3";
        private const string WINDOW_POS_TOP_STRING      = "WindowPosTop";
        private const string WINDOW_POS_LEFT_STRING     = "WindowPosLeft";
        private const string WINDOW_POS_HEIGHT_STRING   = "WindowPosHeight";
        private const string WINDOW_POS_WIDTH_STRING    = "WindowPosWidth";
        private const string WINDOW_MAXIMIZED_STRING    = "WindowMaximized";
        private const string STRING_VERSION             = "Version=";
        private const string CAPTION_STRING             = "Symulator/3";

    #if DEBUG
        private byte m_yOldCR = (byte)(CEmulatorEngine.ERegisterFlags.COND_Low             |
                                       CEmulatorEngine.ERegisterFlags.COND_DecimalOverflow |
                                       CEmulatorEngine.ERegisterFlags.COND_TestFalse       |
                                       CEmulatorEngine.ERegisterFlags.COND_BinaryOverflow);
    #else
        private byte m_yOldCR = 0;
    #endif

        private SolidColorBrush m_brBlack      = new SolidColorBrush (Colors.Black);
        private SolidColorBrush m_brRed        = new SolidColorBrush (Colors.Red);
        private SolidColorBrush m_brGray       = new SolidColorBrush (Colors.LightGray);
        private SolidColorBrush m_brFireBrick  = new SolidColorBrush (Colors.Firebrick);
        private SolidColorBrush m_brLightCoral = new SolidColorBrush (Colors.LightCoral);

        private string m_strAssemblyTitle         = "";
        private string m_strAssemblyFileVersion   = "";
        private string m_strAssemblyVersion       = "";
        private string m_strCompanyName           = "";
        private string m_strConfiguration         = "";
        private string m_strTargetNetVersion      = "";
        private string m_strCpuType               = "";
        private string m_strLastFocusPanel        = "";
        private string m_strTableName             = "";
        private string m_strItemName              = "";
        private string m_strTokenName             = "";
        private string m_strDasmOutputFilename    = "";
        private string m_strTraceOutputFilename   = "";
        private string m_strPrinterOutputFilename = "";
        private uint   m_uiMaxClockSpeed          = 0;
        private uint   m_uiCurrentClockSpeed      = 0;
        private int    m_iCurrentLineIdx          = 25;
        private double m_dEffectiveSys3ClockMHz   = 0;
        private List<string> m_lstrDisASM         = new List<string> ();

        DateTime m_dtBuildDate = new System.IO.FileInfo (System.Reflection.Assembly.GetExecutingAssembly ().Location).LastWriteTime;

        //System.Windows.Threading.DispatcherTimer SystemTimer = new System.Windows.Threading.DispatcherTimer ();

        // Dummy columns for layers 0 and 1:
        ColumnDefinition coldefColumn1CloneForLayer0;
        ColumnDefinition coldefColumn2CloneForLayer0;
        ColumnDefinition coldefColumn2CloneForLayer1;
    
        CEmulatorEngine m_objEmulatorEngine = new CEmulatorEngine (true);

        public string GetAppTitle ()
        {
            return CAPTION_STRING;
        }

        //private void OnTimerClick (object sender, EventArgs e)
        //{
        //}

        private void UpdateLabel (Label lbl, string strInput, bool bForceBlack = false)
        {
            //Console.WriteLine ("UpdateLabel (" + lbl.Name + ", " + strInput + " (" + ((bForceBlack || lbl.Content.ToString () == strInput) ? "Black" : "Red") + ')');
            lbl.Foreground = (bForceBlack || lbl.Content.ToString () == strInput) ? m_brBlack : m_brRed;
            lbl.Content    = strInput;
            //ResetLastLabelColor (lbl);
        }

        private void ResetLabels ()
        {
            // CPU Registers
            x_lblRegisterARR_IL0.Foreground = m_brBlack;
            //x_lblRegisterARR_IL1.Foreground = m_brBlack;
            x_lblRegisterXR1.Foreground     = m_brBlack;
            x_lblRegisterXR2.Foreground     = m_brBlack;

            // 5203 Printer Registers
            x_lblRegisterLPFLR.Foreground = m_brBlack;
            x_lblRegisterLPIAR.Foreground = m_brBlack;
            x_lblRegisterLPDAR.Foreground = m_brBlack;

            // 5424 MFCU Registers
            x_lblRegisterMPDAR.Foreground = m_brBlack;
            x_lblRegisterMRDAR.Foreground = m_brBlack;
            x_lblRegisterMUDAR.Foreground = m_brBlack;

            // 5444 Disk Registers
            x_lblRegisterDCAR.Foreground  = m_brBlack;
            x_lblRegisterDRWAR.Foreground = m_brBlack;
        }

        private void EnableNewCPUDials (Int16 iCPUDials, bool bEnable)
        {
            if (!bEnable)
            {
                x_txbCPUDials.Text = string.Format ("{0:X4}", iCPUDials);
            }

            x_txbCPUDials.Foreground  = bEnable ? m_brBlack     : m_brGray;
            x_lblCpuDials.Foreground  = bEnable ? m_brBlack     : m_brGray;
            x_txbCPUDials.BorderBrush = bEnable ? m_brFireBrick : m_brLightCoral;
            x_txbCPUDials.IsReadOnly  = !bEnable;
        }

        #region Event Handler Methods
        public void OnNewIAR (object sender, NewIAREventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New IAR: {0}", e.NewIAR);
                int iLineIdx = m_objEmulatorEngine.GetLineIdxFromInstructionAddress (e.NewIAR);
                if (iLineIdx > -1)
                {
                    Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                                new Action (() => x_cdbxDisassemblyOutput.SetCurrentLineIdx (iLineIdx)));
                }
                if (e.NewIL == 0)
                {
                    Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                                new Action (() => x_lblRegisterIAR_IL0.Content = string.Format ("{0:X4}", e.NewIAR)));
                }
                else if (e.NewIL == 0)
                {
                    Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                                new Action (() => x_lblRegisterIAR_IL1.Content = string.Format ("{0:X4}", e.NewIAR)));
                }
            }
        }

        public void OnNewARR (object sender, NewARREventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New ARR: {0}", e.NewARR);

                if (e.NewIL == 0)
                {
                    Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                                //new Action (() => x_lblRegisterARR_IL0.Content = string.Format ("{0:X4}", e.NewARR)));
                                                                new Action (() => UpdateLabel (x_lblRegisterARR_IL0, string.Format ("{0:X4}", e.NewARR))));
                }
                else if (e.NewIL == 0)
                {
                    Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                                new Action (() => UpdateLabel (x_lblRegisterARR_IL1, string.Format ("{0:X4}", e.NewARR))));
                }
            }
        }

        public void OnNewXR1 (object sender, NewXR1EventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New XR1: {0}", e.NewXR1);

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => UpdateLabel (x_lblRegisterXR1, string.Format ("{0:X4}", e.NewXR1))));
            }
        }

        public void OnNewXR2 (object sender, NewXR2EventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New XR2: {0}", e.NewXR2);

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => UpdateLabel (x_lblRegisterXR2, string.Format ("{0:X4}", e.NewXR2))));
            }
        }

        public void OnNewCR (object sender, NewCREventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New CR: 0x{0:X0}", e.NewCR);

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => ManageNewCR (e.NewCR, e.SystemReset)));
            }
        }

        public void OnNewLPFLR (object sender, NewLPFLREventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New LPFLR: {0}", e.NewLPFLR);

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => UpdateLabel (x_lblRegisterLPFLR, string.Format ("{0:X4}", e.NewLPFLR))));
            }
        }

        public void OnNewLPIAR (object sender, NewLPIAREventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New LPIAR: {0}", e.NewLPIAR);

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => UpdateLabel (x_lblRegisterLPIAR, string.Format ("{0:X4}", e.NewLPIAR))));
            }
        }

        public void OnNewLPDAR (object sender, NewLPDAREventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New LPDAR: {0}", e.NewLPDAR);

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => UpdateLabel (x_lblRegisterLPDAR, string.Format ("{0:X4}", e.NewLPDAR))));
            }
        }

        public void OnNewMPDAR (object sender, NewMPDAREventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New MPDAR: {0}", e.NewMPDAR);

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => UpdateLabel (x_lblRegisterMPDAR, string.Format ("{0:X4}", e.NewMPDAR))));
            }
        }

        public void OnNewMRDAR (object sender, NewMRDAREventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New MRDAR: {0}", e.NewMRDAR);

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => UpdateLabel (x_lblRegisterMRDAR, string.Format ("{0:X4}", e.NewMRDAR))));
            }
        }

        public void OnNewMUDAR (object sender, NewMUDAREventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New MUDAR: {0}", e.NewMUDAR);

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => UpdateLabel (x_lblRegisterMUDAR, string.Format ("{0:X4}", e.NewMUDAR))));
            }
        }

        public void OnNewDCAR (object sender, NewDCAREventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New DCAR: {0}", e.NewDCAR);

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => UpdateLabel (x_lblRegisterDCAR, string.Format ("{0:X4}", e.NewDCAR))));
            }
        }

        public void OnNewDRWAR (object sender, NewDRWAREventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New DRWAR: {0}", e.NewDRWAR);

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => UpdateLabel (x_lblRegisterDRWAR, string.Format ("{0:X4}", e.NewDRWAR))));
            }
        }

        public void OnNewStepCount (object sender, NewStepCountEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New StepCount: {0}", e.NewStepCount);

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => x_lblStepCount.Content = e.NewStepCount.ToString ()));
            }
        }

        public void OnNewDASMString (object sender, NewDASMStringEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New DASM string: {0}", e.NewDASMString);

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => { x_cdbxDisassemblyOutput.Text += e.NewDASMString; x_cdbxDisassemblyOutput.ScrollToEnd (); }));
            }
        }

        public void OnNewDASMStringList (object sender, NewDASMStringListEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New DASM string: {0}", e.NewDASMString);

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => { x_cdbxDisassemblyOutput.SuspendRendering (true); }));

                foreach (string str in e.NewDASMStrings)
                {
                    Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                                new Action (() => { x_cdbxDisassemblyOutput.Text += str + Environment.NewLine;
                                                                                    x_cdbxDisassemblyOutput.ScrollToEnd (); }));
                }

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => { x_cdbxDisassemblyOutput.SuspendRendering (false); }));
            }
        }

        public void OnNewTraceString (object sender, NewTraceStringEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New Trace string: {0}", e.NewTraceString);

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => { x_cdbxTraceOutput.ScrollToEnd ();
                                                                                x_cdbxTraceOutput.Text += e.NewTraceString;
                                                                                x_cdbxTraceOutput.ScrollToEnd (); }));
            }
        }

        public void OnNewTraceStringList (object sender, NewTraceStringListEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New DASM string: {0}", e.NewDASMString);

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Normal,
                                                            new Action (() => { x_cdbxTraceOutput.SuspendRendering (true); }));

                foreach (string str in e.NewTraceStrings)
                {
                    Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Normal,
                                                                new Action (() => { x_cdbxTraceOutput.Text += str + Environment.NewLine;
                                                                                    x_cdbxTraceOutput.ScrollToEnd (); }));
                }

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Normal,
                                                            new Action (() => { x_cdbxTraceOutput.SuspendRendering (false); }));
            }
        }

        public void OnNew5471String (object sender, New5471StringEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("New 5471 string: {0}", e.New5471String);
            }
        }

        public void OnNewPrinterString (object sender, NewPrinterStringEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New Printer string: {0}", e.NewPrinterString);

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => { x_tbxPrinterOutput.Text += e.NewPrinterString;
                                                                                x_tbxPrinterOutput.ScrollToEnd (); }));
            }
        }

        public void OnNewPrinterStringList (object sender, NewPrinterStringListEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New DASM string: {0}", e.NewDASMString);

                foreach (string str in e.NewPrinterStrings)
                {
                    Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                                new Action (() => { x_tbxPrinterOutput.Text += str + Environment.NewLine;
                                                                                    x_tbxPrinterOutput.ScrollToEnd (); }));
                }
            }
        }

        public void OnNewHaltCode (object sender, NewHaltCodeEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New Hale code: {0}", e.NewHaltCode);

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => x_lblHPLDisplay.Content = e.NewHaltCode));
            }
        }

        public void OnNew5475Code (object sender, New5475CodeEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New 5475 code: {0}", e.New5475Code);

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => x_lbl5475Display.Content = e.New5475Code));
            }
        }

        public void OnNewProgramState (object sender, NewProgramStateEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New Program State: {0}", e.NewProgramState);

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => x_lblProgramState.Content = e.NewProgramState));
            }
        }

        public void OnNewProcessorCheck1 (object sender, NewProcessorCheck1EventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New ProcessorCheck1: {0}", e.ProcessorCheck1);

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => x_lblProcCheck1.Content = e.ProcessorCheck1));
            }
        }

        public void OnNewProcessorCheck2 (object sender, NewProcessorCheck2EventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("New ProcessorCheck2: {0}", e.ProcessorCheck2);

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => x_lblProcCheck2.Content = e.ProcessorCheck2));
            }
        }

        public void OnResetControlColors (object sender, EventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnResetControlColors");

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => ResetLabels ()));
            }
        }

        public void OnShowTraceState (object sender, EventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnResetControlColors");

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => ShowTraceState ()));
            }
        }

        public void OnResetTraceList (object sender, EventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnResetTraceList");

                m_strTraceOutputFilename = "";
                m_objEmulatorEngine.m_objTraceQueue.Clear ();
            }
        }

        public void OnInitializeTracePanel (object sender, EventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnInitializeTracePanel");

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => InitializeTracePanel ()));
            }
        }

        public void OnMakeRegisterLabelsDormant (object sender, EventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnMakeRegisterLabelsDormant");

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => MakeRegisterLabelsDormant ()));
            }
        }

        public void OnMakeRegisterLabelsActive (object sender, EventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnMakeRegisterLabelsActive");

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => MakeRegisterLabelsActive ()));
            }
        }

        public void OnUpdateAppTitle (object sender, EventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnUpdateAppTitle");

                Title = GetAppVersionAndProgramName ();
            }
        }

        public void OnClearPrintOutputPanel (object sender, EventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnClearPrintOutputPanel");

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => x_tbxPrinterOutput.Clear ()));
            }
        }

        public void OnClearGrayedLinesList (object sender, EventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnClearGrayedLinesList");

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => x_cdbxDisassemblyOutput.ClearGrayedCodeLines ()));
            }
        }

        public void OnNewCPUDials (object sender, NewCPUDialsEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnNewCPUDials");

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => EnableNewCPUDials (e.NewCPUDials, e.Enable)));
            }
        }

        public void OnNewEnableHighlightLine (object sender, NewEnableHighlightLineEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnNewEnableHighlightLine: ", e.Enable.ToString ());

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Background,
                                                            new Action (() => { x_cdbxDisassemblyOutput.EnableHighlightLine (e.Enable);
                                                                                x_cdbxDisassemblyOutput.Focus (); }));
            }
        }

        public void OnNewDisassemblyListing (object sender, NewDisassemblyEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnNewDisassemblyListing: " + "0x" + e.BeginDasmAddress.ToString ("X4") + ", " +
                                                                 "0x" + e.EndDasmAddress.ToString ("X4")   + ", " +
                                                                 "0x" + e.DasmEntryPoint.ToString ("X4")   + ", " +
                                                                 "0x" + e.XR1.ToString ("X4")              + ", " +
                                                                 "0x" + e.XR2.ToString ("X4"));

                // Generate new disassembly using BC destination address as the entry point
                bool bInEmulator = m_objEmulatorEngine.GetIsInEmulator ();
                m_objEmulatorEngine.SetIsInEmulator (false);
                m_objEmulatorEngine.SetHeaderProgrmaName (PrepProgramName (m_strItemName, false));
                //m_strDasmOutputFilename = MakeNewDasmOutputFilename (m_strItemName);
                m_lstrDisASM = m_objEmulatorEngine.DisassembleCodeImage (e.BeginDasmAddress, e.EndDasmAddress, e.DasmEntryPoint, e.XR1, e.XR2);
                m_objEmulatorEngine.SetIsInEmulator (bInEmulator);

                // Create new code-coverage lines list
                SortedDictionary<int, int> sdLineAddresses = m_objEmulatorEngine.GetLineAddresses ();
                x_cdbxDisassemblyOutput.SetLineAddresses (ref sdLineAddresses);
                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Normal,
                                                            new Action (() => { x_cdbxDisassemblyOutput.EnableHighlightLine (false);
                                                                                x_cdbxDisassemblyOutput.Clear (); }));

                if (sdLineAddresses.ContainsKey (e.DasmEntryPoint))
                {
                    Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Normal,
                                                                new Action (() => { x_cdbxDisassemblyOutput.SetCurrentLineIdx (sdLineAddresses[e.DasmEntryPoint]); }));
                }

                // Display new disassembly in DASM panel
                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Normal,
                                                            new Action (() => { x_cdbxDisassemblyOutput.SuspendRendering (true); }));
                foreach (string str in m_lstrDisASM)
                {
                    Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Normal,
                                                                new Action (() => { x_cdbxDisassemblyOutput.Text += str + Environment.NewLine;
                                                                                    x_cdbxDisassemblyOutput.ScrollToEnd (); }));
                }

                // Break (all run modes) and set to Single-Step mode
                m_objEmulatorEngine.m_objEmulatorState.SetRunState (CEmulatorState.ERunMode.RUN_SingleStep);

                Application.Current.Dispatcher.BeginInvoke (System.Windows.Threading.DispatcherPriority.Normal,
                                                            new Action (() => { x_cdbxDisassemblyOutput.SuspendRendering (false);
                                                                                x_cdbxDisassemblyOutput.Focus (); }));
            }
        }
        #endregion

        private void ManageNewCR (byte yNewCR, bool bSystemReset = false)
        {
            bool bHI     = (yNewCR   & (byte)CEmulatorEngine.ERegisterFlags.COND_High)            == (byte)CEmulatorEngine.ERegisterFlags.COND_High;
            bool bEQ     = (yNewCR   & (byte)CEmulatorEngine.ERegisterFlags.COND_Equal)           == (byte)CEmulatorEngine.ERegisterFlags.COND_Equal;
            bool bLO     = (yNewCR   & (byte)CEmulatorEngine.ERegisterFlags.COND_Low)             == (byte)CEmulatorEngine.ERegisterFlags.COND_Low;
            bool bDO     = (yNewCR   & (byte)CEmulatorEngine.ERegisterFlags.COND_DecimalOverflow) == (byte)CEmulatorEngine.ERegisterFlags.COND_DecimalOverflow;
            bool bTF     = (yNewCR   & (byte)CEmulatorEngine.ERegisterFlags.COND_TestFalse)       == (byte)CEmulatorEngine.ERegisterFlags.COND_TestFalse;
            bool bBO     = (yNewCR   & (byte)CEmulatorEngine.ERegisterFlags.COND_BinaryOverflow)  == (byte)CEmulatorEngine.ERegisterFlags.COND_BinaryOverflow;

            bool bPrevHI = (m_yOldCR & (byte)CEmulatorEngine.ERegisterFlags.COND_High)            == (byte)CEmulatorEngine.ERegisterFlags.COND_High;
            bool bPrevEQ = (m_yOldCR & (byte)CEmulatorEngine.ERegisterFlags.COND_Equal)           == (byte)CEmulatorEngine.ERegisterFlags.COND_Equal;
            bool bPrevLO = (m_yOldCR & (byte)CEmulatorEngine.ERegisterFlags.COND_Low)             == (byte)CEmulatorEngine.ERegisterFlags.COND_Low;
            bool bPrevDO = (m_yOldCR & (byte)CEmulatorEngine.ERegisterFlags.COND_DecimalOverflow) == (byte)CEmulatorEngine.ERegisterFlags.COND_DecimalOverflow;
            bool bPrevTF = (m_yOldCR & (byte)CEmulatorEngine.ERegisterFlags.COND_TestFalse)       == (byte)CEmulatorEngine.ERegisterFlags.COND_TestFalse;
            bool bPrevBO = (m_yOldCR & (byte)CEmulatorEngine.ERegisterFlags.COND_BinaryOverflow)  == (byte)CEmulatorEngine.ERegisterFlags.COND_BinaryOverflow;

            // x_lblCR_LO_EQ_HI
            x_lblCR_LO_EQ_HI.Content = bHI ? "HI" : (bEQ ? "EQ" : "LO");
            if (bHI != bPrevHI ||
                bEQ != bPrevEQ ||
                bLO != bPrevLO)
            {
                x_lblCR_LO_EQ_HI.Foreground = m_brRed;
            }
            else
            {
                x_lblCR_LO_EQ_HI.Foreground = m_brBlack;
            }

            // x_lblCR_DO
            if (bSystemReset ||
                bDO == bPrevDO)
            {
                x_lblCR_DO.Visibility = Visibility.Hidden;
            }
            else
            {
                x_lblCR_DO.Visibility = Visibility.Visible;

                if (bDO)
                {
                    x_lblCR_DO.Foreground = m_brRed;
                }
                else
                {
                    x_lblCR_DO.Foreground = m_brGray;
                }
            }

            // x_lblCR_TF
            if (bSystemReset ||
                bTF == bPrevTF)
            {
                x_lblCR_TF.Visibility = Visibility.Hidden;
            }
            else
            {
                x_lblCR_TF.Visibility = Visibility.Visible;

                if (bTF)
                {
                    x_lblCR_TF.Foreground = m_brRed;
                }
                else
                {
                    x_lblCR_TF.Foreground = m_brGray;
                }
            }

            // x_lblCR_BO
            if (bSystemReset ||
                bBO == bPrevBO)
            {
                x_lblCR_BO.Visibility = Visibility.Hidden;
            }
            else
            {
                x_lblCR_BO.Visibility = Visibility.Visible;

                if (bBO)
                {
                    x_lblCR_BO.Foreground = m_brRed;
                }
                else
                {
                    x_lblCR_BO.Foreground = m_brGray;
                }
            }

            m_yOldCR = yNewCR;
        }

        //public void SetDasmGrayedCodeLines ()
        //{
        //    List<int> liGrayedCodeLines = m_objEmulatorEngine.GetGrayedCodeLines ();
        //    x_cdbxDisassemblyOutput.SetGrayedCodeLines (ref liGrayedCodeLines);
        //}

        public void SetLineAddresses ()
        {
            SortedDictionary<int, int> sdLineAddresses = m_objEmulatorEngine.GetLineAddresses ();
            x_cdbxDisassemblyOutput.SetLineAddresses (ref sdLineAddresses);
        }

        public CSymulator3GUI ()
        {
            InitializeComponent();

            ManageSubscriptions ();

            LoadVersionData (System.Reflection.Assembly.GetExecutingAssembly ());

            // Initialize the dummy columns used when docking:
            coldefColumn1CloneForLayer0 = new ColumnDefinition();
            coldefColumn1CloneForLayer0.SharedSizeGroup = "ssgColumn1";
            coldefColumn2CloneForLayer0 = new ColumnDefinition();
            coldefColumn2CloneForLayer0.SharedSizeGroup = "ssgColumn2";
            coldefColumn2CloneForLayer1 = new ColumnDefinition();
            coldefColumn2CloneForLayer1.SharedSizeGroup = "ssgColumn2";

            m_objEmulatorEngine.OnNewIAR                    += OnNewIAR;
            m_objEmulatorEngine.OnNewARR                    += OnNewARR;
            m_objEmulatorEngine.OnNewXR1                    += OnNewXR1;
            m_objEmulatorEngine.OnNewXR2                    += OnNewXR2;
            m_objEmulatorEngine.OnNewCR                     += OnNewCR;
            m_objEmulatorEngine.OnNewLPFLR                  += OnNewLPFLR;
            m_objEmulatorEngine.OnNewLPIAR                  += OnNewLPIAR;
            m_objEmulatorEngine.OnNewLPDAR                  += OnNewLPDAR;
            m_objEmulatorEngine.OnNewMPDAR                  += OnNewMPDAR;
            m_objEmulatorEngine.OnNewMRDAR                  += OnNewMRDAR;
            m_objEmulatorEngine.OnNewMUDAR                  += OnNewMUDAR;
            m_objEmulatorEngine.OnNewDCAR                   += OnNewDCAR;
            m_objEmulatorEngine.OnNewDRWAR                  += OnNewDRWAR;
            m_objEmulatorEngine.OnNewStepCount              += OnNewStepCount;
            m_objEmulatorEngine.OnNewDASMString             += OnNewDASMString;
            m_objEmulatorEngine.OnNewDASMStringList         += OnNewDASMStringList;
            m_objEmulatorEngine.OnNewTraceString            += OnNewTraceString;
            m_objEmulatorEngine.OnNewTraceStringList        += OnNewTraceStringList;
            m_objEmulatorEngine.OnNew5471String             += OnNew5471String;
            m_objEmulatorEngine.OnNewPrinterString          += OnNewPrinterString;
            m_objEmulatorEngine.OnNewPrinterStringList      += OnNewPrinterStringList;
            m_objEmulatorEngine.OnNewHaltCode               += OnNewHaltCode;
            m_objEmulatorEngine.OnNew5475Code               += OnNew5475Code;
            m_objEmulatorEngine.OnNewProgramState           += OnNewProgramState;
            m_objEmulatorEngine.OnNewProcessorCheck1        += OnNewProcessorCheck1;
            m_objEmulatorEngine.OnNewProcessorCheck2        += OnNewProcessorCheck2;
            m_objEmulatorEngine.OnClearPrintOutputPanel     += OnClearPrintOutputPanel;
            m_objEmulatorEngine.OnClearGrayedLinesList      += OnClearGrayedLinesList;
            m_objEmulatorEngine.OnResetControlColors        += OnResetControlColors;
            m_objEmulatorEngine.OnResetTraceList            += OnResetTraceList;
            m_objEmulatorEngine.OnShowTraceState            += OnShowTraceState;
            m_objEmulatorEngine.OnInitializeTracePanel      += OnInitializeTracePanel;
            m_objEmulatorEngine.OnMakeRegisterLabelsDormant += OnMakeRegisterLabelsDormant;
            m_objEmulatorEngine.OnMakeRegisterLabelsActive  += OnMakeRegisterLabelsActive;
            m_objEmulatorEngine.OnUpdateAppTitle            += OnUpdateAppTitle;
            m_objEmulatorEngine.OnNewCPUDials               += OnNewCPUDials;
            m_objEmulatorEngine.OnNewEnableHighlightLine    += OnNewEnableHighlightLine;
            m_objEmulatorEngine.OnNewDisassemblyListing     += OnNewDisassemblyListing;

            LoadStateSettings ();

            //// Initialize timer
            //SystemTimer.Interval = new TimeSpan (0, 0, 0, 0, 50); // 50ms = 1/20th second
            //SystemTimer.Start ();

            LoadCPUData ();
            ShowTraceState ();
            Title = GetAppVersionAndProgramName ();

            m_objEmulatorEngine.m_objEmulatorState.ChangeState (CEmulatorState.EProgramState.PSTATE_1_NoProgramLoaded);
            m_objEmulatorEngine.FireMakeRegisterLabelsDormantEvent ();

            x_txbCPUDials.Text = string.Format ("{0:X4}", m_objEmulatorEngine.GetConsoleDials ());
            x_cdbxDisassemblyOutput.Focus ();
        }

        private void LoadCPUData ()
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

#if DEBUG
            m_dEffectiveSys3ClockMHz = m_uiCurrentClockSpeed / 142.31;
#else
            m_dEffectiveSys3ClockMHz = m_uiCurrentClockSpeed / 49.26;
#endif

            x_lblProcessorSpeed.Content = string.Format ("Effective System/3 clock speed: {0:F2}MHz", m_dEffectiveSys3ClockMHz);
        }

        public void OnAboutBox (object sender, RoutedEventArgs e)
        {
            ShowAboutBox ();
        }

        private string GetAppVersionAndProgramName ()
        {
            string strTitleAppendix = CAPTION_STRING + " " + m_strAssemblyVersion + ((IntPtr.Size == 8) ? " (64-bit)" : " (32-bit)") + " (" + m_strConfiguration + ")";
            if (m_objEmulatorEngine.m_objEmulatorState.IsProgramLoaded () &&
                m_strItemName.Length > 0)
            {
                strTitleAppendix += (" (" + PrepProgramName (m_strItemName, false) + ')');
            }

            return strTitleAppendix;
        }

        private void ShowAboutBox ()
        {
            string rstrShortName = "";
            string rstrFullName  = "";
            string rstrCSharpVer = "";
            string rstrDotNetVer = "";
            string rstrWpfVer    = "";
            string strCurrentVersions = GetCurrentVersions (ref rstrShortName, ref rstrFullName, ref rstrCSharpVer, ref rstrDotNetVer, ref rstrWpfVer);

            CAboutBox wndAboutBox = new CAboutBox (m_strAssemblyTitle, m_strAssemblyVersion, m_strCompanyName, m_strTargetNetVersion,
                                                   m_strConfiguration, strCurrentVersions, m_dtBuildDate);
            wndAboutBox.Show ();
        }

        public static string GetCurrentVersions (ref string rstrShortName, ref string rstrFullName, ref string rstrCSharpVer, ref string rstrDotNetVer, ref string rstrWpfVer)
        {
            //_MSC_VER  <--------------- VS Version ---------------->   C#    .NET   WPF
            //  1200    VS 6.0         Visual Studio 6.0
            //  1300    VS2002         Visual Studio .NET 2002 (7.0)    1.0   1
            //  1310    VS2003         Visual Studio .NET 2003 (7.1)    1.0   1.1
            //  1400    VS2005         Visual Studio 2005 (8.0)         2.0   2
            //  1500    VS2008         Visual Studio 2008 (9.0)         3.0   3.5    3.5
            //  1600    VS2010         Visual Studio 2010 (10.0)        4.0   4      4
            //  1700    VS2012         Visual Studio 2012 (11.0)        5.0   4.5    4.5
            //  1800    VS2013         Visual Studio 2013 (12.0)        5.0   4.5.1  4.5.1
            //  1900    VS2015         Visual Studio 2015 (14.0)        6.0   4.6    4.6
            //  1910    VS2017 15.0    Visual Studio 2017 RTW (15.0)    7.0   4.6.1
            //  1911    VS2017 15.3    Visual Studio 2017 version 15.3  7.0   4.6.2
            //  1912    VS2017 15.5    Visual Studio 2017 version 15.5  7.0
            //  1913    VS2017 15.6    Visual Studio 2017 version 15.6
            //  1914    VS2017 15.7    Visual Studio 2017 version 15.7
            //  1915    VS2017 15.8    Visual Studio 2017 version 15.8
            //  1916    VS2017 15.9    Visual Studio 2017 version 15.9
            //  1920    VS2019 16.0    Visual Studio 2019 RTW (16.0)
            //  1921    VS2019 16.1    Visual Studio 2019 version 16.1
            //  1922    VS2019 16.2    Visual Studio 2019 version 16.2
            //  1923    VS2019 16.3    Visual Studio 2019 version 16.3
            //  1924    VS2019 16.4    Visual Studio 2019 version 16.4
            //  1925    VS2019 16.5    Visual Studio 2019 version 16.5
            //  1926    VS2019 16.6    Visual Studio 2019 version 16.6
            //  1927    VS2019 16.7    Visual Studio 2019 version 16.7
            //  1928    VS2019 16.8    Visual Studio 2019 version 16.8

            StringBuilder sbCurrentVersions = new StringBuilder (); // Ver: 1900 (VS2015)  C# 6.0  .Net 4.6  WPF  4.6

            //Int32  i32CompilerVersion     = CompilerVersion.CCompilerVersion.GetMscVer ();
            //Int32  i32CompilerFullVersion = CompilerVersion.CCompilerVersion.GetMscFullVer ();
            Int32  i32CompilerVersion     = GetMscVersionLib.CGetMscVersionLib.GetCompilerVersionStatic ();
            Int32  i32CompilerFullVersion = GetMscVersionLib.CGetMscVersionLib.GetCompilerFullVersionStatic ();
            string strCompilerVersion     = string.Format ("{0:##-##}", i32CompilerVersion).Replace ('-', '.');
            if (i32CompilerFullVersion > 0)
            {
                strCompilerVersion = string.Format ("{0:##-#-#####-#}", i32CompilerFullVersion).Replace ('-', '.');
            }

            //Int32 i32MscCompilerVersion = CGetMscVersionLib.GetCompilerVersionStatic ();
            //Int32 i32MscCompilerFullVersion = CGetMscVersionLib.GetCompilerFullVersionStatic ();
            //string strMscCompilerFullVersion = string.Format ("{0:##-#-#####-#}", i32MscCompilerFullVersion).Replace ('-', '.');

            //Console.WriteLine ("CompilerVersion: " + i32CompilerVersion.ToString ());
            //Console.WriteLine ("CompilerFullVersion: " + i32CompilerFullVersion.ToString ());

//#if DEBUG
//            Debug.Assert (i32CompilerVersion == i32MscCompilerVersion);
//#endif

            //  1928    VS2019 16.8    Visual Studio 2019 version 16.8
            if (i32CompilerVersion >= 1928)
            {
                rstrShortName = "VS2019 16.8";
                rstrFullName  = "Visual Studio 2019 version 16.8";
                rstrCSharpVer = "";
                rstrDotNetVer = "";
                rstrWpfVer    = "";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
                if (rstrCSharpVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  C# " + rstrCSharpVer);
                }
                if (rstrDotNetVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  .Net " + rstrDotNetVer);
                }
                if (rstrWpfVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  WPF " + rstrWpfVer);
                }
            }
            //  1927    VS2019 16.7    Visual Studio 2019 version 16.7
            else if (i32CompilerVersion >= 1927)
            {
                rstrShortName = "VS2019 16.7";
                rstrFullName  = "Visual Studio 2019 version 16.7";
                rstrCSharpVer = "";
                rstrDotNetVer = "";
                rstrWpfVer    = "";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
                if (rstrCSharpVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  C# " + rstrCSharpVer);
                }
                if (rstrDotNetVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  .Net " + rstrDotNetVer);
                }
                if (rstrWpfVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  WPF " + rstrWpfVer);
                }
            }
            //  1926    VS2019 16.6    Visual Studio 2019 version 16.6
            else if (i32CompilerVersion >= 1926)
            {
                rstrShortName = "VS2019 16.6";
                rstrFullName  = "Visual Studio 2019 version 16.6";
                rstrCSharpVer = "";
                rstrDotNetVer = "";
                rstrWpfVer    = "";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
                if (rstrCSharpVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  C# " + rstrCSharpVer);
                }
                if (rstrDotNetVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  .Net " + rstrDotNetVer);
                }
                if (rstrWpfVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  WPF " + rstrWpfVer);
                }
            }
            //  1925    VS2019 16.5    Visual Studio 2019 version 16.5
            else if (i32CompilerVersion >= 1925)
            {
                rstrShortName = "VS2019 16.5";
                rstrFullName  = "Visual Studio 2019 version 16.5";
                rstrCSharpVer = "";
                rstrDotNetVer = "";
                rstrWpfVer    = "";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
                if (rstrCSharpVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  C# " + rstrCSharpVer);
                }
                if (rstrDotNetVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  .Net " + rstrDotNetVer);
                }
                if (rstrWpfVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  WPF " + rstrWpfVer);
                }
            }
            //  1924    VS2019 16.4    Visual Studio 2019 version 16.4
            else if (i32CompilerVersion >= 1924)
            {
                rstrShortName = "VS2019 16.4";
                rstrFullName  = "Visual Studio 2019 version 16.4";
                rstrCSharpVer = "";
                rstrDotNetVer = "";
                rstrWpfVer    = "";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
                if (rstrCSharpVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  C# " + rstrCSharpVer);
                }
                if (rstrDotNetVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  .Net " + rstrDotNetVer);
                }
                if (rstrWpfVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  WPF " + rstrWpfVer);
                }
            }
            //  1923    VS2019 16.3    Visual Studio 2019 version 16.3
            else if (i32CompilerVersion >= 1923)
            {
                rstrShortName = "VS2019 16.3";
                rstrFullName  = "Visual Studio 2019 version 16.3";
                rstrCSharpVer = "";
                rstrDotNetVer = "";
                rstrWpfVer    = "";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
                if (rstrCSharpVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  C# " + rstrCSharpVer);
                }
                if (rstrDotNetVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  .Net " + rstrDotNetVer);
                }
                if (rstrWpfVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  WPF " + rstrWpfVer);
                }
            }
            //  1922    VS2019 16.2    Visual Studio 2019 version 16.2
            else if (i32CompilerVersion >= 1922)
            {
                rstrShortName = "VS2019 16.2";
                rstrFullName  = "Visual Studio 2019 version 16.2";
                rstrCSharpVer = "";
                rstrDotNetVer = "";
                rstrWpfVer    = "";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
                if (rstrCSharpVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  C# " + rstrCSharpVer);
                }
                if (rstrDotNetVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  .Net " + rstrDotNetVer);
                }
                if (rstrWpfVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  WPF " + rstrWpfVer);
                }
            }
            //  1921    VS2019 16.1    Visual Studio 2019 version 16.1
            else if (i32CompilerVersion >= 1921)
            {
                rstrShortName = "VS2019 16.1";
                rstrFullName  = "Visual Studio 2019 version 16.1";
                rstrCSharpVer = "";
                rstrDotNetVer = "";
                rstrWpfVer    = "";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
                if (rstrCSharpVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  C# " + rstrCSharpVer);
                }
                if (rstrDotNetVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  .Net " + rstrDotNetVer);
                }
                if (rstrWpfVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  WPF " + rstrWpfVer);
                }
            }
            //  1920    VS2019 16.0    Visual Studio 2019 RTW (16.0)
            else if (i32CompilerVersion >= 1920)
            {
                rstrShortName = "VS2019 16.0";
                rstrFullName  = "Visual Studio 2019 RTW (16.0)";
                rstrCSharpVer = "";
                rstrDotNetVer = "";
                rstrWpfVer    = "";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
                if (rstrCSharpVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  C# " + rstrCSharpVer);
                }
                if (rstrDotNetVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  .Net " + rstrDotNetVer);
                }
                if (rstrWpfVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  WPF " + rstrWpfVer);
                }
            }
            //  1916    VS2017 15.9    Visual Studio 2017 version 15.9
            else if (i32CompilerVersion >= 1916)
            {
                rstrShortName = "VS2017 15.9";
                rstrFullName  = "Visual Studio 2017 version 15.9";
                rstrCSharpVer = "";
                rstrDotNetVer = "";
                rstrWpfVer    = "";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
                if (rstrCSharpVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  C# " + rstrCSharpVer);
                }
                if (rstrDotNetVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  .Net " + rstrDotNetVer);
                }
                if (rstrWpfVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  WPF " + rstrWpfVer);
                }
            }
            //  1915    VS2017 15.8    Visual Studio 2017 version 15.8
            else if (i32CompilerVersion >= 1915)
            {
                rstrShortName = "VS2017 15.8";
                rstrFullName  = "Visual Studio 2017 version 15.8";
                rstrCSharpVer = "";
                rstrDotNetVer = "";
                rstrWpfVer    = "";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
                if (rstrCSharpVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  C# " + rstrCSharpVer);
                }
                if (rstrDotNetVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  .Net " + rstrDotNetVer);
                }
                if (rstrWpfVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  WPF " + rstrWpfVer);
                }
            }
            //  1914    VS2017 15.7    Visual Studio 2017 version 15.7
            else if (i32CompilerVersion >= 1914)
            {
                rstrShortName = "VS2017 15.7";
                rstrFullName  = "Visual Studio 2017 version 15.7";
                rstrCSharpVer = "";
                rstrDotNetVer = "";
                rstrWpfVer    = "";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
                if (rstrCSharpVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  C# " + rstrCSharpVer);
                }
                if (rstrDotNetVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  .Net " + rstrDotNetVer);
                }
                if (rstrWpfVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  WPF " + rstrWpfVer);
                }
            }
            //  1913    VS2017 15.6    Visual Studio 2017 version 15.6
            else if (i32CompilerVersion >= 1913)
            {
                rstrShortName = "VS2017 15.6";
                rstrFullName  = "Visual Studio 2017 version 15.6";
                rstrCSharpVer = "";
                rstrDotNetVer = "";
                rstrWpfVer    = "";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
                if (rstrCSharpVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  C# " + rstrCSharpVer);
                }
                if (rstrDotNetVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  .Net " + rstrDotNetVer);
                }
                if (rstrWpfVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  WPF " + rstrWpfVer);
                }
            }
            //  1912    VS2017 15.5    Visual Studio 2017 version 15.5  7.0
            else if (i32CompilerVersion >= 1912)
            {
                rstrShortName = "VS2017 15.5";
                rstrFullName  = "Visual Studio 2017 version 15.5";
                rstrCSharpVer = "7.0";
                rstrDotNetVer = "";
                rstrWpfVer    = "";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
                if (rstrCSharpVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  C# " + rstrCSharpVer);
                }
                if (rstrDotNetVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  .Net " + rstrDotNetVer);
                }
                if (rstrWpfVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  WPF " + rstrWpfVer);
                }
            }
            //  1911    VS2017 15.3    Visual Studio 2017 version 15.3  7.0   4.6.2
            else if (i32CompilerVersion >= 1911)
            {
                rstrShortName = "VS2017 15.3";
                rstrFullName  = "Visual Studio 2017 version 15.3";
                rstrCSharpVer = "7.0";
                rstrDotNetVer = "4.6.2";
                rstrWpfVer    = "";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
                if (rstrCSharpVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  C# " + rstrCSharpVer);
                }
                if (rstrDotNetVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  .Net " + rstrDotNetVer);
                }
                if (rstrWpfVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  WPF " + rstrWpfVer);
                }
            }
            //  1910    VS2017 15.0    Visual Studio 2017 RTW (15.0)    7.0   4.6.1
            else if (i32CompilerVersion >= 1910)
            {
                rstrShortName = "VS2017 15.0";
                rstrFullName  = "Visual Studio 2017 RTW (15.0)";
                rstrCSharpVer = "7.0";
                rstrDotNetVer = "4.6.1";
                rstrWpfVer    = "";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
                if (rstrCSharpVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  C# " + rstrCSharpVer);
                }
                if (rstrDotNetVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  .Net " + rstrDotNetVer);
                }
                if (rstrWpfVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  WPF " + rstrWpfVer);
                }
            }
            //  1900    VS2015         Visual Studio 2015 (14.0)        6.0   4.6    4.6
            else if (i32CompilerVersion >= 1900)
            {
                rstrShortName = "VS2015";
                rstrFullName  = "Visual Studio 2015 (14.0)";
                rstrCSharpVer = "6.0";
                rstrDotNetVer = "4.6";
                rstrWpfVer    = "4.6";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
                if (rstrCSharpVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  C# " + rstrCSharpVer);
                }
                if (rstrDotNetVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  .Net " + rstrDotNetVer);
                }
                if (rstrWpfVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  WPF " + rstrWpfVer);
                }
            }
            //  1800    VS2013         Visual Studio 2013 (12.0)        5.0   4.5.1  4.5.1
            else if (i32CompilerVersion >= 1800)
            {
                rstrShortName = "VS2013";
                rstrFullName  = "Visual Studio 2013 (12.0)";
                rstrCSharpVer = "5.0";
                rstrDotNetVer = "4.5.1";
                rstrWpfVer    = "4.5.1";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
                if (rstrCSharpVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  C# " + rstrCSharpVer);
                }
                if (rstrDotNetVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  .Net " + rstrDotNetVer);
                }
                if (rstrWpfVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  WPF " + rstrWpfVer);
                }
            }
            //  1700    VS2012         Visual Studio 2012 (11.0)        5.0   4.5    4.5
            else if (i32CompilerVersion >= 1700)
            {
                rstrShortName = "VS2012";
                rstrFullName  = "Visual Studio 2012 (11.0)";
                rstrCSharpVer = "5.0";
                rstrDotNetVer = "4.5";
                rstrWpfVer    = "4.5";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
                if (rstrCSharpVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  C# " + rstrCSharpVer);
                }
                if (rstrDotNetVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  .Net " + rstrDotNetVer);
                }
                if (rstrWpfVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  WPF " + rstrWpfVer);
                }
            }
            //  1600    VS2010         Visual Studio 2010 (10.0)        4.0   4.0    4.0
            else if (i32CompilerVersion >= 1600)
            {
                rstrShortName = "VS2010";
                rstrFullName  = "Visual Studio 2010 (10.0)";
                rstrCSharpVer = "4.0";
                rstrDotNetVer = "4.0";
                rstrWpfVer    = "4.0";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
                if (rstrCSharpVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  C# " + rstrCSharpVer);
                }
                if (rstrDotNetVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  .Net " + rstrDotNetVer);
                }
                if (rstrWpfVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  WPF " + rstrWpfVer);
                }
            }
            //  1500    VS2008         Visual Studio 2008 (9.0)         3.0   3.5    3.5
            else if (i32CompilerVersion >= 1500)
            {
                rstrShortName = "VS2008";
                rstrFullName  = "Visual Studio 2008 (9.0)";
                rstrCSharpVer = "3.0";
                rstrDotNetVer = "3.5";
                rstrWpfVer    = "3.5";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
                if (rstrCSharpVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  C# " + rstrCSharpVer);
                }
                if (rstrDotNetVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  .Net " + rstrDotNetVer);
                }
                if (rstrWpfVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  WPF " + rstrWpfVer);
                }
            }
            //  1400    VS2005         Visual Studio 2005 (8.0)         2.0   2.0
            else if (i32CompilerVersion >= 1400)
            {
                rstrShortName = "VS2005";
                rstrFullName  = "Visual Studio 2005 (8.0)";
                rstrCSharpVer = "2.0";
                rstrDotNetVer = "2.0";
                rstrWpfVer    = "";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
                if (rstrCSharpVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  C# " + rstrCSharpVer);
                }
                if (rstrDotNetVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  .Net " + rstrDotNetVer);
                }
                if (rstrWpfVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  WPF " + rstrWpfVer);
                }
            }
            //  1310    VS2003         Visual Studio .NET 2003 (7.1)    1.0   1.1
            else if (i32CompilerVersion >= 1310)
            {
                rstrShortName = "VS2003";
                rstrFullName  = "Visual Studio .NET 2003 (7.1)";
                rstrCSharpVer = "1.0";
                rstrDotNetVer = "1.1";
                rstrWpfVer    = "";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
                if (rstrCSharpVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  C# " + rstrCSharpVer);
                }
                if (rstrDotNetVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  .Net " + rstrDotNetVer);
                }
                if (rstrWpfVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  WPF " + rstrWpfVer);
                }
            }
            //  1300    VS2002         Visual Studio .NET 2002 (7.0)    1.0   1.0
            else if (i32CompilerVersion >= 1300)
            {
                rstrShortName = "VS2002";
                rstrFullName  = "Visual Studio .NET 2002 (7.0)";
                rstrCSharpVer = "1.0";
                rstrDotNetVer = "1.0";
                rstrWpfVer    = "";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
                if (rstrCSharpVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  C# " + rstrCSharpVer);
                }
                if (rstrDotNetVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  .Net " + rstrDotNetVer);
                }
                if (rstrWpfVer.Length > 0)
                {
                    sbCurrentVersions.Append ("  WPF " + rstrWpfVer);
                }
            }
            //  1200    VS 6.0         Visual Studio 6.0
            else if (i32CompilerVersion == 1200)
            {
                rstrShortName = "VS 6.0";
                rstrFullName  = "Visual Studio 6.0";
                rstrCSharpVer = "";
                rstrDotNetVer = "";
                rstrWpfVer    = "";
                sbCurrentVersions.Append ("MSC Ver: " + strCompilerVersion + " (" + rstrShortName + ")");
            }

            return sbCurrentVersions.ToString ();
        }

        public void LoadVersionData (System.Reflection.Assembly asm)
        {
            // Get the AssemblyVersion from the Fullname string
            // "PlotterWriterWpfUI, Version=0.2.1.5, Culture=neutral, PublicKeyToken=null"
            string strAsmFullname = asm.FullName;
            int iVersionStart = strAsmFullname.IndexOf (STRING_VERSION) + STRING_VERSION.Length;
            int iVersionLength = (iVersionStart > 0) ? strAsmFullname.IndexOf (',', iVersionStart) - iVersionStart : -1;
            if (iVersionStart > 0 &&
                iVersionLength > 0)
            {
                m_strAssemblyVersion = strAsmFullname.Substring (iVersionStart, iVersionLength);
            }

            IEnumerable<System.Reflection.CustomAttributeData> ienumCustAttribData = asm.CustomAttributes;
            foreach (System.Reflection.CustomAttributeData cad in ienumCustAttribData)
            {
                string strCad = cad.ToString ();
                if (strCad.IndexOf ("AssemblyTitle") >= 0)
                {
                    m_strAssemblyTitle = ExtractStringInQuotes (strCad);
                }
                else if (strCad.IndexOf ("AssemblyFileVersion") >= 0)
                {
                    m_strAssemblyFileVersion = ExtractStringInQuotes (strCad);
                }
                else if (strCad.IndexOf ("AssemblyCompany") >= 0)
                {
                    m_strCompanyName = ExtractStringInQuotes (strCad);
                }
                else if (strCad.IndexOf ("AssemblyConfiguration") >= 0)
                {
                    m_strConfiguration = ExtractStringInQuotes (strCad);
                }
                else if (strCad.IndexOf ("TargetFramework") >= 0)
                {
                    //"[System.Runtime.Versioning.TargetFrameworkAttribute(\".NETFramework,Version=v4.5\", FrameworkDisplayName = \".NET Framework 4.5\")]"
                    m_strTargetNetVersion = ExtractStringInQuotes (strCad, false);
                }

                if (m_strAssemblyTitle.Length    > 0 &&
                    m_strAssemblyVersion.Length  > 0 &&
                    m_strTargetNetVersion.Length > 0 &&
                    m_strCompanyName.Length      > 0)
                {
                    break;
                }
            }
        }

        public string ExtractStringInQuotes (string strIn, bool bFirstString = true)
        {
            int iFirstQuote = bFirstString ? strIn.IndexOf ('\"') : strIn.LastIndexOf ('\"');
            int iLastQuote = -1;
            if (iFirstQuote >= 0)
            {
                if (bFirstString)
                {
                    if (iFirstQuote < strIn.Length - 1)
                    {
                        iLastQuote = strIn.IndexOf ('\"', iFirstQuote + 1);
                    }
                }
                else
                {
                    if (iFirstQuote > 0 &&
                        iFirstQuote <= strIn.Length - 1)
                    {
                        iLastQuote = strIn.LastIndexOf ('\"', iFirstQuote - 1);
                    }
                }
            }

            if (iFirstQuote > 0)
            {
                if (bFirstString)
                {
                    if (iLastQuote > iFirstQuote)
                    {
                        return strIn.Substring (iFirstQuote + 1, iLastQuote - iFirstQuote - 1);
                    }
                }
                else
                {
                    if (iLastQuote < iFirstQuote)
                    {
                        return strIn.Substring (iLastQuote + 1, iFirstQuote - iLastQuote - 1);
                    }
                }
            }

            return "";
        }

        private void ManageSubscriptions (bool bSubscribe = true)
        {
            if (bSubscribe)
            {
                Closing          += OnWindowClosing;
                //KeyDown          += OnKeyDown; // Redundant when referenced in xaml for the top-level window
                //KeyUp            += OnKeyUp;   // Redundant when referenced in xaml for the top-level window
                //SizeChanged      += OnSizeChanged;
                //SystemTimer.Tick += OnTimerClick;
            }
            else
            {
                Closing          -= OnWindowClosing;
                //KeyDown          -= OnKeyDown;
                //KeyUp            -= OnKeyUp;
                //SizeChanged      -= OnSizeChanged;
                //SystemTimer.Tick -= OnTimerClick;
            }
        }

        //private void OnKeyDown (object sender, KeyEventArgs e)
        //{
        //    Console.WriteLine ("OnKeyDown: " + e.Key);
        //    lock (m_objEmulatorEngine.m_objLock)
        //    {
        //        Key key = e.Key;
        //        char c = (char)key;
        //        string s = key.ToString ();
        //    }
        //}

        //private char KeyToChar (Key key)
        //{
        //    bool rbIsCtrlDown = false;
        //    char cReturn = KeyToChar (key, ref rbIsCtrlDown);
        //    return rbIsCtrlDown ? '\x00' : cReturn;
        //}

        private char KeyToChar (Key key, ref bool rbIsCtrlDown)
        {
            if (Keyboard.IsKeyDown (Key.LeftAlt) ||
                Keyboard.IsKeyDown (Key.RightAlt))
            {
                m_objEmulatorEngine.SetKeyAlt (true);
                return '\x00';
            }
            else
            {
                m_objEmulatorEngine.SetKeyAlt (false);
            }

            bool bCapLock = Console.CapsLock;
            bool bShift   = Keyboard.IsKeyDown (Key.LeftShift) || 
                            Keyboard.IsKeyDown (Key.RightShift);
            bool bIsCtrl  = Keyboard.IsKeyDown (Key.LeftCtrl) ||
                            Keyboard.IsKeyDown (Key.RightCtrl);
            //bool bIsAlt   = Keyboard.IsKeyDown (Key.LeftAlt) ||
            //                Keyboard.IsKeyDown (Key.RightAlt);
            bool bIsCap   = (bCapLock && !bShift) || (!bCapLock && bShift);
            rbIsCtrlDown = bIsCtrl;
            Console.WriteLine ("KeyToChar:" + " bCapLock: " + bCapLock.ToString () + 
                                              " bShift: "   + bShift.ToString ()   +
                                              " bIsCtrl: "  + bIsCtrl.ToString ()  +
                                              //" bIsAlt: "   + bIsAlt.ToString ()   +
                                              " Key: "      + key.ToString ());

            switch(key)
            {
                case Key.A:               return (bIsCap || bIsCtrl ? 'A' : 'a');
                case Key.B:               return (bIsCap || bIsCtrl ? 'B' : 'b');
                case Key.C:               return (bIsCap || bIsCtrl ? 'C' : 'c');
                case Key.D:               return (bIsCap || bIsCtrl ? 'D' : 'd');
                case Key.E:               return (bIsCap || bIsCtrl ? 'E' : 'e');
                case Key.F:               return (bIsCap || bIsCtrl ? 'F' : 'f');
                case Key.G:               return (bIsCap || bIsCtrl ? 'G' : 'g');
                case Key.H:               return (bIsCap || bIsCtrl ? 'H' : 'h');
                case Key.I:               return (bIsCap || bIsCtrl ? 'I' : 'i');
                case Key.J:               return (bIsCap || bIsCtrl ? 'J' : 'j');
                case Key.K:               return (bIsCap || bIsCtrl ? 'K' : 'k');
                case Key.L:               return (bIsCap || bIsCtrl ? 'L' : 'l');
                case Key.M:               return (bIsCap || bIsCtrl ? 'M' : 'm');
                case Key.N:               return (bIsCap || bIsCtrl ? 'N' : 'n');
                case Key.O:               return (bIsCap || bIsCtrl ? 'O' : 'o');
                case Key.P:               return (bIsCap || bIsCtrl ? 'P' : 'p');
                case Key.Q:               return (bIsCap || bIsCtrl ? 'Q' : 'q');
                case Key.R:               return (bIsCap || bIsCtrl ? 'R' : 'r');
                case Key.S:               return (bIsCap || bIsCtrl ? 'S' : 's');
                case Key.T:               return (bIsCap || bIsCtrl ? 'T' : 't');
                case Key.U:               return (bIsCap || bIsCtrl ? 'U' : 'u');
                case Key.V:               return (bIsCap || bIsCtrl ? 'V' : 'v');
                case Key.W:               return (bIsCap || bIsCtrl ? 'W' : 'w');
                case Key.X:               return (bIsCap || bIsCtrl ? 'X' : 'x');
                case Key.Y:               return (bIsCap || bIsCtrl ? 'Y' : 'y');
                case Key.Z:               return (bIsCap || bIsCtrl ? 'Z' : 'z');
                case Key.D0:              return (bShift ? ')' : '0');
                case Key.D1:              return (bShift ? '!' : '1');
                case Key.D2:              return (bShift ? '@' : '2');
                case Key.D3:              return (bShift ? '#' : '3');
                case Key.D4:              return (bShift ? '$' : '4');
                case Key.D5:              return (bShift ? '%' : '5');
                case Key.D6:              return (bShift ? '^' : '6');
                case Key.D7:              return (bShift ? '&' : '7');
                case Key.D8:              return (bShift ? '*' : '8');
                case Key.D9:              return (bShift ? '(' : '9');
                case Key.OemPlus:         return (bShift ? '+' : '=');
                case Key.OemMinus:        return (bShift ? '_' : '-');
                case Key.OemQuestion:     return (bShift ? '?' : '/');
                case Key.OemComma:        return (bShift ? '<' : ',');
                case Key.OemPeriod:       return (bShift ? '>' : '.');
                case Key.OemOpenBrackets: return (bShift ? '{' : '[');
                case Key.OemQuotes:       return (bShift ? '"' : '\'');
                case Key.Oem1:            return (bShift ? ':' : ';');
                case Key.Oem3:            return (bShift ? '~' : '`');                   
                case Key.Oem5:            return (bShift ? '|' : '\\');
                case Key.Oem6:            return (bShift ? '}' : ']');
                case Key.Enter:           return '\n';
                case Key.Tab:             return '\t';
                case Key.Space:           return ' ';

                // Number Pad
                case Key.NumPad0:         return '0';
                case Key.NumPad1:         return '1';
                case Key.NumPad2:         return '2';
                case Key.NumPad3:         return '3';
                case Key.NumPad4:         return '4';
                case Key.NumPad5:         return '5';
                case Key.NumPad6:         return '6';
                case Key.NumPad7:         return '7';
                case Key.NumPad8:         return '8';
                case Key.NumPad9:         return '9';
                case Key.Subtract:        return '-';
                case Key.Add:             return '+';
                case Key.Decimal:         return '.';
                case Key.Divide:          return '/';
                case Key.Multiply:        return '*';

                default:                  return '\x00';
            }
        }

        private void HandleConsoleDialSpining (bool bIncrement)
        {
            int iCaretPos = x_txbCPUDials.SelectionStart;
            ushort usConsoldeDialsValue = m_objEmulatorEngine.GetConsoleDials ();

            if (iCaretPos < 4)
            {
                byte[] yaDials = CDataConversion.ConvertUShortToByteArray (usConsoldeDialsValue);

                if (bIncrement)
                {
                    yaDials[iCaretPos]++;
                }
                else
                {
                    yaDials[iCaretPos]--;
                }

                usConsoldeDialsValue = CDataConversion.ConvertByteArrayToUShort (yaDials);
            }
            else
            {
                if (bIncrement)
                {
                    usConsoldeDialsValue++;
                }
                else
                {
                    if (usConsoldeDialsValue == 0)
                    {
                        usConsoldeDialsValue = 0xFFFF;
                    }
                    else
                    {
                        usConsoldeDialsValue--;
                    }
                }
            }

            m_objEmulatorEngine.SetConsoleDials (usConsoldeDialsValue);
            x_txbCPUDials.Text = usConsoldeDialsValue.ToString ("X4");
            x_txbCPUDials.SelectionStart = iCaretPos;

            //ushort us = (ushort)((usConsoldeDialsValue << (iCaretPos * 4)) & 0xF000);

            //byte y = (byte)us;
            //ushort us0 = (ushort)(usConsoldeDialsValue << 0);
            //ushort us1 = (ushort)(usConsoldeDialsValue << 4);
            //ushort us2 = (ushort)(usConsoldeDialsValue << 8);
            //ushort us3 = (ushort)(usConsoldeDialsValue << 12);
        }

        private void OnKeyUp (object sender, KeyEventArgs e)
        {
            //Console.WriteLine ("OnKeyUp: " + e.Key);
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Key key = e.Key;
                //char c = (char)key;
                //string s1 = key.ToString ();
                //ModifierKeys mk = e.KeyboardDevice.Modifiers;
                //string s2 = mk.ToString ();
                //Console.WriteLine ("OnKeyUp: " + ((byte)c).ToString () + "  " + s1 + "  " + s2 + " {" + KeyToChar (e.Key) + "}");
                if (m_strLastFocusPanel == "PrinterOutput")
                {
                    if (e.Key == Key.Home)
                    {
                        x_tbxPrinterOutput.ScrollToHome ();
                    }
                    else if (e.Key == Key.End)
                    {
                        x_tbxPrinterOutput.ScrollToEnd();
                    }
                    else if (e.Key == Key.PageUp)
                    {
                        x_tbxPrinterOutput.PageUp ();
                    }
                    else if (e.Key == Key.PageDown)
                    {
                        x_tbxPrinterOutput.PageDown ();
                    }
                    else if (e.Key == Key.Up)
                    {
                        x_tbxPrinterOutput.LineUp ();
                    }
                    else if (e.Key == Key.Down)
                    {
                        x_tbxPrinterOutput.LineDown ();
                    }
                }
                else if (m_strLastFocusPanel == "TraceOutput")
                {
                    if (e.Key == Key.Home)
                    {
                        x_cdbxTraceOutput.ScrollToHome (); //.OnHomeKey ();
                    }
                    else if (e.Key == Key.End)
                    {
                        x_cdbxTraceOutput.ScrollToEnd (); //.OnEndKey ();
                    }
                    else if (e.Key == Key.PageUp)
                    {
                        x_cdbxTraceOutput.PageUp (); //.OnPageUpKey ();
                    }
                    else if (e.Key == Key.PageDown)
                    {
                        x_cdbxTraceOutput.PageDown (); //.OnPageDownKey ();
                    }
                    else if (e.Key == Key.Up)
                    {
                        x_cdbxTraceOutput.LineUp (); //.OnUpArrowKey ();
                    }
                    else if (e.Key == Key.Down)
                    {
                        x_cdbxTraceOutput.LineDown (); //.OnDownArrowKey ();
                    }
                }
                else if (m_strLastFocusPanel == "DASM")
                {
                    if (e.Key == Key.Home)
                    {
                        x_cdbxDisassemblyOutput.ScrollToHome (); //.OnHomeKey ();
                    }
                    else if (e.Key == Key.End)
                    {
                        x_cdbxDisassemblyOutput.ScrollToEnd (); //.OnEndKey ();
                    }
                    else if (e.Key == Key.PageUp)
                    {
                        x_cdbxDisassemblyOutput.PageUp (); //.OnPageUpKey ();
                    }
                    else if (e.Key == Key.PageDown)
                    {
                        x_cdbxDisassemblyOutput.PageDown (); //.OnPageDownKey ();
                    }
                    else if (e.Key == Key.Up)
                    {
                        x_cdbxDisassemblyOutput.LineUp (); //.OnUpArrowKey ();
                    }
                    else if (e.Key == Key.Down)
                    {
                        x_cdbxDisassemblyOutput.LineDown (); //.OnDownArrowKey ();
                    }
                    else if (e.Key == Key.Left)
                    {
                        x_cdbxDisassemblyOutput.SetCurrentLineIdx (Math.Max (--m_iCurrentLineIdx, 0));
                    }
                    else if (e.Key == Key.Right)
                    {
                        x_cdbxDisassemblyOutput.SetCurrentLineIdx (++m_iCurrentLineIdx);
                    }
                }

                if (m_objEmulatorEngine.IsKeyboardInputEnabled ())
                //if (m_objEmulatorEngine.IsDataKeyInputEnabled ())
                {
                    bool bKeyCtrl = false;
                    m_objEmulatorEngine.SetKeyStroke (KeyToChar (e.Key, ref bKeyCtrl));
                    m_objEmulatorEngine.SetKeyCtrl (bKeyCtrl);
                }
            }

            base.OnKeyUp (e);
        }

        public void OnWindowClosing (object sender, CancelEventArgs e)
        {
            SaveStateSettings ();
            ManageSubscriptions (false);
        }

        #region Registry save & restore methods
        private void SaveSettingCurrentUser (string strRegistryPath, string strValueName, object value)
        {
            RegistryKey rkPathKey = Registry.CurrentUser.CreateSubKey (strRegistryPath);
            if (rkPathKey != null)
            {
                rkPathKey.SetValue (strValueName, value);
            }
        }

        public void LoadWindowPosition (Window wndTarget, string strWindowPosTop, string strWindowPosLeft, string strWindowPosHeight,
                                        string strWindowPosWidth, string strWindowMaximized)
        {
            // If window position found, move window there
            if (strWindowPosTop != null       &&
                strWindowPosTop.Length > 0    &&
                strWindowPosLeft != null      &&
                strWindowPosLeft.Length > 0   &&
                strWindowPosHeight != null    &&
                strWindowPosHeight.Length > 0 &&
                strWindowPosWidth != null     &&
                strWindowPosWidth.Length > 0)
            {
                double dTop    = 0.0;
                double dLeft   = 0.0;
                double dHeight = 0.0;
                double dWidth  = 0.0;
                bool bSuccess;

                try
                {
                    dTop     = Convert.ToDouble (strWindowPosTop);
                    dLeft    = Convert.ToDouble (strWindowPosLeft);
                    dHeight  = Convert.ToDouble (strWindowPosHeight);
                    dWidth   = Convert.ToDouble (strWindowPosWidth);
                    bSuccess = true;
                }
                catch
                {
                    bSuccess = false;
                }

                if (bSuccess)
                {
                    wndTarget.Top    = dTop;
                    wndTarget.Left   = dLeft;
                    wndTarget.Height = dHeight;
                    wndTarget.Width  = dWidth;
                }

                bool bIsMaxized = (strWindowMaximized != null    &&
                                   strWindowMaximized.Length > 0 &&
                                   strWindowMaximized != "0"     &&
                                   strWindowMaximized.ToLower () != "false");

                WindowState = bIsMaxized ? WindowState.Maximized : WindowState.Normal;
            }
        }

        //private bool DoesCurrentUserValueExist (string strRegistryPath, string strValueName)
        //{
        //    RegistryKey rkPathKey  = Registry.CurrentUser.OpenSubKey (strRegistryPath, true);
        //    if (rkPathKey == null)
        //    {
        //        return false;
        //    }

        //    object objRegValue = rkPathKey.GetValue (strValueName);
        //    try
        //    {
        //        return (objRegValue.ToString ().Length > 0);
        //    }
        //    catch (Exception)
        //    {
        //        // Fail silently
        //        return false;
        //    }
        //}

        //private bool DeleteSettingCurrentUser (string strRegistryPath, string strValueName)
        //{
        //    RegistryKey rkPathKey = Registry.CurrentUser.OpenSubKey (strRegistryPath, true);
        //    if (rkPathKey != null)
        //    {
        //        object objRegValue = rkPathKey.GetValue (strValueName);
        //        if (objRegValue != null)
        //        {
        //            try
        //            {
        //                rkPathKey.DeleteValue (strValueName);
        //                return true;
        //            }
        //            catch (Exception)
        //            {
        //                // Fail silently
        //                return false;
        //            }
        //        }
        //    }

        //    return false;
        //}

        private string LoadSetting (string strRegistryPath, string strValueName)
        {
            RegistryKey rkPathKey = Registry.CurrentUser.OpenSubKey (strRegistryPath, true);
            if (rkPathKey == null)
            {
                return "";
            }

            object objRegValue = rkPathKey.GetValue (strValueName);
            try
            {
                //x_menuRestoreSettings.IsEnabled = true;

                // Treat all types as string
                return objRegValue.ToString ();
            }
            catch (Exception)
            {
                // Fail silently
            }

            return "";
        }

        //private bool DeleteKey (string strRegistryPath, string strAppName)
        //{
        //    RegistryKey rkPathKey  = Registry.CurrentUser.OpenSubKey (strRegistryPath, true);

        //    if (rkPathKey != null)
        //    {
        //        rkPathKey.DeleteSubKeyTree (strAppName, false);

        //        string[] straSubKeyNames = rkPathKey.GetSubKeyNames ();
        //        rkPathKey.Close ();
        //        if (straSubKeyNames.Length == 0)
        //        {
        //            return true; // True indicates parent key path is empty
        //        }
        //    }

        //    return false;
        //}

        private void SaveStateSettings ()
        {
            string strRegistryPath = WPF_SYM3_PATH_STRING;

            // Save window position to registry
            SaveSettingCurrentUser (strRegistryPath, WINDOW_POS_TOP_STRING, Top);
            SaveSettingCurrentUser (strRegistryPath, WINDOW_POS_LEFT_STRING, Left);
            SaveSettingCurrentUser (strRegistryPath, WINDOW_POS_HEIGHT_STRING, Height);
            SaveSettingCurrentUser (strRegistryPath, WINDOW_POS_WIDTH_STRING, Width);
            SaveSettingCurrentUser (strRegistryPath, WINDOW_MAXIMIZED_STRING, WindowState == WindowState.Maximized ? "true" : "false");
        }

        private void LoadStateSettings ()
        {
            string strRegistryPath    = WPF_SYM3_PATH_STRING;
            string strWindowPosTop    = LoadSetting (strRegistryPath, WINDOW_POS_TOP_STRING);
            string strWindowPosLeft   = LoadSetting (strRegistryPath, WINDOW_POS_LEFT_STRING);
            string strWindowPosHeight = LoadSetting (strRegistryPath, WINDOW_POS_HEIGHT_STRING);
            string strWindowPosWidth  = LoadSetting (strRegistryPath, WINDOW_POS_WIDTH_STRING);
            string strWindowMaximized = LoadSetting (strRegistryPath, WINDOW_MAXIMIZED_STRING);

            LoadWindowPosition (this, strWindowPosTop, strWindowPosLeft, strWindowPosHeight, strWindowPosWidth, strWindowMaximized);
        }
        #endregion

        #region Panel docking methods
        // Toggle between docked and undocked states (Pane 1)
        public void OnClickPane1TraceOutputPin (object sender, RoutedEventArgs e) // Trace Output
        {
            if (x_btnPane2TraceOutput.Visibility == Visibility.Collapsed)
            {
                UndockPane (1);
            }
            else
            {
                DockPane (1);
            }
        }

        // Toggle between docked and undocked states (Pane 2)
        public void OnClickPane2PrinterOutputPin (object sender, RoutedEventArgs e) // Printer Output
        {
            if (x_btnPane2PrinterOutput.Visibility == Visibility.Collapsed)
            {
                UndockPane (2);
            }
            else
            {
                DockPane (2);
            }
        }

        // Show Pane 1 when hovering over its button
        public void OnMouseEnterPane1TraceOutputTab (object sender, RoutedEventArgs e) // Trace Output
        {
            x_gridLayer1TraceOutput.Visibility = Visibility.Visible;

            // Adjust Z order to ensure the pane is on top:
            x_gridParent.Children.Remove (x_gridLayer1TraceOutput);
            x_gridParent.Children.Add (x_gridLayer1TraceOutput);

            // Ensure the other pane is hidden if it is undocked
            if (x_btnPane2PrinterOutput.Visibility == Visibility.Visible)
                x_gridLayer2PrinterOutput.Visibility = Visibility.Collapsed;

            if (x_btnPane3TreeViewDB.Visibility == Visibility.Visible)
                x_gridLayer3TreeViewDB.Visibility = Visibility.Collapsed;
        }

        // Show Pane 2 when hovering over its button
        public void OnMouseEnterPane2PrinterOutputTab (object sender, RoutedEventArgs e) // Printer Output
        {
            x_gridLayer2PrinterOutput.Visibility = Visibility.Visible;

            // Adjust Z order to ensure the pane is on top:
            x_gridParent.Children.Remove (x_gridLayer2PrinterOutput);
            x_gridParent.Children.Add (x_gridLayer2PrinterOutput);

            // Ensure the other pane is hidden if it is undocked
            if (x_btnPane2TraceOutput.Visibility == Visibility.Visible)
                x_gridLayer1TraceOutput.Visibility = Visibility.Collapsed;

            if (x_btnPane3TreeViewDB.Visibility == Visibility.Visible)
                x_gridLayer3TreeViewDB.Visibility = Visibility.Collapsed;
        }

        // Show Pane 3 when hovering over its button
        public void OnMouseEnterPane3TreeViewDBTab (object sender, RoutedEventArgs e) // Database TreeView
        {
            if (m_objEmulatorEngine.m_objEmulatorState.IsProgramLoaded ())
            {
                return;
            }

            x_gridLayer3TreeViewDB.Visibility = Visibility.Visible;

            // Adjust Z order to ensure the pane is on top:
            x_gridParent.Children.Remove (x_gridLayer3TreeViewDB);
            x_gridParent.Children.Add (x_gridLayer3TreeViewDB);

            // Ensure the other pane is hidden if it is undocked
            if (x_btnPane2TraceOutput.Visibility == Visibility.Visible)
                x_gridLayer1TraceOutput.Visibility = Visibility.Collapsed;

            if (x_btnPane2PrinterOutput.Visibility == Visibility.Visible)
                x_gridLayer2PrinterOutput.Visibility = Visibility.Collapsed;
        }

        // Hide any undocked panes when the mouse enters Layer 0
        public void OnMouseEnterLayer0BackgroundSym3 (object sender, RoutedEventArgs e) // Disassembly Listing
        {
            if (x_btnPane2TraceOutput.Visibility == Visibility.Visible)
                x_gridLayer1TraceOutput.Visibility = Visibility.Collapsed;

            if (x_btnPane2PrinterOutput.Visibility == Visibility.Visible)
                x_gridLayer2PrinterOutput.Visibility = Visibility.Collapsed;

            if (x_btnPane3TreeViewDB.Visibility == Visibility.Visible)
                x_gridLayer3TreeViewDB.Visibility = Visibility.Collapsed;
        }

        // Hide the other pane if undocked when the mouse enters Pane 1
        public void OnMouseEnterLayer1TraceOutput (object sender, RoutedEventArgs e) // Trace Output
        {
            // Ensure the other pane is hidden if it is undocked
            if (x_btnPane2PrinterOutput.Visibility == Visibility.Visible)
                x_gridLayer2PrinterOutput.Visibility = Visibility.Collapsed;

            if (x_btnPane3TreeViewDB.Visibility == Visibility.Visible)
                x_gridLayer3TreeViewDB.Visibility = Visibility.Collapsed;
        }

        // Hide the other pane if undocked when the mouse enters Pane 2
        public void OnMouseEnterLayer2PrinterOutput (object sender, RoutedEventArgs e) // Printer Output
        {
            // Ensure the other pane is hidden if it is undocked
            if (x_btnPane2TraceOutput.Visibility == Visibility.Visible)
                x_gridLayer1TraceOutput.Visibility = Visibility.Collapsed;

            if (x_btnPane3TreeViewDB.Visibility == Visibility.Visible)
                x_gridLayer3TreeViewDB.Visibility = Visibility.Collapsed;
        }

        private void OnMouseEnterLayer3TreeViewDB (object sender, MouseEventArgs e)
        {
            // Ensure the other pane is hidden if it is undocked
            if (x_btnPane2TraceOutput.Visibility == Visibility.Visible)
                x_gridLayer1TraceOutput.Visibility = Visibility.Collapsed;

            if (x_btnPane2PrinterOutput.Visibility == Visibility.Visible)
                x_gridLayer2PrinterOutput.Visibility = Visibility.Collapsed;
        }

        // Docks a pane, which hides the corresponding pane button
        public void DockPane (int paneNumber)
        {
            if (paneNumber == 1)
            {
                x_btnPane2TraceOutput.Visibility = Visibility.Collapsed;
                x_imgPane1TraceOutputPinImage.Source = new BitmapImage (new Uri ("pin.gif", UriKind.Relative));

                // Add the cloned column to layer 0:
                x_gridLayer0BackgroundSym3.ColumnDefinitions.Add (coldefColumn1CloneForLayer0);

                // Add the cloned column to layer 1, but only if pane 2 is docked:
                if (x_btnPane2PrinterOutput.Visibility == Visibility.Collapsed)
                    x_gridLayer1TraceOutput.ColumnDefinitions.Add (coldefColumn2CloneForLayer1);
            }
            else if (paneNumber == 2)
            {
                x_btnPane2PrinterOutput.Visibility = Visibility.Collapsed;
                x_imgPane2PrinterOutputPinImage.Source = new BitmapImage (new Uri ("pin.gif", UriKind.Relative));

                // Add the cloned column to layer 0:
                x_gridLayer0BackgroundSym3.ColumnDefinitions.Add (coldefColumn2CloneForLayer0);

                // Add the cloned column to layer 1, but only if pane 1 is docked:
                if (x_btnPane2TraceOutput.Visibility == Visibility.Collapsed)
                    x_gridLayer1TraceOutput.ColumnDefinitions.Add (coldefColumn2CloneForLayer1);
            }
        }

        // Undocks a pane, which reveals the corresponding pane button
        public void UndockPane (int paneNumber)
        {
            if (paneNumber == 1)
            {
                x_gridLayer1TraceOutput.Visibility = Visibility.Collapsed;
                x_btnPane2TraceOutput.Visibility = Visibility.Visible;
                x_imgPane1TraceOutputPinImage.Source = new BitmapImage (new Uri ("pinHorizontal.gif", UriKind.Relative));

                // Remove the cloned columns from layers 0 and 1:
                x_gridLayer0BackgroundSym3.ColumnDefinitions.Remove (coldefColumn1CloneForLayer0);

                // This won't always be present, but Remove silently ignores bad columns:
                x_gridLayer1TraceOutput.ColumnDefinitions.Remove (coldefColumn2CloneForLayer1);
            }
            else if (paneNumber == 2)
            {
                x_gridLayer2PrinterOutput.Visibility = Visibility.Collapsed;
                x_btnPane2PrinterOutput.Visibility = Visibility.Visible;
                x_imgPane2PrinterOutputPinImage.Source = new BitmapImage (new Uri ("pinHorizontal.gif", UriKind.Relative));

                // Remove the cloned columns from layers 0 and 1:
                x_gridLayer0BackgroundSym3.ColumnDefinitions.Remove (coldefColumn2CloneForLayer0);

                // This won't always be present, but Remove silently ignores bad columns:
                x_gridLayer1TraceOutput.ColumnDefinitions.Remove (coldefColumn2CloneForLayer1);
            }
        }

        private void OnDatabaseTreeViewGridSizeChanged (object sender, SizeChangedEventArgs e)
        {
            Size sz = e.NewSize;
            
            x_tvDataBaseView.Height = sz.Height - 38; // Horizontal scrollbar height: how to get these numbers from WPF?
            //x_tvDataBaseView.Width  = sz.Width  - 16; // Vertical scrollbar width: how to get these numbers from WPF?
        }
        #endregion

        private void OnTreeViewLoaded (object sender, RoutedEventArgs e)
        {
            //bool b = File.Exists (@"..\Databases\Symulator3.accdb");
            //b = File.Exists (@"..\..\Databases\Symulator3.accdb");
            //b = File.Exists (@"..\..\..\Databases\Symulator3.accdb");
            //b = File.Exists (@"..\..\..\..\Databases\Symulator3.accdb");
            //b = File.Exists (@"..\..\..\..\..\Databases\Symulator3.accdb");
            //b = File.Exists (@"..\..\..\..\..\..\Databases\Symulator3.accdb");
            //SortedDictionary<string, CDBFileToken> sdTokens = m_objEmulatorEngine.ReadFileTokens (@"D:\SoftwareDev\SacredCat\IBMSystem3\Databases\Symulator3.accdb");
            //SortedDictionary<string, CDBFileToken> sdTokens = m_objEmulatorEngine.ReadFileTokens (@"..\..\..\..\..\Databases\Symulator3.accdb");
            SortedDictionary<string, CDBFileToken> sdTokens = m_objEmulatorEngine.ReadFileTokens (CEmulatorEngine.GetDBPath ());
            string strLastTableName = "";
            TreeViewItem tviTableName = new TreeViewItem ();
            TreeView tvDataBase = sender as TreeView;

            foreach (KeyValuePair<string, CDBFileToken> kvp in sdTokens)
            {
                if (kvp.Value.DataName.Length  > 0 &&
                    kvp.Value.TableName.Length > 0)
                {
                    if (kvp.Value.TableName == "ScriptingMacros" ||
                        kvp.Value.TableName == "SavedScripts")
                    {
                        continue;
                    }

                    //if (kvp.Value.DataName == "PowerOfTwo(NewBuild)_1" ||
                    //    kvp.Value.DataName == "PowerOfTwo(NewBuild)_2" ||
                    //    kvp.Value.DataName == "TrigTables(NewBuild)_1" ||
                    //    kvp.Value.DataName == "TrigTables(NewBuild)_2")
                    if (kvp.Value.DataName.Contains ("(NewBuild)_"))
                    {
                        continue;
                    }

                    if (strLastTableName != kvp.Value.TableName)
                    {
                        if (strLastTableName.Length > 0)
                        {
                            tvDataBase.Items.Add (tviTableName);
                            tviTableName = new TreeViewItem ();
                        }
                        strLastTableName = kvp.Value.TableName;
                        tviTableName.Header = kvp.Value.TableName;
                    }

                    TreeViewItem tviDataName = new TreeViewItem ();
                    tviDataName.Header = kvp.Value.DataName;
                    tviDataName.Name = kvp.Value.FileTokenKey;
                    tviTableName.Items.Add (tviDataName);
                }
            }

            tvDataBase.Items.Add (tviTableName);
        }

        //private void OnTreeViewSelectedItemChanged (object sender, RoutedPropertyChangedEventArgs<object> e)
        //{
        //    TreeView tvDataBase = sender as TreeView;

        //    // ... Determine type of SelectedItem.
        //    if (tvDataBase.SelectedItem is TreeViewItem)
        //    {
        //        // ... Handle a TreeViewItem.
        //        TreeViewItem tvi = tvDataBase.SelectedItem as TreeViewItem;
        //        //this.Title = "Selected header: " + tvi.Header.ToString () + "  " + tvi.Name.ToString ();
        //    }
        //    else if (tvDataBase.SelectedItem is string)
        //    {
        //        // ... Handle a string.
        //        //this.Title = "Selected: " + tvDataBase.SelectedItem.ToString ();
        //    }
        //}

        private void OnTreeViewDoubleClick (object sender, MouseButtonEventArgs e)
        {
            x_gridLayer3TreeViewDB.Visibility = Visibility.Collapsed; 

            TreeView tvDataBase = sender as TreeView;

            // ... Determine type of SelectedItem.
            if (tvDataBase.SelectedItem is TreeViewItem)
            {
                // ... Handle a TreeViewItem.
                TreeViewItem tvi = tvDataBase.SelectedItem as TreeViewItem;
                m_strItemName  = tvi.Header.ToString ();
                m_strTokenName = tvi.Name; //.DataContext.ToString ();
                string strParentItemName = tvi.Parent.ToString (); // "System.Windows.Controls.TreeViewItem Header:CardObjectIPL Items.Count:101"

                int iStartIdx = strParentItemName.IndexOf (" Header:") + 8;
                int iEndIdx   = strParentItemName.IndexOf (" Items");
                if (iStartIdx > 0 &&
                    iEndIdx   > 0 &&
                    iEndIdx   > iStartIdx)
                {
                    m_strTableName = strParentItemName.Substring (iStartIdx, iEndIdx - iStartIdx);
                }
                m_strTokenName = tvi.Name.ToString ();
            }
            else if (tvDataBase.SelectedItem is string)
            {
                // ... Handle a string.
                string strSelected = "Selected: " + tvDataBase.SelectedItem.ToString ();
            }

            x_lblProgramName.Content = m_strItemName;
            int iRecordCount = 0;

            if (m_strTableName == "CardData") // Print to PrinterOutput
            {
                OutputListLine ("  Table: " + m_strTableName + "  File: " + m_strItemName + "  Token: " + m_strTokenName);
                m_strPrinterOutputFilename = MakeNewPrinterOutputFilename (m_strItemName);
                List<string> lstrCardData = m_objEmulatorEngine.ReadCardFileToStringList (EDatabaseTable.TABLE_CardData, "", m_strItemName);
                iRecordCount = lstrCardData.Count;
                OutputListLines (lstrCardData, true);
            }
            else if (m_strTableName == "CardRPGiiSource") // Print to PrinterOutput
            {
                OutputListLine ("  Table: " + m_strTableName + "  File: " + m_strItemName + "  Token: " + m_strTokenName);
                m_strPrinterOutputFilename = MakeNewPrinterOutputFilename (m_strItemName);
                List<string> lstrCardRPGiiSource = m_objEmulatorEngine.ReadCardFileToStringList (EDatabaseTable.TABLE_CardRPGiiSource, "", m_strItemName);
                iRecordCount = lstrCardRPGiiSource.Count;
                OutputListLines (lstrCardRPGiiSource, true);
            }
            else if (m_strTableName == "ScriptingMacros") // Print to PrinterOutput
            {
                OutputListLine ("  Table: " + m_strTableName + "  File: " + m_strItemName + "  Token: " + m_strTokenName);
                m_strPrinterOutputFilename = MakeNewPrinterOutputFilename (m_strItemName);
                List<string> lstrScriptingMacros = m_objEmulatorEngine.ReadScriptDataToStringList (EDatabaseTable.TABLE_ScriptingMacros, "", m_strItemName);
                iRecordCount = lstrScriptingMacros.Count;
                OutputListLines (lstrScriptingMacros, true);
            }
            else if (m_strTableName == "SavedScripts") // Print to PrinterOutput
            {
                OutputListLine ("  Table: " + m_strTableName + "  File: " + m_strItemName + "  Token: " + m_strTokenName);
                m_strPrinterOutputFilename = MakeNewPrinterOutputFilename (m_strItemName);
                List<string> lstrSavedScripts = m_objEmulatorEngine.ReadScriptDataToStringList (EDatabaseTable.TABLE_SavedScripts, "", m_strItemName);
                iRecordCount = lstrSavedScripts.Count;
                OutputListLines (lstrSavedScripts, true);
            }
            else if (m_strTableName == "CardObjectIPL") // Load & DASM
            {
                //DoDisassembleIPLCards (true);
                OutputListLine ("  Table: " + m_strTableName + "  File: " + m_strItemName + "  Token: " + m_strTokenName);
                m_strDasmOutputFilename = MakeNewDasmOutputFilename (m_strItemName);
                m_strPrinterOutputFilename = MakeNewPrinterOutputFilename (m_strItemName);

                m_objEmulatorEngine.ResetTestOneCardClockProgram ();
                m_objEmulatorEngine.ResetPrintNonAscii ();

                if (m_strItemName.Contains ("CLOCK"))
                {
                    m_objEmulatorEngine.m_objEmulatorState.SetRunState (CEmulatorState.ERunMode.RUN_FreeRun);
                    m_objEmulatorEngine.SetTestOneCardClockProgram ();
                    ShowSystem3ClockSpeed ();
                }
                else
                {
                    m_objEmulatorEngine.SetTrace ();
                    m_objEmulatorEngine.m_objEmulatorState.SetRunState (CEmulatorState.ERunMode.RUN_SingleStep);
                    m_objEmulatorEngine.m_objTraceQueue.UpdateOnly ();
                }

                //if (m_strItemName == "RIPPLE PRINT PROGRAM")
                //{
                //    m_objEmulatorEngine.SetPrintNonAscii ();
                //}

                iRecordCount = m_objEmulatorEngine.ProgramLoadDB (m_strTokenName, "", false, false);
                m_objEmulatorEngine.SetIPLCardCount (iRecordCount);
                if (iRecordCount > 1)
                {
                    m_objEmulatorEngine.SetUIRunMode (CEmulatorEngine.EUIRunMode.RUN_LoadFromIPL);
                }

                InitializeDisASMPanel ();
                m_strTraceOutputFilename = MakeNewTraceOutputFilename (m_strItemName);

                if (m_objEmulatorEngine.IsAbsoluteCardLoader ())
                {
                    // Special case: no DASM until all cards loaded
                    m_objEmulatorEngine.m_objEmulatorState.SetRunState (CEmulatorState.ERunMode.RUN_FreeRun);
                    m_objEmulatorEngine.ResetTrace ();
                    m_objEmulatorEngine.DoRun (0x0000, true);
                    return;
                }

                m_objEmulatorEngine.SetHeaderProgrmaName (PrepProgramName (m_strItemName, false));
                m_lstrDisASM = m_objEmulatorEngine.DisassembleCodeImage ();
                m_objEmulatorEngine.FireNewDASMStringListEvent (m_lstrDisASM);

                x_lblProgramState.Content = m_objEmulatorEngine.m_objEmulatorState.GetProgramStateString ();
                SetLineAddresses ();

                m_objEmulatorEngine.DoRun ();
            }
            else if (m_strTableName == "CardObjectText") // Load
            {
                //DoDisassembleTextCards ();
                OutputListLine ("  Table: " + m_strTableName + "  File: " + m_strItemName + "  Token: " + m_strTokenName);
                m_strDasmOutputFilename = MakeNewDasmOutputFilename (m_strItemName);
                m_strPrinterOutputFilename = MakeNewPrinterOutputFilename (m_strItemName);

                if (m_strItemName == "BLCK" || // 45  EC entry: 0x0000  (0x0300)      Run   Block-Letter Printer                 Primary hopper    CardData:"IBM Set"
                    m_strItemName == "HXPL" || // 23  EC entry: 0x0000  (0x0600)      Run   Hexadecimal to IPL-Punch Program     Primary hopper    CardData:"HXPL Test Input"
                    m_strItemName == "KBPL" || // 31  EC entry: 0x0600                DASM  5475 Hex-to-IPL Punch                No input
                    m_strItemName == "KBST" || // 30  EC entry: 0x0000  Relocatable?  Run   Keyboard-to-Storage                  No input
                    m_strItemName == "KYBD" || // 16  EC entry: 0x0A38                DASM  5475 Function Test                   No input
                    m_strItemName == "PLLT" || // 14  EC entry: 0x0000  (0x0300)      Run   Card IPL-to-Hexadecimal List         Primary hopper    CardData:"IPLCards"
                    m_strItemName == "PLTX" || // 16  EC entry: 0x0000                Run   Card IPL-to-Text Conversion Program  Primary hopper    CardData:"IPLCards"
                    m_strItemName == "PRNT" || // 11  EC entry: 0x0000  (0x0300)      Run   (Card Punch-Pattern Print Program?)  Secondary hopper  CardData:"IPL PRNT Test Secondary"
                                               //                                     Run                                                                   "PunchPatternCards(small)"
                    m_strItemName == "TXLT" || // 26  EC entry: 0x0237                DASM  Text-to-List Program                 Primary hopper    CardData:"TextCards"
                    m_strItemName == "TYPE")   // 14  EC entry: 0x0000  (0x0400)      Run   Typewriter Program                   No input
                {
                    DoDisassembleTextCards ();
                    //    //// Load hopper(s)
                    //    //if (m_strItemName == "BLCK")      // Primary hopper    CardData:"IBM Set"
                    //    //{
                    //    //    m_objEmulatorEngine.AssignTokenToPrimaryHopper ("DBCrdDataIBMSet");
                    //    //}
                    //    //else if (m_strItemName == "HXPL") // Primary hopper    CardData:"HXPL Test Input"
                    //    //{
                    //    //    m_objEmulatorEngine.AssignTokenToPrimaryHopper ("DBCrdDataHXPLTestInput");
                    //    //}
                    //    //else if (m_strItemName == "PLLT") // Primary hopper    CardData:"IPLCards"
                    //    //{
                    //    //    m_objEmulatorEngine.AssignTokenToPrimaryHopper ("DBCrdDataIPLCards");
                    //    //}
                    //    //else if (m_strItemName == "PLTX") // Primary hopper    CardData:"IPLCards"
                    //    //{
                    //    //    m_objEmulatorEngine.AssignTokenToPrimaryHopper ("DBCrdDataIPLCards");
                    //    //}
                    //    //else if (m_strItemName == "PRNT") // Secondary hopper  CardData:"IPL PRNT Test Secondary" "PunchPatternCards(small)"
                    //    //{
                    //    //    m_objEmulatorEngine.AssignTokenToPrimaryHopper ("DBCrdDataPunchPatternCardsSmall");
                    //    //}
                    //    //else if (m_strItemName == "TXLT") // Primary hopper    CardData:"TextCards"
                    //    //{
                    //    //    m_objEmulatorEngine.AssignTokenToPrimaryHopper ("DBCrdDataTextCards");
                    //    //}

                    //    //// DASM
                    //    //InitializeDisASMPanel ();

                    //    //// Just load and stop
                    //    //m_objEmulatorEngine.m_objEmulatorState.SetRunState (CEmulatorState.ERunMode.RUN_SingleStep);

                    //    //int iStartAddress = 0x0000;
                    //    //int iEndAddress   = 0x0000;
                    //    //int iEntryPoint   = 0x0000;
                    //    //byte[] lyEndCardImage = null;
                    //    //iRecordCount = m_objEmulatorEngine.ProgramLoadText (m_strTokenName, ref iStartAddress, ref iEndAddress, ref iEntryPoint, ref lyEndCardImage);
                    //    //m_objEmulatorEngine.SetUIRunMode (CEmulatorEngine.EUIRunMode.RUN_LoadFromText);

                    //    //m_objEmulatorEngine.SetHeaderProgrmaName (PrepProgramName (m_strItemName, false));
                    //    //List<string> lstrEndCardDASM = new List<string> (m_objEmulatorEngine.DisassembleCodeText (true));

                    //    //m_objEmulatorEngine.FireNewDASMStringListEvent (lstrEndCardDASM);

                    //    //x_lblProgramState.Content = m_objEmulatorEngine.m_objEmulatorState.GetProgramStateString ();
                    //    //SetLineAddresses ();

                    //    //m_strTraceOutputFilename = MakeNewTraceOutputFilename (m_strItemName);
                    //    //m_objEmulatorEngine.DoRun (0x0019, true);
                    }
                    else if (m_strItemName.Contains ("PowerOfTwo") || // 126
                             m_strItemName.Contains ("TrigTables"))   // 168
                {
                    m_objEmulatorEngine.m_objPrintQueue.Clear ();
                    m_objEmulatorEngine.FireClearPrinterPanelEvent ();

                    m_objEmulatorEngine.m_objEmulatorState.SetRunState (CEmulatorState.ERunMode.RUN_FreeRun);
                    iRecordCount = m_objEmulatorEngine.ProgramLoadDB (m_strTokenName);
                }
                else
                {
                    //OutputListLines (m_objEmulatorEngine.ReadCardFileToStringList (EDatabaseTable.TABLE_CardObjectText, "", m_strItemName));
                    //  BLCK 45
                    //  HeaderDump 44
                    //  IPLCardDump 45
                }
            }
            else if (m_strTableName == "DiskCreatedImages") // Load only
            {
                OutputListLine ("  Table: " + m_strTableName + "  File: " + m_strItemName + "  Token: " + m_strTokenName);
            }
            else if (m_strTableName == "DiskOriginalImages") // Load only
            {
                OutputListLine ("  Table: " + m_strTableName + "  File: " + m_strItemName + "  Token: " + m_strTokenName);
            }

            x_lblProgramCardCount.Content = iRecordCount.ToString ();
        }

        // Select TreeView Node on right click before displaying ContextMenu
        // https://stackoverflow.com/questions/592373/select-treeview-node-on-right-click-before-displaying-contextmenu
        private void OnPreviewMouseRightButtonDown (object sender, MouseButtonEventArgs e)
        {
            TreeViewItem tviSelected = VisualUpwardSearch (e.OriginalSource as DependencyObject);

            if (tviSelected != null)
            {
                tviSelected.Focus ();
                e.Handled = true;
            }
        }

        // Select TreeView Node on right click before displaying ContextMenu
        // https://stackoverflow.com/questions/592373/select-treeview-node-on-right-click-before-displaying-contextmenu
        static TreeViewItem VisualUpwardSearch (DependencyObject doSource)
        {
            while (doSource != null &&
                   !(doSource is TreeViewItem))
            {
                doSource = VisualTreeHelper.GetParent (doSource);
            }

            return doSource as TreeViewItem;
        }

        private void OnTreeViewMouseRightButtonDown (object sender, MouseButtonEventArgs e)
        {
            TreeView tvDataBase = sender as TreeView;

            // ... Determine type of SelectedItem.
            if (tvDataBase.SelectedItem is TreeViewItem)
            {
                // ... Handle a TreeViewItem.
                TreeViewItem tvi = tvDataBase.SelectedItem as TreeViewItem;
                m_strItemName = tvi.Header.ToString ();
                string strParentItemName = tvi.Parent.ToString (); // "System.Windows.Controls.TreeViewItem Header:CardObjectIPL Items.Count:101"
                int iStartIdx = strParentItemName.IndexOf (" Header:") + 8;
                int iEndIdx   = strParentItemName.IndexOf (" Items");
                if (iStartIdx > 0 &&
                    iEndIdx   > 0 &&
                    iEndIdx   > iStartIdx)
                {
                    m_strTableName = strParentItemName.Substring (iStartIdx, iEndIdx - iStartIdx);
                }
                m_strTokenName = tvi.Name.ToString ();
            }
            else if (tvDataBase.SelectedItem is string)
            {
                // ... Handle a string.
                string strSelected = "Selected: " + tvDataBase.SelectedItem.ToString ();
            }
        }

        private void OnTreeViewMouseRightButtonUp (object sender, MouseButtonEventArgs e)
        {
            TreeView tvDataBase = sender as TreeView;

            // ... Determine type of SelectedItem.
            if (tvDataBase.SelectedItem is TreeViewItem)
            {
                // ... Handle a TreeViewItem.
                TreeViewItem tvi = tvDataBase.SelectedItem as TreeViewItem;
                m_strItemName = tvi.Header.ToString ();
                string strParentItemName = tvi.Parent.ToString (); // "System.Windows.Controls.TreeViewItem Header:CardObjectIPL Items.Count:101"
                int iStartIdx = strParentItemName.IndexOf (" Header:") + 8;
                int iEndIdx   = strParentItemName.IndexOf (" Items");
                if (iStartIdx > 0 &&
                    iEndIdx   > 0 &&
                    iEndIdx   > iStartIdx)
                {
                    m_strTableName = strParentItemName.Substring (iStartIdx, iEndIdx - iStartIdx);
                }
                m_strTokenName = tvi.Name.ToString ();
            }
            else if (tvDataBase.SelectedItem is string)
            {
                // ... Handle a string.
                string strSelected = "Selected: " + tvDataBase.SelectedItem.ToString ();
            }

            if (m_strItemName == "CardData"          ||
                m_strItemName == "CardRPGiiSource"   ||
                m_strItemName == "ScriptingMacros"   ||
                m_strItemName == "SavedScripts"      ||
                m_strItemName == "CardObjectIPL"     ||
                m_strItemName == "CardObjectText"    ||
                m_strItemName == "DiskCreatedImages" ||
                m_strItemName == "DiskOriginalImages")
            {
                // Suppress context menu when clicking on nodes
                e.Handled = true;
                return;
            }

            x_menuOutputToPrinter.Visibility              = Visibility.Collapsed;
            x_menuShowCardPunchPattern.Visibility         = Visibility.Collapsed;
            x_menuProgramSeparator.Visibility             = Visibility.Collapsed;
            //x_menuLoadProgramSingleStep.Visibility        = Visibility.Collapsed;
            x_menuLoadProgramHexDump.Visibility           = Visibility.Collapsed;
            x_menuLoadProgramDisassemble.Visibility       = Visibility.Collapsed;
            x_menuIPLLoadProgramFreeRun.Visibility        = Visibility.Collapsed;
            x_menuIPLLoadProgramBreak.Visibility          = Visibility.Collapsed;
            x_menuShowTextCardsAsHex.Visibility           = Visibility.Collapsed;
            x_menuBootFromDiskImage.Visibility            = Visibility.Collapsed;
            x_menuScriptsSeparator.Visibility             = Visibility.Collapsed;
            x_menuEditScript.Visibility                   = Visibility.Collapsed;
            x_menuRunScript.Visibility                    = Visibility.Collapsed;
            x_menuLoadHopperSeparator.Visibility          = Visibility.Collapsed;
            x_menuClearPrimaryHopper.Visibility           = Visibility.Collapsed;
            x_menuClearSecondaryHopper.Visibility         = Visibility.Collapsed;
            x_menuClearHoppers.Visibility                 = Visibility.Collapsed;
            x_menuLoadInPrimaryHopper.Visibility          = Visibility.Collapsed;
            x_menuLoadInPrimaryHopperReplace.Visibility   = Visibility.Collapsed;
            x_menuLoadInPrimaryHopperAppend.Visibility    = Visibility.Collapsed;
            x_menuLoadInSecondaryHopper.Visibility        = Visibility.Collapsed;
            x_menuLoadInSecondaryHopperReplace.Visibility = Visibility.Collapsed;
            x_menuLoadInSecondaryHopperAppend.Visibility  = Visibility.Collapsed;

            if (m_strTableName == "CardData") // Print to PrinterOutput
            {
                x_menuOutputToPrinter.Visibility              = Visibility.Visible;
                x_menuShowCardPunchPattern.Visibility         = Visibility.Visible;
                x_menuLoadHopperSeparator.Visibility          = Visibility.Visible;
                x_menuClearPrimaryHopper.Visibility           = m_objEmulatorEngine.IsPrimaryHopperEmpty () ? Visibility.Collapsed : Visibility.Visible;
                x_menuClearSecondaryHopper.Visibility         = m_objEmulatorEngine.IsSecondaryHopperEmpty () ? Visibility.Collapsed : Visibility.Visible;
                if (!m_objEmulatorEngine.IsPrimaryHopperEmpty () ||
                    !m_objEmulatorEngine.IsSecondaryHopperEmpty ())
                {
                    x_menuClearHoppers.Visibility             = Visibility.Visible;
                }
                x_menuLoadInPrimaryHopper.Visibility          = m_objEmulatorEngine.IsPrimaryHopperEmpty () ? Visibility.Visible : Visibility.Collapsed;
                x_menuLoadInPrimaryHopperReplace.Visibility   = Visibility.Visible;
                x_menuLoadInPrimaryHopperAppend.Visibility    = Visibility.Visible;
                x_menuLoadInSecondaryHopper.Visibility        = m_objEmulatorEngine.IsSecondaryHopperEmpty () ? Visibility.Visible : Visibility.Collapsed;
                x_menuLoadInSecondaryHopperReplace.Visibility = Visibility.Visible;
                x_menuLoadInSecondaryHopperAppend.Visibility  = Visibility.Visible;
            }
            else if (m_strTableName == "CardRPGiiSource") // Print to PrinterOutput
            {
                x_menuOutputToPrinter.Visibility              = Visibility.Visible;
                x_menuLoadHopperSeparator.Visibility          = Visibility.Visible;
                x_menuClearPrimaryHopper.Visibility           = m_objEmulatorEngine.IsPrimaryHopperEmpty () ? Visibility.Collapsed : Visibility.Visible;
                x_menuClearSecondaryHopper.Visibility         = m_objEmulatorEngine.IsSecondaryHopperEmpty () ? Visibility.Collapsed : Visibility.Visible;
                if (!m_objEmulatorEngine.IsPrimaryHopperEmpty () ||
                    !m_objEmulatorEngine.IsSecondaryHopperEmpty ())
                {
                    x_menuClearHoppers.Visibility             = Visibility.Visible;
                }
                x_menuLoadInPrimaryHopper.Visibility          = m_objEmulatorEngine.IsPrimaryHopperEmpty () ? Visibility.Visible : Visibility.Collapsed;
                x_menuLoadInPrimaryHopperReplace.Visibility   = Visibility.Visible;
                x_menuLoadInPrimaryHopperAppend.Visibility    = Visibility.Visible;
                x_menuLoadInSecondaryHopper.Visibility        = m_objEmulatorEngine.IsSecondaryHopperEmpty () ? Visibility.Visible : Visibility.Collapsed;
                x_menuLoadInSecondaryHopperReplace.Visibility = Visibility.Visible;
                x_menuLoadInSecondaryHopperAppend.Visibility  = Visibility.Visible;
            }
            else if (m_strTableName == "ScriptingMacros") // Print to PrinterOutput
            {
                x_menuOutputToPrinter.Visibility        = Visibility.Visible;
                x_menuScriptsSeparator.Visibility       = Visibility.Visible;
                x_menuEditScript.Visibility             = Visibility.Visible;
                x_menuRunScript.Visibility              = Visibility.Visible;
            }
            else if (m_strTableName == "SavedScripts") // Print to PrinterOutput
            {
                x_menuOutputToPrinter.Visibility        = Visibility.Visible;
                x_menuScriptsSeparator.Visibility       = Visibility.Visible;
                x_menuEditScript.Visibility             = Visibility.Visible;
                x_menuRunScript.Visibility              = Visibility.Visible;
            }
            else if (m_strTableName == "CardObjectIPL") // Load & DASM
            {
                x_menuOutputToPrinter.Visibility              = Visibility.Visible;
                x_menuProgramSeparator.Visibility             = Visibility.Visible;
                //x_menuLoadProgramSingleStep.Visibility        = Visibility.Visible;
                x_menuLoadProgramHexDump.Visibility           = Visibility.Visible;
                x_menuLoadProgramDisassemble.Visibility       = Visibility.Visible;
                x_menuIPLLoadProgramFreeRun.Visibility        = Visibility.Visible;
                x_menuIPLLoadProgramBreak.Visibility          = Visibility.Visible;
                x_menuLoadHopperSeparator.Visibility          = Visibility.Visible;
                x_menuClearPrimaryHopper.Visibility           = m_objEmulatorEngine.IsPrimaryHopperEmpty () ? Visibility.Collapsed : Visibility.Visible;
                x_menuClearSecondaryHopper.Visibility         = m_objEmulatorEngine.IsSecondaryHopperEmpty () ? Visibility.Collapsed : Visibility.Visible;
                if (!m_objEmulatorEngine.IsPrimaryHopperEmpty () ||
                    !m_objEmulatorEngine.IsSecondaryHopperEmpty ())
                {
                    x_menuClearHoppers.Visibility             = Visibility.Visible;
                }
                x_menuLoadInPrimaryHopper.Visibility          = m_objEmulatorEngine.IsPrimaryHopperEmpty () ? Visibility.Visible : Visibility.Collapsed;
                x_menuLoadInPrimaryHopperReplace.Visibility   = Visibility.Visible;
                x_menuLoadInPrimaryHopperAppend.Visibility    = Visibility.Visible;
                x_menuLoadInSecondaryHopper.Visibility        = m_objEmulatorEngine.IsSecondaryHopperEmpty () ? Visibility.Visible : Visibility.Collapsed;
                x_menuLoadInSecondaryHopperReplace.Visibility = Visibility.Visible;
                x_menuLoadInSecondaryHopperAppend.Visibility  = Visibility.Visible;
            }
            else if (m_strTableName == "CardObjectText") // Load & DASM
            {
                x_menuOutputToPrinter.Visibility              = Visibility.Visible;
                x_menuProgramSeparator.Visibility             = Visibility.Visible;
                //x_menuLoadProgramSingleStep.Visibility        = Visibility.Visible;
                x_menuLoadProgramHexDump.Visibility           = Visibility.Visible;
                x_menuLoadProgramDisassemble.Visibility       = Visibility.Visible;
                x_menuIPLLoadProgramFreeRun.Visibility        = Visibility.Visible;
                x_menuIPLLoadProgramBreak.Visibility          = Visibility.Visible;
                x_menuShowTextCardsAsHex.Visibility           = Visibility.Visible;
                x_menuLoadHopperSeparator.Visibility          = Visibility.Visible;
                x_menuClearPrimaryHopper.Visibility           = m_objEmulatorEngine.IsPrimaryHopperEmpty () ? Visibility.Collapsed : Visibility.Visible;
                x_menuClearSecondaryHopper.Visibility         = m_objEmulatorEngine.IsSecondaryHopperEmpty () ? Visibility.Collapsed : Visibility.Visible;
                if (!m_objEmulatorEngine.IsPrimaryHopperEmpty () ||
                    !m_objEmulatorEngine.IsSecondaryHopperEmpty ())
                {
                    x_menuClearHoppers.Visibility             = Visibility.Visible;
                }
                x_menuLoadInPrimaryHopper.Visibility          = m_objEmulatorEngine.IsPrimaryHopperEmpty () ? Visibility.Visible : Visibility.Collapsed;
                x_menuLoadInPrimaryHopperReplace.Visibility   = Visibility.Visible;
                x_menuLoadInPrimaryHopperAppend.Visibility    = Visibility.Visible;
                x_menuLoadInSecondaryHopper.Visibility        = m_objEmulatorEngine.IsSecondaryHopperEmpty () ? Visibility.Visible : Visibility.Collapsed;
                x_menuLoadInSecondaryHopperReplace.Visibility = Visibility.Visible;
                x_menuLoadInSecondaryHopperAppend.Visibility  = Visibility.Visible;
            }
            else if (m_strTableName == "DiskCreatedImages") // Load only
            {
                x_menuBootFromDiskImage.Visibility      = Visibility.Visible;
            }
            else if (m_strTableName == "DiskOriginalImages") // Load only
            {
                x_menuBootFromDiskImage.Visibility      = Visibility.Visible;
            }

            if (x_menuOutputToPrinter.Visibility              == Visibility.Collapsed &&
                x_menuShowCardPunchPattern.Visibility         == Visibility.Collapsed &&
                x_menuProgramSeparator.Visibility             == Visibility.Collapsed &&
                //x_menuLoadProgramSingleStep.Visibility        == Visibility.Collapsed &&
                x_menuLoadProgramHexDump.Visibility           == Visibility.Collapsed &&
                x_menuLoadProgramDisassemble.Visibility       == Visibility.Collapsed &&
                x_menuIPLLoadProgramFreeRun.Visibility        == Visibility.Collapsed &&
                x_menuIPLLoadProgramBreak.Visibility          == Visibility.Collapsed &&
                x_menuShowTextCardsAsHex.Visibility           == Visibility.Collapsed &&
                x_menuBootFromDiskImage.Visibility            == Visibility.Collapsed &&
                x_menuScriptsSeparator.Visibility             == Visibility.Collapsed &&
                x_menuEditScript.Visibility                   == Visibility.Collapsed &&
                x_menuRunScript.Visibility                    == Visibility.Collapsed &&
                x_menuClearPrimaryHopper.Visibility           == Visibility.Collapsed &&
                x_menuClearSecondaryHopper.Visibility         == Visibility.Collapsed &&
                x_menuClearHoppers.Visibility                 == Visibility.Collapsed &&
                x_menuLoadHopperSeparator.Visibility          == Visibility.Collapsed &&
                x_menuLoadInPrimaryHopper.Visibility          == Visibility.Collapsed &&
                x_menuLoadInPrimaryHopperReplace.Visibility   == Visibility.Collapsed &&
                x_menuLoadInPrimaryHopperAppend.Visibility    == Visibility.Collapsed &&
                x_menuLoadInSecondaryHopper.Visibility        == Visibility.Collapsed &&
                x_menuLoadInSecondaryHopperReplace.Visibility == Visibility.Collapsed &&
                x_menuLoadInSecondaryHopperAppend.Visibility  == Visibility.Collapsed)
            {
                e.Handled = true; // Suppress context menu display
            }
        }

        private void OnOutputToPrinter (object sender, RoutedEventArgs e)
        {
            //Console.WriteLine (m_strTableName + " - " + m_strItemName);
            // Clear PrinterOutput panel
            m_objEmulatorEngine.FireClearPrinterPanelEvent ();
            m_objEmulatorEngine.m_objPrintQueue.Clear ();

            OutputListLine ("  Table: " + m_strTableName + "  File: " + m_strItemName + "  Token: " + m_strTokenName);
            OutputListLine ("", true);
            m_strPrinterOutputFilename = MakeNewPrinterOutputFilename (m_strItemName);

            m_objEmulatorEngine.SetUIRunMode (CEmulatorEngine.EUIRunMode.RUN_OutputToPrinterPanel);
            List<string> lstrCardImages   = m_objEmulatorEngine.ReadListFromToken (m_strTokenName);
            x_lblProgramCardCount.Content = lstrCardImages.Count;
            OutputListLines (lstrCardImages, true);
        }

        private void OnShowCardPunchPattern (object sender, RoutedEventArgs e)
        {
            // Clear PrinterOutput panel
            m_objEmulatorEngine.FireClearPrinterPanelEvent ();
            m_objEmulatorEngine.m_objPrintQueue.Clear ();

            OutputListLine ("  Table: " + m_strTableName + "  File: " + m_strItemName + "  Token: " + m_strTokenName);
            OutputListLine ("", true);
            m_strPrinterOutputFilename = MakeNewPunchPatternOutputFilename (m_strItemName);

            m_objEmulatorEngine.SetUIRunMode (CEmulatorEngine.EUIRunMode.RUN_OutputPunchPattern);
            List<string> lstrCardImages      = m_objEmulatorEngine.ReadCardFileToStringList (EDatabaseTable.TABLE_CardData, "", m_strItemName);
            List<string> lstrPunchImages     = new List<string> ();
            List<string> lstrSingleCardImage = new List<string> ();
            x_lblProgramCardCount.Content    = lstrCardImages.Count;

            foreach (string str1 in lstrCardImages)
            {
                lstrSingleCardImage = m_objEmulatorEngine.CreatePunchImage (str1);

                if (lstrPunchImages.Count > 0)
                {
                    lstrPunchImages.Add ("");
                    lstrPunchImages.Add ("");
                }

                foreach (string str2 in lstrSingleCardImage)
                {
                    lstrPunchImages.Add (str2);
                }
            }

            OutputListLines (lstrPunchImages, true);
        }

        //private void OnLoadProgramSingleStep (object sender, RoutedEventArgs e)
        //{
        //    m_objEmulatorEngine.SetUIRunMode (m_strTableName == "CardObjectIPL"  ? CEmulatorEngine.EUIRunMode.RUN_LoadFromIPL  :
        //                                      m_strTableName == "CardObjectText" ? CEmulatorEngine.EUIRunMode.RUN_LoadFromText : CEmulatorEngine.EUIRunMode.RUN_Undefined);
        //    m_objEmulatorEngine.m_objEmulatorState.SetRunState (CEmulatorState.ERunMode.RUN_SingleStep);
        //    m_strTraceOutputFilename = MakeNewTraceOutputFilename (m_strItemName);

        //    int iRecordCount = m_objEmulatorEngine.ProgramLoadDB (m_strTokenName);
        //    m_objEmulatorEngine.SetIPLCardCount (m_strTableName == "CardObjectIPL"  ? iRecordCount : 0);
        //    x_lblProgramCardCount.Content = iRecordCount.ToString ();
        //    x_lblProgramState.Content = m_objEmulatorEngine.m_objEmulatorState.GetProgramStateString ();
        //}

        private void OnLoadProgramHexDump (object sender, RoutedEventArgs e)
        {
            m_strDasmOutputFilename = MakeNewDumpOutputFilename (m_strItemName);
            m_lstrDisASM = new List<string> ();

            if (m_strTableName == "CardObjectText")
            {
                EDatabaseTable eDatabaseTable = CEmulatorEngine.TableStringToEnum (m_strTableName);
                List<string> lstrCardImages = m_objEmulatorEngine.ReadCardFileToStringList (eDatabaseTable, "", m_strItemName);
                x_lblProgramCardCount.Content = lstrCardImages.Count;

                byte[] yaBinaryImage = m_objEmulatorEngine.LoadTextFile (lstrCardImages);

                string strTitle = string.Format ("Hex Dump of {0} End Card", m_strItemName);
                string strLeadPadding = new string (' ', (102 - strTitle.Length) / 2);
                m_lstrDisASM.Add (strLeadPadding + strTitle);
                m_lstrDisASM.AddRange (m_objEmulatorEngine.BinaryToDumpEndCard (yaBinaryImage));
                m_lstrDisASM.Add ("");
                strTitle = string.Format ("Hex Dump of {0} Main Program", m_strItemName);
                strLeadPadding = new string (' ', (102 - strTitle.Length) / 2);
                m_lstrDisASM.Add (strLeadPadding + strTitle);
                m_lstrDisASM.AddRange (m_objEmulatorEngine.BinaryToDumpMainProgram (yaBinaryImage));
            }
            else if (m_strTableName == "CardObjectIPL")
            {
                EDatabaseTable eDatabaseTable = CEmulatorEngine.TableStringToEnum (m_strTableName);
                List<string> lstrCardImages = m_objEmulatorEngine.ReadCardFileToStringList (eDatabaseTable, "", m_strItemName);
                x_lblProgramCardCount.Content = lstrCardImages.Count;

                // Get card image(s) from database
                List<string> lstrListFromToken = m_objEmulatorEngine.ReadListFromToken (m_strTokenName);
                if (lstrListFromToken.Count == 0)
                {
                    return;
                }

                for (int iIdx = 0; iIdx < lstrListFromToken.Count; ++iIdx)
                {
                    string strTitle = string.Format ("Hex Dump of {0} IPL Card", m_strItemName);
                    if (lstrListFromToken.Count > 1)
                    {
                        strTitle += string.Format (" ({0} / {1})", iIdx + 1, lstrListFromToken.Count);
                    }
                    string strLeadPadding = new string (' ', (102 - strTitle.Length) / 2);
                    if (iIdx > 0)
                    {
                        m_lstrDisASM.Add ("");
                    }
                    m_lstrDisASM.Add (strLeadPadding + strTitle);
                    m_lstrDisASM.AddRange (m_objEmulatorEngine.DumpIplLine (lstrListFromToken[iIdx]));
                }
            }

            InitializeDisASMPanel ();

            m_objEmulatorEngine.FireNewDASMStringListEvent (m_lstrDisASM);
        }

        private void OnLoadProgramDisassemble (object sender, RoutedEventArgs e)
        {
            //m_objEmulatorEngine.SetUIRunMode (m_strTableName == "CardObjectIPL"  ? CEmulatorEngine.EUIRunMode.RUN_LoadFromIPL  :
            //                                  m_strTableName == "CardObjectText" ? CEmulatorEngine.EUIRunMode.RUN_LoadFromText : CEmulatorEngine.EUIRunMode.RUN_Undefined);
            if (m_strTableName == "CardObjectIPL")
            {
                m_objEmulatorEngine.SetUIRunMode (CEmulatorEngine.EUIRunMode.RUN_LoadFromIPL);

                DoDisassembleIPLCards ();
            }
            else if (m_strTableName == "CardObjectText")
            {
                m_objEmulatorEngine.SetUIRunMode (CEmulatorEngine.EUIRunMode.RUN_LoadFromText);

                DoDisassembleTextCards ();
            }
        }

        private void OnIPLLoadProgramFreeRun (object sender, RoutedEventArgs e)
        {
            // Absolute Card Loader:         "BA  *.",*GX3:|A=} 4  "
            // Relocating Card Loader IPL 1: "BA  *.",*GX3:|A=} 4  "
            // Relocating Card Loader IPL 2: "BA  *.",*GX3:|A=} 4  "
            // Relocating Card Loader IPL 3: "BA  *.",*GX3:|A=} 4  "
        }

        private void OnIPLLoadProgramBreak (object sender, RoutedEventArgs e)
        {
        }

        private void OnShowTextCardsAsHex (object sender, RoutedEventArgs e)
        {
            // Clear PrinterOutput panel
            m_objEmulatorEngine.FireClearPrinterPanelEvent ();
            m_objEmulatorEngine.m_objPrintQueue.Clear ();

            m_strPrinterOutputFilename = MakeNewTextOutputFilename (m_strItemName);
            OutputListLine ("  Table: " + m_strTableName + "  File: " + m_strItemName + "  Token: " + m_strTokenName);
            OutputListLine ("", true);

            m_objEmulatorEngine.SetUIRunMode (CEmulatorEngine.EUIRunMode.RUN_LoadFromText);
            List<string> lstrCardImages   = m_objEmulatorEngine.ReadCardFileToStringList (EDatabaseTable.TABLE_CardObjectText, "", m_strItemName);
            List<string> lstrHexLines     = m_objEmulatorEngine.DumpTextFileLines (lstrCardImages, true, true, true);
            x_lblProgramCardCount.Content = lstrCardImages.Count;

            OutputListLines (lstrHexLines, true);
        }
        
        private void OnEditScript (object sender, RoutedEventArgs e)
        {
            m_objEmulatorEngine.SetUIRunMode (CEmulatorEngine.EUIRunMode.RUN_EditScript);
        }

        private void OnRunScript (object sender, RoutedEventArgs e)
        {
            m_objEmulatorEngine.SetUIRunMode (CEmulatorEngine.EUIRunMode.RUN_RunScript);
        }

        public void OnClearPrimaryHopper (object sender, RoutedEventArgs e)
        {
            m_objEmulatorEngine.ClearPrimaryHopper ();
        }

        public void OnClearSecondaryHopper (object sender, RoutedEventArgs e)
        {
            m_objEmulatorEngine.ClearSecondaryHopper ();
        }

        public void OnClearBothHoppers (object sender, RoutedEventArgs e)
        {
            m_objEmulatorEngine.ClearPrimaryHopper ();
            m_objEmulatorEngine.ClearSecondaryHopper ();
        }

        private void OnLoadInPrimaryHopper (object sender, RoutedEventArgs e)
        {
            m_objEmulatorEngine.AssignTokenToPrimaryHopper (m_strTokenName);
        }

        private void OnLoadInPrimaryHopperReplace (object sender, RoutedEventArgs e)
        {
            m_objEmulatorEngine.AssignTokenToPrimaryHopper (m_strTokenName, C5424MFCU.ELoadHopperMode.LOAD_ReplaceFile);
        }

        private void OnLoadInPrimaryHopperAppend (object sender, RoutedEventArgs e)
        {
            m_objEmulatorEngine.AssignTokenToPrimaryHopper (m_strTokenName, C5424MFCU.ELoadHopperMode.LOAD_AppendFile);
        }

        private void OnLoadInSecondaryHopper (object sender, RoutedEventArgs e)
        {
            m_objEmulatorEngine.AssignTokenToSecondaryHopper (m_strTokenName);
        }

        private void OnLoadInSecondaryHopperReplace (object sender, RoutedEventArgs e)
        {
            m_objEmulatorEngine.AssignTokenToSecondaryHopper (m_strTokenName, C5424MFCU.ELoadHopperMode.LOAD_ReplaceFile);
        }

        private void OnLoadInSecondaryHopperAppend (object sender, RoutedEventArgs e)
        {
            m_objEmulatorEngine.AssignTokenToSecondaryHopper (m_strTokenName, C5424MFCU.ELoadHopperMode.LOAD_AppendFile);
        }

        private void OnBootFromDiskImage (object sender, RoutedEventArgs e)
        {
            m_objEmulatorEngine.SetUIRunMode (CEmulatorEngine.EUIRunMode.RUN_LoadFromDiskImage);
        }

        private void DoDisassembleIPLCards (bool bRunIfClock = false)
        {
            OutputListLine ("  Table: " + m_strTableName + "  File: " + m_strItemName + "  Token: " + m_strTokenName);
            m_strDasmOutputFilename    = MakeNewDasmOutputFilename (m_strItemName);
            m_strPrinterOutputFilename = MakeNewPrinterOutputFilename (m_strItemName);

            m_objEmulatorEngine.ResetTestOneCardClockProgram ();
            m_objEmulatorEngine.ResetPrintNonAscii ();
            //m_objEmulatorEngine.m_objEmulatorState.ChangeState (CEmulatorState.EProgramState.PSTATE_2_ProgramLoaded);

            if (bRunIfClock &&
                m_strItemName.Contains ("CLOCK"))
            {
                m_objEmulatorEngine.m_objEmulatorState.SetRunState (CEmulatorState.ERunMode.RUN_FreeRun);
                m_objEmulatorEngine.SetTestOneCardClockProgram ();
                ShowSystem3ClockSpeed ();
            }
            else
            {
                m_objEmulatorEngine.SetTrace ();
                m_objEmulatorEngine.m_objEmulatorState.SetRunState (CEmulatorState.ERunMode.RUN_SingleStep);
                m_objEmulatorEngine.m_objTraceQueue.UpdateOnly ();
            }

            //if (m_strItemName == "RIPPLE PRINT PROGRAM")
            //{
            //    m_objEmulatorEngine.SetPrintNonAscii ();
            //}

            int iRecordCount = m_objEmulatorEngine.ProgramLoadDB (m_strTokenName, "", false, false);
            m_objEmulatorEngine.SetIPLCardCount (iRecordCount);
            if (iRecordCount > 1)
            {
                m_objEmulatorEngine.SetUIRunMode (CEmulatorEngine.EUIRunMode.RUN_LoadFromIPL);
            }

            InitializeDisASMPanel ();
            m_strTraceOutputFilename = MakeNewTraceOutputFilename (m_strItemName);

            if (m_objEmulatorEngine.IsAbsoluteCardLoader ())
            {
                // Special case: no DASM until all cards loaded
                m_objEmulatorEngine.m_objEmulatorState.SetRunState (CEmulatorState.ERunMode.RUN_FreeRun);
                m_objEmulatorEngine.ResetTrace ();
                m_objEmulatorEngine.DoRun (0x0000, true);
                return;
            }

            m_objEmulatorEngine.SetHeaderProgrmaName (PrepProgramName (m_strItemName, false));
            m_lstrDisASM = m_objEmulatorEngine.DisassembleCodeImage ();
            m_objEmulatorEngine.FireNewDASMStringListEvent (m_lstrDisASM);

            x_lblProgramCardCount.Content = iRecordCount.ToString ();
            x_lblProgramState.Content = m_objEmulatorEngine.m_objEmulatorState.GetProgramStateString ();
            SetLineAddresses ();

            m_objEmulatorEngine.DoRun ();
        }

        private void DoDisassembleTextCards ()
        {
            // Load hopper(s)
            if (m_strItemName == "BLCK")      // Primary hopper    CardData:"IBM Set"
            {
                m_objEmulatorEngine.AssignTokenToPrimaryHopper ("DBCrdDataIBMSet");
            }
            else if (m_strItemName == "HXPL") // Primary hopper    CardData:"HXPL Test Input"
            {
                m_objEmulatorEngine.AssignTokenToPrimaryHopper ("DBCrdDataHXPLTestInput");
            }
            else if (m_strItemName == "KBPL") // Primary hopper    Blank card
            {
                m_objEmulatorEngine.LoadPrimaryHopperBlankCard ();
            }
            else if (m_strItemName == "PLLT") // Primary hopper    CardData:"IPLCards"
            {
                m_objEmulatorEngine.AssignTokenToSecondaryHopper ("DBCrdDataIPLCards");
            }
            else if (m_strItemName == "PLTX") // Primary hopper    CardData:"IPLCards"
            {
                m_objEmulatorEngine.AssignTokenToPrimaryHopper ("DBCrdDataIPLCards");
            }
            else if (m_strItemName == "PRNT") // Secondary hopper  CardData:"IPL PRNT Test Secondary" "PunchPatternCards(small)"
            {
                m_objEmulatorEngine.AssignTokenToSecondaryHopper ("DBCrdDataPunchPatternCardsSmall");
            }
            else if (m_strItemName == "TXLT") // Primary hopper    CardData:"TextCards"
            {
                m_objEmulatorEngine.AssignTokenToSecondaryHopper ("DBCrdDataTextCards");
            }

            // DASM
            InitializeDisASMPanel ();

            int iStartAddress = 0x0000;
            int iEndAddress   = 0x0000;
            int iEntryPoint   = 0x0000;
            byte[] lyEndCardImage = null;
            int iRecordCount = m_objEmulatorEngine.ProgramLoadText (m_strTokenName, ref iStartAddress, ref iEndAddress, ref iEntryPoint, ref lyEndCardImage);

            // Just load and stop
            m_objEmulatorEngine.m_objEmulatorState.SetRunState (CEmulatorState.ERunMode.RUN_SingleStep);
            m_objEmulatorEngine.SetUIRunMode (CEmulatorEngine.EUIRunMode.RUN_LoadFromText);

            m_objEmulatorEngine.SetHeaderProgrmaName (PrepProgramName (m_strItemName, false));
            m_lstrDisASM = new List<string> (m_objEmulatorEngine.DisassembleCodeText (true));

            m_objEmulatorEngine.FireNewDASMStringListEvent (m_lstrDisASM);

            x_lblProgramCardCount.Content = iRecordCount.ToString ();
            x_lblProgramState.Content     = m_objEmulatorEngine.m_objEmulatorState.GetProgramStateString ();
            SetLineAddresses ();

            m_strDasmOutputFilename  = MakeNewDasmOutputFilename (m_strItemName);
            m_strTraceOutputFilename = MakeNewTraceOutputFilename (m_strItemName);
            m_objEmulatorEngine.DoRun (0x0019, true);
        }

        private void InitializeDisASMPanel ()
        {
            x_cdbxDisassemblyOutput.FontSize = 12;
            x_cdbxDisassemblyOutput.Clear ();

            x_cdbxDisassemblyOutput.Background = new SolidColorBrush (Colors.Transparent);//Color.FromRgb (0xCE, 0xE9, 0xC9));
            x_cdbxDisassemblyOutput.Foreground = new SolidColorBrush (Colors.Transparent);
            x_cdbxDisassemblyOutput.BaseBackground = new SolidColorBrush (Color.FromRgb (0xCE, 0xE9, 0xC9));
            x_cdbxDisassemblyOutput.BaseForeground = new SolidColorBrush (Colors.Transparent);
        }

        private string MakeNewDasmOutputFilename (string strProgramName)
        {
            return MakeNewOutputFilename (strProgramName, "DASM");
        }

        private string MakeNewDumpOutputFilename (string strProgramName)
        {
            return MakeNewOutputFilename (strProgramName, "DUMP");
        }

        private string MakeNewTextOutputFilename (string strProgramName)
        {
            return MakeNewOutputFilename (strProgramName, "TEXT");
        }

        private string MakeNewTraceOutputFilename (string strProgramName)
        {
            return MakeNewOutputFilename (strProgramName, "TRACE");
        }

        private string MakeNewPrinterOutputFilename (string strProgramName)
        {
            return MakeNewOutputFilename (strProgramName, "PRINTER");
        }

        private string MakeNewPunchPatternOutputFilename (string strProgramName)
        {
            return MakeNewOutputFilename (strProgramName, "PUNCH");
        }

        private string MakeNewOutputFilename (string strProgramName, string strPrefix)
        {
            DateTime dtNow = DateTime.Now;
            return strPrefix + string.Format ("_{0}_{1:D4}-{2:D2}-{3:D2}_{4:D2}-{5:D2}-{6:D2}.txt", PrepProgramName (strProgramName),
                                               dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
        }

        private string PrepProgramName (string strRawProgramName, bool bRemoveSpaces = true)
        {
            StringBuilder sbCleanProgramName = new StringBuilder ();

            // If mixed-case, leave case alone, just filter spaces & non-alnum characters
            if (CStringProcessor.IsMixedCase (strRawProgramName))
            {
                foreach (char c in strRawProgramName)
                {
                    if (CEmulatorEngine.IsAlNum (c) ||
                        c  == '&'                   ||
                        (c == ' ' && !bRemoveSpaces))
                    {
                        sbCleanProgramName.Append (c);
                    }
                }

                return sbCleanProgramName.ToString ();
            }
            else if (strRawProgramName.Length == 4)
            {
                return strRawProgramName; // Leave 4-char uppercase strings unchanged
            }

            // Otherwise handle the all upper-case strings
            strRawProgramName = strRawProgramName.ToLower ();
            int iLastSpaceIdx = strRawProgramName.LastIndexOf (' ');
            string strRomanNumeralSuffix = "";

            bool bPrevCharBlank = false;
            char cCaseDiff = (char)('a' - 'A');

            if (iLastSpaceIdx > 0 &&
                iLastSpaceIdx < strRawProgramName.Length)
            {
                string strLastSegment = strRawProgramName.Substring (iLastSpaceIdx + 1);
                if (CEmulatorEngine.IsRomanNumeral (strLastSegment))
                {
                    strRomanNumeralSuffix = strLastSegment.ToUpper ();
                }
            }

            foreach (char c in strRawProgramName)
            {
                if (CEmulatorEngine.IsAlNum (c) ||
                    c  == '&'                   ||
                    (c == ' ' && !bRemoveSpaces))
                {
                    char cThisChar = c;
                    if (sbCleanProgramName.Length == 0 ||
                        bPrevCharBlank)
                    {
                        if (cThisChar >= 'a' && c <= 'z')
                        {
                            cThisChar -= cCaseDiff;
                        }
                    }

                    sbCleanProgramName.Append (cThisChar);
                }

                bPrevCharBlank = c == ' ' || c == '-' || c == '_';
            }

            if (strRomanNumeralSuffix.Length > 0)
            {
                sbCleanProgramName.Remove (sbCleanProgramName.Length - strRomanNumeralSuffix.Length, strRomanNumeralSuffix.Length);
                sbCleanProgramName.Append (strRomanNumeralSuffix);
            }

            return sbCleanProgramName.ToString ();
        }

        void OutputListLine (string strLines, bool bAppend = false)
        {
            List<string> lstrLines = new List<string> ();
            lstrLines.Add (strLines);
            lstrLines.Add ("");
            OutputListLines (lstrLines, bAppend);
        }

        void OutputListLines (List<string> lstrLines, bool bAppend = false)
        {
            if (!bAppend)
            {
                m_objEmulatorEngine.m_objPrintQueue.Clear ();
            }

            m_objEmulatorEngine.m_objPrintQueue.Enqueue (lstrLines, false, true);
        }

        static void DisplayFileTokens (SortedDictionary<string, CDBFileToken> sdTokens)
        {
            foreach (KeyValuePair<string,CDBFileToken> kvp in sdTokens)
            {
                Console.WriteLine (kvp.Key + ':');
                Console.WriteLine ("  " + kvp.Value.FileTokenKey);
                Console.WriteLine ("  " + kvp.Value.TableName);
                Console.WriteLine ("  " + kvp.Value.DataName);
                Console.WriteLine ("  " + kvp.Value.FilePath);
            }

            Console.WriteLine ();
        }

        static void DisplayFileTokens (List<CDBFileToken> lftTokens)
        {
            foreach (CDBFileToken ft in lftTokens)
            {
                Console.WriteLine (ft.FileTokenKey);
                Console.WriteLine (ft.TableName);
                Console.WriteLine (ft.DataName);
                Console.WriteLine (ft.FilePath);
            }

            Console.WriteLine ();
        }

        private void InitializeTracePanel ()
        {
            x_cdbxTraceOutput.FontSize = 12;
            x_cdbxTraceOutput.Clear ();

            x_cdbxTraceOutput.Background = new SolidColorBrush (Colors.Transparent);//Color.FromRgb (0xCE, 0xE9, 0xC9));
            x_cdbxTraceOutput.Foreground = new SolidColorBrush (Colors.Transparent);
            x_cdbxTraceOutput.BaseForeground = new SolidColorBrush (Colors.Transparent);

            //x_cdbxTraceOutput.SetLockObject (ref m_objEmulatorEngine.m_objLock);
        }

        private void ShowSystem3ClockSpeed ()
        {
            if (m_objEmulatorEngine.GetTestOneCardClockProgram ())
            {
                x_lblProcessorSpeed.Content = "Effective System/3 clock speed: 657.895Hz (clock)";
            }
            else if (m_objEmulatorEngine.GetSimulateSystem3CpuTiming ())
            {
                x_lblProcessorSpeed.Content = "Effective System/3 clock speed: 657.895Hz (sim)";
            }
            else
            {
                x_lblProcessorSpeed.Content = string.Format ("Effective System/3 clock speed: {0:F2}MHz", m_dEffectiveSys3ClockMHz);
            }
        }

        private void ShowTraceState ()
        {
            x_lblTraceState.Content = m_objEmulatorEngine.IsInTrace () ? "Trace On" : "Trace Off";
        }

        public void MakeRegisterLabelsDormant ()
        {
            x_lblRegisterIAR_IL0.Content = "0000";
            x_lblRegisterIAR_IL1.Content = "0000";
            x_lblRegisterARR_IL0.Content = "0000";
            x_lblRegisterARR_IL1.Content = "0000";
            x_lblRegisterXR1.Content     = "0000";
            x_lblRegisterXR2.Content     = "0000";
            x_lblCR_LO_EQ_HI.Content     = "EQ";
            x_lblCR_DO.Content           = "  ";
            x_lblCR_TF.Content           = "  ";
            x_lblCR_BO.Content           = "  ";

            x_lblRegisterIAR_IL0.Foreground = m_brGray;
            x_lblRegisterIAR_IL1.Foreground = m_brGray;
            x_lblRegisterARR_IL0.Foreground = m_brGray;
            x_lblRegisterARR_IL1.Foreground = m_brGray;
            x_lblRegisterXR1.Foreground     = m_brGray;
            x_lblRegisterXR2.Foreground     = m_brGray;
            x_lblCR_LO_EQ_HI.Foreground     = m_brGray;
            x_lblCR_DO.Foreground           = m_brGray;
            x_lblCR_TF.Foreground           = m_brGray;
            x_lblCR_BO.Foreground           = m_brGray;
            x_lblRegisterLPFLR.Foreground   = m_brGray;
            x_lblRegisterLPIAR.Foreground   = m_brGray;
            x_lblRegisterLPDAR.Foreground   = m_brGray;
            x_lblRegisterMPDAR.Foreground   = m_brGray;
            x_lblRegisterMRDAR.Foreground   = m_brGray;
            x_lblRegisterMUDAR.Foreground   = m_brGray;
            x_lblRegisterDCAR.Foreground    = m_brGray;
            x_lblRegisterDRWAR.Foreground   = m_brGray;
        }

        public void MakeRegisterLabelsActive ()
        {
            x_lblRegisterIAR_IL0.Foreground = m_brBlack;
            //x_lblRegisterIAR_IL1.Foreground = m_brBlack;
            x_lblRegisterARR_IL0.Foreground = m_brBlack;
            //x_lblRegisterARR_IL1.Foreground = m_brBlack;
            x_lblRegisterXR1.Foreground     = m_brBlack;
            x_lblRegisterXR2.Foreground     = m_brBlack;
            x_lblCR_LO_EQ_HI.Foreground     = m_brBlack;
            x_lblCR_DO.Foreground           = m_brBlack;
            x_lblCR_TF.Foreground           = m_brBlack;
            x_lblCR_BO.Foreground           = m_brBlack;
            x_lblRegisterLPFLR.Foreground   = m_brBlack;
            x_lblRegisterLPIAR.Foreground   = m_brBlack;
            x_lblRegisterLPDAR.Foreground   = m_brBlack;
            x_lblRegisterMPDAR.Foreground   = m_brBlack;
            x_lblRegisterMRDAR.Foreground   = m_brBlack;
            x_lblRegisterMUDAR.Foreground   = m_brBlack;
            x_lblRegisterDCAR.Foreground    = m_brBlack;
            x_lblRegisterDRWAR.Foreground   = m_brBlack;
        }

        #region Command definitions
        //// <CommandBinding Command="Local:CCustomCommands.Exit" CanExecute="OnFileExitCommandCanExecute" Executed="OnFileExitCommandExecuted" />
        //// Exit<alt>-F4
        //private void OnFileExitCommandCanExecute (object sender, CanExecuteRoutedEventArgs e)
        //{
        //    lock (m_objEmulatorEngine.m_objLock)
        //    {
        //        //Console.WriteLine ("OnFileExitCommandCanExecute");
        //        e.CanExecute = true;
        //    }
        //}

        //// <CommandBinding Command="Local:CCustomCommands.Exit" CanExecute="OnFileExitCommandCanExecute" Executed="OnFileExitCommandExecuted" />
        //// Exit<alt>-F4
        //private void OnFileExitCommandExecuted (object sender, ExecutedRoutedEventArgs e)
        //{
        //    lock (m_objEmulatorEngine.m_objLock)
        //    {
        //        //Console.WriteLine ("OnFileExitCommandExecuted");
        //        Application.Current.Shutdown ();
        //    }
        //}

        private void OnExit (object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown ();
        }


        // <CommandBinding Command="Local:CCustomCommands.ShowAboutBox" CanExecute="OnHelpShowAboutBoxCanExecute" Executed="OnHelpShowAboutBoxExecuted" />
        // ShowAboutBox       F1
        private void OnHelpShowAboutBoxCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnHelpShowAboutBoxCanExecute");
                e.CanExecute = true;
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.ShowAboutBox" CanExecute="OnHelpShowAboutBoxCanExecute" Executed="OnHelpShowAboutBoxExecuted" />
        // ShowAboutBox       F1
        private void OnHelpShowAboutBoxExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnHelpShowAboutBoxExecuted");
                ShowAboutBox ();
            }
        }

        //// <CommandBinding Command="Local:CCustomCommands.NextBookmark" CanExecute="OnEditNextBookmarkCanExecute" Executed="OnEditNextBookmarkExecuted" />
        //// NextBookmark       F2
        //private void OnEditNextBookmarkCanExecute (object sender, CanExecuteRoutedEventArgs e)
        //{
        //    lock (m_objEmulatorEngine.m_objLock)
        //    {
        //        //Console.WriteLine ("OnEditNextBookmarkCanExecute");
        //        //Requires a DASM listing in the DASM window
        //        //Requires at least one bookmark to be set
        //        e.CanExecute = false;
        //    }
        //}

        //// <CommandBinding Command="Local:CCustomCommands.NextBookmark" CanExecute="OnEditNextBookmarkCanExecute" Executed="OnEditNextBookmarkExecuted" />
        //// NextBookmark       F2
        //private void OnEditNextBookmarkExecuted (object sender, ExecutedRoutedEventArgs e)
        //{
        //    lock (m_objEmulatorEngine.m_objLock)
        //    {
        //        //Console.WriteLine ("OnEditNextBookmarkExecuted");
        //    }
        //}


        //// <CommandBinding Command="Local:CCustomCommands.PreviousBookmark" CanExecute="OnEditPreviousBookmarkCanExecute" Executed="OnEditPreviousBookmarkExecuted" />
        //// PreviousBookmark   <shft>-F2
        //private void OnEditPreviousBookmarkCanExecute (object sender, CanExecuteRoutedEventArgs e)
        //{
        //    lock (m_objEmulatorEngine.m_objLock)
        //    {
        //        //Console.WriteLine ("OnEditPreviousBookmarkCanExecute");
        //        //Requires a DASM listing in the DASM window
        //        //Requires at least one bookmark to be set
        //        e.CanExecute = false;
        //    }
        //}

        //// <CommandBinding Command="Local:CCustomCommands.PreviousBookmark" CanExecute="OnEditPreviousBookmarkCanExecute" Executed="OnEditPreviousBookmarkExecuted" />
        //// PreviousBookmark   <shft>-F2
        //private void OnEditPreviousBookmarkExecuted (object sender, ExecutedRoutedEventArgs e)
        //{
        //    lock (m_objEmulatorEngine.m_objLock)
        //    {
        //        //Console.WriteLine ("OnEditPreviousBookmarkExecuted");
        //    }
        //}


        //// <CommandBinding Command="Local:CCustomCommands.ToggleBookmark" CanExecute="OnEditToggleBookmarkCanExecute" Executed="OnEditToggleBookmarkExecuted" />
        //// ToggleBookmark     <ctrl>-F2
        //private void OnEditToggleBookmarkCanExecute (object sender, CanExecuteRoutedEventArgs e)
        //{
        //    lock (m_objEmulatorEngine.m_objLock)
        //    {
        //        //Console.WriteLine ("OnEditToggleBookmarkCanExecute");
        //        //Requires a DASM listing in the DASM window
        //        //Requires a line to be selected (if not on a line with an address, the use the address of the closest addressible line about plus line count address)
        //        e.CanExecute = false;
        //    }
        //}

        //// <CommandBinding Command="Local:CCustomCommands.ToggleBookmark" CanExecute="OnEditToggleBookmarkCanExecute" Executed="OnEditToggleBookmarkExecuted" />
        //// ToggleBookmark     <ctrl>-F2
        //private void OnEditToggleBookmarkExecuted (object sender, ExecutedRoutedEventArgs e)
        //{
        //    lock (m_objEmulatorEngine.m_objLock)
        //    {
        //        //Console.WriteLine ("OnEditToggleBookmarkExecuted");
        //    }
        //}


        //// <CommandBinding Command="Local:CCustomCommands.SearchNext" CanExecute="OnEditSearchNextCanExecute" Executed="OnEditSearchNextExecuted" />
        //// SearchNext  F3
        //private void OnEditSearchNextCanExecute (object sender, CanExecuteRoutedEventArgs e)
        //{
        //    lock (m_objEmulatorEngine.m_objLock)
        //    {
        //        //Console.WriteLine ("OnEditSearchNextCanExecute");
        //        // If no DASM listing, then generate one
        //        // If DASM listing in DASM panel, then either search for last search string or prompt for one
        //        e.CanExecute = false;
        //    }
        //}

        //// <CommandBinding Command="Local:CCustomCommands.SearchNext" CanExecute="OnEditSearchNextCanExecute" Executed="OnEditSearchNextExecuted" />
        //// SearchNext  F3
        //private void OnEditSearchNextExecuted (object sender, ExecutedRoutedEventArgs e)
        //{
        //    lock (m_objEmulatorEngine.m_objLock)
        //    {
        //        //Console.WriteLine ("OnEditSearchNextExecuted");
        //    }
        //}


        //// <CommandBinding Command="Local:CCustomCommands.ReverseFind" CanExecute="OnEditReverseFindCanExecute" Executed="OnEditReverseFindExecuted" />
        //// ReverseFind        <shft>-F3
        //private void OnEditReverseFindCanExecute (object sender, CanExecuteRoutedEventArgs e)
        //{
        //    lock (m_objEmulatorEngine.m_objLock)
        //    {
        //        //Console.WriteLine ("OnEditReverseFindCanExecute");
        //        // Requires a DASM listing in the DASM window and a search string to already be entered
        //        e.CanExecute = false;
        //    }
        //}

        //// <CommandBinding Command="Local:CCustomCommands.ReverseFind" CanExecute="OnEditReverseFindCanExecute" Executed="OnEditReverseFindExecuted" />
        //// ReverseFind        <shft>-F3
        //private void OnEditReverseFindExecuted (object sender, ExecutedRoutedEventArgs e)
        //{
        //    lock (m_objEmulatorEngine.m_objLock)
        //    {
        //        //Console.WriteLine ("OnEditReverseFindExecuted");
        //    }
        //}


        //// <CommandBinding Command="Local:CCustomCommands.TrackCodeCoverage" CanExecute="OnEditTrackCodeCoverageCanExecute" Executed="OnEditTrackCodeCoverageExecuted" />
        //// TrackCodeCoverage            F4
        //private void OnEditTrackCodeCoverageCanExecute (object sender, CanExecuteRoutedEventArgs e)
        //{
        //    lock (m_objEmulatorEngine.m_objLock)
        //    {
        //        //Console.WriteLine ("OnEditTrackCodeCoverageCanExecute");
        //        e.CanExecute = m_objEmulatorEngine.m_objEmulatorState.IsProgramLoaded ();
        //    }
        //}

        //// <CommandBinding Command="Local:CCustomCommands.TrackCodeCoverage" CanExecute="OnEditTrackCodeCoverageCanExecute" Executed="OnEditTrackCodeCoverageExecuted" />
        //// TrackCodeCoverage            F4
        //private void OnEditTrackCodeCoverageExecuted (object sender, ExecutedRoutedEventArgs e)
        //{
        //    lock (m_objEmulatorEngine.m_objLock)
        //    {
        //        Console.WriteLine ("OnEditTrackCodeCoverageExecuted");
        //        SetDasmGrayedCodeLines ();
        //    }
        //}


        // <CommandBinding Command="Local:CCustomCommands.Run" CanExecute="OnEmulatorRunCanExecute" Executed="OnEmulatorRunExecuted" />
        // Run                F5
        private void OnEmulatorRunCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnEmulatorRunCanExecute");
                e.CanExecute = m_objEmulatorEngine.m_objEmulatorState.IsProgramLoaded () &&
                               !m_objEmulatorEngine.m_objEmulatorState.IsProgramDead ();
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.Run" CanExecute="OnEmulatorRunCanExecute" Executed="OnEmulatorRunExecuted" />
        // Run                F5
        private void OnEmulatorRunExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnEmulatorRunExecuted");
                m_objEmulatorEngine.m_objEmulatorState.SetRunState (CEmulatorState.ERunMode.RUN_BreakPoints);
                if (m_objEmulatorEngine.m_objEmulatorState.IsRunning ())
                {
                    m_objEmulatorEngine.FireResetControlColorsEvent ();
                    m_objEmulatorEngine.FireMakeRegisterLabelsDormantEvent ();
                }
                else
                {
                    m_objEmulatorEngine.DoRun ();
                }
            }
        }


        // <CommandBinding Command="Local:CCustomCommands.FreeRun" CanExecute="OnEmulatorFreeRunCanExecute" Executed="OnEmulatorFreeRunExecuted" />
        // FreeRun            <ctrl>-F5, F6
        private void OnEmulatorFreeRunCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnEmulatorFreeRunCanExecute");
                e.CanExecute = m_objEmulatorEngine.m_objEmulatorState.IsProgramLoaded () &&
                               !m_objEmulatorEngine.m_objEmulatorState.IsProgramDead ();
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.FreeRun" CanExecute="OnEmulatorFreeRunCanExecute" Executed="OnEmulatorFreeRunExecuted" />
        // FreeRun            <ctrl>-F5, F6
        private void OnEmulatorFreeRunExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnEmulatorFreeRunExecuted");
                m_objEmulatorEngine.m_objEmulatorState.SetRunState (CEmulatorState.ERunMode.RUN_FreeRun);
                m_objEmulatorEngine.m_objEmulatorState.ChangeState (CEmulatorState.EProgramState.PSTATE_3_ProgramRunningFreeRun);
                if (m_objEmulatorEngine.m_objEmulatorState.IsRunning ())
                {
                    m_objEmulatorEngine.FireResetControlColorsEvent ();
                    m_objEmulatorEngine.FireMakeRegisterLabelsDormantEvent ();
                }
                else
                {
                    m_objEmulatorEngine.DoRun ();
                }
            }
        }


        // <CommandBinding Command="Local:CCustomCommands.Stop" CanExecute="OnEmulatorStopCanExecute" Executed="OnEmulatorStopExecuted" />
        // Stop               <shft>-F5), F7
        private void OnEmulatorStopCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnEmulatorStopCanExecute");
                e.CanExecute = m_objEmulatorEngine.m_objEmulatorState.IsProgramLoaded () &&
                               m_objEmulatorEngine.m_objEmulatorState.IsInEmulator ();
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.Stop" CanExecute="OnEmulatorStopCanExecute" Executed="OnEmulatorStopExecuted" />
        // Stop               <shft>-F5), F7
        private void OnEmulatorStopExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnEmulatorStopExecuted");
                m_objEmulatorEngine.Stop ();

                x_lbl5475Display.Content = "";
                x_lblHPLDisplay.Content  = "";
            }
        }


        // <CommandBinding Command="Local:CCustomCommands.UnloadProgram" CanExecute="OnEmulatorUnloadProgramCanExecute" Executed="OnEmulatorUnloadProgramExecuted" />
        // UnloadProgram      F8
        private void OnEmulatorUnloadProgramCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnEmulatorUnloadProgramCanExecute");
                e.CanExecute = m_objEmulatorEngine.m_objEmulatorState.IsProgramLoaded ();
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.UnloadProgram" CanExecute="OnEmulatorUnloadProgramCanExecute" Executed="OnEmulatorUnloadProgramExecuted" />
        // UnloadProgram      F8
        private void OnEmulatorUnloadProgramExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnEmulatorUnloadProgramExecuted");
                m_objEmulatorEngine.Stop ();
                m_objEmulatorEngine.UnloadProgram ();
                m_objEmulatorEngine.FireResetControlColorsEvent ();
                m_objEmulatorEngine.FireMakeRegisterLabelsDormantEvent ();

                x_lbl5475Display.Content      = "";
                x_lblHPLDisplay.Content       = "";
                x_lblProgramName.Content      = "";
                x_lblProgramCardCount.Content = "";
                x_lblProgramSize.Content      = "";

                x_tbxPrinterOutput.Clear ();
                x_cdbxDisassemblyOutput.Clear ();
                x_cdbxTraceOutput.Clear ();
            }
        }


        // <CommandBinding Command="Local:CCustomCommands.RotateBreakpoint" CanExecute="OnEmulatorRotateBreakpointCanExecute" Executed="OnEmulatorRotateBreakpointExecuted" />
        // RotateBreakpoint   F9  (none / present / disabled)
        private void OnEmulatorRotateBreakpointCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnEmulatorRotateBreakpointCanExecute");
                // Requires a DASM listing in the DASM panel
                // Requires a line with an address to be selected in the DASM panel
                e.CanExecute = m_objEmulatorEngine.m_objEmulatorState.IsProgramLoaded () &&
                               !m_objEmulatorEngine.m_objEmulatorState.IsProgramDead ();
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.RotateBreakpoint" CanExecute="OnEmulatorRotateBreakpointCanExecute" Executed="OnEmulatorRotateBreakpointExecuted" />
        // RotateBreakpoint   F9  (none / present / disabled)
        private void OnEmulatorRotateBreakpointExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnEmulatorRotateBreakpointExecuted");
            }
        }


        // <CommandBinding Command="Local:CCustomCommands.SingleStep" CanExecute="OnEmulatorSingleStepCanExecute" Executed="OnEmulatorSingleStepExecuted" />
        // SingleStep         F10
        private void OnEmulatorSingleStepCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnEmulatorSingleStepCanExecute");
                e.CanExecute = m_objEmulatorEngine.m_objEmulatorState.IsProgramLoaded () &&
                               !m_objEmulatorEngine.m_objEmulatorState.IsProgramDead ();
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.SingleStep" CanExecute="OnEmulatorSingleStepCanExecute" Executed="OnEmulatorSingleStepExecuted" />
        // SingleStep         F10
        private void OnEmulatorSingleStepExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnEmulatorSingleStepExecuted");
                //Debug.Assert (x_cdbxDisassemblyOutput.Focus ());
                if (m_objEmulatorEngine.m_objEmulatorState.IsInEmulator ())
                {
                    m_objEmulatorEngine.m_objEmulatorState.SetRunState (CEmulatorState.ERunMode.RUN_SingleStep); //.SetF10Pressed ();
                    m_objEmulatorEngine.m_objEmulatorState.ChangeState (CEmulatorState.EProgramState.PSTATE_3_ProgramRunningSingleStep);
                    // if in Halted mode, F10 sets single-step mode as well as escapes from the halt like Enter
                    //if (m_objEmulatorEngine.m_objEmulatorState.IsHalted ())
                    //{
                    //}
                }
                else
                {
                    m_objEmulatorEngine.m_objEmulatorState.SetRunState (CEmulatorState.ERunMode.RUN_SingleStep);
                    m_objEmulatorEngine.DoRun ();
                }
            }
        }


        // Toggle simulation of System/3 CPU clock speed
        // <CommandBinding Command="Local:CCustomCommands.ToggleCPUClock" CanExecute="OnEmulatorToggleCPUClockCanExecute" Executed="OnEmulatorToggleCPUClockExecuted" />
        // ToggleCPUClock     F11  Toggle simulation of System/3 CPU clock speed
        private void OnEmulatorToggleCPUClockCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnEmulatorToggleCPUClockCanExecute");
                e.CanExecute = true;
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.ToggleCPUClock" CanExecute="OnEmulatorToggleCPUClockCanExecute" Executed="OnEmulatorToggleCPUClockExecuted" />
        // ToggleCPUClock     F11  Toggle simulation of System/3 CPU clock speed
        private void OnEmulatorToggleCPUClockExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnEmulatorToggleCPUClockExecuted");
                m_objEmulatorEngine.RotateCPUClock ();
                ShowSystem3ClockSpeed ();
            }
        }


        // <CommandBinding Command="Local:CCustomCommands.ToggleTrace" CanExecute="OnEmulatorToggleTraceCanExecute" Executed="OnEmulatorToggleTraceExecuted" />
        // ToggleTrace        F12
        private void OnEmulatorToggleTraceCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnEmulatorToggleTraceCanExecute");
                e.CanExecute = m_objEmulatorEngine.m_objEmulatorState.IsProgramLoaded ();
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.ToggleTrace" CanExecute="OnEmulatorToggleTraceCanExecute" Executed="OnEmulatorToggleTraceExecuted" />
        // ToggleTrace        F12
        private void OnEmulatorToggleTraceExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnEmulatorToggleTraceExecuted");
                if (m_objEmulatorEngine.IsInTrace ())
                {
                    m_objEmulatorEngine.ResetTrace (true);
                    m_objEmulatorEngine.ResetShowDisassembly ();
                    m_objEmulatorEngine.ResetShowChangedValues ();
                    m_objEmulatorEngine.ResetShowIOBuffers ();
                    m_objEmulatorEngine.WriteOutput ("- - - - - - - - - - - - - - - - - - - - - - - - < S T O P    T R A C E > - - - - - - - - - - - - - - - - - - - - - - - -",
                    //m_objEmulatorEngine.WriteOutput ("                                                < S T O P    T R A C E >",
                                                     CEmulatorEngine.EOutputTarget.OUTPUT_TracePanel);
                }
                else
                {
                    m_objEmulatorEngine.SetTrace (true);
                    m_objEmulatorEngine.SetShowDisassembly ();
                    m_objEmulatorEngine.SetShowChangedValues ();
                    m_objEmulatorEngine.SetShowIOBuffers ();
                    m_objEmulatorEngine.WriteOutput ("- - - - - - - - - - - - - - - - - - - - - - - - < S T A R T   T R A C E > - - - - - - - - - - - - - - - - - - - - - - - -",
                    //m_objEmulatorEngine.WriteOutput ("                                                < S T A R T   T R A C E >",
                                                     CEmulatorEngine.EOutputTarget.OUTPUT_TracePanel);
                }

                ShowTraceState ();
            }
        }


        // Keystroke-only (no menu) commands
        // <CommandBinding Command="Local:CCustomCommands.SystemReset" CanExecute="OnEmulatorSystemResetCanExecute" Executed="OnEmulatorSystemResetExecuted" />
        // SystemReset        <ctrl>-R
        private void OnEmulatorSystemResetCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnEmulatorSystemResetCanExecute");
                e.CanExecute = true;
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.SystemReset" CanExecute="OnEmulatorSystemResetCanExecute" Executed="OnEmulatorSystemResetExecuted" />
        // SystemReset        <ctrl>-R
        private void OnEmulatorSystemResetExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnEmulatorSystemResetExecuted");
                m_objEmulatorEngine.Stop ();
                m_objEmulatorEngine.SystemReset (true);
                m_objEmulatorEngine.ResetTestOneCardClockProgram ();
                m_objEmulatorEngine.ResetSimulateSystem3CpuTiming ();
                m_objEmulatorEngine.FireResetControlColorsEvent ();
                m_objEmulatorEngine.FireMakeRegisterLabelsDormantEvent ();

                ShowSystem3ClockSpeed ();
                ShowTraceState ();

                m_lstrDisASM.Clear ();
                m_objEmulatorEngine.m_objTraceQueue.Clear ();
                m_strDasmOutputFilename       = "";
                m_strTraceOutputFilename      = "";
                m_strPrinterOutputFilename    = "";

                x_lbl5475Display.Content      = "";
                x_lblHPLDisplay.Content       = "";
                x_lblProgramName.Content      = "";
                x_lblProgramCardCount.Content = "";
                x_lblProgramSize.Content      = "";

                x_tbxPrinterOutput.Clear ();
                x_cdbxDisassemblyOutput.Clear ();
                x_cdbxTraceOutput.Clear ();
            }
        }

        #region Key Navigation Commands
        //<CommandBinding Command = "Local:CCustomCommands.SaveToFile" CanExecute="OnFileSaveToFileCanExecute" Executed="OnFileSaveToFileExecuted"/>
        // <ctrl>-S
        private void OnFileSaveToFileCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnFileSaveToFileCanExecute");
                e.CanExecute = ((m_strDasmOutputFilename.Length > 0                     &&
                                 m_lstrDisASM.Count > 0)                                ||
                                (m_objEmulatorEngine.m_objTraceQueue.HasQueuedLines ()) ||
                                (m_objEmulatorEngine.m_objPrintQueue.HasQueuedLines ()));
            }
        }

        //<CommandBinding Command = "Local:CCustomCommands.SaveToFile" CanExecute="OnFileSaveToFileCanExecute" Executed="OnFileSaveToFileExecuted"/>
        // <ctrl>-S
        private void OnFileSaveToFileExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnFileSaveToFileExecuted");
                if (m_strLastFocusPanel == "DASM")
                {
                    if (m_lstrDisASM.Count > 0)
                    {
                        //File.WriteAllLines (m_strDasmOutputFilename, m_lstrDisASM.ToArray ());
                        if (File.Exists (m_strDasmOutputFilename))
                        {
                            List<string> lstrBlankLines = new List<string> ();
                            lstrBlankLines.Add ("\n");
                            lstrBlankLines.Add ("\n");
                            File.AppendAllLines (m_strDasmOutputFilename, lstrBlankLines);
                        }
                        File.AppendAllLines (m_strDasmOutputFilename, m_lstrDisASM.ToArray ());
                    }
                }
                else if (m_strLastFocusPanel == "TraceOutput")
                {
                    if (m_objEmulatorEngine.m_objTraceQueue.HasQueuedLines ())
                    {
                        m_objEmulatorEngine.m_objTraceQueue.WriteToFile (m_strTraceOutputFilename);
                    }
                }
                else if (m_strLastFocusPanel == "PrinterOutput")
                {
                    m_objEmulatorEngine.m_objPrintQueue.WriteToFile (m_strPrinterOutputFilename);
                }
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.EnterKey" CanExecute="OnHelpEnterKeyCanExecute" Executed="OnHelpEnterKeyExecuted" />
        // EnterKey
        private void OnEnterKeyCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnEnterKeyCanExecute");
                e.CanExecute = m_objEmulatorEngine.m_objEmulatorState.IsHalted () ||
                               m_objEmulatorEngine.m_objEmulatorState.IsPaused () ||
                               m_objEmulatorEngine.IsKeyboardInputEnabled ();
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.EnterKey" CanExecute="OnHelpEnterKeyCanExecute" Executed="OnHelpEnterKeyExecuted" />
        // EnterKey
        private void OnEnterKeyExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnEnterKeyExecuted");
                if (m_objEmulatorEngine.m_objEmulatorState.IsHalted () ||
                    m_objEmulatorEngine.m_objEmulatorState.IsPaused ())
                {
                    m_objEmulatorEngine.m_objEmulatorState.SetRunState ();
                }
                else if (m_objEmulatorEngine.IsKeyboardInputEnabled () &&
                         m_objEmulatorEngine.IsKeyboard5471 ())
                {
                    // Capture Enter keystroke for 5471 keyboard
                    m_objEmulatorEngine.SetKeyStroke ("Enter");
                }
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.EscapeKey" CanExecute="OnEscapeKeyCanExecute" Executed="OnEscapeKeyExecuted" />
        // EscapeKey
        private void OnEscapeKeyCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnEscapeKeyCanExecute");
                e.CanExecute = m_objEmulatorEngine.m_objEmulatorState.IsHalted () ||
                               m_objEmulatorEngine.IsKeyboardInputEnabled ();
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.EscapeKey" CanExecute="OnEscapeKeyCanExecute" Executed="OnEscapeKeyExecuted" />
        // EscapeKey
        private void OnEscapeKeyExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                //Console.WriteLine ("OnEscapeKeyExecuted");
                if (m_objEmulatorEngine.m_objEmulatorState.IsHalted ())
                {
                    m_objEmulatorEngine.m_objEmulatorState.ChangeState (CEmulatorState.EProgramState.PSTATE_5_Aborted); // .PSTATE_1_NoProgramLoaded);
                    m_objEmulatorEngine.FireResetControlColorsEvent ();
                    m_objEmulatorEngine.FireMakeRegisterLabelsDormantEvent ();
                    m_objEmulatorEngine.FireClearHaltAnd5475Events ();
                    m_objEmulatorEngine.FireClearPrinterPanelEvent ();
                }
                else if (m_objEmulatorEngine.IsKeyboardInputEnabled ())
                {
                    // Capture Escape keystroke for 5471/5475 keyboard
                    m_objEmulatorEngine.SetKeyStroke ("Escape");
                }
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.PauseKey" CanExecute="OnPauseKeyCanExecute" Executed="OnPauseKeyExecuted" />
        // PauseKey
        private void OnPauseKeyCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnPauseKeyCanExecute");
                e.CanExecute = m_objEmulatorEngine.m_objEmulatorState.IsRunning ()/* &&
                               !m_objEmulatorEngine.m_objEmulatorState.IsFreeRun ()*/;
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.PauseKey" CanExecute="OnPauseKeyCanExecute" Executed="OnPauseKeyExecuted" />
        // PauseKey
        private void OnPauseKeyExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnPauseKeyExecuted");
                //x_cdbxTraceOutput.TogglePauseState (); // TODO: Needs Render to be running in a separate thread
                //m_objEmulatorEngine.m_objEmulatorState.ChangeState (CEmulatorState.EProgramState.PSTATE_4_ProgramPaused);
                //m_objEmulatorEngine.FireResetControlColorsEvent ();
                //m_objEmulatorEngine.FireMakeRegisterLabelsDormantEvent ();
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.HomeKey" CanExecute="OnHomeKeyCanExecute" Executed="OnHomeKeyExecuted" />
        // HomeKey
        private void OnHomeKeyCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnHomeKeyCanExecute");
                e.CanExecute = m_objEmulatorEngine.m_objEmulatorState.IsPaused () &&
                               m_objEmulatorEngine.m_objEmulatorState.IsSingleStep ();
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.HomeKey" CanExecute="OnHomeKeyCanExecute" Executed="OnHomeKeyExecuted" />
        // HomeKey
        private void OnHomeKeyExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnHomeKeyExecuted");
                //x_cdbxTraceOutput.OnHomeKey ();
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.EndKey" CanExecute="OnEndKeyCanExecute" Executed="OnEndKeyExecuted" />
        // EndKey
        private void OnEndKeyCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnEndKeyCanExecute");
                e.CanExecute = m_objEmulatorEngine.m_objEmulatorState.IsPaused () &&
                               m_objEmulatorEngine.m_objEmulatorState.IsSingleStep ();
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.EndKey" CanExecute="OnEndKeyCanExecute" Executed="OnEndKeyExecuted" />
        // EndKey
        private void OnEndKeyExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnEndKeyExecuted");
                //x_cdbxTraceOutput.OnEndKey ();
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.PageUpKey" CanExecute="OnPageUpKeyCanExecute" Executed="OnPageUpKeyExecuted" />
        // PageUpKey
        private void OnPageUpKeyCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnPageUpKeyCanExecute");
                //e.CanExecute = m_objEmulatorEngine.m_objEmulatorState.IsPaused () &&
                //               m_objEmulatorEngine.m_objEmulatorState.IsSingleStep ();
                e.CanExecute = false;
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.PageUpKey" CanExecute="OnPageUpKeyCanExecute" Executed="OnPageUpKeyExecuted" />
        // PageUpKey
        private void OnPageUpKeyExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnPageUpKeyExecuted");
                //x_cdbxTraceOutput.OnPageUpKey ();
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.PageDownKey" CanExecute="OnPageDownKeyCanExecute" Executed="OnPageDownKeyExecuted" />
        // PageDownKey
        private void OnPageDownKeyCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnPageDownKeyCanExecute");
                //e.CanExecute = m_objEmulatorEngine.m_objEmulatorState.IsPaused () &&
                //               m_objEmulatorEngine.m_objEmulatorState.IsSingleStep ();
                e.CanExecute = false;
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.PageDownKey" CanExecute="OnPageDownKeyCanExecute" Executed="OnPageDownKeyExecuted" />
        // PageDownKey
        private void OnPageDownKeyExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnPageDownKeyExecuted");
                //x_cdbxTraceOutput.OnPageDownKey ();
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.UpArrowKey" CanExecute="OnUpArrowKeyCanExecute" Executed="OnUpArrowKeyExecuted" />
        // UpArrowKey
        private void OnUpArrowKeyCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnUpArrowKeyCanExecute");
                e.CanExecute = m_objEmulatorEngine.m_objEmulatorState.IsPaused () &&
                               m_objEmulatorEngine.m_objEmulatorState.IsSingleStep ();
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.UpArrowKey" CanExecute="OnUpArrowKeyCanExecute" Executed="OnUpArrowKeyExecuted" />
        // UpArrowKey
        private void OnUpArrowKeyExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnUpArrowKeyExecuted");
                //x_cdbxTraceOutput.OnUpArrowKey ();
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.DownArrowKey" CanExecute="OnDownArrowKeyCanExecute" Executed="OnDownArrowKeyExecuted" />
        // DownArrowKey
        private void OnDownArrowKeyCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnDownArrowKeyCanExecute");
                e.CanExecute = m_objEmulatorEngine.m_objEmulatorState.IsPaused () &&
                               m_objEmulatorEngine.m_objEmulatorState.IsSingleStep ();
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.DownArrowKey" CanExecute="OnDownArrowKeyCanExecute" Executed="OnDownArrowKeyExecuted" />
        // DownArrowKey
        private void OnDownArrowKeyExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnDownArrowKeyExecuted");
                //x_cdbxTraceOutput.OnDownArrowKey ();
            }
        }
        #endregion

        #region 5471/5475 Key Definitions
        // <CommandBinding Command="Local:CCustomCommands.F1Key" CanExecute="OnF1KeyCanExecute" Executed="OnF1KeyExecuted" />
        // F1Key
        private void OnF1KeyCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnF1KeyCanExecute");
                e.CanExecute = m_objEmulatorEngine.IsKeyboardInputEnabled (); // Only availble with 5475 or 5471 keyboard active
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.F1Key" CanExecute="OnF1KeyCanExecute" Executed="OnF1KeyExecuted" />
        // F1Key
        private void OnF1KeyExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnF1KeyExecuted");
                if (m_objEmulatorEngine.IsKeyboardInputEnabled () &&
                    m_objEmulatorEngine.IsKeyboard5475 ())
                {
                    // Capture F1 keystroke for 5475 keyboard
                    m_objEmulatorEngine.SetKeyStroke ("F1");
                }
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.F2Key" CanExecute="OnF2KeyCanExecute" Executed="OnF2KeyExecuted" />
        // F2Key
        private void OnF2KeyCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnF2KeyCanExecute");
                e.CanExecute = m_objEmulatorEngine.IsKeyboardInputEnabled (); // Only availble with 5475 or 5471 keyboard active
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.F2Key" CanExecute="OnF2KeyCanExecute" Executed="OnF2KeyExecuted" />
        // F2Key
        private void OnF2KeyExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnF2KeyExecuted");
                if (m_objEmulatorEngine.IsKeyboardInputEnabled () &&
                    m_objEmulatorEngine.IsKeyboard5475 ())
                {
                    // Capture F2 keystroke for 5475 keyboard
                    m_objEmulatorEngine.SetKeyStroke ("F2");
                }
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.F3Key" CanExecute="OnF3KeyCanExecute" Executed="OnF3KeyExecuted" />
        // F3Key
        private void OnF3KeyCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnF3KeyCanExecute");
                e.CanExecute = m_objEmulatorEngine.IsKeyboardInputEnabled (); // Only availble with 5475 or 5471 keyboard active
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.F3Key" CanExecute="OnF3KeyCanExecute" Executed="OnF3KeyExecuted" />
        // F3Key
        private void OnF3KeyExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnF3KeyExecuted");
                if (m_objEmulatorEngine.IsKeyboardInputEnabled () &&
                    m_objEmulatorEngine.IsKeyboard5475 ())
                {
                    // Capture F3 keystroke for 5475 keyboard
                    m_objEmulatorEngine.SetKeyStroke ("F3");
                }
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.F4Key" CanExecute="OnF4KeyCanExecute" Executed="OnF4KeyExecuted" />
        // F4Key
        private void OnF4KeyCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnF4KeyCanExecute");
                e.CanExecute = m_objEmulatorEngine.IsKeyboardInputEnabled (); // Only availble with 5475 or 5471 keyboard active
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.F4Key" CanExecute="OnF4KeyCanExecute" Executed="OnF4KeyExecuted" />
        // F4Key
        private void OnF4KeyExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnF4KeyExecuted");
                if (m_objEmulatorEngine.IsKeyboardInputEnabled ())
                {
                    // Capture F4 keystroke for 5471/5475 keyboard
                    m_objEmulatorEngine.SetKeyStroke ("F4");
                }
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.F5Key" CanExecute="OnF5KeyCanExecute" Executed="OnF5KeyExecuted" />
        // F5Key
        private void OnF5KeyCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnF5KeyCanExecute");
                e.CanExecute = m_objEmulatorEngine.IsKeyboardInputEnabled (); // Only availble with 5475 or 5471 keyboard active
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.F5Key" CanExecute="OnF5KeyCanExecute" Executed="OnF5KeyExecuted" />
        // F5Key
        private void OnF5KeyExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnF5KeyExecuted");
                if (m_objEmulatorEngine.IsKeyboardInputEnabled () &&
                    m_objEmulatorEngine.IsKeyboard5475 ())
                {
                    // Capture F5 keystroke for 5475 keyboard
                    m_objEmulatorEngine.SetKeyStroke ("F5");
                }
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.F6Key" CanExecute="OnF6KeyCanExecute" Executed="OnF6KeyExecuted" />
        // F6Key
        private void OnF6KeyCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnF6KeyCanExecute");
                e.CanExecute = m_objEmulatorEngine.IsKeyboardInputEnabled (); // Only availble with 5475 or 5471 keyboard active
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.F6Key" CanExecute="OnF6KeyCanExecute" Executed="OnF6KeyExecuted" />
        // F6Key
        private void OnF6KeyExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnF6KeyExecuted");
                if (m_objEmulatorEngine.IsKeyboardInputEnabled ())
                {
                    // Capture F6 keystroke for 5471/5475 keyboard
                    m_objEmulatorEngine.SetKeyStroke ("F6");
                }
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.F7Key" CanExecute="OnF7KeyCanExecute" Executed="OnF7KeyExecuted" />
        // F7Key
        private void OnF7KeyCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnF7KeyCanExecute");
                e.CanExecute = m_objEmulatorEngine.IsKeyboardInputEnabled (); // Only availble with 5475 or 5471 keyboard active
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.F7Key" CanExecute="OnF7KeyCanExecute" Executed="OnF7KeyExecuted" />
        // F7Key
        private void OnF7KeyExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnF7KeyExecuted");
                if (m_objEmulatorEngine.IsKeyboardInputEnabled () &&
                    m_objEmulatorEngine.IsKeyboard5475 ())
                {
                    // Capture F7 keystroke for 5475 keyboard
                    m_objEmulatorEngine.SetKeyStroke ("F7");
                }
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.F8Key" CanExecute="OnF8KeyCanExecute" Executed="OnF8KeyExecuted" />
        // F8Key
        private void OnF8KeyCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnF8KeyCanExecute");
                e.CanExecute = m_objEmulatorEngine.IsKeyboardInputEnabled (); // Only availble with 5475 or 5471 keyboard active
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.F8Key" CanExecute="OnF8KeyCanExecute" Executed="OnF8KeyExecuted" />
        // F8Key
        private void OnF8KeyExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnF8KeyExecuted");
                if (m_objEmulatorEngine.IsKeyboardInputEnabled () &&
                    m_objEmulatorEngine.IsKeyboard5475 ())
                {
                    // Capture F8 keystroke for 5475 keyboard
                    m_objEmulatorEngine.SetKeyStroke ("F8");
                }
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.F9Key" CanExecute="OnF9KeyCanExecute" Executed="OnF9KeyExecuted" />
        // F9Key
        private void OnF9KeyCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnF9KeyCanExecute");
                e.CanExecute = m_objEmulatorEngine.IsKeyboardInputEnabled (); // Only availble with 5475 or 5471 keyboard active
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.F9Key" CanExecute="OnF9KeyCanExecute" Executed="OnF9KeyExecuted" />
        // F9Key
        private void OnF9KeyExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnF9KeyExecuted");
                if (m_objEmulatorEngine.IsKeyboardInputEnabled () &&
                    m_objEmulatorEngine.IsKeyboard5475 ())
                {
                    // Capture F9 keystroke for 5475 keyboard
                    m_objEmulatorEngine.SetKeyStroke ("F9");
                }
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.F10Key" CanExecute="OnF10KeyCanExecute" Executed="OnF10KeyExecuted" />
        // F10Key
        private void OnF10KeyCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnF10KeyCanExecute");
                e.CanExecute = m_objEmulatorEngine.IsKeyboardInputEnabled (); // Only availble with 5475 or 5471 keyboard active
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.F10Key" CanExecute="OnF10KeyCanExecute" Executed="OnF10KeyExecuted" />
        // F10Key
        private void OnF10KeyExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnF10KeyExecuted");
                if (m_objEmulatorEngine.IsKeyboardInputEnabled () &&
                    m_objEmulatorEngine.IsKeyboard5475 ())
                {
                    // Capture F10 keystroke for 5475 keyboard
                    m_objEmulatorEngine.SetKeyStroke ("F10");
                }
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.F11Key" CanExecute="OnF11KeyCanExecute" Executed="OnF11KeyExecuted" />
        // F11Key
        private void OnF11KeyCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnF11KeyCanExecute");
                e.CanExecute = m_objEmulatorEngine.IsKeyboardInputEnabled (); // Only availble with 5475 or 5471 keyboard active
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.F11Key" CanExecute="OnF11KeyCanExecute" Executed="OnF11KeyExecuted" />
        // F11Key
        private void OnF11KeyExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnF11KeyExecuted");
                if (m_objEmulatorEngine.IsKeyboardInputEnabled () &&
                    m_objEmulatorEngine.IsKeyboard5475 ())
                {
                    // Capture F11 keystroke for 5475 keyboard
                    m_objEmulatorEngine.SetKeyStroke ("F11");
                }
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.F12Key" CanExecute="OnF12KeyCanExecute" Executed="OnF12KeyExecuted" />
        // F12Key
        private void OnF12KeyCanExecute (object sender, CanExecuteRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnF12KeyCanExecute");
                e.CanExecute = m_objEmulatorEngine.IsKeyboardInputEnabled (); // Only availble with 5475 or 5471 keyboard active
            }
        }

        // <CommandBinding Command="Local:CCustomCommands.F12Key" CanExecute="OnF12KeyCanExecute" Executed="OnF12KeyExecuted" />
        // F12Key
        private void OnF12KeyExecuted (object sender, ExecutedRoutedEventArgs e)
        {
            lock (m_objEmulatorEngine.m_objLock)
            {
                Console.WriteLine ("OnF12KeyExecuted");
                if (m_objEmulatorEngine.IsKeyboardInputEnabled () &&
                    m_objEmulatorEngine.IsKeyboard5475 ())
                {
                    // Capture F12 keystroke for 5475 keyboard
                    m_objEmulatorEngine.SetKeyStroke ("F12");
                }
            }
        }
        #endregion
        #endregion

        private void OnStatusBarToolTip (object sender, ToolTipEventArgs e)
        {
            x_sbMainWindow.ToolTip = "ProgramName:"      + x_lblProgramName.Content      + '\n' +
                                     "ProgramCardCount:" + x_lblProgramCardCount.Content + '\n' +
                                     "ProgramSize:"      + x_lblProgramSize.Content      + '\n' +
                                     "ProgramState:"     + x_lblProgramState.Content     + '\n' +
                                     "ProcessorSpeed:"   + x_lblProcessorSpeed.Content   + '\n' +
                                     "StepCount:"        + x_lblStepCount.Content        + '\n' +
                                     "TraceState:"       + x_lblTraceState.Content;
        }

        private void OnPreviewCPUDialKeystroke (object sender, KeyEventArgs e)
        {
            //if (x_txbCPUDials.Text.Length == 4)
            //{
            //    e.Handled = true;
            //}
            if (x_txbCPUDials.Text.Length > 4)
            {
                x_txbCPUDials.Text = x_txbCPUDials.Text.Substring (0, 4);
            }

            string strKey = e.Key.ToString ();
            if (strKey == "Back"       ||
                strKey == "Delete"     ||
                strKey == "Left"       ||
                strKey == "Right"      ||
                strKey == "LeftShift"  ||
                strKey == "RightShift" ||
                strKey == "System")
            {
                return;
            }

            if (strKey == "Up" ||
                strKey == "Down")
            {
                HandleConsoleDialSpining (strKey == "Up");
            }

            if ((strKey.Length == 2     &&
                 strKey.Contains ("D")) ||
                (strKey.Length == 7     &&
                 strKey.Contains ("NumPad")))
            {
                strKey = strKey.Substring (strKey.Length - 1);
            }

            Key key = e.Key;

            if (!CStringProcessor.IsHexadecimal (strKey.ToString ()/*) &&
                CStringProcessor.IsPrintable    (key.ToString ())*/))
            {
                e.Handled = true;
            }
        }

        private void OnKeyUpCPUDialKeystroke (object sender, KeyEventArgs e)
        {
            string strKey = x_txbCPUDials.Text.ToUpper ();
            m_objEmulatorEngine.SetConsoleDials (CStringProcessor.SafeConvertHexadecimalStringToInt16 (strKey));
        }

        //private void OnMouseLeftButtonUpCPUDials (object sender, MouseButtonEventArgs e)
        //{
        //    x_txbCPUDials.SelectAll ();
        //}

        private void OnGotKeyboardFocusCPUDials (object sender, KeyboardFocusChangedEventArgs e)
        {
            string s = e.NewFocus.ToString ();
            Console.WriteLine ("OnGotKeyboardFocus: OldFocus: " + e.OldFocus.ToString () + " NewFocus: " + s);
            m_strLastFocusPanel = "";
            if (x_txbCPUDials.Text.Length < 4)
            {
                x_txbCPUDials.Text = m_objEmulatorEngine.GetConsoleDials ().ToString ("X4");
            }
            //x_txbCPUDials.SelectAll ();
        }

        //private void OnLostKeyboardFocus (object sender, KeyboardFocusChangedEventArgs e)
        //{
        //    string s = e.NewFocus.ToString ();
        //    Console.WriteLine ("OnLostKeyboardFocus: OldFocus: " + e.OldFocus.ToString () + " NewFocus: " + s);
        //}

        private void OnGotKeyboardFocusPrinterOutput (object sender, KeyboardFocusChangedEventArgs e)
        {
            //Console.WriteLine ("OnGotKeyboardFocusPrinterOutput ()");

            m_strLastFocusPanel = "PrinterOutput";
        }

        private void OnGotKeyboardFocusTraceOutput (object sender, KeyboardFocusChangedEventArgs e)
        {
            //Console.WriteLine ("OnGotKeyboardFocusTraceOutput ()");

            m_strLastFocusPanel = "TraceOutput";
        }

        private void OnGotKeyboardFocusDASM (object sender, KeyboardFocusChangedEventArgs e)
        {
            //Console.WriteLine ("OnGotKeyboardFocusDASM ()");

            m_strLastFocusPanel = "DASM";
        }
    }

    ////////////////////////////////////////
    //
    //  TODO: 5471 CODE REQUIRES F4, F6, ENTER, AND ESCAPE
    //        5475 CODE REQUIRES F1 THROUGH F12
    //
    ////////////////////////////////////////
    public static class CCustomCommands
    {
        public static readonly RoutedUICommand SaveToFile        = new RoutedUICommand ("SaveToFile", "SaveToFile", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.S, ModifierKeys.Control) });
        public static readonly RoutedUICommand ShowAboutBox      = new RoutedUICommand ("ShowAboutBox", "ShowAboutBox", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.F1),
                                                                                          new KeyGesture (Key.H, ModifierKeys.Control) });
        //public static readonly RoutedUICommand NextBookmark      = new RoutedUICommand ("NextBookmark", "NextBookmark", typeof (CCustomCommands), new InputGestureCollection ()
        //                                                                                { new KeyGesture (Key.F2),
        //                                                                                  new KeyGesture (Key.K, ModifierKeys.Control) });
        //public static readonly RoutedUICommand PreviousBookmark  = new RoutedUICommand ("PreviousBookmark", "PreviousBookmark", typeof (CCustomCommands), new InputGestureCollection ()
        //                                                                                { new KeyGesture (Key.F2, ModifierKeys.Shift) });
        //public static readonly RoutedUICommand ToggleBookmark    = new RoutedUICommand ("ToggleBookmark", "ToggleBookmark", typeof (CCustomCommands), new InputGestureCollection ()
        //                                                                                { new KeyGesture (Key.F2, ModifierKeys.Control) });
        //public static readonly RoutedUICommand SearchNext        = new RoutedUICommand ("SearchNext", "SearchNext", typeof (CCustomCommands), new InputGestureCollection ()
        //                                                                                { new KeyGesture (Key.F3),
        //                                                                                  new KeyGesture (Key.F, ModifierKeys.Control) });
        //public static readonly RoutedUICommand ReverseFind       = new RoutedUICommand ("ReverseFind", "ReverseFind", typeof (CCustomCommands), new InputGestureCollection ()
        //                                                                                { new KeyGesture (Key.F3, ModifierKeys.Shift) });
        //public static readonly RoutedUICommand TrackCodeCoverage = new RoutedUICommand ("TrackCodeCoverage", "TrackCodeCoverage", typeof (CCustomCommands), new InputGestureCollection ()
        //                                                                                { new KeyGesture (Key.F4) });
        public static readonly RoutedUICommand Run               = new RoutedUICommand ("Run", "Run", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.F5),
                                                                                          new KeyGesture (Key.R, ModifierKeys.Control) });
        public static readonly RoutedUICommand FreeRun           = new RoutedUICommand ("FreeRun", "FreeRun", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.F5, ModifierKeys.Control),
                                                                                          new KeyGesture (Key.F6),
                                                                                          new KeyGesture (Key.E, ModifierKeys.Control) });
        public static readonly RoutedUICommand Stop              = new RoutedUICommand ("Stop", "Stop", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.F5, ModifierKeys.Shift),
                                                                                          new KeyGesture (Key.F7),
                                                                                          new KeyGesture (Key.S, ModifierKeys.Control) });
        public static readonly RoutedUICommand UnloadProgram     = new RoutedUICommand ("UnloadProgram", "UnloadProgram", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.F8),
                                                                                          new KeyGesture (Key.U, ModifierKeys.Control) });
        // RotateBreakpoint status (none / present / disabled)
        public static readonly RoutedUICommand RotateBreakpoint  = new RoutedUICommand ("RotateBreakpoint", "RotateBreakpoint", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.F9),
                                                                                          new KeyGesture (Key.B, ModifierKeys.Control) });
        public static readonly RoutedUICommand SingleStep        = new RoutedUICommand ("SingleStep", "SingleStep", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.F10),
                                                                                          new KeyGesture (Key.I, ModifierKeys.Control) });
        // Toggle simulation of System/3 CPU clock speed
        public static readonly RoutedUICommand ToggleCPUClock    = new RoutedUICommand ("ToggleCPUClock", "ToggleCPUClock", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.F11),
                                                                                          new KeyGesture (Key.P, ModifierKeys.Control) });
        public static readonly RoutedUICommand ToggleTrace       = new RoutedUICommand ("ToggleTrace", "ToggleTrace", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.F12),
                                                                                          new KeyGesture (Key.T, ModifierKeys.Control) });

        // Keystroke-only (no menu) commands
        public static readonly RoutedUICommand SystemReset       = new RoutedUICommand ("SystemReset", "SystemReset", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.Y, ModifierKeys.Control) });
        public static readonly RoutedUICommand EnterKey          = new RoutedUICommand ("EnterKey", "EnterKey", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.Enter) });
        public static readonly RoutedUICommand EscapeKey         = new RoutedUICommand ("EscapeKey", "EscapeKey", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.Escape) });
        public static readonly RoutedUICommand PauseKey          = new RoutedUICommand ("PauseKey", "PauseKey", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.Pause) });
        public static readonly RoutedUICommand HomeKey           = new RoutedUICommand ("HomeKey", "HomeKey", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.Home) });
        public static readonly RoutedUICommand EndKey            = new RoutedUICommand ("EndKey", "EndKey", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.End) });
        public static readonly RoutedUICommand PageUpKey         = new RoutedUICommand ("PageUpKey", "PageUpKey", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.PageUp) });
        public static readonly RoutedUICommand PageDownKey       = new RoutedUICommand ("PageDownKey", "PageDownKey", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.PageDown) });
        public static readonly RoutedUICommand UpArrowKey        = new RoutedUICommand ("UpArrowKey", "UpArrowKey", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.Up) });
        public static readonly RoutedUICommand DownArrowKey      = new RoutedUICommand ("DownArrowKey", "DownArrowKey", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.Down) });
        public static readonly RoutedUICommand F1Key             = new RoutedUICommand ("F1Key", "F1Key", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.F1, ModifierKeys.Alt) });
        public static readonly RoutedUICommand F2Key             = new RoutedUICommand ("F2Key", "F2Key", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.F2, ModifierKeys.Alt) });
        public static readonly RoutedUICommand F3Key             = new RoutedUICommand ("F3Key", "F3Key", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.F3, ModifierKeys.Alt) });
        //public static readonly RoutedUICommand Exit              = new RoutedUICommand ("Exit", "Exit", typeof (CCustomCommands), new InputGestureCollection ()
        //                                                                                { new KeyGesture (Key.F4, ModifierKeys.Alt) });
        public static readonly RoutedUICommand F4Key             = new RoutedUICommand ("F4Key", "F4Key", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.F4, ModifierKeys.Alt) });
        public static readonly RoutedUICommand F5Key             = new RoutedUICommand ("F5Key", "F5Key", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.F5, ModifierKeys.Alt) });
        public static readonly RoutedUICommand F6Key             = new RoutedUICommand ("F6Key", "F6Key", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.F6, ModifierKeys.Alt) });
        public static readonly RoutedUICommand F7Key             = new RoutedUICommand ("F7Key", "F7Key", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.F7, ModifierKeys.Alt) });
        public static readonly RoutedUICommand F8Key             = new RoutedUICommand ("F8Key", "F8Key", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.F8, ModifierKeys.Alt) });
        public static readonly RoutedUICommand F9Key             = new RoutedUICommand ("F9Key", "F9Key", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.F9, ModifierKeys.Alt) });
        public static readonly RoutedUICommand F10Key            = new RoutedUICommand ("F10Key", "F10Key", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.F10, ModifierKeys.Alt) });
        public static readonly RoutedUICommand F11Key            = new RoutedUICommand ("F11Key", "F11Key", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.F11, ModifierKeys.Alt) });
        public static readonly RoutedUICommand F12Key            = new RoutedUICommand ("F12Key", "F12Key", typeof (CCustomCommands), new InputGestureCollection ()
                                                                                        { new KeyGesture (Key.F12, ModifierKeys.Alt) });
    }
}