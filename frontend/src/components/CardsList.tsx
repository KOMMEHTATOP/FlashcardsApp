import { AnimatePresence, motion } from "framer-motion";
import { BookHeartIcon, Trophy, Lock, ArrowRight } from "lucide-react";
import type { GroupCardType, GroupType } from "@/types/types";
import { CardQuestion } from "@/shared/ui/CardQuestion";
import AddFlashcardForm from "@/components/modal/AddFlashcardForm";
import type { Dispatch, SetStateAction } from "react";
import { useNavigate } from "react-router-dom";
import { Button } from "@/shared/ui/Button";

interface CardsListProps {
    cards: GroupCardType[];
    group: GroupType;
    isSubscriptionView: boolean;
    onCardClick?: (cards: GroupCardType[], group: GroupType, index: number) => void;
    onDeleteCard?: (card: GroupCardType) => void;
    onEditCard?: (card: GroupCardType) => void;
    blurAfterIndex?: number;
    isAuthenticated?: boolean;
    addCardFormProps?: {
        isOpen: boolean;
        question: string;
        answer: string;
        loading: boolean;
        error: string;
        isUpdateCard: boolean;
        onAddCard: (question: string, answer: string) => Promise<boolean>;
        onNewCard: () => void;
        onCloseModal: () => void;
        setQuestion: Dispatch<SetStateAction<string>>;
        setAnswer: Dispatch<SetStateAction<string>>;
    };
}

export function CardsList({
    cards,
    group,
    isSubscriptionView,
    onCardClick,
    onDeleteCard,
    onEditCard,
    addCardFormProps,
    blurAfterIndex,
    isAuthenticated,
}: CardsListProps) {
    const navigate = useNavigate();
    const completedCount = cards.filter((item) => item.LastRating > 0).length;
    const showLockedContent = blurAfterIndex !== undefined && cards.length > blurAfterIndex;

    return (
        <div className="w-full py-12 relative">
            <motion.div
                initial={{ opacity: 0 }}
                animate={{ opacity: 1 }}
                transition={{ duration: 0.5 }}
                className="flex flex-col md:flex-row md:items-center md:justify-between mb-6 gap-4"
            >
                {/* ... (Верхняя часть без изменений: Заголовок, Кнопка добавления, Трофей) ... */}
                <h2 className="text-lg md:text-2xl text-base-content/80">
                    Путь обучения
                </h2>

                <div className="grid grid-cols-1 sm:grid-cols-3 md:flex md:flex-row md:items-center gap-3 md:gap-4 text-base-content/80 w-full md:w-auto">
                    {!isSubscriptionView && addCardFormProps && (
                        <div className="flex justify-center md:justify-start">
                            <AddFlashcardForm
                                handleAddCard={addCardFormProps.onAddCard}
                                isOpen={addCardFormProps.isOpen}
                                handleNewCard={addCardFormProps.onNewCard}
                                handleCloseModal={addCardFormProps.onCloseModal}
                                question={addCardFormProps.question}
                                answer={addCardFormProps.answer}
                                setQuestion={addCardFormProps.setQuestion}
                                setAnswer={addCardFormProps.setAnswer}
                                loading={addCardFormProps.loading}
                                isUpdateCard={addCardFormProps.isUpdateCard}
                                error={addCardFormProps.error}
                                subjectColor={group.GroupColor || "from-pink-400 to-rose-500"}
                            />
                        </div>
                    )}

                    {!showLockedContent && (
                        <div className="flex items-center justify-center md:justify-start gap-2">
                            <Trophy className="w-5 h-5" />
                            <span>Завершено {completedCount} из {cards.length}</span>
                        </div>
                    )}
                </div>
            </motion.div>

            <div className="space-y-4 relative">
                {cards.length === 0 && (
                    <motion.div
                        initial={{ opacity: 0, y: 20 }}
                        animate={{ opacity: 1, y: 0 }}
                        transition={{ duration: 0.5 }}
                        className="text-center text-base-content/80 text-xl rounded-2xl py-20"
                    >
                        <BookHeartIcon className="w-15 h-15 inline-block mr-2" />
                        <p>Не нашлось ни одной карточки</p>
                    </motion.div>
                )}

                <AnimatePresence>
                    {cards.map((item, index) => {
                        const isBlur = blurAfterIndex !== undefined && index >= blurAfterIndex;
                        const isFirstBlur = blurAfterIndex !== undefined && index === blurAfterIndex;

                        return (
                            <motion.div
                                key={item.CardId}
                                initial={{ opacity: 0, x: -20 }}
                                animate={{ opacity: 1, x: 0 }}
                                exit={{ opacity: 0, x: -20 }}
                                transition={{ delay: index * 0.1 }}
                                className="relative"
                            >
                                <div className={isBlur ? "blur-sm opacity-40 pointer-events-none select-none" : ""}>
                                    <CardQuestion
                                        item={item}
                                        // Передаем onClick если передан родителем
                                        onClick={onCardClick ? () => onCardClick(cards, group, index) : undefined}

                                        onDelete={!isSubscriptionView && onDeleteCard ? () => onDeleteCard(item) : undefined}
                                        onEdit={!isSubscriptionView && onEditCard ? () => onEditCard(item) : undefined}

                                        // Показываем кнопку обзор, если пользователь авторизован
                                        showOverviewButton={isAuthenticated}
                                    />
                                </div>

                                {isFirstBlur && (
                                    <div className="absolute top-0 left-0 right-0 z-20 pt-4 px-6 md:px-20">
                                        <div className="bg-base-100/90 backdrop-blur-md shadow-2xl rounded-3xl p-6 md:p-8 text-center border border-base-content/10">
                                            <div className="flex justify-center mb-4">
                                                <div className="bg-primary/10 p-3 rounded-full">
                                                    <Lock className="w-6 h-6 text-primary" />
                                                </div>
                                            </div>
                                            <h3 className="text-xl md:text-2xl font-bold mb-2">
                                                Хотите увидеть остальные карточки?
                                            </h3>
                                            <p className="text-base-content/70 mb-6 text-sm md:text-base">
                                                Зарегистрируйтесь бесплатно, чтобы получить полный доступ к этой колоде,
                                                отслеживать свой прогресс и использовать режим интервального повторения.
                                            </p>
                                            <div className="flex justify-center">
                                                <Button
                                                    variant="confirm"
                                                    onClick={() => navigate("/login")}
                                                    rightIcon={ArrowRight}
                                                >
                                                    Начать бесплатно
                                                </Button>
                                            </div>
                                        </div>
                                    </div>
                                )}
                            </motion.div>
                        );
                    })}
                </AnimatePresence>
            </div>
        </div>
    );
}