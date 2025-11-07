import { motion, AnimatePresence } from "framer-motion";
import {
  Brain,
  Trophy,
  CheckCircle2,
  ArrowRight,
  ArrowUpFromDotIcon,
} from "lucide-react";
import { useNavigate } from "react-router-dom";
import { LandingContentService } from "./landing/landingContent";
import FeatureCard from "../components/landing/FeatureCard";
import StatCard from "../components/landing/StatCard";
import BenefitItem from "../components/landing/BenefitItem";
import Header from "../components/Header";
import { Button } from "../components/ui/button";
import CardSwap from "../components/CardSwap";
import { forwardRef, useEffect, useMemo, useState } from "react";
import { questions } from "../test/testData";
import GridMotion from "../components/GridMotion";
import FeaturesGrid from "../components/landing/FeatureGrid";
import ScrollStack, { ScrollStackItem } from "../components/ScrollStack";
import RotatingText from "../components/RotatingText";
import { LandingHelmet } from "../components/landing/HelmetLanding";
import { useApp } from "../context/AppContext";
import Footer from "../components/Footer";

export interface CardProps extends React.HTMLAttributes<HTMLDivElement> {
  customClass?: string;
}

export const Card = forwardRef<HTMLDivElement, CardProps>(
  ({ customClass, ...rest }, ref) => (
    <div
      ref={ref}
      {...rest}
      className={`absolute top-1/2 left-1/2 rounded-xl [transform-style:preserve-3d] [will-change:transform] [backface-visibility:hidden] ${
        customClass ?? ""
      } ${rest.className ?? ""}`.trim()}
    />
  )
);

