using FlashcardsApp.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashcardsApp.DAL.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{

    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.Property(g => g.GroupName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(g => g.GroupColor)
            .HasMaxLength(100);

        // ДЕНОРМАЛИЗАЦИЯ - счетчик подписчиков
        builder.Property(g => g.SubscriberCount)
            .IsRequired()
            .HasDefaultValue(0)
            .HasAnnotation("CheckConstraint", "SubscriberCount >= 0");

        builder.HasIndex(g => new
            {
                g.UserId, g.GroupName
            })
            .IsUnique()
            .HasDatabaseName("IX_Groups_User_Name");

        builder.HasOne(g => g.User)
            .WithMany(u => u.Groups)
            .HasForeignKey(g => g.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(g => g.Cards)
            .WithOne(c => c.Group)
            .HasForeignKey(c => c.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(g => g.Subscriptions)
            .WithOne(ugs => ugs.Group)
            .HasForeignKey(ugs => ugs.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
