import { motion } from "framer-motion";
import { Store, Search, TrendingUp, Calendar, SortAsc, ChevronLeft, ChevronRight, BookHeartIcon, X } from "lucide-react";
import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import apiFetch from "../../utils/apiFetch";
import type { PublicGroupDto, TagDto } from "../../types/types"; // Добавил TagDto
import PublicGroupCard from "../cards/PublicGroupCard";
import { availableIcons } from "../../test/data";
import { useData } from "../../context/DataContext";

// Маппинг простых цветов из БД в красивые стили Tailwind
const tagStyles: Record<string, string> = {
    blue: "bg-blue-100 text-blue-700 border-blue-200 hover:bg-blue-200",
    green: "bg-emerald-100 text-emerald-700 border-emerald-200 hover:bg-emerald-200",
    red: "bg-rose-100 text-rose-700 border-rose-200 hover:bg-rose-200",
    yellow: "bg-amber-100 text-amber-700 border-amber-200 hover:bg-amber-200",
    purple: "bg-purple-100 text-purple-700 border-purple-200 hover:bg-purple-200",
    pink: "bg-pink-100 text-pink-700 border-pink-200 hover:bg-pink-200",
    indigo: "bg-indigo-100 text-indigo-700 border-indigo-200 hover:bg-indigo-200",
    teal: "bg-teal-100 text-teal-700 border-teal-200 hover:bg-teal-200",
    orange: "bg-orange-100 text-orange-700 border-orange-200 hover:bg-orange-200",
    default: "bg-base-200 text-base-content border-base-300 hover:bg-base-300"
};

