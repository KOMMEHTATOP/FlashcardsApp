import { motion } from "framer-motion";
import { Settings2Icon, BookHeartIcon, Users } from "lucide-react";
import SortableList from "../SortebleList";
import SubscriptionCard from "../cards/SubscriptionCard.tsx";
import type { GroupType, SubscribedGroupDto, GroupCardType } from "../../types/types";
import { useData } from "../../context/DataContext";
import { availableIcons } from "../../test/data";
import { useNavigate } from "react-router-dom";
import apiFetch from "../../utils/apiFetch";
import shuffleArray from "../../utils/shuffleArray";

interface LessonsTabProps {
    groups: GroupType[] | undefined;
    onOpenSettings: () => void;
}

export function LessonsTab({ groups, onOpenSettings }: LessonsTabProps) {
    const { user, handleSelectLesson, setting } = useData();
    const navigate = useNavigate();

    const subscriptions = user?.MySubscriptions || [];

    const handleStartSubscriptionLesson = async (subscription: SubscribedGroupDto) => {
        try {
            let cards: GroupCardType[] = await apiFetch
                .get(`/Subscriptions/public/${subscription.Id}/cards`)
                .then((res) => res.data)
                .catch(() => []);

            if (!cards || cards.length === 0) return;

            // Применяем настройки пользователя
            cards = cards.filter((card) => card.LastRating >= setting?.MinRating);
            cards = cards.filter((card) => card.LastRating <= setting?.MaxRating);
            if (setting?.ShuffleOnRepeat) cards = shuffleArray(cards);

            if (!cards || cards.length === 0) return;

            // Создаём объект группы для handleSelectLesson
            const groupForLesson: GroupType = {
                Id: subscription.Id,
                GroupName: subscription.GroupName,
                GroupColor: subscription.GroupColor,
                GroupIcon: subscription.GroupIcon || "",
                CreatedAt: subscription.SubscribedAt,
                Order: 0,
                CardCount: subscription.CardCount,
                Icon: subscription.GroupIcon || "",
            };

            handleSelectLesson(cards, groupForLesson);
        } catch (err) {
            console.error("Ошибка загрузки карточек подписки:", err);
        }
    };

    const handleViewSubscription = (subscriptionId: string) => {
        // Переходим на страницу просмотра подписки
        navigate(`/subscription/${subscriptionId}`);
    };

    return (
        <motion.div
            key="lessons"
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -20 }}
            transition={{ duration: 0.4 }}
            className="space-y-6"
        >
            {/* Секция "Ваши карточки" */}
            <div className="flex items-center justify-between">
                <h2 className="text-2xl text-base-content">Ваши карточки</h2>
                <div
                    onClick={onOpenSettings}
                    className="flex items-center gap-2 hover:scale-105 duration-300 transition-all cursor-pointer group opacity-70 hover:opacity-100"
                >
                    <span className="group-hover:opacity-90 opacity-0 duration-500 transition-opacity">
                        Настройки
                    </span>
                    <Settings2Icon className="w-8 h-8 z-10" />
                </div>
            </div>
            <SortableList initalItems={groups || []} />

            {/* Секция "Мои подписки" — Теперь всегда отображается */}
            <div className="space-y-6 mt-12">
                <div className="flex items-center gap-3">
                    <Users className="w-6 h-6 text-base-content/70" />
                    <h2 className="text-2xl text-base-content">Мои подписки</h2>
                    <span className="text-base-content/50 text-lg">
                        ({subscriptions.length})
                    </span>
                </div>

                {subscriptions.length > 0 ? (
                    // 1. Отображение списка подписок
                    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
                        {subscriptions.map((subscription) => (
                            <SubscriptionCard
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
                    // 2. Отображение заглушки
                    <motion.div
                        initial={{ opacity: 0, y: 10 }}
                        animate={{ opacity: 1, y: 0 }}
                        transition={{ duration: 0.5 }}
                        className="bg-base-200/50 backdrop-blur-sm rounded-xl p-12 mt-4 text-center border border-dashed border-base-content/10"
                    >
                        <BookHeartIcon className="w-10 h-10 text-base-content/30 mx-auto mb-4" />
                        <h3 className="text-xl text-base-content/70 font-semibold">
                            У вас пока нет подписок на общие группы.
                        </h3>
                        <p className="text-base-content/50 mt-2">
                            Перейдите на вкладку "Общие группы", чтобы найти и подписаться на интересные наборы карточек.
                        </p>
                    </motion.div>
                )}
            </div>
        </motion.div>
    );
}