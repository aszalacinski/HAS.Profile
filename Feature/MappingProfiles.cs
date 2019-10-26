using AutoMapper;
using MapperProfile = AutoMapper.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static HAS.Profile.Data.ProfileContext;
using MongoDB.Bson;
using HAS.Profile.Model;
using static HAS.Profile.Data.TribeContext;

namespace HAS.Profile.Feature
{
    public class ProfileDAOProfile : MapperProfile
    {
        public ProfileDAOProfile()
        {
            CreateMap<Model.Profile, ProfileDAO>()
                .ForMember(m => m.Id, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Id) ? ObjectId.GenerateNewId() : ObjectId.Parse(src.Id)));
            CreateMap<PersonalDetails, PersonalDetailsDAO>();
            CreateMap<AppDetails, AppDetailsDAO>();
            CreateMap<LocationDetails, LocationDetailsDAO>();
            CreateMap<SubscriptionDetails, SubscriptionDetailsDAO>();
            CreateMap<InstructorDetails, InstructorDetailsDAO>();
            CreateMap<ClassDetails, ClassDetailsDAO>();
        }
    }

    public class ProfileProfile : MapperProfile
    {
        public ProfileProfile()
        {
            CreateMap<ProfileDAO, Model.Profile>()
                .ForMember(m => m.PersonalDetails, opt => opt.MapFrom(src => src.PersonalDetails))
                .ForMember(m => m.AppDetails, opt => opt.MapFrom(src => src.AppDetails));
            CreateMap<PersonalDetailsDAO, PersonalDetails>()
                .ForMember(m => m.Location, opt => opt.MapFrom(src => src.Location));
            CreateMap<LocationDetailsDAO, LocationDetails>();
            CreateMap<AppDetailsDAO, AppDetails>()
                .ForMember(m => m.Subscriptions, opt => opt.MapFrom(src => src.Subscriptions))
                .ForMember(m => m.InstructorDetails, opt => opt.MapFrom(src => src.InstructorDetails));
            CreateMap<SubscriptionDetailsDAO, SubscriptionDetails>()
                .ForMember(m => m.Classes, opt => opt.MapFrom(src => src.Classes));
            CreateMap<InstructorDetailsDAO, InstructorDetails>();
            CreateMap<ClassDetailsDAO, ClassDetails>();
        }

    }

    public class TribeDAOProfile : MapperProfile
    {
        public TribeDAOProfile()
        {
            CreateMap<Tribe, TribeDAO>()
                .ForMember(m => m.Id, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.Id) ? ObjectId.GenerateNewId() : ObjectId.Parse(src.Id)));
            CreateMap<Member, MemberDAO>();
        }
    }

    public class TribeProfile : MapperProfile
    {
        public TribeProfile()
        {
            CreateMap<TribeDAO, Tribe>()
                .ForMember(m => m.Members, opt => opt.MapFrom(src => src.Members));
            CreateMap<MemberDAO, Member>();
        }
    }
}
