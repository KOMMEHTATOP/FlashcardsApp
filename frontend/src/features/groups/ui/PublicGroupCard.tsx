import { motion } from "framer-motion";
import { GalleryVerticalEndIcon, Users, Eye, UserMinus, UserPlus, type LucideIcon } from "lucide-react";
import React from "react";
import { Button } from "@/shared/ui/Button";

interface PublicGroupCardProps {
    id: string;
    icon: LucideIcon | string;
    title: string;
    cardCount: number;
    subscriberCount: number;
    authorName: string;
    gradient: string;
    createdAt: string;
    isSubscribed?: boolean;
    onView?: () => void;
    onSubscribe?: () => void;
    onUnsubscribe?: () => void;
}

export default function PublicGroupCard({
    id,
    icon,
    title,
    cardCount,
    subscriberCount,
    authorName,
    gradient,
    createdAt,
    isSubscribed = false,
    onView,
    onSubscribe,
    onUnsubscribe,
}: PublicGroupCardProps) {
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
        const action = target.closest("[data-action]")?.getAttribute("data-action");

        if (action === "subscribe") {
            if (isSubscribed) {
                onUnsubscribe?.();
            } else {
                onSubscribe?.();
            }
            return;
        }

        if (action === "view") {
            onView?.();
            return;
        }

        const dx = Math.abs(e.clientX - pointerStart.x);
        const dy = Math.abs(e.clientY - pointerStart.y);
        if (dx < 5 && dy < 5) onView?.();
    };

    return (
        <motion.div
            layout
            layoutId={`public-card-${id}`}
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
                <div
                    className={`bg-card text-card-foreground flex flex-col rounded-xl p-4 bg-gradient-to-br 
                    ${gradient} shadow-lg min-h-[240px]`}
                >
                    <div className="flex items-start justify-between mb-4">
                        <motion.div
                            className="bg-white/20 backdrop-blur-sm p-3 rounded-2xl flex items-center justify-center"
                        >
                            {typeof icon === "string" ? (
                                <span className="text-3xl">{icon}</span>
                            ) : (
                                React.createElement(icon, { className: "w-8 h-8 text-white" })
                            )}
                        </motion.div>

                        <motion.div
                            animate={{ rotate: [0, 10, -10, 0] }}
                            transition={{ duration: 1, repeat: Infinity, repeatDelay: 2 }}
                            className="bg-orange-500 text-white px-3 py-1 rounded-full text-sm flex items-center gap-1"
                        >
                            <GalleryVerticalEndIcon className="w-4 h-4 text-white" />
                            {cardCount}
                        </motion.div>
                    </div>

                    <h3 className="text-white text-xl mb-2 select-none">{title}</h3>

                    <div className="space-y-1 mb-3 flex-1">
                        <p className="text-white/80 text-sm">
                            👤 {authorName}
                        </p>
                        <div className="flex items-center gap-2 text-white/80 text-sm">
                            <Users className="w-4 h-4" />
                            <span>{subscriberCount} подписчиков</span>
                        </div>
                        <p className="text-white/60 text-xs mt-2">
                            Создано: {new Date(createdAt).toLocaleDateString("ru-RU")}
                        </p>
                    </div>

                    <div className="flex gap-2 mt-auto">
                        <Button
                            data-action="view"
                            variant="accent"
                            size="sm"
                            className="rounded-xl flex-1 flex items-center justify-center gap-2"
                        >
                            <Eye className="w-4 h-4" />
                            Просмотр
                        </Button>
                        <Button
                            data-action="subscribe"
                            variant="accent"
                            size="sm"
                            className={`rounded-xl flex-1 flex items-center justify-center gap-2 ${isSubscribed
                                    ? 'bg-red-500/80 hover:bg-red-600/80'
                                    : ''
                                }`}
                        >
                            {isSubscribed ? (
                                <>
                                    <UserMinus className="w-4 h-4" />
                                    Отписаться
                                </>
                            ) : (
                                <>
                                    <UserPlus className="w-4 h-4" />
                                    Подписаться
                                </>
                            )}
                        </Button>
                    </div>
                </div>
            </motion.div>
        </motion.div>
    );
}