using AutoMapper;
using HAS.Profile.Data;
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
    public class GetSubscriptionsByProfileId
    {
        public GetSubscriptionsByProfileId() { }

        public class GetSubscriptionsByProfileIdQuery : IRequest<IEnumerable<string>>
        {
            public string ProfileId { get; private set; }

            public GetSubscriptionsByProfileIdQuery(string profileId) => ProfileId = profileId;
        }

        public class GetSubscriptionsByProfileIdQueryHandler : IRequestHandler<GetSubscriptionsByProfileIdQuery, IEnumerable<string>>
        {
            private readonly ProfileContext _db;
            private readonly IMediator _mediator;
            private readonly MapperConfiguration _mapperConfiguration;
            
            public GetSubscriptionsByProfileIdQueryHandler(ProfileContext db, IMediator mediator)
            {
                _db = db;
                _mediator = mediator;
                _mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<ProfileProfile>();
                });
            }

            public async Task<IEnumerable<string>> Handle(GetSubscriptionsByProfileIdQuery request, CancellationToken cancellationToken)
            {
                var mapper = new Mapper(_mapperConfiguration);

                var projection = Builders<ProfileDAO>.Projection.Expression(x => mapper.Map<Model.Profile>(x));

                var profile = await _db.Profile
                                        .Find(x => x.Id == ObjectId.Parse(request.ProfileId))
                                        .Project(projection)
                                        .FirstOrDefaultAsync();

                var subscriptions = profile.AppDetails.Subscriptions.ToList().Select(x => x.InstructorId).ToList();

                return subscriptions;
            }
        }
    }
}
