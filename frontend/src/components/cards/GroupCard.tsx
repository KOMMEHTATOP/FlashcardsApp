import { motion } from "framer-motion";
import {
    GalleryVerticalEndIcon,
    Globe,
    PenSquareIcon,
    Users,
    X,
    type LucideIcon,
} from "lucide-react";
import React from "react";
import { Button, ButtonCard } from "../../shared/ui/Button";

interface GroupCardProps {
    id: string;
    icon: LucideIcon;
    title: string;
    cardCount: number;
    gradient: string;
    onClick?: () => void;
    onStartLesson?: () => void;
    onDelete?: () => void;
    onEdit?: () => void;
    dragHandleProps?: any;
    isPublished?: boolean;
    authorName?: string;
}

export default function GroupCard({
                                      id,
                                      icon: Icon,
                                      title,
                                      cardCount,
                                      gradient,
                                      onClick,
                                      onStartLesson,
                                      onDelete,
                                      onEdit,
                                      dragHandleProps,
                                      isPublished,
                                      authorName,
                                  }: GroupCardProps) {
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

        switch (action) {
            case "delete":
                onDelete?.();
                return;
            case "edit":
                onEdit?.();
                return;
            case "lesson":
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
            layoutId={`card-${id}`}
            initial={{ opacity: 0, scale: 0.95 }}
            animate={{ opacity: 1, scale: 1 }}
            exit={{ opacity: 0, scale: 0.95 }}
            transition={{ duration: 0.3 }}
            {...dragHandleProps}
            style={{ touchAction: "none" }}
        >
            <motion.div
                whileHover={{ scale: 1.01, y: -5 }}
                whileTap={{ scale: 1 }}
                onPointerDown={handlePointerDown}
                onPointerUp={handlePointerUp}
                transition={{ duration: 0.2 }}
                className="cursor-pointer relative group"
            >
                {onDelete && (
                    <ButtonCard
                        data-name="delete"
                        className="group-hover:opacity-100 opacity-0 absolute -top-4 -right-4"
                    >
                        <X className="w-6 h-6 text-white" data-name="delete" />
                    </ButtonCard>
                )}

                <div
                    className={`bg-card text-card-foreground flex flex-col rounded-xl p-4 bg-gradient-to-br 
                    ${gradient} shadow-lg min-h-[180px]`}
                >
                    <div className="flex items-start justify-between mb-2">
                        <div className="relative">
                            <motion.div
                                className="bg-white/20 backdrop-blur-sm p-2 rounded-xl flex items-center justify-center"
                                animate={{ opacity: [1, 1] }}
                                transition={{ duration: 0.2 }}
                            >
                                <Icon className="w-6 h-6 text-white transition-all duration-300 group-hover:opacity-0" />
                            </motion.div>

                            {onEdit && (
                                <motion.div
                                    data-name="edit"
                                    initial={{ scale: 0.95 }}
                                    whileHover={{ scale: 1.0 }}
                                    whileTap={{ scale: 0.9 }}
                                    className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 
                                               opacity-0 group-hover:opacity-100 transition-all duration-300"
                                >
                                    <ButtonCard>
                                        <PenSquareIcon className="w-5 h-5 text-base-content" />
                                    </ButtonCard>
                                </motion.div>
                            )}
                        </div>

                        <div className="flex items-center gap-2">
                            {isPublished && (
                                <motion.div
                                    initial={{ scale: 0 }}
                                    animate={{ scale: 1 }}
                                    className="bg-green-500/80 backdrop-blur-sm text-white p-1.5 rounded-full"
                                    title="Опубликовано"
                                >
                                    <Globe className="w-4 h-4" />
                                </motion.div>
                            )}
                            <motion.div
                                animate={{ rotate: [0, 10, -10, 0] }}
                                transition={{ duration: 1, repeat: Infinity, repeatDelay: 2 }}
                                className="bg-orange-500 text-white px-3 py-1 rounded-full text-sm flex items-center gap-1"
                            >
                                <GalleryVerticalEndIcon className="w-4 h-4 text-white" />
                                {cardCount}
                            </motion.div>
                        </div>
                    </div>

                    <div className="flex-1">
                        <h3 className="text-white text-lg mb-1 select-none">{title}</h3>

                        {authorName && (
                            <div className="flex items-center gap-1 text-white/70 text-sm mb-3">
                                <Users className="w-4 h-4" />
                                <span>{authorName}</span>
                            </div>
                        )}
                    </div>

                    <div className="flex justify-end">
                        <Button
                            data-name="lesson"
                            variant="accent"
                            size="sm"
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