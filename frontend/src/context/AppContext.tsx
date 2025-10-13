import React, { createContext, useContext, useEffect, useState } from "react";
import type {
  UserData,
  SubjectDetailType,
  UserState,
  AchievementsType,
  GroupType,
} from "../types/types";
import apiFetch from "../utils/apiFetch";

type AppContextType = {
  user: UserData | undefined;
  userState: UserState | undefined;
  achivment: AchievementsType[] | undefined;
  userAchivment: AchievementsType[] | undefined;
  groups: GroupType[] | undefined;
  setUserState: (userState: UserState) => void;
  handleSelectLesson: (subject: SubjectDetailType) => void;
  currentLesson: SubjectDetailType | undefined;
  handleCompliteLesson: () => void;
};

const AppContext = createContext<AppContextType | null>(null);

export function AppProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<UserData>();
  const [userState, setUserState] = useState<UserState>();
  const [currentLesson, setCurrentLesson] = useState<SubjectDetailType>();
  const [achivment, setAchivment] = useState<AchievementsType[]>([]);
  const [userAchivment, setUserAchivment] = useState<AchievementsType[]>([]);
  const [groups, setGroups] = useState<GroupType[]>([]);

  useEffect(() => {
    fetchUserData();
    fetchUserStatistic();
    fetchAcihments();
    fetchUserAchiments();
    fetchGroups();
  }, []);

  const fetchUserData = async () => {
    try {
      const res = await apiFetch.get("/Auth/me", {
        headers: {
          "Content-Type": "application/json",
        },
      });
      if (res.status !== 200) {
        console.log("Ошибка получние данных!");
      }
      setUser(res.data);
    } catch (err) {
      console.log(err);
    }
  };

  const fetchUserStatistic = async () => {
    try {
      const res = await apiFetch.get("/UserStatistics");
      if (res.status !== 200) {
        console.log("Ошибка получние данных!");
      }
      setUserState(res.data);
      // console.log(res.data);
    } catch (err) {
      console.log(err);
    }
  };

  const fetchAcihments = async () => {
    try {
      const res = await apiFetch.get("/Achievements/all");
      if (res.status !== 200) {
        console.log("Ошибка получние данных!");
      }
      setAchivment(res.data);
      console.log("all aschimn ", res.data);
    } catch (err) {
      console.log(err);
    }
  };

  const fetchUserAchiments = async () => {
    try {
      const res = await apiFetch.get("/Achievements/my");
      if (res.status !== 200) {
        console.log("Ошибка получние данных!");
      }

      setUserAchivment(res.data);
      console.log("my aschimn ", res.data);
    } catch (err) {
      console.log(err);
    }
  };

  const fetchGroups = async () => {
    try {
      const res = await apiFetch.get("/Group");
      if (res.status !== 200) {
        console.log("Ошибка получние данных!");
      }
      console.log(res.data);
    } catch (err) {
      console.log(err);
    }
  };

  const handleSelectLesson = (subject: SubjectDetailType) => {
    setCurrentLesson(subject);
  };

  const handleCompliteLesson = () => {
    setCurrentLesson(undefined);
  };

  return (
    <AppContext.Provider
      value={{
        user,
        userState,
        achivment,
        groups,
        setUserState,
        handleSelectLesson,
        currentLesson,
        handleCompliteLesson,
        userAchivment,
      }}
    >
      {children}
    </AppContext.Provider>
  );
}

export function useApp() {
  const context = useContext(AppContext);
  if (!context) {
    throw new Error("Не задан контекст App");
  }
  return context;
}
