import React, {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useState,
} from "react";
import type {
  UserData,
  AchievementsType,
  GroupType,
  GroupCardType,
  ConfrimModalState,
  SettingType,
  MotivationType,
  AppContextType,
  CurrentLessonState,
} from "../types/types";
import { service } from "../utils/apiService";

const AppContext = createContext<AppContextType | null>(null);

export function AppProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<UserData>();
  const [setting, setSetting] = useState<SettingType>({
    StudyOrder: "Random",
    MinRating: 0,
    MaxRating: 5,
    CompletionThreshold: 0,
    ShuffleOnRepeat: false,
  });

  const [currentLesson, setCurrentLesson] = useState<CurrentLessonState>();

  const [achivment, setAchivment] = useState<AchievementsType[]>([]);
  const [groups, setGroups] = useState<GroupType[]>([]);

  const [confrimModal, setModalConfrimDetail] = useState<ConfrimModalState>();
  const [loading, setLoading] = useState<boolean>(true);

  const [motivationText, setMotivationText] = useState<MotivationType>();

  useEffect(() => {
    withLoading(fetchData);
  }, []);

  const fetchData = useCallback(async () => {
    try {
      const { user, groups, achievements } = await service.getDashboard();
      setUser(user);
      setGroups(groups);
      setAchivment(achievements);
      // подгрузка настроек
      const settings = await service.getSettings();
      setSetting(settings);
      // подгружаем мотивацию
      const motivation = await service.getMotivation();
      setMotivationText(motivation);
    } catch (err) {
      console.error("Ошибка при загрузке данных:", err);
    }
  }, []);

  const withLoading = useCallback(async (callback: () => Promise<void>) => {
    try {
      setLoading(true);
      await callback();
    } finally {
      setLoading(false);
    }
  }, []);

  const handleOpenConfrimModal = useCallback((modal: ConfrimModalState) => {
    setModalConfrimDetail(modal);
  }, []);

  const handleCloseConfrimModal = useCallback(() => {
    setModalConfrimDetail(undefined);
  }, []);

  const handleSelectLesson = useCallback(
    (cards: GroupCardType[], group: GroupType, index: number = 0) => {
      setCurrentLesson({
        group,
        cards,
        initialIndex: index,
        length: cards.length,
      });
    },
    []
  );

  const logout = useCallback(async () => {
    if (!user) return;
    await service.logout();
    setUser(undefined);
  }, [user]);

  const setNewGroups = (newGroup: GroupType) => {
    setGroups((groups) => [newGroup, ...groups]);
  };

  const putGroups = async (group: GroupType) => {
    setGroups((prev) => prev.map((g) => (g.Id === group.Id ? group : g)));
  };

  const deleteGroup = async (id: string) => {
    setGroups((prev) => prev.filter((group) => group.Id !== id));
    await service.deleteGroup(id);
  };

  const deleteCard = async (id: string) => {
    await service.deleteCard(id);
  };

  const handleCompliteLesson = useCallback(() => {
    setCurrentLesson(undefined);
  }, []);

  const questionAnswered = async (CardId: string, Rating: number) => {
    try {
      const data = await service.answerQuestion(CardId, Rating);
      setCurrentLesson(
        (prev) =>
          prev && {
            ...prev,
            cards: prev.cards.map((card) =>
              card.CardId === CardId ? { ...card, LastRating: Rating } : card
            ),
          }
      );
      setUser(
        (prev) =>
          prev && {
            ...prev,
            Statistics: {
              ...prev?.Statistics,
              Level: data.CurrentLevel,
              XPProgressInCurrentLevel: data.CurrentLevelXP,
              XPForNextLevel: data.XPToNextLevel,
              XPRequiredForCurrentLevel: data.XPForNextLevel,
            },
          }
      );
      return data.XPEarned;
    } catch (err) {
      console.log(err);
      return 0;
    }
  };

  const value: AppContextType = {
    // state of user
    user,
    setUser,
    logout,

    // Settings & motivation
    setting,
    setSetting,
    motivationText,

    // groups & lessons
    groups,
    setGroups,
    setNewGroups,
    putGroups,
    currentLesson,
    handleSelectLesson,
    handleCompliteLesson,
    deleteGroup,
    deleteCard,

    // achievements
    achivment,
    questionAnswered,

    // UI state
    confrimModal,
    handleOpenConfrimModal,
    handleCloseConfrimModal,
    loading,
  };

  return <AppContext.Provider value={value}>{children}</AppContext.Provider>;
}

export function useApp() {
  const context = useContext(AppContext);
  if (!context) {
    throw new Error("Не задан контекст App");
  }
  return context;
}
