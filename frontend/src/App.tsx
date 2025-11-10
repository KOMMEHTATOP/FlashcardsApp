import "./App.css";
import { Routes, Route } from "react-router-dom";
import AppLayout from "./layout/AppLayout";
import { HomePage } from "./pages/Home";
import StudyPage from "./pages/Study";
import LoginPage from "./pages/Login";
import { NotFoundPage } from "./pages/NotFound";
import { PrivateRoute } from "./layout/PrivateRoute";
import { GuestRoute } from "./layout/GuestRoute";
import { lazy } from "react";
import ScrollToTop from "./utils/scrollToTop";

const LandingPage = lazy(() => import("./pages/LandingPage"));

export const DEV = false;

function App() {
    return (
        <Routes>
            <Route path="/about" element={
                <>
                    <ScrollToTop />
                    <LandingPage />
                </>
            } />

            <Route path="/login" element={
                <GuestRoute>
                    <LoginPage />
                </GuestRoute>
            } />

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

                <Route path="*" element={<NotFoundPage />} />
            </Route>
        </Routes>
    );
}

export default App;