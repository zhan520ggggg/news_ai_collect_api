namespace Common.Models;

/// <summary>
/// 业务异常基类
/// </summary>
public class BusinessException : Exception
{
    /// <summary>
    /// 业务错误码
    /// </summary>
    public int ErrorCode { get; }

    public BusinessException(string message) : base(message)
    {
        ErrorCode = 400;
    }

    public BusinessException(int errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }

    public BusinessException(int errorCode, string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

/// <summary>
/// 资源不存在异常
/// </summary>
public class NotFoundException : BusinessException
{
    public NotFoundException(string message) : base(404, message) { }

    public NotFoundException(string entityName, object id)
        : base(404, $"{entityName} (Id={id}) 不存在") { }
}

/// <summary>
/// 参数验证异常
/// </summary>
public class ValidationException : BusinessException
{
    public ValidationException(string message) : base(422, message) { }

    public ValidationException(IEnumerable<string> errors)
        : base(422, string.Join("; ", errors)) { }
}

/// <summary>
/// 冲突异常
/// </summary>
public class ConflictException : BusinessException
{
    public ConflictException(string message) : base(409, message) { }
}
