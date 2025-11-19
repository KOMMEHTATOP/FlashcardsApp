using System.Linq.Expressions;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Groups.Responses;
using FlashcardsApp.Models.DTOs.Tags;

namespace FlashcardsApp.BLL.Mapping;

public static class GroupMapper
{
    /// <summary>
    /// Маппинг для загруженной сущности (когда используем FindAsync или получили объект после SaveChanges)
    /// </summary>
    public static ResultGroupDto ToDto(this Group model)
    {
        return new ResultGroupDto
        {
            Id = model.Id,
            GroupName = model.GroupName,
            GroupColor = model.GroupColor,
            GroupIcon = model.GroupIcon,
            IsPublished = model.IsPublished,
            CreatedAt = model.CreatedAt,
            Order = model.Order,
            SubscriberCount = model.SubscriberCount,
            CardCount = model.Cards?.Count ?? 0,
            
            // --- МАППИНГ ТЕГОВ ---
            Tags = model.Tags?.Select(t => new GroupTagDto
            {
                Id = t.Id,
                Name = t.Name,
                Slug = t.Slug,
                Color = t.Color
            }).ToList() ?? new List<GroupTagDto>()
        };
    }

    /// <summary>
    /// Проекция для EF Core (SQL генерация)
    /// </summary>
    public static Expression<Func<Group, ResultGroupDto>> ToDtoExpression()
    {
        return g => new ResultGroupDto
        {
            Id = g.Id,
            GroupName = g.GroupName,
            GroupColor = g.GroupColor,
            GroupIcon = g.GroupIcon,
            IsPublished = g.IsPublished,
            CreatedAt = g.CreatedAt,
            Order = g.Order,
            SubscriberCount = g.SubscriberCount,
            CardCount = g.Cards!.Count, // COUNT(*)
            
            // --- ПРОЕКЦИЯ ТЕГОВ ---
            // EF Core превратит это в эффективный JOIN
            Tags = g.Tags.Select(t => new GroupTagDto
            {
                Id = t.Id,
                Name = t.Name,
                Slug = t.Slug,
                Color = t.Color
            }).ToList()
        };
    }
}