using AutoMapper;
using HAS.Profile.Data;
using HAS.Profile.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static HAS.Profile.Data.ProfileContext;
using static HAS.Profile.Data.TribeContext;

namespace HAS.Profile.Feature.Tribe
{
    public class AddTribe
    {
        public AddTribe() { }

        public class AddTribeCommand : IRequest<string>
        {
            public string InstructorId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }

        public class AddTribeCommandHandler : IRequestHandler<AddTribeCommand, string>
        {
            private readonly TribeContext _db;
            private readonly MapperConfiguration _mapperConfiguration;

            public AddTribeCommandHandler(TribeContext db)
            {
                _db = db;
                _mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<TribeDAOProfile>();
                });
            }

            public async Task<string> Handle(AddTribeCommand request, CancellationToken cancellationToken)
            {
                var tribe = Model.Tribe.Create(string.Empty, request.InstructorId, request.Name, request.Description, DateTime.UtcNow, Model.TribeType.STUDENT, false, new List<Member>());

                var mapper = new Mapper(_mapperConfiguration);

                var dao = mapper.Map<TribeDAO>(tribe);

                try
                {
                    await _db.Tribe.InsertOneAsync(dao);
                }
                catch(Exception)
                {
                    return string.Empty;
                }

                return dao.Id.ToString();

            }
        }
    }
}
