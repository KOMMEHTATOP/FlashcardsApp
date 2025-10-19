import { motion } from "framer-motion";
import Card from "../ui/card";
import { Check, PlusCircle, X } from "lucide-react";
import { useEffect, useState } from "react";
import Input from "../ui/input";
import { availableColors, availableIcons } from "../../test/data";
import { Button, ButtonCircle } from "../ui/button";
import apiFetch from "../../utils/apiFetch";
import { useApp } from "../../context/AppContext";
import type { GroupType } from "../../types/types";

interface GroupFromProps {
  targetGroup?: GroupType;
  isOpen: boolean;
  handleCancle: () => void;
}

export default function GroupForm({
  targetGroup,
  isOpen,
  handleCancle,
}: GroupFromProps) {
  const { setNewGroups, putGroups } = useApp();
  const [name, setName] = useState<string>("");
  const [selectColor, setSelectColor] = useState<string>(
    availableColors[0].gradient
  );
  const [selectIcon, setSelectIcon] = useState<number>(0);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string>("");

  useEffect(() => {
    if (targetGroup) {
      setName(targetGroup.GroupName);
      setSelectColor(
        availableColors.find((c) => c.gradient === targetGroup.GroupColor)
          ?.gradient || availableColors[0].gradient
      );
      setSelectIcon(
        targetGroup.GroupIcon
          ? availableIcons.findIndex((i) => i.name === targetGroup.GroupIcon)
          : 0
      );
    } else {
      setName("");
      setSelectColor(availableColors[0].gradient);
      setSelectIcon(0);
    }
  }, [targetGroup]);

  const handleSubmit = async () => {
    setError("");

    if (!name || name.trim() === "") {
      setError("Введите название группы");
      return;
    } else if (!selectColor) {
      setError("Выберите цвет группы");
      return;
    } else if (!selectIcon) {
      setError("Выберите иконку группы");
      return;
    }

    try {
      setLoading(true);
      const data = {
        Name: name,
        Color: selectColor,
        Order: 0,
        Icon: availableIcons[selectIcon].icon,
      };
      console.log(data);
      if (targetGroup) {
        await apiFetch
          .put(`/Group/${targetGroup.Id}`, data)
          .then((res) => {
            setTimeout(() => {
              putGroups(res.data);
              setLoading(false);
              handleCancle();
            }, 100);
          })
          .catch((err) =>
            setError(err.response?.data.message || "Произошла ошибка")
          );
      } else {
        await apiFetch
          .post("/Group", data)
          .then((res) => {
            setNewGroups(res.data);
            setTimeout(() => {
              setLoading(false);
              handleCancle();
              setName("");
              setSelectColor(availableColors[0].gradient);
              setSelectIcon(0);
            }, 100);
          })
          .catch((err) =>
            setError(err.response?.data.message || "Произошла ошибка")
          );
      }
    } catch (err) {
      console.log(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      {isOpen && (
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          transition={{ duration: 0.3, ease: "easeInOut" }}
          className="fixed top-0 left-0 w-full h-full bg-black/50 flex items-center justify-center z-50"
        >
          <div className="w-[90dvw] md:w-[80dvw] lg:w-[35dvw] max-h-[90dvh] overflow-y-auto bg-white p-6 rounded-2xl">
            <div>
              <div className="flex justify-between">
                <span className="text-2xl bg-gradient-to-r from-purple-600 to-pink-600 bg-clip-text text-transparent">
                  {targetGroup
                    ? "Редактировать группу"
                    : "Создать новую группу"}
                </span>
                <ButtonCircle
                  onClick={handleCancle}
                  className="hover:bg-gray-300"
                >
                  <X className="w-6 h-6 text-gray-600" />
                </ButtonCircle>
              </div>
              <p className="text-gray-700">
                {targetGroup
                  ? "Редактировать учебную группу"
                  : "Создать учебную группу"}
              </p>
            </div>

            <div className="space-y-6 mt-4">
              <div className="space-y-2">
                <Input
                  name="Название"
                  placeholder="например, математика, история..."
                  required={true}
                  onChange={(e) => setName(e.target.value)}
                  value={name}
                  type="text"
                  className="bg-white/50 border-purple-200 focus:border-purple-500"
                  icon={PlusCircle}
                />
              </div>

              <div className="space-y-2">
                <label className="text-gray-600">Иконка</label>
                <div className="grid grid-cols-3 gap-3">
                  {availableIcons.map((item, index) => (
                    <motion.button
                      key={item.name}
                      type="button"
                      onClick={() => setSelectIcon(index)}
                      whileHover={{ scale: 1.05 }}
                      whileTap={{ scale: 0.95 }}
                      className={`p-4 rounded-xl border-2 transition-all ${
                        selectIcon === index
                          ? "border-purple-500 bg-purple-50"
                          : "border-gray-200 hover:border-pink-300"
                      }`}
                    >
                      <item.icon
                        className={`w-6 h-6 mx-auto ${
                          selectIcon === index
                            ? "text-purple-600"
                            : "text-gray-600"
                        }`}
                      />
                      <p className="text-xs text-gray-600">{item.name}</p>
                    </motion.button>
                  ))}
                </div>
              </div>

              <div className="space-y-2">
                <label className="text-gray-700">Выберите цветовую тему</label>
                <div className="grid grid-cols-3 gap-3 mt-2">
                  {availableColors.map((item, _) => (
                    <motion.button
                      key={item.id}
                      type="button"
                      onClick={() => setSelectColor(item.gradient)}
                      whileHover={{ scale: 1.05 }}
                      whileTap={{ scale: 0.95 }}
                      className="relative"
                    >
                      <div
                        className={`h-20 rounded-xl bg-gradient-to-br ${
                          item.gradient
                        } shadow transition-all ${
                          selectColor === item.gradient
                            ? "ring-4 ring-purple-500 ring-offset-2"
                            : ""
                        }`}
                      >
                        {selectColor === item.gradient && (
                          <motion.div
                            initial={{ scale: 0 }}
                            animate={{ scale: 1 }}
                            className="absolute inset-0 flex items-center justify-center"
                          >
                            <div className="bg-white rounded-full p-1">
                              <Check className="w-5 h-5 text-purple-600" />
                            </div>
                          </motion.div>
                        )}
                      </div>
                      <p className="text-xs text-gray-600 mt-2 text-center">
                        {item.name}
                      </p>
                    </motion.button>
                  ))}
                </div>
              </div>

              <div className="space-y-2">
                <label className="text-gray-600">
                  Предварительный просмотр
                </label>
                <Card
                  className={`p-6 bg-gradient-to-br ${selectColor} border-none shadow-xl relative overflow-hidden`}
                >
                  <div
                    className="absolute inset-0 bg-white/10"
                    style={{
                      backgroundImage:
                        "radial-gradient(circle at 4px 4px, rgba(255,255,255,0.15) 1px, transparent 0)",
                      backgroundSize: "40px 40px",
                    }}
                  />
                  <div className="relative z-10 flex items-center justify-between">
                    <div className="flex items-center gap-4">
                      <div className="bg-white/20 backdrop-blur-sm p-3 rounded-2xl">
                        {(() => {
                          const IconComponent = availableIcons[selectIcon].icon;
                          return (
                            <IconComponent className="w-8 h-8 text-white" />
                          );
                        })()}
                      </div>
                      <div>
                        <h3 className="text-white text-xl">
                          {name || "Название группы"}
                        </h3>
                        <p className="text-white/80 text-sm">0% пройдено</p>
                      </div>
                    </div>
                  </div>
                </Card>
              </div>
              {error && (
                <p className="text-red-500 text-center font-semibold">
                  {error}
                </p>
              )}
              <div className="flex gap-3">
                <Button
                  type="button"
                  variant="outline"
                  onClick={handleCancle}
                  className="flex-1 text-gray-600 shadow-lg rounded-xl"
                >
                  Отменить
                </Button>
                <Button
                  loading={loading}
                  type="submit"
                  onClick={handleSubmit}
                  className="flex-1 bg-gradient-to-r from-purple-500 to-pink-500 hover:from-purple-600 hover:to-pink-600 text-white border-0 shadow-lg rounded-xl"
                >
                  {loading
                    ? "Сохранение..."
                    : targetGroup
                    ? "Сохранить"
                    : "Создать"}
                </Button>
              </div>
            </div>
          </div>
        </motion.div>
      )}
    </>
  );
}
