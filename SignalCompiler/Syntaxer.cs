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
            return Program(lexTable,errors);
        }

        private SyntaxTree Program(int[] lexTable, IList<CompilerError> errors)
        {
            return null;
        }

        private SyntaxTree Block(int[] lexTable, IList<CompilerError> errors, int pos)
        {
            return null;
        }

        private SyntaxTree Stmtlist(int[] lexTable, IList<CompilerError> errors, int pos)
        {
            return null;
        }

        private SyntaxTree Stmt(int[] lexTable, IList<CompilerError> errors, int pos)
        {
            return null;
        }

        private SyntaxTree CondStmt(int[] lexTable, IList<CompilerError> errors, int pos)
        {
            return null;
        }

        private SyntaxTree ThenStmt(int[] lexTable, IList<CompilerError> errors, int pos)
        {
            return null;
        }

        private SyntaxTree ElseStmt(int[] lexTable, IList<CompilerError> errors, int pos)
        {
            return null;
        }

        private SyntaxTree CondExpr(int[] lexTable, IList<CompilerError> errors, int pos)
        {
            return null;
        }

        private SyntaxTree ComparisonOp(int[] lexTable, IList<CompilerError> errors, int pos)
        {
            return null;
        }

        private SyntaxTree Expression(int[] lexTable, IList<CompilerError> errors, int pos)
        {
            return null;
        }

        private SyntaxTree VarIdentifier(int[] lexTable, IList<CompilerError> errors, int pos)
        {
            return null;
        }

        private SyntaxTree ProcIdentifier(int[] lexTable, IList<CompilerError> errors, int pos)
        {
            return null;
        }

        private SyntaxTree Identifier(int[] lexTable, IList<CompilerError> errors, int pos)
        {
            return null;
        }
    }
}
