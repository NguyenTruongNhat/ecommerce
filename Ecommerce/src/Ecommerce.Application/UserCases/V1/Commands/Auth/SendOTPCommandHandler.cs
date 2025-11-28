using System.Security.Cryptography;
using System.Text;
using Ecommerce.Application.DependencyInjection.Options;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Enumerations;
using Ecommerce.Contract.Services.V1.Identity;
using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Abstractions.Repositories.IdentityRepository;
using Ecommerce.Persistence;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ecommerce.Application.UserCases.V1.Commands.Auth;
public class SendOTPCommandHandler : ICommandHandler<Command.SendOTP>
{
    private readonly IUserRepository _userRepository;
    private readonly IVerificationCodeRepository _verificationCodeRepository;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SendOTPCommandHandler> _logger;
    private readonly OtpOptions _otpOptions;


    public SendOTPCommandHandler(IUserRepository userRepository,
                                 ApplicationDbContext context,
                                 ILogger<SendOTPCommandHandler> logger,
                                 IOptionsMonitor<OtpOptions> otpOptions,
                                 IVerificationCodeRepository verificationCodeRepository)
    {
        _userRepository = userRepository;
        _context = context;
        _logger = logger;
        _otpOptions = otpOptions.CurrentValue;
        _verificationCodeRepository = verificationCodeRepository;
    }

    public async Task<Result> Handle(Command.SendOTP request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindSingleAsync(x => x.Email.Equals(request.Email));

        if (!Enum.TryParse<VerificationCodeType>(request.Type, true, out var verificationType))
            return Result.Failure(new Error("Invalid.Type", $"Type '{request.Type}' is not valid."));

        if (verificationType == VerificationCodeType.REGISTER && user is not null)
            return Result.Failure(new Error("Email.AlreadyExists", "Email already exists."));

        if (verificationType == VerificationCodeType.FORGOT_PASSWORD && user is null)
            return Result.Failure(new Error("Email.NotFound", "Email not found."));

        // generate OTP
        var code = GenerateNumericCode(6);
        var expiresAt = DateTime.UtcNow.AddMilliseconds(_otpOptions.ExpiresInMs);

        // add or update OTP in database
        await _verificationCodeRepository.UpsertAsync(request.Email, code, verificationType, expiresAt);

        await _context.SaveChangesAsync(cancellationToken);

        // TODO: replace this block with a real email service (inject IEmailService).
        // Example: await _emailService.SendOtpAsync(normalizedEmail, code);       

        return Result.Success();
    }

    private static string GenerateNumericCode(int length)
    {
        var digits = new StringBuilder(length);
        var buffer = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(buffer);
        for (var i = 0; i < length; i++)
            digits.Append((buffer[i] % 10).ToString());
        return digits.ToString();
    }
}
