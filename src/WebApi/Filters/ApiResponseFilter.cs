using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Filters;

/// <summary>
/// 统一响应格式过滤器
/// 将所有 Controller 返回值包装为 ApiResponse 格式
/// </summary>
public class ApiResponseFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // 如果已经是 ApiResponse 类型则不包装
        if (context.Result is ObjectResult { Value: ApiResponse } || context.Result is StatusCodeResult)
            return;

        if (context.Result is ObjectResult objectResult)
        {
            context.Result = new ObjectResult(new ApiResponse<object>
            {
                Code = 0,
                Message = "success",
                Data = objectResult.Value,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
