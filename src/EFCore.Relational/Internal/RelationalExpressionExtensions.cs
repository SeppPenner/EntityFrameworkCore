// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public static class RelationalExpressionExtensions
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static bool IsSimpleExpression([NotNull] this Expression expression)
        {
            Check.NotNull(expression, nameof(expression));

            var unwrappedExpression = expression.RemoveConvert();

            return unwrappedExpression is ConstantExpression
                   || unwrappedExpression is ColumnExpression
                   || unwrappedExpression is ParameterExpression
                   || unwrappedExpression is ColumnReferenceExpression
                   || unwrappedExpression is AliasExpression;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static ColumnReferenceExpression LiftExpressionFromSubquery(
            [NotNull] this Expression expression,
            [NotNull] TableExpressionBase table)
        {
            Check.NotNull(expression, nameof(expression));
            Check.NotNull(table, nameof(table));

            switch (expression)
            {
                case ColumnExpression columnExpression:
                    return new ColumnReferenceExpression(columnExpression, table);
                case AliasExpression aliasExpression:
                    return new ColumnReferenceExpression(aliasExpression, table);
                case ColumnReferenceExpression columnReferenceExpression:
                    return new ColumnReferenceExpression(columnReferenceExpression, table);
            }

            Debug.Fail("LiftExpressionFromSubquery was called on incorrect expression type.");

            return null;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static Expression UnwrapNullableExpression(this Expression expression)
            => expression is NullableExpression nullableExpression
                ? nullableExpression.Operand
                : expression;

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static ColumnExpression FindOriginatingColumnExpression([NotNull] this Expression expression)
        {
            switch (expression)
            {
                case ColumnExpression columnExpression:
                    return columnExpression;

                case ColumnReferenceExpression columnReferenceExpression:
                    return columnReferenceExpression.Expression.FindOriginatingColumnExpression();

                case AliasExpression aliasExpression:
                    return aliasExpression.Expression.FindOriginatingColumnExpression();

                case UnaryExpression unaryExpression:
                    return unaryExpression.Operand.FindOriginatingColumnExpression();
            }

            return null;
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public static Expression UnwrapAliasExpression(this Expression expression)
            => (expression as AliasExpression)?.Expression ?? expression;
    }
}
