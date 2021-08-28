using System;
using System.Collections.Generic;
using System.IO;

namespace EmulatorEngine
{
    public abstract class CSystem3IOBase : CDataBaseReaderWriter
    {
        // System/3 main memory
        protected byte[] m_yaMainMemory = new byte[64 * ONE_K_BYTES];

        public virtual void DeviceLIO (byte yOpCode, byte yQByte)
        {
            FeatureNotImplemented ("***          DeviceLIO          ***");
            return;
        }

        public virtual void DeviceSIO (byte yOpCode, byte yQByte, byte yControlCode)
        {
            FeatureNotImplemented ("***          DeviceSIO          ***");
            return;
        }

        public virtual short DeviceSNS (byte yOpCode, byte yQByte)
        {
            FeatureNotImplemented ("***          DeviceSNS          ***");
            return 0;
        }

        public virtual bool DeviceTIO (byte yOpCode, byte yQByte)
        {
            InvalidQByte ("***     DeviceTIO      ***");
            return false;
        }

        public virtual void DeviceAPL (byte yOpCode, byte yQByte)
        {
            InvalidQByte ("***     DeviceAPL      ***");
            return;
        }

        protected void FeatureNotImplemented (string strMessage)
        {
            Console.WriteLine ("");
            Console.WriteLine ("***********************************");
            Console.WriteLine ("***********************************");
            Console.WriteLine ("***                             ***");
            Console.WriteLine (strMessage);
            Console.WriteLine ("***   Feature Not Implemented   ***");
            Console.WriteLine ("***                             ***");
            Console.WriteLine ("***********************************");
            Console.WriteLine ("***********************************");
            Console.WriteLine ("");
        }

        protected void InvalidQByte (string strMessage)
        {
            Console.WriteLine ("");
            Console.WriteLine ("**************************");
            Console.WriteLine ("**************************");
            Console.WriteLine ("***                    ***");
            Console.WriteLine (strMessage);
            Console.WriteLine ("***   Invalid Q Byte   ***");
            Console.WriteLine ("***                    ***");
            Console.WriteLine ("**************************");
            Console.WriteLine ("**************************");
            Console.WriteLine ("");
        }
    }

    public class C5424MFCU : CSystem3IOBase
    {
        CDataConversion objDataConversion = new CDataConversion ();

        // Configuration settings
        private int  m_iReadCPM                = 0;
        private int  m_iPunchPrintCPM          = 0;
        private int  m_iWaitMillisecRead       = 0;
        private int  m_iWaitMillisecPunchPrint = 0;
        private int  m_iWaitCyclesRead         = 0;
        private int  m_iWaitCyclesPunchPrint   = 0;

        // Filenames assigned to I/O functions
        //private string m_str5424PrimaryHopperFilename   = "";
        //private string m_str5424SecondaryHopperFilename = "";
        private string m_str5424Stacker1PunchFilename  = "";
        private string m_str5424Stacker2PunchFilename  = "";
        private string m_str5424Stacker3PunchFilename  = "";
        private string m_str5424Stacker4PunchFilename  = "";
        private string m_str5424Stacker1PrintFilename  = "";
        private string m_str5424Stacker2PrintFilename  = "";
        private string m_str5424Stacker3PrintFilename  = "";
        private string m_str5424Stacker4PrintFilename  = "";

        // File details
        private List<string> m_lstrPrimaryHopperReadFile   = new List<string> ();
        private List<string> m_lstrSecondaryHopperReadFile = new List<string> ();
        private List<string> m_lstrStacker1PunchFile       = new List<string> ();
        private List<string> m_lstrStacker2PunchFile       = new List<string> ();
        private List<string> m_lstrStacker3PunchFile       = new List<string> ();
        private List<string> m_lstrStacker4PunchFile       = new List<string> ();
        private List<string> m_lstrStacker1PrintFile       = new List<string> ();
        private List<string> m_lstrStacker2PrintFile       = new List<string> ();
        private List<string> m_lstrStacker3PrintFile       = new List<string> ();
        private List<string> m_lstrStacker4PrintFile       = new List<string> ();
        //private int  m_iPrimaryFileIdx   = 0;
        //private int  m_iSecondaryFileIdx = 0;

        // State veriables
        private bool m_bCardInPrimaryWaitStation       = false;
        private bool m_bCardInSecondaryWaitStation     = false;
        private bool m_bClearWaitStations              = false;
        private bool m_bResetStateOnTest               = true;
        private bool m_bPrimarySlashAsteriskReturned   = false;
        private bool m_bSecondarySlashAsteriskReturned = false;

        private int m_i5424ReadDataAddressRegister  = 0;
        public int i5424ReadDAR
        {
            get { return m_i5424ReadDataAddressRegister; }
            set { m_i5424ReadDataAddressRegister = value; }
        }

        private int m_i5424PunchDataAddressRegister = 0;
        public int i5424PunchDAR
        {
            get { return m_i5424PunchDataAddressRegister; }
            set { m_i5424PunchDataAddressRegister = value; }
        }

        private int m_i5424PrintDataAddressRegister = 0;
        public int i5424PrintDAR
        {
            get { return m_i5424PrintDataAddressRegister; }
            set { m_i5424PrintDataAddressRegister = value; }
        }

        private enum EMFCUStatusFlags
        {
            MFCU_Busy_Read_Feed      = 0x01,
            MFCU_Busy_Punch          = 0x02,
            MFCU_Busy_Print          = 0x04,
            MFCU_Primary_Not_Ready   = 0x08,
            MFCU_Secondary_Not_Ready = 0x16
        };
        private int m_iMFCUStatus = 0; // (int)EMFCUStatusFlags.MFCU_Primary_Not_Ready | (int)EMFCUStatusFlags.MFCU_Secondary_Not_Ready;

