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
import type { FlashCard } from "../types/types";

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

export { testStudyData, testBadgesData, flashcards };
