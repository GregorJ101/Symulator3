using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// Todo:
// Add CXString methods (SpellNumber, ToRomanNumeral, FromRomanNumeral, ...)
// Add CXString methods for type-testing strings (IsWhitespace, ...)
// Add huge integer class from UCSD Extendion C# class (Write PowersOfTwo in C# using this class
//     Add comma formatting to huge integer class if needed

namespace EmulatorEngine
{
    public class CStringProcessor : CSystem3IOBase
    {
        protected bool m_bLogToFile    = false;
        public void SetLogToFile ()   { m_bLogToFile = true; }
        public void ResetLogToFile () { m_bLogToFile = false; }
        public bool GetLogToFile ()   { return m_bLogToFile; }

        protected bool m_bLogToConsole = true;
        public void SetLogToConsole ()   { m_bLogToConsole = true; }
        public void ResetLogToConsole () { m_bLogToConsole = false; }
        public bool GetLogToConsole ()   { return m_bLogToConsole; }

        protected const string ROMAN_NUMERAL_CHARACTERS = "IVXLCDM";

        StreamWriter m_swLogFile = null;
        string m_strLogOutput = "";

        public List<string> ReadFileToStringList (string strFilename, int iLineLimit)
        {
            List<string> strlFileLines = new List<string> ();

            try
            {
                string[] straFileData = File.ReadAllLines (strFilename);

                foreach (string strLine in straFileData)
                {
                    string strLineCopy = strLine;

                    if (iLineLimit > 0)
                    {
                        if (strLineCopy.Length < iLineLimit)
                        {
                            strLineCopy += new string (' ', iLineLimit - strLineCopy.Length);
                        }
                        else if (strLineCopy.Length > iLineLimit)
                        {
                            strLineCopy = strLineCopy.Substring (0, iLineLimit);
                        }
                    }

                    strlFileLines.Add (strLineCopy);
                }
            }
            catch (System.ArgumentNullException)
            {
                return strlFileLines;
            }
            catch (System.Security.SecurityException)
            {
                return strlFileLines;
            }
            catch (System.NotSupportedException)
            {
                return strlFileLines;
            }
            catch (System.ArgumentException)
            {
                return strlFileLines;
            }
            catch (System.IO.PathTooLongException)
            {
                return strlFileLines;
            }
            catch (System.IO.IOException)
            {
                return strlFileLines;
            }
            catch (System.UnauthorizedAccessException)
            {
                return strlFileLines;
            }

            return strlFileLines;
        }

        public byte[] StringToByteArray (string strInput)
        {
            byte[] yaReturn = new byte[strInput.Length];

            for (int iIdx = 0; iIdx < strInput.Length; iIdx++)
            {
                char cIdx = strInput[iIdx];
                yaReturn.SetValue ((byte)cIdx, iIdx);
            }

            return yaReturn;
        }

        public List<string> ParseStringToList (string strInput, string strDelimiters)
        {
            char[] caDelimiters = strDelimiters.ToCharArray ();
            string[] straOutput = strInput.Split (caDelimiters);
            List<string> strlParsedTokens = new List<string> (straOutput.Length);

            foreach (string str in straOutput)
            {
                if (str.Length > 0)
                {
                    strlParsedTokens.Add (str);
                }
            }

            return strlParsedTokens;
        }

        public void AppendStringList (List<string> strlSource, List<string> strlTarget, bool bClearSourceAfter = false)
        {
            foreach (string str in strlSource)
                strlTarget.Add (str);

            if (bClearSourceAfter)
                strlSource.Clear ();
        }

        public void PrintStringListToConsole (List<string> strlInput, int iSpacing = 2, bool bClearAfter = false)
        {
            foreach (string str in strlInput)
            {
                Console.WriteLine (str);
            }

            for (int iCount = 0; iCount < iSpacing; ++iCount)
            {
                Console.WriteLine ("");
            }

            if (bClearAfter)
                strlInput.Clear ();
        }

        public void PrintStringList (List<string> strlInput, int iSpacing = 2, bool bClearAfter = false)
        {
            foreach (string str in strlInput)
            {
                WriteOutputLine (str);
            }

            for (int iCount = 0; iCount < iSpacing; ++iCount)
            {
                WriteOutputLine ("");
            }

            if (bClearAfter)
                strlInput.Clear ();
        }

