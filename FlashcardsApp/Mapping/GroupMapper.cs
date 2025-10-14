using FlashcardsApp.Models;
using FlashcardsAppContracts.DTOs.Responses;
using FlashcardsAppContracts.Constants;
using FlashcardsAppContracts.DTOs.Groups.Responses;

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
            Order = model.Order
        };
    }
}
