type AppContextType = {
  user: UserData | undefined;
  setUser: React.Dispatch<React.SetStateAction<UserData | undefined>>;
  logout: () => void;

  setting: SettingType;
  setSetting: React.Dispatch<React.SetStateAction<SettingType>>;

  achivment: AchievementsType[] | undefined;

  groups: GroupType[] | undefined;
  setGroups: React.Dispatch<React.SetStateAction<GroupType[]>>;
  setNewGroups: (newGroup: GroupType) => void;
  putGroups: (group: GroupType) => void;

  handleSelectLesson: (
    subject: GroupCardType[],
    group: GroupType,
    index?: number
  ) => void;

  currentLesson: CurrentLessonState | undefined;

  handleCompliteLesson: () => void;
  questionAnswered: (id: string, ratting: number) => Promise<number>;

  deleteGroup: (id: string) => void;
  deleteCard: (id: string) => void;

  handleOpenConfrimModal: (modal: ConfrimModalState) => void;
  handleCloseConfrimModal: () => void;
  confrimModal: ConfrimModalState | undefined;
  loading: boolean;

  motivationText: MotivationType | undefined;
};

type CurrentLessonState = {
  group: GroupType;
  cards: GroupCardType[];
  initialIndex: number;
  length: number;
};

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
  Description: string;
  IconUrl: string;
  Name: string;
  Gradient: string;
  IsUnlocked: boolean;
  ConditionValue: number;
  ConditionType: string;
  Rarity: string;
}

interface GroupType {
  Id: string;
  GroupName: string;
  GroupColor: string;
  GroupIcon: string;
  CreatedAt: string;
  Order: number;
  CardCount: number;
  Icon: string;
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

type MotivationType = {
  Icon: string;
  Message: string;
  Type: string;
};

interface QuestionType {
  question: string;
  gradient: string;
}

// Типы для публичного магазина групп
interface PublicGroupDto {
  Id: string;
  GroupName: string;
  GroupColor: string;
  GroupIcon: string | null;
  AuthorName: string;
  CardCount: number;
  SubscriberCount: number;
  CreatedAt: string;
}

// Типы для подписок
interface SubscribedGroupDto {
  Id: string;
  GroupName: string;
  GroupColor: string;
  GroupIcon: string | null;
  AuthorName: string;
  CardCount: number;
  SubscribedAt: string;
}



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
  MotivationType,
  AppContextType,
  CurrentLessonState,
  QuestionType,
  PublicGroupDto,        
  SubscribedGroupDto,    

};
