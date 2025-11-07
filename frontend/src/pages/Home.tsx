import {
  Award,
  Clock,
  Zap,
  Star,
  BookOpen,
  Medal,
  BookCopyIcon,
  Settings2Icon,
} from "lucide-react";
import LevelCard from "../components/cards/Level_card";
import StateCard from "../components/cards/State_card";
import { useApp } from "../context/AppContext";
import { motion, AnimatePresence } from "framer-motion";
import MotivationCard from "../components/cards/Motivation_card";
import { useMemo, useState } from "react";
import { BadgeCard } from "../components/cards/Badge_card";
import SortableList from "../components/SortebleList";
import useTitle from "../utils/useTitle";
import formatTotalHour from "../utils/formatTotalHour";
import SettingModal from "../components/modal/SettingModal";

const modulePage = [
  {
    name: "Уроки",
  },
  {
    name: "Достижения",
  },
];

export function HomePage() {
  const { user, achivment, groups, motivationText } = useApp();
  useTitle("Главная");

  const [modul] = useState<typeof modulePage>(modulePage);
  const [currentModul, setCurrentModul] = useState<number>(0);
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
  };

  const handleOpenSetting = () => setIsOpenSetting(true);
  const handleCloseSetting = () => setIsOpenSetting(false);

  return (
    <div className="w-full pb-10">
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
            label="Общое количество карточек"
            value={totalCardCount.toString() || "0"}
            gradient="from-purple-500 to-pink-500"
            delay={0.4}
          />
        </div>
        <div role="tablist" className="tabs tabs-border">
          <button
            role="tab"
            className={`tab gap-2 transition-all duration-300 ${
              currentModul === 0 ? "tab-active bg-base-100" : "opacity-50"
            }`}
            onClick={() => selectModul("Уроки")}
          >
            <BookOpen className="h-5 w-5 text-base-content" />
            Уроки
          </button>
          <button
            role="tab"
            className={`tab gap-2 transition-all duration-300 ${
              currentModul === 1 ? "tab-active bg-base-100" : "opacity-50"
            }`}
            onClick={() => selectModul("Достижения")}
          >
            <Medal className="h-5 w-5 text-base-content" />
            Достижения
          </button>
        </div>
        {isOpenSetting && (
          <SettingModal
            handleCancel={handleCloseSetting}
            handleSave={() => {}}
          />
        )}
        <div className="space-y-6 mb-12 ">
          <AnimatePresence mode="wait">
            {currentModul === 0 ? (
              <motion.div
                key="lessons"
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0, y: -20 }}
                transition={{ duration: 0.4 }}
                className="space-y-6"
              >
                <div className="flex items-center justify-between">
                  <h2 className="text-2xl text-base-content">Ваши карточки</h2>
                  <div
                    onClick={handleOpenSetting}
                    className="flex items-center gap-2 hover:scale-105 duration-300 transition-all cursor-pointer group opacity-70 hover:opacity-100"
                  >
                    <span className="group-hover:opacity-90 opacity-0 duration-500 transition-opacity ">
                      Настройки
                    </span>
                    <Settings2Icon className="w-8 h-8 z-10" />
                  </div>
                </div>

                <SortableList initalItems={groups || []} />
              </motion.div>
            ) : (
              <motion.div
                key="badges"
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0, y: -20 }}
                transition={{ duration: 0.4 }}
                className="space-y-6"
              >
                <div className="grid grid-cols-2 sm:grid-cols-2 lg:grid-cols-3 gap-4">
                  {achivment?.map((item, index) => (
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
            )}
          </AnimatePresence>
        </div>
        <MotivationCard
          animated="rotate"
          animatedDelay={20}
          icon={Star}
          // textIcon={}
          label="Продолжай идти!"
          description={` ${motivationText?.Message || ""} ${
            motivationText?.Icon || ""
          }`}
          gradient="from-indigo-500 via-purple-500 to-pink-500"
        />
      </div>
    </div>
  );
}
