using System;
using System.Linq.Expressions;

namespace ExpressionBooster.Transformers
{
    public class ConditionTransformer : ExpressionTransformer
    {
        private static Expression TransformDoubleNot(Expression expression)
        {
            if (expression is UnaryExpression
                {
                    NodeType: ExpressionType.Not,
                    Operand: UnaryExpression
                    {
                        NodeType: ExpressionType.Not,
                        Operand: var innerExpression
                    }
                })
            {
                return innerExpression;
            }

            return expression;
        }

        private static Expression TransformEquivalentConditionArm(Expression expression, ExpressionSimplifier simplifier)
        {
            {
                // a && a, a || a
                if (expression is BinaryExpression
                    {
                        Left: var left,
                        Right: var right,
                        NodeType: ExpressionType.AndAlso or ExpressionType.OrElse
                    } && simplifier.IsEquivalent(left, right))
                {
                    return left;
                }
            }

            {
                // a || (a && b)
                if (expression is BinaryExpression
                    {
                        Left: var expr,
                        Right: BinaryExpression
                        {
                            Left: var innerExpr,
                            NodeType: ExpressionType.AndAlso
                        },
                        NodeType: ExpressionType.OrElse
                    } && simplifier.IsEquivalent(expr, innerExpr))
                {
                    return expr;
                }
            }

            {
                // (a && b) || a
                if (expression is BinaryExpression
                    {
                        Right: var expr,
                        Left: BinaryExpression
                        {
                            Left: var innerExpr,
                            NodeType: ExpressionType.AndAlso
                        },
                        NodeType: ExpressionType.OrElse
                    } && simplifier.IsEquivalent(expr, innerExpr))
                {
                    return expr;
                }
            }

            {
                // a || (b && a)
                if (expression is BinaryExpression
                    {
                        Left: var expr,
                        Right: BinaryExpression
                        {
                            Right: var innerExpr,
                            NodeType: ExpressionType.AndAlso
                        },
                        NodeType: ExpressionType.OrElse
                    } && simplifier.IsEquivalent(expr, innerExpr))
                {
                    return expr;
                }
            }

            {
                // (b && a) || a
                if (expression is BinaryExpression
                    {
                        Right: var expr,
                        Left: BinaryExpression
                        {
                            Right: var innerExpr,
                            NodeType: ExpressionType.AndAlso
                        },
                        NodeType: ExpressionType.OrElse
                    } && simplifier.IsEquivalent(expr, innerExpr))
                {
                    return expr;
                }
            }

            {
                // a && (a || b)
                if (expression is BinaryExpression
                    {
                        Left: var expr,
                        Right: BinaryExpression
                        {
                            Left: var innerExpr,
                            NodeType: ExpressionType.OrElse
                        },
                        NodeType: ExpressionType.AndAlso
                    } && simplifier.IsEquivalent(expr, innerExpr))
                {
                    return expr;
                }
            }

            {
                // (a || b) && a
                if (expression is BinaryExpression
                    {
                        Right: var expr,
                        Left: BinaryExpression
                        {
                            Left: var innerExpr,
                            NodeType: ExpressionType.OrElse
                        },
                        NodeType: ExpressionType.AndAlso
                    } && simplifier.IsEquivalent(expr, innerExpr))
                {
                    return expr;
                }
            }

            {
                // a && (b || a)
                if (expression is BinaryExpression
                    {
                        Left: var expr,
                        Right: BinaryExpression
                        {
                            Right: var innerExpr,
                            NodeType: ExpressionType.OrElse
                        },
                        NodeType: ExpressionType.AndAlso
                    } && simplifier.IsEquivalent(expr, innerExpr))
                {
                    return expr;
                }
            }

            {
                // (b || a) && a
                if (expression is BinaryExpression
                    {
                        Right: var expr,
                        Left: BinaryExpression
                        {
                            Right: var innerExpr,
                            NodeType: ExpressionType.OrElse
                        },
                        NodeType: ExpressionType.AndAlso
                    } && simplifier.IsEquivalent(expr, innerExpr))
                {
                    return expr;
                }
            }

            {
                if (expression is BinaryExpression
                    {
                        Right: BinaryExpression
                        {
                            Left: var leftLeft,
                            Right: var leftRight
                        },
                        Left: BinaryExpression
                        {
                            Left: var rightLeft,
                            Right: var rightRight
                        },
                        NodeType: ExpressionType.AndAlso
                    })
                {
                    {
                        // (!a || b) && (!a || !b)
                        if (leftLeft is UnaryExpression
                            { NodeType: ExpressionType.Not, Operand: var rLeftLeft }
                            && rightLeft is UnaryExpression
                            { NodeType: ExpressionType.Not, Operand: var rRightLeft }
                            && rightRight is UnaryExpression
                            { NodeType: ExpressionType.Not, Operand: var rRightRight }
                            && simplifier.IsEquivalent(rLeftLeft, rRightLeft)
                            && simplifier.IsEquivalent(leftRight, rRightRight))
                        {
                            return leftLeft;
                        }
                    }

                    {
                        // (!a || b) && (!b || !a)
                        if (leftLeft is UnaryExpression
                            { NodeType: ExpressionType.Not, Operand: var rLeftLeft }
                            && rightLeft is UnaryExpression
                            { NodeType: ExpressionType.Not, Operand: var rRightLeft }
                            && rightRight is UnaryExpression
                            { NodeType: ExpressionType.Not, Operand: var rRightRight }
                            && simplifier.IsEquivalent(rLeftLeft, rRightRight)
                            && simplifier.IsEquivalent(leftRight, rRightLeft))
                        {
                            return leftLeft;
                        }
                    }

                    {
                        // (b || !a) && (!a || !b)
                        if (leftRight is UnaryExpression
                            { NodeType: ExpressionType.Not, Operand: var rLeftRight }
                            && rightLeft is UnaryExpression
                            { NodeType: ExpressionType.Not, Operand: var rRightLeft }
                            && rightRight is UnaryExpression
                            { NodeType: ExpressionType.Not, Operand: var rRightRight }
                            && simplifier.IsEquivalent(rLeftRight, rRightLeft)
                            && simplifier.IsEquivalent(leftLeft, rRightRight))
                        {
                            return leftLeft;
                        }
                    }

                    {
                        // (b || !a) && (!b || !a)
                        if (leftRight is UnaryExpression
                            { NodeType: ExpressionType.Not, Operand: var rLeftRight }
                            && rightLeft is UnaryExpression
                            { NodeType: ExpressionType.Not, Operand: var rRightLeft }
                            && rightRight is UnaryExpression
                            { NodeType: ExpressionType.Not, Operand: var rRightRight }
                            && simplifier.IsEquivalent(leftLeft, rRightLeft)
                            && simplifier.IsEquivalent(rLeftRight, rRightRight))
                        {
                            return leftLeft;
                        }
                    }
                }
            }

            {
                if (expression is BinaryExpression
                    {
                        Left: BinaryExpression
                        {
                            Left: var leftLeft,
                            Right: var leftRight
                        },
                        Right: BinaryExpression
                        {
                            Left: var rightLeft,
                            Right: var rightRight
                        },
                        NodeType: ExpressionType.AndAlso
                    })
                {
                    {
                        // (!a || !b) && (!a || b)
                        if (leftLeft is UnaryExpression
                            { NodeType: ExpressionType.Not, Operand: var rLeftLeft }
                            && rightLeft is UnaryExpression
                            { NodeType: ExpressionType.Not, Operand: var rRightLeft }
                            && rightRight is UnaryExpression
                            { NodeType: ExpressionType.Not, Operand: var rRightRight }
                            && simplifier.IsEquivalent(rLeftLeft, rRightLeft)
                            && simplifier.IsEquivalent(leftRight, rRightRight))
                        {
                            return leftLeft;
                        }
                    }

                    {
                        // (!b || !a) && (!a || b)
                        if (leftLeft is UnaryExpression
                            { NodeType: ExpressionType.Not, Operand: var rLeftLeft }
                            && rightLeft is UnaryExpression
                            { NodeType: ExpressionType.Not, Operand: var rRightLeft }
                            && rightRight is UnaryExpression
                            { NodeType: ExpressionType.Not, Operand: var rRightRight }
                            && simplifier.IsEquivalent(rLeftLeft, rRightRight)
                            && simplifier.IsEquivalent(leftRight, rRightLeft))
                        {
                            return leftLeft;
                        }
                    }

                    {
                        // (!a || !b) && (b || !a)
                        if (leftRight is UnaryExpression
                            { NodeType: ExpressionType.Not, Operand: var rLeftRight }
                            && rightLeft is UnaryExpression
                            { NodeType: ExpressionType.Not, Operand: var rRightLeft }
                            && rightRight is UnaryExpression
                            { NodeType: ExpressionType.Not, Operand: var rRightRight }
                            && simplifier.IsEquivalent(rLeftRight, rRightLeft)
                            && simplifier.IsEquivalent(leftLeft, rRightRight))
                        {
                            return leftLeft;
                        }
                    }

                    {
                        // (!b || !a) && (b || !a)
                        if (leftRight is UnaryExpression
                            { NodeType: ExpressionType.Not, Operand: var rLeftRight }
                            && rightLeft is UnaryExpression
                            { NodeType: ExpressionType.Not, Operand: var rRightLeft }
                            && rightRight is UnaryExpression
                            { NodeType: ExpressionType.Not, Operand: var rRightRight }
                            && simplifier.IsEquivalent(leftLeft, rRightLeft)
                            && simplifier.IsEquivalent(rLeftRight, rRightRight))
                        {
                            return leftLeft;
                        }
                    }
                }
            }

            return expression;
        }

