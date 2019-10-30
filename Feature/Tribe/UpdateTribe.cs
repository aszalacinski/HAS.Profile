using AutoMapper;
using HAS.Profile.Data;
using HAS.Profile.Model;
using MediatR;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static HAS.Profile.Data.TribeContext;
using static HAS.Profile.Feature.Tribe.GetTribeByTribeId;

namespace HAS.Profile.Feature.Tribe
{
    public class UpdateTribe
    {
        public class UpdateTribeCommand : IRequest<string>
        {
            public string InstructorId { get; set; }
            public string TribeId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }

        public class UpdateTribeCommandHandler : IRequestHandler<UpdateTribeCommand, string>
        {
            private readonly TribeContext _db;
            private readonly IMediator _mediator;
            private readonly MapperConfiguration _mapperConfiguration;

            public UpdateTribeCommandHandler(TribeContext db, IMediator mediator)
            {
                _db = db;
                _mediator = mediator;
                _mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<TribeDAOProfile>();
                    cfg.CreateMap<GetTribeByTribeIdResult, Model.Tribe>()
                        .ForMember(x => x.Members, opt => opt.MapFrom(src => src.Members));

                });
            }

            public async Task<string> Handle(UpdateTribeCommand cmd, CancellationToken cancellationToken)
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
