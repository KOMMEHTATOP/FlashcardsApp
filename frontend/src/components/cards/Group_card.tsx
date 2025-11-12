import { motion } from "framer-motion";
import {
  GalleryVerticalEndIcon,
  GripVertical,
  PenSquareIcon,
  X,
  type LucideIcon,
} from "lucide-react";
import React from "react";
import { Button, ButtonCard } from "../../shared/ui/Button";

interface StudyCardProps {
  id: string;
  icon: LucideIcon;
  title: string;
  progress: number;
  streak: number;
  gradient: string;
  onClick?: () => void;
  onDelete?: () => void;
  onEdit?: () => void;
  onLessonPlayer?: () => void;
  dragHandleProps?: any;
}

export default function GroupCard({
  id,
  icon: Icon,
  title,
  // progress,
  streak,
  gradient,
  onClick,
  onDelete,
  onEdit,
  onLessonPlayer,
  dragHandleProps,
}: StudyCardProps) {
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
        onLessonPlayer?.();
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
      exit={{ opacity: 0, y: -30 }}
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
          {...dragHandleProps}
          style={{ touchAction: "none" }}
          className="absolute top-0 left-0 text-base-content/50 hover:text-base-content cursor-grab active:cursor-grabbing p-1 rounded-md"
        >
          <GripVertical className="w-7 h-7" />
        </div>
        {/* Удаление */}
        {onDelete && (
          <ButtonCard
            data-name="delete"
            className="group-hover:opacity-100 opacity-0 absolute -top-4 -right-4"
          >
            <X className="w-6 h-6 text-white" data-name="delete" />
          </ButtonCard>
        )}

        {/* Контент */}
        <div
          className={`bg-card text-card-foreground flex flex-col rounded-xl p-6 bg-gradient-to-br ${gradient} shadow-lg`}
        >
          <div className="flex items-start justify-between mb-4">
            {/* иконка_кнопка редактирования */}
            <div className="relative">
              {/* Иконка */}
              <motion.div
                className="bg-white/20 backdrop-blur-sm p-3 rounded-2xl flex items-center justify-center"
                animate={{ opacity: [1, 1] }}
                transition={{ duration: 0.2 }}
              >
                <Icon className="w-8 h-8 text-white transition-all duration-300 group-hover:opacity-0" />
              </motion.div>

              {/* Кнопка редактирования */}
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
                    <PenSquareIcon className="w-6 h-6 text-base-content" />
                  </ButtonCard>
                </motion.div>
              )}
            </div>

            {/* Количество дней */}
            <motion.div
              animate={{ rotate: [0, 10, -10, 0] }}
              transition={{ duration: 1, repeat: Infinity, repeatDelay: 2 }}
              className="bg-orange-500 text-white px-3 py-1 rounded-full text-sm flex items-center gap-1"
            >
              <GalleryVerticalEndIcon className="w-4 h-4 text-white" />
              {streak}
            </motion.div>
          </div>

          {/* Заголовок и кнопка */}
          <div className="justify-between flex items-center">
            <h3 className="text-white text-xl mb-3 select-none">{title}</h3>
            <Button
              data-name="lesson"
              variant="accent"
              size="md"
              className="rounded-xl"
            >
              Начать
            </Button>
          </div>

          {/* Прогресс */}
          {/* <div className="space-y-2">
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
        </div> */}
        </div>
      </motion.div>
    </motion.div>
  );
}
