import { motion } from "framer-motion";
import { Settings2Icon, BookHeartIcon, Users, ChevronLeft, ChevronRight, Plus } from "lucide-react"; 
import SortableList from "../SortebleList";
import GroupCard from "../../components/cards/GroupCard";
import type { GroupType, SubscribedGroupDto, GroupCardType, TagDto } from "../../types/types";
import { useData } from "../../context/DataContext";
import { availableIcons } from "../../test/data";
import { useNavigate } from "react-router-dom";
import apiFetch from "../../utils/apiFetch";
import { useState, useMemo, useEffect } from "react";
import { SearchFilterBar, type SortOption } from "../filters/SearchFilterBar";

interface LessonsTabProps {
    groups: GroupType[] | undefined;
    onOpenSettings: () => void;
    onCreateGroup: () => void;
    onSwitchToStore: () => void;
}

const ITEMS_PER_PAGE = 12;

export function LessonsTab({ groups, onOpenSettings, onCreateGroup, onSwitchToStore }: LessonsTabProps) {
    const { user, handleSelectLesson } = useData();
    const navigate = useNavigate();

    const [search, setSearch] = useState("");
    const [sortBy, setSortBy] = useState<SortOption>("date");
    const [selectedTagId, setSelectedTagId] = useState<string | null>(null);
    const [tags, setTags] = useState<TagDto[]>([]);
    const [currentPage, setCurrentPage] = useState(1);

    const subscriptions = user?.MySubscriptions || [];

    useEffect(() => {
        const loadTags = async () => {
            try {
                const response = await apiFetch.get("/Subscriptions/tags");
                setTags(response.data);
            } catch (err) {
                console.error("Error loading tags", err);
            }
        };
        loadTags();
    }, []);

    useEffect(() => {
        setCurrentPage(1);
    }, [search, sortBy, selectedTagId]);

    const filteredData = useMemo(() => {
        const query = search.toLowerCase().trim();

        const processList = <T extends { GroupName: string; CreatedAt?: string; CardCount?: number; Tags?: TagDto[] }>(items: T[]) => {
            let result = items;

            if (query) {
                result = result.filter(item => item.GroupName.toLowerCase().includes(query));
            }

            if (selectedTagId) {
                result = result.filter(item =>
                    item.Tags && item.Tags.some(tag => tag.Id === selectedTagId)
                );
            }

            result = [...result].sort((a, b) => {
                if (sortBy === "name") return a.GroupName.localeCompare(b.GroupName);
                if (sortBy === "popular") return (b.CardCount || 0) - (a.CardCount || 0);
                return new Date(b.CreatedAt || 0).getTime() - new Date(a.CreatedAt || 0).getTime();
            });

            return result;
        };

        const processedGroups = processList(groups || []);
        const processedSubs = processList(subscriptions);

        return {
            allGroups: processedGroups,
            allSubs: processedSubs,
            paginatedGroups: processedGroups.slice((currentPage - 1) * ITEMS_PER_PAGE, currentPage * ITEMS_PER_PAGE),
            totalPages: Math.ceil(processedGroups.length / ITEMS_PER_PAGE)
        };

    }, [groups, subscriptions, search, sortBy, selectedTagId, currentPage]);

    const handleStartSubscriptionLesson = async (subscription: SubscribedGroupDto) => {
        try {
            const response = await apiFetch.get(`/Subscriptions/public/${subscription.Id}/cards`);
            const cards: GroupCardType[] = response.data.map((card: any) => ({
                CardId: card.CardId,
                GroupId: subscription.Id,
                Question: card.Question,
                Answer: card.Answer,
                LastRating: 0,
                completed: false,
                UpdatedAt: card.CreatedAt,
                CreatedAt: card.CreatedAt
            }));

            if (!cards || cards.length === 0) return;

            const groupForLesson: GroupType = {
                Id: subscription.Id,
                GroupName: subscription.GroupName,
                GroupColor: subscription.GroupColor,
                GroupIcon: subscription.GroupIcon || "",
                CreatedAt: subscription.SubscribedAt,
                Order: 0,
                CardCount: subscription.CardCount,
                Tags: []
            };

            window.scrollTo({ top: 0, behavior: "smooth" });
            handleSelectLesson(cards, groupForLesson);
        } catch (err) {
            console.error("Ошибка загрузки карточек подписки:", err);
        }
    };

    const handleViewSubscription = (subscriptionId: string) => {
        navigate(`/subscription/${subscriptionId}`);
    };

    return (
        <motion.div
            key="lessons"
            className="space-y-6"
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -20 }}
            transition={{ duration: 0.4 }}
        >
            <SearchFilterBar
                search={search}
                onSearchChange={setSearch}
                onSearchSubmit={(e) => { e.preventDefault(); (document.activeElement as HTMLElement)?.blur(); }}
                tags={tags}
                selectedTagId={selectedTagId}
                onTagSelect={setSelectedTagId}
                sortBy={sortBy}
                onSortChange={setSortBy}
            />

            {/* --- СЕКЦИЯ: МОИ КАРТОЧКИ --- */}
            <div className="space-y-4">
                <div className="flex items-center justify-between">
                    <h2 className="text-2xl text-base-content flex items-center gap-2">
                        Мои карточки <span className="text-sm opacity-50">({filteredData.allGroups.length})</span>
                    </h2>

                    {/* УДАЛИЛИ КНОПКУ "СОЗДАТЬ", ОСТАВИЛИ ТОЛЬКО НАСТРОЙКИ */}
                    <div onClick={onOpenSettings} className="btn btn-sm btn-square btn-ghost opacity-70 hover:opacity-100">
                        <Settings2Icon className="w-5 h-5" />
                    </div>
                </div>

                {filteredData.paginatedGroups.length > 0 ? (
                    <>
                        <SortableList
                            key={`list-${currentPage}-${search}-${sortBy}-${selectedTagId}`}
                            initalItems={filteredData.paginatedGroups}
                        />

                        {filteredData.totalPages > 1 && (
                            <div className="flex justify-center items-center gap-4 mt-6 pt-4 border-t border-base-content/5">
                                <button
                                    onClick={() => setCurrentPage(p => Math.max(1, p - 1))}
                                    disabled={currentPage === 1}
                                    className="btn btn-sm btn-circle btn-ghost"
                                >
                                    <ChevronLeft className="w-5 h-5" />
                                </button>
                                <span className="font-mono text-sm">
                                    Страница {currentPage} из {filteredData.totalPages}
                                </span>
                                <button
                                    onClick={() => setCurrentPage(p => Math.min(filteredData.totalPages, p + 1))}
                                    disabled={currentPage === filteredData.totalPages}
                                    className="btn btn-sm btn-circle btn-ghost"
                                >
                                    <ChevronRight className="w-5 h-5" />
                                </button>
                            </div>
                        )}
                    </>
                ) : (
                    <div className="text-center py-12 opacity-70 bg-base-200/30 rounded-xl border border-dashed border-base-content/10">
                        {groups && groups.length > 0 ? (
                            "Ничего не найдено по выбранным фильтрам"
                        ) : (
                            // ПУСТОЙ СТЕЙТ С ПРИЗЫВОМ К ДЕЙСТВИЮ
                            <div className="flex flex-col items-center gap-3">
                                <p className="text-lg">У вас пока нет созданных колод</p>
                                <button
                                    onClick={onCreateGroup}
                                    className="btn btn-primary btn-outline gap-2"
                                >
                                    <Plus className="w-5 h-5" />
                                    Создать первую колоду
                                </button>
                            </div>
                        )}
                    </div>
                )}
            </div>

            {/* --- СЕКЦИЯ: МОИ ПОДПИСКИ --- */}
            <div className="space-y-6 mt-12 pt-8 border-t border-base-content/5">
                <div className="flex items-center gap-3">
                    <Users className="w-6 h-6 text-base-content/70" />
                    <h2 className="text-2xl text-base-content">Мои подписки</h2>
                    <span className="text-base-content/50 text-lg">
                        ({filteredData.allSubs.length})
                    </span>
                </div>

                {filteredData.allSubs.length > 0 ? (
                    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
                        {filteredData.allSubs.map((subscription) => (
                            <GroupCard
                                key={subscription.Id}
                                id={subscription.Id}
                                icon={
                                    subscription.GroupIcon
                                        ? (availableIcons.find((i) => i.name === subscription.GroupIcon)?.icon || BookHeartIcon)
                                        : BookHeartIcon
                                }
                                title={subscription.GroupName}
                                cardCount={subscription.CardCount}
                                authorName={subscription.AuthorName}
                                gradient={subscription.GroupColor}
                                onClick={() => handleViewSubscription(subscription.Id)}
                                onStartLesson={() => handleStartSubscriptionLesson(subscription)}
                            />
                        ))}
                    </div>
                ) : (
                    <motion.div
                        initial={{ opacity: 0, y: 10 }}
                        animate={{ opacity: 1, y: 0 }}
                        transition={{ duration: 0.5 }}
                        className="bg-base-200/50 backdrop-blur-sm rounded-xl p-12 mt-4 text-center border border-dashed border-base-content/10"
                    >
                        {subscriptions.length === 0 ? (
                            <>
                                <BookHeartIcon className="w-10 h-10 text-base-content/30 mx-auto mb-4" />
                                <h3 className="text-xl text-base-content/70 font-semibold">
                                    У вас пока нет подписок.
                                </h3>
                                {/* ССЫЛКА НА БИБЛИОТЕКУ */}
                                <p className="text-base-content/50 mt-2">
                                    Перейдите на вкладку{" "}
                                    <button
                                        onClick={onSwitchToStore}
                                        className="link link-primary font-semibold hover:text-primary-focus transition-colors"
                                    >
                                        Библиотека
                                    </button>
                                    , чтобы найти интересные наборы.
                                </p>
                            </>
                        ) : (
                            <div className="opacity-60">
                                Подписки не найдены по запросу "{search}".
                            </div>
                        )}
                    </motion.div>
                )}
            </div>
        </motion.div>
    );
}