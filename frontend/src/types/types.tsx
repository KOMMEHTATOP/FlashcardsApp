import type { LucideIcon } from "lucide-react";

interface UserData {
  Id: String;
  UserName: String;
  Email: String;
  Login?: String;
}

interface UserState {
  Level: number;
  TotalXP: number;
  XPForNextLevel: number;
  CurrentStreak: number;
  BestStreak: number;
  TotalStudyTime: String;
  XPRequiredForCurrentLevel: number;
  XPProgressInCurrentLevel: number;
}

interface AchievementsType {
  Id: string;
  Description: String;
  IconUrl: String;
  Name: String;
  UUserAchievements?: any;
}

interface GroupType {
  Id: string;
  GroupName: string;
  GroupColor: string;
  CreatedAt: string;
  Order: number;
}

interface GroupDetailType {
  Id: string;
  // icon: LucideIcon;
  GroupName: string;
  GroupColor: string;
  CreatedAt: string;
  Order: number;
  // stats: SubjectStatsType[];
}

interface SubjectStatsType {
  label: string;
  value: string;
}

interface GroupCardType {
  CardId: string;
  GroupId: string;
  Question: string;
  Answer: string;
  LastRating: number;
  completed: boolean;
  UpdatedAt: string;
  CreatedAt: string;
}

type RatingValue = 0 | 1 | 2 | 3 | 4 | 5;

interface FlashCard {
  id: number;
  question: string;
  answer: string;
  hint?: string;
}

type ConfrimModalState = {
  title: string;
  target: string;
  handleCancel: () => void;
  handleConfirm: () => void;
};

export type {
  UserState,
  UserData,
  GroupDetailType,
  GroupCardType,
  RatingValue,
  FlashCard,
  AchievementsType,
  GroupType,
  ConfrimModalState,
};
