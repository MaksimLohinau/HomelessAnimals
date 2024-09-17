using AutoMapper;
using Repository = HomelessAnimals.DataAccess.Entities;
using Business = HomelessAnimals.BusinessLogic.Models;
using HomelessAnimals.Models;

namespace HomelessAnimals.Mapping
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<Repository.Account, Business.AuthenticationInfo>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Volunteer.FirstName))
                .ForMember(dest => dest.Permissions,
                    opt => opt.MapFrom(src =>
                        src.RoleAssignments.SelectMany(ra => ra.Role.Permissions).Select(p => p.Name)))
                .ForMember(dest => dest.Scopes,
                    opt => opt.MapFrom(src => src.RoleAssignments.SelectMany(ra => ra.Scopes)));

            CreateMap<Repository.Account, Business.PermissionsInfo>()
                .ForMember(dest => dest.Permissions,
                    opt => opt.MapFrom(src =>
                        src.RoleAssignments.SelectMany(ra => ra.Role.Permissions).Select(p => p.Name)))
                .ForMember(dest => dest.Scopes,
                    opt => opt.MapFrom(src => src.RoleAssignments.SelectMany(ra => ra.Scopes)));

            CreateMap<Business.AuthenticationInfo, AuthenticationInfo>();
            CreateMap<Repository.Scope, Business.Scope>();
        }
    }
}
