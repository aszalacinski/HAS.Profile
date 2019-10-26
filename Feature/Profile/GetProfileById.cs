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
    public class GetProfileById
    {
        private readonly IMediator _mediator;

        public GetProfileById(IMediator mediator) => _mediator = mediator;

        public class GetProfileByIdQuery : IRequest<GetProfileByIdResult>
        {
            public string ProfileId { get; private set; }

            public GetProfileByIdQuery(string profileId) => ProfileId = profileId;
        }

        public class GetProfileByIdResult
        {
            public string Id { get; private set; }
            public DateTime LastUpdate { get; private set; }
            public PersonalDetails PersonalDetails { get; private set; }
            public AppDetails AppDetails { get; private set; }
        }

        public class GetProfileByIdQueryHandler : IRequestHandler<GetProfileByIdQuery, GetProfileByIdResult>
        {
            private readonly ProfileContext _db;
            private readonly MapperConfiguration _mapperConfiguration;

            public GetProfileByIdQueryHandler(ProfileContext db)
            {
                _db = db;
                _mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<ProfileProfile>();
                    cfg.CreateMap<ProfileDAO, GetProfileByIdResult>()
                        .ForMember(m => m.AppDetails, opt => opt.MapFrom(src => src.AppDetails))
                        .ForMember(m => m.PersonalDetails, opt => opt.MapFrom(src => src.PersonalDetails));
                });
            }

            public async Task<GetProfileByIdResult> Handle(GetProfileByIdQuery request, CancellationToken cancellationToken)
            {
                var mapper = new Mapper(_mapperConfiguration);

                var projection = Builders<ProfileDAO>.Projection.Expression(x => mapper.Map<GetProfileByIdResult>(x));

                var profile = await _db.Profile
                                        .Find(x => x.Id == ObjectId.Parse(request.ProfileId))
                                        .Project(projection)
                                        .FirstOrDefaultAsync();

                return profile;
            }
        }
    }
}