        public enum ELoadHopperMode
        {
            LOAD_OnlyIfEmpty,
            LOAD_ReplaceFile,
            LOAD_AppendFile
        }

        public void SetClearWaitStations ()   { m_bClearWaitStations = true; }
        public void ResetClearWaitStations () { m_bClearWaitStations = false; }

        public void SetPrimaryNotReady ()     { m_iMFCUStatus |=  (int)EMFCUStatusFlags.MFCU_Primary_Not_Ready; }
        public void SetSecondaryNotReady ()   { m_iMFCUStatus |=  (int)EMFCUStatusFlags.MFCU_Secondary_Not_Ready; }
        public void SetReadFeedBusy ()        { m_iMFCUStatus |=  (int)EMFCUStatusFlags.MFCU_Busy_Read_Feed; }
        public void SetPunchBusy ()           { m_iMFCUStatus |=  (int)EMFCUStatusFlags.MFCU_Busy_Punch; }
        public void SetPrintBusy ()           { m_iMFCUStatus |=  (int)EMFCUStatusFlags.MFCU_Busy_Print; }
        public void ResetPrimaryNotReady ()   { if (m_lstrPrimaryHopperReadFile.Count > 0)   m_iMFCUStatus &= ~(int)EMFCUStatusFlags.MFCU_Primary_Not_Ready; }
        public void ResetSecondaryNotReady () { if (m_lstrSecondaryHopperReadFile.Count > 0) m_iMFCUStatus &= ~(int)EMFCUStatusFlags.MFCU_Secondary_Not_Ready; }
        public void ResetReadFeedBusy ()      { m_iMFCUStatus &= ~(int)EMFCUStatusFlags.MFCU_Busy_Read_Feed; }
        public void ResetPunchBusy ()         { m_iMFCUStatus &= ~(int)EMFCUStatusFlags.MFCU_Busy_Punch; }
        public void ResetPrintBusy ()         { m_iMFCUStatus &= ~(int)EMFCUStatusFlags.MFCU_Busy_Print; }
        public void ResetAllMFCU ()           { m_iMFCUStatus  = 0; }

