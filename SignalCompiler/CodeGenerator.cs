using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignalCompiler.Models;

namespace SignalCompiler
{
    public class CodeGenerator
    {
        private int _labelsAmount;

        private string GetNewLabel()
        {
            var str = string.Format("?lbl{0}", _labelsAmount);
            _labelsAmount++;
            return str;
        }
        public string Feed(SyntaxTree tree, IList<CompilerError> errors)
        {
            var lines = new List<string>();
            _labelsAmount = 0;
            Feed(tree.RootNode, lines, errors);
            return string.Join("\n", lines);
        }

        private void Feed(Program node, IList<string> listing, IList<CompilerError> errors)
        {
            
        }

        private void Feed(Block node, IList<string> listing, IList<CompilerError> errors)
        {

        }

        private void Feed(StmtList node, IList<string> listing, IList<CompilerError> errors)
        {

        }

        private void Feed(Stmt node, IList<string> listing, IList<CompilerError> errors)
        {

        }

        private void Feed(WhileStmt node, IList<string> listing, IList<CompilerError> errors)
        {

        }

        private void Feed(IfStmt node, IList<string> listing, IList<CompilerError> errors)
        {
            bool? compRes = node.Condition.Evaluate();
            if (compRes == null)
            {

            }
            else
            {
                if ((bool) compRes)
                {

                }
                else
                {
                    
                }
            }
        }

        private void Feed(IfThen node, IList<string> listing, IList<CompilerError> errors)
        {

        }

        private void Feed(IfElse node, IList<string> listing, IList<CompilerError> errors)
        {

        }

        private void Feed(SyntaxTree.Node node, IList<string> listing, IList<CompilerError> errors)
        {
            throw new Exception("wtf is this?" + node.GetType());
        }

    }
}
