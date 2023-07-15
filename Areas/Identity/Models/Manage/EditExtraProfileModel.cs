using System;
using System.ComponentModel.DataAnnotations;

namespace MVC.Areas.Identity.Models.Manage
{
  public class EditExtraProfileModel
  {
      [Display(Name = "Tên tài khoản")]
      public string UserName { get; set; } = default!;

      [Display(Name = "Địa chỉ email")]
      public string UserEmail { get; set; }= default!;
      [Display(Name = "Số điện thoại")]
      public string PhoneNumber { get; set; } = default!;

      [Display(Name = "Địa chỉ")]
      [StringLength(400)]
      public string HomeAdress { get; set; } = default!;


        [Display(Name = "Ngày sinh")]
      public DateTime? BirthDate { get; set; }
  }
}