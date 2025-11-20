import { forwardRef } from "react";

export interface ThreeDCardProps extends React.HTMLAttributes<HTMLDivElement> {
    customClass?: string;
}

export const ThreeDCard = forwardRef<HTMLDivElement, ThreeDCardProps>(
    ({ customClass, ...rest }, ref) => (
        <div
            ref={ref}
            {...rest}
            className={`absolute top-1/2 left-1/2 rounded-xl [transform-style:preserve-3d] [will-change:transform] [backface-visibility:hidden] ${customClass ?? ""
                } ${rest.className ?? ""}`.trim()}
        />
    )
);
