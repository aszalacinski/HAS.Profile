using HAS.Profile.Model;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace HAS.Profile.Data
{
    public class TribeContext
    {
        private readonly DbContext _db;
        private IMongoCollection<TribeDAO> _profile;

        public IMongoCollection<TribeDAO> Profile { get; }

        public TribeContext(IConfiguration configuration)
        {
            _db = DbContext.Create("my-practice", configuration["MongoDB:Tribe:ConnectionString"]);
            _profile = _db.Database.GetCollection<TribeDAO>("tribe");
            Profile = _profile;
        }

        [BsonIgnoreExtraElements]
        public class TribeDAO
        {
            [BsonId]
            [BsonElement("_id")]
            public ObjectId Id { get; set; }

            [BsonRepresentation(BsonType.ObjectId)]
            [BsonElement("i_id")]
            public string InstructorId { get; set; }

            [BsonElement("name")]
            public string Name { get; set; }

            [BsonElement("desc")]
            public string Description { get; set; }

            [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
            [BsonElement("cdate")]
            public DateTime CreatedDate { get; set; }

            [BsonRepresentation(BsonType.String)]
            [BsonElement("type")]
            public TribeType Type { get; set; }

            [BsonRepresentation(BsonType.Boolean)]
            [BsonElement("isSub")]
            public bool IsSubscription { get; set; }

            [BsonElement("members")]
            public IEnumerable<MemberDAO> Members { get; set; }
        }

        [BsonIgnoreExtraElements]
        public class MemberDAO
        {
            [BsonRepresentation(BsonType.ObjectId)]
            [BsonElement("_id")]
            public ObjectId Id { get; set; }

            [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
            [BsonElement("jdate")]
            public DateTime JoinDate { get; set; }
        }
    }
}
