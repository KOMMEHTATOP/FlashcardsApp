import React from "react";

interface ButtonCircleProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
    className?: string;
    size?: "sm" | "md" | "lg";
    children?: React.ReactNode;
}

const circleSizes: Record<string, string> = {
    sm: "w-8 h-8",
    md: "w-10 h-10",
    lg: "w-12 h-12",
};

export function ButtonCircle({
    className = "",
    size = "md",
    children,
    ...props
}: ButtonCircleProps) {
    return (
        <button
            className={`shrink-0 rounded-full ${circleSizes[size]} p-2 flex items-center justify-center hover:bg-base-300 hover:scale-105 transition-all duration-300 cursor-pointer ${className}`}
            {...props}
        >
            {children}
        </button>
    );
}
