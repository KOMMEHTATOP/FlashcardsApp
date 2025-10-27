using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Groups.Responses;

namespace FlashcardsApp.BLL.Mapping;

public static class GroupMapper
{
    public static ResultGroupDto ToDto(this Group model)
    {
        return new ResultGroupDto()
        {
            Id = model.Id,
            GroupName = model.GroupName,
            GroupColor = model.GroupColor,
            GroupIcon = model.GroupIcon,
            CreatedAt = model.CreatedAt,
            Order = model.Order,
            CardCount = model.Cards?.Count ?? 0
        };
    }
}
