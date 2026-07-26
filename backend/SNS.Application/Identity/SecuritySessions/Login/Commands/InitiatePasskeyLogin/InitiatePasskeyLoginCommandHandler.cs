using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Caching;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.ValueObjects;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using System.Linq.Expressions;

namespace SNS.Application.Identity.SecuritySessions.Login.Commands.InitiatePasskeyLogin;

public class InitiatePasskeyLoginCommandHandler
    : ICommandHandler<InitiatePasskeyLoginCommand, AssertionOptions>
{
    private readonly IFido2 _fido2;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICacheService _cache;

    public InitiatePasskeyLoginCommandHandler(
        IFido2 fido2,
        IApplicationDbContext dbContext,
        ICacheService cache)
    {
        _fido2 = fido2;
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<Result<AssertionOptions>> Handle(
        InitiatePasskeyLoginCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Identifier))
        {
            return Result<AssertionOptions>.Failure(UserStatusCodes.NotFound);
        }

        var identifier = new UserIdentifier(request.Identifier);

        Expression<Func<User, bool>> userGetCondition = 
            u => u.Email == request.Identifier;

        if (identifier.Type == IdentifierType.UserName)
        {
            userGetCondition = u => u.UserName == request.Identifier;
        }

        var user = await _dbContext
            .Users
            .Where(userGetCondition)
            .FirstOrDefaultAsync(cancellationToken);
            

        if (user == null)
        {
            return Result<AssertionOptions>.Failure(UserStatusCodes.NotFound);
        }

        var userCredentials = await _dbContext.UserPasskeys
            .Where(p => p.UserId == user.Id)
            .Select(p => new PublicKeyCredentialDescriptor(p.CredentialId))
            .ToListAsync(cancellationToken);

        if (!userCredentials.Any())
        {
            return Result<AssertionOptions>.Failure(SecurityStatusCodes.VerificationFailed);
        }

       
        var getAssertionOptionsParams = new GetAssertionOptionsParams
        {
            AllowedCredentials = userCredentials, 
            UserVerification = UserVerificationRequirement.Required
        };

        var options = _fido2.GetAssertionOptions(getAssertionOptionsParams);

        var cacheKey = $"fido2_login_options:{user.Id}";
        await _cache.SetAsync(cacheKey, options, TimeSpan.FromMinutes(3), cancellationToken);

        return Result<AssertionOptions>.Success(options, OperationStatusCode.Success);
    }
}