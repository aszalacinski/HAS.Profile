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

        private PersonalDetails() { }

        private PersonalDetails(string userId, string email, string screenName, string firstName, string lastName, LocationDetails locationDetails)
        {
            UserId = userId;
            Email = email;
            ScreenName = screenName;
            FirstName = firstName;
            LastName = lastName;
            Location = locationDetails;
        }

        public static PersonalDetails Create(string userId, string email, string screenName, string firstName, string lastName, LocationDetails locationDetails)
            => new PersonalDetails(userId, email, screenName, firstName, lastName, locationDetails);
    }

    public class LocationDetails
    {
        public string Country { get; private set; }
        public string City { get; private set; }
        public string StateProvince { get; private set; }

        private LocationDetails() { }

        private LocationDetails(string country, string city, string stateProvince)
        {
            Country = country;
            City = city;
            StateProvince = stateProvince;
        }

        public static LocationDetails Create(string country, string city, string stateProvince)
            => new LocationDetails(country, city, stateProvince);
    }

    public class AppDetails
    {
        public AccountType AccountType { get; private set; }
        public DateTime LastLogin { get; private set; }
        public DateTime JoinDate { get; private set; }
        public IEnumerable<SubscriptionDetails> Subscriptions { get; private set; }
        public InstructorDetails InstructorDetails { get; private set; }

        private AppDetails() { }

        private AppDetails(AccountType accountType, DateTime lastLogin, DateTime joinDate, IEnumerable<SubscriptionDetails> subscriptions, InstructorDetails instructorDetails)
        {
            AccountType = accountType;
            LastLogin = lastLogin;
            JoinDate = joinDate;
            Subscriptions = subscriptions;
            InstructorDetails = instructorDetails;
        }

        public static AppDetails Create(AccountType accountType, DateTime lastLogin, DateTime joinDate, IEnumerable<SubscriptionDetails> subscriptions, InstructorDetails instructorDetails)
            => new AppDetails(accountType, lastLogin, joinDate, subscriptions, instructorDetails);
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

        private SubscriptionDetails() { }

        private SubscriptionDetails(string instructorId, IEnumerable<ClassDetails> classes)
        {
            InstructorId = instructorId;
            Classes = classes;
        }

        public static SubscriptionDetails Create(string instructorId, IEnumerable<ClassDetails> classes)
            => new SubscriptionDetails(instructorId, classes);

    }

    public class InstructorDetails
    {
        public DateTime? StartDate { get; private set; }

        private InstructorDetails() { }

        private InstructorDetails(DateTime? startDate)
        {
            StartDate = startDate;
        }

        public static InstructorDetails Create(DateTime? startDate)
            => new InstructorDetails(startDate);
    }

    public class ClassDetails
    {
        public string ClassId { get; private set; }
        public bool Liked { get; private set; }

        private ClassDetails() { }

        private ClassDetails(string classId, bool liked)
        {
            ClassId = classId;
            Liked = liked;
        }

        public static ClassDetails Create(string classId, bool liked)
            => new ClassDetails(classId, liked);
    }
}
