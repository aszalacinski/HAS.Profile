using AutoMapper;
using HAS.Profile.Data;
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
    public class UpdateTribeToNonSubscription
    {
        public UpdateTribeToNonSubscription() { }

        public class UpdateTribeToNonSubscriptionCommand : IRequest<string>
        {
            public string TribeId { get; set; }
            public string InstructorId { get; set; }
            public UpdateTribeToNonSubscriptionCommand(string tribeId, string instructorId)
            {
                TribeId = tribeId;
                InstructorId = instructorId;
            }
        }

        public class UpdateTribeToNonSubscriptionCommandHandler : IRequestHandler<UpdateTribeToNonSubscriptionCommand, string>
        {
            private readonly TribeContext _db;
            private readonly IMediator _mediator;
            private readonly MapperConfiguration _mapperConfiguration;

            public UpdateTribeToNonSubscriptionCommandHandler(TribeContext db, IMediator mediator)
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

            public async Task<string> Handle(UpdateTribeToNonSubscriptionCommand cmd, CancellationToken cancellationToken)
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
