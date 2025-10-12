import { motion } from "framer-motion";
import Card from "../ui/card";
import { LightbulbIcon, RotateCcwIcon, Sparkles } from "lucide-react";

interface MainCardLessensProps {
  currentCardIndex: number;
  currentCard: any;
  isNext: boolean;
  isFlipped: boolean;
  subjectColor: string;
  handleFlip: () => void;
}

export default function MainCardLessens({
  currentCardIndex,
  currentCard,
  isNext,
  isFlipped,
  subjectColor,
  handleFlip,
}: MainCardLessensProps) {
  return (
    <motion.div
      key={currentCardIndex}
      initial={{
        opacity: 0,
        x: isNext ? 100 : -100,
      }}
      animate={{ opacity: 1, x: 0 }}
      exit={{
        opacity: 0,
        x: isNext ? -100 : 100,
      }}
      transition={{ duration: 0.3 }}
      className="mb-8"
    >
      <motion.div
        className="relative cursor-pointer"
        style={{ perspective: 1000 }}
      >
        <motion.div
          animate={{ rotateY: isFlipped ? 180 : 0 }}
          transition={{ duration: 0.6, type: "spring", stiffness: 100 }}
          style={{
            transformStyle: "preserve-3d",
            position: "relative",
          }}
        >
          {/* перед карточки */}
          <Card
            className={`min-h-[400px] p-12 cursor-pointer bg-gradient-to-br ${subjectColor} border-none shadow-lg relative overflow-hidden hover:shadow-white/20  transition duration-500 ease-in-out`}
            onClick={handleFlip}
          >
            <div
              className="absolute inset-0 bg-white/10"
              style={{
                backgroundImage:
                  "radial-gradient(circle at 2px 2px, rgba(255,255,255,0.15) 1px, transparent 0)",
                backgroundSize: "40px 40px",
              }}
            />

            <div className="relative z-10 h-full flex flex-col items-center justify-center text-center">
              {!isFlipped ? (
                <>
                  <motion.div
                    initial={{ scale: 0 }}
                    animate={{ scale: 1 }}
                    className="mb-6"
                  >
                    <div className="bg-white/20 backdrop-blur-sm p-4 rounded-full">
                      <Sparkles className="w-12 h-12 text-white" />
                    </div>
                  </motion.div>
                  <h3 className="text-3xl text-white mb-6">
                    {currentCard.question}
                  </h3>
                  {currentCard.hint && (
                    <div className="bg-white/20 backdrop-blur-sm px-6 py-3 rounded-full">
                      <p className="text-white/90 flex items-center gap-1">
                        <LightbulbIcon className="w-6 h-6 text-white" />{" "}
                        Подсказка: {currentCard.hint}
                      </p>
                    </div>
                  )}
                  <motion.div
                    animate={{ y: [0, -10, 0] }}
                    transition={{ duration: 2, repeat: Infinity }}
                    className="mt-8"
                  >
                    <RotateCcwIcon className="w-6 h-6 text-white/60" />
                  </motion.div>
                  <p className="text-white/60 text-sm mt-2">
                    Нажмите, чтобы открыть ответ
                  </p>
                </>
              ) : (
                <div
                  style={{
                    transform: "rotateY(180deg)",
                  }}
                >
                  <motion.div
                    initial={{ scale: 0 }}
                    animate={{ scale: 1 }}
                    className="mb-6 flex justify-center"
                  >
                    {/* <div className="bg-white/20 backdrop-blur-sm px-0 py-4 rounded-full items-center flex justify-center">
                            <Star className="w-12 h-12 text-white" />
                          </div> */}
                    <div className="bg-white/60 backdrop-blur-sm w-16 h-2 rounded-full"></div>
                  </motion.div>
                  <h3 className="text-4xl text-white mb-6">
                    {currentCard.answer}
                  </h3>
                </div>
              )}
            </div>
          </Card>
        </motion.div>
      </motion.div>
    </motion.div>
  );
}
