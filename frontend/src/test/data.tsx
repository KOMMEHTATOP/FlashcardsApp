import {
  Atom,
  Beaker,
  BookHeart,
  BookOpen,
  Brain,
  Briefcase,
  Calculator,
  Code,
  Coffee,
  Dna,
  Dumbbell,
  Gamepad2,
  Globe,
  GraduationCap,
  Heart,
  Languages,
  Lightbulb,
  Microscope,
  Music,
  Palette,
  Rocket,
  Smile,
  Sparkles,
  Star,
  Terminal,
  Trophy,
  Zap,
} from "lucide-react";

const TITLE_APP = import.meta.env.VITE_APP_TITLE || "Studing";

const floatingIcons = [
  { icon: Trophy, color: "text-yellow-400", delay: 0, x: "10%", y: "20%" },
  { icon: Star, color: "text-pink-400", delay: 0.2, x: "85%", y: "15%" },
  { icon: Zap, color: "text-purple-400", delay: 0.4, x: "15%", y: "75%" },
  { icon: Sparkles, color: "text-blue-400", delay: 0.6, x: "80%", y: "70%" },
  { icon: Brain, color: "text-pink-400", delay: 0.8, x: "20%", y: "40%" },
  { icon: BookHeart, color: "text-yellow-400", delay: 2, x: "90%", y: "50%" },
];

const recallRatingInfo: Record<number, string> = {
  1: "😵 Забыл начисто",
  2: "🤔 Что-то знакомое",
  3: "😌 Почти вспомнил",
  4: "💪 Запомнил хорошо",
  5: "🚀 Мастер памяти!",
};

const motivationTexts: Record<string, string> = {
  "0-20": "😵 Не беда! Всё только начинается. Попробуй ещё раз - успех рядом!",
  "21-40": "🤔 Уже лучше! Ты начинаешь вспоминать. Ещё немного практики!",
  "41-60": "😌 Хороший прогресс! Ещё пара повторений - и запомнишь идеально.",
  "61-80": "💪 Отлично! Память работает как часы, чуть-чуть до мастерства!",
  "81-94": "🔥 Почти идеально! Осталось чуть-чуть до полного совершенства!",
  "95-100": "🚀 Мастер памяти! Ты выжал из себя максимум, великолепная работа!",
};

const levelMotivationVariants: Record<string, string[]> = {
  "0-10": [
    "🌱 Начало пути - всё впереди, главное не останавливаться!",
    "🧩 Первый шаг сделан, и это уже успех!",
    "🚶‍♂️ Каждый эксперт когда-то начинал - продолжай, и всё получится!",
  ],
  "11-30": [
    "🔥 Хорошее начало! Немного усилий - и будет новый уровень!",
    "⚡ Ты уже в движении, не сбавляй темп!",
    "💡 С каждым действием ты становишься сильнее - продолжай!",
  ],
  "31-60": [
    "💪 Уже видно прогресс - половина пути пройдена!",
    "🚀 Всё идёт по плану, не останавливайся сейчас!",
    "✨ Твоя память становится всё лучше, так держать!",
  ],
  "61-90": [
    "⚔️ Осталось немного - ты почти на новом уровне!",
    "🔥 Вот-вот, ещё чуть-чуть - и уровень твой!",
    "🚀 Почти взлетел! Не теряй фокус!",
  ],
  "91-99": [
    "🏁 Финальная прямая - не сдавайся!",
    "💥 Осталось совсем чуть-чуть - давай, ты можешь!",
    "✨ Один последний рывок до победы!",
  ],
  "100": [
    "🏆 Уровень повышен! Ты герой обучения!",
    "🎉 Новый уровень! Отличная работа, продолжай прокачку!",
    "🚀 Ты стал умнее и сильнее - гордость системы!",
  ],
};

const colorRatingCard = {
  0: "bg-gray-300 dark:bg-gray-600",
  1: "bg-red-500",
  2: "bg-purple-500",
  3: "bg-gray-300",
  4: "bg-green-500",
  5: "bg-yellow-300",
};

// Расширенная палитра градиентов (совпадает с C# генератором)
const availableColors = [
  { id: 1, gradient: "from-blue-500 to-cyan-500", name: "Синий океан" },
  { id: 2, gradient: "from-emerald-500 to-teal-500", name: "Изумрудный" },
  { id: 3, gradient: "from-orange-500 to-yellow-500", name: "Солнечный" },
  { id: 4, gradient: "from-pink-500 to-rose-500", name: "Розовый" },
  { id: 5, gradient: "from-purple-600 to-blue-600", name: "Глубокий космос" },
  { id: 6, gradient: "from-indigo-500 to-purple-500", name: "Индиго" },
  { id: 7, gradient: "from-red-500 to-orange-500", name: "Огненный" },
  { id: 8, gradient: "from-lime-500 to-green-500", name: "Лайм" },
  { id: 9, gradient: "from-teal-400 to-blue-500", name: "Морская волна" },
  { id: 10, gradient: "from-fuchsia-600 to-pink-600", name: "Фуксия" },
  { id: 11, gradient: "from-rose-400 to-red-500", name: "Алая роза" },
  { id: 12, gradient: "from-sky-500 to-indigo-500", name: "Небесный" },
  { id: 13, gradient: "from-violet-600 to-indigo-600", name: "Ультрафиолет" },
  { id: 14, gradient: "from-amber-500 to-orange-600", name: "Янтарь" },
  { id: 15, gradient: "from-cyan-500 to-blue-500", name: "Лазурь" },
];

// Расширенный список иконок (English keys для совпадения с БД)
const availableIcons = [
  { icon: BookOpen, name: "BookOpen" }, // Книга
  { icon: Code, name: "Code" }, // Код
  { icon: Globe, name: "Globe" }, // Глобус
  { icon: Languages, name: "Languages" }, // Языки
  { icon: Brain, name: "Brain" }, // Мозг
  { icon: Calculator, name: "Calculator" }, // Калькулятор
  { icon: Dna, name: "Dna" }, // ДНК
  { icon: Atom, name: "Atom" }, // Атом
  { icon: Music, name: "Music" }, // Музыка
  { icon: Palette, name: "Palette" }, // Палитра
  { icon: Briefcase, name: "Briefcase" }, // Бизнес
  { icon: Coffee, name: "Coffee" }, // Отдых/Кофе
  { icon: Dumbbell, name: "Dumbbell" }, // Спорт
  { icon: Gamepad2, name: "Gamepad2" }, // Игры
  { icon: GraduationCap, name: "GraduationCap" }, // Образование
  { icon: Heart, name: "Heart" }, // Здоровье
  { icon: Lightbulb, name: "Lightbulb" }, // Идеи
  { icon: Microscope, name: "Microscope" }, // Наука
  { icon: Rocket, name: "Rocket" }, // Стартап
  { icon: Smile, name: "Smile" }, // Развлечения
  { icon: Terminal, name: "Terminal" }, // Терминал
  { icon: Beaker, name: "Beaker" }, // Химия
  { icon: Zap, name: "Zap" }, // Энергия
];

export {
  floatingIcons,
  recallRatingInfo,
  motivationTexts,
  colorRatingCard,
  levelMotivationVariants,
  availableColors,
  availableIcons,
  TITLE_APP,
};