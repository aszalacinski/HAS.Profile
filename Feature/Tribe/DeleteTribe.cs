using HAS.Profile.Data;
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
    public class DeleteTribe
    {
        public class DeleteTribeCommand : IRequest<long>
        {
            public string TribeId { get; set; }

            public string InstructorId { get; set; }

            public DeleteTribeCommand(string hubId, string profileId)
            {
                TribeId = hubId;
                InstructorId = profileId;
            }
        }

        public class DeleteTribeCommandHandler : IRequestHandler<DeleteTribeCommand, long>
        {
            public readonly TribeContext _db;

            public DeleteTribeCommandHandler(TribeContext db)
            {
                _db = db;
            }

            public async Task<long> Handle(DeleteTribeCommand cmd, CancellationToken cancellationToken)
            {
                var delete = await _db.Tribe.DeleteManyAsync(x => x.Id == ObjectId.Parse(cmd.TribeId) && x.InstructorId == cmd.InstructorId);

                return delete.DeletedCount;
            }
        }
    }
}
