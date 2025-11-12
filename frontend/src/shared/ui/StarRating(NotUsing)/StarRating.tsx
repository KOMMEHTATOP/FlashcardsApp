import React from "react";
import { Button } from "../Button";
import { StarButton } from "./StarButton";
import { useStarRating } from "./useStarRating";

type StarRatingProps = {
    value?: number;
    defaultValue?: number;
    onChange?: (value: number) => void;
    max?: number;
    readOnly?: boolean;
    size?: number;
    className?: string;
    showReset?: boolean;
    resetLabel?: string;
    name?: string;
};

export const StarRating: React.FC<StarRatingProps> = ({
                                                          value,
                                                          defaultValue = 0,
                                                          onChange,
                                                          max = 5,
                                                          readOnly = false,
                                                          size = 20,
                                                          className = "",
                                                          showReset = true,
                                                          resetLabel = "Сброс",
                                                          name,
                                                      }) => {
    const {
        stars,
        hoverIndex,
        setHoverIndex,
        currentValue,
        handleSelect,
        handleReset,
        handleKeyDown,
    } = useStarRating({ value, defaultValue, onChange, max, readOnly });

    return (
        <div className={`flex items-center gap-3 ${className}`}>
            <div
                className="flex items-center gap-1"
                role="radiogroup"
                aria-label="Рейтинг"
            >
                {name && <span>{name}</span>}

                {stars.map((n) => {
                    const filled = hoverIndex !== null ? n <= hoverIndex : n <= currentValue;
                    return (
                        <StarButton
                            key={n}
                            number={n}
                            filled={filled}
                            size={size}
                            readOnly={readOnly}
                            currentValue={currentValue}
                            onSelect={handleSelect}
                            onHover={setHoverIndex}
                            onKeyDown={handleKeyDown}
                        />
                    );
                })}
            </div>

            {showReset && (
                <Button
                    type="button"
                    variant="ghost"
                    onClick={handleReset}
                    disabled={readOnly || currentValue === 0}
                    className={`text-sm px-2 py-1 rounded-md border transition-colors duration-150 border-white/50 ${
                        readOnly
                            ? "opacity-50 cursor-not-allowed"
                            : "hover:bg-base-300 hover:text-base-content"
                    }`}
                    aria-label="Сбросить рейтинг"
                    title={resetLabel}
                >
                    {resetLabel}
                </Button>
            )}
        </div>
    );
};
