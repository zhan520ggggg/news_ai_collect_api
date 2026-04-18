using System.Linq.Expressions;
using Domain.Entities;

namespace Domain.Interfaces;

/// <summary>
/// 泛型仓储接口
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// 根据 ID 获取实体
    /// </summary>
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// 获取全部列表
    /// </summary>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// 根据条件查找列表
    /// </summary>
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

    /// <summary>
    /// 根据条件查找单个实体
    /// </summary>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

    /// <summary>
    /// 判断是否存在
    /// </summary>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

    /// <summary>
    /// 新增实体
    /// </summary>
    Task<T> AddAsync(T entity, CancellationToken ct = default);

    /// <summary>
    /// 新增多个实体
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);

    /// <summary>
    /// 更新实体
    /// </summary>
    Task UpdateAsync(T entity, CancellationToken ct = default);

    /// <summary>
    /// 删除实体（软删除）
    /// </summary>
    Task DeleteAsync(T entity, CancellationToken ct = default);

    /// <summary>
    /// 删除实体（硬删除）
    /// </summary>
    Task HardDeleteAsync(T entity, CancellationToken ct = default);

    /// <summary>
    /// 根据条件批量硬删除
    /// </summary>
    Task<int> DeleteManyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

    /// <summary>
    /// 分页查询
    /// </summary>
    Task<IReadOnlyList<T>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken ct = default);

    /// <summary>
    /// 统计数量
    /// </summary>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);

    /// <summary>
    /// 开始事务
    /// </summary>
    Task<IDbTransaction> BeginTransactionAsync(CancellationToken ct = default);
}
