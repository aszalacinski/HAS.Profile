using System;
using System.Collections.Generic;

namespace HAS.Profile.Model
{
    public class Tribe : IEntity
    {
        public string Id { get; private set; }
        public string InstructorId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public TribeType Type { get; private set; }
        public bool IsSubscription { get; private set; }
        public IEnumerable<Member> Members { get; private set; }

        private Tribe() { }

        private Tribe(string id, string instructorId, string name, string description, DateTime createdDate, TribeType type, bool isSubscription, IEnumerable<Member> members)
        {
            Id = id;
            InstructorId = instructorId;
            Name = name;
            Description = description;
            CreatedDate = createdDate;
            Type = type;
            IsSubscription = isSubscription;
            Members = members;
        }

        public static Tribe Create(string id, string instructorId, string name, string description, DateTime createdDate, TribeType type, bool isSubscription, IEnumerable<Member> members)
            => new Tribe(id, instructorId, name, description, createdDate, type, isSubscription, members);
    }

    public enum TribeType
    {
        NONE = 0,
        STUDENT,
        SOLO,
        ARCHIVED
    }

    public class Member
    {
        public string Id { get; private set; }
        public DateTime JoinDate { get; private set; }

        private Member() { }

        private Member(string id, DateTime joinDate)
        {
            Id = id;
            JoinDate = joinDate;
        }

        public static Member Create(string id, DateTime joinDate)
            => new Member(id, joinDate);
    }
}
