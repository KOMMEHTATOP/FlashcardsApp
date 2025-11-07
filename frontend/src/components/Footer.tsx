import { Brain } from "lucide-react";
import { Link } from "react-router-dom";

export default function Footer() {
  return (
    <footer className="bg-base-300 py-8 px-4">
      <div className="max-w-6xl mx-auto text-center">
        <div className="flex items-center justify-center gap-2 mb-4">
          <div className="w-8 h-8 rounded-xl bg-gradient-to-br from-purple-400 to-pink-500 flex items-center justify-center">
            <Brain className="w-5 h-5 text-white" />
          </div>
          <Link to="/about" className="text-xl font-bold text-base-content">
            FlashcardsLoop
          </Link>
        </div>
        <p className="text-base-content/60">
          © 2025 FlashcardsLoop. Учись и развивайся.
        </p>
        <div className="space-x-2 flex justify-center text-base-content/60 items-center mt-2">
          <h2 className="">Контакты для обратной связи: </h2>
          <div className="flex items-center justify-center gap-1">
            <img src="tg_icon.svg" alt="" className="w-5 h-5" loading="lazy" />
            <a href="https://t.me/aisblack" className="hover:underline">
              Frontend: @aisblack
            </a>
          </div>
          <div className="flex items-center justify-center gap-1">
            <img src="tg_icon.svg" alt="" className="w-5 h-5" loading="lazy" />
            <a
              href="https://t.me/BMBasharov"
              className="hover:underline"
              target="_blank"
            >
              Backend: @BMBasharov
            </a>
          </div>
        </div>
      </div>
    </footer>
  );
}
