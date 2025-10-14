import { CheckCircle, Edit, Play, Star, Trash } from "lucide-react";
import type { GroupCardType } from "../../types/types";
import { ButtonCircle } from "../ui/button";

type CardProps = {
  item: GroupCardType;
  onClick?: () => void;
  onDelete?: () => void;
  onEdit?: () => void;
};

export default function CardQuestion({
  item,
  onClick,
  onDelete,
  onEdit,
}: CardProps) {
  const isCompleted = item.LastRating > 0;

  return (
    <div
      className={`relative rounded-xl transition-all shadow-lg group ${
        isCompleted
          ? "p-[2px] shadow-gradient-success-r"
          : "border border-base-300"
      }`}
    >
      {/* Градиентная подсветка для завершённых */}
      {isCompleted && (
        <div className="absolute inset-0 rounded-xl bg-[length:200%_200%] border-gradient-success-r animate-[shine_3s_linear_infinite]" />
      )}

      <div className="relative z-10 rounded-[10px] p-6 shadow-lg transition-all bg-base-100">
        <div className="flex items-center gap-4">
          {/* Иконка */}
          <div
            className={`shrink-0 w-12 h-12 rounded-full flex items-center justify-center transition-all ${
              isCompleted
                ? "bg-gradient-to-br from-green-400 to-emerald-500 animate-pulse"
                : "bg-gray-400/70 hover:bg-green-400/80"
            }`}
          >
            {isCompleted ? (
              <CheckCircle className="w-6 h-6 text-white" />
            ) : (
              <Play className="w-6 h-6 text-white" />
            )}
          </div>

          {/* Вопрос и рейтинг */}
          <div className="flex-1">
            <h3
              className={`mb-1 text-base font-medium ${
                isCompleted ? "text-base-content" : "text-gray-400"
              }`}
            >
              {item.Question}
            </h3>

            <div className="flex items-center gap-1">
              {[...Array(5)].map((_, i) => (
                <Star
                  key={i}
                  className={`w-5 h-5 transition-colors ${
                    i < item.LastRating
                      ? "text-yellow-500 fill-yellow-500"
                      : "text-gray-600"
                  }`}
                />
              ))}
            </div>
          </div>
          <div className=" opacity-0 group-hover:opacity-100 transition-all duration-600 flex gap-3">
            <ButtonCircle onClick={onDelete}>
              <Trash className="w-6 h-6 text-error" />
            </ButtonCircle>
            <ButtonCircle onClick={onEdit}>
              <Edit className="w-6 h-6 text-base-content" />
            </ButtonCircle>
          </div>
          {/* Кнопка “Обзор */}
          <button
            onClick={onClick}
            className={`px-4 py-2 rounded text-base font-medium ${"bg-green-500 hover:bg-green-600 text-white"}`}
          >
            Обзор
          </button>
        </div>
      </div>
    </div>
  );
}
