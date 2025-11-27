import { motion } from "framer-motion";
import { useData } from "@/context/DataContext";
import { Helmet } from "react-helmet-async";
import { Link } from "react-router-dom";
import { ShieldAlert } from "lucide-react";
import { ProfileHeader } from "@/features/profile/ui/ProfileHeader";
import { ProfileStats } from "@/features/profile/ui/ProfileStats";
import { ProfileStreak } from "@/features/profile/ui/ProfileStreak";
import { ProfileAchievements } from "@/features/profile/ui/ProfileAchievements";
import { ProfileDecks } from "@/features/profile/ui/ProfileDecks";
import { ProfileHistory } from "@/features/profile/ui/ProfileHistory";

export function ProfilePage() {
    const { user, achivment, groups } = useData();

    const currentXP = user?.Statistics?.XPProgressInCurrentLevel ?? 0;
    const xpForNextLevel = user?.Statistics?.XPRequiredForCurrentLevel ?? 0;

    const progressPercent = xpForNextLevel > 0
        ? Math.round((currentXP / xpForNextLevel) * 100)
        : 0;

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
                <ProfileHeader
                    user={user ?? null}
                    level={level}
                    progressPercent={progressPercent}
                />

                {user?.Role === 'Admin' && (
                    <motion.div
                        initial={{ opacity: 0 }} animate={{ opacity: 1 }}
                        className="flex justify-end"
                    >
                        <Link
                            to="/admin"
                            className="btn btn-neutral btn-sm gap-2 shadow-lg"
                        >
                            <ShieldAlert className="w-4 h-4 text-warning" />
                            Управление пользователями
                        </Link>
                    </motion.div>
                )}

                <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                    <ProfileStats
                        user={user ?? null}
                        groupsCount={groups?.length || 0}
                    />
                    <div className="space-y-6">
                        <ProfileStreak user={user ?? null} />
                        <ProfileAchievements achievements={achivment ?? []} />
                    </div>
                </div>

                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                    <ProfileDecks
                        groups={groups ?? []}
                    />
                    <ProfileHistory />
                </div>
            </motion.div>
        </div>
    );
}