        private static Expression TransformConstCondition(Expression expression, ExpressionSimplifier simplifier)
        {
            // const bool ? a : b
            if (expression is ConditionalExpression
                {
                    Test: ConstantExpression
                    {
                        Value: bool testValue
                    },
                    IfFalse: var falseCase,
                    IfTrue: var trueCase
                })
            {
                return testValue ? trueCase : falseCase;
            }

            // !const bool
            if (expression is UnaryExpression
                {
                    NodeType: ExpressionType.Not,
                    Operand: ConstantExpression { Value: bool value }
                })
            {
                return Expression.Constant(!value);
            }

            if (expression is BinaryExpression
                {
                    Left: ConstantExpression { Value: var left },
                    Right: ConstantExpression { Value: var right },
                    NodeType: var type
                })
            {
                switch (type)
                {
                    // const bool == const bool
                    case ExpressionType.Equal:
                        return Expression.Constant((left == null && right == null) || (left != null && left.Equals(right)));
                    // const bool != const bool
                    case ExpressionType.NotEqual when left is not null && right is not null:
                        return Expression.Constant(!left.Equals(right));
                    // const bool && const bool
                    case ExpressionType.AndAlso when left is bool lb && right is bool rb:
                        return Expression.Constant(lb == rb);
                    // const bool || const bool
                    case ExpressionType.OrElse when left is bool lb && right is bool rb:
                        return Expression.Constant(lb || rb);
                }
            }

            {
                // a && !a
                if (expression is BinaryExpression
                    {
                        Left: var expr,
                        Right: UnaryExpression
                        {
                            NodeType: ExpressionType.Not,
                            Operand: var innerExpr
                        },
                        NodeType: ExpressionType.AndAlso
                    } && simplifier.IsEquivalent(expr, innerExpr))
                {
                    return Expression.Constant(false);
                }
            }

            {
                // !a && a
                if (expression is BinaryExpression
                    {
                        Right: var expr,
                        Left: UnaryExpression
                        {
                            NodeType: ExpressionType.Not,
                            Operand: var innerExpr
                        },
                        NodeType: ExpressionType.AndAlso
                    } && simplifier.IsEquivalent(expr, innerExpr))
                {
                    return Expression.Constant(false);
                }
            }

            {
                // a || !a
                if (expression is BinaryExpression
                    {
                        Left: var expr,
                        Right: UnaryExpression
                        {
                            NodeType: ExpressionType.Not,
                            Operand: var innerExpr
                        },
                        NodeType: ExpressionType.OrElse
                    } && simplifier.IsEquivalent(expr, innerExpr))
                {
                    return Expression.Constant(true);
                }
            }

            {
                // !a || a
                if (expression is BinaryExpression
                    {
                        Right: var expr,
                        Left: UnaryExpression
                        {
                            NodeType: ExpressionType.Not,
                            Operand: var innerExpr
                        },
                        NodeType: ExpressionType.OrElse
                    } && simplifier.IsEquivalent(expr, innerExpr))
                {
                    return Expression.Constant(true);
                }
            }
            return expression;
        }

