using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bai1.Models
{
    public class LogOnModel 
    {
        [Required(ErrorMessage = "Please enter your username"), MinLength(5)]
        [Display(Name = "Username")]
        public string Username { get; set; }
        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me ?")]
        public bool RememberMe { get; set; }

    }

    public class RegisterModel 
    {
        [Required(ErrorMessage = "Please enter your username")]
        [StringLength(50,ErrorMessage ="The {0} must be at least {2}", MinimumLength = 5)]
        [Display(Name = "User Name")]
        public string Username { get; set; }


        [Required(ErrorMessage = "Please enter your email")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName ="date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        [ValidBirthDate]
        [Display(Name = "Date of Birth")]
        public DateTime BirthDate { get; set; }

        
        [Required(ErrorMessage = "Please enter your password")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).*$", 
            ErrorMessage = "Password must be include uppercase, lowcase, number and special character"
            )]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(100, ErrorMessage =" The {0} must be at least {2} characters long", MinimumLength = 6)]
        public string Password { get; set; }


        [Required(ErrorMessage = "Please confirm your password")]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

    }

    public class ValidBirthDateAttribute : ValidationAttribute {

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime birthDate) {
                if (birthDate > DateTime.Today) {
                    return new ValidationResult("Birthday cannot be in the future.");
                }
                int age = DateTime.Today.Year - birthDate.Year;
                if (age > 100) {
                    return new ValidationResult("Age cannot be greater than 100.");
                }
            }
            return ValidationResult.Success;
        }
    }

}
