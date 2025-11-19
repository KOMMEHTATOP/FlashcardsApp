// ==================== CORE TYPES ====================

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

// ==================== USER ====================

interface UserData {
  Id: string;
  Login?: string;
  Email: string;
  TotalSubscribers?: number;
  MySubscriptions?: SubscribedGroupDto[];
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
  TotalStudyTime: string;
  TotalCardsStudied: number;
  TotalCardsCreated: number;
  PerfectRatingsStreak: number;
}

// ==================== GROUPS ====================

// Своя группа пользователя
interface GroupType {
  Id: string;
  GroupName: string;
  GroupColor: string;
  GroupIcon: string;
  CreatedAt: string;
  Order: number;
  CardCount: number;
  IsPublished?: boolean;
  SubscriberCount?: number;
}

// Публичная группа в магазине
interface PublicGroupDto {
  Id: string;
  GroupName: string;
  GroupColor: string;
  GroupIcon: string | null;
  AuthorName: string;
  CardCount: number;
  SubscriberCount: number;
  CreatedAt: string;
  IsSubscribed: boolean;
}

// Подписка на чужую группу
interface SubscribedGroupDto {
  Id: string;
  GroupName: string;
  GroupColor: string;
  GroupIcon: string | null;
  AuthorName: string;
  CardCount: number;
  SubscribedAt: string;
}

// ==================== CARDS ====================

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

// Для предпросмотра публичных карточек (API response)
interface PublicGroupCardDto {
  CardId: string;
  Question: string;
  Answer: string;
  CreatedAt: string;
}

// ==================== STUDY ====================

type CurrentLessonState = {
  group: GroupType;
  cards: GroupCardType[];
  initialIndex: number;
  length: number;
};

interface StudyHistoryDto {
  CardId: string;
  CardQuestion: string;
  Answer: string;
  Rating: number;
  StudiedAt: string;
  XPEarned: number;
  GroupName: string;
  GroupColor: string;
}

type RatingValue = 0 | 1 | 2 | 3 | 4 | 5;

// ==================== SETTINGS ====================

interface SettingType {
  StudyOrder: string;
  MinRating: number;
  MaxRating: number;
  CompletionThreshold: number;
  ShuffleOnRepeat: boolean;
}

// ==================== ACHIEVEMENTS ====================

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

// ==================== UI ====================

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

// ==================== EXPORTS ====================

export type {
  // Core
  AppContextType,

  // User
  UserState,
  UserData,

  // Groups
  GroupType,
  PublicGroupDto,
  SubscribedGroupDto,

  // Cards
  GroupCardType,
  PublicGroupCardDto,
  RatingValue,

  // Study
  CurrentLessonState,
  StudyHistoryDto,

  // Settings
  SettingType,

  // Achievements
  AchievementsType,

  // UI
  ConfrimModalState,
  MotivationType,
};