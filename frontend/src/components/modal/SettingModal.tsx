import { motion } from "framer-motion";
import { SettingsIcon, Star, X } from "lucide-react";
import { useEffect, useState } from "react";
import { ButtonCircle, Button } from "../ui/button";
import Slider from "../ui/slider";
import Switch from "../ui/switch";
import { useApp } from "../../context/AppContext";
import type { SettingType } from "../../types/types";
import apiFetch from "../../utils/apiFetch";

export default function SettingModal({
  handleCancel,
  handleSave,
}: {
  handleCancel: () => void;
  handleSave: () => void;
}) {
  const { setting, setSetting } = useApp();
  const [minRatting, setMinRatting] = useState<number>(setting?.MinRating || 0);
  const [maxRatting, setMaxRatting] = useState<number>(setting?.MaxRating || 0);
  const [RandomMode, setRandomMode] = useState<boolean>(
    setting!.ShuffleOnRepeat
  );
  const [compliteThreshold, setCompliteThreshold] = useState<number>(
    setting?.CompletionThreshold || 0
  );

  useEffect(() => {
    setMinRatting(setting?.MinRating || 0);
    setMaxRatting(setting?.MaxRating || 0);
    setRandomMode(setting!.ShuffleOnRepeat);
    setCompliteThreshold(setting?.CompletionThreshold || 0);
  }, [setting]);

  const renderStars = (count: number, filled: boolean = true) => {
    return (
      <div className="flex gap-1">
        {Array.from({ length: 5 }).map((_, index) => (
          <Star
            key={index}
            className={`w-4 h-4 ${
              index < count
                ? filled
                  ? "fill-yellow-400 text-yellow-400"
                  : "text-yellow-400"
                : "text-gray-300 dark:text-gray-600"
            }`}
          />
        ))}
      </div>
    );
  };

  const handleSaveSettings = async () => {
    await apiFetch
      .post("/StudySettings", {
        StudyOrder: setting?.StudyOrder || "Random",
        MinRating: minRatting,
        MaxRating: maxRatting,
        ShuffleOnRepeat: RandomMode,
        CompletionThreshold: compliteThreshold,
      })
      .then(() => {
        setSetting((prev): SettingType => {
          return {
            ...prev,
            MinRating: minRatting,
            MaxRating: maxRatting,
            ShuffleOnRepeat: RandomMode,
            CompletionThreshold: compliteThreshold,
            StudyOrder: prev.StudyOrder,
          };
        });
        handleSave();
        handleCancel();
      });
  };

  return (
    <motion.div
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      transition={{ duration: 0.5 }}
      className="fixed top-0 left-0 w-full h-full bg-black/50 flex items-center justify-center z-50"
    >
      <div className="w-[90dvw] md:w-[80dvw] lg:w-[35dvw] max-h-[90dvh] overflow-y-auto bg-white p-6 rounded-2xl">
        {/* {Заголовок} */}
        <div>
          <div className="flex justify-between">
            <span className="text-2xl bg-gradient-to-r from-purple-600 to-pink-600 bg-clip-text text-transparent flex items-center gap-1">
              <SettingsIcon className="w-6 h-6 text-purple-500" />
              Настройки
            </span>
            <ButtonCircle onClick={handleCancel} className="hover:bg-gray-300">
              <X className="w-6 h-6 text-gray-600" />
            </ButtonCircle>
          </div>
          <p className="text-gray-700">
            Настройте свои предпочтения в обучении
          </p>
        </div>

        <div className="space-y-6 py-4">
          <div className="space-y-4">
            <div className="bg-gradient-to-r from-purple-50 to-pink-50 p-4 rounded-xl space-y-4">
              <label className="text-gray-900">Фильтр оценки карт</label>
              <p className="text-sm text-gray-600 dark:text-gray-400">
                Показывать только карты в пределах этого диапазона рейтингов
              </p>

              {/* Мин рейтинг */}
              <div className="space-y-2">
                <div className="flex items-center justify-between">
                  <label className="text-sm text-gray-700 ">От</label>
                  {renderStars(minRatting)}
                </div>
                <Slider
                  value={minRatting}
                  onValueChange={(value) => {
                    const newMin = value;
                    setMinRatting(newMin);
                    if (newMin > maxRatting) {
                      setMaxRatting(newMin);
                    }
                  }}
                  min={0}
                  max={5}
                  step={1}
                  className="w-full"
                />
              </div>

              {/* Макс рейтинг */}
              <div className="space-y-2">
                <div className="flex items-center justify-between">
                  <label className="text-sm text-gray-700">До</label>
                  {renderStars(maxRatting)}
                </div>
                <Slider
                  value={maxRatting}
                  onValueChange={(value) => {
                    const newMax = value;
                    setMaxRatting(newMax);
                    if (newMax < minRatting) {
                      setMinRatting(newMax);
                    }
                  }}
                  min={0}
                  max={5}
                  step={1}
                  className="w-full"
                />
              </div>
            </div>
          </div>

          {/* кнопка случайного режима */}
          <div className="bg-gradient-to-r from-blue-50 to-pink-50  p-4 rounded-xl">
            <div className="flex items-center justify-between">
              <div className="space-y-1">
                <label className="text-gray-900 ">Случайный режим</label>
                <p className="text-sm text-gray-600 ">
                  Перемешайте карточки в случайном порядке
                </p>
              </div>
              <Switch checked={RandomMode} onCheckedChange={setRandomMode} />
            </div>
          </div>

          {/* Порог завершения */}
          <div className="bg-gradient-to-r from-purple-50 to-pink-50 p-4 rounded-xl space-y-3">
            <div className="space-y-1">
              <label className="text-gray-900 ">Порог завершения</label>
              <p className="text-sm text-gray-600 ">
                Отметка когда карточка считается как завершенная
              </p>
            </div>
            <div className="flex items-center space-x-2">
              <div className="flex-1">
                <Slider
                  value={compliteThreshold}
                  onValueChange={(value) => {
                    setCompliteThreshold(value);
                  }}
                  min={1}
                  max={5}
                  step={1}
                />
              </div>
              <motion.div
                key={compliteThreshold}
                initial={{ scale: 1.2 }}
                animate={{ scale: 1 }}
                className="bg-white px-4 py-2 rounded-lg border-2 border-green-500 min-w-[60px] text-center"
              >
                <span className="bg-gradient-to-r from-green-600 to-emerald-600 bg-clip-text text-transparent">
                  {compliteThreshold}
                </span>
              </motion.div>
            </div>
          </div>
        </div>

        <div className="flex gap-3 justify-end pt-4 border-t border-gray-200 ">
          <Button
            variant="outline"
            onClick={handleCancel}
            className="text-gray-600 shadow-lg rounded-xl"
          >
            Закрыть
          </Button>
          <Button
            onClick={handleSaveSettings}
            className="bg-gradient-to-r from-purple-500 to-pink-500 hover:from-purple-600 hover:to-pink-600 text-white border-0 shadow-lg rounded-xl"
          >
            Сохранить настройки
          </Button>
        </div>
      </div>
    </motion.div>
  );
}
