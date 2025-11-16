import { motion } from "framer-motion";
import { useEffect, useState } from "react";
import { BadgeCard } from "../../shared/ui/BadgeCard";
import type { AchievementsType } from "../../types/types";
import apiFetch from "../../utils/apiFetch";
import { Award } from "lucide-react";

export function AchievementsTab() {
    const [achievements, setAchievements] = useState<AchievementsType[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const controller = new AbortController();

        const loadAchievements = async () => {
            setLoading(true);
            setError(null);

            try {
                const response = await apiFetch.get('/Achievements/all-with-status', {
                    signal: controller.signal
                });

                if (!controller.signal.aborted) {
                    setAchievements(response.data);
                }
            } catch (err: any) {
                if (err.name === 'AbortError' || err.name === 'CanceledError') {
                    return;
                }
                console.error("Ошибка загрузки достижений:", err);
                setError("Не удалось загрузить достижения");
            } finally {
                if (!controller.signal.aborted) {
                    setLoading(false);
                }
            }
        };

        loadAchievements();

        return () => {
            controller.abort();
        };
    }, []);

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

            {loading && (
                <div className="flex justify-center py-12">
                    <span className="loading loading-spinner loading-lg"></span>
                </div>
            )}

            {error && (
                <div className="alert alert-error">
                    <span>{error}</span>
                </div>
            )}

            {!loading && !error && achievements.length === 0 && (
                <div className="text-center py-12">
                    <Award className="w-16 h-16 mx-auto mb-4 opacity-30" />
                    <p className="text-base-content/70">Достижения не найдены</p>
                </div>
            )}

            {!loading && !error && achievements.length > 0 && (
                <div className="grid grid-cols-2 sm:grid-cols-2 lg:grid-cols-3 gap-4">
                    {achievements.map((item, index) => (
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
            )}
        </motion.div>
    );
}