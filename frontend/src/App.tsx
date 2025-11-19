import "./App.css";
import { Routes, Route } from "react-router-dom";
import AppLayout from "./layout/AppLayout";
import { HomePage } from "./pages/Home";
import { ProfilePage } from "./pages/Profile";
import StudyPage from "./pages/Study";
import LoginPage from "./pages/Login";
import { NotFoundPage } from "./pages/NotFound";
import { PrivateRoute } from "./layout/PrivateRoute";
import { GuestRoute } from "./layout/GuestRoute";
import { lazy } from "react";
import ScrollToTop from "./utils/scrollToTop";
import { DataProvider } from "./context/DataContext"; // <--- Не забудь этот импорт!

const LandingPage = lazy(() => import("./pages/LandingPage"));
const PublicStorePage = lazy(() => import("./pages/PublicStore"));
const PublicGroupView = lazy(() => import("./pages/PublicGroupView")); // <--- Импорт новой страницы

export const DEV = false;

function App() {
    return (
        <Routes>
            {/* --- ПУБЛИЧНЫЕ СТРАНИЦЫ --- */}

            <Route path="/about" element={
                <>
                    <ScrollToTop />
                    <LandingPage />
                </>
            } />

            <Route path="/store" element={
                <>
                    <ScrollToTop />
                    <PublicStorePage />
                </>
            } />

            {/* Страница просмотра группы (доступна всем, поэтому оборачиваем в DataProvider вручную) */}
            <Route path="/subscription/:id" element={
                <DataProvider>
                    <ScrollToTop />
                    <PublicGroupView />
                </DataProvider>
            } />

            {/* --------------------------- */}

            <Route path="/login" element={
                <GuestRoute>
                    <LoginPage />
                </GuestRoute>
            } />

            {/* --- ЗАЩИЩЕННЫЕ СТРАНИЦЫ (Только для авторизованных) --- */}
            <Route element={<AppLayout />}>
                <Route
                    path="/"
                    element={
                        <PrivateRoute>
                            <HomePage />
                        </PrivateRoute>
                    }
                />
                <Route
                    path="/profile"
                    element={
                        <PrivateRoute>
                            <ProfilePage />
                        </PrivateRoute>
                    }
                />

                {/* Обрати внимание: Маршрут /subscription/:id отсюда УБРАН. 
                    Теперь он публичный и обрабатывается выше.
                    
                    Маршрут /study/:id остался для режима обучения.
                */}
                <Route
                    path="/study/:id"
                    element={
                        <PrivateRoute>
                            <>
                                <ScrollToTop />
                                <StudyPage />
                            </>
                        </PrivateRoute>
                    }
                />
            </Route>

            <Route path="*" element={<NotFoundPage />} />
        </Routes>
    );
}

export default App;