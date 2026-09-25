import { defineConfig } from "vite";
import react from "@vitejs/plugin-react-swc";
import mkcert from "vite-plugin-mkcert";

// https://vite.dev/config/
export default defineConfig({
  build:{
    outDir:'../API/wwwroot',
    emptyOutDir:true,
    rollupOptions:{
      output:{
        manualChunks:{
          react:['react','react-dom','react-router-dom'],
          mui:['@mui/material','@mui/icons-material','@mui/lab'],
          redux:['@reduxjs/toolkit','react-redux'],
          stripe:['@stripe/stripe-js','@stripe/react-stripe-js'],
          forms:['react-hook-form','@hookform/resolvers','zod']
        }
      }
    }
  },
  server: {
    port: 3000,
  },
  plugins: [react(), mkcert()],
});
