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
                .ReverseMap();

            CreateMap<UserDbModel, UserAccount>()
                .ReverseMap();
        }
    }
}
