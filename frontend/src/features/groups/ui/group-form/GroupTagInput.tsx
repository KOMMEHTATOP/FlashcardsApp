import { motion, AnimatePresence } from "framer-motion";
import { X, Tag as TagIcon } from "lucide-react";
import { useState, useMemo } from "react";
import type { TagDto } from "@/types/types";

interface GroupTagInputProps {
    selectedTags: string[];
    onTagsChange: (tags: string[]) => void;
    availableTags: TagDto[];
}

export function GroupTagInput({
    selectedTags,
    onTagsChange,
    availableTags,
}: GroupTagInputProps) {
    const [tagInput, setTagInput] = useState("");
    const [showSuggestions, setShowSuggestions] = useState(false);

    const tagSuggestions = useMemo(() => {
        if (!tagInput.trim()) return [];
        const inputLower = tagInput.toLowerCase();
        return availableTags
            .filter(t => t.Name.toLowerCase().includes(inputLower) && !selectedTags.includes(t.Name))
            .slice(0, 5);
    }, [tagInput, availableTags, selectedTags]);

    const addTag = (tag: string) => {
        const parts = tag.split(',').map(p => p.trim()).filter(p => p.length > 0);
        if (parts.length === 0 && tag.includes(',')) return;

        const newTags = [...selectedTags];
        parts.forEach(cleanTag => {
            if (cleanTag && !newTags.includes(cleanTag)) {
                newTags.push(cleanTag);
            }
        });
        onTagsChange(newTags);
        setTagInput("");
        setShowSuggestions(false);
    };

    const removeTag = (tagToRemove: string) => {
        onTagsChange(selectedTags.filter(t => t !== tagToRemove));
    };

    const handleTagInputKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
        if (e.key === "Enter" || e.key === "Tab" || e.key === ",") {
            e.preventDefault();
            if (tagInput.trim()) {
                addTag(tagInput);
            }
        }
        if (e.key === "Backspace" && !tagInput && selectedTags.length > 0) {
            removeTag(selectedTags[selectedTags.length - 1]);
        }
    };

    return (
        <div className="space-y-2 relative">
            <label className="text-sm font-medium text-gray-700">Теги (Категории)</label>
            <div className="py-1 bg-gray-50 border border-gray-200 rounded-xl focus-within:border-purple-500 focus-within:ring-2 focus-within:ring-purple-200 transition-all flex flex-wrap gap-2">
                <AnimatePresence>
                    {selectedTags.map(tag => (
                        <motion.span
                            key={tag}
                            initial={{ scale: 0.8, opacity: 0 }}
                            animate={{ scale: 1, opacity: 1 }}
                            exit={{ scale: 0.8, opacity: 0 }}
                            className="bg-white border border-purple-200 text-purple-700 px-3 py-1 rounded-full text-sm flex items-center gap-1 shadow-sm"
                        >
                            #{tag}
                            <button
                                onClick={() => removeTag(tag)}
                                className="hover:text-red-500 transition-colors ml-1 focus:outline-none"
                            >
                                <X className="w-3 h-3" />
                            </button>
                        </motion.span>
                    ))}
                </AnimatePresence>

                <input
                    type="text"
                    value={tagInput}
                    onChange={(e) => {
                        const value = e.target.value;
                        setTagInput(value);
                        if (value.endsWith(',')) {
                            addTag(value.slice(0, -1));
                        }
                        setShowSuggestions(true);
                    }}
                    onKeyDown={handleTagInputKeyDown}
                    onBlur={() => setTimeout(() => setShowSuggestions(false), 200)}
                    onFocus={() => setShowSuggestions(true)}
                    placeholder={selectedTags.length === 0 ? "Например: Программирование, SQL, English" : "+ тег"}
                    className="bg-transparent border-none outline-none text-sm flex-1 min-w-[120px] h-8 text-gray-800 placeholder:text-gray-400"
                />
            </div>

            <AnimatePresence>
                {showSuggestions && tagSuggestions.length > 0 && (
                    <motion.div
                        initial={{ opacity: 0, y: -10 }}
                        animate={{ opacity: 1, y: 0 }}
                        exit={{ opacity: 0, y: -10 }}
                        className="absolute top-full left-0 w-full bg-white border border-gray-200 rounded-xl shadow-xl mt-1 z-20 overflow-hidden"
                    >
                        {tagSuggestions.map(tag => (
                            <button
                                key={tag.Id}
                                onMouseDown={() => addTag(tag.Name)}
                                className="w-full text-left px-4 py-2 hover:bg-purple-50 text-sm text-gray-700 flex items-center gap-2"
                            >
                                <TagIcon className="w-3 h-3 text-gray-400" />
                                {tag.Name}
                            </button>
                        ))}
                    </motion.div>
                )}
            </AnimatePresence>
            <p className="text-xs text-gray-400 pl-1">Нажмите Enter или запятую, чтобы добавить тег</p>
        </div>
    );
}
