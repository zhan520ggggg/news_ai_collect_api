using FluentValidation;
using Application.DTOs;

namespace Application.Validators;

/// <summary>
/// 创建产品验证器
/// </summary>
public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("产品名称不能为空")
            .MaximumLength(200).WithMessage("产品名称不能超过 200 个字符");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("产品描述不能超过 1000 个字符");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("价格必须大于 0");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("库存不能为负数");

        RuleFor(x => x.Category)
            .MaximumLength(100).WithMessage("分类不能超过 100 个字符");
    }
}

/// <summary>
/// 更新产品验证器
/// </summary>
public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("产品名称不能超过 200 个字符");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("产品描述不能超过 1000 个字符");

        RuleFor(x => x.Price)
            .GreaterThan(0).When(x => x.Price.HasValue)
            .WithMessage("价格必须大于 0");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).When(x => x.Stock.HasValue)
            .WithMessage("库存不能为负数");

        RuleFor(x => x.Category)
            .MaximumLength(100).WithMessage("分类不能超过 100 个字符");
    }
}
