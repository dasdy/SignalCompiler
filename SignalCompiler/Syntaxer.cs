using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignalCompiler.Models;

namespace SignalCompiler
{
    public class Syntaxer
    {
        public SyntaxTree Feed(int[] lexTable, IList<CompilerError> errors)
        {
            var lexStack = lexTable.ToList();
            return Program(lexStack, errors);
        }

        private SyntaxTree Program(List<int> lexTable, IList<CompilerError> errors)
        {
            if (lexTable[0] != Constants.GetLexemId("PROGRAM"))
            {
                errors.Add(new CompilerError
                {
                    Line = 0,
                    Message = @"expected 'PROGRAM'",
                    Position = 0
                });
                return null;
            }
            lexTable.RemoveAt(0);

            var procedureIdentifier = ProcIdentifier(lexTable, errors);

            if (lexTable[0] != Constants.GetLexemId(";"))
            {
                errors.Add(new CompilerError
                {
                    Line = 0,
                    Message = @"expected ';'",
                    Position = 0
                });
                return null;
            }
            lexTable.RemoveAt(0);

            var block = Block(lexTable, errors);

            var rootNode = new SyntaxTree.Node
            {
                Children = new List<SyntaxTree.Node> { procedureIdentifier, block },
                Type = "Program",
                Value = null,
            };

            return new SyntaxTree { RootNode = rootNode };
        }

        private SyntaxTree.Node Block(List<int> lexTable, IList<CompilerError> errors)
        {
            if (lexTable[0] != Constants.GetLexemId("BEGIN"))
            {
                errors.Add(new CompilerError
                {
                    Line = 0,
                    Message = @"expected 'BEGIN'",
                    Position = 0
                });
                return null;
            }
            lexTable.RemoveAt(0);

            var stmtlist = Stmt(lexTable, errors);

            return new SyntaxTree.Node
            {
                Children = new List<SyntaxTree.Node> { stmtlist },
                Type = "Block",
                Value = null,
            };

        }

        private SyntaxTree.Node Stmtlist(List<int> lexTable, IList<CompilerError> errors)
        {
            var stmt = Stmt(lexTable, errors);
            if (stmt == null)
            {
                return null;
            }

            var nextStatements = Stmtlist(lexTable, errors);

            var children = new List<SyntaxTree.Node> { stmt };
            children.AddRange(nextStatements.Children);

            return new SyntaxTree.Node
            {
                Children = children,
                Type = "StmtList",
                Value = null,
            };
        }

        private SyntaxTree.Node Stmt(List<int> lexTable, IList<CompilerError> errors)
        {

            if (lexTable[0] == Constants.GetLexemId("WHILE"))
            {
                lexTable.RemoveAt(0);
                var condExpr = CondExpr(lexTable, errors);

                if (lexTable[0] != Constants.GetLexemId("DO"))
                {
                    errors.Add(new CompilerError
                    {
                        Line = 0,
                        Message = @"expected 'DO'",
                        Position = 0
                    });
                    return null;
                }
                lexTable.RemoveAt(0);

                var stmtlist = Stmtlist(lexTable, errors);

                if (lexTable[0] != Constants.GetLexemId("ENDWHILE"))
                {
                    errors.Add(new CompilerError
                    {
                        Line = 0,
                        Message = @"expected 'ENDWHILE'",
                        Position = 0
                    });
                    return null;
                }
                lexTable.RemoveAt(0);

                if (lexTable[0] != Constants.GetLexemId(";"))
                {
                    errors.Add(new CompilerError
                    {
                        Line = 0,
                        Message = @"expected ';'",
                        Position = 0
                    });
                    return null;
                }
                lexTable.RemoveAt(0);

                return new SyntaxTree.Node
                {
                    Children = new List<SyntaxTree.Node> { condExpr, stmtlist },
                    Type = "Stmt",
                    Value = null
                };
            }


            var condStatement = CondStmt(lexTable, errors);

            if (lexTable[0] != Constants.GetLexemId("ENDIF"))
            {
                errors.Add(new CompilerError
                {
                    Line = 0,
                    Message = @"expected 'ENDIF'",
                    Position = 0
                });
                return null;
            }
            lexTable.RemoveAt(0);


            if (lexTable[0] != Constants.GetLexemId(";"))
            {
                errors.Add(new CompilerError
                {
                    Line = 0,
                    Message = @"expected ';'",
                    Position = 0
                });
                return null;
            }


            lexTable.RemoveAt(0);
            return new SyntaxTree.Node
            {
                Children = new List<SyntaxTree.Node> { condStatement },
                Type = "Stmt",
                Value = null,
            };
        }

        private SyntaxTree.Node CondStmt(List<int> lexTable, IList<CompilerError> errors)
        {
            var thenStmt = ThenStmt(lexTable, errors);
            var elseStmt = ElseStmt(lexTable, errors);
            var children = new List<SyntaxTree.Node> { thenStmt };
            if (elseStmt != null) children.Add(elseStmt);
            return new SyntaxTree.Node
            {
                Children = children,
                Type = "ConditionalStmt",
                Value = null,
            };
        }

