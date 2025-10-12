import { ArrowLeft, Zap } from "lucide-react";
import { motion } from "framer-motion";

interface Props {
  lessonTitle: string;
  onBack: () => void;
  earnedXP: number;
  progress: number;
  from: number;
  to: number;
}

export default function HeaderLessons({
  lessonTitle,
  onBack,
  earnedXP,
  progress,
  from,
  to,
}: Props) {
  return (
    <div className="sticky top-0 z-40 backdrop-blur-xl bg-base-100 border-b border-base-100 ">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
        <div className="flex items-center justify-between mb-4">
          <button onClick={onBack} className="btn btn-ghost text-title">
            <ArrowLeft className="w-4 h-4 mr-2" />
            Назад
          </button>

          <div className="flex items-center gap-2">
            <Zap className="w-5 h-5 text-yellow-500" />
            <span className="text-base-content/80">{earnedXP} XP</span>
          </div>
        </div>

        <div className="space-y-2">
          <div className="flex items-center justify-between text-lg">
            <span className="text-base-content/60">{lessonTitle}</span>
            <span className="text-base-content/60">
              {from} / {to}
            </span>
          </div>
          <div className="h-2 w-full bg-gray-200/30 rounded-full">
            <motion.div
              initial={{ width: "0%" }}
              animate={{ width: `${progress}%` }}
              transition={{ duration: 0.3 }}
              className="w-full h-2 bg-gray-200 rounded-full"
            />
          </div>
        </div>
      </div>
    </div>
  );
}
