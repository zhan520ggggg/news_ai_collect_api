using AutoMapper;
using Common.Models;
using Domain.Interfaces;
using Application.DTOs;
using Application.Interfaces;

namespace Application.Services;

/// <summary>
/// 用户服务实现
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IJwtTokenProvider _jwtTokenProvider;

    public UserService(IUserRepository userRepository, IMapper mapper, IJwtTokenProvider jwtTokenProvider)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _jwtTokenProvider = jwtTokenProvider;
    }

    public async Task<UserResponseDto> CreateUserAsync(CreateUserDto dto, CancellationToken ct = default)
    {
        if (await _userRepository.IsUserNameExistsAsync(dto.UserName, ct))
            throw new ConflictException($"用户名 '{dto.UserName}' 已存在");

        if (await _userRepository.IsEmailExistsAsync(dto.Email, ct))
            throw new ConflictException($"邮箱 '{dto.Email}' 已存在");

        var user = _mapper.Map<Domain.Entities.User>(dto);
        user.PasswordHash = HashPassword(dto.Password);

        var created = await _userRepository.AddAsync(user, ct);
        return _mapper.Map<UserResponseDto>(created);
    }

    public async Task<UserResponseDto> GetUserByIdAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), id);

        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task<IReadOnlyList<UserResponseDto>> GetAllUsersAsync(CancellationToken ct = default)
    {
        var users = await _userRepository.GetAllAsync(ct);
        return _mapper.Map<IReadOnlyList<UserResponseDto>>(users);
    }

    public async Task<PagedResponse<UserResponseDto>> GetUsersPagedAsync(
        PagedRequest request, CancellationToken ct = default)
    {
        var items = await _userRepository.GetPagedAsync(
            request.PageNumber, request.PageSize, ct: ct);

        var total = await _userRepository.CountAsync(ct: ct);

        return new PagedResponse<UserResponseDto>
        {
            Items = _mapper.Map<IReadOnlyList<UserResponseDto>>(items),
            TotalCount = total,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<UserResponseDto> UpdateUserAsync(
        Guid id, UpdateUserDto dto, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), id);

        if (!string.IsNullOrWhiteSpace(dto.UserName) && dto.UserName != user.UserName)
        {
            if (await _userRepository.IsUserNameExistsAsync(dto.UserName, ct))
                throw new ConflictException($"用户名 '{dto.UserName}' 已存在");
            user.UserName = dto.UserName;
        }

        if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
        {
            if (await _userRepository.IsEmailExistsAsync(dto.Email, ct))
                throw new ConflictException($"邮箱 '{dto.Email}' 已存在");
            user.Email = dto.Email;
        }

        if (dto.Phone != null) user.Phone = dto.Phone;
        if (dto.DisplayName != null) user.DisplayName = dto.DisplayName;

        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user, ct);
        return _mapper.Map<UserResponseDto>(user);
    }

    public async Task DeleteUserAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), id);

        await _userRepository.DeleteAsync(user, ct);
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto, CancellationToken ct = default)
    {
        var user = await _userRepository.FindByUserNameWithRolesAsync(dto.UserName, ct)
            ?? throw new BusinessException(401, "用户名或密码错误");

        if (!VerifyPassword(dto.Password, user.PasswordHash))
            throw new BusinessException(401, "用户名或密码错误");

        if (!user.IsActive)
            throw new BusinessException(403, "账号已被禁用");

        // 提取角色列表
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

        // 生成 JWT Token
        var (token, expiresAt) = _jwtTokenProvider.GenerateToken(user.Id, user.UserName, roles);

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = _mapper.Map<UserResponseDto>(user)
        };
    }

    public async Task<UserResponseDto> RegisterAsync(RegisterDto dto, CancellationToken ct = default)
    {
        if (await _userRepository.IsUserNameExistsAsync(dto.UserName, ct))
            throw new ConflictException($"用户名 '{dto.UserName}' 已存在");

        if (await _userRepository.IsEmailExistsAsync(dto.Email, ct))
            throw new ConflictException($"邮箱 '{dto.Email}' 已存在");

        var user = new Domain.Entities.User
        {
            UserName = dto.UserName,
            Email = dto.Email,
            DisplayName = dto.DisplayName,
            PasswordHash = HashPassword(dto.Password),
            IsActive = true
        };

        var created = await _userRepository.AddAsync(user, ct);
        return _mapper.Map<UserResponseDto>(created);
    }

    private static string HashPassword(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

    private static bool VerifyPassword(string password, string hash) =>
        BCrypt.Net.BCrypt.Verify(password, hash);
}
