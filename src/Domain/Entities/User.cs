using Domain.Common.Entities;
using Domain.Enum;

namespace Domain.Entities
{
    public class User : BaseEntity
    {
        public long AccountNumber { get; set; }
        public string? Pin { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public long Balance { get; set; }
        public Status Status { get; set; }
        public string? StatusDesc { get; set; }
    }
}
