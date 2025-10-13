using FlashcardsApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashcardsApp.Data.Configurations;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        // Настройка свойств
        builder.Property(c => c.Question)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(c => c.Answer)
            .HasMaxLength(2000)
            .IsRequired();

        // Уникальный индекс
        builder.HasIndex(c => new { c.UserId, c.Question })
            .IsUnique()
            .HasDatabaseName("IX_Cards_User_Question");

        // Связь с User
        builder.HasOne(c => c.User)
            .WithMany(u => u.Cards)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь с Group
        builder.HasOne(c => c.Group)
            .WithMany(g => g.Cards)
            .HasForeignKey(c => c.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь с Ratings
        builder.HasMany(c => c.Ratings)
            .WithOne(r => r.Card)
            .HasForeignKey(r => r.CardId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
