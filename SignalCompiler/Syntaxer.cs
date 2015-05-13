using System.Collections.Generic;
using System.Linq;
using SignalCompiler.Models;

namespace SignalCompiler
{
    public class Syntaxer
    {
        public SyntaxTree Feed(List<Lexem> lexTable, IList<CompilerError> errors)
        {
            return Program(lexTable, errors);
        }

        private bool CheckTop(List<Lexem> lexTable, string val, IList<CompilerError> errors)
        {
            if (!lexTable.Any())
            {
                errors.Add(new CompilerError
                {
                    Message = string.Format("expected {0}, got EOF", val),
                });
                return false;
            }
            if (lexTable[0].Id != Constants.GetLexemId(val))
            {
                errors.Add(new CompilerError
                {
                    Message = string.Format("expected: '{0}'", val),
                    Position = lexTable[0].Position,
                });

                return false;
            }
            lexTable.RemoveAt(0);
            return true;
        }

        public void PrintLexTable(List<Lexem> lexTable, string message)
        {
            //Console.WriteLine(message);

            //foreach (var lexem in lexTable)
            //{
            //    Console.WriteLine(lexem);
            //}
            ////Console.ReadKey(true);
            //Console.WriteLine();

        }

        private SyntaxTree Program(List<Lexem> lexTable, IList<CompilerError> errors)
        {

            if (!CheckTop(lexTable, "PROGRAM", errors)) return null;

            PrintLexTable(lexTable, "after program popup");
            var procedureIdentifier = ProcIdentifier(lexTable, errors);

            PrintLexTable(lexTable, "after procIdentificator popup");
            if (!CheckTop(lexTable, ";", errors)) return null;
            PrintLexTable(lexTable, "after ';' popup 1");

            var block = Block(lexTable, errors);

            var rootNode = new ProgramNode
            {
                Children = new List<SyntaxTree.Node> { procedureIdentifier, block },
                Value = null,
            };

            return new SyntaxTree { RootNode = rootNode };
        }

        private SyntaxTree.Node Block(List<Lexem> lexTable, IList<CompilerError> errors)
        {
            if (!CheckTop(lexTable, "BEGIN", errors)) return null;

            PrintLexTable(lexTable, "after begin popup");

            var stmtlist = Stmtlist(lexTable, errors);
            PrintLexTable(lexTable, "after stmtlist popup");
            if (!CheckTop(lexTable, "END", errors)) return null;

            PrintLexTable(lexTable, "after end popup");
            return new Block
            {
                Children = new List<SyntaxTree.Node> { stmtlist },
                Value = null,
            };

        }

        private SyntaxTree.Node Stmtlist(List<Lexem> lexTable, IList<CompilerError> errors)
        {
            var stmt = Stmt(lexTable, errors);
            //PrintLexTable(lexTable, "after stmt popup");
            if (stmt == null)
            {
                return new StmtList
                {
                    Children = null,
                    Value = null,
                };
            }

            var nextStatements = Stmtlist(lexTable, errors);

            //PrintLexTable(lexTable, "after stmtlist popup");

            var children = new List<SyntaxTree.Node> { stmt };
            if (nextStatements.Children != null)
            {
                children.AddRange(nextStatements.Children);
            }

            return new StmtList
            {
                Children = children,
                Value = null,
            };
        }

