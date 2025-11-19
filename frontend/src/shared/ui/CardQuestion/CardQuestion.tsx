import { useState } from "react";
import { Edit, Star, Trash, FileText, ChevronRight, ChevronDown } from "lucide-react";
import type { GroupCardType } from "../../../types/types";
import { ButtonCircle } from "../Button";
import { AnimatePresence, motion } from "framer-motion";

type CardQuestionProps = {
    item: GroupCardType;
    onClick?: () => void;
    onDelete?: () => void;
    onEdit?: () => void;
    showOverviewButton?: boolean;
};

export function CardQuestion({
                                 item,
                                 onClick,
                                 onDelete,
                                 onEdit,
                                 showOverviewButton,
                             }: CardQuestionProps) {
    const [isOpen, setIsOpen] = useState(false);
    const isCompleted = item.LastRating > 0;
    const isOwnerMode = !!onEdit;

    const handleClick = (e?: React.MouseEvent) => {
        e?.stopPropagation();
        if (showOverviewButton) {
            setIsOpen(!isOpen);
        } else {
            onClick?.();
        }
    };

    return (
        <div
            className={`relative rounded-xl transition-all shadow-lg group ${
                isCompleted
                    ? "p-[2px] shadow-gradient-success-r"
                    : "border border-base-300"
            }`}
        >
            {isCompleted && (
                <div className="absolute inset-0 rounded-xl bg-[length:200%_200%] border-gradient-success-r animate-[shine_3s_linear_infinite]" />
            )}

            <div
                className={`relative z-10 rounded-[10px] p-6 shadow-lg transition-all bg-base-100 
                ${(onClick || showOverviewButton) ? "cursor-pointer hover:bg-base-200/50" : "cursor-default"}`}
                onClick={handleClick}
            >
                <div className="flex items-start overflow-hidden">
                    {/* ЛЕВАЯ ИКОНКА */}
                    <div
                        className={`shrink-0 w-12 h-12 rounded-full flex items-center justify-center transition-all mt-1
                        ${isOwnerMode ? "bg-gray-400/70 hover:bg-green-400/80" : "bg-base-200"}`}
                    >
                        {isOwnerMode ? (
                            <ButtonCircle
                                onClick={(e) => {
                                    e.stopPropagation();
                                    onEdit?.();
                                }}
                            >
                                <Edit className="w-6 h-6 text-base-content" />
                            </ButtonCircle>
                        ) : (
                            <FileText className="w-6 h-6 text-base-content/30" />
                        )}
                    </div>

                    {/* ЦЕНТР: Контент */}
                    <div className="flex-1 ml-4">
                        {/* Вопрос */}
                        <div
                            className={`mb-2 text-base font-medium prose prose-sm max-w-none ${
                                isCompleted ? "text-base-content" : "text-gray-500"
                            }`}
                            dangerouslySetInnerHTML={{ __html: item.Question }}
                        />

                        {/* Рейтинг */}
                        <div className="flex items-center gap-1 mb-2">
                            {[...Array(5)].map((_, i) => (
                                <Star
                                    key={i}
                                    className={`w-4 h-4 transition-colors ${
                                        i < item.LastRating
                                            ? "text-yellow-500 fill-yellow-500"
                                            : "text-base-content/20"
                                    }`}
                                />
                            ))}
                        </div>

                        {/* ОТВЕТ (Раскрывается) */}
                        <AnimatePresence>
                            {isOpen && (
                                <motion.div
                                    initial={{ height: 0, opacity: 0 }}
                                    animate={{ height: "auto", opacity: 1 }}
                                    exit={{ height: 0, opacity: 0 }}
                                    className="overflow-hidden"
                                >
                                    <div className="pt-4 mt-2 border-t border-base-200 text-base-content">
                                        <span className="text-xs font-bold text-base-content/50 uppercase block mb-1">Ответ:</span>
                                        <div
                                            className="prose prose-sm max-w-none"
                                            dangerouslySetInnerHTML={{ __html: item.Answer }}
                                        />
                                    </div>
                                </motion.div>
                            )}
                        </AnimatePresence>
                    </div>

                    {/* ПРАВАЯ ЧАСТЬ */}

                    {/* Удаление (только владелец) */}
                    {onDelete && (
                        <div className="md:opacity-0 group-hover:opacity-100 transition-all duration-600 flex mx-2 self-center">
                            <ButtonCircle
                                onClick={(e) => {
                                    e.stopPropagation();
                                    onDelete();
                                }}
                            >
                                <Trash className="w-6 h-6 text-error" />
                            </ButtonCircle>
                        </div>
                    )}

                    {/* Кнопка ОБЗОР (Владелец ИЛИ Авторизованный) */}
                    <div className="ml-2 self-center">
                        {(isOwnerMode || showOverviewButton) ? (
                            <button
                                onClick={handleClick}
                                className={`px-4 py-2 rounded text-sm font-medium hidden md:flex items-center gap-2 
                                ${isOpen ? "bg-base-300 text-base-content" : "bg-green-500 hover:bg-green-600 text-white"} 
                                shadow-md hover:shadow-lg transition-all`}
                            >
                                {isOpen ? "Скрыть" : "Обзор"}
                                {isOpen ? <ChevronDown className="w-4 h-4"/> : <ChevronRight className="w-4 h-4"/>}
                            </button>
                        ) : (
                            onClick && (
                                <div className="p-2 text-base-content/30">
                                    <ChevronRight className="w-6 h-6" />
                                </div>
                            )
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
}