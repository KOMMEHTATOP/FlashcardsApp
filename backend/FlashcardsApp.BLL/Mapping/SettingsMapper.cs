using FlashcardsApp.DAL.Models;
using FlashcardsApp.Models.DTOs.Responses;

namespace FlashcardsApp.BLL.Mapping;

public static class SettingsMapper
{
    public static ResultSettingsDto ToDto(this StudySettings studySettings)
    {
        return new ResultSettingsDto()
        {
            StudyOrder = studySettings.StudyOrder,
            MinRating = studySettings.MinRating,
            MaxRating = studySettings.MaxRating,
            CompletionThreshold = studySettings.CompletionThreshold,
            ShuffleOnRepeat = studySettings.ShuffleOnRepeat,
        };
    }
}
