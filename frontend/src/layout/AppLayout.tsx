import { Outlet, useNavigate } from "react-router-dom";
import Header from "../components/Header";
import ThemeSwithcer from "../components/ThemeSwitcher";
import { useApp } from "../context/AppContext";
import LessonPlayer from "../pages/LessonPlayer";
import ConfrimModal from "../components/modal/ConfrimModal";
import Footer from "../components/Footer";

export default function AppLayout() {
  const { currentLesson, handleCompliteLesson, user, confrimModal, logout } =
    useApp();

  const navigate = useNavigate();

  if (currentLesson) {
    return (
      <div>
        <LessonPlayer
          lessonTitle={currentLesson?.group.GroupName || ""}
          subjectColor={currentLesson?.group.GroupColor || ""}
          initialIndex={currentLesson.initialIndex}
          onComplete={() => {
            handleCompliteLesson();
            navigate(`/`);
          }}
          onBack={() => {
            handleCompliteLesson();
            navigate(`/`);
          }}
        />
      </div>
    );
  }

  return (
    <div className="flex flex-col justify-center items-center min-h-screen bg-base-300">
      <Header user={user} onLogout={logout} />

      <main className="flex-1 w-full max-w-6xl pt-30 px-2 md:px-0">
        {confrimModal && (
          <ConfrimModal
            text={confrimModal.title}
            target={confrimModal.target}
            handleCancel={confrimModal.handleCancel}
            handleConfirm={confrimModal.handleConfirm}
          />
        )}
        <Outlet />
      </main>

      <Footer />
      <ThemeSwithcer />
    </div>
  );
}
