import { motion } from "framer-motion";
import {
    GalleryVerticalEndIcon,
    Users,
    type LucideIcon,
} from "lucide-react";
import type { GroupType } from "../types/types";

interface GroupHeaderProps {
    group: GroupType;
    icon: LucideIcon;
    progress: number;
    isSubscriptionView: boolean;
    isSubscribed: boolean;
    isPublishing: boolean;
    submittingSubscription: boolean;
    publishError: string | null;
    canPublish: boolean;
    onTogglePublish: () => void;
    onToggleSubscription: () => void;
}

export function GroupHeader({
                                group,
                                icon: Icon,
                                progress,
                                isSubscriptionView,
                                isSubscribed,
                                isPublishing,
                                submittingSubscription,
                                publishError,
                                canPublish,
                                onTogglePublish,
                                onToggleSubscription,
                            }: GroupHeaderProps) {
    return (
        <motion.div
            initial={{ opacity: 0, y: -20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.5 }}
            className={`relative bg-gradient-to-br ${group.GroupColor || 'from-blue-500 to-blue-700'} px-4 sm:px-6 lg:px-8 py-12 overflow-hidden rounded-2xl shadow-xl flex flex-col items-center`}
        >
            {/* Паттерн фона */}
            <div
                className="absolute inset-0 bg-white/10"
                style={{
                    backgroundImage:
                        "radial-gradient(circle at 2px 2px, rgba(255,255,255,0.15) 1px, transparent 0)",
                    backgroundSize: "40px 40px",
                }}
            />

            <div className="max-w-7xl mx-auto relative z-10 w-full">
                <div className="flex flex-col sm:flex-row items-center justify-center sm:justify-start gap-6 mb-8 w-full">
                    {/* Иконка группы */}
                    <motion.div
                        initial={{ scale: 0 }}
                        animate={{ scale: 1 }}
                        transition={{ type: "spring", stiffness: 200 }}
                        className="bg-white/20 backdrop-blur-sm p-6 rounded-3xl flex-shrink-0"
                    >
                        <Icon className="w-20 h-20 md:w-16 md:h-16 text-white" />
                    </motion.div>

                    <div className="flex-1 text-center sm:text-left">
                        {/* Заголовок и бейджи */}
                        <div className="flex flex-col sm:flex-row items-center sm:items-start gap-3 mb-2 w-full flex-wrap">
                            <h1 className="text-4xl text-white">{group.GroupName}</h1>

                            {/* Количество карточек */}
                            {Number(group.CardCount) > 0 && (
                                <motion.div
                                    animate={{ rotate: [0, 10, -10, 0] }}
                                    transition={{
                                        duration: 1,
                                        repeat: Infinity,
                                        repeatDelay: 2,
                                    }}
                                    className="bg-orange-500 text-white px-3 py-2 rounded-full flex items-center gap-1 text-sm md:text-base line-clamp-1 truncate"
                                >
                                    <GalleryVerticalEndIcon className="w-4 md:w-5 text-yellow-300" />
                                    <span className="font-mono">
                                        {group.CardCount} карточек
                                    </span>
                                </motion.div>
                            )}

                            {/* Счётчик подписчиков */}
                            {group.IsPublished && (group.SubscriberCount || 0) > 0 && (
                                <div className="bg-white/20 text-white px-3 py-2 rounded-full flex items-center gap-1 text-sm">
                                    <Users className="w-4 h-4" />
                                    <span>{group.SubscriberCount} подписчиков</span>
                                </div>
                            )}

                            {/* Spacer */}
                            <div className="flex-1" />

                            {/* Checkbox публикации / подписки */}
                            <div className="flex flex-col items-end">
                                {isSubscriptionView ? (
                                    <label className={`
                                        flex items-center gap-2 cursor-pointer
                                        ${submittingSubscription ? 'opacity-50 cursor-wait' : ''}
                                    `}>
                                        <span className="text-white text-sm">
                                            Подписка на группу
                                        </span>
                                        <input
                                            type="checkbox"
                                            checked={isSubscribed}
                                            onChange={onToggleSubscription}
                                            disabled={submittingSubscription}
                                            className="checkbox checkbox-success bg-white border-2 border-gray-800"
                                        />
                                        {submittingSubscription && (
                                            <span className="loading loading-spinner loading-xs text-white"></span>
                                        )}
                                    </label>
                                ) : (
                                    <>
                                        <label className={`
                                            flex items-center gap-2 cursor-pointer
                                            ${!canPublish ? 'opacity-50 cursor-not-allowed' : ''}
                                            ${isPublishing ? 'opacity-50 cursor-wait' : ''}
                                        `}>
                                            <span className="text-white text-sm">
                                                Поделиться с другими
                                            </span>
                                            <input
                                                type="checkbox"
                                                checked={group.IsPublished || false}
                                                onChange={onTogglePublish}
                                                disabled={isPublishing || !canPublish}
                                                className="checkbox checkbox-success bg-white border-2 border-gray-800"
                                            />
                                            {isPublishing && (
                                                <span className="loading loading-spinner loading-xs text-white"></span>
                                            )}
                                        </label>

                                        {!canPublish && (
                                            <div className="text-xs text-white/70 mt-1">
                                                Нужно минимум 10 карточек
                                            </div>
                                        )}
                                    </>
                                )}
                            </div>
                        </div>

                        {/* Ошибка публикации */}
                        {publishError && !isSubscriptionView && (
                            <div className="bg-red-500/20 text-white px-3 py-2 rounded-lg text-sm mb-2">
                                {publishError}
                            </div>
                        )}

                        <p className="text-white/90 text-lg mb-4">
                            Овладейте основами и раскройте свой потенциал
                        </p>

                        {/* Прогресс-бар */}
                        <div className="space-y-2 w-full">
                            <div className="flex justify-between text-white/80 text-sm">
                                <span className="text-base-content/80">Общий прогресс</span>
                                <span className="font-mono">
                                    {progress ? Number(progress).toFixed(0) : 0}%
                                </span>
                            </div>
                            <div className="relative z-10 w-full h-4 bg-white/10 rounded-full">
                                <motion.div
                                    initial={{ width: 0 }}
                                    animate={{ width: `${progress || 0}%` }}
                                    transition={{ duration: 1, ease: "easeOut" }}
                                    className="h-full bg-gradient-to-r from-yellow-50 to-yellow-100 rounded-full"
                                />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </motion.div>
    );
}