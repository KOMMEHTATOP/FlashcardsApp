import { motion } from "framer-motion";
import { useState } from "react";
import type { RatingValue } from "../../types/types";
import { recallRatingInfo } from "../../test/data";
import { RecallStar } from "./RecallStar";

interface RecallRatingProps {
    disabled?: boolean;
    value: RatingValue;
    onChange: (val: RatingValue) => void;
    size?: number;
}

export const RecallRating = ({
    disabled = false,
    value,
    onChange,
    size = 8,
}: RecallRatingProps) => {
    const [hoverValue, setHoverValue] = useState<RatingValue | null>(null);
    const displayedValue = hoverValue ?? value;

    return (
        <div className="flex flex-col items-center space-y-2 select-none group relative">
            <div className="rating flex gap-1">
                {[1, 2, 3, 4, 5].map((num) => (
                    <RecallStar
                        key={num}
                        num={num}
                        value={value}
                        hoverValue={hoverValue}
                        displayedValue={displayedValue}
                        disabled={disabled}
                        size={size}
                        onChange={onChange}
                        onHover={setHoverValue}
                    />
                ))}
            </div>

            <motion.span
                key={displayedValue}
                initial={{ opacity: 0, y: -5 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.2 }}
                className={`font-medium transition-all duration-300 ease-in-out h-6 ${displayedValue === 5
                        ? "text-yellow-500 drop-shadow-[0_0_6px_rgba(255,215,0,0.6)] text-2xl"
                        : "text-base-content/50 text-lg"
                    }`}
            >
                {recallRatingInfo[displayedValue] ?? "Оцените, насколько запомнили"}
            </motion.span>
        </div>
    );
};
