import { motion } from "framer-motion";
import { useState } from "react";
import { colorRatingCard } from "../../test/data";
import type { RatingValue } from "../../types/types";
import { recallRatingInfo } from "../../test/data";

interface RecallRatingProps {
  disabled?: boolean;
  value: RatingValue;
  onChange: (val: RatingValue) => void;
  size?: number;
}

export default function RecallRating({
  disabled = false,
  value,
  onChange,
  size = 8,
}: RecallRatingProps) {
  const [hoverValue, setHoverValue] = useState<RatingValue | null>(null);
  const displayedValue = hoverValue ?? value;

  return (
    <div className="flex flex-col items-center space-y-2 select-none group relative">
      <div className="rating flex gap-1">
        {[1, 2, 3, 4, 5].map((num) => {
          const active = hoverValue ? num <= hoverValue : num <= value;
          const isMax = value === 5 && active;

          return (
            <motion.input
              key={num}
              disabled={disabled}
              type="radio"
              name="recall-rating"
              value={num}
              animate={
                isMax
                  ? {
                      scale: [1, 1.15, 1],
                      y: [-1, 0, -1],
                      filter: [
                        "drop-shadow(0 0 0px rgba(255, 200, 0, 0.4))",
                        "drop-shadow(0 0 8px rgba(255, 220, 100, 0.9))",
                        "drop-shadow(0 0 0px rgba(255, 200, 0, 0.4))",
                      ],
                    }
                  : hoverValue && active
                  ? {
                      scale: 1.1,
                      filter: "drop-shadow(0 0 5px rgba(255, 220, 150, 0.6))",
                    }
                  : {}
              }
              transition={{ duration: 0.3 }}
              className={`mask mask-star-2 cursor-pointer w-${size} h-${size} transition-all duration-150 group-hover:opacity-70 ${
                active
                  ? colorRatingCard[displayedValue as RatingValue]
                  : "bg-gray-300 dark:bg-gray-600"
              }`}
              checked={value === num}
              onChange={() => onChange(num as RatingValue)}
              onMouseEnter={() => setHoverValue(num as RatingValue)}
              onMouseLeave={() => setHoverValue(null)}
            />
          );
        })}
      </div>

      <motion.span
        key={displayedValue}
        initial={{ opacity: 0, y: -5 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.2 }}
        className={` font-medium transition-all duration-300 ease-in-out h-6 ${
          displayedValue === 5
            ? "text-yellow-500 drop-shadow-[0_0_6px_rgba(255,215,0,0.6)] text-2xl"
            : "text-base-content/50 text-lg"
        }`}
      >
        {recallRatingInfo[displayedValue] ?? "Оцените, насколько запомнили"}
      </motion.span>
    </div>
  );
}
