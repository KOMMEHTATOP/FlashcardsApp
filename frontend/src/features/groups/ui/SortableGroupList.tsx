import {
    closestCenter,
    DndContext,
    type DragEndEvent,
    type DragOverEvent,
    PointerSensor,
    TouchSensor,
    useSensor,
    useSensors,
} from "@dnd-kit/core";
import {
    arrayMove,
    SortableContext,
    useSortable,
    verticalListSortingStrategy,
} from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import { AnimatePresence, motion } from "framer-motion";
import React, { useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useData } from "@/context/DataContext";
import type {
    ConfrimModalState,
    GroupCardType,
    GroupType,
} from "@/types/types";
import { BookHeartIcon, Check, Plus } from "lucide-react";
import GroupForm from "./GroupForm";
import apiFetch from "@/utils/apiFetch";
import { Card } from "@/shared/ui/Card";
import shuffleArray from "@/utils/shuffleArray";
import { availableIcons } from "@/shared/data";
import GroupCard from "./GroupCard";

export function SortableItem({
    id,
    children,
    isEditMode = false,
}: {
    id: string;
    children: React.ReactElement<any>;
    index: number;
    isEditMode?: boolean;
}) {
    const { attributes, listeners, setNodeRef, transform, isDragging } =
        useSortable({ id });

    const style: React.CSSProperties = {
        transform: transform ? CSS.Transform.toString(transform) : undefined,
        zIndex: isDragging ? 50 : 1,
    };

    return (
        <motion.div
            ref={setNodeRef}
            style={style}
            animate={{
                scale: isDragging ? 1.05 : 1,
                rotate: isEditMode && !isDragging
                    ? [0, -0.5, 0.5, -0.5, 0]
                    : isDragging
                        ? [0, -1, 1, -1, 0]
                        : 0,
            }}
            transition={{
                type: "spring",
                stiffness: 100, // Снижаем жесткость для более мягкого движения
                damping: 50,    // Оставляем или немного увеличиваем демпфирование
                rotate: {
                    duration: 0.4, //Чем выше значение тем медленнее тряска
                    repeat: isEditMode || isDragging ? Infinity : 0,
                    ease: "easeInOut",
                },
            }}
            className={`relative ${isDragging ? "shadow-2xl" : ""}`}
        >
            {React.cloneElement(children, {
                dragHandleProps: { ...attributes, ...listeners },
            })}
        </motion.div>
    );
}

