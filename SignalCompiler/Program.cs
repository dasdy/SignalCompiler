﻿using System;
using System.Collections.Generic;
using System.IO;

namespace SignalCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            var filename = GetProgramCode(args);
            if (filename == null)
            {
                return;
            }
            var lexer = new Lexer();
            var errors = new List<CompilerError>();
            var lexTable = lexer.Feed(filename, errors);
            foreach (int lexem in lexTable)
            {
                Console.WriteLine("i = {0}, lexem: {1}", lexem, Constants.GetLexem(lexem));
            }
            Console.WriteLine("Errors:");
            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }
        }

        private static string GetProgramCode(string[] args)
        {
            string filename;
            if (args.Length > 1)
            {
                Console.WriteLine("Too many args");
                return null;
            }
            if (args.Length == 0)
            {
                Console.WriteLine("Enter filename");
                filename = Console.ReadLine();
            }
            else
            {
                filename = args[0];
            }

            if (!File.Exists(filename))
            {
                Console.WriteLine("File doesn't exist!");
                return null;
            }
            return filename;
        }
    }
}
