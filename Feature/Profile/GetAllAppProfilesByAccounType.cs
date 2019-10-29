using AutoMapper;
using HAS.Profile.Data;
using HAS.Profile.Model;
using MediatR;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static HAS.Profile.Data.ProfileContext;
using static HAS.Profile.Feature.Profile.GetAllAppProfilesByAccounType;

namespace HAS.Profile.Feature.Profile
{
    public class GetAllAppProfilesByAccounType
    {
        public GetAllAppProfilesByAccounType() { }

        public class GetAllAppProfilesByAccounTypeQuery : IRequest<IEnumerable<GetAllAppProfilesByAccounTypeResult>>
        { 
            public AccountType AccountType { get; private set; }

            public GetAllAppProfilesByAccounTypeQuery(string accountType)
            {
                AccountType aType = AccountType.NONE;

                if(Enum.TryParse(accountType, true, out aType))
                {
                    AccountType = aType;
                }
                else
                {
                    throw new ArgumentException($"{accountType} is not a proper Account Type");
                }
            }
        }

        public class GetAllAppProfilesByAccounTypeResult
        {
            public string Id { get; private set; }
            public DateTime LastUpdate { get; private set; }
            public PersonalDetails PersonalDetails { get; private set; }
            public GetAllAppProfilesByAccounTypeAppDetailsResult AppDetails { get; private set; }
        }

        public class GetAllAppProfilesByAccounTypeAppDetailsResult
        {
            public string AccountType { get; private set; }
            public DateTime LastLogin { get; private set; }
            public DateTime JoinDate { get; private set; }
            public IEnumerable<SubscriptionDetails> Subscriptions { get; private set; }
            public InstructorDetails InstructorDetails { get; private set; }
        }

        public class GetAllAppProfilesByAccounTypeQueryHandler : IRequestHandler<GetAllAppProfilesByAccounTypeQuery, IEnumerable<GetAllAppProfilesByAccounTypeResult>>
        {
            private readonly ProfileContext _db;
            private readonly MapperConfiguration _mapperConfiguration;

            public GetAllAppProfilesByAccounTypeQueryHandler(ProfileContext db)
            {
                _db = db;
                _mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<ProfileProfile>();
                    cfg.CreateMap<ProfileDAO, GetAllAppProfilesByAccounTypeResult>()
                        .ForMember(m => m.AppDetails, opt => opt.MapFrom(src => src.AppDetails))
                        .ForMember(m => m.PersonalDetails, opt => opt.MapFrom(src => src.PersonalDetails));
                    cfg.CreateMap<AppDetailsDAO, GetAllAppProfilesByAccounTypeAppDetailsResult>()
                        .ForMember(m => m.AccountType, opt => opt.MapFrom(source => Enum.GetName(typeof(AccountType), source.AccountType)));

                });
            }

            public async Task<IEnumerable<GetAllAppProfilesByAccounTypeResult>> Handle(GetAllAppProfilesByAccounTypeQuery request, CancellationToken cancellationToken)
            {
                var storedProfiles = await _db.Profile
                                        .Find(x => x.AppDetails.AccountType.Equals(request.AccountType))
                                        .ToListAsync();

                var mapper = new Mapper(_mapperConfiguration);
                var profiles = mapper.Map<IEnumerable<GetAllAppProfilesByAccounTypeResult>>(storedProfiles);

                return profiles;
            }
        }
    }
}
