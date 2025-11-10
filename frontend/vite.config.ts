import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
    server: {
        allowedHosts: [
            'flashcardsloop.org',
            'api.flashcardsloop.org',
        ]
    },
    plugins: [react(), tailwindcss()],
})