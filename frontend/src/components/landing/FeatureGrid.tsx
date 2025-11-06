import { useState } from "react";
import FeatureCard from "./FeatureCard";
import type { Feature } from "../../pages/landing/landingContent";

export default function FeaturesGrid({ features }: { features: Feature[] }) {
  const [hovered, setHovered] = useState<boolean>(false);

  return (
    <div className="relative">
      {/* затемнение экрана */}
      <div
        className={`fixed inset-0 bg-black/50 transition-all duration-300 z-10 pointer-events-none ${
          hovered ? "opacity-50" : "opacity-0"
        }`}
      />

      {/* сетка карточек */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 relative z-20">
        {features.map((feature, index) => (
          <div
            key={index}
            className={`group relative ${
              index % 2 === 0 ? " hover:rotate-2" : " hover:-rotate-2"
            } hover:scale-110 transition-all duration-300 ease-in-out`}
            onMouseEnter={() => setHovered(true)}
            onMouseLeave={() => setHovered(false)}
          >
            <FeatureCard
              icon={feature.icon}
              title={feature.title}
              description={feature.description}
              gradient={feature.gradient}
              delay={index * 0.1}
            />
          </div>
        ))}
      </div>
    </div>
  );
}
