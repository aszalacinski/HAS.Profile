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

        public class GetAppProfileByAuthUserIdQuery : IRequest<string>
        {
            public string UserId { get; private set; }

            public GetAppProfileByAuthUserIdQuery(string userId) => UserId = userId;
        }


        public class GetAppProfileByAuthUserIdQueryHandler : IRequestHandler<GetAppProfileByAuthUserIdQuery, string>
        {
            private readonly ProfileContext _db;
            private readonly MapperConfiguration _mapperConfiguration;

            public GetAppProfileByAuthUserIdQueryHandler(ProfileContext db)
            {
                _db = db;
                _mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<ProfileProfile>();
                });
            }

            public async Task<string> Handle(GetAppProfileByAuthUserIdQuery request, CancellationToken cancellationToken)
            {
                var profile = await _db.Profile
                                        .Find(x => x.PersonalDetails.UserId == request.UserId)
                                        .FirstOrDefaultAsync();

                return profile.Id.ToString();
            }
        }
    }
}
