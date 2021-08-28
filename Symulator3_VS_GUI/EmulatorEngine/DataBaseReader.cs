using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace EmulatorEngine
{
    public enum EDatabaseTable
    {
        TABLE_Undefined,
        TABLE_CardData,
        TABLE_CardObjectIPL,
        TABLE_CardObjectText,
        TABLE_CardRPGiiSource,
        TABLE_DiskCreatedImages,
        TABLE_DiskOriginalImages,
        TABLE_FileMaster,
        TABLE_SavedScripts,
        TABLE_ScriptingMacros
    };

    public enum ETokenType
    {
        TOKEN_Invalid,
        TOKEN_CardBinary,
        TOKEN_DiskBinary,
        TOKEN_CardData,
        TOKEN_ScriptData,
        TOKEN_Empty,
        TOKEN_ValidFilePath,
        TOKEN_InvalidFilePath
    };

    public class CDBFileToken
    {
        private string m_strFileTokenKey = "";
        public  string FileTokenKey
        {
            get { return m_strFileTokenKey; }
            set { m_strFileTokenKey = value; }
        }

        private string m_strTableName = "";
        public  string TableName
        {
            get { return m_strTableName; }
            set { m_strTableName = value; }
        }

        private string m_strDataName = "";
        public  string DataName
        {
            get { return m_strDataName; }
            set { m_strDataName = value; }
        }

        private string m_strFilePath = "";
        public  string FilePath
        {
            get { return m_strFilePath; }
            set { m_strFilePath = value; }
        }

        public ETokenType GetTokenType ()
        {
            if (m_strTableName == "CardObjectIPL" ||
                m_strTableName == "CardObjectText")
            {
                return ETokenType.TOKEN_CardBinary;
            }
            else if (m_strTableName == "CardData" ||
                     m_strTableName == "CardRPGiiSource")
            {
                return ETokenType.TOKEN_CardData;
            }
            else if (m_strTableName == "DiskOriginalImages" ||
                     m_strTableName == "DiskCreatedImages")
            {
                return ETokenType.TOKEN_DiskBinary;
            }
            else if (m_strTableName == "ScriptingMacros" ||
                     m_strTableName == "SavedScripts")
            {
                return ETokenType.TOKEN_ScriptData;
            }
            else if (m_strTableName.Length == 0 &&
                     m_strFilePath.Length > 0)
            {
                return File.Exists (m_strFilePath) ? ETokenType.TOKEN_ValidFilePath : ETokenType.TOKEN_InvalidFilePath;
            }
            else if (m_strFilePath.Length == 0)
            {
                return ETokenType.TOKEN_Empty;
            }

            return ETokenType.TOKEN_Invalid;
        }

        //public string GetFileTokenKey () { return m_strFileTokenKey; }
        //public string GetTableName    () { return m_strTableName; }
        //public string GetDataName     () { return m_strDataName; }
        //public string GetFileFolder   () { return m_strFileFolder; }

        //public void SetFileTokenKey (string strFileTokenKey) { m_strFileTokenKey = strFileTokenKey; }
        //public void SetTableName    (string strTableName)    { m_strTableName    = strTableName; }
        //public void SetDataName     (string strDataName)     { m_strDataName     = strDataName; }
        //public void SetFileFolder   (string strFileFolder)   { m_strFileFolder   = strFileFolder; }
    }

    public class CDataBaseReaderWriter : CControlFlags
    {
        private   string m_strDatabasePath = "";
        protected SortedDictionary<string, CDBFileToken> m_sdFileTokens = new SortedDictionary<string,CDBFileToken> ();
        public    SortedDictionary<string, CDBFileToken> FileTokensDict
        {
            get { return m_sdFileTokens; }
        }

        private System.Data.OleDb.OleDbConnection m_odbConnection = new OleDbConnection ();

        public static EDatabaseTable TableStringToEnum (string strTableName)
        {
            if (strTableName == "CardData")
            {
                return EDatabaseTable.TABLE_CardData;
            }
            else if (strTableName == "CardObjectIPL")
            {
                return EDatabaseTable.TABLE_CardObjectIPL;
            }
            else if (strTableName == "CardObjectText")
            {
                return EDatabaseTable.TABLE_CardObjectText;
            }
            else if (strTableName == "CardRPGiiSource")
            {
                return EDatabaseTable.TABLE_CardRPGiiSource;
            }
            else if (strTableName == "DiskCreatedImages")
            {
                return EDatabaseTable.TABLE_DiskCreatedImages;
            }
            else if (strTableName == "DiskOriginalImages")
            {
                return EDatabaseTable.TABLE_DiskOriginalImages;
            }
            else if (strTableName == "SavedScripts")
            {
                return EDatabaseTable.TABLE_SavedScripts;
            }
            else if (strTableName == "ScriptingMacros")
            {
                return EDatabaseTable.TABLE_ScriptingMacros;
            }

            return EDatabaseTable.TABLE_Undefined;
        }

        public CDataBaseReaderWriter () { }

        public CDataBaseReaderWriter (string strDatabasePath)
        {
            m_strDatabasePath = strDatabasePath;
            m_odbConnection.ConnectionString = (string.Format ("Provider=Microsoft.ACE.OLEDB.12.0;Data Source='{0}';", strDatabasePath));
        }

        public void DBInit (string strDatabasePath)
        {
            if (strDatabasePath.Length > 0 &&
                (m_strDatabasePath != strDatabasePath ||
                 m_sdFileTokens.Count == 0))
            {
                m_strDatabasePath = strDatabasePath;
                m_sdFileTokens    = ReadFileTokens ();
            }
        }

        public SortedDictionary<string, CDBFileToken> ReadFileTokens (string strDatabasePath = "")
        {
            SortedDictionary<string, CDBFileToken> sdFileTokens = new SortedDictionary<string, CDBFileToken> ();

            if (strDatabasePath.Length == 0)
            {
                if (m_strDatabasePath.Length == 0)
                {
                    return sdFileTokens;
                }

                strDatabasePath = m_strDatabasePath;
            }

            try
            {
                System.Data.OleDb.OleDbConnection odbConnection = new System.Data.OleDb.OleDbConnection
                    (string.Format ("Provider=Microsoft.ACE.OLEDB.12.0;Data Source='{0}';", strDatabasePath));
                System.Data.OleDb.OleDbCommand odbCommand = new System.Data.OleDb.OleDbCommand ();

                string strSqlCommand = "SELECT * FROM FileTokens ORDER BY FileToken";
                OleDbDataAdapter ordAdapter = new OleDbDataAdapter (strSqlCommand, odbConnection);
                DataSet dsFT = new DataSet ("FileTokensSet");

                DataTable dtFT = dsFT.Tables.Add ("FileTokensTable");
                DataColumn dcdiDiskSectorPartOne   = dtFT.Columns.Add ("FileToken", typeof (string));
                DataColumn dcdiDiskSectorPartTwo   = dtFT.Columns.Add ("TableName", typeof (string));
                DataColumn dcdiDiskSectorPartThree = dtFT.Columns.Add ("DataName", typeof (string));
                DataColumn dcdiDiskSectorPartFour  = dtFT.Columns.Add ("FilePath", typeof (string));

                ordAdapter.Fill (dtFT);
                DataTableReader dtrFT = dtFT.CreateDataReader ();
                if (dtrFT.HasRows)
                {
                    while (dtrFT.Read ())
                    {
                        //Console.WriteLine ("FileToken: " + SafeGetString (dtrFT, 0));
                        //Console.WriteLine ("TableName: " + SafeGetString (dtrFT, 1));
                        //Console.WriteLine ("DataName:  " + SafeGetString (dtrFT, 2));
                        //Console.WriteLine ("FilePath:  " + SafeGetString (dtrFT, 3));
                        CDBFileToken ft = new CDBFileToken ();
                        ft.FileTokenKey = SafeGetString (dtrFT, 0);
                        ft.TableName = SafeGetString (dtrFT, 1);
                        ft.DataName = SafeGetString (dtrFT, 2);
                        ft.FilePath = SafeGetString (dtrFT, 3);
                        sdFileTokens.Add (ft.FileTokenKey, ft);
                        //Console.WriteLine ("Count: " + sdFileTokens.Count.ToString ());
                        //Console.WriteLine ();
                    }
                }

                odbConnection.Close ();
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
                return sdFileTokens;
            }

            return sdFileTokens;
        }

        public List<string> ReadListFromToken (string strFileToken)
        {
            List<string> lstrDataLines = new List<string> ();
            if (m_strDatabasePath.Length == 0 ||
                !m_sdFileTokens.ContainsKey (strFileToken))
            {
                return lstrDataLines;
            }

            CDBFileToken ft = m_sdFileTokens[strFileToken];

            string strSelectName  = "";
            string strOrderByName = "";
            string strDataName    = "";

            if (ft.TableName == "CardData")
            {
                strSelectName  = "DataFileName";
                strOrderByName = "CardSequence";
                strDataName    = "CardImage";
            }
            else if (ft.TableName == "CardObjectIPL")
            {
                strSelectName  = "ProgramName";
                strOrderByName = "CardSequence";
                strDataName    = "CardImage";
            }
            else if (ft.TableName == "CardObjectText")
            {
                strSelectName  = "ProgramName";
                strOrderByName = "CardSequence";
                strDataName    = "CardImage";
            }
            else if (ft.TableName == "CardRPGiiSource")
            {
                strSelectName  = "ProgramName";
                strOrderByName = "CardSequence";
                strDataName    = "CardImage";
            }
            else if (ft.TableName == "SavedScripts")
            {
                strSelectName  = "ScriptName";
                strOrderByName = "ScriptLineSequence";
                strDataName    = "ScriptLineText";
            }
            else if (ft.TableName == "ScriptingMacros")
            {
                strSelectName  = "MacroName";
                strOrderByName = "MacroLineSequence";
                strDataName    = "MacroLineText";
            }
            else
            {
                return lstrDataLines;
            }

            string strSqlCommand = string.Format ("SELECT {0}.[{3}] FROM {0} WHERE {1} = '{4}' Order by {2} ASC;",
                                                  ft.TableName, strSelectName, strOrderByName, strDataName, ft.DataName);

            try
            {
                if (m_odbConnection.ConnectionString.Length == 0)
                {
                    m_odbConnection.ConnectionString = (string.Format ("Provider=Microsoft.ACE.OLEDB.12.0;Data Source='{0}';", m_strDatabasePath));
                }

                OleDbDataAdapter ordAdapter = new OleDbDataAdapter (strSqlCommand, m_odbConnection);
                DataSet ds = new DataSet ("CardObjectIPLSet");

                DataTable dtIPL = ds.Tables.Add ("CardObjectIPLTable");
                DataColumn dciplCardImage = dtIPL.Columns.Add (strDataName, typeof (string));

                ordAdapter.Fill (dtIPL);
                DataTableReader dtr = dtIPL.CreateDataReader ();
                if (dtr.HasRows)
                {
                    while (dtr.Read ())
                    {
                        //lstrDataLines.Add (SqlInsertDecode (SafeGetString (dtr, 0)));
                        string strNewString = SafeGetString (dtr, 0);
                        if (strNewString.Length < 96)
                        {
                            strNewString += new string (' ', 96 - strNewString.Length);
                        }
                        lstrDataLines.Add (SqlInsertDecode (strNewString));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
                return lstrDataLines;
            }

            return lstrDataLines;
        }

        public byte[] ReadBinaryFromToken (string strFileToken)
        {
            List<byte> lyDiskImage = new List<byte> ();

            List<string> lstrDataLines = new List<string> ();
            if (m_strDatabasePath.Length == 0 ||
                !m_sdFileTokens.ContainsKey (strFileToken))
            {
                return lyDiskImage.ToArray ();
            }

            CDBFileToken ft = m_sdFileTokens[strFileToken];

            try
            {
                if (m_odbConnection.ConnectionString.Length == 0)
                {
                    m_odbConnection.ConnectionString = (string.Format ("Provider=Microsoft.ACE.OLEDB.12.0;Data Source='{0}';", m_strDatabasePath));
                }

                string strSqlCommand = string.Format ("SELECT {0}.[DiskSectorPartOne], {0}.[DiskSectorPartTwo], {0}.[DiskSectorPartThree], {0}.[DiskSectorPartFour]" +
                                                     "FROM {0} WHERE DiskImageName = '{1}' ORDER BY DiskRecordSequence", ft.TableName, ft.DataName);

                OleDbDataAdapter ordAdapter = new OleDbDataAdapter (strSqlCommand, m_odbConnection);
                DataSet ds = new DataSet ("DiskOriginalImagesSet");

                DataTable dtIPL = ds.Tables.Add ("DiskOriginalImagesTable");
                DataColumn dcdiDiskSectorPartOne   = dtIPL.Columns.Add ("DiskSectorPartOne", typeof (string));
                DataColumn dcdiDiskSectorPartTwo   = dtIPL.Columns.Add ("DiskSectorPartTwo", typeof (string));
                DataColumn dcdiDiskSectorPartThree = dtIPL.Columns.Add ("DiskSectorPartThree", typeof (string));
                DataColumn dcdiDiskSectorPartFour  = dtIPL.Columns.Add ("DiskSectorPartFour", typeof (string));

                ordAdapter.Fill (dtIPL);
                DataTableReader dtr = dtIPL.CreateDataReader ();
                if (dtr.HasRows)
                {
                    while (dtr.Read ())
                    {
                        CompressHexToByte (SafeGetString (dtr, 0), ref lyDiskImage);
                        CompressHexToByte (SafeGetString (dtr, 1), ref lyDiskImage);
                        CompressHexToByte (SafeGetString (dtr, 2), ref lyDiskImage);
                        CompressHexToByte (SafeGetString (dtr, 3), ref lyDiskImage);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
                return lyDiskImage.ToArray ();
            }

            return lyDiskImage.ToArray ();
        }

        public List<string> ReadCardFileToStringList (EDatabaseTable eDatabaseTable, string strDatabasePath, string strFilename)
        {
            List<string> lstrCardLines = new List<string> ();
            if (strDatabasePath.Length == 0)
            {
                if (m_strDatabasePath.Length == 0)
                {
                    return lstrCardLines;
                }

                strDatabasePath = m_strDatabasePath;
            }

            string strTableName   = "";
            string strSelectName  = "";
            string strOrderByName = "";
            string strDataName    = "";

            if (eDatabaseTable == EDatabaseTable.TABLE_CardData)
            {
                strTableName   = "CardData";
                strSelectName  = "DataFileName";
                strOrderByName = "CardSequence";
                strDataName    = "CardImage";
            }
            else if (eDatabaseTable == EDatabaseTable.TABLE_CardObjectIPL)
            {
                strTableName   = "CardObjectIPL";
                strSelectName  = "ProgramName";
                strOrderByName = "CardSequence";
                strDataName    = "CardImage";
            }
            else if (eDatabaseTable == EDatabaseTable.TABLE_CardObjectText)
            {
                strTableName   = "CardObjectText";
                strSelectName  = "ProgramName";
                strOrderByName = "CardSequence";
                strDataName    = "CardImage";
            }
            else if (eDatabaseTable == EDatabaseTable.TABLE_CardRPGiiSource)
            {
                strTableName   = "CardRPGiiSource";
                strSelectName  = "ProgramName";
                strOrderByName = "CardSequence";
                strDataName    = "CardImage";
            }
            else
            {
                return lstrCardLines;
            }

            string strSqlCommand = string.Format ("SELECT {0}.[{3}] FROM {0} WHERE {1} = '{4}' Order by {2} ASC;",
                                                  strTableName, strSelectName, strOrderByName, strDataName, strFilename);

            try
            {
                if (m_odbConnection.ConnectionString.Length == 0)
                {
                    m_odbConnection.ConnectionString = (string.Format ("Provider=Microsoft.ACE.OLEDB.12.0;Data Source='{0}';", strDatabasePath));
                }

                OleDbDataAdapter ordAdapter = new OleDbDataAdapter (strSqlCommand, m_odbConnection);
                DataSet ds = new DataSet ("CardObjectIPLSet");

                DataTable dtIPL = ds.Tables.Add ("CardObjectIPLTable");
                DataColumn dciplCardImage = dtIPL.Columns.Add ("CardImage", typeof (string));

                ordAdapter.Fill (dtIPL);
                DataTableReader dtr = dtIPL.CreateDataReader ();
                if (dtr.HasRows)
                {
                    while (dtr.Read ())
                    {
                        string strNewString = SafeGetString (dtr, 0);
                        if (strNewString.Length < 96)
                        {
                            strNewString += new string (' ', 96 - strNewString.Length);
                        }
                        lstrCardLines.Add (SqlInsertDecode (strNewString));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
                return lstrCardLines;
            }

            return lstrCardLines;
        }

        public byte[] ReadDiskImageToBinary (EDatabaseTable eDatabaseTable, string strDatabasePath, string strFilename)
        {
            List<byte> lyDiskImage = new List<byte> ();

            if (strDatabasePath.Length > 0)
            {
                if (m_strDatabasePath.Length == 0)
                {
                    return lyDiskImage.ToArray ();
                }

                strDatabasePath = m_strDatabasePath;
            }

            string strTableName = "";

            if (eDatabaseTable == EDatabaseTable.TABLE_DiskCreatedImages)
            {
                strTableName = "DiskCreatedImages";
            }
            else if (eDatabaseTable == EDatabaseTable.TABLE_DiskOriginalImages)
            {
                strTableName = "DiskOriginalImages";
            }
            else
            {
                return lyDiskImage.ToArray ();
            }

            try
            {
                if (m_odbConnection.ConnectionString.Length == 0)
                {
                    m_odbConnection.ConnectionString = (string.Format ("Provider=Microsoft.ACE.OLEDB.12.0;Data Source='{0}';", strDatabasePath));
                }

                string strSqlCommand = string.Format ("SELECT {0}.[DiskSectorPartOne], {0}.[DiskSectorPartTwo], {0}.[DiskSectorPartThree], {0}.[DiskSectorPartFour]" +
                                                     "FROM {0} WHERE DiskImageName = '{1}' ORDER BY DiskRecordSequence", strTableName, strFilename);

                OleDbDataAdapter ordAdapter = new OleDbDataAdapter (strSqlCommand, m_odbConnection);
                DataSet ds = new DataSet ("DiskOriginalImagesSet");

                DataTable dtIPL = ds.Tables.Add ("DiskOriginalImagesTable");
                DataColumn dcdiDiskSectorPartOne   = dtIPL.Columns.Add ("DiskSectorPartOne", typeof (string));
                DataColumn dcdiDiskSectorPartTwo   = dtIPL.Columns.Add ("DiskSectorPartTwo", typeof (string));
                DataColumn dcdiDiskSectorPartThree = dtIPL.Columns.Add ("DiskSectorPartThree", typeof (string));
                DataColumn dcdiDiskSectorPartFour  = dtIPL.Columns.Add ("DiskSectorPartFour", typeof (string));

                ordAdapter.Fill (dtIPL);
                DataTableReader dtr = dtIPL.CreateDataReader ();
                if (dtr.HasRows)
                {
                    while (dtr.Read ())
                    {
                        CompressHexToByte (SafeGetString (dtr, 0), ref lyDiskImage);
                        CompressHexToByte (SafeGetString (dtr, 1), ref lyDiskImage);
                        CompressHexToByte (SafeGetString (dtr, 2), ref lyDiskImage);
                        CompressHexToByte (SafeGetString (dtr, 3), ref lyDiskImage);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
                return lyDiskImage.ToArray ();
            }

            return lyDiskImage.ToArray ();
        }

        public List<string> ReadScriptDataToStringList (EDatabaseTable eDatabaseTable, string strDatabasePath, string strScriptName)
        {
            List<string> lstrScriptLines = new List<string> ();
            if (strDatabasePath.Length == 0)
            {
                if (m_strDatabasePath.Length == 0)
                {
                    return lstrScriptLines;
                }

                strDatabasePath = m_strDatabasePath;
            }

            System.Data.OleDb.OleDbConnection odbConnection;
            System.Data.OleDb.OleDbCommand odbCommand = new System.Data.OleDb.OleDbCommand ();
            string strTableName   = "";
            string strSelectName  = "";
            string strOrderByName = "";
            string strDataName    = "";

            if (eDatabaseTable == EDatabaseTable.TABLE_SavedScripts)
            {
                strTableName   = "SavedScripts";
                strSelectName  = "ScriptName";
                strOrderByName = "ScriptLineSequence";
                strDataName    = "ScriptLineText";
            }
            else if (eDatabaseTable == EDatabaseTable.TABLE_ScriptingMacros)
            {
                strTableName   = "ScriptingMacros";
                strSelectName  = "MacroName";
                strOrderByName = "MacroLineSequence";
                strDataName    = "MacroLineText";
            }
            else
            {
                return lstrScriptLines;
            }

            string strSqlCommand = string.Format ("SELECT {0}.[{3}] FROM {0} WHERE {1} = '{4}' Order by {2} ASC;",
                                                  strTableName, strSelectName, strOrderByName, strDataName, strScriptName);

            try
            {
                odbConnection = new System.Data.OleDb.OleDbConnection (string.Format ("Provider=Microsoft.ACE.OLEDB.12.0;Data Source='{0}';", strDatabasePath));

                OleDbDataAdapter ordAdapter = new OleDbDataAdapter (strSqlCommand, odbConnection);
                DataSet dsScript = new DataSet ("ScripSet");

                DataTable dtScript = dsScript.Tables.Add ("ScriptTable");
                DataColumn dciplScriptLine = dtScript.Columns.Add (strDataName, typeof (string));

                ordAdapter.Fill (dtScript);
                DataTableReader dtrScript = dtScript.CreateDataReader ();
                if (dtrScript.HasRows)
                {
                    while (dtrScript.Read ())
                    {
                        lstrScriptLines.Add (SqlInsertDecode (SafeGetString (dtrScript, 0)));
                    }
                }

                odbConnection.Close ();
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
                return lstrScriptLines;
            }

            return lstrScriptLines;
        }

        private string SafeGetString (DataTableReader dtr, int iIdx)
        {
            try
            {
                return (dtr.IsDBNull (iIdx)) ? "" : dtr.GetString (iIdx);
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
                return "";
            }
        }

        private string CompressHexToString (string strHexData)
        {
            StringBuilder sbldCompressedString = new StringBuilder (strHexData.Length / 2);

            for (int iIdx = 0; iIdx < strHexData.Length; iIdx += 2)
            {
                char cHexData = CompressHexToNybble (strHexData[iIdx]);
                cHexData <<= 4;
                cHexData += CompressHexToNybble (strHexData[iIdx + 1]);

                sbldCompressedString.Append (cHexData);
            }

            return sbldCompressedString.ToString ();
        }

        private char CompressHexToNybble (char cHexData)
        {
            if (cHexData >= '0' &&
                cHexData <= '9')
            {
                cHexData -= '0';
            }
            else if (cHexData >= 'a' &&
                     cHexData <= 'f')
            {
                cHexData -= 'a';
                cHexData += (char)0x0A;
            }
            else if (cHexData >= 'A' &&
                     cHexData <= 'F')
            {
                cHexData -= 'A';
                cHexData += (char)0x0A;
            }
            else
            {
                return ' ';
            }

            return cHexData;
        }

        private void CompressHexToByte (string strDiskImage, ref List<byte> lyDiskImage)
        {
            for (int iIdx = 0; iIdx < strDiskImage.Length; iIdx += 2)
            {
                char cHexData = CompressHexToNybble (strDiskImage[iIdx]);
                cHexData <<= 4;
                cHexData += CompressHexToNybble (strDiskImage[iIdx + 1]);

                lyDiskImage.Add ((byte)cHexData);
            }
        }

        private static string SqlInsertEncode (string strLine)
        {
            StringBuilder sbldPrepped = new StringBuilder (strLine.Length);

            for (int iIdx = 0; iIdx < strLine.Length; ++iIdx)
            {
                char cIndex = strLine[iIdx];

                if (cIndex == '"')
                {
                    cIndex = '[';
                }
                else if (cIndex == '\'')
                {
                    cIndex = ']';
                }

                sbldPrepped.Append (cIndex);
            }

            return sbldPrepped.ToString ();
        }

        private static string SqlInsertDecode (string strLine)
        {
            StringBuilder sbldPrepped = new StringBuilder (strLine.Length);

            for (int iIdx = 0; iIdx < strLine.Length; ++iIdx)
            {
                char cIndex = strLine[iIdx];

                if (cIndex == '[')
                {
                    cIndex = '"';
                }
                else if (cIndex == ']')
                {
                    cIndex = '\'';
                }

                sbldPrepped.Append (cIndex);
            }

            return sbldPrepped.ToString ();
        }
    }
}
