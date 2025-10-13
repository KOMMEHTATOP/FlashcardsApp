import { Brain, UserRound } from "lucide-react";
import { motion } from "framer-motion";
import { useNavigate } from "react-router-dom";
import type { UserData } from "../types/types";

export default function Header({ user }: { user: UserData | undefined }) {
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
              STUDING
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
          className="flex items-center gap-4"
          onClick={() => navigate("/login")}
        >
          <span>{user?.UserName}</span>
          <UserRound className="h-7 w-7 text-base-content" />
        </motion.div>
      </div>
    </header>
  );
}
