import { motion } from "framer-motion";
import { User, Crown } from "lucide-react";
import type { UserData } from "@/types/types";

interface ProfileHeaderProps {
    user: UserData | null;
    level: number;
    progressPercent: number;
}

export function ProfileHeader({ user, level, progressPercent }: ProfileHeaderProps) {
    return (
        <div className="neon-bg rounded-2xl p-6">
            <div className="flex items-center gap-6">
                <div className="relative" title={`Уровень ${level}`}>
                    <div className="w-32 h-32 rounded-full bg-white/20 backdrop-blur-sm p-1">
                        <div className="w-full h-full rounded-full bg-base-100 flex items-center justify-center relative overflow-hidden">
                            <User className="w-16 h-16 text-base-content/50" />
                            <svg className="absolute inset-0 w-full h-full -rotate-90">
                                <circle
                                    cx="50%"
                                    cy="50%"
                                    r="45%"
                                    fill="none"
                                    stroke="currentColor"
                                    strokeWidth="4"
                                    className="text-base-300"
                                />
                                <circle
                                    cx="50%"
                                    cy="50%"
                                    r="45%"
                                    fill="none"
                                    stroke="url(#gradient)"
                                    strokeWidth="4"
                                    strokeDasharray={`${progressPercent * 2.83} 283`}
                                    strokeLinecap="round"
                                />
                                <defs>
                                    <linearGradient id="gradient">
                                        <stop offset="0%" stopColor="#a855f7" />
                                        <stop offset="100%" stopColor="#ec4899" />
                                    </linearGradient>
                                </defs>
                            </svg>
                        </div>
                    </div>
                    <div className="absolute -bottom-2 left-1/2 -translate-x-1/2 bg-gradient-to-r from-purple-500 to-pink-500 px-4 py-1 rounded-full shadow-lg">
                        <span className="text-white font-bold whitespace-nowrap">Lvl {level}</span>
                    </div>
                </div>

                <div className="flex-1">
                    <div className="flex items-center justify-between mb-2">
                        <h1 className="text-3xl font-bold text-white">{user?.Login}</h1>
                        <div
                            className="flex items-center gap-2 bg-gradient-to-r from-amber-500 to-yellow-400 px-4 py-2 rounded-full shadow-lg cursor-help"
                            title="Количество подписчиков на ваши колоды"
                        >
                            <Crown className="w-5 h-5 text-white" />
                            <span className="text-white font-bold text-lg">
                                {user?.TotalSubscribers || 0}
                            </span>
                        </div>
                    </div>
                    <div className="text-white/90 mb-4 text-lg">{user?.Email}</div>

                    <div className="relative bg-transparent rounded-full h-4 w-full overflow-hidden border-2 border-cyan-400 shadow-[0_0_15px_rgba(34,211,238,0.5)]">
                        <motion.div
                            initial={{ width: 0 }}
                            animate={{ width: `${progressPercent}%` }}
                            transition={{ duration: 1, ease: "easeOut" }}
                            className="h-full bg-white rounded-full"
                        />
                    </div>
                    <div className="flex justify-between w-full mt-2 text-base font-bold text-white">
                        <span>{user?.Statistics?.TotalXP || 0} XP</span>
                        <span>{progressPercent}% до уровня {level + 1}</span>
                    </div>
                </div>
            </div>
        </div>
    );
}