        public void NameOutputFiles ()
        {
            DateTime dtNow = DateTime.Now;
            string strSuffix = string.Format ("_{0:D4}-{1:D2}-{2:D2}_{3:D2}-{4:D2}-{5:D2}.txt",
                                              dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
            m_str5424Stacker1PunchFilename = "Stacker1Punch" + strSuffix;
            m_str5424Stacker2PunchFilename = "Stacker2Punch" + strSuffix;
            m_str5424Stacker3PunchFilename = "Stacker3Punch" + strSuffix;
            m_str5424Stacker4PunchFilename = "Stacker4Punch" + strSuffix;
            m_str5424Stacker1PrintFilename = "Stacker1Print" + strSuffix;
            m_str5424Stacker2PrintFilename = "Stacker2Print" + strSuffix;
            m_str5424Stacker3PrintFilename = "Stacker3Print" + strSuffix;
            m_str5424Stacker4PrintFilename = "Stacker4Print" + strSuffix;
        }

        public void WriteStringListsToOutputFiles (bool bAppendFile = false)
        {
            if (m_lstrStacker1PunchFile.Count > 0)
            {
                if (bAppendFile)
                {
                    File.AppendAllLines (m_str5424Stacker1PunchFilename, m_lstrStacker1PunchFile);
                }
                else
                {
                    File.WriteAllLines (m_str5424Stacker1PunchFilename, m_lstrStacker1PunchFile);
                }
            }

            if (m_lstrStacker2PunchFile.Count > 0)
            {
                if (bAppendFile)
                {
                    File.AppendAllLines (m_str5424Stacker2PunchFilename, m_lstrStacker2PunchFile);
                }
                else
                {
                    File.WriteAllLines (m_str5424Stacker2PunchFilename, m_lstrStacker2PunchFile);
                }
            }

            if (m_lstrStacker3PunchFile.Count > 0)
            {
                if (bAppendFile)
                {
                    File.AppendAllLines (m_str5424Stacker3PunchFilename, m_lstrStacker3PunchFile);
                }
                else
                {
                    File.WriteAllLines (m_str5424Stacker3PunchFilename, m_lstrStacker3PunchFile);
                }
            }

            if (m_lstrStacker4PunchFile.Count > 0)
            {
                if (bAppendFile)
                {
                    File.AppendAllLines (m_str5424Stacker4PunchFilename, m_lstrStacker4PunchFile);
                }
                else
                {
                    File.WriteAllLines (m_str5424Stacker4PunchFilename, m_lstrStacker4PunchFile);
                }
            }

            if (m_lstrStacker1PrintFile.Count > 0)
            {
                if (bAppendFile)
                {
                    File.AppendAllLines (m_str5424Stacker1PrintFilename, m_lstrStacker1PrintFile);
                }
                else
                {
                    File.WriteAllLines (m_str5424Stacker1PrintFilename, m_lstrStacker1PrintFile);
                }
            }

            if (m_lstrStacker2PrintFile.Count > 0)
            {
                if (bAppendFile)
                {
                    File.AppendAllLines (m_str5424Stacker2PrintFilename, m_lstrStacker2PrintFile);
                }
                else
                {
                    File.WriteAllLines (m_str5424Stacker2PrintFilename, m_lstrStacker2PrintFile);
                }
            }

            if (m_lstrStacker3PrintFile.Count > 0)
            {
                if (bAppendFile)
                {
                    File.AppendAllLines (m_str5424Stacker3PrintFilename, m_lstrStacker3PrintFile);
                }
                else
                {
                    File.WriteAllLines (m_str5424Stacker3PrintFilename, m_lstrStacker3PrintFile);
                }
            }

            if (m_lstrStacker4PrintFile.Count > 0)
            {
                if (bAppendFile)
                {
                    File.AppendAllLines (m_str5424Stacker4PrintFilename, m_lstrStacker4PrintFile);
                }
                else
                {
                    File.WriteAllLines (m_str5424Stacker4PrintFilename, m_lstrStacker4PrintFile);
                }
            }
        }

        public string Stacker1PunchFilename
        {
            get { return m_str5424Stacker1PunchFilename; }
            set { m_str5424Stacker1PunchFilename = value; }
        }

        public string Stacker2PunchFilename
        {
            get { return m_str5424Stacker2PunchFilename; }
            set { m_str5424Stacker2PunchFilename = value; }
        }

        public string Stacker3PunchFilename
        {
            get { return m_str5424Stacker3PunchFilename; }
            set { m_str5424Stacker3PunchFilename = value; }
        }

        public string Stacker4PunchFilename
        {
            get { return m_str5424Stacker4PunchFilename; }
            set { m_str5424Stacker4PunchFilename = value; }
        }

        public string Stacker1PrintFilename
        {
            get { return m_str5424Stacker1PrintFilename; }
            set { m_str5424Stacker1PrintFilename = value; }
        }

        public string Stacker2PrintFilename
        {
            get { return m_str5424Stacker2PrintFilename; }
            set { m_str5424Stacker2PrintFilename = value; }
        }

        public string Stacker3PrintFilename
        {
            get { return m_str5424Stacker3PrintFilename; }
            set { m_str5424Stacker3PrintFilename = value; }
        }

        public string Stacker4PrintFilename
        {
            get { return m_str5424Stacker4PrintFilename; }
            set { m_str5424Stacker4PrintFilename = value; }
        }

        public void ClearPrimaryHopper ()
            {
                m_lstrPrimaryHopperReadFile.Clear ();
                SetPrimaryNotReady ();
            }

        public void ClearSecondaryHopper ()
        {
            m_lstrSecondaryHopperReadFile.Clear ();
            SetSecondaryNotReady ();
        }

        public bool IsPrimaryNotReady ()
        {
            bool bReturn = (m_iMFCUStatus & (int)EMFCUStatusFlags.MFCU_Primary_Not_Ready) > 0;

            if (m_bResetStateOnTest)
            {
                ResetPrimaryNotReady ();
            }

            return bReturn;
        }

        public bool IsSecondaryNotReady ()
        {
            bool bReturn = (m_iMFCUStatus & (int)EMFCUStatusFlags.MFCU_Primary_Not_Ready) > 0;

            if (m_bResetStateOnTest)
            {
                ResetSecondaryNotReady ();
            }

            return bReturn;
        }

        public bool IsReadFeedBusy ()
        {
            bool bReturn = (m_iMFCUStatus & (int)EMFCUStatusFlags.MFCU_Busy_Read_Feed) > 0;

            if (m_bResetStateOnTest)
            {
                ResetReadFeedBusy ();
            }

            return bReturn;
        }

        public bool IsPunchBusy ()
        {
            bool bReturn = (m_iMFCUStatus & (int)EMFCUStatusFlags.MFCU_Busy_Punch) > 0;

            if (m_bResetStateOnTest)
            {
                ResetPunchBusy ();
            }

            return bReturn;
        }

        public bool IsPrintBusy ()
        {
            bool bReturn = (m_iMFCUStatus & (int)EMFCUStatusFlags.MFCU_Busy_Print) > 0;

            if (m_bResetStateOnTest)
            {
                ResetPrintBusy ();
            }

            return bReturn;
        }

        #region Timing Simulation
        public int ReadCPM
        {
            get { return m_iReadCPM; }
            set { m_iReadCPM = value; }
        }

        public int PunchPrintCPM
        {
            get { return m_iPunchPrintCPM; }
            set { m_iPunchPrintCPM = value; }
        }

        public int WaitMillisecRead
        {
            get { return m_iWaitMillisecRead; }
            set { m_iWaitMillisecRead = value; }
        }

        public int WaitMillisecPunchPrint
        {
            get { return m_iWaitMillisecPunchPrint; }
            set { m_iWaitMillisecPunchPrint = value; }
        }

        public int WaitCyclesRead
        {
            get { return m_iWaitCyclesRead; }
            set { m_iWaitCyclesRead = value; }
        }

        public int WaitCyclesPunchPrint
        {
            get { return m_iWaitCyclesPunchPrint; }
            set { m_iWaitCyclesPunchPrint = value; }
        }
        #endregion

        public bool IsCardInPrimaryWaitStation ()
        {
            return m_bCardInPrimaryWaitStation;
        }

        public bool IsCardInSecondaryWaitStation ()
        {
            return m_bCardInSecondaryWaitStation;
        }

        //public bool IsPrimaryHopperEmpty ()
        //{
        //    return m_lstrPrimaryFile.Count == 0;
        //}

        //public bool IsSecondaryHopperEmpty ()
        //{
        //    return m_lstrSecondaryFile.Count == 0;
        //}

        public void LoadPrimaryHopperBlankCard ()
        {
            List<string> lstrSingleCard = new List<string> ();
            lstrSingleCard.Add (new string (' ', 96));
            LoadPrimaryHopperFile (lstrSingleCard, ELoadHopperMode.LOAD_OnlyIfEmpty);
        }

        public void LoadSecondaryHopperBlankCard ()
        {
            List<string> lstrSingleCard = new List<string> ();
            lstrSingleCard.Add (new string (' ', 96));
            LoadSecondaryHopperFile (lstrSingleCard, ELoadHopperMode.LOAD_OnlyIfEmpty);
        }

        public void LoadPrimaryHopperFile (string strFilename, ELoadHopperMode eLoadHopperMode = ELoadHopperMode.LOAD_OnlyIfEmpty)
        {
            LoadPrimaryHopperFile (objDataConversion.ReadFileToStringList (strFilename, 96), eLoadHopperMode);
        }

        public void LoadPrimaryHopperFile (List<string> lstrPrimary, ELoadHopperMode eLoadHopperMode = ELoadHopperMode.LOAD_OnlyIfEmpty)
        {
            if (eLoadHopperMode == ELoadHopperMode.LOAD_ReplaceFile)
            {
                m_lstrPrimaryHopperReadFile.Clear ();
                m_lstrPrimaryHopperReadFile.AddRange (lstrPrimary);
            }
            else if (eLoadHopperMode == ELoadHopperMode.LOAD_OnlyIfEmpty)
            {
                if (m_lstrPrimaryHopperReadFile.Count == 0)
                {
                    m_lstrPrimaryHopperReadFile.AddRange (lstrPrimary);
                }
            }
            else // if (eLoadHopperMode == ELoadHopperMode.LOAD_AppendFile)
            {
                m_lstrPrimaryHopperReadFile.AddRange (lstrPrimary);
            }

            if (m_lstrPrimaryHopperReadFile.Count > 0)
            {
                ResetPrimaryNotReady ();
            }

            m_bPrimarySlashAsteriskReturned = false;
        }

        public void LoadSecondaryHopperFile (string strFilename, ELoadHopperMode eLoadHopperMode = ELoadHopperMode.LOAD_OnlyIfEmpty)
        {
            LoadSecondaryHopperFile (objDataConversion.ReadFileToStringList (strFilename, 96), eLoadHopperMode);
        }

        public void LoadSecondaryHopperFile (List<string> lstrSecondary, ELoadHopperMode eLoadHopperMode = ELoadHopperMode.LOAD_OnlyIfEmpty)
        {
            if (eLoadHopperMode == ELoadHopperMode.LOAD_ReplaceFile)
            {
                m_lstrSecondaryHopperReadFile.Clear ();
                m_lstrSecondaryHopperReadFile.AddRange (lstrSecondary);
            }
            else if (eLoadHopperMode == ELoadHopperMode.LOAD_OnlyIfEmpty)
            {
                if (m_lstrSecondaryHopperReadFile.Count == 0)
                {
                    m_lstrSecondaryHopperReadFile.AddRange (lstrSecondary);
                }
            }
            else // if (eLoadHopperMode == ELoadHopperMode.LOAD_AppendFile)
            {
                m_lstrSecondaryHopperReadFile.AddRange (lstrSecondary);
            }

            if (m_lstrPrimaryHopperReadFile.Count > 0)
            {
                ResetSecondaryNotReady ();
            }

            m_bSecondarySlashAsteriskReturned = false;
        }

        //public bool IsPrimaryEndOfFile ()
        //{
        //    return (m_iPrimaryFileIdx + 1 >= m_strlPrimaryFile.Count);
        //}

        //public bool IsSecondaryEndOfFile ()
        //{
        //    return (m_iSecondaryFileIdx + 1 >= m_strlSecondaryFile.Count);
        //}

        public bool IsPrimaryHopperEmpty ()
        {
            if (m_lstrPrimaryHopperReadFile.Count == 0)
            {
                SetPrimaryNotReady ();
            }

            return m_lstrPrimaryHopperReadFile.Count == 0;
        }

        public bool IsSecondaryHopperEmpty ()
        {
            if (m_lstrSecondaryHopperReadFile.Count == 0)
            {
                SetSecondaryNotReady ();
            }

            return m_lstrSecondaryHopperReadFile.Count == 0;
        }

        public string ReadCardFromPrimary ()
        {
            string strReturn = null;

            if (m_lstrPrimaryHopperReadFile.Count > 0)
            {
                m_bCardInPrimaryWaitStation = !m_bClearWaitStations;
                strReturn = objDataConversion.ConvertASCIIstringToEBCDIC (m_lstrPrimaryHopperReadFile[0]);
                m_lstrPrimaryHopperReadFile.RemoveAt (0);
            }
            else if (!m_bPrimarySlashAsteriskReturned)
            {
                m_bPrimarySlashAsteriskReturned = true;
                SetPrimaryNotReady ();
                return null; // objDataConversion.ConvertASCIIstringToEBCDIC ("/*"); // System/3 End-of-file marker
            }

            SetPrimaryNotReady ();
            return strReturn;
        }

        public byte[] ReadCardFromPrimaryIPL ()
        {
            if (m_lstrPrimaryHopperReadFile.Count > 0)
            {
                m_bCardInPrimaryWaitStation = !m_bClearWaitStations;
                string strPrimaryCardImage  = m_lstrPrimaryHopperReadFile[0];
                m_lstrPrimaryHopperReadFile.RemoveAt (0);
                if (m_lstrPrimaryHopperReadFile.Count == 0)
                {
                    SetPrimaryNotReady ();
                }
                return objDataConversion.ReadIPLCard (strPrimaryCardImage);
            }
            else
            {
                SetPrimaryNotReady ();
                return new byte[96];
            }
        }

        public string ReadCardFromSecondary ()
        {
            string strReturn = "";

            if (m_lstrSecondaryHopperReadFile.Count > 0)
            {
                m_bCardInSecondaryWaitStation = !m_bClearWaitStations;
                strReturn = objDataConversion.ConvertASCIIstringToEBCDIC (m_lstrSecondaryHopperReadFile[0]);
                m_lstrSecondaryHopperReadFile.RemoveAt (0);
            }
            else if (!m_bSecondarySlashAsteriskReturned)
            {
                m_bSecondarySlashAsteriskReturned = true;
                SetSecondaryNotReady ();
                return null; // objDataConversion.ConvertASCIIstringToEBCDIC ("/*"); // System/3 End-of-file marker
            }

            SetSecondaryNotReady ();
            return strReturn;
        }

        public byte[] ReadCardFromSecondaryIPL ()
        {
            if (m_lstrSecondaryHopperReadFile.Count > 0)
            {
                m_bCardInSecondaryWaitStation = !m_bClearWaitStations;
                string strSecondaryCardImage = m_lstrSecondaryHopperReadFile[0];
                m_lstrSecondaryHopperReadFile.RemoveAt (0);
                if (m_lstrSecondaryHopperReadFile.Count == 0)
                {
                    SetSecondaryNotReady ();
                }
                return objDataConversion.ReadIPLCard (strSecondaryCardImage);
            }
            else
            {
                SetSecondaryNotReady ();
                return new byte[96];
            }
        }

        public void WritePunchOutput (int iStacker, string strOutput)
        {
            if (iStacker == 1)
            {
                m_lstrStacker1PunchFile.Add (strOutput);
            }
            else if (iStacker == 2)
            {
                m_lstrStacker2PunchFile.Add (strOutput);
            }
            else if (iStacker == 3)
            {
                m_lstrStacker3PunchFile.Add (strOutput);
            }
            else if (iStacker == 4)
            {
                m_lstrStacker4PunchFile.Add (strOutput);
            }
        }

        public void WritePrintOutput (int iStacker, string strOutput)
        {
            if (iStacker == 1)
            {
                m_lstrStacker1PrintFile.Add (strOutput);
            }
            else if (iStacker == 2)
            {
                m_lstrStacker2PrintFile.Add (strOutput);
            }
            else if (iStacker == 3)
            {
                m_lstrStacker3PrintFile.Add (strOutput);
            }
            else if (iStacker == 4)
            {
                m_lstrStacker4PrintFile.Add (strOutput);
            }
        }

        public void NonProcessRunOut ()
        {
            m_bCardInPrimaryWaitStation   = false;
            m_bCardInSecondaryWaitStation = false;
        }

        public override void DeviceLIO (byte yOpCode, byte yQByte) { }
        public override void DeviceSIO (byte yOpCode, byte yQByte, byte yControlCode) { }
        public override bool DeviceTIO (byte yOpCode, byte yQByte) { return false; }
        public override void DeviceAPL (byte yOpCode, byte yQByte) { }
        public override short DeviceSNS (byte yOpCode, byte yQByte) { return 0; }
    }

