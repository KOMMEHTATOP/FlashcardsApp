import {
  Star,
  Flame,
  BookOpen,
  Calculator,
  Beaker,
  Globe,
  Target,
  Medal,
  Crown,
  Rocket,
} from "lucide-react";

interface FlashCard {
  id: number;
  question: string;
  answer: string;
  hint?: string;
}

interface QuestionType {
  question: string;
  gradient: string;
}


const testStudyData = [
  {
    id: 1,
    icon: BookOpen,
    title: "Литература",
    progress: 75,
    streak: 5,
    gradient: "from-purple-500 to-pink-500",
    stats: [
      { label: "Уроки завершены", value: "3/6" },
      { label: "Общее количество заработанного опыта", value: "225" },
      { label: "Потраченное время", value: "55 мин" },
      { label: "Текущая полоса", value: `${(3 / 6) * 100} дней` },
    ],
  },
  {
    id: 2,
    icon: Calculator,
    title: "Математика",
    progress: 60,
    streak: 3,
    gradient: "from-blue-500 to-cyan-500",
    stats: [
      { label: "Уроки завершены", value: "1/6" },
      { label: "Общее количество заработанного опыта", value: "50" },
      { label: "Потраченное время", value: "25 мин" },
      { label: "Текущая полоса", value: `${(3 / 6) * 2} дней` },
    ],
  },
  {
    id: 3,
    icon: Beaker,
    title: "Химия",
    progress: 85,
    streak: 7,
    gradient: "from-green-500 to-emerald-500",
    stats: [
      { label: "Уроки завершены", value: "6/6" },
      { label: "Общее количество заработанного опыта", value: "100" },
      { label: "Потраченное время", value: "60 мин" },
      { label: "Текущая полоса", value: `${(3 / 6) * 110} дней` },
    ],
  },
  {
    id: 4,
    icon: Globe,
    title: "География",
    progress: 40,
    streak: 0,
    gradient: "from-orange-500 to-yellow-500",
    stats: [
      { label: "Уроки завершены", value: "5/6" },
      { label: "Общее количество заработанного опыта", value: "600" },
      { label: "Потраченное время", value: "30 мин" },
      { label: "Текущая полоса", value: `${(3 / 6) * 50} дней` },
    ],
  },
];

const testBadgesData = [
  {
    id: 1,
    icon: Flame,
    title: "7 Дней подряд",
    description: "Занимайтесь 7 дней подряд",
    earned: true,
    gradient: "from-orange-400 to-red-500",
  },
  {
    id: 2,
    icon: Star,
    title: "Первые шаги",
    description: "Завершите свой первый урок",
    earned: true,
    gradient: "from-yellow-400 to-orange-500",
  },
  {
    id: 3,
    icon: Target,
    title: "Высший балл",
    description: "Получите 100% выигрыша в викторине",
    earned: true,
    gradient: "from-green-400 to-emerald-500",
  },
  {
    id: 4,
    icon: Medal,
    title: "Быстро обучающийся",
    description: "Пройдите 10 уроков за один день",
    earned: false,
    gradient: "from-blue-400 to-purple-500",
  },
  {
    id: 5,
    icon: Crown,
    title: "Король знаний",
    description: "Достигните 10-го уровня",
    earned: false,
    gradient: "from-purple-400 to-pink-500",
  },
  {
    id: 6,
    icon: Rocket,
    title: "Восходящая звезда",
    description: "Заработайте 1000 очков опыта",
    earned: false,
    gradient: "from-cyan-400 to-blue-500",
  },
];

