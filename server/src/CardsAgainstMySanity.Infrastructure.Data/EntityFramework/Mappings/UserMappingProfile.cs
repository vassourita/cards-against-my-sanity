namespace CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Mappings;

using AutoMapper;
using CardsAgainstMySanity.Domain.Auth;
using CardsAgainstMySanity.Infrastructure.Data.EntityFramework.Models;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        this.CreateMap<UserDbModel, Guest>()
            .ReverseMap()
            .ForMember(udbm => udbm.UserTypeId, opt => opt.MapFrom(_ => 2));

        this.CreateMap<UserDbModel, UserAccount>()
            .ReverseMap();
    }
}
