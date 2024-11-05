using Microsoft.AspNetCore.Identity;

namespace Restaurant_API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
