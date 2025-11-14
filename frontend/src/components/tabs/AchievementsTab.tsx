import { motion } from "framer-motion";
import { BadgeCard } from "../../shared/ui/BadgeCard";
import type { AchievementsType } from "../../types/types";

interface AchievementsTabProps {
    achievements: AchievementsType[] | undefined;
}

export function AchievementsTab({ achievements }: AchievementsTabProps) {
    return (
        <motion.div
            key="achievements"
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -20 }}
            transition={{ duration: 0.4 }}
            className="space-y-6"
        >
            <h2 className="text-2xl text-base-content">Достижения</h2>
            <div className="grid grid-cols-2 sm:grid-cols-2 lg:grid-cols-3 gap-4">
                {achievements?.map((item, index) => (
                    <motion.div
                        key={item.Id}
                        initial={{ opacity: 0, y: 20 }}
                        animate={{ opacity: 1, y: 0 }}
                        transition={{ delay: index * 0.1 }}
                    >
                        <BadgeCard
                            title={item.Name}
                            description={item.Description}
                            earned={item.IsUnlocked}
                            gradient={item.Gradient}
                            icon={item.IconUrl}
                        />
                    </motion.div>
                ))}
            </div>
        </motion.div>
    );
}