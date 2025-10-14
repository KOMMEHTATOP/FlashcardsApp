import { Star } from "lucide-react";
import React, { useEffect, useMemo, useState } from "react";
import { Button } from "./button";

type StarInputProps = {
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

const StarInput: React.FC<StarInputProps> = ({
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

  const stars = useMemo(
    () => Array.from({ length: max }, (_, i) => i + 1),
    [max]
  );

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

  return (
    <div className={`flex items-center gap-3 ${className}`}>
      <div
        className="flex items-center gap-1"
        role="radiogroup"
        aria-label="Рейтинг"
      >
        <span>{name}</span>
        {stars.map((n) => {
          const filled =
            hoverIndex !== null ? n <= hoverIndex : n <= currentValue;
          return (
            <button
              key={n}
              type="button"
              aria-checked={n === currentValue}
              role="radio"
              tabIndex={readOnly ? -1 : 0}
              onMouseEnter={() => !readOnly && setHoverIndex(n)}
              onMouseLeave={() => !readOnly && setHoverIndex(null)}
              onFocus={() => !readOnly && setHoverIndex(n)}
              onBlur={() => !readOnly && setHoverIndex(null)}
              onClick={() => handleSelect(n)}
              onKeyDown={(e) => handleKeyDown(e, n)}
              className={`flex items-center justify-center transition-colors duration-150 ${
                readOnly ? "cursor-default" : "cursor-pointer"
              }`}
              title={`${n} ${n === 1 ? "звезда" : "звезды"}`}
              style={{ width: size, height: size, lineHeight: 0 }}
            >
              <Star
                className={`w-full h-full ${
                  filled
                    ? "text-yellow-500 fill-yellow-500"
                    : "text-gray-600 fill-gray-600"
                } `}
              ></Star>
            </button>
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

function clamp(v: number | undefined, a: number, b: number) {
  if (v === undefined || Number.isNaN(v)) return a;
  return Math.min(Math.max(v, a), b);
}

export default StarInput;
