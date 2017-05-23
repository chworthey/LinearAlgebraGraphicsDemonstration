using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionParserEngine
{
    class ExpNodeDivisionOperator : ExpNodeBinaryOperator
    {
        public ExpNodeDivisionOperator(ExpNode parent) : base(parent) { }
        public override double Evaluate()
        {
            return LeftNode.Evaluate() / RightNode.Evaluate();
        }
    }
}
