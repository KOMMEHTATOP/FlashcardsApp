import React, { createContext, useContext, useEffect, useState } from "react";
import type {
  UserData,
  GroupDetailType,
  UserState,
  AchievementsType,
  GroupType,
  GroupCardType,
  ConfrimModalState,
} from "../types/types";
import apiFetch from "../utils/apiFetch";

type AppContextType = {
  user: UserData | undefined;
  setUser: React.Dispatch<React.SetStateAction<UserData | undefined>>;
  userState: UserState | undefined;
  setUserState: (userState: UserState) => void;
  logout: () => void;

  achivment: AchievementsType[] | undefined;
  userAchivment: AchievementsType[] | undefined;

  groups: GroupType[] | undefined;
  setGroups: React.Dispatch<React.SetStateAction<GroupType[]>>;
  setNewGroups: (newGroup: GroupType) => void;

  handleSelectLesson: (
    subject: GroupCardType[],
    group: GroupDetailType,
    index?: number
  ) => void;

  currentLesson: CurrentLessonState | undefined;

  handleCompliteLesson: () => void;
  questionAnswered: (id: string, ratting: number) => void;

  deleteGroup: (id: string) => void;
  deleteCard: (id: string) => void;

  handleOpenConfrimModal: (modal: ConfrimModalState) => void;
  handleCloseConfrimModal: () => void;
  confrimModal: ConfrimModalState | undefined;
  loading: boolean;
};

const AppContext = createContext<AppContextType | null>(null);

const fetchAllUserData = async () => {
  const [stats, allAch, myAch, groups, setting] = await Promise.all([
    apiFetch.get("/UserStatistics"),
    apiFetch.get("/Achievements/all"),
    apiFetch.get("/Achievements/my"),
    apiFetch.get("/Group"),
    apiFetch.get("/StudySettings"),
  ]);

  return { stats, allAch, myAch, groups, setting };
};

type CurrentLessonState = {
  group: GroupDetailType;
  cards: GroupCardType[];
  initialIndex: number;
  length: number;
};

export function AppProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<UserData>();
  const [userState, setUserState] = useState<UserState>();

  const [currentLesson, setCurrentLesson] = useState<CurrentLessonState>();

  const [achivment, setAchivment] = useState<AchievementsType[]>([]);
  const [userAchivment, setUserAchivment] = useState<AchievementsType[]>([]);
  const [groups, setGroups] = useState<GroupType[]>([]);

  const [isShowModalConfrim, setIsShowModalConfrim] = useState<boolean>(false);
  const [confrimModal, setModalConfrimDetail] = useState<ConfrimModalState>();
  const [loading, setLoading] = useState<boolean>(true);

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      const res = await apiFetch.get("/Auth/me");

      const data = res.data;
      setUser(data);
      if (data) {
        fetchAllUserData().then(({ stats, allAch, myAch, groups, setting }) => {
          setUserState(stats.data);
          setAchivment(allAch.data);
          setUserAchivment(myAch.data);
          setGroups(groups.data);
        });
      }
    } catch (err) {
    } finally {
      setLoading(false);
    }
  };

  const handleOpenConfrimModal = (modal: ConfrimModalState) => {
    setIsShowModalConfrim(true);
    setModalConfrimDetail(modal);
  };

  const handleCloseConfrimModal = () => {
    setIsShowModalConfrim(false);
    setModalConfrimDetail(undefined);
  };

  const handleSelectLesson = (
    cards: GroupCardType[],
    group: GroupDetailType,
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
    await apiFetch.post(`/cards/${id}/ratings`, { Rating: ratting }).then(() =>
      setCurrentLesson(
        (prev) =>
          prev && {
            ...prev,
            cards: prev.cards.map((card) =>
              card.CardId === id ? { ...card, LastRating: ratting } : card
            ),
          }
      )
    );
  };

  const value: AppContextType = {
    user,
    setUser,
    userState,
    setUserState,
    logout,
    groups,
    setGroups,
    setNewGroups,
    handleSelectLesson,
    currentLesson,
    handleCompliteLesson,
    achivment,
    userAchivment,
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
