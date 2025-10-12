import React, { createContext, useContext, useEffect, useState } from "react";
import type { SubjectDetailType, UserState } from "../types/types";

type AppContextType = {
  userState: UserState | undefined;
  setUserState: (userState: UserState) => void;
  handleSelectLesson: (subject: SubjectDetailType) => void;
  currentLesson: SubjectDetailType | undefined;
  handleCompliteLesson: () => void;
};

const AppContext = createContext<AppContextType | null>(null);

export function AppProvider({ children }: { children: React.ReactNode }) {
  const [userState, setUserState] = useState<UserState>();
  const [currentLesson, setCurrentLesson] = useState<SubjectDetailType>();

  useEffect(() => {
    const testData = {
      level: 1,
      currentXP: 50,
      xpToNextLevel: 1000,
    };
    if (!userState) {
      setUserState(testData);
    }
  }, []);

  const handleSelectLesson = (subject: SubjectDetailType) => {
    setCurrentLesson(subject);
  };

  const handleCompliteLesson = () => {
    setCurrentLesson(undefined);
  };

  return (
    <AppContext.Provider
      value={{
        userState,
        setUserState,
        handleSelectLesson,
        currentLesson,
        handleCompliteLesson,
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
