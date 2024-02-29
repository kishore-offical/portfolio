using ChatWe.Persistance.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatWe.Persistance.Configurations
{
    public class CustomConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.Property(x => x.PhoneNumber).HasMaxLength(20);
            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(50);
            builder.HasMany(u => u.Logins).WithOne().HasForeignKey(ul => ul.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class RoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
        {
            builder.ToTable("RoleClaims");
        }
    }

    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasOne(ur => ur.Role)
              .WithMany(r => r.UserRoles)
              .HasForeignKey(ur => ur.RoleId)
              .IsRequired();

            builder.HasOne(ur => ur.User)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
        }
    }

    public class UserClaimConfiguration : IEntityTypeConfiguration<IdentityUserClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserClaim<string>> builder)
        {
            builder.ToTable("UserClaims");
        }
    }

    public class UserLoginConfiguration : IEntityTypeConfiguration<IdentityUserLogin<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> builder)
        {
            builder.ToTable("UserLogins");
        }
    }

    public class UserTokenConfiguration : IEntityTypeConfiguration<IdentityUserToken<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserToken<string>> builder)
        {
            builder.ToTable("UserTokens");
        }
    }

    public class ConversationsConfiguration : IEntityTypeConfiguration<Conversations>
    {
        public void Configure(EntityTypeBuilder<Conversations> builder)
        {
            builder.ToTable("Conversations");
            builder.HasKey(a => a.Id);
            builder.Property(x => x.GroupId).IsRequired(false);
            builder.Property(x => x.AttachmentId).IsRequired(false);
            builder.HasMany(u => u.Attachments).WithOne(x => x.Conversation).HasForeignKey(ul => ul.AttachmentId).IsRequired(false).OnDelete(DeleteBehavior.NoAction);
        }
    }

    public class AttachmentConfiguration : IEntityTypeConfiguration<Attachments>
    {
        public void Configure(EntityTypeBuilder<Attachments> builder)
        {
            builder.ToTable("Attachments");
            builder.HasKey(a => a.AttachmentId);
        }
    }

    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.ToTable("Group");
            builder.HasKey(a => a.Id);
            builder.Property(x => x.Name).IsRequired(false);
            builder.Property(x => x.IsActive);
        }
    }
}