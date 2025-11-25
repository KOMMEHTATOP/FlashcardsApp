import { motion } from "framer-motion";
import ScrollStack, { ScrollStackItem } from "@/shared/ui/effects/ScrollStack";
import type { Step } from "@/pages/landing/landingContent";

interface LandingHowItWorksProps {
    steps: Step[];
}

export function LandingHowItWorks({ steps }: LandingHowItWorksProps) {
    return (
        <section className="py-24 px-4 bg-base-200">
            <div className="max-w-6xl mx-auto">
                <motion.div
                    initial={{ opacity: 0, y: 20 }}
                    whileInView={{ opacity: 1, y: 0 }}
                    viewport={{ once: true }}
                    className="text-center mb-12"
                >
                    <h2 className="text-4xl md:text-5xl font-bold text-base-content mb-4">
                        Как начать?
                    </h2>
                    <p className="text-xl text-base-content/70">
                        Всего 4 простых шага до первого урока
                    </p>
                </motion.div>

                <div className="grid grid-cols-1 lg:grid-cols-2 gap-10 items-center">
                    <div className="h-[500px] w-full relative">
                        <ScrollStack
                            className="h-full w-full scroll-bar-none"
                            baseScale={0.9}
                            itemDistance={40}
                            blurAmount={1}
                            stackPosition="0%"
                            itemStackDistance={20}
                        >
                            {steps.map((step, index) => (
                                <ScrollStackItem key={index}>
                                    <div
                                        className={`bg-gradient-to-br ${step.gradient} rounded-2xl p-8 shadow-lg h-full overflow-hidden flex flex-col justify-center`}
                                    >
                                        <div className="text-6xl font-bold text-white/40 mb-4">
                                            {step.number}
                                        </div>
                                        <h3 className="text-3xl font-bold text-white mb-3">
                                            {step.title}
                                        </h3>
                                        <p className="text-xl text-white/90">{step.description}</p>
                                    </div>
                                </ScrollStackItem>
                            ))}
                        </ScrollStack>
                    </div>

                    <motion.div
                        initial={{ opacity: 0, x: 20 }}
                        whileInView={{ opacity: 1, x: 0 }}
                        viewport={{ once: true }}
                        transition={{ duration: 0.6 }}
                        className="text-left space-y-6 px-4 lg:px-10"
                    >
                        <h3 className="text-3xl md:text-4xl font-bold text-base-content">
                            Готовы к быстрому старту?
                        </h3>
                        <p className="text-lg text-base-content/70">
                            Вам не нужно ничего устанавливать. Регистрация займет всего пару кликов,
                            и вы сразу сможете создать свою первую колоду или найти готовые карточки в библиотеке.
                        </p>
                    </motion.div>
                </div>
            </div>
        </section>
    );
}
