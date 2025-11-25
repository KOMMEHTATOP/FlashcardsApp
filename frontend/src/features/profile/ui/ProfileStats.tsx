import { Clock, BookOpen, BookCopyIcon, Target, Flame, Library } from "lucide-react";
import type { UserData } from "@/types/types";
import formatTotalHour from "@/utils/formatTotalHour";

interface ProfileStatsProps {
    user: UserData | null;
    groupsCount: number;
}

export function ProfileStats({ user, groupsCount }: ProfileStatsProps) {
    return (
        <div className="lg:col-span-2 neon-border p-6">
            <h2 className="text-xl font-bold text-base-content mb-4">Статистика обучения</h2>
            <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                <div className="bg-gradient-to-br from-blue-500 to-cyan-500 rounded-xl p-4 text-white">
                    <div className="flex items-center gap-2 mb-2 opacity-90">
                        <Clock className="w-5 h-5" />
                        <span className="text-sm font-medium">Время в учебе</span>
                    </div>
                    <div className="text-2xl font-bold">
                        {formatTotalHour(user?.Statistics?.TotalStudyTime || "0")} ч
                    </div>
                </div>

                <div className="bg-gradient-to-br from-green-500 to-emerald-500 rounded-xl p-4 text-white">
                    <div className="flex items-center gap-2 mb-2 opacity-90">
                        <BookOpen className="w-5 h-5" />
                        <span className="text-sm font-medium">Изучено карточек</span>
                    </div>
                    <div className="text-2xl font-bold">
                        {user?.Statistics?.TotalCardsStudied || 0}
                    </div>
                </div>

                <div className="bg-gradient-to-br from-purple-500 to-pink-500 rounded-xl p-4 text-white">
                    <div className="flex items-center gap-2 mb-2 opacity-90">
                        <BookCopyIcon className="w-5 h-5" />
                        <span className="text-sm font-medium">Создано карточек</span>
                    </div>
                    <div className="text-2xl font-bold">
                        {user?.Statistics?.TotalCardsCreated || 0}
                    </div>
                </div>

                <div className="bg-gradient-to-br from-yellow-500 to-orange-500 rounded-xl p-4 text-white">
                    <div className="flex items-center gap-2 mb-2 opacity-90">
                        <Target className="w-5 h-5" />
                        <span className="text-sm font-medium">Всего XP</span>
                    </div>
                    <div className="text-2xl font-bold">
                        {user?.Statistics?.TotalXP || 0}
                    </div>
                </div>

                <div className="bg-gradient-to-br from-rose-500 to-red-500 rounded-xl p-4 text-white">
                    <div className="flex items-center gap-2 mb-2 opacity-90">
                        <Flame className="w-5 h-5" />
                        <span className="text-sm font-medium">Идеальных ответов</span>
                    </div>
                    <div className="text-2xl font-bold">
                        {user?.Statistics?.PerfectRatingsStreak || 0}
                    </div>
                </div>

                <div className="bg-gradient-to-br from-cyan-500 to-blue-500 rounded-xl p-4 text-white">
                    <div className="flex items-center gap-2 mb-2 opacity-90">
                        <Library className="w-5 h-5" />
                        <span className="text-sm font-medium">Мои колоды</span>
                    </div>
                    <div className="text-2xl font-bold">
                        {groupsCount}
                    </div>
                </div>
            </div>
        </div>
    );
}
