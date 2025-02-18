import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import dotenv from 'dotenv';
dotenv.config();

console.log("HOST: " + process.env.DashboardHost);

const port = parseInt(process.env.VITE_PORT || '5173' , 10);
// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  server: {
    host: process.env.DashboardHost,
    port: port
  },
})
