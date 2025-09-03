using FlashcardsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsApp.Data;

//Наследуемся от DbContext - получаем все возможности EF
public class ApplicationDbContext : DbContext
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
        // Настройка Group
        modelBuilder.Entity<Group>(entity =>
        {
            entity.Property(g => g.GroupName)
                .HasMaxLength(100).IsRequired();
        });
        
        // Настройка Card
        modelBuilder.Entity<Card>(entity =>
        {
            entity.Property(c => c.Question)
                .HasMaxLength(300).IsRequired();
            entity.Property(c => c.Answer)
                .HasMaxLength(2000).IsRequired();
        });
        
        // Настройка конфигураций связей Card->Group
        modelBuilder.Entity<Card>()
            .HasOne(c => c.Group) // У карты одна группа
            .WithMany(g => g.Cards) // У группы много карт (там массив)
            .HasForeignKey(c => c.GroupId); // Связь в Card 
        
        // Настройка конфигурация связей Card->CardRating
        modelBuilder.Entity<Card>()
            .HasMany(c => c.Ratings) // У карты много оценок (там массив)
            .WithOne(r => r.Card) // У оценок, одна карта
            .HasForeignKey(r => r.CardId); // Связь по свойству CardId
        
        
        // Вызываем настройки базового DbContext (конвенции EF (приводит свойства классов в типы для БД, переименовывает и т.д.) + настройки Identity при добавлении)
        base.OnModelCreating(modelBuilder);
    }
}
