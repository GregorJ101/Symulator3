using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EmulatorEngine
{
    // Punch Pattern Decoder Engine
    public partial class CDataConversion : CStringProcessor
    {
        // Punch Pattern Decoder Engine member data
        // ABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789+_!:;=&<>}|^~%?'"()@#*$-,./
        // oooooooooooooooooo                   o o o  o oooo    oo  ooo o
        // ooooooooo         oooooooo o         oo    ooooo ooo  o      ooo
        //        oo       oo      oo         ooooooooooo ooooooooooooo oo
        //    oooo     oooo    oooo       oooo  oo  oo oo oo ooooooo o
        //  oo  oo   oo  oo  oo  oo     oo  oo  o ooooo o ooo o o   o o oo
        // o o o o oo o o o o o o o o  o o o o o o        oo  ooooo o o ooo

        //   + _ ! : ; = & < > } | ^ ~ % ? ' " ( ) @ # * $ - , . /

        // 2 o   o   o     o   o o o o         o o     o o o   o 
        // 1 o o         o o o o o   o o o     o             o o o

        // 8 o o o o o o o o o   o o o o o o o o o o o o o   o o 
        // 4 o o     o o   o o   o o   o o o o o o o   o         
        // 2 o   o o o o o   o   o o o   o   o       o   o   o o 
        // 1   o                 o o     o o o o o   o   o   o o o

        //  123456789:#@'="0/STUVWXYZ&,%_>?-JKLMNOPQR!$*);^}ABCDEFGHI~.<(+|
        //                                 oooooooooooooooooooooooooooooooo
        //                 oooooooooooooooo                oooooooooooooooo
        //         oooooooo        oooooooo        oooooooo        oooooooo
        //     oooo    oooo    oooo    oooo    oooo    oooo    oooo    oooo
        //   oo  oo  oo  oo  oo  oo  oo  oo  oo  oo  oo  oo  oo  oo  oo  oo
        //  o o o o o o o o o o o o o o o o o o o o o o o o o o o o o o o o

        // <blank>              0x00
        // A                    0x31
        // B                    0x32
        // C                    0x33
        // D                    0x34
        // E                    0x35
        // F                    0x36
        // G                    0x37
        // H                    0x38
        // I                    0x39
        // J                    0x21
        // K                    0x22
        // L                    0x23
        // M                    0x24
        // N                    0x25
        // O                    0x26
        // P                    0x27
        // Q                    0x28
        // R                    0x29
        // S                    0x12
        // T                    0x13
        // U                    0x14
        // V                    0x15
        // W                    0x16
        // X                    0x17
        // Y                    0x18
        // Z                    0x19
        // 0                    0x10
        // 1                    0x01
        // 2                    0x02
        // 3                    0x03
        // 4                    0x04
        // 5                    0x05
        // 6                    0x06
        // 7                    0x07
        // 8                    0x08
        // 9                    0x09
        // +                    0x3E
        // _                    0x1D
        // !                    0x2A
        // :                    0x0A
        // ;                    0x2E
        // =                    0x0E
        // &                    0x1A
        // <                    0x3C
        // >                    0x1E
        // }                    0x30
        // |                    0x3F
        // ^ <top right corner> 0x2F
        // ~ <cents>            0x3A ??
        // %                    0x1C
        // ?                    0x1F
        // '                    0x0D
        // "                    0x0F
        // (                    0x3D
        // )                    0x2D
        // @                    0x0C
        // #                    0x0B
        // *                    0x2C
        // $                    0x2B
        // -                    0x20
        // ,                    0x1B
        // .                    0x3B
        // /                    0x11

        protected char m_cPrintChar = ' ';
        protected bool m_bColumnSpacing = false;
        protected char[] m_caCharacters = { ' ', 'A', 'B', 'C', 'D', 'E', 'F', 'G',
                                            'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O',
                                            'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W',
                                            'X', 'Y', 'Z', '0', '1', '2', '3', '4',
                                            '5', '6', '7', '8', '9', '+', '_', '!',
                                            ':', ';', '=', '&', '<', '>', '}', '|',
                                            '^', '~', '%', '?', '\'', '"', '(', ')',
                                            '@', '#', '*', '$', '-', ',', '.', '/' };
        protected string m_strCharacters = " ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789+_!:;=&<>}|^~%?\'\"()@#*$-,./";

        protected byte[] m_yaPunchValues = { 0x00, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,
                                             0x38, 0x39, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26,
                                             0x27, 0x28, 0x29, 0x12, 0x13, 0x14, 0x15, 0x16,
                                             0x17, 0x18, 0x19, 0x10, 0x01, 0x02, 0x03, 0x04,
                                             0x05, 0x06, 0x07, 0x08, 0x09, 0x3E, 0x1D, 0x2A,
                                             0x0A, 0x2E, 0x0E, 0x1A, 0x3C, 0x1E, 0x30, 0x3F,
                                             0x2F, 0x3A, 0x1C, 0x1F, 0x0D, 0x0F, 0x3D, 0x2D,
                                             0x0C, 0x0B, 0x2C, 0x2B, 0x20, 0x1B, 0x3B, 0x11 };

        protected List<string> m_listPunchRows;

        public List<string> CreatePunchImage (string strCardData, char cPrintChar, bool bColumnSpacing)
        {
            m_cPrintChar = cPrintChar;
            m_bColumnSpacing = bColumnSpacing;
            m_listPunchRows = new List<string> ();

            if (strCardData.Length < 96)
            {
                strCardData = strCardData + new string (' ', 96 - strCardData.Length);
            }

            //StringBuilder strbldrPunchImage = new StringBuilder ();

            for (int iTier = 1; iTier <= 3; ++iTier)
            {
                for (byte yPunchBit = 0x20; yPunchBit > 0; yPunchBit >>= 1)
                {
                    //if (strbldrPunchImage.Length > 0)
                    //{
                    //    strbldrPunchImage.Append (0x0D);
                    //    strbldrPunchImage.Append (0x0A);
                    //}

                    //strbldrPunchImage.Append (CreatePunchRow (strCardData.Substring (32 * (iTier - 1), 32), yPunchBit));
                    m_listPunchRows.Add (CreatePunchRow (strCardData.Substring (32 * (iTier - 1), 32), yPunchBit));
                }
            }

            //return strbldrPunchImage.ToString ();
            return m_listPunchRows;
        }

        protected string CreatePunchRow (string strTier, byte yPunchBit)
        {
            StringBuilder strbldrPunchRow = new StringBuilder (32);
            for (int iColumn = 1; iColumn <= 32; ++iColumn)
            {
                byte yPunchPattern = CharacterToPunchCode (strTier[iColumn - 1]);
                //for (int iIdx = 0; iIdx < m_caCharacters.Length; ++iIdx)
                //{
                //    if (m_caCharacters[iIdx] == strTier[iColumn - 1])
                //    {
                //        yPunchPattern = m_yaPunchValues[iIdx];
                //        break;
                //    }
                //}
                char cPrintChar = (m_cPrintChar == ' ') ? strTier[iColumn - 1] : m_cPrintChar;
                strbldrPunchRow.Append (((yPunchPattern & yPunchBit) > 0) ? cPrintChar : ' ');
                if (m_bColumnSpacing)
                {
                    strbldrPunchRow.Append (" ");
                }
            }

            return strbldrPunchRow.ToString ();
        }

        protected byte CharacterToPunchCode (char cInput)
        {
            for (int iIdx = 0; iIdx < m_caCharacters.Length; ++iIdx)
            {
                if (m_caCharacters[iIdx] == cInput)
                {
                    return m_yaPunchValues[iIdx];
                }
            }

            return 0x00;
        }
    }
}