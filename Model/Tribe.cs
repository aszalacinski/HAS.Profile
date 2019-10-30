using System;
using System.Collections.Generic;
using System.Linq;
using static HAS.Profile.Feature.Tribe.AddStudentToTribe;
using static HAS.Profile.Feature.Tribe.DeleteStudentFromTribe;
using static HAS.Profile.Feature.Tribe.UpdateTribe;
using static HAS.Profile.Feature.Tribe.UpdateTribeToNonSubscription;
using static HAS.Profile.Feature.Tribe.UpdateTribeToSubscription;

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

        public bool Handle(UpdateTribeToSubscriptionCommand cmd)
        {
            return SetToSubscription();
        }

        public bool Handle(UpdateTribeToNonSubscriptionCommand cmd)
        {
            return SetToNonSubscription();
        }

        public bool Handle(AddStudentToTribeCommand cmd)
        {
            if(IsStudentTribe())
            {
                return AddMember(cmd.StudentId);
            }

            return false;
        }

        public bool Handle(DeleteStudentFromTribeCommand cmd)
        {
            if(cmd.InstructorId != cmd.StudentId)
            {
                return RemoveMember(cmd.StudentId);
            }

            return false;
        }

        public bool Handle(UpdateTribeCommand cmd)
        {
            Name = cmd.Name;
            Description = cmd.Description;

            return Name == cmd.Name && Description == cmd.Description;
        }

        private bool SetToNonSubscription()
        {
            IsSubscription = false;
            return IsSubscription != true;
        }

        private bool SetToSubscription()
        {
            IsSubscription = true;
            return IsSubscription == true;
        }

        private bool AddMember(string profileId)
        {
            if(!Members.Any(x => x.Id == profileId))
            {
                var list = Members.ToList();
                list.Add(Member.Create(profileId, DateTime.UtcNow));
                Members = list;
            }

            return Members.Any(x => x.Id == profileId);
        }

        private bool RemoveMember(string profileId)
        {
            if(Members.Any(x => x.Id == profileId))
            {
                var list = Members.ToList();
                var index = list.FindIndex(x => x.Id == profileId);
                list.Remove(list[index]);
                Members = list;
            }

            return !Members.Any(x => x.Id == profileId);
        }

        private bool IsStudentTribe() => Type == TribeType.STUDENT;
        private bool IsSoloTribe() => Type == TribeType.SOLO;


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
