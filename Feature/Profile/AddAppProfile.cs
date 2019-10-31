using AutoMapper;
using HAS.Profile.Data;
using HAS.Profile.Feature.EventLog;
using HAS.Profile.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static HAS.Profile.Data.ProfileContext;

namespace HAS.Profile.Feature.Profile
{
    public class AddAppProfile
    {
        private readonly IMediator _mediator;

        public AddAppProfile(IMediator mediator) => _mediator = mediator;

        public class AddAppProfileCommand : IRequest<string>, ICommandEvent
        {
            public string UserId { get; set; }
            public string Email { get; set; }
        }

        public class AddAppProfileCommandHandler : IRequestHandler<AddAppProfileCommand, string>
        {
            private readonly ProfileContext _db;
            private readonly MapperConfiguration _mapperConfiguration;

            public AddAppProfileCommandHandler(ProfileContext db)
            {
                _db = db;
                _mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<ProfileDAOProfile>();
                });
            }

            public async Task<string> Handle(AddAppProfileCommand request, CancellationToken cancellationToken)
            {
                var locationDetails = LocationDetails.Create(string.Empty, string.Empty, string.Empty);
                var personalDetails = PersonalDetails.Create(request.UserId, request.Email, string.Empty, string.Empty, string.Empty, locationDetails);

                var instructorDetails = InstructorDetails.Create(null);
                var appDetails = AppDetails.Create(AccountType.STUDENT, DateTime.UtcNow, DateTime.UtcNow, new List<SubscriptionDetails>(), instructorDetails);

                var profile = Model.Profile.Create(string.Empty, DateTime.UtcNow, personalDetails, appDetails);

                var mapper = new Mapper(_mapperConfiguration);

                var dao = mapper.Map<ProfileDAO>(profile);

                try
                {
                    await _db.Profile.InsertOneAsync(dao);
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
