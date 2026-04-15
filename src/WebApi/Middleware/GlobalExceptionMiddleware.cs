using System.Net;
using System.Text.Json;
using Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WebApi.Middleware;

/// <summary>
/// 全局异常处理中间件
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "未处理异常: {Message}", exception.Message);

        var response = exception switch
        {
            ValidationException ve => new ApiResponse(
                ve.ErrorCode,
                ve.Message),

            NotFoundException nfe => new ApiResponse(
                nfe.ErrorCode,
                nfe.Message),

            ConflictException ce => new ApiResponse(
                ce.ErrorCode,
                ce.Message),

            BusinessException be => new ApiResponse(
                be.ErrorCode,
                be.Message),

            _ => new ApiResponse(
                (int)HttpStatusCode.InternalServerError,
                "服务器内部错误，请稍后重试")
        };

        var statusCode = exception switch
        {
            NotFoundException => (int)HttpStatusCode.NotFound,
            ConflictException => (int)HttpStatusCode.Conflict,
            ValidationException => (int)HttpStatusCode.UnprocessableEntity,
            BusinessException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}
