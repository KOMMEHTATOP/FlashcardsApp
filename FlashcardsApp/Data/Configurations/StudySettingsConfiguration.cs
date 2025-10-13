using FlashcardsApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashcardsApp.Data.Configurations;

public class StudySettingsConfiguration : IEntityTypeConfiguration<StudySettings>
{
    public void Configure(EntityTypeBuilder<StudySettings> builder)
    {
        // Уникальный индекс: один пользователь = одна глобальная настройка
        builder.HasIndex(s => s.UserId)
            .IsUnique()
            .HasDatabaseName("IX_StudySettings_UserId");

        // Связь с User
        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
