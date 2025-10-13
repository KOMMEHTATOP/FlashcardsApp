using FlashcardsApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashcardsApp.Data.Configurations;

public class UserStatisticsConfiguration : IEntityTypeConfiguration<UserStatistics>
{
    public void Configure(EntityTypeBuilder<UserStatistics> builder)
    {
        // Первичный ключ
        builder.HasKey(us => us.UserId);

        // Связь One-to-One с User
        builder.HasOne(us => us.User)
            .WithOne(u => u.Statistics)
            .HasForeignKey<UserStatistics>(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
