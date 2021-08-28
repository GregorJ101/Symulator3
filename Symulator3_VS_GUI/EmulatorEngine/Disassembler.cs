using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EmulatorEngine
{
    // Disassembler Engine
    public partial class CDataConversion : CStringProcessor
    {
        public  const char   MICRO_SIGN              = (char)0x00B5;
        private const int    ALWAYS_CONVERT_ADDRESS  = 3;
        private const string ADDRESS_SPACE_HOLDER    = "        ";
        private const string TAG_LABEL_HOLDER        = "JorL_##_xxxx  ";
        private const string DASM_HEADER_LINE_ONE    = "Destination    IAR  Mnem  Machine Code     AdCalc  Annotation <ASCII> <EBCDIC> <HPL> <5475>";
        private const string DASM_HEADER_LINE_TWO    = "------------  ----- ----  ---------------  ------  " +
                                                     "------------------------------------------------------------------------------";
        //private const string LABEL_UNREACHABLE_CODE  = "<UNREACHABLE CODE>";

        // Type definitions
        // ZAZ: RAM  CR
        // AZ:  RAM  CR  ARR
        // SZ:  RAM  CR  ARR
        // ALC: RAM  CR
        // SLC: RAM  CR
        // MVX: RAM
        // MVC: RAM
        // ED:  RAM  CR
        // ITC: RAM  ARR
        // CLC: CR
        // LA:  XR1  XR2
        // L:   Current IAR  PL1 IAR  PL2 IAR  ARR  PSR  XR1  XR2  IL0 IAR  IL1 IAR  IL2 IAR  IL3 IAR  IL4 IAR
        // A:   Current IAR  PL1 IAR  PL2 IAR  ARR  PSR  XR1  XR2  IL0 IAR  IL1 IAR  IL2 IAR  IL3 IAR  IL4 IAR
        // ST:  RAM
        // TBN: CR
        // TBF: CR
        // SBN: RAM
        // SBF: RAM
        // MVI: RAM
        // CLI: CR
        // BC:  Current IAR  ARR
        // HPL: <none>
        // APL:
        // JC:  Current IAR  ARR
        // SNS: RAM
        // LIO: IO registers depending on device
        // SIO: RAM depending on device & operation
        // TIO: Current IAR  ARR

        // ZAZ 04-6  14-5  24-5  44-5  54-4  64-4  84-5  94-4  A4-4
        // AZ  06-6  16-5  26-5  46-5  56-4  66-4  86-5  96-4  A6-4
        // SZ  07-6  17-5  27-5  47-5  57-4  67-4  87-5  97-4  A7-4
        // MVX 08-6  18-5  28-5  48-5  58-4  68-4  88-5  98-4  A8-4
        // ED  0A-6  1A-5  2A-5  4A-5  5A-4  6A-4  8A-5  9A-4  AA-4
        // ITC 0B-6  1B-5  2B-5  4B-5  5B-4  6B-4  8B-5  9B-4  AB-4
        // MVC 0C-6  1C-5  2C-5  4C-5  5C-4  6C-4  8C-5  9C-4  AC-4
        // CLC 0D-6  1D-5  2D-5  4D-5  5D-4  6D-4  8D-5  9D-4  AD-4
        // ALC 0E-6  1E-5  2E-5  4E-5  5E-4  6E-4  8E-5  9E-4  AE-4
        // SLC 0F-6  1F-5  2F-5  4F-5  5F-4  6F-4  8F-5  9F-4  AF-4

        // SNS 30-4  70-3  B0-3
        // LIO 31-4  71-3  B1-3
        // ST  34-4  74-3  B4-3
        // L   35-4  75-3  B5-3
        // A   36-4  76-3  B6-3
        // TBN 38-4  78-3  B8-3
        // TBF 39-4  79-3  B9-3
        // SBN 3A-4  7A-3  BA-3
        // SBF 3B-4  7B-3  BB-3
        // MVI 3C-4  7C-3  BC-3
        // CLI 3D-4  7D-3  BD-3

        // BC  C0-4  D0-3  E0-3
        // TIO C1-4  D1-3  E1-3
        // LA  C2-4  D2-3  E2-3

        // HPL F0-3
        // APL F1-3
        // JC  F2-3
        // SIO F3-3

        //    | x0   x1   x2   x3   x4   x5   x6   x7   x8   x9   xA   xB   xC   xD   xE   xF
        // ---+-------------------------------------------------------------------------------
        // 0x |  -    -    -    -   ZAZ   -    AZ  SZ   MVX   -    ED  ITC  MVC  CLC  ALC  SLC
        // 1x |  -    -    -    -   ZAZ   -    AZ  SZ   MVX   -    ED  ITC  MVC  CLC  ALC  SLC
        // 2x |  -    -    -    -   ZAZ   -    AZ  SZ   MVX   -    ED  ITC  MVC  CLC  ALC  SLC
        // 3x | SNS  LIO   -    -    ST   L    A   -    TBN  TBF  SBN  SBF  MVI  CLI   -    -
        //    |
        // 4x |  -    -    -    -   ZAZ   -    AZ  SZ   MVX   -    ED  ITC  MVC  CLC  ALC  SLC
        // 5x |  -    -    -    -   ZAZ   -    AZ  SZ   MVX   -    ED  ITC  MVC  CLC  ALC  SLC
        // 6x |  -    -    -    -   ZAZ   -    AZ  SZ   MVX   -    ED  ITC  MVC  CLC  ALC  SLC
        // 7x | SNS  LIO   -    -    ST   L    A   -    TBN  TBF  SBN  SBF  MVI  CLI   -    -
        //    |
        // 8x |  -    -    -    -   ZAZ   -    AZ  SZ   MVX   -    ED  ITC  MVC  CLC  ALC  SLC
        // 9x |  -    -    -    -   ZAZ   -    AZ  SZ   MVX   -    ED  ITC  MVC  CLC  ALC  SLC
        // Ax |  -    -    -    -   ZAZ   -    AZ  SZ   MVX   -    ED  ITC  MVC  CLC  ALC  SLC
        // Bx | SNS  LIO   -    -    ST   L    A   -    TBN  TBF  SBN  SBF  MVI  CLI   -    -
        //    |
        // Cx |  BC  TIO   LA   -    -    -    -   -     -    -    -    -    -    -    -    -
        // Dx |  BC  TIO   LA   -    -    -    -   -     -    -    -    -    -    -    -    -
        // Ex |  BC  TIO   LA   -    -    -    -   -     -    -    -    -    -    -    -    -
        // Fx | HPL  APL   JC  SIO   -    -    -   -     -    -    -    -    -    -    -    -

        // Todo: Treat areas following unconditional jumps & branches as potential data:
        //         disassemle to temp string array until data found and discard if no unconditional branches/jumps found
        //           (or jumps/branches found that cover all possible conditions) discard disassembly and dump as data
        //       Note addresses of jump & unindexed branches as code locations, generate labels for them

        protected enum ESegmentStart
        {
            SEG_StartOfProgram,
            SEG_AfterBranch,
            SEG_AfterData
        };

        protected enum ETagType
        {
            TAG_Entry_Point = 10,
            TAG_IL1_Entry   =  9,
            TAG_MAIN_IAR    =  8,
            TAG_PL1_Entry   =  7,
            TAG_Tag         =  6,
            TAG_IL4_Entry   =  5,
            TAG_IL3_Entry   =  4,
            TAG_IL2_Entry   =  3,
            TAG_IL0_Entry   =  2,
            TAG_PL2_Entry   =  1,
            TAG_Undefined   =  0
        };

        public enum EControlPointType
        {
            POINT_Undefined,
            POINT_Begin,      // Computer  
            POINT_Entry,      // Command   Code  Tag/Comment
            POINT_Code,       // Command   Code  Text line   If address not reference by jump or loop, add CODE_##_addr label
            POINT_NewXR1,     // Command   Code
            POINT_NewXR2,     // Command   Code
            POINT_Data,       // Command   Data  Text line
            POINT_Skip,       // Command
            POINT_Tag,        // Command   Code  Tag/Comment
            POINT_Variable,   // Command   Data  Tag/Comment
            POINT_Constant,   // Command   Data  Tag/Comment
            POINT_Comment,    // Command         Comment
            POINT_Jump,       // Computer  Code  Tag/Comment
            POINT_Loop,       // Computer  Code  Tag/Comment
            POINT_End         // Computer
        };

        protected class CDasmControlPoint
        {
            int m_iAddress  = -1;
            int m_iXR1Value = 0x0000;
            int m_iXR2Value = 0x0000;
            bool m_bIsCode  = false;
            //ENotationType m_eNotationType = ENotationType.NOTE_Undefined;
            EControlPointType m_eControlPointType;
            string m_strNotation;

            //public CDasmControlPoint (int iAddress, ENotationType eNotationType, string strNotation)
            //{
            //    m_iAddress = iAddress;
            //    m_eControlPointType = eNotationType;
            //    m_strNotation = strNotation;
            //}

            public CDasmControlPoint (int iAddress, int iXR1Value, int iXR2Value, bool bIsCode, EControlPointType eControlPointType, string strNotation)
            {
                m_iAddress          = iAddress;
                m_iXR1Value         = iXR1Value;
                m_iXR2Value         = iXR2Value;
                m_bIsCode           = bIsCode;
                m_eControlPointType = eControlPointType;
                m_strNotation       = strNotation;
            }

            //public CDasmControlPoint (int iAddress, bool bIsCode, ENotationType eNotationType, string strNotation)
            //{
            //    m_iAddress = iAddress;
            //    m_bIsCode = bIsCode;
            //    m_eControlPointType = eNotationType;
            //    m_strNotation = strNotation;
            //}

            public int Address
            {
                get { return m_iAddress; }
                //set { m_iAddress = value; }
            }

            public int XR1
            {
                get { return m_iXR1Value; }
                set { m_iXR1Value = value; }
            }

            public int XR2
            {
                get { return m_iXR2Value; }
                set { m_iXR2Value = value; }
            }

            public bool IsCode
            {
                get { return m_bIsCode; }
                //set { m_bIsCode = value; }
            }

            public EControlPointType ControlPointType
            {
                get { return m_eControlPointType; }
            }
            //public ENotationType NotationType
            //{
            //    get { return m_eNotationType; }
            //    set { m_eNotationType = value; }
            //}

            public string Notation
            {
                get { return m_strNotation; }
                //set { m_strNotation = value; }
            }
        };

        protected class CTagEntry
        {
            static int s_iSequence    = 0;
            int        m_iLineNumber  = 0;
            int        m_iTagAddress  = 0;
            int        m_iTagSequence = 0;
            bool       m_bIsJump      = false;
            bool       m_bIsLoop      = false;
            ETagType   m_eTagType     = ETagType.TAG_Undefined;

            private const string LABEL_ENTRY_POINT = "Entry_";
            private const string LABEL_MAIN_IAR    = "Cur_IAR_";
            private const string LABEL_PL1_ENTRY   = "PL1_IAR_";
            private const string LABEL_PL2_ENTRY   = "PL2_IAR_";
            private const string LABEL_IL0_ENTRY   = "IL0_IAR_";
            private const string LABEL_IL1_ENTRY   = "IL1_IAR_";
            private const string LABEL_IL2_ENTRY   = "IL2_IAR_";
            private const string LABEL_IL3_ENTRY   = "IL3_IAR_";
            private const string LABEL_IL4_ENTRY   = "IL4_IAR_";
            private const string LABEL_JUMP_POINT  = "Jump_";
            private const string LABEL_LOOP_POINT  = "Loop_";
            private const string LABEL_TAG_POINT   = "Tag_";
            private const string LABEL_CODE_POINT  = "Code_";
            private const string LABEL_DATA_POINT  = "Data_";

            public static void ResetSequenceCounter () { s_iSequence = 0; }
            public void ResetTagSequence () { m_iTagSequence = 0; }

            public CTagEntry (int iLineNumber, int iTagAddress, ETagType eTagType)
            {
                m_iLineNumber = iLineNumber;
                m_iTagAddress = iTagAddress;
                m_eTagType    = eTagType;
            }

            public bool ChangeTagType (ETagType eTagType)
            {
                if (eTagType > m_eTagType)
                {
                    m_eTagType = eTagType;
                    return true;
                }

                return false;
            }

            public string GetTagName ()
            {
                string strTagName = "";

                if (m_eTagType == ETagType.TAG_Entry_Point)
                {
                    strTagName = LABEL_ENTRY_POINT;
                }
                else if (m_eTagType == ETagType.TAG_IL1_Entry)
                {
                    strTagName = LABEL_IL1_ENTRY;
                }
                else if (m_eTagType == ETagType.TAG_MAIN_IAR)
                {
                    strTagName = LABEL_MAIN_IAR;
                }
                else if (m_eTagType == ETagType.TAG_PL1_Entry)
                {
                    strTagName = LABEL_PL1_ENTRY;
                }
                else if (m_eTagType == ETagType.TAG_Tag)
                {
                    if (m_bIsJump)
                    {
                        strTagName = m_bIsLoop ? LABEL_TAG_POINT : LABEL_JUMP_POINT;
                    }
                    else
                    {
                        strTagName = LABEL_LOOP_POINT;
                    }

                    return string.Format ("{0}{1:D2}_{2:X4}{3}", strTagName, m_iTagSequence, m_iTagAddress, (m_bIsJump && m_bIsLoop) ? " " : "");
                }
                else if (m_eTagType == ETagType.TAG_IL4_Entry)
                {
                    strTagName = LABEL_IL4_ENTRY;
                }
                else if (m_eTagType == ETagType.TAG_IL3_Entry)
                {
                    strTagName = LABEL_IL3_ENTRY;
                }
                else if (m_eTagType == ETagType.TAG_IL2_Entry)
                {
                    strTagName = LABEL_IL2_ENTRY;
                }
                else if (m_eTagType == ETagType.TAG_IL0_Entry)
                {
                    strTagName = LABEL_IL0_ENTRY;
                }
                else if (m_eTagType == ETagType.TAG_PL2_Entry)
                {
                    strTagName = LABEL_PL2_ENTRY;
                }
                else
                {
                    return "";
                }

                strTagName = string.Format ("{0}{1:X4}", strTagName, m_iTagAddress);
                if (strTagName.Length < 12)
                {
                    strTagName += new string (' ', 12 - strTagName.Length);
                }

                return strTagName;
            }

            public void AssignSequenceNumber ()
            {
                if (m_eTagType == ETagType.TAG_Tag &&
                    m_iTagSequence == 0)
                {
                    m_iTagSequence = ++s_iSequence;
                }
            }

            public void SetJump () { m_bIsJump = true; }
            public void SetLoop () { m_bIsLoop = true; }
            public bool IsJump { get { return m_bIsJump; } }
            public bool IsLoop { get { return m_bIsLoop; } }
            public int LineNumber { get { return m_iLineNumber; } }
            public int TagAddress { get { return m_iTagAddress; } }
            public int TagSequence { get { return m_iTagSequence; } }
            public ETagType TagType { get { return m_eTagType; } }
        };

        public enum EKeyboard
        {
            KEY_None,
            KEY_5471,
            KEY_5475
        };
        public EKeyboard m_eKeyboard = EKeyboard.KEY_None;
        public void SetKeyboard5471 () { m_eKeyboard = EKeyboard.KEY_5471; }
        public void SetKeyboard5475 () { m_eKeyboard = EKeyboard.KEY_5475; }
        public void SetKeyboardNone () { m_eKeyboard = EKeyboard.KEY_None; }
        public bool IsKeyboard5471 ()  { return m_eKeyboard == EKeyboard.KEY_5471; }
        public bool IsKeyboard5475 ()  { return m_eKeyboard == EKeyboard.KEY_5475; }

        // Disassembler Engine member data
        protected List<byte> m_ylDataBuffer;
        protected byte[]     m_yaObjectCode;

        protected string m_strTagText           = "";
        protected string m_strHeaderProgramName = "";

        public string GetHeaderProgrmaName () { return m_strHeaderProgramName; }
        public void   SetHeaderProgrmaName (string strProgramName) { m_strHeaderProgramName = strProgramName; }

        protected List<string>               m_strlComments   = new List<string> (10);
        protected SortedDictionary<int, int> m_sdLineAddresses = new SortedDictionary<int, int> ();
        protected SortedDictionary<int, CDasmControlPoint> m_sdDCP = new SortedDictionary<int, CDasmControlPoint> ();
        protected SortedDictionary<int, CDasmControlPoint> m_sdBackupDCP = new SortedDictionary<int, CDasmControlPoint> ();
        protected SortedDictionary<int, int> m_sdDisassemblyLineIndexes = new SortedDictionary<int, int> (); // [inst address], list index
        protected List<string> m_strlDisassemblyLines = new List<string> ();
        protected SortedDictionary<int, CTagEntry> m_sdAllTags = new SortedDictionary<int, CTagEntry> ();
        protected SortedDictionary<int, int> m_sdTagCalls = new SortedDictionary<int, int> (); // [target address], address of BC/JC/TIO
        protected ESegmentStart m_eSegmentStart = ESegmentStart.SEG_StartOfProgram;

        public SortedDictionary<int, int> GetLineAddresses ()
        {
            return m_sdLineAddresses;
        }

        //         1         2         3
        //123456789012345678901234567890
        //0360: MVC  0C 04 0280 0281   <comment start>
        //0366: ALC  5E 00 5F,1 08,1   <comment start>
        const int m_kiLastDasmLinePos   = 26;
        const int m_kiLastPreCommentPos = m_kiLastDasmLinePos + 3;

#if LIST_DCP
        static int     s_iLastDCPCount = 0;
#endif
        protected int  m_iIdxDCP       = 0;
        protected int  m_iMaxTagLength = 12; // Jump_##_xxxx
        protected int  m_iObjectCodeIdx;
        protected int  m_iDumpStartAddress;
        //protected int  m_iColumnOffset;
        protected int  m_iEntryPoint                         = 0xFFFF;
        protected int  m_iHighAddress                        = 0xFFFF;
        protected int  m_iLowAddress                         = 0xFFFF;
        protected int  m_iDasmTriggerAddress                 = 0x00010000;
        protected int  m_iDasmStartAddress                   = 0x0000;
        protected int  m_iDasmEndAddress                     = 0x0000;
        protected int  m_iDumpLineLength                     = 8;
        protected int  m_iMaxLineLength                      = 80;
        protected int  m_iJumpID                             = 0;
        protected int  m_iLoopID                             = 0;
        protected int  m_iLineOffset                         = 0;
        protected int  m_iEmulatorXR1Value                   = 0;
        protected int  m_iEmulatorXR2Value                   = 0;
        protected int  m_iEmulatorIAR                        = 0;
        protected int  m_iFirstDecodedInstructionAddress     = 0;
        protected int  m_iPrevFirstDecodedInstructionAddress = 0;
        protected int  m_iSimulatedXR1Value                  = 0;
        protected int  m_iSimulatedXR2Value                  = 0;
        //protected int  m_iOldFirstInstruction                = 0;
        //protected int  m_iNewFirstInstruction                = 0;
        protected bool m_bSimulatedXR1ValueLoaded            = false;
        protected bool m_bSimulatedXR2ValueLoaded            = false;
        protected bool m_bInDataFlagSet                      = false;
        protected bool m_bTextCardDASM                       = false;
        protected bool m_bDumpComplete                       = false;
        protected bool m_bDasmComplete                       = false;
        protected bool m_bAutoTagJump                        = false;
        protected bool m_bAutoTagLoop                        = false;
        protected volatile bool m_bNewDisassemblyListingEventCalled = false;

        // Flags for controlling disassembly content
        //protected bool m_bShowTagLabels                      = false;
        protected bool m_bShowHeader                         = false;
        protected bool m_bHeaderLinesAdded                   = false;
        protected bool m_bInSingleLineTrace                  = false;
        protected bool m_bInData                             = false;

        public void SetAutoTagJump ()         { m_bAutoTagJump       = true;  }
        public void ResetAutoTagJump ()       { m_bAutoTagJump       = false; }
        public void SetAutoTagLoop ()         { m_bAutoTagLoop       = true;  }
        public void ResetAutoTagLoop ()       { m_bAutoTagLoop       = false; }
        public void SetExtendMnemonicBC ()    { m_bExtendMnemonicBC  = true;  }
        public void ResetExtendMnemonicBC ()  { m_bExtendMnemonicBC  = false; }
        public void SetExtendMnemonicJC ()    { m_bExtendMnemonicJC  = true;  }
        public void ResetExtendMnemonicJC ()  { m_bExtendMnemonicJC  = false; }
        public void SetExtendMnemonicMVX ()   { m_bExtendMnemonicMVX = true;  }
        public void ResetExtendMnemonicMVX () { m_bExtendMnemonicMVX = false; }

        public void SetMaxLineLength (int iMaxLineLength)
        {
            m_iMaxLineLength = iMaxLineLength;
        }

        protected byte[] m_yaHaltCodes = { 0x00, 0x10, 0x40, 0x03, 0x76, 0x57, 0x1B, 0x5D, 0x7D,
                                           0x07, 0x7F, 0x5F, 0x6F, 0x3F, 0x79, 0x6C, 0x73, 0x7C,
                                           0x3C, 0x3B, 0x63, 0x68, 0x3E, 0x6B, 0x02, 0x08, 0x0A };
        protected char[] m_caHaltDisplay =  { ' ', '-', '_', '1', '2', '3', '4', '5', '6',
                                              '7', '8', '9', '0', 'A', 'b', 'C', 'd', 'E'
                                             ,'F', 'H', 'J', 'L', 'P', 'U', '\'', '\'', '"' };
        protected string m_strHaltDisplay = " -_1234567890AbCdEFHJLPU''\"";

        protected byte[] m_ya5475Codes = { 0x00, 0x10, 0x02, 0x24, 0xBA, 0xB6, 0x74, 0xD6, 0xDE,
                                           0xA4, 0xFE, 0xF6, 0xEE, 0xFC, 0x5E, 0xCA, 0x3E, 0xDA,
                                           0xD8, 0x7C, 0x2E, 0x4A, 0xF8, 0x6E, 0x20, 0x40, 0x60 };
        protected char[] m_ca5475Display =  { ' ', '-', '_', '1', '2', '3', '4', '5', '6',
                                              '7', '8', '9', '0', 'A', 'b', 'C', 'd', 'E'
                                             ,'F', 'H', 'J', 'L', 'P', 'U', '\'', '\'', '"' };
        protected string m_str5475Display = " -_1234567890AbCdEFHJLPU''\"";

        // StringCollection of all detected errors
        //StringCollection m_scErrors = new StringCollection ();

        public void ShowLinesAndTags (string strLabel)
        {
            Console.WriteLine ();
            Console.WriteLine ('<' + strLabel + '>');
            Console.WriteLine ("m_strlDisassemblyLines [" + m_strlDisassemblyLines.Count.ToString () + ']');
            foreach (string str in m_strlDisassemblyLines)
            {
                Console.WriteLine ("  " + str);
            }

            Console.WriteLine ("m_sdAllTags [" + m_sdAllTags.Count.ToString () + ']');
            foreach (KeyValuePair<int, CTagEntry> kvp in m_sdAllTags)
            {
                Console.WriteLine ("  [" + kvp.Key.ToString ("X4") + "] " + kvp.Value.TagAddress.ToString ("X4") + ' ' + kvp.Value.TagType.ToString ());
            }
        }

        public int GetLineIdxFromInstructionAddress (int iInstAddress)
        {
            if (m_sdLineAddresses.ContainsKey (iInstAddress))
            {
                return m_sdLineAddresses[iInstAddress];
            }

            return -1;
        }

        public List<int> GetGrayedCodeLines ()
        {
            List<int> liGrayedCode = new List<int> ();
            foreach (KeyValuePair<int, int> kvp in m_sdLineAddresses)
            {
                liGrayedCode.Add (kvp.Value);
            }
            return liGrayedCode;
        }

        private void PrepForDASM ()
        {
            DumpListDCP ();

            m_sdDisassemblyLineIndexes.Clear ();
            m_strlDisassemblyLines.Clear ();
            m_sdAllTags.Clear ();
            m_sdTagCalls.Clear ();
            CTagEntry.ResetSequenceCounter ();
            m_bDumpComplete       = false;
            m_iLowAddressMI       = 0;
            m_iHighAddressMI      = 0;
            m_iLowAddressEC       = 0;
            m_iHighAddressEC      = 0;
            m_iLowAddress         = 0;
            m_iHighAddress        = 0;
            m_iIdxDCP             = 0;
            m_iJumpID             = 0;
            m_iLoopID             = 0;
            m_iEndCardLoadAddress = 0;

            m_sdDCP.Clear ();
        }

        public List<string> DisassembleCodeTrace (byte[] yaBinary, int iEmulatorXR1Value = 0, int iEmulatorXR2Value = 0, int iEmulatorIAR = 0)
        {
            m_bInSingleLineTrace = true;
            List<string> lstrCodeTrace = AddTagLabels (DisassembleCodeInternal (yaBinary, 0, yaBinary.Length - 1,  iEmulatorXR1Value,
                                                                                iEmulatorXR2Value, iEmulatorIAR, false, true));
            m_bInSingleLineTrace = false;
            return lstrCodeTrace;
        }

        public List<string> DisassembleCodeEndCard (byte[] yaBinary, int iStartAddress, int iEndAddress)
        {
            m_bTextCardDASM = true;
            m_sdAllTags.Clear ();
            CTagEntry.ResetSequenceCounter ();
            m_iSimulatedXR1Value       = 0;
            m_iSimulatedXR2Value       = 0;
            m_bSimulatedXR1ValueLoaded = true;
            m_bSimulatedXR2ValueLoaded = false;
            m_bInData = (iStartAddress < 0x0019);
            m_bAutoTagJump             = true;
            m_bAutoTagLoop             = true;
            m_bExtendMnemonicBC        = true;
            m_bExtendMnemonicJC        = true;
            m_bExtendMnemonicMVX       = true;
            List<string> strlReturn = DisassembleCodeImage (yaBinary, m_iLowAddressEC, m_iHighAddressEC, 0x0019);
            CTagEntry.ResetSequenceCounter ();
            m_bTextCardDASM = false;
            return strlReturn;
        }

        public List<string> DisassembleCodeText (bool bEndCard)
        {
            m_bTextCardDASM = true;
            m_sdAllTags.Clear ();
            CTagEntry.ResetSequenceCounter ();
            m_iSimulatedXR1Value       = 0;
            m_iSimulatedXR2Value       = 0;
            m_bSimulatedXR1ValueLoaded = true;
            m_bSimulatedXR2ValueLoaded = false;
            m_bInData                  = true;
            m_bAutoTagJump             = true;
            m_bAutoTagLoop             = true;
            m_bExtendMnemonicBC        = true;
            m_bExtendMnemonicJC        = true;
            m_bExtendMnemonicMVX       = true;
            m_bTextCardDASM            = false;
            //List<string> strlReturn = bEndCard ? DisassembleCodeImage (m_yaEndCard,                m_iLowAddressEC, m_iHighAddressEC, 0x0019) :
            //                                     DisassembleCodeImage (m_ylMemoryImage.ToArray (), m_iLowAddressMI, m_iHighAddressMI, 0x0600);
            List<string> strlReturn = bEndCard ? DisassembleCodeImage (m_yaMainMemory, m_iLowAddressEC, m_iHighAddressEC, 0x0019) :
                                                 DisassembleCodeImage (m_yaMainMemory, m_iLowAddressMI, m_iHighAddressMI, 0x0600);
            return strlReturn;
        }

        public List<string> DisassembleCodeFromText (byte[] yaBinary)
        {
            m_bTextCardDASM                       = true;
            m_bDasmComplete                       = false;
            m_bInData                             = (m_iLowAddress < m_iEntryPoint);
            m_bAutoTagJump                        = true;
            m_bAutoTagLoop                        = true;
            m_bExtendMnemonicBC                   = true;
            m_bExtendMnemonicJC                   = true;
            m_bExtendMnemonicMVX                  = true;
            m_eKeyboard                           = EKeyboard.KEY_5475;
            List<string> strlReturn               = new List<string> ();
            m_iFirstDecodedInstructionAddress     = 0;
            m_iPrevFirstDecodedInstructionAddress = 0;

            while (!m_bDasmComplete)
            {
                CTagEntry.ResetSequenceCounter ();
                foreach (KeyValuePair<int, CTagEntry> kvp in m_sdAllTags)
                {
                    kvp.Value.ResetTagSequence ();
                }
                strlReturn = DisassembleCodeImage (yaBinary, m_iLowAddressMI, m_iHighAddressMI, m_iEndCardLoadAddress);
            }
            m_bTextCardDASM = false;
            return strlReturn;
        }

        public List<string> DisassembleCodeFromEmulator (byte[] yaBinary, int iEmulatorXR1Value = 0, int iEmulatorXR2Value = 0)
        {
            List<string> strlDasm = new List<string> ();
            //m_iSimulatedXR1Value       = m_iEmulatorXR1Value = iEmulatorXR1Value;
            //m_iSimulatedXR2Value       = m_iEmulatorXR2Value = iEmulatorXR2Value;
            //m_bSimulatedXR1ValueLoaded = true;
            //m_bSimulatedXR2ValueLoaded = true;

            //if (m_bInDataFlagSet)
            //{
            //    m_bInData = (m_iDasmStartAddress < m_iEntryPoint);
            //}
            //else
            //{
            //    m_bInData = false;
            //}

            //m_sdAllTags.Clear ();
            //CTagEntry.ResetSequenceCounter ();
            //AddTag (m_iEntryPoint, 0xFFFF, ETagType.TAG_Entry_Point);
            //m_bDasmComplete = false;
            //m_iPrevFirstDecodedInstructionAddress = 0xFFFF;
            //m_iFirstDecodedInstructionAddress     = 0xFFFF;
            //while (!m_bDasmComplete)
            //{
            //    if (!m_bInDataFlagSet)
            //    {
            //        m_bInData = false;
            //    }
            //    CTagEntry.ResetSequenceCounter ();
            //    foreach (KeyValuePair<int, CTagEntry> kvp in m_sdAllTags)
            //    {
            //        kvp.Value.ResetTagSequence ();
            //    }
            //    strlDasm = AddTagLabels (DisassembleCodeInternal (yaBinary, m_iDasmStartAddress, m_iDasmEndAddress, m_iEntryPoint));
            //}

            return strlDasm;
        }

        public List<string> DisassembleCodeImage (int iStartAddress = 0, int iEndAddress = 0x3F, int iEntryPoint = 0,
                                                  int iEmulatorXR1Value = 0, int iEmulatorXR2Value = 0)
        {
            m_bAutoTagJump       = true;
            m_bAutoTagLoop       = true;
            m_bExtendMnemonicBC  = true;
            m_bExtendMnemonicJC  = true;
            m_bExtendMnemonicMVX = true;
            m_eKeyboard          = EKeyboard.KEY_5475;
            return DisassembleCodeImage (m_yaMainMemory, iStartAddress, iEndAddress, iEntryPoint, iEmulatorXR1Value, iEmulatorXR2Value);
        }

        public List<string> DisassembleCodeImage (byte[] yaBinary, int iStartAddress = 0, int iEndAddress = 0, int iEntryPoint = 0xFFFF,
                                                  int iEmulatorXR1Value = 0, int iEmulatorXR2Value = 0, int iEmulatorIAR = 0)
        {
            if (iStartAddress == 0)
            {
                iStartAddress = m_iLowAddressMI;
            }
            if (iEndAddress == 0)
            {
                iEndAddress = m_iHighAddressMI;
            }
            if (iEntryPoint < 0xFFFF)
            {
                m_iEntryPoint = iEntryPoint;
            }

            if (iEndAddress == 0)
            {
                iStartAddress = 0;
                iEndAddress   = yaBinary.Length - 1;
            }

            return AddTagLabels (DisassembleCodeInternal (yaBinary, iStartAddress, iEndAddress, iEmulatorXR1Value,
                                                          iEmulatorXR2Value, iEmulatorIAR));
        }

        private List<string> DisassembleCodeInternal (byte[] yaBinary, int iLowAddress, int iHighAddress, int iEmulatorXR1Value = 0,
                                                      int iEmulatorXR2Value = 0, int iEmulatorIAR = 0, bool bShowHeader = true,
                                                      bool bTraceSingleLine = false)
        {
            if (!m_bTextCardDASM    &&
                !m_bDasmCalledByEmu &&
                !bTraceSingleLine)
            {
                m_sdAllTags.Clear ();
                CTagEntry.ResetSequenceCounter ();
                m_sdDisassemblyLineIndexes.Clear ();
                m_iSimulatedXR1Value       = 0;
                m_iSimulatedXR2Value       = 0;
                m_bInData                  = false;
                m_bSimulatedXR1ValueLoaded = false;
                m_bSimulatedXR2ValueLoaded = false;
            }

            if (!bTraceSingleLine)
            {
                m_sdTagCalls.Clear ();
                m_sdLineAddresses.Clear ();
            }

            m_sdDisassemblyLineIndexes.Clear ();
            m_strlDisassemblyLines.Clear ();
            m_yaObjectCode                    = yaBinary;
            m_iObjectCodeIdx                  = iLowAddress;
            m_iDumpStartAddress               = -1;
#if LIST_DCP
            s_iLastDCPCount                   = 0;
#endif
            m_iDumpLineLength                 = 8;
            m_iJumpID                         = 0;
            m_iLoopID                         = 0;
            m_iLineOffset                     = 0;
            m_iFirstDecodedInstructionAddress = 0;
            m_iEmulatorXR1Value               = iEmulatorXR1Value;
            m_iEmulatorXR2Value               = iEmulatorXR2Value;
            m_iEmulatorIAR                    = iEmulatorIAR;
            m_iHighAddress                    = iHighAddress;
            m_iLowAddress                     = iLowAddress;
            m_bShowHeader                     = bShowHeader;
            m_bHeaderLinesAdded               = false;

            m_ylDataBuffer = new List<byte> ();

            if (m_iEntryPoint < 0xFFFF)
            {
                AddTag (m_iEntryPoint, 0xFFFF, ETagType.TAG_Entry_Point);
            }

            if (iHighAddress >= yaBinary.Length)
            {
                iLowAddress  = 0;
                iHighAddress = yaBinary.Length - 1;
            }

            if (bTraceSingleLine)
            {
                DisassembleInstruction (true);
            }
            else
            {
                int iNewFirstInstruction = m_iEntryPoint;
                int iOldFirstInstruction = iNewFirstInstruction + 1;
                bool bTagIterationComplete = false;

                // What happens after the first pass with PLTX to fuck things up?
                while (iNewFirstInstruction < iOldFirstInstruction)
                {
                    iOldFirstInstruction = iNewFirstInstruction;
                    m_iObjectCodeIdx = iLowAddress;

                    m_sdDisassemblyLineIndexes.Clear ();
                    m_strlDisassemblyLines.Clear ();
                    m_bHeaderLinesAdded = false;

                    while (m_iObjectCodeIdx <= iHighAddress &&
                           !m_bDumpComplete)
                    {
                        DisassembleInstruction ();
                    }

                    if (!m_bDumpComplete)
                    {
                        DumpAccumulatedData ();
                    }

                    foreach (KeyValuePair<int, CTagEntry> kvp in m_sdAllTags)
                    {
                        if (kvp.Key < iNewFirstInstruction)
                        {
                            iNewFirstInstruction = kvp.Key;
                        }
                    }
                    ShowLinesAndTags ("DisassembleCodeInternal 1");
                }

                iOldFirstInstruction = iNewFirstInstruction;
                m_iObjectCodeIdx     = iLowAddress;

                m_sdDisassemblyLineIndexes.Clear ();
                m_strlDisassemblyLines.Clear ();
                m_bHeaderLinesAdded = false;

                ////while (m_iObjectCodeIdx <= iHighAddress &&
                ////        !m_bDumpComplete)
                ////{
                ////    DisassembleInstruction ();
                ////}

                ////[x] Treat each new byte as an instruction if decodable
                ////    [x] Save address of first decodable instruction
                ////    [x] Save state of this instruction is at the start of the DASM area, following a J/B/L, or following data
                ////[x] Continue decoding instructions until either J/B/L[IAR] or byte not an opcode
                ////    [ ] If J/B/L, add address of first instruction as "CODE_" if not already in m_sdAllTags collection
                ////        [ ] If address already defined as "Jump", "Loop", or "Tag" use that label with the matching address
                ////    [ ] If not an opcode, save address of first "instruction" as "DATA_"
                ////[x] Add blank skip line to separate code segments ending with B / J / L[IAR]
                ////    [x] Also separate code areas from data areas
                ////[ ] Save start address of each new potential code segment
                ////    [ ] Do not to data dump until beginning of data segment confirmed by start of successfully disassembled code segment (prevent fragmented data dumps)
                ////[ ] After first pass of new disassembler design, scan DASM code for all C2 LA instructions and look for following BC instructions
                ////    [ ] Replace XR values beginning @ BC LOOP targets and recalculate all address until the next C2 LA instruction
                //while (m_iObjectCodeIdx <= iHighAddress &&
                //       !m_bDumpComplete)
                //{
                //    m_eSegmentStart = ESegmentStart.SEG_StartOfProgram;
                //    List<string> lstrTempDasm = new List<string> ();
                //    bool bIsValidCodeSegment = false;
                //    short sFirstInstructionAddress = 0;
                //    short sSegmentStartAddress = (short)iLowAddress;
    
                //    // Try to decode byte as opcode
                //    while (byte is opcode)
                //    {
                //        decode instruction and add to DASM list lstrTempDasm
                //        if first instruction in segment
                //        {
                //            if (m_eSegmentStart == ESegmentStart.SEG_AfterData)
                //            {
                //                DumpAccumulatedData ();
                //            }
                //            Create new tag CODE_##_addr
                //            // Save address of first instruction
                //            sFirstInstructionAddress = iCodeIdx;
                //        }
                //        if J/B/L[IAR]
                //        {
                //            // Temp segment is valid code
                //            bIsValidCodeSegment = true;
                //            if master DASM list not empty
                //                add blank line to master DASM list
                //            Save lstrTempDasm to master DASM list
                //            if (m_sdAllTage.ContainsKey (sFirstInstructionAddress)
                //            {
                //                // If address already defined as "Jump", "Loop", or "Tag" use that label with the matching address
                //                Update new tag with m_sdAllTage[sFirstInstructionAddress].Value
                //            }
                //            else
                //            {
                //                Add new tag to m_sdAddTags
                //            }
            
                //            lstrTempDasm.Clear ();
                //            m_eSegmentStart = ESegmentStart.SEG_AfterBranch;
            
                //            // Begin creation of new code temp segment
                //            Create new tag CODE_##_addr
                //            // Save address of first instruction
                //            sFirstInstructionAddress = iCodeIdx;
                //        }
                //    }
                //    else
                //    {
                //        if (lstrTempDasm.Count > 0)
                //        {
                //            if master DASM list not empty
                //                add blank line to master DASM list
                //            Dump data from address of byte of first "instruction" in lstrTempDasm
                //            lstrTempDasm.Clear ();
                //            Change CODE_##_addr to DATA_##_addr
                //            m_eSegmentStart = ESegmentStart.SEG_AfterData;
                //        }
                //        else
                //        {
                //            // Begin data segment
                //            bIsValidCodeSegment = false;
                //            Create new tag DATA_##_addr
                //            save byte as data
                //        }
                //    }
                //}

                if (!m_bDumpComplete)
                {
                    DumpAccumulatedData ();
                }

                foreach (KeyValuePair<int, CTagEntry> kvp in m_sdAllTags)
                {
                    if (kvp.Key < iNewFirstInstruction)
                    {
                        iNewFirstInstruction = kvp.Key;
                    }
                }
                ShowLinesAndTags ("DisassembleCodeInternal 1");
            }

            int iFirstTagAddress = 0xFFFF;
            foreach (KeyValuePair<int, CTagEntry> kvp in m_sdAllTags)
            {
                if (iFirstTagAddress == 0xFFFF ||
                    iFirstTagAddress > kvp.Key)
                {
                    iFirstTagAddress = kvp.Key;
                }
            }

            // In case there are tag addresses that weren't disassembled, do it again
            m_bDasmComplete = (m_iFirstDecodedInstructionAddress == iFirstTagAddress ||
                               m_iFirstDecodedInstructionAddress == m_iPrevFirstDecodedInstructionAddress);
            // Save the last start-of-disassembly address to prevent infinite looping
            m_iPrevFirstDecodedInstructionAddress = m_iFirstDecodedInstructionAddress;

            if (!bTraceSingleLine)
            {
                ShowLinesAndTags ("DisassembleCodeInternal 2");
            }

            return m_strlDisassemblyLines;
        }

        private void DisassembleInstruction (bool bInTraceCall = false)
        {
            if (!bInTraceCall)
            {
                if (m_sdAllTags.ContainsKey (m_iObjectCodeIdx))
                {
                    m_bInData       = false;
                    m_bDumpComplete = false;
                }
                else if (m_iObjectCodeIdx >= 0x0017 &&
                         m_iObjectCodeIdx <= 0x0018 &&
                         m_iLowAddressEC  == 0x0017)
                {
                    m_bInData       = true;
                    m_bDumpComplete = false;
                }

                if (m_bInData)
                {
                    if (m_bDumpComplete)
                    {
                        return;
                    }

                    AccumulateData ();
                    return;
                }

                if (m_bDumpComplete)
                {
                    return;
                }
            }

            switch (m_yaObjectCode[m_iObjectCodeIdx])
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
                    FormatTwoOperandInst ("ZAZ ");
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
                    FormatTwoOperandInst ("AZ  ");
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
                    FormatTwoOperandInst ("SZ  ");
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
                    FormatTwoOperandInst ("MVX ");
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
                    FormatTwoOperandInst ("ED  ");
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
                    FormatTwoOperandInst ("ITC ");
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
                    FormatTwoOperandInst ("MVC ");
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
                    FormatTwoOperandInst ("CLC ");
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
                    FormatTwoOperandInst ("ALC ");
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
                    FormatTwoOperandInst ("SLC ");
                    break;
                }

                // SNS   
                case 0x30:
                case 0x70:
                case 0xB0:
                {
                    FormatOneOperandInst ("SNS ");
                    break;
                }

                // LIO
                case 0x31:
                case 0x71:
                case 0xB1:
                {
                    FormatOneOperandInst ("LIO ");
                    break;
                }

                // ST
                case 0x34:
                case 0x74:
                case 0xB4:
                {
                    FormatOneOperandInst ("ST  ");
                    break;
                }

                // L
                case 0x35:
                case 0x75:
                case 0xB5:
                {
                    FormatOneOperandInst ("L   ");
                    break;
                }

                // A
                case 0x36:
                case 0x76:
                case 0xB6:
                {
                    FormatOneOperandInst ("A   ");
                    break;
                }

                // TBN
                case 0x38:
                case 0x78:
                case 0xB8:
                {
                    FormatOneOperandInst ("TBN ");
                    break;
                }

                // TBF
                case 0x39:
                case 0x79:
                case 0xB9:
                {
                    FormatOneOperandInst ("TBF ");
                    break;
                }

                // SBN
                case 0x3A:
                case 0x7A:
                case 0xBA:
                {
                    FormatOneOperandInst ("SBN ");
                    break;
                }

                // SBF
                case 0x3B:
                case 0x7B:
                case 0xBB:
                {
                    FormatOneOperandInst ("SBF ");
                    break;
                }

                // MVI
                case 0x3C:
                case 0x7C:
                case 0xBC:
                {
                    FormatOneOperandInst ("MVI ");
                    break;
                }

                // CLI
                case 0x3D:
                case 0x7D:
                case 0xBD:
                {
                    FormatOneOperandInst ("CLI ");
                    break;
                }

                // BC
                case 0xC0:
                case 0xD0:
                case 0xE0:
                {
                    FormatOneOperandInst ("BC  ");
                    break;
                }

                // TIO
                case 0xC1:
                case 0xD1:
                case 0xE1:
                {
                    FormatOneOperandInst ("TIO ");
                    break;
                }

                // LA
                case 0xC2:
                case 0xD2:
                case 0xE2:
                {
                    FormatOneOperandInst ("LA  ");
                    break;
                }

                // HLP
                case 0xF0:
                {
                    FormatCommandInst ("HPL ");
                    break;
                }

                // APL
                case 0xF1:
                {
                    FormatCommandInst ("APL ");
                    break;
                }

                // JC
                case 0xF2:
                {
                    FormatCommandInst ("JC  ");
                    break;
                }

                // SIO
                case 0xF3:
                {
                    FormatCommandInst ("SIO ");
                    break;
                }

                // data
                default:
                {
                    AccumulateData ();
                    break;
                }
            }
        }

        protected List<string> AddTagLabels (List<string> strlDasmOutput)
        {
            int iLabelsAdded = 0;
            if (m_bInSingleLineTrace)
            {
                return strlDasmOutput;
            }

            // Build dictionary of line# and line IAR value
            m_sdLineAddresses.Clear ();
            for (int iIdx = 0; iIdx < m_strlDisassemblyLines.Count; iIdx++)
            {
                string strDasmLine = m_strlDisassemblyLines[iIdx];
                int iColonPos = strDasmLine.IndexOf (':');
                if (iColonPos < 4)
                {
                    continue;
                }
                string strLineAddress = strDasmLine.Substring (iColonPos - 4, 4);
                int iLineAddress = IsHexadecimal (strLineAddress) ? Convert.ToInt32 (strLineAddress, 16) : -1;
                if (!m_sdLineAddresses.ContainsKey (iLineAddress))
                {
                    m_sdLineAddresses.Add (iLineAddress, iIdx);
                }
            }

            // Process all the tags accumulated in both dictionaries
            if (m_sdAllTags.Count > 0)
            {
                // First, apply sequence numbers to tag labels
                foreach (KeyValuePair<int, CTagEntry> kvp in m_sdAllTags)
                {
                    kvp.Value.AssignSequenceNumber ();
                }

                // Iterate through all DASM strings
                for (int iIdx = 0; iIdx < m_strlDisassemblyLines.Count; iIdx++)
                {
                    string strDasmLine = m_strlDisassemblyLines[iIdx];
                    int iColonPos = strDasmLine.IndexOf (':');
                    if (iColonPos < 4)
                    {
                        continue;
                    }
                    string strLineAddress = strDasmLine.Substring (iColonPos - 4, 4);
                    int iLineAddress = IsHexadecimal (strLineAddress) ? Convert.ToInt32 (strLineAddress, 16) : -1;

                    if (iLineAddress >= 0)
                    {
                        if (m_sdAllTags.ContainsKey (iLineAddress))
                        {
                            CTagEntry tag = m_sdAllTags[iLineAddress];
                            string strLabel = tag.GetTagName ();
                            // Add tag label to DASM line
                            strDasmLine = strLabel + strDasmLine.Substring (strLabel.Length);
                            m_strlDisassemblyLines[iIdx] = strDasmLine;
                            ++iLabelsAdded;
                        }

                        if (m_sdTagCalls.ContainsKey (iLineAddress))
                        {
                            int iTargetAddress = m_sdTagCalls[iLineAddress];
                            string strPadding = "";
                            CTagEntry tag = m_sdAllTags[iTargetAddress];
                            if ((tag.TagType == ETagType.TAG_Tag &&
                                 tag.TagSequence > 0)            ||
                                 tag.TagType != ETagType.TAG_Undefined)
                            {
                                string strLabel = tag.GetTagName ();
                                // Add comment to line of the branching instruction
                                bool bJump = iLineAddress < iTargetAddress;
                                string strComment = string.Format ("{0:s} to {1:s}", bJump ? "Jump" : "Loop", strLabel);
                                if (!m_sdLineAddresses.ContainsKey (iTargetAddress))
                                {
                                    strComment += "  <UNMATCHED LABEL>";
                                }

                                if (ReplaceTagLabelHolder (ref strDasmLine, strComment))
                                {
                                    m_strlDisassemblyLines[iIdx] = strDasmLine;
                                }
                                else
                                {
                                    if (strDasmLine[strDasmLine.Length - 1] != ' ' &
                                        strDasmLine[strDasmLine.Length - 2] != ' ')
                                    {
                                        strPadding = "  ";
                                    }
                                    if (m_kiLastPreCommentPos + m_iLineOffset > strDasmLine.Length)
                                        strPadding = new string (' ', m_kiLastPreCommentPos + m_iLineOffset - strDasmLine.Length);
                                    m_strlDisassemblyLines[iIdx] = strDasmLine + strPadding + strComment;
                                }
                                ++iLabelsAdded;
                            }
                            else
                            {
                                if (strDasmLine[strDasmLine.Length - 1] != ' ' &
                                    strDasmLine[strDasmLine.Length - 2] != ' ')
                                {
                                    strPadding = "  ";
                                }
                                m_strlDisassemblyLines[iIdx] = strDasmLine + strPadding + tag.GetTagName ();
                            }
                        }
                    }
                }
            }

            if (!m_bInEmulator &&
                iLabelsAdded == 0)
            {
                // No tags, so remove leading spaces from each dasm line
                for (int iIdx = 0; iIdx < strlDasmOutput.Count; iIdx++)
                {
                    strlDasmOutput[iIdx] = strlDasmOutput[iIdx].Substring (m_iLineOffset);
                }
            }

            return strlDasmOutput;
        }

        protected bool ReplaceTagLabelHolder (ref string strDasmLine, string strComment)
        {
            int iTagIdx = strDasmLine.IndexOf (TAG_LABEL_HOLDER);
            if (iTagIdx < 0)
            {
                return false;
            }

            string strBefore = strDasmLine.Substring (0, iTagIdx),
                   strAfter  = strDasmLine.Substring (iTagIdx + TAG_LABEL_HOLDER.Length);

            strDasmLine = strBefore + strComment.Trim () + "  " + strAfter;

            return true;
        }

        protected bool InstructionReadsRAM (byte yOpCode)
        {
            if ((yOpCode & 0xF0) == 0xF0)
            {
                // Command format
                return false;
            }
            else if ((yOpCode & 0x30) == 0x30)
            {
                // Y format
                return ((yOpCode & 0x0F) == 0x01 || // LIO
                        (yOpCode & 0x0F) == 0x05 || // L
                        (yOpCode & 0x0F) == 0x06 || // A
                        (yOpCode & 0x0F) == 0x08 || // TBN
                        (yOpCode & 0x0F) == 0x09 || // TBF
                        (yOpCode & 0x0F) == 0x0D);  // CLI
            }
            else if ((yOpCode & 0xC0) == 0xC0)
            {
                // Z format
                return false;
            }
            else
            {
                // X format
                return ((yOpCode & 0x0F) == 0x0D);  // CLC
            }
        }

        protected bool InstructionWritesRAM (byte yOpCode)
        {
            if ((yOpCode & 0xF0) == 0xF0)
            {
                // Command format
                return false; // ((yOpCode & 0x0F) == 0x03); // SIO
            }
            else if ((yOpCode & 0x30) == 0x30)
            {
                // Y format
                return ((yOpCode & 0x0F) == 0x00 || // SNS
                        (yOpCode & 0x0F) == 0x04 || // ST
                        (yOpCode & 0x0F) == 0x0A || // SBN
                        (yOpCode & 0x0F) == 0x0B || // SBF
                        (yOpCode & 0x0F) == 0x0C);  // MVI
            }
            else if ((yOpCode & 0xC0) == 0xC0)
            {
                // Z format
                return false;
            }
            else
            {
                // X format
                return ((yOpCode & 0x0F) == 0x04 || // ZAZ
                        (yOpCode & 0x0F) == 0x06 || // AZ
                        (yOpCode & 0x0F) == 0x07 || // SZ
                        (yOpCode & 0x0F) == 0x08 || // MVX
                        (yOpCode & 0x0F) == 0x0A || // ED
                        (yOpCode & 0x0F) == 0x0B || // ITC
                        (yOpCode & 0x0F) == 0x0C || // MVC
                        (yOpCode & 0x0F) == 0x0E || // ALC
                        (yOpCode & 0x0F) == 0x0F);  // SLC
            }
        }

        private byte GetNextByte ()
        {
            if (m_iObjectCodeIdx <= m_iHighAddress)
            {
                return m_yaObjectCode[m_iObjectCodeIdx++];
            }
            else
            {
                return 0x00;
            }
        }

        private byte[] PeekNextBytes (int iNumberBytes)
        {
            byte[] yaEmpty = new byte[1] { 0 };
            byte[] yaReturn = new byte[iNumberBytes];

            if (m_iObjectCodeIdx <= m_iHighAddress + iNumberBytes + 1)
            {
                for (int iIdx = 0; iIdx < iNumberBytes; ++iIdx)
                {
                    yaReturn[iIdx] = m_yaObjectCode[m_iObjectCodeIdx + iIdx + 1];
                }
                return yaReturn;
            }
            else
            {
                return yaEmpty;
            }
        }

        // <label>  addr:  nem  op qb xxxx xxxx
        // <label>  addr:  nem  op qb xxxx xx,i
        // <label>  addr:  nem  op qb xx,i xxxx
        // <label>  addr:  nem  op qb xx,i xx,i
        private void FormatTwoOperandInst (string strDasmMnemonic)
        {
            DumpAccumulatedData ();

            int iInstAddress = m_iObjectCodeIdx;
            string strInstLine = "";

            byte yOpCode = GetNextByte ();
            int iIR1 = ((yOpCode & 0x80) == 0x80 ? 2 :
                        (yOpCode & 0x40) == 0x40 ? 1 : 0);
            int iIR2 = ((yOpCode & 0x20) == 0x20 ? 2 :
                        (yOpCode & 0x10) == 0x10 ? 1 : 0);

            byte yQByte = GetNextByte ();
            string strExtendedMnemonic = GetExtendedMnemonic (strDasmMnemonic, yQByte);
            int iOperand1Address = 0,
                iOperand2Address = 0;

            if (iIR1 == 0 && iIR2 == 0)
            {
                // addr:  nem  op qb xxxx xxxx
                byte yAddress1 = GetNextByte (),
                     yAddress2 = GetNextByte (),
                     yAddress3 = GetNextByte (),
                     yAddress4 = GetNextByte ();
                strInstLine = string.Format ("{0:X4}: {1:S4}  {2:X2} {3:X2} {4:X2}{5:X2} {6:X2}{7:X2}",
                                             iInstAddress, strExtendedMnemonic, yOpCode, yQByte,
                                             yAddress1, yAddress2,
                                             yAddress3, yAddress4);
                iOperand1Address = (yAddress1 << 8) + yAddress2;
                iOperand2Address = (yAddress3 << 8) + yAddress4;
            }
            else if (iIR1 > 0 && iIR2 == 0)
            {
                // addr:  nem  op qb xx,i xxxx
                byte yAddress1 = GetNextByte (),
                     yAddress2 = GetNextByte (),
                     yAddress3 = GetNextByte ();
                strInstLine = string.Format ("{0:X4}: {1:S4}  {2:X2} {3:X2} {4:X2},{5:X1} {6:X2}{7:X2}",
                                             iInstAddress, strExtendedMnemonic, yOpCode, yQByte,
                                             yAddress1, iIR1,
                                             yAddress2, yAddress3);
                iOperand1Address = yAddress1;
                iOperand2Address = (yAddress2 << 8) + yAddress3;
            }
            else if (iIR1 == 0 && iIR2 > 0)
            {
                // addr:  nem  op qb xxxx xx,i
                byte yAddress1 = GetNextByte (),
                     yAddress2 = GetNextByte (),
                     yAddress3 = GetNextByte ();
                strInstLine = string.Format ("{0:X4}: {1:S4}  {2:X2} {3:X2} {4:X2}{5:X2} {6:X2},{7:X1}",
                                             iInstAddress, strExtendedMnemonic, yOpCode, yQByte,
                                             yAddress1, yAddress2,
                                             yAddress3, iIR2);
                iOperand1Address = (yAddress1 << 8) + yAddress2;
                iOperand2Address = yAddress3;
            }
            else if (iIR1 > 0 && iIR2 > 0)
            {
                // addr:  nem  op qb xx,i xx,i
                byte yAddress1 = GetNextByte (),
                     yAddress2 = GetNextByte ();
                strInstLine = string.Format ("{0:X4}: {1:S4}  {2:X2} {3:X2} {4:X2},{5:X1} {6:X2},{7:X1}",
                                             iInstAddress, strExtendedMnemonic, yOpCode, yQByte,
                                             yAddress1, iIR1,
                                             yAddress2, iIR2);
                iOperand1Address = yAddress1;
                iOperand2Address = yAddress2;
            }

            AnnotateTwoOperandInst (strDasmMnemonic, yOpCode, yQByte, iOperand1Address, iOperand2Address, iIR1, iIR2);

            AddDasmLine (iInstAddress, strInstLine);

            if (m_iFirstDecodedInstructionAddress == 0)
            {
                m_iFirstDecodedInstructionAddress = iInstAddress;
            }

            // Now that there's a line of disassembled code, put the header lines at the beginning
            InsertHeaderLines ();
        }

        // <label>  addr:  nem  op qb xxxx
        // <label>  addr:  nem  op qb xx,i
        private void FormatOneOperandInst (string strDasmMnemonic)
        {
            DumpAccumulatedData ();

            int iInstAddress    = m_iObjectCodeIdx;
            int iOperandAddress = 0;
            string strInstLine  = "";

            byte yOpCode = GetNextByte ();
            int iIR = 0;
            if ((yOpCode & 0xC0) == 0xC0)
            {
                iIR = ((yOpCode & 0x20) == 0x20 ? 2 :
                       (yOpCode & 0x10) == 0x10 ? 1 : 0);
            }
            else
            {
                iIR = ((yOpCode & 0x80) == 0x80 ? 2 :
                       (yOpCode & 0x40) == 0x40 ? 1 : 0);
            }

            byte yQByte = GetNextByte ();
            string strExtendedMnemonic = GetExtendedMnemonic (strDasmMnemonic, yQByte);
            /// TODO: Is this a cause of <UNMATCHED LABEL> messages?
            if (!m_bInEmulator                &&
                strExtendedMnemonic[0] == 'B' &&
                strExtendedMnemonic[1] == ' ')
            {
                m_bInData = true;
            }

            if (iIR == 0)
            {
                // addr:  nem  op qb xxxx
                iOperandAddress = GetNextByte ();
                iOperandAddress <<= 8;
                iOperandAddress += GetNextByte ();
                strInstLine = string.Format ("{0:X4}: {1:S4}  {2:X2} {3:X2} {4:X4}", iInstAddress,
                                             strExtendedMnemonic, yOpCode, yQByte, iOperandAddress);

                if ((yOpCode & 0xCF) == 0xC2) // LA
                {
                    if ((yQByte & 0x01) == 0x01)
                    {
                        m_iSimulatedXR1Value       = iOperandAddress;
                        m_bSimulatedXR1ValueLoaded = true;
                    }

                    if ((yQByte & 0x02) == 0x02)
                    {
                        m_iSimulatedXR2Value       = iOperandAddress;
                        m_bSimulatedXR2ValueLoaded = true;
                    }
                }
            }
            else if (iIR > 0)
            {
                // addr:  nem  op qb xx,i
                iOperandAddress = GetNextByte ();
                strInstLine = string.Format ("{0:X4}: {1:S4}  {2:X2} {3:X2} {4:X2},{5:X1}", iInstAddress,
                                             strExtendedMnemonic, yOpCode, yQByte, iOperandAddress, iIR);
                if ((yOpCode & 0xCF) == 0xC2 && // LA
                    (yQByte  & 0x01) == 0x01)   // XR1
                {
                    m_iSimulatedXR1Value       = iOperandAddress + (iIR == 1 ? m_iSimulatedXR1Value : m_iSimulatedXR2Value);
                    m_bSimulatedXR1ValueLoaded = true;
                }

                if ((yOpCode & 0xCF) == 0xC2 && // LA
                    (yQByte  & 0x02) == 0x02)   // XR2
                {
                    m_iSimulatedXR2Value       = iOperandAddress + (iIR == 1 ? m_iSimulatedXR1Value : m_iSimulatedXR2Value);
                    m_bSimulatedXR2ValueLoaded = true;
                }
            }

            AnnotateOneOperandInst (iInstAddress, yOpCode, yQByte, iOperandAddress, iIR);

            AddDasmLine (iInstAddress, strInstLine);

            if (m_iFirstDecodedInstructionAddress == 0)
            {
                m_iFirstDecodedInstructionAddress = iInstAddress;
            }

            // Now that there's a line of disassembled code, put the header lines at the beginning
            InsertHeaderLines ();
        }

        // <Jump_ss_xxxx>  addr:  nem  op qb cc
        private void FormatCommandInst (string strDasmMnemonic)
        {
            DumpAccumulatedData ();

            int iInstAddress = m_iObjectCodeIdx;

            byte yOpCode = GetNextByte ();
            byte yQByte = GetNextByte ();
            byte yControlCode = GetNextByte ();
            string strExtendedMnemonic = GetExtendedMnemonic (strDasmMnemonic, yQByte);

            string strInstLine = string.Format ("{0:X4}: {1:S4}  {2:X2} {3:X2} {4:X2}", iInstAddress,
                                                strExtendedMnemonic, yOpCode, yQByte, yControlCode);

            AnnotateCommandInst (yOpCode, yQByte, yControlCode, iInstAddress);

            AddDasmLine (iInstAddress, strInstLine, true);

            if (m_iFirstDecodedInstructionAddress == 0)
            {
                m_iFirstDecodedInstructionAddress = iInstAddress;
            }

            // Now that there's a line of disassembled code, put the header lines at the beginning
            InsertHeaderLines ();
        }

        private void AnnotateTwoOperandInst (string strMnemonic, byte yOpCode, byte yQByte, int iOperand1Address, int iOperand2Address, int iIR1, int iIR2)
        {
            CalcAddress (iIR1, ref iOperand1Address);
            CalcAddress (iIR2, ref iOperand2Address);

            if ((yOpCode & 0x0F) == 0x04 || // ZAZ
                (yOpCode & 0x0F) == 0x06 || // AZ
                (yOpCode & 0x0F) == 0x07)   // SZ
            {
                m_strlComments.Add (ADDRESS_SPACE_HOLDER);
                m_strlComments.Add (AnnotateZoned (yQByte, iOperand1Address, iOperand2Address, iIR1, iIR2));
            }
            else if ((yOpCode & 0x0F) == 0x0A || // ED
                     (yOpCode & 0x0F) == 0x0B || // ITC
                     (yOpCode & 0x0F) == 0x0C || // MVC
                     (yOpCode & 0x0F) == 0x0D || // CLC
                     (yOpCode & 0x0F) == 0x0E || // ALC
                     (yOpCode & 0x0F) == 0x0F)   // SLC
            {
                m_strlComments.Add (ADDRESS_SPACE_HOLDER);
                m_strlComments.Add (AnnotateLogical (yQByte, iOperand1Address, iOperand2Address, iIR1, iIR2));
            }
            else if ((yOpCode & 0x0F) == 0x08)   // MVX
            {
                m_strlComments.Add (GetActualAddress (iIR1, iOperand1Address));
                m_strlComments.Add (GetActualAddress (iIR2, iOperand2Address));
            }
        }

        private void AnnotateOneOperandInst (int iInstAddress, byte yOpCode, byte yQByte, int iOperandAddress, int iIR)
        {
            string strComment = "";

            CalcAddress (iIR, ref iOperandAddress);

            if ((yOpCode & 0x3F) == 0x30) // SNS   
            {
                StringBuilder sbldAnnotation = new StringBuilder ();
                sbldAnnotation.Append (GetActualAddress (iIR, iOperandAddress));

                sbldAnnotation.Append (GetIODeviceName (yQByte, yOpCode));
                if (sbldAnnotation.Length > 0)
                    sbldAnnotation.Append ("  ");
                if (sbldAnnotation.ToString ().ToLower ().IndexOf ("invalid") < 0)
                    sbldAnnotation.Append (GetSNSDetails (yOpCode, yQByte));
                m_strlComments.Add (sbldAnnotation.ToString ());
            }
            else if ((yOpCode & 0x3F) == 0x31) // LIO
            {
                m_strlComments.Add (GetActualAddress (iIR, iOperandAddress));
                m_strlComments.Add (GetIODeviceName (yQByte, yOpCode));
                m_strlComments.Add (GetLIODetails (yOpCode, yQByte));
                int iFetchValue = 0;
                if (FetchValue (iOperandAddress, ref iFetchValue))
                {
                    m_strlComments.Add (string.Format ("  (0x{0:X4})", iFetchValue));
                    if (yQByte == 0x10 &&
                        m_eKeyboard == EKeyboard.KEY_5475)
                    {
                        m_strlComments.Add ("  " + "\"" + Get5475DisplayCode ((byte)(iFetchValue >> 8)) +
                                                          Get5475DisplayCode ((byte)iFetchValue)        + "\"");
                    }
                }
            }
            else if ((yOpCode & 0x3F) == 0x34) // ST
            {
                m_strlComments.Add (GetActualAddress (iIR, iOperandAddress));
                m_strlComments.Add (GetRegisterNames (yQByte));
            }
            else if ((yOpCode & 0x3F) == 0x35) // L
            {
                m_strlComments.Add (GetActualAddress (iIR, iOperandAddress));
                m_strlComments.Add (GetRegisterNames (yQByte));
                int iFetchValue = 0;
                if (FetchValue (iOperandAddress, ref iFetchValue))
                {
                    m_strlComments.Add (string.Format ("  (0x{0:X4})", iFetchValue));
                    if ((yQByte & 0x80) == 0x00)
                    {
                        if ((yQByte & 0x20) == 0x20) // PL1 IAR
                        {
                            AddTag (iFetchValue, iInstAddress, ETagType.TAG_PL1_Entry);
                            m_sdTagCalls[iInstAddress] = iFetchValue;
                        }
                        if ((yQByte & 0x10) == 0x10) // current IAR
                        {
                            AddTag (iFetchValue, iInstAddress, ETagType.TAG_MAIN_IAR);
                            m_sdTagCalls[iInstAddress] = iFetchValue;
                            if (m_bTextCardDASM             &&
                                m_iEndCardLoadAddress == 0 &&
                                iFetchValue > m_iHighAddress)
                            {
                                m_iEndCardLoadAddress = iFetchValue;
                            }
                        }
                        if ((yQByte & 0x02) == 0x02) // XR2
                        {
                            LoadIndexRegister (2, iFetchValue);
                        }
                        if ((yQByte & 0x01) == 0x01) // XR1
                        {
                            LoadIndexRegister (1, iFetchValue);
                        }
                    }
                    else
                    {
                        if ((yQByte & 0x78) == 0x00) // Interrupt level 0 IAR
                        {
                            AddTag (iFetchValue, iInstAddress, ETagType.TAG_IL0_Entry);
                            m_sdTagCalls[iInstAddress] = iFetchValue;
                        }
                        else if ((yQByte & 0x40) == 0x40) // Interrupt level 1 IAR
                        {
                            AddTag (iFetchValue, iInstAddress, ETagType.TAG_IL1_Entry);
                            m_sdTagCalls[iInstAddress] = iFetchValue;
                        }
                        else if ((yQByte & 0x20) == 0x20) // Interrupt level 2 IAR
                        {
                            AddTag (iFetchValue, iInstAddress, ETagType.TAG_IL2_Entry);
                            m_sdTagCalls[iInstAddress] = iFetchValue;
                        }
                        else if ((yQByte & 0x10) == 0x10) // Interrupt level 3 IAR
                        {
                            AddTag (iFetchValue, iInstAddress, ETagType.TAG_IL3_Entry);
                            m_sdTagCalls[iInstAddress] = iFetchValue;
                        }
                        else if ((yQByte & 0x08) == 0x08) // Interrupt level 4 IAR
                        {
                            AddTag (iFetchValue, iInstAddress, ETagType.TAG_IL4_Entry);
                            m_sdTagCalls[iInstAddress] = iFetchValue;
                        }
                    }
                }
            }
            else if ((yOpCode & 0x3F) == 0x36) // A
            {
                m_strlComments.Add (GetActualAddress (iIR, iOperandAddress));
                m_strlComments.Add (GetRegisterNames (yQByte));
            }
            else if ((yOpCode & 0x3F) == 0x38) // TBN
            {
                m_strlComments.Add (GetActualAddress (iIR, iOperandAddress));
            }
            else if ((yOpCode & 0x3F) == 0x39) // TBF
            {
                m_strlComments.Add (GetActualAddress (iIR, iOperandAddress));
            }
            else if ((yOpCode & 0x3F) == 0x3A) // SBN
            {
                m_strlComments.Add (GetActualAddress (iIR, iOperandAddress));
            }
            else if ((yOpCode & 0x3F) == 0x3B) // SBF
            {
                m_strlComments.Add (GetActualAddress (iIR, iOperandAddress));
            }
            else if ((yOpCode & 0x3F) == 0x3C) // MVI
            {
                byte yASCII = ConvertEBCDICtoASCII (yQByte);

                if (IsPrintable (yASCII))
                    strComment = string.Format ("Move '{0}' to destination", (char)yASCII);

                m_strlComments.Add (GetActualAddress (iIR, iOperandAddress));
                m_strlComments.Add (strComment);
            }
            else if ((yOpCode & 0x3F) == 0x3D) // CLI
            {
                byte yASCII = ConvertEBCDICtoASCII (yQByte);

                if (IsPrintable (yASCII))
                    strComment = string.Format ("Compare '{0}' to destination", (char)yASCII);

                m_strlComments.Add (GetActualAddress (iIR, iOperandAddress));
                m_strlComments.Add (strComment);
            }
            else if ((yOpCode & 0xCF) == 0xC0) // BC
            {
                if (m_bAutoTagJump)
                {
                    AddTag (iOperandAddress, iInstAddress);
                    m_sdTagCalls[iInstAddress] = iOperandAddress;
                }
                m_strlComments.Add (GetActualAddress (iIR, iOperandAddress));

                if (m_bTextCardDASM                  &&
                    m_iEndCardLoadAddress == 0       &&
                    iOperandAddress > m_iHighAddress &&
                    IsUnconditionalBC (yQByte))
                {
                    m_iEndCardLoadAddress = iOperandAddress;
                }
            }
            else if ((yOpCode & 0xCF) == 0xC1) // TIO
            {
                if (m_bAutoTagJump)
                {
                    AddTag (iOperandAddress, iInstAddress);
                    m_sdTagCalls[iInstAddress] = iOperandAddress;
                    if (!m_bInEmulator)
                    {
                        m_strlComments.Add (TAG_LABEL_HOLDER); // Place holder for "Jump/Loop to ..." comment
                    }
                }
                m_strlComments.Insert (0, GetActualAddress (iIR, iOperandAddress));
                m_strlComments.Add (GetIODeviceName (yQByte, yOpCode) + "  " + GetAPLTIODetails (yQByte));
            }
            else if ((yOpCode & 0xCF) == 0xC2) // LA
            {
                if ((yQByte & 0x03) == 0x00)
                {
                    strComment = "(no register selected)";
                }
                else if ((yQByte & 0x03) == 0x01)
                {
                    strComment = "XR1";
                }
                else if ((yQByte & 0x03) == 0x02)
                {
                    strComment = "XR2";
                }
                else if ((yQByte & 0x03) == 0x03)
                {
                    strComment = "XR1, XR2";
                }

                m_strlComments.Add (GetActualAddress (iIR, iOperandAddress));
                if (strComment.Length > 0)
                {
                    m_strlComments.Add (strComment);
                }
            }
        }

        private bool IsUnconditionalBC (byte yQByte)
        {
            return yQByte == 0x00 ||
                   ((yQByte & 0x87) == 0x87);
        }

        protected void AddTag (int iOperandAddress, int iInstAddress, ETagType eTagType = ETagType.TAG_Tag)
        {
            bool bIsJump = (eTagType == ETagType.TAG_Tag && iOperandAddress >  iInstAddress);
            bool bIsLoop = (eTagType == ETagType.TAG_Tag && iOperandAddress <= iInstAddress);

            if (m_sdAllTags.ContainsKey (iOperandAddress))
            {
                if (bIsJump)
                {
                    m_sdAllTags[iOperandAddress].SetJump ();
                }
                if (bIsLoop)
                {
                    m_sdAllTags[iOperandAddress].SetLoop ();
                }

                m_sdAllTags[iOperandAddress].ChangeTagType (eTagType);
            }
            else
            {
                CTagEntry objTagEntry = new CTagEntry (m_strlDisassemblyLines.Count, iOperandAddress, eTagType);

                if (bIsJump)
                {
                    objTagEntry.SetJump ();
                }
                if (bIsLoop)
                {
                    objTagEntry.SetLoop ();
                }

                m_sdAllTags[iOperandAddress] = objTagEntry;
            }
        }

        //     Instruction                  Mnemonic  Format   Time (In microseconds)
        //     ---------------------------  --------  -------  -----------------------------
        // [x] Advance Program Level        APL       Command  4.56
        // [x] Halt Program Level           HPL       Command  4.56
        // [x] Jump on Condition            JC        Command  4.56
        // [x] Start I/O                    SIO       Command  4.56
        // [x] Insert and Test Characters   ITC       X        1.52 (N + 1 + L1)
        // [x] Move Hex Character           MVX       X        1.52 (N + 2)
        // [x] Add Logical Characters       ALC       X        1.52 (N + 2L)
        // [x] Compare Logical Characters   CLC       X        1.52 (N + 2L)
        // [x] Move Characters              MVC       X        1.52 (N + 2L)
        // [x] Subtract Logica1 Characters  SLC       X        1.52 (N + 2L)
        // [x] Edit                         ED        X        1.52 (N + L1 + L2)
        // [x] Subtract Zoned Decimal       SZ        X        1.52 (N + L1 + L2) + 1.52 (R)
        // [x] Zero and Add Zoned           ZAZ       X        1.52 (N + L1 + L2) + 1.52 (R)
        // [x] Add Zoned Decimal            AZ        X        1.52 {N + L1 + L2) + 1.52 (R)
        // [x] Compare Logical Immediate    CLI       Y        1.52 (N + 1)
        // [x] Move Logical Immediate       MVI       Y        1.52 (N + 1)
        // [x] Set Bits Off Masked          SBF       Y        1.52 (N + 1)
        // [x] Set Bits On Masked           SBN       Y        1.52 (N + 1)
        // [x] Test Bits Off Masked         TBF       Y        1.52 (N + 1)
        // [x] Test Bits On Masked          TBN       Y        1.52 (N + 1)
        // [x] Add to Register              A         Y        1.52 (N + 2)
        // [x] Load I/O                     LIO       Y        1.52 (N + 2)
        // [x] Load Register                L         Y        1.52 (N + 2)
        // [x] Sense I/O                    SNS       Y        1.52 (N + 2)
        // [x] Store Register               ST        Y        1.52 (N + 2)
        // [x] Branch on Condition          BC        Z        1.52 (N)
        // [x] Load Address                 LA        Z        1.52 (N)
        // [x] Test I/O and Branch          TIO       Z        1.52 (N)
        protected int ComputeTiming (byte[] yaInstruction)
        {
            if (yaInstruction.Length < 3)
                return 0;

            byte yOpCode = yaInstruction[0],
                 yQByte  = yaInstruction[1];
            int iIR1 = -1,
                iIR2 = -1;

            if ((yOpCode & 0xF0) == 0xF0)
            {
                return 456; // Instruction timing (GA21-9103 A-17)
            }
            else if ((yOpCode & 0x30) == 0x30)
            {
                // Y format
                iIR1 = ((yOpCode & 0xC0) == 0x80 ? 2 :
                        (yOpCode & 0xC0) == 0x40 ? 1 : 0);
                if (iIR1 == 0 &&
                    yaInstruction.Length < 4)
                    return 0;

                return 152 * ((iIR1 == 0 ? 4 : 3) + 2); // Instruction timing (GA21-9103 A-17)
            }
            else if ((yOpCode & 0xC0) == 0xC0)
            {
                // Z format
                iIR1 = ((yOpCode & 0x30) == 0x20 ? 2 :
                        (yOpCode & 0x30) == 0x10 ? 1 : 0);
                if (iIR1 == 0 &&
                    yaInstruction.Length < 4)
                    return 0;

                return 152 * (iIR1 == 0 ? 4 : 3); // Instruction timing (GA21-9103 A-17)
            }
            else
            {
                // X format
                if (yaInstruction.Length < 4)
                    return 0;

                iIR1 = ((yOpCode & 0x80) == 0x80 ? 2 :
                        (yOpCode & 0x40) == 0x40 ? 1 : 0);
                iIR2 = ((yOpCode & 0x20) == 0x20 ? 2 :
                        (yOpCode & 0x10) == 0x10 ? 1 : 0);

                if ((iIR1 == 0 || iIR2 == 0) &&
                    yaInstruction.Length < 5)
                    return 0;

                if ((iIR1 == 0 && iIR2 == 0) &&
                    yaInstruction.Length < 6)
                    return 0;

                int iN = 6 - (iIR1 == 0 ? 0 : 1) - (iIR2 == 0 ? 0 : 1); // Instruction length for timing

                // Calculate operand overlap
                if ((yOpCode & 0x0F) == 0x04 || // ZAZ
                    (yOpCode & 0x0F) == 0x06 || // AZ
                    (yOpCode & 0x0F) == 0x07)   // SZ
                {
                    // Zoned arithmetic addressing
                    int iL1 = (yQByte & 0xF0) >> 4,
                        iL2 = (yQByte & 0x0F) + 1;
                    iL1 += iL2;

                    // Instruction timing (GA21-9103 A-17)
                    return 152 * (iN + iL1 + iL2); // 1.52 (N + L1 + L2) + 1.52 (R)
                }
                else if ((yOpCode & 0x0F) == 0x08) // MVX
                {
                    // Instruction timing (GA21-9103 A-17)
                    return 152 * (iN + 2); // 1.52 (N + 2)
                }

                // Instruction timing (GA21-9103 A-17)
                if ((yOpCode & 0x0F) == 0x0B)      // ITC
                {
                    return 152 * (iN + 1 + (yQByte + 1)); // 1.52 (N + 1 + L1)
                }
                else if ((yOpCode & 0x0F) == 0x0E || // ALC
                         (yOpCode & 0x0F) == 0x0C || // MVC
                         (yOpCode & 0x0F) == 0x0D || // CLC
                         (yOpCode & 0x0F) == 0x0F)   // SLC
                {
                    return 152 * (iN + ((yQByte + 1) * 2)); // 1.52 (N + 2L)
                }
                else if ((yOpCode & 0x0F) == 0x0A)   // ED
                {
                    return 152 * (iN + ((yQByte + 1) * 2));  // 1.52 (N + L1 + L2)
                }

                return 0; // Done with 2-operand instruction address calculation
            }
        }

        protected int CalcAddress (int iIR, ref int iOperand)
        {
            //Console.WriteLine (string.Format ("CalcAddress ({0}, 0x{1:X4})", iIR, iOperand));
            int iInputOperand = iOperand;
            if (iIR == 1)
            {
                int iXR1 = (/*m_bInEmulator ||*/ m_iEmulatorXR1Value > 0) ? m_iEmulatorXR1Value : m_iSimulatedXR1Value; // Simulated values fix issues with PLTX
                iOperand += iXR1;
                //Console.WriteLine (string.Format ("  m_bInEmulator ({0}), m_iEmulatorXR1Value: 0x{1:X4}, m_bSimulatedXR1ValueLoaded ({2}),\n" +
                //                                  "  m_iSimulatedXR1Value: 0x{3:X4} iOperand 0x{4:X4} -> 0x{5:X4}",
                //                                  m_bInEmulator, m_iEmulatorXR1Value, m_bSimulatedXR1ValueLoaded, m_iSimulatedXR1Value,
                //                                  iInputOperand, iOperand));
            }
            else if (iIR == 2)
            {
                int iXR2 = (/*m_bInEmulator ||*/ m_iEmulatorXR2Value > 0) ? m_iEmulatorXR2Value : m_iSimulatedXR2Value; // Simulated values fix issues with PLTX
                iOperand += iXR2;
                //Console.WriteLine (string.Format ("  m_bInEmulator ({0}), m_iEmulatorXR2Value: 0x{1:X4}, m_bSimulatedXR2ValueLoaded ({2}),\n" +
                //                                  "  m_iSimulatedXR2Value: 0x{3:X4} iOperand 0x{4:X4} -> 0x{5:X4}",
                //                                  m_bInEmulator, m_iEmulatorXR2Value, m_bSimulatedXR2ValueLoaded, m_iSimulatedXR2Value,
                //                                  iInputOperand, iOperand));
            }

            iOperand &= 0x0000FFFF;

            return iOperand;
        }

        protected void LoadIndexRegister (int iIR, int iValue)
        {
            if (!m_bInEmulator)
            {
                if (iIR == 1)
                {
                    m_iSimulatedXR1Value       = iValue;
                    m_bSimulatedXR1ValueLoaded = true;
                }
                else if (iIR == 2)
                {
                    m_iSimulatedXR2Value       = iValue;
                    m_bSimulatedXR2ValueLoaded = true;
                }
            }
        }

        protected bool FetchValue (int iAddress, ref int iFetchValue)
        {
            if (iAddress <= m_iHighAddress &&
                iAddress > m_iLowAddress)
            {
                iFetchValue = m_yaObjectCode[iAddress - 1];
                iFetchValue <<= 8;
                iFetchValue |= m_yaObjectCode[iAddress];

                return true;
            }

            return false;
        }

        protected bool ComputeAddresses (byte[] yaInstruction, ref int iOp1Start, ref int iOp1End, ref int iOp2Start,
                                         ref int iOp2End, ref int iOverlap, ref int iTiming)
        {
            return ComputeAddresses (yaInstruction, ref iOp1Start, ref iOp1End, ref iOp2Start,
                                     ref iOp2End, ref iOverlap, ref iTiming, 0, 0);
        }

        protected bool ComputeAddresses (byte[] yaInstruction, ref int iOp1Start, ref int iOp1End, ref int iOp2Start,
                                         ref int iOp2End, ref int iOverlap, ref int iTiming, int iXR1, int iXR2)
        {
            if (yaInstruction.Length < 3)
                return false;

            byte yOpCode = yaInstruction[0],
                 yQByte  = yaInstruction[1];
            int iIR1 = -1,
                iIR2 = -1;

            if ((yOpCode & 0xF0) == 0xF0)
            {
                iTiming = 456; // Instruction timing (GA21-9103 A-17)

                return false; // Command format, no addressing
            }
            else if ((yOpCode & 0x30) == 0x30)
            {
                // Y format
                iIR1 = ((yOpCode & 0xC0) == 0x80 ? 2 :
                        (yOpCode & 0xC0) == 0x40 ? 1 : 0);
                if (iIR1 == 0 &&
                    yaInstruction.Length < 4)
                    return true;

                iOp1End = yaInstruction[2];
                if (iIR1 == 0)
                {
                    // addr:  nem  op qb xxxx
                    iOp1End <<= 8;
                    iOp1End += yaInstruction[3];
                }
                else if (iIR1 == 1)
                {
                    iOp1End += iXR1;
                    iOp1End &= 0x0000FFFF;
                }
                else
                {
                    iOp1End += iXR2;
                    iOp1End &= 0x0000FFFF;
                }

                if ((yOpCode & 0x0F) == 0x00 || // SNS
                    (yOpCode & 0x0F) == 0x01 || // LIO
                    (yOpCode & 0x0F) == 0x04 || // ST
                    (yOpCode & 0x0F) == 0x05 || // L
                    (yOpCode & 0x0F) == 0x06)   // A
                {
                    iOp1Start = iOp1End - 1;
                    iOp1Start &= 0x0000FFFF;
                }
                else if ((yOpCode & 0x0F) == 0x08 || // TBN
                         (yOpCode & 0x0F) == 0x09 || // TBF
                         (yOpCode & 0x0F) == 0x0A || // SBN
                         (yOpCode & 0x0F) == 0x0B || // SBF
                         (yOpCode & 0x0F) == 0x0C || // MVI
                         (yOpCode & 0x0F) == 0x0D)   // CLI
                {
                    iOp1Start = iOp1End;
                }

                iTiming = 152 * ((iIR1 == 0 ? 4 : 3) + 2); // Instruction timing (GA21-9103 A-17)

                return true;
            }
            else if ((yOpCode & 0xC0) == 0xC0)
            {
                // Z format
                iIR1 = ((yOpCode & 0x30) == 0x20 ? 2 :
                        (yOpCode & 0x30) == 0x10 ? 1 : 0);
                if (iIR1 == 0 &&
                    yaInstruction.Length < 4)
                    return true;

                iOp1End = yaInstruction[2];
                if (iIR1 == 0)
                {
                    // addr:  nem  op qb xxxx
                    iOp1End <<= 8;
                    iOp1End += yaInstruction[3];
                }
                else if (iIR1 == 1)
                {
                    iOp1End += iXR1;
                    iOp1End &= 0x0000FFFF;
                }
                else
                {
                    iOp1End += iXR2;
                    iOp1End &= 0x0000FFFF;
                }

                iOp1Start = iOp1End - 1;
                iOp1Start &= 0x0000FFFF;

                iTiming = 152 * (iIR1 == 0 ? 4 : 3); // Instruction timing (GA21-9103 A-17)

                return true;
            }
            else
            {
                // X format
                if (yaInstruction.Length < 4)
                    return true;

                iIR1 = ((yOpCode & 0x80) == 0x80 ? 2 :
                        (yOpCode & 0x40) == 0x40 ? 1 : 0);
                iIR2 = ((yOpCode & 0x20) == 0x20 ? 2 :
                        (yOpCode & 0x10) == 0x10 ? 1 : 0);

                if ((iIR1 == 0 || iIR2 == 0) &&
                    yaInstruction.Length < 5)
                    return true;

                if ((iIR1 == 0 && iIR2 == 0) &&
                    yaInstruction.Length < 6)
                    return true;

                int iN = 6 - (iIR1 == 0 ? 0 : 1) - (iIR2 == 0 ? 0 : 1); // Instruction length for timing

                if (iIR1 == 0 && iIR2 == 0)
                {
                    // addr:  nem  op qb xxxx xxxx
                    byte yAddress1 = yaInstruction[2],
                         yAddress2 = yaInstruction[3],
                         yAddress3 = yaInstruction[4],
                         yAddress4 = yaInstruction[5];
                    iOp1End = (yAddress1 << 8) + yAddress2;
                    iOp2End = (yAddress3 << 8) + yAddress4;
                }
                else if (iIR1 > 0 && iIR2 == 0)
                {
                    // addr:  nem  op qb xx,i xxxx
                    byte yAddress1 = yaInstruction[2],
                         yAddress2 = yaInstruction[3],
                         yAddress3 = yaInstruction[4];
                    iOp1End = yAddress1;
                    iOp2End = (yAddress2 << 8) + yAddress3;

                    if (iIR1 == 1)
                    {
                        iOp1End += iXR1;
                        iOp1End &= 0x0000FFFF;
                    }
                    else
                    {
                        iOp1End += iXR2;
                        iOp1End &= 0x0000FFFF;
                    }
                }
                else if (iIR1 == 0 && iIR2 > 0)
                {
                    // addr:  nem  op qb xxxx xx,i
                    byte yAddress1 = yaInstruction[2],
                         yAddress2 = yaInstruction[3],
                         yAddress3 = yaInstruction[4];
                    iOp1End = (yAddress1 << 8) + yAddress2;
                    iOp2End = yAddress3;

                    if (iIR2 == 1)
                    {
                        iOp2End += iXR1;
                        iOp2End &= 0x0000FFFF;
                    }
                    else
                    {
                        iOp2End += iXR2;
                        iOp2End &= 0x0000FFFF;
                    }
                }
                else if (iIR1 > 0 && iIR2 > 0)
                {
                    // addr:  nem  op qb xx,i xx,i
                    byte yAddress1 = yaInstruction[2],
                         yAddress2 = yaInstruction[3];
                    iOp1End = yAddress1;
                    iOp2End = yAddress2;

                    if (iIR1 == 1)
                    {
                        iOp1End += iXR1;
                        iOp1End &= 0x0000FFFF;
                    }
                    else
                    {
                        iOp1End += iXR2;
                        iOp1End &= 0x0000FFFF;
                    }

                    if (iIR2 == 1)
                    {
                        iOp2End += iXR1;
                        iOp2End &= 0x0000FFFF;
                    }
                    else
                    {
                        iOp2End += iXR2;
                        iOp2End &= 0x0000FFFF;
                    }
                }

                // Calculate operand overlap
                if ((yOpCode & 0x0F) == 0x04 || // ZAZ
                    (yOpCode & 0x0F) == 0x06 || // AZ
                    (yOpCode & 0x0F) == 0x07)   // SZ
                {
                    // Zoned arithmetic addressing
                    int iL1 = (yQByte & 0xF0) >> 4,
                        iL2 = (yQByte & 0x0F) + 1;
                    iL1 += iL2;
                    iOverlap = 0;
                    iOp1Start = (iOp1End > iL1) ? iOp1End - iL1 + 1 : 0;
                    iOp1Start &= 0x0000FFFF;
                    iOp2Start = (iOp2End > iL2) ? iOp2End - iL2 + 1 : 0;
                    iOp2Start &= 0x0000FFFF;

                    if (iIR1 == iIR2)
                    {
                        if (iOp1Start >= iOp2Start && iOp1Start <= iOp2End)
                            iOverlap = iOp2End - iOp1Start + 1;
                        else if (iOp2Start >= iOp1Start && iOp2Start <= iOp1End)
                            iOverlap = iOp1End - iOp2Start + 1;
                    }

                    // Instruction timing (GA21-9103 A-17)
                    iTiming = 152 * (iN + iL1 + iL2); // 1.52 (N + L1 + L2) + 1.52 (R)
                }
                else if ((yOpCode & 0x0F) == 0x08) // MVX
                {
                    iOp1Start = iOp1End;
                    iOp2Start = iOp2End;

                    // Instruction timing (GA21-9103 A-17)
                    iTiming = 152 * (iN + 2); // 1.52 (N + 2)
                }
                else
                {
                    // Logical addressing
                    iOverlap = 0;
                    iOp1Start = (iOp1End > yQByte) ? iOp1End - yQByte : 0;
                    iOp1Start &= 0x0000FFFF;
                    iOp2Start = (iOp2End > yQByte) ? iOp2End - yQByte : 0;
                    iOp2Start &= 0x0000FFFF;
                    if (iIR1 == iIR2)
                    {
                        if (iOp1Start >= iOp2Start && iOp1Start <= iOp2End)
                            iOverlap = iOp2End - iOp1Start + 1;
                        else if (iOp2Start >= iOp1Start && iOp2Start <= iOp1End)
                            iOverlap = iOp1End - iOp2Start + 1;
                    }
                }

                // Instruction timing (GA21-9103 A-17)
                if ((yOpCode & 0x0F) == 0x0B)      // ITC
                {
                    iTiming = 152 * (iN + 1 + (yQByte + 1)); // 1.52 (N + 1 + L1)
                }
                else if ((yOpCode & 0x0F) == 0x0E || // ALC
                         (yOpCode & 0x0F) == 0x0C || // MVC
                         (yOpCode & 0x0F) == 0x0D || // CLC
                         (yOpCode & 0x0F) == 0x0F)   // SLC
                {
                    iTiming = 152 * (iN + ((yQByte + 1) * 2)); // 1.52 (N + 2L)
                }
                else if ((yOpCode & 0x0F) == 0x0A)   // ED
                {
                    iTiming = 152 * (iN + ((yQByte + 1) * 2));  // 1.52 (N + L1 + L2)
                }

                return true; // Done with 2-operand instruction address calculation
            }
        }

        private string GetActualAddress (int iIR, int iOperandAddress)
        {
            if (iIR == ALWAYS_CONVERT_ADDRESS)
            {
                return string.Format ("0x{0:X4}  ", iOperandAddress);
            }

            if (m_bSimulatedXR1ValueLoaded ||
                m_bSimulatedXR2ValueLoaded)
            {
                return string.Format ("0x{0:X4}  ", iOperandAddress);
            }

            return ADDRESS_SPACE_HOLDER;
        }

        private string AnnotateZoned (byte yQByte, int iOperand1Address, int iOperand2Address, int iIR1, int iIR2)
        {
            // "Add 8 bytes from  0x0020 - 0x0027 to 0x0100 - 0x0107
            // "Add 8 bytes from  0x0020 - 0x0027 to 0x0025 - 0x002C (3 bytes overlap)

            int iL1 = (yQByte & 0xF0) >> 4,
                iL2 = (yQByte & 0x0F) + 1;
            iL1 += iL2;
            int iOverlap = 0;
            int iStart1 = (iOperand1Address > iL1) ? iOperand1Address - iL1 + 1 : 0;
            int iStart2 = (iOperand2Address > iL2) ? iOperand2Address - iL2 + 1 : 0;

            if (iIR1 == iIR2)
            {
                if (iStart1 >= iStart2 && iStart1 <= iOperand2Address)
                    iOverlap = iOperand2Address - iStart1 + 1;
                else if (iStart2 >= iStart1 && iStart2 <= iOperand1Address)
                    iOverlap = iOperand1Address - iStart2 + 1;
            }

            if (iOverlap > 0)
                return string.Format ("{0} bytes: 0x{1:X4}/0x{2:X4}, {3} bytes: 0x{4:X4}/0x{5:X4} (overlap: {6})",
                                      iL1, iStart1, iOperand1Address, iL2, iStart2, iOperand2Address, iOverlap);
            else
                return string.Format ("{0} bytes: 0x{1:X4}/0x{2:X4}, {3} bytes: 0x{4:X4}/0x{5:X4}",
                                      iL1, iStart1, iOperand1Address, iL2, iStart2, iOperand2Address);
        }

        private string AnnotateLogical (byte yQByte, int iOperand1Address, int iOperand2Address, int iIR1, int iIR2)
        {
            // "Move 8 bytes from  0x0020 - 0x0027 to 0x0100 - 0x0107
            // "Move 8 bytes from  0x0020 - 0x0027 to 0x0025 - 0x002C (3 bytes overlap)

            int iOverlap = 0;
            int iStart1 = (iOperand1Address > yQByte) ? iOperand1Address - yQByte : 0;
            int iStart2 = (iOperand2Address > yQByte) ? iOperand2Address - yQByte : 0;
            if (iIR1 == iIR2)
            {
                if (iStart1 >= iStart2 && iStart1 <= iOperand2Address)
                    iOverlap = iOperand2Address - iStart1 + 1;
                else if (iStart2 >= iStart1 && iStart2 <= iOperand1Address)
                    iOverlap = iOperand1Address - iStart2 + 1;
            }

            if (iOverlap > 0)
                return string.Format ("{0} bytes: 0x{1:X4}/0x{2:X4}, 0x{3:X4}/0x{4:X4} (overlap: {5})", yQByte + 1,
                                      iStart1, iOperand1Address, iStart2, iOperand2Address, iOverlap);
            else
                return string.Format ("{0} bytes: 0x{1:X4}/0x{2:X4}, 0x{3:X4}/0x{4:X4}", yQByte + 1,
                                      iStart1, iOperand1Address, iStart2, iOperand2Address);
        }

        private string GetRegisterNames (byte yQByte)
        {
            StringBuilder strbldRegisterNames = new StringBuilder ();

            if ((yQByte & 0x80) == 0x00)
            {
                if ((yQByte & 0x40) == 0x40)
                {
                    strbldRegisterNames.Append ("PL2 IAR");
                }

                if ((yQByte & 0x20) == 0x20)
                {
                    if (strbldRegisterNames.Length > 0)
                    {
                        strbldRegisterNames.Append (", ");
                    }

                    strbldRegisterNames.Append ("PL1 IAR");
                }

                if ((yQByte & 0x10) == 0x10)
                {
                    if (strbldRegisterNames.Length > 0)
                    {
                        strbldRegisterNames.Append (", ");
                    }

                    strbldRegisterNames.Append ("current IAR");
                }

                if ((yQByte & 0x08) == 0x08)
                {
                    if (strbldRegisterNames.Length > 0)
                    {
                        strbldRegisterNames.Append (", ");
                    }

                    strbldRegisterNames.Append ("ARR");
                }

                if ((yQByte & 0x04) == 0x04)
                {
                    if (strbldRegisterNames.Length > 0)
                    {
                        strbldRegisterNames.Append (", ");
                    }

                    strbldRegisterNames.Append ("PSR");
                }

                if ((yQByte & 0x02) == 0x02)
                {
                    if (strbldRegisterNames.Length > 0)
                    {
                        strbldRegisterNames.Append (", ");
                    }

                    strbldRegisterNames.Append ("XR2");
                }

                if ((yQByte & 0x01) == 0x01)
                {
                    if (strbldRegisterNames.Length > 0)
                    {
                        strbldRegisterNames.Append (", ");
                    }

                    strbldRegisterNames.Append ("XR1");
                }
            }
            else
            {
                if ((yQByte & 0x78) == 0x00)
                {
                    strbldRegisterNames.Append ("Interrupt level 0 IAR");
                }
                else if ((yQByte & 0x40) == 0x40)
                {
                    strbldRegisterNames.Append ("Interrupt level 1 IAR");
                }
                else if ((yQByte & 0x20) == 0x20)
                {
                    if (strbldRegisterNames.Length > 0)
                    {
                        strbldRegisterNames.Append (", ");
                    }

                    strbldRegisterNames.Append ("Interrupt level 2 IAR");
                }
                else if ((yQByte & 0x10) == 0x10)
                {
                    if (strbldRegisterNames.Length > 0)
                    {
                        strbldRegisterNames.Append (", ");
                    }

                    strbldRegisterNames.Append ("Interrupt level 3 IAR");
                }
                else if ((yQByte & 0x08) == 0x08)
                {
                    if (strbldRegisterNames.Length > 0)
                    {
                        strbldRegisterNames.Append (", ");
                    }

                    strbldRegisterNames.Append ("Interrupt level 4 IAR");
                }
            }

            return strbldRegisterNames.ToString ();
        }

        private void AnnotateCommandInst (byte yOpCode, byte yQByte, byte yControlCode, int iInstAddress)
        {
            if (yOpCode == 0xF0) // HPL
            {
                m_strlComments.Insert (0, string.Format ("Halt display: \"{0:S1}{1:S1}\"", GetHaltDisplayCode (yQByte), GetHaltDisplayCode (yControlCode)));
                if (!m_bInEmulator)
                {
                    m_strlComments.Insert (0, ADDRESS_SPACE_HOLDER);
                }
            }
            else if (yOpCode == 0xF1) // APL
            {
                if (!m_bInEmulator)
                {
                    m_strlComments.Add (ADDRESS_SPACE_HOLDER);
                }
                m_strlComments.Add (GetIODeviceName (yQByte, yOpCode) + "  " + GetAPLTIODetails (yQByte));
            }
            else if (yOpCode == 0xF2) // JC
            {
                int iJumpAddress = m_iObjectCodeIdx + yControlCode;
                if (m_bInEmulator)
                {
                    iJumpAddress += m_iEmulatorIAR;
                }

                if (yControlCode > 0)
                {
                    m_strlComments.Add (GetActualAddress (ALWAYS_CONVERT_ADDRESS, iJumpAddress));
                }

                if (m_bAutoTagJump &&
                    yControlCode > 0)
                {
                    // m_iObjectCodeIdx already points to the next byte after the current instruction
                    //CTagEntry objTagEntry = new CTagEntry (m_strlDisassemblyLines.Count, iJumpAddress, ETagType.TAG_Tag);
                    //objTagEntry.SetJump ();
                    //m_sdAllTags[iJumpAddress] = objTagEntry;
                    if (m_sdAllTags.ContainsKey (iJumpAddress))
                    {
                        m_sdAllTags[iJumpAddress].SetJump ();
                    }
                    else
                    {
                        CTagEntry objTagEntry = new CTagEntry (m_strlDisassemblyLines.Count, iJumpAddress, ETagType.TAG_Tag);
                        objTagEntry.SetJump ();
                        m_sdAllTags[iJumpAddress] = objTagEntry;
                    }
                    m_sdTagCalls[iInstAddress] = iJumpAddress;
                }
            }
            else if (yOpCode == 0xF3) // SIO
            {
                if (!m_bInEmulator)
                {
                    m_strlComments.Add (ADDRESS_SPACE_HOLDER);
                }
                if ((yQByte & 0xF0) == 0xA0 ||
                    (yQByte & 0xF0) == 0xB0)
                {
                    string strSioDetails = GetSIODetails (yQByte, yControlCode);
                    List<string> strlSioDetails = SplitLongLines (strSioDetails);

                    AppendStringList (strlSioDetails, m_strlComments);
                }
                else
                {
                    string strComment = GetSIODetails (yQByte);
                    m_strlComments.Add (strComment);
                    if (strComment.ToLower ().IndexOf ("invalid") < 0)
                    {

                        string strControlCodeDetails = GetSIOControlCodeDetails (yQByte, yControlCode);
                        List<string> strlControlCodeDetails = SplitLongLines (strControlCodeDetails);
                        AppendStringList (strlControlCodeDetails, m_strlComments);
                    }
                }
            }
            else
            {
                // Invalid OP code
            }
        }

        //          1         2         3         4         5         6         7         8         9
        //0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
        //Program Lower Shift  Restore Data Key  Unlock Data Key  Enable Interrupt  Reset Interrupt
        //                   1                 3                5                 7                8
        //0                  9                 7                4                 2                9
        //1234567890123456789  1234567890123456  123456789012345  1234567890123456  123456789012345
        //19-0-1               37-19-2           54-37-2          72-54-2
        List<string> SplitLongLines (string strLongLine, bool bSplitAlways = true)
        {
            List<string> strlReturn = new List<string> ();

            if (!bSplitAlways &&
                strLongLine.Length <= m_iMaxLineLength)
            {
                strlReturn.Add (strLongLine);
                return strlReturn;
            }

            int iLastBreak = 0;
            strLongLine += "  "; // Trailing match line
            for (int iBreak = strLongLine.IndexOf ("  "); iLastBreak < strLongLine.Length; iBreak = strLongLine.IndexOf ("  ", iBreak + 1))
            {
                if (bSplitAlways||
                    (iBreak >= m_iMaxLineLength &&
                     iBreak < strLongLine.Length))
                {
                    string strShortLine = strLongLine.Substring (iLastBreak, iBreak - iLastBreak);
                    strlReturn.Add (strShortLine + "  ");
                    if (!bSplitAlways &&
                        strLongLine.Length < m_iMaxLineLength)
                    {
                        strlReturn.Add (strLongLine.TrimEnd ());
                        break;
                    }
                    iLastBreak = iBreak + 2;
                }
                else
                {
                    iLastBreak = iBreak + 2;
                }
            }

            return strlReturn;
        }

        private string GetIODeviceName (byte yQByte, byte yOpCode)
        {
            if ((yQByte & 0xF0) == 0x00)
            {
                if ((yOpCode & 0x3F) == 0x30) // SNS
                    return "CPU Console";
                else
                    return "DPF";
            }
            else if ((yQByte & 0xF0) == 0xE0)
            {
                return "5203 Line Printer";
            }
            else if ((yQByte & 0xF0) == 0xF0)
            {
                return "5424 MFCU";
            }
            else if ((yQByte & 0xF0) == 0xA0)
            {
                return "5444 Disk Drive 1";
            }
            else if ((yQByte & 0xF0) == 0xB0)
            {
                return "5444 Disk Drive 2";
            }
            else if ((yQByte & 0xF0) == 0x10)
            {
                if (m_eKeyboard == EKeyboard.KEY_5471)
                {
                    return "5471 Printer/Keyboard Console  " + (((yQByte & 0x08) == 0x08) ? "-  Printer  " : "-  Keyboard");
                }
                else if (m_eKeyboard == EKeyboard.KEY_5475)
                {
                    return "5475 Keyboard";
                }
                else
                {
                    return "5471/5475 Keyboard";
                }
            }
            else if ((yQByte & 0xF0) == 0x30)
            {
                return "Serial Input/Output Channel Adapter";
            }
            else if ((yQByte & 0xF0) == 0x80)
            {
                return "BSCA 1";
            }
            else if ((yQByte & 0xF0) == 0xC0)
            {
                return "BSCA 2";
            }
            else if ((yQByte & 0xF0) == 0x60 ||
                     (yQByte & 0xF0) == 0x70)
            {
                if ((yQByte & 0xF8) == 0x60)
                {
                    return "Tape Drive 1";
                }
                else if ((yQByte & 0xF8) == 0x68)
                {
                    return "Tape Drive 2";
                }
                else if ((yQByte & 0xF8) == 0x70)
                {
                    return "Tape Drive 3";
                }
                else if ((yQByte & 0xF8) == 0x78)
                {
                    return "Tape Drive 4";
                }
            }
            else if ((yQByte & 0xF0) == 0x50)
            {
                return "1442 Card Read Punch";
            }
            else if ((yQByte & 0xF8) == 0x40)
            {
                return "3741 Data/Work Station";
            }

            return "";
        }

        private string GetSIODetails (byte yQByte, byte yControlCode = 0)
        {
            if ((yQByte & 0xF0) == 0x00) // Dual Programming Feature
            {
                return "DPF";
            }
            else if ((yQByte & 0xF0) == 0xE0) // 5203 Line Printer
            {
                StringBuilder sbldDevice = new StringBuilder ();

                sbldDevice.Append ("5203 Line Printer");

                if ((yQByte & 0x07) == 0x00 ||
                    (yQByte & 0x07) == 0x02 ||
                    (yQByte & 0x07) == 0x04 ||
                    (yQByte & 0x07) == 0x06)
                {
                    sbldDevice.Append ((yQByte & 0x08) == 0 ? "  Left/only carriage" : "  Right carriage");
                }

                switch (yQByte & 0x07)
                {
                    case 0x00: sbldDevice.Append ("  Space only"); break;
                    case 0x02: sbldDevice.Append ("  Print and space"); break;
                    case 0x04: sbldDevice.Append ("  Skip only"); break;
                    case 0x06: sbldDevice.Append ("  Print and skip"); break;
                    default:   sbldDevice.Append (string.Format ("  <Invalid Q byte> 0x{0:X2}", yQByte)); break;
                }

                return sbldDevice.ToString ();
            }
            else if ((yQByte & 0xF0) == 0xF0) // 5424 MFCU
            {
                StringBuilder sbldDevice = new StringBuilder ();

                sbldDevice.Append ("5424 MFCU");
                sbldDevice.Append ((yQByte & 0x08) == 0 ? "  Primary" : "  Secondary");

                switch (yQByte & 0x07)
                {
                    case 0x00: sbldDevice.Append ("  Feed only"); break;
                    case 0x01: sbldDevice.Append ("  Read"); break;
                    case 0x02: sbldDevice.Append ("  Punch"); break;
                    case 0x03: sbldDevice.Append ("  Read/Punch"); break;
                    case 0x04: sbldDevice.Append ("  Print"); break;
                    case 0x05: sbldDevice.Append ("  Read/Print"); break;
                    case 0x06: sbldDevice.Append ("  Punch/Print"); break;
                    case 0x07: sbldDevice.Append ("  Read/Punch/Print"); break;
                }

                return sbldDevice.ToString ();
            }
            else if ((yQByte & 0xF0) == 0xA0 || // 5444 Disk Drive 1
                     (yQByte & 0xF0) == 0xB0)   // 5444 Disk Drive 2
            {
                StringBuilder sbldDevice = new StringBuilder ();

                sbldDevice.Append ((yQByte & 0xF0) == 0xA0 ? "5444 Disk Drive 1" : "5444 Disk Drive 2");

                string strAction = "";
                bool bInvalidQByte = false;

                switch (yQByte & 0x07)
                {
                    case 0x00: strAction = "  Seek"; break;
                    case 0x01:
                    {
                        if ((yControlCode & 0x03) == 0)
                        {
                            strAction = "  Read Data";
                        }
                        else if ((yControlCode & 0x03) == 1)
                        {
                            strAction = "  Read Identifier";
                        }
                        else if ((yControlCode & 0x03) == 2)
                        {
                            strAction = "  Read Data Diagnostic";
                        }
                        else // 3
                        {
                            strAction = "  Verify";
                        }
                        break;
                    }
                    case 0x02:
                    {
                        if ((yControlCode & 0x01) == 0)
                        {
                            strAction = "  Write Data";
                        }
                        else
                        {
                            strAction = "  Write Identifier";
                        }
                        break;
                    }
                    case 0x03:
                    {
                        if ((yControlCode & 0x03) == 0)
                        {
                            strAction = "  Scan Equal";
                        }
                        else if ((yControlCode & 0x03) == 1)
                        {
                            strAction = "  Scan Low or Equal";
                        }
                        else // 2 or 3
                        {
                            strAction = "  Scan High or Equal";
                        }
                        break;
                    }
                    default:
                    {
                        sbldDevice.Append (string.Format ("  <Invalid Q byte> 0x{0:X2}", yQByte));
                        bInvalidQByte = true;
                        break;
                    }
                }

                if (!bInvalidQByte)
                {
                    sbldDevice.Append ((yQByte & 0x08) == 0 ? "  Removable disk" : "  Fixed disk");
                    sbldDevice.Append (strAction);
                }

                return sbldDevice.ToString ();
            }
            else if ((yQByte & 0xF0) == 0x10) // 5471/5475 Keyboard
            {
                StringBuilder sbldDevice = new StringBuilder ();

                if (m_eKeyboard == EKeyboard.KEY_5471)
                {
                    sbldDevice.Append ("5471 Printer/Keyboard Console  ");
                    sbldDevice.Append (((yQByte & 0x08) == 0x08) ? "-  Printer  " : "-  Keyboard");
                }
                else if (m_eKeyboard == EKeyboard.KEY_5475)
                {
                    sbldDevice.Append ("5475 Keyboard");
                    sbldDevice.Append ((yQByte & 0x0F) == 0 ? "  " : string.Format ("  <Invalid Q byte> 0x{0:X2}", yQByte));
                }
                else
                {
                    sbldDevice.Append ("5471/5475 Keyboard");
                }

                return sbldDevice.ToString ();
            }
            else if ((yQByte & 0xF0) == 0x30) // Serial Input/Output Channel Adapter
            {
                return "Serial Input/Output Channel Adapter";
            }
            else if ((yQByte & 0xF0) == 0x80) // BSCA
            {
                return "BSCA 1";
            }
            else if ((yQByte & 0xF0) == 0xC0) // BSCA
            {
                return "BSCA 2";
            }
            else if ((yQByte & 0xF0) == 0x60 || // Tape Drives
                     (yQByte & 0xF0) == 0x70)
            {
                if ((yQByte & 0xF8) == 0x60)
                {
                    return "Tape Drive 1";
                }
                else if ((yQByte & 0xF8) == 0x68)
                {
                    return "Tape Drive 2";
                }
                else if ((yQByte & 0xF8) == 0x70)
                {
                    return "Tape Drive 3";
                }
                else if ((yQByte & 0xF8) == 0x78)
                {
                    return "Tape Drive 4";
                }
            }
            else if ((yQByte & 0xF0) == 0x50) // 1442 Card Read Punch
            {
                return "1442 Card Read Punch";
            }
            else if ((yQByte & 0xF8) == 0x40) // 3741 Data/Work Station
            {
                return "3741 Data/Work Station";
            }

            return "";
        }

        private string GetSIOControlCodeDetails (byte yQByte, byte yControlCode)
        {
            if ((yQByte & 0xF0) == 0x00) // Dual Programming Feature
            {
                StringBuilder sbldDevice = new StringBuilder ();

                if ((yControlCode & 0x80) > 0)
                {
                    sbldDevice.Append ("  Bit 0: reserved");
                }
                if ((yControlCode & 0x40) > 0)
                {
                    sbldDevice.Append ("  Bit 1: reserved");
                }
                if ((yControlCode & 0x20) > 0)
                {
                    sbldDevice.Append ("  Bit 2: reserved");
                }
                if ((yControlCode & 0x10) > 0)
                {
                    sbldDevice.Append ("  Bit 3: reserved");
                }
                if ((yControlCode & 0x08) > 0)
                {
                    sbldDevice.Append ("  Bit 4: reserved");
                }
                if ((yControlCode & 0x04) > 0)
                {
                    sbldDevice.Append ("  Bit 5: Enable/Disable DPF");
                }
                if ((yControlCode & 0x02) > 0)
                {
                    sbldDevice.Append ("  Bit 6: Enable/Disable IL0 Interrupt Button");
                }
                if ((yControlCode & 0x01) > 0)
                {
                    sbldDevice.Append ("  Bit 7: Reset Interrupt Request 0");
                }

                return sbldDevice.ToString ();
            }
            else if ((yQByte & 0xF0) == 0xE0) // 5203 Line Printer
            {
                if ((yQByte & 0x04) == 0)
                {
                    if (yControlCode > 3)
                        return (string.Format ("  Space {0} lines invalid", yControlCode));
                    else
                        return (string.Format ("  Space {0} lines", yControlCode));
                }
                else
                {
                    if (yControlCode > 112)
                        return (string.Format ("  Skip to line {0} invalid", yControlCode));
                    else
                        return (string.Format ("  Skip to line {0}", yControlCode));
                }
            }
            else if ((yQByte & 0xF0) == 0xF0) // 5424 MFCU
            {
                StringBuilder sbldDevice = new StringBuilder ();

                if ((yQByte & 0x04) > 0)
                {
                    sbldDevice.Append ((yControlCode & 0x80) == 0 ? "  Print buffer 1" : "  Print buffer 2");
                    if ((yControlCode & 0x20) > 0)
                        sbldDevice.Append ("  Print 4 lines");
                }

                if ((yQByte & 0x01) > 0 &&
                    (yControlCode & 0x40) > 0)
                    sbldDevice.Append ("  IPL Read");

                switch ((yControlCode & 0x07))
                {
                    case 0x04: sbldDevice.Append ("  Stacker 4"); break;
                    case 0x05: sbldDevice.Append ("  Stacker 1"); break;
                    case 0x06: sbldDevice.Append ("  Stacker 2"); break;
                    case 0x07: sbldDevice.Append ("  Stacker 3"); break;
                    default:   sbldDevice.Append ((yQByte & 0x08) == 0 ? "  Stacker 1 (default)" : "  Stacker 4 (default)"); break;
                }

                return sbldDevice.ToString ();
            }
            else if ((yQByte & 0xF0) == 0x10) // 5471/5475 Keyboard
            {
                StringBuilder sbldDevice = new StringBuilder ();

                if (m_eKeyboard == EKeyboard.KEY_5471)
                {
                    if ((yQByte & 0x08) > 0) // Printer
                    {
                        if ((yControlCode & 0x20) > 0)
                        {
                            sbldDevice.Append ("  <spare>");
                        }

                        if ((yControlCode & 0x10) > 0)
                        {
                            sbldDevice.Append ("  <spare>");
                        }

                        if ((yControlCode & 0x08) > 0)
                        {
                            sbldDevice.Append ("  <spare>");
                        }

                        if ((yControlCode & 0x02) > 0)
                        {
                            sbldDevice.Append ("  <spare>");
                        }

                        if ((yControlCode & 0x80) > 0)
                        {
                            sbldDevice.Append ("  Start Print");
                        }

                        if ((yControlCode & 0x40) > 0)
                        {
                            sbldDevice.Append ("  Start Carrier Return");
                        }

                        if ((yControlCode & 0x01) > 0)
                        {
                            sbldDevice.Append ("  Reset Printer Interrupt");
                        }

                        if ((yControlCode & 0x04) > 0)
                        {
                            sbldDevice.Append ("  Enable Printer Interrupt");
                        }
                        else
                        {
                            sbldDevice.Append ("  Disable Printer Interrupt");
                        }
                    }
                    else // Keyboard
                    {
                        if ((yControlCode & 0x80) > 0)
                        {
                            sbldDevice.Append ("  <spare>");
                        }

                        if ((yControlCode & 0x40) > 0)
                        {
                            sbldDevice.Append ("  <spare>");
                        }

                        if ((yControlCode & 0x08) > 0)
                        {
                            sbldDevice.Append ("  <spare>");
                        }

                        if ((yControlCode & 0x01) > 0)
                        {
                            sbldDevice.Append ("  Reset Request Key or Other Key Interrupts");
                        }

                        if ((yControlCode & 0x20) > 0)
                        {
                            sbldDevice.Append ("  Turn On Request Pending Indicator");
                        }
                        else
                        {
                            sbldDevice.Append ("  Turn Off Request Pending Indicator");
                        }

                        if ((yControlCode & 0x10) > 0)
                        {
                            sbldDevice.Append ("  Turn On Proceed Indicator");
                        }
                        else
                        {
                            sbldDevice.Append ("  Turn Off Proceed Indicator");
                        }

                        if ((yControlCode & 0x04) > 0)
                        {
                            sbldDevice.Append ("  Enable Request Key Interrupts");
                        }
                        else
                        {
                            sbldDevice.Append ("  Disable Request Key Interrupts");
                        }

                        if ((yControlCode & 0x02) > 0)
                        {
                            sbldDevice.Append ("  Enable Other Key Interrupts");
                        }
                        else
                        {
                            sbldDevice.Append ("  Disable Other Key Interrupts");
                        }
                    }
                }
                else if (m_eKeyboard == EKeyboard.KEY_5475)
                {
                    if (yControlCode == 0)
                    {
                        sbldDevice.Append ("  (no action specified in control code)");
                    }
                    else
                    {
                        if ((yControlCode & 0x80) > 0)
                        {
                            sbldDevice.Append ("  Program Numeric Mode");
                        }

                        if ((yControlCode & 0x40) > 0)
                        {
                            sbldDevice.Append ("  Program Lower Shift");
                        }

                        if ((yControlCode & 0x20) > 0)
                        {
                            sbldDevice.Append ("  Set Error Indicator");
                        }

                        if ((yControlCode & 0x10) > 0)
                        {
                            sbldDevice.Append ("  <spare>");
                        }

                        if ((yControlCode & 0x08) > 0)
                        {
                            sbldDevice.Append ("  Restore Data Key");
                        }

                        if ((yControlCode & 0x04) > 0)
                        {
                            sbldDevice.Append ("  Unlock Data Key");
                        }

                        if ((yControlCode & 0x02) > 0)
                        {
                            sbldDevice.Append ("  Enable Interrupt");
                        }
                        else
                        {
                            sbldDevice.Append ("  Disable Interrupt");
                        }

                        if ((yControlCode & 0x01) > 0)
                        {
                            sbldDevice.Append ("  Reset Interrupt");
                        }
                    }
                }

                string strDevice = sbldDevice.ToString ();

                if (sbldDevice.Length > 2)
                    return sbldDevice.ToString ().Substring (2);
                else
                    return "";
            }
            else if ((yQByte & 0xF0) == 0x30) // Serial Input/Output Channel Adapter
            {
                return "";
            }
            else if ((yQByte & 0xF0) == 0x80 || // BSCA 1
                     (yQByte & 0xF0) == 0xC0)   // BSCA 2
            {
                return "";
            }
            else if ((yQByte & 0xF0) == 0x60 || // Tape Drive
                     (yQByte & 0xF0) == 0x70)
            {
                return "";
            }
            else if ((yQByte & 0xF0) == 0x50) // 1442 Card Read Punch
            {
                return "";
            }
            else if ((yQByte & 0xF8) == 0x40) // 3741 Data/Work Station
            {
                return "";
            }

            return "";
        }

        private string GetAPLTIODetails (byte yQByte)
        {
            if ((yQByte & 0xF0) == 0x00) // Dual Programming Feature
            {
                StringBuilder sbldDevice = new StringBuilder ();

                sbldDevice.Append ((yQByte & 0x04) == 0 ? "PL1" : "PL2");

                if ((yQByte & 0x03) == 0)
                    sbldDevice.Append ("  Cancel program level");
                else if ((yQByte & 0x03) == 1)
                    sbldDevice.Append ("  Load program level from MFCU");
                else if ((yQByte & 0x03) == 2)
                    sbldDevice.Append ("  Reserved");
                else if ((yQByte & 0x03) == 3)
                    sbldDevice.Append ("  Load program level from printer keyboard");

                return sbldDevice.ToString ();
            }
            else if ((yQByte & 0xF0) == 0xE0) // 5203 Line Printer
            {
                StringBuilder sbldDevice = new StringBuilder ();

                switch ((yQByte & 0x0F))
                {
                    case 0x00: sbldDevice.Append ("Not ready / check"); break;
                    case 0x02: sbldDevice.Append ("Print buffer busy"); break;
                    case 0x04: sbldDevice.Append ("Carriage busy"); break;
                    case 0x06: sbldDevice.Append ("Printer busy"); break;
                    case 0x09: sbldDevice.Append ("Diagnostic mode"); break;
                    default: sbldDevice.Append (string.Format ("<Invalid Q byte> 0x{0:X2}", yQByte)); break;
                }

                return sbldDevice.ToString ();
            }
            else if ((yQByte & 0xF0) == 0xF0) // 5424 MFCU
            {
                StringBuilder sbldDevice = new StringBuilder ();

                sbldDevice.Append ((yQByte & 0x08) == 0 ? "Primary" : "Secondary");

                switch (yQByte & 0x07)
                {
                    case 0x00: sbldDevice.Append ("  Not ready / check"); break;
                    case 0x01: sbldDevice.Append ("  Read/feed busy"); break;
                    case 0x02: sbldDevice.Append ("  Punch data busy"); break;
                    case 0x03: sbldDevice.Append ("  Read/feed/punch busy"); break;
                    case 0x04: sbldDevice.Append ("  Print busy"); break;
                    case 0x05: sbldDevice.Append ("  Read/feed/print busy"); break;
                    case 0x06: sbldDevice.Append ("  Print/punch busy"); break;
                    case 0x07: sbldDevice.Append ("  Print/punch/read/feed busy"); break;
                }

                return sbldDevice.ToString ();
            }
            else if ((yQByte & 0xF0) == 0xA0 || // 5444 Disk Drive 1
                     (yQByte & 0xF0) == 0xB0)   // 5444 Disk Drive 2
            {
                StringBuilder sbldDevice = new StringBuilder ();

                switch (yQByte & 0x07)
                {
                    case 0x00: sbldDevice.Append ("Not ready / check"); break;
                    case 0x02: sbldDevice.Append ("Busy"); break;
                    case 0x04: sbldDevice.Append ("Scan found"); break;
                    default: sbldDevice.Append (string.Format ("<Invalid Q byte> 0x{0:X2}", yQByte)); break;
                }

                return sbldDevice.ToString ();
            }
            else if ((yQByte & 0xF0) == 0x10) // 5471/5475 Keyboard
            {
                return string.Format ("<Invalid Q byte> 0x{0:X2}", yQByte);
            }
            else if ((yQByte & 0xF0) == 0x30) // Serial Input/Output Channel Adapter
            {
                return "";
            }
            else if ((yQByte & 0xF0) == 0x80 || // BSCA 1
                     (yQByte & 0xF0) == 0xC0)   // BSCA 2
            {
                return "";
            }
            else if ((yQByte & 0xF0) == 0x60 || // Tape Drive
                     (yQByte & 0xF0) == 0x70)
            {
                return "";
            }
            else if ((yQByte & 0xF0) == 0x50) // 1442 Card Read Punch
            {
                return "";
            }
            else if ((yQByte & 0xF8) == 0x40) // 3741 Data/Work Station
            {
                return "";
            }

            return "";
        }

        private string GetSNSDetails (byte yOpCode, byte yQByte)
        {
            if ((yQByte & 0xF0) == 0x00)
            {
                return "Read Console Dials";
            }
            else if ((yQByte & 0xF0) == 0xE0) // 5203 Line Printer
            {
                StringBuilder sbldDevice = new StringBuilder ();

                if ((yQByte & 0x08) == 0)
                {
                    switch (yQByte & 0x07)
                    {
                        case 0:  sbldDevice.Append ("Carriage Print Line Location Counter"); break;
                        case 1:  sbldDevice.Append ("Chain Character Counter"); break;
                        case 2:  sbldDevice.Append ("Printer Timing"); break;
                        case 3:  sbldDevice.Append ("Printer Check Status"); break;
                        case 4:  sbldDevice.Append ("Printer Chain Image Address Register"); break;
                        case 6:  sbldDevice.Append ("Printer Data Address Register"); break;
                        default: sbldDevice.Append (string.Format ("<Invalid Q byte> 0x{0:X2}", yQByte)); break;
                    }
                }
                else
                {
                    sbldDevice.Append (string.Format ("<Invalid Q byte> 0x{0:X2}", yQByte));
                }

                return sbldDevice.ToString ();
            }
            else if ((yQByte & 0xF0) == 0xF0) // 5424 MFCU
            {
                StringBuilder sbldDevice = new StringBuilder ();

                if ((yQByte & 0x08) == 0)
                {
                    switch (yQByte & 0x07)
                    {
                        case 0:  sbldDevice.Append ("Special indicators for CE use"); break;
                        case 1:  sbldDevice.Append ("Special indicators for CE use"); break;
                        case 3:  sbldDevice.Append ("Status indicators"); break;
                        case 4:  sbldDevice.Append ("MFCU print DAR"); break;
                        case 5:  sbldDevice.Append ("MFCU read DAR");  break;
                        case 6:  sbldDevice.Append ("MFCU punch DAR"); break;
                        default: sbldDevice.Append (string.Format ("<Invalid Q byte> 0x{0:X2}", yQByte)); break;
                    }
                }
                else
                {
                    sbldDevice.Append (string.Format ("<Invalid Q byte> 0x{0:X2}", yQByte));
                }

                return sbldDevice.ToString ();
            }
            else if ((yQByte & 0xF0) == 0xA0 || // 5444 Disk Drive 1
                     (yQByte & 0xF0) == 0xB0)   // 5444 Disk Drive 2
            {
                StringBuilder sbldDevice = new StringBuilder ();

                switch (yQByte & 0x07)
                {
                    case 2:  sbldDevice.Append ("Status bytes 0 and 1"); break;
                    case 3:  sbldDevice.Append ("Status bytes 2 and 3"); break;
                    case 4:  sbldDevice.Append ("Disk read/write address register"); break;
                    case 6:  sbldDevice.Append ("Disk control address register"); break;
                    default: sbldDevice.Append (string.Format ("<Invalid Q byte> 0x{0:X2}", yQByte)); break;
                }

                return sbldDevice.ToString ();
            }
            else if ((yQByte & 0xF0) == 0x10) // 5471/5475 Keyboard
            {
                StringBuilder sbldDevice = new StringBuilder ();

                if (m_eKeyboard == EKeyboard.KEY_5471)
                {
                    switch (yQByte & 0x07)
                    {
                        case 0x01:
                        {
                            if ((yQByte & 0x08) > 0) // Printer
                            {
                                return "Sense bytes 0 & 1";
                            }
                            else // Keyboard
                            {
                                return "Keystroke byte & sense byte 1";
                            }
                        }
                        case 0x03:
                        {
                            return "Diagnostic sense bytes";
                        }
                        default:
                        {
                            return string.Format ("<Invalid Q byte> 0x{0:2X}", yQByte & 0x07);
                        }
                    }
                }
                else if (m_eKeyboard == EKeyboard.KEY_5475)
                {
                    if ((yQByte & 0x08) == 0)
                    {
                        switch (yQByte & 0x07)
                        {
                            case 1:  sbldDevice.Append ("Keystroke byte & 1 sense byte"); break;
                            case 2:  sbldDevice.Append ("2 sense bytes"); break;
                            case 3:  sbldDevice.Append ("Diagnostic sense bytes"); break;
                            default: sbldDevice.Append (string.Format ("<Invalid Q byte> 0x{0:X2}", yQByte)); break;
                        }
                    }
                    else
                    {
                        sbldDevice.Append (string.Format ("<Invalid Q byte> 0x{0:X2}", yQByte));
                    }
                }

                return sbldDevice.ToString ();
            }
            else if ((yQByte & 0xF0) == 0x30) // Serial Input/Output Channel Adapter
            {
                return "";
            }
            else if ((yQByte & 0xF0) == 0x80 || // BSCA 1
                     (yQByte & 0xF0) == 0xC0)   // BSCA 2
            {
                return "";
            }
            else if ((yQByte & 0xF0) == 0x60 || // Tape Drive
                     (yQByte & 0xF0) == 0x70)
            {
                return "";
            }
            else if ((yQByte & 0xF0) == 0x50) // 1442 Card Read Punch
            {
                return "";
            }
            else if ((yQByte & 0xF8) == 0x40) // 3741 Data/Work Station
            {
                return "";
            }

            return "";
        }

        private string GetLIODetails (byte yOpCode, byte yQByte)
        {
            if ((yQByte & 0xF0) == 0xE0) // 5203 Line Printer
            {
                StringBuilder sbldDevice = new StringBuilder ();

                if ((yQByte & 0x08) == 0)
                {
                    switch (yQByte & 0x07)
                    {
                        case 0:  sbldDevice.Append ("  Forms length register"); break;
                        case 4:  sbldDevice.Append ("  Printer image address register");  break;
                        case 6:  sbldDevice.Append ("  Printer data address register"); break;
                        default: sbldDevice.Append (string.Format ("<Invalid Q byte> 0x{0:X2}", yQByte)); break;
                    }
                }
                else
                {
                    sbldDevice.Append (string.Format ("<Invalid Q byte> 0x{0:X2}", yQByte));
                }

                return sbldDevice.ToString ();
            }
            else if ((yQByte & 0xF0) == 0xF0) // 5424 MFCU
            {
                StringBuilder sbldDevice = new StringBuilder ();

                if ((yQByte & 0x07) == 0x04 ||
                    (yQByte & 0x07) == 0x05 ||
                    (yQByte & 0x07) == 0x06)
                {
                    sbldDevice.Append ((yQByte & 0x08) == 0 ? "  Normal mode  " : "  Diagnostic mode  ");
                }

                switch (yQByte & 0x07)
                {
                    case 4:  sbldDevice.Append ("MFCU print DAR"); break;
                    case 5:  sbldDevice.Append ("MFCU read DAR");  break;
                    case 6:  sbldDevice.Append ("MFCU punch DAR"); break;
                    default: sbldDevice.Append (string.Format ("<Invalid Q byte> 0x{0:X2}", yQByte)); break;
                }

                return sbldDevice.ToString ();
            }
            else if ((yQByte & 0xF0) == 0xA0 || // 5444 Disk Drive 1
                     (yQByte & 0xF0) == 0xB0)   // 5444 Disk Drive 2
            {
                StringBuilder sbldDevice = new StringBuilder ();

                switch (yQByte & 0x07)
                {
                    case 3:  sbldDevice.Append ("CE use"); break;
                    case 4:  sbldDevice.Append ("Disk read/write address register");  break;
                    case 6:  sbldDevice.Append ("Disk control address register"); break;
                    default: sbldDevice.Append (string.Format ("<Invalid Q byte> 0x{0:X2}", yQByte)); break;
                }

                return sbldDevice.ToString ();
            }
            else if ((yQByte & 0xF0) == 0x10) // 5471/5475 Keyboard
            {
                if (m_eKeyboard == EKeyboard.KEY_5471)
                {
                    if ((yQByte & 0x08) > 0)
                    {
                        return "Load character to be printed in high-order byte";
                    }
                    else
                    {
                        return string.Format ("<Invalid Q byte> 0x{0:X2}", yQByte);
                    }
                }
                else if (m_eKeyboard == EKeyboard.KEY_5475)
                {
                    return (((yQByte & 0x0F) == 0) ? "  Set column indicators" : string.Format ("<Invalid Q byte> 0x{0:X2}", yQByte));
                }
            }
            else if ((yQByte & 0xF0) == 0x03) // Serial Input/Output Channel Adapter
            {
                return "";
            }
            else if ((yQByte & 0xF0) == 0x80 || // BSCA 1
                     (yQByte & 0xF0) == 0xC0)   // BSCA 2
            {
                return "";
            }
            else if ((yQByte & 0xF0) == 0x60 || // Tape Drive
                     (yQByte & 0xF0) == 0x70)
            {
                return "";
            }
            else if ((yQByte & 0xF0) == 0x50) // 1442 Card Read Punch
            {
                return "";
            }
            else if ((yQByte & 0xF8) == 0x40) // 3741 Data/Work Station
            {
                return "";
            }

            return "";
        }

        private string GetExtendedMnemonic (string strMnemonic, byte yQByte)
        {
            if (CompareNoCase (strMnemonic, "MVX "))
            {
                if (!m_bExtendMnemonicMVX)
                {
                    return strMnemonic;
                }

                if (yQByte == 0x00)
                {
                    return "MZZ ";
                }
                else if (yQByte == 0x01)
                {
                    return "MNZ ";
                }
                else if (yQByte == 0x02)
                {
                    return "MZN ";
                }
                else if (yQByte == 0x03)
                {
                    return "MNN ";
                }
            }
            else if (CompareNoCase (strMnemonic, "BC  ") ||
                     CompareNoCase (strMnemonic, "JC  "))
            {
                if (!m_bExtendMnemonicBC &&
                    !CompareNoCase (strMnemonic, "BC  "))
                {
                    return strMnemonic;
                }

                if (!m_bExtendMnemonicJC &&
                    !CompareNoCase (strMnemonic, "JC  "))
                {
                    return strMnemonic;
                }

                // "An unconditional branch occurs when the Q byte
                //  contains 00, X7, or XF (where X= 8 through F)"
                if (yQByte == 0x00)
                {
                    return string.Format ("{0:S1}   ", strMnemonic[0]);
                }
                else if (yQByte == 0x01)
                {
                    return string.Format ("{0:S1}NE ", strMnemonic[0]);
                }
                else if (yQByte == 0x02)
                {
                    return string.Format ("{0:S1}NL ", strMnemonic[0]);
                }
                else if (yQByte == 0x04)
                {
                    return string.Format ("{0:S1}NH ", strMnemonic[0]);
                }
                else if (yQByte == 0x08)
                {
                    return string.Format ("{0:S1}NOZ", strMnemonic[0]);
                }
                else if (yQByte == 0x10)
                {
                    return string.Format ("{0:S1}T  ", strMnemonic[0]);
                }
                else if (yQByte == 0x20)
                {
                    return string.Format ("{0:S1}NOL", strMnemonic[0]);
                }
                else if (yQByte == 0x80)
                {
                    return string.Format ("NOP{0:S1}", strMnemonic[0]);
                }
                else if (yQByte == 0x81)
                {
                    return string.Format ("{0:S1}E  ", strMnemonic[0]);
                }
                else if (yQByte == 0x82)
                {
                    return string.Format ("{0:S1}L  ", strMnemonic[0]);
                }
                else if (yQByte == 0x84)
                {
                    return string.Format ("{0:S1}H  ", strMnemonic[0]);
                }
                else if ((yQByte & 0x87) == 0x87)
                {
                    return string.Format ("{0:S1}   ", strMnemonic[0]);
                }
                else if (yQByte == 0x88)
                {
                    return string.Format ("{0:S1}OZ ", strMnemonic[0]);
                }
                else if (yQByte == 0x90)
                {
                    return string.Format ("{0:S1}F  ", strMnemonic[0]);
                }
                else if (yQByte == 0xA0)
                {
                    return string.Format ("{0:S1}OL ", strMnemonic[0]);
                }
                // "The Q code 80, X7, or XF (where X = 0 through 7)
                //  causes the branch operation to perfonn as a no op"
                else if ((yQByte & 0x80) == 0 &&
                         (yQByte & 0x07) == 7)
                {
                    return string.Format ("NOP{0:S1}", strMnemonic[0]);
                }
            }

            // If no match found, return the original mnemonic
            return strMnemonic;
        }

        private void AddDasmLine (int iInstAddress, string strDasmLine, bool bAlwaysIndent = false)
        {
            if (!m_bInData &&
                iInstAddress == m_iEntryPoint)
            {
                AddTag (m_iEntryPoint, 0xFFFF, ETagType.TAG_Entry_Point);
            }

            m_iLineOffset                 = m_iMaxTagLength > 0 ? m_iMaxTagLength + 2 : 0;
            StringBuilder strbldrDasmLine = new StringBuilder (100);

            // First, add any tag value:
            if (m_iLineOffset > 0)
            {
                strbldrDasmLine.Append (m_strTagText);
                if (m_strTagText.Length < m_iLineOffset)
                {
                    if (m_strTagText.Length > 0)
                    {
                        strbldrDasmLine.Append ("  ");
                    }
                    strbldrDasmLine.Append (new string (' ', m_iLineOffset - strbldrDasmLine.Length));
                }
            }
            
            // Then, add the DASM line:
            strbldrDasmLine.Append (strDasmLine);

            // Next, add any comments found:
            int iIdx = 0;
            int iDisassemblyLineIdx = -1;
            if (m_strlComments.Count > 0)
            {
                for (; iIdx < m_strlComments.Count; iIdx++)
                {
                    if (m_strlComments[iIdx].Length > 0)
                    {
                        if (m_kiLastPreCommentPos + m_iLineOffset > strbldrDasmLine.Length)
                            strbldrDasmLine.Append (new string (' ', m_kiLastPreCommentPos + m_iLineOffset - strbldrDasmLine.Length));
                        if (strbldrDasmLine.Length + m_strlComments[iIdx].Length > (m_kiLastPreCommentPos + m_iLineOffset + m_iMaxLineLength))
                        {
                            m_strlDisassemblyLines.Add (strbldrDasmLine.ToString ());
                            strbldrDasmLine.Clear ();
                            strbldrDasmLine.Append (new string (' ', m_kiLastPreCommentPos + m_iLineOffset));
                            if (m_bSimulatedXR1ValueLoaded ||
                                m_bSimulatedXR1ValueLoaded ||
                                bAlwaysIndent)
                            {
                                strbldrDasmLine.Append (ADDRESS_SPACE_HOLDER);
                            }
                            if (iDisassemblyLineIdx == -1)
                                iDisassemblyLineIdx = m_strlDisassemblyLines.Count;
                        }
                        strbldrDasmLine.Append (m_strlComments[iIdx]);
                    }
                }
            }

            //m_sdDisassemblyLineIndexes.Add (iInstAddress, iDisassemblyLineIdx);
            m_strlDisassemblyLines.Add (strbldrDasmLine.ToString ());

            // Now, add any additional comments for this address:
            for (; iIdx < m_strlComments.Count; iIdx++)
            {
                m_strlDisassemblyLines.Add (new string (' ', m_kiLastPreCommentPos + m_iLineOffset) + m_strlComments[iIdx]);
            }

            // Finally, clear out tag & comments               
            m_strTagText = "";
            m_strlComments.Clear ();
        }

        private void AccumulateData ()
        {
            if (m_bInData && !m_bInEmulator &&
                m_sdAllTags.ContainsKey (m_iObjectCodeIdx))
            {
                m_bInData       = false;
                m_bDumpComplete = false;
                return;
            }

            m_bInData = true;
            //m_bInData = m_iObjectCodeIdx < m_iNewFirstInstruction;

            // If accumulation buffer is full, print line
            if (m_ylDataBuffer.Count >= m_iDumpLineLength)
            {
                DumpAccumulatedData ();
            }

            if (m_iDumpStartAddress == -1)
            {
                m_iDumpStartAddress = m_iObjectCodeIdx;
            }

            // Add the data to the buffer
            m_ylDataBuffer.Add (GetNextByte ());
        }

        private void InsertHeaderLines ()
        {
            if (m_bShowHeader         &&
                !m_bInSingleLineTrace &&
                !m_bHeaderLinesAdded)
            {
                m_strlDisassemblyLines.Insert (0, DASM_HEADER_LINE_TWO);
                m_strlDisassemblyLines.Insert (0, DASM_HEADER_LINE_ONE);

                if (m_strHeaderProgramName.Length > 0)
                {
                    int iPadding = (DASM_HEADER_LINE_TWO.Length - m_strHeaderProgramName.Length) / 2;
                    string strPadding = new string (' ', iPadding);
                    m_strlDisassemblyLines.Insert (0, strPadding + m_strHeaderProgramName);
                }

                m_bHeaderLinesAdded = true;
            }
        }

        private bool IsInData ()
        {
            DumpListDCP ();

            //NOTE_Begin,      // Computer  
            //NOTE_Entry,      // Command   Code  Tag/Comment
            //NOTE_Code,       // Command   Code  Text line
            //NOTE_Data,       // Command   Data  Text line
            //NOTE_Skip,       // Command
            //NOTE_Tag,        // Command   Code  Tag/Comment
            //NOTE_Variable,   // Command   Data  Tag/Comment
            //NOTE_Constant,   // Command   Data  Tag/Comment
            //NOTE_Comment,    // Command         Comment
            //NOTE_Jump,       // Computer  Code  Tag/Comment
            //NOTE_Loop,       // Computer  Code  Tag/Comment
            //NOTE_End         // Computer

            foreach (KeyValuePair<int, CDasmControlPoint> kvp in m_sdDCP)
            {
                switch (kvp.Value.ControlPointType)
                {
                    // NOTE_Begin     Computer  
                    case EControlPointType.POINT_Begin:
                    {
                        break;
                    }

                    // NOTE_Entry     Command   Code  Comment
                    case EControlPointType.POINT_Entry:
                    {
                        m_strTagText = kvp.Value.Notation;
                        m_bInData = false;

                        break;
                    }

                    // NOTE_Code      Command   Code  Text line
                    case EControlPointType.POINT_Code:
                    {
                        m_strlDisassemblyLines.Add ("");
                        m_strlDisassemblyLines.Add ("");
                        m_strlDisassemblyLines.Add (kvp.Value.Notation);
                        m_strlDisassemblyLines.Add ("");
                        m_bInData = false;

                        break;
                    }

                    // NOTE_Data      Command   Data  Text line
                    case EControlPointType.POINT_Data:
                    {
                        m_strlDisassemblyLines.Add ("");
                        m_strlDisassemblyLines.Add ("");
                        m_strlDisassemblyLines.Add (kvp.Value.Notation);
                        m_strlDisassemblyLines.Add ("");

                        // Look for the next DCP to get next address
                        int iDumpEndAddress = m_yaObjectCode.GetUpperBound (0);
                        //for (int iIdx = m_iIdxDCP + 1; iIdx < m_sdDCP.Count; iIdx++)
                        //{
                        //    if (kvp.Value.NotationType == EControlPointType.POINT_Code     ||
                        //        kvp.Value.NotationType == EControlPointType.POINT_Comment  ||
                        //        kvp.Value.NotationType == EControlPointType.POINT_Data     ||
                        //        kvp.Value.NotationType == EControlPointType.POINT_Jump     ||
                        //        kvp.Value.NotationType == EControlPointType.POINT_Loop     ||
                        //        kvp.Value.NotationType == EControlPointType.POINT_Tag      ||
                        //        kvp.Value.NotationType == EControlPointType.POINT_Variable ||
                        //        kvp.Value.NotationType == EControlPointType.POINT_Constant ||
                        //        kvp.Value.NotationType == EControlPointType.POINT_End)
                        //    {
                        //        m_iObjectCodeIdx = kvp.Value.Address;
                        //        iDumpEndAddress = m_iObjectCodeIdx;

                        //        if (kvp.Value.NotationType != ENotationType.NOTE_End)
                        //        {
                        //            iDumpEndAddress--;
                        //        }

                        //        break;
                        //    }
                        //}

                        // Dump from preset address to the next address foung - 1
                        List<string> strlDataDump = BinaryToDump (m_yaObjectCode, kvp.Value.Address, iDumpEndAddress);
                        AppendStringList (strlDataDump, m_strlDisassemblyLines);

                        m_bInData = true;

                        break;
                    }

                    // NOTE_Skip      Command
                    case EControlPointType.POINT_Skip:
                    {
                        // Set the address of the next byte to decode
                        foreach (KeyValuePair<int, CDasmControlPoint> kvp2 in m_sdDCP)
                        {
                            if (kvp2.Value.ControlPointType == EControlPointType.POINT_Code     ||
                                kvp2.Value.ControlPointType == EControlPointType.POINT_Comment  ||
                                kvp2.Value.ControlPointType == EControlPointType.POINT_Data     ||
                                kvp2.Value.ControlPointType == EControlPointType.POINT_Jump     ||
                                kvp2.Value.ControlPointType == EControlPointType.POINT_Loop     ||
                                kvp2.Value.ControlPointType == EControlPointType.POINT_Tag      ||
                                kvp2.Value.ControlPointType == EControlPointType.POINT_Variable ||
                                kvp2.Value.ControlPointType == EControlPointType.POINT_Constant)
                            {
                                m_iObjectCodeIdx = kvp.Value.Address;
                                m_strlDisassemblyLines.Add ("");

                                break;
                            }
                        }

                        break;
                    }

                    // NOTE_Tag       Command   Code  Comment
                    case EControlPointType.POINT_Tag:
                    {
                        m_strTagText = kvp.Value.Notation;
                        m_bInData = false;

                        break;
                    }

                    // NOTE_Variable  Command   Data  Comment
                    // NOTE_Constant  Command   Data  Comment
                    case EControlPointType.POINT_Variable:
                    case EControlPointType.POINT_Constant:
                    {
                        m_strTagText = kvp.Value.Notation;
                        m_bInData = true;

                        break;
                    }

                    // NOTE_Comment   Command         Comment
                    case EControlPointType.POINT_Comment:
                    {
                        m_strlComments.Add (kvp.Value.Notation);
                        break;
                    }

                    // NOTE_Jump      Computer  Code  Comment
                    case EControlPointType.POINT_Jump:
                    {
                        m_strTagText = kvp.Value.Notation;
                        m_bInData = false;

                        break;
                    }

                    // NOTE_Loop      Computer  Code  Comment
                    case EControlPointType.POINT_Loop:
                    {
                        // TODO: revise line in map that match address
                        m_strTagText = kvp.Value.Notation;
                        m_bInData = false;

                        break;
                    }

                    // NOTE_End       Computer
                    case EControlPointType.POINT_End:
                    {
                        //m_bDumpComplete = true;

                        break;
                    }
                }

                m_iIdxDCP++;
            }

            return m_bInData;
        }

        int InsertDasmControlPoint (int iAddress, EControlPointType eControlPointType, string strNotation)
        {
            bool bEntryInserted = false;

            //POINT_Begin,      // Computer  
            //POINT_Entry,      // Command   Code  Tag/Comment
            //POINT_Code,       // Command   Code  Text line
            //POINT_Data,       // Command   Data  Text line
            //POINT_Skip,       // Command
            //POINT_Tag,        // Command   Code  Tag/Comment
            //POINT_Variable,   // Command   Data  Tag/Comment
            //POINT_Constant,   // Command   Data  Tag/Comment
            //POINT_Comment,    // Command         Comment
            //POINT_Jump,       // Computer  Code  Tag/Comment
            //POINT_Loop,       // Computer  Code  Tag/Comment
            //POINT_End         // Computer
            if (eControlPointType == EControlPointType.POINT_Tag      ||
                eControlPointType == EControlPointType.POINT_Variable ||
                eControlPointType == EControlPointType.POINT_Constant ||
                eControlPointType == EControlPointType.POINT_Entry    ||
                eControlPointType == EControlPointType.POINT_Jump     ||
                eControlPointType == EControlPointType.POINT_Loop)
            {
                //if (m_sdTagNames.ContainsKey (iAddress))
                //{
                //    if (eNotationType == EControlPointType.POINT_Tag)
                //    {
                //        // Tags override all other values
                //        if (strNotation.Equals (m_sdTagNames[iAddress], StringComparison.OrdinalIgnoreCase))
                //        {
                //            strNotation = "Tag" + strNotation.Substring (4) + ' ';
                //            m_sdTagNames[iAddress] = strNotation;
                //        }
                //        else
                //        {
                //            // Trouble!
                //        }
                //    }
                //}
                //else
                //{
                //    m_sdTagNames.Add (iAddress, strNotation);
                //}
            }

            foreach (KeyValuePair<int, CDasmControlPoint> kvp in m_sdDCP)
            {
                if (kvp.Value.Address == iAddress)
                {
                    if (eControlPointType == EControlPointType.POINT_Variable ||
                        eControlPointType == EControlPointType.POINT_Constant ||
                        eControlPointType == EControlPointType.POINT_Jump     ||
                        eControlPointType == EControlPointType.POINT_Loop)
                    {
                        if (kvp.Value.ControlPointType == EControlPointType.POINT_Entry    ||
                            kvp.Value.ControlPointType == EControlPointType.POINT_Tag      ||
                            kvp.Value.ControlPointType == EControlPointType.POINT_Variable ||
                            kvp.Value.ControlPointType == EControlPointType.POINT_Constant ||
                            kvp.Value.ControlPointType == EControlPointType.POINT_Jump     ||
                            kvp.Value.ControlPointType == EControlPointType.POINT_Loop)
                        {
                            // Each address can have only one tag, so mark all others as comments
                            eControlPointType = EControlPointType.POINT_Comment;
                        }
                    }
                    //else if (eControlPointType == EControlPointType.POINT_Tag)
                    //{
                    //    if (kvp.Value.ControlPointType == EControlPointType.POINT_Entry ||
                    //        kvp.Value.ControlPointType == EControlPointType.POINT_Variable ||
                    //        kvp.Value.ControlPointType == EControlPointType.POINT_Constant ||
                    //        kvp.Value.ControlPointType == EControlPointType.POINT_Jump ||
                    //        kvp.Value.ControlPointType == EControlPointType.POINT_Loop)
                    //    {
                    //        // Tags pre-empt all other types for the same address
                    //        kvp.Value.ControlPointType = EControlPointType.POINT_Comment;
                    //    }
                    //}
                }
                else if (kvp.Value.Address > iAddress)
                {
                    if (eControlPointType == EControlPointType.POINT_Entry)
                    {
                        if (strNotation.Length == 0)
                        {
                            strNotation = "Entry ->";
                        }
                    }

                    if (eControlPointType == EControlPointType.POINT_Tag      ||
                        eControlPointType == EControlPointType.POINT_Variable ||
                        eControlPointType == EControlPointType.POINT_Constant ||
                        eControlPointType == EControlPointType.POINT_Jump)
                    {
                        if (strNotation.Length > m_iMaxTagLength)
                        {
                            // Get the length of the longest tag being added
                            m_iMaxTagLength = strNotation.Length;
                        }
                    }
                    //else if (eControlPointType == EControlPointType.POINT_Loop)
                    //{
                    //    if (strNotation.Length > m_iMaxTagLength)
                    //    {
                    //        // Loops are added after the disassembly has begun,
                    //        // so their text must not exceed m_iMaxTagLength
                    //        strNotation = strNotation.Substring (0, m_iMaxTagLength);
                    //    }
                    //}

                    //if (eControlPointType == ENotationType.NOTE_Loop &&
                    //    strNotation.Length > 0)
                    //{
                    //    int iStrIdx = m_sdDisassemblyLineIndexes[iAddress];
                    //    if (iStrIdx >= 0 &&
                    //        iStrIdx < m_strlDisassemblyLines.Count)
                    //    {
                    //        StringBuilder strbldrNewLine = new StringBuilder (128);
                    //        strbldrNewLine.Append (strNotation);
                    //        strbldrNewLine.Append (new string (' ', m_iMaxTagLength - strNotation.Length));
                    //        strbldrNewLine.Append (m_strlDisassemblyLines[iStrIdx].Substring (m_iMaxTagLength));
                    //        m_strlDisassemblyLines[iStrIdx] = strbldrNewLine.ToString ();
                    //    }
                    //}
                    else
                    {
                        m_sdDCP[iAddress] = new CDasmControlPoint (iAddress, 0, 0, true, eControlPointType, strNotation);
                        bEntryInserted = true;
                    }

                    break; // No need to continue searching after inserting
                }
            }

            if (!bEntryInserted)
            {
                m_sdDCP[iAddress] = new CDasmControlPoint (iAddress, 0, 0, true, eControlPointType, strNotation);
            }

            DumpListDCP ();

            return m_sdDCP.Count;
        }

        private void DumpListDCP ()
        {
#if LIST_DCP
            if (m_sdDCP.Count > s_iLastDCPCount)
            {
                Console.WriteLine ("--------------- DumpListDCP --------------");
                foreach (KeyValuePair<int, CDasmControlPoint> kvp in m_sdDCP)
                {
                    Console.WriteLine (string.Format ("{0:X4}  {1:S}  {2:S}", kvp.Value.Address, kvp.Value.NotationType.ToString (), kvp.Value.Notation));
                }
                Console.WriteLine ("------------------------------------------");

                s_iLastDCPCount = m_sdDCP.Count;
            }
#endif
        }

        private void DumpAccumulatedData ()
        {
            if (m_ylDataBuffer.Count == 0)
            {
                return;
            }

            //          1         2         3      v  4      v  5         6         7         8         9         0
            // 1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
            // addr: data xx xx xx xx  xx xx xx xx  <.... ....>  <.... ....>
            // 0039: data F5 FD FC F0  C6 F0 C2     <.... ... >  <5..0 F0B >
            // 0039: data F5 FD FC F0  C6 F0 C2     <.... ... >  <5..0 F0B >
            // <label>  addr:  data xx xx xx xx  xx xx xx xx  <.... ....>  <.... ....>
            //          1         2         3         4         5         6  v      7         8  v      9         0  v
            // 12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567
            // addr: data xx xx xx xx  xx xx xx xx  xx xx xx xx  xx xx xx xx  <.... .... .... ....>  <.... .... .... ....>
            // 0039: data F5 FD FC F0  C6 F0 C2     <.... ... >  <5..0 F0B >
            // 0039: data F5 FD FC F0  C6 F0 C2     <.... ... >  <5..0 F0B >
            // <label>  addr:  data xx xx xx xx  xx xx xx xx  <.... ....>  <.... ....>
            //          1         2         3         4         5         6  v      7         8  v      9         0  v
            // 12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567
            // 0000: F0 00 00 0E  00 00 01 00  06 0E 00 00  02 00 06 3D  <.... .... .... ...=> <0... .... .... ....>

            StringBuilder strbldrDumpLine = new StringBuilder (60);

            //if (m_iLineOffset > 0)
            //{
            //    strbldrDumpLine.Append (new string (' ', m_iLineOffset));
            //}
            strbldrDumpLine.Append (string.Format ("{0:X4}: data  ", m_iDumpStartAddress));

            //int iHexDumpEnd    = m_iLineOffset + 37,  // 58,  // 63, //37,
            //    iAsciiDumpEnd  = m_iLineOffset + 47,  // 78,  // 84, //47,
            //    iEbcdicDumpEnd = m_iLineOffset + 60,  //100,  //108, //60,
            //          1         2         3         4         5         6         7         8         9         0         1         2
            //0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
            //001E: data  01 00 6F 03  76 57 1B 5D  <..o. vW.]>  <..?. ...)> <..01 2345> <.... ....>
            int iHexDumpEnd    =  38, // 37,  // 58,  // 63, //37,
                iAsciiDumpEnd  =  48, // 47,  // 78,  // 84, //47,
                iEbcdicDumpEnd =  61, // 60,  //100,  //108, //60,
                iHPLDumpEnd    =  74,
                i5475DumpEnd   =  87,
                iOffset        =   0;

            foreach (byte yData in m_ylDataBuffer)
            {
                strbldrDumpLine.Append (ByteToHexPair (yData));
                strbldrDumpLine.Append (" ");

                ++iOffset;
                if (iOffset > 0                 &&
                    iOffset < m_iDumpLineLength &&
                    iOffset % 4 == 0)
                {
                    strbldrDumpLine.Append (" ");
                }
            }

            // Pad string
            if (strbldrDumpLine.Length < iHexDumpEnd)
            {
                strbldrDumpLine.Append (new string (' ', iHexDumpEnd - strbldrDumpLine.Length));
            }

            // Print ASCII characters
            strbldrDumpLine.Append ("<");
            iOffset = 0;
            foreach (byte yData in m_ylDataBuffer)
            {
                char cData = (char)yData;
                if (cData >= (char)0x20 &&
                    cData <  (char)0x7F)
                {
                    strbldrDumpLine.Append (cData);
                }
                else
                {
                    strbldrDumpLine.Append (".");
                }

                ++iOffset;
                if (iOffset > 0                 &&
                    iOffset < m_iDumpLineLength &&
                    iOffset % 4 == 0)
                {
                    strbldrDumpLine.Append (" ");
                }
            }
            // Pad string
            if (strbldrDumpLine.Length < iAsciiDumpEnd)
            {
                strbldrDumpLine.Append (new string (' ', iAsciiDumpEnd - strbldrDumpLine.Length));
            }
            strbldrDumpLine.Append (">  <");

            // Print EBCDIC characters
            iOffset = 0;
            foreach (byte yData in m_ylDataBuffer)
            {
                char cData = (char)ConvertEBCDICtoASCII (yData);
                if (cData >= (char)0x20 &&
                    cData < (char)0x7F)
                {
                    strbldrDumpLine.Append (cData);
                }
                else
                {
                    strbldrDumpLine.Append (".");
                }

                ++iOffset;
                if (iOffset > 0 &&
                    iOffset < m_iDumpLineLength &&
                    iOffset % 4 == 0)
                {
                    strbldrDumpLine.Append (" ");
                }
            }
            // Pad string
            if (strbldrDumpLine.Length < iEbcdicDumpEnd)
            {
                strbldrDumpLine.Append (new string (' ', iEbcdicDumpEnd - strbldrDumpLine.Length));
            }
            strbldrDumpLine.Append (">  <");

            // Print HPL characters
            iOffset = 0;
            foreach (byte yData in m_ylDataBuffer)
            {
                char cData = (char)GetHaltDisplayCode (yData);
                if (cData != '?')
                {
                    strbldrDumpLine.Append (cData);
                }
                else
                {
                    strbldrDumpLine.Append (".");
                }

                ++iOffset;
                if (iOffset > 0 &&
                    iOffset < m_iDumpLineLength &&
                    iOffset % 4 == 0)
                {
                    strbldrDumpLine.Append (" ");
                }
            }
            // Pad string
            if (strbldrDumpLine.Length < iHPLDumpEnd)
            {
                strbldrDumpLine.Append (new string (' ', iHPLDumpEnd - strbldrDumpLine.Length));
            }
            strbldrDumpLine.Append (">  <");

            // Print 5475 characters
            iOffset = 0;
            foreach (byte yData in m_ylDataBuffer)
            {
                char cData = (char)Get5475DisplayCode (yData);
                if (cData != '?')
                {
                    strbldrDumpLine.Append (cData);
                }
                else
                {
                    strbldrDumpLine.Append (".");
                }

                ++iOffset;
                if (iOffset > 0 &&
                    iOffset < m_iDumpLineLength &&
                    iOffset % 4 == 0)
                {
                    strbldrDumpLine.Append (" ");
                }
            }
            // Pad string
            if (strbldrDumpLine.Length < i5475DumpEnd)
            {
                strbldrDumpLine.Append (new string (' ', i5475DumpEnd - strbldrDumpLine.Length));
            }
            strbldrDumpLine.Append (">");

            AddDasmLine (m_iDumpStartAddress, strbldrDumpLine.ToString ());

            // Empty the buffer
            m_ylDataBuffer.Clear ();
            m_iDumpStartAddress = -1;
        }

        protected char GetHaltDisplayCode (byte yHaltCode)
        {
            for (int iIdx = 0; iIdx < m_yaHaltCodes.Length; iIdx++)
            {
                if (m_yaHaltCodes[iIdx] == (yHaltCode & 0x7F))
                {
                    return m_strHaltDisplay[iIdx];
                }
            }

            return '?';
        }

        protected char Get5475DisplayCode (byte y5475Code)
        {
            for (int iIdx = 0; iIdx < m_ya5475Codes.Length; iIdx++)
            {
                if (m_ya5475Codes[iIdx] == (y5475Code & 0xFE))
                {
                    return m_str5475Display[iIdx];
                }
            }

            return '?';
        }
    }
}
