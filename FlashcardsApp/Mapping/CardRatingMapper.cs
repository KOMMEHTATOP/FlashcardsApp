using FlashcardsApp.Models;
using FlashcardsAppContracts.DTOs.Responses;

namespace FlashcardsApp.Mapping;

public static class CardRatingMapper
{
    public static ResultCardRating ToDto(this CardRating model)
    {
        return new ResultCardRating
        {
            CardId = model.CardId, 
            UserId = model.UserId,
            RatingId = model.Id,
            Rating = model.Rating, 
            CreatedAt = model.CreatedAt,
        };
    }
}
