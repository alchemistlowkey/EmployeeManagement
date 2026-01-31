// vite.config.js
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  root: 'src',                    // ← important: Vite starts from src/
  build: {
    outDir: '../wwwroot/dist',    // ← outputs to wwwroot
    emptyOutDir: true,
    manifest: true                // helps in production if needed
  },
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'https://localhost:5181',   // ← your ASP.NET port (check launchSettings.json)
        changeOrigin: true,
        secure: false
      }
    }
  }
})