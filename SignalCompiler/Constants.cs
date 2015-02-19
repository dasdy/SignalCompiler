using System.Collections.Generic;
using System.Linq;

namespace SignalCompiler
{
    public static class Constants
    {
        public enum LexemType
        {
            Whitespace,
            Const, 
            Identifier,
            ShortDelimiter, 
            LongDelimiter, 
            BegComment, 
            Unacceptable
        }
        public static IEnumerable<char> Letters;
        public static IEnumerable<char> Digits;
        public static IEnumerable<char> Delimiter;
        public static IEnumerable<char> Whitespace;
        public static IEnumerable<string> MultiSymbolDelimiter;
        public static IEnumerable<string> Keywords;
        public static IEnumerable<string> GlobalLexemTable;
        public const string BegComment = "(*";
        public const string EndComment = "*)";
        public static readonly IDictionary<int, string> LexemsTable;
        public static readonly IDictionary<string, int> LexemsIdTable;
        public static LexemType[] Attributes;
        public static int LastLexemId;
        public static int LastConstId;
        public const int DelimStartIndex = 300;
        public const int KeywordStartIndex = 400;
        public const int ConstStartIndex = 500;
        public const int IdentifierStartIndex = 1000;

        public static int? GetId(string lexem)
        {
            if (LexemsIdTable.ContainsKey(lexem))
                return LexemsIdTable[lexem];

            return null;
        }

        public static string GetLexem(int id)
        {
            if (LexemsTable.ContainsKey(id))
                return LexemsTable[id];

            return null;
        }

        public static void RegisterLexem(string lexem)
        {
            LastLexemId++;
            LexemsTable.Add(LastLexemId, lexem);
            LexemsIdTable.Add(lexem, LastLexemId);
        }

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
                '\x20' //space
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

            LexemsTable = new SortedDictionary<int, string>();
            LexemsIdTable = new SortedDictionary<string, int>();
            
            for (int i = 0; i < 255; i++)
            {
                Attributes[i] = CharType((char) i);
                if (Attributes[i] != LexemType.Unacceptable)
                {
                    string stringRepOfChar = ((char) i).ToString();
                    LexemsTable.Add(i, stringRepOfChar);
                    LexemsIdTable.Add(stringRepOfChar, i);
                }
            }

            int ind = DelimStartIndex;
            foreach (var delim in MultiSymbolDelimiter)
            {
                ind++;
                LexemsTable.Add(ind, delim);
                LexemsIdTable.Add(delim, ind);
            }

            ind = KeywordStartIndex;
            foreach (var keyword in Keywords)
            {
                ind++;
                LexemsTable.Add(ind, keyword);
                LexemsIdTable.Add(keyword, ind);
            }
        }

        public static LexemType CharType(char i)
        {
            if (Letters.Contains(i))
            {
                return LexemType.Identifier;
            }
            if (Digits.Contains(i))
            {
                return LexemType.Const;
            }
            if (Whitespace.Contains(i))
            {
                return LexemType.Whitespace;
            }
            if (Delimiter.Contains(i))
            {
                return LexemType.ShortDelimiter;
            }
            return LexemType.Unacceptable;
        }
    }
}
