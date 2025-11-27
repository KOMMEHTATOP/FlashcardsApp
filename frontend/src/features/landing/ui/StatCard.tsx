import { motion } from "framer-motion";
import gsap from "gsap";
import { useEffect, useRef, type ComponentType, type SVGProps } from "react";

type IconType = ComponentType<SVGProps<SVGSVGElement>>;

type Props = {
  icon: IconType;
  value: string;
  valuePrefix?: string;
  label: string;
  gradient: string;
  delay?: number;
};

export default function StatCard({
  icon: Icon,
  value,
  valuePrefix,
  label,
  gradient,
  delay = 0,
}: Props) {
  const numberRef = useRef<HTMLDivElement>(null);
  const hasAnimated = useRef(false);
  useEffect(() => {
    const num = parseInt(value.replace(/[^0-9]/g, ""), 10);
    if (isNaN(num) || !numberRef.current || hasAnimated.current) return;

    const displayRef = numberRef.current;

    displayRef.textContent = "0";

    gsap.to(
      { val: 0 },
      {
        val: num,
        duration: 2.2,
        delay: delay + 0.3,
        ease: "power2.out",
        onUpdate: function () {
          displayRef.textContent = Math.round(
            this.targets()[0].val
          ).toLocaleString();
        },
        onComplete: () => {
          hasAnimated.current = true;
        },
      }
    );
  }, [value, delay]);

  const isNumber = !isNaN(parseInt(value, 10));

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ delay }}
      className={`bg-gradient-to-br ${gradient} rounded-2xl p-6 text-white shadow-lg`}
    >
      <Icon className="w-8 h-8 mb-2 mx-auto" />
      <div className="text-3xl font-bold mb-1 ">
        {isNumber ? <span ref={numberRef}>0</span> : value}
        {valuePrefix && ` ${valuePrefix}`}
      </div>
      <div className="text-sm opacity-90">{label}</div>
    </motion.div>
  );
}
