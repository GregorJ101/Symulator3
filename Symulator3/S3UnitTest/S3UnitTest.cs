using System;
using System.Collections.Generic;
using System.Text;

using EmulatorEngine;

namespace S3UnitTest
{
    class CS3UnitTest : CEmulatorEngine
    {
        static void Main (string[] args)
        {
            //bool bDoRegressionTests = false;

            //if (bDoRegressionTests)
            //{
            //    EarliestTesting ();
            //    TestSystem3ToolsLib (true);
            //}
            //else
            //{
            //    TestSystem3ToolsLib (false);
            //}

            //TestEmulatorMethods ();

            //TestDisassemblerLabels ();

            RunSystem3Diagnostics ();
        }

        #region Test Data
        // Test data
        byte[] m_yaObjectCode1 = { 0x04, 0x01, 0x10, 0x21, 0x30, 0x41, // ZAZ -6
                                   0x16, 0x02, 0x11, 0x22, 0x31,       // AZ  -5
                                   0x27, 0x03, 0x12, 0x23, 0x32,       // SZ  -5
                                   0x48, 0x04, 0x13, 0x24, 0x33,       // MVX -5
                                   0x5A, 0x05, 0x14, 0x25,             // ED  -4
                                   0x6B, 0x06, 0x15, 0x26,             // ITC -4
                                   0x8C, 0x07, 0x16, 0x27, 0x34,       // MVC -5
                                   0x9D, 0x07, 0x17, 0x28,             // CLC -4
                                   0xAE, 0x08, 0x18, 0x29,             // ALC -4
                                   0x30, 0x0A, 0x19, 0x2A,             // SNS -4
                                   0xC0, 0x87, 0x19, 0x2A,             // BC  -4
                                   0xD0, 0x87, 0x2A,                   // BC  -3
                                   0xE0, 0x87, 0x2A,                   // BC  -3
                                   0x31, 0x0B, 0x1A, 0x2A,             // LIO -4
                                   0x71, 0x0B, 0x1A,                   // LIO -3
                                   0xB4, 0x0C, 0x1B,                   // ST  -3
                                   0xF2, 0x87, 0x1C };                 // JC  -3

        byte[] m_yaLoopingHaltProgram =
            { 0xF0, 0x71, 0x71, 0xF0, 0x71, 0x1E, 0xF0, 0x1E, 0x1E, 0xF0, 0x1E, 0x71, 0xF0, 0x71, 0x71, 0xF0,
              0x75, 0x75, 0xF0, 0x75, 0x5E, 0xF0, 0x5E, 0x5E, 0xF0, 0x5E, 0x75, 0xF0, 0x75, 0x75, 0xF0, 0x40,
              0x40, 0xF0, 0x40, 0x10, 0xF0, 0x40, 0x04, 0xF0, 0x10, 0x40, 0xF0, 0x10, 0x10, 0xF0, 0x10, 0x04,
              0xF0, 0x04, 0x40, 0xF0, 0x04, 0x10, 0xF0, 0x04, 0x04, 0xF0, 0x7C, 0x63, 0xC0, 0x00, 0x00, 0x00 };

        byte[] m_yaCountingHaltProgramI =
            { 0xF0, 0x00, 0x00, 0x0E, 0x01, 0x00, 0x02, 0x00, 0x0E, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x01 };

        byte[] m_yaCountingHaltProgramII =
            { 0xC2, 0x01, 0x00, 0x01, 0xF0, 0x00, 0x00, 0x7A, 0x80, 0x05, 0x5E, 0x01, 0x05, 0x02, 0xD0, 0x00, 0x03 };

        byte[] m_yaCountingHaltProgramIV =
            { 0xF0, 0x00, 0x00, 0x0E, 0x00, 0x00, 0x01, 0x00, 0x06, 0x0E, 0x00, 0x00, 0x02, 0x00, 0x06, 0x3D,
              0x7E, 0x00, 0x02, 0xC0, 0x04, 0x00, 0x00, 0xF0, 0x7F, 0x7F, 0x0F, 0x00, 0x00, 0x18, 0x00, 0x06,
              0x0F, 0x00, 0x00, 0x19, 0x00, 0x06, 0x3D, 0x01, 0x00, 0x18, 0xC0, 0x02, 0x00, 0x17, 0x0C, 0x01,
              0x00, 0x19, 0x00, 0x02, 0x0C, 0x01, 0x00, 0x02, 0x00, 0x3F, 0xC0, 0x87, 0x00, 0x00, 0x00, 0x00 };

        byte[] m_yaCountingHaltProgramV =
            { 0xF0, 0x00, 0x01, 0x0E, 0x00, 0x00, 0x01, 0x00, 0x06, 0x0E, 0x00, 0x00, 0x02, 0x00, 0x06, 0x3D,
              0x7E, 0x00, 0x02, 0xC0, 0x04, 0x00, 0x00, 0xF0, 0x7F, 0x7E, 0x0F, 0x00, 0x00, 0x18, 0x00, 0x06,
              0x0F, 0x00, 0x00, 0x19, 0x00, 0x06, 0x3D, 0x01, 0x00, 0x18, 0xC0, 0x02, 0x00, 0x17, 0x0C, 0x01,
              0x00, 0x19, 0x00, 0x02, 0x0C, 0x01, 0x00, 0x02, 0x00, 0x3F, 0xC0, 0x87, 0x00, 0x00, 0x00, 0x01 };

        byte[] m_yaKeyboardClockProgramV = 
            { 0xC2, 0x01, 0x00, 0x00, 0xD2, 0x02, 0x36, 0x7C, 0x09, 0x14, 0x7C, 0x09, 0x10, 0x6C, 0x00, 0x54,
              0x00, 0x6C, 0x00, 0x53, 0x00, 0x71, 0x10, 0x54, 0x7C, 0xA1, 0x60, 0x5F, 0x02, 0x62, 0x04, 0xD0,
              0x84, 0x1B, 0x5F, 0x00, 0x10, 0x01, 0xD0, 0x85, 0x0D, 0x5F, 0x00, 0x14, 0x01, 0x7D, 0x03, 0x14,
              0xD0, 0x84, 0x0A, 0xD0, 0x00, 0x07, 0xF6, 0xFE, 0xA4, 0xDE, 0xD6, 0x74, 0xB6, 0xBA, 0x24, 0xEE,
              0xF1, 0x7D, 0xF4, 0xF5, 0xF1, 0x7F, 0x7C, 0xF1, 0x7D, 0x7C, 0xF1, 0x7F, 0xF7, 0x40, 0xF7, 0x7B,
              0xF4, 0xF1, 0xF5, 0xF8, 0xF5, 0x7B, 0xF4, 0x7A, 0xF1, 0x7E, 0x40, 0xF2, 0x7D, 0x7B, 0x7F, 0xF2 };

        byte[] m_yaPowersOfTwoProgramV =
            { 0xC2, 0x01, 0x00, 0x00, 0x7C, 0x40, 0xFF, 0x5C, 0x82, 0xFE, 0xFF, 0xF3, 0xE4, 0x06, 0x7C, 0xF1,
              0x50, 0x7C, 0x20, 0xB7, 0x5C, 0x20, 0xB6, 0xB7, 0x7C, 0x6B, 0xB4, 0x5C, 0x20, 0xB3, 0xB7, 0x5A,
              0x14, 0xB7, 0x50, 0x5B, 0x40, 0x90, 0x05, 0x71, 0xE6, 0x04, 0xF3, 0xE2, 0x01, 0xD1, 0xE6, 0x2D,
              0x56, 0x0F, 0x50, 0x50, 0xD0, 0x08, 0x11, 0xF3, 0xE4, 0x01, 0xF0, 0x7C, 0x63, 0xD0, 0x00, 0x04 };

        byte[] m_yaOneCardCoreDumpProgram = 
            { 0xC2, 0x01, 0xFF, 0xFF, 0x4C, 0x29, 0x6A, 0x01, 0x29, 0x5C, 0x05, 0x4F, 0x6A, 0xC2, 0x02, 0x00,
              0x00, 0x68, 0x02, 0x21, 0x00, 0x68, 0x03, 0x25, 0x00, 0xC2, 0x02, 0x00, 0xC0, 0x9C, 0x00, 0xCC,
              0x40, 0x9C, 0x00, 0xCD, 0x40, 0x5E, 0x01, 0x11, 0x3F, 0x5E, 0x00, 0x1D, 0x0F, 0xD0, 0x01, 0x01,
              0x7C, 0xC0, 0x1D, 0x71, 0xE6, 0x31, 0xF3, 0xE2, 0x02, 0xD1, 0xE6, 0x3A, 0xD0, 0x00, 0x01, 0xF0 };

        byte[] m_yaRipplePrintProgram =
            { 0xC2, 0x03, 0x01, 0x00, 0x74, 0x01, 0xFF, 0x36, 0x01, 0x00, 0x3A, 0xC0, 0x01, 0x00, 0x04, 0xAC,
              0x7F, 0x7F, 0xFF, 0xF3, 0xE0, 0x01, 0x6C, 0x83, 0xFF, 0xFF, 0x71, 0xE4, 0x03, 0x71, 0xE6, 0x38,
              0xF3, 0xE2, 0x01, 0xD1, 0xE2, 0x23, 0xAC, 0x00, 0x7B, 0xFF, 0xAC, 0x83, 0xFF, 0xFE, 0xB8, 0x0F,
              0x80, 0xD0, 0x10, 0x13, 0xD0, 0x87, 0x16, 0x00, 0x7C, 0xFF, 0xFF, 0x00, 0xF0, 0xC5, 0xF0, 0xC1 };

        byte[] m_ya24kDiskSystemInitialization =
            { 0x31, 0xE4, 0x00, 0x0F, 0x0C, 0x2F, 0x04, 0x2F, 0x00, 0x3F, 0x3C, 0x5F, 0x01, 0x7E, 0x04, 0x00,
              0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8, 0xF9, 0xF0, 0x7B, 0x7C, 0x61, 0xE2, 0xE3, 0xE4,
              0xE5, 0xE6, 0xE7, 0xE8, 0xE9, 0x50, 0x6B, 0x6C, 0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8,
              0xD9, 0x60, 0x5B, 0x5C, 0xC1, 0xC2, 0xC3, 0xC4, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9, 0x4E, 0x4B, 0x7D };

        byte[] m_yaOneCardPowersOfTwoProgram =
            { 0xC2, 0x01, 0x00, 0x00, 0x7C, 0x40, 0xDD, 0x5C, 0x9C, 0xDC, 0xDD, 0xF3, 0xE4, 0x07, 0x7C, 0xF1,
              0x50, 0x7C, 0x20, 0xB7, 0x5C, 0x20, 0xB6, 0xB7, 0x7C, 0x6B, 0xB4, 0x5C, 0x20, 0xB3, 0xB7, 0x5A,
              0x14, 0xB7, 0x50, 0x5B, 0x40, 0x90, 0x05, 0x71, 0xE6, 0x04, 0xF3, 0xE2, 0x01, 0xD1, 0xE6, 0x2D,
              0x56, 0x0F, 0x50, 0x50, 0xD0, 0x08, 0x11, 0xF3, 0xE4, 0x01, 0xF0, 0x7C, 0x63, 0xD0, 0x00, 0x3A };
        #endregion

        #region Test EBCDIC Characters
        //   !  is  0x3F  s/b 0x2A
        //   |  is  0x1A  s/b 0x3F
        //   ~  is  0x11  s/b 0x3A

        // Mismatch at index 42: ! 21 | 7C
        // Mismatch at index 58: ~ 7E . 2E
        // Mismatch at index 63: | 7C . 2E

        // Index 42 (4F) s/b ! 21 is | 7C original: !
        // Index 58 (A1) s/b ~ 7E is . 2E original: ~
        // Index 63 (6A) s/b | 7C is . 2E original: |

        //   !  is  0x3F  s/b 0x2A
        // Mismatch at index 42: ! 21 | 7C
        // Index 42 (4F) s/b ! 21 is | 7C original: !

        //   |  is  0x1A  s/b 0x3F
        // Mismatch at index 63: | 7C . 2E
        // Index 63 (6A) s/b | 7C is . 2E original: |

        //   ~  is  0x11  s/b 0x3A
        // Mismatch at index 58: ~ 7E . 2E
        // Index 58 (A1) s/b ~ 7E is . 2E original: ~

        //Mismatch at index 42: ! 21 | 7C
        //Mismatch at index 58: ~ 7E x 78
        //Mismatch at index 63: | 7C x 78
        static void TestEbcdicCharacters1 (CDataConversion objSystem3Tools)
        {
            // Test ASCII to EBCDIC conversion
            string strSystem3CharacterSet = " 123456789:#@'=\"0/STUVWXYZ&,%_>?-JKLMNOPQR!$*);^}ABCDEFGHI~.<(+|";
            for (int iIdx = 0; iIdx < strSystem3CharacterSet.Length; iIdx++)
            {
                char cEBCDIC1 = (char)objSystem3Tools.ConvertEBCDICtoASCII (objSystem3Tools.ConvertASCIItoEBCDIC ((byte)strSystem3CharacterSet[iIdx]));
                char cEBCDIC2 = objSystem3Tools.ConvertEbcdicToAsciiChar ((char)objSystem3Tools.ConvertASCIItoEBCDIC ((byte)strSystem3CharacterSet[iIdx]));
                if (cEBCDIC1 != cEBCDIC2)
                {
                    Console.WriteLine (string.Format ("Mismatch at index {0:D}: {1:S1} {2:X2} {3:S1} {4:X2}", iIdx, cEBCDIC1, (byte)cEBCDIC1, cEBCDIC2, (byte)cEBCDIC2));
                }
            }
        }

        //Index 42 (4F) s/b ! 21 is | 7C original: !
        //Index 58 (A1) s/b ~ 7E is x 78 original: ~
        //Index 63 (6A) s/b | 7C is x 78 original: |
        //Char: ! ASCII 21 -> EBCDIC 4F -> ASCII 7C / 21
        //Char: ~ ASCII 7E -> EBCDIC A1 -> ASCII 78 / 7E
        //Char: | ASCII 7C -> EBCDIC 6A -> ASCII 78 / 7C
        static void TestEbcdicCharacters2 (CDataConversion objSystem3Tools)
        {
            // Test ASCII to EBCDIC conversion
            string strSystem3CharacterSet = " 123456789:#@'=\"0/STUVWXYZ&,%_>?-JKLMNOPQR!$*);^}ABCDEFGHI~.<(+|";
            for (int iIdx = 0; iIdx < strSystem3CharacterSet.Length; iIdx++)
            {
                byte yEBCDIC = objSystem3Tools.ConvertASCIItoEBCDIC ((byte)strSystem3CharacterSet[iIdx]);
                char cASCII1 = (char)objSystem3Tools.ConvertEBCDICtoASCII (yEBCDIC);
                char cASCII2 = objSystem3Tools.ConvertEbcdicToAsciiChar ((char)yEBCDIC);
                if (cASCII1 != cASCII2)
                {
                    Console.WriteLine (string.Format ("Index {0:D} ({1:X2}) s/b {2:S1} {3:X2} is {4:S1} {5:X2} original: {6:S}",
                                                      iIdx, yEBCDIC, cASCII1, (byte)cASCII1, cASCII2, (byte)cASCII2, strSystem3CharacterSet[iIdx]));
                }
            }
        }

        //Char: ! ASCII 21 -> EBCDIC 4F -> ASCII 7C
        //Char: ~ ASCII 7E -> EBCDIC A1
        //Char: | ASCII 7C -> EBCDIC 6A
        static void TestEbcdicCharacters3 (CDataConversion objSystem3Tools)
        {
            // Test ASCII to EBCDIC conversion
            string strSystem3CharacterSet = " 123456789:#@'=\"0/STUVWXYZ&,%_>?-JKLMNOPQR!$*);^}ABCDEFGHI~.<(+|";
            for (int iIdx = 0; iIdx < strSystem3CharacterSet.Length; iIdx++)
            {
                byte yEBCDIC = objSystem3Tools.ConvertASCIItoEBCDIC ((byte)strSystem3CharacterSet[iIdx]);
                char cASCII1 = objSystem3Tools.ConvertEbcdicToAsciiChar ((char)yEBCDIC);
                byte cASCII2 = objSystem3Tools.ConvertEBCDICtoASCII (yEBCDIC);
                if (cASCII1 != strSystem3CharacterSet[iIdx])
                {
                    Console.WriteLine (string.Format ("Char: {0:S} ASCII {1:X2} -> EBCDIC {2:X2} -> ASCII {3:X2} / {4:X2}",
                                                       strSystem3CharacterSet[iIdx], (byte)strSystem3CharacterSet[iIdx],
                                                       yEBCDIC, (byte)cASCII1, cASCII2));
                }
            }
        }

        //static void TestEbcdicCharacters4 (CDataConversion objSystem3Tools)
        //{
        //    // Test ASCII to EBCDIC conversion
        //    string strSystem3CharacterSet = " 123456789:#@'=\"0/STUVWXYZ&,%_>?-JKLMNOPQR!$*);^}ABCDEFGHI~.<(+|";
        //    for (int iIdx = 0; iIdx < strSystem3CharacterSet.Length; iIdx++)
        //    {
        //        byte yEBCDIC1 = objSystem3Tools.ConvertASCIItoEBCDIC ((byte)strSystem3CharacterSet[iIdx]);
        //        byte yEBCDIC2 = (byte)objSystem3Tools.ConvertEbcdicToAsciiChar (strSystem3CharacterSet[iIdx]);
        //        byte yASCII1  = objSystem3Tools.ConvertEBCDICtoASCII (yEBCDIC1);
        //        byte yASCII2  = objSystem3Tools.ConvertEBCDICtoASCII (yEBCDIC2);

        //        if (yASCII1 != yASCII2)
        //        {
        //            Console.WriteLine (string.Format ("Char: {0:S} EBCDIC {1:X2} -> ASCII {2:X2}",
        //                                              strSystem3CharacterSet[iIdx], yEBCDIC1, yASCII1));
        //            if (yEBCDIC2 != 0x2E)
        //            {
        //                Console.WriteLine (string.Format ("        EBCDIC {0:X2} -> ASCII {1:X2}", yEBCDIC2, yASCII2));
        //            }
        //        }
        //    }
        //}

        static void TestEbcdicCharacters5 (CDataConversion objSystem3Tools)
        {
            // Test ASCII to EBCDIC conversion
            for (int iIdx = 0; iIdx < 0x0100; ++iIdx)
            {
                byte yAsciiValue = (byte)iIdx;
                byte yEbcdicValue = objSystem3Tools.ConvertASCIItoEBCDIC (yAsciiValue);
                byte yReturnValue = objSystem3Tools.ConvertEBCDICtoASCII (yEbcdicValue);
                byte yConverted = (byte)objSystem3Tools.ConvertEbcdicToAsciiChar ((char)yEbcdicValue);
                string strCompare = "";
                if (yConverted != '.')
                {
                    strCompare = (yConverted == yReturnValue) ? "match" : "mismatch";
                }
                Console.WriteLine ("{0:X2} {1:X2} {2:X2} {5:S1} {3:X2} {6:S1} {4:S}",
                                   yAsciiValue, yEbcdicValue, yReturnValue, yConverted,
                                   strCompare, (char)yReturnValue, (char)yConverted);
            }
        }

        static void TestEbcdicCharacters6 (CDataConversion objSystem3Tools)
        {
            // Test ASCII to EBCDIC conversion
            for (int iIdx = 0; iIdx < 0x0100; ++iIdx)
            {
                byte yAsciiValue = (byte)iIdx;
                byte yEbcdicValue = objSystem3Tools.ConvertASCIItoEBCDIC (yAsciiValue);
                byte yReturnValue = objSystem3Tools.ConvertEBCDICtoASCII (yEbcdicValue);
                byte yConverted = (byte)objSystem3Tools.ConvertEbcdicToAsciiChar ((char)yEbcdicValue);
                string strCompare = "";
                if (yConverted != '.')
                {
                    strCompare = (yConverted == yReturnValue) ? "match" : "mismatch";
                }
                Console.WriteLine ("{0:X2} {1:X2} {2:X2} {5:S1} {3:X2} {6:S1} {4:S}",
                                   yAsciiValue, yEbcdicValue, yReturnValue, yConverted,
                                   strCompare, (char)yReturnValue, (char)yConverted);
            }
        }

        //Mismatch at index 42: ! 21 | 7C
        //Mismatch at index 58: ~ 7E x 78
        //Mismatch at index 63: | 7C x 78
        static void TestEbcdicCharacters7 (CDataConversion objSystem3Tools)
        {
            // Test ASCII to EBCDIC conversion
            string strSystem3CharacterSet = " 123456789:#@'=\"0/STUVWXYZ&,%_>?-JKLMNOPQR!$*);^}ABCDEFGHI~.<(+|";
            for (int iIdx = 0; iIdx < strSystem3CharacterSet.Length; iIdx++)
            {
                char cEBCDIC1 = (char)objSystem3Tools.ConvertEBCDICtoASCII (objSystem3Tools.ConvertASCIItoEBCDIC ((byte)strSystem3CharacterSet[iIdx]));
                char cEBCDIC2 = objSystem3Tools.ConvertEbcdicToAsciiChar ((char)objSystem3Tools.ConvertASCIItoEBCDIC ((byte)strSystem3CharacterSet[iIdx]));
                if (cEBCDIC1 != cEBCDIC2)
                {
                    Console.WriteLine (string.Format ("Mismatch at index {0:D}: {1:S1} {2:X2} {3:S1} {4:X2}",
                                                      iIdx, cEBCDIC1, (byte)cEBCDIC1, cEBCDIC2, (byte)cEBCDIC2));
                }
            }
        }
        #endregion

        public void TestHaltDisplay ()
        {
            for (int iIdx = 0; iIdx < m_yaHaltCodes.Length; iIdx += 2)
            {
                PrintStringList (GetHaltDisplay (m_yaHaltCodes[iIdx], m_yaHaltCodes[iIdx + 1 < m_yaHaltCodes.Length ? iIdx + 1 : iIdx]));
            }

            foreach (byte yCode in m_yaHaltCodes)
            {
                PrintStringList (GetHaltDisplay (yCode, yCode));
            }

            for (byte yTestCode = (byte)0x01; yTestCode > 0x00; yTestCode <<= 1)
            {
                byte y5475Code = ConvertHaltCodeTo5745 (yTestCode);
                byte yHaltCode = Convert5745CodeToHalt (y5475Code);
                PrintStringList (GetHaltDisplay (yTestCode, yHaltCode));
            }

            foreach (byte yCode in m_yaHaltCodes)
            {
                byte y5475Code = ConvertHaltCodeTo5745 (yCode);
                byte yHaltCode = Convert5745CodeToHalt (y5475Code);
                PrintStringList (GetHaltDisplay (yCode, yHaltCode));
            }
        }

        public void Test5475Display ()
        {
            foreach (byte yCode in m_ya5475Codes)
            {
                PrintStringList (Get5475Display (yCode, yCode));
            }

            foreach (byte yCode in m_yaHaltCodes)
            {
                byte y5475Code = ConvertHaltCodeTo5745 (yCode);
                PrintStringList (Get5475Display (y5475Code, y5475Code));
            }

            for (byte yTestCode = (byte)0x01; yTestCode > 0x00; yTestCode <<= 1)
            {
                byte yHaltCode = Convert5745CodeToHalt (yTestCode);
                byte y5475Code = ConvertHaltCodeTo5745 (yHaltCode);
                PrintStringList (Get5475Display (yTestCode, y5475Code));
            }

            foreach (byte yCode in m_yaHaltCodes)
            {
                byte y5475Code = ConvertHaltCodeTo5745 (yCode);
                byte yHaltCode = Convert5745CodeToHalt (y5475Code);
                PrintStringList (GetHaltDisplay (yCode, yHaltCode));
                PrintStringList (Get5475Display (y5475Code, y5475Code));
            }

            foreach (byte yCode in m_yaHaltCodes)
            {
                byte y5475Code = ConvertHaltCodeTo5745 (yCode);
                Console.WriteLine ("0x{0:X2} = 0x{1:X2}", yCode, y5475Code);
            }
        }

        public void TestFetchAndGetAddress ()
        {
            LoadBinaryImage (m_yaObjectCode1, 0x0000, 0);
            m_iXR1 = 0x1000;
            m_iXR2 = 0x4000;

            byte[] yaInstruction = FetchInstruction (); // 0x04, 0x01, 0x10, 0x21, 0x30, 0x41, // ZAZ -6
            CTwoOperandAddress objTOA = GetTwoOperandAddress (yaInstruction);
            Console.WriteLine (string.Format ("{0:X4}  {1:X4}", objTOA.OperandOneAddress, objTOA.OperandTwoAddress));

            yaInstruction = FetchInstruction ();        // 0x16, 0x02, 0x11, 0x22, 0x31,       // AZ  -5
            objTOA = GetTwoOperandAddress (yaInstruction);
            Console.WriteLine (string.Format ("{0:X4}  {1:X4}", objTOA.OperandOneAddress, objTOA.OperandTwoAddress));

            yaInstruction = FetchInstruction ();        // 0x27, 0x03, 0x12, 0x23, 0x32,       // SZ  -5
            objTOA = GetTwoOperandAddress (yaInstruction);
            Console.WriteLine (string.Format ("{0:X4}  {1:X4}", objTOA.OperandOneAddress, objTOA.OperandTwoAddress));

            yaInstruction = FetchInstruction ();        // 0x48, 0x04, 0x13, 0x24, 0x33,       // MVX -5
            objTOA = GetTwoOperandAddress (yaInstruction);
            Console.WriteLine (string.Format ("{0:X4}  {1:X4}", objTOA.OperandOneAddress, objTOA.OperandTwoAddress));

            yaInstruction = FetchInstruction ();        // 0x5A, 0x05, 0x14, 0x25,             // ED  -4
            objTOA = GetTwoOperandAddress (yaInstruction);
            Console.WriteLine (string.Format ("{0:X4}  {1:X4}", objTOA.OperandOneAddress, objTOA.OperandTwoAddress));

            yaInstruction = FetchInstruction ();        // 0x6B, 0x06, 0x15, 0x26,             // ITC -4
            objTOA = GetTwoOperandAddress (yaInstruction);
            Console.WriteLine (string.Format ("{0:X4}  {1:X4}", objTOA.OperandOneAddress, objTOA.OperandTwoAddress));

            yaInstruction = FetchInstruction ();        // 0x8C, 0x07, 0x16, 0x27, 0x34,       // MVC -5
            objTOA = GetTwoOperandAddress (yaInstruction);
            Console.WriteLine (string.Format ("{0:X4}  {1:X4}", objTOA.OperandOneAddress, objTOA.OperandTwoAddress));

            yaInstruction = FetchInstruction ();        // 0x9D, 0x07, 0x17, 0x28,             // CLC -4
            objTOA = GetTwoOperandAddress (yaInstruction);
            Console.WriteLine (string.Format ("{0:X4}  {1:X4}", objTOA.OperandOneAddress, objTOA.OperandTwoAddress));

            yaInstruction = FetchInstruction ();        // 0xAE, 0x08, 0x18, 0x29,             // ALC -4
            objTOA = GetTwoOperandAddress (yaInstruction);
            Console.WriteLine (string.Format ("{0:X4}  {1:X4}", objTOA.OperandOneAddress, objTOA.OperandTwoAddress));


            yaInstruction = FetchInstruction ();        // 0x30, 0x0A, 0x19, 0x2A,             // SNS -4
            int iOperandAddress = GetOneOperandAddress (yaInstruction);
            Console.WriteLine (string.Format ("{0:X4}", iOperandAddress));

            yaInstruction = FetchInstruction ();        // 0xC0, 0x87, 0x19, 0x2A,             // BC  -4
            iOperandAddress = GetOneOperandAddress (yaInstruction);
            Console.WriteLine (string.Format ("{0:X4}", iOperandAddress));
            
            yaInstruction = FetchInstruction ();        // 0xD0, 0x87, 0x2A,                   // BC  -3
            iOperandAddress = GetOneOperandAddress (yaInstruction);
            Console.WriteLine (string.Format ("{0:X4}", iOperandAddress));
            
            yaInstruction = FetchInstruction ();        // 0xE0, 0x87, 0x2A,                   // BC  -3
            iOperandAddress = GetOneOperandAddress (yaInstruction);
            Console.WriteLine (string.Format ("{0:X4}", iOperandAddress));
            
            yaInstruction = FetchInstruction ();        // 0x31, 0x0B, 0x1A, 0x2A,             // LIO -4
            iOperandAddress = GetOneOperandAddress (yaInstruction);
            Console.WriteLine (string.Format ("{0:X4}", iOperandAddress));
            
            yaInstruction = FetchInstruction ();        // 0x71, 0x0B, 0x1A,                   // LIO -3
            iOperandAddress = GetOneOperandAddress (yaInstruction);
            Console.WriteLine (string.Format ("{0:X4}", iOperandAddress));
            
            yaInstruction = FetchInstruction ();        // 0xB4, 0x0C, 0x1B,                   // ST  -3
            iOperandAddress = GetOneOperandAddress (yaInstruction);
            Console.WriteLine (string.Format ("{0:X4}", iOperandAddress));
            

            yaInstruction = FetchInstruction ();        // 0xF2, 0x87, 0x1C };                 // JC  -3
            Console.WriteLine (string.Format ("{0:X2} {1:X2} {2:X2}", yaInstruction[0], yaInstruction[1], yaInstruction[2]));
        }

