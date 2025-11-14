import { motion } from "framer-motion";
import { Settings2Icon } from "lucide-react";
import SortableList from "../SortebleList";
import type { GroupType } from "../../types/types";

interface LessonsTabProps {
    groups: GroupType[] | undefined;
    onOpenSettings: () => void;
}

export function LessonsTab({ groups, onOpenSettings }: LessonsTabProps) {
    return (
        <motion.div
            key="lessons"
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -20 }}
            transition={{ duration: 0.4 }}
            className="space-y-6"
        >
            <div className="flex items-center justify-between">
                <h2 className="text-2xl text-base-content">Ваши карточки</h2>
                <div
                    onClick={onOpenSettings}
                    className="flex items-center gap-2 hover:scale-105 duration-300 transition-all cursor-pointer group opacity-70 hover:opacity-100"
                >
                    <span className="group-hover:opacity-90 opacity-0 duration-500 transition-opacity">
                        Настройки
                    </span>
                    <Settings2Icon className="w-8 h-8 z-10" />
                </div>
            </div>
            <SortableList initalItems={groups || []} />
        </motion.div>
    );
}