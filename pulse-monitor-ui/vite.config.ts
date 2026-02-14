import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 3000,
    proxy: {
      '/api': { target: 'https://localhost:7166', changeOrigin: true, secure: false },
      '/hubs': { target: 'https://localhost:7166', changeOrigin: true, secure: false, ws: true },
    },
  },
})
