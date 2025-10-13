using FlashcardsApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashcardsApp.Data.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        // Настройка свойств
        builder.Property(g => g.GroupName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(g => g.GroupColor)
            .HasMaxLength(20);

        // Уникальный индекс
        builder.HasIndex(g => new { g.UserId, g.GroupName })
            .IsUnique()
            .HasDatabaseName("IX_Groups_User_Name");

        // Связь с User
        builder.HasOne(g => g.User)
            .WithMany(u => u.Groups)
            .HasForeignKey(g => g.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
