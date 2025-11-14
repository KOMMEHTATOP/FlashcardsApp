import { motion } from "framer-motion";
import { Store, Search, TrendingUp, Calendar, SortAsc, ChevronLeft, ChevronRight } from "lucide-react";
import { useState, useEffect } from "react";
import apiFetch from "../../utils/apiFetch";
import type { PublicGroupDto } from "../../types/types";
import PublicGroupCard from "../cards/PublicGroupCard";
import GroupPreviewModal from "../../../src/components/modal/GroupPreviewModal";
import { availableIcons } from "../../test/data";
import { BookHeartIcon } from "lucide-react";

export function StoreTab() {
    const [groups, setGroups] = useState<PublicGroupDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    // Параметры фильтрации
    const [search, setSearch] = useState("");
    const [sortBy, setSortBy] = useState<"date" | "popular" | "name">("date");
    const [page, setPage] = useState(1);
    const [pageSize] = useState(20);

    // Модалка предпросмотра
    const [previewModal, setPreviewModal] = useState<{
        isOpen: boolean;
        group: PublicGroupDto | null;
    }>({
        isOpen: false,
        group: null,
    });

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

    const handleSubscribe = async (groupId: string) => {
        try {
            await apiFetch.post(`/Subscriptions/${groupId}/subscribe`);
            // Обновляем список после подписки
            loadPublicGroups();
        } catch (err: any) {
            alert(err.response?.data?.errors?.[0] || "Ошибка подписки");
        }
    };

    const handleView = (group: PublicGroupDto) => {
        setPreviewModal({
            isOpen: true,
            group,
        });
    };

    const handleClosePreview = () => {
        setPreviewModal({
            isOpen: false,
            group: null,
        });
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
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
                    {groups.map((group) => (
                        <PublicGroupCard
                            key={group.Id}
                            id={group.Id}
                            icon={
                                group.GroupIcon
                                    ? (availableIcons.find((i) => i.name === group.GroupIcon)?.icon || group.GroupIcon)
                                    : BookHeartIcon
                            }
                            title={group.GroupName}
                            cardCount={group.CardCount}
                            subscriberCount={group.SubscriberCount}
                            authorName={group.AuthorName}
                            gradient={group.GroupColor}
                            createdAt={group.CreatedAt}
                            onView={() => handleView(group)}
                            onSubscribe={() => handleSubscribe(group.Id)}
                        />
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

            {/* Модалка предпросмотра */}
            {previewModal.group && (
                <GroupPreviewModal
                    isOpen={previewModal.isOpen}
                    onClose={handleClosePreview}
                    groupId={previewModal.group.Id}
                    groupName={previewModal.group.GroupName}
                    gradient={previewModal.group.GroupColor}
                    onSubscribe={() => handleSubscribe(previewModal.group!.Id)}
                />
            )}
        </motion.div>
    );
}