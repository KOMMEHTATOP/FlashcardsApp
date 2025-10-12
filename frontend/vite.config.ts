import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
  server: {
    allowedHosts: [
      'f0b03934ac19.ngrok-free.app'
    ]
  },
  plugins: [react(), tailwindcss()],
})
