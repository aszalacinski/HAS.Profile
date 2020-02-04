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
using static HAS.Profile.Data.TribeContext;

namespace HAS.Profile.Feature.Tribe
{
    public class GetTribeByTribeId
    {
        public GetTribeByTribeId() { }

        public class GetTribeByTribeIdQuery : IRequest<GetTribeByTribeIdResult>
        {
            public string TribeId { get; private set; }

            public GetTribeByTribeIdQuery(string tribeId) => TribeId = tribeId;

        }

        public class GetTribeByTribeIdResult
        {
            public string Id { get; private set; }
            public string InstructorId { get; private set; }
            public string Name { get; private set; }
            public string Description { get; private set; }
            public DateTime CreatedDate { get; private set; }
            public string Type { get; private set; }
            public bool IsSubscription { get; private set; }
            public TribeSubscriptionDetails SubscriptionDetails { get; private set; }
            public IEnumerable<Member> Members { get; private set; }
        }

        public class GetTribeByTribeIdQueryHandler : IRequestHandler<GetTribeByTribeIdQuery, GetTribeByTribeIdResult>
        {
            private readonly TribeContext _db;
            private readonly MapperConfiguration _mapperConfiguration;

            public GetTribeByTribeIdQueryHandler(TribeContext db)
            {
                _db = db;
                _mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<TribeProfile>();
                    cfg.CreateMap<TribeDAO, GetTribeByTribeIdResult>()
                        .ForMember(m => m.Type, opt => opt.MapFrom(src => Enum.GetName(typeof(TribeType), src.Type)));
                    cfg.CreateMap<MemberDAO, Member>();
                    cfg.CreateMap<TribeSubscriptionDetailsDAO, TribeSubscriptionDetails>();
                    cfg.CreateMap<SubscriptionRateDAO, SubscriptionRate>();
                });
            }

            public async Task<GetTribeByTribeIdResult> Handle(GetTribeByTribeIdQuery request, CancellationToken cancellationToken)
            {
                var mapper = new Mapper(_mapperConfiguration);

                var projection = Builders<TribeDAO>.Projection.Expression(x => mapper.Map<GetTribeByTribeIdResult>(x));

                var tribe = await _db.Tribe
                                        .Find(x => x.Id == ObjectId.Parse(request.TribeId))
                                        .Project(projection)
                                        .FirstOrDefaultAsync();

                return tribe;
            }
        }
    }

}
