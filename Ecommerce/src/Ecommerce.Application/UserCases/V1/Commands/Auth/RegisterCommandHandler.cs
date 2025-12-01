using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Enumerations;
using Ecommerce.Contract.Services.V1.Identity;
using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Abstractions.Repositories.IdentityRepository;
using Ecommerce.Domain.Entities.Identity;
using Ecommerce.Domain.Exceptions;

namespace Ecommerce.Application.UserCases.V1.Commands.Auth;
public class RegisterCommandHandler : ICommandHandler<Command.RegisterCommand>
{
    // Khai báo các Dependencies tương đương
    private readonly IRoleRepository _roleRepository;
    private readonly IHashingService _hashingService;
    private readonly IUserRepository _userRepository;
    private readonly IVerificationCodeRepository _verificationCodeRepository;

    // Dependency Injection qua Constructor
    public RegisterCommandHandler(
        IRoleRepository roleRepository,
        IHashingService hashingService,
        IUserRepository userRepository,
        IVerificationCodeRepository verificationCodeRepository)
    {
        _roleRepository = roleRepository;
        _hashingService = hashingService;
        _userRepository = userRepository;
        _verificationCodeRepository = verificationCodeRepository;
    }

    public async Task<Result> Handle(Command.RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingCode = await _verificationCodeRepository
            .FindSingleAsync(x =>
                x.Email == request.Email &&
                x.Code == request.Code &&
                x.Type == VerificationCodeType.REGISTER,
                cancellationToken
            ) ?? throw new CommonException.NotFound();

        var clientRoleId = await _roleRepository.GetClientRoleIdAsync(RoleType.Client);
        var hashedPassword = _hashingService.Hash(request.Password) ?? throw new CommonException.NotFound();

        var newUser = new User
        {
            Email = request.Email,
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            Password = hashedPassword,
            RoleId = clientRoleId,
        };

        _userRepository.Add(newUser);
        _verificationCodeRepository.Remove(existingCode);

        return Result.Success();
    }
}

