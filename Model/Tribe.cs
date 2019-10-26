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
    }
}
