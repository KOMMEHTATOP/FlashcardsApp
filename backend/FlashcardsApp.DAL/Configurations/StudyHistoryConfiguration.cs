using FlashcardsApp.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashcardsApp.DAL.Configurations;

public class StudyHistoryConfiguration : IEntityTypeConfiguration<StudyHistory>
{
    public void Configure(EntityTypeBuilder<StudyHistory> builder)
    {
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_StudyHistory_Rating", 
            "\"Rating\" >= 1 AND \"Rating\" <= 5"));

        builder.HasIndex(sh => new { sh.UserId, sh.CardId, sh.StudiedAt })
            .HasDatabaseName("IX_StudyHistory_User_Card_Date");

        builder.HasOne(sh => sh.User)
            .WithMany(u => u.StudyHistory)
            .HasForeignKey(sh => sh.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sh => sh.Card)
            .WithMany()
            .HasForeignKey(sh => sh.CardId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
