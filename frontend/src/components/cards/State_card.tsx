import { motion } from "framer-motion";
import type { LucideIcon } from "lucide-react";

interface StateCardProps {
  icon: LucideIcon;
  label: string;
  value: string;
  gradient: string;
  delay?: number;
}

export default function StateCard({
  icon: Icon,
  label,
  value,
  gradient,
  delay = 0,
}: StateCardProps) {
  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.5, delay: delay }}
      className={`bg-gradient-to-br ${gradient} p-6 rounded-2xl shadow-xl relative overflow-hidden`}
    >
      <div className="absolute top-0 right-0 w-24 h-24 bg-white/10 rounded-full -mr-12 -mt-12" />
      <div className="relative z-10">
        <Icon className="w-8 h-8 text-white mb-3" />
        <p className="text-white/80 text-sm mb-1">{label}</p>
        <p className="text-3xl text-white text-number">{value}</p>
      </div>
    </motion.div>
  );
}
