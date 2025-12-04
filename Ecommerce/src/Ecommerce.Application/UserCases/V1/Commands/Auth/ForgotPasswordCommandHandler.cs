using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Enumerations;
using Ecommerce.Contract.Services.V1.Identity;
using Ecommerce.Domain.Abstractions;
using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Abstractions.Repositories.IdentityRepository;
using Ecommerce.Domain.Exceptions;
using static Ecommerce.Contract.Services.V1.Identity.Command;

namespace Ecommerce.Application.UserCases.V1.Commands.Auth;
public class ForgotPasswordCommandHandler : ICommandHandler<Command.ForgotPassword>
{
    private readonly IUserRepository _userRepository;
    private readonly IHashingService _hashingService;
    private readonly IVerificationCodeRepository _verificationCodeRepository;

    public ForgotPasswordCommandHandler(
        IUserRepository userRepository,
        IHashingService hashingService,
        IVerificationCodeRepository verificationCodeRepository)
    {
        _userRepository = userRepository;
        _hashingService = hashingService;
        _verificationCodeRepository = verificationCodeRepository;
    }

    public async Task<Result> Handle(ForgotPassword request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindSingleAsync(x => x.Email == request.Email, cancellationToken)
            ?? throw new CommonException.NotFound();

        var verificationCode = await _verificationCodeRepository.FindSingleAsync(
                x => x.Email == request.Email &&
                     x.Type == VerificationCodeType.FORGOT_PASSWORD &&
                     x.Code == request.Code, cancellationToken) ?? throw new CommonException.NotFound();

        _verificationCodeRepository.Remove(verificationCode);

        var hashedPassword = _hashingService.Hash(request.NewPassword);
        user.Password = hashedPassword;
        _userRepository.Update(user);

        return Result.Success();
    }
}
