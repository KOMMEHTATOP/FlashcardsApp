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
    ChevronRight,
    Library 
} from "lucide-react";
import { motion } from "framer-motion";
import { useEffect, useState } from "react";
import { useData } from "../context/DataContext";
import { Helmet } from "react-helmet-async"; 
import formatTotalHour from "../utils/formatTotalHour";
import apiFetch from "../utils/apiFetch";
import { recallRatingInfo } from "../test/data";
import { Link } from "react-router-dom";
import { availableIcons } from "../test/data";

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
            {/* Устанавливаем заголовок страницы */}
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
                <div className="neon-bg rounded-2xl p-6">
                    <div className="flex items-center gap-6">
                        {/* Аватар с уровнем */}
                        <div className="relative" title={`Уровень ${level}`}>
                            <div className="w-32 h-32 rounded-full bg-white/20 backdrop-blur-sm p-1">
                                <div className="w-full h-full rounded-full bg-base-100 flex items-center justify-center relative overflow-hidden">
                                    <User className="w-16 h-16 text-base-content/50" />
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
                            <div className="absolute -bottom-2 left-1/2 -translate-x-1/2 bg-gradient-to-r from-purple-500 to-pink-500 px-4 py-1 rounded-full shadow-lg">
                                <span className="text-white font-bold whitespace-nowrap">Lvl {level}</span>
                            </div>
                        </div>

                        {/* Информация о пользователе */}
                        <div className="flex-1">
                            <div className="flex items-center justify-between mb-2">
                                <h1 className="text-3xl font-bold text-white">{user?.Login}</h1>
                                <div
                                    className="flex items-center gap-2 bg-gradient-to-r from-amber-500 to-yellow-400 px-4 py-2 rounded-full shadow-lg cursor-help"
                                    title="Количество подписчиков на ваши колоды"
                                >
                                    <Crown className="w-5 h-5 text-white" />
                                    <span className="text-white font-bold text-lg">
                                        {user?.TotalSubscribers || 0}
                                    </span>
                                </div>
                            </div>
                            <div className="text-white/90 mb-4 text-lg">{user?.Email}</div>

                            {/* XP прогресс */}
                            <div className="relative bg-transparent rounded-full h-4 w-full overflow-hidden border-2 border-cyan-400 shadow-[0_0_15px_rgba(34,211,238,0.5)]">
                                <motion.div
                                    initial={{ width: 0 }}
                                    animate={{ width: `${progressPercent}%` }}
                                    transition={{ duration: 1, ease: "easeOut" }}
                                    className="h-full bg-white rounded-full"
                                />
                            </div>
                            <div className="flex justify-between w-full mt-2 text-base font-bold text-white">
                                <span>{user?.Statistics?.TotalXP || 0} XP</span>
                                <span>{progressPercent}% до уровня {level + 1}</span>
                            </div>
                        </div>
                    </div>
                </div>

                {/* II. ЦЕНТРАЛЬНЫЙ БЛОК - Статистика */}
                <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                    {/* Левая часть - Плитки статистики */}
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
                                    {/* Иконка и текст */}
                                    <Library className="w-5 h-5" />
                                    <span className="text-sm font-medium">Мои колоды</span>
                                </div>
                                <div className="text-2xl font-bold">
                                    {groups?.length || 0}
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* Правая часть - Streak и Достижения */}
                    <div className="space-y-6">
                        {/* Streak */}
                        <div className="bg-gradient-to-br from-orange-500 to-red-500 rounded-2xl p-6 text-white shadow-lg">
                            <h3 className="text-lg font-bold mb-3">Ваше упорство</h3>
                            <div className="flex items-center gap-4">
                                <div className="bg-white/20 p-3 rounded-full">
                                    <Zap className="w-8 h-8" />
                                </div>
                                <div>
                                    <div className="text-3xl font-bold">
                                        {user?.Statistics?.CurrentStreak || 0} дн.
                                    </div>
                                    <div className="text-white/80 text-sm mt-1">
                                        Текущая серия. Лучшая: {user?.Statistics?.BestStreak || 0}
                                    </div>
                                </div>
                            </div>
                        </div>

                        {/* Достижения (превью) */}
                        <div className="neon-border-blue p-6">
                            <div className="flex justify-between items-center mb-4">
                                <h3 className="text-lg font-bold text-base-content">
                                    Достижения ({unlockedAchievements.length}/{achivment?.length || 0})
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
                    </div>
                </div>

                {/* III. НИЖНИЙ БЛОК - Колоды и Активность */}
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                    {/* Мои колоды */}
                    <div className="neon-border p-6">
                        {/* ИЗМЕНЕНО: Заголовок */}
                        <h2 className="text-xl font-bold text-base-content mb-4">
                            Мои колоды ({groups?.length || 0})
                        </h2>
                        <div className="space-y-3 max-h-64 overflow-y-auto pr-2">
                            {groups?.slice(0, 5).map((group) => {
                                const IconComponent = availableIcons.find(
                                    (icon) => icon.name === group.GroupIcon
                                )?.icon;

                                return (
                                    <div
                                        key={group.Id}
                                        className="flex items-center justify-between bg-base-100 hover:bg-base-200 transition-colors rounded-xl p-3 border border-base-200"
                                    >
                                        <div className="flex items-center gap-3">
                                            <div className={`w-10 h-10 rounded-lg bg-gradient-to-br ${group.GroupColor} flex items-center justify-center text-white shadow-sm`}>
                                                {IconComponent ? (
                                                    <IconComponent className="w-5 h-5" />
                                                ) : (
                                                    <span className="text-lg">{group.GroupIcon}</span>
                                                )}
                                            </div>
                                            <span className="font-medium text-base-content">{group.GroupName}</span>
                                        </div>
                                        <div className="flex items-center gap-2">
                                            <span className="text-xs font-medium bg-base-300 px-2 py-1 rounded-md text-base-content/70">
                                              {group.CardCount} карт.
                                            </span>
                                            <div className="w-4">
                                                {group.IsPublished && (
                                                    <div className="tooltip tooltip-left" data-tip="Опубликовано в библиотеке">
                                                        <Globe className="w-4 h-4 text-green-500" />
                                                    </div>
                                                )}
                                            </div>
                                        </div>
                                    </div>
                                );
                            })}
                            {(!groups || groups.length === 0) && (
                                <div className="text-center py-4 text-base-content/50">
                                    У вас пока нет колод
                                </div>
                            )}
                        </div>
                    </div>

                    {/* Недавняя активность */}
                    <div className="neon-border p-6">
                        <h2 className="text-xl font-bold text-base-content mb-4">
                            История обучения
                        </h2>
                        {historyLoading ? (
                            <div className="flex justify-center py-8">
                                <span className="loading loading-spinner loading-md text-primary"></span>
                            </div>
                        ) : (
                            <div className="space-y-3 max-h-64 overflow-y-auto pr-2">
                                {studyHistory.length > 0 ? studyHistory.slice(0, 5).map((item) => (
                                    <div
                                        key={item.Id}
                                        className={`bg-gradient-to-r ${item.GroupColor} rounded-xl p-3 text-white shadow-sm hover:opacity-95 transition-opacity`}
                                    >
                                        <div className="flex justify-between items-center">
                                            <div className="truncate flex-1 mr-3">
                                                <div className="font-medium truncate text-sm md:text-base">{item.CardQuestion}</div>
                                                <div className="text-xs opacity-80 flex gap-2 mt-0.5">
                                                    <span>{new Date(item.StudiedAt).toLocaleDateString('ru-RU')}</span>
                                                    <span>•</span>
                                                    <span className="truncate max-w-[150px]">{item.GroupName}</span>
                                                </div>
                                            </div>
                                            <div className="flex items-center gap-2 shrink-0">
                                                <span className="text-xs md:text-sm font-medium bg-black/20 px-2 py-1 rounded">
                                                    {recallRatingInfo[item.Rating]}
                                                </span>
                                                <span className="bg-white/20 px-2 py-1 rounded text-xs font-bold flex items-center">
                                                    +{item.XPEarned} XP
                                                </span>
                                            </div>
                                        </div>
                                    </div>
                                )) : (
                                    <div className="text-center py-4 text-base-content/50">
                                        История пуста. Начните учиться!
                                    </div>
                                )}
                            </div>
                        )}
                    </div>
                </div>
            </motion.div>
        </div>
    );
}