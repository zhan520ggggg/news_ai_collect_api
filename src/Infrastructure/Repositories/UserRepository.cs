using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// 用户仓储实现
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<User?> FindByUserNameAsync(string userName, CancellationToken ct = default) =>
        await _dbSet.FirstOrDefaultAsync(u => u.UserName == userName, ct);

    public async Task<User?> FindByEmailAsync(string email, CancellationToken ct = default) =>
        await _dbSet.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<bool> IsUserNameExistsAsync(string userName, CancellationToken ct = default) =>
        await _dbSet.AnyAsync(u => u.UserName == userName, ct);

    public async Task<bool> IsEmailExistsAsync(string email, CancellationToken ct = default) =>
        await _dbSet.AnyAsync(u => u.Email == email, ct);

    public async Task<User?> FindByUserNameWithRolesAsync(string userName, CancellationToken ct = default) =>
        await _dbSet
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.UserName == userName, ct);
}
