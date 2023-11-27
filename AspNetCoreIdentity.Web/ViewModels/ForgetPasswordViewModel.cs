﻿using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Web.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [EmailAddress(ErrorMessage = "Email formatı yanlış")]
        [Required(ErrorMessage = "Email alanı boş bırakılamaz!")]
        [Display(Name = "Email :")]
        public string Email { get; set; } = null!;

        [Display(Name = "Mesaj :")]
        public string Message { get; set; } = null!;
    }
}
