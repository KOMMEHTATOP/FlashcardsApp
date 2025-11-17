import { AnimatePresence, motion } from "framer-motion";
import { BookHeartIcon, Frown, Trophy } from "lucide-react";
import type { GroupCardType, GroupType } from "../types/types";
import { CardQuestion } from "../shared/ui/CardQuestion";
import AddFlashcardForm from "../components/modal/AddFlashcardForm";
import type { Dispatch, SetStateAction } from "react";

interface CardsListProps {
    cards: GroupCardType[];
    group: GroupType;
    isSubscriptionView: boolean;
    onCardClick: (cards: GroupCardType[], group: GroupType, index: number) => void;
    onDeleteCard?: (card: GroupCardType) => void;
    onEditCard?: (card: GroupCardType) => void;
    // Props для формы добавления карточки (только для своих групп)
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
                          }: CardsListProps) {
    const completedCount = cards.filter((item) => item.LastRating > 0).length;

    return (
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
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
                    {/* Кнопка добавления карточки (только для своих групп) */}
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

                    {/* Счётчик завершённых */}
                    <div className="flex items-center justify-center md:justify-start gap-2">
                        <Trophy className="w-5 h-5" />
                        <span>
                            Завершено {completedCount} из {cards.length}
                        </span>
                    </div>
                </div>
            </motion.div>

            {/* Список карточек */}
            <div className="space-y-4">
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
                    {cards.map((item, index) => (
                        <motion.div
                            key={item.CardId}
                            initial={{ opacity: 0, x: -20 }}
                            animate={{ opacity: 1, x: 0 }}
                            exit={{ opacity: 0, x: -20 }}
                            transition={{ delay: index * 0.1 }}
                        >
                            <CardQuestion
                                item={item}
                                onClick={() => onCardClick(cards, group, index)}
                                onDelete={!isSubscriptionView && onDeleteCard ? () => onDeleteCard(item) : undefined}
                                onEdit={!isSubscriptionView && onEditCard ? () => onEditCard(item) : undefined}
                            />
                        </motion.div>
                    ))}
                </AnimatePresence>
            </div>
        </div>
    );
}