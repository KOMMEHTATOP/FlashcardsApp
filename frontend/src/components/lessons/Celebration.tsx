import { floatingIcons, recallRatingInfo } from "../../test/data";
import { motion } from "framer-motion";
import Card from "../ui/card";
import { PartyPopperIcon, Trophy, Zap } from "lucide-react";
import { Button } from "../ui/button";
import getMotivationText from "../../utils/getMotivationText";

interface CelebrationProps {
  subjectColor: string;
  earnedXP: number;
  handleComplete: () => void;
  total: number;
  values: number[];
}

export default function Celebration({
  subjectColor,
  earnedXP,
  handleComplete,
  total,
  values,
}: CelebrationProps) {
  return (
    <div className="min-h-screen bg-gradient-to-br from-base-300 via-base-200 to-base-300 flex items-center justify-center p-2 md:p-4">
      {floatingIcons.map((icon, index) => (
        <motion.div
          key={index}
          initial={{ opacity: 0, scale: 0 }}
          animate={{
            opacity: [0.3, 0.6, 0.3],
            scale: [1, 1.2, 1],
            y: [0, -20, 0],
          }}
          transition={{
            duration: 3,
            repeat: Infinity,
            delay: icon.delay,
          }}
          className="absolute"
          style={{ left: icon.x, top: icon.y }}
        >
          <icon.icon className={`w-12 h-12 ${icon.color}`} />
        </motion.div>
      ))}

      <motion.div
        initial={{ opacity: 0, scale: 0.8 }}
        animate={{ opacity: 1, scale: 1 }}
        className="max-w-2xl w-full text-center"
      >
        <Card
          className={`p-1 py-3 md:p-12 bg-gradient-to-br ${subjectColor} border-none shadow-2xl relative overflow-hidden`}
        >
          <div
            className="absolute inset-0 bg-white/10"
            style={{
              backgroundImage:
                "radial-gradient(circle at 2px 2px, rgba(255,255,255,0.15) 1px, transparent 0)",
              backgroundSize: "40px 40px",
            }}
          />

          <div className="relative z-10">
            <motion.div
              animate={{
                rotate: [0, 10, -10, 0],
                scale: [1, 1.1, 1],
              }}
              transition={{ duration: 0.5, repeat: 3 }}
            >
              <Trophy className="w-24 h-24 text-yellow-300 mx-auto mb-6" />
            </motion.div>

            <h2 className="text-4xl text-white mb-4 flex items-center justify-center gap-2">
              Урок Завершен!{" "}
              <PartyPopperIcon className="w-8 h-8 text-yellow-300 inline" />
            </h2>
            <p className="text-white/90 text-xl mb-8">
              {getMotivationText(total)}
            </p>

            <div className="bg-white/20 backdrop-blur-sm rounded-3xl p-8 mb-8">
              <div className="flex items-center justify-center gap-3 mb-4">
                <Zap className="w-8 h-8 text-yellow-300" />
                <motion.span
                  initial={{ scale: 0 }}
                  animate={{ scale: 1 }}
                  transition={{ delay: 0.3, type: "spring", stiffness: 200 }}
                  className="text-5xl text-white text-number"
                >
                  +{earnedXP} XP
                </motion.span>
              </div>
              <p className="text-white/80">Заработанные очки опыта</p>
            </div>

            <div className="grid grid-cols-3 gap-4 mb-8">
              {Object.keys(recallRatingInfo).map((_, index) => (
                <div key={index} className="bg-white/10 rounded-2xl p-4">
                  <div className="text-3xl text-white mb-1 text-number">
                    {values.filter((val) => val === index + 1).length || 0}
                  </div>
                  <div className="text-white/70 text-xs md:text-sm">
                    {recallRatingInfo[index + 1]}
                  </div>
                </div>
              ))}

              <div className="bg-white/10 rounded-2xl p-4">
                <div className="text-3xl text-white mb-1 text-number">
                  {total}%
                </div>
                <div className="text-white/70 text-sm">Общая оценка</div>
              </div>
            </div>

            <Button
              onClick={handleComplete}
              size="lg"
              className="bg-white text-gray-900 hover:bg-gray-100 shadow-lg text-lg px-8 py-6 border-0 items-center gap-2 inline-flex"
            >
              Продолжить
            </Button>
          </div>
        </Card>
      </motion.div>
    </div>
  );
}
