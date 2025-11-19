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

const LandingPage = lazy(() => import("./pages/LandingPage"));
const PublicStorePage = lazy(() => import("./pages/PublicStore"));

export const DEV = false;

function App() {
    return (
        <Routes>
            {/* Публичный лендинг */}
            <Route path="/about" element={
                <>
                    <ScrollToTop />
                    <LandingPage />
                </>
            } />

            {/* Публичный каталог (Магазин) */}
            <Route path="/store" element={
                <>
                    <ScrollToTop />
                    <PublicStorePage />
                </>
            } />

            {/* Страница входа (доступна только гостям) */}
            <Route path="/login" element={
                <GuestRoute>
                    <LoginPage />
                </GuestRoute>
            } />

            {/* Защищенная зона приложения */}
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

                <Route
                    path="/subscription/:id"
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

            {/* Страница 404 */}
            <Route path="*" element={<NotFoundPage />} />
        </Routes>
    );
}

export default App;