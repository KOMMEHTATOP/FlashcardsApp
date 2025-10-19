import { Cookie, Moon, Shell, Sun, X } from "lucide-react";
import { useEffect, useState } from "react";

const themeData = [
  {
    name: "dark",
    icon: <Moon />,
    title: "Темная тема",
    btn: "btn-primary",
  },
  {
    name: "light",
    icon: <Sun />,
    title: "Светлая тема",
    btn: "btn-secondary",
  },
  {
    name: "cupcake",
    icon: <Cookie />,
    title: "Печенька",
    btn: "btn-accent",
  },
  {
    name: "mainbreaker",
    icon: <Shell />,
    title: "Чертеж",
    btn: "btn-info",
  },
];

export default function ThemeSwithcer() {
  const [theme, setTheme] = useState(localStorage.getItem("theme") || "dark");

  useEffect(() => {
    document.documentElement.setAttribute("data-theme", theme);
    localStorage.setItem("theme", theme);
  }, [theme]);

  return (
    <div className="fab fab-flower right-5 bottom-5 md:bottom-10">
      <div
        tabIndex={0}
        role="button"
        className="btn btn-lg btn-info btn-circle"
      >
        {themeData.find((item) => item.name === theme)?.icon}
      </div>

      <button className="fab-main-action btn btn-circle btn-lg btn-success">
        <X />
      </button>

      {themeData.map((item) => (
        <div
          className="tooltip tooltip-left text-base-content"
          data-tip={item.title}
          key={item.name}
          onClick={() => setTheme(item.name)}
        >
          <button className={`btn btn-circle btn-lg btn-soft ${item.btn}`}>
            {item.icon}
          </button>
        </div>
      ))}
    </div>
  );
}
