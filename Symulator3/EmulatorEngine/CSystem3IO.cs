using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EmulatorEngine
{
    class CSystem3IOBase
    {
        public virtual void ExecuteLIO (byte[] yaInstruction)
        {
            FeatureNotImplemented ("***         ExecuteLIO          ***");
            return;
        }

        public virtual void ExecuteSIO (byte[] yaInstruction)
        {
            FeatureNotImplemented ("***         ExecuteSIO          ***");
            return;
        }

        public virtual void ExecuteSNS (byte[] yaInstruction)
        {
            FeatureNotImplemented ("***         ExecuteSNS          ***");
            return;
        }

        public virtual void ExecuteTIO (byte[] yaInstruction)
        {
            InvalidQByte ("***     ExecuteTIO     ***");
            return;
        }

        public virtual void ExecuteAPL (byte[] yaInstruction)
        {
            InvalidQByte ("***     ExecuteAPL     ***");
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

    class C5242MFCU : CSystem3IOBase
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
        private string m_str5424PrimaryHopperFilename = "";
        private string m_str5424SecondaryHopperFilename = "";
        private string m_str5424Stacker1OutputFilename = "";
        private string m_str5424Stacker2OutputFilename = "";
        private string m_str5424Stacker3OutputFilename = "";
        private string m_str5424Stacker4OutputFilename = "";

        // File details
        private List<string> m_strlPrimaryFile = new List<string> ();
        private List<string> m_strlSecondaryFile = new List<string> ();
        private int  m_iPrimaryFileIdx   = 0;
        private int  m_iSecondaryFileIdx = 0;

        // State veriables
        private bool m_bCardInPrimaryWaitStation   = false;
        private bool m_bCardInSecondaryWaitStation = false;

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

        public bool IsCardInPrimaryWaitStation ()
        {
            return m_bCardInPrimaryWaitStation;
        }

        public bool IsCardInSecondaryWaitStation ()
        {
            return m_bCardInSecondaryWaitStation;
        }

        public void LoadPrimaryHopperFile (string strFilename)
        {
            m_str5424PrimaryHopperFilename = strFilename;
            m_strlPrimaryFile = objDataConversion.ReadFileToStringList (strFilename, 96);
            m_iPrimaryFileIdx = 0;
        }

        public void LoadSecondaryHopperFile (string strFilename)
        {
            m_str5424SecondaryHopperFilename = strFilename;
            m_strlSecondaryFile = objDataConversion.ReadFileToStringList (strFilename, 96);
            m_iSecondaryFileIdx = 0;
        }

        public bool IsPrimaryEndOfFile ()
        {
            return (m_iPrimaryFileIdx + 1 >= m_strlPrimaryFile.Count);
        }

        public bool IsSecondaryEndOfFile ()
        {
            return (m_iSecondaryFileIdx + 1 >= m_strlSecondaryFile.Count);
        }

        public string ReadCardFromPrimary ()
        {
            if (m_iPrimaryFileIdx < m_strlPrimaryFile.Count)
            {
                m_bCardInPrimaryWaitStation = true;
                return objDataConversion.ConvertASCIIstringToEBCDIC (m_strlPrimaryFile[m_iPrimaryFileIdx++]);
            }
            else
            {
                return objDataConversion.ConvertASCIIstringToEBCDIC ("/*"); // System/3 End-of-file marker
            }
        }

        public byte[] ReadCardFromPrimaryIPL ()
        {
            if (m_iPrimaryFileIdx < m_strlPrimaryFile.Count)
            {
                m_bCardInPrimaryWaitStation = true;
                string strPrimaryCardImage = m_strlPrimaryFile[m_iPrimaryFileIdx++];
                return objDataConversion.ReadIPLCard (strPrimaryCardImage);
            }
            else
            {
                return new byte[96];
            }
        }

        public string ReadCardFromSecondary ()
        {
            if (m_iSecondaryFileIdx < m_strlSecondaryFile.Count)
            {
                m_bCardInSecondaryWaitStation = true;
                return objDataConversion.ConvertASCIIstringToEBCDIC (m_strlSecondaryFile[m_iSecondaryFileIdx++]);
            }
            else
            {
                return objDataConversion.ConvertASCIIstringToEBCDIC ("/*"); // System/3 End-of-file marker
            }
        }

        public byte[] ReadCardFromSecondaryIPL ()
        {
            if (m_iSecondaryFileIdx < m_strlSecondaryFile.Count)
            {
                m_bCardInSecondaryWaitStation = true;
                string strSecondaryCardImage = m_strlSecondaryFile[m_iSecondaryFileIdx++];
                return objDataConversion.ReadIPLCard (strSecondaryCardImage);
            }
            else
            {
                return new byte[96];
            }
        }

        public string Stacker1OutputFilename
        {
            get { return m_str5424Stacker1OutputFilename; }
            set { m_str5424Stacker1OutputFilename = value; }
        }

        public string Stacker2OutputFilename
        {
            get { return m_str5424Stacker2OutputFilename; }
            set { m_str5424Stacker2OutputFilename = value; }
        }

        public string Stacker3OutputFilename
        {
            get { return m_str5424Stacker3OutputFilename; }
            set { m_str5424Stacker3OutputFilename = value; }
        }

        public string Stacker4OutputFilename
        {
            get { return m_str5424Stacker4OutputFilename; }
            set { m_str5424Stacker4OutputFilename = value; }
        }

        public void NonProcessRunOut ()
        {
            m_bCardInPrimaryWaitStation = false;
            m_bCardInPrimaryWaitStation = false;
        }
    }

    class C5203LinePrinter : CSystem3IOBase
    {
        //private bool m_bPrintBufferBusy     = false;
        //private bool m_bPrinterCarriageBusy = false;

        int m_iFormLength            = 0;
        int m_iLinePosition          = 1;
        int m_iWaitMillisecLinePrint = 0;  // (60 sec * 1000) / SpeedLPM
        int m_iWaitMillisecLineSkip  = 0;  // m_iWaitMillisecLinePrint / 10
        int m_iWaitMillisecLineFeed  = 0;  // m_iWaitMillisecLinePrint / 15
        int m_iSpeedLPM              = 1000;
        int m_iWaitCyclesLinePrint   = 0;
        int m_iWaitCyclesLineSkip    = 0;
        int m_iWaitCyclesLineFeed    = 0;
        string m_strChainImage = "";

        public int FormLength
        {
            get { return m_iFormLength; }
            set { m_iFormLength = value >> 8; } // Ignore right carriage length
        }

        public int LinePosition
        {
            get { return m_iLinePosition; }
            set { m_iLinePosition = value; }
        }

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
            if (m_iLinePosition > m_iFormLength)
            {
                m_iLinePosition -= m_iFormLength;
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
                if (m_iFormLength > m_iLinePosition)
                {
                    iLinesToSkip = m_iFormLength - m_iLinePosition;
                }
                else
                {
                    iLinesToSkip = 1; // Default value if no forms length has been loaded
                }
            }
            else // iDestinationLine == m_iLinePosition
            {
                return m_iFormLength;
            }

            m_iLinePosition = iDestinationLine;

            //m_bPrinterCarriageBusy = false;

            return iLinesToSkip;
        }
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

        const int m_kiSectorsPerCylinder = 48;
        const int m_kiBytesPerSector     = 256;

        byte[] m_yaDiskImageFixed1     = null;
        byte[] m_yaDiskImageFixed2     = null;
        byte[] m_yaDiskImageRemovable1 = null;
        byte[] m_yaDiskImageRemovable2 = null;

        public void SetWaitTime (int iMilliseconds) { }
        public void SetCycleTime (int iBusyTestCount) { }

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

        public byte[] ReadSector (int iFileOffset, bool bFixed, bool bPrimary)
        {
            byte[] yaSector = new byte[m_kiBytesPerSector];

            if      (bFixed && bPrimary)
            {
                for (int iIdx = 0; iIdx < m_kiBytesPerSector; ++iIdx)
                {
                    yaSector[iIdx] = m_yaDiskImageFixed1[iFileOffset + iIdx];
                }
            }
            else if (!bFixed && bPrimary)
            {
                for (int iIdx = 0; iIdx < m_kiBytesPerSector; ++iIdx)
                {
                    yaSector[iIdx] = m_yaDiskImageRemovable1[iFileOffset + iIdx];
                }
            }
            else if (bFixed && !bPrimary)
            {
                for (int iIdx = 0; iIdx < m_kiBytesPerSector; ++iIdx)
                {
                    yaSector[iIdx] = m_yaDiskImageFixed2[iFileOffset + iIdx];
                }
            }
            else if (!bFixed && !bPrimary)
            {
                for (int iIdx = 0; iIdx < m_kiBytesPerSector; ++iIdx)
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
                for (int iIdx = 0; iIdx < m_kiBytesPerSector; ++iIdx)
                {
                    m_yaDiskImageFixed1[iFileOffset + iIdx] = yaSector[iIdx];
                }
            }
            else if (!bFixed && bPrimary)
            {
                for (int iIdx = 0; iIdx < m_kiBytesPerSector; ++iIdx)
                {
                    m_yaDiskImageRemovable1[iFileOffset + iIdx] = yaSector[iIdx];
                }
            }
            else if (bFixed && !bPrimary)
            {
                for (int iIdx = 0; iIdx < m_kiBytesPerSector; ++iIdx)
                {
                    m_yaDiskImageFixed2[iFileOffset + iIdx] = yaSector[iIdx];
                }
            }
            else if (!bFixed && !bPrimary)
            {
                for (int iIdx = 0; iIdx < m_kiBytesPerSector; ++iIdx)
                {
                    m_yaDiskImageRemovable2[iFileOffset + iIdx] = yaSector[iIdx];
                }
            }
        }

        public int AddressFromCylinderAndSector (int iCylinder, int iSector)
        {
            int iAddress = iCylinder * m_kiSectorsPerCylinder + m_kiBytesPerSector;
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

        public override void ExecuteLIO (byte[] yaInstruction) { }
        public override void ExecuteSIO (byte[] yaInstruction) { }
        public override void ExecuteTIO (byte[] yaInstruction) { }
        public override void ExecuteAPL (byte[] yaInstruction) { }
        public override void ExecuteSNS (byte[] yaInstruction) { }
    }

    class C5475DataEntryKeyboard : CSystem3IOBase
    {
        public override void ExecuteLIO (byte[] yaInstruction) { }
        public override void ExecuteSIO (byte[] yaInstruction) { }
        public override void ExecuteSNS (byte[] yaInstruction) { }
    }

    class C5471KeyboardPrinter : CSystem3IOBase
    {
        public override void ExecuteLIO (byte[] yaInstruction) { }
        public override void ExecuteSIO (byte[] yaInstruction) { }
        public override void ExecuteSNS (byte[] yaInstruction) { }
    }
}
