import { Hono } from "hono";
import { logger } from "hono/logger";
import { serveStatic } from "hono/bun";
import { loadRoutes } from "./util/routing";
import * as path from "path";
import dotenv from 'dotenv';
dotenv.config();


const app = new Hono();

app.use(logger())

await loadRoutes(path.join(__dirname, "./routes"), app)


app.use(
    "/public/*",
    serveStatic({
      root: "./src",
    })
  );

app.all("*", async (c) => {
    return c.text("404!!!!");
});

Bun.serve({
   // development: false,
    fetch: app.fetch,
    port: process.env.PORT || 2222
});

console.log(`FortDashboard is hosted at http://127.0.0.1:${process.env.PORT}`);

export default app;
 