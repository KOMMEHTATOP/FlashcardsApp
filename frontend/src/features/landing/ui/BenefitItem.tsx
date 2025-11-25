import { motion } from "framer-motion";
import { CheckCircle2 } from "lucide-react";

type Props = {
  text: string;
  delay?: number;
  className?: string;
};

export default function BenefitItem({ text, delay = 0, className }: Props) {
  return (
    <motion.div
      initial={{ opacity: 0, x: 0 }}
      whileInView={{ opacity: 1, x: 0 }}
      viewport={{ once: true }}
      transition={{ delay }}
      className={`flex items-center gap-4 bg-base-200 rounded-xl justify-start p-6 border-2 border-base-200 hover:border-success/50 transition-all duration-300
       shadow-lg hover:shadow-success/20 ${className} hover:-translate-y-1`}
    >
      <div className="bg-gradient-to-br from-green-400 to-emerald-500 rounded-full p-2 flex-shrink-0">
        <CheckCircle2 className="w-7 h-7 text-white" />
      </div>
      <p className="text-base-content text-lg">{text}</p>
    </motion.div>
  );
}
