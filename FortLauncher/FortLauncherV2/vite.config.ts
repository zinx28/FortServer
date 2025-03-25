import { defineConfig } from 'vite'
import path from 'node:path'
import electron from 'vite-plugin-electron/simple'
import vue from '@vitejs/plugin-vue'
import dotenv from 'dotenv';
dotenv.config();
console.log("BUILDING");
console.log(process.env.VITE_BACKEND_URL);
// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    electron({
      main: {
        entry: 'backend/index.ts',
        vite: {
          build: {
            outDir: 'out/main',
          },
          define: {
            'process.env.VITE_BACKEND_URL': JSON.stringify(process.env.VITE_BACKEND_URL),
          }
        },
      },
      preload: {
        input: path.join(__dirname, 'backend/preload.ts'),
        vite: {
          build: {
            outDir: 'out/main',
          }
        }
      }
    }),
  ],
  build: {
    rollupOptions: {
      output: {
        format: 'cjs',
      },
    },
    outDir: "out"
  }
})
