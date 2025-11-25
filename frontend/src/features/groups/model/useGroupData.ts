import { useEffect, useState } from "react";
import { useParams, useLocation } from "react-router-dom";
import type { GroupCardType, GroupType } from "@/types/types";
import apiFetch from "@/utils/apiFetch";

interface GroupDetailDto extends GroupType {
    IsSubscribed?: boolean;
}

interface UseGroupDataResult {
    group: GroupType | null;
    setGroup: React.Dispatch<React.SetStateAction<GroupType | null>>;
    cards: GroupCardType[];
    setCards: React.Dispatch<React.SetStateAction<GroupCardType[]>>;
    loading: boolean;
    isSubscriptionView: boolean;
    isSubscribed: boolean;
    setIsSubscribed: React.Dispatch<React.SetStateAction<boolean>>;
    groupId: string | undefined;
}

export function useGroupData(): UseGroupDataResult {
    const { id } = useParams();
    const { pathname } = useLocation();

    const isSubscriptionView = pathname.startsWith("/subscription");

    const [group, setGroup] = useState<GroupType | null>(null);
    const [cards, setCards] = useState<GroupCardType[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [isSubscribed, setIsSubscribed] = useState<boolean>(false);

    useEffect(() => {
        const controller = new AbortController();
        const signal = controller.signal;

        const fetchData = async () => {
            if (!id) return;

            try {
                setLoading(true);

                if (isSubscriptionView) {
                    // === ЗАГРУЗКА ПОДПИСКИ: ПОСЛЕДОВАТЕЛЬНО (Устранение гонки) ===

                    // 1. Запрос данных группы. Добавляем { signal }
                    const groupRes = await apiFetch.get<GroupDetailDto>(`/Subscriptions/${id}`, { signal });
                    const groupData = groupRes.data;

                    // 2. Запрос карточек. Добавляем { signal }
                    const cardsRes = await apiFetch.get(`/Subscriptions/public/${id}/cards`, { signal });
                    const cardsData = cardsRes.data;

                    setGroup(groupData);
                    setIsSubscribed(groupData.IsSubscribed ?? false);

                    // Маппим карточки подписки
                    const mappedCards: GroupCardType[] = cardsData.map((card: any) => ({
                        CardId: card.CardId,
                        GroupId: id,
                        Question: card.Question,
                        Answer: card.Answer,
                        LastRating: 0,
                        completed: false,
                        UpdatedAt: card.CreatedAt,
                        CreatedAt: card.CreatedAt
                    }));
                    setCards(mappedCards.reverse());

                } else {
                    // === ЗАГРУЗКА СВОЕЙ ГРУППЫ ===

                    // 1. Запрос данных группы. Добавляем { signal }
                    const groupRes = await apiFetch.get<GroupType>(`/Group/${id}`, { signal });
                    const groupData = groupRes.data;

                    // 2. Запрос карточек. Добавляем { signal }
                    const cardsRes = await apiFetch.get(`/groups/${id}/cards`, { signal });
                    const cardsData = cardsRes.data;

                    setGroup(groupData);
                    setCards(cardsData.reverse());
                }
            } catch (err: any) {
                // Игнорируем ошибку отмены (запрос AbortController)
                if (err.name === 'CanceledError' || (err.response && err.response.data && err.response.data.message === 'Request aborted')) {
                    console.log('Request aborted by cleanup.');
                    return;
                }
                console.error("Ошибка загрузки данных группы:", err);
            } finally {
                setLoading(false);
            }
        };

        fetchData();

        // Функция очистки: вызывается, чтобы отменить запросы при следующем вызове useEffect
        return () => {
            controller.abort();
        };
    }, [id, isSubscriptionView]);

    return {
        group,
        setGroup,
        cards,
        setCards,
        loading,
        isSubscriptionView,
        isSubscribed,
        setIsSubscribed,
        groupId: id
    };
}