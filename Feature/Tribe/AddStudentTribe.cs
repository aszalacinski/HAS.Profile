using AutoMapper;
using HAS.Profile.Data;
using HAS.Profile.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static HAS.Profile.Data.TribeContext;

namespace HAS.Profile.Feature.Tribe
{
    public class AddStudentTribe
    {
        public AddStudentTribe() { }

        public class AddStudentTribeCommand : IRequest<string>
        {
            public string InstructorId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }

        public class AddStudentTribeCommandHandler : IRequestHandler<AddStudentTribeCommand, string>
        {
            private readonly TribeContext _db;
            private readonly MapperConfiguration _mapperConfiguration;

            public AddStudentTribeCommandHandler(TribeContext db)
            {
                _db = db;
                _mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<TribeDAOProfile>();
                });
            }

            public async Task<string> Handle(AddStudentTribeCommand request, CancellationToken cancellationToken)
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
