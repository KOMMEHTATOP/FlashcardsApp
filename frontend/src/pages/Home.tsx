import {
    Award,
    Clock,
    Zap,
    Star,
    BookOpen,
    Medal,
    BookCopyIcon,
    Library
} from "lucide-react";
import { Helmet } from "react-helmet-async";
import { useMemo, useState, useRef } from "react";
import { AnimatePresence } from "framer-motion";

import LevelCard from "@/features/profile/ui/LevelСard";
import StateCard from "@/features/profile/ui/StateСard";
import MotivationCard from "@/features/dashboard/ui/MotivationCard";
import SettingModal from "@/shared/ui/modals/SettingModal";
import GroupForm from "@/features/groups/ui/GroupForm";
import { LessonsTab } from "@/features/dashboard/ui/LessonsTab";
import { StoreTab } from "@/features/dashboard/ui/StoreTab";
import { AchievementsTab } from "@/features/dashboard/ui/AchievementsTab";
import { LeaderboardWidget } from "@/features/leaderboard/ui/LeaderboardWidget";
import { useData } from "@/context/DataContext";
import formatTotalHour from "@/utils/formatTotalHour";

const modulePage = [
    { name: "Мои колоды" },
    { name: "Библиотека" },
    { name: "Достижения" }
];

export function HomePage() {
    const { user, achivment, groups, motivationText } = useData();
    const [modul] = useState<typeof modulePage>(modulePage);
    const tabsRef = useRef<HTMLDivElement>(null);

    const [currentModul, setCurrentModul] = useState<number>(() => {
        const saved = localStorage.getItem('activeTab');
        return saved ? parseInt(saved, 10) : 0;
    });

    const [isOpenSetting, setIsOpenSetting] = useState<boolean>(false);
    const [isCreateModalOpen, setCreateModalOpen] = useState<boolean>(false);

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
        localStorage.setItem('activeTab', index.toString());
    };

    const handleSwitchToStore = () => {
        setCurrentModul(1);
        localStorage.setItem('activeTab', '1');
        setTimeout(() => {
            tabsRef.current?.scrollIntoView({ behavior: "smooth", block: "start" });
        }, 100);
    };

    const handleOpenSetting = () => setIsOpenSetting(true);
    const handleCloseSetting = () => setIsOpenSetting(false);
    const handleCreateGroup = () => setCreateModalOpen(true);
    const handleCloseCreateModal = () => setCreateModalOpen(false);

    const renderCurrentTab = () => {
        switch (currentModul) {
            case 0:
                return (
                    <LessonsTab
                        groups={groups}
                        onOpenSettings={handleOpenSetting}
                        onCreateGroup={handleCreateGroup}
                        onSwitchToStore={handleSwitchToStore}
                    />
                );
            case 1:
                return <StoreTab />;
            case 2:
                return <AchievementsTab />;
            default:
                return null;
        }
    };

    return (
        <div className="w-full pb-10">
            <Helmet>
                <title>Моё обучение | FlashcardsLoop</title>
            </Helmet>
            <LeaderboardWidget />
            <div className="space-y-8">
                <div>
                    <LevelCard
                        level={level}
                        currentXP={currentXP}
                        xpToNextLevel={xpToNextLevel}
                        xpForNextLevel={xpForNextLevel}
                    />
                </div>

                <div className="grid grid-cols-2 sm:grid-cols-2 lg:grid-cols-4 gap-4">
                    <StateCard
                        icon={Clock}
                        label="Общее время учебы"
                        value={`${formatTotalHour(user?.Statistics?.TotalStudyTime || "0")} ч.`}
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
                        value={`${achivment?.filter((item) => item.IsUnlocked).length || 0}/${achivment?.length || 0}`}
                        gradient="from-green-500 to-emerald-500"
                        delay={0.3}
                    />
                    <StateCard
                        icon={BookCopyIcon}
                        label="Всего карточек"
                        value={totalCardCount.toString() || "0"}
                        gradient="from-purple-500 to-pink-500"
                        delay={0.4}
                    />
                </div>

                {/* Табы */}
                <div
                    ref={tabsRef}
                    role="tablist"
                    className="tabs tabs-border scroll-mt-24"
                >
                    {modulePage.map((item, index) => (
                        <button
                            key={item.name}
                            role="tab"
                            className={`tab gap-2 transition-all duration-300 ${
                                currentModul === index ? "tab-active bg-base-100 font-medium" : "opacity-60"
                            }`}
                            onClick={() => selectModul(item.name)}
                        >
                            {index === 0 && <BookOpen className="h-5 w-5" />}
                            {index === 1 && <Library className="h-5 w-5" />}
                            {index === 2 && <Medal className="h-5 w-5" />}
                            {item.name}
                        </button>
                    ))}
                </div>

                {isOpenSetting && (
                    <SettingModal handleCancel={handleCloseSetting} handleSave={() => { }} />
                )}
                {isCreateModalOpen && (
                    <GroupForm isOpen={isCreateModalOpen} handleCancle={handleCloseCreateModal} />
                )}

                <div className="space-y-6 mb-12">
                    <AnimatePresence mode="wait">
                        {renderCurrentTab()}
                    </AnimatePresence>
                </div>

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