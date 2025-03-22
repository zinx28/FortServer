import { app, BrowserWindow, dialog, ipcMain } from "electron";
import axios from "axios";
import path, { join } from "path";
import icon from "../public/vite.svg";
import { login } from "./login";
import { saveTokenToIni } from "./IniConfig";
import { existsSync, lstatSync } from "fs";

let mainWindow: BrowserWindow | null;

function createWindow(): void {
  console.log(__dirname);
  const preloadPath = join(__dirname, "preload.mjs");
  console.log("Preload script path:", preloadPath);

  if (!mainWindow) {
    mainWindow = new BrowserWindow({
      width: 1170,
      height: 720,
      show: false,
      ...(process.platform === "linux" ? { icon } : {}),
      webPreferences: {
        preload: join(__dirname, "preload.js"),
        contextIsolation: true,
        nodeIntegration: false,
        sandbox: false,
      },
      resizable: false,
    });

    mainWindow.on("ready-to-show", () => {
      mainWindow!.show();
    });

    mainWindow.on("closed", () => {
      mainWindow = null;
    });

    console.log(process.env["ELECTRON_RENDERER_URL"]);
    if (process.env.NODE_ENV === "development") {
      mainWindow.loadURL("http://localhost:5173");
    } else {
      mainWindow.loadFile(join(__dirname, "../index.html"));
    }

    ipcMain.handle("fortlauncher:ping", async () => {
      console.log(process.env.VITE_BACKEND_URL);
      try {
        await axios.get(`${process.env.VITE_BACKEND_URL}`).then((response) => {
          if (response.data) {
            console.log(response.data);
            // up!
            mainWindow?.webContents.send("update-status", { status: "online" });
          } else {
            mainWindow?.webContents.send("update-status", {
              status: "offline",
            });
          }
        });
      } catch (err) {
        console.error(err);
        mainWindow?.webContents.send("update-status", { status: "offline" });
      }
      return "am i the rizzler?";
    });

    ipcMain.handle("fortlauncher:login", async () => {
      return login(mainWindow!);
    });

    ipcMain.handle("fortlauncher:openfile", async () => {
        const result = await dialog.showOpenDialog({
          properties: ['openDirectory']
          
        })

        if(result.canceled){
          return null; // might need a message instweaD?
        }
        else {
          const selectedPath = result.filePaths[0];
          const fortniteGamePath = path.join(selectedPath, "FortniteGame")
          const EnginePath = path.join(selectedPath, "Engine")

          const hasFortniteGame = existsSync(fortniteGamePath) && lstatSync(fortniteGamePath).isDirectory();
          const hasEngine = existsSync(EnginePath) && lstatSync(EnginePath).isDirectory();

          if(hasFortniteGame && hasEngine)
            return selectedPath;
          else
            return 'Error~~'
        }
    });

    ipcMain.handle("fortlauncher:login~email", async (e, s) => {
      console.log(process.env.VITE_BACKEND_URL);
      try {
        console.log("f " + JSON.stringify(s));
        const response = await axios.post(
          `${process.env.VITE_BACKEND_URL}/launcher/api/v1/login`,
          s,
          {
            headers: {
              "Content-Type": "multipart/form-data",
            },
          }
        );

        if (response.data) {
          console.log("g");
          console.log(response.data);
          if (response.data.token) {
            saveTokenToIni(response.data.token);
            var LoginData = await login(mainWindow!, true);
            console.log("YEAH");
            console.log(LoginData);
            if (LoginData) return LoginData;
            else
              return {
                message: "Failed to login",
                error: true,
              };
          }
        }

        if (response.status) {
          console.log(response.status);
        }
      } catch (err: any) {
        if (err.response) {
          console.log("Daata:", err.response.data);
          return {
            message: err.response.data.message,
            error: true,
          };
        }
        return {
          message: "failed to login!",
          error: true,
        };
      }

      return {
        message: "uhgm!",
        error: true,
      };
    });
  }
}

app.on("window-all-closed", () => {
  if (process.platform !== "darwin") {
    app.quit();
    mainWindow = null;
  }
});

app.whenReady().then(() => {
  createWindow();

  app.on("activate", () => {
    if (BrowserWindow.getAllWindows().length === 0) {
      createWindow();
    }
  });
});
