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
    public class GetTribeByInstructorId
    {
        public GetTribeByInstructorId() { }

        public class GetTribeByInstructorIdQuery : IRequest<IEnumerable<GetTribeByInstructorIdResult>>
        {
            public string InstructorId { get; private set; }

            public GetTribeByInstructorIdQuery(string instructorId) => InstructorId = instructorId;

        }

        public class GetTribeByInstructorIdResult
        {
            public string Id { get; private set; }
            public string InstructorId { get; private set; }
            public string Name { get; private set; }
            public string Description { get; private set; }
            public DateTime CreatedDate { get; private set; }
            public string Type { get; private set; }
            public bool IsSubscription { get; private set; }
            public IEnumerable<Member> Members { get; private set; }
        }

        public class GetTribeByInstructorIdQueryHandler : IRequestHandler<GetTribeByInstructorIdQuery, IEnumerable<GetTribeByInstructorIdResult>>
        {
            private readonly TribeContext _db;
            private readonly MapperConfiguration _mapperConfiguration;

            public GetTribeByInstructorIdQueryHandler(TribeContext db)
            {
                _db = db;
                _mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<TribeProfile>();
                    cfg.CreateMap<TribeDAO, GetTribeByInstructorIdResult>()
                        .ForMember(m => m.Type, opt => opt.MapFrom(src => Enum.GetName(typeof(TribeType), src.Type)));
                    cfg.CreateMap<MemberDAO, Member>();
                });
            }

            public async Task<IEnumerable<GetTribeByInstructorIdResult>> Handle(GetTribeByInstructorIdQuery request, CancellationToken cancellationToken)
            {
                var mapper = new Mapper(_mapperConfiguration);

                var storedTribes = await _db.Tribe
                                        .Find(x => x.InstructorId == request.InstructorId)
                                        .ToListAsync();

                var tribes = mapper.Map<IEnumerable<GetTribeByInstructorIdResult>>(storedTribes);

                return tribes;
            }
        }
    }
}
