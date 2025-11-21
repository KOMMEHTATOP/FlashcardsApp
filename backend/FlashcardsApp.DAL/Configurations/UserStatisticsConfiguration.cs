using FlashcardsApp.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashcardsApp.DAL.Configurations;

public class UserStatisticsConfiguration : IEntityTypeConfiguration<UserStatistics>
{
    public void Configure(EntityTypeBuilder<UserStatistics> builder)
    {
        builder.HasKey(us => us.UserId);

        builder.HasOne(us => us.User)
            .WithOne(u => u.Statistics)
            .HasForeignKey<UserStatistics>(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
