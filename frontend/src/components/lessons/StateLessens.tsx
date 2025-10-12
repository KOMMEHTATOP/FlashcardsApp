import { motion } from "framer-motion";
import Card from "../ui/card";
import { recallRatingInfo } from "../../test/data";

interface GradientState {
  [key: number]: string;
}

const gradinetState: GradientState = {
  1: "from-red-400 to-rose-500",
  2: "from-purple-400 to-indigo-500",
  3: "from-gray-400 to-gray-500",
  4: "from-green-400 to-emerald-500",
  5: "from-yellow-400 to-orange-500",
};

export default function StateLessens({
  values,
  total,
}: {
  values: number[];
  total?: number;
}) {
  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ delay: 0.3 }}
      className="grid grid-cols-2 sm:grid-cols-5 gap-4 mt-14"
    >
      {Object.keys(recallRatingInfo).map((_, index) => (
        <motion.div
          key={index}
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5, delay: index * 0.1 }}
        >
          <Card
            className={`p-4 text-center bg-gradient-to-br border-none shadow-lg ${
              gradinetState[index + 1]
            }`}
          >
            <div className="text-xl text-white mb-1 text-number">
              {values.filter((val) => val === index + 1).length || 0}
            </div>
            <div className="text-white/80 text-sm">
              {recallRatingInfo[index + 1]}
            </div>
          </Card>
        </motion.div>
      ))}
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5, delay: 0.5 }}
      >
        <Card className="p-4 text-center bg-gradient-to-br from-purple-400 to-pink-500 border-none shadow-lg">
          <div className="text-2xl text-white mb-1 text-number">{total}%</div>
          <div className="text-white/80 text-sm">Общая оценка</div>
        </Card>
      </motion.div>
    </motion.div>
  );
}
