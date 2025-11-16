import {
    Award,
    Clock,
    Zap,
    Star,
    BookOpen,
    Medal,
    BookCopyIcon,
    Store
} from "lucide-react";

import LevelCard from "../components/cards/Level_card";
import StateCard from "../components/cards/State_card";
import { useData } from "../context/DataContext";
import { AnimatePresence } from "framer-motion";
import MotivationCard from "../components/cards/Motivation_card";
import { useMemo, useState } from "react";
import useTitle from "../utils/useTitle";
import formatTotalHour from "../utils/formatTotalHour";
import SettingModal from "../components/modal/SettingModal";

import { LessonsTab } from "../components/tabs/LessonsTab";
import { StoreTab } from "../components/tabs/StoreTab";
import { AchievementsTab } from "../components/tabs/AchievementsTab";

const modulePage = [
    { name: "Мои группы" },
    { name: "Общие группы" },
    { name: "Достижения" }
];

export function HomePage() {
    const { user, achivment, groups, motivationText } = useData();
    useTitle("Главная");

    const [modul] = useState<typeof modulePage>(modulePage);

    // Восстанавливаем активную вкладку из localStorage при загрузке
    const [currentModul, setCurrentModul] = useState<number>(() => {
        const saved = localStorage.getItem('activeTab');
        return saved ? parseInt(saved, 10) : 0;
    });

    const [isOpenSetting, setIsOpenSetting] = useState<boolean>(false);

    const currentXP = user?.Statistics?.XPProgressInCurrentLevel ?? 0;
    const xpForNextLevel = user?.Statistics?.XPRequiredForCurrentLevel ?? 0;
    const xpToNextLevel = Math.max(0, xpForNextLevel - currentXP);

    const level = user?.Statistics?.Level || 0;

    const totalCardCount = useMemo(
        () => groups?.reduce((sum, g) => sum + (g.CardCount || 0), 0) ?? 0,
        [groups]
    );

    const selectModul = (name: string) => {
        const index = modul.findIndex((item) => item.name === name);
        setCurrentModul(index);
        // Сохраняем активную вкладку в localStorage
        localStorage.setItem('activeTab', index.toString());
    };

    const handleOpenSetting = () => setIsOpenSetting(true);
    const handleCloseSetting = () => setIsOpenSetting(false);

    // Функция для рендера текущей вкладки
    const renderCurrentTab = () => {
        switch (currentModul) {
            case 0:
                return (
                    <LessonsTab
                        groups={groups}
                        onOpenSettings={handleOpenSetting}
                    />
                );
            case 1:
                return <StoreTab />;
            case 2:
                return <AchievementsTab achievements={achivment} />;
            default:
                return null;
        }
    };

    return (
        <div className="w-full pb-10">
            <div className="space-y-8">
                {/* Карточка уровня */}
                <div>
                    <LevelCard
                        level={level}
                        currentXP={currentXP}
                        xpToNextLevel={xpToNextLevel}
                        xpForNextLevel={xpForNextLevel}
                    />
                </div>

                {/* Статистика */}
                <div className="grid grid-cols-2 sm:grid-cols-2 lg:grid-cols-4 gap-4">
                    <StateCard
                        icon={Clock}
                        label="Общее время учебы"
                        value={`${formatTotalHour(
                            user?.Statistics?.TotalStudyTime || "0"
                        )} ч.`}
                        gradient="from-blue-500 to-cyan-500"
                        delay={0.1}
                    />
                    <StateCard
                        icon={Zap}
                        label="Ударный режим"
                        value={`${user?.Statistics.CurrentStreak || 0} дней`}
                        gradient="from-orange-500 to-red-500"
                        delay={0.2}
                    />
                    <StateCard
                        icon={Award}
                        label="Достижения"
                        value={
                            `${achivment?.filter((item) => item.IsUnlocked).length}/${
                                achivment?.length
                            }` || "0/0"
                        }
                        gradient="from-green-500 to-emerald-500"
                        delay={0.3}
                    />
                    <StateCard
                        icon={BookCopyIcon}
                        label="Общее количество карточек"
                        value={totalCardCount.toString() || "0"}
                        gradient="from-purple-500 to-pink-500"
                        delay={0.4}
                    />
                </div>

                {/* Табы */}
                <div role="tablist" className="tabs tabs-border">
                    <button
                        role="tab"
                        className={`tab gap-2 transition-all duration-300 ${
                            currentModul === 0 ? "tab-active bg-base-100" : "opacity-50"
                        }`}
                        onClick={() => selectModul("Мои группы")}
                    >
                        <BookOpen className="h-5 w-5 text-base-content" />
                        Мои группы
                    </button>
                    <button
                        role="tab"
                        className={`tab gap-2 transition-all duration-300 ${
                            currentModul === 1 ? "tab-active bg-base-100" : "opacity-50"
                        }`}
                        onClick={() => selectModul("Общие группы")}
                    >
                        <Store className="h-5 w-5 text-base-content" />
                        Общие группы
                    </button>
                    <button
                        role="tab"
                        className={`tab gap-2 transition-all duration-300 ${
                            currentModul === 2 ? "tab-active bg-base-100" : "opacity-50"
                        }`}
                        onClick={() => selectModul("Достижения")}
                    >
                        <Medal className="h-5 w-5 text-base-content" />
                        Достижения
                    </button>
                </div>

                {/* Модальное окно настроек */}
                {isOpenSetting && (
                    <SettingModal
                        handleCancel={handleCloseSetting}
                        handleSave={() => {}}
                    />
                )}

                {/* Контент вкладок */}
                <div className="space-y-6 mb-12">
                    <AnimatePresence mode="wait">
                        {renderCurrentTab()}
                    </AnimatePresence>
                </div>

                {/* Мотивационная карточка */}
                <MotivationCard
                    animated="rotate"
                    animatedDelay={20}
                    icon={Star}
                    label="Продолжай идти!"
                    description={`${motivationText?.Message || ""} ${motivationText?.Icon || ""}`}
                    gradient="from-indigo-500 via-purple-500 to-pink-500"
                />
            </div>
        </div>
    );
}