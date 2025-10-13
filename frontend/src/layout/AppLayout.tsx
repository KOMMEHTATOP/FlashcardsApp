import { Outlet } from "react-router-dom";
import Header from "../components/Header";
import ThemeSwithcer from "../components/ThemeSwitcher";
import { useApp } from "../context/AppContext";
import LessonPlayer from "../pages/LessonPlayer";

export default function AppLayout() {
  const { currentLesson, handleCompliteLesson, user } = useApp();

  if (currentLesson) {
    return (
      <div>
        <LessonPlayer
          lessonTitle={currentLesson.title}
          subjectColor={currentLesson.gradient}
          totalXp={currentLesson.progress}
          onComplete={() => {
            handleCompliteLesson();
          }}
          onBack={() => {
            handleCompliteLesson();
          }}
        />
      </div>
    );
  }

  return (
    <div className="flex flex-col justify-center items-center min-h-screen bg-base-300">
      <Header user={user} />

      <main className="flex-1 w-full max-w-6xl pt-30 px-2 md:px-0">
        <Outlet />
      </main>

      <footer className="footer items-center p-4 bg-base-300 text-base-content justify-center">
        <h1>Footer</h1>
      </footer>
      <ThemeSwithcer />
    </div>
  );
}
