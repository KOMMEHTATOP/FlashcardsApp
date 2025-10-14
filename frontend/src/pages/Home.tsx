import {
  Award,
  Clock,
  Zap,
  TrendingUp,
  Star,
  BookOpen,
  Medal,
  PlusSquareIcon,
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
import { getLevelMotivationText } from "../utils/getMotivationText";
import formatTotalHour from "../utils/formatTotalHour";

const modulePage = [
  {
    name: "Уроки",
  },
  {
    name: "Достижения",
  },
];

export function HomePage() {
  const { userState, achivment, userAchivment, groups } = useApp();
  useTitle("Главная");

  const [modul, setModul] = useState<typeof modulePage>(modulePage);
  const [currentModul, setCurrentModul] = useState<number>(0);

  const currentXP = userState?.XPProgressInCurrentLevel || 0;
  const xpToNextLevel = userState?.XPRequiredForCurrentLevel || 0;
  const level = userState?.Level || 0;

  const motivationText = useMemo(() => {
    return getLevelMotivationText(currentXP, xpToNextLevel);
  }, [level, currentXP, xpToNextLevel]);
  const xpLeft = xpToNextLevel - currentXP;

  const selectModul = (name: string) => {
    const index = modul.findIndex((item) => item.name === name);
    setCurrentModul(index);
  };

  return (
    <div className="w-full pb-10">
      <div className="space-y-8">
        <div>
          <LevelCard
            level={level}
            currentXP={currentXP}
            xpToNextLevel={xpToNextLevel}
          />
        </div>
        <div className="grid grid-cols-2 sm:grid-cols-2 lg:grid-cols-4 gap-4">
          <StateCard
            icon={Clock}
            label="Общее время учебы"
            value={`${formatTotalHour(userState?.TotalStudyTime || "0")} ч.`}
            gradient="from-blue-500 to-cyan-500"
            delay={0.1}
          />
          <StateCard
            icon={Zap}
            label="Ударный режим"
            value={`${userState?.CurrentStreak || 0} дней`}
            gradient="from-orange-500 to-red-500"
            delay={0.2}
          />
          <StateCard
            icon={Award}
            label="Достижения"
            value="3/6"
            gradient="from-green-500 to-emerald-500"
            delay={0.3}
          />
          <StateCard
            icon={TrendingUp}
            label="Статистика"
            value="+25%"
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
        <div className="space-y-6 mb-12">
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
                  <div className="flex items-center gap-2 hover:scale-110 duration-300 transition-all cursor-pointer group ">
                    <span className="text-success group-hover:opacity-90 opacity-0 duration-500 transition-opacity ">
                      Добавить
                    </span>
                    <PlusSquareIcon className="w-8 h-8 text-success z-10" />
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
                        earned={userAchivment?.some((x) => x.Id === item.Id)}
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
          // textIcon={Rocket}
          label="Продолжай идти!"
          description={`${motivationText}  
        Осталось ${xpLeft} XP до ${level + 1}-го уровня.`}
          gradient="from-indigo-500 via-purple-500 to-pink-500"
        />
      </div>
    </div>
  );
}
