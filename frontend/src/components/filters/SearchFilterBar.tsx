import { Search, Calendar, TrendingUp, SortAsc } from "lucide-react";
import { useState, useEffect } from "react"; // Добавили useState и useEffect
import type { TagDto } from "../../types/types";

// Стили тегов
const tagStyles: Record<string, string> = {
    blue: "bg-blue-100 text-blue-700 border-blue-200 hover:bg-blue-200",
    green: "bg-emerald-100 text-emerald-700 border-emerald-200 hover:bg-emerald-200",
    red: "bg-rose-100 text-rose-700 border-rose-200 hover:bg-rose-200",
    yellow: "bg-amber-100 text-amber-700 border-amber-200 hover:bg-amber-200",
    purple: "bg-purple-100 text-purple-700 border-purple-200 hover:bg-purple-200",
    pink: "bg-pink-100 text-pink-700 border-pink-200 hover:bg-pink-200",
    indigo: "bg-indigo-100 text-indigo-700 border-indigo-200 hover:bg-indigo-200",
    teal: "bg-teal-100 text-teal-700 border-teal-200 hover:bg-teal-200",
    orange: "bg-orange-100 text-orange-700 border-orange-200 hover:bg-orange-200",
    default: "bg-base-200 text-base-content border-base-300 hover:bg-base-300"
};

export type SortOption = "date" | "popular" | "name";

interface SearchFilterBarProps {
    // Поиск
    search: string;
    onSearchChange: (value: string) => void;
    onSearchSubmit?: (e: React.FormEvent) => void;

    // Теги
    tags?: TagDto[];
    selectedTagId: string | null;
    onTagSelect: (tagId: string | null) => void;

    // Сортировка
    sortBy: SortOption;
    onSortChange: (sort: SortOption) => void;

    // Состояние загрузки
    loading?: boolean;
}

export function SearchFilterBar({
                                    search,
                                    onSearchChange,
                                    onSearchSubmit,
                                    tags = [],
                                    selectedTagId,
                                    onTagSelect,
                                    sortBy,
                                    onSortChange,
                                    loading = false
                                }: SearchFilterBarProps) {

    // 1. Локальный стейт для инпута, чтобы ввод был плавным
    const [localSearch, setLocalSearch] = useState(search);

    // 2. Синхронизация: если URL изменился извне (например, кнопка "Назад"), обновляем инпут
    useEffect(() => {
        setLocalSearch(search);
    }, [search]);

    // 3. Debounce: обновляем родительский стейт (и URL) только через 600мс после окончания ввода
    useEffect(() => {
        const handler = setTimeout(() => {
            // Вызываем onSearchChange только если значение реально отличается
            if (localSearch !== search) {
                onSearchChange(localSearch);
            }
        }, 600);

        return () => {
            clearTimeout(handler);
        };
    }, [localSearch, search, onSearchChange]);

    // Обработчик клика по тегу
    const handleTagClick = (tagId: string | null) => {
        if (selectedTagId === tagId) {
            onTagSelect(null);
        } else {
            onTagSelect(tagId);
        }
    };

    return (
        <div className="space-y-4">
            {/* Поиск */}
            <form
                onSubmit={(e) => {
                    e.preventDefault();
                    // При нажатии Enter форсируем поиск мгновенно, без задержки
                    onSearchChange(localSearch);
                    onSearchSubmit?.(e);
                }}
                className="flex gap-2"
            >
                <div className="relative flex-1">
                    <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-base-content opacity-50" />
                    <input
                        type="text"
                        placeholder="Поиск..."
                        // Связываем с локальным стейтом
                        value={localSearch}
                        onChange={(e) => setLocalSearch(e.target.value)}
                        className="input input-bordered w-full pl-10 bg-base-100"
                        // ВАЖНО: Убрали disabled={loading}, чтобы фокус не слетал
                        // Можно оставить disabled только при сабмите, если критично
                    />
                </div>
                {onSearchSubmit && (
                    <button type="submit" className="btn btn-primary" disabled={loading}>
                        Найти
                    </button>
                )}
            </form>

            {/* Список тегов */}
            {tags && tags.length > 0 && (
                <div className="relative group">
                    <div className="flex gap-2 overflow-x-auto pb-2 scrollbar-hide mask-linear-fade">
                        <button
                            onClick={() => handleTagClick(null)}
                            className={`btn btn-sm rounded-full border transition-all ${
                                selectedTagId === null
                                    ? "btn-neutral"
                                    : "btn-ghost border-base-300 bg-base-100"
                            }`}
                        >
                            Все
                        </button>

                        {tags.map((tag) => {
                            const isSelected = selectedTagId === tag.Id;
                            const styleClass = isSelected
                                ? "bg-neutral text-neutral-content border-neutral ring-2 ring-offset-2 ring-neutral"
                                : tagStyles[tag.Color?.toLowerCase() || "default"] || tagStyles["default"];

                            return (
                                <button
                                    key={tag.Id}
                                    onClick={() => handleTagClick(tag.Id)}
                                    className={`
                                        px-4 py-1 rounded-full text-sm font-medium border transition-all whitespace-nowrap
                                        ${styleClass}
                                        ${!isSelected && "hover:scale-105 active:scale-95"}
                                    `}
                                >
                                    {tag.Name}
                                </button>
                            );
                        })}
                    </div>
                    <div className="absolute right-0 top-0 bottom-2 w-8 bg-gradient-to-l from-base-300 to-transparent pointer-events-none md:hidden" />
                </div>
            )}

            {/* Сортировка */}
            <div className="flex gap-2 flex-wrap border-t border-base-content/10 pt-4">
                <span className="text-sm text-base-content/60 flex items-center mr-2">Сортировка:</span>
                <button onClick={() => onSortChange("date")} className={`btn btn-xs sm:btn-sm gap-2 ${sortBy === "date" ? "btn-active" : "btn-ghost"}`}>
                    <Calendar className="w-3 h-3 sm:w-4 sm:h-4" /> Новые
                </button>
                <button onClick={() => onSortChange("popular")} className={`btn btn-xs sm:btn-sm gap-2 ${sortBy === "popular" ? "btn-active" : "btn-ghost"}`}>
                    <TrendingUp className="w-3 h-3 sm:w-4 sm:h-4" /> Популярные
                </button>
                <button onClick={() => onSortChange("name")} className={`btn btn-xs sm:btn-sm gap-2 ${sortBy === "name" ? "btn-active" : "btn-ghost"}`}>
                    <SortAsc className="w-3 h-3 sm:w-4 sm:h-4" /> По алфавиту
                </button>
            </div>
        </div>
    );
}