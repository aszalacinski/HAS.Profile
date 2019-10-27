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
    public class GetAppProfileByAuthUserId
    {
        private readonly IMediator _mediator;

        public GetAppProfileByAuthUserId(IMediator mediator) => _mediator = mediator;

        public class GetAppProfileByAuthUserIdQuery : IRequest<GetAppProfileByAuthUserIdResult>
        {
            public string UserId { get; private set; }

            public GetAppProfileByAuthUserIdQuery(string userId) => UserId = userId;
        }

        public class GetAppProfileByAuthUserIdResult
        {
            public string Id { get; private set; }
            public DateTime LastUpdate { get; private set; }
            public PersonalDetails PersonalDetails { get; private set; }
            public GetAppProfileByAuthUserIdAppDetailsResult AppDetails { get; private set; }
        }

        public class GetAppProfileByAuthUserIdAppDetailsResult
        {
            public string AccountType { get; private set; }
            public DateTime LastLogin { get; private set; }
            public DateTime JoinDate { get; private set; }
            public IEnumerable<SubscriptionDetails> Subscriptions { get; private set; }
            public InstructorDetails InstructorDetails { get; private set; }
        }

        public class GetAppProfileByAuthUserIdQueryHandler : IRequestHandler<GetAppProfileByAuthUserIdQuery, GetAppProfileByAuthUserIdResult>
        {
            private readonly ProfileContext _db;
            private readonly MapperConfiguration _mapperConfiguration;

            public GetAppProfileByAuthUserIdQueryHandler(ProfileContext db)
            {
                _db = db;
                _mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<ProfileProfile>();
                    cfg.CreateMap<ProfileDAO, GetAppProfileByAuthUserIdResult>()
                        .ForMember(m => m.AppDetails, opt => opt.MapFrom(src => src.AppDetails))
                        .ForMember(m => m.PersonalDetails, opt => opt.MapFrom(src => src.PersonalDetails));
                    cfg.CreateMap<AppDetailsDAO, GetAppProfileByAuthUserIdAppDetailsResult>()
                        .ForMember(m => m.AccountType, opt => opt.MapFrom(source => Enum.GetName(typeof(AccountType), source.AccountType)));
                });
            }

            public async Task<GetAppProfileByAuthUserIdResult> Handle(GetAppProfileByAuthUserIdQuery request, CancellationToken cancellationToken)
            {
                var mapper = new Mapper(_mapperConfiguration);

                var projection = Builders<ProfileDAO>.Projection.Expression(x => mapper.Map<GetAppProfileByAuthUserIdResult>(x));

                var profile = await _db.Profile
                                        .Find(x => x.PersonalDetails.UserId == request.UserId)
                                        .Project(projection)
                                        .FirstOrDefaultAsync();

                return profile;
            }
        }
    }
}
