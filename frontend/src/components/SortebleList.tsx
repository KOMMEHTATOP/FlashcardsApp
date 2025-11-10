import {
    closestCenter,
    DndContext,
    type DragEndEvent,
    type DragOverEvent,
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
import { useData } from "../context/DataContext";
import type {
    ConfrimModalState,
    GroupCardType,
    GroupType,
} from "../types/types";
import { BookHeartIcon, Plus } from "lucide-react";
import GroupForm from "./modal/GroupForm";
import apiFetch from "../utils/apiFetch";
import Card from "./ui/card";
import shuffleArray from "../utils/shuffleArray";
import { availableIcons } from "../test/data";
import GroupCard from "./cards/Group_card";

export function SortableItem({
                                 id,
                                 children,
                             }: // index,
                             {
                                 id: string;
                                 children: React.ReactElement<any>;
                                 index: number;
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
            animate={{ scale: isDragging ? 1.03 : 1 }}
            transition={{ type: "spring", stiffness: 350, damping: 30 }}
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
    const {
        handleSelectLesson,
        deleteGroup,
        handleOpenConfrimModal,
        handleCloseConfrimModal,
        setting,
    } = useData();
    const navigate = useNavigate();

    const timerRef = useRef<ReturnType<typeof setTimeout>>(null);

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
        let res: GroupCardType[] = await apiFetch
            .get(`/groups/${group.Id}/cards`)
            .then((res) => res.data)
            .catch();

        if (!res || res.length === 0) return;
        res = res.filter((card) => card.LastRating >= setting?.MinRating);
        res = res.filter((card) => card.LastRating <= setting?.MaxRating);
        if (setting?.ShuffleOnRepeat) res = shuffleArray(res);

        if (!res || res.length === 0) return;
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

    return (
        <DndContext
            collisionDetection={closestCenter}
            onDragOver={handleDragOver}
            onDragEnd={handleDragEnd}
        >
            <SortableContext
                items={items.map((i) => i.Id)}
                strategy={verticalListSortingStrategy}
            >
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 space-y-1">
                    <AnimatePresence mode="popLayout">
                        <motion.button
                            whileHover={{ scale: 1.01, y: -5 }}
                            whileTap={{ scale: 1 }}
                            transition={{ duration: 0.2 }}
                            className="group relative w-full md:w-70 h-40"
                            onClick={handleOpen}
                        >
                            <Card className="p-6 border-2 border-dashed border-purple-300  hover:border-purple-500 bg-base-300  backdrop-blur-sm transition-all cursor-pointer h-full flex flex-col items-center justify-center">
                                <motion.div
                                    animate={{ rotate: [0, 90, 0] }}
                                    transition={{ duration: 2, repeat: Infinity, repeatDelay: 1 }}
                                    className="bg-gradient-to-br from-purple-400 to-pink-500 p-4 rounded-2xl mb-4 shadow-lg"
                                >
                                    <Plus className="w-8 h-8 text-white" />
                                </motion.div>
                                <p className="text-base-content/80 ">Добавить новую группу</p>
                            </Card>
                        </motion.button>
                        <GroupForm
                            key={"form"}
                            targetGroup={targetGroup}
                            isOpen={isOpen}
                            handleCancle={handleClose}
                        />
                        {items.map((item, index) => (
                            <SortableItem key={item.Id} id={item?.Id || ""} index={index}>
                                <GroupCard
                                    id={item.Id}
                                    gradient={item.GroupColor || "from-green-400 to-emerald-500"}
                                    streak={item.CardCount}
                                    progress={(item.CardCount / 10) * 100}
                                    icon={
                                        availableIcons.find((i) => i.name === item.GroupIcon)
                                            ?.icon || BookHeartIcon
                                    }
                                    title={item.GroupName}
                                    onClick={() => navigate(`/study/${item.Id.toString()}`)}
                                    onDelete={() => handleDeleteGroup(item)}
                                    onEdit={() => handleEdit(item)}
                                    onLessonPlayer={() => handleStartLesson(item)}
                                />
                            </SortableItem>
                        ))}
                    </AnimatePresence>
                </div>
            </SortableContext>
        </DndContext>
    );
}