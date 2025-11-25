import { motion } from "framer-motion";
import { Brain, ArrowRight, Search } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { Button } from "@/shared/ui/Button";
import GridMotion from "../../GridMotion";
import RotatingText from "../../RotatingText";
import StatCard from "../StatCard";
import { questions } from "@/shared/data/testData";
import type { Stat } from "@/pages/landing/landingContent";

interface LandingHeroProps {
    isMobile: boolean;
    stats: Stat[];
}

export function LandingHero({ isMobile, stats }: LandingHeroProps) {
    const navigate = useNavigate();

    return (
        <section className="relative overflow-hidden pt-30 md:py-28 px-4 pb-4">
            <div className="absolute inset-0 z-0">
                <GridMotion
                    items={questions}
                    gradientColor="black"
                    isMobile={isMobile}
                />
            </div>
            <div className="absolute inset-0 z-10 bg-gradient-to-br backdrop-filter backdrop-brightness-20 backdrop-blur-[1px]" />
            <div className="max-w-6xl mx-auto relative z-20" id="top">
                <motion.div
                    initial={{ opacity: 0, y: 30 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ duration: 0.6 }}
                    className="text-center space-y-8"
                >
                    <motion.div
                        animate={{ rotate: [0, 5, -5, 0] }}
                        transition={{ duration: 3, repeat: Infinity }}
                        className="inline-block"
                    >
                        <div className="w-28 h-28 mx-auto rounded-3xl bg-gradient-to-br from-purple-400 via-pink-500 to-orange-500 flex items-center justify-center shadow-2xl">
                            <Brain className="w-13 h-13 text-white" />
                        </div>
                    </motion.div>
                    <div className="items-center justify-center flex text-5xl text-white">
                        <RotatingText
                            texts={["Быстро", "Легко", "Бесплатно"]}
                            mainClassName="px-2 sm:px-2 md:px-3 bg-gradient-to-r from-purple-600 to-pink-600 overflow-hidden py-0.5 sm:py-1 md:py-2 justify-center 
                rounded-lg transition-all duration-300 ease-in-out"
                            staggerFrom={"last"}
                            initial={{ y: 100 }}
                            animate={{ y: 0 }}
                            exit={{ y: 120 }}
                            staggerDuration={0.025}
                            splitLevelClassName="overflow-hidden pb-0.5 sm:pb-1 md:pb-1"
                            transition={{ type: "spring", damping: 30, stiffness: 400 }}
                            rotationInterval={4000}
                        />
                    </div>
                    <h1 className="text-5xl md:text-7xl font-bold text-base-content">
                        <span className="block mt-4 bg-gradient-to-r from-purple-400 via-pink-500 to-orange-500 bg-clip-text text-transparent">
                            FlashcardsLoop
                        </span>
                    </h1>

                    {/* ТЕКСТ С КЛЮЧЕВЫМИ СЛОВАМИ ДЛЯ SEO */}
                    <p className="text-xl md:text-2xl text-base-content/70 max-w-3xl mx-auto">
                        Простой способ запомнить всё: от английских слов до кода.
                        Используйте <b>интервальные повторения</b>, создавайте свои колоды
                        или изучайте тысячи готовых карточек в библиотеке.
                    </p>

                    <div className="flex flex-col sm:flex-row gap-4 justify-center items-center">
                        {/* Кнопка 1: Начать */}
                        <Button
                            className="flex hover:scale-105 transition-all duration-300 shadow-2xl hover:shadow-pink-500/50 group text-xl w-full md:w-fit"
                            onClick={() => navigate("/login")}
                            size="lg"
                            variant="secondary"
                        >
                            Начать бесплатно
                            <ArrowRight className="w-5 h-5 group-hover:translate-x-2 transition-transform duration-300" />
                        </Button>

                        {/* Кнопка 2: Библиотека */}
                        <Button
                            size="lg"
                            variant="ghost"
                            onClick={() => navigate("/store")}
                            className="hover:scale-105 transition-transform text-xl w-full md:w-fit border border-white/20 hover:bg-white/10"
                        >
                            <Search className="w-5 h-5 mr-2" />
                            Найти карточки
                        </Button>
                    </div>

                    <div className="grid grid-cols-2 md:grid-cols-4 gap-4 max-w-4xl mx-auto ">
                        {stats.map((stat, index) => (
                            <StatCard
                                key={index}
                                icon={stat.icon}
                                value={stat.value}
                                valuePrefix={stat.valuePrefix}
                                label={stat.label}
                                gradient={stat.gradient}
                                delay={index * 0.1}
                            />
                        ))}
                    </div>
                </motion.div>
            </div>
        </section>
    );
}
