using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParserEngine
{
    /// <summary>
    /// The base class for all nodes in our expression tree
    /// </summary>
    abstract class ExpNode
    {
        /// <summary>
        /// A reference to the parent node of this node. Can be null if root node
        /// </summary>
        protected ExpNode parentNode;

        /// <summary>
        /// Constructs a new expression node
        /// </summary>
        /// <param name="parent">The parent node to attach this to</param>
        protected ExpNode(ExpNode parent)
        {
            parentNode = parent;
        }

        /// <summary>
        /// Parses the node-- meaning replaces ExpNodeUnparsed-typed nodes with actual parsed nodes
        /// </summary>
        public virtual void Parse() { }

        /// <summary>
        /// Recursively evaluates the node, and all child nodes, if any
        /// </summary>
        /// <returns>The result of the expression</returns>
        public virtual double Evaluate() { throw new NotImplementedException("Evaluation is not implemented for this node."); }
        
        /// <summary>
        /// Recursively climbs the tree until the root node is reached, where we will have a variable lookup table stored for efficiently looking up variables by name
        /// </summary>
        /// <returns>The lookup table</returns>
        protected Dictionary<string, ExpNodeVariable> GetVariableLookupTable()
        {
            if (this is ExpNodeRootNode)
            {
                return (this as ExpNodeRootNode).VariableLookupTable;
            }
            else
            {
                if (parentNode == null)
                    throw new Exception("Could not find root node.");
                return parentNode.GetVariableLookupTable();
            }
        }
    }
}
