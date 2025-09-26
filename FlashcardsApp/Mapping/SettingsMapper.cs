using FlashcardsApp.Models;
using FlashcardsAppContracts.DTOs.Responses;

namespace FlashcardsApp.Mapping;

public static class SettingsMapper
{
    public static ResultSettingsDto ToDto(this StudySettings studySettings)
    {
        return new ResultSettingsDto()
        {
            StudySettingsId = studySettings.StudySettingsId,
            GroupId = studySettings.GroupId,
            StudyOrder = studySettings.StudyOrder,
            PresetName = studySettings.PresetName,
            MinRating = studySettings.MinRating,
            MaxRating = studySettings.MaxRating,
        };
    }
}
