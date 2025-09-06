using FlashcardsApp.Models;
using FlashcardsApp.Models.DTOs;

namespace FlashcardsApp.Mapping;

public static class GroupMapper
{
    public static ResultGroupDto ToDto(this Group model)
    {
        return new ResultGroupDto()
        {
            Id = model.Id,
            GroupName = model.GroupName, 
            GroupColor = model.GroupColor,
            CreatedAt = model.CreatedAt,
        };
    }
}