        public void Print7SegmentList (List<string> strlInput, int iTop, int iLeft, bool bClearBefore = false, bool bClearAfter = false)
        {
            if (bClearBefore)
            {
                Console.Clear ();
            }

            foreach (string str in strlInput)
            {
                Console.SetCursorPosition (iLeft, iTop++);
                Console.Write (str);
                // Write an invisible character since Write optimizes by truncating trailing spaces
                ConsoleColor ccForeground = Console.ForegroundColor;
                Console.ForegroundColor = Console.BackgroundColor;
                Console.Write ('+');
                Console.ForegroundColor = ccForeground;
            }

            if (bClearAfter)
                strlInput.Clear ();
        }

        public void PrintStringList (List<string> strlInput, bool bSpacing, bool bDividerLine)
        {
            foreach (string str in strlInput)
            {
                WriteOutputLine (str);
            }

            if (bSpacing)
            {
                WriteOutputLine ("");
            }
            if (bDividerLine)
            {
                WriteOutputLine ("--------------------------------");
            }
            if (bSpacing)
            {
                WriteOutputLine ("");
            }
        }

        public void PrintPunchStringList (List<string> listPunchRows)
        {
            foreach (string str in listPunchRows)
            {
                WriteOutputLine (str);
            }

            WriteOutputLine ("");
            WriteOutputLine ("--------------------------------");
            WriteOutputLine ("");
        }

        public bool WriteTextFile (string strFilename, List<string> listTextLines)
        {
            if (strFilename == null)
            {
                return false;
            }

            FileInfo fiOutput = new FileInfo (strFilename);
            if (fiOutput.Exists)
            {
                fiOutput.Delete ();
            }

            using (StreamWriter swExtractTxt = fiOutput.CreateText ())
            {
                foreach (string strTextLine in listTextLines)
                {
                    swExtractTxt.WriteLine (strTextLine);
                }
            }

            return fiOutput.Exists;
        }

        public bool WriteTextFile (string strFilename, string strText)
        {
            if (strFilename == null ||
                strText == null)
            {
                return false;
            }

            FileInfo fiOutput = new FileInfo (strFilename);
            if (fiOutput.Exists)
            {
                fiOutput.Delete ();
            }

            using (StreamWriter swExtractTxt = fiOutput.CreateText ())
            {
                swExtractTxt.WriteLine (strText);
            }

            return fiOutput.Exists;
        }

        public void WriteOutputLine (string strOutput)
        {
            //char[] caTrimChars = { (char)0x00, (char)0x01, (char)0x02, (char)0x03,
            //                       (char)0x04, (char)0x05, (char)0x06, (char)0x07,
            //                       (char)0x08, (char)0x09, (char)0x0A, (char)0x0B,
            //                       (char)0x0C, (char)0x0D, (char)0x0E, (char)0x0F };
            char[] caTrimChars = { (char)0x00, (char)0x20 };
            //Console.WriteLine (strOutput.TrimEnd (caTrimChars));
            WriteLogOutputLine (strOutput.TrimEnd (caTrimChars));
        }

        public bool SetLogFileame (string strFilename)
        {
            if (m_bLogToFile &&
                strFilename.Length > 0)
            {
                m_swLogFile = new StreamWriter (strFilename);
                return true;
            }

            return false;
        }

        public void WriteLogOutput (string strOutput)
        {
            if (m_bLogToConsole)
            {
                Console.Write (strOutput);
            }

            m_strLogOutput = strOutput;
        }

        public void WriteLogOutputLine (string strOutput)
        {
            if (m_bLogToConsole)
            {
                Console.WriteLine (strOutput);
            }

            if (m_swLogFile != null)
            {
                if (m_strLogOutput.Length > 0)
                {
                    strOutput = m_strLogOutput + strOutput;
                    m_strLogOutput = "";
                }

                m_swLogFile.WriteLine (strOutput);
                m_swLogFile.Flush ();
            }
        }

        // TODO: Interface with UI layer
        //public string GetOutputDASM ()
        //{
        //    return "";
        //}

        //public string GetOutputTrace ()
        //{
        //    return "";
        //}

        //public string GetOutputPrinter ()
        //{
        //    return "";
        //}

        public int StringToInt (string strInput, int iBase)
        {
            try
            {
                return Convert.ToInt16 (strInput, iBase);
            }
            catch (System.FormatException)
            {
                return 0;
            }
        }

        public bool CompareNoCase (string strOne, string strTwo)
        {
            return strOne.Equals (strTwo, StringComparison.OrdinalIgnoreCase);
        }

        public byte ToUpper (byte yCharacter)
        {
            if (yCharacter < (byte)'a' ||
                yCharacter > (byte)'z')
            {
                return yCharacter;
            }

            return (byte)(yCharacter - 0x20);
        }

        public byte ToLower (byte yCharacter)
        {
            if (yCharacter < (byte)'A' ||
                yCharacter > (byte)'Z')
            {
                return yCharacter;
            }

            return (byte)(yCharacter + 0x20);
        }

