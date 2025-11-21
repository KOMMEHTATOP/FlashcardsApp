using System.Linq.Expressions;
using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Groups.Responses;
using FlashcardsApp.Models.DTOs.Tags;

namespace FlashcardsApp.BLL.Mapping;

public static class GroupMapper
{
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
            
            Tags = model.Tags?.Select(t => new GroupTagDto
            {
                Id = t.Id,
                Name = t.Name,
                Slug = t.Slug,
                Color = t.Color
            }).ToList() ?? new List<GroupTagDto>()
        };
    }
    
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
            CardCount = g.Cards!.Count, 
            
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