    class C5203LinePrinter : CSystem3IOBase
    {
        //private bool m_bPrintBufferBusy     = false;
        //private bool m_bPrinterCarriageBusy = false;

        private enum EPrinterStatusFlags
        {
            PRINTER_Print_Buffer_Busy = 0x01,
            PRINTER_Carriage_Busy     = 0x02
        };

        private int  m_i5203FormsLengthRegister       = 0;
        private int  m_iLinePosition                  = 1;
        private int  m_iWaitMillisecLinePrint         = 0;  // (60 sec * 1000) / SpeedLPM
        private int  m_iWaitMillisecLineSkip          = 0;  // m_iWaitMillisecLinePrint / 10
        private int  m_iWaitMillisecLineFeed          = 0;  // m_iWaitMillisecLinePrint / 15
        private int  m_iSpeedLPM                      = 1000;
        private int  m_iWaitCyclesLinePrint           = 0;
        private int  m_iWaitCyclesLineSkip            = 0;
        private int  m_iWaitCyclesLineFeed            = 0;
        private int  m_iStatusBytes                   = 0x04; // Set 48-character chain status bit
        private int  m_i5203LineWidth                 = 132;
        private int  m_i5203DataAddressRegister       = 0x0000;
        private int  m_i5203ChainImageAddressRegister = 0x0000;
        private int  m_iPrinterStatus                 = 0;
        private bool m_bResetStateOnTest              = true;
        string m_strChainImage                        = "";

