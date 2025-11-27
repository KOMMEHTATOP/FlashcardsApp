import { motion } from "framer-motion";
import { Card } from "@/shared/ui/Card";
import { Button } from "@/shared/ui/Button";

interface ConfirmModalProps {
  text: string;
  target?: string;
  handleCancel: () => void;
  handleConfirm: () => void;
  isAlert?: boolean;
}

export default function ConfirmModal({
                                       text,
                                       target,
                                       handleCancel,
                                       handleConfirm,
                                       isAlert = false,
                                     }: ConfirmModalProps) {
  return (
      <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          transition={{ duration: 0.1 }}
          className="fixed bg-black/60 inset-0 items-center justify-center flex z-50"
          onClick={(e) => {
            if (e.target === e.currentTarget && isAlert) handleCancel();
          }}
      >
        <Card className="w-full m-4 max-w-sm bg-white flex flex-col items-center p-6 justify-center rounded-2xl shadow-2xl">
          <div className="text-gray-800 text-center mb-6">
            <h1 className={`font-bold mb-2 ${isAlert ? "text-xl text-red-500" : "text-xl"}`}>
              {text}
            </h1>
            {target && (
                <p className="text-base text-gray-600 leading-relaxed">
                  {target}
                </p>
            )}
          </div>

          <div className="flex gap-3 w-full">
            {!isAlert && (
                <Button
                    className="flex-1 text-gray-600 bg-gray-100 hover:bg-gray-200 border-none rounded-xl h-12"
                    variant="outline"
                    onClick={handleCancel}
                >
                  Нет
                </Button>
            )}

            <Button
                className={`flex-1 rounded-xl h-12 text-white font-medium shadow-lg transition-transform active:scale-95 ${
                    isAlert
                        ? "bg-gray-900 hover:bg-black" 
                        : "bg-red-500 hover:bg-red-600" 
                }`}
                onClick={handleConfirm}
            >
              {isAlert ? "Понятно" : "Да, удалить"}
            </Button>
          </div>
        </Card>
      </motion.div>
  );
}