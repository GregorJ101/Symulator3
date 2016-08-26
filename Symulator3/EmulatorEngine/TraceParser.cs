using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EmulatorEngine
{
    public class CTraceParser
    {
        string[] m_straTraceLog;
        int m_iTraceIdx = 0;
        int m_iLastCPULine = 0;

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
            m_straTraceLog = File.ReadAllLines (@"D:\SoftwareDev\System3Simulator\trace.log");
            //m_straTraceLog = File.ReadAllLines (strTraceFilename);
            //List <string> strlTest = new List<string> ();
            //strlTest.Add ("CPU> ARR=1001 XR1=2002 XR2=3003 IAR=1218 LVL=8 LIO 10,0,6,(67,XR1)");
            //strlTest.Add ("=D=> 1218 LIO 0,6,126C DAR=4004 CAR=5005 F=66 C=FF, S=77, N=88");
            //strlTest.Add ("=D=> Sense = 1234");
            //m_straTraceLog = strlTest.ToArray ();
            return m_straTraceLog.Length;
        }

        public string AnalyzeLine (int iARR, int iIAR, int iXR1, int iXR2, string strInstMnem,
                                   int iDAR, int iCAR, int iSNS, CDiskControlField dcf)
        {
            ParseTraceLine ();

            StringBuilder strbldCompareResults = new StringBuilder ();
            char[] caSpace = { ' ' };

            if (m_iARR != iARR)
            {
                if (!m_bARRErrorFound ||
                    m_iLastBadARR != iARR)
                {
                    strbldCompareResults.Append (string.Format ("ARR @ #{0:D} ({1:X4})", m_iLastCPULine, iARR));
                    m_iLastBadARR = iARR;
                    m_bARRErrorFound = true;
                }
            }
            else
            {
                m_bARRErrorFound = false;
            }

            if (m_iIAR != iIAR)
            {
                m_bAbort = true;

                if (strbldCompareResults.Length > 0)
                {
                    strbldCompareResults.Append (", ");
                }

                strbldCompareResults.Append (string.Format ("IAR @ #{0:D} ({1:X4})", m_iLastCPULine, iIAR));
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

                    strbldCompareResults.Append (string.Format ("XR1 @ #{0:D} ({1:X4})", m_iLastCPULine, iXR1));
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

                    strbldCompareResults.Append (string.Format ("XR2 @ #{0:D} ({1:X4})", m_iLastCPULine, iXR2));
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

                        strbldCompareResults.Append (string.Format ("DAR @ #{0:D} ({1:X4})", m_iLastCPULine, iDAR));
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

                        strbldCompareResults.Append (string.Format ("CAR @ #{0:D} ({1:X4})", m_iLastCPULine, iCAR));
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

                    strbldCompareResults.Append (string.Format ("F @ #{0:D} ({1:X2} s/b {2:X2})", m_iLastCPULine, dcf.yF, m_iF));
                }

                if (m_bDiskValid &&
                    m_iC != dcf.yC)
                {
                    if (strbldCompareResults.Length > 0)
                    {
                        strbldCompareResults.Append (", ");
                    }

                    strbldCompareResults.Append (string.Format ("C @ #{0:D} ({1:X2} s/b {2:X2})", m_iLastCPULine, dcf.yC, m_iC));
                }

                if (m_bDiskValid &&
                    m_iS != dcf.yS)
                {
                    if (strbldCompareResults.Length > 0)
                    {
                        strbldCompareResults.Append (", ");
                    }

                    strbldCompareResults.Append (string.Format ("S @ #{0:D} ({1:X2} s/b {2:X2})", m_iLastCPULine, dcf.yS, m_iS));
                }

                if (m_bDiskValid &&
                    m_iN != dcf.yN)
                {
                    if (strbldCompareResults.Length > 0)
                    {
                        strbldCompareResults.Append (", ");
                    }

                    strbldCompareResults.Append (string.Format ("N @ #{0:D} ({1:X2} s/b {2:X2})", m_iLastCPULine, dcf.yN, m_iN));
                }
            }

            if (m_bSNSValid &&
                m_iDiskSNS != iSNS)
            {
                if (strbldCompareResults.Length > 0)
                {
                    strbldCompareResults.Append (", ");
                }

                strbldCompareResults.Append (string.Format ("SNS @ #{0:D} ({1:X4} != {2:X4})", m_iLastCPULine, iSNS, m_iDiskSNS));
            }

            if (m_strInstMnem.TrimEnd (caSpace) != strInstMnem.TrimEnd (caSpace))
            {
                m_bAbort = true;

                if (strbldCompareResults.Length > 0)
                {
                    strbldCompareResults.Append (", ");
                }

                strbldCompareResults.Append (string.Format ("Mnem @ #{0:D} ({1:S})", m_iLastCPULine, m_strInstMnem));
            }

            if (strbldCompareResults.Length > 0)
            {
                return string.Format ("  * * *  {0:S}", strbldCompareResults.ToString ());
            }
            else
            {
                return "";
            }
        }

        private void ParseTraceLine ()
        {
            bool bFoundCpuLine = false;
            m_bDiskValid = false;
            m_bSNSValid = false;

            while (true)
            {
                string strTraceLine = m_straTraceLog[m_iTraceIdx++];
                //CPU> ARR=1001 XR1=2002 XR2=3003 IAR=1218 LVL=8 LIO 10,0,6,(67,XR1)
                if (strTraceLine.Substring (0, 4) == "CPU>")
                {
                    if (bFoundCpuLine)
                    {
                        if (ConvertHexString (strTraceLine.Substring (36, 4)) == m_iIAR)
                        {
                            int iIdx = m_iTraceIdx;

                            while (++iIdx < m_straTraceLog.Length)
                            {
                                string strTestLine = m_straTraceLog[iIdx];
                                if (strTestLine.Substring (0, 4) == "CPU>")
                                {
                                    int iNextIAR = ConvertHexString (strTestLine.Substring (36, 4));
                                    if (iNextIAR != m_iIAR)
                                    {
                                        m_iTraceIdx = iIdx;
                                        break;
                                    }
                                }
                            }
                        }

                        break;
                    }
                    else
                    {
                        m_iLastCPULine = m_iTraceIdx;
                        bFoundCpuLine = true;
                    }

                    string strNextTraceLine = FindNextTraceLine ("CPU>", m_iTraceIdx - 1);
                    if (strNextTraceLine.Length == 0)
                    {
                        strNextTraceLine = strTraceLine;
                    }

                    if (strTraceLine.Length > 50)
                    {
                        m_iIAR = ConvertHexString (strTraceLine.Substring (36, 4));
                        int iNextSpace = strTraceLine.IndexOf (' ', 47);
                        m_strInstMnem = strTraceLine.Substring (47, iNextSpace - 47);
                    }

                    if (strNextTraceLine.Length > 50)
                    {
                        m_iARR = ConvertHexString (strNextTraceLine.Substring (9, 4));
                        m_iXR1 = ConvertHexString (strNextTraceLine.Substring (18, 4));
                        m_iXR2 = ConvertHexString (strNextTraceLine.Substring (27, 4));
                    }

                    //FindNextIAR ();
                }
                //=D=> Sense = 1234 
                else if (strTraceLine.Substring (0, 10) == "=D=> Sense")
                {
                    if (strTraceLine.Length > 16)
                    {
                        m_iDiskSNS = ConvertHexString (strTraceLine.Substring (13, 4));
                        m_bSNSValid = true;
                    }
                }
                //=D=> 1218 LIO 0,6,126C DAR=4004 CAR=5005 F=66 C=FF, S=77, N=88
                else if (strTraceLine.Substring (0, 4) == "=D=>")
                {
                    string strNextTraceLine = FindNextTraceLine ("=D=>", m_iTraceIdx);
                    if (strNextTraceLine.Length == 0)
                    {
                        strNextTraceLine = strTraceLine;
                    }

                    if (strNextTraceLine.Length > 61)
                    {
                        m_iDAR = ConvertHexString (strNextTraceLine.Substring (27, 4));
                        m_iCAR = ConvertHexString (strNextTraceLine.Substring (36, 4));
                        m_bFisDots = strNextTraceLine.Substring (43, 2) == "..";
                        m_iF = ConvertHexString (strNextTraceLine.Substring (43, 2));
                        m_iC = ConvertHexString (strNextTraceLine.Substring (48, 2));
                        m_iS = ConvertHexString (strNextTraceLine.Substring (54, 2));
                        m_iN = ConvertHexString (strNextTraceLine.Substring (60, 2));
                        m_bDiskValid = true;
                    }
                }
            }

            --m_iTraceIdx;
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
                        return m_straTraceLog[iIdx];
                    }
                }
            }

            return "";
        }

        private void FindNextIAR ()
        {
            int iIdx = m_iTraceIdx - 1;

            while (++iIdx < m_straTraceLog.Length)
            {
                string strTraceLine = m_straTraceLog[iIdx];
                if (strTraceLine.Substring (0, 4) == "CPU>")
                {
                    int iNextIAR = ConvertHexString (strTraceLine.Substring (36, 4));
                    if (iNextIAR != m_iIAR)
                    {
                        m_iTraceIdx = iIdx;
                        break;
                    }
                }
            }
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

        private int TryGetInt (string strValue)
        {
            try
            {
                return Convert.ToInt32 (strValue);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }
    }
}