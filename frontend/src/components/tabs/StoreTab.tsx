import { motion } from "framer-motion";
import { Store, X, BookHeartIcon } from "lucide-react";
import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import apiFetch from "../../utils/apiFetch";
import type { PublicGroupDto, TagDto } from "../../types/types";
import PublicGroupCard from "../cards/PublicGroupCard";
import { availableIcons } from "../../test/data";
import { useData } from "../../context/DataContext";
import { SearchFilterBar, type SortOption } from "../filters/SearchFilterBar"; 

export function StoreTab() {
    const { user, setUser } = useData();
    const navigate = useNavigate();

    const [groups, setGroups] = useState<PublicGroupDto[]>([]);
    const [tags, setTags] = useState<TagDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    // Фильтры
    const [search, setSearch] = useState("");
    const [sortBy, setSortBy] = useState<SortOption>("date");
    const [selectedTagId, setSelectedTagId] = useState<string | null>(null);
    const [page, setPage] = useState(1);
    const [pageSize] = useState(20);

    const subscribedGroupIds = user?.MySubscriptions?.map(sub => sub.Id) || [];

    // Загрузка тегов
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

    // Загрузка групп (Server-side filtering)
    useEffect(() => {
        loadPublicGroups();
    }, [search, sortBy, page, selectedTagId]);

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
                    tagId: selectedTagId || undefined
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

    const handleSortChange = (newSort: SortOption) => {
        setSortBy(newSort);
        setPage(1);
    };

    const handleTagSelect = (tagId: string | null) => {
        setSelectedTagId(tagId);
        setPage(1);
    };

    const handleSubscribe = async (groupId: string) => {
        if (!user) { navigate("/login"); return; }
        try {
            await apiFetch.post(`/Subscriptions/${groupId}/subscribe`);
            const subscribedGroup = groups.find(g => g.Id === groupId);
            if (subscribedGroup && user) {
                setUser(prev => prev ? {
                    ...prev,
                    MySubscriptions: [...(prev.MySubscriptions || []), { Id: subscribedGroup.Id, GroupName: subscribedGroup.GroupName, GroupColor: subscribedGroup.GroupColor, GroupIcon: subscribedGroup.GroupIcon, AuthorName: subscribedGroup.AuthorName, CardCount: subscribedGroup.CardCount, SubscribedAt: new Date().toISOString() }]
                } : prev);
            }
            setGroups(prev => prev.map(g => g.Id === groupId ? { ...g, SubscriberCount: g.SubscriberCount + 1 } : g));
        } catch (err: any) { alert(err.response?.data?.errors?.[0] || "Ошибка подписки"); }
    };
    const handleUnsubscribe = async (groupId: string) => {
        try {
            await apiFetch.delete(`/Subscriptions/${groupId}/subscribe`);
            if (user) { setUser(prev => prev ? { ...prev, MySubscriptions: (prev.MySubscriptions || []).filter(sub => sub.Id !== groupId) } : prev); }
            setGroups(prev => prev.map(g => g.Id === groupId ? { ...g, SubscriberCount: Math.max(0, g.SubscriberCount - 1) } : g));
        } catch (err: any) { alert(err.response?.data?.errors?.[0] || "Ошибка отписки"); }
    };
    const handleView = (group: PublicGroupDto) => { navigate(`/subscription/${group.Id}`); };

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
                {selectedTagId && (
                    <button onClick={() => setSelectedTagId(null)} className="btn btn-sm btn-ghost gap-2 text-base-content/60 hover:text-error">
                        Сбросить фильтры <X className="w-4 h-4" />
                    </button>
                )}
            </div>

            {/* ИСПОЛЬЗУЕМ НОВЫЙ КОМПОНЕНТ */}
            <SearchFilterBar
                search={search}
                onSearchChange={setSearch}
                onSearchSubmit={handleSearchSubmit}
                tags={tags}
                selectedTagId={selectedTagId}
                onTagSelect={handleTagSelect}
                sortBy={sortBy}
                onSortChange={handleSortChange}
                loading={loading}
            />

            {/* Состояния загрузки и ошибок ... */}
            {loading && <div className="flex justify-center py-12"><span className="loading loading-spinner loading-lg text-primary"></span></div>}
            {error && <div className="alert alert-error"><span>{error}</span></div>}

            {!loading && !error && groups.length === 0 && (
                <div className="text-center py-20">
                    <div className="bg-base-200 w-20 h-20 rounded-full flex items-center justify-center mx-auto mb-4">
                        <Store className="w-10 h-10 opacity-30" />
                    </div>
                    <h3 className="text-lg font-bold opacity-70">Ничего не найдено</h3>
                    <button onClick={() => {setSearch(""); setSelectedTagId(null)}} className="btn btn-link mt-2">Сбросить фильтры</button>
                </div>
            )}

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
        </motion.div>
    );
}