        public int i5203LineWidth
        {
            get { return m_i5203LineWidth; }
        }

        public int i5203PrintDAR
        {
            get { return m_i5203DataAddressRegister; }
            set { m_i5203DataAddressRegister = value; }
        }

        public int i5203ChainImageAddressRegister
        {
            get { return m_i5203ChainImageAddressRegister; }
            set { m_i5203ChainImageAddressRegister = value; }
        }

        public void SetPrintBufferBusy ()   { m_iPrinterStatus |=  (int)EPrinterStatusFlags.PRINTER_Print_Buffer_Busy; }
        public void SetCarriageBusy ()      { m_iPrinterStatus |=  (int)EPrinterStatusFlags.PRINTER_Carriage_Busy; }
        public void ResetPrintBufferBusy () { m_iPrinterStatus &= ~(int)EPrinterStatusFlags.PRINTER_Print_Buffer_Busy; }
        public void ResetCarriageBusy ()    { m_iPrinterStatus &= ~(int)EPrinterStatusFlags.PRINTER_Carriage_Busy; }
        public void ResetAllPrinter ()      { m_iPrinterStatus  = 0; }

        public bool IsPrintBufferBusy ()
        {
            bool bReturn = (m_iPrinterStatus & (int)EPrinterStatusFlags.PRINTER_Print_Buffer_Busy) > 0;

            if (m_bResetStateOnTest)
            {
                ResetPrintBufferBusy ();
            }

            return bReturn;
        }
        public bool IsCarriageBusy ()
        {
            bool bReturn = (m_iPrinterStatus & (int)EPrinterStatusFlags.PRINTER_Carriage_Busy) > 0;

            if (m_bResetStateOnTest)
            {
                ResetCarriageBusy ();
            }

            return bReturn;
        }

