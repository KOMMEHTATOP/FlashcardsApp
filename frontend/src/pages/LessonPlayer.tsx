import { useState } from "react";
import { motion, AnimatePresence } from "framer-motion";
import { Brain } from "lucide-react";

import type { RatingValue } from "@/types/types";
import { RecallRating } from "@/shared/ui/RecallRating/RecallRating";

import Celebration from "@/features/game/ui/Celebration";
import { HeaderLessons } from "@/shared/ui/widgets/HeaderLessons";
import NavigationLessons from "@/features/game/ui/NavigationLessons";
import MainCardLessens from "@/features/game/ui/StudyFlashcard";
import { useData } from "@/context/DataContext";

interface LessonPlayerProps {
    lessonTitle: string;
    subjectColor: string;
    initialIndex?: number;
    onComplete: (earnedXP: number) => void;
    onBack: () => void;
}

export default function LessonPlayer({
    lessonTitle,
    subjectColor,
    initialIndex,
    onComplete,
    onBack,
}: LessonPlayerProps) {
    const { currentLesson, questionAnswered } = useData();

    const [currentCardIndex, setCurrentCardIndex] = useState(initialIndex || 0);
    const [isFlipped, setIsFlipped] = useState<boolean>(false);
    const [answeredCards, setAnsweredCards] = useState<Set<string>>(new Set());

    const [earnetXp, setEarnetXp] = useState<number>(0);

    const [showCelebration, setShowCelebration] = useState<boolean>(false);
    const [isNext, setIsNext] = useState<boolean>(true);

    const [rating, setRating] = useState<Record<string, RatingValue>>({});

    const currentCard = currentLesson!.cards[currentCardIndex];

    const progress = (Object.keys(rating).length / currentLesson!.length) * 100;

    const values = Object.values(rating);

    const handleFlip = () => {
        setIsFlipped(!isFlipped);
    };

    const calculateOverallScore = (
        ratings: Record<number, RatingValue>
    ): number => {
        const values = Object.values(ratings);

        if (values.length === 0) return 0;

        const total = values.reduce<number>((acc, val) => acc + val, 0);
        const average = total / values.length;

        return Math.round((average / 5) * 100);
    };

    const handleAnswer = async (ratting: RatingValue) => {
        const XpEarned = await questionAnswered(currentCard.CardId, ratting);
        setEarnetXp((prev) => prev + XpEarned);
        setRating((prev) => ({
            ...prev,
            [currentCard.CardId]: ratting,
        }));

        const newAnswerd = new Set(answeredCards);
        newAnswerd.add(currentCard.CardId);
        setAnsweredCards(newAnswerd);

        const allAnswered = newAnswerd.size === currentLesson?.length;
        if (allAnswered) {
            setTimeout(() => setShowCelebration(true), 500);
            return;
        }

        let nextIndex = currentLesson?.cards.findIndex(
            (card, i) => i > currentCardIndex && !newAnswerd.has(card.CardId)
        );

        if (nextIndex === -1) {
            nextIndex = currentLesson?.cards.findIndex(
                (card) => !newAnswerd.has(card.CardId)
            );
        }

        if (nextIndex !== -1) {
            setTimeout(() => {
                setIsFlipped(false);
                setIsNext(true);
                setCurrentCardIndex(nextIndex || 0);
            }, 600);
        } else {
            setTimeout(() => {
                setShowCelebration(true);
            }, 500);
        }
    };

    const handlePrev = () => {
        if (currentCardIndex > 0) {
            setIsNext(false);
            setCurrentCardIndex(currentCardIndex - 1);
            setIsFlipped(false);
        }
    };

    const handleNext = () => {
        if (currentLesson && currentCardIndex < currentLesson.length - 1) {
            setIsNext(true);
            setCurrentCardIndex(currentCardIndex + 1);
            setIsFlipped(false);
        }
    };

    const handleSelect = (index: number) => {
        setIsNext(index < currentCardIndex ? false : true);
        setCurrentCardIndex(index);
        setIsFlipped(false);
    };

    const handleComplete = () => {
        onComplete(earnetXp);
    };

    if (showCelebration) {
        return (
            <Celebration
                subjectColor={subjectColor}
                earnedXP={earnetXp}
                handleComplete={handleComplete}
                total={calculateOverallScore(rating)}
                values={values}
            />
        );
    }

    return (
        <div className="min-h-screen bg-gradient-to-br from-base-300 via-base-200 to-base-300 ">
            <HeaderLessons
                lessonTitle={lessonTitle}
                onBack={onBack}
                earnedXP={earnetXp}
                progress={progress}
                from={currentCardIndex + 1}
                to={currentLesson?.length || 0}
            />

            <div className="max-w-4xl mx-auto px-4 py-12">
                <div className="mb-8 text-center">
                    <motion.div
                        key={`card-indicator-${currentCardIndex}`}
                        initial={{ scale: 0.8, opacity: 0 }}
                        animate={{ scale: 1, opacity: 1 }}
                        className="inline-flex items-center gap-2 bg-info/50 backdrop-blur-sm px-6 py-3 rounded-full shadow-lg"
                    >
                        <Brain
                            className={`w-5 h-5 bg-gradient-to-r ${subjectColor} bg-clip-text`}
                        />
                        <span className="text-base-content">
                            Карточка {currentCardIndex + 1} из {currentLesson?.length || 0}
                        </span>
                    </motion.div>
                </div>

                <AnimatePresence mode="wait">
                    <motion.div
                        key={currentCardIndex}
                        initial={{ opacity: 0, x: 50 }}
                        animate={{ opacity: 1, x: 0 }}
                        exit={{ opacity: 0, x: -50 }}
                        transition={{ duration: 0.3 }}
                    >
                        <MainCardLessens
                            currentCardIndex={currentCardIndex}
                            currentCard={currentCard}
                            isNext={isNext}
                            isFlipped={isFlipped}
                            subjectColor={subjectColor}
                            handleFlip={handleFlip}
                        />
                    </motion.div>
                </AnimatePresence>

                <AnimatePresence mode="wait">
                    <div
                        className={`${isFlipped ? "h-30" : "h-10"
                            } transition-all duration-500 ease-in-out `}
                    >
                        {isFlipped && (
                            <motion.div
                                initial={{ opacity: 0, y: 20 }}
                                animate={{ opacity: 1, y: 0 }}
                                exit={{ opacity: 0, y: 20 }}
                                className="gap-4 mb-8 justify-center flex flex-col items-center"
                            >
                                <p className="text-base-content/50 text-lg">
                                    Насколько верно вы ответили?
                                </p>
                                <RecallRating
                                    onChange={handleAnswer}
                                    value={rating[currentCard.CardId] || 0}
                                    disabled={false}
                                />
                            </motion.div>
                        )}
                    </div>
                </AnimatePresence>

                <NavigationLessons
                    flashcards={currentLesson?.cards || []}
                    currentCardIndex={currentCardIndex}
                    rating={rating}
                    handlePrev={handlePrev}
                    handleNext={handleNext}
                    handleSelect={handleSelect}
                />
            </div>
        </div>
    );
}