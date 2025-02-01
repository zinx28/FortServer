/// <reference types="vite/client" />

interface ImportMetaEnv {
  VITE_API_URL: string;
  VITE_PORT: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
