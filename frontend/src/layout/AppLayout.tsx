import { Outlet, useNavigate } from "react-router-dom";
import { Header } from "@/shared/ui/widgets/Header";
import ThemeSwithcer from "@/shared/ui/ThemeSwitcher";
import { DataProvider, useData } from "@/context/DataContext";
import { useAuth } from "@/context/AuthContext";
import LessonPlayer from "@/pages/LessonPlayer";
import ConfrimModal from "@/shared/ui/modals/ConfirmModal";
import Footer from "@/widgets/Footer";

function AppLayoutContent() {
    const { currentLesson, handleCompliteLesson, user, confrimModal } = useData();
    const { logout } = useAuth();
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

            <main className="flex-1 w-full max-w-6xl pt-30 px-4 sm:px-6 md:px-10">
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

export default function AppLayout() {
    const { isAuthenticated, isLoading } = useAuth();

    if (isLoading) {
        return (
            <div className="w-full h-full min-h-screen flex justify-center items-center bg-base-300">
                <div className="loading loading-dots loading-lg"></div>
            </div>
        );
    }

    if (isAuthenticated) {
        return (
            <DataProvider>
                <AppLayoutContent />
            </DataProvider>
        );
    }

    return (
        <div className="flex flex-col justify-center items-center min-h-screen bg-base-300">
            <main className="flex-1 w-full max-w-6xl pt-30 px-4 sm:px-6 md:px-10">
                <Outlet />
            </main>
            <Footer />
            <ThemeSwithcer />
        </div>
    );
}