        private SyntaxTree.Node Stmt(List<Lexem> lexTable, IList<CompilerError> errors)
        {
            if (lexTable.Any() && lexTable[0].Id == Constants.GetLexemId("WHILE"))
            {
                lexTable.RemoveAt(0);
                PrintLexTable(lexTable, "after while popup");
                var condExpr = CondExpr(lexTable, errors);
                //PrintLexTable(lexTable, "after condExpr popup");
                if (!CheckTop(lexTable, "DO", errors)) return null;
                PrintLexTable(lexTable, "after DO popup");
                var stmtlist = Stmtlist(lexTable, errors);
                //PrintLexTable(lexTable, "after stmtlist popup");

                if (!CheckTop(lexTable, "ENDWHILE", errors)) return null;
                PrintLexTable(lexTable, "after ENDWHILE popup");
                if (!CheckTop(lexTable, ";", errors)) return null;
                PrintLexTable(lexTable, "after ';' popup 2");
                return new Stmt
                {
                    Children = new List<SyntaxTree.Node> { new WhileStmt()
                    {
                     Children =  new []{condExpr, stmtlist},
                     Value = null,
                    }},
                    Value = null
                };
            }

            if (lexTable.Any() && lexTable[0].Id == Constants.GetLexemId("IF"))
            {


                var condStatement = CondStmt(lexTable, errors);

                //PrintLexTable(lexTable, "after condStmt popup");
                if (!CheckTop(lexTable, "ENDIF", errors)) return null;
                PrintLexTable(lexTable, "after endIf popup");
                if (!CheckTop(lexTable, ";", errors)) return null;
                PrintLexTable(lexTable, "after ';' popup 3");

                return new Stmt
                {
                    Children = new List<SyntaxTree.Node> { condStatement },
                    Value = null,
                };

            }

            return null;
        }

        private SyntaxTree.Node CondStmt(List<Lexem> lexTable, IList<CompilerError> errors)
        {
            var thenStmt = ThenStmt(lexTable, errors);
            //PrintLexTable(lexTable, "after thenStmt popup");
            var elseStmt = ElseStmt(lexTable, errors);
            //PrintLexTable(lexTable, "after elseStmt popup");
            var children = new List<SyntaxTree.Node> { thenStmt };

            if (elseStmt != null) children.Add(elseStmt);

            return new IfStmt
            {
                Children = children,
                Value = null,
            };
        }

        private SyntaxTree.Node ThenStmt(List<Lexem> lexTable, IList<CompilerError> errors)
        {
            if (!CheckTop(lexTable, "IF", errors)) return null;
            PrintLexTable(lexTable, "after IF popup");
            var condition = CondExpr(lexTable, errors);

            if (!CheckTop(lexTable, "THEN", errors)) return null;
            PrintLexTable(lexTable, "after THEN popup");
            var stmts = Stmtlist(lexTable, errors);
            //PrintLexTable(lexTable, "after stmtlist popup");
            return new IfThen()
            {
                Children = new List<SyntaxTree.Node> { condition, stmts },
                Value = null,
            };
        }

        private SyntaxTree.Node ElseStmt(List<Lexem> lexTable, IList<CompilerError> errors)
        {
            if (lexTable.Any() && lexTable[0].Id == Constants.GetLexemId("ELSE"))
            {
                lexTable.RemoveAt(0);
                PrintLexTable(lexTable, "before ELSE popup");
                var stmtList = Stmtlist(lexTable, errors);
                //PrintLexTable(lexTable, "after stmtlist popup");
                return new IfElse
                {
                    Children = new List<SyntaxTree.Node> { stmtList },
                    Value = null,
                };
            }

            return null;
        }

        private SyntaxTree.Node CondExpr(List<Lexem> lexTable, IList<CompilerError> errors)
        {
            var lExpr = Expression(lexTable, errors);
            //PrintLexTable(lexTable, "after lExpr popup");
            var comparison = ComparisonOp(lexTable, errors);
            //PrintLexTable(lexTable, "after condOp popup");
            var rExpr = Expression(lexTable, errors);
            //PrintLexTable(lexTable, "after rExpr popup");
            return new CondExpr
            {
                Children = new List<SyntaxTree.Node> { lExpr, comparison, rExpr },
                Value = null,
            };
        }

