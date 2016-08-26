using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EmulatorEngine
{
    public class CDiskControlField
    {
        public byte yF = 0x00; // Flags: 0x02, 0x01
        public byte yC = 0x00; // Cylinder number (not used in seek operations)
        public byte yS = 0x00; // Sector number 00 - 23, 32 - 55; (Seek: 0x80 Head 0 or 1; 0x01 direction: 0 toward sector 0)
        public byte yN = 0x00; // Number of cylinders to seek or sectors ot read/write/verify

        // Diagnostic code
        private bool bStateSaved = false;
        private byte yFp = 0x00;
        private byte yCp = 0x00;
        private byte ySp = 0x00;
        private byte yNp = 0x00;

        public void SaveState ()
        {
            yFp = yF;
            yCp = yC;
            ySp = yS;
            yNp = yN;
            bStateSaved = true;
        }

        public void ResetState ()
        {
            yFp = 0;
            yCp = 0;
            ySp = 0;
            yNp = 0;
            bStateSaved = false;
        }

        public void DisplayStates ()
        {
            if (bStateSaved)
            {
                Console.WriteLine ("            DCF before SIO - F:{0:X2} C:{1:X2} S:{2:X2} N:{3:X2}", yFp, yCp, ySp, yNp);
            }

            Console.WriteLine ("            DCF after SIO -  F:{0:X2} C:{1:X2} S:{2:X2} N:{3:X2}", yF, yC, yS, yN);

            ResetState ();
        }
    }
}