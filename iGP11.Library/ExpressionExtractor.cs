using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace iGP11.Library
{
    public class ExpressionExtractor : ExpressionVisitor
    {
        private readonly ICollection<Expression> _expressions = new List<Expression>();

        public bool EnsureUniqueness { get; set; }

        public IEnumerable<Expression> Extract(Expression expression)
        {
            _expressions.Clear();
            Visit(expression);

            return _expressions.ToArray();
        }

        public override Expression Visit(Expression node)
        {
            if (!EnsureUniqueness || !_expressions.Contains(node))
            {
                _expressions.Add(node);
            }

            return base.Visit(node);
        }
    }
}
