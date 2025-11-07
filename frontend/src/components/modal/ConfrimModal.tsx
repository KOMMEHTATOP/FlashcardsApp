import { motion } from "framer-motion";
import Card from "../ui/card";
import { Button } from "../ui/button";

interface ConfrimModalProps {
  text: string;
  target?: string;
  handleCancel: () => void;
  handleConfirm: () => void;
}

export default function ConfrimModal({
  text,
  target,
  handleCancel,
  handleConfirm,
}: ConfrimModalProps) {
  return (
    <motion.div
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      transition={{ duration: 0.1 }}
      className="fixed bg-black/60 inset-0 items-center justify-center flex z-50"
    >
      <Card className="w-full m-4 max-w-2xl h-fit bg-white flex items-center p-4 justify-center">
        <div className="text-gray-600 text-center">
          <h1 className="text-xl">{text}</h1>
          <span className="text-lg text-red-400">{target}</span>
        </div>

        <div className="flex gap-3 pt-4 w-full">
          <Button
            className="flex-1 text-gray-600 shadow-lg rounded-xl"
            variant="outline"
            onClick={handleCancel}
          >
            Нет
          </Button>
          <Button
            className="flex-1 rounded-xl shadow-lg"
            variant="error"
            onClick={handleConfirm}
          >
            Да
          </Button>
        </div>
      </Card>
    </motion.div>
  );
}
