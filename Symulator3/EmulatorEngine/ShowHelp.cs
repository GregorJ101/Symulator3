using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EmulatorEngine
{
    // Show Help
    public partial class CDataConversion : CStringProcessor
    {
        public void ShowHelp ()
        {
            WriteOutputLine ("Usage: CardDeckReader [filename]");
            WriteOutputLine ("       This program will read data from an IBM System/3 card or deck of cards");
            WriteOutputLine ("       and perform a variety of operations on the data, including punch pattern");
            WriteOutputLine ("       recognition, hex/ASCII/EBCDIC dumping, compex disassembly, and emulation.");
            WriteOutputLine ("");
            WriteOutputLine ("       Following is a detailed description of all the commands that may be embedded");
            WriteOutputLine ("       within a file from a deck of punch cards from an IBM System/3.  The cards to");
            WriteOutputLine ("       processed must follow and precede some of the command cards as explained below.");
            WriteOutputLine ("");

            ShowProgramHelp ();
            ShowCoreSizeHelp ();
            ShowTitleHelp ();
            ShowSeparatorHelp ();
            ShowSpaceHelp ();
            ShowPrntSetupHelp ();
            ShowPrntDefaultHelp ();
            ShowPrntHelp ();
            ShowPrntTextMatrixHelp ();
            ShowPrnt96Help ();
            ShowPrntAllHelp ();
            ShowPrntGroupHelp ();
            ShowListGroupHelp ();
            ShowEndGroupHelp ();
            ShowLoadHelp ();
            ShowLoadPlusHelp ();
            ShowTxltHelp ();
            ShowDumpHelp ();
            ShowDumpAllHelp ();
            ShowDASMHelp ();
            ShowDasmRangeHelp ();
            ShowDasmEndCardHelp ();
            ShowDasmEntryHelp ();
            ShowDasmCodeHelp ();
            ShowDasmDataHelp ();
            ShowDasmSkipHelp ();
            ShowDasmTagHelp ();
            ShowDasmVariableHelp ();
            ShowDasmConstantHelp ();
            ShowDasmCommentHelp ();
            ShowDasmAutoTagHelp ();
            ShowDasmExtendedSetHelp ();
            ShowDasmSmartHelp ();
            ShowDasmAnnotateHelp ();
            ShowDasmEndHelp ();
            ShowIgnoreHelp ();
        }

        private void ShowHelpHelp ()
        {
            WriteOutputLine ("/Help or /? produces this display of instructions for all commands");
            WriteOutputLine ("");
        }

        private void ShowProgramHelp ()
        {
            WriteOutputLine ("/Program:{name of program}");
            WriteOutputLine ("      Program name to be displayed in end card label, ...");
            WriteOutputLine ("");
        }

        private void ShowCoreSizeHelp ()
        {
            WriteOutputLine ("/CoreSize:{main memory size for emulator in increments of 1024 byts}");
            WriteOutputLine ("");
        }

        private void ShowTitleHelp ()
        {
            WriteOutputLine ("/Title:Text/Matrix,<title text to be centered in 100-char line");
            WriteOutputLine ("       Text means print line out as is");
            WriteOutputLine ("       Matrix means print as 5x5 block letters");
            WriteOutputLine ("");
        }

        private void ShowSeparatorHelp ()
        {
            WriteOutputLine ("Separator:{lenght of line}");
            WriteOutputLine ("          {{,}#leading spaces}");
            WriteOutputLine ("          {{,}char/string to repeat}");
            WriteOutputLine ("          {{,}#blank lines before}");
            WriteOutputLine ("          {{,}#blank lines after}");
            WriteOutputLine ("");
        }

        private void ShowSpaceHelp ()
        {
            WriteOutputLine ("/Space:{# blank lines to insert}");
            WriteOutputLine ("       No number specified causes a default of a single blank line inserted");
            WriteOutputLine ("");
        }

        private void ShowPrntSetupHelp ()
        {
            WriteOutputLine ("/PrntSetup {Edge}{{,}Char:x}{{,}TightColumns}{{,}Wrap96|NoWrap}{{,}ShowTrailer}{{,}Separator}{{,}Spacing:n}");
            WriteOutputLine ("            This sets conditions that affect Prnt96 commands");
            WriteOutputLine ("            Edge to show card edge outline around punch pattern characters");
            WriteOutputLine ("            Char: spedifies the character to use for corresponding hole punch");
            WriteOutputLine ("            TightColumns eliminates blanks between each punch column");
            WriteOutputLine ("            Wrap96 causes characters to be wrapped to simulate the punches in a 96-column card");
            WriteOutputLine ("            NoWrap causes characters to be shown in a single group of 6 lines");
            WriteOutputLine ("            ShowTrailer causes displaying any characters past the 96th on a separate line");
            WriteOutputLine ("            Separator creates a line of 100 '=' characters after showing the punch matrix");
            WriteOutputLine ("            Spacing specifies the number of blank lines to insert after matrix or separator");
            WriteOutputLine ("");
        }

        private void ShowPrntDefaultHelp ()
        {
            WriteOutputLine ("/PrntDefault (no arguments)");
            WriteOutputLine ("      Resets all settins set in /PrntSetup");
            WriteOutputLine ("");
        }

        private void ShowPrntHelp ()
        {
            WriteOutputLine ("PRNT:<text to treat as punch characters>");
            WriteOutputLine ("      Groups of PRNT lines are centered in 80-char line or longest input line if longer");
            WriteOutputLine ("      and displayed in the order in which they are read; these display before all else");
            WriteOutputLine ("");
        }

        private void ShowPrntTextMatrixHelp ()
        {
            WriteOutputLine ("/PrntTextMatrix:<followed by text to be expanded to small block letters in 5x5 matrix,");
            WriteOutputLine ("      centered in 100-character line unless longer.");
            WriteOutputLine ("    If several /PrntTextMatrix lines appear, all are centered in the longest line if the longest");
            WriteOutputLine ("      line exceeds 100 characters");
            WriteOutputLine ("");
        }

        private void ShowPrnt96Help ()
        {
            WriteOutputLine ("/Prnt96:<96 columns of text> for individual cards; limited to or expanded to 96 char");
            WriteOutputLine ("");
        }

        private void ShowPrntAllHelp ()
        {
            WriteOutputLine ("/PrntAll:{96}/{LINE}");
            WriteOutputLine ("         96: Treat each line as a 3-tier card");
            WriteOutputLine ("         LINE: Treat each line as single long tier, centered on 100-char line");
            WriteOutputLine ("");
        }

        private void ShowPrntGroupHelp ()
        {
            WriteOutputLine ("/PrntGroup: Accumulate lines treated as 1 tier per line");
            WriteOutputLine ("            Center all on 100 or length of longest line, whichever is longer");
            WriteOutputLine ("            Accumulate lines until /ENDGROUP found or end of file");
            WriteOutputLine ("");
        }

        private void ShowListGroupHelp ()
        {
            WriteOutputLine ("/ListGroup (no arguments)");
            WriteOutputLine ("            Indicates that all non-command lines are to be copied directly to the output");
            WriteOutputLine ("            Continue until /EndGroup is encountered.");
            WriteOutputLine ("");
        }

        private void ShowEndGroupHelp ()
        {
            WriteOutputLine ("/EndGroup (no arguments)");
            WriteOutputLine ("          Closes the group opened by /PrntGroup or /ListGroup");
            WriteOutputLine ("          Triggers the processing of any accumulated lines");
            WriteOutputLine ("          ");
            WriteOutputLine ("");
        }

        private void ShowLoadHelp ()
        {
            WriteOutputLine ("/Load:<IPL/TEXT/HEX>,<start source address>,<start target address>,<length>");
            WriteOutputLine ("      addresses & length ignored for TEXT");
            WriteOutputLine ("      IPL:  lines that follow are to be interpreted as IPL-format cards");
            WriteOutputLine ("      TEXT: lines that follow are to be interpreted as text-format cards");
            WriteOutputLine ("      HEX:  lines that follow are to be simply compressed from the hex character pairs");
            WriteOutputLine ("            The following parameters <start>/<end>/<entry> are required for HEX:");
            WriteOutputLine ("              <start> Load characters begining at this address");
            WriteOutputLine ("              <end>   End address of code block");
            WriteOutputLine ("              <entry> Entry point");
            WriteOutputLine ("      /Load must be the last command line before any data lines to be dumped or disassembled");
            WriteOutputLine ("      The commands /TXLT, /DUMP, and /DASM operate on the lines (cards) just loaded, and may");
            WriteOutputLine ("        ONLY be used AFTER a /LOAD command has been processed, AND subsequent lines (cards)");
            WriteOutputLine ("        successfully loaded into a binary image");
            WriteOutputLine ("      When load comes again, the previous load image is destroyed and replaced with new data");
            WriteOutputLine ("      The command /PLLT may only be used if the last /LOAD line processed was for TEXT line");
            WriteOutputLine ("      The commands /DUMP and /DASM may be used on any load format");
            WriteOutputLine ("      /Load clears any program loaded earlier and reset all values for the next program");
            WriteOutputLine ("");
        }

        private void ShowLoadPlusHelp ()
        {
            WriteOutputLine ("/LOAD+ indicates that the following line(s) are to be added to the existing binary image");
            WriteOutputLine ("       Identical to /Load except that /Load+ does not clear memory or any setting");
            WriteOutputLine ("");
        }

        private void ShowTxltHelp ()
        {
            WriteOutputLine ("/TXLT:{ShowRaw}{{,}DumpLiterals}{{,}ShowMove} for text-to-list");
            WriteOutputLine ("      ShowRaw displays card characters before conversion");
            WriteOutputLine ("      DumpLiterals shows EBCDIC characters after conversion");
            WriteOutputLine ("      ShowMove shows number of bytes moved and destination address range");
            WriteOutputLine ("      Indicates immedate display action");
            WriteOutputLine ("");
        }

        private void ShowDumpHelp ()
        {
            WriteOutputLine ("/DUMP:<start address>,<end address>");
            WriteOutputLine ("      Performs hex dump with ASCII and EBCDIC literals with specificed address range");
            WriteOutputLine ("      Indicates immedate display action");
            WriteOutputLine ("");
        }

        private void ShowDumpAllHelp ()
        {
            WriteOutputLine ("/DUMPALL for entire binary image");
            WriteOutputLine ("      Performs hex dump with ASCII and EBCDIC literals on entire memory image");
            WriteOutputLine ("      Indicates immedate display action");
            WriteOutputLine ("");
        }

        private void ShowDASMHelp ()
        {
            WriteOutputLine ("/DASM:<start address>,<end address>,<entry point>");
            WriteOutputLine ("      Performs System/3 disassembly on specificed address range");
            WriteOutputLine ("      Indicates immedate display action");
            WriteOutputLine ("");
        }

        private void ShowDasmRangeHelp ()
        {
            WriteOutputLine ("/DASM-Range:<start address>,<end address>,<entry point>");
            WriteOutputLine ("      Sets specificed address range for System/3 disassembly");
            WriteOutputLine ("");
        }

        private void ShowDasmEndCardHelp ()
        {
            WriteOutputLine ("/DASM-EndCard:<start address>,<end address>,<entry point>");
            WriteOutputLine ("      Performs System/3 disassembly on end card with specificed address range");
            WriteOutputLine ("      Indicates immedate display action");
            WriteOutputLine ("");
        }

        private void ShowDasmEntryHelp ()
        {
            WriteOutputLine ("/DASM-Entry: <address>, <label to be displayed as label or comment on same line>");
            WriteOutputLine ("");
        }

        private void ShowDasmCodeHelp ()
        {
            WriteOutputLine ("/DASM-Code: <address>, <label to be displayed as a separate line ahead of code block>");
            WriteOutputLine ("");
        }

        private void ShowDasmDataHelp ()
        {
            WriteOutputLine ("/DASM-Data: <address>, <label to be displayed as a separate line ahead of data block>");
            WriteOutputLine ("");
        }

        private void ShowDasmSkipHelp ()
        {
            WriteOutputLine ("/DASM-Skip: <address>");
            WriteOutputLine ("      Indicates start address of area to ignore");
            WriteOutputLine ("");
        }

        private void ShowDasmTagHelp ()
        {
            WriteOutputLine ("/DASM-Tag: <address>, <label to be displayed on same line as location and any branches / jumps>");
            WriteOutputLine ("");
        }

        private void ShowDasmVariableHelp ()
        {
            WriteOutputLine ("/DASM-Variable: <address>, <length>, <name to be applied to operand addresses>");
            WriteOutputLine ("");
        }

        private void ShowDasmConstantHelp ()
        {
            WriteOutputLine ("/DASM-Constant: <address>, <length>, <name to be applied to operand addresses>");
            WriteOutputLine ("");
        }

        private void ShowDasmCommentHelp ()
        {
            WriteOutputLine ("/DASM-Comment:<address>,<label> for instruction comments");
            WriteOutputLine ("");
        }

        private void ShowDasmAutoTagHelp ()
        {
            WriteOutputLine ("/DASM-AutoTag:{Jumps}{{,}{Loops}");
            WriteOutputLine ("");
        }

        private void ShowDasmExtendedSetHelp ()
        {
            WriteOutputLine ("/DASM-ExtendedSet:{JC}{{,}{BC}{{,}MVX}{{,}{All} for expanding to BAL extended mnemonics");
            WriteOutputLine ("      If no arguments found, assume ALL");
            WriteOutputLine ("");
        }

        private void ShowDasmSmartHelp ()
        {
            WriteOutputLine ("/DASM-Smart");
            WriteOutputLine ("");
        }

        private void ShowDasmAnnotateHelp ()
        {
            WriteOutputLine ("/DASM-Annotate:{MultiLine}{{,}}{SIO}{{,}LIO}{{,}TIO}{{,}SNS}{{,}IO}{{,}A}{{,}L}{{,}LA}{{,}ST}{{,}REG}");
            WriteOutputLine ("      Add comments interpretting meaning of op code & Q byte");
            WriteOutputLine ("      IO means SIO, SNS, TIO, LIO");
            WriteOutputLine ("      REG means A, L, LA, ST");
            WriteOutputLine ("      MultiLine allows expanding comments to multiple lines as needed; first line is on instruction line,");
            WriteOutputLine ("        add blank lines with comments only as needed");
            WriteOutputLine ("");
        }

        private void ShowDasmEndHelp ()
        {
            WriteOutputLine ("/DASM-End signal end of command lines and command to perform disassembly");
            WriteOutputLine ("");
        }

        private void ShowIgnoreHelp ()
        {
            WriteOutputLine ("/Ignore (this line can contain any comments useful when editing the file to be parsed)");
        }
    }
}