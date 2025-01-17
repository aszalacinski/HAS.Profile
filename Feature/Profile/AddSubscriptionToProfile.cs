﻿using AutoMapper;
using HAS.Profile.Data;
using HAS.Profile.Feature.EventLog;
using HAS.Profile.Model;
using MediatR;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static HAS.Profile.Data.ProfileContext;
using static HAS.Profile.Feature.Profile.GetAppProfileByProfileId;

namespace HAS.Profile.Feature.Profile
{
    public class AddSubscriptionToProfile
    {
        public AddSubscriptionToProfile() { }

        public class AddSubscriptionToProfileCommand : IRequest<string>, ICommandEvent
        {
            public string ProfileId { get; private set; }
            public string InstructorId { get; private set; }

            public AddSubscriptionToProfileCommand(string profileId, string instructorId)
            {
                ProfileId = profileId;
                InstructorId = instructorId;
            }

            public class AddSubscriptionToProfileCommandHandler : IRequestHandler<AddSubscriptionToProfileCommand, string>
            {

                private readonly ProfileContext _db;
                private readonly IMediator _mediator;
                private readonly MapperConfiguration _mapperConfiguration;

                public AddSubscriptionToProfileCommandHandler(ProfileContext db, IMediator mediator)
                {
                    _db = db;
                    _mediator = mediator;
                    _mapperConfiguration = new MapperConfiguration(cfg =>
                    {
                        cfg.AddProfile<ProfileDAOProfile>();
                        cfg.CreateMap<GetAppProfileByProfileIdResult, Model.Profile>();
                        cfg.CreateMap<GetAppProfileByProfileIdAppDetailsResult, AppDetails>()
                            .ForMember(m => m.AccountType, opt => opt.MapFrom(source => Enum.Parse(typeof(AccountType), source.AccountType)));
                    });
                }

                public async Task<string> Handle(AddSubscriptionToProfileCommand cmd, CancellationToken cancellationToken)
                {
                    var init = await _mediator.Send(new GetAppProfileByProfileIdQuery(cmd.ProfileId));

                    var mapper = new Mapper(_mapperConfiguration);

                    Model.Profile profile = mapper.Map<Model.Profile>(init);

                    if (profile.Handle(cmd))
                    {
                        var dao = mapper.Map<ProfileDAO>(profile);

                        try
                        {
                            var filter = Builders<ProfileDAO>.Filter.Eq(x => x.Id, dao.Id);
                            var options = new FindOneAndReplaceOptions<ProfileDAO> { ReturnDocument = ReturnDocument.After };

                            var update = await _db.Profile.FindOneAndReplaceAsync(filter, dao, options);

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
}
