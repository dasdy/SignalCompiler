using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            string code = File.ReadAllText(filename);
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
