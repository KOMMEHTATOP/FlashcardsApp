import {
    Clock,
    Zap,
    BookOpen,
    Target,
    User,
    Flame,
    BookCopyIcon,
    Globe,
    Crown,
    ChevronRight
} from "lucide-react";
import { motion } from "framer-motion";
import { useEffect, useState } from "react";
import { useData } from "../context/DataContext";
import useTitle from "../utils/useTitle";
import formatTotalHour from "../utils/formatTotalHour";
import apiFetch from "../utils/apiFetch";
import { recallRatingInfo } from "../test/data";
import { Link } from "react-router-dom";

interface StudyHistoryItem {
    Id: string;
    CardId: string;
    CardQuestion: string;
    Rating: number;
    StudiedAt: string;
    XPEarned: number;
    GroupName: string;
    GroupColor: string;
}

export function ProfilePage() {
    const { user, achivment, groups } = useData();
    useTitle("Профиль");

    const currentXP = user?.Statistics?.XPProgressInCurrentLevel ?? 0;
    const xpForNextLevel = user?.Statistics?.XPRequiredForCurrentLevel ?? 0;
    const progressPercent = xpForNextLevel > 0 ? Math.round((currentXP / xpForNextLevel) * 100) : 0;
    const level = user?.Statistics?.Level || 0;

    const unlockedAchievements = achivment?.filter((a) => a.IsUnlocked) || [];
    const lockedAchievements = achivment?.filter((a) => !a.IsUnlocked) || [];

    const [studyHistory, setStudyHistory] = useState<StudyHistoryItem[]>([]);
    const [historyLoading, setHistoryLoading] = useState(false);

    const loadHistory = async () => {
        setHistoryLoading(true);
        try {
            const response = await apiFetch.get('/Study/history');
            setStudyHistory(response.data);
        } catch (err) {
            console.error("Ошибка загрузки истории:", err);
        } finally {
            setHistoryLoading(false);
        }
    };

    useEffect(() => {
        loadHistory();
    }, []);

    return (
        <div className="w-full pb-10">
            <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.4 }}
                className="space-y-6"
            >
                {/* I. ХЕДЕР - Профиль и Уровень */}
                <div className="bg-base-200 rounded-2xl p-6">
                    <div className="flex items-center gap-6">
                        {/* Аватар с уровнем */}
                        <div className="relative">
                            <div className="w-32 h-32 rounded-full bg-gradient-to-br from-purple-500 to-pink-500 p-1">
                                <div className="w-full h-full rounded-full bg-base-100 flex items-center justify-center relative overflow-hidden">
                                    <User className="w-16 h-16 text-base-content/50" />
                                    {/* Прогресс-кольцо */}
                                    <svg className="absolute inset-0 w-full h-full -rotate-90">
                                        <circle
                                            cx="50%"
                                            cy="50%"
                                            r="45%"
                                            fill="none"
                                            stroke="currentColor"
                                            strokeWidth="4"
                                            className="text-base-300"
                                        />
                                        <circle
                                            cx="50%"
                                            cy="50%"
                                            r="45%"
                                            fill="none"
                                            stroke="url(#gradient)"
                                            strokeWidth="4"
                                            strokeDasharray={`${progressPercent * 2.83} 283`}
                                            strokeLinecap="round"
                                        />
                                        <defs>
                                            <linearGradient id="gradient">
                                                <stop offset="0%" stopColor="#a855f7" />
                                                <stop offset="100%" stopColor="#ec4899" />
                                            </linearGradient>
                                        </defs>
                                    </svg>
                                </div>
                            </div>
                            <div className="absolute -bottom-2 left-1/2 -translate-x-1/2 bg-gradient-to-r from-purple-500 to-pink-500 px-4 py-1 rounded-full">
                                <span className="text-white font-bold">Lvl {level}</span>
                            </div>
                        </div>

                        {/* Информация о пользователе */}
                        <div className="flex-1">
                            <div className="flex items-center gap-3 mb-2">
                                <h1 className="text-3xl font-bold text-base-content">{user?.Login}</h1>
                                <div className="flex items-center gap-1 bg-amber-500/20 px-3 py-1 rounded-full">
                                    <Crown className="w-4 h-4 text-amber-500" />
                                    <span className="text-amber-500 font-bold text-sm">
                                        Rating: {user?.TotalSubscribers || 0}
                                    </span>
                                </div>
                            </div>
                            <div className="text-base-content/70 mb-4">{user?.Email}</div>

                            {/* XP прогресс */}
                            <div className="bg-base-300 rounded-full h-3 w-full max-w-md overflow-hidden">
                                <motion.div
                                    initial={{ width: 0 }}
                                    animate={{ width: `${progressPercent}%` }}
                                    transition={{ duration: 1, ease: "easeOut" }}
                                    className="h-full bg-gradient-to-r from-purple-500 to-pink-500"
                                />
                            </div>
                            <div className="flex justify-between max-w-md mt-1 text-sm text-base-content/70">
                                <span>{user?.Statistics?.TotalXP || 0} XP</span>
                                <span>{progressPercent}%</span>
                            </div>
                        </div>
                    </div>
                </div>

                {/* II. ЦЕНТРАЛЬНЫЙ БЛОК - Статистика и Streak */}
                <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                    {/* Левая часть - 6 плиток статистики */}
                    <div className="lg:col-span-2 bg-base-200 rounded-2xl p-6">
                        <h2 className="text-xl font-bold text-base-content mb-4">Battle Stats</h2>
                        <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                            <div className="bg-base-100 rounded-xl p-4">
                                <div className="flex items-center gap-2 mb-2">
                                    <Clock className="w-5 h-5 text-blue-500" />
                                    <span className="text-base-content/70 text-sm">Время</span>
                                </div>
                                <div className="text-2xl font-bold text-base-content">
                                    {formatTotalHour(user?.Statistics?.TotalStudyTime || "0")} ч
                                </div>
                            </div>

                            <div className="bg-base-100 rounded-xl p-4">
                                <div className="flex items-center gap-2 mb-2">
                                    <BookOpen className="w-5 h-5 text-green-500" />
                                    <span className="text-base-content/70 text-sm">Изучено</span>
                                </div>
                                <div className="text-2xl font-bold text-base-content">
                                    {user?.Statistics?.TotalCardsStudied || 0}
                                </div>
                            </div>

                            <div className="bg-base-100 rounded-xl p-4">
                                <div className="flex items-center gap-2 mb-2">
                                    <BookCopyIcon className="w-5 h-5 text-purple-500" />
                                    <span className="text-base-content/70 text-sm">Создано</span>
                                </div>
                                <div className="text-2xl font-bold text-base-content">
                                    {user?.Statistics?.TotalCardsCreated || 0}
                                </div>
                            </div>

                            <div className="bg-base-100 rounded-xl p-4">
                                <div className="flex items-center gap-2 mb-2">
                                    <Target className="w-5 h-5 text-yellow-500" />
                                    <span className="text-base-content/70 text-sm">Общий XP</span>
                                </div>
                                <div className="text-2xl font-bold text-base-content">
                                    {user?.Statistics?.TotalXP || 0}
                                </div>
                            </div>

                            <div className="bg-base-100 rounded-xl p-4">
                                <div className="flex items-center gap-2 mb-2">
                                    <Flame className="w-5 h-5 text-rose-500" />
                                    <span className="text-base-content/70 text-sm">Идеальных</span>
                                </div>
                                <div className="text-2xl font-bold text-base-content">
                                    {user?.Statistics?.PerfectRatingsStreak || 0}
                                </div>
                            </div>

                            <div className="bg-base-100 rounded-xl p-4">
                                <div className="flex items-center gap-2 mb-2">
                                    <Globe className="w-5 h-5 text-cyan-500" />
                                    <span className="text-base-content/70 text-sm">Мои группы</span>
                                </div>
                                <div className="text-2xl font-bold text-base-content">
                                    {groups?.length || 0}
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* Правая часть - Streak и Достижения */}
                    <div className="space-y-6">
                        {/* Streak */}
                        <div className="bg-gradient-to-br from-orange-500 to-red-500 rounded-2xl p-6 text-white">
                            <h3 className="text-lg font-bold mb-3">Streak & Glory</h3>
                            <div className="flex items-center gap-3">
                                <Zap className="w-10 h-10" />
                                <div>
                                    <div className="text-3xl font-bold">
                                        {user?.Statistics?.CurrentStreak || 0}-DAY STREAK!
                                    </div>
                                    <div className="text-white/80 text-sm">
                                        Best: {user?.Statistics?.BestStreak || 0} days
                                    </div>
                                </div>
                            </div>
                        </div>

                        {/* Достижения (превью) */}
                        <div className="bg-base-200 rounded-2xl p-6">
                            <div className="flex justify-between items-center mb-4">
                                <h3 className="text-lg font-bold text-base-content">
                                    MY ACHIEVEMENTS ({unlockedAchievements.length}/{achivment?.length || 0})
                                </h3>
                            </div>
                            <div className="flex gap-3 mb-3">
                                {unlockedAchievements.slice(0, 3).map((item) => (
                                    <div
                                        key={item.Id}
                                        className={`w-14 h-14 rounded-full bg-gradient-to-br ${item.Gradient} flex items-center justify-center text-2xl`}
                                    >
                                        {item.IconUrl}
                                    </div>
                                ))}
                                {lockedAchievements.length > 0 && (
                                    <div className="w-14 h-14 rounded-full bg-base-300 flex items-center justify-center">
                                        <span className="text-base-content/30 text-2xl">🔒</span>
                                    </div>
                                )}
                            </div>
                            <Link
                                to="/"
                                className="text-primary hover:underline text-sm flex items-center gap-1"
                            >
                                View All <ChevronRight className="w-4 h-4" />
                            </Link>
                        </div>
                    </div>
                </div>

                {/* III. НИЖНИЙ БЛОК - Группы и Активность */}
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                    {/* Мои группы */}
                    <div className="bg-base-200 rounded-2xl p-6">
                        <h2 className="text-xl font-bold text-base-content mb-4">
                            Мои Колоды ({groups?.length || 0})
                        </h2>
                        <div className="space-y-3 max-h-64 overflow-y-auto">
                            {groups?.slice(0, 5).map((group) => (
                                <div
                                    key={group.Id}
                                    className="flex items-center justify-between bg-base-100 rounded-lg p-3"
                                >
                                    <div className="flex items-center gap-3">
                                        <span className="text-xl">{group.GroupIcon}</span>
                                        <span className="font-medium">{group.GroupName}</span>
                                    </div>
                                    <span className="text-sm text-base-content/70">
                                        {group.CardCount} карточек
                                    </span>
                                </div>
                            ))}
                        </div>
                    </div>

                    {/* Недавняя активность */}
                    <div className="bg-base-200 rounded-2xl p-6">
                        <h2 className="text-xl font-bold text-base-content mb-4">
                            Недавняя активность
                        </h2>
                        {historyLoading ? (
                            <div className="flex justify-center py-8">
                                <span className="loading loading-spinner loading-md"></span>
                            </div>
                        ) : (
                            <div className="space-y-3 max-h-64 overflow-y-auto">
                                {studyHistory.slice(0, 5).map((item) => (
                                    <div
                                        key={item.Id}
                                        className={`bg-gradient-to-r ${item.GroupColor} rounded-lg p-3 text-white`}
                                    >
                                        <div className="flex justify-between items-center">
                                            <div className="truncate flex-1 mr-2">
                                                <div className="font-medium truncate">{item.CardQuestion}</div>
                                                <div className="text-xs opacity-80">
                                                    {new Date(item.StudiedAt).toLocaleDateString('ru-RU')}
                                                </div>
                                            </div>
                                            <div className="flex items-center gap-2">
                                                <span className="text-sm">{recallRatingInfo[item.Rating]}</span>
                                                <span className="bg-white/20 px-2 py-1 rounded text-xs font-bold">
                                                    +{item.XPEarned}
                                                </span>
                                            </div>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>
                </div>
            </motion.div>
        </div>
    );
}