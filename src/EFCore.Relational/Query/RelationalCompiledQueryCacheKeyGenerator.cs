// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.Query
{
    /// <summary>
    ///     <para>
    ///         This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///         directly from your code. This API may change or be removed in future releases.
    ///     </para>
    ///     <para>
    ///         The service lifetime is <see cref="ServiceLifetime.Scoped"/>. This means that each
    ///         <see cref="DbContext"/> instance will use its own instance of this service.
    ///         The implementation may depend on other services registered with any lifetime.
    ///         The implementation does not need to be thread-safe.
    ///     </para>
    /// </summary>
    [EntityFrameworkInternal]
    public class RelationalCompiledQueryCacheKeyGenerator : CompiledQueryCacheKeyGenerator
    {
        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// <param name="dependencies"> Parameter object containing dependencies for this service. </param>
        /// <param name="relationalDependencies"> Parameter object containing relational dependencies for this service. </param>
        [EntityFrameworkInternal]
        public RelationalCompiledQueryCacheKeyGenerator(
            [NotNull] CompiledQueryCacheKeyGeneratorDependencies dependencies,
            [NotNull] RelationalCompiledQueryCacheKeyGeneratorDependencies relationalDependencies)
            : base(dependencies)
        {
            Check.NotNull(relationalDependencies, nameof(relationalDependencies));

            RelationalDependencies = relationalDependencies;
        }

        /// <summary>
        ///     Dependencies used to create a <see cref="RelationalCompiledQueryCacheKeyGenerator" />
        /// </summary>
        protected virtual RelationalCompiledQueryCacheKeyGeneratorDependencies RelationalDependencies { get; }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [EntityFrameworkInternal]
        public override object GenerateCacheKey(Expression query, bool async)
            => GenerateCacheKeyCore(query, async);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [EntityFrameworkInternal]
        protected new RelationalCompiledQueryCacheKey GenerateCacheKeyCore([NotNull] Expression query, bool async)
            => new RelationalCompiledQueryCacheKey(
                base.GenerateCacheKeyCore(query, async),
                RelationalOptionsExtension.Extract(RelationalDependencies.ContextOptions).UseRelationalNulls);

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [EntityFrameworkInternal]
        protected readonly struct RelationalCompiledQueryCacheKey
        {
            private readonly CompiledQueryCacheKey _compiledQueryCacheKey;
            private readonly bool _useRelationalNulls;

            /// <summary>
            ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [EntityFrameworkInternal]
            public RelationalCompiledQueryCacheKey(
                CompiledQueryCacheKey compiledQueryCacheKey, bool useRelationalNulls)
            {
                _compiledQueryCacheKey = compiledQueryCacheKey;
                _useRelationalNulls = useRelationalNulls;
            }

            /// <summary>
            ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [EntityFrameworkInternal]
            public override bool Equals(object obj)
                => !(obj is null)
                   && obj is RelationalCompiledQueryCacheKey
                   && Equals((RelationalCompiledQueryCacheKey)obj);

            private bool Equals(RelationalCompiledQueryCacheKey other)
                => _compiledQueryCacheKey.Equals(other._compiledQueryCacheKey)
                   && _useRelationalNulls == other._useRelationalNulls;

            /// <summary>
            ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [EntityFrameworkInternal]
            public override int GetHashCode()
            {
                unchecked
                {
                    return (_compiledQueryCacheKey.GetHashCode() * 397) ^ _useRelationalNulls.GetHashCode();
                }
            }
        }
    }
}
