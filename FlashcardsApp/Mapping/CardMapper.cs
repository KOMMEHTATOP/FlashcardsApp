using FlashcardsApp.Models;
using FlashcardsApp.Models.DTOs;

namespace FlashcardsApp.Mapping;

public static class CardMapper
{
    public static ResultCardDto ToDto(this Card model)
    {
        return new ResultCardDto()
        {
            CardId = model.Id,
            GroupId = model.GroupId,
            Question = model.Question,
            Answer = model.Answer,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        };
    }
}
