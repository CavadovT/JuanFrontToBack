using System.ComponentModel.DataAnnotations;

namespace IdentityPractise.ViewModels
{
    public class RegisterVM
    {
        [Required,StringLength(50)]
        public string FullName { get; set; }

        [Required, StringLength(50)]
        public string UserName { get; set; }

        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required,DataType(DataType.Password)]
        public string Password { get; set; }

        [Required,DataType(DataType.Password),Compare(nameof(Password))]
        public string RepeatPassword { get; set; }
    }
}
