using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationMVC.Entities
{
    [Table("UserTable")]
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        public string? FullName { get; set; }
        [Required]
        [MaxLength(50)]
        //[Index(IsUnique = true)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(100)]
        public string Password { get; set; }
        public bool Locked { get; set; } = false;
        public DateTime CreationTime { get; set; } = DateTime.Now;

        [Required]
        public string Role { get; set; } = "user";
    }
}
