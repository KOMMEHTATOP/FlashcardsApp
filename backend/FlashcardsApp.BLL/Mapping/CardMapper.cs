using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Cards.Responses;

namespace FlashcardsApp.BLL.Mapping;

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
