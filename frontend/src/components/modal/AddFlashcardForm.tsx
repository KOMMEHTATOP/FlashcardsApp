import { motion } from "framer-motion";
import { Button, ButtonCircle } from "../../shared/ui/Button";
import { CheckCircle, Plus, Sparkle, Sparkles, X } from "lucide-react";
import { Card } from "../../shared/ui/Card";
import { RichTextEditor } from "../../shared/ui/RichTextEditor";

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
        {isOpen && (
            <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.5 }}
                className={`fixed top-0 left-0 w-full h-full bg-black/50 flex items-center justify-center z-50`}
            >
              <div className="w-[90dvw] md:w-1/2 lg:w-1/3 max-h-[90dvh] overflow-y-auto bg-white p-6 rounded-2xl">
                <div className="flex justify-between items-center">
                  <div
                      className={`text-2xl bg-gradient-to-r ${subjectColor} bg-clip-text text-transparent flex items-center gap-2`}
                  >
                    <Sparkles className="w-6 h-6 text-gray-600" />
                    <label className="">
                      {isUpdateCard ? "Обновить" : "Добавить"} карточку
                    </label>
                  </div>
                  <ButtonCircle onClick={handleClose} className="hover:bg-gray-300">
                    <X className="w-6 h-6 text-gray-600" />
                  </ButtonCircle>
                </div>
                <span className="text-gray-600">
              {isUpdateCard
                  ? "Обновите вопрос и ответ"
                  : "Добавьте вопрос и ответ, чтобы попрактиковаться позже"}
            </span>

                <div className="space-y-6 mt-4">
                  {/* Вопрос */}
                  <div className="space-y-2">
                    <label className="text-gray-700 flex items-center gap-2">
                  <span
                      className={`w-6 h-6 rounded-full bg-gradient-to-r ${subjectColor} text-white flex items-center justify-center text-sm`}
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

                  {/* Ответ */}
                  <div className="space-y-2">
                    <label className="text-gray-700 flex items-center gap-2">
                  <span
                      className={`w-6 h-6 rounded-full bg-gradient-to-r ${subjectColor} text-white flex items-center justify-center text-sm`}
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

                  {/* Превью */}
                  {(question || answer) && (
                      <motion.div
                          initial={{ opacity: 0, y: 10 }}
                          animate={{ opacity: 1, y: 0 }}
                          className="space-y-2"
                      >
                        <label className="text-gray-600">Предпросмотр</label>
                        <div className="grid grid-cols-2 gap-3">
                          {/* Пример вопроса */}
                          <Card
                              className={`p-4 bg-gradient-to-br ${subjectColor} border-none shadow-lg relative overflow-hidden min-h-[120px]`}
                          >
                            <div
                                className="absolute inset-0 bg-white/10"
                                style={{
                                  backgroundImage:
                                      "radial-gradient(circle at 2px 2px, rgba(255,255,255,0.15) 1px, transparent 0)",
                                  backgroundSize: "30px 30px",
                                }}
                            />
                            <div className="relative z-10">
                              <div className="bg-white/20 backdrop-blur-sm px-2 py-1 rounded-lg inline-block mb-2">
                                <span className="text-white text-xs">Вопрос</span>
                              </div>
                              <div
                                  className="text-white text-sm break-words prose prose-sm prose-invert max-w-none"
                                  dangerouslySetInnerHTML={{
                                    __html: question || "Your question will appear here...",
                                  }}
                              />
                            </div>
                          </Card>

                          {/* Пример ответа */}
                          <Card
                              className={`p-4 bg-gradient-to-br ${subjectColor} border-none shadow-lg relative overflow-hidden min-h-[120px]`}
                          >
                            <div
                                className="absolute inset-0 bg-white/10"
                                style={{
                                  backgroundImage:
                                      "radial-gradient(circle at 2px 2px, rgba(255,255,255,0.15) 1px, transparent 0)",
                                  backgroundSize: "30px 30px",
                                }}
                            />
                            <div className="relative z-10">
                              <div className="bg-white/20 backdrop-blur-sm px-2 py-1 rounded-lg inline-block mb-2">
                                <span className="text-white text-xs">Ответ</span>
                              </div>
                              <div
                                  className="text-white text-sm break-words prose prose-sm prose-invert max-w-none"
                                  dangerouslySetInnerHTML={{
                                    __html: answer || "Ваш ответ появится здесь...",
                                  }}
                              />
                            </div>
                          </Card>
                        </div>
                      </motion.div>
                  )}

                  {/* Ошибка */}
                  {error && (
                      <div className="text-red-500 mt-2 text-center">{error}</div>
                  )}

                  {/* Кнопки */}
                  <div className="flex gap-3 pt-4">
                    <Button
                        type="button"
                        variant="outline"
                        onClick={handleClose}
                        className="flex-1 text-gray-600 shadow-lg rounded-xl"
                    >
                      Закрыть
                    </Button>
                    <Button
                        type="button"
                        onClick={handleSubmit}
                        disabled={!question.trim() || !answer.trim() || loading}
                        className={`flex-1 bg-gradient-to-r ${subjectColor} disabled:opacity-50 text-white rounded-xl border-none shadow-lg`}
                    >
                      {loading ? (
                          <Sparkle className={`w-4 h-4 mr-2 animate-spin`} />
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