        public void SetPrinterWidth96 ()         { m_i5203LineWidth = 96; }
        public void SetPrinterWidth132 ()        { m_i5203LineWidth = 132; }

        public void ResetLinePosition ()         { m_iLinePosition = 1; }
        public void IncrementLinePosition ()     { ++m_iLinePosition; }
        public int  GetLinePosition ()           { return m_iLinePosition; }

        public void SetUnprintableCharacter ()   { m_iStatusBytes |= 0x20; }
        public void ResetUnprintableCharacter () { m_iStatusBytes &= ~0x20; }
        public bool GetUnprintableCharacter ()   { return (m_iStatusBytes & 0x20) == 0x20; }
        public void ClearStatusFlags ()          { m_iStatusBytes = 0x04; }
        public int  GetStatusBytes ()            { return m_iStatusBytes; }

        public int FormLength
        {
            get { return m_i5203FormsLengthRegister; }
            set { m_i5203FormsLengthRegister = value >> 8; } // Ignore right carriage length
        }

        public int LinePosition
        {
            get { return m_iLinePosition; }
            set { m_iLinePosition = value; }
        }

        #region Timing Simulation
        public int WaitMillisecLinePrint
        {
            get { return m_iWaitMillisecLinePrint; }
            set { m_iWaitMillisecLinePrint = value; }
        }

        public int WaitMillisecLineSkip
        {
            get { return m_iWaitMillisecLineSkip; }
            set { m_iWaitMillisecLineSkip = value; }
        }

        public int WaitMillisecLineFeed
        {
            get { return m_iWaitMillisecLineFeed; }
            set { m_iWaitMillisecLineFeed = value; }
        }

        public int SpeedLPM
        {
            get { return m_iSpeedLPM; }
            set { m_iSpeedLPM = value; }
        }

        public int WaitCyclesLinePrint
        {
            get { return m_iWaitCyclesLinePrint; }
            set { m_iWaitCyclesLinePrint = value; }
        }

        public int WaitCyclesLineSkip
        {
            get { return m_iWaitCyclesLineSkip; }
            set { m_iWaitCyclesLineSkip = value; }
        }

        public int WaitCyclesLineFeed
        {
            get { return m_iWaitCyclesLineFeed; }
            set { m_iWaitCyclesLineFeed = value; }
        }
        #endregion

        public string ChainImage
        {
            get { return m_strChainImage; }
            set { m_strChainImage = value; }
        }

        public bool IsInChainImage (char cTest) // Expects char to be an ASCII value
        {
            int iCharIdx = m_strChainImage.IndexOf (cTest);
            return (iCharIdx >= 0 && iCharIdx < m_strChainImage.Length);
        }

        public void PrintLine (int iLinesToSkip, int iDestinationLine)
        {
            //m_bPrintBufferBusy = true;

            // Write output
            // Time to simulate busy time
            //m_bPrintBufferBusy = false;

            if (iLinesToSkip > 0)
            {
                SkipLines (iLinesToSkip);
            }
            else if (iDestinationLine > 0)
            {
                SkipToLine (iDestinationLine);
            }
        }

        public void SkipLines (int iLinesToSkip)
        {
            //m_bPrinterCarriageBusy = true;

            // Time to simulate busy time

            m_iLinePosition += iLinesToSkip;
            if (m_iLinePosition > m_i5203FormsLengthRegister)
            {
                m_iLinePosition -= m_i5203FormsLengthRegister;
            }

            //m_bPrinterCarriageBusy = false;
        }
        
        public int SkipToLine (int iDestinationLine)
        {
            //m_bPrinterCarriageBusy = true;

            int iLinesToSkip = 0;

            if (iDestinationLine > m_iLinePosition)
            {
                iLinesToSkip = iDestinationLine - m_iLinePosition;
            }
            else if (iDestinationLine < m_iLinePosition)
            {
                if (m_i5203FormsLengthRegister > m_iLinePosition)
                {
                    iLinesToSkip = m_i5203FormsLengthRegister - m_iLinePosition;
                }
                else
                {
                    iLinesToSkip = 1; // Default value if no forms length has been loaded
                }
            }
            else // iDestinationLine == m_iLinePosition
            {
                return m_i5203FormsLengthRegister;
            }

            m_iLinePosition = iDestinationLine;

            //m_bPrinterCarriageBusy = false;

            return iLinesToSkip;
        }

        public override void DeviceLIO (byte yOpCode, byte yQByte) { }
        public override void DeviceSIO (byte yOpCode, byte yQByte, byte yControlCode) { }
        public override bool DeviceTIO (byte yOpCode, byte yQByte) { return false; }
        public override void DeviceAPL (byte yOpCode, byte yQByte) { }
        public override short DeviceSNS (byte yOpCode, byte yQByte) { return 0; }
    }

    class C5444DiskDrive : CSystem3IOBase
    {
        // p. 6-5 (Components Reference Manual)
        // - 256 bytes per sector
        // - 48 sectors per cylinder
        //   sectors numbered  0 - 23 for upper surface
        //   sectors numbered 32 - 55 for lower surface
        //
        // - Local storage registers for disk
        //  > Disk Control Field: 4 bytes (pointed to by Disk Control Address Register)
        //    1: F  Specifies disk operation
        //    2: C  Cylinder number
        //    3: S  Sector number (updated with each sector read to always indicate the
        //          next sector to be read)
        //          After I/O operation, points to last sector processed
        //    4: N  Number of cylinders for seek operation
        //          Number of sectors for all other operations (0-based: 0 indicates 1 sector)
        //    p. 6-12 explains the values after a completed I/O operation
        //    After reading, the F, C, and S bytes are overwritten by the sector ID values of
        //      the last sector read; N is not changed.
        //   After reading, the Disk Control Address Register's value is unchanged.
        //   > Disk Read/Write Address Register points to the low-address byte of data buffer
        //     (Also updated during reading multiple sectors to always point to the next byte
        //     that would be read to)
        //
        // - Reading details
        //   Up to 48 sectors can be read, but no more than 24 should be read to avoid errors.
        //   Multiple-sector reads can't span cylinders, so if the first sector to be read is
        //     not #0, then the max number of sectors that can be read is reduced by the
        //     number of the first sector to be read.
        //   IPL reads sector 0 from cylinder 0 to main memory beginning at 0x0000, then
        //     execution begins at address 0x0000.

