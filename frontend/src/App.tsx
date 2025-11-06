import "./App.css";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import AppLayout from "./layout/AppLayout";
import { HomePage } from "./pages/Home";
import StudyPage from "./pages/Study";
import LoginPage from "./pages/Login";
import { NotFoundPage } from "./pages/NotFound";
import { PrivateRoute } from "./layout/PrivateRoute";
import { GuestRoute } from "./layout/GuestRoute";
import LandingPage from "./pages/LandingPage.tsx";

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
        path: "/about",
        element: <LandingPage />,
      },
      {
        path: "*",
        element: <NotFoundPage />,
      },
    ],
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
