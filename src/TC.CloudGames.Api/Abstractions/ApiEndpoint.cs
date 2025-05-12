using Ardalis.Result;
using FastEndpoints;
using FluentValidation.Results;
using System.Net;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;
using TC.CloudGames.Infra.CrossCutting.Commons.Caching;
using ZiggyCreatures.Caching.Fusion;

namespace TC.CloudGames.Api.Abstractions
{
    public abstract class ApiEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse>
        where TRequest : notnull
    {
        /// <summary>
        /// Handles the result of a query or command execution and sends the appropriate response.
        /// </summary>
        /// <param name="response">The result to handle.</param>
        /// <param name="ct">The cancellation token.</param>
        protected async Task MatchResultAsync(
            Result<TResponse> response,
            CancellationToken ct = default)
        {
            if (response.IsSuccess)
            {
                await SendAsync(response.Value, cancellation: ct).ConfigureAwait(false);
                return;
            }

            if (response.IsNotFound())
            {
                await SendErrorsAsync((int)HttpStatusCode.NotFound, ct).ConfigureAwait(false);
                return;
            }

            if (response.IsUnauthorized())
            {
                await SendErrorsAsync((int)HttpStatusCode.Unauthorized, ct).ConfigureAwait(false);
                return;
            }

            await SendErrorsAsync(cancellation: ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Handles caching and validation for queries or commands.
        /// </summary>
        /// <param name="cache">The FusionCache instance.</param>
        /// <param name="dataCacheKey">The cache key for the data.</param>
        /// <param name="validationCacheKey">The cache key for validation failures.</param>
        /// <param name="executeQuery">The query execution function.</param>
        /// <param name="validationFailures">The list of validation failures.</param>
        /// <param name="userContext">The user context for generating unique cache keys.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>The result of the query or command execution.</returns>
        protected async Task<Result<TResponse>> GetOrSetWithValidationAsync(
            IFusionCache cache,
            string dataCacheKey,
            string validationCacheKey,
            Func<CancellationToken, Task<Result<TResponse>>> executeQuery,
            List<ValidationFailure> validationFailures,
            IUserContext userContext,
            CancellationToken ct = default)
        {
            // Generate a unique key based on the user context
            var uniqueKey = $"-{userContext.UserId}-{userContext.UserEmail}";
            dataCacheKey = $"{dataCacheKey}{uniqueKey}";
            validationCacheKey = $"{validationCacheKey}{uniqueKey}";

            // Try to get data from cache
            var response = await cache.GetOrSetAsync(dataCacheKey,
                async token =>
                {
                    var result = await executeQuery(token).ConfigureAwait(false);

                    // Cache validation failures if they exist
                    if (validationFailures.Count != 0)
                    {
                        await cache.SetAsync(validationCacheKey, validationFailures, CacheOptions.DefaultExpiration, token)
                            .ConfigureAwait(false);
                    }

                    return result;
                },
                options: CacheOptions.DefaultExpiration,
                ct).ConfigureAwait(false);

            // Retrieve validation failures from cache if available
            if (!response.IsSuccess)
            {
                var cachedValidationFailures = await cache.TryGetAsync<List<ValidationFailure>>(validationCacheKey, CacheOptions.DefaultExpiration, ct)
                    .ConfigureAwait(false);

                if (cachedValidationFailures.HasValue && validationFailures.Count == 0)
                {
                    validationFailures.AddRange(cachedValidationFailures.Value);
                }
            }

            return response;
        }
    }
}


