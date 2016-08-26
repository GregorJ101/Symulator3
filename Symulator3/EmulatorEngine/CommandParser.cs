using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EmulatorEngine
{
    // Command Parser
    public partial class CDataConversion : CStringProcessor
    {
        // Type definitions
        protected enum ECardProcessState
        {
            CARD_Normal,
            CARD_Ignore,
            CARD_ListGroup,
            CARD_PRNT,
            CARD_PrntGroup,
            CARD_Matrix,
            CARD_IPL,
            CARD_Text,
            CARD_RelocatableText,
            CARD_Hex,
            CARD_Binary
        };

        protected enum ENotationType
        {
            NOTE_Undefined,
            NOTE_Begin,      // Computer  
            NOTE_Entry,      // Command   Code  Tag/Comment
            NOTE_Code,       // Command   Code  Text line
            NOTE_Data,       // Command   Data  Text line
            NOTE_Skip,       // Command
            NOTE_Tag,        // Command   Code  Tag/Comment
            NOTE_Variable,   // Command   Data  Tag/Comment
            NOTE_Constant,   // Command   Data  Tag/Comment
            NOTE_Comment,    // Command         Comment
            NOTE_Jump,       // Computer  Code  Tag/Comment
            NOTE_Loop,       // Computer  Code  Tag/Comment
            NOTE_End         // Computer
        };

        // Command Parser member data
        protected string m_kstrParseSeparators = "/:,\"";

        protected ECardProcessState m_eCardProcessState = ECardProcessState.CARD_Normal;

        protected byte[] m_yaProgramImage;
        protected List<string> m_strlLoadedTextFile;
        protected List<string> m_strlPatternCardGroup;

        protected string m_strProgramName;

        protected int  m_iBlankLines   = 0,
                       m_iCoreSize     = 64 * 1024,
                       m_iDasmStart    = -1,
                       m_iDasmEnd      = -1,
                       m_iDasmEntry    = -1,
                       m_iDasmStartEC  = -1,
                       m_iDasmEndEC    = -1,
                       m_iDasmEntryEC  = -1;
        protected char m_cHole         = 'o';
        protected bool m_bEdge             = false,
                       m_bSingleSpace      = false,
                       m_bWrap96           = false,
                       m_bShowTralier      = false,
                       m_bSeperator        = false,
                       m_bEndCardRead      = false,
                       m_bTextCardRead     = false,
                       m_bShowHelp         = false,
                       m_bReadExternalFile = false;

        public bool ProcessDiskCommandLine (string strCommandLine)
        {
            // /Load Disk loads disk file (only one loaded at a time)
            // /Dump c:s-e[;c:s-e[;c:s-e[...]]] dumps sectors (c = cylinder, s = start sec#, e = end sec#)
            // /DASM c:s-e[;0xaaaa(0xbbbb)] aaaa = start offset to begin DASM, bbbb = starting address to display

            return true;
        }

        public bool ProcessCardCommandLine (string strCommandLine)
        {
            //for (int iIdx = 0; iIdx < m_yaHaltCodes.Length; iIdx++)
            //{
            //    Console.WriteLine (string.Format ("GetHaltDisplay (0x{0:X2}) == '{1:S1}'", m_yaHaltCodes[iIdx], GetHaltDisplay (m_yaHaltCodes[iIdx])));
            //}
            // See if all lines are being skipped:
            if (m_eCardProcessState == ECardProcessState.CARD_Ignore &&
                !CompareNoCase (strCommandLine, "/EndIgnore"))
            {
                return true;
            }

            // Ignore all blank lines
            if (m_bShowHelp ||
                strCommandLine.Length < 1)
            {
                return false;
            }

            // Ignore all non-command lines
            if (strCommandLine[0] != '/')
            {
                return false;
            }

            // Before processing command, first validate text deck if read in
            if (m_eCardProcessState == ECardProcessState.CARD_Text)
            {
                if (!m_bTextCardRead)
                {
                    WriteOutputLine ("No Text cards found.");
                }
                else if (!m_bEndCardRead)
                {
                    WriteOutputLine ("No End card found.");
                }

                // Load text card file into memory
                m_yaProgramImage = LoadTextFile (m_strlLoadedTextFile).ToArray ();
            }

            // Each file may contain multiple sequence groups.
            // Within each group must be at least one command line /LOAD for loading following lines (cards)
            // Optional lines include PRNT in various forms, TITLE, TXLT, DUMP, DASM
            List<string> strlTokens = ParseStringToList (strCommandLine, m_kstrParseSeparators);
            if (CompareNoCase (strlTokens[0], "Help") ||
                CompareNoCase (strlTokens[0], "?"))
            {
                ShowHelp ();

                m_bShowHelp = true;

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "IgnoreGroup"))
            {
                // /Ignore this line and all others until /EndIgnore is found
                m_eCardProcessState = ECardProcessState.CARD_Ignore;
                return true;
            }
            else if (CompareNoCase (strlTokens[0], "EndIgnore"))
            {
                m_eCardProcessState = ECardProcessState.CARD_Normal;
                return true;
            }
            else if (strlTokens[0].Length >= 6 &&
                CompareNoCase (strlTokens[0].Substring (0,6), "Ignore"))
            {
                // /Ignore (this line can contain any comments useful when editing the file to be parsed)
                return true;
            }
            else if (CompareNoCase (strlTokens[0], "Program"))
            {
                // /Program:{name of program}
                if (strlTokens.Count > 1)
                {
                    string strSearchKey = strlTokens[0] + ":";
                    int iIdx = strCommandLine.IndexOf (strSearchKey) + strSearchKey.Length;
                    m_strProgramName = strCommandLine.Substring (iIdx);
                }
                else
                {
                    // Missing argument
                    ShowProgramHelp ();
                }

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "CoreSize"))
            {
                // /CoreSize:{main memory size for emulator in increments of 1024 byts}
                if (strlTokens.Count > 1)
                {
                    int iNewSize = StringToInt (strlTokens[1], 10);
                    if (iNewSize > 1024 &&
                        iNewSize <= 64 * 1024)
                    {
                        m_iCoreSize = iNewSize;
                    }
                }
                else
                {
                    // Missing argument
                    ShowCoreSizeHelp ();
                }

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "Title"))
            {
                // /Title:Text/Matrix,<title text to be centered in 100-char line
                //        Text means print line out as is
                //        Matrix means print as 5x5 block letters
                bool bShowHelp = false;
                if (strlTokens.Count > 1)
                {
                    if (CompareNoCase (strlTokens[1], "Text"))
                    {
                        string strSearchKey = strlTokens[1] + ":";
                        int iIdx = strCommandLine.IndexOf (strSearchKey) + strSearchKey.Length;
                        string strTitle = strCommandLine.Substring (iIdx);

                        WriteOutputLine (strTitle);

                        char[] caSpace = { ' ' };
                        int iTrimLength = strTitle.TrimStart (caSpace).Length,
                            iLeadingSpaceCount = strTitle.Length - iTrimLength;

                        string strLead = new string (' ', iLeadingSpaceCount);
                        string strUnderLine = new string ('=', iTrimLength);
                        WriteOutputLine (strLead + strUnderLine);
                        WriteOutputLine ("");
                    }
                    else if (CompareNoCase (strlTokens[1], "Matrix"))
                    {
                    }
                    else
                    {
                        // Unrecognized argument
                        WriteOutputLine (string.Format ("Unrecognized Title argument: {0:S}", strlTokens[1]));
                        bShowHelp = true;
                    }
                }
                else
                {
                    // Missing argument
                    ShowTitleHelp ();
                }

                if (bShowHelp)
                {
                    ShowTitleHelp ();
                }

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "Separator"))
            {
                // /Separator:{lenght of line}{{,}#leading spaces}{{,}char/string to repeat}{{,}#blank lines before}{{,}#blank lines after}
                int iLeadingSpaces       = 0,
                    iCharacterLineLength = 0,
                    iLeadingLines        = 0,
                    iTrailingLines       = 0;
                string strRepeatingCharacters = "";
                StringBuilder strbldrSeperatorLine = new StringBuilder (128);

                if (strlTokens.Count < 2)
                {
                    // Missing arguments
                    ShowSeparatorHelp ();

                    return true;
                }

                for (int iIdx = 1; iIdx < strlTokens.Count; iIdx++)
                {
                    if (iIdx == 1)
                    {
                        iLeadingSpaces = StringToInt (strlTokens[iIdx], 10);
                    }
                    else if (iIdx == 2)
                    {
                        iCharacterLineLength = StringToInt (strlTokens[iIdx], 10);
                    }
                    else if (iIdx == 3)
                    {
                        strRepeatingCharacters = strlTokens[iIdx];
                    }
                    else if (iIdx == 4)
                    {
                        iLeadingLines = StringToInt (strlTokens[iIdx], 10);
                    }
                    else if (iIdx == 5)
                    {
                        iTrailingLines = StringToInt (strlTokens[iIdx], 10);
                    }
                }

                if (iCharacterLineLength == 0 ||
                    strRepeatingCharacters.Length == 0)
                {
                    // Nothing to print, so bail
                    return true;
                }

                for (int iIdx = 0; iIdx < iLeadingLines; iIdx++)
                {
                    WriteOutputLine ("");
                }

                if (iLeadingSpaces > 0)
                {
                    strbldrSeperatorLine.Append (new string (' ', iLeadingSpaces));
                }

                while (iCharacterLineLength > 0)
                {
                    strbldrSeperatorLine.Append (strRepeatingCharacters);
                    iCharacterLineLength -= strRepeatingCharacters.Length;
                }
                WriteOutputLine (strbldrSeperatorLine.ToString ());

                for (int iIdx = 0; iIdx < iTrailingLines; iIdx++)
                {
                    WriteOutputLine ("");
                }

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "Space"))
            {
                // /Space:{# blank lines to insert}
                //        No number specified causes a default of a single blank line inserted
                if (strlTokens.Count > 1)
                {
                    byte yLineCount = Convert.ToByte (strlTokens[1]);
                    while (yLineCount-- > 0)
                    {
                        WriteOutputLine ("");
                    }
                }
                else
                {
                    // Missing argument, default to a single space
                    WriteOutputLine ("");
                }

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "PrntSetup"))
            {
                // /PrntSetup {Edge}{{,}Char:x}{{,}TightColumns}{{,}Wrap96|NoWrap}{{,}ShowTrailer}{{,}Separator}{{,}Spacing:n}
                //             Edge to show card edge outline around punch pattern characters
                if (strlTokens.Count < 2)
                {
                    // Missing argument
                    ShowPrntSetupHelp ();

                    return true;
                }

                bool bShowHelp = false;
                for (int iIdx = 1; iIdx < strlTokens.Count; iIdx++)
                {
                    if (CompareNoCase (strlTokens[iIdx], "Edge"))
                    {
                        m_bEdge = true;
                    }
                    else if (CompareNoCase (strlTokens[iIdx], "Char"))
                    {
                        if (strlTokens.Count - iIdx > 1)
                        {
                            m_cHole = strlTokens[++iIdx][0];
                        }
                    }
                    else if (CompareNoCase (strlTokens[iIdx], "TightColumns"))
                    {
                        m_bSingleSpace = true;
                    }
                    else if (CompareNoCase (strlTokens[iIdx], "Wrap96"))
                    {
                        m_bWrap96 = true;
                    }
                    else if (CompareNoCase (strlTokens[iIdx], "NoWrap"))
                    {
                        m_bWrap96 = false;
                    }
                    else if (CompareNoCase (strlTokens[iIdx], "ShowTrailer"))
                    {
                        m_bShowTralier = true;
                    }
                    else if (CompareNoCase (strlTokens[iIdx], "Separator"))
                    {
                        m_bSeperator = true;
                    }
                    else if (CompareNoCase (strlTokens[iIdx], "Spacing"))
                    {
                        if (strlTokens.Count - iIdx > 1)
                        {
                            m_iBlankLines = StringToInt (strlTokens[++iIdx], 10);
                        }
                    }
                    else
                    {
                        // Unrecognized argument
                        WriteOutputLine (string.Format ("Unrecognized PrntSetup argument: {0:S}", strlTokens[iIdx]));
                        bShowHelp = true;
                    }
                }

                if (bShowHelp)
                {
                    ShowPrntSetupHelp ();
                }

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "PrntDefault"))
            {
                // /PrntDefault (no arguments)
                m_bEdge = false;
                m_bSingleSpace = false;
                m_bWrap96 = false;
                m_bShowTralier = false;
                m_bSeperator = false;
                m_cHole = 'o';
                m_iBlankLines = 0;

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "PRNT"))
            {
                // /PRNT:<text to treat as punch characters>
                //        Groups of PRNT lines are centered in 80-char line or longest input line if longer
                //        and displayed in the order in which they are read; these display before all else

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "PrntTextMatrix"))
            {
                // /PrntTextMatrix:<followed by text to be expanded to small block letters in 5x5 matrix,
                //       centered in 100-character line unless longer.
                //     If several /PrntTextMatrix lines appear, all are centered in the longest line if the longest
                //       line exceeds 100 characters

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "Prnt96"))
            {
                // /Prnt96:<96 columns of text> for individual cards; limited to or expanded to 96 char
                if (strlTokens.Count > 1)
                {
                    int iIdx = strCommandLine.IndexOf ("96:") + 3;
                    int iStringLength = strCommandLine.Length - iIdx;
                    if (iStringLength > 96)
                    {
                        iStringLength = 96;
                    }

                    string strPatternString = strCommandLine.Substring (iIdx, iStringLength);
                    List<string> strlPunchImage = CreatePunchImage (strPatternString, m_cHole, !m_bSingleSpace);
                    PrintStringList (strlPunchImage);

                    if (m_bShowTralier)
                    {
                        if (strCommandLine.Length - iIdx > 96)
                        {
                            WriteOutputLine (strCommandLine.Substring (iIdx + 96));
                        }
                    }

                    if (m_bSeperator)
                    {
                        string strSeperator = new string ('=', 100);
                        WriteOutputLine (strSeperator);
                    }

                    if (m_iBlankLines > 0)
                    {
                        for (int iLine = 0; iLine < m_iBlankLines; iLine++)
                        {
                            WriteOutputLine ("");
                        }
                    }
                }
                else
                {
                    // Missing argument
                    ShowPrnt96Help ();
                }

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "PrntAll"))
            {
                // /PrntAll:{96}/{LINE}
                //          96: Treat each line as a 3-tier card
                //          LINE: Treat each line as single long tier, centered on 100-char line

                if (strlTokens.Count < 2)
                {
                    // Missing argument
                    ShowPrntAllHelp ();

                    return true;
                }

                m_eCardProcessState = ECardProcessState.CARD_PRNT;
                bool bShowHelp = false;
                for (int iIdx = 1; iIdx < strlTokens.Count; iIdx++)
                {
                    if (CompareNoCase (strlTokens[iIdx], "96"))
                    {
                        m_bWrap96 = true;
                    }
                    else if (CompareNoCase (strlTokens[iIdx], "Line"))
                    {
                        m_bWrap96 = false;
                    }
                    else
                    {
                        // Unrecognized argument
                        WriteOutputLine (string.Format ("Unrecognized PrntAll argument: {0:S}", strlTokens[iIdx]));
                        bShowHelp = true;
                    }
                }

                if (bShowHelp)
                {
                    ShowPrntAllHelp ();
                }

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "PrntGroup"))
            {
                // /PrntGroup: Accumulate lines treated as 1 tier per line
                //             Center all on 100 or length of longest line, whichever is longer
                //             Accumulate lines until /ENDGROUP found or end of file
                m_strlPatternCardGroup.Clear ();
                m_eCardProcessState = ECardProcessState.CARD_PrntGroup;

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "ListGroup"))
            {
                m_eCardProcessState = ECardProcessState.CARD_ListGroup;

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "EndGroup"))
            {
                if (m_eCardProcessState == ECardProcessState.CARD_Matrix)
                {
                }
                else if (m_eCardProcessState == ECardProcessState.CARD_PRNT)
                {
                }
                else if (m_eCardProcessState == ECardProcessState.CARD_PrntGroup)
                {
                }

                m_eCardProcessState = ECardProcessState.CARD_Normal;

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "Load"))
            {
                // /Load:<IPL/TEXT/HEX>{,File}},<start source address>,<start target address>,<length>
                //       addresses & length ignored for TEXT
                //    IPL:  lines that follow are to be interpreted as IPL-format cards
                //    TEXT: lines that follow are to be interpreted as text-format cards
                //    HEX:  lines that follow are to be simply compressed from the hex character pairs
                //          The following parameters <start>/<end>/<entry> are required for HEX:
                //            <start> Load characters begining at this address
                //            <end>   End address of code block
                //            <entry> Entry point
                // /Load must be the last command line before any data lines
                //       The commands /PLLT, /DUMP, and /DASM operate on the lines (cards) just loaded, and may
                //         ONLY be used AFTER a /LOAD command has been processed, AND subsequent lines (cards)
                //         successfully loaded into a binary image
                //       When load comes again, the previous load image is destroyed and replaced with new data
                //       The command /PLLT may only be used if the last /LOAD line processed was for TEXT line
                //       The commands /DUMP and /DASM may be used on any load format

                // Reset values
                PrepForLoad ();
                PrepForDASM ();

                if (strlTokens.Count > 1)
                {
                    //m_yaCoreMemory = new byte[m_iCoreSize];

                    if (CompareNoCase (strlTokens[1], "IPL"))
                    {
                        m_eCardProcessState = ECardProcessState.CARD_IPL;
                    }
                    else if (CompareNoCase (strlTokens[1], "TEXT"))
                    {
                        m_eCardProcessState = ECardProcessState.CARD_Text;
                    }
                    else if (CompareNoCase (strlTokens[1], "RTEXT"))
                    {
                        m_eCardProcessState = ECardProcessState.CARD_RelocatableText;
                    }
                    else if (CompareNoCase (strlTokens[1], "HEX"))
                    {
                        m_eCardProcessState = ECardProcessState.CARD_Hex;
                    }
                    else if (CompareNoCase (strlTokens[1], "Binary"))
                    {
                        m_eCardProcessState = ECardProcessState.CARD_Binary;
                    }
                    else
                    {
                        // Unrecognized argument
                        WriteOutputLine (string.Format ("Unrecognized Load argument: {0:S}", strlTokens[1]));
                        ShowLoadHelp ();

                        return true;
                    }
                }
                else
                {
                    // Missing argument
                    ShowLoadHelp ();
                }

                if (strlTokens.Count > 2)
                {
                    if (CompareNoCase (strlTokens[2], "File"))
                    {
                        m_bReadExternalFile = true;
                    }
                }

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "Load+"))
            {
                // /LOAD+ indicates that the following line(s) are to be added to the existing binary image

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "TXLT"))
            {
                // /TXLT:{ShowRaw}{{,}DumpLiterals}{{,}ShowMove} for text-to-list
                //   Indicates immedate display action
                bool bShowRawText              = false;
                bool bShowCompressedCharacters = false;
                bool bShowMoveDetails          = false;
                bool bShowHelp                 = false;

                for (int iIdx = 1; iIdx < strlTokens.Count; iIdx++)
                {
                    if (CompareNoCase (strlTokens[iIdx], "ShowRaw"))
                    {
                        bShowRawText = true;
                    }
                    else if (CompareNoCase (strlTokens[iIdx], "DumpLiterals"))
                    {
                        bShowCompressedCharacters = true;
                    }
                    else if (CompareNoCase (strlTokens[iIdx], "ShowMove"))
                    {
                        bShowMoveDetails = true;
                    }
                    else
                    {
                        // Unrecognized argument
                        WriteOutputLine (string.Format ("Unrecognized TXLT argument: {0:S}", strlTokens[iIdx]));
                        bShowHelp = true;
                    }
                }

                if (bShowHelp)
                {
                    ShowTxltHelp ();
                    return true;
                }

                PrintStringList (DumpTextFileLines (m_strlLoadedTextFile, bShowRawText, bShowCompressedCharacters, bShowMoveDetails));

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "Dump"))
            {
                // /DUMP:<start address>,<end address>
                int iDumpStart = -1,
                    iDumpEnd = -1;

                for (int iIdx = 1; iIdx < strlTokens.Count; iIdx++)
                {
                    if (iIdx == 1)
                    {
                        iDumpStart = StringToInt (strlTokens[iIdx], 16);
                    }
                    else if (iIdx == 2)
                    {
                        int iNewValue = StringToInt (strlTokens[iIdx], 16);
                        if (iNewValue > iDumpStart)
                        {
                            iDumpEnd = iNewValue;
                        }
                    }
                }

                if (iDumpStart >= 0 &&
                    iDumpEnd >= 0)
                {
                    PrintStringList (BinaryToDump (m_yaProgramImage, iDumpStart, iDumpEnd));
                }
                else if (m_iLowAddressMI < m_iHighAddressMI &&
                         m_iHighAddressMI > 0)
                {
                    PrintStringList (BinaryToDump (m_yaProgramImage, m_iLowAddressMI, m_iHighAddressMI));
                }
                else
                {
                    PrintStringList (BinaryToDump (m_yaProgramImage));
                }

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "DumpAll"))
            {
                // /DUMPALL for entire binary image
                //   Indicates immedate display action
                PrintStringList (BinaryToDump (m_yaProgramImage));

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "DASM") ||
                     CompareNoCase (strlTokens[0], "DASM-Range"))
            {
                // /DASM:<start address>,<end address>,<entry point>
                for (int iIdx = 1; iIdx < strlTokens.Count; iIdx++)
                {
                    if (iIdx == 1)
                    {
                        m_iDasmStart = StringToInt (strlTokens[iIdx], 16);
                    }
                    else if (iIdx == 2)
                    {
                        int iNewValue = StringToInt (strlTokens[iIdx], 16);
                        if (iNewValue > m_iDasmEnd)
                        {
                            m_iDasmEnd = iNewValue;
                        }
                    }
                    else if (iIdx == 3)
                    {
                        int iNewValue = StringToInt (strlTokens[iIdx], 16);
                        if (iNewValue > m_iDasmStart &&
                            iNewValue < m_iDasmEnd)
                        {
                            m_iDasmEntry = iNewValue;
                        }
                    }
                }

                if (CompareNoCase (strlTokens[0], "DASM"))
                {
                    if (m_iDasmStart >= 0 &&
                        m_iDasmEnd   > m_iDasmStart)
                    {
                        PrintStringList (DisassembleCode (m_yaProgramImage, m_iDasmStart, m_iDasmEnd));
                    }
                    else if (m_iLowAddressMI  < m_iHighAddressMI &&
                             m_iHighAddressMI > 0)
                    {
                        PrintStringList (DisassembleCode (m_yaProgramImage, m_iLowAddressMI, m_iHighAddressMI));
                    }
                    else
                    {
                        PrintStringList (DisassembleCode (m_yaProgramImage));
                    }
                }

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "DASM-EndCard"))
            {
                // /DASM-EndCard:<start address>,<end address>,<entry point>
                for (int iIdx = 1; iIdx < strlTokens.Count; iIdx++)
                {
                    if (iIdx == 1)
                    {
                        m_iDasmStartEC = StringToInt (strlTokens[iIdx], 16);
                    }
                    else if (iIdx == 2)
                    {
                        int iNewValue = StringToInt (strlTokens[iIdx], 16);
                        if (iNewValue > m_iDasmEndEC)
                        {
                            m_iDasmEndEC = iNewValue;
                        }
                    }
                    else if (iIdx == 3)
                    {
                        int iNewValue = StringToInt (strlTokens[iIdx], 16);
                        if (iNewValue > m_iDasmStartEC &&
                            iNewValue < m_iDasmEndEC)
                        {
                            m_iDasmEntryEC = iNewValue;
                        }
                    }
                }

                if (m_iDasmStartEC != -1 &&
                    m_iDasmEndEC   != -1 &&
                    m_iDasmEndEC   > m_iDasmStartEC)
                {
                    WriteEndCardTitleLine ();
                    BackupAndClearListDCP (m_iDasmStartEC, m_iDasmEndEC);
                    PrintStringList (DisassembleCode (m_yaEndCard, m_iDasmStartEC, m_iDasmEndEC));
                    RestoreListDCP ();
                }
                else if (m_iLowAddressEC  < m_iHighAddressEC &&
                         m_iHighAddressEC > 0)
                {
                    BackupAndClearListDCP (m_iLowAddressEC, m_iHighAddressEC);
                    PrintStringList (DisassembleCode (m_yaEndCard, m_iLowAddressEC, m_iHighAddressEC));
                    RestoreListDCP ();
                }
                else
                {
                    BackupAndClearListDCP (0x019, m_yaEndCard.GetUpperBound (0));
                    PrintStringList (DisassembleCode (m_yaEndCard));
                    RestoreListDCP ();
                }

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "DASM-Entry"))
            {
                // /DASM-Entry: <address>, <label to be displayed as label or comment on same line>
                if (strlTokens.Count < 2)
                {
                    // Missing argument
                    ShowDasmEntryHelp ();

                    return true;
                }

                InsertDasmControlPoint (strlTokens, ENotationType.NOTE_Entry, strCommandLine);

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "DASM-Code"))
            {
                // /DASM-Code: <address>, <label to be displayed as a separate line ahead of code block>
                if (strlTokens.Count < 2)
                {
                    // Missing argument
                    ShowDasmCodeHelp ();

                    return true;
                }

                InsertDasmControlPoint (strlTokens, ENotationType.NOTE_Code, strCommandLine);

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "DASM-Data"))
            {
                // /DASM-Data: <address>, <label to be displayed as a separate line ahead of data block>
                if (strlTokens.Count < 2)
                {
                    // Missing argument
                    ShowDasmDataHelp ();

                    return true;
                }

                InsertDasmControlPoint (strlTokens, ENotationType.NOTE_Data, strCommandLine);

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "DASM-Skip"))
            {
                // /DASM-Skip: <address>
                // Indicates start address of area to ignore
                InsertDasmControlPoint (strlTokens, ENotationType.NOTE_Skip);

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "DASM-Tag"))
            {
                // /DASM-Tag: <address>, <label to be displayed on same line as location and any branches / jumps>
                if (strlTokens.Count < 2)
                {
                    // Missing argument
                    ShowDasmTagHelp ();

                    return true;
                }

                InsertDasmControlPoint (strlTokens, ENotationType.NOTE_Tag, strCommandLine);

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "DASM-Variable"))
            {
                // /DASM-Variable: <address>, <length>, <name to be applied to operand addresses>
                if (strlTokens.Count < 2)
                {
                    // Missing argument
                    ShowDasmVariableHelp ();

                    return true;
                }

                InsertDasmControlPoint (strlTokens, ENotationType.NOTE_Variable, strCommandLine);

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "DASM-Constant"))
            {
                // /DASM-Constant: <address>, <length>, <name to be applied to operand addresses>
                if (strlTokens.Count < 2)
                {
                    // Missing argument
                    ShowDasmConstantHelp ();

                    return true;
                }

                InsertDasmControlPoint (strlTokens, ENotationType.NOTE_Constant, strCommandLine);

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "DASM-Comment"))
            {
                // /DASM-Comment:<address>,<label> for instruction comments
                if (strlTokens.Count < 2)
                {
                    // Missing argument
                    ShowDasmCommentHelp ();

                    return true;
                }

                InsertDasmControlPoint (strlTokens, ENotationType.NOTE_Comment, strCommandLine);

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "DASM-AutoTag"))
            {
                // /DASM-AutoTag:{Jumps}{{,}{Loops}{{,}{All}{{,}{None}
                if (strlTokens.Count < 2)
                {
                    // Missing argument
                    m_bAutoTagJump = true;
                    m_bAutoTagLoop = true;
                    m_iMaxTagLength = 7; // Length of "Jump_00" & "Loop_00"

                    return true;
                }

                bool bShowHelp = false;
                for (int iIdx = 1; iIdx < strlTokens.Count; iIdx++)
                {
                    if (CompareNoCase (strlTokens[iIdx], "Jumps"))
                    {
                        m_bAutoTagJump = true;
                        m_iMaxTagLength = 7; // Length of "Jump_00" & "Loop_00"
                    }
                    else if (CompareNoCase (strlTokens[iIdx], "Loops"))
                    {
                        m_bAutoTagLoop = true;
                        m_iMaxTagLength = 7; // Length of "Jump_00" & "Loop_00"
                    }
                    else if (CompareNoCase (strlTokens[iIdx], "All"))
                    {
                        m_bAutoTagJump = true;
                        m_bAutoTagLoop = true;
                        m_iMaxTagLength = 7; // Length of "Jump_00" & "Loop_00"
                    }
                    else if (CompareNoCase (strlTokens[iIdx], "None"))
                    {
                        m_bAutoTagJump = false;
                        m_bAutoTagLoop = false;
                        m_iMaxTagLength = 0;
                    }
                    else
                    {
                        // Unrecognized argument
                        WriteOutputLine (string.Format ("Unrecognized AutoTag argument: {0:S}", strlTokens[iIdx]));
                        bShowHelp = true;
                    }
                }

                if (bShowHelp)
                {
                    ShowDasmAutoTagHelp ();
                    return true;
                }

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "DASM-ExtendedSet"))
            {
                // /DASM-ExtendedSet:{JC}{{,}{BC}{{,}MVX}{{,}{All} for expanding to BAL extended mnemonics
                //       If no arguments found, assume ALL
                if (strlTokens.Count < 2)
                {
                    // Missing argument, assume all
                    m_bExtendMnemonicBC  = true;
                    m_bExtendMnemonicJC  = true;
                    m_bExtendMnemonicMVX = true;

                    return true;
                }

                bool bShowHelp = false;
                for (int iIdx = 1; iIdx < strlTokens.Count; iIdx++)
                {
                    if (CompareNoCase (strlTokens[iIdx], "BC"))
                    {
                        m_bExtendMnemonicBC = true;
                    }
                    else if (CompareNoCase (strlTokens[iIdx], "JC"))
                    {
                        m_bExtendMnemonicJC = true;
                    }
                    else if (CompareNoCase (strlTokens[iIdx], "MVX"))
                    {
                        m_bExtendMnemonicMVX = true;
                    }
                    else if (CompareNoCase (strlTokens[iIdx], "All"))
                    {
                        m_bExtendMnemonicBC  = true;
                        m_bExtendMnemonicJC  = true;
                        m_bExtendMnemonicMVX = true;
                    }
                    else if (CompareNoCase (strlTokens[iIdx], "None"))
                    {
                        m_bExtendMnemonicBC  = false;
                        m_bExtendMnemonicJC  = false;
                        m_bExtendMnemonicMVX = false;
                    }
                    else
                    {
                        // Unrecognized argument
                        WriteOutputLine (string.Format ("Unrecognized DASM-ExtendedSet argument: {0:S}", strlTokens[iIdx]));
                        bShowHelp = true;
                    }
                }

                if (bShowHelp)
                {
                    ShowDasmExtendedSetHelp ();
                    return true;
                }

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "DASM-Smart"))
            {
                // /DASM-Smart:{SkipData}{{,}}
                //       SkipData means after unconditional jump/branch, treat disassembly as tentative unless
                //         uncondional branch/jump found, or all instructions until back in code range
                if (strlTokens.Count < 2)
                {
                    // Missing argument
                    ShowDasmSmartHelp ();

                    return true;
                }

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "DASM-Annotate"))
            {
                // /DASM-Annotate:{MultiLine}{{,}}{SIO}{{,}LIO}{{,}TIO}{{,}SNS}{{,}IO}{{,}A}{{,}L}{{,}LA}{{,}ST}{{,}REG}
                //       Add comments interpretting meaning of op code & Q byte
                //       IO means SIO, SNS, TIO, LIO
                //       REG means A, L, LA, ST
                //       MultiLine allows expanding comments to multiple lines as needed; first line is on instruction line,
                //         add blank lines with comments only as needed
                if (strlTokens.Count < 2)
                {
                    // Missing argument
                    ShowDasmAnnotateHelp ();

                    return true;
                }

                return true;
            }
            else if (CompareNoCase (strlTokens[0], "DASM-End"))
            {
                // /DASM-End signal end of command lines and command to perform disassembly
                PrintStringList (DisassembleCode (m_yaProgramImage));

                return true;
            }

            // Error: Unidentified command line
            WriteOutputLine ("");
            WriteOutputLine (string.Format ("* * *  Unrecognized command: {0:S} ... from command line:", strlTokens[0]));
            WriteOutputLine (strCommandLine);
            WriteOutputLine ("       For help use /? or /Help as command line in file or program command argument");
            WriteOutputLine ("");

            return true; // Return true: even if not a valid command line, it was still recognized as a command line.
                         // Returning true prevents the line from being treated as a data line.
        }

        public void ProcessCardLine (string strCardLine)
        {
            // Ignore all blank lines
            if (strCardLine.Length < 1)
            {
                return;
            }

            if (m_bReadExternalFile)
            {
                m_bReadExternalFile = false;

                char[] caTrimChars = { ' ', '\"' };
                strCardLine = strCardLine.TrimStart (caTrimChars);
                strCardLine = strCardLine.TrimEnd (caTrimChars);
                if (File.Exists (strCardLine))
                {
                    if (m_eCardProcessState == ECardProcessState.CARD_Binary)
                    {
                        m_yaProgramImage = File.ReadAllBytes (strCardLine);
                    }
                    else
                    {
                        List<string> strlInputFile = ReadFileToStringList (strCardLine, 96);

                        if (strlInputFile.Count > 0)
                        {
                            foreach (string str in strlInputFile)
                            {
                                InternalProcessCardLine (str);
                            }
                        }
                        else
                        {
                            WriteOutputLine ("File " + strCardLine + "is empty.");
                        }
                    }
                }
                else
                {
                    WriteOutputLine ("Unable to open file " + strCardLine + ".");
                }

                return;
            }
            else
            {
                InternalProcessCardLine (strCardLine);
            }
        }

        private void InternalProcessCardLine (string strCardLine)
        {
            if (m_eCardProcessState == ECardProcessState.CARD_IPL)
            {
                m_yaProgramImage = ReadIPLCard (strCardLine);
            }
            else if (m_eCardProcessState == ECardProcessState.CARD_Matrix)
            {
            }
            else if (m_eCardProcessState == ECardProcessState.CARD_PRNT)
            {
                if (m_bWrap96)
                {
                    List<string> strlPunchImage = CreatePunchImage (strCardLine, m_cHole, !m_bSingleSpace);
                    PrintStringList (strlPunchImage);

                    if (m_bShowTralier)
                    {
                        if (strCardLine.Length > 96)
                        {
                            WriteOutputLine (strCardLine.Substring (96));
                        }
                    }

                    if (m_bSeperator)
                    {
                        string strSeperator = new string ('=', 100);
                        WriteOutputLine (strSeperator);
                    }

                    if (m_iBlankLines > 0)
                    {
                        for (int iLine = 0; iLine < m_iBlankLines; iLine++)
                        {
                            WriteOutputLine ("");
                        }
                    }
                }
                else
                {
                }
            }
            else if (m_eCardProcessState == ECardProcessState.CARD_PrntGroup)
            {
                m_strlPatternCardGroup.Add (strCardLine);
            }
            else if (m_eCardProcessState == ECardProcessState.CARD_ListGroup)
            {
                WriteOutputLine (strCardLine);
            }
            else if (m_eCardProcessState == ECardProcessState.CARD_Text)
            {
                if (strCardLine[0] == 'T')
                {
                    if (m_bEndCardRead)
                    {
                        WriteOutputLine ("Error: Extra Text cards found after End card:");
                        WriteOutputLine ("strCardLine");
                    }
                    else
                    {
                        m_strlLoadedTextFile.Add (strCardLine);
                        m_bTextCardRead = true;
                    }
                }
                else if (strCardLine[0] == 'E')
                {
                    if (m_bTextCardRead)
                    {
                        if (m_bEndCardRead)
                        {
                            WriteOutputLine ("Error: Extra End card found:");
                            WriteOutputLine ("strCardLine");
                        }
                        else
                        {
                            m_strlLoadedTextFile.Add (strCardLine);
                            m_bEndCardRead = true;
                        }
                    }
                    else
                    {
                        WriteOutputLine ("Error: End card found before Text cards:");
                        WriteOutputLine ("strCardLine");
                    }
                }
                else
                {
                }
            }
            else if (m_eCardProcessState == ECardProcessState.CARD_Binary)
            {
            }
            else if (m_eCardProcessState == ECardProcessState.CARD_RelocatableText)
            {
            }
        }

        private void BackupAndClearListDCP (int iLowAddress, int iHighAddress)
        {
            DumpListDCP ();

            foreach (KeyValuePair<int, CDasmControlPoint> kvp in m_sdDCP)
            {
                m_sdBackupDCP[kvp.Key] = kvp.Value;
            }

            PrepForDASM (); // Also clears m_listDCP

            m_sdDCP[iLowAddress]  = new CDasmControlPoint (iLowAddress,  ENotationType.NOTE_Begin, "Low address");
            m_sdDCP[iHighAddress] = new CDasmControlPoint (iHighAddress, ENotationType.NOTE_End,   "High address");
            
            DumpListDCP ();
        }

        private void RestoreListDCP ()
        {
            DumpListDCP ();

            PrepForDASM (); // Also clears m_listDCP

            foreach (KeyValuePair<int, CDasmControlPoint> kvp in m_sdBackupDCP)
            {
                m_sdDCP[kvp.Key] = kvp.Value;
            }

            DumpListDCP ();
        }

        private void PrepForLoad ()
        {
            m_strlLoadedTextFile = new List<string> ();
            m_bEndCardRead  = false;
            m_bTextCardRead = false;
            m_iDasmStart    = -1;
            m_iDasmEnd      = -1;
            m_iDasmEntry    = -1;
            m_iDasmStartEC  = -1;
            m_iDasmEndEC    = -1;
            m_iDasmEntryEC  = -1;
            m_iMaxTagLength = m_bAutoTagJump || m_bAutoTagLoop ? 7 : 0; // Length of "Jump_00" & "Loop_00"
        }

        private void WriteEndCardTitleLine ()
        {
            if (m_strProgramName != null &&
                m_strProgramName.Length > 0)
            {
                WriteOutputLine (string.Format ("{0:S} end card instructions", m_strProgramName));
                WriteOutputLine ("");
            }
            else
            {
                WriteOutputLine (string.Format ("End card instructions"));
                WriteOutputLine ("");
            }
        }

        int InsertDasmControlPoint (List<string> strlTokens, ENotationType eNotationType)
        {
            string strNotation = "";
            int   iAddress     = -1;

            for (int iIdx = 1; iIdx < strlTokens.Count; iIdx++)
            {
                if (iIdx == 1)
                {
                    iAddress = StringToInt (strlTokens[iIdx], 16);
                }
                else if (iIdx == 2 &&
                         eNotationType != ENotationType.NOTE_Skip)
                {
                    strNotation = strlTokens[iIdx];
                }
            }

            return InsertDasmControlPoint (iAddress, eNotationType, strNotation);
        }

        int InsertDasmControlPoint (List<string> strlTokens, ENotationType eNotationType, string strCommandLine)
        {
            string strNotation = "";
            int   iAddress       = -1;

            for (int iIdx = 1; iIdx < strlTokens.Count; iIdx++)
            {
                if (iIdx == 1)
                {
                    iAddress = StringToInt (strlTokens[iIdx], 16);
                }
                else if (iIdx == 2)
                {
                    string strSearchKey = strlTokens[1] + ",";
                    int iSearchIdx = strCommandLine.IndexOf (strSearchKey) + strSearchKey.Length;
                    strNotation = strCommandLine.Substring (iSearchIdx);

                    if (strCommandLine.Length > iSearchIdx)
                    {
                        strNotation = strCommandLine.Substring (iSearchIdx);
                    }
                }
            }

            return InsertDasmControlPoint (iAddress, eNotationType, strNotation);
        }
    }
}