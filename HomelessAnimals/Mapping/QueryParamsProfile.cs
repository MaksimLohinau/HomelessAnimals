using AutoMapper;
using Repository = HomelessAnimals.DataAccess.QueryOptions;
using Business = HomelessAnimals.BusinessLogic.QueryParams;
using HomelessAnimals.Models;
using HomelessAnimals.QueryParams;

namespace HomelessAnimals.Mapping
{
    public class QueryParamsProfile : Profile
    {
        public QueryParamsProfile()
        {
            CreateMap<GetVolunteersQueryParams, Business.GetVolunteersQueryParams>()
                .ForMember(dest => dest.Page,
                           opt => opt.MapFrom(src =>
                           !src.Page.HasValue || src.Page < 1 ? 1 : src.Page.Value));

            CreateMap<Business.GetVolunteersQueryParams, Repository.VolounteersQueryOptions>();

            CreateMap<GetAnimalsQueryParams, Business.GetAnimalsQueryParams>()
                .ForMember(dest => dest.Page,
                           opt => opt.MapFrom(src =>
                           !src.Page.HasValue || src.Page < 1 ? 1 : src.Page.Value));

            CreateMap<Business.GetAnimalsQueryParams, Repository.AnimalsQueryOptions>();
        }
    }
}
