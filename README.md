# Expression Booster
A simple expression simplifier using Math Rules.

## Current Supported Rules
| Output | Intput |
| - | - |
| `a` | `!!a`, `a && a`, `a \|\| a`, `a \|\| (a && b)`, `a && (a \|\| b)`, `a \|\| false`, `a && true` |
| `false` | `false && a`, `a && (!a)` |
| `true` | `true \|\| a`, `a \|\| (!a)` |
| `!a` | `(!a \|\| b) && (!a \|\| !b)` |

## Usage
```csharp
// use transformers to simplify expressions
var transformers = new List<ExpressionTransformer>
{
    new ConditionTransformer(),
};

var simplifier = new ExpressionSimplifier(transformers);

// (a, b) => Not(Not((((Not(Not((IIF(((a == b) OrElse True), 4, 7) == 7))) == False) AndAlso (((6 - a) == 8) OrElse Not(((5 - a) == 8)))) AndAlso (Not(((5 - a) == 8)) OrElse Not(((6 - a) == 8))))))
Expression<Func<int, int, bool>> expr =
    (a, b) => !!(
        !!(((a == b || true) ? 4 : 7) == 7) == false
        && (6 - a == 8 || !(5 - a == 8))
        && (!(5 - a == 8) || !(6 - a == 8)));

Console.WriteLine($"Original: {expr}");
Console.WriteLine($"Simplified: {simplifier.Simplify(expr)}");
// Output: (a, b) => ((6 - a) == 8)
```