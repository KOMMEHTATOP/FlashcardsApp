import { ChevronLeft, ChevronRight } from "lucide-react";
import { Button } from "../ui/button";
import { motion } from "framer-motion";
import type { FlashCard, RatingValue } from "../../types/types";
import { colorRatingCard } from "../../test/data";

interface NavigationLessonsProps {
  flashcards: FlashCard[];
  currentCardIndex: number;
  rating: Record<number, RatingValue>;
  handlePrev: () => void;
  handleNext: () => void;
  handleSelect: (index: number) => void;
}

export default function NavigationLessons({
  flashcards,
  currentCardIndex,
  rating,
  handlePrev,
  handleNext,
  handleSelect,
}: NavigationLessonsProps) {
  return (
    <div className="relative flex items-center justify-between w-full mt-4">
      <motion.div
        initial={{ opacity: 0, x: -20 }}
        animate={{ opacity: 1, x: 0 }}
        transition={{ duration: 0.3 }}
        className="flex items-center gap-2"
      >
        <Button
          onClick={handlePrev}
          disabled={currentCardIndex === 0}
          variant="outline"
          size="lg"
          className="bg-base-100 backdrop-blur-sm border-0"
        >
          <ChevronLeft className="w-5 h-5 mr-2" />
          Предыдущий
        </Button>
      </motion.div>

      {/* Индикаторы */}
      <div
        className="
                    hidden sm:flex absolute left-1/2 -translate-x-1/2 
                    gap-2 items-center justify-center
                    "
      >
        {flashcards.map((card, index) => (
          <motion.div
            key={card.id}
            initial={{ scale: 0 }}
            animate={{ scale: 1 }}
            transition={{ delay: index * 0.05 }}
            onClick={() => handleSelect(index)}
            className={`w-3 h-3 rounded-full cursor-pointer ${
              colorRatingCard[rating[card.id] || 0]
            } ${
              index === currentCardIndex &&
              "border-1 border-white shadow shadow-white"
            }`}
          />
        ))}
      </div>
      <motion.div
        initial={{ opacity: 0, x: 20 }}
        animate={{ opacity: 1, x: 0 }}
        transition={{ duration: 0.3 }}
        className="flex items-center gap-2"
      >
        <Button
          onClick={handleNext}
          disabled={currentCardIndex === flashcards.length - 1}
          variant="outline"
          size="lg"
          className="bg-base-100 backdrop-blur-sm border-0"
        >
          Следующий
          <ChevronRight className="w-5 h-5 ml-2" />
        </Button>
      </motion.div>

      {/* Индикаторы для мобилок */}
      <div className="flex sm:hidden justify-center gap-2 absolute -bottom-8 left-1/2 -translate-x-1/2">
        {flashcards.map((card, index) => (
          <div
            key={card.id}
            onClick={() => handleSelect(index)}
            className={`w-3 h-3 rounded-full cursor-pointer ${
              colorRatingCard[rating[card.id] || 0]
            } ${
              index === currentCardIndex &&
              "border-1 border-white shadow shadow-white"
            }`}
          />
        ))}
      </div>
    </div>
  );
}
