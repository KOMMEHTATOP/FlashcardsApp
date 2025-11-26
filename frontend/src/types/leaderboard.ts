/**
 * Одиночная запись в таблице лидеров.
 * Используем PascalCase, так как на бэкенде PropertyNamingPolicy = null
 */
export interface LeaderboardEntryDto {
    Position: number;
    UserId: string;
    Login: string;
    TotalRating: number;
}

/**
 * Полный ответ от API
 */
export interface LeaderboardResponseDto {
    TopList: LeaderboardEntryDto[];
    TotalUsersCount: number;
}

export type LeaderboardUpdateEvent = LeaderboardResponseDto;