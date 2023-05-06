using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace Wallet.Models
{
    public class Transaction
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Type { get; set; }

        public string? State { get; set; }
        public string? TxnId { get; set; }
        public DateTime TxnTime { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public decimal Amount { get; set; }
    }
}