export function StoreTab() {
    const { user, setUser } = useData();
    const navigate = useNavigate();

    const [groups, setGroups] = useState<PublicGroupDto[]>([]);
    const [tags, setTags] = useState<TagDto[]>([]); // Стейт для списка тегов

    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    // Параметры фильтрации
    const [search, setSearch] = useState("");
    const [sortBy, setSortBy] = useState<"date" | "popular" | "name">("date");
    const [selectedTagId, setSelectedTagId] = useState<string | null>(null); // Выбранный тег
    const [page, setPage] = useState(1);
    const [pageSize] = useState(20);

    const subscribedGroupIds = user?.MySubscriptions?.map(sub => sub.Id) || [];

    // 1. Загрузка тегов при старте
    useEffect(() => {
        const loadTags = async () => {
            try {
                const response = await apiFetch.get("/Subscriptions/tags");
                setTags(response.data);
            } catch (err) {
                console.error("Не удалось загрузить теги", err);
            }
        };
        loadTags();
    }, []);

    // 2. Загрузка групп при изменении любого фильтра
    useEffect(() => {
        loadPublicGroups();
    }, [search, sortBy, page, selectedTagId]); // Добавил selectedTagId в зависимости

    const loadPublicGroups = async () => {
        setLoading(true);
        setError(null);
        try {
            const response = await apiFetch.get("/Subscriptions/public", {
                params: {
                    search: search || undefined,
                    sortBy,
                    page,
                    pageSize,
                    tagId: selectedTagId || undefined // Передаем ID тега на бэкенд
                }
            });
            setGroups(response.data);
        } catch (err: any) {
            console.error("Ошибка загрузки публичных групп:", err);
            setError(err.response?.data?.errors?.[0] || "Ошибка загрузки данных");
        } finally {
            setLoading(false);
        }
    };

    const handleSearchSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        setPage(1);
        loadPublicGroups();
    };

    const handleSortChange = (newSort: "date" | "popular" | "name") => {
        setSortBy(newSort);
        setPage(1);
    };

    // Обработчик клика по тегу
    const handleTagClick = (tagId: string | null) => {
        if (selectedTagId === tagId) {
            setSelectedTagId(null); // Если кликнули по активному - сбрасываем
        } else {
            setSelectedTagId(tagId);
        }
        setPage(1); // Сбрасываем на первую страницу
    };

    const handleSubscribe = async (groupId: string) => {
        if (!user) {
            navigate("/login");
            return;
        }
        try {
            await apiFetch.post(`/Subscriptions/${groupId}/subscribe`);
            const subscribedGroup = groups.find(g => g.Id === groupId);
            if (subscribedGroup && user) {
                setUser(prev => prev ? {
                    ...prev,
                    MySubscriptions: [
                        ...(prev.MySubscriptions || []),
                        {
                            Id: subscribedGroup.Id,
                            GroupName: subscribedGroup.GroupName,
                            GroupColor: subscribedGroup.GroupColor,
                            GroupIcon: subscribedGroup.GroupIcon,
                            AuthorName: subscribedGroup.AuthorName,
                            CardCount: subscribedGroup.CardCount,
                            SubscribedAt: new Date().toISOString()
                        }
                    ]
                } : prev);
            }
            setGroups(prev => prev.map(g =>
                g.Id === groupId ? { ...g, SubscriberCount: g.SubscriberCount + 1 } : g
            ));
        } catch (err: any) {
            alert(err.response?.data?.errors?.[0] || "Ошибка подписки");
        }
    };

    const handleUnsubscribe = async (groupId: string) => {
        try {
            await apiFetch.delete(`/Subscriptions/${groupId}/subscribe`);
            if (user) {
                setUser(prev => prev ? {
                    ...prev,
                    MySubscriptions: (prev.MySubscriptions || []).filter(sub => sub.Id !== groupId)
                } : prev);
            }
            setGroups(prev => prev.map(g =>
                g.Id === groupId ? { ...g, SubscriberCount: Math.max(0, g.SubscriberCount - 1) } : g
            ));
        } catch (err: any) {
            alert(err.response?.data?.errors?.[0] || "Ошибка отписки");
        }
    };

    const handleView = (group: PublicGroupDto) => {
        navigate(`/subscription/${group.Id}`);
    };

    return (
        <motion.div
            key="store"
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -20 }}
            transition={{ duration: 0.4 }}
            className="space-y-6"
        >
            <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                <h2 className="text-2xl text-base-content font-bold">Библиотека</h2>

                {/* Отображение активного фильтра (если выбран тег) */}
                {selectedTagId && (
                    <button
                        onClick={() => setSelectedTagId(null)}
                        className="btn btn-sm btn-ghost gap-2 text-base-content/60 hover:text-error"
                    >
                        Сбросить фильтры
                        <X className="w-4 h-4" />
                    </button>
                )}
            </div>

            {/* БЛОК ФИЛЬТРОВ */}
            <div className="space-y-4">
                {/* 1. Поиск */}
                <form onSubmit={handleSearchSubmit} className="flex gap-2">
                    <div className="relative flex-1">
                        <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-base-content opacity-50" />
                        <input
                            type="text"
                            placeholder="Поиск колод по названию..."
                            value={search}
                            onChange={(e) => setSearch(e.target.value)}
                            className="input input-bordered w-full pl-10 bg-base-100"
                        />
                    </div>
                    <button type="submit" className="btn btn-primary">Найти</button>
                </form>

                {/* 2. Список тегов (Горизонтальный скролл) */}
                {tags.length > 0 && (
                    <div className="relative group">
                        <div className="flex gap-2 overflow-x-auto pb-2 scrollbar-hide mask-linear-fade">
                            <button
                                onClick={() => setSelectedTagId(null)}
                                className={`btn btn-sm rounded-full border transition-all ${
                                    selectedTagId === null
                                        ? "btn-neutral"
                                        : "btn-ghost border-base-300 bg-base-100"
                                }`}
                            >
                                Все
                            </button>

                            {tags.map((tag) => {
                                const isSelected = selectedTagId === tag.Id;
                                // Выбираем стиль: если активен - темный/контрастный, если нет - цветной чипс
                                const styleClass = isSelected
                                    ? "bg-neutral text-neutral-content border-neutral ring-2 ring-offset-2 ring-neutral"
                                    : tagStyles[tag.Color?.toLowerCase() || "default"] || tagStyles["default"];

                                return (
                                    <button
                                        key={tag.Id}
                                        onClick={() => handleTagClick(tag.Id)}
                                        className={`
                                            px-4 py-1 rounded-full text-sm font-medium border transition-all whitespace-nowrap
                                            ${styleClass}
                                            ${!isSelected && "hover:scale-105 active:scale-95"}
                                        `}
                                    >
                                        {tag.Name}
                                    </button>
                                );
                            })}
                        </div>
                        {/* Градиенты по краям для индикации скролла (опционально) */}
                        <div className="absolute right-0 top-0 bottom-2 w-8 bg-gradient-to-l from-base-300 to-transparent pointer-events-none md:hidden" />
                    </div>
                )}

                {/* 3. Сортировка */}
                <div className="flex gap-2 flex-wrap border-t border-base-content/10 pt-4">
                    <span className="text-sm text-base-content/60 flex items-center mr-2">Сортировка:</span>
                    <button onClick={() => handleSortChange("date")} className={`btn btn-xs sm:btn-sm gap-2 ${sortBy === "date" ? "btn-active" : "btn-ghost"}`}>
                        <Calendar className="w-3 h-3 sm:w-4 sm:h-4" /> Новые
                    </button>
                    <button onClick={() => handleSortChange("popular")} className={`btn btn-xs sm:btn-sm gap-2 ${sortBy === "popular" ? "btn-active" : "btn-ghost"}`}>
                        <TrendingUp className="w-3 h-3 sm:w-4 sm:h-4" /> Популярные
                    </button>
                    <button onClick={() => handleSortChange("name")} className={`btn btn-xs sm:btn-sm gap-2 ${sortBy === "name" ? "btn-active" : "btn-ghost"}`}>
                        <SortAsc className="w-3 h-3 sm:w-4 sm:h-4" /> По алфавиту
                    </button>
                </div>
            </div>

            {/* Состояния загрузки */}
            {loading && <div className="flex justify-center py-12"><span className="loading loading-spinner loading-lg text-primary"></span></div>}
            {error && <div className="alert alert-error"><span>{error}</span></div>}

            {/* Пустое состояние */}
            {!loading && !error && groups.length === 0 && (
                <div className="text-center py-20">
                    <div className="bg-base-200 w-20 h-20 rounded-full flex items-center justify-center mx-auto mb-4">
                        <Store className="w-10 h-10 opacity-30" />
                    </div>
                    <h3 className="text-lg font-bold opacity-70">Ничего не найдено</h3>
                    <p className="text-base-content/60">Попробуйте изменить параметры поиска или выбрать другой тег</p>
                    <button onClick={() => {setSearch(""); setSelectedTagId(null)}} className="btn btn-link mt-2">Сбросить фильтры</button>
                </div>
            )}

            {/* Сетка карточек */}
            {!loading && !error && groups.length > 0 && (
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
                    {groups.map((group) => (
                        <PublicGroupCard
                            key={group.Id}
                            id={group.Id}
                            icon={group.GroupIcon ? (availableIcons.find((i) => i.name === group.GroupIcon)?.icon || group.GroupIcon) : BookHeartIcon}
                            title={group.GroupName}
                            cardCount={group.CardCount}
                            subscriberCount={group.SubscriberCount}
                            authorName={group.AuthorName}
                            gradient={group.GroupColor}
                            createdAt={group.CreatedAt}
                            isSubscribed={subscribedGroupIds.includes(group.Id)}
                            onView={() => handleView(group)}
                            onSubscribe={() => handleSubscribe(group.Id)}
                            onUnsubscribe={() => handleUnsubscribe(group.Id)}
                        />
                    ))}
                </div>
            )}

            {/* Пагинация */}
            {!loading && groups.length > 0 && (
                <div className="flex justify-center gap-2 mt-8">
                    <button onClick={() => setPage(p => Math.max(1, p - 1))} disabled={page === 1} className="btn btn-circle btn-sm"><ChevronLeft className="w-4 h-4" /></button>
                    <span className="flex items-center px-4 font-mono bg-base-200 rounded-lg text-sm">Str. {page}</span>
                    <button onClick={() => setPage(p => p + 1)} disabled={groups.length < pageSize} className="btn btn-circle btn-sm"><ChevronRight className="w-4 h-4" /></button>
                </div>
            )}
        </motion.div>
    );
}