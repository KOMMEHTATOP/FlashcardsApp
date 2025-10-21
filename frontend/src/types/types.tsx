interface UserData {
  Id: String;
  Login?: String;
  Email: String;
  Statistics: UserState;
  Groups: GroupType[];
}

interface UserState {
  TotalXP: number;
  Level: number;
  XPForNextLevel: number;
  XPProgressInCurrentLevel: number;
  XPRequiredForCurrentLevel: number;
  CurrentStreak: number;
  BestStreak: number;
  TotalStudyTime: String;
  TotalCardsStudied: number;
  TotalCardsCreated: number;
  PerfectRatingsStreak: number;
}

interface SettingType {
  StudyOrder: string;
  MinRating: number;
  MaxRating: number;
  CompletionThreshold: number;
  ShuffleOnRepeat: boolean;
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
  GroupIcon: string;
  CreatedAt: string;
  Order: number;
  CardCount: number;
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
  GroupCardType,
  RatingValue,
  FlashCard,
  AchievementsType,
  GroupType,
  ConfrimModalState,
  SettingType,
};
