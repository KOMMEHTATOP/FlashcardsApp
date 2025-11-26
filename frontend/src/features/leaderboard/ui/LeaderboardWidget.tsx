import { useState, useRef, useEffect } from 'react';
import { useLeaderboard } from '@/hooks/useLeaderboard';
import { Trophy, Medal, ChevronDown, ChevronUp, GripHorizontal } from 'lucide-react';
import { useData } from '@/context/DataContext';
import { motion, AnimatePresence } from 'framer-motion';

export const LeaderboardWidget = () => {
    const { data, loading } = useLeaderboard();
    const { user } = useData();
    const currentUserId = user?.Id;
    const [isCollapsed, setIsCollapsed] = useState(true);
    const constraintsRef = useRef(null);
    const [isMobile, setIsMobile] = useState(window.innerWidth < 640);

    useEffect(() => {
        const handleResize = () => setIsMobile(window.innerWidth < 640);
        window.addEventListener('resize', handleResize);
        return () => window.removeEventListener('resize', handleResize);
    }, []);

    if (loading && !data) return null;
    if (!data || !data.TopList) return null;

    const topList = data.TopList.slice(0, 10);
    const currentUserEntry = data.TopList.length > 10 ? data.TopList[10] : null;
    const myEntry = topList.find(u => u.UserId === currentUserId) || currentUserEntry;

    const getRankIcon = (pos: number) => {
        if (pos === 1) return <Trophy className="w-4 h-4 text-yellow-400" />;
        if (pos === 2) return <Medal className="w-4 h-4 text-gray-300" />;
        if (pos === 3) return <Medal className="w-4 h-4 text-amber-600" />;
        return <span className="font-bold text-xs opacity-60 w-4 text-center">{pos}</span>;
    };

    const UserRow = ({ entry, isMe = false, simple = false }: { entry: any, isMe?: boolean, simple?: boolean }) => (
        <div className={`flex items-center justify-between p-2 rounded-lg transition-all duration-200 ${
            isMe ? 'bg-primary/10 border border-primary/20' : 'hover:bg-base-200'
        }`}>
            <div className="flex items-center gap-3 overflow-hidden flex-1">
                <div className="flex-shrink-0 flex justify-center w-6">
                    {getRankIcon(entry.Position)}
                </div>
                <div className="flex flex-col min-w-0 flex-1">
                    <span className={`truncate font-medium text-sm ${isMe ? 'text-primary' : ''}`} title={entry.Login}>
                        {entry.Login}
                    </span>
                    {isMe && !simple && <span className="text-[10px] leading-none opacity-60">(Вы)</span>}
                </div>
            </div>
            <div className="flex-shrink-0 ml-2">
                <span className={`text-xs font-bold px-2 py-1 rounded-full ${
                    isMe ? 'bg-primary text-primary-content' : 'bg-base-300 opacity-70'
                }`}>
                    {entry.TotalRating}
                </span>
            </div>
        </div>
    );

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
                        Лидерборд
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
                                {myEntry ? (
                                    <UserRow entry={myEntry} isMe={true} simple={true} />
                                ) : (
                                    <div className="text-xs text-center py-1 opacity-50">Войдите в рейтинг</div>
                                )}
                            </motion.div>
                        ) : (
                            <motion.div
                                key="expanded"
                                initial={{ opacity: 0, height: 0 }}
                                animate={{ opacity: 1, height: 'auto' }}
                                exit={{ opacity: 0, height: 0 }}
                                className="flex flex-col gap-1 pt-2 max-h-[60vh] overflow-y-auto custom-scrollbar"
                            >
                                {topList.map((entry) => (
                                    <UserRow key={entry.UserId} entry={entry} isMe={entry.UserId === currentUserId} />
                                ))}

                                {currentUserEntry && !topList.some(u => u.UserId === currentUserId) && (
                                    <>
                                        <div className="divider my-1 text-xs opacity-50 h-4">...</div>
                                        <UserRow entry={currentUserEntry} isMe={true} />
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