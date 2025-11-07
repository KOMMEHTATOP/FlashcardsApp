import { Brain, LogOut, UserRound } from "lucide-react";
import { motion } from "framer-motion";
import { Link, useNavigate } from "react-router-dom";
import type { UserData } from "../types/types";
import { TITLE_APP } from "../test/data";

export default function Header({
  user,
  onLogout,
  className,
}: {
  user: UserData | undefined;
  onLogout: () => void;
  className?: string;
}) {
  const navigate = useNavigate();

  return (
    <header
      className={`backdrop-blur-md bg-base-200/20 shadow-lg fixed top-0 z-50 w-full 
                  py-3 px-4 sm:px-6 md:px-10 ${className || ""}`}
    >
      <div className="flex items-center justify-between max-w-7xl mx-auto">
        {/* =левая часть ==== */}
        <motion.div
          initial={{ opacity: 0, x: -20 }}
          animate={{ opacity: 1, x: 0 }}
          transition={{ duration: 0.3 }}
          className="flex items-center gap-3 sm:gap-4"
        >
          <div className="bg-gradient-to-br from-purple-500 to-pink-500 p-2.5 sm:p-3 rounded-2xl flex items-center justify-center">
            <Brain className="h-7 w-7 sm:h-9 sm:w-9 text-white" />
          </div>

          <div className="flex flex-col">
            <Link
              to="/"
              className="text-xl sm:text-3xl font-bold bg-gradient-to-r from-purple-600 to-pink-600 
                         dark:from-purple-400 dark:to-pink-400 bg-clip-text text-transparent leading-none"
            >
              {TITLE_APP}
            </Link>
            <p className=" sm:block text-sm sm:text-base text-base-content/70">
              Учись и развивайся
            </p>
          </div>
        </motion.div>

        {/* =правая часть ==== */}
        <motion.div
          initial={{ opacity: 0, x: 20 }}
          animate={{ opacity: 1, x: 0 }}
          transition={{ duration: 0.3 }}
          className="flex items-center gap-3 sm:gap-4"
        >
          {user ? (
            <>
              {/* 🔹 Аватар и имя */}
              <motion.div
                whileHover={{ scale: 1.05 }}
                className="flex items-center gap-2 bg-base-100/50 backdrop-blur-sm px-3 py-1.5 
                           rounded-xl border border-base-300/30"
              >
                <div className="bg-gradient-to-br from-purple-400 to-pink-400 w-8 h-8 sm:w-9 sm:h-9 rounded-full flex items-center justify-center">
                  <UserRound className="h-4 w-4 sm:h-5 sm:w-5 text-white" />
                </div>
                <span className="hidden sm:inline text-base-content font-medium text-sm sm:text-base">
                  {user?.Login || user?.Email}
                </span>
              </motion.div>

              {/* 🔹 Кнопка выхода */}
              <motion.button
                onClick={onLogout}
                whileHover={{ scale: 1.05 }}
                whileTap={{ scale: 0.95 }}
                className="flex items-center justify-center btn btn-error gap-2
                           bg-gradient-to-r from-red-500 to-pink-700 border-none 
                           text-white shadow-lg hover:shadow-xl transition-all duration-200 group 
                           px-3 py-1.5 sm:px-4 sm:py-2 text-sm"
              >
                <span className="hidden sm:inline">Выйти</span>
                <LogOut className="h-4 w-4 group-hover:rotate-12 transition-transform duration-200" />
              </motion.button>
            </>
          ) : (
            <motion.div
              whileHover={{ scale: 1.05 }}
              whileTap={{ scale: 0.95 }}
              onClick={() => navigate("/login")}
              className="flex items-center gap-2 sm:gap-3 bg-base-100/50 backdrop-blur-sm 
                         px-3 sm:px-4 py-1.5 sm:py-2 rounded-xl border border-base-300/30 
                         cursor-pointer hover:bg-base-100/70 transition-all duration-200"
            >
              <span className="hidden sm:inline text-base-content font-medium">
                Войти
              </span>
              <div className="bg-gradient-to-br from-green-400 to-emerald-500 p-1.5 rounded-full">
                <UserRound className="h-4 w-4 text-white" />
              </div>
            </motion.div>
          )}
        </motion.div>
      </div>
    </header>
  );
}
