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

namespace HAS.Profile.Feature.Profile
{
    public class GetAppProfileByPublicName
    {
        public GetAppProfileByPublicName() { }

        public class GetAppProfileByPublicNameQuery : IRequest<string>
        {
            public string PublicName { get; private set; }
            public GetAppProfileByPublicNameQuery(string publicName) => PublicName = publicName;
        }

        public class GetAppProfileByPublicNameResult
        {
            public string Id { get; private set; }
            public DateTime LastUpdate { get; private set; }
            public PersonalDetails PersonalDetails { get; private set; }
            public GetAppProfileByPublicNameAppDetailsResult AppDetails { get; private set; }
        }

        public class GetAppProfileByPublicNameAppDetailsResult
        {
            public string AccountType { get; private set; }
            public DateTime LastLogin { get; private set; }
            public DateTime JoinDate { get; private set; }
            public IEnumerable<SubscriptionDetails> Subscriptions { get; private set; }
            public InstructorDetails InstructorDetails { get; private set; }
        }

        public class GetAppProfileByPublicNameQueryHandler : IRequestHandler<GetAppProfileByPublicNameQuery, string>
        {
            private readonly ProfileContext _db;
            private readonly MapperConfiguration _mapperConfiguration;

            public GetAppProfileByPublicNameQueryHandler(ProfileContext db)
            {
                _db = db;
                _mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<ProfileProfile>();
                    cfg.CreateMap<ProfileDAO, GetAppProfileByPublicNameResult>()
                        .ForMember(m => m.AppDetails, opt => opt.MapFrom(src => src.AppDetails))
                        .ForMember(m => m.PersonalDetails, opt => opt.MapFrom(src => src.PersonalDetails));
                    cfg.CreateMap<AppDetailsDAO, GetAppProfileByPublicNameAppDetailsResult>()
                        .ForMember(m => m.AccountType, opt => opt.MapFrom(source => Enum.GetName(typeof(AccountType), source.AccountType)));
                });
            }

            public async Task<string> Handle(GetAppProfileByPublicNameQuery query, CancellationToken cancellationToken)
            {
                var mapper = new Mapper(_mapperConfiguration);

                var projection = Builders<ProfileDAO>.Projection.Expression(x => mapper.Map<GetAppProfileByPublicNameResult>(x));

                var profile = await _db.Profile
                                        .Find(x => x.AppDetails.InstructorDetails.PublicName.ToUpper() == query.PublicName.ToUpper())
                                        .Project(projection)
                                        .FirstOrDefaultAsync();

                return profile?.Id;
            }
        }
    }
}
