import "./App.css";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import AppLayout from "./layout/AppLayout";
import { HomePage } from "./pages/Home";
import AboutPage from "./pages/About";
import StudyPage from "./pages/Study";
import LoginPage from "./pages/Login";

const router = createBrowserRouter([
  {
    element: <AppLayout />,
    children: [
      { path: "/", element: <HomePage /> },
      { path: "/study/:id", element: <StudyPage /> },
      { path: "/about", element: <AboutPage /> },
    ],
  },
  {
    path: "/login",
    element: <LoginPage />,
  },
]);

function App() {
  return <RouterProvider router={router} />;
}

export default App;
