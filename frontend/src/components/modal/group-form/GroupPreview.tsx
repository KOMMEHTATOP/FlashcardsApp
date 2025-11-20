import { Card } from "../../../shared/ui/Card";
import { availableIcons } from "../../../test/data";

interface GroupPreviewProps {
    name: string;
    color: string;
    iconIndex: number;
    tags: string[];
}

export function GroupPreview({
    name,
    color,
    iconIndex,
    tags,
}: GroupPreviewProps) {
    return (
        <div className="space-y-2">
            <label className="text-sm font-medium text-gray-700">
                Предварительный просмотр
            </label>
            <Card
                className={`p-6 bg-gradient-to-br ${color} border-none shadow-lg relative overflow-hidden transition-all duration-500`}
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
                        <div className="bg-white/20 backdrop-blur-sm p-3 rounded-2xl shadow-inner">
                            {(() => {
                                const IconComponent = availableIcons[iconIndex].icon;
                                return (
                                    <IconComponent className="w-8 h-8 text-white" />
                                );
                            })()}
                        </div>
                        <div>
                            <h3 className="text-white text-xl font-bold tracking-wide drop-shadow-sm">
                                {name || "Название колоды"}
                            </h3>
                            <div className="flex items-center gap-2 text-white/80 text-sm">
                                <span>0 карточек</span>
                                {tags.length > 0 && (
                                    <>
                                        <span>•</span>
                                        <span className="opacity-90">#{tags[0]}</span>
                                        {tags.length > 1 && <span>+{tags.length - 1}</span>}
                                    </>
                                )}
                            </div>
                        </div>
                    </div>
                </div>
            </Card>
        </div>
    );
}
