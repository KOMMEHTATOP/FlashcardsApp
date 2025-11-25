import { motion } from "framer-motion";
import { PlusCircle, X } from "lucide-react";
import { useEffect, useState } from "react";
import { Input } from "@/shared/ui/input";
import { availableColors, availableIcons } from "@/shared/data";
import { Button, ButtonCircle } from "@/shared/ui/Button";
import apiFetch from "@/utils/apiFetch";
import { useData } from "@/context/DataContext";
import type { GroupType, TagDto } from "@/types/types";
import { errorFormater } from "@/utils/errorFormater";
import { GroupTagInput } from "./group-form/GroupTagInput";
import { GroupIconSelector } from "./group-form/GroupIconSelector";
import { GroupColorSelector } from "./group-form/GroupColorSelector";
import { GroupPreview } from "./group-form/GroupPreview";

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
    const { setNewGroups, putGroups } = useData();

    const [name, setName] = useState<string>("");
    const [selectColor, setSelectColor] = useState<string>(availableColors[0].gradient);
    const [selectIcon, setSelectIcon] = useState<number>(0);
    const [selectedTags, setSelectedTags] = useState<string[]>([]);
    const [availableTags, setAvailableTags] = useState<TagDto[]>([]);

    const [loading, setLoading] = useState<boolean>(false);
    const [error, setError] = useState<string>("");

    useEffect(() => {
        if (isOpen) {
            apiFetch.get("/Subscriptions/tags")
                .then(res => setAvailableTags(res.data))
                .catch(console.error);
        }
    }, [isOpen]);

    useEffect(() => {
        if (targetGroup) {
            setName(targetGroup.GroupName);
            setSelectColor(
                availableColors.find((c) => c.gradient === targetGroup.GroupColor)?.gradient || availableColors[0].gradient
            );
            setSelectIcon(
                targetGroup.GroupIcon
                    ? availableIcons.findIndex((i) => i.name === targetGroup.GroupIcon)
                    : 0
            );
            if (targetGroup.Tags) {
                setSelectedTags(targetGroup.Tags.map(t => t.Name));
            } else {
                setSelectedTags([]);
            }
        } else {
            setName("");
            setSelectColor(availableColors[0].gradient);
            setSelectIcon(0);
            setSelectedTags([]);
        }
    }, [targetGroup, isOpen]);

    const handleSubmit = async () => {
        setError("");

        if (!name.trim()) {
            setError("Введите название группы");
            return;
        }

        if (!selectColor) {
            setError("Выберите цвет группы");
            return;
        }

        setLoading(true);

        const data = {
            Name: name,
            Color: selectColor,
            Order: 0,
            GroupIcon: availableIcons[selectIcon].name,
            Tags: selectedTags
        };

        try {
            const res = targetGroup
                ? await apiFetch.put(`/Group/${targetGroup.Id}`, data)
                : await apiFetch.post("/Group", data);

            if (targetGroup) {
                putGroups(res.data);
            } else {
                setNewGroups(res.data);
                setName("");
                setSelectColor(availableColors[0].gradient);
                setSelectIcon(0);
                setSelectedTags([]);
            }
            handleCancle();
        } catch (err: any) {
            setError(errorFormater(err) || "Произошла ошибка");
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
                    transition={{ duration: 0.3 }}
                    className="fixed top-0 left-0 w-full h-full bg-black/50 flex items-center justify-center z-50"
                    onMouseDown={(e) => e.target === e.currentTarget && handleCancle()}
                >
                    <div className="w-[95dvw] md:w-[80dvw] lg:w-[40dvw] max-h-[90dvh] overflow-y-auto bg-white p-6 rounded-2xl scrollbar-hide relative">
                        <div>
                            <div className="flex justify-between items-start">
                                <div>
                                    <span className="text-2xl font-bold bg-gradient-to-r from-purple-600 to-pink-600 bg-clip-text text-transparent">
                                        {targetGroup ? "Редактировать колоду" : "Создать новую колоду"}
                                    </span>
                                    <p className="text-gray-500 text-sm mt-1">
                                        {targetGroup ? "Измените параметры" : "Заполните данные для создания"}
                                    </p>
                                </div>
                                <ButtonCircle onClick={handleCancle} className="hover:bg-gray-100">
                                    <X className="w-6 h-6 text-gray-500" />
                                </ButtonCircle>
                            </div>
                        </div>

                        <div className="space-y-6 mt-6">
                            <div className="space-y-2">
                                <Input
                                    name="Название"
                                    placeholder="Например: Основы React, English A1..."
                                    required={true}
                                    onChange={(e) => setName(e.target.value)}
                                    value={name}
                                    type="text"
                                    className="bg-gray-50 border-gray-200 focus:border-purple-500 focus:ring-purple-200"
                                    icon={PlusCircle}
                                />
                            </div>

                            <GroupTagInput
                                selectedTags={selectedTags}
                                onTagsChange={setSelectedTags}
                                availableTags={availableTags}
                            />

                            <GroupIconSelector
                                selectedIconIndex={selectIcon}
                                onIconSelect={setSelectIcon}
                            />

                            <GroupColorSelector
                                selectedColor={selectColor}
                                onColorSelect={setSelectColor}
                            />

                            <GroupPreview
                                name={name}
                                color={selectColor}
                                iconIndex={selectIcon}
                                tags={selectedTags}
                            />

                            {error && (
                                <div className="p-3 bg-red-50 border border-red-200 rounded-xl text-red-600 text-sm text-center">
                                    {error}
                                </div>
                            )}

                            <div className="flex gap-3 pt-2">
                                <Button
                                    type="button"
                                    variant="ghost"
                                    onClick={handleCancle}
                                    className="flex-1 text-gray-500 hover:bg-gray-100"
                                >
                                    Отменить
                                </Button>
                                <Button
                                    loading={loading}
                                    type="submit"
                                    onClick={handleSubmit}
                                    className="flex-1 bg-gradient-to-r from-purple-600 to-pink-600 hover:from-purple-700 hover:to-pink-700 text-white shadow-lg shadow-purple-500/30 border-0"
                                >
                                    {loading
                                        ? "Сохранение..."
                                        : targetGroup
                                            ? "Сохранить изменения"
                                            : "Создать колоду"}
                                </Button>
                            </div>
                        </div>
                    </div>
                </motion.div>
            )}
        </>
    );
}