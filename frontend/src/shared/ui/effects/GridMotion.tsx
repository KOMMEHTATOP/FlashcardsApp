import { useEffect, useRef, type FC } from "react";
import { gsap } from "gsap";

interface GridItem {
  question: string;
  gradient: string;
}

interface GridMotionProps {
  items?: GridItem[];
  gradientColor?: string;
  isMobile?: boolean;
}

const GridMotion: FC<GridMotionProps> = ({
  items = [],
  gradientColor = "black",
  isMobile = false,
}) => {
  const gridRef = useRef<HTMLDivElement>(null);
  const rowRefs = useRef<(HTMLDivElement | null)[]>([]);
  const mouseXRef = useRef<number>(window.innerWidth / 2);

  const totalItems = 28;

  const defaultItems: GridItem[] = Array.from(
    { length: totalItems },
    (_, index) => ({
      question: `Item ${index + 1}`,
      gradient: "from-gray-500 to-gray-700",
    })
  );

  const combinedItems: GridItem[] =
    items.length > 0 ? items.slice(0, totalItems) : defaultItems;

  useEffect(() => {
    gsap.ticker.lagSmoothing(0);

    let autoTweens: gsap.core.Tween[] = [];
    let lastMouseMove = Date.now();

    const handleMouseMove = (e: MouseEvent): void => {
      mouseXRef.current = e.clientX;
      lastMouseMove = Date.now();

      autoTweens.forEach((tween) => tween.kill());
      autoTweens = [];
    };

    const startAutoMotion = () => {
      if (autoTweens.length > 0) return;

      const maxDrift = 80;
      const duration = 8 + Math.random() * 4;

      rowRefs.current.forEach((row, index) => {
        if (row) {
          const direction = index % 2 === 0 ? 2 : -2;
          const targetX = (Math.random() - 0.5) * maxDrift * direction;

          const tween = gsap.to(row, {
            x: targetX,
            duration: duration,
            ease: "sine.inOut",
            onComplete: () => {
              autoTweens = autoTweens.filter((t) => t !== tween);
            },
          });

          autoTweens.push(tween);
        }
      });
    };

    const updateMotion = (): void => {
      const now = Date.now();
      const idleTime = now - lastMouseMove;

      if (idleTime > 1500 && autoTweens.length === 0) {
        startAutoMotion();
      }

      if (idleTime < 300) {
        const maxMoveAmount = 300;
        const baseDuration = 0.8;
        const inertiaFactors = [0.6, 0.4, 0.3, 0.2];

        rowRefs.current.forEach((row, index) => {
          if (row) {
            const direction = index % 2 === 0 ? 1 : -1;
            const moveAmount =
              ((mouseXRef.current / window.innerWidth) * maxMoveAmount -
                maxMoveAmount / 2) *
              direction;

            gsap.to(row, {
              x: moveAmount,
              duration:
                baseDuration + inertiaFactors[index % inertiaFactors.length],
              ease: "power3.out",
              overwrite: "auto",
            });
          }
        });
      }
    };

    const removeAnimationLoop = gsap.ticker.add(updateMotion);
    window.addEventListener("mousemove", handleMouseMove);

    setTimeout(() => {
      if (Date.now() - lastMouseMove > 100 && autoTweens.length === 0) {
        startAutoMotion();
      }
    }, 100);

    return () => {
      window.removeEventListener("mousemove", handleMouseMove);
      removeAnimationLoop();
      autoTweens.forEach((tween) => tween.kill());
    };
  }, []);

  return (
    <div ref={gridRef} className="h-full w-full overflow-hidden">
      <section
        className="w-full h-screen overflow-hidden relative flex items-center justify-center"
        style={{
          background: `radial-gradient(circle, ${gradientColor} 0%, transparent 100%)`,
        }}
      >
        <div className="absolute inset-0 pointer-events-none z-[4] bg-[length:250px]"></div>
        <div className="gap-4 flex-none relative w-[150vw] h-[150vh] grid grid-rows-4 grid-cols-1 rotate-[-15deg] origin-center z-[2]">
          {Array.from({ length: 4 }, (_, rowIndex) => (
            <div
              key={rowIndex}
              className="grid gap-4 grid-cols-3 md:grid-cols-7"
              style={{ willChange: "transform, filter" }}
              ref={(el) => {
                if (el) rowRefs.current[rowIndex] = el;
              }}
            >
              {Array.from({ length: isMobile ? 3 : 7 }, (_, itemIndex) => {
                const content = combinedItems[rowIndex * 7 + itemIndex];
                return (
                  <div key={itemIndex} className="relative">
                    <div
                      className={`relative w-full h-full overflow-hidden rounded-[10px] bg-gradient-to-r ${
                        content?.gradient || "from-gray-500 to-gray-700"
                      } flex items-center justify-center text-white text-[1.5rem]`}
                    >
                      <div className="p-4 text-center z-[1]">
                        {content?.question}
                      </div>
                    </div>
                  </div>
                );
              })}
            </div>
          ))}
        </div>
        <div className="relative w-full h-full top-0 left-0 pointer-events-none"></div>
      </section>
    </div>
  );
};

export default GridMotion;
