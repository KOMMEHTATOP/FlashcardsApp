import { motion } from "framer-motion";
import { Check, Lock } from "lucide-react";
import React from "react";

interface BadgeCardProps {
    icon: React.ReactNode;
    title: string;
    description: string;
    earned?: boolean;
    gradient?: string;
}

export function BadgeCard({
                              icon,
                              title,
                              description,
                              earned,
                              gradient,
                          }: BadgeCardProps) {
    return (
        <motion.div
            // Увеличиваем масштаб
            whileHover={{ scale: 1.10, y: -5 }}

            // duration: 0.1 делает реакцию мгновенной.
            // ease: "easeOut" делает движение резким в начале и плавным в конце.
            transition={{ duration: 0.1, ease: "easeOut" }}

            className={`relative p-6 rounded-2xl shadow-lg transition-all h-full flex flex-col justify-between cursor-default ${
                earned
                    ? `bg-gradient-to-br from-yellow-400 to-orange-500 ${gradient}`
                    : "bg-base-300 opacity-80"
            }`}
        >
            {earned && (
                <motion.div
                    initial={{ scale: 0 }}
                    animate={{ scale: 1 }}
                    transition={{ duration: 0.2 }}
                    className="absolute -top-2 -right-2 bg-yellow-400 rounded-full p-1.5 z-10 shadow-sm border border-white/20"
                >
                    <Check className="w-4 h-4 text-white" />
                </motion.div>
            )}

            <div className="flex flex-col items-center text-center gap-3">
                <div
                    className={`p-4 rounded-2xl ${
                        earned ? "bg-white/20 backdrop-blur-sm" : "bg-base-200"
                    }`}
                >
                    <span className={`text-2xl flex items-center justify-center ${earned ? "text-white" : "text-gray-500"}`}>
                        {typeof icon === 'string' && icon.startsWith('http') ? (
                            <img src={icon} alt={title} className="w-8 h-8 object-contain" />
                        ) : (
                            icon
                        )}
                    </span>
                </div>

                <div className="w-full">
                    <h4
                        className={`font-bold text-lg mb-1 leading-tight ${
                            earned ? "text-white" : "text-gray-500 dark:text-gray-400"
                        }`}
                    >
                        {title}
                    </h4>
                    <p
                        className={`text-xs ${
                            earned ? "text-white/90" : "text-gray-400 dark:text-gray-500"
                        }`}
                    >
                        {description}
                    </p>
                </div>
            </div>

            {!earned && (
                <div className="absolute inset-0 bg-base-100/10 dark:bg-black/20 rounded-2xl flex items-center justify-center pointer-events-none">
                    <div className="bg-base-100/80 p-3 rounded-full backdrop-blur-sm shadow-sm">
                        <Lock className="w-6 h-6 text-gray-400" />
                    </div>
                </div>
            )}
        </motion.div>
    );
}