export default function SortableList({
    initalItems,
}: {
    initalItems: GroupType[];
}) {
    const [items, setItems] = useState(initalItems);
    const [isEditMode, setIsEditMode] = useState(false);
    const {
        handleSelectLesson,
        deleteGroup,
        handleOpenConfrimModal,
        handleCloseConfrimModal,
        setting,
    } = useData();
    const navigate = useNavigate();

    const timerRef = useRef<ReturnType<typeof setTimeout>>(null);

    const sensors = useSensors(
        useSensor(PointerSensor, {
            activationConstraint: isEditMode
                ? { distance: 5 }
                : { delay: 500, tolerance: 5 },
        }),
        useSensor(TouchSensor, {
            activationConstraint: isEditMode
                ? { distance: 5 }
                : { delay: 500, tolerance: 5 },
        })
    );

    useEffect(() => {
        const sorted = [...initalItems].sort((a, b) => a.Order - b.Order);
        setItems(sorted);
    }, [initalItems]);

    useEffect(() => {
        return () => {
            if (timerRef.current) {
                clearTimeout(timerRef.current);
                timerRef.current = null;
            }
        };
    }, []);

    const handleDragStart = () => {
        setIsEditMode(true);
    };

    function handleDragOver(event: DragOverEvent) {
        const { active, over } = event;
        if (!over) return;
        if (active.id !== over.id) {
            setItems((prev) => {
                const oldIndex = prev.findIndex((x) => x.Id === active.id);
                const newIndex = prev.findIndex((x) => x.Id === over.id);
                const updated = arrayMove(prev, oldIndex, newIndex);

                const data = updated.map((item, index) => ({
                    Id: item.Id,
                    Order: index + 1,
                }));

                if (timerRef.current) clearTimeout(timerRef.current);

                timerRef.current = setTimeout(() => {
                    apiFetch.put(`/Group/reorder`, data);
                }, 2000);

                return updated;
            });
        }
    }

    function handleDragEnd(event: DragEndEvent) {
        const { active, over } = event;
        if (!over) return;
        if (active.id !== over.id) {
            setItems((prev) => {
                const oldIndex = prev.findIndex((x) => x.Id === active.id);
                const newIndex = prev.findIndex((x) => x.Id === over.id);
                const updated = arrayMove(prev, oldIndex, newIndex);
                return updated;
            });
        }
    }

    const handleStartLesson = async (group: GroupType) => {
        if (isEditMode) return; // Не начинать урок в режиме редактирования

        let res: GroupCardType[] = await apiFetch
            .get(`/groups/${group.Id}/cards`)
            .then((res) => res.data)
            .catch();

        if (!res || res.length === 0) return;
        res = res.filter((card) => card.LastRating >= setting?.MinRating);
        res = res.filter((card) => card.LastRating <= setting?.MaxRating);
        if (setting?.ShuffleOnRepeat) res = shuffleArray(res);

        if (!res || res.length === 0) return;
        window.scrollTo({ top: 0, behavior: "smooth" });
        handleSelectLesson(res, group);
    };

    const handleDeleteGroup = (item: GroupType) => {
        const modal: ConfrimModalState = {
            title: "Вы точно хотите удалить группу?",
            target: item.GroupName,
            handleCancel: () => handleCloseConfrimModal(),
            handleConfirm: () => {
                deleteGroup(item.Id);
                handleCloseConfrimModal();
            },
        };
        handleOpenConfrimModal(modal);
    };

    const [isOpen, setIsOpen] = useState(false);
    const [targetGroup, setTargetGroup] = useState<GroupType>();
    const handleEdit = (group: GroupType) => {
        setTargetGroup(group);
        setIsOpen(true);
    };
    const handleOpen = () => {
        setTargetGroup(undefined);
        setIsOpen(true);
    };
    const handleClose = () => setIsOpen(false);

    const handleCardClick = (groupId: string) => {
        if (isEditMode) return; // Не переходить в режиме редактирования
        navigate(`/study/${groupId}`);
    };

    return (
        <>
            <DndContext
                sensors={sensors}
                collisionDetection={closestCenter}
                onDragStart={handleDragStart}
                onDragOver={handleDragOver}
                onDragEnd={handleDragEnd}
            >
                <SortableContext
                    items={items.map((i) => i.Id)}
                    strategy={verticalListSortingStrategy}
                >
                    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
                        <AnimatePresence mode="popLayout">
                            <motion.button
                                whileHover={{ scale: 1.01, y: -5 }}
                                whileTap={{ scale: 1 }}
                                transition={{ duration: 0.2 }}
                                className="group relative w-full"
                                onClick={handleOpen}
                            >
                                <Card className="p-4 border-2 border-dashed border-purple-300 hover:border-purple-500 bg-base-300 backdrop-blur-sm transition-all cursor-pointer h-full flex flex-col items-center justify-center min-h-[180px]">
                                    <motion.div
                                        animate={{ rotate: [0, 90, 0] }}
                                        transition={{
                                            duration: 2,
                                            repeat: Infinity,
                                            repeatDelay: 1,
                                        }}
                                        className="bg-gradient-to-br from-purple-400 to-pink-500 p-3 rounded-2xl mb-3 shadow-lg"
                                    >
                                        <Plus className="w-6 h-6 text-white" />
                                    </motion.div>
                                    <p className="text-base-content/80 text-sm">
                                        Добавить новую группу
                                    </p>
                                </Card>
                            </motion.button>
                            <GroupForm
                                key={"form"}
                                targetGroup={targetGroup}
                                isOpen={isOpen}
                                handleCancle={handleClose}
                            />
                            {items.map((item, index) => (
                                <SortableItem
                                    key={item.Id}
                                    id={item?.Id || ""}
                                    index={index}
                                    isEditMode={isEditMode}
                                >
                                    <GroupCard
                                        id={item.Id}
                                        gradient={
                                            item.GroupColor || "from-green-400 to-emerald-500"
                                        }
                                        cardCount={item.CardCount}
                                        icon={
                                            availableIcons.find((i) => i.name === item.GroupIcon)
                                                ?.icon || BookHeartIcon
                                        }
                                        title={item.GroupName}
                                        onClick={() => handleCardClick(item.Id.toString())}
                                        onDelete={() => handleDeleteGroup(item)}
                                        onEdit={() => handleEdit(item)}
                                        onStartLesson={() => handleStartLesson(item)}
                                        isPublished={item.IsPublished}
                                    />
                                </SortableItem>
                            ))}
                        </AnimatePresence>
                    </div>
                </SortableContext>
            </DndContext>

            {/* Кнопка выхода из режима редактирования */}
            <AnimatePresence>
                {isEditMode && (
                    <motion.button
                        initial={{ opacity: 0, scale: 0.8, y: 20 }}
                        animate={{ opacity: 1, scale: 1, y: 0 }}
                        exit={{ opacity: 0, scale: 0.8, y: 20 }}
                        onClick={() => setIsEditMode(false)}
                        className="fixed bottom-6 right-6 bg-gradient-to-r from-green-500 to-emerald-500 text-white px-6 py-3 rounded-full shadow-lg z-50 flex items-center gap-2 hover:shadow-xl transition-shadow"
                    >
                        <Check className="w-5 h-5" />
                        Готово
                    </motion.button>
                )}
            </AnimatePresence>
        </>
    );
}