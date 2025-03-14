using System.ComponentModel.DataAnnotations;

namespace UserApi.Models
{
    public enum Gender
    {
        Male,
        Female
    }

    public class dtoNewUser
    {

            [Required(ErrorMessage = "Name is required.")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress]
            public string Email { get; set; }
            [Required(ErrorMessage = "Phone Number Is Required")]
            public string PhoneNumber { get; set; }
            [Required(ErrorMessage = "Enter Your BirthDate")]

            public DateOnly BirthDate { get; set; }
            [Required(ErrorMessage = "Choose A Valid Gender")]
            public Gender SelectGender { get; set; }
            //public List<SelectListItem> GenderList { get; set; }

            [Required(ErrorMessage = "Password is required.")]
            [StringLength(40, MinimumLength = 8, ErrorMessage = "The {0} must be at {2} and at max {1} characters long.")]
            [DataType(DataType.Password)]
            [Compare("ConfirmPassword", ErrorMessage = "Password does not match.")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Confirm Password is required.")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            public string ConfirmPassword { get; set; }

    }
}
