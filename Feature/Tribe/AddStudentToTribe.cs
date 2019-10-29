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
using static HAS.Profile.Data.TribeContext;
using static HAS.Profile.Feature.Tribe.GetTribeByTribeId;

namespace HAS.Profile.Feature.Tribe
{
    public class AddStudentToTribe
    {
        public class AddStudentToTribeCommand : IRequest<string>
        {
            public string InstructorId { get; private set; }
            public string TribeId { get; private set; }
            public string StudentId { get; private set; }

            public AddStudentToTribeCommand(string instructorId, string tribeId, string studentId)
            {
                InstructorId = instructorId;
                TribeId = tribeId;
                StudentId = studentId;
            }
        }

        public class AddStudentToTribeCommandHandler : IRequestHandler<AddStudentToTribeCommand, string>
        {
            private readonly IMediator _mediator;
            private readonly MapperConfiguration _mapperConfiguration;
            private readonly TribeContext _db;

            public AddStudentToTribeCommandHandler(TribeContext db, IMediator mediator)
            {
                _db = db;
                _mediator = mediator;
                _mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<TribeDAOProfile>();
                    cfg.CreateMap<GetTribeByTribeIdResult, Model.Tribe>()
                        .ForMember(m => m.Type, opt => opt.MapFrom(src => Enum.Parse(typeof(TribeType), src.Type)))
                        .ForMember(m => m.Members, opt => opt.MapFrom(src => src.Members));

                });
            }

            public async Task<string> Handle(AddStudentToTribeCommand cmd, CancellationToken cancellationToken)
            {
                var init = await _mediator.Send(new GetTribeByTribeIdQuery(cmd.TribeId));

                var mapper = new Mapper(_mapperConfiguration);

                Model.Tribe tribe = mapper.Map<Model.Tribe>(init);

                if (tribe.Handle(cmd))
                {
                    var dao = mapper.Map<TribeDAO>(tribe);

                    try
                    {
                        var filter = Builders<TribeDAO>.Filter.Eq(x => x.Id, dao.Id);
                        var options = new FindOneAndReplaceOptions<TribeDAO> { ReturnDocument = ReturnDocument.After };

                        var update = await _db.Tribe.FindOneAndReplaceAsync(filter, dao, options);

                        return update.Id.ToString();
                    }
                    catch (Exception)
                    {
                        return string.Empty;
                    }
                }

                return string.Empty;
            }
        }
    }
}
