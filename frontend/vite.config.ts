import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

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
})