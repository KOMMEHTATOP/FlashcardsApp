export interface LeaderboardEntryDto {
    Position: number;
    UserId: string;
    Login: string;
    TotalRating: number;
}

export interface LeaderboardResponseDto {
    TopList: LeaderboardEntryDto[];
    TotalUsersCount: number;
}

export type LeaderboardUpdateEvent = LeaderboardResponseDto;