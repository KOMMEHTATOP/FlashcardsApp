import React, {
    createContext,
    useCallback,
    useContext,
    useEffect,
    useState,
} from "react";
import type {
    UserData,
    AchievementsType,
    GroupType,
    GroupCardType,
    ConfrimModalState,
    SettingType,
    MotivationType,
    CurrentLessonState,
} from "../types/types";
import { service } from "../utils/apiService";

interface DataContextType {
    // User data
    user: UserData | undefined;
    setUser: React.Dispatch<React.SetStateAction<UserData | undefined>>;

    // Settings & motivation
    setting: SettingType;
    setSetting: React.Dispatch<React.SetStateAction<SettingType>>;
    motivationText: MotivationType | undefined;

    // Groups & lessons
    groups: GroupType[];
    setGroups: React.Dispatch<React.SetStateAction<GroupType[]>>;
    setNewGroups: (newGroup: GroupType) => void;
    putGroups: (group: GroupType) => Promise<void>;
    currentLesson: CurrentLessonState | undefined;
    handleSelectLesson: (cards: GroupCardType[], group: GroupType, index?: number) => void;
    handleCompliteLesson: () => void;
    deleteGroup: (id: string) => Promise<void>;
    deleteCard: (id: string) => Promise<void>;

    // Achievements
    achivment: AchievementsType[];
    questionAnswered: (CardId: string, Rating: number) => Promise<number>;

    // UI state
    confrimModal: ConfrimModalState | undefined;
    handleOpenConfrimModal: (modal: ConfrimModalState) => void;
    handleCloseConfrimModal: () => void;
    loading: boolean;
}

const DataContext = createContext<DataContextType | null>(null);

export function DataProvider({ children }: { children: React.ReactNode }) {
    const [user, setUser] = useState<UserData>();
    const [setting, setSetting] = useState<SettingType>({
        StudyOrder: "Random",
        MinRating: 0,
        MaxRating: 5,
        CompletionThreshold: 0,
        ShuffleOnRepeat: false,
    });

    const [currentLesson, setCurrentLesson] = useState<CurrentLessonState>();
    const [achivment, setAchivment] = useState<AchievementsType[]>([]);
    const [groups, setGroups] = useState<GroupType[]>([]);
    const [confrimModal, setModalConfrimDetail] = useState<ConfrimModalState>();
    const [loading, setLoading] = useState<boolean>(true);
    const [motivationText, setMotivationText] = useState<MotivationType>();

    // Загрузка данных с правильным cleanup
    useEffect(() => {
        const controller = new AbortController();

        const loadData = async () => {
            try {
                setLoading(true);

                const { user, groups, achievements } = await service.getDashboard(controller.signal);
                const settings = await service.getSettings(controller.signal);
                const motivation = await service.getMotivation(controller.signal);

                // Устанавливаем данные только если запрос не отменен
                if (!controller.signal.aborted) {
                    setUser(user);
                    setGroups(groups);
                    setAchivment(achievements);
                    setSetting(settings);
                    setMotivationText(motivation);
                }
            } catch (err: any) {
                // Игнорируем ошибки отмены запроса
                if (err.name === 'AbortError' || err.name === 'CanceledError') {
                    return;
                }
                console.error("Ошибка при загрузке данных:", err);
            } finally {
                if (!controller.signal.aborted) {
                    setLoading(false);
                }
            }
        };

        loadData();

        return () => {
            controller.abort(); // Cleanup - отменяем все запросы
        };
    }, []);

    const handleOpenConfrimModal = useCallback((modal: ConfrimModalState) => {
        setModalConfrimDetail(modal);
    }, []);

    const handleCloseConfrimModal = useCallback(() => {
        setModalConfrimDetail(undefined);
    }, []);

    const handleSelectLesson = useCallback(
        (cards: GroupCardType[], group: GroupType, index: number = 0) => {
            setCurrentLesson({
                group,
                cards,
                initialIndex: index,
                length: cards.length,
            });
        },
        []
    );

    const setNewGroups = (newGroup: GroupType) => {
        setGroups((groups) => [newGroup, ...groups]);
    };

    const putGroups = async (group: GroupType) => {
        setGroups((prev) => prev.map((g) => (g.Id === group.Id ? group : g)));
    };

    const deleteGroup = async (id: string) => {
        setGroups((prev) => prev.filter((group) => group.Id !== id));
        await service.deleteGroup(id);
    };

    const deleteCard = async (id: string) => {
        await service.deleteCard(id);
    };

    const handleCompliteLesson = useCallback(() => {
        setCurrentLesson(undefined);
    }, []);

    const questionAnswered = async (CardId: string, Rating: number) => {
        try {
            const data = await service.answerQuestion(CardId, Rating);
            setCurrentLesson(
                (prev) =>
                    prev && {
                        ...prev,
                        cards: prev.cards.map((card) =>
                            card.CardId === CardId ? { ...card, LastRating: Rating } : card
                        ),
                    }
            );
            setUser(
                (prev) =>
                    prev && {
                        ...prev,
                        Statistics: {
                            ...prev?.Statistics,
                            Level: data.CurrentLevel,
                            XPProgressInCurrentLevel: data.CurrentLevelXP,
                            XPForNextLevel: data.XPToNextLevel,
                            XPRequiredForCurrentLevel: data.XPForNextLevel,
                        },
                    }
            );
            return data.XPEarned;
        } catch (err) {
            console.log(err);
            return 0;
        }
    };

    const value: DataContextType = {
        // User data
        user,
        setUser,

        // Settings & motivation
        setting,
        setSetting,
        motivationText,

        // Groups & lessons
        groups,
        setGroups,
        setNewGroups,
        putGroups,
        currentLesson,
        handleSelectLesson,
        handleCompliteLesson,
        deleteGroup,
        deleteCard,

        // Achievements
        achivment,
        questionAnswered,

        // UI state
        confrimModal,
        handleOpenConfrimModal,
        handleCloseConfrimModal,
        loading,
    };

    return <DataContext.Provider value={value}>{children}</DataContext.Provider>;
}

export function useData() {
    const context = useContext(DataContext);
    if (!context) {
        throw new Error("useData must be used within DataProvider");
    }
    return context;
}