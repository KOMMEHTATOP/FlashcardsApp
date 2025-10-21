import { Brain, LogOut, UserRound } from "lucide-react";
import { motion } from "framer-motion";
import { useNavigate } from "react-router-dom";
import type { UserData } from "../types/types";
import { TITLE_APP } from "../test/data";

export default function Header({
  user,
  onLogout,
}: {
  user: UserData | undefined;
  onLogout: () => void;
}) {
  const navigate = useNavigate();

  return (
    <header className="navbar backdrop-blur-md bg-base-200/20 shadow-lg py-3 items-center justify-center fixed top-0 z-50">
      <div className="flex-1 flex max-w-6xl">
        <motion.div
          initial={{ opacity: 0, x: -20 }}
          animate={{ opacity: 1, x: 0 }}
          transition={{ duration: 0.3 }}
          className="flex items-center gap-2"
        >
          <div className="bg-gradient-to-br from-purple-500 to-pink-500 p-2 rounded-2xl items-center justify-center flex">
            <Brain className="h-8 w-8 text-white" />
          </div>
          <div className="flex flex-col ml-2">
            <h1 className="text-2xl bg-gradient-to-r from-purple-600 to-pink-600 dark:from-purple-400 dark:to-pink-400 bg-clip-text text-transparent text-title">
              {TITLE_APP}
            </h1>
            <p className="text-md text-base-content/70 text-subtitle">
              Учись и развивайся
            </p>
          </div>
        </motion.div>
      </div>
      <div className="flex-none">
        <motion.div
          initial={{ opacity: 0, x: 20 }}
          animate={{ opacity: 1, x: 0 }}
          transition={{ duration: 0.3 }}
        >
          {user ? (
            <div className="flex items-center gap-4">
              {/* Аватар пользователя */}
              <motion.div
                whileHover={{ scale: 1.05 }}
                className="flex items-center gap-3 bg-base-100/50 backdrop-blur-sm px-4 py-2 rounded-2xl border border-base-300/30"
              >
                <div className="bg-gradient-to-br from-purple-400 to-pink-400 w-8 h-8 rounded-full flex items-center justify-center">
                  <UserRound className="h-4 w-4 text-white" />
                </div>
                <span className="text-base-content font-medium">
                  {user?.Login || user?.Email}
                </span>
              </motion.div>

              {/* Кнопка выхода */}
              <motion.button
                onClick={onLogout}
                whileHover={{ scale: 1.05 }}
                whileTap={{ scale: 0.95 }}
                className="flex btn btn-error items-center gap-2 justify-center 
                         bg-gradient-to-r from-red-500 to-pink-700  
                         border-none text-white shadow-lg hover:shadow-xl 
                         transition-all duration-200 group"
              >
                <span>Выйти</span>
                <motion.div
                  initial={{ x: 0 }}
                  whileHover={{ x: 2 }}
                  transition={{ type: "spring", stiffness: 400 }}
                >
                  <LogOut className="h-4 w-4 group-hover:rotate-12 transition-transform duration-200" />
                </motion.div>
              </motion.button>
            </div>
          ) : (
            <motion.div
              whileHover={{ scale: 1.05 }}
              whileTap={{ scale: 0.95 }}
              onClick={() => navigate("/login")}
              className="flex items-center gap-3 bg-base-100/50 backdrop-blur-sm 
                       px-4 py-2 rounded-2xl border border-base-300/30 
                       cursor-pointer hover:bg-base-100/70 transition-all duration-200"
            >
              <span className="text-base-content font-medium">Войти</span>
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
