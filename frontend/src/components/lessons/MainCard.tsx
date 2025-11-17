import { motion } from "framer-motion";
import { Card } from "../../shared/ui/Card";
import type { GroupCardType } from "../../types/types";

interface MainCardLessensProps {
  currentCardIndex: number;
  currentCard: GroupCardType;
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
            <Card
                className={`min-h-[400px] p-12 cursor-pointer bg-gradient-to-br ${subjectColor} border-none shadow-lg relative overflow-hidden hover:shadow-white/20 transition duration-500 ease-in-out flex`}
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

              <div className="relative z-10 flex-1 flex flex-col items-center justify-center text-center">
                {!isFlipped ? (
                    <>
                      <div
                          className="text-3xl text-white mb-6 prose prose-invert max-w-none [&>p]:m-0"
                          dangerouslySetInnerHTML={{ __html: currentCard?.Question || '' }}
                      />
                    </>
                ) : (
                    <div
                        style={{
                          transform: "rotateY(180deg)",
                        }}
                        className="flex-1 flex flex-col items-center justify-center"
                    >
                      <div
                          className="text-4xl text-white prose prose-invert max-w-none [&>p]:m-0"
                          dangerouslySetInnerHTML={{ __html: currentCard.Answer }}
                      />
                    </div>
                )}
              </div>
            </Card>
          </motion.div>
        </motion.div>
      </motion.div>
  );
}