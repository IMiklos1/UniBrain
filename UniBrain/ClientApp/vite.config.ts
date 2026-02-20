import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
    plugins: [react()],
    server: {
        port: 5173, // Fix port a fejlesztéshez
        proxy: {
            // Ha a Reactban a "/api"-ra hivatkozol, azt átirányítja a .NET-re
            '/api': {
                target: 'http://localhost:5000', // Ellenőrizd: A .NET ezen a porton fut? (Properties/launchSettings.json)
                changeOrigin: true,
                secure: false,
            },
            '/uploads': {
                target: 'http://localhost:5000',
                changeOrigin: true,
                secure: false,
            }
        }
    },
    build: {
        // A kész buildet egy szinttel feljebb, a .NET wwwroot mappájába teszi!
        outDir: '../wwwroot',
        emptyOutDir: true, // Törli a wwwroot tartalmát build előtt
    }
})
