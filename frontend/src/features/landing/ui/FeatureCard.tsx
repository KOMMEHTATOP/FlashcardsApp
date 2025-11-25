import { motion } from "framer-motion";
import type { ComponentType, SVGProps } from "react";

type IconType = ComponentType<SVGProps<SVGSVGElement>>;

type Props = {
  icon: IconType;
  title: string;
  description: string;
  gradient: string;
  delay?: number;
  className?: string;
};

export default function FeatureCard({
  icon: Icon,
  title,
  description,
  gradient,
  delay = 0,
  className = "",
}: Props) {
  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      whileInView={{ opacity: 1, y: 0 }}
      viewport={{ once: true }}
      transition={{ delay }}
      className={`bg-gradient-to-br ${gradient} rounded-2xl p-6 shadow-lg cursor-pointer ${className}  py-4`}
    >
      <div className="bg-white/20 backdrop-blur-sm p-3 rounded-2xl w-fit mb-4">
        <Icon className="w-9 h-9 text-white" />
      </div>
      <h3 className="text-3xl font-bold text-white mb-2">{title}</h3>
      <p className="text-white/80 text-xl md:text-lg">{description}</p>
    </motion.div>
  );
}
