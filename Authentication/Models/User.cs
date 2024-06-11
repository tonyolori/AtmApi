using Authentication.Enum;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Authentication.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public long AccountNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Pin { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long Balance { get; set; }
        public UserRole Role { get; set; }

    }
}
