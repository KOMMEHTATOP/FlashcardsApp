import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { ArrowLeft, BookHeartIcon, BowArrowIcon, Trophy } from "lucide-react";
import MotivationCard from "@/features/dashboard/ui/MotivationCard";
import { useData } from "@/context/DataContext";
import apiFetch from "@/utils/apiFetch";
import SkeletonGroupDetail from "@/shared/ui/skeletons/StudySkeleton";
import { availableIcons } from "@/shared/data";
import { useGroupData } from "@/features/groups/model/useGroupData";
import {GroupHeader} from "@/features/groups/ui/GroupHeader";
import {CardsList} from "@/features/groups/ui/CardsList";
import {useAuth} from "@/context/AuthContext";
import LessonPlayer from "@/pages/LessonPlayer";
import type { GroupType } from "@/types/types";
import { Seo } from "@/shared/components/Seo/Seo"; 

export default function SubscriberGroupPage() {
    const {
        group,
        setGroup,
        cards,
        loading,
        isSubscribed,
        setIsSubscribed,
    } = useGroupData();

    const { handleSelectLesson, currentLesson, handleCompliteLesson } = useData();
    const { isAuthenticated } = useAuth();

    const [submittingSubscription, setSubmittingSubscription] = useState(false);

    const progress = useMemo(() => {
        if (cards.length === 0) return 0;
        const completedCards = cards.filter((card) => card.LastRating > 0).length;
        return (completedCards / cards.length) * 100;
    }, [cards]);
    
    if (currentLesson) {
        return (
            <LessonPlayer
                lessonTitle={currentLesson.group.GroupName}
                subjectColor={currentLesson.group.GroupColor}
                initialIndex={currentLesson.initialIndex}
                onComplete={(earnedXP) => {
                    console.log("Урок завершен, получено XP:", earnedXP);
                    handleCompliteLesson();
                }}
                onBack={handleCompliteLesson}
            />
        );
    }

    const handleToggleSubscription = async () => {
        if (!group) return;
        setSubmittingSubscription(true);
        try {
            if (isSubscribed) {
                await apiFetch.delete(`/Subscriptions/${group.Id}/subscribe`);
                setGroup((prev: GroupType | null) =>
                    prev
                        ? {
                            ...prev,
                            SubscriberCount: Math.max(0, (prev.SubscriberCount ?? 0) - 1),
                        }
                        : null
                );
            } else {
                await apiFetch.post(`/Subscriptions/${group.Id}/subscribe`);
                setGroup((prev: GroupType | null) =>
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

    const handleStartLearning = () => {
        if (group && cards.length > 0) {
            handleSelectLesson(cards, group, 0);
        }
    };

    if (!group || loading) return <SkeletonGroupDetail />;

    const Icon =
        availableIcons.find((icon) => icon.name === group.GroupIcon)?.icon ||
        BookHeartIcon;

    // 1. Берем первые 3-5 вопросов из карточек для формирования "превью" контента
    const previewTerms = cards
        .slice(0, 5)                    // Берем первые 5 карточек
        .map((c) => c.Question)         // Достаем текст вопроса
        .filter((q) => q && q.length < 50) // (Опционально) Игнорируем слишком длинные вопросы, чтобы не забить всё место
        .join(", ");                    // Соединяем через запятую

    // 2. Генерируем динамическое описание
    // Шаблон: "Колода [Название]. [Кол-во] терминов: [Термин1], [Термин2]..."
    const generatedDescription =
        `Изучайте набор "${group.GroupName}" (${cards.length} шт). ` +
        (previewTerms ? `Содержит вопросы: ${previewTerms}...` : `Бесплатные карточки для запоминания.`);
    
    return (
        <div className="min-h-screen bg-base-300 py-8">
            <Seo
                title={`${group.GroupName} | Flashcards Loop`}
                description={generatedDescription}
                type="article"
            />

            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <Link
                    to="/"
                    className="text-base-content/70 hover:bg-base-content/10 mb-6 flex items-center rounded px-4 py-2 duration-300 transition w-fit"
                >
                    <ArrowLeft className="w-5 h-5 mr-2" />
                    Назад на главную
                </Link>

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
                    onTogglePublish={async () => { }}
                    onToggleSubscription={handleToggleSubscription}
                    onStart={handleStartLearning}
                    hasCards={cards.length > 0}
                />

                <CardsList
                    cards={cards}
                    group={group}
                    isSubscriptionView={true}
                    isAuthenticated={isAuthenticated}
                    onDeleteCard={() => { }}
                    onEditCard={() => { }}
                    addCardFormProps={{
                        isOpen: false,
                        question: "",
                        answer: "",
                        loading: false,
                        error: "",
                        isUpdateCard: false,
                        onAddCard: async () => false,
                        onNewCard: () => { },
                        onCloseModal: () => { },
                        setQuestion: () => { },
                        setAnswer: () => { },
                    }}
                />

                {cards.length > 5 && (
                    <MotivationCard
                        animated="scale"
                        animatedDelay={4}
                        icon={Trophy}
                        label="У тебя отлично получается!"
                        description={`Пройдите еще ${cards.filter((item) => !item.completed).length
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