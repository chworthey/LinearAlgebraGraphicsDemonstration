using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParserEngine
{
    /// <summary>
    /// An expression node that stores a variable
    /// </summary>
    class ExpNodeVariable : ExpNode
    {
        /// <summary>
        /// The name of the variable
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Whether or not the variable has a value. False until the Value is set
        /// </summary>
        public bool HasValue { get; private set; }

        /// <summary>
        /// The value, if set, of the variable
        /// </summary>
        public double Value
        {
            get
            {
                return value;
            }
            set
            {
                HasValue = true;
                this.value = value;
            }
        }
        double value;

        /// <summary>
        /// Constructs a new expression node variable
        /// </summary>
        /// <param name="parent">The parent node of this node</param>
        /// <param name="variableName">The name of the variable</param>
        public ExpNodeVariable(ExpNode parent, string variableName)
            : base(parent)
        {
            Name = variableName;
        }

        /// <summary>
        /// Gets the value, but throws an exception if the value hasn't been set
        /// </summary>
        /// <returns>The value if successful</returns>
        public override double Evaluate()
        {
            if (!HasValue)
                throw new Exception("Could not evaluate. Variable: " + Name + " has not been set to a value.");

            return Value;
        }
    }
}
