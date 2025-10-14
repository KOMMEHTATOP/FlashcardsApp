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
import { motion } from "framer-motion";
import React, { useState } from "react";
import StudyCard from "./cards/Study_card";
import { useNavigate } from "react-router-dom";
import { useApp } from "../context/AppContext";
import type { GroupType } from "../types/types";
import { Star } from "lucide-react";
import GroupForm from "./modal/GroupForm";

export function SortableItem({
  id,
  children,
  index,
}: {
  id: string;
  children: React.ReactNode;
  index: number;
}) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    // transition,
    isDragging,
  } = useSortable({ id });

  const style = {
    transform: transform ? CSS.Transform.toString(transform) : undefined,
    touchAction: "none" as const,

    zIndex: isDragging ? 50 : 1,

    pointerEvents: isDragging ? "auto" : "auto",
  };

  return (
    <motion.div
      ref={setNodeRef}
      {...attributes}
      {...listeners}
      layout
      layoutId={id}
      initial={{ opacity: 0 }}
      animate={{ scale: isDragging ? 1.03 : 1, opacity: 1 }}
      transition={{
        type: "spring",
        stiffness: 350,
        damping: 30,
        mass: 0.8,
        delay: index * 0.1,
      }}
      className={`cursor-grab active:cursor-grabbing ${
        isDragging ? "shadow-2xl" : ""
      }`}
      style={style as React.CSSProperties}
    >
      {children}
    </motion.div>
  );
}

export default function SortableList({
  initalItems,
}: {
  initalItems: GroupType[];
}) {
  const [items, setItems] = useState(initalItems);
  const { handleSelectLesson } = useApp();
  const navigate = useNavigate();

  function handleDragOver(event: DragOverEvent) {
    const { active, over } = event;
    if (!over) return;
    if (active.id !== over.id) {
      setItems((prev) => {
        const oldIndex = prev.findIndex((x) => x.Id === active.id);
        const newIndex = prev.findIndex((x) => x.Id === over.id);
        return arrayMove(prev, oldIndex, newIndex);
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
        return arrayMove(prev, oldIndex, newIndex);
      });
    }
  }

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
          {items.map((item, index) => (
            <SortableItem key={item.Id} id={item?.Id || ""} index={index}>
              <motion.div layout layoutId={`card-${item.Id}`}>
                <StudyCard
                  gradient={"from-green-400 to-emerald-500"}
                  streak={4}
                  progress={21}
                  icon={Star}
                  title={item.GroupName}
                  onClick={() => navigate(`/study/${item.Id.toString()}`)}
                  onDelete={() => {}}
                  // onLessonPlayer={() => handleSelectLesson(item)}
                />
              </motion.div>
            </SortableItem>
          ))}
          <GroupForm />
        </div>
      </SortableContext>
    </DndContext>
  );
}
