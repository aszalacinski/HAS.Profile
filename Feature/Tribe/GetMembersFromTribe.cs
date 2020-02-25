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
using static HAS.Profile.Data.TribeContext;
using static HAS.Profile.Feature.Tribe.GetMembersFromTribe.GetMembersByTribeQueryHandler;

namespace HAS.Profile.Feature.Tribe
{
    public class GetMembersFromTribe
    {
        public GetMembersFromTribe() { }

        public class GetMembersFromTribeQuery : IRequest<IEnumerable<MiniProfileResult>>
        {
            public string InstructorId { get; private set; }
            public string TribeId { get; private set; }

            public GetMembersFromTribeQuery(string instructorId, string tribeId)
            {
                InstructorId = instructorId;
                TribeId = tribeId;
            }
        }

        public class GetMembersByTribeQueryHandler : IRequestHandler<GetMembersFromTribeQuery, IEnumerable<MiniProfileResult>>
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
                    cfg.CreateMap<ProfileDAO, MiniProfileResult>()
                        .ForMember(m => m.Id, opt => opt.MapFrom(src => src.Id))
                        .ForMember(m => m.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                        .ForMember(m => m.JoinDate, opt => opt.MapFrom(src => src.AppDetails.JoinDate))
                        .ForMember(m => m.LastLogin, opt => opt.MapFrom(src => src.AppDetails.LastLogin))
                        .ForMember(m => m.PersonalDetails, opt => opt.MapFrom(src => src.PersonalDetails));
                });
            }


            public class MiniProfileResult
            {
                public string Id { get; set; }
                public DateTime LastUpdate { get; set; }
                public DateTime JoinDate { get; set; }
                public DateTime LastLogin { get; set; }
                public PersonalDetails PersonalDetails { get; set; }
            }

            public async Task<IEnumerable<MiniProfileResult>> Handle(GetMembersFromTribeQuery query, CancellationToken cancellationToken)
            {
                var mapper = new Mapper(_mapperConfiguration);

                var tribeProjection = Builders<TribeDAO>.Projection.Expression(x => mapper.Map<Model.Tribe>(x));

                var tribe = await _tribeDb.Tribe
                    .Find(x => x.Id == ObjectId.Parse(query.TribeId))
                    .Project(tribeProjection)
                    .FirstOrDefaultAsync();

                List<MiniProfileResult> profiles = new List<MiniProfileResult>();

                var profileProjection = Builders<ProfileDAO>.Projection.Expression(x => mapper.Map<MiniProfileResult>(x));

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
