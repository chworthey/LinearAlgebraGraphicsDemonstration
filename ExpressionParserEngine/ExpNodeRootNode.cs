using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParserEngine
{
    /// <summary>
    /// A special node placed at the top of the tree
    /// </summary>
    class ExpNodeRootNode : ExpNode
    {
        /// <summary>
        /// The one and only child node (special since this is the root)
        /// </summary>
        public ExpNode ChildNode { get; set; }

        /// <summary>
        /// A variable lookup table that looks variables up by name
        /// </summary>
        public Dictionary<string, ExpNodeVariable> VariableLookupTable { get; set; }

        /// <summary>
        /// Constructs a new root node. Parent node is null
        /// </summary>
        public ExpNodeRootNode() 
            : base(null) 
        {
            VariableLookupTable = new Dictionary<string, ExpNodeVariable>();
        }

        /// <summary>
        /// Parses the child node, and all of their child nodes, recursively
        /// </summary>
        public override void Parse()
        {
            ChildNode.Parse();
        }

        /// <summary>
        /// Evaluates the expression recursively
        /// </summary>
        /// <returns>The result of the expression</returns>
        public override double Evaluate()
        {
            return ChildNode.Evaluate();
        }
    }
}
