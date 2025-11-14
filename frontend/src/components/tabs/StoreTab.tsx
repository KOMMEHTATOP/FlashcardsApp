import { motion } from "framer-motion";
import { Store, Search, TrendingUp, Calendar, SortAsc, ChevronLeft, ChevronRight } from "lucide-react";
import { useState, useEffect } from "react";
import apiFetch from "../../../src/utils/apiFetch.ts";
import type { PublicGroupDto } from "../../types/types";

export function StoreTab() {
    const [groups, setGroups] = useState<PublicGroupDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    // Параметры фильтрации
    const [search, setSearch] = useState("");
    const [sortBy, setSortBy] = useState<"date" | "popular" | "name">("date");
    const [page, setPage] = useState(1);
    const [pageSize] = useState(20);

    // Загрузка данных при изменении фильтров
    useEffect(() => {
        loadPublicGroups();
    }, [search, sortBy, page]);

    const loadPublicGroups = async () => {
        setLoading(true);
        setError(null);

        try {
            const response = await apiFetch.get("/Subscriptions/public", {
                params: {
                    search: search || undefined,
                    sortBy,
                    page,
                    pageSize
                }
            });

            console.log("Response data:", response.data);
            console.log("Is array?", Array.isArray(response.data));

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
        setPage(1); // Сброс на первую страницу при новом поиске
        loadPublicGroups();
    };

    const handleSortChange = (newSort: "date" | "popular" | "name") => {
        setSortBy(newSort);
        setPage(1); // Сброс на первую страницу
    };

    const handleSubscribe = async (groupId: string) => {
        try {
            await apiFetch.post(`Subscriptions/${groupId}/subscribe`);
            // Обновляем список после подписки
            loadPublicGroups();
        } catch (err: any) {
            alert(err.response?.data?.errors?.[0] || "Ошибка подписки");
        }
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
            <h2 className="text-2xl text-base-content">Магазин публичных колод</h2>

            {/* Поиск и фильтры */}
            <div className="space-y-4">
                {/* Поиск */}
                <form onSubmit={handleSearchSubmit} className="flex gap-2">
                    <div className="relative flex-1">
                        <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-base-content opacity-50" />
                        <input
                            type="text"
                            placeholder="Поиск по названию или автору..."
                            value={search}
                            onChange={(e) => setSearch(e.target.value)}
                            className="input input-bordered w-full pl-10"
                        />
                    </div>
                    <button type="submit" className="btn btn-primary">
                        Найти
                    </button>
                </form>

                {/* Сортировка */}
                <div className="flex gap-2 flex-wrap">
                    <button
                        onClick={() => handleSortChange("date")}
                        className={`btn btn-sm gap-2 ${sortBy === "date" ? "btn-primary" : "btn-ghost"}`}
                    >
                        <Calendar className="w-4 h-4" />
                        Новые
                    </button>
                    <button
                        onClick={() => handleSortChange("popular")}
                        className={`btn btn-sm gap-2 ${sortBy === "popular" ? "btn-primary" : "btn-ghost"}`}
                    >
                        <TrendingUp className="w-4 h-4" />
                        Популярные
                    </button>
                    <button
                        onClick={() => handleSortChange("name")}
                        className={`btn btn-sm gap-2 ${sortBy === "name" ? "btn-primary" : "btn-ghost"}`}
                    >
                        <SortAsc className="w-4 h-4" />
                        По алфавиту
                    </button>
                </div>
            </div>

            {/* Состояния загрузки и ошибки */}
            {loading && (
                <div className="flex justify-center py-12">
                    <span className="loading loading-spinner loading-lg"></span>
                </div>
            )}

            {error && (
                <div className="alert alert-error">
                    <span>{error}</span>
                </div>
            )}

            {/* Список групп */}
            {!loading && !error && groups.length === 0 && (
                <div className="text-center py-12">
                    <Store className="w-16 h-16 mx-auto mb-4 opacity-30" />
                    <p className="text-base-content opacity-70">
                        {search ? "Ничего не найдено" : "Пока нет публичных колод"}
                    </p>
                </div>
            )}

            {!loading && !error && groups.length > 0 && (
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                    {groups.map((group, index) => (
                        <motion.div
                            key={group.Id}
                            initial={{ opacity: 0 }}
                            animate={{ opacity: 1 }}
                            transition={{ delay: index * 0.05 }}
                            className="card bg-base-100 shadow-xl hover:shadow-2xl transition-shadow relative overflow-hidden"
                        >
                            {/* Цветная полоска слева */}
                            <div className={`absolute left-0 top-0 bottom-0 w-1 bg-gradient-to-b ${group.GroupColor}`}></div>

                            <div className="card-body">
                                {/* Заголовок группы */}
                                <div className="flex items-start justify-between gap-2">
                                    <div className="flex items-center gap-2 flex-1">
                                        {group.GroupIcon && (
                                            <span className="text-2xl">{group.GroupIcon}</span>
                                        )}
                                        <h3 className="card-title text-base">{group.GroupName}</h3>
                                    </div>
                                </div>

                                {/* Информация об авторе */}
                                <p className="text-sm opacity-70">
                                    👤 {group.AuthorName}
                                </p>

                                {/* Статистика */}
                                <div className="flex gap-4 text-sm opacity-70">
                                    <span>📚 {group.CardCount} карточек</span>
                                    <span>👥 {group.SubscriberCount} подписчиков</span>
                                </div>

                                {/* Дата создания */}
                                <p className="text-xs opacity-50">
                                    Создано: {new Date(group.CreatedAt).toLocaleDateString("ru-RU")}
                                </p>

                                {/* Кнопка подписки */}
                                <div className="card-actions justify-end mt-4">
                                    <button
                                        onClick={() => handleSubscribe(group.Id)}
                                        className="btn btn-primary btn-sm"
                                    >
                                        Подписаться
                                    </button>
                                </div>
                            </div>
                        </motion.div>
                    ))}
                </div>
            )}

            {/* Пагинация */}
            {!loading && groups.length > 0 && (
                <div className="flex justify-center gap-2 mt-6">
                    <button
                        onClick={() => setPage(p => Math.max(1, p - 1))}
                        disabled={page === 1}
                        className="btn btn-circle btn-sm"
                    >
                        <ChevronLeft className="w-4 h-4" />
                    </button>
                    <span className="flex items-center px-4">
                        Страница {page}
                    </span>
                    <button
                        onClick={() => setPage(p => p + 1)}
                        disabled={groups.length < pageSize}
                        className="btn btn-circle btn-sm"
                    >
                        <ChevronRight className="w-4 h-4" />
                    </button>
                </div>
            )}
        </motion.div>
    );
}