        private SyntaxTree.Node ComparisonOp(List<Lexem> lexTable, IList<CompilerError> errors)
        {
            if (!lexTable.Any())
            {
                errors.Add(new CompilerError
                {
                    Message = "expected comparisonOp, got EOF"
                });
                return null;
            }
            var node = Constants.GetLexem(lexTable[0].Id);
            var top = lexTable[0];
            lexTable.RemoveAt(0);

            if (Constants.ComparisonOperators.Contains(node))
            {
                return new ComparisonOp
                {
                    Children = null,
                    Value = top,
                };
            }


            PrintLexTable(lexTable, "after compOp popup");
            errors.Add(new CompilerError
            {
                Message = "Unknown comparisonOperator:" + node,
                Position = lexTable[0].Position
            });
            return null;
        }

        private SyntaxTree.Node Expression(List<Lexem> lexTable, IList<CompilerError> errors)
        {
            if (!lexTable.Any())
            {
                errors.Add(new CompilerError
                {
                    Message = "expected expression, got EOF"
                });
                return null;
            }

            PrintLexTable(lexTable, "before expr popup");
            var id = lexTable[0].Id;
            string type;
            SyntaxTree.Node child;

            if (id >= Constants.IdentifierStartIndex)
            {
                //it is a identifier
                child = VarIdentifier(lexTable, errors);
            }
            else if (id >= Constants.ConstStartIndex)
            {
                //it is constant
                child = Integer(lexTable, errors);
            }
            else
            {
                errors.Add(new CompilerError
                {
                    Message = "expected id or number",
                    Position = lexTable[0].Position

                });
                return null;
            }
            PrintLexTable(lexTable, "after expression popup");
            return new Expression
            {
                Children = new List<SyntaxTree.Node> { child },
                Value = null,
            };
        }

        private SyntaxTree.Node VarIdentifier(List<Lexem> lexTable, IList<CompilerError> errors)
        {
            var node = Identifier(lexTable, errors);
            //PrintLexTable(lexTable, "after id popup");
            return new VarIdentifier
            {
                Children = new List<SyntaxTree.Node> { node },
                Value = null,
            };
        }

        private SyntaxTree.Node ProcIdentifier(List<Lexem> lexTable, IList<CompilerError> errors)
        {
            var node = Identifier(lexTable, errors);
            //PrintLexTable(lexTable, "after id popup");
            return new ProcIdentifier
            {
                Children = new List<SyntaxTree.Node> { node },
                Value = null,
            };
        }

        private SyntaxTree.Node Identifier(List<Lexem> lexTable, IList<CompilerError> errors)
        {
            if (!lexTable.Any())
            {
                errors.Add(new CompilerError
                {
                    Message = "expected identifier, got EOF"
                });
                return null;
            }
            var node = Constants.GetLexem(lexTable[0].Id);
            var top = lexTable[0];

            if (lexTable[0].Id < Constants.IdentifierStartIndex)
            {
                errors.Add(new CompilerError
                {
                    Position = lexTable[0].Position,
                    Message = "Not a identifier:" + node
                });
                return null;
            }
            lexTable.RemoveAt(0);

            PrintLexTable(lexTable, "after id popup");

            return new Identifier
            {
                Children = null,
                Value = top,
            };
        }

        private SyntaxTree.Node Integer(List<Lexem> lexTable, IList<CompilerError> errors)
        {
            if (!lexTable.Any())
            {
                errors.Add(new CompilerError
                {
                    Message = "expected integer, got EOF"
                });
                return null;
            }
            var node = Constants.GetLexem(lexTable[0].Id);


            if (lexTable[0].Id < Constants.ConstStartIndex || lexTable[0].Id > Constants.IdentifierStartIndex)
            {
                errors.Add(new CompilerError
                {
                    Position = lexTable[0].Position,
                    Message = "Not an integer:" + node
                });
                return null;
            }
            var top = lexTable[0];
            lexTable.RemoveAt(0);

            PrintLexTable(lexTable, "after const popup");

            return new Integer
            {
                Children = null,
                Value = top
            };
        }
    }
}
