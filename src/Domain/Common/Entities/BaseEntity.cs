using Microsoft.AspNetCore.Identity;

namespace Domain.Common.Entities;
public interface IBaseEntity
{
    public DateTime CreatedDate { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}
public abstract class BaseEntity : IdentityUser, IBaseEntity
{
    public DateTime CreatedDate { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}
