import { motion } from "framer-motion";
import { useData } from "../context/DataContext";
import { Helmet } from "react-helmet-async";
import { ProfileHeader } from "../components/profile/ProfileHeader";
import { ProfileStats } from "../components/profile/ProfileStats";
import { ProfileStreak } from "../components/profile/ProfileStreak";
import { ProfileAchievements } from "../components/profile/ProfileAchievements";
import { ProfileDecks } from "../components/profile/ProfileDecks";
import { ProfileHistory } from "../components/profile/ProfileHistory";

export function ProfilePage() {
    const { user, achivment, groups } = useData();
    const currentXP = user?.Statistics?.XPProgressInCurrentLevel ?? 0;
    const xpForNextLevel = user?.Statistics?.XPRequiredForCurrentLevel ?? 0;
    const progressPercent = xpForNextLevel > 0 ? Math.round((currentXP / xpForNextLevel) * 100) : 0;
    const level = user?.Statistics?.Level || 0;

    return (
        <div className="w-full pb-10">
            <Helmet>
                <title>Мой профиль | FlashcardsLoop</title>
            </Helmet>

            <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.4 }}
                className="space-y-6"
            >
                {/* I. ХЕДЕР - Профиль и Уровень */}
                <ProfileHeader
                    user={user}
                    level={level}
                    progressPercent={progressPercent}
                />

                {/* II. ЦЕНТРАЛЬНЫЙ БЛОК - Статистика */}
                <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                    {/* Левая часть - Плитки статистики */}
                    <ProfileStats
                        user={user}
                        groupsCount={groups?.length || 0}
                    />

                    {/* Правая часть - Streak и Достижения */}
                    <div className="space-y-6">
                        <ProfileStreak user={user} />
                        <ProfileAchievements achievements={achivment} />
                    </div>
                </div>

                {/* III. НИЖНИЙ БЛОК - Колоды и Активность */}
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                    <ProfileDecks groups={groups} />
                    <ProfileHistory />
                </div>
            </motion.div>
        </div>
    );
}