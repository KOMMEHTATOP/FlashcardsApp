using FlashcardsApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashcardsApp.Data.Configurations;

public class StudyHistoryConfiguration : IEntityTypeConfiguration<StudyHistory>
{
    public void Configure(EntityTypeBuilder<StudyHistory> builder)
    {
        // Constraint: оценка от 1 до 5
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_StudyHistory_Rating", 
            "\"Rating\" >= 1 AND \"Rating\" <= 5"));

        // Индекс для быстрого поиска
        builder.HasIndex(sh => new { sh.UserId, sh.CardId, sh.StudiedAt })
            .HasDatabaseName("IX_StudyHistory_User_Card_Date");

        // Связь с User 
        builder.HasOne(sh => sh.User)
            .WithMany(u => u.StudyHistory)
            .HasForeignKey(sh => sh.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь с Card 
        builder.HasOne(sh => sh.Card)
            .WithMany()
            .HasForeignKey(sh => sh.CardId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
