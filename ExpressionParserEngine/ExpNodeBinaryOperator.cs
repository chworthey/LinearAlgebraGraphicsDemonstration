using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParserEngine
{
    /// <summary>
    /// A base class for any node that represents a binary operator. Special class in the sense that it has a left and right child node
    /// </summary>
    abstract class ExpNodeBinaryOperator : ExpNode
    {
        /// <summary>
        /// Constructs a new binary operator
        /// </summary>
        /// <param name="parent">The parent node of this node</param>
        protected ExpNodeBinaryOperator(ExpNode parent) : base(parent) { }

        /// <summary>
        /// The left node which represents the expression on the left of the binary operator
        /// </summary>
        public ExpNode LeftNode { get; set; }

        /// <summary>
        /// The right node which represents the expression on the right of the binary operator
        /// </summary>
        public ExpNode RightNode { get; set; }

        /// <summary>
        /// Parses the child nodes
        /// </summary>
        public override void Parse()
        {
            LeftNode.Parse();
            RightNode.Parse();
        }
    }
}
