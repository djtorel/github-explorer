import { defineConfig } from "vite";
import { svelte } from "@sveltejs/vite-plugin-svelte";
import tailwindcss from "@tailwindcss/vite";

export default defineConfig({
	plugins: [svelte(), tailwindcss()],
	server: {
		port: 5173,
		proxy: {
			"/api": {
				target: "http://localhost:5115",
				changeOrigin: true,
			},
		},
	},
	build: {
		outDir: "../GitHubExplorer.Api/wwwroot",
		emptyOutDir: true,
	},
});
