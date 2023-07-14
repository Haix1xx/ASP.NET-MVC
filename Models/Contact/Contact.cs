using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models.Contact
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(60)]
        [Required(ErrorMessage = "*Required")]
        [DisplayName("Full Name")]
        public string FullName { get; set; } = "";

        [EmailAddress(ErrorMessage = "*Wrong Format")]
        [Required(ErrorMessage = "*Required")]
        public string Email { get; set; } = "";

        public DateTime? SentDate { get; set; }
        public string? Message { get; set; }

        [StringLength(20)]
        [Phone(ErrorMessage = "*Wrong Format")]
        public string? PhoneNumber { get; set; }

    }
}
