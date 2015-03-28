using System.Collections.Generic;

namespace SignalCompiler.Models
{
    public class SyntaxTree
    {
        public Node RootNode { get; set; }

        public class Node
        {
            public string Type { get; set; }
            public string Value { get; set; }
            public IList<Node> Children { get; set; } 
        }
    }
}
