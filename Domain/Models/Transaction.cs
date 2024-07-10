using Domain.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public long? SendingAccount { get; set; }
        public long ReceivingAccount { get; set; }


        [EnumDataType(typeof(TransactionType))]
        [Column(TypeName = "varchar(20)")] // Adjust the size as per your enum values
        public TransactionType Type { get; set; }
        public long Amount { get; set; }
        public bool Status { get; set; }
    }
}
