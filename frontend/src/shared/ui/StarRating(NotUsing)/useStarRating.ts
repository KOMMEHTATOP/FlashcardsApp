import { useEffect, useMemo, useState } from "react";

function clamp(v: number | undefined, a: number, b: number) {
    if (v === undefined || Number.isNaN(v)) return a;
    return Math.min(Math.max(v, a), b);
}

export type UseStarRatingProps = {
    value?: number;
    defaultValue?: number;
    onChange?: (value: number) => void;
    max?: number;
    readOnly?: boolean;
};

export function useStarRating({
                                  value,
                                  defaultValue = 0,
                                  onChange,
                                  max = 5,
                                  readOnly = false,
                              }: UseStarRatingProps) {
    const isControlled = value !== undefined;
    const [internalValue, setInternalValue] = useState<number>(
        defaultValue ? clamp(defaultValue, 0, max) : 0
    );
    const [hoverIndex, setHoverIndex] = useState<number | null>(null);

    useEffect(() => {
        if (!isControlled) {
            setInternalValue(clamp(defaultValue, 0, max));
        }
    }, [defaultValue, max, isControlled]);

    const currentValue = isControlled ? clamp(value || 0, 0, max) : internalValue;

    const stars = useMemo(() => Array.from({ length: max }, (_, i) => i + 1), [max]);

    function handleSelect(v: number) {
        if (readOnly) return;
        if (!isControlled) setInternalValue(v);
        onChange?.(v);
    }

    function handleReset() {
        if (readOnly) return;
        if (!isControlled) setInternalValue(0);
        onChange?.(0);
        setHoverIndex(null);
    }

    function handleKeyDown(e: React.KeyboardEvent, starNum: number) {
        if (readOnly) return;
        if (e.key === "Enter" || e.key === " ") {
            e.preventDefault();
            handleSelect(starNum);
        }
        if (e.key === "ArrowLeft") {
            e.preventDefault();
            const next = clamp(currentValue - 1, 0, max);
            handleSelect(next);
        }
        if (e.key === "ArrowRight") {
            e.preventDefault();
            const next = clamp(currentValue + 1, 0, max);
            handleSelect(next);
        }
    }

    return {
        stars,
        hoverIndex,
        setHoverIndex,
        currentValue,
        handleSelect,
        handleReset,
        handleKeyDown,
        readOnly,
    };
}
