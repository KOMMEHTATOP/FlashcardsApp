import { motion } from "framer-motion";
import {
    GalleryVerticalEndIcon,
    Users,
    Play,
    type LucideIcon,
} from "lucide-react";
import type { GroupType } from "@/types/types";

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
    onStart?: () => void;
    hasCards?: boolean;
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
                                onStart,
                                hasCards = false
                            }: GroupHeaderProps) {

    const showStartButton = hasCards && (!isSubscriptionView || isSubscribed);

    return (
        <motion.div
            initial={{ opacity: 0, y: -20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.5 }}
            className={`relative bg-gradient-to-br ${group.GroupColor || "from-blue-500 to-blue-700"
            } w-full px-4 sm:px-6 lg:px-8 pt-8 pb-6 overflow-hidden rounded-2xl shadow-xl flex flex-col items-center`}
        >
            <div
                className="absolute inset-0 bg-white/10"
                style={{
                    backgroundImage:
                        "radial-gradient(circle at 2px 2px, rgba(255,255,255,0.15) 1px, transparent 0)",
                    backgroundSize: "40px 40px",
                }}
            />

            <div className="relative z-10 w-full">
                <div className="flex flex-col sm:flex-row items-center justify-center sm:justify-start gap-6 mb-0 w-full">

                    <motion.div
                        initial={{ scale: 0 }}
                        animate={{ scale: 1 }}
                        transition={{ type: "spring", stiffness: 200 }}
                        className="bg-white/20 backdrop-blur-sm p-4 rounded-3xl flex-shrink-0"
                    >
                        <Icon className="w-16 h-16 md:w-16 md:h-16 text-white" />
                    </motion.div>

                    <div className="flex-1 text-center sm:text-left w-full">

                        <div className="flex flex-col md:flex-row items-center md:items-start justify-between gap-4 mb-4">
                            <div className="flex flex-col items-center md:items-start">
                                <h1 className="text-3xl text-white font-bold mb-2">{group.GroupName}</h1>

                                <div className="flex flex-wrap justify-center md:justify-start gap-2">
                                    {Number(group.CardCount) > 0 && (
                                        <div className="bg-white/20 text-white px-3 py-1 rounded-full flex items-center gap-1 text-sm backdrop-blur-md">
                                            <GalleryVerticalEndIcon className="w-4 h-4" />
                                            <span>{group.CardCount} карточек</span>
                                        </div>
                                    )}
                                    {group.IsPublished && (group.SubscriberCount || 0) > 0 && (
                                        <div className="bg-white/20 text-white px-3 py-1 rounded-full flex items-center gap-1 text-sm backdrop-blur-md">
                                            <Users className="w-4 h-4" />
                                            <span>{group.SubscriberCount} подп.</span>
                                        </div>
                                    )}
                                </div>
                            </div>

                            <div className="flex flex-col items-center md:items-end">
                                {isSubscriptionView ? (
                                    <label className={`
                                        flex items-center gap-2 cursor-pointer bg-black/20 px-3 py-2 rounded-lg hover:bg-black/30 transition
                                        ${submittingSubscription ? 'opacity-50 cursor-wait' : ''}
                                    `}>
                                        <span className="text-white text-sm font-medium">
                                            {isSubscribed ? "Вы подписаны" : "Подписаться"}
                                        </span>
                                        <input
                                            type="checkbox"
                                            checked={isSubscribed}
                                            onChange={onToggleSubscription}
                                            disabled={submittingSubscription}
                                            className="checkbox checkbox-sm checkbox-success bg-white border-white/50"
                                        />
                                    </label>
                                ) : (
                                    <div className="flex flex-col items-end gap-1">
                                        <label className={`
                                            flex items-center gap-2 cursor-pointer bg-black/20 px-3 py-2 rounded-lg hover:bg-black/30 transition
                                            ${!canPublish ? 'opacity-50 cursor-not-allowed' : ''}
                                            ${isPublishing ? 'opacity-50 cursor-wait' : ''}
                                        `}>
                                            <span className="text-white text-sm font-medium">
                                                {group.IsPublished ? "Опубликовано" : "Публичная"}
                                            </span>
                                            <input
                                                type="checkbox"
                                                checked={group.IsPublished || false}
                                                onChange={onTogglePublish}
                                                disabled={isPublishing || !canPublish}
                                                className="checkbox checkbox-sm checkbox-success bg-white border-white/50"
                                            />
                                        </label>
                                        {!canPublish && (
                                            <span className="text-[10px] text-white/70">Мин. 10 карточек</span>
                                        )}
                                    </div>
                                )}
                            </div>
                        </div>

                        <div className="mt-6 flex flex-col md:flex-row items-center gap-4">

                            {showStartButton && (
                                <motion.button
                                    whileHover={{ scale: 1.05 }}
                                    whileTap={{ scale: 0.95 }}
                                    onClick={onStart}
                                    className="btn border-none bg-white text-gray-900 hover:bg-gray-100 px-6 rounded-full shadow-lg gap-2 flex items-center group w-full md:w-auto"
                                >
                                    <Play className="w-5 h-5 fill-current group-hover:text-primary transition-colors" />
                                    <span className="font-bold">Начать обучение</span>
                                </motion.button>
                            )}

                            <div className="flex-1 w-full min-w-[200px]">
                                <div className="flex justify-between text-white/80 text-xs mb-1">
                                    <span>Прогресс изучения</span>
                                    <span className="font-mono">{progress ? Number(progress).toFixed(0) : 0}%</span>
                                </div>
                                <div className="relative w-full h-2 bg-black/20 rounded-full overflow-hidden">
                                    <motion.div
                                        initial={{ width: 0 }}
                                        animate={{ width: `${progress || 0}%` }}
                                        transition={{ duration: 1, ease: "easeOut" }}
                                        className="h-full bg-white rounded-full"
                                    />
                                </div>
                            </div>
                        </div>

                        {publishError && !isSubscriptionView && (
                            <div className="mt-4 bg-red-500/80 text-white px-3 py-2 rounded-lg text-sm text-center md:text-left">
                                {publishError}
                            </div>
                        )}

                    </div>
                </div>
            </div>
        </motion.div>
    );
}