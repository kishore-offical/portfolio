using ChatWe.Persistance.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatWe.Persistance.Context
{
    public interface IChatWeContext
    {
        DbSet<User> User { get; set; }
        DbSet<Attachments> Attachments { get; set; }
        DbSet<Conversations> Conversations { get; set; }
        DbSet<Group> Group { get; set; }

        Task<int> SaveChangesAsync();
    }
}