        #region String test methods
        public static bool IsBlank (string strTest)
        {
            return IsBlank (strTest, 0, strTest.Length - 1);
        }

        public static bool IsAllBlank (string strTest)
        {
            return IsBlank (strTest, 0, strTest.Length);
        }

        public static bool IsBlank (string strTest, int iStart)
        {
            return IsBlank (strTest, iStart, strTest.Length - 1 - iStart);
        }

        public static bool IsBlank (string strTest, int iStart, int iLength)
        {
            string strCopy;

            if (iStart > 0 || iLength < strTest.Length)
            {
                strCopy = strTest.Substring (iStart, iLength);
            }
            else
            {
                strCopy = strTest;
            }

            foreach (char cTest in strCopy)
            {
                if (cTest != ' ')
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsPrintable (string strTest)
        {
            return IsPrintable (strTest, 0, strTest.Length - 1);
        }

        public static bool IsPrintable (string strTest, int iStart)
        {
            return IsPrintable (strTest, iStart, strTest.Length - 1 - iStart);
        }

        public static bool IsPrintable (string strTest, int iStart, int iLength)
        {
            string strCopy;

            if (iStart > 0 || iLength < strTest.Length)
            {
                strCopy = strTest.Substring (iStart, iLength);
            }
            else
            {
                strCopy = strTest;
            }

            foreach (char cTest in strCopy)
            {
                {
                    if (!IsPrintable (cTest))
                        return false;
                }
            }

            return true;
        }

        public static bool IsPrintable (byte yTest)
        {
            return (yTest >= 0x20 &&
                    yTest <  0x7F);
        }

        public static bool IsPrintable (char cTest)
        {
            return (cTest >= (char)0x20 &&
                    cTest <  (char)0x7F);
        }

        public static bool IsSystem3Set (char cTest)
        {
            string strTest = new string (cTest, 1);
            return IsSystem3Set (strTest, 0, strTest.Length - 1);
        }

        public static bool IsSystem3Set (string strTest)
        {
            return IsSystem3Set (strTest, 0, strTest.Length - 1);
        }

        public static bool IsSystem3Set (string strTest, int iStart)
        {
            return IsSystem3Set (strTest, iStart, strTest.Length - 1 - iStart);
        }

        public static bool IsSystem3Set (string strTest, int iStart, int iLength)
        {
            string strCopy;
            const string kstrSystem3CharacterSet = " 123456789:#@'=\"0/STUVWXYZ&,%_>?-JKLMNOPQR!$*);^}ABCDEFGHI~.<(+|";

            if (iStart > 0 || iLength < strTest.Length)
            {
                strCopy = strTest.Substring (iStart, iLength);
            }
            else
            {
                strCopy = strTest;
            }

            for (int iIdx = 0; iIdx < strTest.Length; iIdx++)
            {
                if (!kstrSystem3CharacterSet.Contains (strTest.Substring (iIdx, 1)))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsNumeric (string strTest)
        {
            return IsNumeric (strTest, 0, strTest.Length - 1);
        }

        public static bool IsNumeric (string strTest, int iStart)
        {
            return IsNumeric (strTest, iStart, strTest.Length - 1 - iStart);
        }

        public static bool IsNumeric (string strTest, int iStart, int iLength)
        {
            for (int iIdx = 0; iIdx < strTest.Length; iIdx++)
            {
                char cTest = strTest[iIdx];

                if (cTest < '0' ||
                    cTest > '9')
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsNumeric (char cTest)
        {
            return (cTest >= '0' &&
                    cTest <= '9');
        }

        public static bool IsHexadecimal (string strTest)
        {
            return IsHexadecimal (strTest, 0, strTest.Length - 1);
        }

        public static bool IsHexadecimal (string strTest, int iStart)
        {
            return IsHexadecimal (strTest, iStart, strTest.Length - 1 - iStart);
        }

        public static bool IsHexadecimal (string strTest, int iStart, int iLength)
        {
            for (int iIdx = 0; iIdx < strTest.Length; iIdx++)
            {
                if (!IsHexadecimal (strTest[iIdx]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsHexadecimal (char cTest)
        {
            if (cTest >= 'a' &&
                cTest <= 'f')
            {
                cTest -= (char)0x0020;
            }

            return ((cTest >= '0'  &&
                     cTest <= '9') ||
                    (cTest >= 'A'  &&
                     cTest <= 'F'));
        }

        public static bool IsAlpha (string strTest)
        {
            foreach (char c in strTest)
            {
                if (!IsAlpha (c))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsAlpha (char cTest)
        {
            return (cTest >= 'a'  &&
                    cTest <= 'z') ||
                   (cTest >= 'A'  &&
                    cTest <= 'Z');
        }

        public static bool IsAlNum (char cTest)
        {
            return (IsAlpha   (cTest) ||
                    IsNumeric (cTest));
        }

        public static bool IsAlNum (string strTest)
        {
            foreach (char c in strTest)
            {
                if (!IsAlpha (c) &&
                    !IsNumeric (c))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsRomanNumeral (string strTest)
        {
            strTest = strTest.ToUpper ();

            foreach (char c in strTest)
            {
                if (ROMAN_NUMERAL_CHARACTERS.IndexOf (c) < 0)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsMixedCase (string strTest)
        {
            bool bLowerCaseFound = false;
            bool bUpperCaseFound = false;

            foreach (char c in strTest)
            {
                if (c >= 'a' && c <= 'z')
                {
                    bLowerCaseFound = true;
                }

                if (c >= 'A' && c <= 'Z')
                {
                    bUpperCaseFound = true;
                }

                if (bLowerCaseFound && bUpperCaseFound)
                {
                    break;
                }
            }

            return bLowerCaseFound && bUpperCaseFound;
        }

        public static ushort SafeConvertHexadecimalStringToInt16 (string strCPUDials)
        {
            ushort usCPUDials = 0;
            strCPUDials = strCPUDials.ToUpper ();

            foreach (char c in strCPUDials)
            {
                usCPUDials <<= 4;

                if (c >= '0' &&
                    c <= '9')
                {
                    usCPUDials += (ushort)(c - '0');
                }
                else if (c >= 'A' &&
                         c <= 'F')
                {
                    usCPUDials += (ushort)(c - 'A' + 10);
                }
            }

            return usCPUDials;
        }
        #endregion

        public List<string> ExtractStrings (List<string> strlSource, bool bCommandLines, bool bMiscellaneiousLines, bool bTextCardLines, bool bEndCardLines)
        {
            List<string> strlExtractedStrings = new List<string> ();
            List<string> strlErrorsFound = new List<string> ();
            bool bMiscellaneousLineFound = false,
                 bTextCardLineFound = false,
                 bEndCardLineFound = false;

            foreach (string str in strlSource)
            {
                if (str[0] == '/')
                {
                    if (bMiscellaneousLineFound ||
                        bTextCardLineFound ||
                        bEndCardLineFound)
                    {
                        strlErrorsFound.Add ("Embedded command line: " + str);
                    }
                    else
                    {
                        if (bCommandLines)
                        {
                            strlExtractedStrings.Add (str);
                        }
                    }
                }
                else if (str[0] == 'T')
                {
                    if (bEndCardLineFound)
                    {
                        strlErrorsFound.Add ("Trailing text card line: " + str);
                    }
                    else
                    {
                        bTextCardLineFound = true;

                        if (bTextCardLines)
                        {
                            strlExtractedStrings.Add (str);
                        }
                    }
                }
                else if (str[0] == 'E')
                {
                    if (bEndCardLineFound)
                    {
                        strlErrorsFound.Add ("Multiple end card line: " + str);
                    }
                    else
                    {
                        bEndCardLineFound = true;

                        if (bEndCardLines)
                        {
                            strlExtractedStrings.Add (str);
                        }
                    }
                }
                else
                {
                    if (bTextCardLineFound ||
                        bEndCardLineFound)
                    {
                        strlErrorsFound.Add ("Embedded miscellanious line: " + str);
                    }
                    else
                    {
                        bMiscellaneousLineFound = true;

                        if (bMiscellaneiousLines)
                        {
                            strlExtractedStrings.Add (str);
                        }
                    }
                }
            }

            return strlExtractedStrings;
        }

        //          1         2         3         4         5
        // 12345678901234567890123456789012345678901234567890
        public List<string> MakeRulerLines (int iLength, int iOffset)
        {
            List<string> strlLines = new List<string> ();

            StringBuilder strbLine = new StringBuilder (new string (' ', iOffset));

            // Create top line
            for (int iCounter = 1; iCounter <= iLength; ++iCounter)
            {
                if (iCounter % 10 == 0)
                {
                    strbLine.Append ((char)((iCounter / 10) + '0'));
                }
                else
                {
                    strbLine.Append (' ');
                }
            }
            strlLines.Add (strbLine.ToString ());

            // Create bottom line
            strbLine.Clear ();
            for (int iCounter = 1; iCounter <= iLength; ++iCounter)
            {
                strbLine.Append ((char)((iCounter % 10) + '0'));
            }
            strlLines.Add (new string (' ', iOffset) + strbLine.ToString ());

            return strlLines;
        }
    }
}
