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
                bool skipAdding = false;
                var attr = Constants.Attributes[cur];
                if (attr == Constants.LexemType.Whitespace)
                {
                    i = SkipWhiteSpace(i, code);
                    skipAdding = true;
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
                    lexemCode = ExamineDelimiter(ref i, code);
                }
                else 
                {
                    //throw error?
                    Console.WriteLine("Error at {0}: unacceptable symbol", i);
                    lexemCode = -1;
                    skipAdding = true;
                    i++;
                }


                if (!skipAdding)
                {
                    Console.WriteLine("lexType= {0}\n", lexemCode);
                    lexems.Add(lexemCode);
                }
                    
            }


            return lexems.ToArray();
        }

        private int ExamineDelimiter(ref int i, string code)
        {
            if (code[i] == ';')
            {
                i++;
                return (int)Constants.GetLexemId(";");
            }

            var subStr = code.Substring(i, 2);
            var possibleLongDelimiter = Constants.GetLexemId(subStr);

            if (possibleLongDelimiter == null)
            {
                i++;
                return (int) Constants.GetLexemId(code[i - 1].ToString());
            }
            i += 2;
            return (int) possibleLongDelimiter;
        }

        private int SkipComment(ref int i, string code)
        {
            throw new NotImplementedException();
        }

        private static int ExamineIdentifier(ref int i, string code)
        {
            StringBuilder buffer = new StringBuilder();
            while (i < code.Length &&
                   (Constants.Attributes[code[i]] == Constants.LexemType.Identifier
                    || Constants.Attributes[code[i]] == Constants.LexemType.Const))
            {
                buffer.Append(code[i]);
                i++;
            }
            var bufRes = buffer.ToString();
            int? identifierId = Constants.GetLexemId(bufRes);
            if (identifierId == null)
            {
                return Constants.RegisterLexem(bufRes);
            }
            return (int) identifierId;
        }

        private static int ExamineConstant(ref int i, string code)
        {
            StringBuilder buffer = new StringBuilder("");
            while (i < code.Length
                   && Constants.Attributes[code[i]] == Constants.LexemType.Const)
            {
                buffer.Append(code[i]);
                i++;
            }
            var bufRes = buffer.ToString();
            int? constId = Constants.GetLexemId(bufRes);
            //if not registered - register
            if (constId == null)
            {
                return Constants.RegisterConstant(bufRes);
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
