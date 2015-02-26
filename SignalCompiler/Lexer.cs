using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SignalCompiler
{
    public class Lexer
    {
        public int[] Feed(string filepath, List<CompilerError> errors)
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
                }
                else if (attr == Constants.LexemType.Const)
                {
                    lexemCode = ExamineConstant(ref i, code);
                }
                else if (attr == Constants.LexemType.Identifier)
                {
                    lexemCode = ExamineIdentifier(ref i, code);
                }
                else if (attr == Constants.LexemType.BegComment)
                {
                    int startCommentPos = i;
                    lexemCode = SkipComment(ref i, code);
                    if (lexemCode == -1)
                    {
                        errors.Add(new CompilerError
                        {
                            Message = "unclosed comment",
                            Position = startCommentPos,
                        });
                    }
                    skipAdding = true;
                }
                else if (attr == Constants.LexemType.LongDelimiter ||
                         attr == Constants.LexemType.ShortDelimiter)
                {
                    lexemCode = ExamineDelimiter(ref i, code);
                }
                else
                {
                    //throw error?
                    errors.Add(new CompilerError
                    {
                        Message = "unacceptable symbol",
                        Position = i,
                    });
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
            if (code[i] == ';' || code[i] == '=')
            {
                i++;
                return (int)Constants.GetLexemId(";");
            }

            var subStr = code.Substring(i, 2);
            var possibleLongDelimiter = Constants.GetLexemId(subStr);

            if (possibleLongDelimiter == null)
            {
                i++;
                return (int)Constants.GetLexemId(code[i - 1].ToString());
            }
            i += 2;
            return (int)possibleLongDelimiter;
        }

        private int SkipComment(ref int i, string code)
        {
            i++;
            if (code[i] != Constants.BegComment[1])
            {
                return -1;
            }
            while (i < code.Length)
            {
                //skip until *
                while (i < code.Length
                       && code[i] != Constants.EndComment[0])
                    i++;

                i++;
                //if after '*' is ')'
                if (i < code.Length - 1
                    && code[i] == Constants.EndComment[1])
                {
                    i++;
                    return 0;
                }
            }
            return -1;
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
            return (int)identifierId;
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
            return (int)constId;
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
