﻿using Sm4shAIEditor.Static;
using System;
using System.Windows.Forms;

namespace Sm4shAIEditor
{
    static class Program
    {
        static string[] keys = { "-a", "-d", "-h" };
        static string helpReminder = string.Format("See {0} for help text", keys[2]);
        static string usage = string.Format("Assemble folder or disassemble file for Smash 4 AI. Usage:\n" +
            "\topen the GUI: no args\n" +
            "\tassembly: {0} [input folder] [output folder]\n" +
            "\tdisassembly: {1} [input file] [output folder]\n" +
            "\tthis text: {2}", keys[0], keys[1], keys[2]);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            else
                ConsoleMain(args);
        }

        private static void ConsoleMain(string[] args)
        {
            try
            {
                string op = args[0];
                if (op == keys[0])
                {
                    aism.AssembleFolder(args[1], args[2]);
                }
                else if (op == keys[1])
                {
                    aism.DisassembleFile(args[1], args[2]);
                }
                else if (op == keys[2])
                    Console.WriteLine(usage);
                else
                    throw new Exception(string.Format("Invalid operation key {0}", op));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(helpReminder);
            }
        }
    }
}
