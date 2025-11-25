import { motion } from "framer-motion";
import { Trophy, ArrowRight, Library, CheckCircle2 } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { Button } from "@/shared/ui/Button";

export function LandingCTA() {
    const navigate = useNavigate();

    return (
        <section className="py-10 px-4 bg-gradient-to-br from-purple-500 via-pink-500 to-orange-500">
            <motion.div
                initial={{ opacity: 0, y: 20 }}
                whileInView={{ opacity: 1, y: 0 }}
                viewport={{ once: true }}
                className="max-w-4xl mx-auto text-center space-y-5"
            >
                <motion.div
                    animate={{ scale: [1, 1.05, 1] }}
                    transition={{ duration: 2, repeat: Infinity }}
                >
                    <Trophy className="w-14 h-14 text-white mx-auto" />
                </motion.div>

                <h2 className="text-3xl md:text-5xl font-bold text-white leading-tight">
                    Готовы начать обучение?
                </h2>

                <p className="text-lg md:text-xl text-white/90 max-w-2xl mx-auto">
                    Присоединяйтесь к тысячам пользователей, которые уже прокачивают свои навыки
                    с FlashcardsLoop
                </p>

                <div className="flex flex-col sm:flex-row gap-4 justify-center items-center pt-2">
                    <Button
                        className="flex hover:scale-105 transition-all duration-300 group shadow-lg shadow-purple-900/20"
                        onClick={() => navigate("/login")}
                        size="lg"
                        variant="accent"
                    >
                        Начать бесплатно
                        <ArrowRight className="w-5 h-5 group-hover:translate-x-5 transition-transform duration-500" />
                    </Button>
                    <Button
                        size="lg"
                        variant="ghost"
                        onClick={() => navigate("/store")}
                        className="flex hover:scale-105 transition-all duration-300 border border-white/30 hover:bg-white/20 text-white"
                    >
                        <Library className="w-5 h-5 mr-2" />
                        Библиотека
                    </Button>
                </div>

                <div className="pt-4 flex flex-wrap justify-center gap-x-8 gap-y-2 text-white/80 text-sm md:text-base">
                    <div className="flex items-center gap-2">
                        <CheckCircle2 className="w-4 h-4" />
                        <span>Бесплатно</span>
                    </div>
                    <div className="flex items-center gap-2">
                        <CheckCircle2 className="w-4 h-4" />
                        <span>Без рекламы</span>
                    </div>
                    <div className="flex items-center gap-2">
                        <CheckCircle2 className="w-4 h-4" />
                        <span>Без ограничений</span>
                    </div>
                </div>
            </motion.div>
        </section>
    );
}
