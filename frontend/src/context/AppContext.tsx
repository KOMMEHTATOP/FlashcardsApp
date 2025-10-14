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

const fetchAllUserData = async () => {
  const [user, stats, allAch, myAch, groups, setting] = await Promise.all([
    apiFetch.get("/Auth/me"),
    apiFetch.get("/UserStatistics"),
    apiFetch.get("/Achievements/all"),
    apiFetch.get("/Achievements/my"),
    apiFetch.get("/Group"),
    apiFetch.get("/StudySettings"),
  ]);

  return { user, stats, allAch, myAch, groups, setting };
};

export function AppProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<UserData>();
  const [userState, setUserState] = useState<UserState>();
  const [currentLesson, setCurrentLesson] = useState<SubjectDetailType>();
  const [achivment, setAchivment] = useState<AchievementsType[]>([]);
  const [userAchivment, setUserAchivment] = useState<AchievementsType[]>([]);
  const [groups, setGroups] = useState<GroupType[]>([]);

  useEffect(() => {
    fetchAllUserData().then(
      ({ user, stats, allAch, myAch, groups, setting }) => {
        setUser(user.data);
        setUserState(stats.data);
        setAchivment(allAch.data);
        setUserAchivment(myAch.data);
        setGroups(groups.data);
        console.log("AppProvider, ", user, groups);
      }
    );
  }, []);

  const handleSelectLesson = (subject: SubjectDetailType) => {
    console.log(subject);
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
