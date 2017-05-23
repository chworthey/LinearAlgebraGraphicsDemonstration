using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParserEngine
{
    /// <summary>
    /// An expression node that stores a constant value
    /// </summary>
    class ExpNodeConstant : ExpNode
    {
        /// <summary>
        /// Gets the value
        /// </summary>
        public double Value { get; private set; }

        /// <summary>
        /// Constructs a new expression node constant
        /// </summary>
        /// <param name="parent">The parent node of this node</param>
        /// <param name="value">The value this node should be</param>
        public ExpNodeConstant(ExpNode parent, double value)
            : base(parent)
        {
            Value = value;
        }

        /// <summary>
        /// The evaluation of this node is simply the value given
        /// </summary>
        /// <returns>The value</returns>
        public override double Evaluate()
        {
            return Value;
        }
    }
}
