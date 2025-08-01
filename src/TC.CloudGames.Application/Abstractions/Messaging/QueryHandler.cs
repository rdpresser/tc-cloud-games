﻿using System.Diagnostics.CodeAnalysis;

namespace TC.CloudGames.Application.Abstractions.Messaging
{
    [ExcludeFromCodeCoverage]
    internal abstract class QueryHandler<TQuery, TResponse> : CommandHandler<TQuery, Result<TResponse>>
            where TQuery : IQuery<TResponse>
            where TResponse : class
    {
        private FastEndpoints.ValidationContext<TQuery> ValidationContext { get; } = Instance;

        protected Result<TResponse> ValidationErrorNotFound()
        {
            if (ValidationContext.ValidationFailures.Count == 0)
            {
                return Result<TResponse>.Success(default!);
            }

            return Result<TResponse>.NotFound([.. ValidationContext.ValidationFailures.Select(x => x.ErrorMessage)]);
        }

        protected Result<TResponse> ValidationErrorNotAuthorized()
        {
            if (ValidationContext.ValidationFailures.Count == 0)
            {
                return Result<TResponse>.Success(default!);
            }

            return Result<TResponse>.Unauthorized([.. ValidationContext.ValidationFailures.Select(x => x.ErrorMessage)]);
        }
    }
}
