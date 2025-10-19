interface SliderProps {
  value: number;
  min: number;
  max: number;
  step: number;
  className?: string;
  onValueChange: (value: number) => void;
}

export default function Slider({
  value,
  min,
  max,
  step,
  className,
  onValueChange,
  ...props
}: SliderProps) {
  return (
    <div>
      <input
        type="range"
        min={min}
        max={max}
        step={step}
        value={value}
        className={`range range-neutral range-xs w-full ${className}`}
        onChange={(e) => onValueChange(Number(e.target.value))}
        {...props}
      />
    </div>
  );
}
