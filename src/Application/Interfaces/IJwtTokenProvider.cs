namespace Application.Interfaces;

/// <summary>
/// JWT Token 提供者接口
/// </summary>
public interface IJwtTokenProvider
{
    /// <summary>
    /// 生成 JWT Token
    /// </summary>
    /// <param name="userId">用户 ID</param>
    /// <param name="userName">用户名</param>
    /// <param name="roles">角色列表</param>
    /// <returns>(Token, 过期时间)</returns>
    (string token, DateTime expiresAt) GenerateToken(Guid userId, string userName, IEnumerable<Guid> roles);
}
