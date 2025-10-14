import { motion } from "framer-motion";
import { Flame, X, type LucideIcon } from "lucide-react";
import React from "react";
import { Button } from "../ui/button";

interface StudyCardProps {
  icon: LucideIcon;
  title: string;
  progress: number;
  streak: number;
  gradient: string;
  onClick?: () => void;
  onDelete?: () => void;
  onLessonPlayer?: () => void;
}

export default function StudyCard({
  icon: Icon,
  title,
  progress,
  streak,
  gradient,
  onClick,
  onDelete,
  onLessonPlayer,
}: StudyCardProps) {
  const [pointerStart, setPointerStart] = React.useState<{
    x: number;
    y: number;
  } | null>(null);

  const handlePointerDown = (e: React.PointerEvent) =>
    setPointerStart({ x: e.clientX, y: e.clientY });

  const handlePointerUp = (e: React.PointerEvent) => {
    if (!pointerStart) return;

    const target = e.target as HTMLElement;
    if (target.closest("button")) {
      onLessonPlayer?.();
      return;
    } else if (target.closest("svg")) {
      onDelete?.();
      return;
    }

    const dx = Math.abs(e.clientX - pointerStart.x);
    const dy = Math.abs(e.clientY - pointerStart.y);
    if (dx < 5 && dy < 5) onClick?.();
  };

  return (
    <motion.div
      whileHover={{ scale: 1.01, y: -5 }}
      whileTap={{ scale: 1 }}
      onPointerDown={handlePointerDown}
      onPointerUp={handlePointerUp}
      transition={{ duration: 0.2 }}
      className="cursor-pointer relative group "
    >
      {/* удаления */}
      {onDelete && (
        <div className="group-hover:opacity-100 opacity-0 absolute -top-4 -right-4 w-10 h-10 flex items-center justify-center bg-base-300/10 rounded-full z-10 hover:bg-white/20 hover:scale-105 transition-all duration-300">
          <X
            className="w-6 h-6 text-base-content"
            onClick={(e) => {
              e.stopPropagation();
              onDelete?.();
            }}
          />
        </div>
      )}

      {/* контент */}
      <div
        className={`bg-card text-card-foreground flex flex-col rounded-xl p-6 bg-gradient-to-br ${gradient} shadow-lg`}
      >
        <div className="flex items-start justify-between mb-4">
          <div className="bg-white/20 backdrop-blur-sm p-3 rounded-2xl">
            <Icon className="w-8 h-8 text-white" />
          </div>
          {streak > 0 && (
            <motion.div
              animate={{ rotate: [0, 10, -10, 0] }}
              transition={{ duration: 1, repeat: Infinity, repeatDelay: 2 }}
              className="bg-orange-500 text-white px-3 py-1 rounded-full text-sm flex items-center gap-1"
            >
              <Flame className="w-4 h-4 text-yellow" /> {streak}
            </motion.div>
          )}
        </div>
        <div className="justify-between flex items-center">
          <h3 className="text-white text-xl mb-3 select-none">{title}</h3>
          <Button
            onClick={(e) => {
              e.stopPropagation();
              onLessonPlayer?.();
            }}
            variant="accent"
            size="md"
            className="rounded-xl"
          >
            Начать
          </Button>
        </div>

        <div className="space-y-2">
          <div className="flex justify-between text-white/80 text-sm select-none">
            <span>Прогресс: {progress}%</span>
          </div>
          <div className="bg-white/20 rounded-full h-2 overflow-hidden">
            <motion.div
              initial={{ width: 0 }}
              animate={{ width: `${progress}%` }}
              transition={{ duration: 1, delay: 0.2 }}
              className="h-full bg-white"
            />
          </div>
        </div>
      </div>
    </motion.div>
  );
}
