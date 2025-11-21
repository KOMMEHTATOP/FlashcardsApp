using FlashcardsApp.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashcardsApp.DAL.Configurations;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.Property(c => c.Question)
            .HasMaxLength(5000)
            .IsRequired();

        builder.Property(c => c.Answer)
            .HasMaxLength(10000)
            .IsRequired();

        builder.HasIndex(c => new { c.GroupId, c.Question })
            .IsUnique()
            .HasDatabaseName("IX_Cards_Group_Question");

        builder.HasOne(c => c.Group)
            .WithMany(g => g.Cards)
            .HasForeignKey(c => c.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
