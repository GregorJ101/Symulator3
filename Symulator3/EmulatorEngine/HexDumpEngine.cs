using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EmulatorEngine
{
    // Hex and Character Dump Engine
    public partial class CDataConversion : CStringProcessor
    {
        //          1         2         3         4         5         6         7         8         9         0
        // 1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
        //                                                            ASCII               EBCDIC
        // xxxx: xx xx xx xx  xx xx xx xx  xx xx xx xx  xx xx xx xx   <................>  <................>
        //public List<string> BinaryToDump (char[] caData)
        //{
        //    byte[] yaData = new byte[caData.Length];

        //    for (int iIdx = 0; iIdx < caData.Length; iIdx++)
        //    {
        //        yaData[iIdx] = (byte)caData[iIdx];
        //    }

        //    return BinaryToDump (yaData, 0, yaData.Length - 1);
        //}

        public List<string> BinaryToDump (byte[] yaData)
        {
            return BinaryToDump (yaData, 0, yaData.Length - 1);
        }

        public List<string> BinaryToDump (List<byte> ylData, int iStartAddress, int iEndAddress)
        {
            return BinaryToDump (ylData.ToArray (), iStartAddress, iEndAddress);
        }

        public List<string> BinaryToDump (byte[] yaData, int iStartAddress, int iEndAddress)
        {
            List<string> lstrHexDump = new List<string> ();

            int iHexDumpEnd    = 58,
                iAsciiDumpEnd  = 78,
                iEbcdicDumpEnd = 101;

            for (int iIdx = iStartAddress; iIdx <= iEndAddress; iIdx += 16)
            {
                StringBuilder strbldrDumpLine = new StringBuilder (98);

                // Print address
                strbldrDumpLine.Append (string.Format ("{0:X4}: ", iIdx));

                // Print hex characters
                for (int iLineIdx = iIdx;
                     iLineIdx - iIdx < 16 && iLineIdx < yaData.Length && iLineIdx <= iEndAddress;
                     iLineIdx++)
                {
                    strbldrDumpLine.Append (ByteToHexPair (yaData[iLineIdx]));
                    strbldrDumpLine.Append (" ");

                    // Handle spacing
                    int iCharOffset = iLineIdx - iIdx + 1;
                    if (iCharOffset % 4 == 0)
                    {
                        strbldrDumpLine.Append (" ");
                    }
                    //if (iCharOffset % 8 == 0)
                    //{
                    //    strbldrDumpLine.Append (" ");
                    //}
                }

                // Pad string
                if (strbldrDumpLine.Length < iHexDumpEnd)
                {
                    strbldrDumpLine.Append (new string (' ', iHexDumpEnd - strbldrDumpLine.Length));
                }

                // Print ASCII characters
                strbldrDumpLine.Append ("<");
                for (int iLineIdx = iIdx;
                     iLineIdx - iIdx < 16 && iLineIdx < yaData.Length && iLineIdx <= iEndAddress;
                     iLineIdx++)
                {
                    char cData = (char)yaData[iLineIdx];
                    if (cData >= (char)0x20 &&
                        cData <= (char)0x7F)
                    {
                        strbldrDumpLine.Append (cData);
                    }
                    else
                    {
                        strbldrDumpLine.Append (".");
                    }

                    if //(iLineIdx - iIdx > 0 &&
                        ((iLineIdx - iIdx + 1) % 4 == 0 &&
                          iLineIdx - iIdx < 15)
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
                for (int iLineIdx = iIdx;
                     iLineIdx - iIdx < 16 && iLineIdx < yaData.Length && iLineIdx <= iEndAddress;
                     iLineIdx++)
                {
                    char cData = (char)ConvertEBCDICtoASCII (yaData[iLineIdx]);
                    if (cData >= (char)0x20 &&
                        cData <= (char)0x7F)
                    {
                        strbldrDumpLine.Append (cData);
                    }
                    else
                    {
                        strbldrDumpLine.Append (".");
                    }

                    if //(iLineIdx - iIdx > 0 &&
                        ((iLineIdx - iIdx + 1) % 4 == 0&&
                          iLineIdx - iIdx < 15)
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

                // Add line to list
                lstrHexDump.Add (strbldrDumpLine.ToString ());
            }

            return lstrHexDump;
        }

        private List<string> BinaryToDumpOld (byte[] yaData)
        {
            List<string> lstrHexDump = new List<string> ();

            for (int iIdx = 0; iIdx < yaData.Length; iIdx += 16)
            {
                StringBuilder strbldrDumpLine = new StringBuilder (98);

                // Print address
                strbldrDumpLine.Append (string.Format ("{0:X4}: ", iIdx));

                // Print hex characters
                for (int iLineIdx = iIdx; iLineIdx - iIdx < 16 && iLineIdx < yaData.Length; ++iLineIdx)
                {
                    strbldrDumpLine.Append (ByteToHexPair (yaData[iLineIdx]));
                    strbldrDumpLine.Append (" ");

                    // Handle spacing
                    int iCharOffset = iLineIdx - iIdx + 1;
                    if (iCharOffset % 4 == 0)
                    {
                        strbldrDumpLine.Append (" ");
                    }
                    if (iCharOffset % 8 == 0)
                    {
                        strbldrDumpLine.Append (" ");
                    }
                }

                // Pad string
                if (strbldrDumpLine.Length < 60)
                {
                    strbldrDumpLine.Append (new string (' ', 60 - strbldrDumpLine.Length));
                }

                // Print ASCII characters
                strbldrDumpLine.Append ("<");
                for (int iLineIdx = iIdx; iLineIdx - iIdx < 16 && iLineIdx < yaData.Length; ++iLineIdx)
                {
                    char cData = (char)yaData[iLineIdx];
                    if (cData >= (char)0x20 &&
                        cData <= (char)0x7F)
                    {
                        strbldrDumpLine.Append (cData);
                    }
                    else
                    {
                        strbldrDumpLine.Append (".");
                    }
                }
                // Pad string
                if (strbldrDumpLine.Length < 77)
                {
                    strbldrDumpLine.Append (new string (' ', 77 - strbldrDumpLine.Length));
                }
                strbldrDumpLine.Append (">  <");

                // Print EBCDIC characters
                for (int iLineIdx = iIdx; iLineIdx - iIdx < 16 && iLineIdx < yaData.Length; ++iLineIdx)
                {
                    char cData = (char)ConvertEBCDICtoASCII (yaData[iLineIdx]);
                    if (cData >= (char)0x20 &&
                        cData <= (char)0x7F)
                    {
                        strbldrDumpLine.Append (cData);
                    }
                    else
                    {
                        strbldrDumpLine.Append (".");
                    }
                }
                // Pad string
                if (strbldrDumpLine.Length < 97)
                {
                    strbldrDumpLine.Append (new string (' ', 97 - strbldrDumpLine.Length));
                }
                strbldrDumpLine.Append (">");

                // Add line to list
                lstrHexDump.Add (strbldrDumpLine.ToString ());
            }

            return lstrHexDump;
        }

        private string ByteToHexPair (byte yData)
        {
            byte yHighOrder = (byte)((yData >> 4) & 0x0F);
            byte yLowOrder = (byte)(yData & 0x0F);
            char cHighOrder;
            char cLowOrder;

            if (yHighOrder > 9)
            {
                cHighOrder = (char)(yHighOrder - (byte)0x0A + (byte)'A');
            }
            else
            {
                cHighOrder = (char)(yHighOrder + (byte)'0');
            }

            if (yLowOrder > 9)
            {
                cLowOrder = (char)(yLowOrder - (byte)0x0A + (byte)'A');
            }
            else
            {
                cLowOrder = (char)(yLowOrder + (byte)'0');
            }

            StringBuilder strbldrHexPair = new StringBuilder (2);
            strbldrHexPair.Append (cHighOrder);
            strbldrHexPair.Append (cLowOrder);
            return strbldrHexPair.ToString ();
        }
    }
}