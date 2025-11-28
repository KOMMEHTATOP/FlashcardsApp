import { useState, useRef, useEffect } from 'react';
import { useLeaderboard } from '@/hooks/useLeaderboard';
import { Trophy, Medal, ChevronDown, ChevronUp, GripHorizontal } from 'lucide-react';
import { useData } from '@/context/DataContext';
import { motion, AnimatePresence } from 'framer-motion';

const RankIcon = ({ pos }: { pos: number }) => {
    if (pos === 1) return <Trophy className="w-4 h-4 text-yellow-400" />;
    if (pos === 2) return <Medal className="w-4 h-4 text-gray-300" />;
    if (pos === 3) return <Medal className="w-4 h-4 text-amber-600" />;
    return <span className="font-bold text-xs opacity-60 w-4 text-center">{pos}</span>;
};

const UserRow = ({ entry, isMe = false, simple = false }: { entry: any, isMe?: boolean, simple?: boolean }) => {
    if (!entry) return null;

    return (
        <div className={`flex items-center justify-between p-2 rounded-lg transition-all duration-200 ${
            isMe ? 'bg-primary/10 border border-primary/20' : 'hover:bg-base-200'
        }`}>
            <div className="flex items-center gap-3 overflow-hidden flex-1">
                <div className="flex-shrink-0 flex justify-center w-6">
                    <RankIcon pos={entry.Position} />
                </div>
                <div className="flex flex-col min-w-0 flex-1">
                    <span className={`truncate font-medium text-sm ${isMe ? 'text-primary' : ''}`} title={entry.Login}>
                        {entry.Login || 'Без имени'}
                    </span>
                    {isMe && !simple && <span className="text-[10px] leading-none opacity-60">(Вы)</span>}
                </div>
            </div>
            <div className="flex-shrink-0 ml-2">
                <span className={`text-xs font-bold px-2 py-1 rounded-full ${
                    isMe ? 'bg-primary text-primary-content' : 'bg-base-300 opacity-70'
                }`}>
                    {entry.TotalRating ?? 0}
                </span>
            </div>
        </div>
    );
};

export const LeaderboardWidget = () => {
    const { data, loading } = useLeaderboard();
    const { user } = useData();
    const [cachedTopList, setCachedTopList] = useState<any[]>([]);
    const currentUserId = user?.Id;
    const [isCollapsed, setIsCollapsed] = useState(true);
    const constraintsRef = useRef(null);
    const [isMobile, setIsMobile] = useState(window.innerWidth < 640);

    useEffect(() => {
        if (data && data.TopList && Array.isArray(data.TopList)) {
            setCachedTopList(data.TopList);
        }
    }, [data]);

    useEffect(() => {
        const handleResize = () => setIsMobile(window.innerWidth < 640);
        window.addEventListener('resize', handleResize);
        return () => window.removeEventListener('resize', handleResize);
    }, []);

    const activeList = (data && data.TopList) ? data.TopList : cachedTopList;
    const hasData = activeList && activeList.length > 0;

    if (loading && !hasData) return null; 
    if (!hasData) return null; 

    const topList = activeList.slice(0, 10);
    const currentUserEntry = activeList.length > 10 ? activeList[10] : null;

    const myEntry = topList.find((u: any) => u.UserId === currentUserId) ||
        (currentUserEntry && currentUserEntry.UserId === currentUserId ? currentUserEntry : null);

    return (
        <>
            <div ref={constraintsRef} className="fixed inset-0 pointer-events-none z-[50]" />

            <motion.div
                drag={!isMobile}
                dragConstraints={constraintsRef}
                dragMomentum={false}
                className="fixed z-[51] pointer-events-auto shadow-[0_-4px_20px_rgba(0,0,0,0.1)] rounded-t-xl overflow-hidden border-x border-t border-base-300 bg-base-100/95 backdrop-blur-sm 
                           inset-x-0 bottom-0 w-full 
                           sm:inset-x-auto sm:right-6 sm:left-auto sm:w-80"
                animate={{
                    height: 'auto',
                }}
                transition={{ type: "spring", stiffness: 300, damping: 30 }}
            >
                <div
                    className="bg-base-200 p-2 flex justify-between items-center cursor-pointer select-none active:bg-base-300 border-b border-base-300/50"
                    onClick={() => setIsCollapsed(!isCollapsed)}
                >
                    <div className="flex items-center gap-2 text-xs font-bold uppercase opacity-70 pl-1">
                        {!isMobile && <GripHorizontal className="w-4 h-4 cursor-move" onPointerDown={(e) => e.stopPropagation()} />}
                        ТОП Авторов
                    </div>
                    <div className="btn btn-circle btn-ghost btn-xs hover:bg-base-300">
                        {isCollapsed ? <ChevronUp className="w-4 h-4" /> : <ChevronDown className="w-4 h-4" />}
                    </div>
                </div>

                <div className="px-2 pb-2 bg-base-100">
                    <AnimatePresence mode="wait">
                        {isCollapsed ? (
                            <motion.div
                                key="collapsed"
                                initial={{ opacity: 0, height: 0 }}
                                animate={{ opacity: 1, height: 'auto' }}
                                exit={{ opacity: 0, height: 0 }}
                                className="pt-2"
                            >
                                <motion.div layout>
                                    {myEntry ? (
                                        <UserRow entry={myEntry} isMe={true} simple={true} />
                                    ) : (
                                        <div className="text-xs text-center py-1 opacity-50">
                                            {loading ? 'Обновление...' : 'Войдите в рейтинг'}
                                        </div>
                                    )}
                                </motion.div>
                            </motion.div>
                        ) : (
                            <motion.div
                                key="expanded"
                                initial={{ opacity: 0, height: 0 }}
                                animate={{ opacity: 1, height: 'auto' }}
                                exit={{ opacity: 0, height: 0 }}
                                className="flex flex-col gap-1 pt-2 max-h-[60vh] overflow-y-auto custom-scrollbar"
                            >
                                {topList.map((entry: any) => (
                                    <motion.div key={entry.UserId || Math.random()} layout>
                                        <UserRow entry={entry} isMe={entry.UserId === currentUserId} />
                                    </motion.div>
                                ))}

                                {currentUserEntry && !topList.some((u: any) => u.UserId === currentUserId) && (
                                    <>
                                        <div className="divider my-1 text-xs opacity-50 h-4">...</div>
                                        <motion.div layout>
                                            <UserRow entry={currentUserEntry} isMe={true} />
                                        </motion.div>
                                    </>
                                )}
                            </motion.div>
                        )}
                    </AnimatePresence>
                </div>
            </motion.div>
        </>
    );
};