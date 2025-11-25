import { useEffect, useState } from "react";
import { Helmet } from "react-helmet-async";
import { ShieldAlert, RefreshCw, Search } from "lucide-react";
import apiFetch from "@/utils/apiFetch";
import type { AdminUserDto } from "@/types/types";
import { useData } from "@/context/DataContext";

export default function AdminPage() {
    const { user } = useData();

    const [users, setUsers] = useState<AdminUserDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [searchTerm, setSearchTerm] = useState("");

    // Функция загрузки данных
    const fetchUsers = async () => {
        setLoading(true);
        setError(null);
        try {
            const res = await apiFetch.get<AdminUserDto[]>("/Admin/users");
            setUsers(res.data);
        } catch (err: any) {
            console.error("Ошибка загрузки пользователей:", err);
            // Если 403 Forbidden - значит не админ
            if (err.response?.status === 403) {
                setError("У вас нет прав администратора.");
            } else {
                setError("Не удалось загрузить список пользователей.");
            }
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        // Загружаем данные при монтировании или смене пользователя
        fetchUsers();
    }, [user]);

    // Фильтрация на клиенте
    const filteredUsers = users.filter(u =>
        u.Login.toLowerCase().includes(searchTerm.toLowerCase()) ||
        u.Email.toLowerCase().includes(searchTerm.toLowerCase())
    );

    // Форматирование даты
    const formatDate = (dateString: string) => {
        if (!dateString || dateString.startsWith("0001")) return "-";
        return new Date(dateString).toLocaleDateString("ru-RU", {
            day: "2-digit", month: "2-digit", year: "numeric",
            hour: "2-digit", minute: "2-digit"
        });
    };

    return (
        <div className="min-h-screen bg-base-200 p-4 md:p-8">
            <Helmet>
                <title>Панель Администратора | FlashcardsLoop</title>
            </Helmet>

            <div className="max-w-7xl mx-auto">
                {/* Заголовок */}
                <div className="flex flex-col md:flex-row justify-between items-center mb-8 gap-4">
                    <h1 className="text-3xl font-bold flex items-center gap-3">
                        <ShieldAlert className="w-8 h-8 text-primary" />
                        Управление пользователями
                        <span className="badge badge-neutral">{users.length}</span>
                    </h1>

                    <div className="flex gap-2 w-full md:w-auto">
                        <div className="relative flex-1 md:w-64">
                            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 opacity-50" />
                            <input
                                type="text"
                                placeholder="Поиск по логину или email..."
                                className="input input-bordered pl-10 w-full"
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                            />
                        </div>
                        <button onClick={fetchUsers} className="btn btn-square btn-ghost">
                            <RefreshCw className={`w-5 h-5 ${loading ? "animate-spin" : ""}`} />
                        </button>
                    </div>
                </div>

                {error ? (
                    <div className="alert alert-error">
                        <span>{error}</span>
                    </div>
                ) : (
                    <div className="overflow-x-auto bg-base-100 rounded-xl shadow-xl">
                        <table className="table table-zebra w-full">
                            <thead>
                                <tr className="bg-base-300 text-base-content/70">
                                    <th>Пользователь</th>
                                    <th>Роль</th>
                                    <th>Статистика (Группы / Карты)</th>
                                    <th>Рейтинг (XP)</th>
                                    <th>Регистрация</th>
                                    <th>Последний вход</th>
                                </tr>
                            </thead>
                            <tbody>
                                {loading ? (
                                    // Скелетон загрузки
                                    [...Array(5)].map((_, i) => (
                                        <tr key={i} className="animate-pulse">
                                            <td colSpan={6} className="h-16 bg-base-200/50"></td>
                                        </tr>
                                    ))
                                ) : filteredUsers.length > 0 ? (
                                    filteredUsers.map((u) => (
                                        <tr key={u.Id} className="hover">
                                            <td>
                                                <div className="flex items-center gap-3">
                                                    <div className="avatar placeholder">
                                                        <div className="bg-neutral text-neutral-content rounded-full w-10">
                                                            <span>{u.Login.charAt(0).toUpperCase()}</span>
                                                        </div>
                                                    </div>
                                                    <div>
                                                        <div className="font-bold">{u.Login}</div>
                                                        <div className="text-sm opacity-50">{u.Email}</div>
                                                    </div>
                                                </div>
                                            </td>
                                            <td>
                                                {u.Role === "Admin" ? (
                                                    <span className="badge badge-error badge-outline gap-1">
                                                        <ShieldAlert className="w-3 h-3" /> Admin
                                                    </span>
                                                ) : (
                                                    <span className="badge badge-ghost">User</span>
                                                )}
                                            </td>
                                            <td>
                                                <div className="flex gap-2">
                                                    <div className="badge badge-info badge-lg gap-1" title="Создано групп">
                                                        📁 {u.GroupsCount}
                                                    </div>
                                                    <div className="badge badge-warning badge-lg gap-1" title="Создано карточек">
                                                        🃏 {u.CardsCount}
                                                    </div>
                                                </div>
                                            </td>
                                            <td className="font-mono font-bold text-primary">
                                                {u.TotalRating.toLocaleString()} XP
                                            </td>
                                            <td className="text-sm">{formatDate(u.CreatedAt)}</td>
                                            <td className="text-sm">{formatDate(u.LastLogin)}</td>
                                        </tr>
                                    ))
                                ) : (
                                    <tr>
                                        <td colSpan={6} className="text-center py-10 opacity-50">
                                            Пользователи не найдены
                                        </td>
                                    </tr>
                                )}
                            </tbody>
                        </table>
                    </div>
                )}
            </div>
        </div>
    );
}