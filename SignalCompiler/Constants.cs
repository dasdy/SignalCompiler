using System.Collections.Generic;
using System.Linq;

namespace SignalCompiler
{
    public static class Constants
    {
        public static IEnumerable<char> Letters;
        public static IEnumerable<char> Digits;
        public static IEnumerable<char> Delimiter;
        public static IEnumerable<char> Whitespace;
        public static IEnumerable<string> MultiSymbolDelimiter;
        public static IEnumerable<string> Keywords;
        public static IEnumerable<string> GlobalLexemTable;
        public const string BegComment = "(*";
        public const string EndComment = "*)";
        private static IEnumerable<char> GenCharEnumerable(char start, char fin)
        {
            return Enumerable.Range(start, (fin - start) + 1).Select(c => (char)c);
        }


        static Constants()
        {
            Letters = GenCharEnumerable('A', 'Z');
            Digits = GenCharEnumerable('0', '9');
            Delimiter = new[]
            {
                '<', '>', '=', ';'
            };
            Whitespace = new[]
            {
                '\x9', //horizontal tab
                '\xA', //LF
                '\xB', //vertical tab
                '\xC', //new page
                '\xD', //CR
                '\x20', //space
            };
            MultiSymbolDelimiter = new[]
            {
                "<=", ">=", "<>"
            };
            Keywords = new[]
            {
                "PROGRAM",
                "BEGIN",
                "END",
                "IF",  "THEN",  "ENDIF",
                "WHILE","DO", "ENDWHILE"
            };

            GlobalLexemTable = Letters.Select(x => x.ToString())
                                    .Union(Digits.Select(x => x.ToString()))
                                    .Union(Delimiter.Select(x => x.ToString()))
                                    .Union(MultiSymbolDelimiter)
                                    .Union(Keywords)
                                    .OrderBy(x => x);
        }
    }
}
