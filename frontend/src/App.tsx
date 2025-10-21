import "./App.css";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import AppLayout from "./layout/AppLayout";
import { HomePage } from "./pages/Home";
import AboutPage from "./pages/About";
import StudyPage from "./pages/Study";
import LoginPage from "./pages/Login";
import { PrivateRoute } from "./layout/PrivateRoute";
import { GuestRoute } from "./layout/GuestRoute";

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
        element: <AboutPage />,
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
