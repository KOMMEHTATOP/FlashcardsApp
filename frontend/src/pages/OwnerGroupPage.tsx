import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import type { ConfrimModalState, GroupCardType } from "../types/types";
import { ArrowLeft, BookHeartIcon, BowArrowIcon, Trophy } from "lucide-react";
import MotivationCard from "../components/cards/Motivation_card";
import { useData } from "../context/DataContext";
import useTitle from "../utils/useTitle";
import apiFetch from "../utils/apiFetch";
import SkeletonGroupDetail from "../components/StudySkeleton";
import { availableIcons } from "../test/data";
import { errorFormater } from "../utils/errorFormater";
import { useGroupData } from "../hooks/useGroupData";
import { GroupHeader } from "../components/GroupHeader";
import { CardsList } from "../components/CardsList";

export default function OwnerGroupPage() {
    // Используем хук данных. 
    // Так как маршрут будет /study/:id, хук сам поймет, что isSubscriptionView = false
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
        deleteCard,
        handleOpenConfrimModal,
        handleCloseConfrimModal,
        setGroups,
    } = useData();

    // --- ЛОГИКА ВЛАДЕЛЬЦА (CRUD) ---

    // State для формы добавления карточки
    const [isOpenAddModal, setIsOpenAddModal] = useState(false);
    const [newQuestion, setNewQuestion] = useState("");
    const [newAnswer, setNewAnswer] = useState("");
    const [targetCard, setTargetCard] = useState<GroupCardType | null>(null);
    const [formLoading, setFormLoading] = useState(false);
    const [formError, setFormError] = useState<string | null>(null);

    // State для публикации
    const [isPublishing, setIsPublishing] = useState(false);
    const [publishError, setPublishError] = useState<string | null>(null);

    // Вычисляемые значения
    const progress = useMemo(() => {
        if (cards.length === 0) return 0;
        const completedCards = cards.filter((card) => card.LastRating > 0).length;
        return (completedCards / cards.length) * 100;
    }, [cards]);

    const canPublish = (group?.CardCount || 0) >= 10;

    useTitle(group?.GroupName || "");

    // Handlers для управления доступом
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

            setGroup((prev) =>
                prev ? { ...prev, IsPublished: newPublishedState } : null
            );

            // Обновляем глобальный список групп, чтобы изменения отразились в меню
            setGroups((prev) =>
                prev.map((g) =>
                    g.Id === groupId ? { ...g, IsPublished: newPublishedState } : g
                )
            );
        } catch (err: any) {
            const errorMessage =
                err.response?.data?.errors?.[0] ||
                err.response?.data?.message ||
                "Ошибка при изменении доступа";
            setPublishError(errorMessage);
            console.error("Ошибка публикации:", err);
        } finally {
            setIsPublishing(false);
        }
    };

    // Handlers для карточек (CRUD)
    const handleAddCard = async (
        question: string,
        answer: string
    ): Promise<boolean> => {
        try {
            setFormLoading(true);
            setFormError(null);

            const data = { question, answer };

            if (targetCard) {
                // Обновление
                await apiFetch.put(`/Cards/${targetCard.CardId}`, data);

                setCards((prev) =>
                    prev.map((card) =>
                        card.CardId === targetCard.CardId
                            ? { ...card, Question: question, Answer: answer }
                            : card
                    )
                );
            } else {
                // Создание
                const res = await apiFetch.post(`/groups/${groupId}/cards`, data);

                setCards((prev) => [res.data, ...prev]);
                setGroups((prev) =>
                    prev.map((g) =>
                        g.Id === groupId ? { ...g, CardCount: g.CardCount + 1 } : g
                    )
                );
            }

            setIsOpenAddModal(false);
            return true;
        } catch (err) {
            setFormError(errorFormater(err) || "Произошла ошибка");
            return false;
        } finally {
            setFormLoading(false);
        }
    };

    const handleDeleteCard = (card: GroupCardType) => {
        const modal: ConfrimModalState = {
            title: "Вы уверены, что хотите удалить карточку?",
            target: card.Question,
            handleConfirm: () => {
                setCards((prev) => prev.filter((c) => c.CardId !== card.CardId));
                deleteCard(card.CardId);
                handleCloseConfrimModal();
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

    if (!group || loading) return <SkeletonGroupDetail />;

    const Icon =
        availableIcons.find((icon) => icon.name === group.GroupIcon)?.icon ||
        BookHeartIcon;

    return (
        <div className="min-h-screen">
            {/* Навигация */}
            <Link
                to="/"
                className="text-white hover:bg-white/20 mb-6 flex items-center rounded px-4 py-2 duration-300 transition w-fit"
            >
                <ArrowLeft className="w-5 h-5 mr-2" />
                Назад на главную
            </Link>

            {/* Заголовок (Режим владельца) */}
            <GroupHeader
                group={group}
                icon={Icon}
                progress={progress}
                isSubscriptionView={false} // Явно указываем false
                isSubscribed={false}       // Не используется
                isPublishing={isPublishing}
                submittingSubscription={false}
                publishError={publishError}
                canPublish={canPublish}
                onTogglePublish={handleTogglePublish}
                onToggleSubscription={async () => {}} // Заглушка
            />

            {/* Список карточек (Режим владельца - с редактированием) */}
            <CardsList
                cards={cards}
                group={group}
                isSubscriptionView={false} // Разрешает кнопки редактирования
                onCardClick={handleSelectLesson}
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

            {/* Общий элемент: Мотивация */}
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
    );
}