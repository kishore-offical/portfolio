using Microsoft.AspNetCore.Identity;

namespace ChatWe.Persistance.Entities
{
    public class UserRole : IdentityUserRole<string>
    {
        /// <summary>
        /// Gets or sets the user associated with this user role.
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets the role associated with this user role.
        /// </summary>
        public virtual Role Role { get; set; }
    }
}