using Microsoft.AspNetCore.Identity;
using System.Reflection;
using UserApi.Models;
using static UserApi.Models.dtoNewUser;

namespace UserApi.Data.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public DateOnly BirthDate { get; set; }
        public Gender SelectGender { get; set; }
    }
}
