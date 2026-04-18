namespace Domain.Interfaces;

/// <summary>
/// 数据库事务接口
/// </summary>
public interface IDbTransaction : IAsyncDisposable
{
    /// <summary>
    /// 提交事务
    /// </summary>
    Task CommitAsync(CancellationToken ct = default);

    /// <summary>
    /// 回滚事务
    /// </summary>
    Task RollbackAsync(CancellationToken ct = default);
}
