import { motion } from "framer-motion";
import { Link } from "react-router-dom";
import { Brain } from "lucide-react";
import { TITLE_APP } from "@/shared/data";

export default function Footer() {
  return (
    <footer className="relative bg-base-300/70 backdrop-blur-md border-t border-base-200 py-10 px-5 mt-10">
      {/* ==== Верхняя подсветка-градиент (декор) ==== */}
      <div className="absolute inset-x-0 top-0 h-[2px] bg-gradient-to-r from-purple-500 via-pink-500 to-purple-500 blur-[1px]" />

      <motion.div
        initial={{ opacity: 0, y: 20 }}
        whileInView={{ opacity: 1, y: 0 }}
        viewport={{ once: true }}
        transition={{ duration: 0.6 }}
        className="max-w-6xl mx-auto text-center flex flex-col items-center justify-center gap-6"
      >
        {/* лого и название  */}
        <motion.div
          whileHover={{ scale: 1.05 }}
          transition={{ type: "spring", stiffness: 200 }}
          className="flex items-center justify-center gap-3"
        >
          <div className="w-10 h-10 rounded-2xl bg-gradient-to-br from-purple-500 to-pink-500 flex items-center justify-center shadow-md shadow-pink-500/20">
            <Brain className="w-6 h-6 text-white" />
          </div>
          <Link
            to="/about"
            className="text-2xl font-bold bg-gradient-to-r from-purple-600 to-pink-600 bg-clip-text text-transparent hover:opacity-90 transition-opacity"
          >
            {TITLE_APP}
          </Link>
        </motion.div>

        {/* контакты */}
        <motion.div
          initial={{ opacity: 0, y: 10 }}
          whileInView={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.2, duration: 0.6 }}
          className="flex flex-col sm:flex-row flex-wrap items-center justify-center gap-3 sm:gap-6 text-base-content/70 text-sm sm:text-base"
        >
          <p className="text-base-content/60">Контакты для обратной связи:</p>

          <div className="flex items-center gap-2">
            <img
              src="/tg_icon.svg" // <--- ИСПРАВЛЕНО (добавлен слэш)
              alt="Telegram"
              className="w-5 h-5"
              loading="lazy"
            />
            <a
              href="https://t.me/aisblack"
              className="hover:text-pink-500 transition-colors"
              target="_blank"
              rel="noopener noreferrer"
            >
              Frontend: @aisblack
            </a>
          </div>

          <div className="flex items-center gap-2">
            <img
              src="/tg_icon.svg"
              alt="Telegram"
              className="w-5 h-5"
              loading="lazy"
            />
            <a
              href="https://t.me/BMBasharov"
              className="hover:text-purple-500 transition-colors"
              target="_blank"
              rel="noopener noreferrer"
            >
              Backend: @BMBasharov
            </a>
          </div>
        </motion.div>

        <motion.p
          initial={{ opacity: 0 }}
          whileInView={{ opacity: 1 }}
          transition={{ delay: 0.4, duration: 1 }}
          className="text-sm text-base-content/50 mt-2"
        >
          © 2025{" "}
          <span className="text-base-content/70 font-medium">{TITLE_APP}</span>.
          Учись и развивайся 💡
        </motion.p>
      </motion.div>
    </footer>
  );
}