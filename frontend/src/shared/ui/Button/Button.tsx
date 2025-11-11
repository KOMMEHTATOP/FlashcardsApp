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