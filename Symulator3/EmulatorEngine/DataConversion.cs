using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EmulatorEngine
{
    public class CDumpData : CDataConversion
    {
        public void DisassembleData (byte[] yaMainMemory)
        {
            if (iStart  > 0 &&
                iLength > 0)
            {
                PrintStringList (DisassembleCode (yaMainMemory, iStart, iStart + iLength));
            }

            iStart  = 0;
            iLength = 0;
        }

        public int iStart  = 0;
        public int iLength = 0;
    }

    public partial class CDataConversion : CStringProcessor
    {
        #region Public Wrapper Methods
        public List<string> DumpIplLine (string strIplLine)
        {
            List<string> strlIplLines = new List<string> ();

            if (IsBlank (strIplLine, 33))
            {
                strlIplLines.Add (strIplLine);
            }
            else
            {
                strlIplLines.AddRange (BinaryToDump (StringToByteArray (strIplLine)));
            }

            return strlIplLines;
        }

        public List<string> DisassembleIplLine (string strIplLine)
        {
            List<string> strlIplLines = new List<string> ();

            if (IsBlank (strIplLine, 33))
            {
                strlIplLines.Add (strIplLine);
            }
            else
            {
                strlIplLines.AddRange (DisassembleCode (ReadIPLCard (strIplLine)));
            }

            return strlIplLines;
        }

        //public List<string> DumpTextLine (string strTextLine, bool bShowRawText, bool bShowCompressedCharacters)
        //{
        //    return DumpTextLine (strTextLine, bShowRawText, bShowCompressedCharacters, false);
        //}

        public List<string> DumpTextLine (string strTextLine, bool bShowRawText, bool bShowCompressedCharacters, bool bShowMoveDetails)
        {
            //          1         2         3         4         5         6         7         8         9         0
            // 1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
            // T  abcd    abcd    abcd    abcd    abcd    abcd    abcd    abcd    abcd    abcd    abcd
            //    012345  012345  012345  012345  012345  012345  012345  012345  012345  012345  012345
            //    . . .   . . .   . . .   . . .   . . .   . . .   . . .   . . .   . . .   . . .   . . .
            //
            //    abcd    abcd    abcd    abcd    abcd    abcd    abcd    abcd    abcd    abcd    abc   xxxxnnnn
            //    012345  012345  012345  012345  012345  012345  012345  012345  012345  012345  0123
            //    . . .   . . .   . . .   . . .   . . .   . . .   . . .   . . .   . . .   . . .   . .
            //
            //
            //          1         2         3         4         5         6         7         8         9         0
            // 1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
            // T  012345  012345  012345  012345  012345  012345  012345  012345  012345  012345  012345
            //    . . .   . . .   . . .   . . .   . . .   . . .   . . .   . . .   . . .   . . .   . . .
            //
            //    012345  012345  012345  012345  012345  012345  012345  012345  012345  012345  0123  xxxxnnnn
            //    . . .   . . .   . . .   . . .   . . .   . . .   . . .   . . .   . . .   . . .   . .
            //
            //          1         2         3         4         5         6         7         8         9         0
            // 1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
            // T abcd   abcd   abcd   abcd   abcd   abcd   abcd   abcd   abcd   abcd   abcd
            //   012345 012345 012345 012345 012345 012345 012345 012345 012345 012345 012345
            //   . . .  . . .  . . .  . . .  . . .  . . .  . . .  . . .  . . .  . . .  . . .
            //
            //   abcd   abcd   abcd   abcd   abcd   abcd   abcd   abcd   abcd   abcd   abc   xxxxnnnn
            //   012345 012345 012345 012345 012345 012345 012345 012345 012345 012345 0123
            //   . . .  . . .  . . .  . . .  . . .  . . .  . . .  . . .  . . .  . . .  . .
            //
            //
            //          1         2         3         4         5         6         7         8         9         0
            // 1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
            // Move nn bytes to: ssss - eeee
            // T 012345 012345 012345 012345 012345 012345 012345 012345 012345 012345 012345
            //   . . .  . . .  . . .  . . .  . . .  . . .  . . .  . . .  . . .  . . .  . . .
            //
            //   012345 012345 012345 012345 012345 012345 012345 012345 012345 012345 0123  xxxxnnnn
            //   . . .  . . .  . . .  . . .  . . .  . . .  . . .  . . .  . . .  . . .  . .
            //

            List<string> lstrDumpTextLines = new List<string> ();
            byte[] yaTextBinary = ReadTextCard (strTextLine);

            // 1. Optionally show move details
            if (bShowMoveDetails &&
                strTextLine[0] == 'T')
            {
                // Move nn bytes to: xxxx - xxxx
                StringBuilder strbldrMoveDetails = new StringBuilder (76);

                int iMoveLength  = ((int)yaTextBinary[0]) + 1,
                    iTargetEnd   = (((int)yaTextBinary[1]) * 0x0100) + (int)yaTextBinary[2],
                    iTargetStart = iTargetEnd - iMoveLength + 1;

                lstrDumpTextLines.Add (string.Format ("Move {0:D} bytes to: {1:X4} - {2:X4}", iMoveLength, iTargetStart, iTargetEnd));
            }

            // 2. Optionally list the characters - line 1
            if (bShowRawText)
            {
                StringBuilder strbldrRawText1 = new StringBuilder (76);

                strbldrRawText1.Append (strTextLine.Substring (0, 1));
                strbldrRawText1.Append (" ");

                for (int iIdx = 1; iIdx < 45; iIdx += 4)
                {
                    strbldrRawText1.Append (strTextLine.Substring (iIdx, 4));
                    if (iIdx < 41)
                    {
                        strbldrRawText1.Append ("   ");
                    }
                }

                lstrDumpTextLines.Add (strbldrRawText1.ToString ());
            }

            // 3. Dump compressed binary to hex - line 1
            {
                StringBuilder strbldrHexList1 = new StringBuilder (78);

                if (bShowRawText)
                {
                    strbldrHexList1.Append ("  ");
                }
                else
                {
                    strbldrHexList1.Append (strTextLine.Substring (0, 1));
                    strbldrHexList1.Append (" ");
                }

                for (int iIdx = 0; iIdx < 32; iIdx += 3)
                {
                    strbldrHexList1.Append (ByteToHexPair (yaTextBinary[iIdx]));
                    strbldrHexList1.Append (ByteToHexPair (yaTextBinary[iIdx + 1]));
                    strbldrHexList1.Append (ByteToHexPair (yaTextBinary[iIdx + 2]));
                    if (iIdx < 30)
                    {
                        strbldrHexList1.Append (" ");
                    }
                }

                lstrDumpTextLines.Add (strbldrHexList1.ToString ());
            }

            // 4. Optionally dump compressed characters - line 1
            if (bShowCompressedCharacters)
            {
                StringBuilder strbldrCompressedChar1 = new StringBuilder (76);

                strbldrCompressedChar1.Append ("  ");

                for (int iIdx = 0; iIdx < 33; iIdx++)
                {
                    strbldrCompressedChar1.Append (ConvertEbcdicToAsciiChar ((char)yaTextBinary[iIdx]));
                    if (iIdx < 32)
                    {
                        strbldrCompressedChar1.Append (" ");
                    }

                    if (iIdx > 0 &&
                        (iIdx + 1) % 3 == 0 &&
                         iIdx < 31)
                    {
                        strbldrCompressedChar1.Append (" ");
                    }
                }

                lstrDumpTextLines.Add (strbldrCompressedChar1.ToString ());
            }

            // Insert blank line
            lstrDumpTextLines.Add ("");

            // 5. Optionally list the characters - line 2
            if (bShowRawText)
            {
                StringBuilder strbldrRawText2 = new StringBuilder (76);

                strbldrRawText2.Append ("  ");

                for (int iIdx = 45; iIdx < 89; iIdx += 4)
                {
                    if (iIdx < 85)
                    {
                        strbldrRawText2.Append (strTextLine.Substring (iIdx, 4));
                    }
                    else
                    {
                        strbldrRawText2.Append (strTextLine.Substring (iIdx, 3));
                    }

                    if (iIdx < 85)
                    {
                        strbldrRawText2.Append ("   ");
                    }
                }

                strbldrRawText2.Append ("   ");
                strbldrRawText2.Append (strTextLine.Substring (88, 8));

                lstrDumpTextLines.Add (strbldrRawText2.ToString ());
            }

            // 6. Dump compressed binary to hex - line 2
            {
                StringBuilder strbldrHexList2 = new StringBuilder (76);

                strbldrHexList2.Append ("  ");

                for (int iIdx = 33; iIdx < 65; iIdx += 3)
                {
                    strbldrHexList2.Append (ByteToHexPair (yaTextBinary[iIdx]));
                    strbldrHexList2.Append (ByteToHexPair (yaTextBinary[iIdx + 1]));
                    if (iIdx < 63)
                    {
                        strbldrHexList2.Append (ByteToHexPair (yaTextBinary[iIdx + 2]));
                    }
                    if (iIdx < 63)
                    {
                        strbldrHexList2.Append (" ");
                    }
                }

                if (!bShowRawText)
                {
                    strbldrHexList2.Append ("  ");
                    strbldrHexList2.Append (strTextLine.Substring (88, 8));
                }

                lstrDumpTextLines.Add (strbldrHexList2.ToString ());
            }

            // 7. Optionally dump compressed characters - line 2
            if (bShowCompressedCharacters)
            {
                StringBuilder strbldrCompressedChar2 = new StringBuilder (76);

                strbldrCompressedChar2.Append ("  ");

                for (int iIdx = 33; iIdx < 65; iIdx++)
                {
                    strbldrCompressedChar2.Append (ConvertEbcdicToAsciiChar ((char)yaTextBinary[iIdx]));
                    if (iIdx < 64)
                    {
                        strbldrCompressedChar2.Append (" ");
                    }

                    if (iIdx > 0 &&
                        (iIdx + 1) % 3 == 0 &&
                         iIdx < 63)
                    {
                        strbldrCompressedChar2.Append (" ");
                    }
                }

                lstrDumpTextLines.Add (strbldrCompressedChar2.ToString ());
            }

            // 8. Optionally add separator line
            if (bShowRawText || bShowCompressedCharacters)
            {
                lstrDumpTextLines.Add ("");
            }

            return lstrDumpTextLines;
        }

        public List<string> DumpIplFile (string strIplFilename)
        {
            // Load file to byte array
            List<string> strlIplFile = ReadFileToStringList (strIplFilename, 96);
            List<string> strlIplLines = new List<string> ();

            // Dump byte array
            foreach (string strIplLine in strlIplFile)
            {
                if (strlIplLines.Count > 0)
                {
                    // Add a couple of separator lines
                    strlIplLines.Add ("");
                    strlIplLines.Add ("----------------------------------------------------------------------");
                    strlIplLines.Add ("");
                }

                strlIplLines.AddRange (DumpIplLine (strIplLine));
            }

            return strlIplLines;
        }

        public List<string> DisassembleIplFile (string strIplFilename)
        {
            PrepForDASM ();

            // Load file to byte array
            List<string> strlIplFile  = ReadFileToStringList (strIplFilename, 96);
            List<string> strlIplLines = new List<string> ();

            // Dump byte array
            foreach (string strIplLine in strlIplFile)
            {
                if (strlIplLines.Count > 0)
                {
                    // Add a couple of separator lines
                    strlIplLines.Add ("");
                    strlIplLines.Add ("----------------------------------------------------------------------");
                    strlIplLines.Add ("");
                }

                strlIplLines.AddRange (DisassembleIplLine (strIplLine));
            }

            return strlIplLines;
        }

        public List<string> DumpTextFileLines (string strFilename, bool bShowRawText, bool bShowCompressedCharacters, bool bShowMoveDetails)
        {
            // Load file to string list
            List<string> strlTextFile = ReadFileToStringList (strFilename, 96);

            return DumpTextFileLines (strlTextFile, bShowRawText, bShowCompressedCharacters, bShowMoveDetails);
        }

        public List<string> DumpTextFileLines (List<string> strlTextFile, bool bShowRawText, bool bShowCompressedCharacters, bool bShowMoveDetails)
        {
            List<string> strlTextDump = new List<string> ();

            // Dump byte array
            if (strlTextFile.Count > 0)
            {
                foreach (string str in strlTextFile)
                {
                    if (str[0] == 'T' ||
                        str[0] == 'E')
                    {
                        List<string> strlTextCard = DumpTextLine (str, bShowRawText, bShowCompressedCharacters, bShowMoveDetails);

                        if (strlTextDump.Count > 0)
                        {
                            strlTextDump.Add ("");
                        }

                        if (strlTextCard.Count > 0)
                        {
                            strlTextDump.AddRange (strlTextCard);
                        }
                    }
                }
            }

            return strlTextDump;
        }

        public List<string> DumpTextFileImage (string strFilename)
        {
            // Load file to byte array
            LoadTextFile (strFilename);

            // Dump byte array
            return BinaryToDump (m_ylMemoryImage, m_iLowAddressMI, m_iHighAddressMI);
        }

        public List<string> DisassembleTextFile (string strFilename)
        {
            PrepForDASM ();

            // Load file to byte array
            LoadTextFile (strFilename);
            m_bDumpComplete = false;

            // Disassemble end card
            List<string> strlDisassembleTextFile = new List<string> ();
            strlDisassembleTextFile.Add ("End Card Instructions");
            strlDisassembleTextFile.AddRange (DisassembleCode (m_yaEndCard, m_iLowAddressEC, m_iHighAddressEC));
            strlDisassembleTextFile.Add ("");
            strlDisassembleTextFile.Add ("");

            // Disassemble byte array
            strlDisassembleTextFile.Add ("Program Body Disassembly");
            strlDisassembleTextFile.AddRange (DisassembleCode (m_ylMemoryImage.ToArray (), m_iLowAddressMI, m_iHighAddressMI));

            return strlDisassembleTextFile;
        }

        public List<byte> LoadTextFile (string strFilename)
        {
            // Load file to string list
            List<string> strlTextFile = ReadFileToStringList (strFilename, 96);

            return LoadTextFile (strlTextFile);
        }

        public List<byte> LoadTextFile (List<string> strlTextFile)
        {
            // Load file to byte array
            if (strlTextFile.Count > 0)
            {
                // Initialize buffers
                m_ylMemoryImage = new List<byte> (m_kiInitialImageSize); // Limit to 8k until there's a reason to expand
                for (int iIdx = 0; iIdx < m_ylMemoryImage.Capacity; iIdx++)
                {
                    m_ylMemoryImage.Add (0x00);
                }
                m_yaEndCard = new byte[m_kiEndCardImageSize];

                // Initialize boundary markers
                m_iLowAddressMI  = m_kiInitialImageSize;
                m_iHighAddressMI = 0;
                m_iLowAddressEC  = m_kiEndCardImageSize;
                m_iHighAddressEC = 0;

                foreach (string str in strlTextFile)
                {
                    if (str[0] == 'T')
                    {
                        byte[] yaTextBinary = ReadTextCard (str);
                        //PrintStringList (BinaryToDump (yaTextBinary));

                        for (int iIdx = yaTextBinary[0] + 1, iTargetIdx = yaTextBinary[1] * 0x0100 + yaTextBinary[2];
                             iIdx > 0;
                             iIdx--, iTargetIdx--)
                        {
                            m_ylMemoryImage[iTargetIdx] = yaTextBinary[iIdx + 2];

                            if (iTargetIdx < m_iLowAddressMI)
                            {
                                m_iLowAddressMI = iTargetIdx;
                            }

                            if (iTargetIdx > m_iHighAddressMI)
                            {
                                m_iHighAddressMI = iTargetIdx;
                            }
                        }

                        //PrintStringList (BinaryToDump (m_laMemoryImage, m_iLowAddressMI, m_iHighAddressMI));
                    }
                    else if (str[0] == 'E')
                    {
                        byte[] yaTextBinary = ReadTextCard (str);
                        //PrintStringList (BinaryToDump (yaTextBinary));

                        for (int iIdx = 0x02, iTargetIdx = 0x19;
                             iIdx < 0x41;
                             iIdx++, iTargetIdx++)
                        {
                            m_yaEndCard[iTargetIdx] = yaTextBinary[iIdx];

                            if (iTargetIdx < m_iLowAddressEC)
                            {
                                m_iLowAddressEC = iTargetIdx;
                            }

                            //if (iTargetIdx > m_iHighAddressEC && m_yaEndCard[iTargetIdx] > 0x00)
                            if (iTargetIdx > m_iHighAddressEC)
                            {
                                m_iHighAddressEC = iTargetIdx;
                            }

                            //PrintStringList (BinaryToDump (m_yaEndCard, m_iLowAddressEC, m_iHighAddressEC));
                        }
                    }
                }
            }

            m_sdDCP[m_iLowAddressMI]  = new CDasmControlPoint (m_iLowAddressMI,  ENotationType.NOTE_Begin, "Low address");
            m_sdDCP[m_iHighAddressMI] = new CDasmControlPoint (m_iHighAddressMI, ENotationType.NOTE_End,   "High address");

            DumpListDCP ();

            return m_ylMemoryImage;
        }
        #endregion

        // Text Card Loading member data
        protected const int m_kiInitialImageSize = 8192;
        protected List<byte> m_ylMemoryImage;
        protected int m_iLowAddressMI;
        protected int m_iHighAddressMI;

        protected const int m_kiEndCardImageSize = 0x100;
        protected byte[] m_yaEndCard;
        protected int m_iLowAddressEC;
        protected int m_iHighAddressEC;

        // Data Conversion member data
        #region Data Conversion Tables
        //   !  is  0x3F  s/b 0x2A
        //   |  is  0x1A  s/b 0x3F
        //   ~  is  0x11  s/b 0x3A
        //Char: ! ASCII 21 -> EBCDIC 4F -> ASCII 7C
        //Char: ~ ASCII 7E -> EBCDIC A1
        //Char: | ASCII 7C -> EBCDIC 6A
        //                                    x0    x1    x2    x3    x4    x5    x6    x7    x8    x9    xA    xB    xC    xD    xE    xF
        byte[] m_yaASCIIinEBCDICsequence = { 0x00, 0x01, 0x02, 0x03, 0x9C, 0x09, 0x86, 0x7F, 0x97, 0x8D, 0x8E, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,   // 0x
                                             0x10, 0x11, 0x12, 0x13, 0x9D, 0x85, 0x08, 0x87, 0x18, 0x19, 0x92, 0x8F, 0x1C, 0x1D, 0x1E, 0x1F,   // 1x
                                             0x80, 0x81, 0x82, 0x83, 0x84, 0x0A, 0x17, 0x1B, 0x88, 0x89, 0x8A, 0x8B, 0x8C, 0x05, 0x06, 0x07,   // 2x
                                             0x90, 0x91, 0x16, 0x93, 0x94, 0x95, 0x96, 0x04, 0x98, 0x99, 0x9A, 0x9B, 0x14, 0x15, 0x9E, 0x1A,   // 3x
                                             0x20, 0xA0, 0xA1, 0xA2, 0xA3, 0xA4, 0xA5, 0xA6, 0xA7, 0xA8, 0x5B, 0x2E, 0x3C, 0x28, 0x2B, 0x21,   // 4x
                                             0x26, 0xA9, 0xAA, 0xAB, 0xAC, 0xAD, 0xAE, 0xAF, 0xB0, 0xB1, 0x5D, 0x24, 0x2A, 0x29, 0x3B, 0x5E,   // 5x
                                             0x2D, 0x2F, 0xB2, 0xB3, 0xB4, 0xB5, 0xB6, 0xB7, 0xB8, 0xB9, 0x7C, 0x2C, 0x25, 0x5F, 0x3E, 0x3F,   // 6x
                                             0xBA, 0xBB, 0xBC, 0xBD, 0xBE, 0xBF, 0xC0, 0xC1, 0xC2, 0x60, 0x3A, 0x23, 0x40, 0x27, 0x3D, 0x22,   // 7x
                                             0xC3, 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0xC4, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9,   // 8x
                                             0xCA, 0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F, 0x70, 0x71, 0x72, 0xCB, 0xCC, 0xCD, 0xCE, 0xCF, 0xD0,   // 9x
                                             0xD1, 0x7E, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7,   // Ax
                                             0xD8, 0xD9, 0xDA, 0xDB, 0xDC, 0xDD, 0xDE, 0xDF, 0xE0, 0xE1, 0xE2, 0xE3, 0xE4, 0xE5, 0xE6, 0xE7,   // Bx
                                             0x7B, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0xE8, 0xE9, 0xEA, 0xEB, 0xEC, 0xED,   // Cx
                                             0x7D, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50, 0x51, 0x52, 0xEE, 0xEF, 0xF0, 0xF1, 0xF2, 0xF3,   // Dx
                                             0x5C, 0x9F, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8, 0xF9,   // Ex
                                             0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0xFA, 0xFB, 0xFC, 0xFD, 0xFE, 0xFF }; // Fx

        //                                    x0    x1    x2    x3    x4    x5    x6    x7    x8    x9    xA    xB    xC    xD    xE    xF
        byte[] m_yaEBCDICinASCIIsequence = { 0x00, 0x01, 0x02, 0x03, 0x37, 0x2D, 0x2E, 0x2F, 0x16, 0x05, 0x25, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,   // 0x
                                             0x10, 0x11, 0x12, 0x13, 0x3C, 0x3D, 0x32, 0x26, 0x18, 0x19, 0x3F, 0x27, 0x1C, 0x1D, 0x1E, 0x1F,   // 1x
                                             0x40, 0x4F, 0x7F, 0x7B, 0x5B, 0x6C, 0x50, 0x7D, 0x4D, 0x5D, 0x5C, 0x4E, 0x6B, 0x60, 0x4B, 0x61,   // 2x
                                             0xF0, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8, 0xF9, 0x7A, 0x5E, 0x4C, 0x7E, 0x6E, 0x6F,   // 3x
                                             0x7C, 0xC1, 0xC2, 0xC3, 0xC4, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9, 0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6,   // 4x
                                             0xD7, 0xD8, 0xD9, 0xE2, 0xE3, 0xE4, 0xE5, 0xE6, 0xE7, 0xE8, 0xE9, 0x4A, 0xE0, 0x5A, 0x5F, 0x6D,   // 5x
                                             0x79, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x91, 0x92, 0x93, 0x94, 0x95, 0x96,   // 6x
                                             0x97, 0x98, 0x99, 0xA2, 0xA3, 0xA4, 0xA5, 0xA6, 0xA7, 0xA8, 0xA9, 0xC0, 0x6A, 0xD0, 0xA1, 0x07,   // 7x
                                             0x20, 0x21, 0x22, 0x23, 0x24, 0x15, 0x06, 0x17, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x09, 0x0A, 0x1B,   // 8x
                                             0x30, 0x31, 0x1A, 0x33, 0x34, 0x35, 0x36, 0x08, 0x38, 0x39, 0x3A, 0x3B, 0x04, 0x14, 0x3E, 0xE1,   // 9x
                                             0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57,   // Ax
                                             0x58, 0x59, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x70, 0x71, 0x72, 0x73, 0x74, 0x75,   // Bx
                                             0x76, 0x77, 0x78, 0x80, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E, 0x8F, 0x90, 0x9A, 0x9B, 0x9C, 0x9D, 0x9E,   // Cx
                                             0x9F, 0xA0, 0xAA, 0xAB, 0xAC, 0xAD, 0xAE, 0xAF, 0xB0, 0xB1, 0xB2, 0xB3, 0xB4, 0xB5, 0xB6, 0xB7,   // Dx
                                             0xB8, 0xB9, 0xBA, 0xBB, 0xBC, 0xBD, 0xBE, 0xBF, 0xCA, 0xCB, 0xCC, 0xCD, 0xCE, 0xCF, 0xDA, 0xDB,   // Ex
                                             0xDC, 0xDD, 0xDE, 0xDF, 0xEA, 0xEB, 0xEC, 0xED, 0xEE, 0xEF, 0xFA, 0xFB, 0xFC, 0xFD, 0xFE, 0xFF }; // Fx
        #endregion

        // Ipl Reader member data
        //   1 2 3 4 5 6 7 8 9 : # @ ' = " 0 / S T U V W X Y Z & , % _ > ? - J K L M N O P Q R ! $ * ) ; ^ } A B C D E F G H I ~ . < ( + |
        // 40F1F2F3F4F5F6F7F8F97A7B7C7D7E7FF061E2E3E4E5E6E7E8E9506B6C6D6E6F60D1D2D3D4D5D6D7D8D95A5B5C5D5E5FD0C1C2C3C4C5C6C7C8C94A4B4C4D4E4F
        #region BCD -> IPL code conversion tables
        string m_strTopTierCharacters = " 123456789:#@'=\"0/STUVWXYZ&,%_>?-JKLMNOPQR!$*);^}ABCDEFGHI~.<(+|";
        //                                123456789:#@'=\"0/STUVWXYZ&,%_>?-JKLMNOPQR]$*);^}ABCDEFGHI[.<(+!
        //                                                                          ^               ^    ^

        byte[] m_yaTopTiers00 = { 0x40, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7,
//                            0000: 40    F1    F2    F3    F4    F5    F6    F7
                                  0xF8, 0xF9, 0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F,
//                                  F8    F9    7A    7B    7C    7D    7E    7F
                                  0xF0, 0x61, 0xE2, 0xE3, 0xE4, 0xE5, 0xE6, 0xE7,
//                            0010: F0    61    E2    E3    E4    E5    E6    E7
                                  0xE8, 0xE9, 0x50, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F,
//                                  E8    E9    50    6B    6C    6D    6E    6F
                                  0x60, 0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7,
//                            0020: 60    D1    D2    D3    D4    D5    D6    D7
                                  0xD8, 0xD9, 0x5A, 0x5B, 0x5C, 0x5D, 0x5E, 0x5F,
//                                  D8    D9    5A    5B    5C    5D    5E    5F
                                  0xD0, 0xC1, 0xC2, 0xC3, 0xC4, 0xC5, 0xC6, 0xC7,
//                            0030: D0    C1    C2    C3    C4    C5    C6    C7
                                  0xC8, 0xC9, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F };
        //                                  C8    C9    4A    4B    4C    4D    4E    4F
        byte[] m_yaTopTiers01 = { 0x00, 0xB1, 0xB2, 0xB3, 0xB4, 0xB5, 0xB6, 0xB7,
//                            0000: 00    B1    B2    B3    B4    B5    B6    B7
                                  0xB8, 0xB9, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F,
//                                  B8    B9    3A    3B    3C    3D    3E    3F
                                  0xB0, 0x21, 0xA2, 0xA3, 0xA4, 0xA5, 0xA6, 0xA7,
//                            0010: B0    21    A2    A3    A4    A5    A6    A7
                                  0xA8, 0xA9, 0x10, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F,
//                                  A8    A9    10    2B    2C    2D    2E    2F
                                  0x20, 0x91, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97,
//                            0020: 20    91    92    93    94    95    96    97
                                  0x98, 0x99, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F,
//                                  98    99    1A    1B    1C    1D    1E    1F
                                  0x90, 0x81, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87,
//                            0030: 90    81    82    83    84    85    86    87
                                  0x88, 0x89, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
        //                                  88    89    0A    0B    0C    0D    0E    0F
        byte[] m_yaTopTiers02 = { 0xC0, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77,
//                            0000: C0    71    72    73    74    75    76    77
                                  0x78, 0x79, 0xFA, 0xFB, 0xFC, 0xFD, 0xFE, 0xFF,
//                                  78    79    FA    FB    FC    FD    FE    FF
                                  0x70, 0xE1, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67,
//                            0010: 70    E1    62    63    64    65    66    67
                                  0x68, 0x69, 0xEA, 0xEB, 0xEC, 0xED, 0xEE, 0xEF,
//                                  68    69    EA    EB    EC    ED    EE    EF
                                  0xE0, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57,
//                            0020: E0    51    52    53    54    55    56    57
                                  0x58, 0x59, 0xDA, 0xDB, 0xDC, 0xDD, 0xDE, 0xDF,
//                                  58    59    DA    DB    DC    DD    DE    DF
                                  0x6A, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47,
//                            0030: 6A    41    42    43    44    45    46    47
                                  0x48, 0x49, 0xCA, 0xCB, 0xCC, 0xCD, 0xCE, 0xCF };
        //                                  48    49    CA    CB    CC    CD    CE    CF
        byte[] m_yaTopTiers03 = { 0x80, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,
//                            0000: 80    31    32    33    34    35    36    37
                                  0x38, 0x39, 0xBA, 0xBB, 0xBC, 0xBD, 0xBE, 0xBF,
//                                  38    39    BA    BB    BC    BD    BE    BF
                                  0x30, 0xA1, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27,
//                            0010: 30    A1    22    23    24    25    26    27
                                  0x28, 0x29, 0xAA, 0xAB, 0xAC, 0xAD, 0xAE, 0xAF,
//                                  28    29    AA    AB    AC    AD    AE    AF
                                  0xA0, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17,
//                            0020: A0    11    12    13    14    15    16    17
                                  0x18, 0x19, 0x9A, 0x9B, 0x9C, 0x9D, 0x9E, 0x9F,
//                                  18    19    9A    9B    9C    9D    9E    9F
                                  0x2A, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07,
//                            0030: 2A    01    02    03    04    05    06    07
                                  0x08, 0x09, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E, 0x8F };
//                                  08    09    8A    8B    8C    8D    8E    8F
        #endregion

        #region Data Conversion Engine
        public byte[] PackHexToBinary (string strHexText)
        {
            byte[] yaBinary = new byte[strHexText.Length / 2];

            for (int iSourceIdx = 0, iTargetIdx = 0;
                 iSourceIdx < strHexText.Length;
                 iSourceIdx += 2, iTargetIdx += 1)
            {
                byte yHighOrder = CharacterToBinary ((byte)strHexText[iSourceIdx]);
                byte yLowOrder = CharacterToBinary ((byte)strHexText[iSourceIdx + 1]);
                yHighOrder <<= 4;
                yaBinary[iTargetIdx] = (byte)(yHighOrder + yLowOrder);
            }

            return yaBinary;
        }

        private byte CharacterToBinary (byte yInput)
        {
            if (yInput <= '9')
            {
                return (byte)(yInput - '0');
            }
            else if (yInput >= 'A' &&
                     yInput <= 'F')
            {
                return (byte)(yInput - 'A' + 0x0A);
            }
            else if (yInput >= 'a' &&
                     yInput <= 'f')
            {
                return (byte)(yInput - 'a' + 0x0A);
            }
            else
            {
                return 0;
            }
        }

        public byte ConvertASCIItoEBCDIC (byte yAsciiValue)
        {
            return m_yaEBCDICinASCIIsequence[yAsciiValue];
        }

        public byte ConvertEBCDICtoASCII (byte yEbcdicValue)
        {
            return m_yaASCIIinEBCDICsequence[yEbcdicValue];
        }

        public string ConvertASCIIstringToEBCDIC (string strAsciiValue)
        {
            StringBuilder strbldtEBCDIC = new StringBuilder (strAsciiValue.Length);

            foreach (char cASCII in strAsciiValue)
            {
                //strbldtEBCDIC.Append ((char)m_yaEBCDICinASCIIsequence[(byte)cASCII]);
                strbldtEBCDIC.Append (ConvertTopTierCharacter (cASCII));
            }

            return strbldtEBCDIC.ToString ();
        }

        public string ConvertASCIIstringToEBCDIC (byte[] yaAsciiValue)
        {
            StringBuilder strbldtEBCDIC = new StringBuilder (yaAsciiValue.Length);

            foreach (byte yASCII in yaAsciiValue)
            {
                //strbldtEBCDIC.Append ((char)m_yaEBCDICinASCIIsequence[yASCII]);
                strbldtEBCDIC.Append (ConvertTopTierCharacter ((char)yASCII));
            }

            return strbldtEBCDIC.ToString ();
        }

        public string ConvertEBCDICtoASCIIstring (byte[] yaEbcdicValue)
        {
            StringBuilder strbldtASCII = new StringBuilder (yaEbcdicValue.Length);

            foreach (byte yEBCDIC in yaEbcdicValue)
            {
                strbldtASCII.Append ((char)m_yaASCIIinEBCDICsequence[yEBCDIC]);
            }

            return strbldtASCII.ToString ();
        }
        #endregion

        #region IPL & Text Card Reader Engine
        public byte[] ReadIPLCard (string strIplCard)
        {
            byte[] yaIplBinary = new byte[96];

            if (strIplCard.Length < 1)
            {
                return yaIplBinary;
            }

            // Set up working copy of card image
            string strIplCardImage = strIplCard;
            if (strIplCardImage.Length < 96)
            {
                strIplCardImage += new string (' ', 96 - strIplCardImage.Length);
            }
            else if (strIplCardImage.Length > 96)
            {
                strIplCardImage = strIplCardImage.Substring (0, 96);
            }

            // First convert the top two punch tiers
            for (int iIdx = 0; iIdx < 64; ++iIdx)
            {
                // First, convert 3rd tier punch codes
                int iTier3PunchCode = CharacterToPunchCode (strIplCardImage[(iIdx % 32) + 64]);
                if (iIdx < 32)
                {
                    iTier3PunchCode &= 0x0C;
                    iTier3PunchCode >>= 2;
                }
                else
                {
                    iTier3PunchCode &= 0x03;
                }

                // Then iterate through top 2 punch tiers
                int iCharIdx = FindTopTierCharacter (strIplCardImage[iIdx]);
                switch (iTier3PunchCode)
                {
                    case 0x00:
                        {
                            yaIplBinary[iIdx] = m_yaTopTiers00[iCharIdx];
                            break;
                        }

                    case 0x01:
                        {
                            yaIplBinary[iIdx] = m_yaTopTiers01[iCharIdx];
                            break;
                        }

                    case 0x02:
                        {
                            yaIplBinary[iIdx] = m_yaTopTiers02[iCharIdx];
                            break;
                        }

                    case 0x03:
                        {
                            yaIplBinary[iIdx] = m_yaTopTiers03[iCharIdx];
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }
            }

            // Then convert the 3rd punch tier
            for (int iIdx = 64; iIdx < 96; ++iIdx)
            {
                int iCharIdx = FindTopTierCharacter (strIplCardImage[iIdx]);
                yaIplBinary[iIdx] = m_yaTopTiers00[iCharIdx];
            }

            return yaIplBinary;
        }

        public byte[] ReadTextCard (string strTextCard)
        {
            byte[] yaTextBinary = new byte[65];

            if (strTextCard.Length < 1)
            {
                return yaTextBinary;
            }

            // Set up working copy of card image
            string strTextCardImage = strTextCard;
            if (strTextCardImage.Length < 88)
            {
                strTextCardImage += new string (' ', 89 - strTextCardImage.Length);
            }
            else if (strTextCardImage.Length > 88)
            {
                // Ensure padding  with at least one space
                strTextCardImage = strTextCardImage.Substring (0, 88);
                strTextCardImage += new string (' ', 1);
            }

            // Create char array of EBCDIC text, since EBCDIC values are expected for compression
            byte[] yaUncompressedText = new byte[84];
            for (int iIdx = 0; iIdx < 84; ++iIdx)
            {
                //yaUncompressedText[iIdx] = ConvertASCIItoEBCDIC ((byte)strTextCardImage[iIdx + 1]);
                int iCharIdx = FindTopTierCharacter (strTextCardImage[iIdx + 1]);
                yaUncompressedText[iIdx] = m_yaTopTiers00[iCharIdx]; // EBCDIC value table
            }

            byte[] yaWorkBuffer = new byte[4];

            // Replace 0xD0 with 0x2A
            for (int iIdx = 0; iIdx < yaUncompressedText.Length; iIdx++)
            {
                //WriteOutputLine (string.Format ("[{0:D2}] {1:X2} {2:S1}", iIdx, yaUncompressedText[iIdx], (char)yaUncompressedText[iIdx]));
                if (yaUncompressedText[iIdx] == (byte)0xD0)
                {
                    yaUncompressedText[iIdx] = (byte)0x2A;
                }
            }
            // Now, to the compression
            for (int iTextIdx = 0; iTextIdx < yaUncompressedText.Length; iTextIdx += 4)
            {
                // Fill the compression buffer
                for (int iSourceIdx = iTextIdx, iTargetIdx = 0;
                    iTargetIdx < 4;
                    ++iSourceIdx, ++iTargetIdx)
                {
                    yaWorkBuffer[iTargetIdx] = yaUncompressedText[iSourceIdx];
                }

                // Now do the compression ...
                for (int iWorkIdx = 0; iWorkIdx < 4; ++iWorkIdx)
                {
                    // First, shift all bytes left 2 bits
                    for (int iBufferIdx = iWorkIdx; iBufferIdx < 4; ++iBufferIdx)
                    {
                        yaWorkBuffer[iBufferIdx] <<= 2;
                    }

                    // Then, wrap the high-order bits to the byte to the left
                    for (int iBufferIdx = iWorkIdx; iBufferIdx < 3; ++iBufferIdx)
                    {
                        //byte yBits = yaWorkBuffer[iBufferIdx + 1];
                        //yBits >>= 6;
                        //yaWorkBuffer[iBufferIdx] |= yBits;
                        yaWorkBuffer[iBufferIdx] |= (byte)((int)(yaWorkBuffer[iBufferIdx + 1]) >> 6);
                    }
                }

                // Finally, store the compressed binary data
                //             1            2
                // 0123 4567 8901 2345 6789 0123
                // 012  345  678  901
                for (int iIdx = 0, iTargetIdx = iTextIdx - iTextIdx / 4;
                    iIdx < 3 && iTargetIdx < yaTextBinary.Length; ++iIdx, ++iTargetIdx)
                {
                    yaTextBinary[iTargetIdx] = yaWorkBuffer[iIdx];
                }
            }

            return yaTextBinary;
        }

        protected char ConvertTopTierCharacter (char cAscii)
        {
            int iIdx = FindTopTierCharacter (cAscii);

            return (iIdx != -1) ? (char)m_yaTopTiers00[iIdx] : ' ';
        }

        private int FindTopTierCharacter (char cSearch)
        {
            for (int iIdx = 0; iIdx < m_strTopTierCharacters.Length; ++iIdx)
            {
                if (m_strTopTierCharacters[iIdx] == cSearch)
                {
                    return iIdx;
                }
            }

            return -1; // Unable to find character match
        }

        //string m_strTopTierCharacters = " 123456789:#@'=\"0/STUVWXYZ&,%_>?-JKLMNOPQR!$*);^}ABCDEFGHI~.<(+|";
        //byte[] m_yaTopTiers00 = { 0x40, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7,
        public char ConvertEbcdicToAsciiChar (char cSearch)
        {
            for (int iIdx = 0; iIdx < m_yaTopTiers00.Length; ++iIdx)
            {
                if (m_yaTopTiers00[iIdx] == (byte)cSearch)
                {
                    return m_strTopTierCharacters[iIdx];
                }
            }

            return 'x'; // Unable to find character match
        }

        public string ConvertEbcdicToAscii (byte[] yaEbcdic)
        {
            StringBuilder strbldrAscii = new StringBuilder (yaEbcdic.Length);
            foreach (byte yEbcdic in yaEbcdic)
            {
                strbldrAscii.Append (ConvertEbcdicToAsciiChar ((char)yEbcdic));
            }
            return strbldrAscii.ToString ();
        }
        #endregion
    }
}