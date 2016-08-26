using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace EmulatorEngine
{
    public class CEmulatorEngine : CDataConversion
    {
        //protected enum EDskProcessState
        //{
        //    DISK_NoFile,
        //    DISK_Loaded
        //};

        // System/3 registers
        protected int m_iIAR = 0;  // Instruction Address Register
        protected int m_iARR = 0;  // Address Recall Register
        protected int m_iXR1 = 0;  // Index Register 1
        protected int m_iXR2 = 0;  // Index Register 2
        //int m_iPSR = 0;  // Program Status Register
        protected int m_iIL  = 0;  // Index into m_aCR array by interrupt level + 1 (not a real register in the System/3)
        protected CConditionRegister[] m_aCR = { new CConditionRegister (),   // [0] Main program, no interrupt
                                                 new CConditionRegister (),   // [1] Interrupt Level 0 - Dual Programming Control Interrupt Key
                                                 new CConditionRegister (),   // [2] Interrupt Level 1 - 5475 or 5471 keyboard
                                                 new CConditionRegister (),   // [3] Interrupt Level 2 - BSCA (Binary Synchronous Communications Adapter)
                                                 new CConditionRegister (),   // [4] Interrupt Level 3 - Unassigned
                                                 new CConditionRegister () }; // [5] Interrupt Level 4 - Serial I/O Channel

        // System/3 main memory
        protected byte[] m_yaMainMemory = new byte[64 * 1024];

        // System/3 reserved memory areas
        protected int m_iMemorySizeInK;
        protected string m_strSystemDate;
        //protected string m_strPrinterChainImage;
        protected string m_strCopyright;

        protected bool m_bMemorySizeSet = false;
        protected bool m_bSystemDateSet = false;
        protected bool m_bPrinterChainImageSet = false;
        protected bool m_bCopyrightSet = false;

        // Emulator member data
        private EProgramState m_eProgramState = EProgramState.STATE_Stopped;
        private EBootDevice   m_eBootDevice   = EBootDevice.BOOT_Card;
        protected int  m_iConsoleDialSetting    = 0x0000;
        protected int  m_iInstructionCount      = 0;
        protected int  m_iInstructionCountLimit = 100000;
        protected int  m_iHaltCount             = 0;
        protected int  m_iHaltCountLimit        = 1000;

        protected bool m_bInTrace = false;
        public void SetTrace () { m_bInTrace = true; }
        public void ResetTrace () { m_bInTrace = false; }
        public bool GetTrace () { return m_bInTrace; }

        protected bool m_bShowDisassembly = false;
        public void SetShowDisassembly () { m_bShowDisassembly = true; }
        public void ResetShowDisassembly () { m_bShowDisassembly = false; }
        public bool GetShowDisassembly () { return m_bShowDisassembly; }

        protected bool m_bResetMemory = true;
        public bool ClearMemory
        {
            get { return m_bResetMemory; }
            set { m_bResetMemory = value; }
        }

        protected EProgramState ProgramState
        {
            get { return m_eProgramState; }
        }

        protected void TestResetProgramState ()
        {
            m_eProgramState = EProgramState.STATE_Running;
        }

        public int ConsoleDials
        {
            get { return m_iConsoleDialSetting; }
            set { m_iConsoleDialSetting = value & 0xFFFF; }
        }

        // I/O device data - 5203 Line Printer
        private int m_i5203LineWidth = 132;
        private int m_i5203DataAddressRegister = 0x0000;
        private int m_i5203ChainImageAddressRegister = 0x0000;
        byte[] m_ya48CharacterPrinterChainImage =
            { 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8, 0xF9, 0xF0, 0x7B, 0x7C, 0x61, 0xE2, 0xE3, 0xE4,
              0xE5, 0xE6, 0xE7, 0xE8, 0xE9, 0x50, 0x6B, 0x6C, 0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8,
              0xD9, 0x60, 0x5B, 0x5C, 0xC1, 0xC2, 0xC3, 0xC4, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9, 0x4E, 0x4B, 0x7D };

        // I/O device data - 5424 MFCU
        private int m_i5424PrintDataAddressRegister = 0;
        private int m_i5424ReadDataAddressRegister  = 0;
        private int m_i5424PunchDataAddressRegister = 0;

        // I/O device data - 5444 Disk Drive
        private CDiskControlField m_objDiskControlField = new CDiskControlField ();
        private CDumpData m_objDumpData = new CDumpData ();
        private bool m_bDrive1Head0Selection = true;
        private bool m_bDrive2Head0Selection = true;
        private int m_iDrive1Cylinder               = 0;
        private int m_iDrive2Cylinder               = 0;
        //private int m_iDrive1FixedHeadSelected      = 0;
        //private int m_iDrive1RemovableHeadSelected  = 0;
        //private int m_iDrive2FixedHeadSelected      = 0;
        //private int m_iDrive2RemovableHeadSelected  = 0;
        private int m_iDiskControlAddressRegister   = 0;
        private int m_iDiskReadWriteAddressRegister = 0;
        private int m_iDiskStatusDrive1Bytes0and1 = m_ki5444Byte1Bit1CylinderZero;
        private int m_iDiskStatusDrive1Bytes2and3   = 0x0000;
        private int m_iDiskStatusDrive2Bytes0and1 = m_ki5444Byte1Bit1CylinderZero;
        private int m_iDiskStatusDrive2Bytes2and3   = 0x0000;
        private int m_iLastDiskSNS                  = 0;

        // Status bit definitions
        private const int m_ki5444Byte0Bit0NoOp                       = 0x8000;  // Reset by SNS that reads this bit
   //   private const int m_ki5444Byte0Bit1InterventionRequired       = 0x4000;  // Drive# specific
   //   private const int m_ki5444Byte0Bit2MissingAddressMarker       = 0x2000;  // Reset by SIO
   //   private const int m_ki5444Byte0Bit3EquipmentCheck             = 0x1000;  // Drive# specific
   //   private const int m_ki5444Byte0Bit4DataCheck                  = 0x0800;
        private const int m_ki5444Byte0Bit5NoRecordFound              = 0x0400;
   //   private const int m_ki5444Byte0Bit6TrackConditionCheck        = 0x0200;
        private const int m_ki5444Byte0Bit7SeekCheck                  = 0x0100;  // Drive# specific
        private const int m_ki5444Byte1Bit0ScanEqualHit               = 0x0080;
        private const int m_ki5444Byte1Bit1CylinderZero               = 0x0040;  // Drive# specific
        private const int m_ki5444Byte1Bit2EndOfCylinder              = 0x0020;
   //   private const int m_ki5444Byte1Bit3SeekBusy                   = 0x0010;  // Drive# specific
   //   private const int m_ki5444Byte1Bit4100Cylinder                = 0x0008;
   //   private const int m_ki5444Byte1Bit5Overrun                    = 0x0004;
        private const int m_ki5444Byte1Bit6StatusAddressA             = 0x0002;  // Reset by SIO; always 0
        private const int m_ki5444Byte1Bit7StatusAddressB             = 0x0001;  // Reset by SIO; 0 = drive 1, 1 = drive 2
        private const int m_ki5444Byte2Bit0Unsafe                     = 0x8000;  // Drive# specific
   //   private const int m_ki5444Byte2Bit1TimingAnalysisProgramLineA = 0x4000;
   //   private const int m_ki5444Byte2Bit2TimingAnalysisProgramLineB = 0x2000;
   //   private const int m_ki5444Byte2Bit3TimingAnalysisProgramLineC = 0x1000;
   //   private const int m_ki5444Byte2Bit4Index                      = 0x0800;  // Drive# specific
   //   private const int m_ki5444Byte2Bit5Settling                   = 0x0400;  // Drive# specific
   //   private const int m_ki5444Byte2Bit6CESenseBit                 = 0x0200;
   //   private const int m_ki5444Byte2Bit7Model6                     = 0x0100;
   //   private const int m_ki5444Byte3Bit0CESenseBit0                = 0x0080;
   //   private const int m_ki5444Byte3Bit1CESenseBit1                = 0x0040;
   //   private const int m_ki5444Byte3Bit2CESenseBit2                = 0x0020;
   //   private const int m_ki5444Byte3Bit3NotBitRingInhibit          = 0x0010;
   //   private const int m_ki5444Byte3Bit4StandardWriteTrigger       = 0x0008;
   //   private const int m_ki5444Byte3Bit5ConditionPriorityRequest   = 0x0004;
   //   private const int m_ki5444Byte3Bit6BitRing0                   = 0x0002;
   //   private const int m_ki5444Byte3Bit7NotCCRegisterPosition17    = 0x0001;

        // Const values
        private const int m_kiSectorSize         = 256;
        private const int m_kiSectorsPerCylinder = 48;
        private const int m_kiBytesPerCyliner    = m_kiSectorSize * m_kiSectorsPerCylinder;
        private const bool m_kbFixedDrive     = true;
        private const bool m_kbRemovableDrive = false;
        private const bool m_kbPrimaryDrive   = true;
        private const bool m_kbSecondaryDrive = false;

        // I/O device objects
        EKeyboard m_eKeyboard = EKeyboard.KEY_None;
        C5203LinePrinter m_obj5203LinePrinter = new C5203LinePrinter ();
        C5242MFCU m_obj5242MFCU = new C5242MFCU ();
        C5444DiskDrive m_obj5444DiskDrive = new C5444DiskDrive ();

        // Diagnostic variables
        string m_strSIODetails;
        string m_strInstructionAppendix;
        //List<string> strlSectorReading = new List<string> ();

        // Enumerations
        //public enum EInitialization
        //{
        //    INIT_None,
        //    INIT_Card,
        //    INIT_Disk,
        //};

        public enum EBootDevice
        {
            BOOT_Card,
            BOOT_Disk_Fixed,
            BOOT_Disk_Removable
        };

        enum ERegType
        {
            REG_PL2IAR = 0x40,
            REG_PL1IAR = 0x20,
            REG_IAR = 0x10,
            REG_ARR = 0x08,
            REG_PSR = 0x04,
            REG_XR2 = 0x02,
            REG_XR1 = 0x01
        };

        protected enum EProgramState
        {
            STATE_Undefined,
            STATE_Running,
            STATE_Stopped,
            STATE_Paused,
            STATE_Halted,
            STATE_No_File_Loaded,
            STATE_PChk_InvalidOpCode,
            STATE_PChk_InvalidQByte,
            STATE_PChk_AddressWrap,
            STATE_PChk_InvalidAddress,
            STATE_PChk_PL2_Unsupported,
            STATE_PChk_UnsupportedDevice
        };

        enum EIODevice
        {
            DEV_5410_Dials   = 0x00,
            DEV_5424_MFCU    = 0xF0,
            DEV_5203_Printer = 0xE0,
            DEV_5444_Disk_1  = 0xA0,
            DEV_5444_Disk_2  = 0xB0,
            DEV_Keyboard     = 0x10
        };

        enum EKeyboard
        {
            KEY_None,
            KEY_5471,
            KEY_5475
        };

        // Address Class
        protected class CTwoOperandAddress
        {
            int m_iOperandOneAddress = 0;
            int m_iOperandTwoAddress = 0;

            public CTwoOperandAddress (int iOperandOneAddress,
                                       int iOperandTwoAddress)
            {
                m_iOperandOneAddress = iOperandOneAddress;
                m_iOperandTwoAddress = iOperandTwoAddress;
            }

            public int OperandOneAddress
            {
                get { return m_iOperandOneAddress; }
            }

            public int OperandTwoAddress
            {
                get { return m_iOperandTwoAddress; }
            }
        }

        // Condition Register Class
        protected class CConditionRegister
        {
            // IBM System/3 bit assignment
            // 0x01  7  Equal
            // 0x02  6  Low
            // 0x04  5  High
            // 0x08  4  Decimal Overflow
            // 0x10  3  Test False
            // 0x20  2  Binary Underflow
            // 0x40  1  Unassigned
            // 0x80  0  Unassigned

            enum ERegisterFlags
            {
                COND_Equal           = 0x01,
                COND_Low             = 0x02,
                COND_High            = 0x04,
                COND_DecimalOverflow = 0x08,
                COND_TestFalse       = 0x10,
                COND_BinaryOverflow  = 0x20
            };

            static bool s_bInTest = false;
            byte m_yConditionRegister = (byte)ERegisterFlags.COND_Equal;

            public static void SetTestMode ()   { s_bInTest = true; }
            public static void ResetTestMode () { s_bInTest = false; }

            public void Load (int iPSRValue)
            {
                m_yConditionRegister = (byte)(iPSRValue & 0x3F);

                if ((m_yConditionRegister & ((byte)ERegisterFlags.COND_Equal |
                                             (byte)ERegisterFlags.COND_Low)) == 0x00)
                {
                    m_yConditionRegister |= (byte)ERegisterFlags.COND_High;
                }
            }

            public int Store ()
            {
                return (int)(m_yConditionRegister & ((byte)ERegisterFlags.COND_Equal           |
                                                     (byte)ERegisterFlags.COND_Low             |
                                                     (byte)ERegisterFlags.COND_DecimalOverflow |
                                                     (byte)ERegisterFlags.COND_TestFalse       |
                                                     (byte)ERegisterFlags.COND_BinaryOverflow));
            }

            public bool IsEqual ()
            {
                return (m_yConditionRegister & (byte)ERegisterFlags.COND_Equal) == (byte)ERegisterFlags.COND_Equal;
            }

            public bool IsLow ()
            {
                return (m_yConditionRegister & (byte)ERegisterFlags.COND_Low) == (byte)ERegisterFlags.COND_Low;
            }

            public bool IsHigh ()
            {
                return (m_yConditionRegister & (byte)ERegisterFlags.COND_High) == (byte)ERegisterFlags.COND_High;
            }

            public bool IsDecimalOverflow ()
            {
                return (m_yConditionRegister & (byte)ERegisterFlags.COND_DecimalOverflow) == (byte)ERegisterFlags.COND_DecimalOverflow;
            }

            public bool IsTestFalse ()
            {
                bool bIsTestFalse = (m_yConditionRegister & (byte)ERegisterFlags.COND_TestFalse) == (byte)ERegisterFlags.COND_TestFalse;

                return (m_yConditionRegister & (byte)ERegisterFlags.COND_TestFalse) == (byte)ERegisterFlags.COND_TestFalse;
            }

            public bool IsBinaryOverflow ()
            {
                return (m_yConditionRegister & (byte)ERegisterFlags.COND_BinaryOverflow) == (byte)ERegisterFlags.COND_BinaryOverflow;
            }

            public void SystemReset ()
            {
                m_yConditionRegister = (byte)ERegisterFlags.COND_Equal;
            }

            public void SetEqual ()
            {
                // Reset High, Low, and Equal
                m_yConditionRegister &= ((byte)ERegisterFlags.COND_DecimalOverflow |
                                         (byte)ERegisterFlags.COND_TestFalse       |
                                         (byte)ERegisterFlags.COND_BinaryOverflow);

                // Set only Equal flag
                m_yConditionRegister |= (byte)ERegisterFlags.COND_Equal;
            }

            public void SetLow ()
            {
                // Reset High, Low, and Equal
                m_yConditionRegister &= ((byte)ERegisterFlags.COND_DecimalOverflow |
                                         (byte)ERegisterFlags.COND_TestFalse       |
                                         (byte)ERegisterFlags.COND_BinaryOverflow);

                // Set only Low flag
                m_yConditionRegister |= (byte)ERegisterFlags.COND_Low;
            }

            public void SetHigh ()
            {
                // Reset High, Low, and Equal
                m_yConditionRegister &= ((byte)ERegisterFlags.COND_DecimalOverflow |
                                         (byte)ERegisterFlags.COND_TestFalse       |
                                         (byte)ERegisterFlags.COND_BinaryOverflow);

                // Set only High flag
                m_yConditionRegister |= (byte)ERegisterFlags.COND_High;
            }

            public void SetDecimalOverflow () { m_yConditionRegister |= (byte)ERegisterFlags.COND_DecimalOverflow; }
            public void SetTestFalse ()       { m_yConditionRegister |= (byte)ERegisterFlags.COND_TestFalse; }
            public void SetBinaryOverflow ()  { m_yConditionRegister |= (byte)ERegisterFlags.COND_BinaryOverflow; }

            public void TestResetDecimalOverflow ()
            {
                if (s_bInTest)
                {
                    m_yConditionRegister &= ((byte)ERegisterFlags.COND_Equal     |
                                             (byte)ERegisterFlags.COND_Low       |
                                             (byte)ERegisterFlags.COND_High      |
                                             (byte)ERegisterFlags.COND_TestFalse |
                                             (byte)ERegisterFlags.COND_BinaryOverflow);
                }
            }

            public void TestResetTestFalse ()
            {
                if (s_bInTest)
                {
                    m_yConditionRegister &= ((byte)ERegisterFlags.COND_Equal           |
                                             (byte)ERegisterFlags.COND_Low             |
                                             (byte)ERegisterFlags.COND_High            |
                                             (byte)ERegisterFlags.COND_DecimalOverflow |
                                             (byte)ERegisterFlags.COND_BinaryOverflow);
                }
            }

            public void ResetBinaryOverflow ()
            {
                m_yConditionRegister &= ((byte)ERegisterFlags.COND_Equal           |
                                         (byte)ERegisterFlags.COND_Low             |
                                         (byte)ERegisterFlags.COND_High            |
                                         (byte)ERegisterFlags.COND_DecimalOverflow |
                                         (byte)ERegisterFlags.COND_TestFalse);
            }

            private void ResetDecimalOverflow ()
            {
                m_yConditionRegister &= ((byte)ERegisterFlags.COND_Equal     |
                                         (byte)ERegisterFlags.COND_Low       |
                                         (byte)ERegisterFlags.COND_High      |
                                         (byte)ERegisterFlags.COND_TestFalse |
                                         (byte)ERegisterFlags.COND_BinaryOverflow);
            }

            private void ResetTestFalse ()
            {
                m_yConditionRegister &= ((byte)ERegisterFlags.COND_Equal           |
                                         (byte)ERegisterFlags.COND_Low             |
                                         (byte)ERegisterFlags.COND_High            |
                                         (byte)ERegisterFlags.COND_DecimalOverflow |
                                         (byte)ERegisterFlags.COND_BinaryOverflow);
            }

            public bool IsNoOp (byte yQByte)
            {
                // NoOp: 80, X7, XF (X = 0 - 7); 0x80 || (yQByte & 0x07)
                if (yQByte == 0x80 ||
                    (yQByte & 0x87) == 0x07)
                {
                    // NoOp
                    return true;
                }

                return false;
            }

            // This should ONLY be called by JC and BC !!!
            public bool IsConditionTrue (byte yQByte)
            {
                // First test for unconditional jump/branch execution
                // Unconditional: 00, X7, XF (X = 8 - F); 0x00 || (yQByte & 0x87) == 0x87
                if (yQByte == 0x00 ||
                    (yQByte & 0x87) == 0x87)
                {
                    // Unconditional branch
                    return true;
                }

                // Reset Test False and Decimal Overflow if tested here
                bool bConditionMet = false;
                if ((yQByte & 0x80) == 0x80)
                {
                    //(yQByte & 0x80) = 0x80: branch if ANY tested condition is true
                    if ((yQByte & m_yConditionRegister) > 0)
                    {
                        bConditionMet = true;
                    }
                }
                else
                {
                    //(yQByte & 0x80) = 0x00: branch only if ALL tested conditions are false        }
                    if ((yQByte & m_yConditionRegister) == 0)
                    {
                        // Condition met; do the branch
                        bConditionMet = true;
                    }
                }

                // Reset Test False if tested
                if ((yQByte & (byte)ERegisterFlags.COND_TestFalse) == (byte)ERegisterFlags.COND_TestFalse)
                {
                    ResetTestFalse ();
                }

                // Reset Decimal Overflow if tested
                if ((yQByte & (byte)ERegisterFlags.COND_DecimalOverflow) == (byte)ERegisterFlags.COND_DecimalOverflow)
                {
                    ResetDecimalOverflow ();
                }

                return bConditionMet;
            }
        };

        #region System Initialization
        public void SetMemorySize (int iMemorySizeInK)
        {
            m_iMemorySizeInK = iMemorySizeInK;
            m_bMemorySizeSet = true;
        }

        public void SetSystemDate (string strSystemDate )
        {
            m_strSystemDate  = strSystemDate;
            m_bSystemDateSet = true;
        }

        //public void SetPrinterChainImage (string strPrinterChainImage)
        //{
        //    m_strPrinterChainImage  = strPrinterChainImage;
        //    m_bPrinterChainImageSet = true;
        //}

        public void SetCopyright (string strCopyright)
        {
            m_strCopyright  = strCopyright;
            m_bCopyrightSet = true;
        }
        #endregion

        public void SystemReset ()
        {
            m_iIAR = 0x0000;
            m_iARR = 0x0000;
            m_eProgramState = EProgramState.STATE_Stopped;
            foreach (CConditionRegister cr in m_aCR)
            {
                cr.SystemReset ();
            }
        }

        public void Initialize (EBootDevice eBootDevice)
        {
            if (m_bResetMemory)
            {
                // Clear system data area
                for (int iIdx = 0x0100; iIdx < 0x0200; iIdx++)
                {
                    m_yaMainMemory[iIdx] = 0x00;
                }

                // Set memory high address
                m_yaMainMemory[0x017E] = 0xFF; // Set memory size at 64k
                SetMemorySize (m_yaMainMemory.Length);

                // Set printer chain image 0x0100 - 0x0177
                if (eBootDevice == EBootDevice.BOOT_Card)
                {
                    m_eKeyboard = EKeyboard.KEY_5475;
                    m_i5203ChainImageAddressRegister = 0x0100;
                    for (int iSAR = 0x0100; iSAR < 0x0178; iSAR++)
                    {
                        m_yaMainMemory[iSAR] = 0x40;
                    }
                    LoadBinaryImage (m_ya48CharacterPrinterChainImage, 0x0100, 0);
                    m_obj5203LinePrinter.ChainImage = ConvertEbcdicToAscii (m_ya48CharacterPrinterChainImage);
                }
                else if (eBootDevice == EBootDevice.BOOT_Disk_Fixed ||
                         eBootDevice == EBootDevice.BOOT_Disk_Removable)
                {
                    m_eKeyboard = EKeyboard.KEY_5471;
                    m_i5203ChainImageAddressRegister = 0x0400;
                    for (int iSAR = 0x0400; iSAR < 0x0478; iSAR++)
                    {
                        m_yaMainMemory[iSAR] = 0x40;
                    }
                    LoadBinaryImage (m_ya48CharacterPrinterChainImage, 0x0400, 0);
                    m_obj5203LinePrinter.ChainImage = ConvertEbcdicToAscii (m_ya48CharacterPrinterChainImage);
                }

                //StringBuilder strbldrChainImage = new StringBuilder (120);
                //for (int iIdx = 0; iIdx < 120; iIdx++)
                //{
                //    strbldrChainImage.Append ((char)ConvertEBCDICtoASCII (m_ya48CharacterPrinterChainImage[iIdx % m_ya48CharacterPrinterChainImage.Length]));
                //}
                //StringBuilder strbldrChainImage = new StringBuilder (m_ya48CharacterPrinterChainImage.Length);
                //foreach (byte yEbcdic in m_ya48CharacterPrinterChainImage)
                //{
                //    strbldrChainImage.Append ((char)ConvertEBCDICtoASCII (yEbcdic));
                //    //strbldrChainImage.Append ((char)m_ya48CharacterPrinterChainImage[iIdx]);
                //}
                //m_obj5203LinePrinter.ChainImage = strbldrChainImage.ToString ();
                //SetPrinterChainImage (strbldrChainImage.ToString ());

                // Set printer default values
                m_obj5203LinePrinter.FormLength = 66;

                // Set system date 0x0178 - 0x017D
                DateTime dtNow = DateTime.Now;
                string strMonth = string.Format ("{0:D2}", dtNow.Month);
                string strDay   = string.Format ("{0:D2}", dtNow.Day);
                string strYear  = string.Format ("{0:D4}", dtNow.Year);

                m_yaMainMemory[0x0178] = (byte)ConvertTopTierCharacter (strMonth[0]);
                m_yaMainMemory[0x0179] = (byte)ConvertTopTierCharacter (strMonth[1]);
                m_yaMainMemory[0x017A] = (byte)ConvertTopTierCharacter (strDay[0]);
                m_yaMainMemory[0x017B] = (byte)ConvertTopTierCharacter (strDay[1]);
                m_yaMainMemory[0x017C] = (byte)ConvertTopTierCharacter (strYear[2]);
                m_yaMainMemory[0x017D] = (byte)ConvertTopTierCharacter (strYear[3]);

                // Set copyright 0x01B2
                for (int iSAR = 0x01B2; iSAR < 0x01B2 + 46; iSAR++)
                {
                    m_yaMainMemory[iSAR] = 0x40;
                }

                string strCopyright = "COPYRIGHT (C) 2010 SACRED CAT SOFTWARE";
                for (int iIdx = 0; iIdx < strCopyright.Length; iIdx++)
                {
                    m_yaMainMemory[0x01B2 + iIdx] = (byte)ConvertTopTierCharacter (strCopyright[iIdx]);
                }
            }

            m_iInstructionCount = 0;
            m_iHaltCount = 0;
        }

        public void AssignFileToPrimaryHopper (string strFilename)
        {
            m_obj5242MFCU.LoadPrimaryHopperFile (strFilename);
        }

        public void AssignFileToSecondaryHopper (string strFilename)
        {
            m_obj5242MFCU.LoadSecondaryHopperFile (strFilename);
        }

        public void ProgramLoad (EBootDevice eBootDevice)
        {
            ProgramLoad (eBootDevice, "", "");
        }

        public void ProgramLoad (EBootDevice eBootDevice, string strBootFilename, string strRemovableDriveFilename = "")
        {
            if (strBootFilename.Length < 1 ||
                !File.Exists (strBootFilename))
            {
                WaveRedFlag ("File " + strBootFilename + " not found");
                m_eProgramState = EProgramState.STATE_No_File_Loaded;
                return;
            }

            m_eBootDevice = eBootDevice;
            SystemReset ();
            Initialize (eBootDevice);

            // Add code to boot from 5424 MFCU and/or 5496 disk
            if (m_eBootDevice == EBootDevice.BOOT_Card)
            {
                if (strBootFilename.Length > 0)
                {
                    m_obj5242MFCU.LoadPrimaryHopperFile (strBootFilename);
                    byte[] yaBootCardImage = m_obj5242MFCU.ReadCardFromPrimaryIPL ();
                    LoadBinaryImage (yaBootCardImage, 0x0000, 0);
                }
            }
            else if (m_eBootDevice == EBootDevice.BOOT_Disk_Fixed ||
                     m_eBootDevice == EBootDevice.BOOT_Disk_Removable)
            {
                if (strBootFilename.Length > 0)
                {
                    m_obj5444DiskDrive.LoadDiskImageFromFile (strBootFilename, m_kbFixedDrive, m_kbPrimaryDrive);
                }

                if (strRemovableDriveFilename.Length > 0)
                {
                    m_obj5444DiskDrive.LoadDiskImageFromFile (strRemovableDriveFilename, m_kbRemovableDrive, m_kbPrimaryDrive);
                }

                byte[] yaBootSectorImage = new byte[1];

                if (m_eBootDevice == EBootDevice.BOOT_Disk_Fixed)
                {
                    yaBootSectorImage = m_obj5444DiskDrive.ReadFixedDriveBootSector ();
                }
                else if (m_eBootDevice == EBootDevice.BOOT_Disk_Removable)
                {
                    yaBootSectorImage = m_obj5444DiskDrive.ReadRemovableDriveBootSector ();
                }

                LoadBinaryImage (yaBootSectorImage, 0x0000, 0);
            }
            else
            {
                // No boot device specified
            }

            // Begin executing program beginning at 0x0000
            Run (0x0000);
        }

        byte[] m_yaPrntObjectImage = 
        { 0x7C, 0x00, 0x52, 0x75, 0x02, 0xCE, 0x71, 0xF5, 0xCE, 0xF3, 0xF9, 0x07, 0xD1, 0xF9, 0x0C, 0x6D,
          0x01, 0xD2, 0x01, 0xD0, 0x81, 0xB3, 0x7C, 0xBC, 0x1C, 0x9D, 0x00, 0x00, 0xBC, 0xF2, 0x81, 0x0D,
          0x5E, 0x00, 0x1C, 0x3A, 0x7D, 0xC2, 0x1C, 0xD0, 0x82, 0x19, 0xF2, 0x00, 0x0B, 0x7C, 0x06, 0x37,
          0x5E, 0x00, 0x37, 0x1C, 0x9C, 0x00, 0x00, 0x06, 0xE2, 0x02, 0x01, 0x74, 0x02, 0xE0, 0x7D, 0x60,
          0xE0, 0xD0, 0x82, 0x16, 0x7C, 0xC7, 0x81, 0x7C, 0xB9, 0x50, 0x7C, 0x20, 0x51, 0x75, 0x02, 0xCE,
          0xB9, 0x20, 0x00, 0xBC, 0x40, 0x9C, 0xF2, 0x90, 0x03, 0xBC, 0x5C, 0x9C, 0xE2, 0x02, 0x01, 0x74,
          0x02, 0xE0, 0x79, 0x1F, 0xE0, 0xD0, 0x90, 0x50, 0x71, 0xE6, 0xD0, 0xF3, 0xE2, 0x01, 0xD1, 0xE6,
          0x6E, 0x7D, 0x10, 0x51, 0xF2, 0x86, 0x03, 0x7C, 0xB8, 0x50, 0x5E, 0x00, 0x81, 0x3A, 0x5C, 0x00,
          0x51, 0xC7, 0x7D, 0xCD, 0x81, 0xD0, 0x82, 0x4D, 0x5E, 0x00, 0x52, 0x4B, 0x7D, 0x60, 0x52, 0xD0,
          0x82, 0x44, 0x70, 0xE0, 0xE0, 0x7D, 0x30, 0xDF, 0xF2, 0x82, 0x09, 0xF3, 0xE4, 0x8C, 0xD1, 0xE4,
          0x9E, 0xD0, 0x00, 0x00, 0xF3, 0xE0, 0x03, 0xD1, 0xE4, 0xA7, 0xF3, 0xE0, 0x03, 0xD1, 0xE4, 0xAD,
          0xD0, 0x00, 0x00, 0xF3, 0xE4, 0x01, 0xF0, 0x7C, 0x63, 0xD0, 0x00, 0xB6, 0x40, 0x50, 0x60, 0x61,
          0xD0, 0xF0, 0x30, 0x2A, 0x10, 0x21, 0x00, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01, 0x04, 0x00, 0x04,
          0x7C, 0x61, 0x5C };

        public void Run (int iStartAddress)
        {
#if DEBUG_DISK_IPL
            CTraceParser objTraceParser = new CTraceParser ();
            objTraceParser.LoadTraceLog (@"D:\SoftwareDev\System3Simulator\trace.log");
#endif

            // Begin executing program beginning at specified address
            m_iIAR = iStartAddress;
            //bool bLoadingComplete = false;
            m_bExtendMnemonicBC  = true;
            m_bExtendMnemonicJC  = true;
            m_bExtendMnemonicMVX = true;

#if DEBUG_DISK_IPL
            string strRegisters = string.Format ("    Initial register values:      XR1: {0:X4}  XR2: {1:X4}  ARR: {2:X4}  CR: {3:S}",
                                                 m_iXR1, m_iXR2, m_iARR, GetConditionFlags (m_aCR[m_iIL]));
            WriteOutputLine (strRegisters);
#endif
            m_eProgramState = EProgramState.STATE_Running;
            while (m_eProgramState == EProgramState.STATE_Running)
            {
                if (m_iInstructionCount++ >= m_iInstructionCountLimit)
                {
                    m_eProgramState = EProgramState.STATE_Paused;
                    break;
                }

                if (m_iHaltCount >= m_iHaltCountLimit)
                {
                    m_eProgramState = EProgramState.STATE_Paused;
                    break;
                }
                
                //if (m_iIAR == 0x007C) // Branch into Absolute Card Loader
                //{
                //    PrintStringList (DisassembleCode (m_yaMainMemory, 0x0060, 0x00FF));
                //}
                //if (m_iIAR == 0x00DB) // Completion of one 4-byte to 3-byte compression pass
                //{
                //    PrintStringList (BinaryToDump (m_yaMainMemory, 0x0000, 0x005F));
                //}
                //if (m_iIAR == 0x0019) // Branch into end card instructions
                //if (m_iIAR == 0x0AA5)
                //{
                //    bLoadingComplete = true;
                //    PrintStringList (BinaryToDump (m_yaMainMemory, 0x0100, 0x01FF));
                //    //PrintStringList (DisassembleCode (m_yaMainMemory, 0x0019, 0x0057)); // End card
                //    PrintStringList (DisassembleCode (m_yaMainMemory, 0x0D56, 0x13FF)); // Main program
                //}

                int iIAR = m_iIAR;
                byte[] yaInstruction = FetchInstruction ();
                string strInstruction = "";

                if (m_bInTrace)
                {
                    List<string> strlInstruction = DisassembleCode (yaInstruction, 0, yaInstruction.Length - 1);
                    //string strCommentedInstruction = strlInstruction[0];
                    strInstruction = FormatInstructionLine (strlInstruction[0], yaInstruction, iIAR);
                    if (m_bShowDisassembly)
                    {
                        Console.WriteLine ("");
                        //int iColon = strCommentedInstruction.IndexOf (":");
                        //if (iColon >= 4)
                        //    Console.WriteLine (strCommentedInstruction.Substring (iColon - 4));
                        foreach (string str in strlInstruction)
                            Console.WriteLine ("     " + str.Substring (19));
                    }
                    Console.Write (string.Format ("{0:S}", strInstruction));
                }

                ExecuteInstruction (yaInstruction);
#if DEBUG_DISK_IPL
                string strError = objTraceParser.AnalyzeLine (m_iARR, iIAR, m_iXR1, m_iXR2, strInstruction.Substring (6, 4),
                                                              m_iDiskReadWriteAddressRegister, m_iDiskControlAddressRegister,
                                                              m_iLastDiskSNS, m_objDiskControlField);
                if (objTraceParser.IsAbort ())
                {
                    m_eProgramState = EProgramState.STATE_Stopped;
                }
#endif

                if (m_bInTrace &&
                    strInstruction.Length > 0)
                {
                    WriteOutputLine (string.Format ("{0:S}XR1: {1:X4}  XR2: {2:X4}  ARR: {3:X4}  CR: {4:S} {5:S}",
                                                    strInstruction.Length < 43 ? new string (' ', 43 - strInstruction.Length) : "",
                                                    m_iXR1, m_iXR2, m_iARR, GetConditionFlags (m_aCR[m_iIL]), m_strInstructionAppendix));
                }

                m_strInstructionAppendix = "";
                if (m_strSIODetails != null &&
                    m_strSIODetails.Length > 0)
                {
                    Console.WriteLine (string.Format ("      {0:S}", m_strSIODetails));
                    m_strSIODetails = null;

                    m_objDiskControlField.DisplayStates ();
                    DisplayStatus ();
                    //foreach (string str in strlSectorReading)
                    //{
                    //    Console.WriteLine (str);
                    //}
                    //strlSectorReading.Clear ();
                    //m_objDumpData.DisassembleData (m_yaMainMemory);
                }

#if DEBUG_DISK_IPL
                if (strError.Length > 0)
                {
                    Console.WriteLine (strError);
                }
#endif
            }

            //// Compare loaded binary image to reference
            //int iTestStartAddress = 0x0300;
            //for (int iIdx = 0; iIdx < m_yaPrntObjectImage.Length; iIdx++)
            //{
            //    if (m_yaPrntObjectImage[iIdx] == m_yaMainMemory[iIdx + iTestStartAddress])
            //    {
            //    }
            //    else
            //    {
            //    }
            //}

            m_bExtendMnemonicBC  = false;
            m_bExtendMnemonicJC  = false;
            m_bExtendMnemonicMVX = false;
        }

        protected void ExecuteInstruction (byte[] yaInstruction)
        {
            switch (yaInstruction[0])
            {
                // ZAZ
                case 0x04:
                case 0x14:
                case 0x24:
                case 0x44:
                case 0x54:
                case 0x64:
                case 0x84:
                case 0x94:
                case 0xA4:
                {
                    ExectuteZAZ (yaInstruction);
                    break;
                }

                // AZ
                case 0x06:
                case 0x16:
                case 0x26:
                case 0x46:
                case 0x56:
                case 0x66:
                case 0x86:
                case 0x96:
                case 0xA6:
                {
                    ExectuteAZ (yaInstruction);
                    break;
                }

                // SZ
                case 0x07:
                case 0x17:
                case 0x27:
                case 0x47:
                case 0x57:
                case 0x67:
                case 0x87:
                case 0x97:
                case 0xA7:
                {
                    ExectuteSZ (yaInstruction);
                    break;
                }

                // MVX
                case 0x08:
                case 0x18:
                case 0x28:
                case 0x48:
                case 0x58:
                case 0x68:
                case 0x88:
                case 0x98:
                case 0xA8:
                {
                    ExectuteMVX (yaInstruction);
                    break;
                }

                // ED
                case 0x0A:
                case 0x1A:
                case 0x2A:
                case 0x4A:
                case 0x5A:
                case 0x6A:
                case 0x8A:
                case 0x9A:
                case 0xAA:
                {
                    ExectuteED (yaInstruction);
                    break;
                }

                // ITC
                case 0x0B:
                case 0x1B:
                case 0x2B:
                case 0x4B:
                case 0x5B:
                case 0x6B:
                case 0x8B:
                case 0x9B:
                case 0xAB:
                {
                    ExectuteITC (yaInstruction);
                    break;
                }

                // MVC
                case 0x0C:
                case 0x1C:
                case 0x2C:
                case 0x4C:
                case 0x5C:
                case 0x6C:
                case 0x8C:
                case 0x9C:
                case 0xAC:
                {
                    ExectuteMVC (yaInstruction);
                    break;
                }

                // CLC
                case 0x0D:
                case 0x1D:
                case 0x2D:
                case 0x4D:
                case 0x5D:
                case 0x6D:
                case 0x8D:
                case 0x9D:
                case 0xAD:
                {
                    ExectuteCLC (yaInstruction);
                    break;
                }

                // ALC
                case 0x0E:
                case 0x1E:
                case 0x2E:
                case 0x4E:
                case 0x5E:
                case 0x6E:
                case 0x8E:
                case 0x9E:
                case 0xAE:
                {
                    ExectuteALC (yaInstruction);
                    break;
                }

                // SLC
                case 0x0F:
                case 0x1F:
                case 0x2F:
                case 0x4F:
                case 0x5F:
                case 0x6F:
                case 0x8F:
                case 0x9F:
                case 0xAF:
                {
                    ExectuteSLC (yaInstruction);
                    break;
                }

                // SNS   
                case 0x30:
                case 0x70:
                case 0xB0:
                {
                    ExectuteSNS (yaInstruction);
                    break;
                }

                // LIO
                case 0x31:
                case 0x71:
                case 0xB1:
                {
                    ExectuteLIO (yaInstruction);
                    break;
                }

                // ST
                case 0x34:
                case 0x74:
                case 0xB4:
                {
                    ExectuteST (yaInstruction);
                    break;
                }

                // L
                case 0x35:
                case 0x75:
                case 0xB5:
                {
                    ExectuteL (yaInstruction);
                    break;
                }

                // A
                case 0x36:
                case 0x76:
                case 0xB6:
                {
                    ExectuteA (yaInstruction);
                    break;
                }

                // TBN
                case 0x38:
                case 0x78:
                case 0xB8:
                {
                    ExectuteTBN (yaInstruction);
                    break;
                }

                // TBF
                case 0x39:
                case 0x79:
                case 0xB9:
                {
                    ExectuteTBF (yaInstruction);
                    break;
                }

                // SBN
                case 0x3A:
                case 0x7A:
                case 0xBA:
                {
                    ExectuteSBN (yaInstruction);
                    break;
                }

                // SBF
                case 0x3B:
                case 0x7B:
                case 0xBB:
                {
                    ExectuteSBF (yaInstruction);
                    break;
                }

                // MVI
                case 0x3C:
                case 0x7C:
                case 0xBC:
                {
                    ExectuteMVI (yaInstruction);
                    break;
                }

                // CLI
                case 0x3D:
                case 0x7D:
                case 0xBD:
                {
                    ExectuteCLI (yaInstruction);
                    break;
                }

                // BC
                case 0xC0:
                case 0xD0:
                case 0xE0:
                {
                    ExectuteBC (yaInstruction);
                    break;
                }

                // TIO
                case 0xC1:
                case 0xD1:
                case 0xE1:
                {
                    ExectuteTIO (yaInstruction);
                    break;
                }

                // LA
                case 0xC2:
                case 0xD2:
                case 0xE2:
                {
                    ExectuteLA (yaInstruction);
                    break;
                }

                // HPL
                case 0xF0:
                {
                    ExectuteHPL (yaInstruction);
                    break;
                }

                // APL
                case 0xF1:
                {
                    ExectuteAPL (yaInstruction);
                    break;
                }

                // JC
                case 0xF2:
                {
                    ExectuteJC (yaInstruction);
                    break;
                }

                // SIO
                case 0xF3:
                {
                    ExectuteSIO (yaInstruction);
                    break;
                }

                // data
                default:
                {
                    InvalidOpCode ();
                    break;
                }
            }
        }

        #region Diagnostic Methods
        // Test data
        //byte[] yaObjectCode1 = { 0x04, 0x01, 0x10, 0x21, 0x30, 0x41, // ZAZ -6
        //                         0x16, 0x02, 0x11, 0x22, 0x31,       // AZ  -5
        //                         0x27, 0x03, 0x12, 0x23, 0x32,       // SZ  -5
        //                         0x48, 0x04, 0x13, 0x24, 0x33,       // MVX -5
        //                         0x5A, 0x05, 0x14, 0x25,             // ED  -4
        //                         0x6B, 0x06, 0x15, 0x26,             // ITC -4
        //                         0x8C, 0x07, 0x16, 0x27, 0x34,       // MVC -5
        //                         0x9D, 0x07, 0x17, 0x28,             // CLC -4
        //                         0xAE, 0x08, 0x18, 0x29,             // ALC -4
        //                         0x30, 0x0A, 0x19, 0x2A,             // SNS -4
        //                         0xC0, 0x87, 0x19, 0x2A,             // BC  -4
        //                         0xD0, 0x87, 0x2A,                   // BC  -3
        //                         0xE0, 0x87, 0x2A,                   // BC  -3
        //                         0x31, 0x0B, 0x1A, 0x2A,             // LIO -4
        //                         0x71, 0x0B, 0x1A,                   // LIO -3
        //                         0xB4, 0x0C, 0x1B,                   // ST  -3
        //                         0xF2, 0x87, 0x1C };                 // JC  -3

        private void DisplayStatus ()
        {
            bool bBitsOn = false;
            StringBuilder strbldStatusFlags = new StringBuilder ("            Drive 1 Status Bits:");

            if ((m_iDiskStatusDrive1Bytes0and1 & m_ki5444Byte0Bit0NoOp) != 0)
            {
                strbldStatusFlags.Append (" NoOp");
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive1Bytes0and1 & m_ki5444Byte0Bit5NoRecordFound) != 0)
            {
                strbldStatusFlags.Append (" NoRec"); // NoRecordFound
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive1Bytes0and1 & m_ki5444Byte0Bit7SeekCheck) != 0)
            {
                strbldStatusFlags.Append (" SkChk"); // SeekCheck
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive1Bytes0and1 & m_ki5444Byte1Bit0ScanEqualHit) != 0)
            {
                strbldStatusFlags.Append (" SEqHit"); // ScanEqualHit
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive1Bytes0and1 & m_ki5444Byte1Bit1CylinderZero) != 0)
            {
                strbldStatusFlags.Append (" Cyl0"); // CylinderZero
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive1Bytes0and1 & m_ki5444Byte1Bit2EndOfCylinder) != 0)
            {
                strbldStatusFlags.Append (" EOCyl"); // EndOfCylinder
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive1Bytes0and1 & m_ki5444Byte1Bit6StatusAddressA) != 0)
            {
                strbldStatusFlags.Append (" StatA"); // StatusAddressA
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive1Bytes0and1 & m_ki5444Byte1Bit7StatusAddressB) != 0)
            {
                strbldStatusFlags.Append (" StatB"); // StatusAddressB
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive1Bytes0and1 & m_ki5444Byte2Bit0Unsafe) != 0)
            {
                strbldStatusFlags.Append (" Unsafe");
                bBitsOn = true;
            }

            if (!bBitsOn)
            {
                strbldStatusFlags.Append (" <none>");
            }

            Console.WriteLine (strbldStatusFlags.ToString ());

            bBitsOn = false;
            strbldStatusFlags = new StringBuilder ("            Drive 2 Status Bits:");

            if ((m_iDiskStatusDrive2Bytes0and1 & m_ki5444Byte0Bit0NoOp) != 0)
            {
                strbldStatusFlags.Append (" NoOp");
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive2Bytes0and1 & m_ki5444Byte0Bit5NoRecordFound) != 0)
            {
                strbldStatusFlags.Append (" NoRec"); // NoRecordFound
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive2Bytes0and1 & m_ki5444Byte0Bit7SeekCheck) != 0)
            {
                strbldStatusFlags.Append (" SkChk"); // SeekCheck
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive2Bytes0and1 & m_ki5444Byte1Bit0ScanEqualHit) != 0)
            {
                strbldStatusFlags.Append (" SEqHit"); // ScanEqualHit
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive2Bytes0and1 & m_ki5444Byte1Bit1CylinderZero) != 0)
            {
                strbldStatusFlags.Append (" Cyl0"); // CylinderZero
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive2Bytes0and1 & m_ki5444Byte1Bit2EndOfCylinder) != 0)
            {
                strbldStatusFlags.Append (" EOCyl"); // EndOfCylinder
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive2Bytes0and1 & m_ki5444Byte1Bit6StatusAddressA) != 0)
            {
                strbldStatusFlags.Append (" StatA"); // StatusAddressA
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive2Bytes0and1 & m_ki5444Byte1Bit7StatusAddressB) != 0)
            {
                strbldStatusFlags.Append (" StatB"); // StatusAddressB
                bBitsOn = true;
            }

            if ((m_iDiskStatusDrive2Bytes0and1 & m_ki5444Byte2Bit0Unsafe) != 0)
            {
                strbldStatusFlags.Append (" Unsafe");
                bBitsOn = true;
            }

            if (!bBitsOn)
            {
                strbldStatusFlags.Append (" <none>");
            }

            Console.WriteLine (strbldStatusFlags.ToString ());
        }

        char GetHexChar (char cData)
        {
            cData &= (char)0x0F;
            if (cData > (char)0x09)
            {
                cData += (char)0x57;
            }
            else
            {
                cData += (char)0x30;
            }

            return cData;
        }

        protected void WaveRedFlag (string strMessage)
        {
            int iLenghth = strMessage.Length + 12;
            string strAsteriskFrame = new string ('*', iLenghth),
                   strSpaceFiller = new string (' ', iLenghth - 6);

            Console.WriteLine ("");
            Console.WriteLine (strAsteriskFrame);
            Console.WriteLine (strAsteriskFrame);
            Console.WriteLine ("***" + strSpaceFiller + "***");
            Console.WriteLine ("***   " + strMessage + "   ***");
            Console.WriteLine ("***" + strSpaceFiller + "***");
            Console.WriteLine (strAsteriskFrame);
            Console.WriteLine (strAsteriskFrame);
            Console.WriteLine ("");
        }
        #endregion

        #region Instruction Methods
        // ZAZ - Zero and Add Zoned
        private void ExectuteZAZ (byte[] yaInstruction)
        {
            CTwoOperandAddress objTOA = GetTwoOperandAddress (yaInstruction);
            byte yQByte = yaInstruction[1];
            int iOperandTwoLength = (yQByte & 0x0F) + 1,
                iOperandOneLength = (yQByte >> 4) + iOperandTwoLength;
            bool bZeroValue = true,
                 bNegative  = false;

            if (ZonedAddressesValid (objTOA, yQByte))
            {
                int iOperandOneAddress = objTOA.OperandOneAddress,
                    iOperandTwoAddress = objTOA.OperandTwoAddress;

                // Check for invalid operand overlap
                if (iOperandOneAddress < iOperandTwoAddress &&
                    iOperandOneAddress > iOperandTwoAddress - iOperandTwoLength)
                {
                    InvalidQByte ();
                    return;
                }

                // Copy the data, setting the zones for each character, zeroing all leading positiona
                while ((iOperandOneLength) > 0)
                {
                    if (iOperandTwoLength > 0)
                    {
                        m_yaMainMemory[iOperandOneAddress] = m_yaMainMemory[iOperandTwoAddress];
                        m_yaMainMemory[iOperandOneAddress] |= (byte)0xF0;
                        if ((m_yaMainMemory[iOperandOneAddress] - (byte)0xF0) > 0)
                        {
                            bZeroValue = false;
                        }
                    }
                    else
                    {
                        m_yaMainMemory[iOperandOneAddress] = (byte)0xF0;
                    }

                    iOperandOneLength--;
                    iOperandTwoLength--;
                    iOperandOneAddress--;
                    iOperandTwoAddress--;
                }

                // Now set the sign bit in the rightmost position
                if ((m_yaMainMemory[objTOA.OperandTwoAddress] & 0xF0) != 0xF0)
                {
                    m_yaMainMemory[objTOA.OperandOneAddress] &= 0xDF;
                    m_yaMainMemory[objTOA.OperandOneAddress] |= 0xD0;
                    bNegative = true;
                }
            }

            if (bZeroValue)
            {
                m_aCR[m_iIL].SetEqual ();
            }
            else if (bNegative)
            {
                m_aCR[m_iIL].SetLow ();
            }
            else
            {
                m_aCR[m_iIL].SetHigh ();
            }
        }

        // AZ - Add Zoned Decimal
        private void ExectuteAZ (byte[] yaInstruction)
        {
            ExecuteDecimalArithmetic (yaInstruction, true);
        }

        // SZ - Subtract Zoned Decimal
        private void ExectuteSZ (byte[] yaInstruction)
        {
            ExecuteDecimalArithmetic (yaInstruction, false);
        }

        // ALC - Add Logical Characters
        private void ExectuteALC (byte[] yaInstruction)
        {
            m_aCR[m_iIL].ResetBinaryOverflow ();
            CTwoOperandAddress objTOA = GetTwoOperandAddress (yaInstruction);
            byte yQByte = (byte)(yaInstruction[1] + 1);

            if (BinaryAddressesValid (objTOA, yQByte))
            {
                bool bCarry = false,
                     bZero  = true;

                int iOperandOneAddress = objTOA.OperandOneAddress,
                    iOperandTwoAddress = objTOA.OperandTwoAddress;

                while (yQByte-- > 0)
                {
                    int iOperand1 = m_yaMainMemory[iOperandOneAddress],
                        iOperand2 = m_yaMainMemory[iOperandTwoAddress],
                        iResult = iOperand1 + iOperand2;

                    if (bCarry)
                    {
                        iResult++;
                        bCarry = false;
                    }

                    if ((iResult & 0xFF) > 0x00)
                    {
                        bZero = false;
                    }

                    if (iResult > 0xFF)
                    {
                        iResult -= 0x0100;
                        bCarry = true;
                    }

                    m_yaMainMemory[iOperandOneAddress] = (byte)(iResult & 0xFF);
                    iOperandOneAddress--;
                    iOperandTwoAddress--;
                }

                // Update the condition register with the result
                if (bCarry)
                {
                    m_aCR[m_iIL].SetBinaryOverflow ();
                }

                if (bZero)
                {
                    m_aCR[m_iIL].SetEqual ();
                }
                else if (bCarry)
                {
                    // Carry occured out of high-order byte and result is non-zero
                    m_aCR[m_iIL].SetHigh ();
                }
                else
                {
                    // NO carry occured out of high-order byte and result is non-zero
                    m_aCR[m_iIL].SetLow ();
                }
            }
        }

        // SLC - Subtract Logical Characters
        private void ExectuteSLC (byte[] yaInstruction)
        {
            CTwoOperandAddress objTOA = GetTwoOperandAddress (yaInstruction);
            byte yQByte = (byte)(yaInstruction[1] + 1);

            if (BinaryAddressesValid (objTOA, yQByte))
            {
                bool bBorrow = false,
                     bZero   = true;

                int iOperandOneAddress = objTOA.OperandOneAddress;
                int iOperandTwoAddress = objTOA.OperandTwoAddress;

                while (yQByte-- > 0)
                {
                    int iOperand1 = m_yaMainMemory[iOperandOneAddress],
                        iOperand2 = m_yaMainMemory[iOperandTwoAddress],
                        iResult   = iOperand1 - iOperand2;

                    if (bBorrow)
                    {
                        iResult--;
                        bBorrow = false;
                    }

                    if ((iResult & 0xFF) > 0x00)
                    {
                        bZero = false;
                    }

                    if ((iResult - (iResult & 0xFF)) != 0)
                    {
                        bBorrow = true;
                    }

                    m_yaMainMemory[iOperandOneAddress] = (byte)(iResult & 0xFF);
                    iOperandOneAddress--;
                    iOperandTwoAddress--;
                }

                // Update the condition register with the result
                if (bZero)
                {
                    m_aCR[m_iIL].SetEqual ();
                }
                else if (bBorrow)
                {
                    // Borrow occured out of high-order byte and result is non-zero
                    m_aCR[m_iIL].SetLow ();
                }
                else
                {
                    // NO borrow occured out of high-order byte and result is non-zero
                    m_aCR[m_iIL].SetHigh ();
                }
            }
        }

        // CLC - Compare Logical Characters
        private void ExectuteCLC (byte[] yaInstruction)
        {
            CTwoOperandAddress objTOA = GetTwoOperandAddress (yaInstruction);
            byte yQByte = yaInstruction[1];

            if (BinaryAddressesValid (objTOA, yQByte))
            {
                int iOperandOneAddress = objTOA.OperandOneAddress - yQByte;
                int iOperandTwoAddress = objTOA.OperandTwoAddress - yQByte;

                // Shortcut: if both addresses match, both operands can't differ
                if (iOperandOneAddress == iOperandTwoAddress)
                {
                    m_aCR[m_iIL].SetEqual ();
                    return;
                }

                int iOffset = (int)yQByte + 1;

                while (iOffset-- > 0)
                {
                    if (m_yaMainMemory[iOperandOneAddress] < m_yaMainMemory[iOperandTwoAddress])
                    {
                        m_aCR[m_iIL].SetLow ();
                        return;
                    }
                    else if (m_yaMainMemory[iOperandOneAddress] > m_yaMainMemory[iOperandTwoAddress])
                    {
                        m_aCR[m_iIL].SetHigh ();
                        return;
                    }
                    else
                    {
                        iOperandOneAddress++;
                        iOperandTwoAddress++;
                    }
                }

                m_aCR[m_iIL].SetEqual ();
            }
        }

        // MVC - Move Characters
        private void ExectuteMVC (byte[] yaInstruction)
        {
            CTwoOperandAddress objTOA = GetTwoOperandAddress (yaInstruction);
            byte yQByte = yaInstruction[1];

            if (BinaryAddressesValid (objTOA, yQByte))
            {
                int iOperandOneAddress = objTOA.OperandOneAddress;
                int iOperandTwoAddress = objTOA.OperandTwoAddress;
                int iOffset = (int)yQByte + 1;

                while (iOffset-- > 0)
                {
                    m_yaMainMemory[iOperandOneAddress--] = m_yaMainMemory[iOperandTwoAddress--];
                }
            }
        }

        // MVX - Move Hex Character
        private void ExectuteMVX (byte[] yaInstruction)
        {
            CTwoOperandAddress objTOA = GetTwoOperandAddress (yaInstruction);
            if (!IsAddressValid (objTOA.OperandOneAddress) ||
                !IsAddressValid (objTOA.OperandTwoAddress))
            {
                return;
            }

            byte yQByte = yaInstruction[1];
            byte yOperand1 = m_yaMainMemory[objTOA.OperandOneAddress];
            byte yOperand2 = m_yaMainMemory[objTOA.OperandTwoAddress];

            if (yQByte == 0x00) // Zone to Zone
            {
                yOperand1 &= 0x0F;
                yOperand1 |= (byte)(yOperand2 & 0xF0);
            }
            else if (yQByte == 0x01) // Numeric to Zone
            {
                yOperand1 &= 0x0F;
                yOperand2 <<= 4;
                yOperand1 |= yOperand2;
            }
            else if (yQByte == 0x02) // Zone to Numeric
            {
                yOperand1 &= 0xF0;
                yOperand2 >>= 4;
                yOperand1 |= yOperand2;
            }
            else if (yQByte == 0x03) // Numeric to Numeric
            {
                yOperand1 &= 0xF0;
                yOperand1 |= (byte)(yOperand2 & 0x0F);
            }

            m_yaMainMemory[objTOA.OperandOneAddress] = yOperand1;
        }

        // ED - Edit
        private void ExectuteED (byte[] yaInstruction)
        {
            CTwoOperandAddress objTOA = GetTwoOperandAddress (yaInstruction);
            byte yQByte = yaInstruction[1];

            if (BinaryAddressesValid (objTOA, yQByte))
            {
                int iOperandOneLength  = (int)yQByte + 1,
                    iOperandOneAddress = objTOA.OperandOneAddress,
                    iOperandTwoAddress = objTOA.OperandTwoAddress,
                    iOperandOneStart   = iOperandOneAddress - iOperandOneLength,
                    iOperandTwoStart   = iOperandTwoAddress - iOperandOneLength;

                // Operands may not overlap
                if ((iOperandOneStart   >= iOperandTwoStart && iOperandOneStart   <= iOperandTwoAddress) ||
                    (iOperandOneAddress >= iOperandTwoStart && iOperandOneAddress <= iOperandTwoAddress) ||
                    (iOperandTwoStart   >= iOperandOneStart && iOperandTwoStart   <= iOperandOneAddress) ||
                    (iOperandTwoAddress >= iOperandOneStart && iOperandTwoAddress <= iOperandOneAddress))
                {
                    InvalidQByte ();
                    return;
                }

                bool bZero = true;
                bool bNegative = (m_yaMainMemory[iOperandTwoAddress] & 0xF0) == 0xD0;
                int iOffsetTwo = 0;

                for (int iOffsetOne = 0; iOffsetOne < iOperandOneLength; iOffsetOne++)
                {
                    if (m_yaMainMemory[iOperandOneAddress - iOffsetOne] == 0x20)
                    {
                        m_yaMainMemory[iOperandOneAddress - iOffsetOne] = m_yaMainMemory[iOperandTwoAddress - iOffsetTwo];
                        iOffsetTwo++;

                        m_yaMainMemory[iOperandOneAddress - iOffsetOne] |= 0xF0;

                        if ((m_yaMainMemory[iOperandOneAddress - iOffsetOne] & 0x0F) > 0)
                        {
                             bZero = false;
                        }
                    }
                }

                // Set condition register
                if (bZero)
                {
                    m_aCR[m_iIL].SetEqual ();
                }
                else if (bNegative)
                {
                    m_aCR[m_iIL].SetLow ();
                }
                else
                {
                    m_aCR[m_iIL].SetHigh ();
                }
            }
        }

        // ITC - Insert and Test Characters
        private void ExectuteITC (byte[] yaInstruction)
        {
            CTwoOperandAddress objTOA = GetTwoOperandAddress (yaInstruction);
            byte yQByte = yaInstruction[1];

            int iOperandOneLength  = (int)yQByte + 1,
                iOperandOneAddress = objTOA.OperandOneAddress,
                iOperandTwoAddress = objTOA.OperandTwoAddress;

            // NOTE: Operand One addresses leftmost byte instead of rightmost
            if ((iOperandOneAddress + iOperandOneLength > m_yaMainMemory.Length) ||
                (iOperandTwoAddress > m_yaMainMemory.Length))
            {
                InvalidAddress ();
                return;
            }

            //iOperandOneLength++;
            m_iARR = iOperandOneAddress + iOperandOneLength; // Default ARR value

            for (int iIdx = 0; iIdx < iOperandOneLength; iIdx++)
            {
                // Upon finding the first significant numeric digit, job is finished
                if (m_yaMainMemory[iOperandOneAddress + iIdx] >= 0xF1 &&
                    m_yaMainMemory[iOperandOneAddress + iIdx] <= 0xF9)
                {
                    m_iARR = iOperandOneAddress + iIdx;
                    break;
                }

                m_yaMainMemory[iOperandOneAddress + iIdx] = m_yaMainMemory[iOperandTwoAddress];
            }

            // No condition register changes
        }

        // MVI - Move Logical Immediate
        private void ExectuteMVI (byte[] yaInstruction)
        {
            int iOperandAddress = GetOneOperandAddress (yaInstruction);
            if (!IsAddressValid (iOperandAddress))
            {
                return;
            }

            m_yaMainMemory[iOperandAddress] = yaInstruction[1];
        }

        // CLI - Compare Logical Immediate
        private void ExectuteCLI (byte[] yaInstruction)
        {
            int iOperandAddress = GetOneOperandAddress (yaInstruction);
            if (!IsAddressValid (iOperandAddress))
            {
                return;
            }

            byte yQByte = yaInstruction[1];

            if (m_yaMainMemory[iOperandAddress] < yQByte)
            {
                m_aCR[m_iIL].SetLow ();
            }
            else if (m_yaMainMemory[iOperandAddress] > yQByte)
            {
                m_aCR[m_iIL].SetHigh ();
            }
            else
            {
                m_aCR[m_iIL].SetEqual ();
            }
        }

        // SBN - Set Bits On Masked
        private void ExectuteSBN (byte[] yaInstruction)
        {
            int iOperandAddress = GetOneOperandAddress (yaInstruction);
            if (!IsAddressValid (iOperandAddress))
            {
                return;
            }


            m_yaMainMemory[iOperandAddress] |= yaInstruction[1];
        }

        // SBF - Set Bits Off Masked
        private void ExectuteSBF (byte[] yaInstruction)
        {
            int iOperandAddress = GetOneOperandAddress (yaInstruction);
            if (!IsAddressValid (iOperandAddress))
            {
                return;
            }


            m_yaMainMemory[iOperandAddress] &= (byte)~yaInstruction[1];
        }

        // TBN - Test Bits On Masked
        private void ExectuteTBN (byte[] yaInstruction)
        {
            int iOperandAddress = GetOneOperandAddress (yaInstruction);
            if (!IsAddressValid (iOperandAddress))
            {
                return;
            }

            byte yQByte = yaInstruction[1];

            if ((yQByte & m_yaMainMemory[iOperandAddress]) != yQByte)
            {
                m_aCR[m_iIL].SetTestFalse ();
            }
        }

        // TBF - Test Bits Off Masked
        private void ExectuteTBF (byte[] yaInstruction)
        {
            int  iOperandAddress = GetOneOperandAddress (yaInstruction);
            if (!IsAddressValid (iOperandAddress))
            {
                return;
            }

            byte yQByte = yaInstruction[1];

            if ((yQByte & (byte)~m_yaMainMemory[iOperandAddress]) != yQByte)
            {
                m_aCR[m_iIL].SetTestFalse ();
            }
        }

        // A - Add to Register
        private void ExectuteA (byte[] yaInstruction)
        {
            int  iOperandAddress = GetOneOperandAddress (yaInstruction);
            byte yQByte          = yaInstruction[1];
            int  iRegValue       = 0;

            if (AddressWraps (iOperandAddress, 2))
            {
                AddressWraparound ();
                return;
            }

            // Extract the register value
            if ((yQByte & (byte)ERegType.REG_PL2IAR) == (byte)ERegType.REG_PL2IAR)
            {
                NoPL2Support ();
                return;
            }
            else if ((yQByte & (byte)ERegType.REG_PL1IAR) == (byte)ERegType.REG_PL1IAR)
            {
                iRegValue = m_iIAR;
            }
            else if ((yQByte & (byte)ERegType.REG_IAR) == (byte)ERegType.REG_IAR)
            {
                iRegValue = m_iIAR;
            }
            else if ((yQByte & (byte)ERegType.REG_ARR) == (byte)ERegType.REG_ARR)
            {
                iRegValue = m_iARR;
            }
            else if ((yQByte & (byte)ERegType.REG_PSR) == (byte)ERegType.REG_PSR)
            {
                iRegValue = m_aCR[m_iIL].Store ();
            }
            else if ((yQByte & (byte)ERegType.REG_XR2) == (byte)ERegType.REG_XR2)
            {
                iRegValue = m_iXR2;
            }
            else if ((yQByte & (byte)ERegType.REG_XR1) == (byte)ERegType.REG_XR1)
            {
                iRegValue = m_iXR1;
            }

            // Perform arithmetic and update CR flags
            iRegValue += LoadInt (iOperandAddress);
            if (iRegValue == 0x0000)
            {
                m_aCR[m_iIL].SetEqual ();
            }
            else if (iRegValue > 0xFFFF)
            {
                iRegValue &= 0xFFFF;
                m_aCR[m_iIL].SetHigh ();
                m_aCR[m_iIL].SetBinaryOverflow ();
            }
            else
            {
                m_aCR[m_iIL].SetLow ();
            }

            // Replace the register value
            if ((yQByte & (byte)ERegType.REG_PL1IAR) == (byte)ERegType.REG_PL1IAR)
            {
                m_iIAR = iRegValue;
            }
            else if ((yQByte & (byte)ERegType.REG_IAR) == (byte)ERegType.REG_IAR)
            {
                m_iIAR = iRegValue;
            }
            else if ((yQByte & (byte)ERegType.REG_ARR) == (byte)ERegType.REG_ARR)
            {
                m_iARR = iRegValue;
            }
            else if ((yQByte & (byte)ERegType.REG_PSR) == (byte)ERegType.REG_PSR)
            {
                m_aCR[m_iIL].Load (iRegValue);
            }
            else if ((yQByte & (byte)ERegType.REG_XR2) == (byte)ERegType.REG_XR2)
            {
                m_iXR2 = iRegValue;
            }
            else if ((yQByte & (byte)ERegType.REG_XR1) == (byte)ERegType.REG_XR1)
            {
                m_iXR1 = iRegValue;
            }
        }

        // L - Load Register
        private void ExectuteL (byte[] yaInstruction)
        {
            int iOperandAddress = GetOneOperandAddress (yaInstruction);
            byte yQByte = yaInstruction[1];

            if (AddressWraps (iOperandAddress, 2))
            {
                AddressWraparound ();
                return;
            }

            // Replace the register value
            if ((yQByte & (byte)ERegType.REG_PL2IAR) == (byte)ERegType.REG_PL2IAR)
            {
                NoPL2Support ();
                return;
            }
            else if ((yQByte & (byte)ERegType.REG_PL1IAR) == (byte)ERegType.REG_PL1IAR)
            {
                m_iIAR = LoadInt (iOperandAddress);
            }
            else if ((yQByte & (byte)ERegType.REG_IAR) == (byte)ERegType.REG_IAR)
            {
                m_iIAR = LoadInt (iOperandAddress);
            }
            else if ((yQByte & (byte)ERegType.REG_ARR) == (byte)ERegType.REG_ARR)
            {
                m_iARR = LoadInt (iOperandAddress);
            }
            else if ((yQByte & (byte)ERegType.REG_PSR) == (byte)ERegType.REG_PSR)
            {
                m_aCR[m_iIL].Load (LoadInt (iOperandAddress));
            }
            else if ((yQByte & (byte)ERegType.REG_XR2) == (byte)ERegType.REG_XR2)
            {
                m_iXR2 = LoadInt (iOperandAddress);
            }
            else if ((yQByte & (byte)ERegType.REG_XR1) == (byte)ERegType.REG_XR1)
            {
                m_iXR1 = LoadInt (iOperandAddress);
            }
        }

        // LA - Load Address
        private void ExectuteLA (byte[] yaInstruction)
        {
            int iOperandAddress = GetOneOperandAddress (yaInstruction);
            byte yQByte = yaInstruction[1];

            if ((yQByte & 0x01) == 0x01)
            {
                m_iXR1 = iOperandAddress;
            }

            if ((yQByte & 0x02) == 0x02)
            {
                m_iXR2 = iOperandAddress;
            }
        }

        // ST - Store Register
        private void ExectuteST (byte[] yaInstruction)
        {
            int iOperandAddress = GetOneOperandAddress (yaInstruction);
            byte yQByte = yaInstruction[1];

            if (AddressWraps (iOperandAddress, 2))
            {
                AddressWraparound ();
                return;
            }

            // Extract the register value
            if ((yQByte & (byte)ERegType.REG_PL2IAR) == (byte)ERegType.REG_PL2IAR)
            {
                NoPL2Support ();
                return;
            }
            else if ((yQByte & (byte)ERegType.REG_PL1IAR) == (byte)ERegType.REG_PL1IAR)
            {
                StoreRegister (m_iIAR, iOperandAddress);
            }
            else if ((yQByte & (byte)ERegType.REG_IAR) == (byte)ERegType.REG_IAR)
            {
                StoreRegister (m_iIAR, iOperandAddress);
            }
            else if ((yQByte & (byte)ERegType.REG_ARR) == (byte)ERegType.REG_ARR)
            {
                StoreRegister (m_iARR, iOperandAddress);
            }
            else if ((yQByte & (byte)ERegType.REG_PSR) == (byte)ERegType.REG_PSR)
            {
                StoreRegister (m_aCR[m_iIL].Store (), iOperandAddress);
            }
            else if ((yQByte & (byte)ERegType.REG_XR2) == (byte)ERegType.REG_XR2)
            {
                StoreRegister (m_iXR2, iOperandAddress);
            }
            else if ((yQByte & (byte)ERegType.REG_XR1) == (byte)ERegType.REG_XR1)
            {
                StoreRegister (m_iXR1, iOperandAddress);
            }
        }

        // BC - Branch On Condition
        private void ExectuteBC (byte[] yaInstruction)
        {
            int iOperandAddress = GetOneOperandAddress (yaInstruction);
            if (!IsAddressValid (iOperandAddress))
            {
                return;
            }

            byte yQByte = yaInstruction[1];

            if (!m_aCR[m_iIL].IsNoOp (yQByte))
            {
                if (m_aCR[m_iIL].IsConditionTrue (yQByte))
                {
                    m_iARR = m_iIAR;
                    m_iIAR = iOperandAddress;
                }
            }
        }

        // LIO - Load I/O
        private DateTime m_dtLast;
        private int m_iCount = 0;
        private int m_iTotal = 0;

        private void ExectuteLIO (byte[] yaInstruction)
        {
            int  iOperandAddress = GetOneOperandAddress (yaInstruction);
            byte yQByte          = yaInstruction[1],
                 yDeviceAddress  = (byte)(yQByte & 0xF0),
                 yMCode          = (byte)(yQByte & 0x08),
                 yNCode          = (byte)(yQByte & 0x07);

            if (AddressWraps (iOperandAddress, 2))
            {
                AddressWraparound ();
                return;
            }

            if (yDeviceAddress == (byte)EIODevice.DEV_5424_MFCU)
            {
                if ((yQByte & 0x08) != 0x08) // Diagnostic mode not supported since I have no idea what it does
                {
                    if ((yQByte & 0x07) == 0x04)
                    {
                        m_i5424PrintDataAddressRegister = LoadInt (iOperandAddress);
                    }
                    else if ((yQByte & 0x07) == 0x05)
                    {
                        m_i5424ReadDataAddressRegister = LoadInt (iOperandAddress);
                    }
                    else if ((yQByte & 0x07) == 0x06)
                    {
                        m_i5424PunchDataAddressRegister = LoadInt (iOperandAddress);
                    }
                    else
                    {
                        InvalidQByte ();
                        return;
                    }
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5203_Printer)
            {
                if ((yQByte & 0x08) == 0x08)
                {
                    InvalidQByte ();
                    return;
                }
                else
                {
                    if ((yQByte & 0x07) == 0x00)
                    {
                        // Load forms length register
                        m_obj5203LinePrinter.FormLength = LoadInt (iOperandAddress);
                    }
                    else if ((yQByte & 0x07) == 0x04)
                    {
                        // Load printer chain image address register
                        m_i5203ChainImageAddressRegister = iOperandAddress;
                        //m_obj5203LinePrinter.ChainImage = LoadString (iOperandAddress & 0xFF00, iOperandAddress & 0x00FF);
                    }
                    else if ((yQByte & 0x07) == 0x06)
                    {
                        // Load printer data address register
                        m_i5203DataAddressRegister = LoadInt (iOperandAddress);
                        //int iDataAddress = LoadInt (iOperandAddress);
                        //m_i5203LineWidth = ((iDataAddress & 0x00FF) == 0xD8) ? 96  :
                        //                   ((iDataAddress & 0x00FF) == 0xF3) ? 120 :
                        //                   ((iDataAddress & 0x00FF) == 0xD8) ? 132 : 0;
                        //if (m_i5203LineWidth > 0)
                        //{
                        //    m_i5203DataAddressRegister = iDataAddress;
                        //}
                    }
                    else
                    {
                        InvalidQByte ();
                        return;
                    }
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5444_Disk_1 ||
                     yDeviceAddress == (byte)EIODevice.DEV_5444_Disk_2)
            {
                // LIO is not drive-sensitive, to either device address will work fine
                // LIO ignores the M code
                if ((yQByte & 0x07) == 0x03)
                {
                    // Reserved for CE use; ignore
                    return;
                }
                else if ((yQByte & 0x07) == 0x04)
                {
                    m_iDiskReadWriteAddressRegister = LoadInt (iOperandAddress);
                    m_strInstructionAppendix = string.Format ("DAR: {0:X4}", m_iDiskReadWriteAddressRegister);
                }
                else if ((yQByte & 0x07) == 0x06)
                {
                    m_iDiskControlAddressRegister = LoadInt (iOperandAddress);
                    m_strInstructionAppendix = string.Format ("CAR: {0:X4}", m_iDiskControlAddressRegister);
                }
                else
                {
                    InvalidQByte ();
                    return;
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_Keyboard)
            {
                // Since both 5471 and 5475 keyboards have the same device address, the
                // keyboard object will need to determine which keyboard model it is
                if (m_eKeyboard == EKeyboard.KEY_5471)
                {
                }
                else if (m_eKeyboard == EKeyboard.KEY_5475)
                {
                    DateTime dtNow = DateTime.Now;
                    if (m_dtLast != null)
                    {
                        int iLastSecond = m_dtLast.Second;
                        int iThisSecond = dtNow.Second;
                        if (iThisSecond < iLastSecond)
                        {
                            iThisSecond += 60;
                        }
                        int iMS = (iThisSecond - iLastSecond) * 1000;
                        iMS += (dtNow.Millisecond - m_dtLast.Millisecond);
                        m_iCount++;
                        if (m_iCount > 10)
                        {
                            m_iTotal += iMS;
                            Console.WriteLine ("Elapsed Time: {0:D3} milliseconds, average: {1:D3}", iMS, m_iTotal / (m_iCount - 10));
                        }
                    }
                    m_dtLast = DateTime.Now;
                    //Thread.Sleep (950);

                    int iDisplayValue  = LoadInt (iOperandAddress);
                    byte yLeftDisplay  = (byte)(iDisplayValue >> 8),
                         yRightDisplay = (byte)(iDisplayValue & 0xFF);

                    List<string> strl5475Display = Get5475Display (yLeftDisplay, yRightDisplay);

                    WriteOutputLine ("* * *  5 4 7 5  * * *");
                    WriteOutputLine ("1 [ " + strl5475Display[0] + " ]");
                    WriteOutputLine ("2 [ " + strl5475Display[1] + " ]");
                    WriteOutputLine ("3 [ " + strl5475Display[2] + " ]");
                    WriteOutputLine ("4 [ " + strl5475Display[3] + " ]");
                    WriteOutputLine ("5 [ " + strl5475Display[4] + " ]");
                }
                else
                {
                    InvalidQByte ();
                    return;
                }
            }
            else
            {
                UnsupportedDevice ();
            }
        }

        // TIO - Test I/O and Branch
        private void ExectuteTIO (byte[] yaInstruction)
        {
            int  iOperandAddress = GetOneOperandAddress (yaInstruction);
            if (!IsAddressValid (iOperandAddress))
            {
                return;
            }

            byte yQByte         = yaInstruction[1],
                 yDeviceAddress = (byte)(yQByte & 0xF0),
                 yMCode         = (byte)(yQByte & 0x08),
                 yNCode         = (byte)(yQByte & 0x07);
            bool bConditionMet  = false;

            if (yDeviceAddress == (byte)EIODevice.DEV_5424_MFCU)
            {
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5203_Printer)
            {
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5444_Disk_1)
            {
                if (yNCode == 0x04) // Scan found
                {
                    bConditionMet = false;
                    FunctionNotImplemented ();
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5444_Disk_2)
            {
                if (yNCode == 0x04) // Scan found
                {
                    bConditionMet = false;
                    FunctionNotImplemented ();
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_Keyboard)
            {
                // Not supported by either keyboard
                InvalidQByte ();
                return;
            }
            else
            {
                UnsupportedDevice ();
                return;
            }

            if (bConditionMet)
            {
                m_iARR = m_iIAR;
                m_iIAR = iOperandAddress;
            }
            else
            {
                m_iARR = iOperandAddress;
            }
        }

        // SNS - Sense I/O
        private void ExectuteSNS (byte[] yaInstruction)
        {
            int  iOperandAddress = GetOneOperandAddress (yaInstruction);
            byte yQByte          = yaInstruction[1],
                 yDeviceAddress  = (byte)(yQByte & 0xF0),
                 yMCode          = (byte)(yQByte & 0x08),
                 yNCode          = (byte)(yQByte & 0x07);

            if (AddressWraps (iOperandAddress, 2))
            {
                AddressWraparound ();
                return;
            }

            if (yDeviceAddress == (byte)EIODevice.DEV_5410_Dials)
            {
                StoreRegister (m_iConsoleDialSetting, iOperandAddress);
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5424_MFCU)
            {
                if (yNCode == 0x00)
                {
                    // Special indicators for CE use - always zero for now
                    StoreRegister (0x0000, iOperandAddress);
                }
                else if (yNCode == 0x01)
                {
                    // Special indicators for CE use - always zero for now
                    StoreRegister (0x0000, iOperandAddress);
                }
                else if (yNCode == 0x03)
                {
                    // Statud indicators - low-order byte for check conditions, always zero as virtual hardware never malfunctions
                    int iStatusBytePair = 0x0000;

                    //if (m_obj5242MFCU.IsPrintBuffer1Busy ())
                    //{
                    //    // Print Buffer 1 Busy
                    //    iStatusBytePair |= 0x8000;
                    //}
                    //if (m_obj5242MFCU.IsPrintBuffer2Busy ())
                    //{
                    //    // Print Buffer 2 Busy
                    //    iStatusBytePair |= 0x4000;
                    //}
                    if (m_obj5242MFCU.IsCardInPrimaryWaitStation ())
                    {
                        iStatusBytePair |= 0x2000;
                    }
                    if (m_obj5242MFCU.IsCardInSecondaryWaitStation ())
                    {
                        iStatusBytePair |= 0x1000;
                    }
                    //if (m_obj5242MFCU.IsHopperCycleNotComplete ())
                    //{
                    //    // Hopper Cycle Not Complete
                    //    iStatusBytePair |= 0x0400;
                    //}
                    //if (m_obj5242MFCU.GetTransportCounterBit2 ())
                    //{
                    //    // Card in Transport/Counter - bit 2
                    //    iStatusBytePair |= 0x0200;
                    //}
                    //if (m_obj5242MFCU.GetTransportCounterBit1 ())
                    //{
                    //    // Card in Transport/Counter - bit 1
                    //    iStatusBytePair |= 0x0100;
                    //}

                    StoreRegister (iStatusBytePair, iOperandAddress);
                }
                else if (yNCode == 0x04)
                {
                    // MFCU print data address register
                    StoreRegister (m_i5424PrintDataAddressRegister, iOperandAddress);
                }
                else if (yNCode == 0x05)
                {
                    // MFCU read data address register
                    StoreRegister (m_i5424ReadDataAddressRegister, iOperandAddress);
                }
                else if (yNCode == 0x06)
                {
                    // MFCU punch data address register
                    StoreRegister (m_i5424PunchDataAddressRegister, iOperandAddress);
                }
                else
                {
                    InvalidQByte ();
                    return;
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5203_Printer)
            {
                if (yNCode == 0x00)
                {
                    // Line position - left carriage is rightmost/low-order byte
                    StoreRegister (m_obj5203LinePrinter.LinePosition << 8, iOperandAddress);
                }
                else if (yNCode == 0x01)
                {
                    // In-print-operation details - always zero for now
                    StoreRegister (0x0000, iOperandAddress);
                }
                else if (yNCode == 0x02)
                {
                    // Printer timing - always zero for now
                    StoreRegister (0x0000, iOperandAddress);
                }
                else if (yNCode == 0x03)
                {
                    // Printer check status
                    StoreRegister (0x0004, iOperandAddress);  // 48-character chain image
                }
                else if (yNCode == 0x04)
                {
                    // Printer image address
                    StoreRegister (m_i5203ChainImageAddressRegister, iOperandAddress);
                }
                else if (yNCode == 0x06)
                {
                    // Printer data address
                    StoreRegister (m_i5203DataAddressRegister, iOperandAddress);
                }
                else
                {
                    InvalidQByte ();
                    return;
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5444_Disk_1)
            {
                if (yNCode == 0x02)
                {
                    StoreRegister (m_iDiskStatusDrive1Bytes0and1, iOperandAddress);
                    m_iLastDiskSNS = m_iDiskStatusDrive1Bytes0and1;
                    m_iDiskStatusDrive1Bytes0and1 &= ~m_ki5444Byte0Bit0NoOp;
                    m_iDiskStatusDrive2Bytes0and1 &= ~m_ki5444Byte0Bit0NoOp;
                }
                else if (yNCode == 0x03)
                {
                    StoreRegister (m_iDiskStatusDrive1Bytes2and3, iOperandAddress);
                    m_iLastDiskSNS = m_iDiskStatusDrive1Bytes2and3;
                }
                else if (yNCode == 0x04)
                {
                    StoreRegister (m_iDiskReadWriteAddressRegister, iOperandAddress);
                    m_iLastDiskSNS = m_iDiskReadWriteAddressRegister;
                }
                else if (yNCode == 0x06)
                {
                    StoreRegister (m_iDiskControlAddressRegister, iOperandAddress);
                    m_iLastDiskSNS = m_iDiskControlAddressRegister;
                }
                else
                {
                    InvalidQByte ();
                    return;
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5444_Disk_2)
            {
                if (yNCode == 0x02)
                {
                    StoreRegister (m_iDiskStatusDrive2Bytes0and1, iOperandAddress);
                    m_iLastDiskSNS = m_iDiskStatusDrive2Bytes0and1;
                    m_iDiskStatusDrive1Bytes0and1 &= ~m_ki5444Byte0Bit0NoOp;
                    m_iDiskStatusDrive2Bytes0and1 &= ~m_ki5444Byte0Bit0NoOp;
                }
                else if (yNCode == 0x03)
                {
                    StoreRegister (m_iDiskStatusDrive2Bytes2and3, iOperandAddress);
                    m_iLastDiskSNS = m_iDiskStatusDrive2Bytes2and3;
                }
                else if (yNCode == 0x04)
                {
                    StoreRegister (m_iDiskReadWriteAddressRegister, iOperandAddress);
                    m_iLastDiskSNS = m_iDiskReadWriteAddressRegister;
                }
                else if (yNCode == 0x06)
                {
                    StoreRegister (m_iDiskControlAddressRegister, iOperandAddress);
                    m_iLastDiskSNS = m_iDiskControlAddressRegister;
                }
                else
                {
                    InvalidQByte ();
                    return;
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_Keyboard)
            {
                // Since both 5471 and 5475 keyboards have the same device address, the
                // keyboard object will need to determine which keyboard model it is.
                if (m_eKeyboard == EKeyboard.KEY_5471)
                {
                }
                else if (m_eKeyboard == EKeyboard.KEY_5475)
                {
                }
                else
                {
                    InvalidQByte ();
                    return;
                }
            }
            else
            {
                UnsupportedDevice ();
            }
        }

        // HPL - Halt Program Level
        private void ExectuteHPL (byte[] yaInstruction)
        {
            m_iHaltCount++;

            byte yLeftDisplay  = yaInstruction[1],
                 yRightDisplay = yaInstruction[2];

            if (yLeftDisplay  == 0x7C &&
                yRightDisplay == 0x63)
            {
                m_eProgramState = EProgramState.STATE_Halted;
            }
            List<string> strlHaltDisplay = GetHaltDisplay (yLeftDisplay, yRightDisplay);

            WriteOutputLine ("* * *  H A L T  * * *");
            WriteOutputLine ("1 [ " + strlHaltDisplay[0] + " ]");
            WriteOutputLine ("2 [ " + strlHaltDisplay[1] + " ]");
            WriteOutputLine ("3 [ " + strlHaltDisplay[2] + " ]");
            WriteOutputLine ("4 [ " + strlHaltDisplay[3] + " ]");
            WriteOutputLine ("5 [ " + strlHaltDisplay[4] + " ]");
            //PrintStringList ();
        }

        // APL - Advance Program Level
        private void ExectuteAPL (byte[] yaInstruction)
        {
            byte yQByte         = yaInstruction[1],
                 yControlCode   = yaInstruction[2],
                 yDeviceAddress = (byte)(yQByte & 0xF0),
                 yMCode         = (byte)(yQByte & 0x08),
                 yNCode         = (byte)(yQByte & 0x07);

            if (yDeviceAddress == (byte)EIODevice.DEV_5424_MFCU)
            {
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5203_Printer)
            {
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5444_Disk_1)
            {
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5444_Disk_2)
            {
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_Keyboard)
            {
                // Not supported by either keyboard
                InvalidQByte ();
                return;
            }
            else
            {
                UnsupportedDevice ();
            }
        }

        // JC - Jump On Condition
        private void ExectuteJC (byte[] yaInstruction)
        {
            byte yQByte = yaInstruction[1];
            byte yControlCode = yaInstruction[2];

            if (!m_aCR[m_iIL].IsNoOp (yQByte))
            {
                if (m_aCR[m_iIL].IsConditionTrue (yQByte))
                {
                    m_iARR = m_iIAR;
                    m_iIAR += yControlCode;

                    if (!IsAddressValid (m_iIAR))
                    {
                        return;
                    }
                }
            }
        }

        // SIO - Start I/O
        private void ExectuteSIO (byte[] yaInstruction)
        {
            byte yQByte         = yaInstruction[1],
                 yControlCode   = yaInstruction[2],
                 yDeviceAddress = (byte)(yQByte & 0xF0),
                 yMCode         = (byte)(yQByte & 0x08),
                 yNCode         = (byte)(yQByte & 0x07);

            if (yDeviceAddress == (byte)EIODevice.DEV_5424_MFCU)
            {
                bool bPrimaryHopper   = ((yQByte & 0x08) != 0x08),
                     bReadAction      = ((yQByte & 0x01) == 0x01),
                     bPunchAction     = ((yQByte & 0x02) == 0x02),
                     bPrintAction     = ((yQByte & 0x04) == 0x04),
                     bReadIPL         = ((yControlCode & 0x40) == 0x40),
                     bPrintFourthLine = ((yControlCode & 0x20) == 0x20);
                int iStackerNumber    = (int)(yControlCode & 0x07);
                if (iStackerNumber >= 4)
                {
                    iStackerNumber -= 4;
                }
                else
                {
                    iStackerNumber = bPrimaryHopper ? 1 : 4;
                }
                
                if (bReadAction)
                {
                    if (bReadIPL)
                    {
                        LoadBinaryImage ((bPrimaryHopper ? m_obj5242MFCU.ReadCardFromPrimaryIPL () : m_obj5242MFCU.ReadCardFromSecondaryIPL ()),
                                         m_i5424ReadDataAddressRegister, 0);
                    }
                    else
                    {
                        StoreString ((bPrimaryHopper ? m_obj5242MFCU.ReadCardFromPrimary () : m_obj5242MFCU.ReadCardFromSecondary ()),
                                     m_i5424ReadDataAddressRegister);
                    }
                }
                
                if (bPunchAction)
                {
                }
                
                if (bPrintAction)
                {
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5203_Printer)
            {
                if ((yQByte & 0x07) == 0x00) // Space only
                {
                    for (int iIdx = 0; iIdx < (int)yControlCode; iIdx++)
                    {
                        WriteOutputLine ("");
                    }
                    m_obj5203LinePrinter.SkipLines (yControlCode > 3 ? 0 : (int)yControlCode);
                }
                else if ((yQByte & 0x07) == 0x02) // Print and space
                {
                    WriteOutputLine (PrintEBCDIC ());
                    for (int iIdx = 1; iIdx < (int)yControlCode; iIdx++)
                    {
                        WriteOutputLine ("");
                    }
                    m_obj5203LinePrinter.SkipLines (yControlCode > 3 ? 0 : (int)yControlCode);
                }
                else if ((yQByte & 0x07) == 0x04) // Skip only
                {
                    int iLinesToSkip = m_obj5203LinePrinter.SkipToLine ((int)yControlCode);

                    while (iLinesToSkip-- > 0)
                    {
                        WriteOutputLine ("");
                    }
                }
                else if ((yQByte & 0x07) == 0x06) // Print and skip
                {
                    WriteOutputLine (PrintEBCDIC ());

                    int iLinesToSkip = m_obj5203LinePrinter.SkipToLine ((int)yControlCode);

                    while (iLinesToSkip-- > 0)
                    {
                        WriteOutputLine ("");
                    }
                }
                else
                {
                    InvalidQByte ();
                    return;
                }
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_5444_Disk_1 ||
                     yDeviceAddress == (byte)EIODevice.DEV_5444_Disk_2)
            {
                LoadDiskControlField ();
                m_objDiskControlField.SaveState ();

                // Reset Status Address A and Status Address B with each diks system SIO
                m_iDiskStatusDrive1Bytes0and1 &= ~(m_ki5444Byte1Bit6StatusAddressA | m_ki5444Byte1Bit7StatusAddressB);

                // Determine target from SIO
                bool bDrive1 = (yDeviceAddress == (byte)EIODevice.DEV_5444_Disk_1);
                bool bFixedDisk = (yMCode > 0);

                if (yNCode == 0) // Seek
                {
                    if (bDrive1)
                    {
                        m_bDrive1Head0Selection = ((m_objDiskControlField.yS & 0x80) == 0); // Head selection
                    }
                    else // Drive 2
                    {
                        m_bDrive2Head0Selection = ((m_objDiskControlField.yS & 0x80) == 0); // Head selection
                    }

                    if ((m_objDiskControlField.yS & 0x01) == 0 && // Toward cylinder 0
                        m_objDiskControlField.yN >= 224)          // Recalibration value
                    {
                        if (bDrive1)
                        {
                            m_iDrive1Cylinder = 0;
                            m_iDiskStatusDrive1Bytes0and1 |= m_ki5444Byte1Bit1CylinderZero;
                            m_bDrive1Head0Selection = false; // Head 0 always set by recallibration
                        }
                        else // Drive 2
                        {
                            m_iDrive2Cylinder = 0;
                            m_iDiskStatusDrive2Bytes0and1 |= m_ki5444Byte1Bit1CylinderZero;
                            m_bDrive2Head0Selection = false; // Head 0 always set by recallibration
                        }
                    }
                    else
                    {
                        if ((m_objDiskControlField.yS & 0x01) == 0) // Toward cylinder 0
                        {
                            if (bDrive1)
                            {
                                // Reset the "at cylinder 0" bit
                                m_iDiskStatusDrive1Bytes0and1 &= ~m_ki5444Byte1Bit1CylinderZero;

                                m_iDrive1Cylinder -= m_objDiskControlField.yN;
                                if (m_iDrive1Cylinder <= 0)
                                {
                                    m_iDrive1Cylinder = 0;
                                    m_iDiskStatusDrive1Bytes0and1 |= m_ki5444Byte1Bit1CylinderZero;
                                }
                            }
                            else // Drive 2
                            {
                                // Reset the "at cylinder 0" bit
                                m_iDiskStatusDrive2Bytes0and1 &= ~m_ki5444Byte1Bit1CylinderZero;

                                m_iDrive2Cylinder -= m_objDiskControlField.yN;
                                if (m_iDrive2Cylinder <= 0)
                                {
                                    m_iDrive2Cylinder = 0;
                                    m_iDiskStatusDrive2Bytes0and1 |= m_ki5444Byte1Bit1CylinderZero;
                                }
                            }
                        }
                        else // Away from cylinder 0
                        {
                            if (bDrive1)
                            {
                                // Reset the "at cylinder 0" bit
                                m_iDiskStatusDrive1Bytes0and1 &= ~m_ki5444Byte1Bit1CylinderZero;

                                m_iDrive1Cylinder += m_objDiskControlField.yN;
                                if (m_iDrive1Cylinder > 203)
                                {
                                    m_iDrive1Cylinder = 0;
                                    m_iDiskStatusDrive1Bytes0and1 |= m_ki5444Byte0Bit7SeekCheck;
                                }
                            }
                            else // Drive 2
                            {
                                // Reset the "at cylinder 0" bit
                                m_iDiskStatusDrive2Bytes0and1 &= ~m_ki5444Byte1Bit1CylinderZero;

                                m_iDrive2Cylinder += m_objDiskControlField.yN;
                                if (m_iDrive2Cylinder > 203)
                                {
                                    m_iDrive2Cylinder = 0;
                                    m_iDiskStatusDrive2Bytes0and1 |= m_ki5444Byte0Bit7SeekCheck;
                                }
                            }
                        }
                    }

                    m_strSIODetails = string.Format ("SIO   Seek to cylinder {0:D}, head {1:D}",
                                                      bDrive1 ? m_iDrive1Cylinder : m_iDrive2Cylinder,
                        (                            (yQByte & 0x80) == 0) ? 0 : 1);
                }
                else if (yNCode == 1 && (yControlCode & 0x03) == 0 || // Read Data
                         yNCode == 2 && (yControlCode & 0x01) == 0)   // Write Data
                {
                    // Use the same code for reading and for writing, since the only difference is the
                    // direction of the data flow.  Address calculation and all else is identical.
                    bool bWrite = (yNCode == 2 && (yControlCode & 0x01) == 0);
                    int iSectorNumber = (m_objDiskControlField.yS >> 2) & 0x3F;
                    int iCylinder = bDrive1 ? m_iDrive1Cylinder : m_iDrive2Cylinder;

                    // Error detection
                    if ((iSectorNumber > 23 &&
                         iSectorNumber < 32) ||
                        iSectorNumber > 55)
                    {
                        // Invalid sector #
                        if (bDrive1)
                        {
                            m_iDiskStatusDrive1Bytes0and1 |= m_ki5444Byte0Bit5NoRecordFound;
                        }
                        else
                        {
                            m_iDiskStatusDrive2Bytes0and1 |= m_ki5444Byte0Bit5NoRecordFound;
                        }

                        m_strSIODetails = string.Format ("SIO   Read invalid sector {0:D}", iSectorNumber);
                        return;
                    }
                    if (iCylinder > 203)
                    {
                        // Invalid cylinder #
                        if (bDrive1)
                        {
                            m_iDiskStatusDrive1Bytes0and1 |= m_ki5444Byte1Bit2EndOfCylinder;
                        }
                        else
                        {
                            m_iDiskStatusDrive2Bytes0and1 |= m_ki5444Byte1Bit2EndOfCylinder;
                        }

                        m_strSIODetails = string.Format ("SIO   Read invalid cyclinder {0:D}", iCylinder);
                        return;
                    }

                    // Sector number 00 - 23, 32 - 55
                    int iRawSectorNumber = iSectorNumber;
                    if (iSectorNumber > 23)
                    {
                        iSectorNumber -= 8;
                    }

                    m_strSIODetails = string.Format ("SIO   Read {0:D} sectors starting at {1:D} on cylinder {2:D}",
                                                     m_objDiskControlField.yN + 1, iSectorNumber, iCylinder);

                    // Determine number of sectors to read
                    int iLastSectorsToRead = m_objDiskControlField.yN + iSectorNumber;

                    m_objDumpData.iStart = m_iDiskReadWriteAddressRegister;

                    for (; iSectorNumber <= iLastSectorsToRead; ++iSectorNumber)
                    {
                        if (iSectorNumber > 47 || // Already read last sector in cylinder
                            m_objDiskControlField.yN == 0xFF)  // Sector count indicates completion
                        {
                            // We're done.  No more reading for now.
                            break;
                        }

                        // Determine starting offset in disk image file
                        int iFileOffset = (m_kiBytesPerCyliner * iCylinder) + (m_kiSectorSize * iSectorNumber);

                        m_objDiskControlField.yS = (byte)(iSectorNumber & 0x00FF); // S field contains ID of last sector read (p.6-12)
                        --m_objDiskControlField.yN; // Decrement N byte at the start of each sector

                        // Copy data to destination
                        if (bWrite)
                        {
                            byte[] yaSector = new byte[m_kiSectorSize];

                            for (int iIdx = 0; iIdx < m_kiSectorSize; ++iIdx)
                            {
                                yaSector[iIdx] = m_yaMainMemory[m_iDiskReadWriteAddressRegister + iIdx];
                            }

                            m_obj5444DiskDrive.WriteSector (iFileOffset, bFixedDisk, bDrive1, yaSector);
                        }
                        else
                        {
                            byte[] yaSector = m_obj5444DiskDrive.ReadSector (iFileOffset, bFixedDisk, bDrive1);

                            //strlSectorReading.Add (string.Format ("Sect: {0:D2} ({1:D2}), cyl: {2:D}, offset: 0x{3:x8}",
                            //                                      iRawSectorNumber++, iSectorNumber, iCylinder, iFileOffset));
                            //DumpSector (yaSector, strlSectorReading);

                            for (int iByteIdx = 0; iByteIdx < m_kiSectorSize; ++iByteIdx)
                            {
                                m_yaMainMemory[m_iDiskReadWriteAddressRegister + iByteIdx] = yaSector[iByteIdx];
                            }
                        }

                        // Update Disk Read/Write Address Register
                        m_iDiskReadWriteAddressRegister += m_kiSectorSize;
                        m_objDumpData.iLength += m_kiSectorSize;
                    }
                }
                else if (yNCode == 1 && (yControlCode & 0x03) == 1) // Read Identifier
                {
                }
                else if (yNCode == 1 && (yControlCode & 0x03) == 2) // Read Data Diagnostic
                {
                }
                else if (yNCode == 1 && (yControlCode & 0x03) == 3) // Verify
                {
                }
                else if (yNCode == 2 && (yControlCode & 0x01) == 1) // Write Identifier
                {
                }
                else if (yNCode == 3 && (yControlCode & 0x03) == 0) // Scan Equal
                {
                }
                else if (yNCode == 3 && (yControlCode & 0x03) == 1) // Scan Low or Equal
                {
                }
                else if (yNCode == 3 && (yControlCode & 0x02) == 2) // Scan High or Equal
                {
                }
                else 
                {
                    InvalidQByte ();
                    return;
                }

                StoreDiskControlField ();
            }
            else if (yDeviceAddress == (byte)EIODevice.DEV_Keyboard)
            {
                // Since both 5471 and 5475 keyboards have the same device address, the
                // keyboard object will need to determine which keyboard model it is.
            }
            else
            {
                UnsupportedDevice ();
            }
        }
        #endregion

        #region Instruction support methods
        protected byte[] FetchInstruction ()
        {
            int iInstructionLength = 0;

            // Fetch op code
            byte yOpCode = m_yaMainMemory[m_iIAR];
            //byte yOpCode = yaObjectCode1[m_iIAR];

            // Determine instruction length from instruction format
            if ((yOpCode & 0xF0) == 0xF0)
            {
                // Command format: 3 bytes
                iInstructionLength = 3;
            }
            else if ((yOpCode & 0x30) == 0x30)
            {
                // Y format, either 3 or 4 bytes
                if ((yOpCode & 0xC0) == 0)
                {
                    iInstructionLength = 4; // No index registers: 4 bytes
                }
                else
                {
                    iInstructionLength = 3; // One index registers: 3 bytes
                }
            }
            else if ((yOpCode & 0xC0) == 0xC0)
            {
                // Z format, either 3 or 4 bytes
                if ((yOpCode & 0x30) == 0)
                {
                    iInstructionLength = 4; // No index registers: 4 bytes
                }
                else
                {
                    iInstructionLength = 3; // One index registers: 3 bytes
                }
            }
            else // X format, 4 to 6 bytes
            {
                bool bXR1 = ((yOpCode & 0xC0) > 0);
                bool bXR2 = ((yOpCode & 0x30) > 0);

                iInstructionLength = 6; // If no index registers are used

                if (bXR1)
                {
                    iInstructionLength--; // First operand is indexed
                }

                if (bXR2)
                {
                    iInstructionLength--; // Second operand is indexed
                }
            }

            // Create buffer for instruction
            byte[] yaInstruction = new byte[iInstructionLength];

            // Fetch instruction
            for (int iIdx = 0; iIdx < iInstructionLength; iIdx++)
            {
                yaInstruction[iIdx] = m_yaMainMemory[m_iIAR + iIdx];
                //yaInstruction[iIdx] = yaObjectCode1[m_iIAR + iIdx];
            }

            // Increment IAR by instruction length
            m_iIAR += iInstructionLength;

            return yaInstruction;
        }

        protected int GetOneOperandAddress (byte[] yaInstruction)
        {
            byte yOpCode = yaInstruction[0];
            int iOperandAddress = 0;

            if ((yOpCode & 0x30) == 0x30)
            {
                // Y format, either 3 or 4 bytes
                if ((yOpCode & 0xC0) == 0)
                {
                    // No index registers: 4 bytes
                    iOperandAddress = yaInstruction[2];
                    iOperandAddress *= 0x0100;
                    iOperandAddress += yaInstruction[3];
                }
                else
                {
                    if ((yOpCode & 0xC0) == 0x40)
                    {
                        // Use index register 1
                        iOperandAddress = m_iXR1;
                        iOperandAddress += yaInstruction[2];
                        iOperandAddress &= 0x0FFFF;
                    }
                    else if ((yOpCode & 0xC0) == 0x80)
                    {
                        // Use index register 2
                        iOperandAddress = m_iXR2;
                        iOperandAddress += yaInstruction[2];
                        iOperandAddress &= 0x0FFFF;
                    }
                    else
                    {
                        // This code block should NEVER be entered
                        WaveRedFlag ("Undefined index register - Y format");
                    }
                }
            }
            else if ((yOpCode & 0xC0) == 0xC0)
            {
                // Z format, either 3 or 4 bytes
                if ((yOpCode & 0x30) == 0)
                {
                    // No index registers: 4 bytes
                    iOperandAddress = yaInstruction[2];
                    iOperandAddress *= 0x0100;
                    iOperandAddress += yaInstruction[3];
                }
                else
                {
                    if ((yOpCode & 0x30) == 0x10)
                    {
                        // Use index register 1
                        iOperandAddress = m_iXR1;
                        iOperandAddress += yaInstruction[2];
                        iOperandAddress &= 0x0FFFF;
                    }
                    else if ((yOpCode & 0x30) == 0x20)
                    {
                        // Use index register 2
                        iOperandAddress = m_iXR2;
                        iOperandAddress += yaInstruction[2];
                        iOperandAddress &= 0x0FFFF;
                    }
                    else
                    {
                        // This code block should NEVER be entered
                        WaveRedFlag ("Undefined index register - Z format");
                    }
                }
            }

            return iOperandAddress;
        }

        protected CTwoOperandAddress GetTwoOperandAddress (byte[] yaInstruction)
        {
            byte yOpCode = yaInstruction[0];
            int iOperandOneAddress = 0;
            int iOperandTwoAddress = 0;

            byte yOperandOneXR = (byte)(yaInstruction[0] & 0xC0);
            byte yOperandTwoXR = (byte)(yaInstruction[0] & 0x30);
            int iOperandTwoAddressOffset = (yOperandOneXR == 0) ? 4 : 3;

            // Computer operand 1 address
            if (yOperandOneXR == 0)
            {
                // No index register
                iOperandOneAddress = yaInstruction[2];
                iOperandOneAddress *= 0x0100;
                iOperandOneAddress += yaInstruction[3];
            }
            else if (yOperandOneXR == 0x40)
            {
                // Use index register 1
                iOperandOneAddress = m_iXR1;
                iOperandOneAddress += yaInstruction[2];
            }
            else if (yOperandOneXR == 0x80)
            {
                // Use index register 2
                iOperandOneAddress = m_iXR2;
                iOperandOneAddress += yaInstruction[2];
            }
            else
            {
                // This code block should NEVER be entered
                WaveRedFlag ("Undefined operand 1 index register - X format");
            }

            // Computer operand 2 address
            if (yOperandTwoXR == 0)
            {
                // No index register
                iOperandTwoAddress = yaInstruction[iOperandTwoAddressOffset];
                iOperandTwoAddress *= 0x0100;
                iOperandTwoAddress += yaInstruction[iOperandTwoAddressOffset + 1];
            }
            else if (yOperandTwoXR == 0x10)
            {
                // Use index register 1
                iOperandTwoAddress = m_iXR1;
                iOperandTwoAddress += yaInstruction[iOperandTwoAddressOffset];
            }
            else if (yOperandTwoXR == 0x20)
            {
                // Use index register 2
                iOperandTwoAddress = m_iXR2;
                iOperandTwoAddress += yaInstruction[iOperandTwoAddressOffset];
            }
            else
            {
                // This code block should NEVER be entered
                WaveRedFlag ("Undefined operand 2 index register - X format");
            }

            return new CTwoOperandAddress (iOperandOneAddress & 0xFFFF, iOperandTwoAddress & 0xFFFF);
        }

        protected int CompareZoned (int iOperandOneAddress, int iOperandOneLength, int iOperandTwoAddress, int iOperandTwoLength, bool bIgnoreSign)
        {
            int iOperandOnePointer = iOperandOneAddress - iOperandOneLength + 1,
                iOperandTwoPointer = iOperandTwoAddress - iOperandTwoLength + 1;

            if (!bIgnoreSign)
            {
                bool bOperandOneNegative = ((m_yaMainMemory[iOperandOneAddress] & 0xF0) == (byte)0xD0),
                     bOperandTwoNegative = ((m_yaMainMemory[iOperandTwoAddress] & 0xF0) == (byte)0xD0);

                if (bOperandOneNegative != bOperandTwoNegative)
                {
                    return (bOperandOneNegative ? -1 : 1);
                }
            }

            // Signs match or are ignored; proceed with the digit-by-digit comparison
            while (iOperandOnePointer <= iOperandOneAddress)
            {
                if ((iOperandOneAddress - iOperandOnePointer) > (iOperandTwoAddress - iOperandTwoPointer))
                {
                    if ((m_yaMainMemory[iOperandOnePointer] & (byte)0x0F) > 0x00)
                    {
                        return 1; // Operand 1 is greater because it has a significant digit before the leftmost digit in operand 2
                    }
                    iOperandOnePointer++;
                }
                else
                {
                    byte yOperandOneDigit = (byte)(m_yaMainMemory[iOperandOnePointer] & (byte)0x0F),
                         yOperandTwoDigit = (byte)(m_yaMainMemory[iOperandTwoPointer] & (byte)0x0F);

                    if (yOperandOneDigit > yOperandTwoDigit)
                    {
                        return 1; // Operand 1 is greater than operand 2
                    }
                    else if (yOperandOneDigit < yOperandTwoDigit)
                    {
                        return -1; // Operand 1 is less than operand 2
                    }

                    iOperandOnePointer++;
                    iOperandTwoPointer++;
                }
            }

            return 0; // Equal value
        }

        private void ExecuteDecimalArithmetic (byte[] yaInstruction, bool bAddition)
        {
            CTwoOperandAddress objTOA = GetTwoOperandAddress (yaInstruction);
            byte yQByte = yaInstruction[1];
            int iOperandTwoLength = (yQByte & 0x0F) + 1,
                iOperandOneLength = (yQByte >> 4) + iOperandTwoLength;

            if (ZonedAddressesValid (objTOA, yQByte))
            {
                int iOperandOneAddress = objTOA.OperandOneAddress,
                    iOperandTwoAddress = objTOA.OperandTwoAddress;

                // Check for invalid operand overlap
                if (iOperandOneAddress < iOperandTwoAddress &&
                    iOperandOneAddress > iOperandTwoAddress - iOperandTwoLength)
                {
                    InvalidQByte ();
                    return;
                }

                bool bOperandOneNegative =              (m_yaMainMemory[iOperandOneAddress] & 0xF0) == (byte)0xD0,
                     bOperandTwoNegative = bAddition ? ((m_yaMainMemory[iOperandTwoAddress] & 0xF0) == (byte)0xD0) :
                                                       ((m_yaMainMemory[iOperandTwoAddress] & 0xF0) != (byte)0xD0),
                     bSubtract           = bOperandOneNegative != bOperandTwoNegative,
                     bSwapOperands       = false,
                     bCarry              = false,
                     bNextCarry          = false,
                     bResultZero         = true,
                     bResultNegative     = bOperandOneNegative && bOperandTwoNegative;

                if (bSubtract)
                {
                    int iCompare = CompareZoned (iOperandOneAddress, iOperandOneLength, iOperandTwoAddress, iOperandTwoLength, true);
                    bSwapOperands = iCompare < 0;
                    if ((iCompare > 0 && bOperandOneNegative) ||
                        (iCompare < 0 && bOperandTwoNegative))
                    {
                        bResultNegative = true;
                    }
                }

                // Do the math
                for (int iOffset = 0; iOffset < iOperandOneLength; iOffset++)
                {
                    byte yOperandOneDigit = (byte)((m_yaMainMemory[iOperandOneAddress - iOffset] & 0x0F) % 10),
                         yOperandTwoDigit = (byte)((iOffset < iOperandTwoLength) ?
                                            ((m_yaMainMemory[iOperandTwoAddress - iOffset] & 0x0F) % 10) : 0x00);

                    if (bSubtract)
                    {
                        if (bSwapOperands)
                        {
                            byte yHold = yOperandOneDigit;
                            yOperandOneDigit = yOperandTwoDigit;
                            yOperandTwoDigit = yHold;
                        }

                        if (yOperandOneDigit < yOperandTwoDigit ||
                            (bCarry &&
                             yOperandOneDigit == yOperandTwoDigit))
                        {
                            yOperandOneDigit += (byte)10;
                            bNextCarry = true;
                        }

                        int iResult = yOperandOneDigit - yOperandTwoDigit;
                        if (bCarry)
                        {
                            iResult--;
                            bCarry = false;
                        }

                        bCarry = bNextCarry;
                        bNextCarry = false;

                        m_yaMainMemory[iOperandOneAddress - iOffset] = (byte)(iResult & 0xFF);
                    }
                    else
                    {
                        int iResult = yOperandOneDigit + yOperandTwoDigit;
                        if (bCarry)
                        {
                            iResult++;
                            bCarry = false;
                        }

                        if (iResult > 9)
                        {
                            iResult = iResult % 10;
                            bCarry = true;
                        }

                        m_yaMainMemory[iOperandOneAddress - iOffset] = (byte)(iResult & 0xFF);
                    }
                }

                // Set all the zones
                for (int iOffset = 0; iOffset < iOperandOneLength; iOffset++)
                {
                    m_yaMainMemory[iOperandOneAddress - iOffset] |= (byte)0xF0;
                    if ((m_yaMainMemory[iOperandOneAddress - iOffset] & (byte)0x0F) > 0)
                    {
                        bResultZero = false;
                        m_iARR = iOperandOneAddress - iOffset;
                    }
                }

                // Set the sign zone
                if (bResultNegative)
                {
                    m_yaMainMemory[iOperandOneAddress] &= (byte)0xDF;
                    m_yaMainMemory[iOperandOneAddress] |= (byte)0xD0;
                }

                // Set the condition register
                if (bResultZero)
                {
                    m_aCR[m_iIL].SetEqual ();
                    m_iARR = iOperandOneAddress + 1; // No significant digits
                }
                else if (bResultNegative)
                {
                    m_aCR[m_iIL].SetLow ();
                }
                else
                {
                    m_aCR[m_iIL].SetHigh ();
                }

                if (bCarry)
                {
                    m_aCR[m_iIL].SetDecimalOverflow ();
                }
            }
        }

        private int LoadInt (int iAddress)
        {
            return (m_yaMainMemory[iAddress - 1] << 8) + (m_yaMainMemory[iAddress]);
        }

        private void LoadRegister (int iRegister, int iTargetAddress)
        {
            if (AddressWraps (iTargetAddress, 2))
            {
                AddressWraparound ();
            }
            else
            {
                iRegister  = (int)m_yaMainMemory[iTargetAddress - 1] * 0x0100;
                iRegister += (int)m_yaMainMemory[iTargetAddress];
            }
        }

        private void StoreRegister (int iRegister, int iTargetAddress)
        {
            if (AddressWraps (iTargetAddress, 2))
            {
                AddressWraparound ();
            }
            else
            {
                m_yaMainMemory[iTargetAddress - 1] = (byte)(iRegister >> 8);
                m_yaMainMemory[iTargetAddress]     = (byte)(iRegister & 0xFF);
            }
        }

        private string LoadString (int iStartAddress, int iLength)
        {
            StringBuilder strbldrData = new StringBuilder (iLength);

            for (int iIdx = 0; iIdx < iLength; iIdx++)
            {
                strbldrData.Append ((char)m_yaMainMemory[iStartAddress + iIdx]);
            }

            return strbldrData.ToString ();
        }

        private void StoreString (string strData, int iStartAddress)
        {
            for (int iIdx = 0; iIdx < strData.Length; iIdx++)
            {
                m_yaMainMemory[iStartAddress + iIdx] = (byte)strData[iIdx];
            }
        }

        private void LoadDiskControlField ()
        {
            m_objDiskControlField.yF = m_yaMainMemory[m_iDiskControlAddressRegister];
            m_objDiskControlField.yC = m_yaMainMemory[m_iDiskControlAddressRegister + 1];
            m_objDiskControlField.yS = m_yaMainMemory[m_iDiskControlAddressRegister + 2];
            m_objDiskControlField.yN = m_yaMainMemory[m_iDiskControlAddressRegister + 3];
        }

        private void StoreDiskControlField ()
        {
            m_yaMainMemory[m_iDiskControlAddressRegister]     = m_objDiskControlField.yF;
            m_yaMainMemory[m_iDiskControlAddressRegister + 1] = m_objDiskControlField.yC;
            m_yaMainMemory[m_iDiskControlAddressRegister + 2] = m_objDiskControlField.yS;
            m_yaMainMemory[m_iDiskControlAddressRegister + 3] = m_objDiskControlField.yN;
        }

        private string LoadStringEBCDIC (int iStartAddress, int iLength)
        {
            StringBuilder strbldrData = new StringBuilder (iLength);

            for (int iIdx = 0; iIdx < iLength; iIdx++)
            {
                strbldrData.Append ((char)ConvertEBCDICtoASCII (m_yaMainMemory[iStartAddress + iIdx]));
            }

            return strbldrData.ToString ();
        }

        private void StoreStringEBCDIC (string strData, int iStartAddress)
        {
            for (int iIdx = 0; iIdx < strData.Length; iIdx++)
            {
                m_yaMainMemory[iStartAddress + iIdx] = ConvertASCIItoEBCDIC ((byte)strData[iIdx]);
            }
        }

        private string PrintEBCDIC ()
        {
            StringBuilder strbldrPrintEBCDIC = new StringBuilder (m_i5203LineWidth);

            for (int iIdx = 0; iIdx < m_i5203LineWidth; iIdx++)
            {
                //byte yEBCDIC = m_yaMainMemory[m_i5203DataAddressRegister + iIdx];
                //strbldrPrintEBCDIC.Append (yEBCDIC > 0x00 ? (char)yEBCDIC : ' ');
                char cASCII  = (char)ConvertEBCDICtoASCII (m_yaMainMemory[m_i5203DataAddressRegister + iIdx]);
                strbldrPrintEBCDIC.Append (cASCII);
                if (m_obj5203LinePrinter.IsInChainImage (cASCII))
                {
                    m_yaMainMemory[m_i5203DataAddressRegister + iIdx] = 0x40;
                }
            }

            return strbldrPrintEBCDIC.ToString ();
        }

        public void LoadBinaryImage (byte[] yaProgramImage, int iStartAddress, int iLength)
        {
            if (iLength <= 0)
            {
                iLength = yaProgramImage.Length;
            }

            for (int iIdx = 0; iIdx < iLength; iIdx++)
            {
                m_yaMainMemory[iStartAddress + iIdx] = yaProgramImage[iIdx % yaProgramImage.Length];
            }
        }
        #endregion

        #region Debugger methods
        private string FormatInstructionLine (string strInstruction, byte[] yaInstruction, int iIAR)
        {
            List<string> strlInstruction = DisassembleCode (yaInstruction, 0, yaInstruction.Length - 1);
            if ((yaInstruction[0] & 0xF0) == 0xF0) // Command instruction
            {
                int iColon = strInstruction.IndexOf (':');
                //Console.WriteLine (strInstruction);
                //Console.WriteLine (strInstruction.Substring (iColon + 2, 14));
                return string.Format ("{0:X4}: {1:S}", iIAR, strInstruction.Substring (iColon + 2, 14));
            }
            else if (((yaInstruction[0] & 0xC0) == 0xC0) || // One-operand instruction
                     ((yaInstruction[0] & 0x30) == 0x30))
            {
                int iColon = strInstruction.IndexOf (':');
                //Console.WriteLine (strInstruction);
                //Console.WriteLine (strInstruction.Substring (iColon + 2, 16));
                int iOperandAddress = GetOneOperandAddress (yaInstruction);
                return string.Format ("{0:X4}: {1:S}       {2:X2} {3:X2}",
                                      iIAR,
                                      strInstruction.Substring (iColon + 2, 16),
                                      iOperandAddress >> 8,
                                      iOperandAddress & 0x00FF);
            }
            else // Two-operand instruction
            {
                int iColon = strInstruction.IndexOf (':');
                //Console.WriteLine (strInstruction);
                //Console.WriteLine (strInstruction.Substring (iColon + 2, 21));
                CTwoOperandAddress obj2O = GetTwoOperandAddress (yaInstruction);
                return string.Format ("{0:X4}: {1:S}  {2:X2} {3:X2}  {4:X2} {5:X2}",
                                      iIAR,
                                      strInstruction.Substring (iColon + 2, 21),
                                      obj2O.OperandOneAddress >> 8,
                                      obj2O.OperandOneAddress & 0x00FF,
                                      obj2O.OperandTwoAddress >> 8,
                                      obj2O.OperandTwoAddress & 0x00FF);
            }
        }

        private string GetConditionFlags (CConditionRegister cr)
        {
            StringBuilder strbldrFlags = new StringBuilder (16);

            if (cr.IsEqual ())
            {
                strbldrFlags.Append ("EQ");
            }
            else if (cr.IsHigh ())
            {
                strbldrFlags.Append ("HI");
            }
            else
            {
                strbldrFlags.Append ("LO");
            }

            strbldrFlags.Append (cr.IsDecimalOverflow () ? " DO" : "   ");
            strbldrFlags.Append (cr.IsTestFalse ()       ? " TF" : "   ");
            strbldrFlags.Append (cr.IsBinaryOverflow ()  ? " BO" : "   ");

            return strbldrFlags.ToString ();
        }
        #endregion

        #region 7-Segment Display Support
        //  Halt Codes:
        //  0x04   ---
        //  0x08  |   |  0x02
        //  0x10   ---
        //  0x20  |   |  0x01
        //  0x40   ---

        //  5475 Codes
        //  0x80   ---
        //  0x40  |   |  0x20
        //  0x10   ---   
        //  0x08  |   |  0x04
        //  0x02   ---

        protected List<string> GetHaltDisplay (byte yLeftDisplay, byte yRightDisplay)
        {
            List<string> strlHaltDisplay = new List<string> (5);

            strlHaltDisplay.Add (string.Format (" {0:S1}    {1:S1} ",           (yLeftDisplay  & 0x04) == 0x04 ? "-" : " ",
                                                                                (yRightDisplay & 0x04) == 0x04 ? "-" : " "));

            strlHaltDisplay.Add (string.Format ("{0:S1} {1:S1}  {2:S1} {3:S1}", (yLeftDisplay  & 0x08) == 0x08 ? "|" : " ",
                                                                                (yLeftDisplay  & 0x02) == 0x02 ? "|" : " ",
                                                                                (yRightDisplay & 0x08) == 0x08 ? "|" : " ",
                                                                                (yRightDisplay & 0x02) == 0x02 ? "|" : " "));

            strlHaltDisplay.Add (string.Format (" {0:S1}    {1:S1} ",           (yLeftDisplay  & 0x10) == 0x10 ? "-" : " ",
                                                                                (yRightDisplay & 0x10) == 0x10 ? "-" : " "));

            strlHaltDisplay.Add (string.Format ("{0:S1} {1:S1}  {2:S1} {3:S1}", (yLeftDisplay  & 0x20) == 0x20 ? "|" : " ",
                                                                                (yLeftDisplay  & 0x01) == 0x01 ? "|" : " ",
                                                                                (yRightDisplay & 0x20) == 0x20 ? "|" : " ",
                                                                                (yRightDisplay & 0x01) == 0x01 ? "|" : " "));

            strlHaltDisplay.Add (string.Format (" {0:S1}    {1:S1} ",           (yLeftDisplay  & 0x40) == 0x40 ? "-" : " ", 
                                                                                (yRightDisplay & 0x40) == 0x40 ? "-" : " "));

            return strlHaltDisplay;
        }

        protected List<string> Get5475Display (byte yLeftDisplay, byte yRightDisplay)
        {
            List<string> strl5475Display = new List<string> (5);

            strl5475Display.Add (string.Format (" {0:S1}    {1:S1} ",           (yLeftDisplay  & 0x80) == 0x80 ? "-" : " ",
                                                                                (yRightDisplay & 0x80) == 0x80 ? "-" : " "));

            strl5475Display.Add (string.Format ("{0:S1} {1:S1}  {2:S1} {3:S1}", (yLeftDisplay  & 0x40) == 0x40 ? "|" : " ",
                                                                                (yLeftDisplay  & 0x20) == 0x20 ? "|" : " ",
                                                                                (yRightDisplay & 0x40) == 0x40 ? "|" : " ",
                                                                                (yRightDisplay & 0x20) == 0x20 ? "|" : " "));

            strl5475Display.Add (string.Format (" {0:S1}    {1:S1} ",           (yLeftDisplay  & 0x10) == 0x10 ? "-" : " ",
                                                                                (yRightDisplay & 0x10) == 0x10 ? "-" : " "));

            strl5475Display.Add (string.Format ("{0:S1} {1:S1}  {2:S1} {3:S1}", (yLeftDisplay  & 0x08) == 0x08 ? "|" : " ",
                                                                                (yLeftDisplay  & 0x04) == 0x04 ? "|" : " ",
                                                                                (yRightDisplay & 0x08) == 0x08 ? "|" : " ",
                                                                                (yRightDisplay & 0x04) == 0x04 ? "|" : " "));

            strl5475Display.Add (string.Format (" {0:S1}    {1:S1} ",           (yLeftDisplay  & 0x02) == 0x02 ? "-" : " ",
                                                                                (yRightDisplay & 0x02) == 0x02 ? "-" : " "));

            return strl5475Display;
        }

        protected byte ConvertHaltCodeTo5745 (byte yHaltCode)
        {
            byte y5475Code = 0x00;

            if ((yHaltCode & 0x01) == (byte)0x01)
            {
                y5475Code |= (byte)0x04;
            }
            if ((yHaltCode & 0x02) == (byte)0x02)
            {
                y5475Code |= (byte)0x20;
            }
            if ((yHaltCode & 0x04) == (byte)0x04)
            {
                y5475Code |= (byte)0x80;
            }
            if ((yHaltCode & 0x08) == (byte)0x08)
            {
                y5475Code |= (byte)0x40;
            }
            if ((yHaltCode & 0x10) == (byte)0x10)
            {
                y5475Code |= (byte)0x10;
            }
            if ((yHaltCode & 0x20) == (byte)0x20)
            {
                y5475Code |= (byte)0x08;
            }
            if ((yHaltCode & 0x40) == (byte)0x40)
            {
                y5475Code |= (byte)0x02;
            }

            return y5475Code;
        }

        protected byte Convert5745CodeToHalt (byte y5475Code)
        {
            byte yHaltCode = 0x00;

            if ((y5475Code & 0x02) == (byte)0x02)
            {
                yHaltCode |= (byte)0x40;
            }
            if ((y5475Code & 0x04) == (byte)0x04)
            {
                yHaltCode |= (byte)0x01;
            }
            if ((y5475Code & 0x08) == (byte)0x08)
            {
                yHaltCode |= (byte)0x20;
            }
            if ((y5475Code & 0x10) == (byte)0x10)
            {
                yHaltCode |= (byte)0x10;
            }
            if ((y5475Code & 0x20) == (byte)0x20)
            {
                yHaltCode |= (byte)0x02;
            }
            if ((y5475Code & 0x40) == (byte)0x40)
            {
                yHaltCode |= (byte)0x08;
            }
            if ((y5475Code & 0x80) == (byte)0x80)
            {
                yHaltCode |= (byte)0x04;
            }

            return yHaltCode;
        }
        #endregion

        #region Validation methods
        private bool AddressWraps (int iAddress, int iLength)
        {
            if (!IsAddressValid (iAddress))
            {
                return false;
            }

            if (iAddress - (iLength - 1) >= 0)
            {
                return false;
            }
            else
            {
                m_eProgramState = EProgramState.STATE_PChk_AddressWrap;
                return true;
            }
        }

        private bool IsAddressValid (int iAddress)
        {
            if (iAddress < m_yaMainMemory.Length)
            {
                return true;
            }
            else
            {
                InvalidAddress ();
                return false;
            }
        }

        private bool BinaryAddressesValid (CTwoOperandAddress objTOA, byte yQByte)
        {
            if (!IsAddressValid (objTOA.OperandOneAddress) ||
                !IsAddressValid (objTOA.OperandTwoAddress))
            {
                return false;
            }

            if (AddressWraps (objTOA.OperandOneAddress, (int)yQByte) ||
                AddressWraps (objTOA.OperandTwoAddress, (int)yQByte))
            {
                AddressWraparound ();
                return false;
            }

            return true;
        }

        private bool ZonedAddressesValid (CTwoOperandAddress objTOA, byte yQByte)
        {
            if (!IsAddressValid (objTOA.OperandOneAddress) ||
                !IsAddressValid (objTOA.OperandTwoAddress))
            {
                return false;
            }

            int iOperandTwoLength = (yQByte & 0x0F) + 1,
                iOperandOneLength = (yQByte >> 4) + iOperandTwoLength;

            if (iOperandOneLength > 31 ||
                iOperandTwoLength > 16)
            {
                InvalidQByte ();
                return false;
            }

            if (AddressWraps (objTOA.OperandOneAddress, iOperandOneLength + iOperandOneLength) ||
                AddressWraps (objTOA.OperandTwoAddress, iOperandTwoLength))
            {
                AddressWraparound ();
                return false;
            }

            return true;
        }
        #endregion

        #region Processor Check State Methods
        private void InvalidOpCode ()
        {  // BP
            m_eProgramState = EProgramState.STATE_PChk_InvalidOpCode;

            WaveRedFlag ("Invalid Op Code");
        }

        private void InvalidQByte ()
        {  // BP
            m_eProgramState = EProgramState.STATE_PChk_InvalidQByte;

            WaveRedFlag ("Invalid Q Byte");
        }

        private void AddressWraparound ()
        {  // BP
            m_eProgramState = EProgramState.STATE_PChk_AddressWrap;

            WaveRedFlag ("Address Wraparound");
        }

        private void InvalidAddress ()
        {  // BP
            m_eProgramState = EProgramState.STATE_PChk_InvalidAddress;

            WaveRedFlag ("Invalid Address");
        }

        private void NoPL2Support ()
        {  // BP
            m_eProgramState = EProgramState.STATE_PChk_PL2_Unsupported;

            WaveRedFlag ("Attempt to use PL2");
        }

        private void UnsupportedDevice ()
        {  // BP
            m_eProgramState = EProgramState.STATE_PChk_UnsupportedDevice;

            WaveRedFlag ("Unsupported device");
        }

        private void FunctionNotImplemented ()
        {  // BP
            Console.WriteLine ("");
            Console.WriteLine ("************************************");
            Console.WriteLine ("************************************");
            Console.WriteLine ("***                              ***");
            Console.WriteLine ("***   Function Not Implemented   ***");
            Console.WriteLine ("***                              ***");
            Console.WriteLine ("************************************");
            Console.WriteLine ("************************************");
            Console.WriteLine ("");
        }
        #endregion
    }
}
