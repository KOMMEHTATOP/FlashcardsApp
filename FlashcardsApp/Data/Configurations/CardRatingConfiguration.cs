using FlashcardsApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashcardsApp.Data.Configurations;

public class CardRatingConfiguration : IEntityTypeConfiguration<CardRating>
{
    public void Configure(EntityTypeBuilder<CardRating> builder)
    {
        // Constraint: оценка от 1 до 5
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_CardRating_Rating", 
            "\"Rating\" >= 1 AND \"Rating\" <= 5"));

        // Связь с User
        builder.HasOne(r => r.User)
            .WithMany(u => u.CardRatings)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь с Card
        builder.HasOne(r => r.Card)
            .WithMany(c => c.Ratings)
            .HasForeignKey(r => r.CardId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
