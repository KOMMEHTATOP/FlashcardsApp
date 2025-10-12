import type { LucideIcon } from "lucide-react";
import { motion } from "framer-motion";

interface MotivationCardProps {
  icon: LucideIcon;
  label: string;
  description: string;
  gradient: string;
  delay?: number;
  animated?: keyof typeof animType;
  animatedDelay?: number;
  className?: string;
  textIcon?: LucideIcon;
}

const animType = {
  rotate: {
    rotate: [0, 360], // вращение
  },
  scale: {
    scale: [1, 1.2, 1], // пульсация
  },
  bounce: {
    y: [0, -10, 0], // подпрыгивание
  },
  float: {
    y: [0, -5, 0], // лёгкое плавание
    transition: { duration: 3, repeat: Infinity, ease: "easeInOut" },
  },
};

export default function MotivationCard({
  icon: Icon,
  label,
  description,
  gradient,
  delay = 0.6,
  animated,
  animatedDelay,
  className,
  textIcon: TextIcon,
}: MotivationCardProps) {
  return (
    <motion.div
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      transition={{ delay: delay }}
      className={`bg-gradient-to-r ${gradient} p-8 rounded-3xl shadow-xl text-center relative overflow-hidden ${className}`}
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
          animate={animated ? animType[animated] : {}}
          transition={{
            duration: animatedDelay,
            repeat: Infinity,
            ease: "linear",
          }}
          className="inline-block mb-4"
        >
          <Icon className="w-16 h-16 text-yellow-300 " />
        </motion.div>
        <h3 className="text-3xl text-white mb-2">{label}</h3>
        <p className="text-white/90 text-lg items-center flex justify-center gap-2">
          {description}{" "}
          {TextIcon && (
            <TextIcon className="w-6 h-6 inline-block text-orange-300" />
          )}
        </p>
      </div>
    </motion.div>
  );
}
