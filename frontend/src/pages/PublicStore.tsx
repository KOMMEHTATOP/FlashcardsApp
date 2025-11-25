import { motion } from "framer-motion";
import { StoreTab } from "@/features/dashboard/ui/StoreTab";
import { Header } from "@/shared/ui/widgets/Header";
import Footer from "@/widgets/Footer";
import { useAuth } from "@/context/AuthContext";
import { LandingHelmet } from "@/features/landing/ui/HelmetLanding";
import { DataProvider, useData } from "@/context/DataContext";

// Внутренний компонент, который уже имеет доступ к Context
function PublicStoreContent() {
    // Теперь useData работает, так как мы внутри DataProvider
    // user будет undefined для гостей, или заполнен для авторизованных
    const { user } = useData();
    const { logout } = useAuth();

    return (
        <div className="min-h-screen bg-base-300 flex flex-col">
            <LandingHelmet />

            {/* Header корректно принимает user | undefined */}
            <Header user={user} onLogout={logout} />

            <main className="flex-grow pt-24 px-4 pb-10">
                <div className="max-w-6xl mx-auto">
                    <motion.div
                        initial={{ opacity: 0, y: 20 }}
                        animate={{ opacity: 1, y: 0 }}
                        transition={{ duration: 0.5 }}
                    >
                        <div className="text-center mb-10">
                            <h1 className="text-4xl md:text-5xl font-bold text-base-content mb-4">
                                Библиотека знаний
                            </h1>
                            <p className="text-xl text-base-content/70">
                                Изучайте открытые наборы карточек от нашего сообщества
                            </p>
                        </div>

                        {/* StoreTab внутри себя вызовет useData(), и это сработает */}
                        <StoreTab />
                    </motion.div>
                </div>
            </main>

            <Footer />
        </div>
    );
}

// Экспортируемый компонент-обертка
export default function PublicStorePage() {
    return (
        <DataProvider>
            <PublicStoreContent />
        </DataProvider>
    );
}