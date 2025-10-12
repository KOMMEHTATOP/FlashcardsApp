import type { LucideIcon } from "lucide-react";

type InputProps = {
  type: string;
  name: string;
  icon: LucideIcon;
  placeholder?: string;
  className?: string;
  required: boolean;
};

export default function Input({
  type,
  name,
  icon: Icon,
  placeholder,
  className,
  required,
}: InputProps) {
  return (
    <div className="space-y-2">
      <label htmlFor={name} className="text-gray-700">
        {name}
      </label>
      <div className="relative">
        <Icon className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400 z-10" />
        <input
          id={name}
          type={type}
          placeholder={placeholder}
          className={`pl-10 input input-bordered w-full rounded-xl bg-white/50 text-gray-900 ${className}`}
          required={required}
        />
      </div>
    </div>
  );
}
