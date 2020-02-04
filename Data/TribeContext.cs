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
        private IMongoCollection<TribeDAO> _tribe;

        public IMongoCollection<TribeDAO> Tribe { get; }

        public TribeContext(IConfiguration configuration)
        {
            _db = DbContext.Create("my-practice", configuration["MongoDB:Tribe:ConnectionString"]);
            _tribe = _db.Database.GetCollection<TribeDAO>("tribe");
            Tribe = _tribe;
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

            [BsonElement("subDets")]
            public TribeSubscriptionDetailsDAO SubscriptionDetails { get; set; }

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

        [BsonIgnoreExtraElements]
        public class TribeSubscriptionDetailsDAO
        {
            [BsonElement("rates")]
            public List<SubscriptionRateDAO> Rates { get; set; }
        }

        [BsonIgnoreExtraElements]
        public class SubscriptionRateDAO
        {
            [BsonElement("rate")]
            public int Rate { get; set; }

            [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
            [BsonElement("udate")]
            public DateTime UpdatedDate { get; set; }
        }
    }
}
