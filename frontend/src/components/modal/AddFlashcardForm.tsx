import { motion } from "framer-motion";
import { Button, ButtonCircle } from "../../shared/ui/Button";
import { CheckCircle, Plus, Sparkles, X } from "lucide-react";
import { RichTextEditor } from "../../shared/ui/RichTextEditor";

// --- ТИПЫ ---
interface AddFlashcardFormProps {
    subjectColor: string;
    handleAddCard: (question: string, answer: string) => Promise<boolean>;
    question: string;
    answer: string;
    setQuestion: React.Dispatch<React.SetStateAction<string>>;
    setAnswer: React.Dispatch<React.SetStateAction<string>>;

    isOpen: boolean;
    handleNewCard: () => void;
    handleCloseModal: () => void;
    loading: boolean;
    isUpdateCard: boolean;
    error?: string;
}

export default function AddFlashcardForm({
                                             subjectColor,
                                             handleAddCard,
                                             question,
                                             answer,
                                             setQuestion,
                                             setAnswer,
                                             isOpen,
                                             handleNewCard,
                                             handleCloseModal,
                                             loading,
                                             isUpdateCard,
                                             error,
                                         }: AddFlashcardFormProps) {

    // --- ЛОГИКА ---
    const handleSubmit = async () => {
        const isSucces = await handleAddCard(question, answer);
        if (isSucces) {
            handleCloseModal();
            setQuestion("");
            setAnswer("");
        }
    };

    const handleClose = () => {
        handleCloseModal();
        setQuestion("");
        setAnswer("");
    };

    return (
        <div>
            {/* Кнопка вызова модалки (Отображается в списке) */}
            <motion.div whileHover={{ scale: 1.02 }} whileTap={{ scale: 0.98 }}>
                <Button
                    size="md"
                    onClick={() => handleNewCard()}
                    className={`bg-gradient-to-r ${subjectColor} hover:opacity-90 text-white shadow-lg w-full border-none `}
                >
                    <Plus className="w-5 h-5 mr-2" />
                    Добавить новую карточку
                </Button>
            </motion.div>

            {/* САМО МОДАЛЬНОЕ ОКНО */}
            {isOpen && (
                <motion.div
                    initial={{ opacity: 0, y: 20 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ duration: 0.5 }}
                    className={`fixed top-0 left-0 w-full h-full bg-black/50 flex items-center justify-center z-50 backdrop-blur-sm`}
                >
                    {/* КОНТЕЙНЕР ОКНА:
                        Было: w-[90dvw] md:w-1/2 lg:w-1/3 (Слишком узко)
                        Стало: w-full max-w-4xl mx-4 (Широкое, удобное окно)
                    */}
                    <div className="w-full max-w-4xl max-h-[90dvh] overflow-y-auto bg-white p-6 rounded-2xl mx-4 shadow-2xl">

                        {/* Шапка модалки */}
                        <div className="flex justify-between items-center mb-2">
                            <div
                                className={`text-2xl bg-gradient-to-r ${subjectColor} bg-clip-text text-transparent flex items-center gap-2`}
                            >
                                <Sparkles className="w-6 h-6 text-gray-600" />
                                <label className="font-bold">
                                    {isUpdateCard ? "Обновить" : "Добавить"} карточку
                                </label>
                            </div>
                            <ButtonCircle onClick={handleClose} className="hover:bg-gray-100 transition-colors">
                                <X className="w-6 h-6 text-gray-600" />
                            </ButtonCircle>
                        </div>

                        <p className="text-gray-500 text-sm mb-6">
                            {isUpdateCard
                                ? "Обновите вопрос и ответ"
                                : "Добавьте вопрос и ответ, чтобы попрактиковаться позже"}
                        </p>

                        {/* Форма */}
                        <div className="space-y-6">

                            {/* Поле: Вопрос */}
                            <div className="space-y-2">
                                <label className="text-gray-700 font-medium flex items-center gap-2">
                                    <span
                                        className={`w-6 h-6 rounded-full bg-gradient-to-r ${subjectColor} text-white flex items-center justify-center text-sm shadow-sm`}
                                    >
                                        Q
                                    </span>
                                    Вопрос
                                </label>
                                <RichTextEditor
                                    content={question}
                                    onChange={setQuestion}
                                    placeholder="например, какая столица Франции?"
                                    maxLength={500}
                                />
                            </div>

                            {/* Поле: Ответ */}
                            <div className="space-y-2">
                                <label className="text-gray-700 font-medium flex items-center gap-2">
                                    <span
                                        className={`w-6 h-6 rounded-full bg-gradient-to-r ${subjectColor} text-white flex items-center justify-center text-sm shadow-sm`}
                                    >
                                        A
                                    </span>
                                    Ответ
                                </label>
                                <RichTextEditor
                                    content={answer}
                                    onChange={setAnswer}
                                    placeholder="например, Париж"
                                    maxLength={2000}
                                />
                            </div>

                            {/* Блок ошибки */}
                            {error && (
                                <div className="bg-red-50 text-red-500 p-3 rounded-lg text-center text-sm border border-red-100">
                                    {error}
                                </div>
                            )}

                            {/* Футер с кнопками */}
                            <div className="flex gap-3 pt-4 border-t border-gray-100 mt-6">
                                <Button
                                    type="button"
                                    variant="outline"
                                    onClick={handleClose}
                                    className="flex-1 text-gray-600 shadow-sm rounded-xl border-gray-200 hover:bg-gray-50"
                                >
                                    Закрыть
                                </Button>
                                <Button
                                    type="button"
                                    onClick={handleSubmit}
                                    disabled={!question.trim() || !answer.trim() || loading}
                                    className={`flex-1 bg-gradient-to-r ${subjectColor} disabled:opacity-50 text-white rounded-xl border-none shadow-lg hover:shadow-xl transition-all`}
                                >
                                    {loading ? (
                                        <div className="w-4 h-4 mr-2 border-2 border-white border-t-transparent rounded-full animate-spin" />
                                    ) : (
                                        <CheckCircle className="w-4 h-4 mr-2" />
                                    )}
                                    {isUpdateCard ? "Обновить" : "Добавить"}
                                </Button>
                            </div>
                        </div>
                    </div>
                </motion.div>
            )}
        </div>
    );
}