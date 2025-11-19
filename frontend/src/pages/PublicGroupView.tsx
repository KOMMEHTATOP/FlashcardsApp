import { useEffect, useState, useMemo } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import { ArrowLeft, BookHeartIcon, Trophy, BowArrowIcon } from "lucide-react";
import { Helmet } from "react-helmet-async";

// Импорты
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
                setError("Эта колода не найдена или была удалена автором");
            } finally {
                setLoading(false);
            }
        };
        fetchData();
    }, [id]);

    // SEO: JSON-LD для Google
    const structuredData = useMemo(() => {
        if (!groupDto || cardsDto.length === 0) return null;
        return {
            "@context": "https://schema.org",
            "@type": "LearningResource", // Более точный тип для учебных материалов
            "name": groupDto.GroupName,
            "description": `Колода карточек по теме ${groupDto.GroupName}. Автор: ${groupDto.AuthorName}.`,
            "learningResourceType": "Flashcards",
            "educationalLevel": "Beginner", // Можно динамически менять, если будет поле
            "author": {
                "@type": "Person",
                "name": groupDto.AuthorName
            },
            "hasPart": {
                "@type": "FAQPage",
                "mainEntity": cardsDto.map(card => ({
                    "@type": "Question",
                    "name": card.Question,
                    "acceptedAnswer": {
                        "@type": "Answer",
                        "text": card.Answer || "Ответ доступен в полной версии"
                    }
                }))
            }
        };
    }, [groupDto, cardsDto]);

    // АДАПТЕРЫ
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
            Order: 0
        } as unknown as GroupCardType));
    }, [cardsDto, id]);

    const handleToggleSubscription = async () => {
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
            console.error("Ошибка действия:", err);
        } finally {
            setSubmittingSubscription(false);
        }
    };

    const handleCardClick = () => {
        if (!user) navigate("/login");
    };

    if (loading) return <div className="min-h-screen flex items-center justify-center bg-base-300"><span className="loading loading-spinner loading-lg text-primary"></span></div>;

    if (error || !groupDto || !groupForHeader) return (
        <div className="min-h-screen flex flex-col items-center justify-center bg-base-300 gap-4">
            <h2 className="text-2xl font-bold text-error">{error || "Ошибка 404"}</h2>
            <Link to="/store" className="btn btn-primary">В библиотеку</Link>
        </div>
    );

    const Icon = availableIcons.find((icon) => icon.name === groupDto.GroupIcon)?.icon || BookHeartIcon;
    const pageTitle = `${groupDto.GroupName} - Карточки для запоминания | FlashcardsLoop`;
    const pageDesc = `Бесплатная колода карточек по теме "${groupDto.GroupName}" от ${groupDto.AuthorName}. ${groupDto.CardCount} вопросов. Учите онлайн с интервальными повторениями.`;

    return (
        <div className="min-h-screen bg-base-300 flex flex-col">
            <Helmet>
                <title>{pageTitle}</title>
                <meta name="description" content={pageDesc} />

                {/* Open Graph / Facebook / Telegram Preview */}
                <meta property="og:type" content="article" />
                <meta property="og:title" content={pageTitle} />
                <meta property="og:description" content={pageDesc} />
                <meta property="og:url" content={window.location.href} />
                {/* Если есть картинка группы, можно добавить og:image */}

                {structuredData && (
                    <script type="application/ld+json">{JSON.stringify(structuredData)}</script>
                )}
            </Helmet>

            <Header user={user} onLogout={logout} />

            <main className="flex-grow pt-24 px-4 pb-10">
                <div className="w-full max-w-6xl mx-auto">
                    <Link
                        to="/store"
                        className="text-base-content/70 hover:text-primary mb-6 flex items-center rounded px-4 py-2 duration-300 transition w-fit"
                    >
                        <ArrowLeft className="w-5 h-5 mr-2" />
                        Назад в библиотеку
                    </Link>

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

                    <CardsList
                        cards={allCardsForList}
                        group={groupForHeader}
                        isSubscriptionView={true}
                        blurAfterIndex={!user ? 3 : undefined}
                        onCardClick={handleCardClick}
                        isAuthenticated={!!user}
                    />

                    {user && (
                        <MotivationCard
                            animated="scale"
                            animatedDelay={1}
                            icon={Trophy}
                            label={groupDto.IsSubscribed ? "Колода добавлена!" : "Добавить в свои колоды"}
                            description={groupDto.IsSubscribed
                                ? "Эта колода находится в вашей библиотеке. Вы можете переходить к изучению."
                                : "Сохраните эту колоду себе, чтобы отслеживать прогресс изучения и повторять сложные карточки."
                            }
                            textIcon={BowArrowIcon}
                            gradient={groupDto.GroupColor || ""}
                            delay={0.2}
                            className="mt-8 cursor-pointer"
                            onClick={handleToggleSubscription}
                        />
                    )}
                </div>
            </main>

            <Footer />
        </div>
    );
}