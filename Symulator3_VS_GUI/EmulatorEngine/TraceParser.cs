using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EmulatorEngine
{
    public class CTraceParser : CControlFlags
    {
        string[] m_straTraceLog;
        int m_iTraceIdx = 0;
        int m_iLastCPULine = 0;
        List<string> m_strlConsole = new List<string> ();

        int m_iARR = 0;
        int m_iIAR = 0;
        int m_iXR1 = 0;
        int m_iXR2 = 0;
        int m_iDAR = 0;
        int m_iCAR = 0;
        int m_iF = 0;
        int m_iC = 0;
        int m_iS = 0;
        int m_iN = 0;
        int m_iDiskSNS = 0;
        bool m_bFisDots   = false;
        bool m_bDiskValid = false;
        bool m_bSNSValid  = false;
        string m_strInstMnem;

        bool m_bARRErrorFound = false;
        bool m_bXR1ErrorFound = false;
        bool m_bXR2ErrorFound = false;
        bool m_bDARErrorFound = false;
        bool m_bCARErrorFound = false;
        int m_iLastBadARR = 0;
        int m_iLastBadXR1 = 0;
        int m_iLastBadXR2 = 0;
        int m_iLastBadDAR = 0;
        int m_iLastBadCAR = 0;
        bool m_bAbort = false;

        public bool IsAbort ()
        {
            return m_bAbort;
        }
        
        public int LoadTraceLog (string strTraceFilename)
        {
            //m_straTraceLog = File.ReadAllLines (@"D:\SoftwareDev\Archive\System3\System3Simulator\trace-edited.log");
            //m_straTraceLog = File.ReadAllLines (@"D:\SoftwareDev\IBMSystem3\Sim\trace-edited.log");
            m_straTraceLog = File.ReadAllLines (@"D:\SoftwareDev\Archive\System3\System3Simulator\trace-edited2.log");
            //m_straTraceLog = File.ReadAllLines (strTraceFilename);
            //List <string> strlTest = new List<string> ();
            //strlTest.Add ("CPU> ARR=1001 XR1=2002 XR2=3003 IAR=1218 LVL=8 LIO 10,0,6,(67,XR1)");
            //strlTest.Add ("=D=> 1218 LIO 0,6,126C DAR=4004 CAR=5005 F=66 C=FF, S=77, N=88");
            //strlTest.Add ("=D=> Sense = 1234");
            //m_straTraceLog = strlTest.ToArray ();
            return m_straTraceLog.Length;
        }

        public List<string> AnalyzeLine (int[] iaARR, int iIAR, int iXR1, int iXR2, string strInstMnem, int iDAR,
                                        int iCAR, int iSNS, CDiskControlField dcf, ref int iIL, ref int iTraceIdx)
        {
            List<string> strlErrors = new List<string> ();

            if (!ParseTraceLine (ref iIL))
            {
                m_bAbort = true;
                return strlErrors;
            }

            char[] caSpace = { ' ' };
            string strLine = m_iTraceIdx.ToString (),
                   strPadding = new string (' ', 8 - strLine.Length);
            string strAsterisks = "  * * *  <" + strPadding + strLine + ">  ";
            int iAsteriskLength = strAsterisks.Length;
            string strSpaces = new string (' ', iAsteriskLength);

            StringBuilder strbldCompareResults = new StringBuilder ();

            if (m_iIAR != iIAR)
            {
                //m_bAbort = true;

                if (strbldCompareResults.Length > 0)
                {
                    strbldCompareResults.Append (", ");
                }

                strlErrors.Add (strAsterisks + string.Format ("IAR @ #{0:D} ({1:X4} s/b {2:X4})", m_iLastCPULine, iIAR, m_iIAR));
                strAsterisks = strSpaces;
            }

            if (m_strInstMnem.TrimEnd (caSpace) != strInstMnem.TrimEnd (caSpace))
            {
                //m_bAbort = true;

                if (strbldCompareResults.Length > 0)
                {
                    strbldCompareResults.Append (", ");
                }

                strlErrors.Add (strAsterisks + string.Format ("Mnem @ #{0:D} ({1:X4} s/b {2:X4})", m_iLastCPULine,
                                                              strInstMnem.TrimEnd (), m_strInstMnem.TrimEnd ()));
                strAsterisks = new string (' ', iAsteriskLength);
            }

            if (m_iARR != iaARR[iIL])
            {
                if (!m_bARRErrorFound ||
                    m_iLastBadARR != iaARR[iIL])
                {
                    strlErrors.Add (strAsterisks + string.Format ("ARR @ #{0:D} ({1:X4} s/b {2:X4})", m_iLastCPULine, iaARR[iIL], m_iARR));
                    strAsterisks = strSpaces;
                    m_iLastBadARR = iaARR[iIL];
                    m_bARRErrorFound = true;
                }
            }
            else
            {
                m_bARRErrorFound = false;
            }

            if (m_iXR1 != iXR1)
            {
                if (!m_bXR1ErrorFound ||
                    m_iLastBadXR1 != iXR1)
                {
                    if (strbldCompareResults.Length > 0)
                    {
                        strbldCompareResults.Append (", ");
                    }

                    strlErrors.Add (strAsterisks + string.Format ("XR1 @ #{0:D} ({1:X4} s/b {2:X4})", m_iLastCPULine, iXR1, m_iXR1));
                    strAsterisks = strSpaces;
                    m_bXR1ErrorFound = true;
                    m_iLastBadXR1 = iXR1;
                }
            }
            else
            {
                m_bXR1ErrorFound = false;
            }

            if (m_iXR2 != iXR2)
            {
                if (!m_bXR2ErrorFound ||
                    m_iLastBadXR2 != iXR2)
                {
                    if (strbldCompareResults.Length > 0)
                    {
                        strbldCompareResults.Append (", ");
                    }

                    strlErrors.Add (strAsterisks + string.Format ("XR2 @ #{0:D} ({1:X4} s/b {2:X4})", m_iLastCPULine, iXR2, m_iXR2));
                    strAsterisks = strSpaces;
                    m_bXR2ErrorFound = true;
                    m_iLastBadXR2 = iXR2;
                }
            }
            else
            {
                m_bXR2ErrorFound = false;
            }

            if (m_bDiskValid)
            {
                if (m_iDAR != iDAR)
                {
                    if (!m_bDARErrorFound ||
                        m_iLastBadDAR != iDAR)
                    {
                        if (strbldCompareResults.Length > 0)
                        {
                            strbldCompareResults.Append (", ");
                        }

                        strlErrors.Add (strAsterisks + string.Format ("DAR @ #{0:D} ({1:X4} s/b {2:X4})", m_iLastCPULine, iDAR, m_iDAR));
                        strAsterisks = strSpaces;
                        m_bDARErrorFound = true;
                        m_iLastBadDAR = iDAR;
                    }
                }
                else
                {
                    m_bDARErrorFound = false;
                }

                if (m_iCAR != iCAR)
                {
                    if (!m_bCARErrorFound ||
                        m_iLastBadCAR != iCAR)
                    {
                        m_bCARErrorFound = true;
                        m_iLastBadCAR = iCAR;
                        if (strbldCompareResults.Length > 0)
                        {
                            strbldCompareResults.Append (", ");
                        }

                        strlErrors.Add (strAsterisks + string.Format ("CAR @ #{0:D} ({1:X4} s/b {2:X4})", m_iLastCPULine, iCAR, m_iCAR));
                        strAsterisks = strSpaces;
                    }
                }
                else
                {
                    m_bCARErrorFound = false;
                }

                if (!m_bFisDots &&
                    m_iF != dcf.yF)
                {
                    if (strbldCompareResults.Length > 0)
                    {
                        strbldCompareResults.Append (", ");
                    }

                    strlErrors.Add (strAsterisks + string.Format ("F @ #{0:D} ({1:X4} s/b {2:X4})", m_iLastCPULine, dcf.yF, m_iF));
                    strAsterisks = strSpaces;
                }

                if (m_bDiskValid &&
                    m_iC != dcf.yC)
                {
                    if (strbldCompareResults.Length > 0)
                    {
                        strbldCompareResults.Append (", ");
                    }

                    strlErrors.Add (strAsterisks + string.Format ("C @ #{0:D} ({1:X4} s/b {2:X4})", m_iLastCPULine, dcf.yC, m_iC));
                    strAsterisks = strSpaces;
                }

                if (m_bDiskValid &&
                    m_iS != dcf.yS)
                {
                    if (strbldCompareResults.Length > 0)
                    {
                        strbldCompareResults.Append (", ");
                    }

                    strlErrors.Add (strAsterisks + string.Format ("S @ #{0:D} ({1:X4} s/b {2:X4})", m_iLastCPULine, dcf.yS, m_iS));
                    strAsterisks = strSpaces;
                }

                if (m_bDiskValid &&
                    m_iN != dcf.yN)
                {
                    if (strbldCompareResults.Length > 0)
                    {
                        strbldCompareResults.Append (", ");
                    }

                    strlErrors.Add (strAsterisks + string.Format ("N @ #{0:D} ({1:X4} s/b {2:X4})", m_iLastCPULine, dcf.yN, m_iN));
                    strAsterisks = strSpaces;
                }
            }

            if (m_bSNSValid &&
                m_iDiskSNS != iSNS)
            {
                if (strbldCompareResults.Length > 0)
                {
                    strbldCompareResults.Append (", ");
                }

                strlErrors.Add (strAsterisks + string.Format ("SNS @ #{0:D} ({1:X4} s/b {2:X4})", m_iLastCPULine, iSNS, m_iDiskSNS));
                strAsterisks = strSpaces;
            }

            iTraceIdx = m_iTraceIdx;
            return strlErrors;
        }

        private bool ParseTraceLine (ref int iIL)
        {
            bool bFoundCpuLine = false;
            m_bDiskValid       = false;
            m_bSNSValid        = false;

            //           1         2         3         4         5
            // 012345678901234567890123456789012345678901234567890123456789
            // CPU> ARR=0000 XR1=0000 XR2=0000 IAR=0000 LVL=8 TBN 0001,FF

            // Get from current line
            //    IAR (substring (36, 4)
            //    mnem (substring (47, 3).trim
            if (m_iTraceIdx < m_straTraceLog.Length)
            {
                string strTraceLine = m_straTraceLog[m_iTraceIdx++];
                if (strTraceLine.Substring (0, 4) == "CPU>")
                {
                    m_iIAR = ConvertHexString (strTraceLine.Substring (36, 4));
                    int iNextSpace = strTraceLine.IndexOf (' ', 47);
                    m_strInstMnem = strTraceLine.Substring (47, iNextSpace - 47);
                    m_iLastCPULine = m_iTraceIdx;
                }
            }
            else
            {
                return false;
            }

            // Read Next line
            for (int iSearchIdx = m_iTraceIdx; iSearchIdx < m_straTraceLog.Length && !bFoundCpuLine; ++iSearchIdx)
            {
                string strNextTraceLine = m_straTraceLog[iSearchIdx];

                // CPU> ARR=0000 XR1=0000 XR2=0000 IAR=0000 LVL=8 TBN 0001,FF
                if (strNextTraceLine.Substring (0, 4) == "CPU>")
                {
                    //      Grab ARR (substring (9, 4)
                    //           XR1 (substring (18, 4)
                    //           XR2 (substring (27, 4)
                    m_iARR = ConvertHexString (strNextTraceLine.Substring (9, 4));
                    m_iXR1 = ConvertHexString (strNextTraceLine.Substring (18, 4));
                    m_iXR2 = ConvertHexString (strNextTraceLine.Substring (27, 4));
                    m_iTraceIdx = iSearchIdx;
                    char cNewIL = strNextTraceLine.Length > 45 ? strNextTraceLine[45] : ' ';
                    if (cNewIL >= '0' &&
                        cNewIL <= '9')
                    {
                        int iNewIL = (int)(cNewIL - '0');
                        if (iNewIL != 8)
                        {
                            iIL = iNewIL + 1;
                        }
                    }
                    bFoundCpuLine = true;
                }
                //=D=> Sense = 1234 
                else if (strNextTraceLine.Substring (0, 10) == "=D=> Sense")
                {
                    if (strNextTraceLine.Length > 16)
                    {
                        m_iDiskSNS = ConvertHexString (strNextTraceLine.Substring (13, 4));
                        m_bSNSValid = true;
                    }
                }
                //=D=> 1218 LIO 0,6,126C DAR=4004 CAR=5005 F=66 C=FF, S=77, N=88
                else if (strNextTraceLine.Substring (0, 4) == "=D=>")
                {
                    string strNextDiskControlField = FindNextTraceLine ("=D=>", iSearchIdx + 1);
                    if (strNextDiskControlField.Length > 61)
                    {
                        m_iDAR = ConvertHexString (strNextDiskControlField.Substring (27, 4));
                        m_iCAR = ConvertHexString (strNextDiskControlField.Substring (36, 4));
                        m_bFisDots = strNextDiskControlField.Substring (43, 2) == "..";
                        m_iF = ConvertHexString (strNextDiskControlField.Substring (43, 2));
                        m_iC = ConvertHexString (strNextDiskControlField.Substring (48, 2));
                        m_iS = ConvertHexString (strNextDiskControlField.Substring (54, 2));
                        m_iN = ConvertHexString (strNextDiskControlField.Substring (60, 2));
                        m_bDiskValid = true;
                    }
                }
            }

            // Done: compare values for error string
            return true;
        }

        private string FindNextTraceLine (string strSearchKey, int iIdx)
        {
            int iKeyLength = strSearchKey.Length;

            while (++iIdx < m_straTraceLog.Length)
            {
                if (m_straTraceLog[iIdx].Substring (0, iKeyLength) == strSearchKey)
                {
                    if (m_straTraceLog[iIdx].Substring (5, 5) != "Sense")
                    {
                        //string strConsole = string.Format ("---> C {0:0000} {1}", iIdx + 1, m_straTraceLog[iIdx]);
                        //m_strlConsole.Add (strConsole);
                        //Console.WriteLine (strConsole);
                        return m_straTraceLog[iIdx];
                    }
                }
            }

            return "";
        }

        public void PrintConsoleLines ()
        {
            foreach (string str in m_strlConsole)
                Console.WriteLine (str);

            m_strlConsole.Clear ();
        }

        private int ConvertHexString (string strValue)
        {
            int iReturnVal = 0;
            string strLower = strValue.ToLower ();
            for (int iIdx = 0; iIdx < strLower.Length; ++iIdx)
            {
                char cValue = strLower[iIdx];
                int iTemp = 0;
                if (cValue >= '0' &&
                    cValue <= '9')
                {
                    iTemp = (int)(cValue) - 0x30;
                }
                else if (cValue >= 'a' &&
                         cValue <= 'f')
                {
                    iTemp = (int)(cValue) - 0x57;
                }
                else
                {
                    return iReturnVal;
                }

                iReturnVal <<= 4;
                iReturnVal += iTemp;
            }

            return iReturnVal;
        }
    }
}