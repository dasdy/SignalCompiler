using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SignalCompiler.Models
{
    public class SyntaxTree
    {
        public Node RootNode { get; set; }

        public class Node
        {
            public string Type { get; set; }
            public Lexem Value { get; set; }
            public IList<Node> Children { get; set; }

            public override string ToString()
            {
                if (Children != null && Children.Any())
                {
                    var childrenStrBuilder = new StringBuilder();
                    foreach (var child in Children)
                    {
                        childrenStrBuilder.AppendFormat("{0} ", child);
                    }
                    return string.Format("({0} {1})", Type, childrenStrBuilder);
                }

                return string.Format("({0} {1})", Type, Value);
            }
        }

        public override string ToString()
        {
            return string.Format("tree:{0}", RootNode);
        }
    }
}
