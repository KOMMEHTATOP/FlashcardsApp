import { Cookie, Moon, Shell, Sun, Palette } from "lucide-react";
import { useEffect, useState } from "react";

const themeData = [
  {
    name: "dark",
    icon: <Moon className="w-4 h-4" />,
    title: "Темная",
  },
  {
    name: "light",
    icon: <Sun className="w-4 h-4" />,
    title: "Светлая",
  },
  {
    name: "cupcake",
    icon: <Cookie className="w-4 h-4" />,
    title: "Печенька",
  },
  {
    name: "mainbreaker",
    icon: <Shell className="w-4 h-4" />,
    title: "Чертеж",
  },
];

export default function ThemeSwitcher() {
  // Инициализируем тему из localStorage или берем дефолтную
  const [theme, setTheme] = useState(localStorage.getItem("theme") || "dark");

  useEffect(() => {
    document.documentElement.setAttribute("data-theme", theme);
    localStorage.setItem("theme", theme);
  }, [theme]);

  // Находим иконку текущей темы
  const currentThemeIcon = themeData.find((item) => item.name === theme)?.icon || <Palette className="w-4 h-4" />;

  return (
      // dropdown-end выравнивает меню по правому краю, чтобы оно не уходило за экран
      // dropdown-bottom (по умолчанию) заставляет меню выпадать вниз
      <div className="dropdown dropdown-end">
        <div
            tabIndex={0}
            role="button"
            className="btn btn-ghost btn-circle btn-sm md:btn-md"
            aria-label="Сменить тему"
        >
          {/* Текущая иконка */}
          {currentThemeIcon}
        </div>

        <ul
            tabIndex={0}
            className="dropdown-content z-[1] menu p-2 shadow-xl bg-base-200 rounded-box w-40 mt-2 border border-base-300"
        >
          {themeData.map((item) => (
              <li key={item.name}>
                <button
                    onClick={() => setTheme(item.name)}
                    className={`flex items-center gap-2 ${theme === item.name ? "active bg-primary text-primary-content" : ""}`}
                >
                  {item.icon}
                  <span>{item.title}</span>
                </button>
              </li>
          ))}
        </ul>
      </div>
  );
}