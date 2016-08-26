using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

//using System3Emulator;
//using IBMSystem3ToolsLib;
//using StringProcessingLib;

namespace ScriptParser
{
    class CScriptParser
    {
        static void Main (string[] args)
        {
            //if (args.Length > 0)
            //{
            //    CEmulatorEngine objSystem3Tools = new CEmulatorEngine ();
            //    //CDataConversion objSystem3Tools = new CDataConversion ();
            //    if (objSystem3Tools.CompareNoCase (args[0], "/?") ||
            //        objSystem3Tools.CompareNoCase (args[0], "/Help"))
            //    {
            //        objSystem3Tools.ShowHelp ();

            //        return;
            //    }

            //    //objSystem3Tools.TestFetch ();
            //    //objSystem3Tools.TestConditionRegister ();
            //    //objSystem3Tools.TestInstructions ();

            //    if (!File.Exists (args[0]))
            //    {
            //        Console.WriteLine ("Unable to open file " + args[0] + ".");
            //    }

            //    List<string> strlCardDeckInput = objSystem3Tools.ReadFileToStringList (args[0], 0);

            //    if (strlCardDeckInput.Count > 0)
            //    {
            //        foreach (string strCard in strlCardDeckInput)
            //        {
            //            if (!objSystem3Tools.ProcessCardCommandLine (strCard))
            //            {
            //                objSystem3Tools.ProcessCardLine (strCard);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        Console.WriteLine ("File " + args[0] + "is empty.");
            //    }
            //}
            //else
            //{
            //    Console.WriteLine ("Usage: CardDeckReader [filename]");
            //    Console.WriteLine ("       For help, type /? or /Help");
            //}
        }
    }
}
