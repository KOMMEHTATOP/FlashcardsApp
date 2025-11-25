import { Globe } from "lucide-react";
import type { GroupType } from "@/types/types";
import { availableIcons } from "@/shared/data";

interface ProfileDecksProps {
    groups: GroupType[] | undefined;
}

export function ProfileDecks({ groups }: ProfileDecksProps) {
    return (
        <div className="neon-border p-6">
            <h2 className="text-xl font-bold text-base-content mb-4">
                Мои колоды ({groups?.length || 0})
            </h2>
            <div className="space-y-3 max-h-64 overflow-y-auto pr-2">
                {groups?.slice(0, 5).map((group) => {
                    const IconComponent = availableIcons.find(
                        (icon) => icon.name === group.GroupIcon
                    )?.icon;

                    return (
                        <div
                            key={group.Id}
                            className="flex items-center justify-between bg-base-100 hover:bg-base-200 transition-colors rounded-xl p-3 border border-base-200"
                        >
                            <div className="flex items-center gap-3">
                                <div className={`w-10 h-10 rounded-lg bg-gradient-to-br ${group.GroupColor} flex items-center justify-center text-white shadow-sm`}>
                                    {IconComponent ? (
                                        <IconComponent className="w-5 h-5" />
                                    ) : (
                                        <span className="text-lg">{group.GroupIcon}</span>
                                    )}
                                </div>
                                <span className="font-medium text-base-content">{group.GroupName}</span>
                            </div>
                            <div className="flex items-center gap-2">
                                <span className="text-xs font-medium bg-base-300 px-2 py-1 rounded-md text-base-content/70">
                                    {group.CardCount} карт.
                                </span>
                                <div className="w-4">
                                    {group.IsPublished && (
                                        <div className="tooltip tooltip-left" data-tip="Опубликовано в библиотеке">
                                            <Globe className="w-4 h-4 text-green-500" />
                                        </div>
                                    )}
                                </div>
                            </div>
                        </div>
                    );
                })}
                {(!groups || groups.length === 0) && (
                    <div className="text-center py-4 text-base-content/50">
                        У вас пока нет колод
                    </div>
                )}
            </div>
        </div>
    );
}
