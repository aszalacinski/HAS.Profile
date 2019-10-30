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
    public class GetAllTribesByStudent
    {
        public class GetAllTribesByStudentQuery : IRequest<IEnumerable<GetAllTribesByStudentResult>>
        {
            public string StudentId { get; private set; }

            public GetAllTribesByStudentQuery(string studentId) => StudentId = studentId;

        }

        public class GetAllTribesByStudentResult
        {
            public string Id { get; private set; }
            public string InstructorId { get; private set; }
            public string Name { get; private set; }
            public string Description { get; private set; }
            public DateTime CreatedDate { get; private set; }
            public string Type { get; private set; }
        }

        public class GetAllTribesByStudentQueryHandler : IRequestHandler<GetAllTribesByStudentQuery, IEnumerable<GetAllTribesByStudentResult>>
        {
            private readonly TribeContext _db;
            private readonly MapperConfiguration _mapperConfiguration;

            public GetAllTribesByStudentQueryHandler(TribeContext db)
            {
                _db = db;
                _mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<TribeProfile>();
                    cfg.CreateMap<TribeDAO, GetAllTribesByStudentResult>()
                        .ForMember(m => m.Type, opt => opt.MapFrom(src => Enum.GetName(typeof(TribeType), src.Type)));
                });
            }

            public async Task<IEnumerable<GetAllTribesByStudentResult>> Handle(GetAllTribesByStudentQuery request, CancellationToken cancellationToken)
            {
                var mapper = new Mapper(_mapperConfiguration);

                var storedTribes = await _db.Tribe
                                        .Find(x => x.Members.Any(y => y.Id == ObjectId.Parse(request.StudentId)))
                                        .ToListAsync();

                var tribes = mapper.Map<IEnumerable<GetAllTribesByStudentResult>>(storedTribes);

                return tribes;
            }
        }
    }
}
