import { motion } from "framer-motion";
import { Check } from "lucide-react";
import { availableColors } from "@/shared/data";

interface GroupColorSelectorProps {
    selectedColor: string;
    onColorSelect: (color: string) => void;
}

export function GroupColorSelector({
    selectedColor,
    onColorSelect,
}: GroupColorSelectorProps) {
    return (
        <div className="space-y-2">
            <label className="text-sm font-medium text-gray-700">Выберите тему</label>
            <div className="grid grid-cols-4 sm:grid-cols-6 gap-3 mt-2">
                {availableColors.map((item) => (
                    <motion.button
                        key={item.id}
                        type="button"
                        onClick={() => onColorSelect(item.gradient)}
                        whileHover={{ scale: 1.1 }}
                        whileTap={{ scale: 0.9 }}
                        className="relative group w-full aspect-square"
                    >
                        <div
                            className={`w-full h-full rounded-full bg-gradient-to-br ${item.gradient
                                } shadow-sm transition-all ${selectedColor === item.gradient
                                    ? "ring-4 ring-purple-500 ring-offset-2"
                                    : "group-hover:ring-2 group-hover:ring-purple-300 group-hover:ring-offset-1"
                                }`}
                        >
                            {selectedColor === item.gradient && (
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
    );
}
