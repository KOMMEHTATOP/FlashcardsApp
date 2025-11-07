import "./App.css";
import {
  createBrowserRouter,
  Navigate,
  RouterProvider,
} from "react-router-dom";
import AppLayout from "./layout/AppLayout";
import { HomePage } from "./pages/Home";
import StudyPage from "./pages/Study";
import LoginPage from "./pages/Login";
import { NotFoundPage } from "./pages/NotFound";
import { PrivateRoute } from "./layout/PrivateRoute";
import { GuestRoute } from "./layout/GuestRoute";
import { lazy } from "react";
const LandingPage = lazy(() => import("./pages/LandingPage"));

export const DEV = true;

const router = createBrowserRouter([
  {
    element: <AppLayout />,
    children: [
      {
        path: "/",
        element: (
          <PrivateRoute>
            <HomePage />
          </PrivateRoute>
        ),
        errorElement: <Navigate to="/about" replace />,
      },
      {
        path: "/study/:id",
        element: (
          <PrivateRoute>
            <StudyPage />
          </PrivateRoute>
        ),
      },
      {
        path: "*",
        element: <NotFoundPage />,
      },
    ],
  },
  {
    path: "/about",
    element: <LandingPage />,
  },
  {
    path: "/login",
    element: (
      <GuestRoute>
        <LoginPage />
      </GuestRoute>
    ),
  },
]);

function App() {
  return <RouterProvider router={router} />;
}

export default App;
