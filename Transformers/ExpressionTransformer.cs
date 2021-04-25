using System.Linq.Expressions;

namespace ExpressionBooster.Transformers
{
    public abstract class ExpressionTransformer
    {
        public abstract Expression Transform(Expression expression, ExpressionSimplifier simplifier);
    }
}