import { useEffect, useMemo, useState } from "react";
import { Link, useParams } from "react-router-dom";
import type {
    ConfrimModalState,
    GroupCardType,
    GroupType,
} from "../types/types";
import {
    ArrowLeft,
    BookHeartIcon,
    BowArrowIcon,
    Frown,
    GalleryVerticalEndIcon,
    Trophy,
    Users,
} from "lucide-react";
import { AnimatePresence, motion } from "framer-motion";
import MotivationCard from "../components/cards/Motivation_card";
import { CardQuestion } from "../shared/ui/CardQuestion";
import { useData } from "../context/DataContext";
import useTitle from "../utils/useTitle";
import apiFetch from "../utils/apiFetch";
import AddFlashcardForm from "../components/modal/AddFlashcardForm";
import SkeletonGroupDetail from "../components/StudySkeleton";
import { availableIcons } from "../test/data";
import { errorFormater } from "../utils/errorFormater";

export default function StudyPage({}) {
    const { id } = useParams();
    const {
        handleSelectLesson,
        deleteCard,
        handleOpenConfrimModal,
        handleCloseConfrimModal,
        setGroups,
    } = useData();

    const [group, setGroup] = useState<GroupType | null>(null);
    const [isOpenAddModal, setIsOpenAddModal] = useState<boolean>(false);
    const [newQuestion, setNewQuestion] = useState<string>("");
    const [newAnswer, setNewAnswer] = useState<string>("");
    const [targetCard, setTargetCard] = useState<GroupCardType | null>(null);

    const [dataDetail, setDataDetail] = useState<GroupCardType[]>([]);
    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);

    const [targetStar] = useState<number>(0);

    // Состояние для публикации
    const [isPublishing, setIsPublishing] = useState(false);
    const [publishError, setPublishError] = useState<string | null>(null);

    const filterCards = useMemo(() => {
        return dataDetail.filter((card) =>
            targetStar !== 0 ? card.LastRating === targetStar : card
        );
    }, [targetStar, dataDetail]);

    const proggresGroup = useMemo(() => {
        const totalCards = dataDetail.length;
        const completedCards = dataDetail.filter(
            (card) => card.LastRating > 0
        ).length;
        return (completedCards / totalCards) * 100;
    }, [dataDetail]);

    useEffect(() => {
        const fetchCards = async () => {
            try {
                const [groupRes, cardsRes] = await Promise.all([
                    apiFetch.get(`/Group/${id}`),
                    apiFetch.get(`/groups/${id}/cards`),
                ]);
                setGroup(groupRes.data);
                setDataDetail(cardsRes.data.reverse());
            } catch (err) {
                console.log(err);
            }
        };

        fetchCards();
    }, [id]);

    useTitle(group?.GroupName || "");

    // Функция переключения публичности
    const handleTogglePublish = async () => {
        if (!group) return;

        setIsPublishing(true);
        setPublishError(null);

        try {
            await apiFetch.post(`/Group/change-access-group`, {
                GroupId: id
            });

            setGroup(prev => prev ? { ...prev, IsPublished: !prev.IsPublished } : null);

            // Синхронизируем с глобальным состоянием
            setGroups(prev =>
                prev.map(g =>
                    g.Id === id ? { ...g, IsPublished: !g.IsPublished } : g
                )
            );
        } catch (err: any) {
            const errorMessage = err.response?.data?.errors?.[0] ||
                err.response?.data?.message ||
                'Ошибка при изменении доступа';
            setPublishError(errorMessage);
            console.error('Ошибка публикации:', err);
        } finally {
            setIsPublishing(false);
        }
    };

    const handleAddCard = async (
        question: string,
        answer: string
    ): Promise<boolean> => {
        try {
            setLoading(true);
            setError("");

            const data = { question, answer };

            if (targetCard) {
                await apiFetch.put(`/Cards/${targetCard.CardId}`, data);

                setDataDetail((prev) =>
                    prev.map((card) =>
                        card.CardId === targetCard.CardId
                            ? { ...card, Question: question, Answer: answer }
                            : card
                    )
                );

                setIsOpenAddModal(false);
                return true;
            } else {
                const res = await apiFetch.post(`/groups/${id}/cards`, data);

                setDataDetail((prev) => [res.data, ...prev]);
                setGroups((prev) =>
                    prev.map((g) =>
                        g.Id === id ? { ...g, CardCount: g.CardCount + 1 } : g
                    )
                );

                setIsOpenAddModal(false);
                return true;
            }
        } catch (err) {
            setError(errorFormater(err) || "Произошла ошибка");
            return false;
        } finally {
            setLoading(false);
        }
    };

    const handleDeleteCard = (card: GroupCardType) => {
        const modal: ConfrimModalState = {
            title: "Вы уверены, что хотите удалить карточку?",
            target: card.Question,
            handleConfirm: () => {
                setDataDetail((prev) => prev.filter((c) => c.CardId !== card.CardId));
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

    if (!group) return <SkeletonGroupDetail />;
    const Icon =
        availableIcons.find((icon) => icon.name === group.GroupIcon)?.icon ||
        BookHeartIcon;

    const canPublish = (group?.CardCount || 0) >= 10;

    return (
        <div className="min-h-screen">
            <Link
                to="/"
                className="text-white hover:bg-white/20 mb-6 flex items-center rounded px-4 py-2 duration-300 transition w-fit"
            >
                <ArrowLeft className="w-5 h-5 mr-2" />
                Назад на главную
            </Link>
            <motion.div
                initial={{ opacity: 0, y: -20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.5 }}
                className={`relative bg-gradient-to-br ${group?.GroupColor || 'from-blue-500 to-blue-700'} px-4 sm:px-6 lg:px-8 py-12 overflow-hidden rounded-2xl shadow-xl flex flex-col items-center`}
            >
                <div
                    className="absolute inset-0 bg-white/10"
                    style={{
                        backgroundImage:
                            "radial-gradient(circle at 2px 2px, rgba(255,255,255,0.15) 1px, transparent 0)",
                        backgroundSize: "40px 40px",
                    }}
                />
                <div className="max-w-7xl mx-auto relative z-10 w-full">
                    <div className="flex flex-col sm:flex-row items-center justify-center sm:justify-start gap-6 mb-8 w-full">
                        <motion.div
                            initial={{ scale: 0 }}
                            animate={{ scale: 1 }}
                            transition={{ type: "spring", stiffness: 200 }}
                            className="bg-white/20 backdrop-blur-sm p-6 rounded-3xl flex-shrink-0"
                        >
                            <Icon className="w-20 h-20 md:w-16 md:h-16 text-white" />
                        </motion.div>

                        <div className="flex-1 text-center sm:text-left">
                            <div className="flex flex-col sm:flex-row items-center sm:items-start gap-3 mb-2 w-full flex-wrap">
                                <h1 className="text-4xl text-white">{group?.GroupName}</h1>
                                {Number(group?.CardCount) > 0 && (
                                    <motion.div
                                        animate={{ rotate: [0, 10, -10, 0] }}
                                        transition={{
                                            duration: 1,
                                            repeat: Infinity,
                                            repeatDelay: 2,
                                        }}
                                        className="bg-orange-500 text-white px-3 py-2 rounded-full flex items-center gap-1 text-sm md:text-base line-clamp-1 truncate"
                                    >
                                        <GalleryVerticalEndIcon className="w-4 md:w-5 text-yellow-300" />
                                        <span className="font-mono">
                                            {group?.CardCount} карточек
                                        </span>
                                    </motion.div>
                                )}

                                {/* Счётчик подписчиков */}
                                {group?.IsPublished && (group?.SubscriberCount || 0) > 0 && (
                                    <div className="bg-white/20 text-white px-3 py-2 rounded-full flex items-center gap-1 text-sm">
                                        <Users className="w-4 h-4" />
                                        <span>{group.SubscriberCount} подписчиков</span>
                                    </div>
                                )}

                                {/* Spacer для прижатия checkbox вправо */}
                                <div className="flex-1" />

                                {/* Checkbox публикации */}
                                <div className="flex flex-col items-end">
                                    <label className={`
                                        flex items-center gap-2 cursor-pointer
                                        ${!canPublish ? 'opacity-50 cursor-not-allowed' : ''}
                                        ${isPublishing ? 'opacity-50 cursor-wait' : ''}
                                    `}>
                                        <span className="text-white text-sm">
                                            Поделиться с другими
                                        </span>
                                        <input
                                            type="checkbox"
                                            checked={group?.IsPublished || false}
                                            onChange={handleTogglePublish}
                                            disabled={isPublishing || !canPublish}
                                            className="checkbox checkbox-success bg-white border-2 border-gray-800"
                                        />
                                        {isPublishing && (
                                            <span className="loading loading-spinner loading-xs text-white"></span>
                                        )}
                                    </label>

                                    {/* Подсказка о минимуме карточек */}
                                    {!canPublish && (
                                        <div className="text-xs text-white/70 mt-1">
                                            Нужно минимум 10 карточек
                                        </div>
                                    )}
                                </div>
                            </div>

                            {/* Ошибка публикации */}
                            {publishError && (
                                <div className="bg-red-500/20 text-white px-3 py-2 rounded-lg text-sm mb-2">
                                    {publishError}
                                </div>
                            )}

                            <p className="text-white/90 text-lg mb-4">
                                Овладейте основами и раскройте свой потенциал
                            </p>

                            <div className="space-y-2 w-full">
                                <div className="flex justify-between text-white/80 text-sm">
                                    <span className="text-base-content/80">Общий прогресс</span>
                                    <span className="font-mono">
                                        {proggresGroup ? Number(proggresGroup).toFixed(0) : 0}%
                                    </span>
                                </div>
                                <div className="relative z-10 w-full h-4 bg-white/10 rounded-full">
                                    <motion.div
                                        initial={{ width: 0 }}
                                        animate={{ width: `${proggresGroup || 0}%` }}
                                        transition={{ duration: 1, ease: "easeOut" }}
                                        className="h-full bg-gradient-to-r from-yellow-50 to-yellow-100 rounded-full"
                                    />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </motion.div>

            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
                <motion.div
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    transition={{ duration: 0.5 }}
                    className="flex flex-col md:flex-row md:items-center md:justify-between mb-6 gap-4"
                >
                    <h2 className="text-lg md:text-2xl text-base-content/80">
                        Путь обучения
                    </h2>

                    <div
                        className="
                grid grid-cols-1 sm:grid-cols-3 md:flex 
                md:flex-row md:items-center 
                gap-3 md:gap-4 text-base-content/80
                w-full md:w-auto
              "
                    >
                        <div className="flex justify-center md:justify-start">
                            <AddFlashcardForm
                                handleAddCard={handleAddCard}
                                isOpen={isOpenAddModal}
                                handleNewCard={handleNewCard}
                                handleCloseModal={handleCloseModal}
                                question={newQuestion}
                                answer={newAnswer}
                                setQuestion={setNewQuestion}
                                setAnswer={setNewAnswer}
                                loading={loading}
                                isUpdateCard={targetCard !== null}
                                error={error || ""}
                                subjectColor={group?.GroupColor || "from-pink-400 to-rose-500"}
                            />
                        </div>

                        <div className="flex items-center justify-center md:justify-start gap-2">
                            <Trophy className="w-5 h-5" />
                            <span>
                Завершено{" "}
                                {dataDetail.filter((item) => item.LastRating > 0).length} из{" "}
                                {dataDetail.length}
              </span>
                        </div>
                    </div>
                </motion.div>

                <div className="space-y-4">
                    {filterCards.length === 0 && (
                        <motion.div
                            initial={{ opacity: 0, y: 20 }}
                            animate={{ opacity: 1, y: 0 }}
                            transition={{ duration: 0.5 }}
                            className="text-center text-base-content/80 text-xl rounded-2xl py-20"
                        >
                            <BookHeartIcon className="w-15 h-15 inline-block mr-2" />
                            <p className="text-center text-base-content/80 text-xl">
                                Не нашлось ни одной карточки
                                <Frown className="w-5 h-5 inline-block ml-2" />
                            </p>
                        </motion.div>
                    )}
                    <AnimatePresence>
                        {filterCards.map((item, index) => (
                            <motion.div
                                key={item.CardId}
                                initial={{ opacity: 0, x: -20 }}
                                animate={{ opacity: 1, x: 0 }}
                                exit={{ opacity: 0, x: -20 }}
                                transition={{ delay: index * 0.1 }}
                            >
                                <CardQuestion
                                    item={item}
                                    onClick={() => {
                                        handleSelectLesson(filterCards, group!, index);
                                    }}
                                    onDelete={() => handleDeleteCard(item)}
                                    onEdit={() => handleEditCard(item)}
                                />
                            </motion.div>
                        ))}
                    </AnimatePresence>
                </div>
                {filterCards.length > 5 && (
                    <MotivationCard
                        animated="scale"
                        animatedDelay={4}
                        icon={Trophy}
                        label="У тебя отлично получается!"
                        description={`Пройдите еще ${
                            dataDetail.filter((item) => !item.completed).length
                        } урока, чтобы получить итоговую оценку и заработать 500 бонусных очков опыта!`}
                        textIcon={BowArrowIcon}
                        gradient={group?.GroupColor || ""}
                        delay={0.6}
                        className="mt-8"
                    />
                )}
            </div>
        </div>
    );
}