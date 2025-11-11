import type { AchievementsType, GroupType, MotivationType, SettingType, UserData } from "../types/types";
import apiFetch from "./apiFetch";

export class ApiService {
    private getToken(): string | null {
        return localStorage.getItem("accessToken");
    }

    private handleUnauthorized(error: any): void {
        if (error?.response?.status === 401) {
            localStorage.removeItem("accessToken");
            window.location.href = "/login";
        }
    }

    async getDashboard(signal?: AbortSignal): Promise<{
        user: UserData;
        groups: GroupType[];
        achievements: AchievementsType[];
    }> {
        const token = this.getToken();

        if (!token) {
            window.location.href = "/login";
            throw new Error("Не авторизован");
        }
        try {
            const res = await apiFetch.get("/User/me/dashboard", { signal });
            return {
                user: res.data,
                groups: res.data.Groups,
                achievements: res.data.Achievements
            };
        } catch (error: any) {
            this.handleUnauthorized(error);
            throw error;
        }
    }

    async getSettings(signal?: AbortSignal): Promise<SettingType> {
        const token = this.getToken();
        if (!token) {
            window.location.href = "/login";
            throw new Error("Не авторизован");
        }
        try {
            const res = await apiFetch.get("/StudySettings", { signal });
            return res.data;
        } catch (error: any) {
            this.handleUnauthorized(error);
            throw error;
        }
    }

    async getMotivation(signal?: AbortSignal): Promise<MotivationType> {
        const token = this.getToken();
        if (!token) {
            window.location.href = "/login";
            throw new Error("Не авторизован");
        }
        try {
            const res = await apiFetch.get("/UserStatistics/motivational-message", { signal });
            return res.data;
        } catch (error: any) {
            this.handleUnauthorized(error);
            throw error;
        }
    }

    async logout(signal?: AbortSignal): Promise<void> {
        await apiFetch.post("/Auth/logout", {}, { signal });
        localStorage.removeItem("accessToken");
        window.location.href = "/login";
    }

    async deleteGroup(id: string, signal?: AbortSignal): Promise<void> {
        const token = this.getToken();
        if (!token) throw new Error("Не авторизован");
        try {
            await apiFetch.delete(`/Group/${id}`, { signal });
        } catch (error: any) {
            this.handleUnauthorized(error);
            throw error;
        }
    }

    async deleteCard(id: string, signal?: AbortSignal): Promise<void> {
        const token = this.getToken();
        if (!token) throw new Error("Не авторизован");
        try {
            await apiFetch.delete(`/Cards/${id}`, { signal });
        } catch (error: any) {
            this.handleUnauthorized(error);
            throw error;
        }
    }

    async answerQuestion(id: string, rating: number, signal?: AbortSignal): Promise<any> {
        const token = this.getToken();
        if (!token) throw new Error("Не авторизован");
        try {
            const res = await apiFetch.post(`/Study/record`, { CardId: id, Rating: rating }, { signal });
            return res.data;
        } catch (error: any) {
            this.handleUnauthorized(error);
            throw error;
        }
    }
}

export const service = new ApiService();