        const int  SECTORS_PER_CYLINDER = 48;
        const int  BYTES_PER_SECTOR     = 256;
        const bool FIXED      = true;
        const bool REMOVABLE  = false;
        const bool PRIMARY    = true;
        const bool SECONDARY  = false;

        private bool m_bFixedPrimaryWrittenTo       = false;
        private bool m_bFixedSecondaryWrittenTo     = false;
        private bool m_bRemovablePrimaryWrittenTo   = false;
        private bool m_bRemovableSecondaryWrittenTo = false;

        byte[] m_yaDiskImageFixed1     = null;
        byte[] m_yaDiskImageFixed2     = null;
        byte[] m_yaDiskImageRemovable1 = null;
        byte[] m_yaDiskImageRemovable2 = null;

        public void SetWaitTime (int iMilliseconds) { }
        public void SetCycleTime (int iBusyTestCount) { }

        public bool LoadDiskImageFromToken (CDBFileToken ftLoadImage, bool bFixed, bool bPrimary)
        {
            if      (bFixed && bPrimary)
            {
                m_yaDiskImageFixed1 = ReadBinaryFromToken (ftLoadImage.FileTokenKey);
                return m_yaDiskImageFixed1.Length > 0;
            }
            else if (!bFixed && bPrimary)
            {
                m_yaDiskImageRemovable1 = ReadBinaryFromToken (ftLoadImage.FileTokenKey);
                return m_yaDiskImageRemovable1.Length > 0;
            }
            else if (bFixed && !bPrimary)
            {
                m_yaDiskImageFixed2 = ReadBinaryFromToken (ftLoadImage.FileTokenKey);
                return m_yaDiskImageFixed2.Length > 0;
            }
            else if (!bFixed && !bPrimary)
            {
                m_yaDiskImageRemovable2 = ReadBinaryFromToken (ftLoadImage.FileTokenKey);
                return m_yaDiskImageRemovable1.Length > 0;
            }

            return false;
        }

        public bool LoadDiskImageFromFile (string strFilename, bool bFixed, bool bPrimary)
        {
            if (File.Exists (strFilename))
            {
                if      (bFixed && bPrimary)
                {
                    m_yaDiskImageFixed1 = File.ReadAllBytes (strFilename);
                    return m_yaDiskImageFixed1.Length > 0;
                }
                else if (!bFixed && bPrimary)
                {
                    m_yaDiskImageRemovable1 = File.ReadAllBytes (strFilename);
                    return m_yaDiskImageRemovable1.Length > 0;
                }
                else if (bFixed && !bPrimary)
                {
                    m_yaDiskImageFixed2 = File.ReadAllBytes (strFilename);
                    return m_yaDiskImageFixed2.Length > 0;
                }
                else if (!bFixed && !bPrimary)
                {
                    m_yaDiskImageRemovable2 = File.ReadAllBytes (strFilename);
                    return m_yaDiskImageRemovable1.Length > 0;
                }
            }

            return false;
        }

        public void WriteAllDirtyImages ()
        {
            if (m_bFixedPrimaryWrittenTo)
            {
                WriteDiskImage (FIXED, PRIMARY);
            }

            if (m_bRemovablePrimaryWrittenTo)
            {
                WriteDiskImage (REMOVABLE, PRIMARY);
            }

            if (m_bFixedSecondaryWrittenTo)
            {
                WriteDiskImage (FIXED, SECONDARY);
            }

            if (m_bRemovableSecondaryWrittenTo)
            {
                WriteDiskImage (REMOVABLE, SECONDARY);
            }
        }

        public bool WriteDiskImage (bool bFixed, bool bPrimary)
        {
            bool bFailure = false,
                 bSuccess = false;

            while (!bSuccess)
            {
                Console.WriteLine ("");
                Console.Write ("Enter filename for "                  +
                               (bPrimary ? "primary " : "secondary ") +
                               (bFixed ? "fixed" : "removeable")      +
                               " image: ");
                Console.CursorSize = 10;
                string strFilename = Console.ReadLine ();
                try
                {
                    WriteDiskImageToFile (strFilename, bFixed, bPrimary);
                    bSuccess = true;
                    bFailure = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine (e.Message);
                    bFailure = true;
                }

                if (bFailure)
                {
                    Console.Write ("Write operation failed.  Try again? Y/N");
                    ConsoleKeyInfo cki = Console.ReadKey ();
                    if (cki.KeyChar != 'Y' &&
                        cki.KeyChar != 'y')
                    {
                        return false;
                    }
                    Console.WriteLine ("");
                }
            }

            return bSuccess;
        }

