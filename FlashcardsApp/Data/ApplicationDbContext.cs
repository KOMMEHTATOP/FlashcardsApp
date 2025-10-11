using FlashcardsApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Data;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    // Конструктор для передачи настроек подключения
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSet для каждой таблицы
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<CardRating> CardRatings { get; set; }
    public DbSet<StudySettings> StudySettings { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<UserAchievement> UserAchievements { get; set; }
    public DbSet<UserStatistics> UserStatistics { get; set; }
    

    // Метод для настройки связей между таблицами
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Вызываем настройки базового DbContext (конвенции EF (приводит свойства классов в типы для БД, переименовывает и т.д.)
        // + настройки Identity при добавлении)
        base.OnModelCreating(modelBuilder);

        // Настройка RefreshToken
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasIndex(r => r.Token).IsUnique();
    
            entity.HasOne(r => r.User)
                .WithMany() // У User нет навигационного свойства для RefreshTokens
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade); // При удалении пользователя удаляем все его токены
        });
        
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

        //Настройка конфигураций связей Group->StudySettings
        modelBuilder.Entity<StudySettings>()
            .HasOne(s => s.Group)
            .WithOne(g => g.StudySettings)
            .HasForeignKey<StudySettings>(s => s.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

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
        
        //Настройка конфигураций связей для User->Achievments
        modelBuilder.Entity<UserStatistics>()
            .HasKey(us => us.UserId);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Statistics)
            .WithOne(us => us.User)
            .HasForeignKey<UserStatistics>(us => us.UserId);
        
        
        // Настройка UserAchievement (составной ключ)
        modelBuilder.Entity<UserAchievement>()
            .HasKey(ua => new { ua.UserId, ua.AchievementId });

        // Связь User -> UserAchievement
        modelBuilder.Entity<UserAchievement>()
            .HasOne(ua => ua.User)
            .WithMany(u => u.UserAchievements)
            .HasForeignKey(ua => ua.UserId);

        // Связь Achievement -> UserAchievement
        modelBuilder.Entity<UserAchievement>()
            .HasOne(ua => ua.Achievement)
            .WithMany(a => a.UserAchievements)
            .HasForeignKey(ua => ua.AchievementId);
        
    }
}