        private SyntaxTree.Node ThenStmt(List<int> lexTable, IList<CompilerError> errors)
        {
            if (lexTable[0] == Constants.GetLexemId("IF"))
            {
                errors.Add(new CompilerError
                {
                    Line = 0,
                    Message = @"expected 'IF'",
                    Position = 0
                });
                return null;
            }
            lexTable.RemoveAt(0);

            var condition = CondExpr(lexTable, errors);

            if (lexTable[0] == Constants.GetLexemId("THEN"))
            {
                errors.Add(new CompilerError
                {
                    Line = 0,
                    Message = @"expected 'THEN'",
                    Position = 0
                });
                return null;
            }
            lexTable.RemoveAt(0);

            var stmts = Stmtlist(lexTable, errors);

            return new SyntaxTree.Node
            {
                Children = new List<SyntaxTree.Node> { condition, stmts },
                Type = "IfThen",
                Value = null,
            };
        }

        private SyntaxTree.Node ElseStmt(List<int> lexTable, IList<CompilerError> errors)
        {
            if (lexTable[0] == Constants.GetLexemId("ELSE"))
            {
                lexTable.RemoveAt(0);
                var stmtList = Stmtlist(lexTable, errors);
                return new SyntaxTree.Node
                {
                    Children = new List<SyntaxTree.Node> { stmtList },
                    Type = "ElseStmt",
                    Value = null,
                };
            }

            return null;
        }

        private SyntaxTree.Node CondExpr(List<int> lexTable, IList<CompilerError> errors)
        {
            var lExpr = Expression(lexTable, errors);
            var comparison = ComparisonOp(lexTable, errors);
            var rExpr = Expression(lexTable, errors);

            return new SyntaxTree.Node
            {
                Children = new List<SyntaxTree.Node> { lExpr, comparison, rExpr },
                Type = "ConditionalExpression",
                Value = null,
            };
        }

        private SyntaxTree.Node ComparisonOp(List<int> lexTable, IList<CompilerError> errors)
        {
            var node = Constants.GetLexem(lexTable[0]);
            lexTable.RemoveAt(0);

            if (Constants.ComparisonOperators.Contains(node))
            {
                return new SyntaxTree.Node
                {
                    Children = null,
                    Type = "ComparisonOperator",
                    Value = node,
                };
            }

            errors.Add(new CompilerError
            {
                Line = 0,
                Message = "Unknown comparisonOperator:" + node,
                Position = 0,
            });
            return null;
        }

        private SyntaxTree.Node Expression(List<int> lexTable, IList<CompilerError> errors)
        {
            var id = lexTable[0];
            string type;
            SyntaxTree.Node child;

            if (id >= Constants.IdentifierStartIndex)
            {
                //it is a identifier
                child = VarIdentifier(lexTable, errors);
                type = "ExprIdentifier";
            }
            else if (id >= Constants.ConstStartIndex)
            {
                //it is constant
                type = "ExprConstant";
                child = Integer(lexTable, errors);
            }
            else
            {
                return null;
            }
            
            return new SyntaxTree.Node
            {
                Children = new List<SyntaxTree.Node> { child },
                Type = type,
                Value = null,
            };
        }

        private SyntaxTree.Node VarIdentifier(List<int> lexTable, IList<CompilerError> errors)
        {
            var node = Identifier(lexTable, errors);
            return new SyntaxTree.Node
            {
                Children = new List<SyntaxTree.Node> { node },
                Type = "VariableIdentifier",
                Value = null,
            };
        }

        private SyntaxTree.Node ProcIdentifier(List<int> lexTable, IList<CompilerError> errors)
        {
            var node = Identifier(lexTable, errors);
            return new SyntaxTree.Node
            {
                Children = new List<SyntaxTree.Node> { node },
                Type = "ProcedureIdentifier",
                Value = null,
            };
        }

        private SyntaxTree.Node Identifier(List<int> lexTable, IList<CompilerError> errors)
        {
            var node = Constants.GetLexem(lexTable[0]);

            if (lexTable[0] < Constants.IdentifierStartIndex)
            {
                errors.Add(new CompilerError
                {
                    Line = 0,
                    Position = 0,
                    Message = "Not a identifier:" + node
                });
                return null;
            } 

            lexTable.RemoveAt(0);

            

            return new SyntaxTree.Node
            {
                Children = null,
                Type = "Identifier",
                Value = node,
            };
        }

        private SyntaxTree.Node Integer(List<int> lexTable, IList<CompilerError> errors)
        {
            var node = Constants.GetLexem(lexTable[0]);


            if (lexTable[0] < Constants.ConstStartIndex)
            {
                errors.Add(new CompilerError
                {
                    Line = 0,
                    Position = 0,
                    Message = "Not a identifier:" + node
                });
                return null;
            } 

            lexTable.RemoveAt(0);

            return new SyntaxTree.Node
            {
                Children = null,
                Type = "Integer",
                Value = node
            };
        }
    }
}
