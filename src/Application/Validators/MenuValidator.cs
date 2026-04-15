using FluentValidation;
using Application.DTOs;

namespace Application.Validators;

/// <summary>
/// 创建菜单验证器
/// </summary>
public class CreateMenuDtoValidator : AbstractValidator<CreateMenuDto>
{
    public CreateMenuDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("菜单编码不能为空")
            .MaximumLength(50).WithMessage("菜单编码不能超过 50 个字符");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("菜单名称不能为空")
            .MaximumLength(100).WithMessage("菜单名称不能超过 100 个字符");

        RuleFor(x => x.Type)
            .InclusiveBetween(0, 2).WithMessage("菜单类型必须为 0（模块）、1（菜单）或 2（按钮）");

        RuleFor(x => x.Sort)
            .GreaterThanOrEqualTo(0).WithMessage("排序值不能为负数");
    }
}

/// <summary>
/// 更新菜单验证器
/// </summary>
public class UpdateMenuDtoValidator : AbstractValidator<UpdateMenuDto>
{
    public UpdateMenuDtoValidator()
    {
        RuleFor(x => x.Code)
            .MaximumLength(50).WithMessage("菜单编码不能超过 50 个字符");

        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("菜单名称不能超过 100 个字符");

        RuleFor(x => x.Type)
            .InclusiveBetween(0, 2).WithMessage("菜单类型必须为 0（模块）、1（菜单）或 2（按钮）");
    }
}
