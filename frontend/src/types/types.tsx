import type { LucideIcon } from "lucide-react";

interface UserData {
  login: String;
  email: String;
}

interface UserState {
  level: number;
  currentXP: number;
  xpToNextLevel: number;
}

interface SubjectDetailType {
  id: number;
  icon: LucideIcon;
  title: string;
  progress: number;
  streak: number;
  gradient: string;
  stats: SubjectStatsType[];
}

interface SubjectStatsType {
  label: string;
  value: string;
}

interface SubjectCardType {
  id: number;
  title: String;
  duration: number;
  xp: number;
  completed: boolean;
}

type RatingValue = 0 | 1 | 2 | 3 | 4 | 5;

interface FlashCard {
  id: number;
  question: string;
  answer: string;
  hint?: string;
}

export type {
  UserState,
  UserData,
  SubjectDetailType,
  SubjectCardType,
  RatingValue,
  FlashCard,
};
