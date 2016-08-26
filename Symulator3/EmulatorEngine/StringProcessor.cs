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
    public class CStringProcessor
    {
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

        public void PrintStringList (List<string> strlInput)
        {
            foreach (string str in strlInput)
            {
                WriteOutputLine (str);
            }

            WriteOutputLine ("");
            WriteOutputLine ("");
        }

        public void PrintStringList (List<string> strlInput, bool bSpacing)
        {
            foreach (string str in strlInput)
            {
                WriteOutputLine (str);
            }

            if (bSpacing)
            {
                WriteOutputLine ("");
                WriteOutputLine ("");
            }
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
            Console.WriteLine (strOutput.TrimEnd (caTrimChars));
        }

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

        #region String test methods
        public static bool IsBlank (string strTest)
        {
            return IsBlank (strTest, 0, strTest.Length - 1);
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
                    yTest <= 0x7F);
        }

        public static bool IsPrintable (char cTest)
        {
            return (cTest >= (char)0x20 &&
                    cTest <= (char)0x7F);
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
    }
}
