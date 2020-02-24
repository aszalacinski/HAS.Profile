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
using static HAS.Profile.Data.TribeContext;

namespace HAS.Profile.Feature.Tribe
{
    public class GetMembersFromTribe
    {
        public GetMembersFromTribe() { }

        public class GetMembersFromTribeQuery : IRequest<IEnumerable<Model.Profile>>
        {
            public string InstructorId { get; private set; }
            public string TribeId { get; private set; }

            public GetMembersFromTribeQuery(string instructorId, string tribeId)
            {
                InstructorId = instructorId;
                TribeId = tribeId;
            }
        }

        public class GetMembersByTribeQueryHandler : IRequestHandler<GetMembersFromTribeQuery, IEnumerable<Model.Profile>>
        {
            private readonly TribeContext _tribeDb;
            private readonly ProfileContext _profileDb;
            private readonly MapperConfiguration _mapperConfiguration;

            public GetMembersByTribeQueryHandler(TribeContext tribeDb, ProfileContext profileDb)
            {
                _tribeDb = tribeDb;
                _profileDb = profileDb;
                _mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<TribeProfile>();
                    cfg.AddProfile<ProfileProfile>();
                });
            }

            public async Task<IEnumerable<Model.Profile>> Handle(GetMembersFromTribeQuery query, CancellationToken cancellationToken)
            {
                var mapper = new Mapper(_mapperConfiguration);

                var tribeProjection = Builders<TribeDAO>.Projection.Expression(x => mapper.Map<Model.Tribe>(x));

                var tribe = await _tribeDb.Tribe
                    .Find(x => x.Id == ObjectId.Parse(query.TribeId))
                    .Project(tribeProjection)
                    .FirstOrDefaultAsync();

                List<Model.Profile> profiles = new List<Model.Profile>();

                var profileProjection = Builders<ProfileDAO>.Projection.Expression(x => mapper.Map<Model.Profile>(x));

                // for each member in tribe, get profile
                foreach(var member in tribe.Members)
                {
                    var profile = await _profileDb.Profile
                        .Find(x => x.Id == ObjectId.Parse(member.Id))
                        .Project(profileProjection)
                        .FirstOrDefaultAsync();

                    profiles.Add(profile);
                }

                return profiles;
            }
        }
    }
}
