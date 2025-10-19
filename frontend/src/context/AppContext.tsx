import React, { createContext, useContext, useEffect, useState } from "react";
import type {
  UserData,
  AchievementsType,
  GroupType,
  GroupCardType,
  ConfrimModalState,
  SettingType,
} from "../types/types";
import apiFetch from "../utils/apiFetch";

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
};

const AppContext = createContext<AppContextType | null>(null);

type CurrentLessonState = {
  group: GroupType;
  cards: GroupCardType[];
  initialIndex: number;
  length: number;
};

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

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      const res = await apiFetch.get("/User/me/dashboard");
      if (res.status === 200) {
        console.log(res.data);
        setUser(res.data);
        setGroups(res.data.Groups);
        await apiFetch
          .get("/StudySettings")
          .then((res) => {
            setSetting(res.data);
          })
          .catch((err) => {
            console.log(err);
          });
      }
    } catch (err) {
    } finally {
      setLoading(false);
    }
  };

  const handleOpenConfrimModal = (modal: ConfrimModalState) => {
    setModalConfrimDetail(modal);
  };

  const handleCloseConfrimModal = () => {
    setModalConfrimDetail(undefined);
  };

  const handleSelectLesson = (
    cards: GroupCardType[],
    group: GroupType,
    index: number = 0
  ) => {
    setCurrentLesson({
      group,
      cards,
      initialIndex: index,
      length: cards.length || 0,
    });
  };

  const logout = async () => {
    await apiFetch.post("/Auth/logout").then((res) => {
      console.log(res);
      setUser(undefined);
      localStorage.removeItem("accessToken");
      window.location.href = "/login";
    });
  };

  const setNewGroups = (newGroup: GroupType) => {
    setGroups((groups) => [newGroup, ...groups]);
  };

  const putGroups = async (group: GroupType) => {
    setGroups((prev) => prev.map((g) => (g.Id === group.Id ? group : g)));
  };

  const deleteGroup = async (id: string) => {
    console.log(id);
    setGroups((prev) => prev.filter((group) => group.Id !== id));
    await apiFetch.delete(`/Group/${id}`).then((res) => console.log(res));
  };

  const deleteCard = async (id: string) => {
    console.log(id);
    await apiFetch.delete(`/Cards/${id}`).then((res) => console.log(res));
  };

  const handleCompliteLesson = () => {
    setCurrentLesson(undefined);
  };

  const questionAnswered = async (id: string, ratting: number) => {
    const res = await apiFetch
      .post(`/Study/record`, { CardId: id, Rating: ratting })
      .then((res) => {
        console.log(res.data);
        const data = res.data;
        setCurrentLesson(
          (prev) =>
            prev && {
              ...prev,
              cards: prev.cards.map((card) =>
                card.CardId === id ? { ...card, LastRating: ratting } : card
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
                TotalXP: data.TotalXP,
                CurrentStreak: data.CurrentStreak,
                XPForNextLevel: data.XPForNextLevel,
              },
            }
        );
        return data.XPEarned;
      })
      .catch((err) => {
        console.log(err);
        return 0;
      });
    return res;
  };

  const value: AppContextType = {
    user,
    setUser,
    logout,
    setting,
    setSetting,

    groups,
    setGroups,
    putGroups,
    setNewGroups,
    handleSelectLesson,
    currentLesson,
    handleCompliteLesson,
    achivment,
    questionAnswered,
    deleteGroup,
    deleteCard,
    handleOpenConfrimModal,
    handleCloseConfrimModal,
    confrimModal,
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