        public void TestConditionRegister ()
        {
            CConditionRegister objCR = new CConditionRegister ();

            CConditionRegister.SetTestMode ();

            Assert (objCR.IsEqual (), "");
            Assert (!objCR.IsHigh (), "");
            Assert (!objCR.IsLow (),  "");
            Assert (!objCR.IsTestFalse (), "");
            Assert (!objCR.IsBinaryOverflow(), "");
            Assert (!objCR.IsDecimalOverflow (), "");

            objCR.SetHigh ();
            Assert (!objCR.IsEqual (), "");
            Assert (objCR.IsHigh (), "");
            Assert (!objCR.IsLow (),  "");
            Assert (!objCR.IsTestFalse (), "");
            Assert (!objCR.IsBinaryOverflow(), "");
            Assert (!objCR.IsDecimalOverflow (), "");

            objCR.SetLow ();
            Assert (!objCR.IsEqual (), "");
            Assert (!objCR.IsHigh (), "");
            Assert (objCR.IsLow (), "");
            Assert (!objCR.IsTestFalse (), "");
            Assert (!objCR.IsBinaryOverflow (), "");
            Assert (!objCR.IsDecimalOverflow (), "");

            objCR.SetTestFalse ();
            Assert (!objCR.IsEqual (), "");
            Assert (!objCR.IsHigh (), "");
            Assert (objCR.IsLow (), "");
            Assert (objCR.IsTestFalse (), "");
            Assert (!objCR.IsBinaryOverflow (), "");
            Assert (!objCR.IsDecimalOverflow (), "");

            objCR.TestResetTestFalse ();
            Assert (!objCR.IsEqual (), "");
            Assert (!objCR.IsHigh (), "");
            Assert (objCR.IsLow (), "");
            Assert (!objCR.IsTestFalse (), "");
            Assert (!objCR.IsBinaryOverflow (), "");
            Assert (!objCR.IsDecimalOverflow (), "");

            objCR.SetBinaryOverflow ();
            Assert (!objCR.IsEqual (), "");
            Assert (!objCR.IsHigh (), "");
            Assert (objCR.IsLow (), "");
            Assert (!objCR.IsTestFalse (), "");
            Assert (objCR.IsBinaryOverflow (), "");
            Assert (!objCR.IsDecimalOverflow (), "");

            objCR.ResetBinaryOverflow ();
            Assert (!objCR.IsEqual (), "");
            Assert (!objCR.IsHigh (), "");
            Assert (objCR.IsLow (), "");
            Assert (!objCR.IsTestFalse (), "");
            Assert (!objCR.IsBinaryOverflow (), "");
            Assert (!objCR.IsDecimalOverflow (), "");

            objCR.SetDecimalOverflow ();
            Assert (!objCR.IsEqual (), "");
            Assert (!objCR.IsHigh (), "");
            Assert (objCR.IsLow (), "");
            Assert (!objCR.IsTestFalse (), "");
            Assert (!objCR.IsBinaryOverflow (), "");
            Assert (objCR.IsDecimalOverflow (), "");

            objCR.TestResetDecimalOverflow ();
            Assert (!objCR.IsEqual (), "");
            Assert (!objCR.IsHigh (), "");
            Assert (objCR.IsLow (), "");
            Assert (!objCR.IsTestFalse (), "");
            Assert (!objCR.IsBinaryOverflow (), "");
            Assert (!objCR.IsDecimalOverflow (), "");

            CConditionRegister.ResetTestMode ();
        }

        public void TestSupportMethods ()
        {
            // * * *  Test StoreZonedLong and LoadZonedLong  * * *
            // Store and retrive numeric value
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            int iTestValue = 1234;
            StoreInt (iTestValue, 0x000A, 4);
            int iResult = LoadInt (0x000A, 4);
            Assert (iResult == iTestValue);

            // * * *  Test StoreZonedLong and LoadZonedLong  * * *
            // Store and retrive positive numeric value
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            iTestValue = 1234567890;
            StoreZonedLong (iTestValue, 0x000A, 11);
            long lResult = LoadZonedLong (0x000A, 11);
            Assert (lResult == iTestValue);

            // Store and retrive negative numeric value
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            iTestValue = -1234567890;
            StoreZonedLong (iTestValue, 0x000A, 11);
            lResult = LoadZonedLong (0x000A, 11);
            Assert (lResult == iTestValue);

            // * * *  Test CompareZoned   * * *
            // Compare positive & positive  1 > 2  equal length
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (123, 0x0010, 3); // Operand one
            StoreZonedLong (122, 0x0020, 3); // Operand two
            iResult = CompareZoned (0x0010, 3, 0x0020, 3, false);
            Assert (iResult > 0);

            // Compare positive & positive  1 < 2  equal length
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (121, 0x0010, 3); // Operand one
            StoreZonedLong (122, 0x0020, 3); // Operand two
            iResult = CompareZoned (0x0010, 3, 0x0020, 3, false);
            Assert (iResult < 0);

            // Compare positive & negative  equal
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (122, 0x0010, 3); // Operand one
            StoreZonedLong (-122, 0x0020, 3); // Operand two
            iResult = CompareZoned (0x0010, 3, 0x0020, 3, false);
            Assert (iResult > 0);

            // Compare positive & negative  equal
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (-122, 0x0010, 3); // Operand one
            StoreZonedLong (122, 0x0020, 3); // Operand two
            iResult = CompareZoned (0x0010, 3, 0x0020, 3, false);
            Assert (iResult < 0);

            // Compare positive & negative  equal, ignore sign
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (122, 0x0010, 3); // Operand one
            StoreZonedLong (-122, 0x0020, 3); // Operand two
            iResult = CompareZoned (0x0010, 3, 0x0020, 3, true);
            Assert (iResult == 0);

            // Compare negative & positive  2 > 1  equal length
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (122, 0x0010, 3); // Operand one
            StoreZonedLong (-123, 0x0020, 3); // Operand two
            iResult = CompareZoned (0x0010, 3, 0x0020, 3, true);
            Assert (iResult < 0);

            // Compare negative & negative  2 < 1
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (-124, 0x0010, 3); // Operand one
            StoreZonedLong (-123, 0x0020, 3); // Operand two
            iResult = CompareZoned (0x0010, 3, 0x0020, 3, true);
            Assert (iResult > 0);

            // Compare negative & negative  2 < 1  Operand 1 longer w/ significant digit beyond operand 2
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (-12456, 0x0010, 5); // Operand one
            StoreZonedLong (-123, 0x0020, 3); // Operand two
            iResult = CompareZoned (0x0010, 3, 0x0020, 3, true);
            Assert (iResult > 0);

            // Compare negative & negative  2 < 1  Operand 1 longer w/ leading zeros, first significant digit same as operand 2
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (-124, 0x0010, 5); // Operand one
            StoreZonedLong (-123, 0x0020, 3); // Operand two
            iResult = CompareZoned (0x0010, 3, 0x0020, 3, true);
            Assert (iResult > 0);

            // * * *  Test StoreString / LoadString  * * *
            string strTest = "0123456789";
            StoreString (strTest, 0x0020);
            string strReturn = LoadString (0x0020, 10);
            Assert (strTest == strReturn);

            // * * *  Test StoreStringEBCDIC / LoadStringEBCDIC  * * *
            strTest = "0123456789";
            StoreStringEBCDIC (strTest, 0x0020);
            strReturn = LoadStringEBCDIC (0x0020, 10);
            Assert (strTest == strReturn);
        }

        // Archived early CPU unit test code
        //void DumpSector (byte[] yaSector, List<string> strlSectorReading)
        //{
        //    for (int iOuterIdx = 0; iOuterIdx < yaSector.Length; iOuterIdx += 16)
        //    {
        //        StringBuilder strbldDumpLine = new StringBuilder (string.Format ("{0:x4}: ", iOuterIdx));
        //        for (int iInnerIdx = 0; iInnerIdx < 16; ++iInnerIdx)
        //        {
        //            if (iInnerIdx > 0 &&
        //                iInnerIdx % 4 == 0)
        //            {
        //                strbldDumpLine.Append (" ");
        //            }

        //            if (iInnerIdx > 0 &&
        //                iInnerIdx % 8 == 0)
        //            {
        //                strbldDumpLine.Append (" ");
        //            }

        //            char cLeft  = (char)(yaSector[iOuterIdx + iInnerIdx] >> 4);
        //            char cRight = (char)(yaSector[iOuterIdx + iInnerIdx] & 0x0F);
        //            strbldDumpLine.Append (GetHexChar (cLeft));
        //            strbldDumpLine.Append (GetHexChar (cRight));
        //        }

        //        strlSectorReading.Add (strbldDumpLine.ToString ());
        //    }

        //    strlSectorReading.Add ("");
        //}

        //public void TestConditionRegister ()
        //{
        //    CConditionRegister objCR = new CConditionRegister ();

        //    Assert (objCR.IsEqual (), "");
        //    Assert (!objCR.IsHigh (), "");
        //    Assert (!objCR.IsLow (),  "");
        //    Assert (!objCR.IsTestFalse (), "");
        //    Assert (!objCR.IsBinaryOverflow(), "");
        //    Assert (!objCR.IsDecimalOverflow (), "");

        //    objCR.SetHigh ();
        //    Assert (!objCR.IsEqual (), "");
        //    Assert (objCR.IsHigh (), "");
        //    Assert (!objCR.IsLow (),  "");
        //    Assert (!objCR.IsTestFalse (), "");
        //    Assert (!objCR.IsBinaryOverflow(), "");
        //    Assert (!objCR.IsDecimalOverflow (), "");

        //    objCR.SetLow ();
        //    Assert (!objCR.IsEqual (), "");
        //    Assert (!objCR.IsHigh (), "");
        //    Assert (objCR.IsLow (), "");
        //    Assert (!objCR.IsTestFalse (), "");
        //    Assert (!objCR.IsBinaryOverflow (), "");
        //    Assert (!objCR.IsDecimalOverflow (), "");

        //    objCR.SetTestFalse ();
        //    Assert (!objCR.IsEqual (), "");
        //    Assert (!objCR.IsHigh (), "");
        //    Assert (objCR.IsLow (), "");
        //    Assert (objCR.IsTestFalse (), "");
        //    Assert (!objCR.IsBinaryOverflow (), "");
        //    Assert (!objCR.IsDecimalOverflow (), "");

        //    objCR.TestResetTestFalse ();
        //    Assert (!objCR.IsEqual (), "");
        //    Assert (!objCR.IsHigh (), "");
        //    Assert (objCR.IsLow (), "");
        //    Assert (!objCR.IsTestFalse (), "");
        //    Assert (!objCR.IsBinaryOverflow (), "");
        //    Assert (!objCR.IsDecimalOverflow (), "");

        //    objCR.SetBinaryOverflow ();
        //    Assert (!objCR.IsEqual (), "");
        //    Assert (!objCR.IsHigh (), "");
        //    Assert (objCR.IsLow (), "");
        //    Assert (!objCR.IsTestFalse (), "");
        //    Assert (objCR.IsBinaryOverflow (), "");
        //    Assert (!objCR.IsDecimalOverflow (), "");

        //    objCR.ResetBinaryOverflow ();
        //    Assert (!objCR.IsEqual (), "");
        //    Assert (!objCR.IsHigh (), "");
        //    Assert (objCR.IsLow (), "");
        //    Assert (!objCR.IsTestFalse (), "");
        //    Assert (!objCR.IsBinaryOverflow (), "");
        //    Assert (!objCR.IsDecimalOverflow (), "");

        //    objCR.SetDecimalOverflow ();
        //    Assert (!objCR.IsEqual (), "");
        //    Assert (!objCR.IsHigh (), "");
        //    Assert (objCR.IsLow (), "");
        //    Assert (!objCR.IsTestFalse (), "");
        //    Assert (!objCR.IsBinaryOverflow (), "");
        //    Assert (objCR.IsDecimalOverflow (), "");

        //    objCR.TestResetDecimalOverflow ();
        //    Assert (!objCR.IsEqual (), "");
        //    Assert (!objCR.IsHigh (), "");
        //    Assert (objCR.IsLow (), "");
        //    Assert (!objCR.IsTestFalse (), "");
        //    Assert (!objCR.IsBinaryOverflow (), "");
        //    Assert (!objCR.IsDecimalOverflow (), "");
        //}

        //public void TestFetchAndGetAddress ()
        //{
        //    m_yaMainMemory = yaObjectCode1;
        //    m_iXR1 = 0x1000;
        //    m_iXR2 = 0x4000;

        //    byte[] yaInstruction = FetchInstruction (); // 0x04, 0x01, 0x10, 0x21, 0x30, 0x41, // ZAZ -6
        //    CTwoOperandAddress objTOA = GetTwoOperandAddress (yaInstruction);
        //    Console.WriteLine (string.Format ("{0:X4}  {1:X4}", objTOA.OperandOneAddress, objTOA.OperandTwoAddress));

        //    yaInstruction = FetchInstruction ();        // 0x16, 0x02, 0x11, 0x22, 0x31,       // AZ  -5
        //    objTOA = GetTwoOperandAddress (yaInstruction);
        //    Console.WriteLine (string.Format ("{0:X4}  {1:X4}", objTOA.OperandOneAddress, objTOA.OperandTwoAddress));

        //    yaInstruction = FetchInstruction ();        // 0x27, 0x03, 0x12, 0x23, 0x32,       // SZ  -5
        //    objTOA = GetTwoOperandAddress (yaInstruction);
        //    Console.WriteLine (string.Format ("{0:X4}  {1:X4}", objTOA.OperandOneAddress, objTOA.OperandTwoAddress));

        //    yaInstruction = FetchInstruction ();        // 0x48, 0x04, 0x13, 0x24, 0x33,       // MVX -5
        //    objTOA = GetTwoOperandAddress (yaInstruction);
        //    Console.WriteLine (string.Format ("{0:X4}  {1:X4}", objTOA.OperandOneAddress, objTOA.OperandTwoAddress));

        //    yaInstruction = FetchInstruction ();        // 0x5A, 0x05, 0x14, 0x25,             // ED  -4
        //    objTOA = GetTwoOperandAddress (yaInstruction);
        //    Console.WriteLine (string.Format ("{0:X4}  {1:X4}", objTOA.OperandOneAddress, objTOA.OperandTwoAddress));

        //    yaInstruction = FetchInstruction ();        // 0x6B, 0x06, 0x15, 0x26,             // ITC -4
        //    objTOA = GetTwoOperandAddress (yaInstruction);
        //    Console.WriteLine (string.Format ("{0:X4}  {1:X4}", objTOA.OperandOneAddress, objTOA.OperandTwoAddress));

        //    yaInstruction = FetchInstruction ();        // 0x8C, 0x07, 0x16, 0x27, 0x34,       // MVC -5
        //    objTOA = GetTwoOperandAddress (yaInstruction);
        //    Console.WriteLine (string.Format ("{0:X4}  {1:X4}", objTOA.OperandOneAddress, objTOA.OperandTwoAddress));

        //    yaInstruction = FetchInstruction ();        // 0x9D, 0x07, 0x17, 0x28,             // CLC -4
        //    objTOA = GetTwoOperandAddress (yaInstruction);
        //    Console.WriteLine (string.Format ("{0:X4}  {1:X4}", objTOA.OperandOneAddress, objTOA.OperandTwoAddress));

        //    yaInstruction = FetchInstruction ();        // 0xAE, 0x08, 0x18, 0x29,             // ALC -4
        //    objTOA = GetTwoOperandAddress (yaInstruction);
        //    Console.WriteLine (string.Format ("{0:X4}  {1:X4}", objTOA.OperandOneAddress, objTOA.OperandTwoAddress));


        //    yaInstruction = FetchInstruction ();        // 0x30, 0x0A, 0x19, 0x2A,             // SNS -4
        //    int iOperandAddress = GetOneOperandAddress (yaInstruction);
        //    Console.WriteLine (string.Format ("{0:X4}", iOperandAddress));

        //    yaInstruction = FetchInstruction ();        // 0xC0, 0x87, 0x19, 0x2A,             // BC  -4
        //    iOperandAddress = GetOneOperandAddress (yaInstruction);
        //    Console.WriteLine (string.Format ("{0:X4}", iOperandAddress));

        //    yaInstruction = FetchInstruction ();        // 0xD0, 0x87, 0x2A,                   // BC  -3
        //    iOperandAddress = GetOneOperandAddress (yaInstruction);
        //    Console.WriteLine (string.Format ("{0:X4}", iOperandAddress));

        //    yaInstruction = FetchInstruction ();        // 0xE0, 0x87, 0x2A,                   // BC  -3
        //    iOperandAddress = GetOneOperandAddress (yaInstruction);
        //    Console.WriteLine (string.Format ("{0:X4}", iOperandAddress));

        //    yaInstruction = FetchInstruction ();        // 0x31, 0x0B, 0x1A, 0x2A,             // LIO -4
        //    iOperandAddress = GetOneOperandAddress (yaInstruction);
        //    Console.WriteLine (string.Format ("{0:X4}", iOperandAddress));

        //    yaInstruction = FetchInstruction ();        // 0x71, 0x0B, 0x1A,                   // LIO -3
        //    iOperandAddress = GetOneOperandAddress (yaInstruction);
        //    Console.WriteLine (string.Format ("{0:X4}", iOperandAddress));

        //    yaInstruction = FetchInstruction ();        // 0xB4, 0x0C, 0x1B,                   // ST  -3
        //    iOperandAddress = GetOneOperandAddress (yaInstruction);
        //    Console.WriteLine (string.Format ("{0:X4}", iOperandAddress));


        //    yaInstruction = FetchInstruction ();        // 0xF2, 0x87, 0x1C };                 // JC  -3
        //}

        //public void TestInstructions ()
        //{
        //    CConditionRegister.InTest = true;

        //    TestA ();
        //    TestST ();
        //    TestL ();
        //    TestALC ();
        //    TestJC ();
        //    TestBC ();
        //    TestCLC ();
        //    TestCLI ();
        //    TestTBF ();
        //    TestTBN ();
        //    TestMVC ();
        //    TestMVX ();
        //    TestSBF ();
        //    TestSBN ();
        //    TestMVI ();
        //    TestLA ();

        //    CConditionRegister.InTest = false;
        //}

        //public void TestA ()
        //{
        //    // A
        //    //case 0x36:
        //}

        //public void TestST ()
        //{
        //    // ST
        //    //case 0x34:
        //}

        //public void TestL ()
        //{
        //    SystemReset ();
        //    m_iXR1 = 0;
        //    m_iXR2 = 0;
        //    m_yaMainMemory[0x0000] = 0x10;
        //    m_yaMainMemory[0x0001] = 0x20;
        //    m_yaMainMemory[0x0002] = 0x30;
        //    m_yaMainMemory[0x0003] = 0x40;
        //    m_yaMainMemory[0x0004] = 0x50;
        //    m_yaMainMemory[0x0005] = 0x60;
        //    m_yaMainMemory[0x0006] = 0x70;
        //    m_yaMainMemory[0x0007] = 0x00;
        //    m_yaMainMemory[0x0008] = 0x3A;

        //    // XR1
        //    byte[] yaL1 = { 0x35, 0x01, 0x00, 0x01 };
        //    ExecuteInstruction (yaL1);
        //    Assert (m_iXR1 == 0x1020);
        //    Assert (m_iXR2 == 0x0000);
        //    Assert (m_iARR == 0x0000);
        //    Assert (m_iIAR == 0x0000);
        //    Assert (m_aCR[m_iIL].IsEqual ());
        //    Assert (!m_aCR[m_iIL].IsLow ());
        //    Assert (!m_aCR[m_iIL].IsHigh ());
        //    Assert (!m_aCR[m_iIL].IsDecimalOverflow ());
        //    Assert (!m_aCR[m_iIL].IsTestFalse ());
        //    Assert (!m_aCR[m_iIL].IsBinaryOverflow ());

        //    // XR2
        //    byte[] yaL2 = { 0x35, 0x02, 0x00, 0x02 };
        //    ExecuteInstruction (yaL2);
        //    Assert (m_iXR1 == 0x1020);
        //    Assert (m_iXR2 == 0x2030);
        //    Assert (m_iARR == 0x0000);
        //    Assert (m_iIAR == 0x0000);
        //    Assert (m_aCR[m_iIL].IsEqual ());
        //    Assert (!m_aCR[m_iIL].IsLow ());
        //    Assert (!m_aCR[m_iIL].IsHigh ());
        //    Assert (!m_aCR[m_iIL].IsDecimalOverflow ());
        //    Assert (!m_aCR[m_iIL].IsTestFalse ());
        //    Assert (!m_aCR[m_iIL].IsBinaryOverflow ());

        //    // CR (PSR)
        //    byte[] yaL3 = { 0x35, 0x04, 0x00, 0x08 };
        //    ExecuteInstruction (yaL3);
        //    Assert (m_iXR1 == 0x1020);
        //    Assert (m_iXR2 == 0x2030);
        //    Assert (m_iARR == 0x0000);
        //    Assert (m_iIAR == 0x0000);
        //    Assert (!m_aCR[m_iIL].IsEqual ());
        //    Assert (m_aCR[m_iIL].IsLow ());
        //    Assert (!m_aCR[m_iIL].IsHigh ());
        //    Assert (m_aCR[m_iIL].IsDecimalOverflow ());
        //    Assert (m_aCR[m_iIL].IsTestFalse ());
        //    Assert (m_aCR[m_iIL].IsBinaryOverflow ());

        //    // ARR
        //    byte[] yaL4 = { 0x35, 0x08, 0x00, 0x03 };
        //    ExecuteInstruction (yaL4);
        //    Assert (m_iXR1 == 0x1020);
        //    Assert (m_iXR2 == 0x2030);
        //    Assert (m_iARR == 0x3040);
        //    Assert (m_iIAR == 0x0000);

        //    // IAR
        //    byte[] yaL5 = { 0x35, 0x10, 0x00, 0x04 };
        //    ExecuteInstruction (yaL5);
        //    Assert (m_iXR1 == 0x1020);
        //    Assert (m_iXR2 == 0x2030);
        //    Assert (m_iARR == 0x3040);
        //    Assert (m_iIAR == 0x4050);

        //    // IAR PL1
        //    byte[] yaL6 = { 0x35, 0x20, 0x00, 0x05 };
        //    ExecuteInstruction (yaL6);
        //    Assert (m_iXR1 == 0x1020);
        //    Assert (m_iXR2 == 0x2030);
        //    Assert (m_iARR == 0x3040);
        //    Assert (m_iIAR == 0x5060);

        //    // IAR PL2
        //    byte[] yaL7 = { 0x35, 0x40, 0x00, 0x05 };
        //    ExecuteInstruction (yaL7);
        //    Assert (m_eProgramState == EProgramState.STATE_PChk_PL2_Unsupported);
        //}

        //public void TestALC ()
        //{
        //    // Positive result, no carry
        //    m_yaMainMemory[0x0002] = 0x0F;
        //    m_yaMainMemory[0x0003] = 0xFF;
        //    m_yaMainMemory[0x0005] = 0x00;
        //    m_yaMainMemory[0x0006] = 0x01;
        //    byte[] yaALC1 = { 0x0E, 0x01, 0x00, 0x03, 0x00, 0x06 };
        //    ExecuteInstruction (yaALC1);
        //    Assert (m_yaMainMemory[0x0002] == 0x10);
        //    Assert (m_yaMainMemory[0x0003] == 0x00);
        //    Assert (m_yaMainMemory[0x0005] == 0x00);
        //    Assert (m_yaMainMemory[0x0006] == 0x01);
        //    Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
        //    Assert (m_aCR[m_iIL].IsLow ());

        //    // Binary Overflow, zero result
        //    m_yaMainMemory[0x0002] = 0xFF;
        //    m_yaMainMemory[0x0003] = 0xFF;
        //    m_yaMainMemory[0x0005] = 0x00;
        //    m_yaMainMemory[0x0006] = 0x01;
        //    byte[] yaALC2 = { 0x0E, 0x01, 0x00, 0x03, 0x00, 0x06 };
        //    ExecuteInstruction (yaALC2);
        //    Assert (m_yaMainMemory[0x0002] == 0x00);
        //    Assert (m_yaMainMemory[0x0003] == 0x00);
        //    Assert (m_yaMainMemory[0x0005] == 0x00);
        //    Assert (m_yaMainMemory[0x0006] == 0x01);
        //    Assert (m_aCR[m_iIL].IsBinaryOverflow ());
        //    Assert (m_aCR[m_iIL].IsEqual ());

        //    // Binary Overflow, non-zero result
        //    m_yaMainMemory[0x0002] = 0xFF;
        //    m_yaMainMemory[0x0003] = 0xFF;
        //    m_yaMainMemory[0x0005] = 0x01;
        //    m_yaMainMemory[0x0006] = 0x00;
        //    byte[] yaALC3 = { 0x0E, 0x01, 0x00, 0x03, 0x00, 0x06 };
        //    ExecuteInstruction (yaALC3);
        //    Assert (m_yaMainMemory[0x0002] == 0x00);
        //    Assert (m_yaMainMemory[0x0003] == 0xFF);
        //    Assert (m_yaMainMemory[0x0005] == 0x01);
        //    Assert (m_yaMainMemory[0x0006] == 0x00);
        //    Assert (m_aCR[m_iIL].IsBinaryOverflow ());
        //    Assert (m_aCR[m_iIL].IsHigh ());
        //}

        //public void TestJC ()
        //{
        //    m_aCR[m_iIL].SetEqual ();
        //    m_aCR[m_iIL].SetDecimalOverflow ();
        //    m_aCR[m_iIL].SetTestFalse ();
        //    m_aCR[m_iIL].SetBinaryOverflow ();

        //    m_iARR = 0x0000;
        //    m_iIAR = 0x0100;

        //    // Test NoOp
        //    byte[] yaBC1 = { 0xF2, 0x80, 0x20 };
        //    ExecuteInstruction (yaBC1);
        //    Assert (m_iARR == 0x0000);
        //    Assert (m_iIAR == 0x0100);

        //    byte[] yaBC2 = { 0xF2, 0x07, 0x20 };
        //    ExecuteInstruction (yaBC2);
        //    Assert (m_iARR == 0x0000);
        //    Assert (m_iIAR == 0x0100);

        //    // Test Unconditional
        //    byte[] yaBC3 = { 0xF2, 0x00, 0x20 };
        //    ExecuteInstruction (yaBC3);
        //    Assert (m_iARR == 0x0100);
        //    Assert (m_iIAR == 0x0120);

        //    byte[] yaBC4 = { 0xF2, 0x87, 0x20 };
        //    ExecuteInstruction (yaBC4);
        //    Assert (m_iARR == 0x0120);
        //    Assert (m_iIAR == 0x0140);

        //    m_iARR = 0x0000;
        //    m_iIAR = 0x0100;