const flashcards: FlashCard[] = [
  {
    id: 1,
    question: "Столица Франции?",
    answer: "Париж",
    hint: "Известен как Город огней",
  },
  {
    id: 2,
    question: "Сколько будет 7 × 8?",
    answer: "56",
    hint: "Вспомни таблицу умножения",
  },
  {
    id: 3,
    question: "Какая самая большая планета в Солнечной системе?",
    answer: "Юпитер",
    hint: "Это газовый гигант",
  },
  {
    id: 4,
    question: "Кто написал «Мону Лизу»?",
    answer: "Леонардо да Винчи",
    hint: "Итальянский художник эпохи Возрождения",
  },
  {
    id: 5,
    question: "Химический символ золота?",
    answer: "Au",
    hint: "От латинского слова «aurum»",
  },
  {
    id: 6,
    question: "Какая самая длинная река в мире?",
    answer: "Нил",
    hint: "Течёт через Египет",
  },
  {
    id: 7,
    question: "Кто изобрёл лампочку?",
    answer: "Томас Эдисон",
    hint: "Американский изобретатель и предприниматель",
  },
  {
    id: 8,
    question: "Сколько континентов на Земле?",
    answer: "7",
    hint: "Европа, Азия, Африка...",
  },
];

const questions: QuestionType[] = [
  { question: "Столица Франции?", gradient: "from-orange-500 to-yellow-500" },
  { question: "Сколько будет 7 × 8?", gradient: "from-blue-500 to-cyan-500" },
  {
    question: "Какая самая большая планета в Солнечной системе?",
    gradient: "from-purple-500 to-pink-500",
  },
  {
    question: "Сколько континентов на Земле?",
    gradient: "from-green-500 to-teal-500",
  },
  {
    question: "Какой химический символ у воды?",
    gradient: "from-red-500 to-rose-500",
  },
  {
    question: "Кто написал 'Войну и мир'?",
    gradient: "from-indigo-500 to-violet-500",
  },
  {
    question: "Сколько сторон у треугольника?",
    gradient: "from-yellow-500 to-amber-500",
  },
  {
    question: "Какая планета ближе всего к Солнцу?",
    gradient: "from-pink-500 to-fuchsia-500",
  },
  { question: "Сколько дней в году?", gradient: "from-cyan-500 to-sky-500" },
  {
    question: "Какой цвет у неба в ясный день?",
    gradient: "from-teal-500 to-emerald-500",
  },
  {
    question: "Сколько пальцев на одной руке?",
    gradient: "from-rose-500 to-red-500",
  },
  {
    question: "Какая валюта в Японии?",
    gradient: "from-violet-500 to-purple-500",
  },
  {
    question: "Кто изобрёл телефон?",
    gradient: "from-amber-500 to-orange-500",
  },
  {
    question: "Сколько часов в сутках?",
    gradient: "from-fuchsia-500 to-pink-500",
  },
  {
    question: "Какая самая высокая гора на Земле?",
    gradient: "from-sky-500 to-blue-500",
  },
  {
    question: "Сколько будет 10 + 15?",
    gradient: "from-emerald-500 to-green-500",
  },
  {
    question: "Какой газ мы вдыхаем?",
    gradient: "from-lime-500 to-yellow-500",
  },
  {
    question: "Сколько месяцев в году?",
    gradient: "from-slate-500 to-gray-500",
  },
  {
    question: "Какая столица России?",
    gradient: "from-zinc-500 to-neutral-500",
  },
  {
    question: "Сколько будет 9 × 9?",
    gradient: "from-stone-500 to-warmgray-500",
  },
  {
    question: "Сколько планет в Солнечной системе?",
    gradient: "from-gray-600 to-slate-800",
  },
  {
    question: "Какой металл плавится в руке?",
    gradient: "from-neutral-600 to-zinc-800",
  },
  {
    question: "Сколько углов у квадрата?",
    gradient: "from-warmgray-600 to-stone-800",
  },
  {
    question: "Какая птица не летает?",
    gradient: "from-orange-600 to-red-700",
  },
  { question: "Сколько будет 5 × 12?", gradient: "from-teal-600 to-cyan-700" },
  {
    question: "Как называется наша галактика?",
    gradient: "from-indigo-600 to-blue-800",
  },
  {
    question: "Сколько секунд в минуте?",
    gradient: "from-emerald-600 to-green-700",
  },
  {
    question: "Какая столица Казахстана?",
    gradient: "from-yellow-600 to-amber-700",
  },
];

export { testStudyData, testBadgesData, flashcards, questions };
