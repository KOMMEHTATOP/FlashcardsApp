import { motion } from "framer-motion";
import { Home, Search, BookOpen, AlertCircle, CompassIcon } from "lucide-react";
import { Button } from "@/shared/ui/Button";
import { useNavigate } from "react-router-dom";

interface NotFoundPageProps {
    onBackToHome?: () => void;
}

export function NotFoundPage({ onBackToHome }: NotFoundPageProps) {
    const navigate = useNavigate();
    return (
        <div>
            <div className="min-h-screen transition-colors duration-500 flex items-center justify-center">
                <div className="max-w-2xl w-full">
                    <motion.div
                        initial={{ opacity: 0, y: 20 }}
                        animate={{ opacity: 1, y: 0 }}
                        className="text-center space-y-8"
                    >
                        <motion.div
                            initial={{ scale: 0.8, opacity: 0 }}
                            animate={{ scale: 1, opacity: 1 }}
                            transition={{ delay: 0.1, type: "spring", stiffness: 200 }}
                            className="relative"
                        >
                            <motion.div
                                animate={{
                                    rotate: [0, 5, -5, 5, 0],
                                }}
                                transition={{
                                    duration: 2,
                                    repeat: Infinity,
                                    ease: "easeInOut",
                                }}
                                className="inline-block"
                            >
                                <div className="bg-gradient-to-br from-orange-400 to-red-500 p-8 rounded-3xl shadow-2xl">
                                    <AlertCircle className="w-24 h-24 text-white" />
                                </div>
                            </motion.div>

                            {[...Array(6)].map((_, i) => (
                                <motion.div
                                    key={i}
                                    initial={{ opacity: 0 }}
                                    animate={{
                                        opacity: [0, 1, 0],
                                        y: [-20, -60],
                                        x: [0, (i % 2 === 0 ? 1 : -1) * 30],
                                    }}
                                    transition={{
                                        duration: 2,
                                        repeat: Infinity,
                                        delay: i * 0.3,
                                        ease: "easeOut",
                                    }}
                                    className="absolute top-0 left-1/2"
                                    style={{ marginLeft: `${(i - 3) * 20}px` }}
                                >
                                    <div className="w-3 h-3 rounded-full bg-gradient-to-br from-purple-400 to-pink-400" />
                                </motion.div>
                            ))}
                        </motion.div>

                        <motion.div
                            initial={{ opacity: 0, y: 20 }}
                            animate={{ opacity: 1, y: 0 }}
                            transition={{ delay: 0.2 }}
                        >
                            <h1 className="text-9xl bg-gradient-to-r from-purple-600 to-pink-600 dark:from-purple-400 dark:to-pink-400 bg-clip-text text-transparent mb-4">
                                404
                            </h1>
                        </motion.div>

                        <motion.div
                            initial={{ opacity: 0, y: 20 }}
                            animate={{ opacity: 1, y: 0 }}
                            transition={{ delay: 0.3 }}
                            className="space-y-4"
                        >
                            <h2 className="text-3xl text-base-content">
                                Упс! Страница не найдена
                            </h2>
                            <div className="flex justify-center gap-2 items-center">
                                <p className="text-lg text-base-content/70">
                                    Похоже, вы сбились с пути обучения!
                                </p>
                                <CompassIcon />
                            </div>
                            <p className="text-base-content/40">
                                Этой страницы не существует, но не волнуйтесь - ваш учебный путь
                                ещё не окончен!
                            </p>
                        </motion.div>

                        <motion.div
                            initial={{ opacity: 0, y: 20 }}
                            animate={{ opacity: 1, y: 0 }}
                            transition={{ delay: 0.4 }}
                            className="grid grid-cols-1 sm:grid-cols-2 gap-4 pt-8"
                        >
                            <motion.div
                                whileHover={{ scale: 1.05 }}
                                className="bg-base-200 backdrop-blur-sm p-6 rounded-2xl border-2 border-purple-800 cursor-pointer group"
                            >
                                <BookOpen className="w-12 h-12 text-purple-500 mb-3 mx-auto group-hover:scale-110 transition-transform" />
                                <h3 className="text-base-content mb-2">Продолжайте учиться</h3>
                                <p className="text-sm text-base-content/60">
                                    Становитесь умнее, изучая новые темы
                                </p>
                            </motion.div>

                            <motion.div
                                whileHover={{ scale: 1.05 }}
                                className="bg-base-200 backdrop-blur-sm p-6 rounded-2xl border-2 border-pink-800 cursor-pointer group"
                            >
                                <Search className="w-12 h-12 text-pink-500 mb-3 mx-auto group-hover:scale-110 transition-transform" />
                                <h3 className="text-base-content mb-2">
                                    Исследуйте новые темы
                                </h3>
                                <p className="text-sm text-base-content/60">
                                    Находите новые темы для изучения
                                </p>
                            </motion.div>
                        </motion.div>

                        <motion.div
                            initial={{ opacity: 0, y: 20 }}
                            animate={{ opacity: 1, y: 0 }}
                            transition={{ delay: 0.5 }}
                            className="pt-4 flex justify-center"
                        >
                            <Button
                                onClick={onBackToHome || (() => navigate("/"))}
                                size="lg"
                                className="border-none bg-gradient-to-r from-purple-500 to-pink-500 hover:from-purple-600 hover:to-pink-600 text-white rounded-full px-8 py-6 text-lg shadow-lg hover:shadow-xl transition-all"
                            >
                                <Home className="w-5 h-5 mr-2" />
                                Вернуться на главную
                            </Button>
                        </motion.div>
                    </motion.div>
                </div>
            </div>
        </div>
    );
}