        //    // Test On Equal
        //    byte[] yaBC5 = { 0xF2, 0x81, 0x20 };
        //    ExecuteInstruction (yaBC5);
        //    Assert (m_iARR == 0x0100);
        //    Assert (m_iIAR == 0x0120);

        //    m_iARR = 0x0000;
        //    m_iIAR = 0x0100;

        //    // Test Not Equal
        //    byte[] yaBC6 = { 0xF2, 0x01, 0x20 };
        //    ExecuteInstruction (yaBC6);
        //    Assert (m_iARR == 0x0000);
        //    Assert (m_iIAR == 0x0100);

        //    // Test Not High
        //    byte[] yaBC7 = { 0xF2, 0x04, 0x20 };
        //    ExecuteInstruction (yaBC7);
        //    Assert (m_iARR == 0x0100);
        //    Assert (m_iIAR == 0x0120);

        //    m_iARR = 0x0000;
        //    m_iIAR = 0x0100;

        //    // Test On BinaryOverflow (no reset)
        //    byte[] yaBC8 = { 0xF2, 0xA0, 0x20 };
        //    ExecuteInstruction (yaBC8);
        //    Assert (m_iARR == 0x0100);
        //    Assert (m_iIAR == 0x0120);

        //    m_iARR = 0x0000;
        //    m_iIAR = 0x0100;

        //    // Test On DecimalOverflow (reset)
        //    byte[] yaBC9 = { 0xF2, 0x88, 0x20 };
        //    ExecuteInstruction (yaBC9);
        //    Assert (m_iARR == 0x0100);
        //    Assert (m_iIAR == 0x0120);
        //    Assert (!m_aCR[m_iIL].IsDecimalOverflow ());

        //    m_iARR = 0x0000;
        //    m_iIAR = 0x0100;

        //    // Test On TestFalse (reset)
        //    byte[] yaBC10 = { 0xF2, 0x90, 0x20 };
        //    ExecuteInstruction (yaBC10);
        //    Assert (m_iARR == 0x0100);
        //    Assert (m_iIAR == 0x0120);
        //    Assert (!m_aCR[m_iIL].IsTestFalse ());
        //}

        //public void TestBC ()
        //{
        //    m_aCR[m_iIL].SetEqual ();
        //    m_aCR[m_iIL].SetDecimalOverflow ();
        //    m_aCR[m_iIL].SetTestFalse ();
        //    m_aCR[m_iIL].SetBinaryOverflow ();

        //    m_iARR = 0x0000;
        //    m_iIAR = 0x0100;

        //    // Test NoOp
        //    byte[] yaBC1 = { 0xC0, 0x80, 0x02, 0x00 };
        //    ExecuteInstruction (yaBC1);
        //    Assert (m_iARR == 0x0000);
        //    Assert (m_iIAR == 0x0100);

        //    byte[] yaBC2 = { 0xC0, 0x07, 0x02, 0x00 };
        //    ExecuteInstruction (yaBC2);
        //    Assert (m_iARR == 0x0000);
        //    Assert (m_iIAR == 0x0100);

        //    // Test Unconditional
        //    byte[] yaBC3 = { 0xC0, 0x00, 0x02, 0x00 };
        //    ExecuteInstruction (yaBC3);
        //    Assert (m_iARR == 0x0100);
        //    Assert (m_iIAR == 0x0200);

        //    byte[] yaBC4 = { 0xC0, 0x87, 0x03, 0x00 };
        //    ExecuteInstruction (yaBC4);
        //    Assert (m_iARR == 0x0200);
        //    Assert (m_iIAR == 0x0300);

        //    m_iARR = 0x0000;
        //    m_iIAR = 0x0100;

        //    // Test On Equal
        //    byte[] yaBC5 = { 0xC0, 0x81, 0x02, 0x00 };
        //    ExecuteInstruction (yaBC5);
        //    Assert (m_iARR == 0x0100);
        //    Assert (m_iIAR == 0x0200);

        //    m_iARR = 0x0000;
        //    m_iIAR = 0x0100;

        //    // Test Not Equal
        //    byte[] yaBC6 = { 0xC0, 0x01, 0x02, 0x00 };
        //    ExecuteInstruction (yaBC6);
        //    Assert (m_iARR == 0x0000);
        //    Assert (m_iIAR == 0x0100);

        //    // Test Not High
        //    byte[] yaBC7 = { 0xC0, 0x04, 0x02, 0x00 };
        //    ExecuteInstruction (yaBC7);
        //    Assert (m_iARR == 0x0100);
        //    Assert (m_iIAR == 0x0200);

        //    m_iARR = 0x0000;
        //    m_iIAR = 0x0100;

        //    // Test On BinaryOverflow (no reset)
        //    byte[] yaBC8 = { 0xC0, 0xA0, 0x02, 0x00 };
        //    ExecuteInstruction (yaBC8);
        //    Assert (m_iARR == 0x0100);
        //    Assert (m_iIAR == 0x0200);

        //    m_iARR = 0x0000;
        //    m_iIAR = 0x0100;

        //    // Test On DecimalOverflow (reset)
        //    byte[] yaBC9 = { 0xC0, 0x88, 0x02, 0x00 };
        //    ExecuteInstruction (yaBC9);
        //    Assert (m_iARR == 0x0100);
        //    Assert (m_iIAR == 0x0200);
        //    Assert (!m_aCR[m_iIL].IsDecimalOverflow ());

        //    m_iARR = 0x0000;
        //    m_iIAR = 0x0100;

        //    // Test On TestFalse (reset)
        //    byte[] yaBC10 = { 0xC0, 0x90, 0x02, 0x00 };
        //    ExecuteInstruction (yaBC10);
        //    Assert (m_iARR == 0x0100);
        //    Assert (m_iIAR == 0x0200);
        //    Assert (!m_aCR[m_iIL].IsTestFalse ());
        //}

        //public void TestCLC ()
        //{
        //    // Reference
        //    m_yaMainMemory[0x1230] = 0x12;
        //    m_yaMainMemory[0x1231] = 0x34;

        //    // Test equal
        //    m_yaMainMemory[0x1232] = 0x12;
        //    m_yaMainMemory[0x1233] = 0x34;

        //    // Test high
        //    m_yaMainMemory[0x1234] = 0x13;
        //    m_yaMainMemory[0x1235] = 0x34;

        //    // Test high
        //    m_yaMainMemory[0x1236] = 0x12;
        //    m_yaMainMemory[0x1237] = 0x35;

        //    // Test low
        //    m_yaMainMemory[0x1238] = 0x11;
        //    m_yaMainMemory[0x1239] = 0x34;

        //    byte[] yaCLC1 = { 0x0D, 0x01, 0x12, 0x31, 0x12, 0x31 };
        //    ExecuteInstruction (yaCLC1);
        //    Assert (!m_aCR[m_iIL].IsLow ());
        //    Assert (m_aCR[m_iIL].IsEqual ());
        //    Assert (!m_aCR[m_iIL].IsHigh ());

        //    byte[] yaCLC2 = { 0x0D, 0x01, 0x12, 0x33, 0x12, 0x31 };
        //    ExecuteInstruction (yaCLC2);
        //    Assert (!m_aCR[m_iIL].IsLow ());
        //    Assert (m_aCR[m_iIL].IsEqual ());
        //    Assert (!m_aCR[m_iIL].IsHigh ());

        //    byte[] yaCLC3 = { 0x0D, 0x01, 0x12, 0x35, 0x12, 0x31 };
        //    ExecuteInstruction (yaCLC3);
        //    Assert (!m_aCR[m_iIL].IsLow ());
        //    Assert (!m_aCR[m_iIL].IsEqual ());
        //    Assert (m_aCR[m_iIL].IsHigh ());

        //    byte[] yaCLC4 = { 0x0D, 0x01, 0x12, 0x37, 0x12, 0x31 };
        //    ExecuteInstruction (yaCLC4);
        //    Assert (!m_aCR[m_iIL].IsLow ());
        //    Assert (!m_aCR[m_iIL].IsEqual ());
        //    Assert (m_aCR[m_iIL].IsHigh ());

        //    byte[] yaCLC5 = { 0x0D, 0x01, 0x12, 0x39, 0x12, 0x31 };
        //    ExecuteInstruction (yaCLC5);
        //    Assert (m_aCR[m_iIL].IsLow ());
        //    Assert (!m_aCR[m_iIL].IsEqual ());
        //    Assert (!m_aCR[m_iIL].IsHigh ());
        //}

        //public void TestCLI ()
        //{
        //    m_yaMainMemory[0x1230] = 0x12;
        //    byte[] yaCLI1 = { 0x3D, 0x13, 0x12, 0x30 };
        //    ExecuteInstruction (yaCLI1);
        //    Assert (m_aCR[m_iIL].IsLow ());
        //    Assert (!m_aCR[m_iIL].IsEqual ());
        //    Assert (!m_aCR[m_iIL].IsHigh ());

        //    byte[] yaCLI2 = { 0x3D, 0x12, 0x12, 0x30 };
        //    ExecuteInstruction (yaCLI2);
        //    Assert (!m_aCR[m_iIL].IsLow ());
        //    Assert (m_aCR[m_iIL].IsEqual ());
        //    Assert (!m_aCR[m_iIL].IsHigh ());

        //    byte[] yaCLI3 = { 0x3D, 0x11, 0x12, 0x30 };
        //    ExecuteInstruction (yaCLI3);
        //    Assert (!m_aCR[m_iIL].IsLow ());
        //    Assert (!m_aCR[m_iIL].IsEqual ());
        //    Assert (m_aCR[m_iIL].IsHigh ());
        //}

        //public void TestTBN ()
        //{
        //    m_aCR[m_iIL].TestResetTestFalse ();

        //    m_yaMainMemory[0x1230] = 0x12;
        //    byte[] yaTBN1 = { 0x38, 0x00, 0x12, 0x30 };
        //    ExecuteInstruction (yaTBN1);
        //    Assert (!m_aCR[m_iIL].IsTestFalse ());

        //    byte[] yaTBN2 = { 0x38, 0x12, 0x12, 0x30 };
        //    ExecuteInstruction (yaTBN2);
        //    Assert (!m_aCR[m_iIL].IsTestFalse ());

        //    byte[] yaTBN3 = { 0x38, 0x02, 0x12, 0x30 };
        //    ExecuteInstruction (yaTBN3);
        //    Assert (!m_aCR[m_iIL].IsTestFalse ());

        //    byte[] yaTBN4 = { 0x38, 0x10, 0x12, 0x30 };
        //    ExecuteInstruction (yaTBN4);
        //    Assert (!m_aCR[m_iIL].IsTestFalse ());

        //    byte[] yaTBN5 = { 0x38, 0x13, 0x12, 0x30 };
        //    ExecuteInstruction (yaTBN5);
        //    Assert (m_aCR[m_iIL].IsTestFalse ());
        //}

        //public void TestTBF ()
        //{
        //    m_aCR[m_iIL].TestResetTestFalse ();

        //    m_yaMainMemory[0x1230] = 0x12;
        //    byte[] yaTBN1 = { 0x39, 0x00, 0x12, 0x30 };
        //    ExecuteInstruction (yaTBN1);
        //    Assert (!m_aCR[m_iIL].IsTestFalse ());

        //    byte[] yaTBN2 = { 0x39, 0x20, 0x12, 0x30 };
        //    ExecuteInstruction (yaTBN2);
        //    Assert (!m_aCR[m_iIL].IsTestFalse ());

        //    byte[] yaTBN3 = { 0x39, 0xE0, 0x12, 0x30 };
        //    ExecuteInstruction (yaTBN3);
        //    Assert (!m_aCR[m_iIL].IsTestFalse ());

        //    byte[] yaTBN4 = { 0x39, 0xED, 0x12, 0x30 };
        //    ExecuteInstruction (yaTBN4);
        //    Assert (!m_aCR[m_iIL].IsTestFalse ());

        //    byte[] yaTBN5 = { 0x39, 0x10, 0x12, 0x30 };
        //    ExecuteInstruction (yaTBN5);
        //    Assert (m_aCR[m_iIL].IsTestFalse ());

        //    m_aCR[m_iIL].TestResetTestFalse ();
        //    byte[] yaTBN6 = { 0x39, 0x02, 0x12, 0x30 };
        //    ExecuteInstruction (yaTBN6);
        //    Assert (m_aCR[m_iIL].IsTestFalse ());
        //}

        //public void TestMVC ()
        //{
        //    m_yaMainMemory[0x1230] = 0x12;
        //    m_yaMainMemory[0x1231] = 0x23;
        //    m_yaMainMemory[0x1232] = 0x34;
        //    m_yaMainMemory[0x1233] = 0x45;
        //    m_yaMainMemory[0x1234] = 0x00;
        //    m_yaMainMemory[0x1235] = 0x00;
        //    m_yaMainMemory[0x1236] = 0x00;
        //    m_yaMainMemory[0x1237] = 0x00;
        //    m_yaMainMemory[0x1238] = 0x00;

        //    byte[] yaMVC1 = { 0x0C, 0x00, 0x12, 0x34, 0x12, 0x31 };
        //    ExecuteInstruction (yaMVC1);
        //    Assert (m_yaMainMemory[0x1234] == 0x23);

        //    byte[] yaMVC2 = { 0x0C, 0x03, 0x12, 0x38, 0x12, 0x33 };
        //    ExecuteInstruction (yaMVC2);
        //    Assert (m_yaMainMemory[0x1230] == 0x12);
        //    Assert (m_yaMainMemory[0x1231] == 0x23);
        //    Assert (m_yaMainMemory[0x1232] == 0x34);
        //    Assert (m_yaMainMemory[0x1233] == 0x45);
        //    Assert (m_yaMainMemory[0x1235] == 0x12);
        //    Assert (m_yaMainMemory[0x1236] == 0x23);
        //    Assert (m_yaMainMemory[0x1237] == 0x34);
        //    Assert (m_yaMainMemory[0x1238] == 0x45);

        //    // Test propogration of overlapping operands
        //    m_yaMainMemory[0x0000] = 0x00;
        //    m_yaMainMemory[0x0001] = 0x00;
        //    m_yaMainMemory[0x0002] = 0x00;
        //    m_yaMainMemory[0x0003] = 0x00;
        //    m_yaMainMemory[0x0004] = 0x00;
        //    m_yaMainMemory[0x0005] = 0x00;
        //    m_yaMainMemory[0x0006] = 0x00;
        //    m_yaMainMemory[0x0007] = 0x34;
        //    m_yaMainMemory[0x0008] = 0x45;
        //    byte[] yaMVC3 = { 0x0C, 0x05, 0x00, 0x06, 0x00, 0x08 };
        //    ExecuteInstruction (yaMVC3);
        //    Assert (m_yaMainMemory[0x0001] == 0x34);
        //    Assert (m_yaMainMemory[0x0002] == 0x45);
        //    Assert (m_yaMainMemory[0x0003] == 0x34);
        //    Assert (m_yaMainMemory[0x0004] == 0x45);
        //    Assert (m_yaMainMemory[0x0005] == 0x34);
        //    Assert (m_yaMainMemory[0x0006] == 0x45);
        //    Assert (m_yaMainMemory[0x0007] == 0x34);
        //    Assert (m_yaMainMemory[0x0008] == 0x45);
        //}

        //public void TestMVX ()
        //{
        //    m_yaMainMemory[0x1230] = 0x12;
        //    m_yaMainMemory[0x1231] = 0x23;
        //    m_yaMainMemory[0x1232] = 0x34;
        //    m_yaMainMemory[0x1233] = 0x45;
        //    m_yaMainMemory[0x1234] = 0x56;
        //    m_yaMainMemory[0x1235] = 0x67;
        //    m_yaMainMemory[0x1236] = 0x78;
        //    m_yaMainMemory[0x1237] = 0x89;
        //    m_yaMainMemory[0x1238] = 0xAF;

        //    byte[] yaMVX1 = { 0x08, 0x00, 0x12, 0x30, 0x12, 0x38 };
        //    ExecuteInstruction (yaMVX1);
        //    Assert (m_yaMainMemory[0x1230] == 0xA2);

        //    byte[] yaMVX2 = { 0x08, 0x01, 0x12, 0x31, 0x12, 0x38 };
        //    ExecuteInstruction (yaMVX2);
        //    Assert (m_yaMainMemory[0x1231] == 0xF3);

        //    byte[] yaMVX3 = { 0x08, 0x02, 0x12, 0x32, 0x12, 0x38 };
        //    ExecuteInstruction (yaMVX3);
        //    Assert (m_yaMainMemory[0x1232] == 0x3A);

        //    byte[] yaMVX4 = { 0x08, 0x03, 0x12, 0x33, 0x12, 0x38 };
        //    ExecuteInstruction (yaMVX4);
        //    Assert (m_yaMainMemory[0x1233] == 0x4F);

        //    m_iXR1 = 0x1200;
        //    m_iXR2 = 0x1232;

        //    m_yaMainMemory[0x1230] = 0x12;
        //    m_yaMainMemory[0x1231] = 0x23;
        //    m_yaMainMemory[0x1232] = 0x34;
        //    m_yaMainMemory[0x1233] = 0x45;
        //    m_yaMainMemory[0x1234] = 0x56;
        //    m_yaMainMemory[0x1235] = 0x67;
        //    m_yaMainMemory[0x1236] = 0x78;
        //    m_yaMainMemory[0x1237] = 0x89;
        //    m_yaMainMemory[0x1238] = 0xAF;

        //    byte[] yaMVX5 = { 0x58, 0x00, 0x30, 0x38 }; // XR1 & XR1
        //    ExecuteInstruction (yaMVX5);
        //    Assert (m_yaMainMemory[0x1230] == 0xA2);

        //    byte[] yaMVX6 = { 0x68, 0x01, 0x31, 0x06 }; // XR1 & XR2
        //    ExecuteInstruction (yaMVX6);
        //    Assert (m_yaMainMemory[0x1231] == 0xF3);

        //    byte[] yaMVX7 = { 0x98, 0x02, 0x00, 0x38 }; // XR2 & XR1
        //    ExecuteInstruction (yaMVX7);
        //    Assert (m_yaMainMemory[0x1232] == 0x3A);

        //    byte[] yaMVX8 = { 0xA8, 0x03, 0x01, 0x06 }; // XR2 & XR2
        //    ExecuteInstruction (yaMVX8);
        //    Assert (m_yaMainMemory[0x1233] == 0x4F);
        //}

        //public void TestSBN ()
        //{
        //    byte[] yaSBN1 = { 0x3A, 0x10, 0x12, 0x34 };
        //    m_yaMainMemory[0x1234] = 0x0F;
        //    ExecuteInstruction (yaSBN1);
        //    Assert (m_yaMainMemory[0x1234] == 0x1F);

        //    m_yaMainMemory[0x1234] = 0x10;
        //    ExecuteInstruction (yaSBN1);
        //    Assert (m_yaMainMemory[0x1234] == 0x10);

        //    m_iXR1 = 0x0300;
        //    m_iXR2 = 0x0400;
        //    byte[] yaSBN2 = { 0x7A, 0x20, 0x11 };

        //    m_yaMainMemory[0x0311] = 0x0F;
        //    ExecuteInstruction (yaSBN2);
        //    Assert (m_yaMainMemory[0x0311] == 0x2F);

        //    m_yaMainMemory[0x0311] = 0x030;
        //    ExecuteInstruction (yaSBN2);
        //    Assert (m_yaMainMemory[0x0311] == 0x30);

        //    byte[] yaSBN3 = { 0xBA, 0x30, 0x22 };
        //    m_yaMainMemory[0x0422] = 0x40;
        //    ExecuteInstruction (yaSBN3);
        //    Assert (m_yaMainMemory[0x0422] == 0x70);

        //    m_yaMainMemory[0x0422] = 0xB0;
        //    ExecuteInstruction (yaSBN3);
        //    Assert (m_yaMainMemory[0x0422] == 0xB0);

        //    byte[] yaSBN4 = { 0xBA, 0x00, 0x23 };
        //    m_yaMainMemory[0x0423] = 0xFF;
        //    ExecuteInstruction (yaSBN3);
        //    Assert (m_yaMainMemory[0x0423] == 0xFF);
        //}

        //public void TestSBF ()
        //{
        //    byte[] yaSBF1 = { 0x3B, 0x10, 0x12, 0x34 };
        //    m_yaMainMemory[0x1234] = 0x3F;
        //    ExecuteInstruction (yaSBF1);
        //    Assert (m_yaMainMemory[0x1234] == 0x2F);

        //    m_iXR1 = 0x0500;
        //    m_iXR2 = 0x0600;
        //    byte[] yaSBF2 = { 0x7B, 0x10, 0x31 };
        //    m_yaMainMemory[0x0531] = 0x3F;
        //    ExecuteInstruction (yaSBF2);
        //    Assert (m_yaMainMemory[0x531] == 0x2F);

        //    m_yaMainMemory[0x0531] = 0x4F;
        //    ExecuteInstruction (yaSBF2);
        //    Assert (m_yaMainMemory[0x531] == 0x4F);

        //    byte[] yaSBF3 = { 0xBB, 0x10, 0x32 };
        //    m_yaMainMemory[0x0632] = 0x30;
        //    ExecuteInstruction (yaSBF3);
        //    Assert (m_yaMainMemory[0x0632] == 0x20);

        //    m_yaMainMemory[0x0632] = 0x22;
        //    ExecuteInstruction (yaSBF3);
        //    Assert (m_yaMainMemory[0x0632] == 0x22);

        //    byte[] yaSBF4 = { 0xBB, 0x00, 0x32 };
        //    m_yaMainMemory[0x0632] = 0x22;
        //    ExecuteInstruction (yaSBF4);
        //    Assert (m_yaMainMemory[0x0632] == 0x22);
        //}

        //public void TestMVI ()
        //{
        //    byte[] yaMVI1 = { 0x3C, 0xAA, 0x12, 0x34 };
        //    ExecuteInstruction (yaMVI1);
        //    Assert (m_yaMainMemory[0x1234] == 0xAA);

        //    m_iXR1 = 0x0100;
        //    m_iXR2 = 0x0200;
        //    byte[] yaMVI2 = { 0x7C, 0xBB, 0x11 };
        //    ExecuteInstruction (yaMVI2);
        //    Assert (m_yaMainMemory[0x0111] == 0xBB);

        //    byte[] yaMVI3 = { 0xBC, 0xCC, 0x22 };
        //    ExecuteInstruction (yaMVI3);
        //    Assert (m_yaMainMemory[0x0222] == 0xCC);
        //}

        //public void TestLA ()
        //{
        //    byte[] yaLA1 = { 0xC2, 0x03, 0x01, 0x00 };
        //    ExecuteInstruction (yaLA1);
        //    Assert (m_iXR1 == 0x0100);
        //    Assert (m_iXR2 == 0x0100);

        //    byte[] yaLA2 = { 0xD2, 0x01, 0x51 };
        //    ExecuteInstruction (yaLA2);
        //    Assert (m_iXR1 == 0x0151);
        //    Assert (m_iXR2 == 0x0100);

        //    byte[] yaLA3 = { 0xE2, 0x02, 0x80 };
        //    ExecuteInstruction (yaLA3);
        //    Assert (m_iXR1 == 0x0151);
        //    Assert (m_iXR2 == 0x0180);
        //}

        //private void Assert (bool bTest)
        //{
        //    if (!bTest)
        //    {
        //        WaveRedFlag ("Assertion Failure");
        //    }
        //}

        //private void Assert (bool bTest, string strMessage)
        //{
        //    if (!bTest)
        //    {
        //        WaveRedFlag (strMessage);
        //    }
        //}

        public void TestCPUInstructions ()
        {
            CConditionRegister.SetTestMode ();
            //m_yaMainMemory = new byte[0x8000];

            TestITC ();
            TestED (); // Produces 5 "Invalid Q Byte" errors
            TestSZ ();
            TestAZ ();
            TestZAZ ();
            TestSLC ();
            TestA (); // Produces "Attempt to use PL2" error
            TestST (); // Produces "Address Wraparound" and "Attempt to use PL2" errors
            TestL (); // Produces "Attempt to use PL2" error
            TestALC ();
            TestJC ();
            TestBC ();
            TestCLC ();
            TestCLI ();
            TestTBF ();
            TestTBN ();
            TestMVC ();
            TestMVX ();
            TestSBF ();
            TestSBN ();
            TestMVI ();
            TestLA ();

            CConditionRegister.ResetTestMode ();
        }

        public void TestInstructions5203LinePrinter ()
        {
            TestLIO5203 ();
            TestSIO5203 ();
            TestSNS5203 ();
            TestTIO5203 ();
            TestAPL5203 ();
        }

        public void TestInstructions5424MFCU ()
        { 
            TestLIO5424 ();
            TestSIO5424 ();
            TestSNS5424 ();
            TestTIO5424 ();
            TestAPL5424 ();
        }

        public void TestInstructions5444DiskDrive ()
        {
            TestLIO5444 ();
            TestSIO5444 ();
            TestSNS5444 ();
            TestTIO5444 ();
            TestAPL5444 ();
        }

        public void TestInstructions5475DataEntryKeyboard ()
        {
            TestLIO5475 ();
            TestSIO5475 ();
            TestSNS5475 ();
            TestTIO5475 ();
            TestAPL5475 ();
        }

        public void TestInstructions5471KeyboardPrinter ()
        {
            TestLIO5471 ();
            TestSIO5471 ();
            TestSNS5471 ();
            TestTIO5471 ();
            TestAPL5471 ();
        }

        public void TestRun (byte[] yaProgramImage)
        {
            Initialize (EBootDevice.BOOT_Card);

            //List<string> strlDump = BinaryToDump (m_yaMainMemory, 0, 95);
            //foreach (string str in strlDump)
            //{
            //    Console.WriteLine (str);
            //}

            LoadBinaryImage (yaProgramImage, 0x0000, 0);

            //strlDump = BinaryToDump (m_yaMainMemory, 0, 95);
            //foreach (string str in strlDump)
            //{
            //    Console.WriteLine (str);
            //}

            SystemReset ();
            Run (0x0000);
        }

