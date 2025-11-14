using System.Linq.Expressions;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Groups.Responses;

namespace FlashcardsApp.BLL.Mapping;

public static class GroupMapper
{
    /// <summary>
    /// Маппинг для загруженной сущности (после ToListAsync)
    /// Используется когда данные уже в памяти
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
            CardCount = model.Cards?.Count ?? 0
        };
    }

    /// <summary>
    /// Expression для проекции в EF Core запросах (до ToListAsync)
    /// Позволяет EF Core генерировать оптимальный SQL с COUNT вместо загрузки всех карточек
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
            CardCount = g.Cards!.Count  // EF Core превратит в COUNT(*) в SQL
        };
    }
}
