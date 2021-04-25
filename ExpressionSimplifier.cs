using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using ExpressionBooster.Transformers;

namespace ExpressionBooster
{
    public sealed class ExpressionSimplifier
    {
        private readonly ICollection<ExpressionTransformer> transformers;

        public ExpressionSimplifier(ICollection<ExpressionTransformer> transformers)
        {
            this.transformers = transformers;
        }

        [return: NotNullIfNotNull("expressions")]
        private ICollection<T>? Simplify<T>(ICollection<T>? expressions) where T : Expression
        {
            if (expressions is null) return null;
            var transformed = new List<T>();
            foreach (var expression in expressions)
            {
                transformed.Add((T)Simplify(expression));
            }

            return transformed;
        }

        private ICollection<CatchBlock> Simplify(ICollection<CatchBlock> catchBlocks)
        {
            var transformed = new List<CatchBlock>();
            foreach (var block in catchBlocks)
            {
                transformed.Add(block.Update(Simplify(block.Variable), Simplify(block.Filter), Simplify(block.Body)));
            }

            return transformed;
        }

        private ICollection<SwitchCase> Simplify(ICollection<SwitchCase> switchCases)
        {
            var transformed = new List<SwitchCase>();
            foreach (var block in switchCases)
            {
                transformed.Add(block.Update(Simplify(block.TestValues), Simplify(block.Body)));
            }

            return transformed;
        }

        private ICollection<ElementInit> Simplify(ICollection<ElementInit> elemInits)
        {
            var transformed = new List<ElementInit>();
            foreach (var block in elemInits)
            {
                transformed.Add(block.Update(Simplify(block.Arguments)));
            }

            return transformed;
        }


        [return: NotNullIfNotNull("expression")]
        public T? Simplify<T>(T? expression) where T : Expression
        {
            if (expression is null) return null;
            Expression expr = expression.Reduce() switch
            {
                UnaryExpression unaryExpr => unaryExpr.Update(Simplify(unaryExpr.Operand)),
                BinaryExpression binaryExpr => binaryExpr.Update(Simplify(binaryExpr.Left), binaryExpr.Conversion, Simplify(binaryExpr.Right)),
                LambdaExpression lambdaExpr => Expression.Lambda(Simplify(lambdaExpr.Body), lambdaExpr.Name, lambdaExpr.TailCall, Simplify(lambdaExpr.Parameters)),
                TryExpression tryExpr => tryExpr.Update(Simplify(tryExpr.Body), Simplify(tryExpr.Handlers), Simplify(tryExpr.Finally), Simplify(tryExpr.Fault)),
                NewExpression newExpr => newExpr.Update(Simplify(newExpr.Arguments)),
                GotoExpression gotoExpr => gotoExpr.Update(gotoExpr.Target,Simplify(gotoExpr.Value)),
                LoopExpression loopExpr => loopExpr.Update(loopExpr.BreakLabel, loopExpr.ContinueLabel, Simplify(loopExpr.Body)),
                BlockExpression blockExpr => blockExpr.Update(Simplify(blockExpr.Variables), Simplify(blockExpr.Expressions)),
                IndexExpression indexExpr => indexExpr.Update(Simplify(indexExpr.Object)!, Simplify(indexExpr.Arguments)),
                LabelExpression labelExpr => labelExpr.Update(labelExpr.Target, Simplify(labelExpr.DefaultValue)),
                MemberExpression memberExpr => memberExpr.Update(Simplify(memberExpr.Expression)),
                SwitchExpression switchExpr => switchExpr.Update(Simplify(switchExpr.SwitchValue), Simplify(switchExpr.Cases), Simplify(switchExpr.DefaultBody)),
                DynamicExpression dynamicExpr => dynamicExpr.Update(Simplify(dynamicExpr.Arguments)),
                ListInitExpression listInitExpr => listInitExpr.Update(Simplify(listInitExpr.NewExpression), Simplify(listInitExpr.Initializers)),
                NewArrayExpression newArrayExpr => newArrayExpr.Update(Simplify(newArrayExpr.Expressions)),
                InvocationExpression invokeExpr => invokeExpr.Update(Simplify(invokeExpr.Expression), Simplify(invokeExpr.Arguments)),
                MemberInitExpression memberInitExpr => memberInitExpr.Update(Simplify(memberInitExpr.NewExpression), memberInitExpr.Bindings),
                MethodCallExpression methodCallExpr => methodCallExpr.Update(Simplify(methodCallExpr.Object), Simplify(methodCallExpr.Arguments)),
                TypeBinaryExpression typeBinaryExpr => typeBinaryExpr.Update(Simplify(typeBinaryExpr.Expression)),
                ConditionalExpression condExpr => condExpr.Update(Simplify(condExpr.Test), Simplify(condExpr.IfTrue), Simplify(condExpr.IfFalse)),
                RuntimeVariablesExpression runtimeVarExpr => runtimeVarExpr.Update(Simplify(runtimeVarExpr.Variables)),
                _ => expression
            };

            foreach (var transform in transformers)
            {
                expr = transform.Transform(expr, this);
            }

            return (T)expr;
        }

        public bool IsEquivalent(Expression left, Expression right)
        {
            var simplifiedLeft = Simplify(left);
            var simplifiedRight = Simplify(right);

            return simplifiedLeft.ToString() == simplifiedRight.ToString();
        }
    }
}