        #region Test CPU Instructions
        public void TestITC ()
        {
            SystemReset ();

            // Define strings   0123456789abcde
            string strMask1  = "$  , 12,545.99",
                   strMask2  = "$  ,   ,   .  ",
                   strMask3  = "*****12,545.99",
                   strMask4  = "??????????????",
                   strFiller = "*?";
            StoreStringEBCDIC (strFiller, 0x0008);

            // Test populated string
            SystemReset (); // Resets CR, ARR, ...
            StoreStringEBCDIC (strMask1, 0x0010);
            byte[] yaITC1 = { 0x0B, 0x0D, 0x00, 0x10, 0x00, 0x08 };
            ExecuteInstruction (yaITC1);
            string strResult = LoadStringEBCDIC (0x0010, 0x0E);
            Assert (strResult == strMask3);
            Assert (m_iARR == 0x0015);
            Assert (m_aCR[m_iIL].IsEqual ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());

            // Test empty string
            SystemReset (); // Resets CR, ARR, ...
            StoreStringEBCDIC (strMask2, 0x0010);
            byte[] yaITC2 = { 0x0B, 0x0D, 0x00, 0x10, 0x00, 0x09 };
            ExecuteInstruction (yaITC2);
            strResult = LoadStringEBCDIC (0x0010, 0x0E);
            Assert (strResult == strMask4);
            Assert (m_iARR == 0x001E);
            Assert (m_aCR[m_iIL].IsEqual ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
        }

        public void TestED () // Produces 5 "Invalid Q Byte" errors
        {
            SystemReset ();

            // Test overlapping operands
            //   0123456789abcdef
            // 1 +--------+
            // 2       +--------+
            byte[] yaED1 = { 0x0A, 0x09, 0x00, 0x19, 0x00, 0x1F };
            ExecuteInstruction (yaED1);
            Assert (ProgramState == EProgramState.STATE_PChk_InvalidQByte);
            TestResetProgramState ();

            //   0123456789abcdef
            // 1 +-----+
            // 2       +-----+
            byte[] yaED2 = { 0x0A, 0x06, 0x00, 0x16, 0x00, 0x1C };
            ExecuteInstruction (yaED2);
            Assert (ProgramState == EProgramState.STATE_PChk_InvalidQByte);
            TestResetProgramState ();

            //   0123456789abcdef
            // 1 +-----+
            // 2 +-----+
            byte[] yaED3 = { 0x0A, 0x06, 0x00, 0x16, 0x00, 0x16 };
            ExecuteInstruction (yaED3);
            Assert (ProgramState == EProgramState.STATE_PChk_InvalidQByte);
            TestResetProgramState ();

            //   0123456789abcdef
            // 1    +-----+
            // 2 +-----+
            byte[] yaED4 = { 0x0A, 0x06, 0x00, 0x19, 0x00, 0x16 };
            ExecuteInstruction (yaED4);
            Assert (ProgramState == EProgramState.STATE_PChk_InvalidQByte);
            TestResetProgramState ();

            //   0123456789abcdef
            // 1       +-----+
            // 2 +-----+
            byte[] yaED5 = { 0x0A, 0x06, 0x00, 0x1C, 0x00, 0x16 };
            ExecuteInstruction (yaED5);
            Assert (ProgramState == EProgramState.STATE_PChk_InvalidQByte);
            TestResetProgramState ();

            // Test with positive value
            byte[] yaED6 = { 0x0A, 0x0C, 0x00, 0x1D, 0x00, 0x2D };
            // 0123456789abcd
            // $  ,   ,   .  
            // 0123456789abcd
            // 123456654321
            FillArea (0x0010, 0x02F, 0x20);
            m_yaMainMemory[0x0010] = ConvertASCIItoEBCDIC ((byte)'$');
            m_yaMainMemory[0x0013] = ConvertASCIItoEBCDIC ((byte)',');
            m_yaMainMemory[0x0017] = ConvertASCIItoEBCDIC ((byte)',');
            m_yaMainMemory[0x001B] = ConvertASCIItoEBCDIC ((byte)'.');
            StoreStringEBCDIC ("12345678765432", 0x0020);
            ExecuteInstruction (yaED6);
            string strReturn = LoadStringEBCDIC (0x0010, 0x0E);
            Assert (strReturn == "$56,787,654.32");
            Assert (m_aCR[m_iIL].IsHigh ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());

            // Test with negative value
            // 0123456789abcd
            // $  ,   ,   .  
            // 0123456789abcd
            // 123456654321
            FillArea (0x0010, 0x02F, 0x20);
            m_yaMainMemory[0x0010] = ConvertASCIItoEBCDIC ((byte)'$');
            m_yaMainMemory[0x0013] = ConvertASCIItoEBCDIC ((byte)',');
            m_yaMainMemory[0x0017] = ConvertASCIItoEBCDIC ((byte)',');
            m_yaMainMemory[0x001B] = ConvertASCIItoEBCDIC ((byte)'.');
            StoreStringEBCDIC ("1234567876543K", 0x0020);
            ExecuteInstruction (yaED6);
            strReturn = LoadStringEBCDIC (0x0010, 0x0E);
            Assert (strReturn == "$56,787,654.32");
            Assert (m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());

            // Test with positive value
            // 0123456789abcd
            // $  ,   ,   .  
            // 0123456789abcd
            // 123456654321
            FillArea (0x0010, 0x02F, 0x20);
            m_yaMainMemory[0x0010] = ConvertASCIItoEBCDIC ((byte)'$');
            m_yaMainMemory[0x0013] = ConvertASCIItoEBCDIC ((byte)',');
            m_yaMainMemory[0x0017] = ConvertASCIItoEBCDIC ((byte)',');
            m_yaMainMemory[0x001B] = ConvertASCIItoEBCDIC ((byte)'.');
            StoreStringEBCDIC ("00000000000000", 0x0020);
            ExecuteInstruction (yaED6);
            strReturn = LoadStringEBCDIC (0x0010, 0x0E);
            Assert (strReturn == "$00,000,000.00");
            Assert (m_aCR[m_iIL].IsEqual ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());
        }

        public void TestSZ ()
        {
            SystemReset ();

            byte[] yaSZ1 = { 0x07, 0x03, 0x00, 0x10, 0x00, 0x20 };
            byte[] yaSZ2 = { 0x07, 0x23, 0x00, 0x10, 0x00, 0x20 };

            // Subtract positive & positive  equal length  positive result
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (123, 0x0010, 3); // Operand one
            StoreZonedLong (122, 0x0020, 3); // Operand two
            ExecuteInstruction (yaSZ1);
            long lResult = LoadZonedLong (0x0010, 3);
            Assert (lResult == 1);
            Assert (m_aCR[m_iIL].IsHigh ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());

            // Subtract positive & positive   positive result
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (999, 0x0010, 3); // Operand one
            StoreZonedLong (1, 0x0020, 3); // Operand two
            ExecuteInstruction (yaSZ1);
            lResult = LoadZonedLong (0x0010, 4);
            Assert (lResult == 998);
            Assert (m_aCR[m_iIL].IsHigh ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());

            // Subtract positive & positive  equal length  positive result
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (122, 0x0010, 3); // Operand one
            StoreZonedLong (-129, 0x0020, 3); // Operand two
            ExecuteInstruction (yaSZ1);
            lResult = LoadZonedLong (0x0010, 4);
            Assert (lResult == 251);
            Assert (m_aCR[m_iIL].IsHigh ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());

            // Subtract positive & negative  equal absolute values  negative result
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (-122, 0x0010, 3); // Operand one
            StoreZonedLong (122, 0x0020, 3); // Operand two
            ExecuteInstruction (yaSZ1);
            lResult = LoadZonedLong (0x0010, 4);
            Assert (lResult == -244);
            Assert (m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());

            // Subtract positive & negative  positive result
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (100, 0x0010, 3); // Operand one
            StoreZonedLong (-101, 0x0020, 3); // Operand two
            ExecuteInstruction (yaSZ1);
            lResult = LoadZonedLong (0x0010, 4);
            Assert (lResult == 201);
            Assert (m_aCR[m_iIL].IsHigh ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());

            // Subtract negative & negative  negative result
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (-124, 0x0010, 3); // Operand one
            StoreZonedLong (-123, 0x0020, 3); // Operand two
            ExecuteInstruction (yaSZ1);
            lResult = LoadZonedLong (0x0010, 4);
            Assert (lResult == -1);
            Assert (m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());

            // Subtract positive & positive  positive result
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (999777, 0x0010, 6); // Operand one
            StoreZonedLong (333, 0x0020, 3); // Operand two
            ExecuteInstruction (yaSZ2);
            lResult = LoadZonedLong (0x0010, 6);
            Assert (lResult == 999444);
            Assert (m_aCR[m_iIL].IsHigh ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());

            // Subtract positive & negative  positive result
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (188, 0x0010, 3); // Operand one
            StoreZonedLong (-99, 0x0020, 3); // Operand two
            ExecuteInstruction (yaSZ1);
            lResult = LoadZonedLong (0x0010, 6);
            Assert (lResult == 287);
            Assert (m_aCR[m_iIL].IsHigh ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());

            // Subtract negative & positive  negative result
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (-224, 0x0010, 6); // Operand one
            StoreZonedLong (128, 0x0020, 4); // Operand two
            ExecuteInstruction (yaSZ2);
            lResult = LoadZonedLong (0x0010, 6);
            Assert (lResult == -352);
            Assert (m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());

            // Compare negative & negative  2 > 1
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (1000, 0x0010, 4); // Operand one
            StoreZonedLong (1001, 0x0020, 4); // Operand two
            ExecuteInstruction (yaSZ2);
            lResult = LoadZonedLong (0x0010, 6);
            Assert (lResult == -1);
            Assert (m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());
            m_aCR[m_iIL].TestResetDecimalOverflow ();
        }

        public void TestAZ ()
        {
            SystemReset ();

            byte[] yaAZ1 = { 0x06, 0x03, 0x00, 0x10, 0x00, 0x20 };
            byte[] yaAZ2 = { 0x06, 0x23, 0x00, 0x10, 0x00, 0x20 };

            // Add positive & positive  1 > 2  equal length
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (123, 0x0010, 3); // Operand one
            StoreZonedLong (122, 0x0020, 3); // Operand two
            ExecuteInstruction (yaAZ1);
            long lResult = LoadZonedLong (0x0010, 3);
            Assert (lResult == 245);
            Assert (m_aCR[m_iIL].IsHigh ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());

            // Add positive & positive  1 < 2  equal length
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (999, 0x0010, 3); // Operand one
            StoreZonedLong (1, 0x0020, 3); // Operand two
            ExecuteInstruction (yaAZ1);
            lResult = LoadZonedLong (0x0010, 4);
            Assert (lResult == 1000);
            Assert (m_aCR[m_iIL].IsHigh ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());

            // Add positive & negative  equal length
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (122, 0x0010, 3); // Operand one
            StoreZonedLong (-129, 0x0020, 3); // Operand two
            ExecuteInstruction (yaAZ1);
            lResult = LoadZonedLong (0x0010, 4);
            Assert (lResult == -7); // -7
            Assert (m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());
            m_aCR[m_iIL].TestResetDecimalOverflow ();

            // Add positive & negative  equal absolute values
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (-122, 0x0010, 3); // Operand one
            StoreZonedLong (122, 0x0020, 3); // Operand two
            ExecuteInstruction (yaAZ1);
            lResult = LoadZonedLong (0x0010, 4);
            Assert (lResult == 0);
            Assert (m_aCR[m_iIL].IsEqual ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());

            // Compare negative & positive for negative result
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (100, 0x0010, 3); // Operand one
            StoreZonedLong (-101, 0x0020, 3); // Operand two
            ExecuteInstruction (yaAZ1);
            lResult = LoadZonedLong (0x0010, 4);
            Assert (lResult == -1); // -1
            Assert (m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());
            m_aCR[m_iIL].TestResetDecimalOverflow ();

            // Add negative & negative
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (-124, 0x0010, 3); // Operand one
            StoreZonedLong (-123, 0x0020, 3); // Operand two
            ExecuteInstruction (yaAZ1);
            lResult = LoadZonedLong (0x0010, 4);
            Assert (lResult == -247);
            Assert (m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());

            // Add positive & positive  Operand 1 longer w/ significant digit beyond operand 2
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (999777, 0x0010, 6); // Operand one
            StoreZonedLong (333, 0x0020, 3); // Operand two
            ExecuteInstruction (yaAZ2);
            lResult = LoadZonedLong (0x0010, 6);
            Assert (lResult == 110);
            Assert (m_aCR[m_iIL].IsHigh ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (m_aCR[m_iIL].IsDecimalOverflow ());
            m_aCR[m_iIL].TestResetDecimalOverflow ();

            // Add negative & positive for positive result
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (188, 0x0010, 3); // Operand one
            StoreZonedLong (-99, 0x0020, 3); // Operand two
            ExecuteInstruction (yaAZ1);
            lResult = LoadZonedLong (0x0010, 6);
            Assert (lResult == 89);
            Assert (m_aCR[m_iIL].IsHigh ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ()); // Not reset by AZ!

            // Add negative & positive for negative result
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (-224, 0x0010, 6); // Operand one
            StoreZonedLong (128, 0x0020, 4); // Operand two
            ExecuteInstruction (yaAZ2);
            lResult = LoadZonedLong (0x0010, 6);
            Assert (lResult == -96);
            Assert (m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ()); // Not reset by AZ!
        }

        public void TestZAZ ()
        {
            SystemReset ();

            // Positive value
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (12345654321, 0x000D, 13);  // Operand 2
            byte[] yaZAZ1 = { 0x04, 0x7C, 0x00, 0x31, 0x00, 0x0D };
            ExecuteInstruction (yaZAZ1);
            long lResult = LoadZonedLong (0x0031, 16);
            Assert (lResult == 12345654321);
            Assert (m_aCR[m_iIL].IsHigh ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());

            // Zero value
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (0, 0x000D, 13);  // Operand 2
            byte[] yaZAZ2 = { 0x04, 0x7C, 0x00, 0x31, 0x00, 0x0D };
            ExecuteInstruction (yaZAZ2);
            lResult = LoadZonedLong (0x0031, 16);
            Assert (lResult == 0);
            Assert (m_aCR[m_iIL].IsEqual());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());

            // Negaive value
            FillArea (0x0000, 0x0050, 0x00); // Clear out the test area
            StoreZonedLong (-12345654321, 0x000D, 13);  // Operand 2
            byte[] yaZAZ3 = { 0x04, 0x7C, 0x00, 0x31, 0x00, 0x0D };
            ExecuteInstruction (yaZAZ3);
            lResult = LoadZonedLong (0x0031, 16);
            Assert (lResult == -12345654321);
            Assert (m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());
        }

        public void TestSLC ()
        {
            SystemReset ();

            // Operand 1 > operand 2
            StoreInt (0x0101, 0x0003, 3);  // Operand 1
            StoreInt (0x0002, 0x0007, 3);  // Operand 2
            byte[] yaSLC1 = { 0x0F, 0x01, 0x00, 0x03, 0x00, 0x07 };
            ExecuteInstruction (yaSLC1);
            int iResult = LoadInt (0x0003, 4);
            Assert (iResult == 0x000000FF);
            Assert (m_aCR[m_iIL].IsHigh ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());

            // Operand 1 == operand 2
            StoreInt (0x0101, 0x0003, 3);  // Operand 1
            StoreInt (0x0101, 0x0007, 3);  // Operand 2
            byte[] yaSLC2 = { 0x0F, 0x01, 0x00, 0x03, 0x00, 0x07 };
            ExecuteInstruction (yaSLC2);
            iResult = LoadInt (0x0003, 4);
            Assert (iResult == 0x00000000);
            Assert (m_aCR[m_iIL].IsEqual ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());

            // Operand 1 < operand 2
            StoreInt (0x0101, 0x0003, 3);  // Operand 1
            StoreInt (0x0102, 0x0007, 3);  // Operand 2
            byte[] yaSLC3 = { 0x0F, 0x01, 0x00, 0x03, 0x00, 0x07 };
            ExecuteInstruction (yaSLC3);
            iResult = LoadInt (0x0003, 4);
            Assert (iResult == 0x0000FFFF);
            Assert (m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());
        }

        public void TestA () // Produces "Attempt to use PL2" error
        {
            SystemReset ();
            m_yaMainMemory[0x0000] = 0x10;
            m_yaMainMemory[0x0001] = 0x10;
            m_yaMainMemory[0x0002] = 0x10;
            m_yaMainMemory[0x0003] = 0x10;
            m_yaMainMemory[0x0004] = 0x00;
            m_yaMainMemory[0x0005] = 0x11;
            m_yaMainMemory[0x0006] = 0xDE;
            m_yaMainMemory[0x0007] = 0xDE;
            m_yaMainMemory[0x0008] = 0x00;
            m_yaMainMemory[0x0009] = 0x00;
            m_iIAR = 0x0102;  // Instruction Address Register
            m_iARR = 0x0304;  // Address Recall Register
            m_iXR1 = 0x0506;  // Index Register 1
            m_iXR2 = 0x0708;  // Index Register 2

            // XR1
            byte[] yaST1 = { 0x36, 0x01, 0x00, 0x01 };
            ExecuteInstruction (yaST1);
            Assert (m_iXR1 == 0x1516);
            Assert (m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());

            // XR2
            byte[] yaST2 = { 0x36, 0x02, 0x00, 0x01 };
            ExecuteInstruction (yaST2);
            Assert (m_iXR2 == 0x1718);
            Assert (m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());

            // IAR
            byte[] yaST3 = { 0x36, 0x10, 0x00, 0x01 };
            ExecuteInstruction (yaST3);
            Assert (m_iIAR == 0x1112);
            Assert (m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());

            // ARR
            byte[] yaST4 = { 0x36, 0x08, 0x00, 0x01 };
            ExecuteInstruction (yaST4);
            Assert (m_iARR == 0x1314);
            Assert (m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());

            // CR (PSR)
            m_aCR[m_iIL].SetEqual ();                 // 0x01
            m_aCR[m_iIL].ResetBinaryOverflow ();      // 0x08
            m_aCR[m_iIL].SetTestFalse ();             // 0x10
            m_aCR[m_iIL].TestResetDecimalOverflow (); // 0x20
            byte[] yaST5 = { 0x36, 0x04, 0x00, 0x05 };
            ExecuteInstruction (yaST5);
            Assert (!m_aCR[m_iIL].IsEqual ());
            Assert (m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsHigh ());
            Assert (m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());
            m_aCR[m_iIL].ResetBinaryOverflow ();

            // IAR PL1
            byte[] yaST6 = { 0x36, 0x20, 0x00, 0x01 };
            ExecuteInstruction (yaST6);
            Assert (m_iIAR == 0x2122);
            Assert (m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());

            // IAR PL1 (Overflow)
            byte[] yaST7 = { 0x36, 0x20, 0x00, 0x07 };
            ExecuteInstruction (yaST7);
            Assert (m_iIAR == 0x0000);
            Assert (m_aCR[m_iIL].IsHigh ());
            Assert (m_aCR[m_iIL].IsBinaryOverflow ());
            m_aCR[m_iIL].ResetBinaryOverflow ();

            // IAR PL1 (zero)
            byte[] yaST8 = { 0x36, 0x20, 0x00, 0x09 };
            ExecuteInstruction (yaST8);
            Assert (m_iIAR == 0x0000);
            Assert (m_aCR[m_iIL].IsEqual ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());

            // IAR PL2
            byte[] yaST9 = { 0x36, 0x40, 0x00, 0x01 };
            ExecuteInstruction (yaST9);
            Assert (ProgramState == EProgramState.STATE_PChk_PL2_Unsupported);
            TestResetProgramState ();
        }

        public void TestST () // Produces "Address Wraparound" and "Attempt to use PL2" errors
        {
            SystemReset ();
            m_yaMainMemory[0x0000] = 0x00;
            m_yaMainMemory[0x0001] = 0x00;
            m_iIAR = 0x0102;  // Instruction Address Register
            m_iARR = 0x0304;  // Address Recall Register
            m_iXR1 = 0x0506;  // Index Register 1
            m_iXR2 = 0x0708;  // Index Register 2

            // XR1
            byte[] yaST1 = { 0x34, 0x01, 0x00, 0x00 };
            ExecuteInstruction (yaST1);
            Assert (ProgramState == EProgramState.STATE_PChk_AddressWrap);
            TestResetProgramState ();

            byte[] yaST2 = { 0x34, 0x01, 0x00, 0x01 };
            ExecuteInstruction (yaST2);
            Assert (m_yaMainMemory[0x0000] == 0x05);
            Assert (m_yaMainMemory[0x0001] == 0x06);

            // XR2
            byte[] yaST3 = { 0x34, 0x02, 0x00, 0x01 };
            ExecuteInstruction (yaST3);
            Assert (m_yaMainMemory[0x0000] == 0x07);
            Assert (m_yaMainMemory[0x0001] == 0x08);

            // CR (PSR)
            m_aCR[m_iIL].SetHigh ();
            m_aCR[m_iIL].SetBinaryOverflow ();
            m_aCR[m_iIL].SetTestFalse ();
            m_aCR[m_iIL].SetDecimalOverflow ();
            byte[] yaST4 = { 0x34, 0x04, 0x00, 0x01 };
            ExecuteInstruction (yaST4);
            Assert (m_yaMainMemory[0x0000] == 0x00);
            Assert (m_yaMainMemory[0x0001] == 0x38);

            // IAR
            byte[] yaST5 = { 0x34, 0x10, 0x00, 0x01 };
            ExecuteInstruction (yaST5);
            Assert (m_yaMainMemory[0x0000] == 0x01);
            Assert (m_yaMainMemory[0x0001] == 0x02);

            // ARR
            byte[] yaST6 = { 0x34, 0x08, 0x00, 0x01 };
            ExecuteInstruction (yaST6);
            Assert (m_yaMainMemory[0x0000] == 0x03);
            Assert (m_yaMainMemory[0x0001] == 0x04);

            // IAR PL1
            byte[] yaST7 = { 0x34, 0x20, 0x00, 0x01 };
            ExecuteInstruction (yaST7);
            Assert (m_yaMainMemory[0x0000] == 0x01);
            Assert (m_yaMainMemory[0x0001] == 0x02);

            // IAR PL2
            byte[] yaST8 = { 0x34, 0x40, 0x00, 0x01 };
            ExecuteInstruction (yaST8);
            Assert (ProgramState == EProgramState.STATE_PChk_PL2_Unsupported);
            TestResetProgramState ();
        }

        public void TestL () // Produces "Attempt to use PL2" error
        {
            SystemReset ();
            m_iXR1 = 0;
            m_iXR2 = 0;
            m_yaMainMemory[0x0000] = 0x10;
            m_yaMainMemory[0x0001] = 0x20;
            m_yaMainMemory[0x0002] = 0x30;
            m_yaMainMemory[0x0003] = 0x40;
            m_yaMainMemory[0x0004] = 0x50;
            m_yaMainMemory[0x0005] = 0x60;
            m_yaMainMemory[0x0006] = 0x70;
            m_yaMainMemory[0x0007] = 0x00;
            m_yaMainMemory[0x0008] = 0x3A;

            // XR1
            byte[] yaL1 = { 0x35, 0x01, 0x00, 0x01 };
            ExecuteInstruction (yaL1);
            Assert (m_iXR1 == 0x1020);
            Assert (m_iXR2 == 0x0000);
            Assert (m_iARR == 0x0000);
            Assert (m_iIAR == 0x0000);
            Assert (m_aCR[m_iIL].IsEqual ());
            Assert (!m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsHigh ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());

            // XR2
            byte[] yaL2 = { 0x35, 0x02, 0x00, 0x02 };
            ExecuteInstruction (yaL2);
            Assert (m_iXR1 == 0x1020);
            Assert (m_iXR2 == 0x2030);
            Assert (m_iARR == 0x0000);
            Assert (m_iIAR == 0x0000);
            Assert (m_aCR[m_iIL].IsEqual ());
            Assert (!m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsHigh ());
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());
            Assert (!m_aCR[m_iIL].IsTestFalse ());
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());

            // CR (PSR)
            byte[] yaL3 = { 0x35, 0x04, 0x00, 0x08 };
            ExecuteInstruction (yaL3);
            Assert (m_iXR1 == 0x1020);
            Assert (m_iXR2 == 0x2030);
            Assert (m_iARR == 0x0000);
            Assert (m_iIAR == 0x0000);
            Assert (!m_aCR[m_iIL].IsEqual ());
            Assert (m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsHigh ());
            Assert (m_aCR[m_iIL].IsDecimalOverflow ());
            Assert (m_aCR[m_iIL].IsTestFalse ());
            Assert (m_aCR[m_iIL].IsBinaryOverflow ());

            // ARR
            byte[] yaL4 = { 0x35, 0x08, 0x00, 0x03 };
            ExecuteInstruction (yaL4);
            Assert (m_iXR1 == 0x1020);
            Assert (m_iXR2 == 0x2030);
            Assert (m_iARR == 0x3040);
            Assert (m_iIAR == 0x0000);

            // IAR
            byte[] yaL5 = { 0x35, 0x10, 0x00, 0x04 };
            ExecuteInstruction (yaL5);
            Assert (m_iXR1 == 0x1020);
            Assert (m_iXR2 == 0x2030);
            Assert (m_iARR == 0x3040);
            Assert (m_iIAR == 0x4050);

            // IAR PL1
            byte[] yaL6 = { 0x35, 0x20, 0x00, 0x05 };
            ExecuteInstruction (yaL6);
            Assert (m_iXR1 == 0x1020);
            Assert (m_iXR2 == 0x2030);
            Assert (m_iARR == 0x3040);
            Assert (m_iIAR == 0x5060);

            // IAR PL2
            byte[] yaL7 = { 0x35, 0x40, 0x00, 0x05 };
            ExecuteInstruction (yaL7);
            Assert (ProgramState == EProgramState.STATE_PChk_PL2_Unsupported);
            TestResetProgramState ();
        }

