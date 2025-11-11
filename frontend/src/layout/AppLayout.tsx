import { Outlet, useNavigate } from "react-router-dom";
import Header from "../components/Header";
import ThemeSwithcer from "../components/ThemeSwitcher";
import { DataProvider, useData } from "../context/DataContext";
import { useAuth } from "../context/AuthContext";
import LessonPlayer from "../pages/LessonPlayer";
import ConfrimModal from "../components/modal/ConfrimModal";
import Footer from "../components/Footer";

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

export default function AppLayout() {
    const { isAuthenticated, isLoading } = useAuth();

    // КРИТИЧНО: Показываем loader пока проверяется авторизация
    // Это предотвращает монтирование DataProvider до проверки токена
    if (isLoading) {
        return (
            <div className="w-full h-full min-h-screen flex justify-center items-center bg-base-300">
                <div className="loading loading-dots loading-lg"></div>
            </div>
        );
    }

    // АРХИТЕКТУРНОЕ РЕШЕНИЕ: DataProvider монтируется ТОЛЬКО для авторизованных
    // Это гарантирует, что запросы к API будут выполняться только с валидным токеном
    if (isAuthenticated) {
        return (
            <DataProvider>
                <AppLayoutContent />
            </DataProvider>
        );
    }

    // Для неавторизованных (например, 404 страница) рендерим базовый layout без данных
    // В вашем случае NotFoundPage не нуждается в DataProvider
    return (
        <div className="flex flex-col justify-center items-center min-h-screen bg-base-300">
            <main className="flex-1 w-full max-w-6xl pt-30 px-2 md:px-0">
                <Outlet />
            </main>
            <Footer />
            <ThemeSwithcer />
        </div>
    );
}