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

import LevelCard from "../components/cards/Level_card";
import StateCard from "../components/cards/State_card";
import MotivationCard from "../components/cards/Motivation_card";
import SettingModal from "../components/modal/SettingModal";
import GroupForm from "../components/modal/GroupForm";

import { LessonsTab } from "../components/tabs/LessonsTab";
import { StoreTab } from "../components/tabs/StoreTab";
import { AchievementsTab } from "../components/tabs/AchievementsTab";

import { useData } from "../context/DataContext";
import formatTotalHour from "../utils/formatTotalHour";

const modulePage = [
    { name: "Мои колоды" },
    { name: "Библиотека" },
    { name: "Достижения" }
];

export function HomePage() {
    const { user, achivment, groups, motivationText } = useData();
    const [modul] = useState<typeof modulePage>(modulePage);

    // Якорь для скролла к табам
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

    // --- Handlers ---

    // Функция для переключения табов при клике на меню
    const selectModul = (name: string) => {
        const index = modul.findIndex((item) => item.name === name);
        setCurrentModul(index);
        localStorage.setItem('activeTab', index.toString());
    };

    // Функция переключения на библиотеку (вызывается из LessonsTab)
    const handleSwitchToStore = () => {
        setCurrentModul(1); // 1 = Библиотека
        localStorage.setItem('activeTab', '1');

        // Скроллим экран к началу табов (где поиск), с небольшим отступом
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

            <div className="space-y-8">
                {/* Блок статистики (Level, Cards...) */}
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

                {/* Табы (Навигация) - СЮДА ДОБАВИЛИ REF */}
                <div
                    ref={tabsRef} // <--- Якорь для скролла
                    role="tablist"
                    className="tabs tabs-border scroll-mt-24" // scroll-mt делает отступ сверху при скролле
                >
                    <button
                        role="tab"
                        className={`tab gap-2 transition-all duration-300 ${
                            currentModul === 0 ? "tab-active bg-base-100 font-medium" : "opacity-60"
                        }`}
                        onClick={() => selectModul("Мои колоды")}
                    >
                        <BookOpen className="h-5 w-5" />
                        Мои колоды
                    </button>
                    <button
                        role="tab"
                        className={`tab gap-2 transition-all duration-300 ${
                            currentModul === 1 ? "tab-active bg-base-100 font-medium" : "opacity-60"
                        }`}
                        onClick={() => selectModul("Библиотека")}
                    >
                        <Library className="h-5 w-5" />
                        Библиотека
                    </button>
                    <button
                        role="tab"
                        className={`tab gap-2 transition-all duration-300 ${
                            currentModul === 2 ? "tab-active bg-base-100 font-medium" : "opacity-60"
                        }`}
                        onClick={() => selectModul("Достижения")}
                    >
                        <Medal className="h-5 w-5" />
                        Достижения
                    </button>
                </div>

                {/* Модалки */}
                {isOpenSetting && (
                    <SettingModal
                        handleCancel={handleCloseSetting}
                        handleSave={() => {}}
                    />
                )}

                {isCreateModalOpen && (
                    <GroupForm
                        isOpen={isCreateModalOpen}
                        handleCancle={handleCloseCreateModal}
                    />
                )}

                {/* Контент вкладок */}
                <div className="space-y-6 mb-12">
                    <AnimatePresence mode="wait">
                        {renderCurrentTab()}
                    </AnimatePresence>
                </div>

                {/* Мотивация */}
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