import {
  Beaker,
  BookHeart,
  BookOpen,
  Brain,
  Calculator,
  Globe,
  Palette,
  Sparkles,
  Star,
  Trophy,
  Zap,
} from "lucide-react";
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

const availableColors = [
  { id: 1, gradient: "from-blue-400 to-cyan-500", name: "Синий океан" },
  { id: 2, gradient: "from-purple-400 to-pink-500", name: "Пурпурно-розовый" },
  { id: 3, gradient: "from-yellow-400 to-orange-500", name: "Закат" },
  { id: 4, gradient: "from-green-400 to-emerald-500", name: "Лесной зеленый" },
  { id: 5, gradient: "from-red-400 to-pink-500", name: "Розово-красный" },
  {
    id: 6,
    gradient: "from-indigo-400 to-purple-500",
    name: "Темно-фиолетовый",
  },
  { id: 7, gradient: "from-cyan-400 to-blue-500", name: "Небесно-голубой" },
  {
    id: 8,
    gradient: "from-amber-400 to-orange-500",
    name: "Янтарное свечение",
  },
  { id: 9, gradient: "from-teal-400 to-green-500", name: "Бирюзовый сон" },
  { id: 10, gradient: "from-pink-400 to-rose-500", name: "Розовая роза" },
  { id: 11, gradient: "from-violet-400 to-purple-500", name: "Фиолетовый" },
  { id: 12, gradient: "from-lime-400 to-green-500", name: "Зеленый лайм" },
];

const availableIcons = [
  { icon: BookOpen, name: "Книга" },
  { icon: Brain, name: "Ум" },
  { icon: Calculator, name: "Калькулятор" },
  { icon: Beaker, name: "Химия" },
  { icon: Globe, name: "Глобус" },
  { icon: Palette, name: "Палитра" },
];

export {
  floatingIcons,
  recallRatingInfo,
  motivationTexts,
  colorRatingCard,
  levelMotivationVariants,
  availableColors,
  availableIcons,
};
