using AutoMapper;
using HomelessAnimals.Models;
using Repository = HomelessAnimals.DataAccess.Entities;
using Business = HomelessAnimals.BusinessLogic.Models;
using HomelessAnimals.Extensions;

namespace HomelessAnimals.Mapping
{
    public class SignUpRequestProfile : Profile
    {
        public SignUpRequestProfile()
        {
            CreateMap<SubmitSignUpRequest, SubmitSignUpRequest>()
                .Trim();

            CreateMap<SubmitSignUpRequest, Business.SignUpRequest>();
            CreateMap<Business.SignUpRequest, Repository.SignUpRequest>();

            CreateMap<SignUpRequest, Business.SignUpRequestInfoShort>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
            CreateMap<Business.SignUpRequestInfoShort, SignUpRequestInfoShort>();

            CreateMap<Repository.SignUpRequest, Repository.Volunteer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
