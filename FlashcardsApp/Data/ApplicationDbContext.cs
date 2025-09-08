using FlashcardsApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Data;

//Наследуемся от DbContext - получаем все возможности EF (для базового случая).
//В нашем случае так как используем Identity, наследуемся от него и задаем параметры полей связей. Можно не указывать,
//но тогда все поля для соединения таблиц должны быть string.
public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    // Конструктор для передачи настроек подключения
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSet для каждой таблицы
    public DbSet<Card> Cards { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<CardRating> CardRatings { get; set; }

    // Метод для настройки связей между таблицами
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Вызываем настройки базового DbContext (конвенции EF (приводит свойства классов в типы для БД, переименовывает и т.д.)
        // + настройки Identity при добавлении)
        base.OnModelCreating(modelBuilder);

        // Настройка Group
        modelBuilder.Entity<Group>(entity =>
        {
            entity.Property(g => g.GroupName)
                .HasMaxLength(100).IsRequired();
            entity.Property(g => g.GroupColor)
                .HasMaxLength(20);

            entity.HasIndex(g => new
            {
                g.UserId, g.GroupName
            }).IsUnique();
        });

        // Настройка Card
        modelBuilder.Entity<Card>(entity =>
        {
            entity.Property(c => c.Question)
                .HasMaxLength(300).IsRequired();
            entity.Property(c => c.Answer)
                .HasMaxLength(2000).IsRequired();

            entity.HasIndex(c => new
            {
                c.UserId, c.Question
            }).IsUnique();
        });
        
        // Настройка системы оценок карточек (от 1 до 5)
        modelBuilder.Entity<CardRating>(entity =>
        {
            entity.ToTable(t => t.HasCheckConstraint("CK_CardRating_Rating", "\"Rating\" >= 1 AND \"Rating\" <= 5"));
        });
        
        // Настройка конфигураций связей Card->Group
        modelBuilder.Entity<Card>()
            .HasOne(c => c.Group) // У карты одна группа
            .WithMany(g => g.Cards) // У группы много карт (там массив)
            .HasForeignKey(c => c.GroupId) // Связь в Card 
            .OnDelete(DeleteBehavior.Cascade); // Если удалить группу, удалить все карточки в ней.
        // Можно написать Restrict, чтобы запретить удаление группы, если в ней есть карточки.

        // Настройка конфигурация связей Card->CardRating
        modelBuilder.Entity<Card>()
            .HasMany(c => c.Ratings) // У карты много оценок (там массив)
            .WithOne(r => r.Card) // У оценок, одна карта
            .HasForeignKey(r => r.CardId) // Связь по свойству CardId
            .OnDelete(DeleteBehavior.Cascade);

        // Настройка конфигурации связей User->Cards
        modelBuilder.Entity<User>()
            .HasMany(u => u.Cards)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Настройка конфигурации связей для User->Group
        modelBuilder.Entity<User>()
            .HasMany(u => u.Groups)
            .WithOne(g => g.User)
            .HasForeignKey(g => g.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Настройка конфигурации связей для User->CardRatings
        modelBuilder.Entity<User>()
            .HasMany(u => u.CardRatings)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
