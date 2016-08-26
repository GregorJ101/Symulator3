using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EmulatorEngine
{
    // Disassembler Engine
    public partial class CDataConversion : CStringProcessor
    {
        // Type definitions
        //protected enum EMachineInstruction
        //{
        //    INST_ZAZ,
        //    INST_AZ,
        //    INST_SZ,
        //    INST_MVX,
        //    INST_ED,
        //    INST_ITC,
        //    INST_MVC,
        //    INST_CLC,
        //    INST_ALC,
        //    INST_SLC,
        //    INST_SNS,
        //    INST_LIO,
        //    INST_ST,
        //    INST_L,
        //    INST_A,
        //    INST_TBN,
        //    INST_TBF,
        //    INST_SBN,
        //    INST_SBF,
        //    INST_MVI,
        //    INST_CLI,
        //    INST_BC,
        //    INST_TIO,
        //    INST_LA,
        //    INST_HPL,
        //    INST_APL,
        //    INST_JC,
        //    INST_SIO
        //};

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

        // Todo: Optionally use extended mnemonics from BAL manual
        //       Add comments: HPL characters that display mapped from hex values
        //                     SIO, SNS, LIO, TIO, L, ST, etc.: comments that explain Q byte definition
        // Treat areas following unconditional jumps & branches as potential data:
        //   disassemle to temp string array until data found and discard if no unconditional branches/jumps found
        //     (or jumps/branches found that cover all possible conditions) discard disassembly and dump as data
        // Note addresses of jump & unindexed branches as code locations, generate labels for them

        protected enum ETagType
        {
            TAG_Undefined,
            TAG_Jump,
            TAG_Loop,
            TAG_Tag
        };

        protected class CDasmControlPoint
        {
            int m_iAddress = -1;
            bool m_bIsCode = false;
            ENotationType m_eNotationType = ENotationType.NOTE_Undefined;
            string m_strNotation;

            public CDasmControlPoint (int iAddress, ENotationType eNotationType, string strNotation)
            {
                m_iAddress = iAddress;
                m_eNotationType = eNotationType;
                m_strNotation = strNotation;
            }

            public CDasmControlPoint (int iAddress, bool bIsCode, ENotationType eNotationType, string strNotation)
            {
                m_iAddress = iAddress;
                m_bIsCode = bIsCode;
                m_eNotationType = eNotationType;
                m_strNotation = strNotation;
            }

            public int Address
            {
                get { return m_iAddress; }
                set { m_iAddress = value; }
            }

            public bool IsCode
            {
                get { return m_bIsCode; }
                set { m_bIsCode = value; }
            }

            public ENotationType NotationType
            {
                get { return m_eNotationType; }
                set { m_eNotationType = value; }
            }

            public string Notation
            {
                get { return m_strNotation; }
                set { m_strNotation = value; }
            }
        };

        protected class CTagEntry
        {
            int  m_iLineNumber  = 0;
            int  m_iTagAddress  = 0;
            int  m_iTagSequence = 0;
            bool m_bIsJump = false;
            bool m_bIsLoop = false;

            public CTagEntry (int iLineNumber, int iTagAddress)
            {
                m_iLineNumber = iLineNumber;
                m_iTagAddress = iTagAddress;
            }

            public int LineNumber
            {
                get { return m_iLineNumber; }
                set { m_iLineNumber = value; }
            }

            public int TagAddress
            {
                get { return m_iTagAddress; }
                set { m_iTagAddress = value; }
            }

            public int TagSequence
            {
                get { return m_iTagSequence; }
                set { m_iTagSequence = value; }
            }

            public bool IsJump
            {
                get { return m_bIsJump; }
                set { m_bIsJump = value; }
            }

            public bool IsLoop
            {
                get { return m_bIsLoop; }
                set { m_bIsLoop = value; }
            }
        };

        // Disassembler Engine member data
        protected List<byte> m_ylDataBuffer;
        protected byte[] m_yaObjectCode;

        protected string m_strTagText = "";
        protected List<string> m_strlComments = new List<string> (10);

        //         1         2         3
        //123456789012345678901234567890
        //0360: MVC  0C 04 0280 0281   <comment start>
        //0366: ALC  5E 00 5F,1 08,1   <comment start>
        const int m_kiLastDasmLinePos   = 26;
        const int m_kiLastPreCommentPos = m_kiLastDasmLinePos + 3;

        static int s_iLastDCPCount = 0;
        protected int  m_iIdxDCP       = 0;
        protected int  m_iMaxTagLength = 12; // Jump_##_xxxx
        protected int  m_iObjectCodeIdx;
        protected int  m_iDumpStartAddress;
        //protected int  m_iColumnOffset;
        protected int  m_iHighAddress;
        protected int  m_iDumpLineLength    = 8;
        protected int  m_iJumpID            = 0;
        protected int  m_iLoopID            = 0;
        protected int  m_iLineOffset        = 0;
        protected int  m_iSimulatedXR1Value = 0;
        protected int  m_iSimulatedXR2Value = 0;
        protected bool m_bInData;
        protected bool m_bDumpComplete      = false;

        protected bool m_bAutoTagJump       = false;
        public bool AutoTagJump
        {
            get { return m_bAutoTagJump; }
            set { m_bAutoTagJump = value; }
        }

        protected bool m_bAutoTagLoop       = false;
        public bool AutoTagLoop
        {
            get { return m_bAutoTagLoop; }
            set { m_bAutoTagLoop = value; }
        }

        protected bool m_bExtendMnemonicBC  = false;
        public bool ExtendMnemonicBC
        {
            get { return m_bExtendMnemonicBC; }
            set { m_bExtendMnemonicBC = value; }
        }

        protected bool m_bExtendMnemonicJC  = false;
        public bool ExtendMnemonicJC
        {
            get { return m_bExtendMnemonicJC; }
            set { m_bExtendMnemonicJC = value; }
        }

        protected bool m_bExtendMnemonicMVX = false;
        public bool ExtendMnemonicMVX
        {
            get { return m_bExtendMnemonicMVX; }
            set { m_bExtendMnemonicMVX = value; }
        }

        protected SortedDictionary<int, CDasmControlPoint> m_sdDCP = new SortedDictionary<int, CDasmControlPoint> ();
        protected SortedDictionary<int, CDasmControlPoint> m_sdBackupDCP = new SortedDictionary<int, CDasmControlPoint> ();
        protected SortedDictionary<int, int> m_sdDisassemblyLineIndexes = new SortedDictionary<int, int> (); // [inst address], list index
        protected List<string> m_strlDisassemblyLines = new List<string> ();
        protected SortedDictionary<int, CTagEntry> m_sdAllTags = new SortedDictionary<int, CTagEntry> ();
        protected SortedDictionary<int, int> m_sdTagCalls = new SortedDictionary<int, int> (); // [target address], address of BC/JC/TIO

        protected byte[] m_yaHaltCodes = { 0x00, 0x10, 0x40, 0x03, 0x76, 0x57, 0x1B, 0x5D, 0x7D,
                                           0x07, 0x7F, 0x5F, 0x6F, 0x3F, 0x79, 0x6C, 0x73, 0x7C,
                                           0x3C, 0x3B, 0x63, 0x68, 0x3E, 0x6B, 0x02, 0x08, 0x0A };
        //protected char[] m_caHaltDisplay =  { ' ', '-', '_', '1', '2', '3', '4', '5', '6',
        //                                      '7', '8', '9', '0', 'A', 'b', 'C', 'd', 'E'
        //                                     ,'F', 'H', 'J', 'L', 'P', 'U', '\'', '\'', '"' };
        protected string m_strHaltDisplay = " -_1234567890AbCdEFHJLPU''\"";

        protected byte[] m_ya5475Codes = { 0x00, 0x10, 0x02, 0x24, 0xBA, 0xB6, 0x74, 0xD6, 0xDE,
                                           0xA4, 0xFE, 0xF6, 0xEE, 0xFC, 0x5E, 0xCA, 0x3E, 0xDA,
                                           0xD8, 0x7C, 0x2E, 0x4A, 0xF8, 0x6E, 0x20, 0x40, 0x60 };
        //protected char[] m_ca5475Display =  { ' ', '-', '_', '1', '2', '3', '4', '5', '6',
        //                                      '7', '8', '9', '0', 'A', 'b', 'C', 'd', 'E'
        //                                     ,'F', 'H', 'J', 'L', 'P', 'U', '\'', '\'', '"' };
        protected string m_str5475Display = " -_1234567890AbCdEFHJLPU''\"";

        // StringCollection of all detected errors
        //StringCollection m_scErrors = new StringCollection ();

        private void PrepForDASM ()
        {
            DumpListDCP ();

            m_sdDisassemblyLineIndexes.Clear ();
            m_strlDisassemblyLines.Clear ();
            m_sdAllTags.Clear ();
            m_sdTagCalls.Clear ();
            m_bDumpComplete = false;
            m_iLowAddressMI = 0;
            m_iHighAddressMI = 0;
            m_iIdxDCP = 0;
            m_iJumpID = 0;
            m_iLoopID = 0;

            m_sdDCP.Clear ();
        }

        public List<string> DisassembleCode (List<byte> ylBinary)
        {
            return DisassembleCode (ylBinary.ToArray (), 0, ylBinary.Count);
        }

        public List<string> DisassembleCode (byte[] yaBinary)
        {
            DumpListDCP ();

            int iStartAddress = m_iLowAddressMI,
                iEndAddress   = m_iHighAddressMI;

            foreach (KeyValuePair<int, CDasmControlPoint> kvp in m_sdDCP)
            {
                if (kvp.Value.NotationType == ENotationType.NOTE_Begin)
                {
                    iStartAddress = kvp.Value.Address;
                }
                if (kvp.Value.NotationType == ENotationType.NOTE_Begin)
                {
                    iEndAddress = kvp.Value.Address;
                }
            }

            if (iEndAddress == 0)
            {
                iStartAddress = 0;
                iEndAddress   = yaBinary.Length;
            }

            return AddTagLabels (DisassembleCode (yaBinary, iStartAddress, iEndAddress));
        }

        protected List<string> AddTagLabels (List<string> strlDasmOutput)
        {
            if (m_sdAllTags.Count > 0)
            {
                // First, apply sequence numbers to tag labels
                int iSeqID = 0;
                foreach (KeyValuePair<int, CTagEntry> kvp in m_sdAllTags)
                {
                    kvp.Value.TagSequence = ++iSeqID;
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
                    int iLineAddress = Convert.ToInt32 (strLineAddress, 16);

                    if (m_sdAllTags.ContainsKey (iLineAddress))
                    {
                        CTagEntry tag = m_sdAllTags[iLineAddress];
                        string strLabel = tag.IsJump && tag.IsLoop ? "Tag_" :
                                          tag.IsJump               ? "Jump_" : "Loop_";
                        strLabel = string.Format ("{0:s}{1:d2}_{2:X4}{3:s}", strLabel, tag.TagSequence, tag.TagAddress,
                                                  tag.IsJump && tag.IsLoop ? " " : "");
                        // Add tag label to DASM line
                        strDasmLine = strLabel + strDasmLine.Substring (strLabel.Length);
                        m_strlDisassemblyLines[iIdx] = strDasmLine;
                    }

                    if (m_sdTagCalls.ContainsKey (iLineAddress))
                    {
                        int iTargetAddress = m_sdTagCalls[iLineAddress];
                        CTagEntry tag = m_sdAllTags[iTargetAddress];
                        string strLabel = tag.IsJump && tag.IsLoop ? "Tag_" :
                                          tag.IsJump               ? "Jump_" : "Loop_";
                        strLabel = string.Format ("{0:s}{1:d2}_{2:X4}{3:s}", strLabel, tag.TagSequence, tag.TagAddress,
                                                  tag.IsJump && tag.IsLoop ? " " : "");
                        // Add comment to line of the branching instruction
                        bool bJump = iLineAddress < iTargetAddress;
                        string strComment = string.Format ("{0:s} to {1:s}", bJump ? "Jump" : "Loop", strLabel);
                        string strPadding = new string (' ', m_kiLastPreCommentPos + m_iLineOffset - strDasmLine.Length);
                        m_strlDisassemblyLines[iIdx] = strDasmLine + strPadding + strComment;
                    }
                }
            }
            else
            {
                // No tags, so remove leading spaces from each dasm line
                for (int iIdx = 0; iIdx < strlDasmOutput.Count; iIdx++)
                {
                    strlDasmOutput[iIdx] = strlDasmOutput[iIdx].Substring (m_iLineOffset);
                }
            }

            //bool bTagsFound = false;

            //if (m_sdDCP.Count > 0)
            //{
            //    // Determine whether any jump/loop/tag entries exist
            //    foreach (KeyValuePair<int, CDasmControlPoint> kvp in m_sdDCP)
            //    {
            //        if (kvp.Value.NotationType == ENotationType.NOTE_Jump ||
            //            kvp.Value.NotationType == ENotationType.NOTE_Loop ||
            //            kvp.Value.NotationType == ENotationType.NOTE_Tag)
            //        {
            //            bTagsFound = true;
            //            break;
            //        }
            //    }

            //    //if (bTagsFound)
            //    //{
            //    //    // Add tag labels to disassembly listing
            //    //    //foreach (KeyValuePair<int, CDasmControlPoint> kvp in m_sdDCP)
            //    //    //{
            //    //    //    if (kvp.Value.NotationType == ENotationType.NOTE_Jump ||
            //    //    //        kvp.Value.NotationType == ENotationType.NOTE_Loop ||
            //    //    //        kvp.Value.NotationType == ENotationType.NOTE_Tag)
            //    //    //    {
            //    //    //        int iLineIdx = m_sdDisassemblyLineIndexes[kvp.Value.Address];
            //    //    //        string strDasmLine = m_strlDisassemblyLines[iLineIdx];
            //    //    //        strDasmLine = kvp.Value.Notation + strDasmLine.Substring (kvp.Value.Notation.Length);
            //    //    //        m_strlDisassemblyLines[iLineIdx] = strDasmLine;
            //    //    //    }
            //    //    //}

            //    //    //for (int iIdx = 0; iIdx < strlDasmOutput.Count; iIdx++)
            //    //    //{
            //    //    //    string str = strlDasmOutput[iIdx];
            //    //    //    int iColonPos = str.IndexOf (':');
            //    //    //    if (iColonPos >= 4)
            //    //    //    {
            //    //    //        string strLineAddress = str.Substring (iColonPos - 4, 4);
            //    //    //        int iLineAddress = Convert.ToInt32 (strLineAddress, 16);
            //    //    //    }
            //    //    //    strlDasmOutput[iIdx] = strlDasmOutput[iIdx].Substring (m_iLineOffset);
            //    //    //}
            //    //}
            //    //else
            //    //{
            //    //    // Remove leading spaces from each dasm line
            //    //    for (int iIdx = 0; iIdx < strlDasmOutput.Count; iIdx++)
            //    //    {
            //    //        strlDasmOutput[iIdx] = strlDasmOutput[iIdx].Substring (m_iLineOffset);
            //    //    }
            //    //}
            //}

            return strlDasmOutput;
        }

        public List<string> DisassembleCode (List<byte> ylBinary, int iLowAddress, int iHighAddress)
        {
            return DisassembleCode (ylBinary.ToArray (), iLowAddress, iHighAddress);
        }

        public List<string> DisassembleCode (byte[] yaBinary, int iLowAddress, int iHighAddress)
        {
            m_iObjectCodeIdx     = iLowAddress;
            m_iDumpStartAddress  = -1;
            m_bInData            = false;
            s_iLastDCPCount      = 0;
            m_iDumpLineLength    = 8;
            m_iJumpID            = 0;
            m_iLoopID            = 0;
            m_iLineOffset        = 0;
            m_iSimulatedXR1Value = 0;
            m_iSimulatedXR2Value = 0;
            //m_iColumnOffset     = 0;

            m_sdDisassemblyLineIndexes.Clear ();
            m_strlDisassemblyLines.Clear ();
            m_sdAllTags.Clear ();
            m_sdTagCalls.Clear ();
            m_ylDataBuffer = new List<byte> ();
            m_yaObjectCode = yaBinary;

            if (iHighAddress >= yaBinary.Length)
            {
                iLowAddress  = 0;
                iHighAddress = yaBinary.Length - 1;
            }
            m_iHighAddress = iHighAddress;

            while (m_iObjectCodeIdx <= iHighAddress &&
                   !m_bDumpComplete)
            {
                DisassembleInstruction ();
            }

            if (!m_bDumpComplete)
            {
                DumpAccumulatedData ();
            }

            return m_strlDisassemblyLines;
        }

        private void DisassembleInstruction ()
        {
            if (IsInData ())
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

            if (iIR == 0)
            {
                // addr:  nem  op qb xxxx
                iOperandAddress = GetNextByte ();
                iOperandAddress <<= 8;
                iOperandAddress += GetNextByte ();
                strInstLine = string.Format ("{0:X4}: {1:S4}  {2:X2} {3:X2} {4:X4}", iInstAddress,
                                             GetExtendedMnemonic (strExtendedMnemonic, yOpCode), yOpCode, yQByte,
                                             iOperandAddress);

                if ((yOpCode & 0xCF) == 0xC2)
                {
                    if ((yQByte & 0x01) == 0x01)
                    {
                        m_iSimulatedXR1Value = iOperandAddress;
                    }

                    if ((yQByte & 0x02) == 0x02)
                    {
                        m_iSimulatedXR2Value = iOperandAddress;
                    }
                }
            }
            else if (iIR > 0)
            {
                // addr:  nem  op qb xx,i
                iOperandAddress = GetNextByte ();
                strInstLine = string.Format ("{0:X4}: {1:S4}  {2:X2} {3:X2} {4:X2},{5:X1}", iInstAddress,
                                             GetExtendedMnemonic (strExtendedMnemonic, yOpCode), yOpCode, yQByte,
                                             iOperandAddress, iIR);
                if ((yOpCode & 0xCF) == 0xC2 &&
                    (yOpCode & 0x30) == 0x10)
                {
                    m_iSimulatedXR1Value = iOperandAddress + (iIR == 1 ? m_iSimulatedXR1Value : m_iSimulatedXR2Value);
                }

                if ((yOpCode & 0xCF) == 0xC2 &&
                    (yOpCode & 0x30) == 0x20)
                {
                    m_iSimulatedXR2Value = iOperandAddress + (iIR == 1 ? m_iSimulatedXR1Value : m_iSimulatedXR2Value);
                }
            }

            AnnotateOneOperandInst (iInstAddress, yOpCode, yQByte, iOperandAddress, iIR);

            AddDasmLine (iInstAddress, strInstLine);
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

            AddDasmLine (iInstAddress, strInstLine);
        }

        private void AnnotateTwoOperandInst (string strMnemonic, byte yOpCode, byte yQByte, int iOperand1Address, int iOperand2Address, int iIR1, int iIR2)
        {
            if ((yOpCode & 0x0F) == 0x04) // ZAZ
            {
                m_strlComments.Add (AnnotateZoned (yQByte, iOperand1Address, iOperand2Address, iIR1, iIR2));
            }
            else if ((yOpCode & 0x0F) == 0x06) // AZ
            {
                m_strlComments.Add (AnnotateZoned (yQByte, iOperand1Address, iOperand2Address, iIR1, iIR2));
            }
            else if ((yOpCode & 0x0F) == 0x07) // SZ
            {
                m_strlComments.Add (AnnotateZoned (yQByte, iOperand1Address, iOperand2Address, iIR1, iIR2));
            }
            //else if ((yOpCode & 0x0F) == 0x08) // MVX
            //{
            //}
            else if ((yOpCode & 0x0F) == 0x0A) // ED
            {
                m_strlComments.Add (AnnotateLogical (yQByte, iOperand1Address, iOperand2Address, iIR1, iIR2));
            }
            else if ((yOpCode & 0x0F) == 0x0B) // ITC
            {
                m_strlComments.Add (AnnotateLogical (yQByte, iOperand1Address, iOperand2Address, iIR1, iIR2));
            }
            else if ((yOpCode & 0x0F) == 0x0C) // MVC
            {
                m_strlComments.Add (AnnotateLogical (yQByte, iOperand1Address, iOperand2Address, iIR1, iIR2));
            }
            else if ((yOpCode & 0x0F) == 0x0D) // CLC
            {
                m_strlComments.Add (AnnotateLogical (yQByte, iOperand1Address, iOperand2Address, iIR1, iIR2));
            }
            else if ((yOpCode & 0x0F) == 0x0E) // ALC
            {
                m_strlComments.Add (AnnotateLogical (yQByte, iOperand1Address, iOperand2Address, iIR1, iIR2));
            }
            else if ((yOpCode & 0x0F) == 0x0F) // SLC
            {
                m_strlComments.Add (AnnotateLogical (yQByte, iOperand1Address, iOperand2Address, iIR1, iIR2));
            }
        }

        private void AnnotateOneOperandInst (int iInstAddress, byte yOpCode, byte yQByte, int iOperandAddress, int iIR)
        {
            string strComment = "";

            if (iIR == 1)
            {
                iOperandAddress += m_iSimulatedXR1Value;
            }
            else if (iIR == 2)
            {
                iOperandAddress += m_iSimulatedXR2Value;
            }

            if ((yOpCode & 0x3F) == 0x30) // SNS   
            {
                StringBuilder sbldAnnotation = new StringBuilder ();
                sbldAnnotation.Append (GetIODeviceName (yQByte));
                if (sbldAnnotation.Length > 0)
                    sbldAnnotation.Append ("  ");
                if (sbldAnnotation.ToString ().IndexOf ("invalid") < 0)
                    sbldAnnotation.Append (GetSNSDetails (yOpCode, yQByte));
                m_strlComments.Add (sbldAnnotation.ToString ());
            }
            else if ((yOpCode & 0x3F) == 0x31) // LIO
            {
                m_strlComments.Add (GetIODeviceName (yQByte) + "  " + GetLIODetails (yOpCode, yQByte));
            }
            else if ((yOpCode & 0x3F) == 0x34) // ST
            {
                m_strlComments.Add (GetRegisterNames (yQByte));
            }
            else if ((yOpCode & 0x3F) == 0x35) // L
            {
                m_strlComments.Add (GetRegisterNames (yQByte));
            }
            else if ((yOpCode & 0x3F) == 0x36) // A
            {
                m_strlComments.Add (GetRegisterNames (yQByte));
            }
            //else if ((yOpCode & 0x3F) == 0x38) // TBN
            //{
            //}
            //else if ((yOpCode & 0x3F) == 0x39) // TBF
            //{
            //}
            //else if ((yOpCode & 0x3F) == 0x3A) // SBN
            //{
            //}
            //else if ((yOpCode & 0x3F) == 0x3B) // SBF
            //{
            //}
            else if ((yOpCode & 0x3F) == 0x3C) // MVI
            {
                if (IsPrintable (yQByte))
                    strComment = string.Format ("Move '{0}' (0x{1:X2}) to destination", (char)yQByte, yQByte);
                //else
                //    strComment = string.Format ("Move 0x{0:X2} to destination", yQByte);

                //strComment = IsPrintable ((char)yQByte) ? string.Format ("Move 0x{0:X2} '{1}' to destination", yQByte, (char)yQByte) :
                //                                          string.Format ("Move 0x{0:X2} to destination", yQByte);

                m_strlComments.Add (strComment);
            }
            else if ((yOpCode & 0x3F) == 0x3D) // CLI
            {
                if (IsPrintable (yQByte))
                    strComment = string.Format ("Compare '{0}' (0x{1:X2}) to destination", (char)yQByte, yQByte);
                //else
                //    strComment = string.Format ("Compare 0x{0:X2} to destination", yQByte);

                //strComment = IsPrintable ((char)yQByte) ? string.Format ("Compare 0x{0:X2} '{1}' to destination", yQByte, (char)yQByte) :
                //                                          string.Format ("Compare 0x{0:X2} to destination", yQByte);

                m_strlComments.Add (strComment);
            }
            else if ((yOpCode & 0xCF) == 0xC0) // BC
            {
                if (m_bAutoTagJump)
                {
                    CTagEntry objTagEntry = new CTagEntry (m_strlDisassemblyLines.Count, iOperandAddress);
                    objTagEntry.IsJump = iOperandAddress > iInstAddress;
                    objTagEntry.IsLoop = iOperandAddress < iInstAddress;
                    m_sdAllTags[iOperandAddress] = objTagEntry;
                    m_sdTagCalls[iInstAddress] = iOperandAddress;
                }
            }
            else if ((yOpCode & 0xCF) == 0xC1) // TIO
            {
                if (m_bAutoTagJump)
                {
                    CTagEntry objTagEntry = new CTagEntry (m_strlDisassemblyLines.Count, iOperandAddress);
                    objTagEntry.IsJump = iOperandAddress > iInstAddress;
                    objTagEntry.IsLoop = iOperandAddress < iInstAddress;
                    m_sdAllTags[iOperandAddress] = objTagEntry;
                    m_sdTagCalls[iInstAddress] = iOperandAddress;
                    m_strlComments.Insert (0, ""); // Place holder for "Jump/Loop to ..." comment
                }
                m_strlComments.Add (GetIODeviceName (yQByte) + "  " + GetAPLTIODetails (yQByte));
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
                    strComment = "XR1";
                }
                else if ((yQByte & 0x03) == 0x03)
                {
                    strComment = "XR1, XR2";
                }

                if (strComment.Length > 0)
                {
                    m_strlComments.Add (strComment);
                }
            }
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
                                      iStart2, iOperand2Address, iStart1, iOperand1Address, iOverlap);
            else
                return string.Format ("{0} bytes: 0x{1:X4}/0x{2:X4}, 0x{3:X4}/0x{4:X4}", yQByte + 1,
                                      iStart2, iOperand2Address, iStart1, iOperand1Address);
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
                m_strlComments.Insert (0, string.Format ("Halt display: \"{0:S1}{1:S1}\"", GetHaltDisplay (yQByte), GetHaltDisplay (yControlCode)));
            }
            else if (yOpCode == 0xF1) // APL
            {
                m_strlComments.Add (GetIODeviceName (yQByte) + "  " + GetAPLTIODetails (yQByte));
            }
            else if (yOpCode == 0xF2) // JC
            {
                if (m_bAutoTagJump &&
                    yControlCode > 0)
                {
                    // m_iObjectCodeIdx already points to the next byte after the current instruction
                    int iJumpAddress = m_iObjectCodeIdx + yControlCode;
                    CTagEntry objTagEntry = new CTagEntry (m_strlDisassemblyLines.Count, iJumpAddress);
                    objTagEntry.IsJump = true;
                    m_sdAllTags[iJumpAddress] = objTagEntry;
                    m_sdTagCalls[iInstAddress] = iJumpAddress;
                }
            }
            else if (yOpCode == 0xF3) // SIO
            {
                if ((yQByte & 0xF0) == 0xA0 ||
                    (yQByte & 0xF0) == 0xB0)
                {
                    m_strlComments.Add (GetSIODetails (yQByte, yControlCode));
                }
                else
                {
                    string strComment = GetSIODetails (yQByte);
                    m_strlComments.Add (strComment);
                    if (strComment.IndexOf ("invalid") < 0)
                    {
                    string strControlCodeDetails = GetSIOControlCodeDetails (yQByte, yControlCode);
                    if (strControlCodeDetails.Length > 2)
                        m_strlComments.Add (strControlCodeDetails);
                    }
                }
            }
            else
            {
                // Invalid OP code
            }
        }

        private string GetIODeviceName (byte yQByte)
        {
            if ((yQByte & 0xF0) == 0x00)
            {
                return "CPU Console";
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
                return "5471/5475 Keyboard";
            }
            else if ((yQByte & 0xF0) == 0x30)
            {
                return "Serial Input/Output Channel Adapter";
            }
            else if ((yQByte & 0xF0) == 0x80)
            {
                return "BSCA";
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

        private string GetSIODetails (byte yQByte)
        {
            return (GetSIODetails (yQByte, 0));
        }

        private string GetSIODetails (byte yQByte, byte yControlCode)
        {
            if ((yQByte & 0xF0) == 0xE0) // 5203 Line Printer
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

                switch ((byte)(yQByte & 0x07))
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

                switch ((byte)(yQByte & 0x07))
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

                switch ((byte)(yQByte & 0x07))
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

                sbldDevice.Append ("5471/5475 Keyboard");
                sbldDevice.Append ((yQByte & 0x0F) == 0 ? "  " : string.Format ("  <Invalid Q byte> 0x{0:X2}", yQByte));

                return sbldDevice.ToString ();
            }
            else if ((yQByte & 0xF0) == 0x30) // Serial Input/Output Channel Adapter
            {
                return "Serial Input/Output Channel Adapter";
            }
            else if ((yQByte & 0xF0) == 0x80) // BSCA
            {
                return "BSCA";
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
            if ((yQByte & 0xF0) == 0xE0) // 5203 Line Printer
            {
                if ((yQByte & 0x04) == 0)
                {
                    if (yControlCode > 3)
                        return (string.Format ("Space {0} lines invalid", yControlCode));
                    else
                        return (string.Format ("Space {0} lines", yControlCode));
                }
                else
                {
                    if (yControlCode > 112)
                        return (string.Format ("Skip to line {0} invalid", yControlCode));
                    else
                        return (string.Format ("Skip to line {0}", yControlCode));
                }
            }
            else if ((yQByte & 0xF0) == 0xF0) // 5424 MFCU
            {
                StringBuilder sbldDevice = new StringBuilder ();

                if ((yQByte & 0x04) > 0)
                {
                    sbldDevice.Append ((yControlCode & 0x80) == 0 ? "Print buffer 1  " : "Print buffer 2  ");
                    if ((yControlCode & 0x20) > 0)
                        sbldDevice.Append ("Print 4 lines  ");
                }

                if ((yQByte & 0x01) > 0 &&
                    (yControlCode & 0x40) > 0)
                    sbldDevice.Append ("IPL Read  ");

                switch ((byte)(yControlCode & 0x07))
                {
                    case 0x04: sbldDevice.Append ("Stacker 4"); break;
                    case 0x05: sbldDevice.Append ("Stacker 1"); break;
                    case 0x06: sbldDevice.Append ("Stacker 2"); break;
                    case 0x07: sbldDevice.Append ("Stacker 3"); break;
                    default:   sbldDevice.Append ((yQByte & 0x08) == 0 ? "Stacker 1 (default)" : "Stacker 4 (default)"); break;
                }

                return sbldDevice.ToString ();
            }
            else if ((yQByte & 0xF0) == 0x10) // 5471/5475 Keyboard
            {
                StringBuilder sbldDevice = new StringBuilder ();

                if (yControlCode == 0)
                {
                    sbldDevice.Append ("  (no action specified in control code)");
                }
                else
                {
                    if ((yControlCode & 0x80) > 0)
                        sbldDevice.Append ("  Program Numeric Mode");

                    if ((yControlCode & 0x40) > 0)
                        sbldDevice.Append ("  Program Lower Shift");

                    if ((yControlCode & 0x20) > 0)
                        sbldDevice.Append ("  Set Error Indicator");

                    if ((yControlCode & 0x10) > 0)
                        sbldDevice.Append ("  <spare>");

                    if ((yControlCode & 0x08) > 0)
                        sbldDevice.Append ("  Restore Data Key");

                    if ((yControlCode & 0x04) > 0)
                        sbldDevice.Append ("  Unlock Data Key");

                    if ((yControlCode & 0x02) > 0)
                        sbldDevice.Append ("  Enable/Disable Interrupt");

                    if ((yControlCode & 0x01) > 0)
                        sbldDevice.Append ("  Reset Interrupt");
                }

                string strDevice = sbldDevice.ToString ();

                if (sbldDevice.Length > 2)
                    return sbldDevice.ToString ().Substring (2);
                else
                    return "";
            }
            else if ((yQByte & 0xF0) == 0x03) // Serial Input/Output Channel Adapter
            {
                return "";
            }
            else if ((yQByte & 0xF0) == 0x08) // BSCA
            {
                return "";
            }
            else if ((yQByte & 0xF0) == 0x60 || // Tape Drive
                     (yQByte & 0xF0) == 0x70)
            {
                return "";
            }
            else if ((yQByte & 0xF0) == 0x05) // 1442 Card Read Punch
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
            if ((yQByte & 0xF0) == 0xE0) // 5203 Line Printer
            {
                StringBuilder sbldDevice = new StringBuilder ();

                switch ((byte)(yQByte & 0x0F))
                {
                    case 0x00: sbldDevice.Append ("Not ready / check"); break;
                    case 0x02: sbldDevice.Append ("Print buffer busy"); break;
                    case 0x04: sbldDevice.Append ("Carriage busy"); break;
                    case 0x06: sbldDevice.Append ("Printer busy"); break;
                    case 0x09: sbldDevice.Append ("Diagnostic mode"); break;
                    default:   sbldDevice.Append (string.Format ("<Invalid Q byte> 0x{0:X2}", yQByte)); break;
                }

                return sbldDevice.ToString ();
            }
            else if ((yQByte & 0xF0) == 0xF0) // 5424 MFCU
            {
                StringBuilder sbldDevice = new StringBuilder ();

                sbldDevice.Append ((yQByte & 0x08) == 0 ? "Primary" : "Secondary");

                switch ((byte)(yQByte & 0x07))
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

                switch ((byte)(yQByte & 0x07))
                {
                    case 0x00: sbldDevice.Append ("Not ready / check"); break;
                    case 0x02: sbldDevice.Append ("Busy"); break;
                    case 0x04: sbldDevice.Append ("Scan found"); break;
                    default:   sbldDevice.Append (string.Format ("<Invalid Q byte> 0x{0:X2}", yQByte)); break;
                }

                return sbldDevice.ToString ();
            }
            else if ((yQByte & 0xF0) == 0x10) // 5471/5475 Keyboard
            {
                return string.Format ("<Invalid Q byte> 0x{0:X2}", yQByte);
            }
            else if ((yQByte & 0xF0) == 0x03) // Serial Input/Output Channel Adapter
            {
                return "";
            }
            else if ((yQByte & 0xF0) == 0x08) // BSCA
            {
                return "";
            }
            else if ((yQByte & 0xF0) == 0x60 || // Tape Drive
                     (yQByte & 0xF0) == 0x70)
            {
                return "";
            }
            else if ((yQByte & 0xF0) == 0x05) // 1442 Card Read Punch
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
                    switch ((byte)(yQByte & 0x07))
                    {
                        case 0:  sbldDevice.Append ("Carriage print line location counter"); break;
                        case 1:  sbldDevice.Append ("Chain Character Counter"); break;
                        case 2:  sbldDevice.Append ("Printer Timing"); break;
                        case 3:  sbldDevice.Append ("Printer Check Status"); break;
                        case 4:  sbldDevice.Append ("Printer image address register"); break;
                        case 6:  sbldDevice.Append ("Printer data address register"); break;
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
                    switch ((byte)(yQByte & 0x07))
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

                switch ((byte)(yQByte & 0x07))
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

                if ((yQByte & 0x08) == 0)
                {
                    switch ((byte)(yQByte & 0x07))
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


                return sbldDevice.ToString ();
            }
            else if ((yQByte & 0xF0) == 0x03) // Serial Input/Output Channel Adapter
            {
                return "";
            }
            else if ((yQByte & 0xF0) == 0x08) // BSCA
            {
                return "";
            }
            else if ((yQByte & 0xF0) == 0x60 || // Tape Drive
                     (yQByte & 0xF0) == 0x70)
            {
                return "";
            }
            else if ((yQByte & 0xF0) == 0x05) // 1442 Card Read Punch
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
                    switch ((byte)(yQByte & 0x07))
                    {
                        case 0:  sbldDevice.Append ("Forms length register"); break;
                        case 4:  sbldDevice.Append ("Printer image address register");  break;
                        case 6:  sbldDevice.Append ("Printer data address register"); break;
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
                    sbldDevice.Append ((yQByte & 0x08) == 0 ? "Normal mode  " : "Diagnostic mode  ");
                }

                switch ((byte)(yQByte & 0x07))
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

                switch ((byte)(yQByte & 0x07))
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
                return (((yQByte & 0x0F) == 0) ? "Set column indicators" : string.Format ("<Invalid Q byte> 0x{0:X2}", yQByte));
            }
            else if ((yQByte & 0xF0) == 0x03) // Serial Input/Output Channel Adapter
            {
                return "";
            }
            else if ((yQByte & 0xF0) == 0x08) // BSCA
            {
                return "";
            }
            else if ((yQByte & 0xF0) == 0x60 || // Tape Drive
                     (yQByte & 0xF0) == 0x70)
            {
                return "";
            }
            else if ((yQByte & 0xF0) == 0x05) // 1442 Card Read Punch
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
                    return "MZN ";
                }
                else if (yQByte == 0x02)
                {
                    return "MNZ ";
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
                else if (yQByte == 0x87)
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
            }

            // If no match found, return the original mnemonic
            return strMnemonic;
        }

        private void AddDasmLine (int iInstAddress, string strDasmLine)
        {
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
            if (m_strlComments.Count > 0)
            {
                for (; iIdx < m_strlComments.Count; iIdx++)
                {
                    if (m_strlComments[iIdx].Length > 0)
                    {
                        if (m_kiLastPreCommentPos + m_iLineOffset > strbldrDasmLine.Length)
                            strbldrDasmLine.Append (new string (' ', m_kiLastPreCommentPos + m_iLineOffset - strbldrDasmLine.Length));
                        strbldrDasmLine.Append (m_strlComments[iIdx]);
                    }
                }
            }

            m_sdDisassemblyLineIndexes.Add (iInstAddress, m_strlDisassemblyLines.Count);
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
            m_bInData = true;

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
                switch (kvp.Value.NotationType)
                {
                    // NOTE_Begin     Computer  
                    case ENotationType.NOTE_Begin:
                    {
                        break;
                    }

                    // NOTE_Entry     Command   Code  Comment
                    case ENotationType.NOTE_Entry:
                    {
                        m_strTagText = kvp.Value.Notation;
                        m_bInData = false;

                        break;
                    }

                    // NOTE_Code      Command   Code  Text line
                    case ENotationType.NOTE_Code:
                    {
                        m_strlDisassemblyLines.Add ("");
                        m_strlDisassemblyLines.Add ("");
                        m_strlDisassemblyLines.Add (kvp.Value.Notation);
                        m_strlDisassemblyLines.Add ("");
                        m_bInData = false;

                        break;
                    }

                    // NOTE_Data      Command   Data  Text line
                    case ENotationType.NOTE_Data:
                    {
                        m_strlDisassemblyLines.Add ("");
                        m_strlDisassemblyLines.Add ("");
                        m_strlDisassemblyLines.Add (kvp.Value.Notation);
                        m_strlDisassemblyLines.Add ("");

                        // Look for the next DCP to get next address
                        int iDumpEndAddress = m_yaObjectCode.GetUpperBound (0);
                        //for (int iIdx = m_iIdxDCP + 1; iIdx < m_sdDCP.Count; iIdx++)
                        //{
                        //    if (kvp.Value.NotationType == ENotationType.NOTE_Code ||
                        //        kvp.Value.NotationType == ENotationType.NOTE_Comment ||
                        //        kvp.Value.NotationType == ENotationType.NOTE_Data ||
                        //        kvp.Value.NotationType == ENotationType.NOTE_Jump ||
                        //        kvp.Value.NotationType == ENotationType.NOTE_Loop ||
                        //        kvp.Value.NotationType == ENotationType.NOTE_Tag ||
                        //        kvp.Value.NotationType == ENotationType.NOTE_Variable ||
                        //        kvp.Value.NotationType == ENotationType.NOTE_Constant ||
                        //        kvp.Value.NotationType == ENotationType.NOTE_End)
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
                        foreach (string str in strlDataDump)
                        {
                            m_strlDisassemblyLines.Add (str);
                        }

                        m_bInData = true;

                        break;
                    }

                    // NOTE_Skip      Command
                    case ENotationType.NOTE_Skip:
                    {
                        // Set the address of the next byte to decode
                        foreach (KeyValuePair<int, CDasmControlPoint> kvp2 in m_sdDCP)
                        {
                            if (kvp2.Value.NotationType == ENotationType.NOTE_Code     ||
                                kvp2.Value.NotationType == ENotationType.NOTE_Comment  ||
                                kvp2.Value.NotationType == ENotationType.NOTE_Data     ||
                                kvp2.Value.NotationType == ENotationType.NOTE_Jump     ||
                                kvp2.Value.NotationType == ENotationType.NOTE_Loop     ||
                                kvp2.Value.NotationType == ENotationType.NOTE_Tag      ||
                                kvp2.Value.NotationType == ENotationType.NOTE_Variable ||
                                kvp2.Value.NotationType == ENotationType.NOTE_Constant)
                            {
                                m_iObjectCodeIdx = kvp.Value.Address;
                                m_strlDisassemblyLines.Add ("");

                                break;
                            }
                        }

                        break;
                    }

                    // NOTE_Tag       Command   Code  Comment
                    case ENotationType.NOTE_Tag:
                    {
                        m_strTagText = kvp.Value.Notation;
                        m_bInData = false;

                        break;
                    }

                    // NOTE_Variable  Command   Data  Comment
                    // NOTE_Constant  Command   Data  Comment
                    case ENotationType.NOTE_Variable:
                    case ENotationType.NOTE_Constant:
                    {
                        m_strTagText = kvp.Value.Notation;
                        m_bInData = true;

                        break;
                    }

                    // NOTE_Comment   Command         Comment
                    case ENotationType.NOTE_Comment:
                    {
                        m_strlComments.Add (kvp.Value.Notation);
                        break;
                    }

                    // NOTE_Jump      Computer  Code  Comment
                    case ENotationType.NOTE_Jump:
                    {
                        m_strTagText = kvp.Value.Notation;
                        m_bInData = false;

                        break;
                    }

                    // NOTE_Loop      Computer  Code  Comment
                    case ENotationType.NOTE_Loop:
                    {
                        // TODO: revise line in map that match address
                        m_strTagText = kvp.Value.Notation;
                        m_bInData = false;

                        break;
                    }

                    // NOTE_End       Computer
                    case ENotationType.NOTE_End:
                    {
                        m_bDumpComplete = true;

                        break;
                    }
                }

                m_iIdxDCP++;
            }

            return m_bInData;
        }

        int InsertDasmControlPoint (int iAddress, ENotationType eNotationType, string strNotation)
        {
            bool bEntryInserted = false;

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
            if (eNotationType == ENotationType.NOTE_Tag ||
                eNotationType == ENotationType.NOTE_Variable ||
                eNotationType == ENotationType.NOTE_Constant ||
                eNotationType == ENotationType.NOTE_Entry ||
                eNotationType == ENotationType.NOTE_Jump ||
                eNotationType == ENotationType.NOTE_Loop)
            {
                //if (m_sdTagNames.ContainsKey (iAddress))
                //{
                //    if (eNotationType == ENotationType.NOTE_Tag)
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
                    if (eNotationType == ENotationType.NOTE_Variable ||
                        eNotationType == ENotationType.NOTE_Constant ||
                        eNotationType == ENotationType.NOTE_Jump ||
                        eNotationType == ENotationType.NOTE_Loop)
                    {
                        if (kvp.Value.NotationType == ENotationType.NOTE_Entry ||
                            kvp.Value.NotationType == ENotationType.NOTE_Tag ||
                            kvp.Value.NotationType == ENotationType.NOTE_Variable ||
                            kvp.Value.NotationType == ENotationType.NOTE_Constant ||
                            kvp.Value.NotationType == ENotationType.NOTE_Jump ||
                            kvp.Value.NotationType == ENotationType.NOTE_Loop)
                        {
                            // Each address can have only one tag, so mark all others as comments
                            eNotationType = ENotationType.NOTE_Comment;
                        }
                    }
                    else if (eNotationType == ENotationType.NOTE_Tag)
                    {
                        if (kvp.Value.NotationType == ENotationType.NOTE_Entry ||
                            kvp.Value.NotationType == ENotationType.NOTE_Variable ||
                            kvp.Value.NotationType == ENotationType.NOTE_Constant ||
                            kvp.Value.NotationType == ENotationType.NOTE_Jump ||
                            kvp.Value.NotationType == ENotationType.NOTE_Loop)
                        {
                            // Tags pre-empt all other types for the same address
                            kvp.Value.NotationType = ENotationType.NOTE_Comment;
                        }
                    }
                }
                else if (kvp.Value.Address > iAddress)
                {
                    if (eNotationType == ENotationType.NOTE_Entry)
                    {
                        if (strNotation.Length == 0)
                        {
                            strNotation = "Entry ->";
                        }
                    }

                    if (eNotationType == ENotationType.NOTE_Tag ||
                        eNotationType == ENotationType.NOTE_Variable ||
                        eNotationType == ENotationType.NOTE_Constant ||
                        eNotationType == ENotationType.NOTE_Jump)
                    {
                        if (strNotation.Length > m_iMaxTagLength)
                        {
                            // Get the length of the longest tag being added
                            m_iMaxTagLength = strNotation.Length;
                        }
                    }
                    //else if (eNotationType == ENotationType.NOTE_Loop)
                    //{
                    //    if (strNotation.Length > m_iMaxTagLength)
                    //    {
                    //        // Loops are added after the disassembly has begun,
                    //        // so their text must not exceed m_iMaxTagLength
                    //        strNotation = strNotation.Substring (0, m_iMaxTagLength);
                    //    }
                    //}

                    //if (eNotationType == ENotationType.NOTE_Loop &&
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
                        m_sdDCP[iAddress] = new CDasmControlPoint (iAddress, eNotationType, strNotation);
                        bEntryInserted = true;
                    }

                    break; // No need to continue searching after inserting
                }
            }

            if (!bEntryInserted)
            {
                m_sdDCP[iAddress] = new CDasmControlPoint (iAddress, eNotationType, strNotation);
            }

            DumpListDCP ();

            return m_sdDCP.Count;
        }

        private void DumpListDCP ()
        {
#if DEBUG
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
            int iHexDumpEnd = 38, // 37,  // 58,  // 63, //37,
                iAsciiDumpEnd = 48, // 47,  // 78,  // 84, //47,
                iEbcdicDumpEnd = 61, // 60,  //100,  //108, //60,
                iOffset        = 0;

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
                    cData <= (char)0x7F)
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
                    cData <= (char)0x7F)
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
            if (strbldrDumpLine.Length < iEbcdicDumpEnd)
            {
                strbldrDumpLine.Append (new string (' ', iEbcdicDumpEnd - strbldrDumpLine.Length));
            }
            strbldrDumpLine.Append (">");

            AddDasmLine (m_iDumpStartAddress, strbldrDumpLine.ToString ());

            // Empty the buffer
            m_ylDataBuffer.Clear ();
            m_iDumpStartAddress = -1;
        }

        private char GetHaltDisplay (byte yHaltCode)
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
    }
}
