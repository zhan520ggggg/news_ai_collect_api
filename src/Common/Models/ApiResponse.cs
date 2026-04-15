namespace Common.Models;

/// <summary>
/// 统一 API 响应模型
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// 状态码：0=成功，非0=失败
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// 消息描述
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 响应数据
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// 请求时间戳
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public ApiResponse() { }

    public ApiResponse(int code, string message, T? data = default)
    {
        Code = code;
        Message = message;
        Data = data;
    }
}

/// <summary>
/// 无数据的统一响应模型
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    public ApiResponse(int code, string message)
        : base(code, message, null) { }
}
