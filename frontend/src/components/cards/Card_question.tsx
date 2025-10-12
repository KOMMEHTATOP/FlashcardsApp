import { CheckCircle, ClockFading, Edit, Lock, Play, Zap } from "lucide-react";
import type { SubjectCardType } from "../../types/types";
import { ButtonCircle } from "../ui/button";

type CardProps = {
  item: SubjectCardType;
  last?: boolean;
  onClick?: () => void;
};

export default function CardQuestion({ item, last, onClick }: CardProps) {
  const showAnimatedBorder = item.completed || last;

  return (
    <div
      className={`relative rounded-xl transition-all shadow-lg group ${
        showAnimatedBorder
          ? "p-[2px] shadow-gradient-success-r "
          : "border border-base-300 "
      }`}
    >
      {showAnimatedBorder && (
        <>
          <div
            className={`absolute inset-0 rounded-xl bg-[length:200%_200%]  ${
              last ? "border-gradient-primary " : "border-gradient-success-r "
            }`}
          ></div>
        </>
      )}

      <div
        className={`relative z-10 rounded-[10px] p-6 shadow-lg transition-all ${
          item.completed
            ? "bg-base-100"
            : last
            ? "bg-gradient-to-r from-base-100 to-base-200"
            : "bg-base-100"
        }`}
      >
        <div className="flex items-center gap-4">
          {/* Иконка */}
          <div
            className={`shrink-0 w-12 h-12 rounded-full flex items-center justify-center ${
              item.completed
                ? "bg-gradient-to-br from-green-400 to-emerald-500"
                : last
                ? "bg-gradient-to-br from-purple-500 to-pink-500"
                : "bg-base-300"
            }`}
          >
            {item.completed ? (
              <CheckCircle className="w-6 h-6 text-white" />
            ) : last ? (
              <Play className="w-6 h-6 text-white" />
            ) : (
              <Lock className="w-6 h-6 text-base-content/60" />
            )}
          </div>

          {/* Текст */}
          <div className="flex-1">
            <h3
              className={`mb-1 ${
                item.completed || last ? "text-base-content" : "text-gray-400"
              }`}
            >
              {item.title}
            </h3>
            <div className="flex items-center gap-4 text-sm truncate">
              <span
                className={
                  item.completed || last
                    ? "text-gray-600 dark:text-gray-400"
                    : "text-gray-400 dark:text-gray-500"
                }
              >
                <span className="flex gap-1 items-center">
                  <ClockFading className="w-4 h-4 text-base-content" />{" "}
                  {item.duration}
                </span>
              </span>
              <span
                className={`flex items-center gap-1 truncate ${
                  item.completed || last
                    ? "text-yellow-600 dark:text-yellow-400"
                    : "text-gray-400 dark:text-gray-500"
                }`}
              >
                <Zap className="w-4 h-4" />
                {item.xp} XP
              </span>
            </div>
          </div>

          {/* <ButtonCircle>
            <Trash className="w-6 h-6 text-error" />
          </ButtonCircle> */}
          <ButtonCircle className="">
            <Edit className="w-6 h-6 text-base-content" />
          </ButtonCircle>

          {/* кнопка */}
          <button
            disabled={!item.completed && !last}
            onClick={onClick}
            className={`px-4 py-2 rounded w-30 text-base ${
              item.completed
                ? "bg-green-500 hover:bg-green-600 text-white"
                : last
                ? "bg-gradient-to-r from-purple-500 to-pink-500 hover:from-purple-600 hover:to-pink-600 text-white"
                : "btn btn-neutral"
            }`}
          >
            {item.completed ? "Обзор" : last ? "Начало урока" : "Закрыт"}
          </button>
        </div>
      </div>
    </div>
  );
}
