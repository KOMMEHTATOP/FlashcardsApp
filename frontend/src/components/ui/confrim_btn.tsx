import { motion } from "framer-motion";

type ConfrimBtnProps = {
  loading: boolean;
  handleSubmit: () => void;
  icon?: any;
  text: string;
  iconLoading: any;
  iconRight?: boolean;
};

export default function ConfrimBtn({
  loading,
  handleSubmit,
  icon: Icon,
  text,
  iconLoading: IconLoading,
  iconRight,
}: ConfrimBtnProps) {
  return (
    <button
      type="button"
      disabled={loading}
      onClick={handleSubmit}
      className="btn btn-block rounded-xl py-6 border-0 bg-gradient-to-r from-purple-500 to-pink-500 hover:from-purple-600 hover:to-pink-600 text-white shadow-lg text-lg"
    >
      {loading ? (
        <motion.div
          animate={{ rotate: 360 }}
          transition={{
            duration: 1,
            repeat: Infinity,
            ease: "linear",
          }}
        >
          <IconLoading className="w-5 h-5" />
        </motion.div>
      ) : (
        <span className="text-number flex items-center gap-2">
          {!iconRight && Icon && <Icon className="w-5 h-5" />}
          {text}
          {iconRight && Icon && <Icon className="w-5 h-5" />}
        </span>
      )}
    </button>
  );
}