        private static Expression TransformShortCutCondition(Expression expression)
        {
            {
                // false && a, a && false
                if (expression is BinaryExpression
                    {
                        Left: ConstantExpression { Value: false },
                        NodeType: ExpressionType.AndAlso
                    }
                    or BinaryExpression
                    {
                        Right: ConstantExpression { Value: false },
                        NodeType: ExpressionType.AndAlso
                    })
                {
                    return Expression.Constant(false);
                }
            }

            {
                // true || a, a || true
                if (expression is BinaryExpression
                    {
                        Left: ConstantExpression { Value: true },
                        NodeType: ExpressionType.OrElse
                    }
                    or BinaryExpression
                    {
                        Right: ConstantExpression { Value: true },
                        NodeType: ExpressionType.OrElse
                    })
                {
                    return Expression.Constant(true);
                }
            }

            {
                // false || a
                if (expression is BinaryExpression
                    {
                        Left: ConstantExpression { Value: false },
                        NodeType: ExpressionType.OrElse,
                        Right: var expr
                    })
                {
                    return expr;
                }
            }

            {
                // a || false
                if (expression is BinaryExpression
                    {
                        Right: ConstantExpression { Value: false },
                        NodeType: ExpressionType.OrElse,
                        Left: var expr
                    })
                {
                    return expr;
                }
            }

            {
                // true && a
                if (expression is BinaryExpression
                    {
                        Left: ConstantExpression { Value: true },
                        NodeType: ExpressionType.AndAlso,
                        Right: var expr
                    })
                {
                    return expr;
                }
            }

            {
                // a && true
                if (expression is BinaryExpression
                    {
                        Right: ConstantExpression { Value: true },
                        NodeType: ExpressionType.AndAlso,
                        Left: var expr
                    })
                {
                    return expr;
                }
            }

            {
                if (expression is BinaryExpression
                    {
                        Left: ConstantExpression { Value: true },
                        NodeType: ExpressionType.AndAlso,
                        Right: var expr
                    })
                {
                    return expr;
                }
            }

            {
                if (expression is BinaryExpression
                    {
                        Right: ConstantExpression { Value: true },
                        NodeType: ExpressionType.AndAlso,
                        Left: var expr
                    })
                {
                    return expr;
                }
            }

            {
                if (expression is BinaryExpression
                    {
                        Right: ConstantExpression { Value: false },
                        NodeType: ExpressionType.OrElse,
                        Left: var expr
                    })
                {
                    return expr;
                }
            }

            {
                if (expression is BinaryExpression
                    {
                        Left: ConstantExpression { Value: false },
                        NodeType: ExpressionType.OrElse,
                        Right: var expr
                    })
                {
                    return expr;
                }
            }

            return expression;
        }

        private static Expression TransformParamCondition(Expression expression)
        {
            if (expression is BinaryExpression
                {
                    Left: ParameterExpression { Name: var leftParam },
                    Right: ParameterExpression { Name: var rightParam },
                    NodeType: ExpressionType.Equal or ExpressionType.NotEqual
                }
                && leftParam == rightParam)
            {
                return Expression.Constant(expression.NodeType == ExpressionType.Equal);
            }

            return expression;
        }

        public override Expression Transform(Expression expression, ExpressionSimplifier simplifier)
        {
            var transformed = TransformDoubleNot(expression);
            transformed = TransformConstCondition(transformed, simplifier);
            transformed = TransformParamCondition(transformed);
            transformed = TransformShortCutCondition(transformed);
            transformed = TransformEquivalentConditionArm(transformed, simplifier);

            return transformed;
        }
    }
}