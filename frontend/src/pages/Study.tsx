import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import type { SubjectCardType, SubjectDetailType } from "../types/types";
import { testStudyData } from "../test/testData";
import { ArrowLeft, BowArrowIcon, Flame, Trophy } from "lucide-react";
import { motion } from "framer-motion";
import MotivationCard from "../components/cards/Motivation_card";
import CardQuestion from "../components/cards/Card_question";
import { useApp } from "../context/AppContext";
import useTitle from "../utils/useTitle";

const testDataDetail = [
  {
    id: 1,
    title: "Введение в основы",
    duration: 15,
    xp: 50,
    completed: true,
  },
  {
    id: 2,
    title: "Основные концепции",
    duration: 20,
    xp: 75,
    completed: true,
  },
  {
    id: 3,
    title: "Передовые методы",
    duration: 25,
    xp: 100,
    completed: true,
  },
  {
    id: 4,
    title: "Практические упражнения",
    duration: 30,
    xp: 125,
    completed: false,
  },
  {
    id: 5,
    title: "Бросайте вызов проблемам",
    duration: 35,
    xp: 150,
    completed: false,
  },
  {
    id: 6,
    title: "Окончательная оценка",
    duration: 40,
    xp: 200,
    completed: false,
  },
];

export default function StudyPage({}) {
  const { id } = useParams();
  const { handleSelectLesson } = useApp();
  const [data, setData] = useState<SubjectDetailType | null>(null);

  const [dataDetail, setDataDetail] =
    useState<SubjectCardType[]>(testDataDetail);

  useEffect(() => {
    setData(testStudyData.find((item) => item.id === Number(id)) || null);
  }, [id]);

  useTitle(data?.title || "");

  return (
    <div className="min-h-screen">
      <div
        className={`relative bg-gradient-to-br ${data?.gradient} px-4 sm:px-6 lg:px-8 py-12 overflow-hidden rounded-2xl shadow-xl`}
      >
        <div
          className="absolute inset-0 bg-white/10"
          style={{
            backgroundImage:
              "radial-gradient(circle at 2px 2px, rgba(255,255,255,0.15) 1px, transparent 0)",
            backgroundSize: "40px 40px",
          }}
        />
        <div className="max-w-7xl mx-auto relative z-10">
          <Link
            to="/"
            className="text-white hover:bg-white/20 mb-6 flex items-center rounded px-4 py-2 duration-300 transition w-fit"
          >
            <ArrowLeft className="w-5 h-5 mr-2" />
            Назад на главную
          </Link>

          <div className="flex flex-col sm:flex-row items-start sm:items-center gap-6 mb-8">
            <motion.div
              initial={{ scale: 0 }}
              animate={{ scale: 1 }}
              transition={{ type: "spring", stiffness: 200 }}
              className="bg-white/20 backdrop-blur-sm p-6 rounded-3xl"
            >
              {data?.icon && <data.icon className="w-12 h-12 text-white" />}
            </motion.div>

            <div className="flex-1">
              <div className="flex items-center gap-3 mb-2">
                <h1 className="text-4xl text-white">{data?.title}</h1>
                {Number(data?.streak) > 0 && (
                  <motion.div
                    animate={{ rotate: [0, 10, -10, 0] }}
                    transition={{
                      duration: 1,
                      repeat: Infinity,
                      repeatDelay: 2,
                    }}
                    className="bg-orange-500 text-white px-3 py-2 rounded-full flex items-center gap-1 text-subtitle"
                  >
                    <Flame className="w-5 h-5 text-yellow" /> {data?.streak}{" "}
                    дней подряд
                  </motion.div>
                )}
              </div>
              <p className="text-white/90 text-lg mb-4">
                Овладейте основами и раскройте свой потенциал
              </p>

              <div className="space-y-2">
                <div className="flex justify-between text-white/80 text-sm">
                  <span className="text-subtitle">Общий прогресс</span>
                  <span className="text-number">{data?.progress}%</span>
                </div>
                <div className="relative z-10 w-full h-2 bg-white/10">
                  <motion.div
                    initial={{ width: 0 }}
                    animate={{ width: `${data?.progress || 0}%` }}
                    transition={{ duration: 1, ease: "easeOut" }}
                    className="h-full bg-gradient-to-r from-yellow-50 to-yellow-100 rounded-full"
                  />
                </div>
              </div>
            </div>
          </div>
          <div className="grid grid-cols-2 sm:grid-cols-4 gap-4">
            {data?.stats.map((stat, index) => (
              <motion.div
                key={stat.label}
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: index * 0.1 }}
                className="bg-white/20 backdrop-blur-sm rounded-2xl p-4 text-center"
              >
                <div className="text-2xl text-white mb-1 text-number">
                  {stat.value}
                </div>
                <div className="text-white/80 text-sm text-subtitle">
                  {stat.label}
                </div>
              </motion.div>
            ))}
          </div>
        </div>
      </div>

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <div className="flex items-center justify-between mb-6">
          <h2 className="text-2xl text-base-content/80">Путь обучения</h2>
          <div className="flex items-center gap-2 text-base-content/80">
            <Trophy className="w-5 h-5" />
            <span>
              Завершено {dataDetail.filter((item) => item.completed).length} из{" "}
              {dataDetail.length}
            </span>
          </div>
        </div>

        <div className="space-y-4">
          {dataDetail.map((item, index) => (
            <motion.div
              key={index}
              initial={{ opacity: 0, x: -20 }}
              animate={{ opacity: 1, x: 0 }}
              transition={{ delay: index * 0.1 }}
            >
              <CardQuestion
                item={item}
                last={index === testDataDetail.length - 1}
                onClick={() => {
                  handleSelectLesson(data!);
                }}
              />
            </motion.div>
          ))}
        </div>

        <MotivationCard
          animated="scale"
          animatedDelay={4}
          icon={Trophy}
          label="У тебя отлично получается!"
          description={`Пройдите еще ${
            dataDetail.filter((item) => !item.completed).length
          } урока, чтобы получить итоговую оценку и заработать 500 бонусных очков опыта!`}
          textIcon={BowArrowIcon}
          gradient={data?.gradient || ""}
          delay={0.6}
          className="mt-8"
        />
      </div>
    </div>
  );
}
