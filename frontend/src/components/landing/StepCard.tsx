import { motion } from "framer-motion";
import { ArrowRight } from "lucide-react";

type Props = {
  number: string;
  title: string;
  description: string;
  gradient: string;
  delay?: number;
  showArrow?: boolean;
};

export default function StepCard({
  number,
  title,
  description,
  gradient,
  delay = 0,
  showArrow = false,
}: Props) {
  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      whileInView={{ opacity: 1, y: 0 }}
      viewport={{ once: true }}
      transition={{ delay }}
      className="relative"
    >
      <div
        className={`bg-gradient-to-br ${gradient} rounded-2xl p-6 shadow-lg h-full`}
      >
        <div className="text-6xl font-bold text-white/40 mb-4">{number}</div>
        <h3 className="text-xl font-bold text-white mb-3">{title}</h3>
        <p className="text-white/90">{description}</p>
        {showArrow && (
          <div className="hidden lg:block absolute top-1/2 -right-5 transform -translate-y-1/2">
            <ArrowRight className="w-8 h-8 text-base-content" />
          </div>
        )}
      </div>
    </motion.div>
  );
}
