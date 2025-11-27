import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'
import path from 'path';

export default defineConfig({
    server: {
        port: 3000,       
        strictPort: true, 
        host: true,       

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
    build: {
        chunkSizeWarningLimit: 1000,
        rollupOptions: {
            output: {
                manualChunks(id) {
                    if (id.includes('node_modules')) {
                        if (id.includes('react') || id.includes('react-dom') || id.includes('react-router')) {
                            return 'vendor-react';
                        }
                        if (id.includes('@tiptap') || id.includes('prosemirror') || id.includes('@remirror')) {
                            return 'vendor-editor';
                        }
                        if (id.includes('framer-motion') || id.includes('gsap') || id.includes('@dnd-kit') || id.includes('daisyui')) {
                            return 'vendor-ui';
                        }
                        return 'vendor-utils';
                    }
                }
            }
        }
    }
})