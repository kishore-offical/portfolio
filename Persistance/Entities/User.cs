using Microsoft.AspNetCore.Identity;

namespace ChatWe.Persistance.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<IdentityUserLogin<string>> Logins { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}