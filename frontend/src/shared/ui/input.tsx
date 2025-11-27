import { Eye, EyeOff, type LucideIcon } from "lucide-react";
import { useState } from "react";

type InputProps = {
  type: string;
  name: string;
  icon: LucideIcon;
  placeholder?: string;
  className?: string;
  required: boolean;
  onChange?: (e: React.ChangeEvent<HTMLInputElement>) => void;
  value?: string;
};

export function Input({
                        type,
                        name,
                        icon: Icon,
                        placeholder,
                        className,
                        required,
                        onChange,
                        value,
                      }: InputProps) {
  const [showPassword, setShowPassword] = useState(false);
  const isPasswordType = type === "password";
  const inputType = isPasswordType ? (showPassword ? "text" : "password") : type;

  const togglePasswordVisibility = () => {
    setShowPassword(!showPassword);
  };

  return (
      <div className="space-y-2">
        <label htmlFor={name} className="text-gray-700 font-medium ml-1">
          {name}
        </label>
        <div className="relative">
          <Icon className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400 z-10" />

          <input
              id={name}
              type={inputType}
              name={name}
              onChange={onChange}
              value={value}
              placeholder={placeholder}
              className={`pl-10 ${isPasswordType ? "pr-10" : ""} input input-bordered w-full rounded-xl bg-white/50 text-gray-900 focus:bg-white transition-colors ${className}`}
              required={required}
          />

          {isPasswordType && (
              <button
                  type="button" 
                  onClick={togglePasswordVisibility}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 transition-colors z-10 focus:outline-none"
              >
                {showPassword ? (
                    <EyeOff className="w-5 h-5" />
                ) : (
                    <Eye className="w-5 h-5" />
                )}
              </button>
          )}
        </div>
      </div>
  );
}