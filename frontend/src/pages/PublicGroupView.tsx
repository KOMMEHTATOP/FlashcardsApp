import { useEffect, useState, useMemo } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import { ArrowLeft, BookHeartIcon, Trophy, BowArrowIcon } from "lucide-react";
import { Helmet } from "react-helmet-async";

import apiFetch from "../utils/apiFetch";
import { useAuth } from "../context/AuthContext";
import { useData } from "../context/DataContext";
import { Header } from "../shared/ui/widgets/Header";
import Footer from "../components/Footer";
import { availableIcons } from "../test/data";
import type { PublicGroupDto, PublicGroupCardDto, GroupType, GroupCardType } from "../types/types";

import { GroupHeader } from "../components/GroupHeader";
import { CardsList } from "../components/CardsList";
import MotivationCard from "../components/cards/Motivation_card";

export default function PublicGroupView() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const { user } = useData();
    const { logout } = useAuth();

    const [groupDto, setGroupDto] = useState<PublicGroupDto | null>(null);
    const [cardsDto, setCardsDto] = useState<PublicGroupCardDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [submittingSubscription, setSubmittingSubscription] = useState(false);

    // --- ЗАГРУЗКА ДАННЫХ ---
    useEffect(() => {
        if (!id) return;

        const fetchData = async () => {
            setLoading(true);
            try {
                const [groupRes, cardsRes] = await Promise.all([
                    apiFetch.get(`/Subscriptions/${id}`),
                    apiFetch.get(`/Subscriptions/public/${id}/cards`)
                ]);
                setGroupDto(groupRes.data);
                setCardsDto(cardsRes.data);
            } catch (err: any) {
                console.error(err);
                setError("Группа не найдена или была удалена автором");
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, [id]);

    // --- АДАПТЕРЫ ТИПОВ (Исправляем ошибки TS) ---

    const groupForHeader: GroupType | null = useMemo(() => {
        if (!groupDto) return null;
        return {
            Id: groupDto.Id,
            GroupName: groupDto.GroupName,
            GroupColor: groupDto.GroupColor,
            GroupIcon: groupDto.GroupIcon || "",
            CardCount: groupDto.CardCount,
            SubscriberCount: groupDto.SubscriberCount,
            IsPublished: true,
            UserId: "",
            CreatedAt: groupDto.CreatedAt,
            // Заглушки для совместимости с GroupType
            Order: 0,
            Cards: []
        } as unknown as GroupType;
    }, [groupDto]);

    const allCardsForList: GroupCardType[] = useMemo(() => {
        return cardsDto.map(c => ({
            CardId: c.CardId,
            Question: c.Question,
            Answer: c.Answer,
            GroupId: id || "",
            CreatedAt: c.CreatedAt,
            LastRating: 0,
            NextReviewDate: "",
            completed: false,
            // Заглушка для совместимости с GroupCardType
            Order: 0
        } as unknown as GroupCardType));
    }, [cardsDto, id]);

    // --- ОБРАБОТЧИКИ ---

    const handleToggleSubscription = async () => {
        // Если гость пытается подписаться - отправляем на логин
        if (!user) {
            navigate("/login");
            return;
        }

        if (!groupDto) return;

        setSubmittingSubscription(true);
        try {
            if (groupDto.IsSubscribed) {
                await apiFetch.delete(`/Subscriptions/${groupDto.Id}/subscribe`);
                setGroupDto(prev => prev ? { ...prev, IsSubscribed: false, SubscriberCount: prev.SubscriberCount - 1 } : null);
            } else {
                await apiFetch.post(`/Subscriptions/${groupDto.Id}/subscribe`);
                setGroupDto(prev => prev ? { ...prev, IsSubscribed: true, SubscriberCount: prev.SubscriberCount + 1 } : null);
            }
        } catch (err: any) {
            console.error("Ошибка подписки:", err);
        } finally {
            setSubmittingSubscription(false);
        }
    };

    // Обработчик клика по карточке
    const handleCardClick = () => {
        // Для авторизованных пользователей здесь можно добавить логику,
        // например, открытие детального просмотра или проигрывания озвучки.
        // Пока оставляем пустым, чтобы не мешать стандартному поведению (если CardQuestion раскрывается сам).
        if (!user) {
            navigate("/login");
        }
    };

    // --- РЕНДЕР ---

    if (loading) {
        return (
            <div className="min-h-screen flex items-center justify-center bg-base-300">
                <span className="loading loading-spinner loading-lg text-primary"></span>
            </div>
        );
    }

    if (error || !groupDto || !groupForHeader) {
        return (
            <div className="min-h-screen flex flex-col items-center justify-center bg-base-300 gap-4">
                <h2 className="text-2xl font-bold text-error">{error || "Ошибка 404"}</h2>
                <Link to="/store" className="btn btn-primary">Вернуться в каталог</Link>
            </div>
        );
    }

    const Icon = availableIcons.find((icon) => icon.name === groupDto.GroupIcon)?.icon || BookHeartIcon;

    return (
        <div className="min-h-screen bg-base-300 flex flex-col">
            <Helmet>
                <title>{`${groupDto.GroupName} | Карточки`}</title>
                <meta name="description" content={`Изучайте карточки по теме "${groupDto.GroupName}". Автор: ${groupDto.AuthorName}.`} />
            </Helmet>

            <Header user={user} onLogout={logout} />

            <main className="flex-grow pt-24 px-4 pb-10">
                <div className="w-full max-w-6xl mx-auto">
                    <Link
                        to="/store"
                        className="text-base-content/70 hover:text-primary mb-6 flex items-center rounded px-4 py-2 duration-300 transition w-fit"
                    >
                        <ArrowLeft className="w-5 h-5 mr-2" />
                        Назад в каталог
                    </Link>

                    {/* Шапка группы */}
                    <GroupHeader
                        group={groupForHeader}
                        icon={Icon}
                        progress={0}
                        isSubscriptionView={true}
                        isSubscribed={groupDto.IsSubscribed}
                        isPublishing={false}
                        submittingSubscription={submittingSubscription}
                        publishError={null}
                        canPublish={false}
                        onTogglePublish={() => {}}
                        onToggleSubscription={handleToggleSubscription}
                    />

                    {/* Список карточек */}
                    <CardsList
                        cards={allCardsForList}
                        group={groupForHeader}
                        isSubscriptionView={true} // Скрывает кнопки редактирования/удаления

                        // Если пользователь не вошел - блюрим всё после 3-й карточки
                        blurAfterIndex={!user ? 3 : undefined}

                        // Обработчик клика
                        onCardClick={handleCardClick}
                    />

                    {/* Мотивационная карточка (видна только авторизованным, так как у гостей есть CTA внутри CardsList) */}
                    {user && (
                        <MotivationCard
                            animated="scale"
                            animatedDelay={1}
                            icon={Trophy}
                            label={groupDto.IsSubscribed ? "Вы подписаны!" : "Добавьте в коллекцию"}
                            description={groupDto.IsSubscribed
                                ? "Эта группа находится в вашем списке 'Мои группы'. Вы можете переходить к изучению."
                                : "Подпишитесь на группу, чтобы она появилась в вашем личном кабинете и вы могли начать обучение."
                            }
                            textIcon={BowArrowIcon}
                            gradient={groupDto.GroupColor || ""}
                            delay={0.2}
                            className="mt-8"
                            onClick={handleToggleSubscription}
                        />
                    )}
                </div>
            </main>

            <Footer />
        </div>
    );
}