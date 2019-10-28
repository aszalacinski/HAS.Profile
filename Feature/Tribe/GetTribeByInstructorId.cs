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

        public class GetTribeByInstructorIdQuery : IRequest<string>
        {
            public string InstructorId { get; private set; }

            public GetTribeByInstructorIdQuery(string instructorId) => InstructorId = instructorId;

        }

        public class GetTribeByInstructorIdQueryHandler : IRequestHandler<GetTribeByInstructorIdQuery, string>
        {
            private readonly TribeContext _db;

            public GetTribeByInstructorIdQueryHandler(TribeContext db)
            {
                _db = db;
            }

            public async Task<string> Handle(GetTribeByInstructorIdQuery request, CancellationToken cancellationToken)
            {

                var tribe = await _db.Tribe
                                        .Find(x => x.InstructorId == request.InstructorId)
                                        .FirstOrDefaultAsync();

                return tribe.Id.ToString();
            }
        }
    }
}
