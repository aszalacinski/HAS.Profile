using AutoMapper;
using HAS.Profile.Data;
using HAS.Profile.Model;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static HAS.Profile.Data.ProfileContext;

namespace HAS.Profile.Feature.Profile
{
    public class GetAppProfileByProfileId
    {
        private readonly IMediator _mediator;

        public GetAppProfileByProfileId(IMediator mediator) => _mediator = mediator;

        public class GetAppProfileByProfileIdQuery : IRequest<GetAppProfileByProfileIdResult>
        {
            public string ProfileId { get; private set; }

            public GetAppProfileByProfileIdQuery(string profileId) => ProfileId = profileId;
        }

        public class GetAppProfileByProfileIdResult
        {
            public string Id { get; private set; }
            public DateTime LastUpdate { get; private set; }
            public PersonalDetails PersonalDetails { get; private set; }
            public GetAppProfileByProfileIdAppDetailsResult AppDetails { get; private set; }
        }

        public class GetAppProfileByProfileIdAppDetailsResult
        {
            public string AccountType { get; private set; }
            public DateTime LastLogin { get; private set; }
            public DateTime JoinDate { get; private set; }
            public IEnumerable<SubscriptionDetails> Subscriptions { get; private set; }
            public InstructorDetails InstructorDetails { get; private set; }
        }

        public class GetProfileByIdQueryHandler : IRequestHandler<GetAppProfileByProfileIdQuery, GetAppProfileByProfileIdResult>
        {
            private readonly ProfileContext _db;
            private readonly MapperConfiguration _mapperConfiguration;

            public GetProfileByIdQueryHandler(ProfileContext db)
            {
                _db = db;
                _mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<ProfileProfile>();
                    cfg.CreateMap<ProfileDAO, GetAppProfileByProfileIdResult>()
                        .ForMember(m => m.AppDetails, opt => opt.MapFrom(src => src.AppDetails))
                        .ForMember(m => m.PersonalDetails, opt => opt.MapFrom(src => src.PersonalDetails));
                    cfg.CreateMap<AppDetailsDAO, GetAppProfileByProfileIdAppDetailsResult>()
                        .ForMember(m => m.AccountType, opt => opt.MapFrom(source => Enum.GetName(typeof(AccountType), source.AccountType)));
                });
            }

            public async Task<GetAppProfileByProfileIdResult> Handle(GetAppProfileByProfileIdQuery request, CancellationToken cancellationToken)
            {
                var mapper = new Mapper(_mapperConfiguration);

                var projection = Builders<ProfileDAO>.Projection.Expression(x => mapper.Map<GetAppProfileByProfileIdResult>(x));

                var profile = await _db.Profile
                                        .Find(x => x.Id == ObjectId.Parse(request.ProfileId))
                                        .Project(projection)
                                        .FirstOrDefaultAsync();

                return profile;
            }
        }
    }
}
