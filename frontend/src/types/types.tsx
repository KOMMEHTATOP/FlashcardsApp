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

interface SubjectDetailType {
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

interface SubjectCardType {
  CardId: string;
  CreatedAt: string;
  GroupId: string;
  LastRating: number;
  Question: string;
  UpdatedAt: string;
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
  AchievementsType,
  GroupType,
};
