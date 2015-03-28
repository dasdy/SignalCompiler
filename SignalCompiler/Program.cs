﻿using System;
using System.Collections.Generic;
using System.IO;
using SignalCompiler.Models;

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
            //foreach (int lexem in lexTable)
            //{
            //    Console.WriteLine("i = {0}, lexem: {1}", lexem, Constants.GetLexem(lexem));
            //}
            Console.WriteLine("Lexems: ");
            foreach (var lex in lexTable)
            {
                Console.WriteLine("{0} {1}", lex.Position, lex);
            }
            Console.WriteLine("\nErrors:");
            Console.ReadKey(false);
            Console.WriteLine("Table: ");
            foreach (var lex in Constants.LexemsTable)
            {
                Console.WriteLine("{0}: {1}", lex.Key, lex.Value);
            }


            var syntaxer = new Syntaxer();
            var tree = syntaxer.Feed(lexTable, errors);

            foreach (var error in errors)
            {
                Console.WriteLine("{0} {1}",error.Position, error.Message);
            }

            Console.WriteLine(tree.ToString());
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
