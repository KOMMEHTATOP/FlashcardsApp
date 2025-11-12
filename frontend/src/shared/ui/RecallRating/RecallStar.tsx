import React from "react";
import { motion } from "framer-motion";
import type { RatingValue } from "../../../types/types";
import { colorRatingCard } from "../../../test/data";

interface RecallStarProps {
    num: number;
    value: RatingValue;
    hoverValue: RatingValue | null;
    displayedValue: RatingValue;
    disabled: boolean;
    size: number;
    onChange: (val: RatingValue) => void;
    onHover: (val: RatingValue | null) => void;
}

export const RecallStar: React.FC<RecallStarProps> = ({
                                                          num,
                                                          value,
                                                          hoverValue,
                                                          displayedValue,
                                                          disabled,
                                                          size,
                                                          onChange,
                                                          onHover,
                                                      }) => {
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
            className={`mask mask-star-2 cursor-pointer transition-all duration-150 group-hover:opacity-70 ${
                active
                    ? colorRatingCard[displayedValue as RatingValue]
                    : "bg-gray-300 dark:bg-gray-600"
            }`}
            checked={value === num}
            onChange={() => onChange(num as RatingValue)}
            onMouseEnter={() => onHover(num as RatingValue)}
            onMouseLeave={() => onHover(null)}
            style={{
                width: `${size * 4}px`,
                height: `${size * 4}px`,
            }}
        />
    );
};
