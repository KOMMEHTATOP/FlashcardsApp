import type { AchievementsType, GroupType, MotivationType, SettingType, UserData } from "../types/types";
import apiFetch from "./apiFetch";


export class ApiService {
    async getDashboard(): Promise<{
        user: UserData;
        groups: GroupType[];
        achievements: AchievementsType[];
    }> {

        const res = await apiFetch.get("/User/me/dashboard");
        return {
            user: res.data,
            groups: res.data.Groups,
            achievements: res.data.Achievements
        };
    }

    async getSettings(): Promise<SettingType> {
        const res = await apiFetch.get("/StudySettings");
        return res.data;
    }

    async getMotivation(): Promise<MotivationType> {
        const res = await apiFetch.get("/UserStatistics/motivational-message");
        return res.data;
    }

    async logout(): Promise<void> {
        await apiFetch.post("/Auth/logout");
        localStorage.removeItem("accessToken");
        window.location.href = "/login";
    }

    async deleteGroup(id: string): Promise<void> {
        await apiFetch.delete(`/Group/${id}`);
    }

    async deleteCard(id: string): Promise<void> {
        await apiFetch.delete(`/Cards/${id}`);
    }

    async answerQuestion(id: string, rating: number): Promise<any> {
        const res = await apiFetch.post(`/Study/record`, { CardId: id, Rating: rating });
        return res.data;
    }
}

export const service = new ApiService();