using Ardalis.Result;
using TC.CloudGames.Application.Abstractions;
using TC.CloudGames.Application.Abstractions.Data;
using TC.CloudGames.Application.Abstractions.Messaging;
using TC.CloudGames.Domain.User;
using TC.CloudGames.Infra.CrossCutting.Commons.Authentication;

namespace TC.CloudGames.Application.Users.GetUserById;

internal sealed class GetUserByIdQueryHandler : QueryHandler<GetUserByIdQuery, UserByIdResponse>
{
    private readonly IUserPgRepository _userRepository;
    private readonly IUserContext _userContext;
    
    public GetUserByIdQueryHandler(IUserPgRepository userRepository, IUserContext userContext)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
    }

    public override async Task<Result<UserByIdResponse>> ExecuteAsync(GetUserByIdQuery command, CancellationToken ct = default)
    {
        UserByIdResponse? userResponse = null;

        if (_userContext.UserRole == AppConstants.UserRole && _userContext.UserId != command.Id)
        {
            AddError(x => x.Id, "You are not authorized to access this user.", UserDomainErrors.NotFound.ErrorCode);
            return ValidationErrorNotAuthorized();
        }

        userResponse = await _userRepository
            .GetByIdAsync(command.Id, ct)
            .ConfigureAwait(false);

        if (userResponse is not null)
            return userResponse;

        AddError(x => x.Id, $"User with id '{command.Id}' not found.", UserDomainErrors.NotFound.ErrorCode);
        return ValidationErrorNotFound();
    }
}