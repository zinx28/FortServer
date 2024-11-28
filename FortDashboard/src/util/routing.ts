import { Hono } from "hono";
import * as fs from "fs";
import * as path from "path";

export async function loadRoutes(dir: string, app: Hono): Promise<void> {
  const routeFiles = await fs.readdirSync(dir);

  if (routeFiles) {
    console.log("Loading Routes")
    const routePromises = routeFiles.map((file) => {
      const routePath = path.join(dir, file);
      if (fs.lstatSync(routePath).isFile() && file.endsWith(".ts")) {
        return import(routePath)
          .then((routeModule) => {
            if (routeModule.default) {
              routeModule.default(app);
              console.log(`Loaded ${file}`);
            }
          })
          .catch((err) => {
            console.error(`Failed to load route ${file}:`, err);
          });
      }
      return Promise.resolve();
    });
    await Promise.all(routePromises);
  }else {
     console.log("FAILED TO LOAD ROUTES")
  }
}
