import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'
import path from 'path';

export default defineConfig({
    server: {
        allowedHosts: [
            'flashcardsloop.org',
            'api.flashcardsloop.org',
        ],
        proxy: {
            '/api': {
                target: 'http://localhost:5153',
                changeOrigin: true,
                secure: false,
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
        // Увеличиваем лимит предупреждения до 1000кб (чтобы не спамил, если vendor-файл будет 600кб)
        chunkSizeWarningLimit: 1000,
        rollupOptions: {
            output: {
                // Стратегия разделения кода
                manualChunks(id) {
                    if (id.includes('node_modules')) {
                        // 1. React и Роутинг (нужны везде)
                        if (id.includes('react') || id.includes('react-dom') || id.includes('react-router')) {
                            return 'vendor-react';
                        }
                        // 2. Тяжелый редактор текста (Tiptap + ProseMirror)
                        if (id.includes('@tiptap') || id.includes('prosemirror') || id.includes('@remirror')) {
                            return 'vendor-editor';
                        }
                        // 3. Анимации и UI библиотеки
                        if (id.includes('framer-motion') || id.includes('gsap') || id.includes('@dnd-kit') || id.includes('daisyui')) {
                            return 'vendor-ui';
                        }
                        // 4. Все остальные библиотеки (axios, lucide и прочее)
                        return 'vendor-utils';
                    }
                }
            }
        }
    }
})