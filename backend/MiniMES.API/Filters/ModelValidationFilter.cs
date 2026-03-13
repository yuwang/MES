using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MiniMES.Shared.Common;

namespace MiniMES.API.Filters;

/// <summary>
/// 模型验证过滤器 - 统一处理请求参数验证错误
/// </summary>
public class ModelValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            // 收集所有验证错误消息
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            // 返回统一格式的失败响应
            var response = ApiResponse<object>.FailResult(string.Join("; ", errors));
            context.Result = new OkObjectResult(response);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
