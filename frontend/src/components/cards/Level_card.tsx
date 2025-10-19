import { Star, Trophy } from "lucide-react";
import { motion, AnimatePresence } from "framer-motion";

type LevelCardProps = {
  level: number;
  currentXP: number;
  xpToNextLevel: number;
  xpForNextLevel: number;
  className?: string;
  isCollapsed?: boolean;
};

export default function LevelCard({
  level,
  currentXP,
  xpToNextLevel,
  xpForNextLevel,
  className,
  isCollapsed = false,
}: LevelCardProps) {
  const current = xpToNextLevel > 0 ? (currentXP / xpForNextLevel) * 100 : 0;

  return (
    <motion.div
      layout
      transition={{ layout: { duration: 0.4, ease: "easeInOut" } }}
      className={`bg-gradient-to-r from-purple-500 via-pink-500 to-orange-500  rounded-2xl shadow-lg ${className} ${
        isCollapsed ? "p-4" : "p-6"
      }`}
    >
      <AnimatePresence mode="wait">
        {!isCollapsed && (
          <motion.div
            key="expanded"
            layout
            initial={{ opacity: 0, height: 0 }}
            animate={{ opacity: 1, height: "auto" }}
            exit={{ opacity: 0, height: 0 }}
            transition={{ duration: 0.3, ease: "easeInOut" }}
          >
            <div className="flex items-center justify-between mb-4">
              <div className="flex items-center gap-3">
                <motion.div
                  animate={{ rotate: [0, -10, 10, -10, 0] }}
                  transition={{ duration: 2, repeat: Infinity, repeatDelay: 3 }}
                  className="bg-white/20 backdrop-blur-sm p-3 rounded-2xl"
                >
                  <Trophy className="text-white w-8 h-8" />
                </motion.div>
                <div className="text-white">
                  <h3 className="text-white/80">Уровень</h3>
                  <p className="text-3xl">{level}</p>
                </div>
              </div>

              <motion.div
                animate={{ scale: [1, 1.1, 1] }}
                transition={{ duration: 2, repeat: Infinity }}
                className="flex items-center gap-1 bg-white/20 backdrop-blur-sm px-4 py-2 rounded-full"
              >
                <Star className="w-5 h-5 text-yellow-300 fill-yellow-300" />
                <span className="text-white text-number">
                  {xpToNextLevel.toFixed(0)} XP
                </span>
              </motion.div>
            </div>
          </motion.div>
        )}
      </AnimatePresence>

      <motion.div layout className="space-y-2">
        <div className="flex justify-between text-white text-sm opacity-90">
          <span>Осталось до следующего уровня {level + 1}</span>
          <span className="text-number">
            {currentXP} / {xpForNextLevel}
          </span>
        </div>
        <div className="bg-white/20 rounded-full h-3 overflow-hidden">
          <motion.div
            initial={{ width: 0 }}
            animate={{ width: `${current ?? 0}%` }}
            transition={{ duration: 1, ease: "easeOut" }}
            className="h-full bg-gradient-to-r from-yellow-300 to-yellow-100 rounded-full"
          />
        </div>
      </motion.div>
    </motion.div>
  );
}
