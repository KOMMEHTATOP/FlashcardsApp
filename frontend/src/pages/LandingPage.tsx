import { AnimatePresence, motion } from "framer-motion";
import { ArrowUpFromDotIcon } from "lucide-react";
import { useEffect, useMemo, useState } from "react";
import Footer from "../components/Footer";
import FeaturesGrid from "../components/landing/FeatureGrid";
import { LandingHelmet } from "../components/landing/HelmetLanding";
import { LandingBenefits } from "../components/landing/sections/LandingBenefits";
import { LandingCTA } from "../components/landing/sections/LandingCTA";
import { LandingGamification } from "../components/landing/sections/LandingGamification";
import { LandingHero } from "../components/landing/sections/LandingHero";
import { LandingHowItWorks } from "../components/landing/sections/LandingHowItWorks";
import { useAuth } from "../context/AuthContext";
import { Header } from "../shared/ui/widgets/Header";
import { LandingContentService } from "./landing/landingContent";

export default function LandingPage() {
    const { logout } = useAuth();
    const features = useMemo(() => LandingContentService.getFeatures(), []);
    const gamificationFeatures = useMemo(
        () => LandingContentService.getGamificationFeatures(),
        []
    );
    const steps = useMemo(() => LandingContentService.getSteps(), []);
    const stats = useMemo(() => LandingContentService.getStats(), []);
    const benefits = useMemo(() => LandingContentService.getBenefits(), []);

    const [isMobile, setIsMobile] = useState<boolean>(false);
    const [showScrollButton, setShowScrollButton] = useState<boolean>(false);

    useEffect(() => {
        const handleResize = () => {
            setIsMobile(window.innerWidth <= 768);
        };
        handleResize();
        window.addEventListener("resize", handleResize);
        return () => {
            window.removeEventListener("resize", handleResize);
        };
    }, []);

    useEffect(() => {
        const handleScrollVisibility = () => {
            if (window.scrollY > 200) {
                setShowScrollButton(true);
            } else {
                setShowScrollButton(false);
            }
        };

        window.addEventListener("scroll", handleScrollVisibility);
        return () => window.removeEventListener("scroll", handleScrollVisibility);
    }, []);

    const handleScroll = (name: string) => {
        const targetSection = document.getElementById(name);
        if (targetSection) {
            targetSection.scrollIntoView({ behavior: "smooth" });
        }
    };

    return (
        <div className="min-h-screen bg-base-300">
            <LandingHelmet />

            <Header user={undefined} onLogout={logout} />
            <AnimatePresence>
                {showScrollButton && (
                    <motion.div
                        key="scroll-button"
                        initial={{ opacity: 0, y: 50 }}
                        animate={{ opacity: 1, y: 0 }}
                        exit={{ opacity: 0, y: 50 }}
                        transition={{ duration: 0.3 }}
                        className="fixed bottom-5 right-4 z-50"
                    >
                        <motion.button
                            whileHover={{ scale: 1.15 }}
                            whileTap={{ scale: 0.95 }}
                            onClick={() => handleScroll("top")}
                            className="p-3 rounded-full bg-info text-white 
                   shadow-lg hover:bg-info/90 
                   transition-all duration-300 
                   opacity-80 hover:opacity-100"
                        >
                            <ArrowUpFromDotIcon className="w-7 h-7" />
                        </motion.button>
                    </motion.div>
                )}
            </AnimatePresence>

            {/* --- HERO SECTION --- */}
            <LandingHero isMobile={isMobile} stats={stats} />

            {/* --- FEATURES SECTION --- */}
            <section id="features" className="py-24 px-4 bg-base-200">
                <div className="max-w-6xl mx-auto">
                    <motion.div
                        initial={{ opacity: 0, y: 20 }}
                        whileInView={{ opacity: 1, y: 0 }}
                        viewport={{ once: true }}
                        className="text-center mb-16"
                    >
                        <h2 className="text-4xl md:text-5xl font-bold text-base-content mb-4">
                            Основные возможности
                        </h2>
                        <p className="text-xl text-base-content/70">
                            Всё что нужно для эффективного обучения
                        </p>
                    </motion.div>
                    <FeaturesGrid features={features} />
                </div>
            </section>

            {/* --- GAMIFICATION SECTION --- */}
            <LandingGamification features={gamificationFeatures} isMobile={isMobile} />

            {/* --- HOW IT WORKS SECTION --- */}
            <LandingHowItWorks steps={steps} />

            {/* --- BENEFITS SECTION --- */}
            <LandingBenefits benefits={benefits} />

            {/* --- CTA SECTION --- */}
            <LandingCTA />

            <Footer />
        </div>
    );
}