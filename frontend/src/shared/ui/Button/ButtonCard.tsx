import React from "react";

interface ButtonCardProps {
    className?: string;
    children?: React.ReactNode;
    onClick?: () => void;
}

export function ButtonCard({
                               className = "",
                               children,
                               onClick,
                           }: ButtonCardProps) {
    return (
        <button
            className={`w-10 h-10 flex items-center justify-center bg-base-300/10 rounded-full z-10 hover:bg-white/20 hover:scale-105 transition-all duration-300 ${className}`}
            onClick={onClick}
        >
            {children}
        </button>
    );
}