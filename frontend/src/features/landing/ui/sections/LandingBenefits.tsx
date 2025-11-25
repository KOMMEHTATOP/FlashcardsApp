import { motion } from "framer-motion";
import BenefitItem from "../BenefitItem";

interface LandingBenefitsProps {
    benefits: string[];
}

export function LandingBenefits({ benefits }: LandingBenefitsProps) {
    return (
        <section className="py-20 px-4 bg-base-300">
            <div className="max-w-4xl mx-auto">
                <motion.div
                    initial={{ opacity: 0, y: 20 }}
                    whileInView={{ opacity: 1, y: 0 }}
                    viewport={{ once: true }}
                    className="text-center mb-16"
                >
                    <h2 className="text-4xl md:text-5xl font-bold text-base-content mb-4">
                        Советы для эффективного обучения
                    </h2>
                </motion.div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    {benefits.map((b, i) => (
                        <BenefitItem key={i} text={b} delay={i * 0.1} />
                    ))}
                </div>
            </div>
        </section>
    );
}
