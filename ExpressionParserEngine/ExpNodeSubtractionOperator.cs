using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParserEngine
{
    class ExpNodeSubtractionOperator : ExpNodeBinaryOperator
    {
        public ExpNodeSubtractionOperator(ExpNode parent) : base(parent) { }
        public override double Evaluate()
        {
            return LeftNode.Evaluate() - RightNode.Evaluate();
        }
    }
}
