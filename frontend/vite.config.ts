import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'
import path from 'path';

export default defineConfig({
    server: {
        // --- ИСПРАВЛЕНИЕ: Фиксируем порт, чтобы избежать конфликтов с Docker/Windows ---
        port: 3000,       // Используем порт 3000 вместо 5173
        strictPort: true, // Если 3000 занят, Vite упадет с ошибкой (чтобы мы знали), а не прыгнет на другой порт
        host: true,       // Разрешает прослушивание на всех адресах (полезно для Docker/WSL)
        // ---------------------------------------------------------------------------------

        allowedHosts: [
            'flashcardsloop.org',
            'api.flashcardsloop.org',
        ],
        proxy: {
            '/api': {
                target: 'http://127.0.0.1:8091',
                changeOrigin: true,
                secure: false,
                ws: true
            }
        }
    },
    plugins: [react(), tailwindcss()],
    resolve: {
        alias: {
            "@": path.resolve(__dirname, "./src"),
        },
    },
    // настройки сборки
    build: {
        // Увеличиваем лимит предупреждения до 1000кб
        chunkSizeWarningLimit: 1000,
        rollupOptions: {
            output: {
                // Стратегия разделения кода
                manualChunks(id) {
                    if (id.includes('node_modules')) {
                        // 1. React и Роутинг
                        if (id.includes('react') || id.includes('react-dom') || id.includes('react-router')) {
                            return 'vendor-react';
                        }
                        // 2. Тяжелый редактор текста
                        if (id.includes('@tiptap') || id.includes('prosemirror') || id.includes('@remirror')) {
                            return 'vendor-editor';
                        }
                        // 3. Анимации и UI библиотеки
                        if (id.includes('framer-motion') || id.includes('gsap') || id.includes('@dnd-kit') || id.includes('daisyui')) {
                            return 'vendor-ui';
                        }
                        // 4. Все остальные библиотеки
                        return 'vendor-utils';
                    }
                }
            }
        }
    }
})