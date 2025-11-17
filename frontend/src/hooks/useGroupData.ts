import { useEffect, useState } from "react";
import { useParams, useLocation } from "react-router-dom";
import type { GroupCardType, GroupType } from "../types/types";
import apiFetch from "../utils/apiFetch";

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
        const fetchData = async () => {
            if (!id) return;

            try {
                setLoading(true);

                if (isSubscriptionView) {
                    // Загрузка данных подписки
                    const [groupRes, cardsRes] = await Promise.all([
                        apiFetch.get(`/Subscriptions/${id}`),
                        apiFetch.get(`/Subscriptions/public/${id}/cards`)
                    ]);

                    const groupData = groupRes.data as GroupType & { IsSubscribed?: boolean };
                    setGroup(groupData);
                    setIsSubscribed(groupData.IsSubscribed ?? false);

                    // Маппим карточки подписки
                    const mappedCards: GroupCardType[] = cardsRes.data.map((card: any) => ({
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
                    // Загрузка данных своей группы
                    const [groupRes, cardsRes] = await Promise.all([
                        apiFetch.get(`/Group/${id}`),
                        apiFetch.get(`/groups/${id}/cards`)
                    ]);

                    setGroup(groupRes.data);
                    setCards(cardsRes.data.reverse());
                }
            } catch (err) {
                console.error("Ошибка загрузки данных группы:", err);
            } finally {
                setLoading(false);
            }
        };

        fetchData();
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