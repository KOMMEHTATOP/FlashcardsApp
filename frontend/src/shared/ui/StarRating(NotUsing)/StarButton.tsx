import React from "react";
import { Star } from "lucide-react";

type StarButtonProps = {
    number: number;
    filled: boolean;
    size: number;
    readOnly: boolean;
    currentValue: number;
    onSelect: (value: number) => void;
    onHover: (value: number | null) => void;
    onKeyDown: (e: React.KeyboardEvent, value: number) => void;
};

export const StarButton: React.FC<StarButtonProps> = ({
                                                          number,
                                                          filled,
                                                          size,
                                                          readOnly,
                                                          currentValue,
                                                          onSelect,
                                                          onHover,
                                                          onKeyDown,
                                                      }) => {
    return (
        <button
            key={number}
            type="button"
            aria-checked={number === currentValue}
            role="radio"
            tabIndex={readOnly ? -1 : 0}
            onMouseEnter={() => !readOnly && onHover(number)}
            onMouseLeave={() => !readOnly && onHover(null)}
            onFocus={() => !readOnly && onHover(number)}
            onBlur={() => !readOnly && onHover(null)}
            onClick={() => onSelect(number)}
            onKeyDown={(e) => onKeyDown(e, number)}
            className={`flex items-center justify-center transition-colors duration-150 ${
                readOnly ? "cursor-default" : "cursor-pointer"
            }`}
            title={`${number} ${number === 1 ? "звезда" : "звезды"}`}
            style={{ width: size, height: size, lineHeight: 0 }}
        >
            <Star
                className={`w-full h-full ${
                    filled
                        ? "text-yellow-500 fill-yellow-500"
                        : "text-gray-600 fill-gray-600"
                }`}
            />
        </button>
    );
};
