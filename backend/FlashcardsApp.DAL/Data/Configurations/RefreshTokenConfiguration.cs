using FlashcardsApp.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashcardsApp.DAL.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        // Уникальный индекс на токен
        builder.HasIndex(r => r.Token)
            .IsUnique()
            .HasDatabaseName("IX_RefreshTokens_Token");

        // Связь с User
        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
