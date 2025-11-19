import { AnimatePresence, motion } from "framer-motion";
import { BookHeartIcon, Frown, Trophy, Lock, ArrowRight } from "lucide-react";
import type { GroupCardType, GroupType } from "../types/types";
import { CardQuestion } from "../shared/ui/CardQuestion";
import AddFlashcardForm from "../components/modal/AddFlashcardForm";
import type { Dispatch, SetStateAction } from "react";
import { useNavigate } from "react-router-dom";
import { Button } from "../shared/ui/Button";

interface CardsListProps {
    cards: GroupCardType[];
    group: GroupType;
    isSubscriptionView: boolean;
    onCardClick?: (cards: GroupCardType[], group: GroupType, index: number) => void; // Сделал опциональным
    onDeleteCard?: (card: GroupCardType) => void;
    onEditCard?: (card: GroupCardType) => void;
    blurAfterIndex?: number; // <--- Новый проп для блюра
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
                          }: CardsListProps) {
    const navigate = useNavigate();
    const completedCount = cards.filter((item) => item.LastRating > 0).length;

    // Если передан индекс для блюра, показываем CTA
    const showLockedContent = blurAfterIndex !== undefined && cards.length > blurAfterIndex;

    return (
        <div className="w-full py-12 relative">
            {/* Заголовок и действия */}
            <motion.div
                initial={{ opacity: 0 }}
                animate={{ opacity: 1 }}
                transition={{ duration: 0.5 }}
                className="flex flex-col md:flex-row md:items-center md:justify-between mb-6 gap-4"
            >
                <h2 className="text-lg md:text-2xl text-base-content/80">
                    Путь обучения
                </h2>

                <div className="grid grid-cols-1 sm:grid-cols-3 md:flex md:flex-row md:items-center gap-3 md:gap-4 text-base-content/80 w-full md:w-auto">
                    {/* Кнопка добавления карточки (Скрыта в SubscriptionView) */}
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

                    {/* Счётчик (Скрываем для гостей, если они не видят прогресс) */}
                    {!showLockedContent && (
                        <div className="flex items-center justify-center md:justify-start gap-2">
                            <Trophy className="w-5 h-5" />
                            <span>
                                Завершено {completedCount} из {cards.length}
                            </span>
                        </div>
                    )}
                </div>
            </motion.div>

            {/* Список карточек */}
            <div className="space-y-4 relative">
                {cards.length === 0 && (
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
                    {cards.map((item, index) => {
                        // Логика блюра: если есть лимит и текущий индекс больше или равен лимиту
                        const isBlur = blurAfterIndex !== undefined && index >= blurAfterIndex;

                        return (
                            <motion.div
                                key={item.CardId}
                                initial={{ opacity: 0, x: -20 }}
                                animate={{ opacity: 1, x: 0 }}
                                exit={{ opacity: 0, x: -20 }}
                                transition={{ delay: index * 0.1 }}
                                className={`relative ${isBlur ? "blur-sm select-none pointer-events-none opacity-60" : ""}`}
                            >
                                <CardQuestion
                                    item={item}
                                    // Если onCardClick передан, вызываем его, иначе undefined
                                    onClick={onCardClick ? () => onCardClick(cards, group, index) : undefined}
                                    // ВАЖНО: Если isSubscriptionView, мы передаем undefined. 
                                    // Если кнопки всё равно видны - проблема в CardQuestion.
                                    onDelete={!isSubscriptionView && onDeleteCard ? () => onDeleteCard(item) : undefined}
                                    onEdit={!isSubscriptionView && onEditCard ? () => onEditCard(item) : undefined}
                                />
                            </motion.div>
                        );
                    })}
                </AnimatePresence>

                {/* OVERLAY CTA для гостей */}
                {showLockedContent && (
                    <div className="absolute inset-x-0 bottom-0 h-2/3 bg-gradient-to-t from-base-300 via-base-300/90 to-transparent flex items-end justify-center pb-20 z-20">
                        <div className="bg-base-100/80 backdrop-blur-md shadow-2xl rounded-3xl p-8 max-w-xl text-center border border-base-content/10 mx-4">
                            <div className="flex justify-center mb-4">
                                <div className="bg-primary/10 p-4 rounded-full">
                                    <Lock className="w-8 h-8 text-primary" />
                                </div>
                            </div>
                            <h3 className="text-2xl font-bold mb-2">
                                Хотите увидеть ещё {cards.length - (blurAfterIndex || 0)} карточек?
                            </h3>
                            <p className="text-base-content/70 mb-6">
                                Зарегистрируйтесь бесплатно, чтобы получить полный доступ к этой колоде,
                                отслеживать свой прогресс и использовать режим интервального повторения.
                            </p>
                            <Button
                                variant="confirm"
                                onClick={() => navigate("/login")}
                                rightIcon={ArrowRight}
                            >
                                Начать бесплатно
                            </Button>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
}