using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace Wallet.Models
{
    public class BankAccount
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? State { get; set; }
        public string? UserId{ get; set; }
        public string? Currency { get; set; }
        public string? AccNumber { get; set; }
        public decimal Balance { get; set; }
    }
}