        public void WriteDiskImageToFile (string strFilename, bool bFixed, bool bPrimary)
        {
            if      (bFixed && bPrimary)
            {
                if (m_yaDiskImageFixed1.Length > 0)
                {
                    File.WriteAllBytes (strFilename, m_yaDiskImageFixed1);
                }
                m_bFixedPrimaryWrittenTo = false;
            }
            else if (!bFixed && bPrimary)
            {
                if (m_yaDiskImageRemovable1.Length > 0)
                {
                    File.WriteAllBytes (strFilename, m_yaDiskImageRemovable1);
                }
                m_bRemovablePrimaryWrittenTo = false;
            }
            else if (bFixed && !bPrimary)
            {
                if (m_yaDiskImageFixed2.Length > 0)
                {
                    File.WriteAllBytes (strFilename, m_yaDiskImageFixed2);
                }
                m_bFixedSecondaryWrittenTo = false;
            }
            else if (!bFixed && !bPrimary)
            {
                if (m_yaDiskImageRemovable2.Length > 0)
                {
                    File.WriteAllBytes (strFilename, m_yaDiskImageRemovable2);
                }
                m_bRemovableSecondaryWrittenTo = false;
            }
        }

        public byte[] ReadSector (int iFileOffset, bool bFixed, bool bPrimary)
        {
            byte[] yaSector = new byte[BYTES_PER_SECTOR];

            if      (bFixed && bPrimary)
            {
                for (int iIdx = 0; iIdx < BYTES_PER_SECTOR; ++iIdx)
                {
                    yaSector[iIdx] = m_yaDiskImageFixed1[iFileOffset + iIdx];
                }
            }
            else if (!bFixed && bPrimary)
            {
                for (int iIdx = 0; iIdx < BYTES_PER_SECTOR; ++iIdx)
                {
                    byte yTest = m_yaDiskImageRemovable1[iFileOffset + iIdx];
                    yaSector[iIdx] = yTest;
                }
            }
            else if (bFixed && !bPrimary)
            {
                for (int iIdx = 0; iIdx < BYTES_PER_SECTOR; ++iIdx)
                {
                    yaSector[iIdx] = m_yaDiskImageFixed2[iFileOffset + iIdx];
                }
            }
            else if (!bFixed && !bPrimary)
            {
                for (int iIdx = 0; iIdx < BYTES_PER_SECTOR; ++iIdx)
                {
                    yaSector[iIdx] = m_yaDiskImageRemovable2[iFileOffset + iIdx];
                }
            }

            return yaSector;
        }

        public void WriteSector (int iFileOffset, bool bFixed, bool bPrimary, byte[] yaSector)
        {
            if      (bFixed && bPrimary)
            {
                for (int iIdx = 0; iIdx < BYTES_PER_SECTOR; ++iIdx)
                {
                    m_yaDiskImageFixed1[iFileOffset + iIdx] = yaSector[iIdx];
                }
                m_bFixedPrimaryWrittenTo = true;
            }
            else if (!bFixed && bPrimary)
            {
                for (int iIdx = 0; iIdx < BYTES_PER_SECTOR; ++iIdx)
                {
                    m_yaDiskImageRemovable1[iFileOffset + iIdx] = yaSector[iIdx];
                }
                m_bRemovablePrimaryWrittenTo = true;
            }
            else if (bFixed && !bPrimary)
            {
                for (int iIdx = 0; iIdx < BYTES_PER_SECTOR; ++iIdx)
                {
                    m_yaDiskImageFixed2[iFileOffset + iIdx] = yaSector[iIdx];
                }
                m_bFixedSecondaryWrittenTo = true;
            }
            else if (!bFixed && !bPrimary)
            {
                for (int iIdx = 0; iIdx < BYTES_PER_SECTOR; ++iIdx)
                {
                    m_yaDiskImageRemovable2[iFileOffset + iIdx] = yaSector[iIdx];
                }
                m_bRemovableSecondaryWrittenTo = true;
            }
        }

        public int AddressFromCylinderAndSector (int iCylinder, int iSector)
        {
            int iAddress = iCylinder * SECTORS_PER_CYLINDER + BYTES_PER_SECTOR;
            iAddress += ConvertSectorNumber (iSector);

            return iAddress;
        }

        public int ConvertSectorNumber (int iS3Sector)
        {
            if (iS3Sector >= 0 &&
                iS3Sector <= 23)
            {
                return iS3Sector;
            }
            else if (iS3Sector >= 32 &&
                     iS3Sector <= 55)
            {
                return iS3Sector - 8;
            }
            else
            {
                return -1;
            }
        }

        public byte[] ReadFixedDriveBootSector ()
        {
            byte[] yaBootSector = new byte[256];
            for (int iIdx = 0; iIdx < 256; ++iIdx)
            {
                yaBootSector[iIdx] = m_yaDiskImageFixed1[iIdx];
            }

            return yaBootSector;
        }

        public byte[] ReadRemovableDriveBootSector ()
        {
            byte[] yaBootSector = new byte[256];
            for (int iIdx = 0; iIdx < 256; ++iIdx)
            {
                yaBootSector[iIdx] = m_yaDiskImageRemovable1[iIdx];
            }

            return yaBootSector;
        }

        public override void DeviceLIO (byte yOpCode, byte yQByte) { }
        public override void DeviceSIO (byte yOpCode, byte yQByte, byte yControlCode) { }
        public override bool DeviceTIO (byte yOpCode, byte yQByte) { return false; }
        public override void DeviceAPL (byte yOpCode, byte yQByte) { }
        public override short DeviceSNS (byte yOpCode, byte yQByte) { return 0; }
    }

    class C5475DataEntryKeyboard : CSystem3IOBase
    {
        public override void DeviceLIO (byte yOpCode, byte yQByte) { }
        public override void DeviceSIO (byte yOpCode, byte yQByte, byte yControlCode) { }
        public override short DeviceSNS (byte yOpCode, byte yQByte) { return 0; }
    }

    class C5471KeyboardPrinter : CSystem3IOBase
    {
        public override void DeviceLIO (byte yOpCode, byte yQByte) { }
        public override void DeviceSIO (byte yOpCode, byte yQByte, byte yControlCode) { }
        public override short DeviceSNS (byte yOpCode, byte yQByte) { return 0; }
    }
}
