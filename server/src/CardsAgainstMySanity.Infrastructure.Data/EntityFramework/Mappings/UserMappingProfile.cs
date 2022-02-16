using AutoMapper;
using CardsAgainstMySanity.Domain.Auth;
using CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Models;

namespace CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserDbModel, Guest>()
                .ReverseMap()
                .ForMember(udbm => udbm.UserTypeId, opt => opt.MapFrom(_ => 2));

            CreateMap<UserDbModel, UserAccount>()
                .ReverseMap();
        }
    }
}
