import { motion } from "framer-motion";
import Card from "../ui/card";
import { Check, Plus, PlusCircle, X } from "lucide-react";
import { useState } from "react";
import Input from "../ui/input";
import { availableColors, availableIcons } from "../../test/data";
import { Button, ButtonCircle } from "../ui/button";
import apiFetch from "../../utils/apiFetch";
import { useApp } from "../../context/AppContext";

export default function GroupForm() {
  const { setNewGroups, groups } = useApp();
  const [isOpen, setIsOpen] = useState<boolean>(false);
  const [name, setName] = useState<string>("");
  const [selectColor, setSelectColor] = useState<string>(
    availableColors[0].gradient
  );
  const [selectIcon, setSelectIcon] = useState<number>(0);
  const [loading, setLoading] = useState<boolean>(false);

  const handleOpen = () => setIsOpen(true);
  const handleClose = () => setIsOpen(false);

  const handleSubmit = async () => {
    try {
      setLoading(true);
      const data = {
        Name: name,
        Color: selectColor,
        Order: 0,
        // Icon: selectIcon
      };
      console.log(data);

      await apiFetch
        .post("/Group", data)
        .then((res) => {
          console.log(res.data, groups);
          setNewGroups(res.data);
          setTimeout(() => {
            setIsOpen(false);
            setLoading(false);
          }, 1000);
        })
        .catch((err) => console.log(err));
    } catch (err) {
      console.log(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <motion.button
        whileHover={{ scale: 1.01, y: -5 }}
        whileTap={{ scale: 1 }}
        transition={{ duration: 0.2 }}
        className="group relative w-full md:w-70 h-50"
        onClick={handleOpen}
      >
        <Card className="p-6 border-2 border-dashed border-purple-300  hover:border-purple-500 bg-base-300  backdrop-blur-sm transition-all cursor-pointer h-full min-h-[200px] flex flex-col items-center justify-center">
          <motion.div
            animate={{ rotate: [0, 90, 0] }}
            transition={{ duration: 2, repeat: Infinity, repeatDelay: 1 }}
            className="bg-gradient-to-br from-purple-400 to-pink-500 p-4 rounded-2xl mb-4 shadow-lg"
          >
            <Plus className="w-8 h-8 text-white" />
          </motion.div>
          <p className="text-base-content/80 ">Добавить новую тему</p>
          <p className="text-sm text-base-content/60 mt-1">
            Создать учебную группу
          </p>
        </Card>
      </motion.button>
      {isOpen && (
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5 }}
          className="fixed top-0 left-0 w-full h-full bg-black/50 flex items-center justify-center z-50"
        >
          <div className="w-[90dvw] md:w-1/2 lg:w-1/3 max-h-[90dvh] overflow-y-auto bg-white p-6 rounded-2xl">
            <div>
              <div className="flex justify-between">
                <span className="text-2xl bg-gradient-to-r from-purple-600 to-pink-600 bg-clip-text text-transparent">
                  Создать новую тему
                </span>
                <ButtonCircle
                  onClick={handleClose}
                  className="hover:bg-gray-300"
                >
                  <X className="w-6 h-6 text-gray-600" />
                </ButtonCircle>
              </div>
              <p className="text-gray-700">Создать учебную группу</p>
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

              <div className="flex gap-3 pt-4">
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => setIsOpen(false)}
                  className="flex-1 text-gray-600 shadow-lg rounded-xl"
                >
                  Отменить
                </Button>
                <Button
                  type="submit"
                  onClick={handleSubmit}
                  className="flex-1 bg-gradient-to-r from-purple-500 to-pink-500 hover:from-purple-600 hover:to-pink-600 text-white border-0 shadow-lg rounded-xl"
                >
                  Создать группу
                </Button>
              </div>
            </div>
          </div>
        </motion.div>
      )}
    </div>
  );
}
