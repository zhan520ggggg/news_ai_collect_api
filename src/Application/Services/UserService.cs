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
    /**
    Application 层是你的业务核心，它只应该关心 "我要做什么"，不应该关心 "用什么技术做"。
    直接依赖 JWT 包 = 业务核心被技术工具绑架了。

    它只关心业务规则，不关心底层技术（不关心是 JWT、Session、还是 OAuth）。

    高层策略（业务）应该不应依赖低层实现（技术工具）。
    Application 层是高层  JWT 是底层技术工具  Application 层 = 业务核心（不能被（技术污染）

    JWT = 外部技术工具（放在基础层）
    */
    //Application 层不应该依赖 JWT 包。需要用接口解耦。IJwtTokenProvider的实现在webapi，通过服务注册进来
    private readonly IUserRepository _userRepository;
    private readonly IRepository<Domain.Entities.Role> _roleRepository;
    private readonly IRepository<Domain.Entities.UserRole> _userRoleRepository;
    private readonly IMapper _mapper;
    private readonly IJwtTokenProvider _jwtTokenProvider;
    private readonly IMenuService _menuService;

    public UserService(IUserRepository userRepository, IRepository<Domain.Entities.Role> roleRepository,
        IRepository<Domain.Entities.UserRole> userRoleRepository, IMapper mapper,
        IJwtTokenProvider jwtTokenProvider, IMenuService menuService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _mapper = mapper;
        _jwtTokenProvider = jwtTokenProvider;
        _menuService = menuService;
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
        // 处理角色分配
        if (dto.RoleIds != null && dto.RoleIds.Any())
        {
            var userRoles = dto.RoleIds
                .Where(roleIdStr => Guid.TryParse(roleIdStr, out _))
                .Select(roleIdStr => new Domain.Entities.UserRole
                {
                    UserId = created.Id,
                    RoleId = Guid.Parse(roleIdStr)
                })
                .ToList();

            await _userRoleRepository.AddRangeAsync(userRoles, ct);
        }
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

        var userIds = items.Select(u => u.Id);
        var rolesMap = await _userRepository.GetUserRolesMapAsync(userIds, ct);

        var dtos = _mapper.Map<IReadOnlyList<UserResponseDto>>(items);
        foreach (var dto in dtos)
        {
            if (rolesMap.TryGetValue(dto.Id, out var roles))
                dto.Roles = roles;
        }

        return new PagedResponse<UserResponseDto>
        {
            Items = dtos,
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

        // 处理角色更新
        if (dto.RoleIds != null)
        {
            await using var transaction = await _userRoleRepository.BeginTransactionAsync(ct);
            try
            {
                var count = await _userRoleRepository.DeleteManyAsync(ur => ur.UserId == id, ct);
                Console.WriteLine($"删除数量{count}");
                if (dto.RoleIds.Any())
                {
                    var userRoles = dto.RoleIds
                        .Where(roleIdStr => Guid.TryParse(roleIdStr, out _))
                        .Select(roleIdStr => new Domain.Entities.UserRole
                        {
                            UserId = id,
                            RoleId = Guid.Parse(roleIdStr)
                        })
                        .ToList();

                    await _userRoleRepository.AddRangeAsync(userRoles, ct);
                }

                await transaction.CommitAsync(ct);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

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
        var roles = user.UserRoles.Select(ur => ur.RoleId);

        // 生成 JWT Token
        var (token, expiresAt) = _jwtTokenProvider.GenerateToken(user.Id, user.UserName, roles);

        // 获取用户可见菜单树
        var menus = await _menuService.GetUserMenusAsync(user.Id, _userRepository, ct);

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = _mapper.Map<UserResponseDto>(user),
            Menus = menus,
            Roles= roles
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

        // 处理角色分配
        if (dto.RoleIds != null && dto.RoleIds.Any())
        {
            var userRoles = dto.RoleIds
                .Where(roleIdStr => Guid.TryParse(roleIdStr, out _))
                .Select(roleIdStr => new Domain.Entities.UserRole
                {
                    UserId = created.Id,
                    RoleId = Guid.Parse(roleIdStr)
                })
                .ToList();

            await _userRoleRepository.AddRangeAsync(userRoles, ct);
        }

        return _mapper.Map<UserResponseDto>(created);
    }

    private static string HashPassword(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

    private static bool VerifyPassword(string password, string hash) =>
        BCrypt.Net.BCrypt.Verify(password, hash);
}
