import { motion, AnimatePresence } from "framer-motion";
import { Card } from "../../shared/ui/Card";
import { Check, PlusCircle, X, ChevronLeft, ChevronRight, Tag as TagIcon } from "lucide-react";
import { useEffect, useState, useRef, useMemo } from "react";
import Input from "../ui/input";
import { availableColors, availableIcons } from "../../test/data";
import { Button, ButtonCircle } from "../../shared/ui/Button";
import apiFetch from "../../utils/apiFetch";
import { useData } from "../../context/DataContext";
import type { GroupType, TagDto } from "../../types/types";
import { errorFormater } from "../../utils/errorFormater";

interface GroupFromProps {
    targetGroup?: GroupType;
    isOpen: boolean;
    handleCancle: () => void;
}

export default function GroupForm({
                                      targetGroup,
                                      isOpen,
                                      handleCancle,
                                  }: GroupFromProps) {
    const { setNewGroups, putGroups } = useData();

    // Refs и State для скролла иконок
    const iconsContainerRef = useRef<HTMLDivElement>(null);
    const [canScrollLeft, setCanScrollLeft] = useState(false);
    const [canScrollRight, setCanScrollRight] = useState(true);

    const [name, setName] = useState<string>("");
    const [selectColor, setSelectColor] = useState<string>(availableColors[0].gradient);
    const [selectIcon, setSelectIcon] = useState<number>(0);

    // --- СТЕЙТЫ ДЛЯ ТЕГОВ ---
    const [tagInput, setTagInput] = useState("");
    const [selectedTags, setSelectedTags] = useState<string[]>([]); // Выбранные теги (имена)
    const [availableTags, setAvailableTags] = useState<TagDto[]>([]); // Загруженные из базы
    const [showSuggestions, setShowSuggestions] = useState(false);
    // ------------------------

    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<string>("");

    // 1. Загрузка списка тегов для автокомплита
    useEffect(() => {
        if (isOpen) {
            apiFetch.get("/Subscriptions/tags")
                .then(res => setAvailableTags(res.data))
                .catch(console.error);
        }
    }, [isOpen]);

    // 2. Инициализация при открытии (Редактирование / Создание)
    useEffect(() => {
        if (targetGroup) {
            setName(targetGroup.GroupName);
            setSelectColor(
                availableColors.find((c) => c.gradient === targetGroup.GroupColor)?.gradient || availableColors[0].gradient
            );
            setSelectIcon(
                targetGroup.GroupIcon
                    ? availableIcons.findIndex((i) => i.name === targetGroup.GroupIcon)
                    : 0
            );
            // Заполняем теги, если они есть в группе (targetGroup.Tags должен быть TagDto[])
            if (targetGroup.Tags) {
                // Маппим объекты тегов обратно в массив имен строк для работы формы
                setSelectedTags(targetGroup.Tags.map(t => t.Name));
            } else {
                setSelectedTags([]);
            }
        } else {
            // Сброс для новой группы
            setName("");
            setSelectColor(availableColors[0].gradient);
            setSelectIcon(0);
            setSelectedTags([]); // Сбрасываем теги после успешного создания
        }
        setTagInput("");
    }, [targetGroup, isOpen]);

    // 3. Фильтрация подсказок тегов
    const tagSuggestions = useMemo(() => {
        if (!tagInput.trim()) return [];
        const inputLower = tagInput.toLowerCase();
        return availableTags
            .filter(t => t.Name.toLowerCase().includes(inputLower) && !selectedTags.includes(t.Name))
            .slice(0, 5); // Показываем топ-5 похожих
    }, [tagInput, availableTags, selectedTags]);

    // --- ЛОГИКА ТЕГОВ ---

    const addTag = (tag: string) => {
        const parts = tag.split(',').map(p => p.trim()).filter(p => p.length > 0);
        if (parts.length === 0 && tag.includes(',')) return; // Если ввели только запятую

        parts.forEach(cleanTag => {
            if (cleanTag && !selectedTags.includes(cleanTag)) {
                setSelectedTags(prev => [...prev, cleanTag]);
            }
        });
        setTagInput("");
        setShowSuggestions(false);
    };

    const removeTag = (tagToRemove: string) => {
        setSelectedTags(selectedTags.filter(t => t !== tagToRemove));
    };

    const handleTagInputKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
        // Добавление тега по Enter или Tab
        if (e.key === "Enter" || e.key === "Tab" || e.key === ",") {
            e.preventDefault();
            if (tagInput.trim()) {
                addTag(tagInput);
            }
        }
        // Удаление последнего тега на Backspace, если инпут пуст
        if (e.key === "Backspace" && !tagInput && selectedTags.length > 0) {
            removeTag(selectedTags[selectedTags.length - 1]);
        }
    };
    // -------------------------

    // Логика проверки скролла (как в прошлом варианте)
    const checkScroll = () => {
        if (iconsContainerRef.current) {
            const { scrollLeft, scrollWidth, clientWidth } = iconsContainerRef.current;
            setCanScrollLeft(scrollLeft > 0);
            setCanScrollRight(scrollLeft < scrollWidth - clientWidth - 1);
        }
    };

    // Проверяем скролл при загрузке и ресайзе
    useEffect(() => {
        checkScroll();
        window.addEventListener("resize", checkScroll);
        return () => window.removeEventListener("resize", checkScroll);
    }, [isOpen]);

    const scrollIcons = (direction: "left" | "right") => {
        if (iconsContainerRef.current) {
            const scrollAmount = 300; // На сколько пикселей крутить
            iconsContainerRef.current.scrollBy({
                left: direction === "left" ? -scrollAmount : scrollAmount,
                behavior: "smooth",
            });
        }
    };

    const handleSubmit = async () => {
        setError("");

        if (!name.trim()) {
            setError("Введите название группы");
            return;
        }

        if (!selectColor) {
            setError("Выберите цвет группы");
            return;
        }

        setLoading(true);

        const data = {
            Name: name,
            Color: selectColor,
            Order: 0,
            GroupIcon: availableIcons[selectIcon].name,
            Tags: selectedTags // Отправляем массив тегов
        };

        try {
            const res = targetGroup
                ? await apiFetch.put(`/Group/${targetGroup.Id}`, data)
                : await apiFetch.post("/Group", data);

            if (targetGroup) {
                putGroups(res.data);
            } else {
                setNewGroups(res.data);
                // Сброс формы
                setName("");
                setSelectColor(availableColors[0].gradient);
                setSelectIcon(0);
                setSelectedTags([]); // Сбрасываем теги после успешного создания
            }
            handleCancle();
        } catch (err: any) {
            setError(errorFormater(err) || "Произошла ошибка");
        } finally {
            setLoading(false);
        }
    };

    return (
        <>
            {isOpen && (
                <motion.div
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    transition={{ duration: 0.3 }}
                    className="fixed top-0 left-0 w-full h-full bg-black/50 flex items-center justify-center z-50"
                    onMouseDown={(e) => e.target === e.currentTarget && handleCancle()} // Закрытие по клику на фон
                >
                    <div className="w-[95dvw] md:w-[80dvw] lg:w-[40dvw] max-h-[90dvh] overflow-y-auto bg-white p-6 rounded-2xl scrollbar-hide relative">
                        {/* Хедер */}
                        <div>
                            <div className="flex justify-between items-start">
                                <div>
                                    <span className="text-2xl font-bold bg-gradient-to-r from-purple-600 to-pink-600 bg-clip-text text-transparent">
                                      {targetGroup ? "Редактировать колоду" : "Создать новую колоду"}
                                    </span>
                                    <p className="text-gray-500 text-sm mt-1">
                                        {targetGroup ? "Измените параметры" : "Заполните данные для создания"}
                                    </p>
                                </div>
                                <ButtonCircle onClick={handleCancle} className="hover:bg-gray-100">
                                    <X className="w-6 h-6 text-gray-500" />
                                </ButtonCircle>
                            </div>
                        </div>

                        <div className="space-y-6 mt-6">
                            {/* 1. Название */}
                            <div className="space-y-2">
                                <Input
                                    name="Название"
                                    placeholder="Например: Основы React, English A1..."
                                    required={true}
                                    onChange={(e) => setName(e.target.value)}
                                    value={name}
                                    type="text"
                                    className="bg-gray-50 border-gray-200 focus:border-purple-500 focus:ring-purple-200"
                                    icon={PlusCircle}
                                />
                            </div>

                            {/* 2. ТЕГИ (Новый блок) */}
                            <div className="space-y-2 relative">
                                <label className="text-sm font-medium text-gray-700">Теги (Категории)</label>
                                {/* Убрал min-h и уменьшил p-2 до py-1, чтобы соответствовать высоте Input */}
                                <div className="py-1 bg-gray-50 border border-gray-200 rounded-xl focus-within:border-purple-500 focus-within:ring-2 focus-within:ring-purple-200 transition-all flex flex-wrap gap-2">
                                    {/* Список выбранных тегов */}
                                    <AnimatePresence>
                                        {selectedTags.map(tag => (
                                            <motion.span
                                                key={tag}
                                                initial={{ scale: 0.8, opacity: 0 }}
                                                animate={{ scale: 1, opacity: 1 }}
                                                exit={{ scale: 0.8, opacity: 0 }}
                                                className="bg-white border border-purple-200 text-purple-700 px-3 py-1 rounded-full text-sm flex items-center gap-1 shadow-sm"
                                            >
                                                #{tag}
                                                <button
                                                    onClick={() => removeTag(tag)}
                                                    className="hover:text-red-500 transition-colors ml-1 focus:outline-none"
                                                >
                                                    <X className="w-3 h-3" />
                                                </button>
                                            </motion.span>
                                        ))}
                                    </AnimatePresence>

                                    {/* Инпут для ввода */}
                                    <input
                                        type="text"
                                        value={tagInput}
                                        onChange={(e) => {
                                            const value = e.target.value;
                                            setTagInput(value);
                                            // Если введена запятая, сразу добавляем тег
                                            if (value.endsWith(',')) {
                                                addTag(value.slice(0, -1)); // Добавляем без запятой
                                            }
                                            setShowSuggestions(true);
                                        }}
                                        onKeyDown={handleTagInputKeyDown}
                                        onBlur={() => setTimeout(() => setShowSuggestions(false), 200)} // Задержка, чтобы успел сработать клик по подсказке
                                        onFocus={() => setShowSuggestions(true)}
                                        // ИСПРАВЛЕНО: Новый Placeholder
                                        placeholder={selectedTags.length === 0 ? "Например: Программирование, SQL, English" : "+ тег"}
                                        // ИСПРАВЛЕНО: Явно указан цвет текста для ввода
                                        className="bg-transparent border-none outline-none text-sm flex-1 min-w-[120px] h-8 text-gray-800 placeholder:text-gray-400"
                                    />
                                </div>

                                {/* Выпадающий список подсказок */}
                                <AnimatePresence>
                                    {showSuggestions && tagSuggestions.length > 0 && (
                                        <motion.div
                                            initial={{ opacity: 0, y: -10 }}
                                            animate={{ opacity: 1, y: 0 }}
                                            exit={{ opacity: 0, y: -10 }}
                                            className="absolute top-full left-0 w-full bg-white border border-gray-200 rounded-xl shadow-xl mt-1 z-20 overflow-hidden"
                                        >
                                            {tagSuggestions.map(tag => (
                                                <button
                                                    key={tag.Id}
                                                    // Важно: onMouseDown, чтобы сработал до onBlur
                                                    onMouseDown={() => addTag(tag.Name)}
                                                    className="w-full text-left px-4 py-2 hover:bg-purple-50 text-sm text-gray-700 flex items-center gap-2"
                                                >
                                                    <TagIcon className="w-3 h-3 text-gray-400" />
                                                    {tag.Name}
                                                </button>
                                            ))}
                                        </motion.div>
                                    )}
                                </AnimatePresence>
                                <p className="text-xs text-gray-400 pl-1">Нажмите Enter или запятую, чтобы добавить тег</p>
                            </div>

                            {/* 3. Иконки (Горизонтальный скролл) */}
                            <div className="space-y-2 relative group/icons">
                                <label className="text-sm font-medium text-gray-700 flex justify-between">
                                    Выберите иконку
                                    <span className="text-xs text-gray-400 font-normal md:hidden">Свайпайте влево</span>
                                </label>

                                <div className="relative -mx-2 px-2">
                                    {/* Левая кнопка скролла */}
                                    <AnimatePresence>
                                        {canScrollLeft && (
                                            <motion.button
                                                initial={{ opacity: 0, x: -10 }}
                                                animate={{ opacity: 1, x: 0 }}
                                                exit={{ opacity: 0, x: -10 }}
                                                onClick={() => scrollIcons("left")}
                                                className="absolute left-0 top-1/2 -translate-y-1/2 z-10 bg-white/80 backdrop-blur-sm p-2 rounded-full shadow-lg border border-gray-100 hover:bg-white transition-all hidden md:flex"
                                            >
                                                <ChevronLeft className="w-5 h-5 text-gray-700" />
                                            </motion.button>
                                        )}
                                    </AnimatePresence>

                                    {/* Правая кнопка скролла */}
                                    <AnimatePresence>
                                        {canScrollRight && (
                                            <motion.button
                                                initial={{ opacity: 0, x: 10 }}
                                                animate={{ opacity: 1, x: 0 }}
                                                exit={{ opacity: 0, x: 10 }}
                                                onClick={() => scrollIcons("right")}
                                                className="absolute right-0 top-1/2 -translate-y-1/2 z-10 bg-white/80 backdrop-blur-sm p-2 rounded-full shadow-lg border border-gray-100 hover:bg-white transition-all hidden md:flex"
                                            >
                                                <ChevronRight className="w-5 h-5 text-gray-700" />
                                            </motion.button>
                                        )}
                                    </AnimatePresence>

                                    {/* Сам список иконок */}
                                    <div
                                        ref={iconsContainerRef}
                                        onScroll={checkScroll}
                                        className="grid grid-rows-3 grid-flow-col gap-3 overflow-x-auto pb-4 pt-1 auto-cols-max pr-2 scrollbar-hide scroll-smooth"
                                    >
                                        {availableIcons.map((item, index) => (
                                            <motion.button
                                                key={item.name}
                                                type="button"
                                                onClick={() => setSelectIcon(index)}
                                                whileHover={{ scale: 1.02 }}
                                                whileTap={{ scale: 0.98 }}
                                                className={`w-24 h-24 flex flex-col items-center justify-center p-2 rounded-xl border-2 transition-all shrink-0 ${
                                                    selectIcon === index
                                                        ? "border-purple-500 bg-purple-50 shadow-md"
                                                        : "border-gray-100 bg-white hover:border-purple-200"
                                                }`}
                                            >
                                                <item.icon
                                                    className={`w-8 h-8 mb-2 ${
                                                        selectIcon === index
                                                            ? "text-purple-600"
                                                            : "text-gray-400"
                                                    }`}
                                                />
                                                <p className={`text-[10px] text-center leading-tight line-clamp-2 w-full ${
                                                    selectIcon === index ? "text-purple-700 font-medium" : "text-gray-500"
                                                }`}>
                                                    {item.name}
                                                </p>
                                            </motion.button>
                                        ))}
                                    </div>
                                </div>
                            </div>

                            {/* 4. Цвета */}
                            <div className="space-y-2">
                                <label className="text-sm font-medium text-gray-700">Выберите тему</label>
                                <div className="grid grid-cols-4 sm:grid-cols-6 gap-3 mt-2">
                                    {availableColors.map((item, _) => (
                                        <motion.button
                                            key={item.id}
                                            type="button"
                                            onClick={() => setSelectColor(item.gradient)}
                                            whileHover={{ scale: 1.1 }}
                                            whileTap={{ scale: 0.9 }}
                                            className="relative group w-full aspect-square"
                                        >
                                            <div
                                                className={`w-full h-full rounded-full bg-gradient-to-br ${
                                                    item.gradient
                                                } shadow-sm transition-all ${
                                                    selectColor === item.gradient
                                                        ? "ring-4 ring-purple-500 ring-offset-2"
                                                        : "group-hover:ring-2 group-hover:ring-purple-300 group-hover:ring-offset-1"
                                                }`}
                                            >
                                                {selectColor === item.gradient && (
                                                    <motion.div
                                                        initial={{ scale: 0 }}
                                                        animate={{ scale: 1 }}
                                                        className="absolute inset-0 flex items-center justify-center"
                                                    >
                                                        <Check className="w-5 h-5 text-white drop-shadow-md" />
                                                    </motion.div>
                                                )}
                                            </div>
                                        </motion.button>
                                    ))}
                                </div>
                            </div>

                            {/* 5. Превью */}
                            <div className="space-y-2">
                                <label className="text-sm font-medium text-gray-700">
                                    Предварительный просмотр
                                </label>
                                <Card
                                    className={`p-6 bg-gradient-to-br ${selectColor} border-none shadow-lg relative overflow-hidden transition-all duration-500`}
                                >
                                    <div
                                        className="absolute inset-0 bg-white/10"
                                        style={{
                                            backgroundImage:
                                                "radial-gradient(circle at 4px 4px, rgba(255,255,255,0.15) 1px, transparent 0)",
                                            backgroundSize: "40px 40px",
                                        }}
                                    />
                                    <div className="relative z-10 flex items-center justify-between">
                                        <div className="flex items-center gap-4">
                                            <div className="bg-white/20 backdrop-blur-sm p-3 rounded-2xl shadow-inner">
                                                {(() => {
                                                    const IconComponent = availableIcons[selectIcon].icon;
                                                    return (
                                                        <IconComponent className="w-8 h-8 text-white" />
                                                    );
                                                })()}
                                            </div>
                                            <div>
                                                <h3 className="text-white text-xl font-bold tracking-wide drop-shadow-sm">
                                                    {name || "Название колоды"}
                                                </h3>
                                                <div className="flex items-center gap-2 text-white/80 text-sm">
                                                    <span>0 карточек</span>
                                                    {/* ПРЕВЬЮ ТЕГОВ */}
                                                    {selectedTags.length > 0 && (
                                                        <>
                                                            <span>•</span>
                                                            <span className="opacity-90">#{selectedTags[0]}</span>
                                                            {selectedTags.length > 1 && <span>+{selectedTags.length - 1}</span>}
                                                        </>
                                                    )}
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </Card>
                            </div>

                            {error && (
                                <div className="p-3 bg-red-50 border border-red-200 rounded-xl text-red-600 text-sm text-center">
                                    {error}
                                </div
                                >
                            )}

                            <div className="flex gap-3 pt-2">
                                <Button
                                    type="button"
                                    variant="ghost"
                                    onClick={handleCancle}
                                    className="flex-1 text-gray-500 hover:bg-gray-100"
                                >
                                    Отменить
                                </Button>
                                <Button
                                    loading={loading}
                                    type="submit"
                                    onClick={handleSubmit}
                                    className="flex-1 bg-gradient-to-r from-purple-600 to-pink-600 hover:from-purple-700 hover:to-pink-700 text-white shadow-lg shadow-purple-500/30 border-0"
                                >
                                    {loading
                                        ? "Сохранение..."
                                        : targetGroup
                                            ? "Сохранить изменения"
                                            : "Создать колоду"}
                                </Button>
                            </div>
                        </div>
                    </div>
                </motion.div>
            )}
        </>
    );
}