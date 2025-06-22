using AutoMapper;
using KoboRack.Core.DTO;
using KoboRack.Data.Repository.DTO;
using KoboRack.Model.Entities;

namespace KoboRack.Api.AutoMapperProfile
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Kyc, KycRequestDto>().ReverseMap();
            CreateMap<KycResponseDto, Kyc>().ReverseMap();
            CreateMap<Saving, PersonalSavingsDTO>().ReverseMap();
            CreateMap<Saving, GetUserSavingsDto>().ReverseMap();
            CreateMap<AppUser, AppUserDto>();
            CreateMap<AppUser, AppUserDto2>();
            CreateMap<GroupSavingsMembers, GroupMembersDto2>();
            CreateMap<GroupDTO, Group>().ReverseMap();
            CreateMap<GroupDTO2, Group>().ReverseMap();
            CreateMap<AppUserUpdateDto, AppUser>().ReverseMap();
            CreateMap<WalletResponseDto, Wallet>().ReverseMap();
            CreateMap<WalletDto, Wallet>().ReverseMap();
        }
    }
}
