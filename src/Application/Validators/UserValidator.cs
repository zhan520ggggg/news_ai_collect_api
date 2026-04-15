using FluentValidation;
using Application.DTOs;

namespace Application.Validators;

/// <summary>
/// 创建用户验证器
/// </summary>
public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("用户名不能为空")
            .MaximumLength(50).WithMessage("用户名不能超过 50 个字符")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("用户名只能包含字母、数字和下划线");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("邮箱不能为空")
            .EmailAddress().WithMessage("邮箱格式不正确")
            .MaximumLength(100);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MinimumLength(6).WithMessage("密码长度不能少于 6 位")
            .MaximumLength(50);

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .Matches(@"^[\d\-\+\(\) ]*$").When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("手机号格式不正确");

        RuleFor(x => x.DisplayName)
            .MaximumLength(100).WithMessage("显示名称不能超过 100 个字符");
    }
}

/// <summary>
/// 更新用户验证器
/// </summary>
public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.UserName)
            .MaximumLength(50).WithMessage("用户名不能超过 50 个字符")
            .Matches(@"^[a-zA-Z0-9_]+$").When(x => !string.IsNullOrEmpty(x.UserName))
            .WithMessage("用户名只能包含字母、数字和下划线");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("邮箱格式不正确")
            .MaximumLength(100);

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .Matches(@"^[\d\-\+\(\) ]*$").When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("手机号格式不正确");
    }
}
