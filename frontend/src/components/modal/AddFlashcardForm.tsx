import { useState } from "react";
import { motion } from "framer-motion";
import { Button, ButtonCircle } from "../ui/button";
import { CheckCircle, Plus, Sparkle, Sparkles, X } from "lucide-react";
import Card from "../ui/card";

interface AddFlashcardFormProps {
  subjectColor: string;
  handleAddCard: (question: string, answer: string) => void;
  isOpen: boolean;
  setIsOpen: (isOpen: boolean) => void;
  loading: boolean;
}

export default function AddFlashcardForm({
  subjectColor,
  handleAddCard,
  isOpen,
  setIsOpen,
  loading,
}: AddFlashcardFormProps) {
  const [question, setQuestion] = useState<string>("");
  const [answer, setAnswer] = useState<string>("");

  const handleSubmit = async () => {
    handleAddCard(question, answer);
  };

  const handleClose = () => {
    setIsOpen(false);
    setQuestion("");
    setAnswer("");
  };

  return (
    <div>
      <motion.div whileHover={{ scale: 1.02 }} whileTap={{ scale: 0.98 }}>
        <Button
          size="lg"
          onClick={() => setIsOpen(true)}
          className={`bg-gradient-to-r ${subjectColor} hover:opacity-90 text-white shadow-lg w-full border-none`}
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
                <Sparkles className="w-6 h-6 text-pink-400" />
                <label className="">Создать новую карточку</label>
              </div>
              <ButtonCircle onClick={handleClose} className="hover:bg-gray-300">
                <X className="w-6 h-6 text-gray-600" />
              </ButtonCircle>
            </div>
            <span className="text-gray-600">
              Добавьте вопрос и ответ, чтобы попрактиковаться позже
            </span>

            <div className="space-y-6 mt-4">
              {/* ВОпрос */}
              <div className="space-y-2">
                <label className="text-gray-700 flex items-center gap-2">
                  <span
                    className={`w-6 h-6 rounded-full bg-gradient-to-r ${subjectColor}  text-white flex items-center justify-center text-sm`}
                  >
                    Q
                  </span>
                  Вопрос
                </label>
                <textarea
                  value={question}
                  onChange={(e) => setQuestion(e.target.value)}
                  placeholder="например, какая столица Франции?"
                  className="min-h-[100px] w-full bg-white/50 border-purple-200  focus:border-purple-500 resize-none border-2 rounded-md px-3 py-2 text-gray-900 placeholder:text-gray-400 focus:outline-none focus:ring-2 focus:ring-purple-600 focus:ring-offset-2"
                  required
                />
                <p className="text-xs text-gray-500">
                  {question.length} / 500 символов
                </p>
              </div>

              {/* Ответ */}
              <div className="space-y-2">
                <label className="text-gray-700 flex items-center gap-2">
                  <span
                    className={`w-6 h-6 rounded-full bg-gradient-to-r ${subjectColor}  text-white flex items-center justify-center text-sm`}
                  >
                    A
                  </span>
                  Ответ
                </label>
                <textarea
                  value={answer}
                  onChange={(e) => setAnswer(e.target.value)}
                  placeholder="например, Париж"
                  className="min-h-[100px] w-full bg-white/50 border-purple-200  focus:border-purple-500 resize-none border-2 rounded-md px-3 py-2 text-gray-900 placeholder:text-gray-400 focus:outline-none focus:ring-2 focus:ring-purple-600 focus:ring-offset-2"
                  required
                />
                <p className="text-xs text-gray-500">
                  {answer.length} / 500 символов
                </p>
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
                        <p className="text-white text-sm break-words">
                          {question || "Your question will appear here..."}
                        </p>
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
                        <p className="text-white text-sm break-words">
                          {answer || "Ваш ответ появится здесь..."}
                        </p>
                      </div>
                    </Card>
                  </div>
                </motion.div>
              )}

              {/* Кнопки */}
              <div className="flex gap-3 pt-4">
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => {
                    setIsOpen(false);
                    setQuestion("");
                    setAnswer("");
                  }}
                  className="flex-1 text-gray-600 shadow-lg rounded-xl"
                >
                  Cancel
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
                  Добавить карточку
                </Button>
              </div>
            </div>
          </div>
        </motion.div>
      )}
    </div>
  );
}
