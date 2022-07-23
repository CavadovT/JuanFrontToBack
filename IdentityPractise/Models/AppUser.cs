using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace IdentityPractise.Models
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
