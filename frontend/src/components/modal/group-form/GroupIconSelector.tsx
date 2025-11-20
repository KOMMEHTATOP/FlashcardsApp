import { motion, AnimatePresence } from "framer-motion";
import { ChevronLeft, ChevronRight } from "lucide-react";
import { useRef, useState, useEffect } from "react";
import { availableIcons } from "../../../test/data";

interface GroupIconSelectorProps {
    selectedIconIndex: number;
    onIconSelect: (index: number) => void;
}

export function GroupIconSelector({
    selectedIconIndex,
    onIconSelect,
}: GroupIconSelectorProps) {
    const iconsContainerRef = useRef<HTMLDivElement>(null);
    const [canScrollLeft, setCanScrollLeft] = useState(false);
    const [canScrollRight, setCanScrollRight] = useState(true);

    const checkScroll = () => {
        if (iconsContainerRef.current) {
            const { scrollLeft, scrollWidth, clientWidth } = iconsContainerRef.current;
            setCanScrollLeft(scrollLeft > 0);
            setCanScrollRight(scrollLeft < scrollWidth - clientWidth - 1);
        }
    };

    useEffect(() => {
        checkScroll();
        window.addEventListener("resize", checkScroll);
        return () => window.removeEventListener("resize", checkScroll);
    }, []);

    const scrollIcons = (direction: "left" | "right") => {
        if (iconsContainerRef.current) {
            const scrollAmount = 300;
            iconsContainerRef.current.scrollBy({
                left: direction === "left" ? -scrollAmount : scrollAmount,
                behavior: "smooth",
            });
        }
    };

    return (
        <div className="space-y-2 relative group/icons">
            <label className="text-sm font-medium text-gray-700 flex justify-between">
                Выберите иконку
                <span className="text-xs text-gray-400 font-normal md:hidden">Свайпайте влево</span>
            </label>

            <div className="relative -mx-2 px-2">
                <AnimatePresence>
                    {canScrollLeft && (
                        <motion.button
                            initial={{ opacity: 0, x: -10 }}
                            animate={{ opacity: 1, x: 0 }}
                            exit={{ opacity: 0, x: -10 }}
                            onClick={() => scrollIcons("left")}
                            className="absolute left-0 top-1/2 -translate-y-1/2 z-10 bg-white/80 backdrop-blur-sm p-2 rounded-full shadow-lg border border-gray-100 hover:bg-white transition-all hidden md:flex"
                        >
                            <ChevronLeft className="w-5 h-5 text-gray-700" />
                        </motion.button>
                    )}
                </AnimatePresence>

                <AnimatePresence>
                    {canScrollRight && (
                        <motion.button
                            initial={{ opacity: 0, x: 10 }}
                            animate={{ opacity: 1, x: 0 }}
                            exit={{ opacity: 0, x: 10 }}
                            onClick={() => scrollIcons("right")}
                            className="absolute right-0 top-1/2 -translate-y-1/2 z-10 bg-white/80 backdrop-blur-sm p-2 rounded-full shadow-lg border border-gray-100 hover:bg-white transition-all hidden md:flex"
                        >
                            <ChevronRight className="w-5 h-5 text-gray-700" />
                        </motion.button>
                    )}
                </AnimatePresence>

                <div
                    ref={iconsContainerRef}
                    onScroll={checkScroll}
                    className="grid grid-rows-3 grid-flow-col gap-3 overflow-x-auto pb-4 pt-1 auto-cols-max pr-2 scrollbar-hide scroll-smooth"
                >
                    {availableIcons.map((item, index) => (
                        <motion.button
                            key={item.name}
                            type="button"
                            onClick={() => onIconSelect(index)}
                            whileHover={{ scale: 1.02 }}
                            whileTap={{ scale: 0.98 }}
                            className={`w-24 h-24 flex flex-col items-center justify-center p-2 rounded-xl border-2 transition-all shrink-0 ${selectedIconIndex === index
                                    ? "border-purple-500 bg-purple-50 shadow-md"
                                    : "border-gray-100 bg-white hover:border-purple-200"
                                }`}
                        >
                            <item.icon
                                className={`w-8 h-8 mb-2 ${selectedIconIndex === index
                                        ? "text-purple-600"
                                        : "text-gray-400"
                                    }`}
                            />
                            <p className={`text-[10px] text-center leading-tight line-clamp-2 w-full ${selectedIconIndex === index ? "text-purple-700 font-medium" : "text-gray-500"
                                }`}>
                                {item.name}
                            </p>
                        </motion.button>
                    ))}
                </div>
            </div>
        </div>
    );
}
