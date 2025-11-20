import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { ArrowLeft, BookHeartIcon, BowArrowIcon, Trophy } from "lucide-react";
import MotivationCard from "../components/cards/Motivation_card";
import { useData } from "../context/DataContext";
import useTitle from "../utils/useTitle";
import apiFetch from "../utils/apiFetch";
import SkeletonGroupDetail from "../components/StudySkeleton";
import { availableIcons } from "../test/data";
import { useGroupData } from "../hooks/useGroupData";
import { GroupHeader } from "../components/GroupHeader";
import { CardsList } from "../components/CardsList";
import { useAuth } from "../context/AuthContext";

export default function SubscriberGroupPage() {
    const {
        group,
        setGroup,
        cards,
        loading,
        isSubscribed,
        setIsSubscribed,
    } = useGroupData();

    const { handleSelectLesson } = useData();
    const { isAuthenticated } = useAuth();

    const [submittingSubscription, setSubmittingSubscription] = useState(false);

    const progress = useMemo(() => {
        if (cards.length === 0) return 0;
        const completedCards = cards.filter((card) => card.LastRating > 0).length;
        return (completedCards / cards.length) * 100;
    }, [cards]);

    useTitle(group?.GroupName || "");

    const handleToggleSubscription = async () => {
        if (!group) return;

        setSubmittingSubscription(true);
        try {
            if (isSubscribed) {
                await apiFetch.delete(`/Subscriptions/${group.Id}/subscribe`);
                setGroup((prev) =>
                    prev
                        ? {
                            ...prev,
                            SubscriberCount: Math.max(0, (prev.SubscriberCount ?? 0) - 1),
                        }
                        : null
                );
            } else {
                await apiFetch.post(`/Subscriptions/${group.Id}/subscribe`);
                setGroup((prev) =>
                    prev
                        ? {
                            ...prev,
                            SubscriberCount: (prev.SubscriberCount ?? 0) + 1,
                        }
                        : null
                );
            }

            setIsSubscribed((prev) => !prev);
        } catch (err) {
            console.error("Ошибка при подписке/отписке:", err);
        } finally {
            setSubmittingSubscription(false);
        }
    };

    if (!group || loading) return <SkeletonGroupDetail />;

    const Icon =
        availableIcons.find((icon) => icon.name === group.GroupIcon)?.icon ||
        BookHeartIcon;

    return (
        // Добавил bg-base-300, чтобы фон совпадал с общим фоном приложения (если там темная тема)
        <div className="min-h-screen bg-base-300 py-8">

            {/* КОНТЕЙНЕР-ОГРАНИЧИТЕЛЬ: max-w-7xl (как в AppLayout) */}
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">

                {/* Навигация */}
                <Link
                    to="/"
                    className="text-base-content/70 hover:bg-base-content/10 mb-6 flex items-center rounded px-4 py-2 duration-300 transition w-fit"
                >
                    <ArrowLeft className="w-5 h-5 mr-2" />
                    Назад на главную
                </Link>

                {/* Заголовок */}
                <GroupHeader
                    group={group}
                    icon={Icon}
                    progress={progress}
                    isSubscriptionView={true}
                    isSubscribed={isSubscribed}
                    isPublishing={false}
                    submittingSubscription={submittingSubscription}
                    publishError={null}
                    canPublish={false}
                    onTogglePublish={async () => {}}
                    onToggleSubscription={handleToggleSubscription}
                />

                {/* Список карточек */}
                <CardsList
                    cards={cards}
                    group={group}
                    isSubscriptionView={true}
                    isAuthenticated={isAuthenticated}
                    onCardClick={handleSelectLesson}
                    onDeleteCard={() => {}}
                    onEditCard={() => {}}
                    addCardFormProps={{
                        isOpen: false,
                        question: "",
                        answer: "",
                        loading: false,
                        error: "",
                        isUpdateCard: false,
                        onAddCard: async () => false,
                        onNewCard: () => {},
                        onCloseModal: () => {},
                        setQuestion: () => {},
                        setAnswer: () => {},
                    }}
                />

                {/* Мотивация */}
                {cards.length > 5 && (
                    <MotivationCard
                        animated="scale"
                        animatedDelay={4}
                        icon={Trophy}
                        label="У тебя отлично получается!"
                        description={`Пройдите еще ${
                            cards.filter((item) => !item.completed).length
                        } урока, чтобы получить итоговую оценку и заработать 500 бонусных очков опыта!`}
                        textIcon={BowArrowIcon}
                        gradient={group.GroupColor || ""}
                        delay={0.6}
                        className="mt-8"
                    />
                )}
            </div>
        </div>
    );
}