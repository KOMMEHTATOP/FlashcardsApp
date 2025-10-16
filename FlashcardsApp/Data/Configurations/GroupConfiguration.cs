using FlashcardsApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashcardsApp.Data.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.Property(g => g.GroupName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(g => g.GroupColor)
            .HasMaxLength(100);

        builder.HasIndex(g => new { g.UserId, g.GroupName })
            .IsUnique()
            .HasDatabaseName("IX_Groups_User_Name");

        // Связь с User
        builder.HasOne(g => g.User)
            .WithMany(u => u.Groups)
            .HasForeignKey(g => g.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(g => g.Cards)
            .WithOne(c => c.Group)
            .HasForeignKey(c => c.GroupId)   
            .OnDelete(DeleteBehavior.Cascade);
    }
}
