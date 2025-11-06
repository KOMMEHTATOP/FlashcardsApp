import { motion, AnimatePresence } from "framer-motion";
import {
  BowArrow,
  Brain,
  Lock,
  Mail,
  Rocket,
  Sparkles,
  Star,
  User,
  Zap,
} from "lucide-react";
import { Activity, useState } from "react";
import Input from "../components/ui/input";
import ConfrimBtn from "../components/ui/confrim_btn";
import Card from "../components/ui/card";

import useTitle from "../utils/useTitle";
import { floatingIcons, TITLE_APP } from "../test/data";
import apiFetch, { BASE_URL } from "../utils/apiFetch";
import axios from "axios";
import { Helmet } from "react-helmet-async";

export default function LoginPage() {
  useTitle("Вход");
  const [selectedBlock, setSelectedBlock] = useState<"login" | "register">(
    "login"
  );

  const [login, setLogin] = useState<string>("");
  const [email, setEmail] = useState<string>("");
  const [password, setPassword] = useState<string>("");

  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string>("");

  const handleSelect = (block: string) => {
    setSelectedBlock(block as "login" | "register");
    setError(" ");
  };

  const handleLogin = async () => {
    if (!email) {
      setError("Введите email");
      return;
    } else if (!password) {
      setError("Введите пароль");
      return;
    }
    setLoading(true);

    const data = {
      email,
      password,
    };
    await axios
      .post(`${BASE_URL}/Auth/login`, data, { withCredentials: true })
      .then((res) => {
        console.log(res);
        const token = res.data.accessToken;
        localStorage.setItem("accessToken", token);
        apiFetch.defaults.headers.common["Authorization"] = `Bearer ${token}`;
        window.location.href = "/";
      })
      .catch((err) => {
        console.log(err);
        setError(err.response?.data.message || "Произошла ошибка");
        setLoading(false);
      })
      .finally(() => {
        setTimeout(() => {
          setLoading(false);
        }, 2000);
      });
  };

  const handleRegister = async () => {
    if (!login) {
      setError("Введите логин");
      return;
    } else if (!email) {
      setError("Введите email");
      return;
    } else if (!password) {
      setError("Введите пароль");
      return;
    }
    setLoading(true);

    const data = {
      Login: login,
      Email: email,
      Password: password,
    };

    await axios
      .post(`${BASE_URL}/Auth/register`, data, { withCredentials: true })
      .then((res) => {
        const accessToken = res.data.accessToken;
        localStorage.setItem("accessToken", accessToken);
        handleSelect("login");
      })
      .catch((err) => {
        const errors = err.response?.data?.errors;

        if (errors && typeof errors === "object") {
          const messages = Object.values(errors).flat().join("\n");
          setError(messages);
        } else {
          setError("Произошла ошибка при регистрации");
        }
      })
      .finally(() => {
        setTimeout(() => {
          setLoading(false);
        }, 2000);
      });
  };
  const hasError = Array.isArray(error)
    ? error.length > 0
    : typeof error === "string"
    ? error.trim() !== ""
    : Boolean(error);

  return (
    <div className="min-h-screen bg-gradient-to-br from-base-300 via-base-100 to-base-300 flex items-center justify-center p-4 relative overflow-hidden">
      <Helmet>
        <title>Вход - {TITLE_APP}</title>
        <meta name="robots" content="noindex, nofollow" />
      </Helmet>
      <div className="absolute inset-0 opacity-30">
        <div
          className="absolute inset-0"
          style={{
            backgroundImage:
              "radial-gradient(circle at 2px 2px, rgba(147, 51, 234, 0.1) 1px, transparent 0)",
            backgroundSize: "60px 60px",
          }}
        />
      </div>

      {floatingIcons.map((icon, index) => (
        <motion.div
          key={index}
          initial={{ opacity: 0, scale: 0 }}
          animate={{
            opacity: [0.3, 0.6, 0.3],
            scale: [1, 1.2, 1],
            y: [0, -20, 0],
          }}
          transition={{
            duration: 3,
            repeat: Infinity,
            delay: icon.delay,
          }}
          className="absolute"
          style={{ left: icon.x, top: icon.y }}
        >
          <icon.icon className={`w-12 h-12 ${icon.color}`} />
        </motion.div>
      ))}

      <div className="w-full max-w-md relative z-10">
        <motion.div
          initial={{ opacity: 0, y: -50 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5, delay: 0.1 }}
          className="text-center mb-8"
        >
          <motion.div
            animate={{ rotate: [0, 5, -5, 0] }}
            transition={{ duration: 2, repeat: Infinity, repeatDelay: 1 }}
            className="inline-block"
          >
            <div className="bg-gradient-to-br from-purple-500 to-pink-500 p-4 rounded-3xl shadow-2xl mb-4 inline-block">
              <Brain className="w-16 h-16 text-white" />
            </div>
          </motion.div>
          <h1 className="text-5xl bg-gradient-to-r from-purple-600 to-pink-600 bg-clip-text text-transparent mb-2 p-2">
            {TITLE_APP}
          </h1>
          <p className="text-gray-600 dark:text-gray-400 text-lg text-subtitle">
            Повышайте уровень своего обучения вместе с нами! 🚀
          </p>
        </motion.div>

        <motion.div
          initial={{ opacity: 0, scale: 0.9 }}
          animate={{ opacity: 1, scale: 1 }}
          transition={{ duration: 0.6, delay: 0.2 }}
        >
          <motion.div
            transition={{ duration: 0.4, ease: "easeInOut" }}
            className={`p-8 backdrop-blur-xl bg-white/80  border-2 border-purple-300 shadow-2xl rounded-xl overflow-hidden max-h-dvh transition-all duration-300 ${
              selectedBlock === "login" ? "h-100" : hasError ? "h-130" : "h-120"
            }`}
          >
            <div className="space-y-4">
              <div className="overflow-hidden grid w-full grid-cols-2 bg-gradient-to-r from-purple-100 to-pink-100 dark:from-purple-900/50 dark:to-pink-900/50 rounded-2xl p-1 relative">
                <div className="absolute inset-1 rounded-full overflow-hidden">
                  <motion.div
                    initial={{ x: 0 }}
                    animate={{ x: selectedBlock === "login" ? "0%" : "100%" }}
                    transition={{ duration: 0.2, delay: 0.1 }}
                    className="absolute top-0 left-0 h-full w-1/2 bg-gradient-to-r from-purple-500 to-pink-500 rounded-full shadow-2xl"
                  />
                </div>
                <div
                  className={`transition-all items-center justify-center flex p-1 z-10 cursor-pointer hover:bg-white/10 rounded-2xl ${
                    selectedBlock === "login" ? "text-white" : "text-gray-900"
                  }`}
                  onClick={() => handleSelect("login")}
                >
                  Вход
                </div>
                <div
                  className={`transition-all items-center justify-center flex p-1 z-10 cursor-pointer hover:bg-white/10 rounded-2xl ${
                    selectedBlock === "register"
                      ? "text-white"
                      : "text-gray-900"
                  }`}
                  onClick={() => handleSelect("register")}
                >
                  Регистрация
                </div>
              </div>

              <AnimatePresence mode="wait">
                {selectedBlock == "login" && (
                  <Activity
                    key={selectedBlock[0]}
                    mode={selectedBlock == "login" ? "visible" : "hidden"}
                  >
                    <div className={`space-y-4`}>
                      <motion.form
                        key={selectedBlock[0]}
                        initial={{ opacity: 0, x: -20 }}
                        animate={{ opacity: 1, x: 0 }}
                        exit={{ opacity: 0, x: 20 }}
                        transition={{ duration: 0.3 }}
                        onSubmit={handleLogin}
                        className="space-y-4"
                      >
                        <Input
                          type="email"
                          name="Email"
                          icon={Mail}
                          value={email}
                          onChange={(val) => setEmail(val.target.value)}
                          placeholder="student@studyquest.com"
                          required={true}
                        />

                        <Input
                          type="password"
                          name="Пароль"
                          icon={Lock}
                          value={password}
                          onChange={(val) => setPassword(val.target.value)}
                          placeholder="••••••••"
                          required={true}
                        />

                        <ConfrimBtn
                          loading={loading}
                          handleSubmit={handleLogin}
                          icon={BowArrow}
                          iconRight={true}
                          iconLoading={Sparkles}
                          text="Войти"
                        />
                        <button className="w-full text-purple-600 hover:text-purple-700 text-subtitle hover:bg-purple-300/20 py-2 rounded-xl">
                          Забыли пароль?
                        </button>
                        <span className="items-center text-center">
                          {error && <p className="text-red-500">{error}</p>}
                        </span>
                      </motion.form>
                    </div>
                  </Activity>
                )}

                {selectedBlock == "register" && (
                  <Activity
                    mode={selectedBlock == "register" ? "visible" : "hidden"}
                    key={selectedBlock[1]}
                  >
                    <div className="space-y-4">
                      <motion.form
                        key="register-form"
                        initial={{ opacity: 0, x: 20 }}
                        animate={{ opacity: 1, x: 0 }}
                        exit={{ opacity: 0, x: -20 }}
                        transition={{ duration: 0.3 }}
                        onSubmit={handleRegister}
                        className="space-y-4"
                      >
                        <Input
                          type="text"
                          name="Логин"
                          icon={User}
                          placeholder="Alex"
                          required={false}
                          value={login}
                          onChange={(val) => setLogin(val.target.value)}
                        />
                        <Input
                          type="email"
                          name="Email"
                          icon={Mail}
                          placeholder="student@studyquest.com"
                          required={true}
                          value={email}
                          onChange={(val) => setEmail(val.target.value)}
                        />

                        <Input
                          type="password"
                          name="Пароль"
                          icon={Lock}
                          placeholder="••••••••"
                          required={true}
                          value={password}
                          onChange={(val) => setPassword(val.target.value)}
                        />
                        <ConfrimBtn
                          loading={loading}
                          handleSubmit={handleRegister}
                          icon={Rocket}
                          iconRight={true}
                          iconLoading={Sparkles}
                          text="Начать путешествие"
                        />

                        <p className="text-xs text-center text-gray-500 dark:text-gray-400">
                          Регистрируясь, вы соглашаетесь с нашими Условиями
                          предоставления услуг и Политикой конфиденциальности
                        </p>
                        <span className="items-center text-center">
                          {error && <p className="text-red-500">{error}</p>}
                        </span>
                      </motion.form>
                    </div>
                  </Activity>
                )}
              </AnimatePresence>
            </div>
          </motion.div>
        </motion.div>

        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.6, delay: 0.4 }}
          className="mt-8 grid grid-cols-3 gap-4"
        >
          {[
            {
              icon: Brain,
              label: "Становитесь умнее",
              gradient: "from-yellow-400 to-orange-500",
            },
            {
              icon: Zap,
              label: "Получите опыт",
              gradient: "from-purple-500 to-pink-500",
            },
            {
              icon: Star,
              label: "Повышайте уровень",
              gradient: "from-blue-500 to-cyan-500",
            },
          ].map((feature, index) => (
            <motion.div
              key={feature.label}
              initial={{ opacity: 0, scale: 0.8 }}
              animate={{ opacity: 1, scale: 1 }}
              transition={{ delay: 0.5 + index * 0.1 }}
              whileHover={{ scale: 1.05, y: -5 }}
            >
              <Card
                className={`p-4 text-center bg-gradient-to-br ${feature.gradient} border-none shadow-lg`}
              >
                <feature.icon className="w-10 h-10 text-white mx-auto mb-2" />
                <p className="text-white text-sm truncate">{feature.label}</p>
              </Card>
            </motion.div>
          ))}
        </motion.div>
      </div>
    </div>
  );
}
