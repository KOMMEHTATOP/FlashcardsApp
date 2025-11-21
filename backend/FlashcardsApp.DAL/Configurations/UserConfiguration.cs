using FlashcardsApp.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashcardsApp.DAL.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.Login)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(u => u.Role)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(u => u.TotalRating)
            .IsRequired()
            .HasDefaultValue(0)
            .HasAnnotation("CheckConstraint", "TotalRating >= 0");
            
        builder.HasIndex(u => u.Login)
            .IsUnique()
            .HasDatabaseName("IX_Users_Login");
            
        builder.HasMany(u => u.Subscriptions)
            .WithOne(ugs => ugs.SubscriberUser)
            .HasForeignKey(ugs => ugs.SubscriberUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
