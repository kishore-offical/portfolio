using Microsoft.AspNetCore.Identity;

namespace ChatWe.Persistance.Entities
{
    public class Role : IdentityRole
    {
        /// <summary>
        /// Gets or sets the collection of user roles associated with this role.
        /// </summary>
        public ICollection<UserRole> UserRoles { get; set; }
    }
}