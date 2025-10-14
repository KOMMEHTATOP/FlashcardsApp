import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import type { SubjectCardType, SubjectDetailType } from "../types/types";
import { ArrowLeft, BowArrowIcon, Trophy } from "lucide-react";
import { motion } from "framer-motion";
import MotivationCard from "../components/cards/Motivation_card";
import CardQuestion from "../components/cards/Card_question";
import { useApp } from "../context/AppContext";
import useTitle from "../utils/useTitle";
import apiFetch from "../utils/apiFetch";
import AddFlashcardForm from "../components/modal/AddFlashcardForm";

export default function StudyPage({}) {
  const { id } = useParams();
  const { handleSelectLesson } = useApp();
  const [group, setGroup] = useState<SubjectDetailType | null>(null);
  const [isOpen, setIsOpen] = useState<boolean>(false);

  const [dataDetail, setDataDetail] = useState<SubjectCardType[]>([]);

  useEffect(() => {
    const fetchCards = async () => {
      try {
        const [groupRes, cardsRes] = await Promise.all([
          apiFetch.get(`/Group/${id}`),
          apiFetch.get(`/groups/${id}/cards`),
        ]);
        console.log(groupRes.data, cardsRes.data);
        setGroup(groupRes.data);
        setDataDetail(cardsRes.data);
      } catch (err) {
        console.log(err);
      }
    };

    fetchCards();
  }, [id]);

  useTitle(group?.GroupName || "");
  console.log(group);

  const handleAddCard = async (question: string, answer: string) => {
    try {
      const data = {
        question,
        answer,
      };
      const res = await apiFetch.post(`/groups/${id}/cards`, data);
      if (res.status !== 200) {
        console.log("Ошибка добавления карточки!");
      }
      console.log(res.data);
    } catch (err) {
      console.log(err);
    }
  };

  return (
    <div className="min-h-screen">
      <div
        className={`relative bg-gradient-to-br ${group?.GroupColor} px-4 sm:px-6 lg:px-8 py-12 overflow-hidden rounded-2xl shadow-xl`}
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
              {/* {group?.icon && <group.icon className="w-12 h-12 text-white" />} */}
            </motion.div>

            <div className="flex-1">
              <div className="flex items-center gap-3 mb-2">
                <h1 className="text-4xl text-white">{group?.GroupName}</h1>
                {/* {Number(group?.streak) > 0 && (
                  <motion.div
                    animate={{ rotate: [0, 10, -10, 0] }}
                    transition={{
                      duration: 1,
                      repeat: Infinity,
                      repeatDelay: 2,
                    }}
                    className="bg-orange-500 text-white px-3 py-2 rounded-full flex items-center gap-1 text-subtitle"
                  >
                    <Flame className="w-5 h-5 text-yellow" /> {group?.streak}{" "}
                    дней подряд
                  </motion.div>
                )} */}
              </div>
              <p className="text-white/90 text-lg mb-4">
                Овладейте основами и раскройте свой потенциал
              </p>

              {/* <div className="space-y-2">
                <div className="flex justify-between text-white/80 text-sm">
                  <span className="text-subtitle">Общий прогресс</span>
                  <span className="text-number">{group?.progress}%</span>
                </div>
                <div className="relative z-10 w-full h-2 bg-white/10">
                  <motion.div
                    initial={{ width: 0 }}
                    animate={{ width: `${group?.progress || 0}%` }}
                    transition={{ duration: 1, ease: "easeOut" }}
                    className="h-full bg-gradient-to-r from-yellow-50 to-yellow-100 rounded-full"
                  />
                </div>
              </div> */}
            </div>
          </div>
          <div className="grid grid-cols-2 sm:grid-cols-4 gap-4">
            {/* {dataDetail.map((stat, index) => (
              <motion.div
                key={stat.Question}
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: index * 0.1 }}
                className="bg-white/20 backdrop-blur-sm rounded-2xl p-4 text-center"
              >
                <div className="text-2xl text-white mb-1 text-number">
                  {stat.UpdatedAt}
                </div>
                <div className="text-white/80 text-sm text-subtitle">
                  {stat.Question}
                </div>
              </motion.div>
            ))} */}
          </div>
        </div>
      </div>

      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <div className="flex items-center justify-between mb-6">
          <h2 className="text-2xl text-base-content/80">Путь обучения</h2>
          <div className="flex items-center gap-2 text-base-content/80">
            <AddFlashcardForm
              handleAddCard={handleAddCard}
              isOpen={isOpen}
              setIsOpen={setIsOpen}
              subjectColor={"from-pink-400 to-rose-500"}
            />
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
                onClick={() => {
                  handleSelectLesson(group!);
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
          gradient={group?.GroupColor || ""}
          delay={0.6}
          className="mt-8"
        />
      </div>
    </div>
  );
}
