import { useState, useEffect } from "react";
import apiFetch from "../../utils/apiFetch";
import { recallRatingInfo } from "../../test/data";

interface StudyHistoryItem {
    Id: string;
    CardId: string;
    CardQuestion: string;
    Rating: number;
    StudiedAt: string;
    XPEarned: number;
    GroupName: string;
    GroupColor: string;
}

export function ProfileHistory() {
    const [studyHistory, setStudyHistory] = useState<StudyHistoryItem[]>([]);
    const [historyLoading, setHistoryLoading] = useState(false);

    const loadHistory = async () => {
        setHistoryLoading(true);
        try {
            const response = await apiFetch.get('/Study/history');
            setStudyHistory(response.data);
        } catch (err) {
            console.error("Ошибка загрузки истории:", err);
        } finally {
            setHistoryLoading(false);
        }
    };

    useEffect(() => {
        loadHistory();
    }, []);

    return (
        <div className="neon-border p-6">
            <h2 className="text-xl font-bold text-base-content mb-4">
                История обучения
            </h2>
            {historyLoading ? (
                <div className="flex justify-center py-8">
                    <span className="loading loading-spinner loading-md text-primary"></span>
                </div>
            ) : (
                <div className="space-y-3 max-h-64 overflow-y-auto pr-2">
                    {studyHistory.length > 0 ? studyHistory.slice(0, 5).map((item) => (
                        <div
                            key={item.Id}
                            className={`bg-gradient-to-r ${item.GroupColor} rounded-xl p-3 text-white shadow-sm hover:opacity-95 transition-opacity`}
                        >
                            <div className="flex justify-between items-center">
                                <div className="truncate flex-1 mr-3">
                                    <div className="font-medium truncate text-sm md:text-base">{item.CardQuestion}</div>
                                    <div className="text-xs opacity-80 flex gap-2 mt-0.5">
                                        <span>{new Date(item.StudiedAt).toLocaleDateString('ru-RU')}</span>
                                        <span>•</span>
                                        <span className="truncate max-w-[150px]">{item.GroupName}</span>
                                    </div>
                                </div>
                                <div className="flex items-center gap-2 shrink-0">
                                    <span className="text-xs md:text-sm font-medium bg-black/20 px-2 py-1 rounded">
                                        {recallRatingInfo[item.Rating]}
                                    </span>
                                    <span className="bg-white/20 px-2 py-1 rounded text-xs font-bold flex items-center">
                                        +{item.XPEarned} XP
                                    </span>
                                </div>
                            </div>
                        </div>
                    )) : (
                        <div className="text-center py-4 text-base-content/50">
                            История пуста. Начните учиться!
                        </div>
                    )}
                </div>
            )}
        </div>
    );
}
