using System;
using System.Collections.Generic;

namespace HAS.Profile.Model
{
    public class Profile : IEntity
    {
        public string Id { get; private set; }
        public DateTime LastUpdate { get; private set; }
        public PersonalDetails PersonalDetails { get; private set; }
        public AppDetails AppDetails { get; private set; }

        private Profile() { }

        private Profile(string id, DateTime lastUpdate, PersonalDetails personalDetails, AppDetails appDetails)
        {
            Id = id;
            LastUpdate = lastUpdate;
            PersonalDetails = personalDetails;
            AppDetails = appDetails;
        }

        public static Profile Create(string id, DateTime lastUpdate, PersonalDetails personalDetails, AppDetails appDetails)
            => new Profile(id, lastUpdate, personalDetails, appDetails);
    }

    public class PersonalDetails
    {
        public string UserId { get; private set; }
        public string Email { get; private set; }
        public string ScreenName { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public LocationDetails Location { get; private set; }
    }

    public class LocationDetails
    {
        public string Country { get; private set; }
        public string City { get; private set; }
        public string StateProvince { get; private set; }
    }

    public class AppDetails
    {
        public AccountType AccounType { get; private set; }
        public DateTime LastLogin { get; private set; }
        public DateTime JoinDate { get; private set; }
        public IEnumerable<SubscriptionDetails> Subscriptions { get; private set; }
        public InstructorDetails InstructorDetails { get; private set; }
    }

    public enum AccountType
    {
        NONE = 0,
        STUDENT,
        INSTRUCTOR
    }

    public class SubscriptionDetails
    {
        public string InstructorId { get; private set; }

        public IEnumerable<ClassDetails> Classes { get; private set; }

    }

    public class InstructorDetails
    {
        public DateTime? StartDate { get; private set; }
    }

    public class ClassDetails
    {
        public string ClassId { get; private set; }
        public bool Liked { get; private set; }
    }
}
