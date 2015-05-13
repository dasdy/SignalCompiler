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

        private void Feed(ProgramNode node, IList<string> listing, IList<CompilerError> errors)
        {
            listing.Add(".386");
            listing.Add("DATA SEGMENT USE16");

            int curIndex = Constants.IdentifierStartIndex + 2;
            string id = Constants.GetLexem(curIndex);
            while (id != null)
            {
                listing.Add(string.Format("{0} db 0", id));
                curIndex++;
                id = Constants.GetLexem(curIndex);
            }

            listing.Add("DATA ENDS");
            Feed(node.Block, listing, errors);
        }

        private void Feed(Block node, IList<string> listing, IList<CompilerError> errors)
        {
            listing.Add("CODE SEGMENT USE16");
            listing.Add("ASSUME CS:CODE, DS:DATA");
            listing.Add("BEGIN:");


            Feed(node.StmtList, listing, errors);


            listing.Add("CODE ENDS");
            listing.Add("END BEGIN");
        }

        private void Feed(StmtList node, IList<string> listing, IList<CompilerError> errors)
        {
            if (node.Children != null && node.Children.Any())
            {
                foreach (var child in node.Children)
                {
                    listing.Add("; new stmt");
                    Feed(child as Stmt, listing, errors);
                }
            }
        }

        private void Feed(Stmt node, IList<string> listing, IList<CompilerError> errors)
        {
            if (node.TheStmt is WhileStmt)
                Feed((WhileStmt) node.TheStmt, listing, errors);
            else
                Feed((IfStmt) node.TheStmt, listing, errors);
        }

        private void Feed(WhileStmt node, IList<string> listing, IList<CompilerError> errors)
        {
            bool? compRes = node.Condition.Evaluate();

            if (compRes == null)
            {
                string begCycleLbl = GetNewLabel();
                string endCycleLbl = GetNewLabel();

                listing.Add(string.Format("{0}: ", begCycleLbl));
                Feed(node.Condition, listing, errors, endCycleLbl);


                Feed(node.Body, listing, errors);

                listing.Add(string.Format("jmp {0}", begCycleLbl));
                listing.Add(string.Format("{0}:", endCycleLbl));
            }
            else
            {
                if ((bool)compRes)
                {
                    string lbl = GetNewLabel();
                    listing.Add(string.Format("{0}:", lbl));

                    Feed(node.Body, listing, errors);

                    listing.Add(string.Format("jmp {0}", lbl));
                }
            }
        }

        private void Feed(IfStmt node, IList<string> listing, IList<CompilerError> errors)
        {
            bool? compRes = node.Condition.Evaluate();
            if (compRes == null)
            {
                string endifLabel = GetNewLabel();
                if (node.ElseStmt != null)
                {
                    string elseLabel = GetNewLabel();

                    Feed(node.Condition, listing, errors, elseLabel);
                    Feed(node.ThenStmt, listing, errors);

                    listing.Add(string.Format("jmp {0}", endifLabel));
                    listing.Add(string.Format("{0}:", elseLabel));

                    Feed(node.ElseStmt, listing, errors);

                    listing.Add(string.Format("{0}:", endifLabel));
                }
                else
                {

                    Feed(node.Condition, listing, errors, endifLabel);
                    Feed(node.ThenStmt, listing, errors);

                    listing.Add(string.Format("{0}:", endifLabel));
                }
            }
            else
            {
                if ((bool)compRes)
                {
                    Feed(node.ThenStmt, listing, errors);
                }
                else
                {
                    Feed(node.ElseStmt, listing, errors);
                }
            }
        }

        private void Feed(CondExpr node, IList<string> listing, IList<CompilerError> errors, string label)
        {
            var jump = CondToJump(Constants.GetLexem(node.Children[1].Value.Id));
            listing.Add(string.Format("cmp {0}, {1}", ((Expression)node.Children[0]).Val, ((Expression)node.Children[2]).Val));
            listing.Add(string.Format("{0} {1}", jump, label));
        }


        private void Feed(IfThen node, IList<string> listing, IList<CompilerError> errors)
        {
            Feed(node.StmtList, listing, errors);
        }

        private void Feed(IfElse node, IList<string> listing, IList<CompilerError> errors)
        {
            Feed(node.StmtList, listing, errors);
        }

        private void Feed(SyntaxTree.Node node, IList<string> listing, IList<CompilerError> errors)
        {
            throw new Exception("wtf is this?" + node.GetType());
        }

        private static string CondToJump(string condOp)
        {
            switch (condOp)
            {
                case ">":
                    return "jng";
                case "<":
                    return "jnl";
                case ">=":
                    return "jnge";
                case "<=":
                    return "jnle";
                case "=":
                    return "jne";
                case "<>":
                    return "je";
                default: throw new Exception("unknown compareOp: " + condOp);
            }
        }
    }
}