        public void TestALC ()
        {
            // Positive result, no carry
            m_yaMainMemory[0x0002] = 0x0F;
            m_yaMainMemory[0x0003] = 0xFF;
            m_yaMainMemory[0x0005] = 0x00;
            m_yaMainMemory[0x0006] = 0x01;
            byte[] yaALC1 = { 0x0E, 0x01, 0x00, 0x03, 0x00, 0x06 };
            ExecuteInstruction (yaALC1);
            Assert (m_yaMainMemory[0x0002] == 0x10);
            Assert (m_yaMainMemory[0x0003] == 0x00);
            Assert (m_yaMainMemory[0x0005] == 0x00);
            Assert (m_yaMainMemory[0x0006] == 0x01);
            Assert (!m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (m_aCR[m_iIL].IsLow ());

            // Binary Overflow, zero result
            m_yaMainMemory[0x0002] = 0xFF;
            m_yaMainMemory[0x0003] = 0xFF;
            m_yaMainMemory[0x0005] = 0x00;
            m_yaMainMemory[0x0006] = 0x01;
            byte[] yaALC2 = { 0x0E, 0x01, 0x00, 0x03, 0x00, 0x06 };
            ExecuteInstruction (yaALC2);
            Assert (m_yaMainMemory[0x0002] == 0x00);
            Assert (m_yaMainMemory[0x0003] == 0x00);
            Assert (m_yaMainMemory[0x0005] == 0x00);
            Assert (m_yaMainMemory[0x0006] == 0x01);
            Assert (m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (m_aCR[m_iIL].IsEqual ());

            // Binary Overflow, non-zero result
            m_yaMainMemory[0x0002] = 0xFF;
            m_yaMainMemory[0x0003] = 0xFF;
            m_yaMainMemory[0x0005] = 0x01;
            m_yaMainMemory[0x0006] = 0x00;
            byte[] yaALC3 = { 0x0E, 0x01, 0x00, 0x03, 0x00, 0x06 };
            ExecuteInstruction (yaALC3);
            Assert (m_yaMainMemory[0x0002] == 0x00);
            Assert (m_yaMainMemory[0x0003] == 0xFF);
            Assert (m_yaMainMemory[0x0005] == 0x01);
            Assert (m_yaMainMemory[0x0006] == 0x00);
            Assert (m_aCR[m_iIL].IsBinaryOverflow ());
            Assert (m_aCR[m_iIL].IsHigh ());
        }

        public void TestJC ()
        {
            m_aCR[m_iIL].SetEqual ();
            m_aCR[m_iIL].SetDecimalOverflow ();
            m_aCR[m_iIL].SetTestFalse ();
            m_aCR[m_iIL].SetBinaryOverflow ();

            m_iARR = 0x0000;
            m_iIAR = 0x0100;

            // Test NoOp
            byte[] yaBC1 = { 0xF2, 0x80, 0x20 };
            ExecuteInstruction (yaBC1);
            Assert (m_iARR == 0x0000);
            Assert (m_iIAR == 0x0100);

            byte[] yaBC2 = { 0xF2, 0x07, 0x20 };
            ExecuteInstruction (yaBC2);
            Assert (m_iARR == 0x0000);
            Assert (m_iIAR == 0x0100);

            // Test Unconditional
            byte[] yaBC3 = { 0xF2, 0x00, 0x20 };
            ExecuteInstruction (yaBC3);
            Assert (m_iARR == 0x0100);
            Assert (m_iIAR == 0x0120);

            byte[] yaBC4 = { 0xF2, 0x87, 0x20 };
            ExecuteInstruction (yaBC4);
            Assert (m_iARR == 0x0120);
            Assert (m_iIAR == 0x0140);

            m_iARR = 0x0000;
            m_iIAR = 0x0100;

            // Test On Equal
            byte[] yaBC5 = { 0xF2, 0x81, 0x20 };
            ExecuteInstruction (yaBC5);
            Assert (m_iARR == 0x0100);
            Assert (m_iIAR == 0x0120);

            m_iARR = 0x0000;
            m_iIAR = 0x0100;

            // Test Not Equal
            byte[] yaBC6 = { 0xF2, 0x01, 0x20 };
            ExecuteInstruction (yaBC6);
            Assert (m_iARR == 0x0000);
            Assert (m_iIAR == 0x0100);

            // Test Not High
            byte[] yaBC7 = { 0xF2, 0x04, 0x20 };
            ExecuteInstruction (yaBC7);
            Assert (m_iARR == 0x0100);
            Assert (m_iIAR == 0x0120);

            m_iARR = 0x0000;
            m_iIAR = 0x0100;

            // Test On BinaryOverflow (no reset)
            byte[] yaBC8 = { 0xF2, 0xA0, 0x20 };
            ExecuteInstruction (yaBC8);
            Assert (m_iARR == 0x0100);
            Assert (m_iIAR == 0x0120);

            m_iARR = 0x0000;
            m_iIAR = 0x0100;

            // Test On DecimalOverflow (reset)
            byte[] yaBC9 = { 0xF2, 0x88, 0x20 };
            ExecuteInstruction (yaBC9);
            Assert (m_iARR == 0x0100);
            Assert (m_iIAR == 0x0120);
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());

            m_iARR = 0x0000;
            m_iIAR = 0x0100;

            // Test On TestFalse (reset)
            byte[] yaBC10 = { 0xF2, 0x90, 0x20 };
            ExecuteInstruction (yaBC10);
            Assert (m_iARR == 0x0100);
            Assert (m_iIAR == 0x0120);
            Assert (!m_aCR[m_iIL].IsTestFalse ());
        }

        public void TestBC ()
        {
            m_aCR[m_iIL].SetEqual ();
            m_aCR[m_iIL].SetDecimalOverflow ();
            m_aCR[m_iIL].SetTestFalse ();
            m_aCR[m_iIL].SetBinaryOverflow ();

            m_iARR = 0x0000;
            m_iIAR = 0x0100;

            // Test NoOp
            byte[] yaBC1 = { 0xC0, 0x80, 0x02, 0x00 };
            ExecuteInstruction (yaBC1);
            Assert (m_iARR == 0x0000);
            Assert (m_iIAR == 0x0100);

            byte[] yaBC2 = { 0xC0, 0x07, 0x02, 0x00 };
            ExecuteInstruction (yaBC2);
            Assert (m_iARR == 0x0000);
            Assert (m_iIAR == 0x0100);

            // Test Unconditional
            byte[] yaBC3 = { 0xC0, 0x00, 0x02, 0x00 };
            ExecuteInstruction (yaBC3);
            Assert (m_iARR == 0x0100);
            Assert (m_iIAR == 0x0200);

            byte[] yaBC4 = { 0xC0, 0x87, 0x03, 0x00 };
            ExecuteInstruction (yaBC4);
            Assert (m_iARR == 0x0200);
            Assert (m_iIAR == 0x0300);

            m_iARR = 0x0000;
            m_iIAR = 0x0100;

            // Test On Equal
            byte[] yaBC5 = { 0xC0, 0x81, 0x02, 0x00 };
            ExecuteInstruction (yaBC5);
            Assert (m_iARR == 0x0100);
            Assert (m_iIAR == 0x0200);

            m_iARR = 0x0000;
            m_iIAR = 0x0100;

            // Test Not Equal
            byte[] yaBC6 = { 0xC0, 0x01, 0x02, 0x00 };
            ExecuteInstruction (yaBC6);
            Assert (m_iARR == 0x0000);
            Assert (m_iIAR == 0x0100);

            // Test Not High
            byte[] yaBC7 = { 0xC0, 0x04, 0x02, 0x00 };
            ExecuteInstruction (yaBC7);
            Assert (m_iARR == 0x0100);
            Assert (m_iIAR == 0x0200);

            m_iARR = 0x0000;
            m_iIAR = 0x0100;

            // Test On BinaryOverflow (no reset)
            byte[] yaBC8 = { 0xC0, 0xA0, 0x02, 0x00 };
            ExecuteInstruction (yaBC8);
            Assert (m_iARR == 0x0100);
            Assert (m_iIAR == 0x0200);

            m_iARR = 0x0000;
            m_iIAR = 0x0100;

            // Test On DecimalOverflow (reset)
            byte[] yaBC9 = { 0xC0, 0x88, 0x02, 0x00 };
            ExecuteInstruction (yaBC9);
            Assert (m_iARR == 0x0100);
            Assert (m_iIAR == 0x0200);
            Assert (!m_aCR[m_iIL].IsDecimalOverflow ());

            m_iARR = 0x0000;
            m_iIAR = 0x0100;

            // Test On TestFalse (reset)
            byte[] yaBC10 = { 0xC0, 0x90, 0x02, 0x00 };
            ExecuteInstruction (yaBC10);
            Assert (m_iARR == 0x0100);
            Assert (m_iIAR == 0x0200);
            Assert (!m_aCR[m_iIL].IsTestFalse ());
        }

        public void TestCLC ()
        {
            // Reference
            m_yaMainMemory[0x1230] = 0x12;
            m_yaMainMemory[0x1231] = 0x34;

            // Test equal
            m_yaMainMemory[0x1232] = 0x12;
            m_yaMainMemory[0x1233] = 0x34;

            // Test high
            m_yaMainMemory[0x1234] = 0x13;
            m_yaMainMemory[0x1235] = 0x34;

            // Test high
            m_yaMainMemory[0x1236] = 0x12;
            m_yaMainMemory[0x1237] = 0x35;

            // Test low
            m_yaMainMemory[0x1238] = 0x11;
            m_yaMainMemory[0x1239] = 0x34;

            byte[] yaCLC1 = { 0x0D, 0x01, 0x12, 0x31, 0x12, 0x31 };
            ExecuteInstruction (yaCLC1);
            Assert (!m_aCR[m_iIL].IsLow ());
            Assert (m_aCR[m_iIL].IsEqual ());
            Assert (!m_aCR[m_iIL].IsHigh ());

            byte[] yaCLC2 = { 0x0D, 0x01, 0x12, 0x33, 0x12, 0x31 };
            ExecuteInstruction (yaCLC2);
            Assert (!m_aCR[m_iIL].IsLow ());
            Assert (m_aCR[m_iIL].IsEqual ());
            Assert (!m_aCR[m_iIL].IsHigh ());

            byte[] yaCLC3 = { 0x0D, 0x01, 0x12, 0x35, 0x12, 0x31 };
            ExecuteInstruction (yaCLC3);
            Assert (!m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsEqual ());
            Assert (m_aCR[m_iIL].IsHigh ());

            byte[] yaCLC4 = { 0x0D, 0x01, 0x12, 0x37, 0x12, 0x31 };
            ExecuteInstruction (yaCLC4);
            Assert (!m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsEqual ());
            Assert (m_aCR[m_iIL].IsHigh ());

            byte[] yaCLC5 = { 0x0D, 0x01, 0x12, 0x39, 0x12, 0x31 };
            ExecuteInstruction (yaCLC5);
            Assert (m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsEqual ());
            Assert (!m_aCR[m_iIL].IsHigh ());
        }

        public void TestCLI ()
        {
            m_yaMainMemory[0x1230] = 0x12;
            byte[] yaCLI1 = { 0x3D, 0x13, 0x12, 0x30 };
            ExecuteInstruction (yaCLI1);
            Assert (m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsEqual ());
            Assert (!m_aCR[m_iIL].IsHigh ());

            byte[] yaCLI2 = { 0x3D, 0x12, 0x12, 0x30 };
            ExecuteInstruction (yaCLI2);
            Assert (!m_aCR[m_iIL].IsLow ());
            Assert (m_aCR[m_iIL].IsEqual ());
            Assert (!m_aCR[m_iIL].IsHigh ());

            byte[] yaCLI3 = { 0x3D, 0x11, 0x12, 0x30 };
            ExecuteInstruction (yaCLI3);
            Assert (!m_aCR[m_iIL].IsLow ());
            Assert (!m_aCR[m_iIL].IsEqual ());
            Assert (m_aCR[m_iIL].IsHigh ());
        }

        public void TestTBF ()
        {
            m_aCR[m_iIL].TestResetTestFalse ();

            m_yaMainMemory[0x1230] = 0x12;
            byte[] yaTBN1 = { 0x39, 0x00, 0x12, 0x30 };
            ExecuteInstruction (yaTBN1);
            Assert (!m_aCR[m_iIL].IsTestFalse ());

            byte[] yaTBN2 = { 0x39, 0x20, 0x12, 0x30 };
            ExecuteInstruction (yaTBN2);
            Assert (!m_aCR[m_iIL].IsTestFalse ());

            byte[] yaTBN3 = { 0x39, 0xE0, 0x12, 0x30 };
            ExecuteInstruction (yaTBN3);
            Assert (!m_aCR[m_iIL].IsTestFalse ());

            byte[] yaTBN4 = { 0x39, 0xED, 0x12, 0x30 };
            ExecuteInstruction (yaTBN4);
            Assert (!m_aCR[m_iIL].IsTestFalse ());

            byte[] yaTBN5 = { 0x39, 0x10, 0x12, 0x30 };
            ExecuteInstruction (yaTBN5);
            Assert (m_aCR[m_iIL].IsTestFalse ());

            m_aCR[m_iIL].TestResetTestFalse ();
            byte[] yaTBN6 = { 0x39, 0x02, 0x12, 0x30 };
            ExecuteInstruction (yaTBN6);
            Assert (m_aCR[m_iIL].IsTestFalse ());
        }

        public void TestTBN ()
        {
            m_aCR[m_iIL].TestResetTestFalse ();

            m_yaMainMemory[0x1230] = 0x12;
            byte[] yaTBN1 = { 0x38, 0x00, 0x12, 0x30 };
            ExecuteInstruction (yaTBN1);
            Assert (!m_aCR[m_iIL].IsTestFalse ());

            byte[] yaTBN2 = { 0x38, 0x12, 0x12, 0x30 };
            ExecuteInstruction (yaTBN2);
            Assert (!m_aCR[m_iIL].IsTestFalse ());

            byte[] yaTBN3 = { 0x38, 0x02, 0x12, 0x30 };
            ExecuteInstruction (yaTBN3);
            Assert (!m_aCR[m_iIL].IsTestFalse ());

            byte[] yaTBN4 = { 0x38, 0x10, 0x12, 0x30 };
            ExecuteInstruction (yaTBN4);
            Assert (!m_aCR[m_iIL].IsTestFalse ());

            byte[] yaTBN5 = { 0x38, 0x13, 0x12, 0x30 };
            ExecuteInstruction (yaTBN5);
            Assert (m_aCR[m_iIL].IsTestFalse ());
        }

        public void TestMVC ()
        {
            m_yaMainMemory[0x1230] = 0x12;
            m_yaMainMemory[0x1231] = 0x23;
            m_yaMainMemory[0x1232] = 0x34;
            m_yaMainMemory[0x1233] = 0x45;
            m_yaMainMemory[0x1234] = 0x00;
            m_yaMainMemory[0x1235] = 0x00;
            m_yaMainMemory[0x1236] = 0x00;
            m_yaMainMemory[0x1237] = 0x00;
            m_yaMainMemory[0x1238] = 0x00;

            byte[] yaMVC1 = { 0x0C, 0x00, 0x12, 0x34, 0x12, 0x31 };
            ExecuteInstruction (yaMVC1);
            Assert (m_yaMainMemory[0x1234] == 0x23);

            byte[] yaMVC2 = { 0x0C, 0x03, 0x12, 0x38, 0x12, 0x33 };
            ExecuteInstruction (yaMVC2);
            Assert (m_yaMainMemory[0x1230] == 0x12);
            Assert (m_yaMainMemory[0x1231] == 0x23);
            Assert (m_yaMainMemory[0x1232] == 0x34);
            Assert (m_yaMainMemory[0x1233] == 0x45);
            Assert (m_yaMainMemory[0x1235] == 0x12);
            Assert (m_yaMainMemory[0x1236] == 0x23);
            Assert (m_yaMainMemory[0x1237] == 0x34);
            Assert (m_yaMainMemory[0x1238] == 0x45);

            // Test propogration of overlapping operands
            m_yaMainMemory[0x0000] = 0x00;
            m_yaMainMemory[0x0001] = 0x00;
            m_yaMainMemory[0x0002] = 0x00;
            m_yaMainMemory[0x0003] = 0x00;
            m_yaMainMemory[0x0004] = 0x00;
            m_yaMainMemory[0x0005] = 0x00;
            m_yaMainMemory[0x0006] = 0x00;
            m_yaMainMemory[0x0007] = 0x34;
            m_yaMainMemory[0x0008] = 0x45;
            byte[] yaMVC3 = { 0x0C, 0x05, 0x00, 0x06, 0x00, 0x08 };
            ExecuteInstruction (yaMVC3);
            Assert (m_yaMainMemory[0x0001] == 0x34);
            Assert (m_yaMainMemory[0x0002] == 0x45);
            Assert (m_yaMainMemory[0x0003] == 0x34);
            Assert (m_yaMainMemory[0x0004] == 0x45);
            Assert (m_yaMainMemory[0x0005] == 0x34);
            Assert (m_yaMainMemory[0x0006] == 0x45);
            Assert (m_yaMainMemory[0x0007] == 0x34);
            Assert (m_yaMainMemory[0x0008] == 0x45);
        }

        public void TestMVX ()
        {
            m_yaMainMemory[0x1230] = 0x12;
            m_yaMainMemory[0x1231] = 0x23;
            m_yaMainMemory[0x1232] = 0x34;
            m_yaMainMemory[0x1233] = 0x45;
            m_yaMainMemory[0x1234] = 0x56;
            m_yaMainMemory[0x1235] = 0x67;
            m_yaMainMemory[0x1236] = 0x78;
            m_yaMainMemory[0x1237] = 0x89;
            m_yaMainMemory[0x1238] = 0xAF;

            byte[] yaMVX1 = { 0x08, 0x00, 0x12, 0x30, 0x12, 0x38 };
            ExecuteInstruction (yaMVX1);
            Assert (m_yaMainMemory[0x1230] == 0xA2);

            byte[] yaMVX2 = { 0x08, 0x01, 0x12, 0x31, 0x12, 0x38 };
            ExecuteInstruction (yaMVX2);
            Assert (m_yaMainMemory[0x1231] == 0xF3);

            byte[] yaMVX3 = { 0x08, 0x02, 0x12, 0x32, 0x12, 0x38 };
            ExecuteInstruction (yaMVX3);
            Assert (m_yaMainMemory[0x1232] == 0x3A);

            byte[] yaMVX4 = { 0x08, 0x03, 0x12, 0x33, 0x12, 0x38 };
            ExecuteInstruction (yaMVX4);
            Assert (m_yaMainMemory[0x1233] == 0x4F);

            m_iXR1 = 0x1200;
            m_iXR2 = 0x1232;

            m_yaMainMemory[0x1230] = 0x12;
            m_yaMainMemory[0x1231] = 0x23;
            m_yaMainMemory[0x1232] = 0x34;
            m_yaMainMemory[0x1233] = 0x45;
            m_yaMainMemory[0x1234] = 0x56;
            m_yaMainMemory[0x1235] = 0x67;
            m_yaMainMemory[0x1236] = 0x78;
            m_yaMainMemory[0x1237] = 0x89;
            m_yaMainMemory[0x1238] = 0xAF;

            byte[] yaMVX5 = { 0x58, 0x00, 0x30, 0x38 }; // XR1 & XR1
            ExecuteInstruction (yaMVX5);
            Assert (m_yaMainMemory[0x1230] == 0xA2);

            byte[] yaMVX6 = { 0x68, 0x01, 0x31, 0x06 }; // XR1 & XR2
            ExecuteInstruction (yaMVX6);
            Assert (m_yaMainMemory[0x1231] == 0xF3);

            byte[] yaMVX7 = { 0x98, 0x02, 0x00, 0x38 }; // XR2 & XR1
            ExecuteInstruction (yaMVX7);
            Assert (m_yaMainMemory[0x1232] == 0x3A);

            byte[] yaMVX8 = { 0xA8, 0x03, 0x01, 0x06 }; // XR2 & XR2
            ExecuteInstruction (yaMVX8);
            Assert (m_yaMainMemory[0x1233] == 0x4F);
        }

        public void TestSBF ()
        {
            byte[] yaSBF1 = { 0x3B, 0x10, 0x12, 0x34 };
            m_yaMainMemory[0x1234] = 0x3F;
            ExecuteInstruction (yaSBF1);
            Assert (m_yaMainMemory[0x1234] == 0x2F);

            m_iXR1 = 0x0500;
            m_iXR2 = 0x0600;
            byte[] yaSBF2 = { 0x7B, 0x10, 0x31 };
            m_yaMainMemory[0x0531] = 0x3F;
            ExecuteInstruction (yaSBF2);
            Assert (m_yaMainMemory[0x531] == 0x2F);

            m_yaMainMemory[0x0531] = 0x4F;
            ExecuteInstruction (yaSBF2);
            Assert (m_yaMainMemory[0x531] == 0x4F);

            byte[] yaSBF3 = { 0xBB, 0x10, 0x32 };
            m_yaMainMemory[0x0632] = 0x30;
            ExecuteInstruction (yaSBF3);
            Assert (m_yaMainMemory[0x0632] == 0x20);

            m_yaMainMemory[0x0632] = 0x22;
            ExecuteInstruction (yaSBF3);
            Assert (m_yaMainMemory[0x0632] == 0x22);

            byte[] yaSBF4 = { 0xBB, 0x00, 0x32 };
            m_yaMainMemory[0x0632] = 0x22;
            ExecuteInstruction (yaSBF4);
            Assert (m_yaMainMemory[0x0632] == 0x22);
        }

        public void TestSBN ()
        {
            byte[] yaSBN1 = { 0x3A, 0x10, 0x12, 0x34 };
            m_yaMainMemory[0x1234] = 0x0F;
            ExecuteInstruction (yaSBN1);
            Assert (m_yaMainMemory[0x1234] == 0x1F);

            m_yaMainMemory[0x1234] = 0x10;
            ExecuteInstruction (yaSBN1);
            Assert (m_yaMainMemory[0x1234] == 0x10);

            m_iXR1 = 0x0300;
            m_iXR2 = 0x0400;
            byte[] yaSBN2 = { 0x7A, 0x20, 0x11 };

            m_yaMainMemory[0x0311] = 0x0F;
            ExecuteInstruction (yaSBN2);
            Assert (m_yaMainMemory[0x0311] == 0x2F);

            m_yaMainMemory[0x0311] = 0x030;
            ExecuteInstruction (yaSBN2);
            Assert (m_yaMainMemory[0x0311] == 0x30);

            byte[] yaSBN3 = { 0xBA, 0x30, 0x22 };
            m_yaMainMemory[0x0422] = 0x40;
            ExecuteInstruction (yaSBN3);
            Assert (m_yaMainMemory[0x0422] == 0x70);

            m_yaMainMemory[0x0422] = 0xB0;
            ExecuteInstruction (yaSBN3);
            Assert (m_yaMainMemory[0x0422] == 0xB0);

            byte[] yaSBN4 = { 0xBA, 0x00, 0x23 };
            m_yaMainMemory[0x0423] = 0xFF;
            ExecuteInstruction (yaSBN3);
            Assert (m_yaMainMemory[0x0423] == 0xFF);
        }

        public void TestMVI ()
        {
            byte[] yaMVI1 = { 0x3C, 0xAA, 0x12, 0x34 };
            ExecuteInstruction (yaMVI1);
            Assert (m_yaMainMemory[0x1234] == 0xAA);

            m_iXR1 = 0x0100;
            m_iXR2 = 0x0200;
            byte[] yaMVI2 = { 0x7C, 0xBB, 0x11 };
            ExecuteInstruction (yaMVI2);
            Assert (m_yaMainMemory[0x0111] == 0xBB);

            byte[] yaMVI3 = { 0xBC, 0xCC, 0x22 };
            ExecuteInstruction (yaMVI3);
            Assert (m_yaMainMemory[0x0222] == 0xCC);
        }

        public void TestLA ()
        {
            byte[] yaLA1 = { 0xC2, 0x03, 0x01, 0x00 };
            ExecuteInstruction (yaLA1);
            Assert (m_iXR1 == 0x0100);
            Assert (m_iXR2 == 0x0100);

            byte[] yaLA2 = { 0xD2, 0x01, 0x51 };
            ExecuteInstruction (yaLA2);
            Assert (m_iXR1 == 0x0151);
            Assert (m_iXR2 == 0x0100);

            byte[] yaLA3 = { 0xE2, 0x02, 0x80 };
            ExecuteInstruction (yaLA3);
            Assert (m_iXR1 == 0x0151);
            Assert (m_iXR2 == 0x0180);
        }
        #endregion

        #region Test I/O: 5203 Line Printer
        public void TestLIO5203 ()
        {
        }

        public void TestSIO5203 ()
        {
        }

        public void TestSNS5203 ()
        {
        }

        public void TestTIO5203 ()
        {
        }

        public void TestAPL5203 ()
        {
        }
        #endregion

        #region Test I/O: 5424 MFCU
        public void TestLIO5424 ()
        {
        }

        public void TestSIO5424 ()
        {
        }

        public void TestSNS5424 ()
        {
        }

        public void TestTIO5424 ()
        {
        }

        public void TestAPL5424 ()
        {
        }
        #endregion

        #region Test I/O: 5444 Disk Drive
        public void TestLIO5444 ()
        {
        }

        public void TestSIO5444 ()
        {
        }

        public void TestSNS5444 ()
        {
        }

        public void TestTIO5444 ()
        {
        }

        public void TestAPL5444 ()
        {
        }
        #endregion

        #region Test I/O: 5475 Data Entry Keyboard
        public void TestLIO5475 ()
        {
        }

        public void TestSIO5475 ()
        {
        }

        public void TestSNS5475 ()
        {
        }

        public void TestTIO5475 ()
        {
        }

        public void TestAPL5475 ()
        {
        }
        #endregion

        #region Test I/O: 5471 Keyboard/Printer
        public void TestLIO5471 ()
        {
        }

        public void TestSIO5471 ()
        {
        }

        public void TestSNS5471 ()
        {
        }

        public void TestTIO5471 ()
        {
        }

        public void TestAPL5471 ()
        {
        }
        #endregion

        #region Emulator Support Methods
        private void Assert (bool bTest)
        {
            if (!bTest)
            {
                WaveRedFlag ("Assertion Failure");
            }
        }

        private void Assert (bool bTest, string strMessage)
        {
            if (!bTest)
            {
                WaveRedFlag (strMessage);
            }
        }

        private void FillArea (int iStart, int iEnd, byte yValue)
        {
            for (int iIdx = iStart; iIdx < iEnd; iIdx++)
            {
                m_yaMainMemory[iIdx] = yValue;
            }
        }

        private int LoadInt (int iAddress, int iLength)
        {
            if (iLength > 4)
            {
                iLength = 4;
            }

            int iData = 0;

            for (int iIdx = 0; iIdx < iLength; iIdx++)
            {
                iData <<= 8;
                iData |= m_yaMainMemory[iAddress - (3 - iIdx)];
            }

            return iData;
        }

        private void StoreInt (int iData, int iAddress, int iLength)
        {
            if (iLength > 4)
            {
                iLength = 4;
            }

            for (int iIdx = 0; iIdx < iLength; iIdx++)
            {
                m_yaMainMemory[iAddress - iIdx] = (byte)(iData & 0xFF);
                iData >>= 8;
            }
        }

        private long LoadZonedLong (int iAddress, int iLength)
        {
            long lZoned = 0;

            //for (int iIdx = 0; iIdx < iLength; iIdx++)
            //{
            //    int iOffset = iLength - iIdx - 1;
            //    int iTestIdx = iLength - iOffset - 1;
            //    int iTestAddress = iAddress - iOffset;
            //    Console.WriteLine ("iIdx: {0:D2}  iOffset: {1:D2}  iTestIdx: {2:D2}  iTestAddress: {3:X4}", iIdx, iOffset, iTestIdx, iTestAddress);
            //}

            for (int iIdx = 0; iIdx < iLength; iIdx++)
            {
                int iOffset = iLength - iIdx - 1;
                int iLoopAddress = iAddress - iOffset;

                lZoned *= 10;
                char cDigit = (char)m_yaMainMemory[iLoopAddress];
                int iDigitValue = (int)(cDigit & 0x0F);
                lZoned += (long)iDigitValue;
            }

            if ((m_yaMainMemory[iAddress] & 0xF0) != (byte)0xF0)
            {
                lZoned *= -1;
            }

            return lZoned;
        }

        private void StoreZonedLong (long lData, int iAddress, int iLength)
        {
            if (iLength > 31)
            {
                iLength = 31;
            }

            bool bNegative = false;
            if (lData < 0)
            {
                bNegative = true;
                lData *= -1;
            }

            StringBuilder strbldrZoned = new StringBuilder (32);
            string strZoned = lData.ToString ();
            if (iLength > strZoned.Length)
            {
                strbldrZoned.Append (new string ('0', iLength - strZoned.Length));
            }

            strbldrZoned.Append (strZoned);

            iLength--;
            for (int iOffset = iLength; iOffset >= 0; iOffset--)
            {
                m_yaMainMemory[iAddress - iOffset] = (byte)strbldrZoned.ToString ()[iLength - iOffset];
                m_yaMainMemory[iAddress - iOffset] |= 0xF0;
            }

            if (bNegative)
            {
                m_yaMainMemory[iAddress] &= 0xDF;
            }
        }

        private void StoreString (string strData, int iStartAddress)
        {
            for (int iIdx = 0; iIdx < strData.Length; iIdx++)
            {
                m_yaMainMemory[iStartAddress + iIdx] = (byte)strData[iIdx];
            }
        }

        private string LoadString (int iStartAddress, int iLength)
        {
            StringBuilder strbldrData = new StringBuilder (iLength);

            for (int iIdx = 0; iIdx < iLength; iIdx++)
            {
                strbldrData.Append ((char)m_yaMainMemory[iStartAddress + iIdx]);
            }

            return strbldrData.ToString ();
        }

        private void StoreStringEBCDIC (string strData, int iStartAddress)
        {
            for (int iIdx = 0; iIdx < strData.Length; iIdx++)
            {
                m_yaMainMemory[iStartAddress + iIdx] = ConvertASCIItoEBCDIC ((byte)strData[iIdx]);
            }
        }

        private string LoadStringEBCDIC (int iStartAddress, int iLength)
        {
            StringBuilder strbldrData = new StringBuilder (iLength);

            for (int iIdx = 0; iIdx < iLength; iIdx++)
            {
                strbldrData.Append ((char)ConvertEBCDICtoASCII (m_yaMainMemory[iStartAddress + iIdx]));
            }

            return strbldrData.ToString ();
        }
        #endregion

        // Imported from TestDriver.cs class CTestDriver
        // Command lines to parse:
        // /PRNT:<characters to show as punch pattern>
        //   Groups of PRNT lines indicate multiple lines, as wide as the longest line with all others centered
        // /Load:<start source address>, <start target address>, <length - default entire card>
        //    One load card per IPL card, otherwise IPL card loads at 0x0040 higher than previous card
        //    Not needed with text card group
        // /Code:<address>,<description line in quotes>
        // /Data:<address>,<description line in quotes>
        // /Tag: <address of instruction>,<tag name> (longest tag defines column offset in dumps)
        // /Comment: <address of instruction>,<comment to show to right of instruction>
        // /Name: <Title of following section> to be displayed w/ 2 blank lines above & 1 below
        // /Entry:<address of program entry point>, overrides value found in end card
        static void TestSystem3ToolsLib (bool bDoAllTests)
        {
            CDataConversion objSystem3Tools = new CDataConversion ();

            if (bDoAllTests)
            {
                #region LegacyTestCases
                bool bDoLegacyTestCases = false;
                if (bDoLegacyTestCases)
                {
                    string strTest1 = "one,two,three,four";
                    char[] caDelimiters1 = { ',' };
                    string strTest2 = "/Load:0030,003f,1080";
                    string strTest3 = "/Data:0030,003f,\"Data Area\"";
                    char[] caDelimiters2 = { '/', ':', ',', '\"' };
                    string strDelimiters = "/:,\"";

                    List<string> strlTokens = objSystem3Tools.ParseStringToList (strTest1, strDelimiters);
                    objSystem3Tools.PrintStringList (strlTokens);
                    strlTokens = objSystem3Tools.ParseStringToList (strTest2, strDelimiters);
                    objSystem3Tools.PrintStringList (strlTokens);
                    strlTokens = objSystem3Tools.ParseStringToList (strTest3, strDelimiters);
                    objSystem3Tools.PrintStringList (strlTokens);

                    string[] straOutput = strTest1.Split (caDelimiters1);
                    foreach (string str in straOutput)
                    {
                        if (str.Length > 0)
                        {
                            Console.WriteLine (str);
                        }
                    }

                    straOutput = strTest2.Split (caDelimiters2);
                    foreach (string str in straOutput)
                    {
                        if (str.Length > 0)
                        {
                            Console.WriteLine (str);
                        }
                    }

                    straOutput = strTest3.Split (caDelimiters2);
                    foreach (string str in straOutput)
                    {
                        if (str.Length > 0)
                        {
                            Console.WriteLine (str);
                        }
                    }

                    TestHexAndCharacterDump (objSystem3Tools);
                    TestIPLCardReader (objSystem3Tools);
                    TestPunchPattern (objSystem3Tools);
                    TestTextCardReader (objSystem3Tools);
                    TestDisassembler (objSystem3Tools);
                }
                #endregion

                TestEbcdicCharacters1 (objSystem3Tools);
                TestEbcdicCharacters2 (objSystem3Tools);
                TestEbcdicCharacters3 (objSystem3Tools);
                TestEbcdicCharacters5 (objSystem3Tools);
                TestEbcdicCharacters6 (objSystem3Tools);
                TestEbcdicCharacters7 (objSystem3Tools);

                #region Test Dumping Program Files
                Console.WriteLine ("Begin RippleMaybe.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\RippleMaybe.OBJTXT", false, false, false));
                //Console.WriteLine ("End RippleMaybe.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin TextJokeCards.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\TextJokeCards.OBJTXT", false, false, false));
                Console.WriteLine ("End TextJokeCards.OBJTXT");

                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                Console.WriteLine ("Begin PRNT.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\PRNT.OBJTXT", false, false, false));
                //Console.WriteLine ("End PRNT.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin PRNT.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileImage
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\PRNT.OBJTXT"));
                //Console.WriteLine ("End PRNT.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin PRNT.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleTextFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\PRNT.OBJTXT"));
                Console.WriteLine ("End PRNT.OBJTXT");

                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                Console.WriteLine ("Begin SingleIPLcards.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpIplFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - IPL\SingleIPLcards.OBJTXT"));
                //Console.WriteLine ("End SingleIPLcards.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin SingleIPLcards.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleIplFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - IPL\SingleIPLcards.OBJTXT"));
                Console.WriteLine ("End SingleIPLcards.OBJTXT");

                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                Console.WriteLine ("Begin IBM_Diagnostics.IPL");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpIplFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - IPL\IBM_Diagnostics.IPL"));
                //Console.WriteLine ("End IBM_Diagnostics.IPL");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin IBM_Diagnostics.IPL");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleIplFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - IPL\IBM_Diagnostics.IPL"));
                Console.WriteLine ("End IBM_Diagnostics.IPL");

                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                Console.WriteLine ("Begin MultiCardIPL&LabelPairs.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpIplFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - IPL\MultiCardIPL&LabelPairs.OBJTXT"));
                //Console.WriteLine ("End MultiCardIPL&LabelPairs.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin MultiCardIPL&LabelPairs.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleIplFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - IPL\MultiCardIPL&LabelPairs.OBJTXT"));
                Console.WriteLine ("End MultiCardIPL&LabelPairs.OBJTXT");

                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                Console.WriteLine ("Begin IPLcardGangPunchGroups.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpIplFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - IPL\IPLcardGangPunchGroups.OBJTXT"));
                //Console.WriteLine ("End IPLcardGangPunchGroups.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin IPLcardGangPunchGroups.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleIplFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - IPL\IPLcardGangPunchGroups.OBJTXT"));
                Console.WriteLine ("End IPLcardGangPunchGroups.OBJTXT");

                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                Console.WriteLine ("Begin SingleCardIPL&LabelGroups.TXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpIplFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - IPL\SingleCardIPL&LabelGroups.TXT"));
                //Console.WriteLine ("End SingleCardIPL&LabelGroups.TXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin SingleCardIPL&LabelGroups.TXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleIplFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - IPL\SingleCardIPL&LabelGroups.TXT"));
                Console.WriteLine ("End SingleCardIPL&LabelGroups.TXT");

                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                Console.WriteLine ("Begin HexSource&IPLgroups.TXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpIplFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - IPL\HexSource&IPLgroups.TXT"));
                //Console.WriteLine ("End HexSource&IPLgroups.TXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin HexSource&IPLgroups.TXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleIplFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - IPL\HexSource&IPLgroups.TXT"));
                Console.WriteLine ("End HexSource&IPLgroups.TXT");

                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                Console.WriteLine ("Begin PLLT.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\PLLT.OBJTXT", false, false, false));
                //Console.WriteLine ("End PLLT.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin PLLT.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileImage
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\PLLT.OBJTXT"));
                //Console.WriteLine ("End PLLT.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin PLLT.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleTextFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\PLLT.OBJTXT"));
                Console.WriteLine ("End PLLT.OBJTXT");

                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                Console.WriteLine ("Begin TYPE.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\TYPE.OBJTXT", false, false, false));
                //Console.WriteLine ("End TYPE.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin TYPE.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileImage
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\TYPE.OBJTXT"));
                //Console.WriteLine ("End TYPE.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin TYPE.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleTextFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\TYPE.OBJTXT"));
                Console.WriteLine ("End TYPE.OBJTXT");

                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                Console.WriteLine ("Begin PLTX.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\PLTX.OBJTXT", false, false, false));
                //Console.WriteLine ("End PLTX.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin PLTX.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileImage
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\PLTX.OBJTXT"));
                //Console.WriteLine ("End PLTX.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin PLTX.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleTextFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\PLTX.OBJTXT"));
                Console.WriteLine ("End PLTX.OBJTXT");

                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                Console.WriteLine ("Begin KYBD.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\KYBD.OBJTXT", false, false, false));
                //Console.WriteLine ("End KYBD.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin KYBD.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileImage
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\KYBD.OBJTXT"));
                //Console.WriteLine ("End KYBD.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin KYBD.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleTextFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\KYBD.OBJTXT"));
                Console.WriteLine ("End KYBD.OBJTXT");

                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                Console.WriteLine ("Begin HXPL.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\HXPL.OBJTXT", false, false, false));
                //Console.WriteLine ("End HXPL.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin HXPL.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileImage
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\HXPL.OBJTXT"));
                //Console.WriteLine ("End HXPL.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin HXPL.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleTextFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\HXPL.OBJTXT"));
                Console.WriteLine ("End HXPL.OBJTXT");

                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                Console.WriteLine ("Begin TXLT.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\TXLT.OBJTXT", false, false, false));
                //Console.WriteLine ("End TXLT.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin TXLT.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileImage
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\TXLT.OBJTXT"));
                //Console.WriteLine ("End TXLT.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin TXLT.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleTextFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\TXLT.OBJTXT"));
                Console.WriteLine ("End TXLT.OBJTXT");

                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                Console.WriteLine ("Begin KBST.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\KBST.OBJTXT", false, false, false));
                //Console.WriteLine ("End KBST.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin KBST.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileImage
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\KBST.OBJTXT"));
                //Console.WriteLine ("End KBST.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin KBST.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleTextFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\KBST.OBJTXT"));
                Console.WriteLine ("End KBST.OBJTXT");

                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                Console.WriteLine ("Begin KBPL.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\KBPL.OBJTXT", false, false, false));
                //Console.WriteLine ("End KBPL.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin KBPL.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileImage
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\KBPL.OBJTXT"));
                //Console.WriteLine ("End KBPL.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin KBPL.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleTextFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\KBPL.OBJTXT"));
                Console.WriteLine ("End KBPL.OBJTXT");

                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                Console.WriteLine ("Begin Expander.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\Expander.OBJTXT", false, false, false));
                //Console.WriteLine ("End Expander.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin Expander.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileImage
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\Expander.OBJTXT"));
                //Console.WriteLine ("End Expander.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin Expander.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleTextFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\Expander.OBJTXT"));
                Console.WriteLine ("End Expander.OBJTXT");

                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                Console.WriteLine ("Begin MnemonicDump.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\MnemonicDump.OBJTXT", false, false, false));
                //Console.WriteLine ("End MnemonicDump.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin MnemonicDump.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileImage
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\MnemonicDump.OBJTXT"));
                //Console.WriteLine ("End MnemonicDump.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin MnemonicDump.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleTextFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\MnemonicDump.OBJTXT"));
                Console.WriteLine ("End MnemonicDump.OBJTXT");

                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                Console.WriteLine ("Begin Trace.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\Trace.OBJTXT", false, false, false));
                //Console.WriteLine ("End Trace.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin Trace.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileImage
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\Trace.OBJTXT"));
                //Console.WriteLine ("End Trace.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin Trace.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleTextFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\Trace.OBJTXT"));
                Console.WriteLine ("End Trace.OBJTXT");

                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                Console.WriteLine ("Begin HeaderDump.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\HeaderDump.OBJTXT", false, false, false));
                //Console.WriteLine ("End HeaderDump.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin HeaderDump.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileImage
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\HeaderDump.OBJTXT"));
                //Console.WriteLine ("End HeaderDump.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin HeaderDump.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleTextFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\HeaderDump.OBJTXT"));
                Console.WriteLine ("End HeaderDump.OBJTXT");

                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                Console.WriteLine ("Begin BLCK.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\BLCK.OBJTXT", false, false, false));
                //Console.WriteLine ("End BLCK.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin BLCK.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileImage
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\BLCK.OBJTXT"));
                //Console.WriteLine ("End BLCK.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin BLCK.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleTextFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\BLCK.OBJTXT"));
                Console.WriteLine ("End BLCK.OBJTXT");

                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                Console.WriteLine ("Begin IPLCardDump.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\IPLCardDump.OBJTXT", false, false, false));
                //Console.WriteLine ("End IPLCardDump.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin IPLCardDump.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileImage
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\IPLCardDump.OBJTXT"));
                //Console.WriteLine ("End IPLCardDump.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin IPLCardDump.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleTextFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\IPLCardDump.OBJTXT"));
                Console.WriteLine ("End IPLCardDump.OBJTXT");

                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                Console.WriteLine ("Begin Remake.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\Remake.OBJTXT", false, false, false));
                //Console.WriteLine ("End Remake.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin Remake.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileImage
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\Remake.OBJTXT"));
                //Console.WriteLine ("End Remake.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin Remake.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleTextFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\Remake.OBJTXT"));
                Console.WriteLine ("End Remake.OBJTXT");

                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                Console.WriteLine ("Begin PowerOfTwo.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\PowerOfTwo.OBJTXT", false, false, false));
                //Console.WriteLine ("End PowerOfTwo.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin PowerOfTwo.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileImage
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\PowerOfTwo.OBJTXT"));
                //Console.WriteLine ("End PowerOfTwo.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin PowerOfTwo.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleTextFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\PowerOfTwo.OBJTXT"));
                Console.WriteLine ("End PowerOfTwo.OBJTXT");

                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                Console.WriteLine ("Begin TrigTables.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\TrigTables.OBJTXT", false, false, false));
                //Console.WriteLine ("End TrigTables.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin TrigTables.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileImage
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\TrigTables.OBJTXT"));
                //Console.WriteLine ("End TrigTables.OBJTXT");
                Console.WriteLine ("   - - - - - - - - - - - - - - -");
                //Console.WriteLine ("Begin TrigTables.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleTextFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\TrigTables.OBJTXT"));
                Console.WriteLine ("End TrigTables.OBJTXT");
                #endregion
            }
            else
            {
                Console.WriteLine ("Begin KYBD.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleTextFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\KYBD.OBJTXT"));
                Console.WriteLine ("End KYBD.OBJTXT");

                Console.WriteLine ("Begin HXPL.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleTextFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\HXPL.OBJTXT"));
                Console.WriteLine ("End HXPL.OBJTXT");

                Console.WriteLine ("Begin TXLT.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleTextFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\TXLT.OBJTXT"));
                Console.WriteLine ("End TXLT.OBJTXT");

                Console.WriteLine ("Begin KBPL.OBJTXT");
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleTextFile
                    (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\KBPL.OBJTXT"));
                Console.WriteLine ("End KBPL.OBJTXT");
            }
        }

        static void TestEmulatorMethods ()
        {
            CS3UnitTest objS3UnitTest = new CS3UnitTest ();

#if MysteryToSolve
            // Why does objS3UnitTest.m_yaPowersOfTwoProgramV produce different out the second time?
            objS3UnitTest.PrintStringList (objS3UnitTest.DisassembleCode (objS3UnitTest.m_yaPowersOfTwoProgramV));
            objS3UnitTest.TestRun (objS3UnitTest.m_yaPowersOfTwoProgramV);
            objS3UnitTest.TestRun (objS3UnitTest.m_yaPowersOfTwoProgramV);
#endif

            objS3UnitTest.TestHaltDisplay ();
            objS3UnitTest.Test5475Display ();
            objS3UnitTest.TestFetchAndGetAddress ();
            objS3UnitTest.TestConditionRegister ();
            objS3UnitTest.TestSupportMethods ();
            objS3UnitTest.TestCPUInstructions ();

            objS3UnitTest.TestInstructions5203LinePrinter ();
            objS3UnitTest.TestInstructions5424MFCU ();
            objS3UnitTest.TestInstructions5444DiskDrive ();
            objS3UnitTest.TestInstructions5475DataEntryKeyboard ();
            objS3UnitTest.TestInstructions5471KeyboardPrinter ();

            objS3UnitTest.TestRun (objS3UnitTest.m_yaLoopingHaltProgram);
            objS3UnitTest.TestRun (objS3UnitTest.m_yaCountingHaltProgramI);

            //objS3UnitTest.PrintStringList (objS3UnitTest.DisassembleCode (objS3UnitTest.m_yaCountingHaltProgramII));
            objS3UnitTest.TestRun (objS3UnitTest.m_yaCountingHaltProgramII); // Runs without end

            //objS3UnitTest.PrintStringList (objS3UnitTest.DisassembleCode (objS3UnitTest.m_yaCountingHaltProgramIV));
            objS3UnitTest.TestRun (objS3UnitTest.m_yaCountingHaltProgramIV); // Runs without end

            //objS3UnitTest.PrintStringList (objS3UnitTest.DisassembleCode (objS3UnitTest.m_yaCountingHaltProgramV));
            objS3UnitTest.TestRun (objS3UnitTest.m_yaCountingHaltProgramV); // Runs without end

            objS3UnitTest.TestRun (objS3UnitTest.m_yaKeyboardClockProgramV); // Runs without end

            //objS3UnitTest.PrintStringList (objS3UnitTest.DisassembleCode (objS3UnitTest.m_yaPowersOfTwoProgramV));
            //objS3UnitTest.TestRun (objS3UnitTest.m_yaPowersOfTwoProgramV);

#if MysteryToSolve
            objS3UnitTest.PrintStringList (objS3UnitTest.DisassembleCode (objS3UnitTest.m_yaOneCardCoreDumpProgram));
            objS3UnitTest.TestRun (objS3UnitTest.m_yaOneCardCoreDumpProgram); // Runs without end
            // Problem w/ original code or emulator?
            // 16ÿ             C201FFFF4C296A01295C054F6AC20200106802210068032500C20200F89C00CCCAT SOFTWARE

            objS3UnitTest.PrintStringList (objS3UnitTest.DisassembleCode (objS3UnitTest.m_yaRipplePrintProgram));
            objS3UnitTest.TestRun (objS3UnitTest.m_yaRipplePrintProgram); // Runs without end
#endif
            
            objS3UnitTest.TestRun (objS3UnitTest.m_ya24kDiskSystemInitialization);

#if MysteryToSolve
            objS3UnitTest.TestRun (objS3UnitTest.m_yaOneCardPowersOfTwoProgram);
            objS3UnitTest.ProgramLoad (EBootDevice.BOOT_Card, @"D:\SoftwareDev\IBMSystem3\Data Files\TestData\96-96 List Test.txt");
            objS3UnitTest.AssignFileToSecondaryHopper (@"D:\SoftwareDev\IBMSystem3\Data Files\TestData\96-96 Compare Test2.txt");
            objS3UnitTest.ProgramLoad (EBootDevice.BOOT_Card, @"D:\SoftwareDev\IBMSystem3\Data Files\TestData\96-96 Compare Test.txt");
#endif

            objS3UnitTest.ProgramLoad (EBootDevice.BOOT_Card, @"D:\SoftwareDev\IBMSystem3\Data Files\Test Data\PRNT-Test.TXT");
            objS3UnitTest.AssignFileToSecondaryHopper (@"D:\SoftwareDev\IBMSystem3\Data Files\TestData\IBM Set.txt");
            objS3UnitTest.AssignFileToSecondaryHopper (@"D:\SoftwareDev\IBMSystem3\Data Files\TestData\IPL PRNT Test Secondary.txt");
            objS3UnitTest.ProgramLoad (EBootDevice.BOOT_Card, @"D:\SoftwareDev\IBMSystem3\Data Files\TestData\IPL PRNT Test Primary.txt");
            objS3UnitTest.ProgramLoad (EBootDevice.BOOT_Card, @"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\Testing\PRNT.OBJTXT");
            objS3UnitTest.ProgramLoad (EBootDevice.BOOT_Card, @"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\Testing\PowerOfTwo.OBJTXT", "");
            objS3UnitTest.ProgramLoad (EBootDevice.BOOT_Card, @"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\TrigTables.OBJTXT");

#if MysteryToSolve
            objS3UnitTest.ProgramLoad (EBootDevice.BOOT_Card, @"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - IPL\CPU_&_MEMORY_TEST.txt");
            objS3UnitTest.ProgramLoad (EBootDevice.BOOT_Card, @"D:\DataFiles\IBM Symulator3\Cards Read\Unknown From SingleIPLCards_matched.txt");
            objS3UnitTest.ProgramLoad (EBootDevice.BOOT_Disk_Fixed, @"D:\SoftwareDev\IBMSystem3\Data Files\Drive Images\5702-sc1.V16.scp.dsk",
                                                                    @"D:\SoftwareDev\IBMSystem3\Data Files\Drive Images\5702SCP.dsk");
            objS3UnitTest.ProgramLoad (EBootDevice.BOOT_Disk_Fixed, @"C:\Tools\disks\f1f1f1.dsk",
                                                                    @"C:\Tools\disks/5702SCP.dsk");
#endif
        }

        static void RunSystem3Diagnostics ()
        {
            CS3UnitTest objS3UnitTest = new CS3UnitTest ();

            objS3UnitTest.AutoTagJump = true;
            objS3UnitTest.AutoTagLoop = true;
            objS3UnitTest.ExtendMnemonicBC = true;
            objS3UnitTest.ExtendMnemonicJC = true;
            objS3UnitTest.ExtendMnemonicMVX = true;
            objS3UnitTest.SetTrace ();
            objS3UnitTest.SetShowDisassembly ();
            objS3UnitTest.ProgramLoad (EBootDevice.BOOT_Card, @"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - Text\Testing\PowerOfTwo.OBJTXT", "");
            objS3UnitTest.ResetTrace ();
            objS3UnitTest.ResetShowDisassembly ();
            objS3UnitTest.PrintStringList (objS3UnitTest.DisassembleIplFile
                (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - IPL\CPU_&_MEMORY_TEST.txt"));
            objS3UnitTest.AutoTagJump = false;
            objS3UnitTest.AutoTagLoop = false;
            objS3UnitTest.ExtendMnemonicBC = false;
            objS3UnitTest.ExtendMnemonicJC = false;
            objS3UnitTest.ExtendMnemonicMVX = false;

            objS3UnitTest.FillArea (0x0000, 0xFFFF, 0xFE); // Initialze memory for diagnostics
            objS3UnitTest.ClearMemory = false;
            objS3UnitTest.ConsoleDials = 0x00FE;
            objS3UnitTest.SetTrace ();
            objS3UnitTest.ProgramLoad (EBootDevice.BOOT_Card, @"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\Object - IPL\CPU_&_MEMORY_TEST.txt");
            objS3UnitTest.ResetTrace ();
            objS3UnitTest.ClearMemory = true;
        }

        // Separate Test Methods
        static void TestHexAndCharacterDump (CDataConversion objSystem3Tools)
        {
            // Test CHexAndCharacterDump class
            Console.WriteLine ("Test CHexAndCharacterDump class");

            List<string> strlHexLines = objSystem3Tools.ReadFileToStringList (@"D:\SoftwareDev\IBMSystem3\Data Files\Test Data\HexLines.txt", 0);
            foreach (string str in strlHexLines)
            {
                objSystem3Tools.PrintStringList (objSystem3Tools.BinaryToDump (objSystem3Tools.PackHexToBinary (str)));
            }

            List<string> strlTextCards = objSystem3Tools.ReadFileToStringList (@"D:\SoftwareDev\IBMSystem3\Data Files\Test Data\TextCards.txt", 96);
            objSystem3Tools.PrintStringList (objSystem3Tools.BinaryToDump (objSystem3Tools.ReadTextCard (strlTextCards[1])));
        }

        static void TestIPLCardReader (CDataConversion objSystem3Tools)
        {
            // Test IPL card reader
            Console.WriteLine ("IPL Card Test 1");
            List<string> strlIplCards = objSystem3Tools.ReadFileToStringList (@"D:\SoftwareDev\IBMSystem3\Data Files\Test Data\IPLCards.txt", 96);
            foreach (string str in strlIplCards)
            {
                byte[] yaIplBinary = objSystem3Tools.ReadIPLCard (str);
                objSystem3Tools.PrintStringList (objSystem3Tools.BinaryToDump (yaIplBinary));
            }
        }

        static void TestPunchPattern (CDataConversion objSystem3Tools)
        {
            // Test CPunchPattern class
            Console.WriteLine ("Test CPunchPattern class");
            objSystem3Tools.PrintPunchStringList (objSystem3Tools.CreatePunchImage
                ("ABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789+_!:;=&<>}| ~%?'\"()@#*$-,./", 'x', true));

            List<string> strlPunchPatternCards = objSystem3Tools.ReadFileToStringList
                (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\PunchPattern\PunchPatternCards.OLD", 96);
            foreach (string str in strlPunchPatternCards)
            {
                Console.WriteLine (str);
                objSystem3Tools.PrintPunchStringList (objSystem3Tools.CreatePunchImage (str, 'o', true));
            }

            strlPunchPatternCards = objSystem3Tools.ReadFileToStringList
                (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\PunchPattern\DeckOfCards.TXT", 96);
            foreach (string str in strlPunchPatternCards)
            {
                Console.WriteLine (str);
                objSystem3Tools.PrintPunchStringList (objSystem3Tools.CreatePunchImage (str, 'o', true));
            }

            strlPunchPatternCards = objSystem3Tools.ReadFileToStringList
                (@"D:\SoftwareDev\IBMSystem3\Data Files\CardDecksReadToFiles\PunchPattern\PunchPatternCards.TXT", 96);
            foreach (string str in strlPunchPatternCards)
            {
                Console.WriteLine (str);
                objSystem3Tools.PrintPunchStringList (objSystem3Tools.CreatePunchImage (str, 'o', true));
            }

            strlPunchPatternCards = objSystem3Tools.ReadFileToStringList
                (@"D:\SoftwareDev\IBMSystem3\Data Files\Character Set Cards\ASCII.txt", 96);
            foreach (string str in strlPunchPatternCards)
            {
                Console.WriteLine (str);
                objSystem3Tools.PrintPunchStringList (objSystem3Tools.CreatePunchImage (str, 'o', true));
            }

            strlPunchPatternCards = objSystem3Tools.ReadFileToStringList
                (@"D:\SoftwareDev\IBMSystem3\Data Files\Test Data\PunchPatternCards.txt", 96);
            foreach (string str in strlPunchPatternCards)
            {
                Console.WriteLine (str);
                objSystem3Tools.PrintPunchStringList (objSystem3Tools.CreatePunchImage (str, 'o', true));
            }
        }

        static void TestTextCardReader (CDataConversion objSystem3Tools)
        {
            // Test Text card reader
            Console.WriteLine ("Text Card Test");
            objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                (@"D:\SoftwareDev\IBMSystem3\Data Files\Test Data\TextCards.txt", false, false, false));
            objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                (@"D:\SoftwareDev\IBMSystem3\Data Files\Test Data\TextCards.txt", true, false, false));
            objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                (@"D:\SoftwareDev\IBMSystem3\Data Files\Test Data\TextCards.txt", false, true, false));
            objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                (@"D:\SoftwareDev\IBMSystem3\Data Files\Test Data\TextCards.txt", true, true, false));

            objSystem3Tools.PrintStringList (objSystem3Tools.ReadFileToStringList
                (@"C:\Documents and Settings\ge97690\My Documents\My Stuff\IBM System 3\ASCII.txt", 96));  // Bad filename
            objSystem3Tools.PrintStringList (objSystem3Tools.ReadFileToStringList
                (@"D:\SoftwareDev\IBMSystem3\Data Files\Test Data\ASCII.txt", 96));  // Valid filename
        }

        static void TestDisassembler (CDataConversion objSystem3Tools)
        {
            // Test CDisassembler class
            Console.WriteLine ("Test CDisassembler class");
            List<string> strlHexLines = objSystem3Tools.ReadFileToStringList
                (@"D:\SoftwareDev\IBMSystem3\Data Files\Test Data\HexLines.txt", 0);
            List<string> strlIplCards = objSystem3Tools.ReadFileToStringList
                (@"D:\SoftwareDev\IBMSystem3\Data Files\Test Data\IPLCards.txt", 96);
            objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleCode (objSystem3Tools.ReadIPLCard (strlIplCards[4])));
            foreach (string str in strlHexLines)
            {
                objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleCode (objSystem3Tools.PackHexToBinary (str)));
            }
        }

        static void TestDisassemblerLabels ()
        {
            CDataConversion objSystem3Tools = new CDataConversion ();

            byte[] yaSingleIPLcards2 = { 0xC2, 0x01, 0x00, 0x00, // LA 
                                         0xF3, 0x10, 0x0F,       // SIO
                                         0xF2, 0x00, 0x19,       // JC 
                                         0x75, 0x20, 0x2F,       // L  
                                         0x70, 0x11, 0x4F,       // SNS
                                         0xD0, 0x00, 0x04,       // BC 
                                         0x58, 0x02, 0x1E, 0x4E, // MVX
                                         0x58, 0x03, 0x22, 0x4E, // MVX
                                         0x5C, 0x00, 0x27, 0x30, // MVC
                                         0x5C, 0x00, 0x28, 0x30, // MVC
                                         0x75, 0xC0, 0x2D,       // L  
                                         0xF0, 0x00, 0x00,       // HPL
                                         0xD0, 0x00, 0x26 };     // BC 
            //              0000: LA    C2 01 0000
            //              0004: SIO   F3 10 0F
            //              0007: J     F2 00 19
            //              000A: L     75 20 2F,1
            //              000D: SNS   70 11 4F,1
            //              0010: B     D0 00 04,1
            //              0013: MNZ   58 02 1E,1 4E,1
            //              0017: MNN   58 03 22,1 4E,1
            //              001B: MVC   5C 00 27,1 30,1
            //              001F: MVC   5C 00 28,1 30,1
            //              0023: L     75 C0 2D,1
            //Jump_01_0026  0026: HPL   F0 00 00         Halt display: "  "
            //              0029: B     D0 00 26,1

            byte[] yaSingleIPLcards3 = { 0xC2, 0x01, 0x00, 0x00, // LA 
                                         0xC2, 0x02, 0x00, 0x00,
                                         0xB5, 0x01, 0x35,
                                         0xB1, 0xE4, 0x02,
                                         0xB1, 0xE6, 0x35,
                                         0x6C, 0x02, 0xAA, 0x3B,
                                         0xF3, 0xF0, 0x00,
                                         0x70, 0xF0, 0x01,
                                         0x78, 0x04, 0x01,
                                         0xE0, 0x90, 0x14,
                                         0x70, 0xF0, 0x01,
                                         0x7A, 0xF0, 0x01,
                                         0x5C, 0x00, 0x00, 0x01,
                                         0xD2, 0x01, 0x01,
                                         0x7D, 0xFC, 0x47,
                                         0xE0, 0x01, 0x1D,
                                         0xF3, 0xE2, 0x03,
                                         0xF0, 0x01, 0x7C,
                                         0xE0, 0x87, 0x04,
                                         0xF5, 0xFD, 0xFC, 0xF0, 0xC6, 0xF0, 0xC2 };
            //              0000: LA    C2 02 0000
            //              0004: L     B5 01 35,2
            //              0007: LIO   B1 E4 02,2
            //              000A: LIO   B1 E6 35,2
            //              000D: MVC   6C 02 AA,1 3B,2
            //              0011: SIO   F3 F0 00
            //              0014: SNS   70 F0 01,1
            //              0017: TBN   78 04 01,1
            //              001A: BC    E0 90 14,2
            //              001D: SNS   70 F0 01,1
            //              0020: SBN   7A F0 01,1
            //              0023: MVC   5C 00 00,1 01,1
            //              0027: LA    D2 01 01,1
            //              002A: CLI   7D FC 47,1
            //              002D: BC    E0 01 1D,2
            //              0030: SIO   F3 E2 03
            //              0033: HPL   F0 01 7C         Halt display: "?E"
            //              0036: BC    E0 87 04,2
            //              0039: data  F5 FD FC F0  C6 F0 C2

            byte[] ya5424MFCU = { 0xF3, 0xF0, 0x00,   // SIO  5424 MFCU  Primary    Feed  Stacker 1
                                  0xF3, 0xF1, 0x44,   // SIO  5424 MFCU  Primary    Read  Stacker 4  Read IPL
                                  0xF3, 0xF2, 0x05,   // SIO  5424 MFCU  Primary    Punch Stacker 1
                                  0xF3, 0xF3, 0x06,   // SIO  5424 MFCU  Primary    Read/Punch        Stacker 2
                                  0xF3, 0xF4, 0x07,   // SIO  5424 MFCU  Primary    Print             Stacker 3  Print buffer 1
                                  0xF3, 0xF5, 0x85,   // SIO  5424 MFCU  Primary    Read/Print        Stacker 1  Print buffer 2
                                  0xF3, 0xF6, 0x24,   // SIO  5424 MFCU  Primary    Punch/Print       Stacker 4  Print 4 lines
                                  0xF3, 0xF7, 0x00,   // SIO  5424 MFCU  Primary    Read/Punch/Print  Stacker 1
                                  0xF3, 0xF8, 0x00,   // SIO  5424 MFCU  Secondary  Feed              Stacker 4
                                  0x70, 0xF8, 0x00,   // SNS  5424 MFCU  Invalid  Special indicators for CE use
                                  0x70, 0xF1, 0x00,   // SNS  5424 MFCU  Special indicators for CE use
                                  0x70, 0xF2, 0x00,   // SNS  5424 MFCU  0x02 <invalid>
                                  0x70, 0xF3, 0x00,   // SNS  5424 MFCU  Status indicators
                                  0x70, 0xF4, 0x00,   // SNS  5424 MFCU  MFCU print DAR
                                  0x70, 0xF5, 0x00,   // SNS  5424 MFCU  MFCU read DAR
                                  0x70, 0xF6, 0x00,   // SNS  5424 MFCU  MFCU punch DAR
                                  0x70, 0xF7, 0x00,   // SNS  5424 MFCU  0x07 <invalid>
                                  0x71, 0xFC, 0x00,   // LIO  5424 MFCU  Diagnostic mode  MFCU print DAR
                                  0x71, 0xF5, 0x00,   // LIO  5424 MFCU  Normal mode  MFCU read DAR
                                  0x71, 0xF6, 0x00,   // LIO  5424 MFCU  Normal mode  MFCU punch DAR
                                  0x71, 0xF7, 0x00,   // LIO  5424 MFCU  <invalid>
                                  0xD1, 0xF0, 0x00,   // TIO  5424 MFCU  Primary  Not ready / check
                                  0xD1, 0xF1, 0x00,   // TIO  5424 MFCU  Primary  Read/feed busy
                                  0xD1, 0xF2, 0x00,   // TIO  5424 MFCU  Primary  Punch data busy
                                  0xD1, 0xF3, 0x00,   // TIO  5424 MFCU  Primary  Read/feed or punch busy
                                  0xD1, 0xFC, 0x00,   // TIO  5424 MFCU  Secondary  Print busy
                                  0xD1, 0xFD, 0x00,   // TIO  5424 MFCU  Secondary  Read/feed or print busy
                                  0xD1, 0xFE, 0x00,   // TIO  5424 MFCU  Secondary  Print or punch busy
                                  0xD1, 0xFF, 0x00,   // TIO  5424 MFCU  Secondary  Print, punch, or read/feed busy
                                  0xF1, 0xF8, 0x00,   // APL  5424 MFCU  Secondary  Not ready / check
                                  0xF1, 0xF9, 0x00,   // APL  5424 MFCU  Secondary  Read/feed busy
                                  0xF1, 0xFA, 0x00,   // APL  5424 MFCU  Secondary  Punch data busy
                                  0xF1, 0xFB, 0x00,   // APL  5424 MFCU  Secondary  Read/feed or punch busy
                                  0xF1, 0xF4, 0x00,   // APL  5424 MFCU  Primary  Print busy
                                  0xF1, 0xF5, 0x00,   // APL  5424 MFCU  Primary  Read/feed or print busy
                                  0xF1, 0xF6, 0x00,   // APL  5424 MFCU  Primary  Print or punch busy
                                  0xF1, 0xF7, 0x00 }; // APL  5424 MFCU  Primary  Print, punch, or read/feed busy
                                                      //- Sense IO      M: 0 always; 1 invalid
                                                      //                N: 000 = Special indicators for CE use.
                                                      //                   001 = Special indicators for CE use.
                                                      //                   010 = Invalid.
                                                      //                   011 = Status indicators.
                                                      //                   100 = MFCU print DAR
                                                      //                   101 = MFCU read DAR
                                                      //                   110 = MFCU punch DAR
                                                      //                   111 = Invalid.
                                                      //- Test IO / APL M: 0 = Primary Hopper  1 = Secondary Hopper
                                                      //                N: 000 = Not ready / check
                                                      //                   001 = Read/feed busy
                                                      //                   010 = Punch data busy
                                                      //                   011 = Read/feed or punch busy
                                                      //                   100 = Print busy
                                                      //                   101 = Read/feed or print busy
                                                      //                   110 = Print or punch busy
                                                      //                   111 = Print, punch, or read/feed busy
                                                      //- Load IO	      M: 0 = Normal mode  1 = Diagnostic mode
                                                      //                N: 100 = MFCU print DAR
                                                      //                   101 = MFCU read DAR
                                                      //                   110 = MFCU punch DAR
            byte[] ya5203Printer = { 0xF3, 0xE8, 0x03,   // SIO  5203 Printer  Right carriage  Space only
                                     0xF3, 0xE2, 0x04,   // SIO  5203 Printer  Left/only carriage  Print and space
                                     0xF3, 0xE4, 0x70,   // SIO  5203 Printer  Left/only carriage  Skip only
                                     0xF3, 0xE6, 0x71,   // SIO  5203 Printer  Left/only carriage  Print and skip
                                     0xF3, 0xE7, 0x00,   // SIO  5203 Printer  <invalid>
                                     0x70, 0xE8, 0x00,   // SNS  5203 Printer  <invalid>
                                     0x70, 0xE0, 0x00,   // SNS  5203 Printer  Carriage print line location counter
                                     0x70, 0xE1, 0x00,   // SNS  5203 Printer  5203 Chain Character Counter
                                     0x70, 0xE2, 0x00,   // SNS  5203 Printer  Printer Timing
                                     0x70, 0xE3, 0x00,   // SNS  5203 Printer  Printer Check Status
                                     0x70, 0xE4, 0x00,   // SNS  5203 Printer  Line printer image address register
                                     0x70, 0xE5, 0x00,   // SNS  5203 Printer  <invalid>
                                     0x70, 0xE6, 0x00,   // SNS  5203 Printer  Line printer data address register
                                     0x70, 0x00, 0x00,   // SNS  CPU Console Dials
                                     0x71, 0xE8, 0x00,   // LIO  5203 Printer  <invalid>
                                     0x71, 0xE0, 0x00,   // LIO  5203 Printer  Forms length register
                                     0x71, 0xE4, 0x00,   // LIO  5203 Printer  Line printer image address register
                                     0x71, 0xE6, 0x00,   // LIO  5203 Printer  Line printer data address register
                                     0x71, 0xE7, 0x00,   // LIO  5203 Printer  <invalid>
                                     0xD1, 0xE0, 0x00,   // TIO  5203 Printer  Not ready/check
                                     0xD1, 0xE2, 0x00,   // TIO  5203 Printer  Print buffer busy
                                     0xD1, 0xE4, 0x00,   // TIO  5203 Printer  Carriage busy
                                     0xD1, 0xE6, 0x00,   // TIO  5203 Printer  Printer busy
                                     0xD1, 0xE9, 0x00,   // TIO  5203 Printer  Diagnostic mode
                                     0xD1, 0xEA, 0x00,   // TIO  5203 Printer  <invalid>
                                     0xF1, 0xE0, 0x00,   // APL  5203 Printer  Not ready/check
                                     0xF1, 0xE2, 0x00,   // APL  5203 Printer  Print buffer busy
                                     0xF1, 0xE4, 0x00,   // APL  5203 Printer  Carriage busy
                                     0xF1, 0xE6, 0x00,   // APL  5203 Printer  Printer busy
                                     0xF1, 0xE9, 0x00,   // APL  5203 Printer  Diagnostic mode
                                     0xF1, 0xEA, 0x00 }; // APL  5203 Printer  <invalid>
                                                         //- Start IO      M: 0 = Left/only carriage  1 = Right carriage
                                                         //                N: 000 Space only.
                                                         //                   010 Print and space.
                                                         //                   100 Skip only.
                                                         //                   110 Print and skip.
                                                         //- Sense IO      M: 0 always  1 invalid
                                                         //                N: 000 Carriage print line location counter
                                                         //                   001 5203 Chain Character Counter
                                                         //                   010 Printer Timing
                                                         //                   011 Printer Check Status
                                                         //                   100 Line printer image address register
                                                         //                   110 Line printer data address register
                                                         //- Test IO / APL Q: 0000  Not ready/check
                                                         //                   0010  Print buffer busy
                                                         //                   0100  Carriage busy
                                                         //                   0110  Printer busy
                                                         //                   1001  Diagnostic mode
                                                         //- Load IO       M: 0 always  1 invalid
                                                         //                N: 000 Load forms length register
                                                         //                   100 Line printer image address register
                                                         //                   110 Line printer data address register

            byte[] ya5475Keyboard = { 0xF3, 0x10, 0x80,   // SIO  5475 Keyboard  Programmed Numeric Mode 
                                      0xF3, 0x10, 0x40,   // SIO  5475 Keyboard  Programmed Lower Shift
                                      0xF3, 0x10, 0x20,   // SIO  5475 Keyboard  Error Indicator
                                      0xF3, 0x10, 0x10,   // SIO  5475 Keyboard  Spare
                                      0xF3, 0x10, 0x08,   // SIO  5475 Keyboard  Restore Data Key
                                      0xF3, 0x10, 0x04,   // SIO  5475 Keyboard  Unlock Data Key
                                      0xF3, 0x10, 0x02,   // SIO  5475 Keyboard  Enable/Disable Interrupt
                                      0xF3, 0x10, 0x01,   // SIO  5475 Keyboard  Reset Interrupt
                                      0xF3, 0x10, 0x00,   // SIO  5475 Keyboard  
                                      0xF3, 0x18, 0x00,   // SIO  5475 Keyboard  <invalid Qbyte>
                                      0xF3, 0x14, 0x00,   // SIO  5475 Keyboard  <invalid Qbyte>
                                      0x70, 0x18, 0x00,   // SNS  5475 Keyboard  <invalid Qbyte>
                                      0x70, 0x10, 0x00,   // SNS  5475 Keyboard  <invalid Qbyte>
                                      0x70, 0x11, 0x00,   // SNS  5475 Keyboard  Keystroke byte & 1 sense byte
                                      0x70, 0x12, 0x00,   // SNS  5475 Keyboard  2 sense bytes 
                                      0x70, 0x13, 0x00,   // SNS  5475 Keyboard  Diagnostic sense bytes
                                      0x71, 0x10, 0x00,   // LIO  5475 Keyboard  Set column indicator lamps
                                      0x71, 0x18, 0x00,   // LIO  5475 Keyboard  <invalid Qbyte>
                                      0x71, 0x14, 0x00,   // LIO  5475 Keyboard  <invalid Qbyte>
                                      0xD1, 0x10, 0x00,   // TIO  5475 Keyboard  <invalid Qbyte>
                                      0xF1, 0x10, 0x00 }; // APL  5475 Keyboard  <invalid Qbyte>
                                                          //5475 Keyboard
                                                          //- Start IO      M: 0 always  1 invalid
                                                          //                N: 0 always  1 invalid
                                                          //     Control code: Bit 0 Programmed Numeric Mode
                                                          //                   Bit 1 Programmed Lower Shift
                                                          //                   Bit 2 Error Indicator
                                                          //                   Bit 3 Spare
                                                          //                   Bit 4 Restore Data Key
                                                          //                   Bit 5 Unlock Data Key
                                                          //                   Bit 6 Enable/Disable Interrupt
                                                          //                   Bit 7 Reset Interrupt
                                                          //- Sense IO      M: 0 always  1 invalid
                                                          //                N: 1 Keystroke byte & 1 sense byte
                                                          //                   2 2 sense bytes 
                                                          //                   3 Diagnostic sense bytes
                                                          //- Test IO / APL <invalid QByte>
                                                          //- Load IO       M: 0 always  1 invalid
                                                          //                N: 0 always  1 invalid

            byte[] ya5444Drive1 = { 0xF3, 0xA0, 0x00,   // SIO  5444 Disk Drive 1  Removable disk  Seek
                                    0xF3, 0xA8, 0xFF,   // SIO  5444 Disk Drive 1  Fixed disk  Seek
                                    0xF3, 0xA1, 0x00,   // SIO  5444 Disk Drive 1  Removable disk  Read Data
                                    0xF3, 0xA1, 0x01,   // SIO  5444 Disk Drive 1  Removable disk  Read Identifier
                                    0xF3, 0xA1, 0x02,   // SIO  5444 Disk Drive 1  Removable disk  Read Data Diagnostic
                                    0xF3, 0xA1, 0x03,   // SIO  5444 Disk Drive 1  Removable disk  Verify
                                    0xF3, 0xA2, 0xFC,   // SIO  5444 Disk Drive 1  Write Data
                                    0xF3, 0xA2, 0x01,   // SIO  5444 Disk Drive 1  Write Identifier
                                    0xF3, 0xA2, 0x02,   // SIO  5444 Disk Drive 1  Write Data
                                    0xF3, 0xA2, 0x03,   // SIO  5444 Disk Drive 1  Write Identifier
                                    0xF3, 0xA3, 0xF0,   // SIO  5444 Disk Drive 1  Scan Equal
                                    0xF3, 0xA3, 0xF1,   // SIO  5444 Disk Drive 1  Scan Low or Equal
                                    0xF3, 0xA3, 0xF2,   // SIO  5444 Disk Drive 1  Scan High or Equal
                                    0xF3, 0xA3, 0xF3,   // SIO  5444 Disk Drive 1  Scan High or Equal
                                    0xF3, 0xA4, 0x00,   // SIO  5444 Disk Drive 1  <Invalid Q byte>
                                    0x70, 0xAA, 0x00,   // SNS  5444 Disk Drive 1  Status bytes 0 and 1
                                    0x70, 0xA3, 0x00,   // SNS  5444 Disk Drive 1  Status bytes 2 and 3
                                    0x70, 0xA4, 0x00,   // SNS  5444 Disk Drive 1  Disk read/write address register
                                    0x70, 0xA6, 0x00,   // SNS  5444 Disk Drive 1  Disk control address register
                                    0x70, 0xA7, 0x00,   // SNS  5444 Disk Drive 1  <Invalid Q byte>
                                    0x71, 0xA3, 0x00,   // LIO  5444 Disk Drive 1  CE use
                                    0x71, 0xAC, 0x00,   // LIO  5444 Disk Drive 1  Disk read/write address register
                                    0x71, 0xA6, 0x00,   // LIO  5444 Disk Drive 1  Disk control address register
                                    0x71, 0xAF, 0x00,   // LIO  5444 Disk Drive 1  <Invalid Q byte>
                                    0xD1, 0xA8, 0x00,   // TIO  5444 Disk Drive 1  Not ready/check
                                    0xD1, 0xA2, 0x00,   // TIO  5444 Disk Drive 1  Busy
                                    0xD1, 0xA4, 0x00,   // TIO  5444 Disk Drive 1  Scan found
                                    0xD1, 0xA7, 0x00,   // TIO  5444 Disk Drive 1  <Invalid Q byte>
                                    0xF1, 0xA8, 0x00,   // APL  5444 Disk Drive 1  Not ready/check
                                    0xF1, 0xA2, 0x00,   // APL  5444 Disk Drive 1  Busy
                                    0xF1, 0xA4, 0x00,   // APL  5444 Disk Drive 1  Scan found
                                    0xF1, 0xA7, 0x00 }; // APL  5444 Disk Drive 1  <Invalid Q byte>

            byte[] ya5444Drive2 = { 0xF3, 0xB0, 0x00,   // SIO  5444 Disk Drive 2  Removable disk  Seek
                                    0xF3, 0xB8, 0xFF,   // SIO  5444 Disk Drive 2  Fixed disk  Seek
                                    0xF3, 0xB1, 0x00,   // SIO  5444 Disk Drive 2  Removable disk  Read Data
                                    0xF3, 0xB1, 0x01,   // SIO  5444 Disk Drive 2  Removable disk  Read Identifier
                                    0xF3, 0xB1, 0x02,   // SIO  5444 Disk Drive 2  Removable disk  Read Data Diagnostic
                                    0xF3, 0xB1, 0x03,   // SIO  5444 Disk Drive 2  Removable disk  Verify
                                    0xF3, 0xB2, 0xFC,   // SIO  5444 Disk Drive 2  Write Data
                                    0xF3, 0xB2, 0x01,   // SIO  5444 Disk Drive 2  Write Identifier
                                    0xF3, 0xB2, 0x02,   // SIO  5444 Disk Drive 2  Write Data
                                    0xF3, 0xB2, 0x03,   // SIO  5444 Disk Drive 2  Write Identifier
                                    0xF3, 0xB3, 0xF0,   // SIO  5444 Disk Drive 2  Scan Equal
                                    0xF3, 0xB3, 0xF1,   // SIO  5444 Disk Drive 2  Scan Low or Equal
                                    0xF3, 0xB3, 0xF2,   // SIO  5444 Disk Drive 2  Scan High or Equal
                                    0xF3, 0xB3, 0xF3,   // SIO  5444 Disk Drive 2  Scan High or Equal
                                    0xF3, 0xB4, 0x00,   // SIO  5444 Disk Drive 2  <Invalid Q byte>
                                    0x70, 0xBA, 0x00,   // SNS  5444 Disk Drive 2  Status bytes 0 and 1
                                    0x70, 0xB3, 0x00,   // SNS  5444 Disk Drive 2  Status bytes 2 and 3
                                    0x70, 0xB4, 0x00,   // SNS  5444 Disk Drive 2  Disk read/write address register
                                    0x70, 0xB6, 0x00,   // SNS  5444 Disk Drive 2  Disk control address register
                                    0x70, 0xB7, 0x00,   // SNS  5444 Disk Drive 2  <Invalid Q byte>
                                    0x71, 0xB3, 0x00,   // LIO  5444 Disk Drive 2  CE use
                                    0x71, 0xBC, 0x00,   // LIO  5444 Disk Drive 2  Disk read/write address register
                                    0x71, 0xB6, 0x00,   // LIO  5444 Disk Drive 2  Disk control address register
                                    0x71, 0xBF, 0x00,   // LIO  5444 Disk Drive 2  <Invalid Q byte>
                                    0xD1, 0xB8, 0x00,   // TIO  5444 Disk Drive 2  Not ready/check
                                    0xD1, 0xB2, 0x00,   // TIO  5444 Disk Drive 2  Busy
                                    0xD1, 0xB4, 0x00,   // TIO  5444 Disk Drive 2  Scan found
                                    0xD1, 0xB7, 0x00,   // TIO  5444 Disk Drive 2  <Invalid Q byte>
                                    0xF1, 0xB8, 0x00,   // APL  5444 Disk Drive 2  Not ready/check
                                    0xF1, 0xB2, 0x00,   // APL  5444 Disk Drive 2  Busy
                                    0xF1, 0xB4, 0x00,   // APL  5444 Disk Drive 2  Scan found
                                    0xF1, 0xB7, 0x00 }; // APL  5444 Disk Drive 2  <Invalid Q byte>

                                                          //Disk Drives
                                                          //- Start IO      M: 0 Removable disk  1 Fixed disk
                                                          //                N: 000 CC 0x03: -- Seek
                                                          //                   001          00 Read Data
                                                          //                   001          01 Read Identifier
                                                          //                   001          10 Read Data Diagnostic
                                                          //                   001          11 Verify
                                                          //                   010          -0 Write Data
                                                          //                   010          -1 Write Identifier
                                                          //                   011          00 Scan Equal
                                                          //                   011          01 Scan Low or Equal
                                                          //                   011          1- Scan High or Equal
                                                          //- Sense IO      M: ignored
                                                          //                N: 010  Status bytes 0 and 1
                                                          //                   011  Status bytes 2 and 3
                                                          //                   100  Disk read/write address register
                                                          //                   110  Disk control address register
                                                          //- Test IO / APL M: ignored
                                                          //                N: 000  Not ready/check
                                                          //                   010  Busy
                                                          //                   100  Scan found
                                                          //- Load IO       M: ignored 
                                                          //                N: 011  CE use
                                                          //                   100  Disk read/write address register
                                                          //                   110  Disk control address register

            byte[] yaDeviceNames = { 0xF3, 0x30, 0x00,   // Serial Input/Output Channel Adapter
                                     0xF3, 0x80, 0x00,   // BSCA
                                     0xF3, 0x60, 0x00,   // Tape Drive 1
                                     0xF3, 0x68, 0x00,   // Tape Drive 2
                                     0xF3, 0x70, 0x00,   // Tape Drive 3
                                     0xF3, 0x78, 0x00,   // Tape Drive 4
                                     0xF3, 0x50, 0x00,   // 1442 Card Read Punch
                                     0xF3, 0x40, 0x00 }; // 3741 Data/Work Station

            byte[] yaCPUGroup = { 0x04, 0x44, 0x00, 0x00, 0x00, 0x00,   // ZAZ
                                  0x06, 0x1F, 0x00, 0x00, 0x00, 0x00,   // AZ	
                                  0x07, 0xF1, 0x00, 0x00, 0x00, 0x00,   // SZ	
                                  0x0A, 0x23, 0x00, 0x00, 0x00, 0x00,   // ED	
                                  0x0B, 0x6C, 0x00, 0x00, 0x00, 0x00,   // ITC	
                                  0x0C, 0x50, 0x00, 0x00, 0x00, 0x00,   // MVC	
                                  0x08, 0x00, 0x00, 0x00, 0x00, 0x00,   // MVX	
                                  0x08, 0x01, 0x00, 0x00, 0x00, 0x00,   // MVX	
                                  0x08, 0x02, 0x00, 0x00, 0x00, 0x00,   // MVX	
                                  0x08, 0x03, 0x00, 0x00, 0x00, 0x00,   // MVX	
                                  0x0D, 0xFE, 0x00, 0x00, 0x00, 0x00,   // CLC	
                                  0x0E, 0xD4, 0x00, 0x00, 0x00, 0x00,   // ALC	
                                  0x0F, 0x20, 0x00, 0x00, 0x00, 0x00,   // SLC	
	                              0xC2, 0x00, 0x04, 0x75,               // LA  (no register selected)
	                              0xC2, 0x01, 0x05, 0x63,               // LA  XR1
	                              0xC2, 0x02, 0x06, 0x52,               // LA  XR2
	                              0xC2, 0x03, 0x07, 0x41,               // LA  (loading both XR1 and XR2)
	                              0xD2, 0x00, 0x41,                     // LA  (no register selected)
	                              0xD2, 0x01, 0x42,                     // LA  XR1
	                              0xD2, 0x02, 0x53,                     // LA  XR2
	                              0xD2, 0x03, 0x64,                     // LA  (loading both XR1 and XR2)
	                              0xE2, 0x00, 0x75,                     // LA  (no register selected)
	                              0xE2, 0x01, 0xA9,                     // LA  XR1
	                              0xE2, 0x02, 0xB8,                     // LA  XR2
	                              0xE2, 0x03, 0xC7,                     // LA  (loading both XR1 and XR2)
                                  0x34, 0x80, 0x00, 0x00,               // ST  Interrupt level 0
                                  0x34, 0xC0, 0x00, 0x00,               // ST  Interrupt level 1
                                  0x34, 0xA0, 0x00, 0x00,               // ST  Interrupt level 2
                                  0x34, 0x90, 0x00, 0x00,               // ST  Interrupt level 3
                                  0x34, 0x88, 0x00, 0x00,               // ST  Interrupt level 4
                                  0x34, 0x40, 0x00, 0x00,               // ST  PL2 IAR
                                  0x34, 0x20, 0x00, 0x00,               // ST  PL1 IAR
                                  0x34, 0x10, 0x00, 0x00,               // ST  Current IAR
                                  0x34, 0x08, 0x00, 0x00,               // ST  ARR
                                  0x34, 0x04, 0x00, 0x00,               // ST  PSR
                                  0x34, 0x02, 0x00, 0x00,               // ST  XR2
                                  0x34, 0x01, 0x00, 0x00,               // ST  XR1
                                  0x35, 0x80, 0x00, 0x00,               // L   Interrupt level 0
                                  0x35, 0xC0, 0x00, 0x00,               // L   Interrupt level 1
                                  0x35, 0xA0, 0x00, 0x00,               // L   Interrupt level 2
                                  0x35, 0x90, 0x00, 0x00,               // L   Interrupt level 3
                                  0x35, 0x88, 0x00, 0x00,               // L   Interrupt level 4
                                  0x35, 0x40, 0x00, 0x00,               // L   PL2 IAR
                                  0x35, 0x20, 0x00, 0x00,               // L   PL1 IAR
                                  0x35, 0x10, 0x00, 0x00,               // L   Current IAR
                                  0x35, 0x08, 0x00, 0x00,               // L   ARR
                                  0x35, 0x04, 0x00, 0x00,               // L   PSR
                                  0x35, 0x02, 0x00, 0x00,               // L   XR2
                                  0x35, 0x01, 0x00, 0x00,               // L   XR1
                                  0x36, 0x80, 0x00, 0x00,               // A   Interrupt level 0
                                  0x36, 0xC0, 0x00, 0x00,               // A   Interrupt level 1
                                  0x36, 0xA0, 0x00, 0x00,               // A   Interrupt level 2
                                  0x36, 0x90, 0x00, 0x00,               // A   Interrupt level 3
                                  0x36, 0x88, 0x00, 0x00,               // A   Interrupt level 4
                                  0x36, 0x40, 0x00, 0x00,               // A   PL2 IAR
                                  0x36, 0x20, 0x00, 0x00,               // A   PL1 IAR
                                  0x36, 0x10, 0x00, 0x00,               // A   Current IAR
                                  0x36, 0x08, 0x00, 0x00,               // A   ARR
                                  0x36, 0x04, 0x00, 0x00,               // A   PSR
                                  0x36, 0x02, 0x00, 0x00,               // A   XR2
                                  0x36, 0x01, 0x00, 0x00,               // A   XR1
                                  0x3C, 0xDD, 0x00, 0x00,               // MVI
                                  0x3C, 0x52, 0x00, 0x00,               // MVI
                                  0x3D, 0x03, 0x00, 0x00,               // CLI
                                  0x3D, 0x49, 0x00, 0x00,               // CLI
                                  0xF0, 0x7C, 0x7C };                   // HPL

                
            byte[] yaCPUInstructions = { 0x04, 0x00, 0x11, 0x22, 0x33, 0x44,     // ZAZ
                                         0x14, 0x00, 0x11, 0x22, 0x33,           // ZAZ
                                         0x24, 0x00, 0x11, 0x22, 0x33,           // ZAZ
                                         0x44, 0x00, 0x11, 0x22, 0x33,           // ZAZ
                                         0x54, 0x00, 0x11, 0x22,                 // ZAZ
                                         0x64, 0x00, 0x11, 0x22,                 // ZAZ
                                         0x84, 0x00, 0x11, 0x22, 0x33,           // ZAZ
                                         0x94, 0x00, 0x11, 0x22,                 // ZAZ
                                         0xA4, 0x00, 0x11, 0x22,                 // ZAZ
                                         0x06, 0x00, 0x11, 0x22, 0x33, 0x44,     // AZ
                                         0x16, 0x00, 0x11, 0x22, 0x33,           // AZ
                                         0x26, 0x00, 0x11, 0x22, 0x33,           // AZ
                                         0x46, 0x00, 0x11, 0x22, 0x33,           // AZ
                                         0x56, 0x00, 0x11, 0x22,                 // AZ
                                         0x66, 0x00, 0x11, 0x22,                 // AZ
                                         0x86, 0x00, 0x11, 0x22, 0x33,           // AZ
                                         0x96, 0x00, 0x11, 0x22,                 // AZ
                                         0xA6, 0x00, 0x11, 0x22,                 // AZ
                                         0x07, 0x00, 0x11, 0x22, 0x33, 0x44,     // SZ
                                         0x17, 0x00, 0x11, 0x22, 0x33,           // SZ
                                         0x27, 0x00, 0x11, 0x22, 0x33,           // SZ
                                         0x47, 0x00, 0x11, 0x22, 0x33,           // SZ
                                         0x57, 0x00, 0x11, 0x22,                 // SZ
                                         0x67, 0x00, 0x11, 0x22,                 // SZ
                                         0x87, 0x00, 0x11, 0x22, 0x33,           // SZ
                                         0x97, 0x00, 0x11, 0x22,                 // SZ
                                         0xA7, 0x00, 0x11, 0x22,                 // SZ
                                         0x08, 0x00, 0x11, 0x22, 0x33, 0x44,     // MVX
                                         0x18, 0x00, 0x11, 0x22, 0x33,           // MVX
                                         0x28, 0x00, 0x11, 0x22, 0x33,           // MVX
                                         0x48, 0x00, 0x11, 0x22, 0x33,           // MVX
                                         0x58, 0x00, 0x11, 0x22,                 // MVX
                                         0x68, 0x00, 0x11, 0x22,                 // MVX
                                         0x88, 0x00, 0x11, 0x22, 0x33,           // MVX
                                         0x98, 0x00, 0x11, 0x22,                 // MVX
                                         0xA8, 0x00, 0x11, 0x22,                 // MVX
                                         0x0A, 0x00, 0x11, 0x22, 0x33, 0x44,     // ED
                                         0x1A, 0x00, 0x11, 0x22, 0x33,           // ED
                                         0x2A, 0x00, 0x11, 0x22, 0x33,           // ED
                                         0x4A, 0x00, 0x11, 0x22, 0x33,           // ED
                                         0x5A, 0x00, 0x11, 0x22,                 // ED
                                         0x6A, 0x00, 0x11, 0x22,                 // ED
                                         0x8A, 0x00, 0x11, 0x22, 0x33,           // ED
                                         0x9A, 0x00, 0x11, 0x22,                 // ED
                                         0xAA, 0x00, 0x11, 0x22,                 // ED
                                         0x0B, 0x00, 0x11, 0x22, 0x33, 0x44,     // ITC
                                         0x1B, 0x00, 0x11, 0x22, 0x33,           // ITC
                                         0x2B, 0x00, 0x11, 0x22, 0x33,           // ITC
                                         0x4B, 0x00, 0x11, 0x22, 0x33,           // ITC
                                         0x5B, 0x00, 0x11, 0x22,                 // ITC
                                         0x6B, 0x00, 0x11, 0x22,                 // ITC
                                         0x8B, 0x00, 0x11, 0x22, 0x33,           // ITC
                                         0x9B, 0x00, 0x11, 0x22,                 // ITC
                                         0xAB, 0x00, 0x11, 0x22,                 // ITC
                                         0x0C, 0x00, 0x11, 0x22, 0x33, 0x44,     // MVC
                                         0x1C, 0x00, 0x11, 0x22, 0x33,           // MVC
                                         0x2C, 0x00, 0x11, 0x22, 0x33,           // MVC
                                         0x4C, 0x00, 0x11, 0x22, 0x33,           // MVC
                                         0x5C, 0x00, 0x11, 0x22,                 // MVC
                                         0x6C, 0x00, 0x11, 0x22,                 // MVC
                                         0x8C, 0x00, 0x11, 0x22, 0x33,           // MVC
                                         0x9C, 0x00, 0x11, 0x22,                 // MVC
                                         0xAC, 0x00, 0x11, 0x22,                 // MVC
                                         0x0D, 0x00, 0x11, 0x22, 0x33, 0x44,     // CLC
                                         0x1D, 0x00, 0x11, 0x22, 0x33,           // CLC
                                         0x2D, 0x00, 0x11, 0x22, 0x33,           // CLC
                                         0x4D, 0x00, 0x11, 0x22, 0x33,           // CLC
                                         0x5D, 0x00, 0x11, 0x22,                 // CLC
                                         0x6D, 0x00, 0x11, 0x22,                 // CLC
                                         0x8D, 0x00, 0x11, 0x22, 0x33,           // CLC
                                         0x9D, 0x00, 0x11, 0x22,                 // CLC
                                         0xAD, 0x00, 0x11, 0x22,                 // CLC
                                         0x0E, 0x00, 0x11, 0x22, 0x33, 0x44,     // ALC
                                         0x1E, 0x00, 0x11, 0x22, 0x33,           // ALC
                                         0x2E, 0x00, 0x11, 0x22, 0x33,           // ALC
                                         0x4E, 0x00, 0x11, 0x22, 0x33,           // ALC
                                         0x5E, 0x00, 0x11, 0x22,                 // ALC
                                         0x6E, 0x00, 0x11, 0x22,                 // ALC
                                         0x8E, 0x00, 0x11, 0x22, 0x33,           // ALC
                                         0x9E, 0x00, 0x11, 0x22,                 // ALC
                                         0xAE, 0x00, 0x11, 0x22,                 // ALC
                                         0x0F, 0x00, 0x11, 0x22, 0x33, 0x44,     // SLC
                                         0x1F, 0x00, 0x11, 0x22, 0x33,           // SLC
                                         0x2F, 0x00, 0x11, 0x22, 0x33,           // SLC
                                         0x4F, 0x00, 0x11, 0x22, 0x33,           // SLC
                                         0x5F, 0x00, 0x11, 0x22,                 // SLC
                                         0x6F, 0x00, 0x11, 0x22,                 // SLC
                                         0x8F, 0x00, 0x11, 0x22, 0x33,           // SLC
                                         0x9F, 0x00, 0x11, 0x22,                 // SLC
                                         0xAF, 0x00, 0x11, 0x22,                 // SLC
                                         0x30, 0x00, 0x11, 0x22,                 // SNS
                                         0x70, 0x00, 0x11,                       // SNS
                                         0xB0, 0x00, 0x11,                       // SNS
                                         0x31, 0x00, 0x11, 0x22,                 // LIO
                                         0x71, 0x00, 0x11,                       // LIO
                                         0xB1, 0x00, 0x11,                       // LIO
                                         0x34, 0x00, 0x11, 0x22,                 // ST
                                         0x74, 0x00, 0x11,                       // ST
                                         0xB4, 0x00, 0x11,                       // ST
                                         0x35, 0x00, 0x11, 0x22,                 // L
                                         0x75, 0x00, 0x11,                       // L
                                         0xB5, 0x00, 0x11,                       // L
                                         0x36, 0x00, 0x11, 0x22,                 // A
                                         0x76, 0x00, 0x11,                       // A
                                         0xB6, 0x00, 0x11,                       // A
                                         0x38, 0x00, 0x11, 0x22,                 // TBN
                                         0x78, 0x00, 0x11,                       // TBN
                                         0xB8, 0x00, 0x11,                       // TBN
                                         0x39, 0x00, 0x11, 0x22,                 // TBF
                                         0x79, 0x00, 0x11,                       // TBF
                                         0xB9, 0x00, 0x11,                       // TBF
                                         0x3A, 0x00, 0x11, 0x22,                 // SBN
                                         0x7A, 0x00, 0x11,                       // SBN
                                         0xBA, 0x00, 0x11,                       // SBN
                                         0x3B, 0x00, 0x11, 0x22,                 // SBF
                                         0x7B, 0x00, 0x11,                       // SBF
                                         0xBB, 0x00, 0x11,                       // SBF
                                         0x3C, 0x00, 0x11, 0x22,                 // MVI
                                         0x7C, 0x00, 0x11,                       // MVI
                                         0xBC, 0x00, 0x11,                       // MVI
                                         0x3D, 0x00, 0x11, 0x22,                 // CLI
                                         0x7D, 0x00, 0x11,                       // CLI
                                         0xBD, 0x00, 0x11,                       // CLI
                                         0xC0, 0x00, 0x11, 0x22,                 // BC
                                         0xD0, 0x00, 0x11,                       // BC
                                         0xE0, 0x00, 0x11,                       // BC
                                         0xC1, 0x00, 0x11, 0x22,                 // TIO
                                         0xD1, 0x00, 0x11,                       // TIO
                                         0xE1, 0x00, 0x11,                       // TIO
                                         0xC2, 0x00, 0x11, 0x22,                 // LA
                                         0xD2, 0x00, 0x11,                       // LA
                                         0xE2, 0x00, 0x11, };                    // LA

            byte[] yaCPUBranching1 = { 0xF1, 0x00, 0x00,         // 0000 APL
                                       0xF2, 0x87, 0x03,         // 0003 JC 
                                       0xF0, 0x7C, 0x7C,         // 0006 HPL
                                       0xC0, 0x87, 0x00, 0x00 }; // 0009 BC 

            byte[] yaCPUBranching2 = { 0xB6, 0x01, 0xF3,         // 0000: A   
                                       0xAF, 0x01, 0xED, 0xF3,   // 0003: SLC 
                                       0xE0, 0x01, 0x9F,         // 0007: BNE 
                                       0xB8, 0x02, 0xD4,         // 000A: TBN 
                                       0xE0, 0x10, 0x51,         // 000D: BT  
                                       0xE1, 0xF0, 0x04,         // 0010: TIO 
                                       0xB1, 0xF5, 0xDF,         // 0013: LIO 
                                       0xF3, 0xF1, 0x45,         // 0016: SIO 
                                       0xE1, 0xF1, 0xCD,         // 0019: TIO 
                                       0xC0, 0x87, 0x00, 0x03 }; // 001C: B   

            byte[] yaCPUAddressing = { 0x0C, 0x03, 0x01, 0x04, 0x01, 0x07,
                                       0x0C, 0x03, 0x01, 0x04, 0x01, 0x08,
                                       0x0C, 0x03, 0x01, 0x07, 0x01, 0x04,
                                       0x0C, 0x03, 0x01, 0x08, 0x01, 0x04,
                                       0x07, 0x13, 0x01, 0x05, 0x01, 0x07,
                                       0x07, 0x13, 0x01, 0x05, 0x01, 0x17,
                                       0x07, 0x13, 0x01, 0x07, 0x01, 0x05,
                                       0x07, 0x13, 0x01, 0x17, 0x01, 0x05 };
           
            objSystem3Tools.AutoTagJump = true;
            objSystem3Tools.AutoTagLoop = true;
            objSystem3Tools.ExtendMnemonicBC = true;
            objSystem3Tools.ExtendMnemonicJC = true;
            objSystem3Tools.ExtendMnemonicMVX = true;

            objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleCode (yaCPUAddressing));

            objSystem3Tools.AutoTagJump = false;
            objSystem3Tools.AutoTagLoop = false;
            objSystem3Tools.ExtendMnemonicBC = false;
            objSystem3Tools.ExtendMnemonicJC = false;
            objSystem3Tools.ExtendMnemonicMVX = false;
        }

        // Earliest test harness using internal data
        static void EarliestTesting ()
        {
            CDataConversion objSystem3Tools = new CDataConversion ();

            objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                (@"D:\SoftwareDev\IBMSystem3\Data Files\Test Data\TextCards.txt", false, false, false));
            objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                (@"D:\SoftwareDev\IBMSystem3\Data Files\Test Data\TextCards.txt", true, false, false));
            objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                (@"D:\SoftwareDev\IBMSystem3\Data Files\Test Data\TextCards.txt", false, true, false));
            objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextFileLines
                (@"D:\SoftwareDev\IBMSystem3\Data Files\Test Data\TextCards.txt", true, true, false));

            // Test DumpTextLine ()
            //                             1         2         3         4         5         6         7         8         9         0
            //                    1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
            string strTestText = "T<AA><BB><CC><DD><EE><FF><GG><HH><II><JJ><KK><LL><MM><NN><OO><PP><QQ><RR><SS><TT><UU><V>TEST9999";
            objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextLine (strTestText, false, false, false));
            objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextLine (strTestText, true, false, false));
            objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextLine (strTestText, false, true, false));
            objSystem3Tools.PrintStringList (objSystem3Tools.DumpTextLine (strTestText, true, true, false));

            objSystem3Tools.PrintStringList (objSystem3Tools.ReadFileToStringList
                (@"C:\Documents and Settings\ge97690\My Documents\My Stuff\IBM System 3\ASCII.txt", 96));  // Bad filename
            objSystem3Tools.PrintStringList (objSystem3Tools.ReadFileToStringList
                (@"D:\SoftwareDev\IBMSystem3\Data Files\Test Data\ASCII.txt", 96));  // Valid filename

            #region Test Data
            byte[] yaObjectCode1 = { 0x04, 0x01, 0x10, 0x21, 0x30, 0x41, // ZAZ -6
                                     0x16, 0x02, 0x11, 0x22, 0x31,       // AZ  -5
                                     0x27, 0x03, 0x12, 0x23, 0x32,       // SZ  -5
                                     0x48, 0x04, 0x13, 0x24, 0x33,       // MVX -5
                                     0x5A, 0x05, 0x14, 0x25,             // ED  -4
                                     0x6B, 0x06, 0x15, 0x26,             // ITC -4
                                     0x8C, 0x07, 0x16, 0x27, 0x34,       // MVC -5
                                     0x9D, 0x07, 0x17, 0x28,             // CLC -4
                                     0xAE, 0x08, 0x18, 0x29,             // ALC -4
                                     0x30, 0x0A, 0x19, 0x2A,             // SNS -4
                                     0xC0, 0x87, 0x19, 0x2A,             // BC  -4
                                     0xD0, 0x87, 0x2A,                   // BC  -3
                                     0x31, 0x0B, 0x1A, 0x2A,             // LIO -4
                                     0x71, 0x0B, 0x1A,                   // LIO -3
                                     0xB4, 0x0C, 0x1B,                   // ST  -3
                                     0xF2, 0x87, 0x1C };                 // JC  -3
            byte[] yaObjectCode2 = { 0xC2, 0x02, 0x00, 0x00, 0xB5, 0x01, 0x35, 0xB1,
                                     0xE4, 0x02, 0xB1, 0xE6, 0x35, 0x6C, 0x02, 0xAA,
                                     0x3B, 0xF3, 0xF0, 0x00, 0x70, 0xF0, 0x01, 0x78,
                                     0x04, 0x01, 0xE0, 0x90, 0x14, 0x70, 0xF0, 0x01,
                                     0x7A, 0xF0, 0x01, 0x5C, 0x00, 0x00, 0x01, 0xD2,
                                     0x01, 0x01, 0x7D, 0xFC, 0x47, 0xE0, 0x01, 0x1D,
                                     0xF3, 0xE2, 0x03, 0xF0, 0x01, 0x7C, 0xE0, 0x87,
                                     0x04, 0xF5, 0xFD, 0xFC, 0xF0, 0xC6, 0xF0, 0xC2, 0xFF };
            #endregion

            // Test Text card reader
            Console.WriteLine ("Text Card Test");
            byte[] yaTextBinary = objSystem3Tools.ReadTextCard ("T+0<#, A\"|50 B-+@ C'^  .4P0 CZ' A ~0 / ;#0HK> &*G,-DGA:0 -0!Y YHE,-DEA!8AA&O> H<E>@BC   ");
            objSystem3Tools.PrintStringList (objSystem3Tools.BinaryToDump (yaTextBinary));
            // 0000: 3B 03 3B AC  00 7F 3F 5C   00 0A 03 BC  00 3F 5F 00
            // 0010: 02 F4 5F 00  03 A7 D0 01   00 AC 00 84  07 BB C0 84
            // 0020: AE 01 07 07  AE 01 07 07   AC 00 83 06  A8 02 82 05
            // 0030: AE 01 05 05  AE 01 05 05   AE 00 83 05  BB C0 83 00
            // 0040: 00
            // Test IPL card reader
            Console.WriteLine ("IPL Card Test 1");
            byte[] yaIplBinary = objSystem3Tools.ReadIPLCard (" 123456789:#@'=\"0/STUVWXYZ&,%_>?-JKLMNOPQR!$*);^}ABCDEFGHI~.<(+|                                ");
            objSystem3Tools.PrintStringList (objSystem3Tools.BinaryToDump (yaIplBinary));

            yaIplBinary = objSystem3Tools.ReadIPLCard (" 123456789:#@'=\"0/STUVWXYZ&,%_>?-JKLMNOPQR!$*);^}ABCDEFGHI~.<(+|55555555555555555555555555555555");
            objSystem3Tools.PrintStringList (objSystem3Tools.BinaryToDump (yaIplBinary));

            yaIplBinary = objSystem3Tools.ReadIPLCard (" 123456789:#@'=\"0/STUVWXYZ&,%_>?-JKLMNOPQR!$*);^}ABCDEFGHI~.<(+|::::::::::::::::::::::::::::::::");
            objSystem3Tools.PrintStringList (objSystem3Tools.BinaryToDump (yaIplBinary));

            yaIplBinary = objSystem3Tools.ReadIPLCard
                (" 123456789:#@'=\"0/STUVWXYZ&,%_>?-JKLMNOPQR!$*);^}ABCDEFGHI~.<(+|\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"\"");
            objSystem3Tools.PrintStringList (objSystem3Tools.BinaryToDump (yaIplBinary));

            Console.WriteLine ("IPL Card Test 2");
            yaIplBinary = objSystem3Tools.ReadIPLCard
                ("BCA 4A\"6A : A D%\"\"\"3-A%C\"\"1UC1W83S JST% #\"%C\"=8| }|L}GO @\"\" 0E0A @'48\"#'@679=6''3 938'358::1@8 @");
            objSystem3Tools.PrintStringList (objSystem3Tools.BinaryToDump (yaIplBinary));
            // 0000: C2 03 01 00  74 01 FF 36   01 00 3A C0  01 00 04 AC
            //       C2 03 01 00  74 01 FF 36   01 00 3A C0  01 00 04 AC
            // 0010: 7F 7F FF F3  E0 01 6C 83   FF FF 71 E4  03 71 E6 38
            //       7F 7F FF F3  E0 01 6C 83   FF FF 71 E4  03 71 E6 38
            // 0020: F3 E2 00 D1  E2 23 AC 00   7B FF AC 83  FF FE B8 0F
            //       F3 E2 00 D1  E2 23 AC 00   7B FF AC 83  FF FE B8 0F
            // 0030: 80 D0 0F 13  D0 87 16 00   7C FF FF 00  F0 C5 F0 C1
            //       80 D0 0F 13  D0 87 16 00   7C FF FF 00  F0 C5 F0 C1

            yaIplBinary = objSystem3Tools.ReadIPLCard ("0@C2  2  2 N0@6                  0@$15 N31E2 ,0@G           0501/8=174174 6@198270@)4@0@'0@^2G");
            objSystem3Tools.PrintStringList (objSystem3Tools.BinaryToDump (yaIplBinary));
            // 0000: F0 FC 03 F2  00 00 F2 00   00 F2 00 15  F0 FC 76 40
            //       F0 FC 03 F2  00 00 F2 00   00 F2 00 15  F0 FC 76 40
            // 0010: 00 40 80 80  00 80 40 80   80 40 80 80  40 00 40 40
            //       00 40 80 80  00 80 40 80   80 40 80 80  40 00 40 40
            // 0020: 00 F0 FC 1B  31 F5 00 15   F3 F1 45 F2  00 2B F0 FC
            //       00 F0 FC 1B  31 F5 00 15   F3 F1 45 F2  00 2B F0 FC
            // 0030: 07 40 40 00  40 40 40 40   00 40 40 80  70 35 F0 F1
            //       07 40 40 00  40 40 40 40   00 40 40 80  70 35 F0 F1
            // 0040: 61 F8 7E F1  F7 F4 F1 F7   F4 40 F6 7C  F1 F9 F8 F2
            //       61 F8 7E F1  F7 F4 F1 F7   F4 40 F6 7C  F1 F9 F8 F2
            // 0050: F7 F0 7C 5D  F4 7C F0 7C   7D F0 7C 5F  F2 C7 40 40
            //       F7 F0 7C 5D  F4 7C F0 7C   7D F0 7C 5F  F2 C7 40 40

            yaIplBinary = objSystem3Tools.ReadIPLCard ("96 COLUMN CARD WITH 6 BIT DATA  ABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789+_!:;=&<>}||~%?'\"()@#*$-,./");
            objSystem3Tools.PrintStringList (objSystem3Tools.BinaryToDump (yaIplBinary));
            // 0000: B9 B6 00 43  56 13 24 54   55 80 03 41  19 04 40 26
            //       B9 B6 00 43  56 13 24 54   55 80 03 41  19 04 40 26
            // 0010: 09 63 08 80  36 80 02 09   23 C0 04 41  E3 41 C0 40
            //       09 63 08 80  36 80 02 09   23 C0 04 41  E3 41 C0 40
            // 0020: 81 42 03 C4  85 46 87 48   49 51 52 53  D4 55 D6 17
            //       81 42 03 C4  85 46 87 48   49 51 52 53  D4 55 D6 17
            // 0030: 18 59 E2 23  A4 25 A6 A7   E8 29 40 30  F1 32 33 B4
            //       18 59 E2 23  A4 25 A6 A7   E8 29 40 30  F1 32 33 B4
            // 0040: F5 F6 F7 F8  F9 4E 6D 5A   7A 5E 7E 50  4C 6E D0 4F
            //       F5 F6 F7 F8  F9 4E 6D 5A   7A 5E 7E 50  4C 6E D0 4F
            // 0050: 4F 4A 6C 6F  7D 7F 4D 5D   7C 7B 5C 5B  60 6B 4B 61
            //       4F 4A 6C 6F  7D 7F 4D 5D   7C 7B 5C 5B  60 6B 4B 61

            yaIplBinary = objSystem3Tools.ReadIPLCard ("ABCDEFGHIJKLMNOPQRSTUVWXYZ      }^.<(+|!$*);~-/&,%->?:#@'=\"     01234567899");
            objSystem3Tools.PrintStringList (objSystem3Tools.BinaryToDump (yaIplBinary));
            // 0000: C1 C2 C3 C4  85 86 87 88   49 51 52 D3  D4 D5 D6 D7
            //       C1 C2 C3 C4  85 86 87 88   49 51 52 D3  D4 D5 D6 D7
            // 0010: D8 D9 E2 E3  E4 E5 E6 E7   E8 E9 40 40  40 40 40 40
            //       D8 D9 E2 E3  E4 E5 E6 E7   E8 E9 40 40  40 40 40 40
            // 0020: D0 1F CB 8C  4D 0E CF 9A   5B 1C 1D 5E  4A 60 61 50
            //       D0 1F CB 8C  4D 0E CF 9A   5B 1C 1D 5E  4A 60 61 50
            // 0030: 6B 6C 60 6E  6F 7A 7B 7C   7D 7E 7F 40  40 40 40 40
            //       6B 6C 60 6E  6F 7A 7B 7C   7D 7E 7F 40  40 40 40 40
            // 0040: F0 F1 F2 F3  F4 F5 F6 F7   F8 F9 F9 40  40 40 40 40
            //       F0 F1 F2 F3  F4 F5 F6 F7   F8 F9 F9 40  40 40 40 40
            // 0050: 40 40 40 40  40 40 40 40   40 40 40 40  40 40 40 40
            //       40 40 40 40  40 40 40 40   40 40 40 40  40 40 40 40

            // Test CHexAndCharacterDump class
            //string str = objHexAndCharacterDump.ByteToHexPaid (0x12);
            //str = objHexAndCharacterDump.ByteToHexPaid (0xBC);
            objSystem3Tools.PrintStringList (objSystem3Tools.BinaryToDump (yaObjectCode2));

            // Test CDataConversion class
            byte[] yaBinary = objSystem3Tools.PackHexToBinary ("C2020000B50135B1E402B1E6356C02AA" +
                                                               "3BF3F00070F001780401E0901470F001" +
                                                               "7AF0015C000001D201017DFC47E0011D" +
                                                               "F3E203F0017CE08704F5FDFCF0C6F0C2");
            objSystem3Tools.PrintStringList (objSystem3Tools.BinaryToDump (yaBinary));

            //for (int iIdx = 0; iIdx < 0x0100; ++iIdx)
            //{
            //    byte yAsciiValue = (byte)iIdx;
            //    byte yEbcdicValue = objSystem3Tools.ConvertASCIItoEBCDIC (yAsciiValue);
            //    byte yReturnValue = objSystem3Tools.ConvertEBCDICtoASCII (yEbcdicValue);
            //    Console.WriteLine ("{0:X2} {1:X2} {2:X2} {3:S}", yAsciiValue, yEbcdicValue, yReturnValue,
            //                       (yAsciiValue == yReturnValue) ? "match" : "mismatch");
            //}

            // Test CDisassembler class
            Console.WriteLine ("Test CDisassembler class");
            objSystem3Tools.PrintStringList (objSystem3Tools.DisassembleCode (yaBinary));

            // Test CPunchPattern class
            Console.WriteLine ("Test CPunchPattern class");
            objSystem3Tools.PrintPunchStringList (objSystem3Tools.CreatePunchImage
                (" 123456789:#@'=\"0/STUVWXYZ&,%_>?-JKLMNOPQR!$*);^}ABCDEFGHI~.<(+|", '*', true));

            objSystem3Tools.PrintPunchStringList (objSystem3Tools.CreatePunchImage
                ("BB  5A51UB1W5%B&#30 00A8DA-}M00A:0A*  AKAA'@G-A)3SC0A@-GD5'@0F0B @745'\"43\"42=2\"\'4 34# =9\"@:6@8 @", 'o', true));

            objSystem3Tools.PrintPunchStringList (objSystem3Tools.CreatePunchImage
                ("ABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789+_!:;=&<>}| ~%?'\"()@#*$-,./", 'x', true));

            objSystem3Tools.PrintPunchStringList (objSystem3Tools.CreatePunchImage
                ("ABCDEFGHIJKLMNOPQRSTUVWXYZ      }^.<(+|!$*);~-/&,%->?:#@'=\"     01234567899", 'o', true));

            objSystem3Tools.PrintPunchStringList (objSystem3Tools.CreatePunchImage
                ("0@C2  2  2 N0@6                  0@$15 N31E2 ,0@G           0501/8=174174 6@198270@)4@0@'0@^2G", 'o', true));

            objSystem3Tools.PrintPunchStringList (objSystem3Tools.CreatePunchImage
                ("96 COLUMN CARD WITH 6 BIT DATA  ABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789+_!:;=&<>}||~%?'\"()@#*$-,./", 'o', true));
        }
    }
}
