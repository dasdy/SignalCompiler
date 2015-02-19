using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalCompiler
{
    public class Lexer
    {
        public int[] Feed(string filepath)
        {
            string code;
            var lexems = new List<int>();
            if (File.Exists(filepath))
            {
                code = File.ReadAllText(filepath);
            }
            else
            {
                throw new FileNotFoundException();
            }

            int i = 0;
            while (i < code.Length)
            {
                char cur = code[i];
                int lexemCode = 0;
                var attr = Constants.Attributes[cur];
                if (attr == Constants.LexemType.Whitespace)
                {
                    i = SkipWhiteSpace(i, code);
                }else if (attr == Constants.LexemType.Const)
                {
                    lexemCode = ExamineConstant(ref i, code);
                }
                else if (attr == Constants.LexemType.Identifier)
                {
                    lexemCode = ExamineIdentifier(ref i, code);
                }
                else if (attr == Constants.LexemType.BegComment)
                {
                    lexemCode = SkipComment(ref i, code);
                }
                else if (attr == Constants.LexemType.LongDelimiter ||
                         attr == Constants.LexemType.ShortDelimiter)
                {
                    lexemCode = ExamideDelimiter(ref i, code);
                }
                else 
                {
                    //throw error?
                    Console.WriteLine("Error at {0}: unacceptable symbol", i);
                    lexemCode = -1;
                    i++;
                }

                Console.WriteLine("lexType= {0}", lexemCode);

                lexems.Add(lexemCode);
            }


            return lexems.ToArray();
        }

        private int ExamideDelimiter(ref int i, string code)
        {
            throw new NotImplementedException();
        }

        private int SkipComment(ref int i, string code)
        {
            throw new NotImplementedException();
        }

        private static int ExamineIdentifier(ref int i, string code)
        {
            string buffer = "";
            while (i < code.Length &&
                   (Constants.Attributes[code[i]] == Constants.LexemType.Identifier
                    || Constants.Attributes[code[i]] == Constants.LexemType.Const))
            {
                buffer += code[i];
                i++;
            }
            int? identifierId = Constants.GetLexemId(buffer);
            if (identifierId == null)
            {
                return Constants.RegisterConstant(buffer);
            }
            return (int) identifierId;
        }

        private static int ExamineConstant(ref int i, string code)
        {
            string buffer = "";
            while (i < code.Length
                   && Constants.Attributes[i] == Constants.LexemType.Const)
            {
                buffer += code[i];
                i++;
            }
            int? constId = Constants.GetConstId(buffer);
            //if not registered - register
            if (constId == null)
            {
                return Constants.RegisterConstant(buffer);
            }
            return (int) constId;
        }

        private static int SkipWhiteSpace(int i, string code)
        {
            while (i < code.Length
                   && Constants.Attributes[code[i]] == Constants.LexemType.Whitespace)
            {
                i++;
            }
            return i;
        }
    }
}
