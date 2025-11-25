import { ChevronRight } from "lucide-react";
import { Link } from "react-router-dom";
import type { AchievementsType } from "@/types/types";

interface ProfileAchievementsProps {
    achievements: AchievementsType[] | undefined;
}

export function ProfileAchievements({ achievements }: ProfileAchievementsProps) {
    const unlockedAchievements = achievements?.filter((a) => a.IsUnlocked) || [];
    const lockedAchievements = achievements?.filter((a) => !a.IsUnlocked) || [];

    return (
        <div className="neon-border-blue p-6">
            <div className="flex justify-between items-center mb-4">
                <h3 className="text-lg font-bold text-base-content">
                    Достижения ({unlockedAchievements.length}/{achievements?.length || 0})
                </h3>
            </div>
            <div className="flex gap-3 mb-3">
                {unlockedAchievements.slice(0, 3).map((item) => (
                    <div
                        key={item.Id}
                        className={`w-14 h-14 rounded-full bg-gradient-to-br ${item.Gradient} flex items-center justify-center text-2xl shadow-sm border-2 border-base-100`}
                        title={item.Name}
                    >
                        {item.IconUrl}
                    </div>
                ))}
                {lockedAchievements.length > 0 && (
                    <div className="w-14 h-14 rounded-full bg-base-300 flex items-center justify-center border-2 border-base-200">
                        <span className="text-base-content/30 text-xl">🔒</span>
                    </div>
                )}
            </div>
            <Link
                to="/"
                onClick={() => localStorage.setItem('activeTab', '2')}
                className="text-primary hover:text-primary-focus hover:underline text-sm flex items-center gap-1 transition-colors"
            >
                Смотреть все <ChevronRight className="w-4 h-4" />
            </Link>
        </div>
    );
}
