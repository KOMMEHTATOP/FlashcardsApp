import { useState } from "react";
import { motion, AnimatePresence } from "framer-motion";
import { Brain } from "lucide-react";

import type { RatingValue } from "../types/types";
import { flashcards } from "../test/testData";

import StarRating from "../components/ui/ratting";
import Celebration from "../components/lessons/Celebration";
import StateLessens from "../components/lessons/StateLessens";
import HeaderLessons from "../components/lessons/HeaderLessons";
import NavigationLessons from "../components/lessons/NavigationLessons";
import MainCardLessens from "../components/lessons/MainCard";

interface LessonPlayerProps {
  lessonTitle: string;
  subjectColor: string;
  totalXp: number;
  onComplete: (earnedXP: number) => void;
  onBack: () => void;
}

export default function LessonPlayer({
  lessonTitle,
  subjectColor,
  totalXp,
  onComplete,
  onBack,
}: LessonPlayerProps) {
  const [currentCardIndex, setCurrentCardIndex] = useState(0);
  const [isFlipped, setIsFlipped] = useState<boolean>(false);
  const [answeredCards, setAnsweredCards] = useState<Set<number>>(new Set());

  const [showCelebration, setShowCelebration] = useState<boolean>(false);
  const [isNext, setIsNext] = useState<boolean>(true);

  const [rating, setRating] = useState<Record<number, RatingValue>>({});

  const currentCard = flashcards[currentCardIndex];
  const progress = (Object.keys(rating).length / flashcards.length) * 100;

  const values = Object.values(rating);
  const averageRating =
    values.length > 0
      ? values.reduce<number>((acc, val) => acc + val, 0) / values.length
      : 0;

  const earnedXP = Math.floor((averageRating / 5) * totalXp);

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

  const handleAnswer = (ratting: RatingValue) => {
    // Если ответ верный добавляем его в правильные для статистикии
    setRating((prev) => ({
      ...prev,
      [currentCard.id]: ratting,
    }));

    const newAnswerd = new Set(answeredCards);
    newAnswerd.add(currentCard.id);
    setAnsweredCards(newAnswerd);

    const allAnswered = newAnswerd.size === flashcards.length;
    if (allAnswered) {
      setTimeout(() => setShowCelebration(true), 500);
      return;
    }

    let nextIndex = flashcards.findIndex(
      (card, i) => i > currentCardIndex && !newAnswerd.has(card.id)
    );

    if (nextIndex === -1) {
      nextIndex = flashcards.findIndex((card) => !newAnswerd.has(card.id));
    }

    // Функция для перехода к следующему вопросу или для показа конца с
    if (nextIndex !== -1) {
      setTimeout(() => {
        setIsFlipped(false);
        setIsNext(true);
        setCurrentCardIndex(nextIndex);
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
    if (currentCardIndex < flashcards.length - 1) {
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
    onComplete(earnedXP);
  };

  // модуль статистики
  if (showCelebration) {
    return (
      <Celebration
        subjectColor={subjectColor}
        earnedXP={earnedXP}
        handleComplete={handleComplete}
        total={calculateOverallScore(rating)}
        values={values}
      />
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-base-300 via-base-200 to-base-300 ">
      {/* заголовок */}
      <HeaderLessons
        lessonTitle={lessonTitle}
        onBack={onBack}
        earnedXP={earnedXP}
        progress={progress}
        from={currentCardIndex + 1}
        to={flashcards.length}
      />

      {/* Главная карточка */}
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
              Карточка {currentCardIndex + 1} из {flashcards.length}
            </span>
          </motion.div>
        </div>

        {/* сама карточка */}
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

        {/* кнопки */}
        <AnimatePresence mode="wait">
          <div
            className={`${
              isFlipped ? "h-30" : "h-10"
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
                <StarRating
                  value={rating[currentCard.id] || 0}
                  onChange={(val: number) => handleAnswer(val as RatingValue)}
                  size={10}
                />
              </motion.div>
            )}

            {/* навигация */}
            {!isFlipped && (
              <NavigationLessons
                flashcards={flashcards}
                currentCardIndex={currentCardIndex}
                rating={rating}
                handlePrev={handlePrev}
                handleNext={handleNext}
                handleSelect={handleSelect}
              />
            )}
          </div>
        </AnimatePresence>

        {/* статистика */}
        <StateLessens values={values} total={calculateOverallScore(rating)} />
      </div>
    </div>
  );
}
