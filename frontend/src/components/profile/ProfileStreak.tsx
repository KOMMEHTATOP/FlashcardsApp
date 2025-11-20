import { Zap } from "lucide-react";
import type { UserData } from "../../types/types";

interface ProfileStreakProps {
    user: UserData | null;
}

export function ProfileStreak({ user }: ProfileStreakProps) {
    return (
        <div className="bg-gradient-to-br from-orange-500 to-red-500 rounded-2xl p-6 text-white shadow-lg">
            <h3 className="text-lg font-bold mb-3">Ваше упорство</h3>
            <div className="flex items-center gap-4">
                <div className="bg-white/20 p-3 rounded-full">
                    <Zap className="w-8 h-8" />
                </div>
                <div>
                    <div className="text-3xl font-bold">
                        {user?.Statistics?.CurrentStreak || 0} дн.
                    </div>
                    <div className="text-white/80 text-sm mt-1">
                        Текущая серия. Лучшая: {user?.Statistics?.BestStreak || 0}
                    </div>
                </div>
            </div>
        </div>
    );
}
