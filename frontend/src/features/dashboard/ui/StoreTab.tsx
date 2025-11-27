import { motion } from "framer-motion";
import { Store, ChevronLeft, ChevronRight, BookHeartIcon } from "lucide-react";
import { useState, useEffect, useRef } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import apiFetch from "@/utils/apiFetch";
import type { PublicGroupDto, TagDto } from "@/types/types";
import PublicGroupCard from "@/features/groups/ui/PublicGroupCard";
import { availableIcons } from "@/shared/data";
import { useData } from "@/context/DataContext";
import { SearchFilterBar, type SortOption } from "@/features/search/ui/SearchFilterBar";

const ITEMS_PER_PAGE = 12;

export function StoreTab() {
    const { user, setUser } = useData();
    const navigate = useNavigate();

    const scrollRef = useRef<HTMLDivElement>(null);

    const [searchParams, setSearchParams] = useSearchParams();

    const page = parseInt(searchParams.get("page") || "1");
    const search = searchParams.get("search") || "";
    const sortBy = (searchParams.get("sort") as SortOption) || "date";
    const selectedTagId = searchParams.get("tag") || null;

    const [groups, setGroups] = useState<PublicGroupDto[]>([]);
    const [tags, setTags] = useState<TagDto[]>([]);

    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [totalCount, setTotalCount] = useState(0);

    const subscribedGroupIds = user?.MySubscriptions?.map(sub => sub.Id) || [];

    const updateParams = (newParams: Record<string, string | null>) => {
        const current = Object.fromEntries(searchParams.entries());
        const merged = { ...current, ...newParams };

        const cleanParams: Record<string, string> = {};

        Object.keys(merged).forEach(key => {
            const value = merged[key];
            if (value !== null && value !== undefined && value !== "") {
                cleanParams[key] = value;
            }
        });

        if (!newParams.page && cleanParams.page && cleanParams.page !== "1") {
            cleanParams.page = "1";
        }

        setSearchParams(cleanParams);
    };

    useEffect(() => {
        apiFetch.get("/Subscriptions/tags")
            .then(res => setTags(res.data))
            .catch(console.error);
    }, []);

    useEffect(() => {
        const fetchGroups = async () => {
            setLoading(true);
            setError(null);
            try {
                const response = await apiFetch.get("/Subscriptions/public", {
                    params: {
                        page: page,
                        pageSize: ITEMS_PER_PAGE,
                        search: search || undefined,
                        sortBy: sortBy,
                        tagId: selectedTagId || undefined
                    }
                });

                setGroups(response.data);

                const serverTotal = response.headers['x-total-count'];

                if (serverTotal) {
                    setTotalCount(parseInt(serverTotal, 10));
                } else {
                    console.warn("⚠️ Пагинация: Отсутствует заголовок 'x-total-count'. Проверьте CORS на бэкенде (WithExposedHeaders).");
                    setTotalCount(response.data.length > 0 ? (page * ITEMS_PER_PAGE) : 0);
                }

            } catch (err: any) {
                console.error("Ошибка загрузки:", err);
                setError("Не удалось загрузить данные");
            } finally {
                setLoading(false);
            }
        };

        fetchGroups();

        if (scrollRef.current && (page > 1 || search || selectedTagId)) {
            scrollRef.current.scrollIntoView({ behavior: 'smooth', block: 'start' });
        }

    }, [searchParams, page, search, sortBy, selectedTagId]);

    const handleSubscribe = async (groupId: string) => {
        if (!user) {
            navigate("/login");
            return;
        }
        try {
            await apiFetch.post(`/Subscriptions/${groupId}/subscribe`);

            const subscribedGroup = groups.find(g => g.Id === groupId);

            if (subscribedGroup) {
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

    const totalPages = Math.ceil(totalCount / ITEMS_PER_PAGE);

    return (
        <motion.div
            key="store"
            initial={{ opacity: 0 }} animate={{ opacity: 1 }}
            className="space-y-6"
        >
            <div ref={scrollRef} className="flex justify-between items-center scroll-mt-24">
                <h2 className="text-2xl text-base-content font-bold">Библиотека</h2>
            </div>

            <SearchFilterBar
                search={search}
                onSearchChange={(val) => updateParams({ search: val })}
                onSearchSubmit={(e) => { e.preventDefault(); }}

                tags={tags}
                selectedTagId={selectedTagId}
                onTagSelect={(id) => updateParams({ tag: id })}

                sortBy={sortBy}
                onSortChange={(sort) => updateParams({ sort })}

                loading={loading}
            />

            {loading && (
                <div className="text-center py-12">
                    <span className="loading loading-spinner loading-lg text-primary"></span>
                </div>
            )}

            {error && !loading && (
                <div className="alert alert-error">{error}</div>
            )}

            {!loading && !error && (
                <>
                    {groups.length > 0 ? (
                        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
                            {groups.map((group) => (
                                <PublicGroupCard
                                    key={group.Id}
                                    id={group.Id}
                                    createdAt={group.CreatedAt}
                                    icon={group.GroupIcon
                                        ? (availableIcons.find((i) => i.name === group.GroupIcon)?.icon || group.GroupIcon)
                                        : BookHeartIcon
                                    }
                                    title={group.GroupName}
                                    cardCount={group.CardCount}
                                    subscriberCount={group.SubscriberCount}
                                    authorName={group.AuthorName}
                                    gradient={group.GroupColor}
                                    isSubscribed={subscribedGroupIds.includes(group.Id)}
                                    onView={() => handleView(group)}
                                    onSubscribe={() => handleSubscribe(group.Id)}
                                    onUnsubscribe={() => handleUnsubscribe(group.Id)}
                                />
                            ))}
                        </div>
                    ) : (
                        <div className="text-center py-20 bg-base-200/30 rounded-xl border border-dashed border-base-content/10">
                            <div className="bg-base-200 w-20 h-20 rounded-full flex items-center justify-center mx-auto mb-4">
                                <Store className="w-10 h-10 opacity-30" />
                            </div>
                            <h3 className="text-lg font-bold opacity-70">Ничего не найдено</h3>
                            <p className="text-base-content/60 mb-4">
                                Попробуйте изменить параметры поиска
                            </p>
                            <button
                                onClick={() => setSearchParams({})}
                                className="btn btn-link btn-sm"
                            >
                                Сбросить все фильтры
                            </button>
                        </div>
                    )}

                    {/* ПАГИНАЦИЯ */}
                    {groups.length > 0 && (
                        <div className="flex justify-center items-center gap-4 mt-8 pt-4 border-t border-base-content/5">
                            <button
                                disabled={page === 1 || loading}
                                onClick={() => updateParams({ page: (page - 1).toString() })}
                                className="btn btn-circle btn-sm btn-ghost"
                            >
                                <ChevronLeft className="w-4 h-4" />
                            </button>

                            <span className="font-mono text-sm opacity-70">
                                Стр. {page} {totalPages > 0 ? `из ${totalPages}` : ""}
                            </span>

                            <button
                                disabled={(totalPages > 0 && page >= totalPages) || loading}
                                onClick={() => updateParams({ page: (page + 1).toString() })}
                                className="btn btn-circle btn-sm btn-ghost"
                            >
                                <ChevronRight className="w-4 h-4" />
                            </button>
                        </div>
                    )}
                </>
            )}
        </motion.div>
    );
}