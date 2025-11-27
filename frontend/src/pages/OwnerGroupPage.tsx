import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import type { ConfrimModalState, GroupCardType } from "@/types/types";
import { ArrowLeft, BookHeartIcon, BowArrowIcon, Trophy } from "lucide-react";
import MotivationCard from "@/features/dashboard/ui/MotivationCard";
import { useData } from "@/context/DataContext";
import useTitle from "@/utils/useTitle";
import apiFetch from "@/utils/apiFetch";
import SkeletonGroupDetail from "@/shared/ui/skeletons/StudySkeleton";
import { availableIcons } from "@/shared/data";
import { errorFormater } from "@/utils/errorFormater";
import { useGroupData } from "@/features/groups/model/useGroupData";
import { GroupHeader } from "@/features/groups/ui/GroupHeader";
import { CardsList } from "@/features/groups/ui/CardsList";
import LessonPlayer from "@/pages/LessonPlayer";

export default function OwnerGroupPage() {
    const {
        group,
        setGroup,
        cards,
        setCards,
        loading,
        groupId,
    } = useGroupData();

    const {
        handleSelectLesson,
        currentLesson,
        handleCompliteLesson,
        handleOpenConfrimModal,
        handleCloseConfrimModal,
        // 👇 Достаем наш новый метод для красивых ошибок
        handleAlert,
        setGroups,
    } = useData();

    const [isOpenAddModal, setIsOpenAddModal] = useState(false);
    const [newQuestion, setNewQuestion] = useState("");
    const [newAnswer, setNewAnswer] = useState("");
    const [targetCard, setTargetCard] = useState<GroupCardType | null>(null);
    const [formLoading, setFormLoading] = useState(false);
    const [formError, setFormError] = useState<string | null>(null);

    const [isPublishing, setIsPublishing] = useState(false);
    const [publishError, setPublishError] = useState<string | null>(null);

    const progress = useMemo(() => {
        if (cards.length === 0) return 0;
        const completedCards = cards.filter((card) => card.LastRating > 0).length;
        return (completedCards / cards.length) * 100;
    }, [cards]);

    const canPublish = (group?.CardCount || 0) >= 10;

    useTitle(group?.GroupName || "");

    if (currentLesson) {
        return (
            <LessonPlayer
                lessonTitle={currentLesson.group.GroupName}
                subjectColor={currentLesson.group.GroupColor}
                initialIndex={currentLesson.initialIndex}
                onComplete={(earnedXP) => {
                    console.log("XP:", earnedXP);
                    handleCompliteLesson();
                }}
                onBack={handleCompliteLesson}
            />
        );
    }

    const handleTogglePublish = async () => {
        if (!group) return;
        setIsPublishing(true);
        setPublishError(null);
        const newPublishedState = !group.IsPublished;

        try {
            await apiFetch.post(`/Group/change-access-group`, {
                GroupId: groupId,
                IsPublished: newPublishedState,
            });
            setGroup((prev) => (prev ? { ...prev, IsPublished: newPublishedState } : null));
            setGroups((prev) =>
                prev.map((g) => (g.Id === groupId ? { ...g, IsPublished: newPublishedState } : g))
            );
        } catch (err: any) {
            const errorMessage = err.response?.data?.errors?.[0] || err.response?.data?.message || "Ошибка";
            setPublishError(errorMessage);
        } finally {
            setIsPublishing(false);
        }
    };

    const handleAddCard = async (question: string, answer: string): Promise<boolean> => {
        try {
            setFormLoading(true);
            setFormError(null);
            const data = { question, answer };

            if (targetCard) {
                await apiFetch.put(`/Cards/${targetCard.CardId}`, data);
                setCards((prev) => prev.map((c) => (c.CardId === targetCard.CardId ? { ...c, Question: question, Answer: answer } : c)));
            } else {
                const res = await apiFetch.post(`/groups/${groupId}/cards`, data);
                setCards((prev) => [res.data, ...prev]);
                setGroups((prev) => prev.map((g) => (g.Id === groupId ? { ...g, CardCount: g.CardCount + 1 } : g)));
                setGroup((prev) => prev ? { ...prev, CardCount: (prev.CardCount || 0) + 1 } : null);
            }
            setIsOpenAddModal(false);
            return true;
        } catch (err) {
            setFormError(errorFormater(err) || "Ошибка");
            return false;
        } finally {
            setFormLoading(false);
        }
    };

    // ИСПРАВЛЕННАЯ ЛОГИКА УДАЛЕНИЯ
    const handleDeleteCard = (card: GroupCardType) => {
        const modal: ConfrimModalState = {
            title: "Удалить карточку?",
            target: card.Question,
            handleConfirm: async () => {
                try {
                    await apiFetch.delete(`/Cards/${card.CardId}`);
                    setCards((prev) => prev.filter((c) => c.CardId !== card.CardId));
                    setGroup((prev) => prev ? { ...prev, CardCount: Math.max(0, (prev.CardCount || 0) - 1) } : null);
                    setGroups((prev) => prev.map((g) => (g.Id === groupId ? { ...g, CardCount: g.CardCount - 1 } : g)));

                    handleCloseConfrimModal();
                } catch (err: any) {
                    const errorMsg = errorFormater(err) || "Не удалось удалить карточку";

                    // 👇 СНАЧАЛА ЗАКРЫВАЕМ ОКНО "УДАЛИТЬ?"
                    handleCloseConfrimModal();

                    // 👇 ЗАТЕМ ОТКРЫВАЕМ КРАСИВОЕ ОКНО ОШИБКИ
                    // Небольшая задержка (setTimeout) нужна, чтобы модалки не конфликтовали анимациями, 
                    // но часто работает и без неё. Попробуем без.
                    handleAlert("Ошибка", errorMsg);
                }
            },
            handleCancel: () => handleCloseConfrimModal(),
        };
        handleOpenConfrimModal(modal);
    };

    const handleEditCard = (card: GroupCardType) => {
        setNewQuestion(card.Question);
        setNewAnswer(card.Answer);
        setTargetCard(card);
        setIsOpenAddModal(true);
    };

    const handleNewCard = () => {
        setNewQuestion("");
        setNewAnswer("");
        setTargetCard(null);
        setIsOpenAddModal(true);
    };

    const handleCloseModal = () => setIsOpenAddModal(false);

    const handleStartLearning = () => {
        if (group && cards.length > 0) {
            handleSelectLesson(cards, group, 0);
        }
    };

    if (!group || loading) return <SkeletonGroupDetail />;

    const Icon = availableIcons.find((icon) => icon.name === group.GroupIcon)?.icon || BookHeartIcon;

    return (
        <div className="min-h-screen">
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
                isSubscriptionView={false}
                isSubscribed={false}
                isPublishing={isPublishing}
                submittingSubscription={false}
                publishError={publishError}
                canPublish={canPublish}
                onTogglePublish={handleTogglePublish}
                onToggleSubscription={async () => { }}
                onStart={handleStartLearning}
                hasCards={cards.length > 0}
            />

            <CardsList
                cards={cards}
                group={group}
                isSubscriptionView={false}
                onDeleteCard={handleDeleteCard}
                onEditCard={handleEditCard}
                addCardFormProps={{
                    isOpen: isOpenAddModal,
                    question: newQuestion,
                    answer: newAnswer,
                    loading: formLoading,
                    error: formError || "",
                    isUpdateCard: targetCard !== null,
                    onAddCard: handleAddCard,
                    onNewCard: handleNewCard,
                    onCloseModal: handleCloseModal,
                    setQuestion: setNewQuestion,
                    setAnswer: setNewAnswer,
                }}
            />

            {cards.length > 5 && (
                <MotivationCard
                    animated="scale"
                    animatedDelay={4}
                    icon={Trophy}
                    label="У тебя отлично получается!"
                    description={`Пройдите еще ${cards.filter((item) => !item.completed).length} урока, чтобы получить бонусы!`}
                    textIcon={BowArrowIcon}
                    gradient={group.GroupColor || ""}
                    delay={0.6}
                    className="mt-8"
                />
            )}
        </div>
    );
}