using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
    public class AppUser : IdentityUser
    {
        [Column(TypeName = "nvarchar")]
        [StringLength(200)]
        public string HomeAddress { get; set; } = "";

        [DataType(DataType.Date)]
        public DateTime? BirthDay { get; set; }


    }
}
