import React from "react";

interface CardProps extends React.ComponentProps<"div"> { }

export function Card({ className, ...props }: CardProps) {
    return (
        <div
            className={`bg-card text-card-foreground flex flex-col gap-2 rounded-xl border ${className}`}
            {...props}
        />
    );
}
