using AutoMapper;
using Application.DTOs;

namespace Application.Mappings;

/// <summary>
/// 菜单 AutoMapper 配置
/// </summary>
public class MenuMappingProfile : Profile
{
    public MenuMappingProfile()
    {
        CreateMap<CreateMenuDto, Domain.Entities.Menu>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.Parent, opt => opt.Ignore())
            .ForMember(dest => dest.Children, opt => opt.Ignore())
            .ForMember(dest => dest.RoleMenus, opt => opt.Ignore());

        CreateMap<UpdateMenuDto, Domain.Entities.Menu>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Domain.Entities.Menu, MenuResponseDto>();
    }
}
