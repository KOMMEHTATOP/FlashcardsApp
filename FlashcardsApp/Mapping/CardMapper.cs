using FlashcardsApp.Models;
using FlashcardsAppContracts.DTOs.Cards.Responses;
using FlashcardsAppContracts.DTOs.Responses;

namespace FlashcardsApp.Mapping;

public static class CardMapper
{
    public static ResultCardDto ToDto(this Card model)
    {
        return new ResultCardDto()
        {
            CardId = model.CardId,
            GroupId = model.GroupId,
            Question = model.Question,
            Answer = model.Answer,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
            LastRating = 0
        };
    }
}
