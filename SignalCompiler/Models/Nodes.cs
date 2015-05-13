using System.Linq;

namespace SignalCompiler.Models
{
    public class ProgramNode : SyntaxTree.Node
    {
        public ProcIdentifier ProcIdentifier { get { return Children[0] as ProcIdentifier; } }
        public Block Block { get { return Children[1] as Block; } }
    }

    public class Block : SyntaxTree.Node
    {
        public StmtList StmtList
        {
            get
            {
                return Children.Any() 
                    ? Children[0] as StmtList
                    : null;
            }
        }
    }
    public class StmtList : SyntaxTree.Node { }

    public class Stmt : SyntaxTree.Node
    {
        public SyntaxTree.Node TheStmt { get { return Children[0]; } }
    }

    public class WhileStmt : SyntaxTree.Node
    {
        public CondExpr Condition { get { return Children[0] as CondExpr; } }

        public StmtList Body
        {
            get
            {
                return Children.Count > 1
                    ? Children[1] as StmtList
                    : null;
            }
        }
    }

    public class IfStmt : SyntaxTree.Node
    {
        public CondExpr Condition { get { return ThenStmt.Condition; } }
        public IfThen ThenStmt { get { return Children[0] as IfThen; } }
        public IfElse ElseStmt
        {
            get
            {
                return (Children.Count > 1)
                    ? Children[1] as IfElse
                    : null;
            }
        }
    }

    public class IfThen : SyntaxTree.Node
    {
        public CondExpr Condition { get { return Children[0] as CondExpr; } }

        public StmtList StmtList
        {
            get
            {
                return (Children.Count > 1)
                    ? Children[1] as StmtList
                    : null;
            }
        }
    }

    public class IfElse : SyntaxTree.Node
    {
        public StmtList StmtList
        {
            get
            {
                return (Children.Any())
                    ? Children[0] as StmtList
                    : null;
            }
        }
    }

    public class CondExpr : SyntaxTree.Node
    {
        private bool IsEvaluatable { get { return Children[0].Children[0] is Integer && Children[2].Children[0] is Integer; } }

        public bool? Evaluate()
        {
            if (!IsEvaluatable) return null;
            var firstInt = ((Integer)Children[0].Children[0]).Val;
            var secondInt = ((Integer)Children[2].Children[0]).Val;
            var compOp = ((ComparisonOp)Children[1]).Val;
            switch (compOp)
            {
                case "<":
                    return firstInt < secondInt;
                case ">":
                    return firstInt > secondInt;
                case "<=":
                    return firstInt <= secondInt;
                case ">=":
                    return firstInt >= secondInt;
                case "<>":
                    return firstInt != secondInt;
                case "=":
                    return firstInt == secondInt;
                default:
                    return null;
            }
        }
    }

    public class ComparisonOp : SyntaxTree.Node
    {
        public string Val { get { return Constants.GetLexem(Value.Id); } }
    }

    public class Expression : SyntaxTree.Node
    {
        public string Val
        {
            get
            {
                var exprVarIdent = Children[0] as VarIdentifier;
                if (exprVarIdent != null)
                    return exprVarIdent.Id;

                var exprInt = Children[0] as Integer;
                if (exprInt != null)
                    return exprInt.Val.ToString();

                return null;
            }
        }
    }

    public class VarIdentifier : SyntaxTree.Node
    {
        public string Id { get { return ((Identifier)Children[0]).Id; } }
    }

    public class ProcIdentifier : SyntaxTree.Node
    {
        public string Id { get { return ((Identifier)Children[0]).Id; } }
    }

    public class Identifier : SyntaxTree.Node
    {
        public string Id { get { return Constants.GetLexem(Value.Id); } }
    }

    public class Integer : SyntaxTree.Node
    {
        public int Val { get { return int.Parse(Constants.GetLexem(Value.Id)); } }
    }
}
