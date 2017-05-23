using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParserEngine
{
    /// <summary>
    /// A node that represents an unknown. It holds an unparsed expression, but has the ability to replace itself with actual nodes
    /// </summary>
    class ExpNodeUnparsed : ExpNode
    {
        string text;
        const string parseError = "Parse error.";

        /// <summary>
        /// Constructs a new expression node unparsed.
        /// </summary>
        /// <param name="parent">The parent node of this node</param>
        /// <param name="text">Any expression text to be parsed</param>
        public ExpNodeUnparsed(ExpNode parent, string text)
            : base(parent)
        {
            this.text = text;
        }

        /// <summary>
        /// Parses the expression. Replaces this node with another node type that is parsed
        /// </summary>
        public override void Parse()
        {
            text = text.Replace(" ", "");

            if (text.Length == 0) // this will be a good spot to implement negation in the future
                throw new Exception(parseError);

            // Try parsing this as a binary operator
            bool wasUnary;

            if (tryParseBinaryOperator(new ExpNodeAdditionOperator(parentNode), '+', true, text, out wasUnary))
                return;

            if (tryParseBinaryOperator(new ExpNodeSubtractionOperator(parentNode), '-', true, text, out wasUnary))
                return;

            if (wasUnary)
            {
                ExpNodeMultiplicationOperator op = new ExpNodeMultiplicationOperator(parentNode);
                op.LeftNode = new ExpNodeConstant(op, -1.0);
                op.RightNode = new ExpNodeUnparsed(op, text.Substring(1));
                replaceThisNode(op);
                op.Parse();
                return;
            }

            if (tryParseBinaryOperator(new ExpNodeMultiplicationOperator(parentNode), '*', true, text, out wasUnary))
                return;

            if (tryParseBinaryOperator(new ExpNodeDivisionOperator(parentNode), '/', true, text, out wasUnary))
                return;

            if (char.IsDigit(text[0])) // Try parsing as constant...
            {
                double result;
                if (double.TryParse(text, out result))
                {
                    replaceThisNode(new ExpNodeConstant(parentNode, result));
                }
                else
                    throw new Exception(parseError);
            }
            else // Parse as variable...
            {
                ExpNodeVariable variable = new ExpNodeVariable(parentNode, text);
                replaceThisNode(variable);
                GetVariableLookupTable()[text] = variable; // Store variable in lookup table
            }
        }
        
        /// <summary>
        /// A helper function which automates trying to parse for a binary operator
        /// </summary>
        /// <param name="node">Insert the correct type of binary operator node here</param>
        /// <param name="character">Insert the character associated with that binary operator node</param>
        /// <param name="evaluateBackwards">Mark true if you want to seach the text from the end, rather than the beginning</param>
        /// <returns>True if parse was successful</returns>
        bool tryParseBinaryOperator(ExpNodeBinaryOperator node, char character, bool evaluateBackwards, string parseText, out bool wasUnaryOperator)
        {
            wasUnaryOperator = false;
            if (parseText.Contains(character))
            {
                int index = evaluateBackwards ? indexOfFromEnd(parseText, character) : indexFromBeginning(parseText, character);

                // Is this actually a unary operation?
                if (index == parseText.Length - 1)
                    return false;

                if (character == '-')
                {
                    int count = 1;
                    while (true)
                    {
                        index = evaluateBackwards ? indexOfFromEnd(parseText, character, count++) : indexFromBeginning(parseText, character, count++);

                        if (index == 0 && character == '-')
                        {
                            wasUnaryOperator = true;
                            return false;
                        }

                        if (index == -1)
                        {
                            return false;
                        }

                        if (index != 0 && !(parseText[index - 1] == '+' ||
                            parseText[index - 1] == '-' ||
                            parseText[index - 1] == '*' ||
                            parseText[index - 1] == '/'))
                        {
                            break;
                        }
                    }
                }

                node.LeftNode = new ExpNodeUnparsed(node, parseText.Substring(0, index));
                node.RightNode = new ExpNodeUnparsed(node, parseText.Substring(index + 1));
                replaceThisNode(node);
                node.Parse();

                return true;
            }

            return false;
        }

        int indexFromBeginning(string str, char c, int nChar = 1)
        {
            int count = 0;
            for (int n = 0; n < str.Length; n++)
            {
                if (str[n] == c)
                {
                    count++;
                    if (count >= nChar)
                        return n;
                }
            }
            return -1;
        }

        /// <summary>
        /// Like string.IndexOf(), but starts searching from the end rather than the beginning
        /// </summary>
        /// <param name="str">String to search</param>
        /// <param name="c">Character to search for</param>
        /// <returns>The index of the first character sighted. Or -1 if not sighted</returns>
        int indexOfFromEnd(string str, char c, int nChar=1)
        {
            int count = 0;
            for (int n = str.Length - 1; n >= 0; n--)
            {
                if (str[n] == c)
                {
                    count++;
                    if (count >= nChar)
                        return n;
                }
            }
            return -1;
        }

        /// <summary>
        /// Replaces this node in the tree with another node
        /// </summary>
        /// <param name="replacementNode">The node to replace it with</param>
        void replaceThisNode(ExpNode replacementNode)
        {
            if (parentNode is ExpNodeRootNode)
                (parentNode as ExpNodeRootNode).ChildNode = replacementNode;
            else if (parentNode is ExpNodeBinaryOperator)
            {
                ExpNodeBinaryOperator node = parentNode as ExpNodeBinaryOperator;
                if (node.LeftNode == this)
                    node.LeftNode = replacementNode;
                else
                    node.RightNode = replacementNode;
            }
            else
                throw new InvalidOperationException("Cannot replace node because parent node cannot have child.");
        }

        /// <summary>
        /// Will throw exception upon call since unparsed expressions cannot be evaluated
        /// </summary>
        /// <returns>Will not return</returns>
        public override double Evaluate()
        {
            throw new InvalidOperationException("Cannot evaluate an expression that is unparsed.");
        }
    }
}
