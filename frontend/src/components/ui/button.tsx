import React from "react";

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?:
    | "primary"
    | "secondary"
    | "accent"
    | "ghost"
    | "outline"
    | "link"
    | "error";
  size?: "sm" | "md" | "lg";
  asChild?: boolean;
  loading?: boolean;
}

export function Button({
  className = "",
  variant = "primary",
  size = "md",
  asChild = false,
  loading = false,
  ...props
}: ButtonProps) {
  const Comp = asChild ? "span" : "button";

  const base = "btn transition-all font-medium flex items-center gap-2";

  const variants: Record<string, string> = {
    primary: "btn-primary",
    secondary: "btn-secondary",
    accent: "btn-accent",
    ghost: "btn-ghost",
    outline: "btn-outline border-base-100/30 hover:bg-base-100/10",
    link: "btn-link text-primary hover:underline",
    error: "btn-error text-white",
  };

  const sizes: Record<string, string> = {
    sm: "btn-sm",
    md: "",
    lg: "btn-lg",
  };

  return (
    <Comp
      className={`${base} ${variants[variant]} ${
        sizes[size]
      } ${className}  transition-all duration-300 ease-in-out ${
        loading ? "cursor-not-allowed opacity-50" : ""
      }`}
      {...props}
    />
  );
}

interface ButtonCircleProps
  extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  className?: string;
  size?: string;
  children?: React.ReactNode;
}

const circleSizes: Record<string, string> = {
  sm: "w-8 h-8",
  md: "w-10 h-10",
  lg: "w-12 h-12",
};

export function ButtonCircle({
  className = "",
  size,
  children,
  ...props
}: ButtonCircleProps) {
  return (
    <button
      className={`shrink-0 rounded-full ${
        circleSizes[size || "md"]
      } p-2 flex items-center justify-center hover:bg-base-300 hover:scale-105 transition-all duration-300 cursor-pointer ${className}`}
      {...props}
    >
      {children}
    </button>
  );
}

interface ButtonCard {
  className?: string;
  size?: string;
  children?: React.ReactNode;
  onClick?: () => void;
}

export function ButtonCard({
  className = "",
  // size,
  children,
  onClick,
}: ButtonCard) {
  return (
    <button
      className={` w-10 h-10 flex items-center justify-center bg-base-300/10 rounded-full z-10 hover:bg-white/20 hover:scale-105 transition-all duration-300 ${className}`}
      onClick={onClick}
    >
      {children}
    </button>
  );
}
