import { motion } from "framer-motion";
import {
  Brain,
  Zap,
  Trophy,
  Target,
  BookOpen,
  Star,
  TrendingUp,
  Users,
  Clock,
  Award,
  Flame,
  CheckCircle2,
  ArrowRight,
  Sparkles,
} from "lucide-react";
import { Link } from "react-router-dom";

export default function LandingPage() {
  const features = [
    {
      icon: Brain,
      title: "Карточки для запоминания",
      description:
        "Создавайте карточки с вопросом и ответом для изучения любой информации",
      gradient: "from-blue-400 to-cyan-500",
    },
    {
      icon: BookOpen,
      title: "Группы карточек",
      description:
        "Организуйте карточки по темам: английский, программирование, история",
      gradient: "from-purple-400 to-pink-500",
    },
    {
      icon: Star,
      title: "Система оценок",
      description:
        "Оценивайте свои знания от 1 до 5 звёзд после изучения каждой карточки",
      gradient: "from-yellow-400 to-orange-500",
    },
    {
      icon: TrendingUp,
      title: "Детальная статистика",
      description:
        "Отслеживайте прогресс, время обучения и количество изученных карточек",
      gradient: "from-green-400 to-emerald-500",
    },
    {
      icon: Trophy,
      title: "Достижения",
      description:
        "Получайте награды за успехи: streak, количество карточек, уровни",
      gradient: "from-amber-400 to-orange-500",
    },
    {
      icon: Zap,
      title: "Уровни и XP",
      description: "Прокачивайте уровень, зарабатывая опыт за изучение",
      gradient: "from-indigo-400 to-purple-500",
    },
  ];

  const gamificationFeatures = [
    {
      icon: Sparkles,
      title: "Система опыта (XP)",
      description:
        "Получайте XP за каждую карточку. Чем выше оценка, тем больше XP",
      gradient: "from-cyan-400 to-blue-500",
    },
    {
      icon: Target,
      title: "Уровни",
      description:
        "Повышайте свой уровень, набирая опыт. Каждый уровень открывает новые возможности",
      gradient: "from-violet-400 to-purple-500",
    },
    {
      icon: Flame,
      title: "Streak система",
      description:
        "Занимайтесь каждый день подряд и получайте бонусный XP. Побейте свой рекорд!",
      gradient: "from-red-400 to-pink-500",
    },
    {
      icon: Award,
      title: "Достижения",
      description:
        "Разблокируйте достижения: Common, Rare, Epic, Legendary",
      gradient: "from-lime-400 to-green-500",
    },
  ];

  const steps = [
    {
      number: "01",
      title: "Зарегистрируйтесь",
      description: "Создайте бесплатный аккаунт за 30 секунд",
      gradient: "from-blue-400 to-cyan-500",
    },
    {
      number: "02",
      title: "Создайте группу",
      description: 'Добавьте первую группу карточек (например, "Английский")',
      gradient: "from-purple-400 to-pink-500",
    },
    {
      number: "03",
      title: "Добавьте карточки",
      description: "Заполните группу карточками для изучения",
      gradient: "from-yellow-400 to-orange-500",
    },
    {
      number: "04",
      title: "Начните изучение",
      description: "Запустите сессию и прокачивайте свой уровень!",
      gradient: "from-green-400 to-emerald-500",
    },
  ];

  const stats = [
    {
      icon: Users,
      value: "1,000+",
      label: "Активных пользователей",
      gradient: "from-blue-400 to-cyan-500",
    },
    {
      icon: BookOpen,
      value: "50,000+",
      label: "Карточек создано",
      gradient: "from-purple-400 to-pink-500",
    },
    {
      icon: Clock,
      value: "10,000+",
      label: "Часов обучения",
      gradient: "from-yellow-400 to-orange-500",
    },
    {
      icon: Flame,
      value: "7 дней",
      label: "Средний streak",
      gradient: "from-red-400 to-pink-500",
    },
  ];

  const benefits = [
    "Занимайтесь регулярно и сохраняйте streak",
    "Честно оценивайте знания для лучших результатов",
    "Создавайте компактные карточки",
    "Следите за статистикой прогресса",
    "Гонитесь за достижениями",
    "Не прерывайте streak — даже 10 минут в день делают разницу",
  ];

  return (
    <div className="min-h-screen bg-base-300">
      {/* Навигация */}
      <nav className="navbar bg-base-300 px-4 py-4 sticky top-0 z-50 backdrop-blur-sm bg-base-300/90">
        <div className="navbar-start">
          <motion.div
            initial={{ opacity: 0, x: -20 }}
            animate={{ opacity: 1, x: 0 }}
            className="flex items-center gap-2"
          >
            <div className="w-10 h-10 rounded-2xl bg-gradient-to-br from-purple-400 to-pink-500 flex items-center justify-center">
              <Brain className="w-6 h-6 text-white" />
            </div>
            <span className="text-2xl font-bold text-base-content">
              FlashcardsLoop
            </span>
          </motion.div>
        </div>
        <div className="navbar-end gap-4">
          <Link to="/login" className="btn btn-ghost">
            Войти
          </Link>
          <Link
            to="/login"
            className="btn bg-gradient-to-r from-purple-400 to-pink-500 text-white border-none hover:scale-105 transition-transform"
          >
            Начать бесплатно
          </Link>
        </div>
      </nav>

      {/* Hero Section */}
      <section className="relative overflow-hidden py-20 px-4">
        <div className="absolute inset-0 bg-gradient-to-br from-purple-500/10 via-pink-500/10 to-orange-500/10" />
        <div className="max-w-6xl mx-auto relative">
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
              <div className="w-24 h-24 mx-auto rounded-3xl bg-gradient-to-br from-purple-400 via-pink-500 to-orange-500 flex items-center justify-center shadow-2xl">
                <Brain className="w-12 h-12 text-white" />
              </div>
            </motion.div>

            <h1 className="text-5xl md:text-7xl font-bold text-base-content">
              Учись и развивайся
              <span className="block mt-4 bg-gradient-to-r from-purple-400 via-pink-500 to-orange-500 bg-clip-text text-transparent">
                с FlashcardsLoop
              </span>
            </h1>

            <p className="text-xl md:text-2xl text-base-content/70 max-w-3xl mx-auto">
              Современное веб-приложение для эффективного запоминания информации
              с помощью карточек. Создавайте, изучайте, прокачивайтесь!
            </p>

            <div className="flex flex-col sm:flex-row gap-4 justify-center items-center">
              <Link
                to="/login"
                className="btn btn-lg bg-gradient-to-r from-purple-400 to-pink-500 text-white border-none hover:scale-105 transition-transform"
              >
                Начать бесплатно
                <ArrowRight className="w-5 h-5" />
              </Link>
              <a
                href="#features"
                className="btn btn-lg btn-outline"
              >
                Узнать больше
              </a>
            </div>

            <div className="grid grid-cols-2 md:grid-cols-4 gap-4 max-w-4xl mx-auto pt-12">
              {stats.map((stat, index) => {
                const Icon = stat.icon;
                return (
                  <motion.div
                    key={index}
                    initial={{ opacity: 0, y: 20 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ delay: index * 0.1 }}
                    className={`bg-gradient-to-br ${stat.gradient} rounded-2xl p-6 text-white shadow-lg`}
                  >
                    <Icon className="w-8 h-8 mb-2 mx-auto" />
                    <div className="text-3xl font-bold mb-1">{stat.value}</div>
                    <div className="text-sm opacity-90">{stat.label}</div>
                  </motion.div>
                );
              })}
            </div>
          </motion.div>
        </div>
      </section>

      {/* Features Section */}
      <section id="features" className="py-20 px-4 bg-base-200">
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

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {features.map((feature, index) => {
              const Icon = feature.icon;
              return (
                <motion.div
                  key={index}
                  initial={{ opacity: 0, y: 20 }}
                  whileInView={{ opacity: 1, y: 0 }}
                  viewport={{ once: true }}
                  transition={{ delay: index * 0.1 }}
                  whileHover={{ scale: 1.02, y: -5 }}
                  className={`bg-gradient-to-br ${feature.gradient} rounded-2xl p-6 shadow-lg cursor-pointer`}
                >
                  <div className="bg-white/20 backdrop-blur-sm p-3 rounded-2xl w-fit mb-4">
                    <Icon className="w-8 h-8 text-white" />
                  </div>
                  <h3 className="text-xl font-bold text-white mb-2">
                    {feature.title}
                  </h3>
                  <p className="text-white/90">{feature.description}</p>
                </motion.div>
              );
            })}
          </div>
        </div>
      </section>

      {/* Gamification Section */}
      <section className="py-20 px-4 bg-base-300">
        <div className="max-w-6xl mx-auto">
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            className="text-center mb-16"
          >
            <div className="inline-block mb-4">
              <div className="w-16 h-16 rounded-2xl bg-gradient-to-br from-yellow-400 to-orange-500 flex items-center justify-center">
                <Trophy className="w-8 h-8 text-white" />
              </div>
            </div>
            <h2 className="text-4xl md:text-5xl font-bold text-base-content mb-4">
              Геймификация
            </h2>
            <p className="text-xl text-base-content/70">
              Превратите обучение в увлекательную игру
            </p>
          </motion.div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {gamificationFeatures.map((feature, index) => {
              const Icon = feature.icon;
              return (
                <motion.div
                  key={index}
                  initial={{ opacity: 0, x: index % 2 === 0 ? -20 : 20 }}
                  whileInView={{ opacity: 1, x: 0 }}
                  viewport={{ once: true }}
                  transition={{ delay: index * 0.1 }}
                  className={`bg-gradient-to-br ${feature.gradient} rounded-2xl p-8 shadow-lg`}
                >
                  <Icon className="w-12 h-12 text-white mb-4" />
                  <h3 className="text-2xl font-bold text-white mb-3">
                    {feature.title}
                  </h3>
                  <p className="text-white/90 text-lg">{feature.description}</p>
                </motion.div>
              );
            })}
          </div>
        </div>
      </section>

      {/* How It Works Section */}
      <section className="py-20 px-4 bg-base-200">
        <div className="max-w-6xl mx-auto">
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            className="text-center mb-16"
          >
            <h2 className="text-4xl md:text-5xl font-bold text-base-content mb-4">
              Как начать?
            </h2>
            <p className="text-xl text-base-content/70">
              Всего 4 простых шага до первого урока
            </p>
          </motion.div>

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            {steps.map((step, index) => (
              <motion.div
                key={index}
                initial={{ opacity: 0, y: 20 }}
                whileInView={{ opacity: 1, y: 0 }}
                viewport={{ once: true }}
                transition={{ delay: index * 0.1 }}
                className="relative"
              >
                <div
                  className={`bg-gradient-to-br ${step.gradient} rounded-2xl p-6 shadow-lg h-full`}
                >
                  <div className="text-6xl font-bold text-white/20 mb-4">
                    {step.number}
                  </div>
                  <h3 className="text-xl font-bold text-white mb-3">
                    {step.title}
                  </h3>
                  <p className="text-white/90">{step.description}</p>
                </div>
                {index < steps.length - 1 && (
                  <div className="hidden lg:block absolute top-1/2 -right-3 transform -translate-y-1/2">
                    <ArrowRight className="w-6 h-6 text-base-content/30" />
                  </div>
                )}
              </motion.div>
            ))}
          </div>
        </div>
      </section>

      {/* Benefits Section */}
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
            {benefits.map((benefit, index) => (
              <motion.div
                key={index}
                initial={{ opacity: 0, x: index % 2 === 0 ? -20 : 20 }}
                whileInView={{ opacity: 1, x: 0 }}
                viewport={{ once: true }}
                transition={{ delay: index * 0.1 }}
                className="flex items-start gap-4 bg-base-200 rounded-xl p-6"
              >
                <div className="bg-gradient-to-br from-green-400 to-emerald-500 rounded-full p-2 flex-shrink-0">
                  <CheckCircle2 className="w-5 h-5 text-white" />
                </div>
                <p className="text-base-content text-lg">{benefit}</p>
              </motion.div>
            ))}
          </div>
        </div>
      </section>

      {/* Final CTA Section */}
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
            Присоединяйтесь к тысячам пользователей, которые уже прокачивают
            свои навыки с FlashcardsLoop
          </p>

          <div className="flex flex-col sm:flex-row gap-4 justify-center items-center">
            <Link
              to="/login"
              className="btn btn-lg bg-white text-purple-500 border-none hover:scale-105 transition-transform shadow-2xl"
            >
              Начать бесплатно
              <ArrowRight className="w-5 h-5" />
            </Link>
          </div>

          <div className="pt-8 flex justify-center gap-8 text-white/80">
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

      {/* Footer */}
      <footer className="bg-base-300 py-8 px-4">
        <div className="max-w-6xl mx-auto text-center">
          <div className="flex items-center justify-center gap-2 mb-4">
            <div className="w-8 h-8 rounded-xl bg-gradient-to-br from-purple-400 to-pink-500 flex items-center justify-center">
              <Brain className="w-5 h-5 text-white" />
            </div>
            <span className="text-xl font-bold text-base-content">
              FlashcardsLoop
            </span>
          </div>
          <p className="text-base-content/60">
            © 2024 FlashcardsLoop. Учись и развивайся.
          </p>
        </div>
      </footer>
    </div>
  );
}
