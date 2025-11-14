import { motion, AnimatePresence } from "framer-motion";
import { X, BookOpen } from "lucide-react";
import { useEffect, useState, useRef } from "react";
import apiFetch from "../../utils/apiFetch";
import type { PublicGroupCardDto } from "../../types/types";
import { Button } from "../../shared/ui/Button";

interface GroupPreviewModalProps {
    isOpen: boolean;
    onClose: () => void;
    groupId: string;
    groupName: string;
    gradient: string;
    onSubscribe: () => void;
}

export default function GroupPreviewModal({
                                              isOpen,
                                              onClose,
                                              groupId,
                                              groupName,
                                              gradient,
                                              onSubscribe,
                                          }: GroupPreviewModalProps) {
    const [cards, setCards] = useState<PublicGroupCardDto[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const loadingRef = useRef(false);  // Guard против двойной загрузки

    useEffect(() => {
        if (!isOpen) {
            // Сбрасываем состояние при закрытии
            setCards([]);
            setError(null);
            loadingRef.current = false;
            return;
        }

        if (!groupId) return;

        const controller = new AbortController();

        const loadCards = async () => {
            // Guard: если уже загружаем, не запускаем повторно
            if (loadingRef.current) return;

            loadingRef.current = true;
            setLoading(true);
            setError(null);

            try {
                const response = await apiFetch.get(
                    `/Subscriptions/public/${groupId}/cards`,
                    { signal: controller.signal }
                );

                if (!controller.signal.aborted) {
                    setCards(response.data);
                }
            } catch (err: any) {
                if (err.name === 'AbortError' || err.name === 'CanceledError') {
                    return;
                }
                console.error("Ошибка загрузки карточек:", err);
                setError(err.response?.data?.errors?.[0] || "Не удалось загрузить карточки");
            } finally {
                if (!controller.signal.aborted) {
                    setLoading(false);
                    loadingRef.current = false;
                }
            }
        };

        loadCards();

        return () => {
            controller.abort();
            loadingRef.current = false;
        };
    }, [isOpen, groupId]);

    const handleSubscribe = () => {
        onSubscribe();
        onClose();
    };

    if (!isOpen) return null;

    return (
        <AnimatePresence>
            {isOpen && (
                <>
                    {/* Backdrop */}
                    <motion.div
                        initial={{ opacity: 0 }}
                        animate={{ opacity: 1 }}
                        exit={{ opacity: 0 }}
                        onClick={onClose}
                        className="fixed inset-0 bg-black/50 z-40 backdrop-blur-sm"
                    />

                    {/* Modal */}
                    <motion.div
                        initial={{ opacity: 0, scale: 0.9 }}
                        animate={{ opacity: 1, scale: 1 }}
                        exit={{ opacity: 0, scale: 0.9 }}
                        transition={{ duration: 0.2 }}
                        className="fixed inset-0 z-50 flex items-center justify-center p-4"
                    >
                        <div className="bg-base-100 rounded-2xl shadow-2xl w-full max-w-4xl max-h-[90vh] overflow-hidden flex flex-col">
                            {/* Header */}
                            <div className={`bg-gradient-to-br ${gradient} p-6 text-white`}>
                                <div className="flex justify-between items-start">
                                    <div>
                                        <h2 className="text-2xl font-bold mb-2">{groupName}</h2>
                                        <p className="text-white/80 text-sm">
                                            {cards.length} карточек в этой группе
                                        </p>
                                    </div>
                                    <button
                                        onClick={onClose}
                                        className="btn btn-ghost btn-sm btn-circle text-white hover:bg-white/20"
                                    >
                                        <X className="w-5 h-5" />
                                    </button>
                                </div>
                            </div>

                            {/* Content - ДОБАВЛЕН min-h для стабильного layout */}
                            <div className="flex-1 overflow-y-auto p-6 min-h-[400px]">
                                {loading && (
                                    <div className="flex justify-center py-12">
                                        <span className="loading loading-spinner loading-lg"></span>
                                    </div>
                                )}

                                {error && (
                                    <div className="alert alert-error">
                                        <span>{error}</span>
                                    </div>
                                )}

                                {!loading && !error && cards.length === 0 && (
                                    <div className="text-center py-12">
                                        <BookOpen className="w-16 h-16 mx-auto mb-4 opacity-30" />
                                        <p className="text-base-content/70">В этой группе пока нет карточек</p>
                                    </div>
                                )}

                                {!loading && !error && cards.length > 0 && (
                                    <AnimatePresence mode="popLayout">
                                        <motion.div
                                            key={`grid-${groupId}-${cards.length}`}
                                            initial={{ opacity: 0 }}
                                            animate={{ opacity: 1 }}
                                            exit={{ opacity: 0 }}
                                            transition={{ duration: 0.3 }}
                                            className="grid grid-cols-1 md:grid-cols-2 gap-4"
                                        >
                                            {cards.map((card) => (
                                                <motion.div
                                                    key={card.Id}
                                                    layout
                                                    initial={{ opacity: 0, y: 20 }}
                                                    animate={{ opacity: 1, y: 0 }}
                                                    exit={{ opacity: 0, y: -20 }}
                                                    transition={{ duration: 0.2 }}
                                                    className="card bg-base-200 shadow-md overflow-hidden"
                                                >
                                                    <div className="card-body p-4">
                                                        {/* Вопрос */}
                                                        <div className="mb-3">
                                                            <h4 className="text-xs font-semibold text-base-content/60 mb-1">
                                                                ВОПРОС:
                                                            </h4>
                                                            <p className="text-base font-medium">{card.Question}</p>
                                                        </div>

                                                        <div className="divider my-1"></div>

                                                        {/* Ответ */}
                                                        <div>
                                                            <h4 className="text-xs font-semibold text-base-content/60 mb-1">
                                                                ОТВЕТ:
                                                            </h4>
                                                            <p className="text-base">{card.Answer}</p>
                                                        </div>
                                                    </div>
                                                </motion.div>
                                            ))}
                                        </motion.div>
                                    </AnimatePresence>
                                )}
                            </div>

                            {/* Footer */}
                            <div className="border-t border-base-300 p-6 flex gap-3">
                                <Button
                                    variant="ghost"
                                    size="lg"
                                    onClick={onClose}
                                    className="flex-1"
                                >
                                    Закрыть
                                </Button>
                                <Button
                                    variant="accent"
                                    size="lg"
                                    onClick={handleSubscribe}
                                    className="flex-1"
                                >
                                    Подписаться
                                </Button>
                            </div>
                        </div>
                    </motion.div>
                </>
            )}
        </AnimatePresence>
    );
}