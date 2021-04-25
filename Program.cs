using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ExpressionBooster.Transformers;

namespace ExpressionBooster
{
    static class Program
    {

        static void Main(string[] args)
        {
            var transformers = new List<ExpressionTransformer>
            {
                new ConditionTransformer(),
            };

            var simplifier = new ExpressionSimplifier(transformers);

            Expression<Func<int, int, bool>> expr =
                (a, b) => !!(
                    !!(((a == b || true) ? 4 : 7) == 7) == false
                    && (6 - a == 8 || !(5 - a == 8))
                    && (!(5 - a == 8) || !(6 - a == 8)));

            Console.WriteLine($"Original: {expr}");
            Console.WriteLine($"Simplified: {simplifier.Simplify(expr)}");
        }
    }
}


