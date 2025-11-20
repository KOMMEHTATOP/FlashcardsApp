import { motion } from "framer-motion";
import { Trophy, Target, Zap } from "lucide-react";
import CardSwap from "../../CardSwap";
import FeatureCard from "../FeatureCard";
import { ThreeDCard } from "../ThreeDCard";
import type { Feature } from "../../../pages/landing/landingContent";

interface LandingGamificationProps {
    features: Feature[];
    isMobile?: boolean;
}

export function LandingGamification({ features, isMobile = false }: LandingGamificationProps) {
    return (
        <section className="py-32 px-4 bg-base-300 overflow-hidden">
            <div className="max-w-6xl mx-auto grid grid-cols-1 lg:grid-cols-2 gap-16 items-center relative">
                <motion.div
                    initial={{ opacity: 0, y: 20 }}
                    whileInView={{ opacity: 1, y: 0 }}
                    viewport={{ once: true }}
                    className="text-center lg:text-left z-10"
                >
                    <div className="inline-block mb-4">
                        <motion.div
                            animate={{ rotate: [0, 5, -5, 0] }}
                            transition={{ duration: 3, repeat: Infinity }}
                            className="w-30 h-30 rounded-2xl bg-gradient-to-br from-yellow-400 to-orange-500 flex items-center justify-center"
                        >
                            <Trophy className="w-14 h-14 text-white" />
                        </motion.div>
                    </div>
                    <h2 className="text-4xl md:text-5xl font-bold text-base-content mb-4">
                        Геймификация
                    </h2>
                    <p className="text-xl text-base-content/70 mb-8">
                        Превратите обучение в увлекательную игру. Получайте достижения,
                        соревнуйтесь с друзьями и отслеживайте свой прогресс.
                    </p>

                    <div className="space-y-6 hidden lg:block">
                        {[
                            {
                                icon: Trophy,
                                title: "Система достижений",
                                desc: "Более 50 уникальных наград за регулярные занятия",
                                color: "text-yellow-500"
                            },
                            {
                                icon: Zap,
                                title: "Ударный режим",
                                desc: "Поддерживайте серию занятий и получайте бонусы",
                                color: "text-orange-500"
                            },
                            {
                                icon: Target,
                                title: "Умные цели",
                                desc: "Персональные планы обучения под ваш ритм жизни",
                                color: "text-green-500"
                            }
                        ].map((item, i) => (
                            <motion.div
                                key={i}
                                initial={{ opacity: 0, x: 50 }}
                                whileInView={{ opacity: 1, x: 0 }}
                                viewport={{ once: true }}
                                transition={{ delay: i * 0.1 }}
                                className="flex items-start gap-4 p-4 rounded-xl bg-base-200/50 hover:bg-base-200 transition-colors"
                            >
                                <div className={`p-3 rounded-lg bg-base-100 shadow-lg ${item.color}`}>
                                    <item.icon className="w-6 h-6" />
                                </div>
                                <div>
                                    <h3 className="text-lg font-bold text-base-content mb-1">{item.title}</h3>
                                    <p className="text-base-content/60">{item.desc}</p>
                                </div>
                            </motion.div>
                        ))}
                    </div>
                </motion.div>

                <div className="relative h-[500px] flex items-center justify-center">
                    <div className="absolute inset-0 bg-gradient-to-br from-purple-500/20 to-pink-500/20 rounded-full blur-3xl" />
                    <CardSwap
                        cardDistance={40}
                        verticalDistance={60}
                        delay={3000}
                        pauseOnHover={false}
                        skewAmount={isMobile ? 0 : 6}
                        easing="elastic"
                    >
                        {features.slice(0, 6).map((feature, index) => (
                            <ThreeDCard
                                key={index}
                                className={`bg-gradient-to-r items-center justify-center flex`}
                            >
                                <FeatureCard
                                    key={index}
                                    icon={feature.icon}
                                    title={feature.title}
                                    description={feature.description}
                                    gradient={feature.gradient}
                                    delay={index * 0.1}
                                    className="w-80 h-96 shadow-xl hover:shadow-white/10 hover:scale-105 transition-all duration-300"
                                />
                            </ThreeDCard>
                        ))}
                    </CardSwap>
                </div>
            </div>
        </section>
    );
}
