using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParserEngine
{
    /// <summary>
    /// A class that represents a mathematical expression
    /// </summary>
    public class Expression
    {
        /// <summary>
        /// The text as originally entered upon construction
        /// </summary>
        public string Text { get; private set; }
        ExpNodeRootNode rootNode;

        /// <summary>
        /// Constructs a new expression, and parses that expression. Will throw exception on parse failure
        /// </summary>
        /// <param name="text">The text to parse into the engine's expression tree</param>
        public Expression(string text)
        {
            Text = text;
            rootNode = new ExpNodeRootNode();
            rootNode.ChildNode = new ExpNodeUnparsed(rootNode, text);
            rootNode.Parse();
        }

        /// <summary>
        /// Set variable of the given name to the given value
        /// </summary>
        /// <param name="name">Must be a variable name given in the text. Will throw exception if it does not exist</param>
        /// <param name="value">The value to set the variable to</param>
        public void SetVariable(string name, double value)
        {
            if (!rootNode.VariableLookupTable.ContainsKey(name))
                throw new Exception("Variable does not exist.");

            rootNode.VariableLookupTable[name].Value = value;
        }

        /// <summary>
        /// Looks at the parsed expression, and gathers its actual value. Will throw exception on failure
        /// </summary>
        /// <returns>The result of the expression</returns>
        public double Evaluate()
        {
            return rootNode.Evaluate();
        }
    }
}
