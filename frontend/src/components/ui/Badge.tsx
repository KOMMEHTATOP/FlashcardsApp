import { motion } from "framer-motion";
import { Check, Lock } from "lucide-react";
import React from "react";

interface BadgeProps {
    icon: React.ReactNode;
    title: string;
    description: string;
    earned?: boolean;
    gradient?: string;
    onClick?: () => void;
    onDelete?: () => void;
}

export function Badge({
    icon,
    title,
    description,
    earned,
    gradient,
    onClick,
    onDelete,
}: BadgeProps) {
    return (
        <motion.div
            whileHover={{ scale: 1.05, y: -5 }}
            whileTap={{ scale: 0.95 }}
            onClick={onClick}
            className={`relative p-6 rounded-2xl shadow-lg transition-all cursor-pointer ${earned
                    ? `bg-gradient-to-br from-yellow-400 to-orange-500 ${gradient}`
                    : "bg-base-300"
                }`}
            role="button"
            tabIndex={0}
        >
            {earned && (
                <motion.div
                    initial={{ scale: 0 }}
                    animate={{ scale: 1 }}
                    className="absolute -top-2 -right-2 bg-yellow-400 rounded-full p-1.5 z-10"
                >
                    <Check className="w-4 h-4 text-white" />
                </motion.div>
            )}
            <div className="flex flex-col items-center text-center gap-3">
                <div
                    className={`p-4 rounded-2xl ${earned ? "bg-white/20 backdrop-blur-sm" : "bg-base-200"
                        }`}
                >
                    <span
                        className={`text-2xl ${earned ? "text-white" : "text-gray-500"}`}
                    >
                        {icon}
                    </span>
                </div>
                <div>
                    <h4
                        className={
                            earned ? "text-white" : "text-gray-500 dark:text-gray-400"
                        }
                    >
                        {title}
                    </h4>
                    <p
                        className={`text-sm mt-1 ${earned ? "text-white/80" : "text-gray-400 dark:text-gray-500"
                            }`}
                    >
                        {description}
                    </p>
                </div>
            </div>
            {!earned && onDelete && (
                <button
                    onClick={(e) => {
                        e.stopPropagation();
                        onDelete();
                    }}
                    className="absolute top-2 right-2 p-1 text-gray-500 hover:text-red-500"
                >
                    {/* Иконка удаления, если нужно */}
                </button>
            )}
            {!earned && (
                <div className="absolute inset-0 bg-black/10 dark:bg-black/30 rounded-2xl flex items-center justify-center backdrop-blur-[1px]">
                    <span className="text-gray-600 dark:text-base-content">
                        <Lock className="w-8 h-8" />
                    </span>
                </div>
            )}
        </motion.div>
    );
}
