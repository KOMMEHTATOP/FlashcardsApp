import React from "react";
import { motion } from "framer-motion";

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
    variant?:
    | "primary"
    | "secondary"
    | "accent"
    | "ghost"
    | "outline"
    | "link"
    | "error"
    | "confirm";
    size?: "sm" | "md" | "lg";
    asChild?: boolean;
    loading?: boolean;
    loadingIcon?: React.ComponentType<{ className?: string }>;
    leftIcon?: React.ComponentType<{ className?: string }>;
    rightIcon?: React.ComponentType<{ className?: string }>;
}

export function Button({
    className = "",
    variant = "primary",
    size = "md",
    asChild = false,
    loading = false,
    loadingIcon: LoadingIcon,
    leftIcon: LeftIcon,
    rightIcon: RightIcon,
    children,
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
        confirm: "w-full rounded-xl py-6 border-0 bg-gradient-to-r from-purple-500 to-pink-500 hover:from-purple-600 hover:to-pink-600 text-white shadow-lg text-lg",
    };

    const sizes: Record<string, string> = {
        sm: "btn-sm",
        md: "",
        lg: "btn-lg",
    };

    const content = loading && variant === "confirm" && LoadingIcon ? (
        <motion.div
            animate={{ rotate: 360 }}
            transition={{
                duration: 1,
                repeat: Infinity,
                ease: "linear",
            }}
        >
            <LoadingIcon className="w-5 h-5" />
        </motion.div>
    ) : (
        <span className="flex items-center gap-2">
            {LeftIcon && !loading && <LeftIcon className="w-5 h-5" />}
            {children}
            {RightIcon && !loading && <RightIcon className="w-5 h-5" />}
        </span>
    );

    return (
        <Comp
            className={`${base} ${variants[variant]} ${sizes[size]
                } ${className} transition-all duration-300 ease-in-out ${loading ? "cursor-not-allowed opacity-50" : ""
                }`}
            disabled={loading}
            {...props}
        >
            {content}
        </Comp>
    );
}
