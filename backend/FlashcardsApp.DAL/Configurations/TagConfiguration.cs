using FlashcardsApp.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashcardsApp.DAL.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        // Основные настройки
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Slug)
            .IsRequired()
            .HasMaxLength(50);

        // Уникальность имени и слага (чтобы не было двух тегов "IT")
        builder.HasIndex(t => t.Name).IsUnique();
        builder.HasIndex(t => t.Slug).IsUnique();

        // Настройка связи Many-to-Many с группами
        // EF Core сам создаст таблицу связей (Join Table)
        builder.HasMany(t => t.Groups)
            .WithMany(g => g.Tags)
            .UsingEntity(j => j.ToTable("GroupTags")); // Явно задаем имя промежуточной таблицы
    }
}