export default function LandingPage() {
  const navigate = useNavigate();
  const { user, logout } = useApp();
  const features = useMemo(() => LandingContentService.getFeatures(), []);
  const gamificationFeatures = useMemo(
    () => LandingContentService.getGamificationFeatures(),
    []
  );
  const steps = useMemo(() => LandingContentService.getSteps(), []);
  const stats = useMemo(() => LandingContentService.getStats(), []);
  const benefits = useMemo(() => LandingContentService.getBenefits(), []);

  const [isMobile, setIsMobile] = useState<boolean>(false);
  const [showScrollButton, setShowScrollButton] = useState<boolean>(false);

  useEffect(() => {
    const handleResize = () => {
      setIsMobile(window.innerWidth <= 768);
    };
    handleResize();
    window.addEventListener("resize", handleResize);
    return () => {
      window.removeEventListener("resize", handleResize);
    };
  }, []);

  useEffect(() => {
    const handleScrollVisibility = () => {
      if (window.scrollY > 200) {
        setShowScrollButton(true);
      } else {
        setShowScrollButton(false);
      }
    };

    window.addEventListener("scroll", handleScrollVisibility);
    return () => window.removeEventListener("scroll", handleScrollVisibility);
  }, []);

  const handleScroll = (name: string) => {
    const targetSection = document.getElementById(name);
    if (targetSection) {
      targetSection.scrollIntoView({ behavior: "smooth" });
    }
  };

  return (
    <div className="min-h-screen bg-base-300">
      {/* мета теги */}
      <LandingHelmet />

      {/* Навигация */}
      <Header user={user} onLogout={logout} />
      <AnimatePresence>
        {showScrollButton && (
          <motion.div
            key="scroll-button"
            initial={{ opacity: 0, y: 50 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: 50 }}
            transition={{ duration: 0.3 }}
            className="fixed bottom-5 right-4 z-50"
          >
            <motion.button
              whileHover={{ scale: 1.15 }}
              whileTap={{ scale: 0.95 }}
              onClick={() => handleScroll("top")}
              className="p-3 rounded-full bg-info text-white 
                   shadow-lg hover:bg-info/90 
                   transition-all duration-300 
                   opacity-80 hover:opacity-100"
            >
              <ArrowUpFromDotIcon className="w-7 h-7" />
            </motion.button>
          </motion.div>
        )}
      </AnimatePresence>

      {/* Главный секция */}
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
                texts={["Учись", "Развивайся", "Изучай"]}
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
              <span className="ml-2">c</span>
            </div>
            <h1 className="text-5xl md:text-7xl font-bold text-base-content">
              <span className="block mt-4 bg-gradient-to-r from-purple-400 via-pink-500 to-orange-500 bg-clip-text text-transparent">
                FlashcardsLoop
              </span>
            </h1>
            <p className="text-xl md:text-2xl text-base-content/70 max-w-3xl mx-auto">
              Современное веб-приложение для эффективного запоминания информации
              с помощью карточек. Создавайте, изучайте, прокачивайтесь!
            </p>
            <div className="flex flex-col sm:flex-row gap-4 justify-center items-center">
              <Button
                className="flex hover:scale-105 transition-all duration-300 shadow-2xl hover:shadow-pink-500/50 group text-2xl w-full md:w-fit"
                onClick={() => navigate("/login")}
                size="lg"
                variant="secondary"
              >
                Начать
                <ArrowRight className="w-5 h-5 group-hover:translate-x-5 transition-transform duration-500" />
              </Button>
              <Button
                size="lg"
                onClick={() => handleScroll("features")}
                className="hover:scale-105 transition-transform text-2xl w-full md:w-fit"
              >
                Узнать больше
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

      {/* Раздел функций */}
      <section id="features" className="py-24 px-4 bg-base-200">
        <div className="max-w-6xl mx-auto">
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            className="text-center mb-16"
          >
            <h2 className="text-4xl md:text-5xl font-bold text-base-content mb-4">
              Основные возможности
            </h2>
            <p className="text-xl text-base-content/70">
              Всё что нужно для эффективного обучения
            </p>
          </motion.div>
          <FeaturesGrid features={features} />
        </div>
      </section>

      {/* Раздел геймификации */}
      <section className="py-32 px-4 bg-base-300 overflow-hidden">
        <div className="max-w-6xl mx-auto flex-row md:flex relative pb-16 md:pb-0">
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            className="text-center mb-30 md:mb-16"
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
            <p className="text-xl text-base-content/70">
              Превратите обучение в увлекательную игру
            </p>
          </motion.div>
          <div className="absolute inset-0 items-center -bottom-10 md:bottom-0 right-25">
            <CardSwap
              cardDistance={70}
              verticalDistance={60}
              delay={3000}
              pauseOnHover={false}
              skewAmount={isMobile ? 0 : 3}
              easing="elastic"
            >
              {gamificationFeatures.slice(0, 6).map((feature, index) => (
                <Card
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
                    className="w-300 h-60 scale-140 md:scale-100 shadow-xl hover:shadow-white/10 hover:scale-105 transition-all duration-300"
                  />
                </Card>
              ))}
            </CardSwap>
          </div>
        </div>
      </section>

      {/* Раздел шагов */}
      <section className="py-24 px-4 bg-base-200">
        <div className="max-w-6xl mx-auto">
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            className="text-center"
          >
            <h2 className="text-4xl md:text-5xl font-bold text-base-content mb-4">
              Как начать?
            </h2>
            <p className="text-xl text-base-content/70">
              Всего 4 простых шага до первого урока
            </p>
          </motion.div>
        </div>
        <div className="relative max-w-7xl mx-auto">
          <ScrollStack
            className="max-h-[70dvh] scroll-bar-none"
            baseScale={0.9}
            itemDistance={160}
            blurAmount={1}
            stackPosition="10%"
            itemStackDistance={60}
          >
            {steps.map((step, index) => (
              <ScrollStackItem key={index}>
                <div
                  className={`bg-gradient-to-br ${step.gradient} rounded-2xl p-8 shadow-lg h-full overflow-hidden `}
                >
                  <div className="text-6xl font-bold text-white/40 mb-4">
                    {step.number}
                  </div>
                  <h3 className="text-4xl font-bold text-white mb-3">
                    {step.title}
                  </h3>
                  <p className="text-2xl text-white/90">{step.description}</p>
                </div>
              </ScrollStackItem>
            ))}
          </ScrollStack>
          <div className="absolute bg-gradient-to-t from-base-200 to-base-300/0 w-full h-10 -bottom-1" />
        </div>
      </section>

      {/* Раздел советов */}
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

      {/* Раздел присоединиться */}
      <section className="py-20 px-4 bg-gradient-to-br from-purple-500 via-pink-500 to-orange-500">
        <motion.div
          initial={{ opacity: 0, y: 30 }}
          whileInView={{ opacity: 1, y: 0 }}
          viewport={{ once: true }}
          className="max-w-4xl mx-auto text-center space-y-8"
        >
          <motion.div
            animate={{ scale: [1, 1.05, 1] }}
            transition={{ duration: 2, repeat: Infinity }}
          >
            <Trophy className="w-20 h-20 text-white mx-auto" />
          </motion.div>

          <h2 className="text-4xl md:text-6xl font-bold text-white">
            Готовы начать обучение?
          </h2>

          <p className="text-xl md:text-2xl text-white/90">
            Присоединяйтесь к пользователям, которые уже прокачивают свои навыки
            с FlashcardsLoop
          </p>

          <div className="flex flex-col sm:flex-row gap-4 justify-center items-center">
            <Button
              className="flex hover:scale-105 transition-all duration-300 group"
              onClick={() => navigate("/login")}
              size="lg"
              variant="accent"
            >
              Начать
              <ArrowRight className="w-5 h-5 group-hover:translate-x-5 transition-transform duration-500" />
            </Button>
          </div>

          <div className="pt-8 flex justify-center gap-8 text-white/80 text-md md:text-lg">
            <div className="flex items-center gap-2">
              <CheckCircle2 className="w-5 h-5" />
              <span>Бесплатно</span>
            </div>
            <div className="flex items-center gap-2">
              <CheckCircle2 className="w-5 h-5" />
              <span>Без рекламы</span>
            </div>
            <div className="flex items-center gap-2">
              <CheckCircle2 className="w-5 h-5" />
              <span>Без ограничений</span>
            </div>
          </div>
        </motion.div>
      </section>

      <Footer />
    </div>
  );
}
