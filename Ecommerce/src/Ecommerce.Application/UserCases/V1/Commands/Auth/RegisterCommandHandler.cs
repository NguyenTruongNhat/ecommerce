using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Enumerations;
using Ecommerce.Contract.Services.V1.Identity;
using Ecommerce.Domain.Abstractions;
using Ecommerce.Domain.Abstractions.Repositories;
using Ecommerce.Domain.Abstractions.Repositories.IdentityRepository;
using Ecommerce.Domain.Entities.Identity;
using Ecommerce.Domain.Exceptions;
using Ecommerce.Persistence;

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
        // ----------------------------------------------------
        // 1. Validate Verification Code
        // (Thao tác DB: kiểm tra mã, kiểm tra hết hạn)
        // ----------------------------------------------------

        // Hàm này (trong service) sẽ ném exception hoặc trả về Result.Failure nếu validation thất bại
        var existingCode = await _verificationCodeRepository
            .FindSingleAsync(x =>
                x.Email == request.Email &&
                x.Code == request.Code &&
                x.Type == VerificationCodeType.REGISTER,
                cancellationToken
            ) ?? throw new CommonException.NotFound();

        // ----------------------------------------------------
        // 2. Lấy Role ID và Hash Mật khẩu (Không cần Promise.all)
        // ----------------------------------------------------

        var clientRoleId = await _roleRepository.GetClientRoleIdAsync(cancellationToken);
        var hashedPassword = _hashingService.Hash(request.Password); // Hashing thường là đồng bộ

        // ----------------------------------------------------
        // 3. Kiểm tra Email Tồn tại (Unique Constraint Check)
        // ----------------------------------------------------

        // Trong .NET, việc kiểm tra độc nhất (unique) thường được thực hiện 
        // NGAY TRƯỚC khi gọi Add() hoặc bằng cách xử lý ngoại lệ DB (như NestJS)

        // Cách làm Clean Architecture: Kiểm tra trước khi tạo Entity
        var existingUser = await _userRepository.FindSingleAsync(u => u.Email == request.Email);
        if (existingUser != null)
        {
            // Tương đương 'throw EmailAlreadyExistsException'
            return Result.Failure(new Error("Email.AlreadyExists", "Email already exists."));
        }

        // ----------------------------------------------------
        // 4. Tạo User VÀ Xóa Code (Sử dụng Task.WhenAll)
        // ----------------------------------------------------

        var newUser = new User
        {
            Email = request.Email,
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            Password = hashedPassword,
            RoleId = clientRoleId,
            CreatedAt = DateTimeOffset.UtcNow // Sử dụng DateTimeOffset
        };

        // Thêm User vào DbContext (chưa Save)
        _userRepository.Add(newUser);

        // Xóa Verification Code (chưa Save)
        var deleteCodeTask = _verificationCodeRepository.DeleteCodeAsync(
            request.Email,
            request.Code,
            VerificationCodeType.REGISTER,
            cancellationToken
        );

        // Thực thi song song (tương đương Promise.all, nhưng chỉ với các Task)
        await Task.WhenAll(deleteCodeTask);

        // ----------------------------------------------------
        // 5. Commit Transaction (Tương đương với việc kết thúc try/catch thành công)
        // ----------------------------------------------------
        await _unitOfWork.CommitAsync(cancellationToken);

        // ----------------------------------------------------
        // 6. Trả về kết quả
        // ----------------------------------------------------
        return Result<UserDto>.Success(UserDto.FromEntity(newUser));
    }
}
}
