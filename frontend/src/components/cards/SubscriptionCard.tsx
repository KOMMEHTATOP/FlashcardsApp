import { motion } from "framer-motion";
import {
    GalleryVerticalEndIcon,
    Users,
    type LucideIcon,
} from "lucide-react";
import React from "react";
import { Button } from "../../shared/ui/Button";

interface SubscriptionCardProps {
    id: string;
    icon: LucideIcon | string;
    title: string;
    cardCount: number;
    authorName: string;
    gradient: string;
    onClick?: () => void;
    onStartLesson?: () => void;
}

export default function SubscriptionCard({
                                             id,
                                             icon,
                                             title,
                                             cardCount,
                                             authorName,
                                             gradient,
                                             onClick,
                                             onStartLesson,
                                         }: SubscriptionCardProps) {
    const [pointerStart, setPointerStart] = React.useState<{
        x: number;
        y: number;
    } | null>(null);

    const handlePointerDown = (e: React.PointerEvent) => {
        setPointerStart({ x: e.clientX, y: e.clientY });
    };

    const handlePointerUp = (e: React.PointerEvent) => {
        if (!pointerStart) return;

        const target = e.target as HTMLElement;
        const action = target.closest("[data-name]")?.getAttribute("data-name");

        if (action === "lesson") {
            onStartLesson?.();
            return;
        }

        const dx = Math.abs(e.clientX - pointerStart.x);
        const dy = Math.abs(e.clientY - pointerStart.y);
        if (dx < 5 && dy < 5) onClick?.();
    };

    return (
        <motion.div
            layout
            layoutId={`subscription-card-${id}`}
            initial={{ opacity: 0, scale: 0.95 }}
            animate={{ opacity: 1, scale: 1 }}
            exit={{ opacity: 0, scale: 0.95 }}
            transition={{ duration: 0.3 }}
        >
            <motion.div
                whileHover={{ scale: 1.01, y: -5 }}
                whileTap={{ scale: 1 }}
                onPointerDown={handlePointerDown}
                onPointerUp={handlePointerUp}
                transition={{ duration: 0.2 }}
                className="cursor-pointer relative group"
            >
                {/* Контент */}
                <div
                    className={`bg-card text-card-foreground flex flex-col rounded-xl p-6 bg-gradient-to-br ${gradient} shadow-lg min-h-[160px]`}
                >
                    <div className="flex items-start justify-between mb-4">
                        {/* Иконка */}
                        <motion.div
                            className="bg-white/20 backdrop-blur-sm p-3 rounded-2xl flex items-center justify-center"
                        >
                            {typeof icon === "string" ? (
                                <span className="text-3xl">{icon}</span>
                            ) : (
                                React.createElement(icon, { className: "w-8 h-8 text-white" })
                            )}
                        </motion.div>

                        {/* Количество карточек */}
                        <motion.div
                            animate={{ rotate: [0, 10, -10, 0] }}
                            transition={{ duration: 1, repeat: Infinity, repeatDelay: 2 }}
                            className="bg-orange-500 text-white px-3 py-1 rounded-full text-sm flex items-center gap-1"
                        >
                            <GalleryVerticalEndIcon className="w-4 h-4 text-white" />
                            {cardCount}
                        </motion.div>
                    </div>

                    {/* Заголовок и автор */}
                    <div className="flex-1">
                        <h3 className="text-white text-xl mb-1 select-none">{title}</h3>
                        <div className="flex items-center gap-1 text-white/70 text-sm mb-3">
                            <Users className="w-4 h-4" />
                            <span>{authorName}</span>
                        </div>
                    </div>

                    {/* Кнопка Начать */}
                    <div className="flex justify-end">
                        <Button
                            data-name="lesson"
                            variant="accent"
                            size="md"
                            className="rounded-xl"
                        >
                            Начать
                        </Button>
                    </div>
                </div>
            </motion.div>
        </motion.div>
    );
}