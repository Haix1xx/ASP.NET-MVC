using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
    public class AppUser : IdentityUser
    {
        [Column(TypeName = "nvarchar")]
        [StringLength(200)]
        public string? HomeAdress { get; set; } = default!;

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; } = default!;


    }
}
