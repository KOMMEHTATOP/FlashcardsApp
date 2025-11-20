import "./App.css";
import { Routes, Route } from "react-router-dom";
import AppLayout from "./layout/AppLayout";
import { HomePage } from "./pages/Home";
import { ProfilePage } from "./pages/Profile";
import OwnerGroupPage from "./pages/OwnerGroupPage"; 

import LoginPage from "./pages/Login";
import { NotFoundPage } from "./pages/NotFound";
import { PrivateRoute } from "./layout/PrivateRoute";
import { GuestRoute } from "./layout/GuestRoute";
import { lazy } from "react";
import ScrollToTop from "./utils/scrollToTop";
import { DataProvider } from "./context/DataContext";

const LandingPage = lazy(() => import("./pages/LandingPage"));
const PublicStorePage = lazy(() => import("./pages/PublicStore"));
const AdminPage = lazy(() => import("./pages/AdminPage"));
const SubscriberGroupPage = lazy(() => import("./pages/SubscriberGroupPage")); // <--- ДОБАВЛЯЕМ НОВЫЙ (ПОДПИСЧИК)

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

            <Route path="/store" element={
                <>
                    <ScrollToTop />
                    <PublicStorePage />
                </>
            } />
            
            <Route path="/subscription/:id" element={
                <DataProvider>
                    <ScrollToTop />
                    <SubscriberGroupPage />
                </DataProvider>
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
                                <OwnerGroupPage />
                            </>
                        </PrivateRoute>
                    }
                />

                {/* Админка */}
                <Route
                    path="/admin"
                    element={
                        <PrivateRoute>
                            <AdminPage />
                        </PrivateRoute>
                    }
                />
                
            </Route>

            <Route path="*" element={<NotFoundPage />} />
        </Routes>
    );
}

export default App;