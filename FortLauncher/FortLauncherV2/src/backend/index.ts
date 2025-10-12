import { app, BrowserWindow, dialog, ipcMain, Menu } from "electron";
import axios from "axios";
import path, { join } from "path";
import icon from '../../resources/icon.png?asset'
import user, { login } from "./login";
import { saveTokenToIni } from "./IniConfig";
import {
  existsSync,
  lstatSync,
  mkdirSync,
  readFileSync,
  writeFileSync,
} from "fs";
import { getBuildVersion } from "./VersionSearcher";
import { handleBuildConfig, TempBuildData } from "./JsonConfig";
import { FortniteDetect } from "./FortniteDetect";
import { Worker, WorkerOptions } from 'worker_threads'
let mainWindow: BrowserWindow | null;

function createWindow(): void {
  console.log(process.env.VITE_BACKEND_URL);
  console.log(__dirname);
  const preloadPath = join(app.getAppPath(), 'out/main/preload.js')
  console.log("Preload script path:", preloadPath);

  if (!mainWindow) {
    mainWindow = new BrowserWindow({
      width: 1170,
      height: 720,
      show: false,
      ...(process.platform === "linux" ? { icon } : {}),
      webPreferences: {
        preload: preloadPath,
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
      mainWindow.loadFile(join(app.getAppPath(), "out/renderer/index.html"));
      Menu.setApplicationMenu(null);
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
        properties: ["openDirectory"],
      });

      if (result.canceled) {
        return null; // might need a message instweaD?
      } else {
        const selectedPath = result.filePaths[0];
        const fortniteGamePath = path.join(selectedPath, "FortniteGame");
        const EnginePath = path.join(selectedPath, "Engine");

        const hasFortniteGame =
          existsSync(fortniteGamePath) &&
          lstatSync(fortniteGamePath).isDirectory();
        const hasEngine =
          existsSync(EnginePath) && lstatSync(EnginePath).isDirectory();

        if (hasFortniteGame && hasEngine) return selectedPath;
        else return "Error~~";
      }
    });

    ipcMain.handle("fortlauncher:addpath", async (_, { PathValue }) => {
      console.log(PathValue);
      const fortniteGamePath = path.join(PathValue, "FortniteGame");
      const enginePath = path.join(PathValue, "Engine");
      const hasFortniteGame =
        existsSync(fortniteGamePath) &&
        lstatSync(fortniteGamePath).isDirectory();
      const hasEngine =
        existsSync(enginePath) && lstatSync(enginePath).isDirectory();

      if (hasFortniteGame && hasEngine) {
        const engineExecutablePath = path.join(
          PathValue,
          "Engine\\Binaries\\Win64\\CrashReportClient.exe"
        );
        var result = "ERROR";
        if (existsSync(engineExecutablePath)) {
          // CrashReportClient is way smaller
          result = await getBuildVersion(engineExecutablePath);
        } else {
          const engineExecutablePath = path.join(
            PathValue,
            "Engine\\Binaries\\Win32\\CrashReportClient.exe"
          );
          if (existsSync(engineExecutablePath)) {
            result = await getBuildVersion(engineExecutablePath);
          } else {
            const gameExecutablePath = path.join(
              PathValue,
              "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping_BE.exe"
            );

            result = await getBuildVersion(gameExecutablePath);
          }
        }

        if (result == "ERROR") {
          return {
            error: true,
            data: {},
          };
        } else {
          var VersionID = Buffer.from(result, "utf-8").toString("base64");
          console.log("BUILD ID IS " + VersionID);
          TempBuildData.VersionID = result;
          TempBuildData.buildID = VersionID;
          TempBuildData.buildPath = PathValue;
          TempBuildData.played = "0";

          return {
            error: false,
            data: {
              VersionID: result,
              buildID: VersionID,
              buildPath: PathValue,
              played: "0",
            },
          };
        }
      } else {
        return {
          error: true,
          data: {},
        };
      }
    });

    ipcMain.on("fortlauncher:launchgame", (_, { gameExePath }) => {
      setImmediate(() => {
        const fortlauncherFolderPath = join(app.getPath("userData"), "FortLauncher");
        const dllPath = join(fortlauncherFolderPath, "FortCurl.dll");

        if (!existsSync(fortlauncherFolderPath)) {
          mkdirSync(fortlauncherFolderPath, { recursive: true });
        }

        mainWindow?.webContents.send("gameStatus", {
          Launching: false,
          Type: "Message",
          Message: "Launching",
        });

        const workerOptions: WorkerOptions = {
          workerData: { gameExePath, dllPath, user: user },
        };

        const worker = new Worker(
          path.join(__dirname, "_gameWorker.js"),
          workerOptions
        );

        worker.on("message", (message) => {
          if (message.status === "success") {
            console.log("Game launched successfully!");
            mainWindow?.webContents.send("gameStatus", {
              Launching: false,
              Type: "",
            });
          } else {
            console.error("Error launching game:", message.message);
            mainWindow?.webContents.send("gameStatus", {
              Launching: false,
              Type: "Error",
            });
          }
        });

        worker.on("error", (error) => {
          console.error("Worker error:", error);
          mainWindow?.webContents.send("gameStatus", {
            Launching: false,
            Type: "Error",
          });
        });

        worker.on("exit", (code) => {
          if (code !== 0) {
            console.error(`Worker stopped with exit code ${code}`);
          }
        });
      });

      console.log("PORN!");
    });

    ipcMain.handle("fortlauncher:addpathV2", async (_) => {
      var Response = await handleBuildConfig(
        TempBuildData.buildID,
        TempBuildData.VersionID,
        TempBuildData.buildPath
      );

      console.log(Response);

      return Response;
    });

    ipcMain.handle("fortlauncher:get-builds", async () => {
      try {
        const fortlauncherFolderPath = join(app.getPath("userData"), "FortLauncher");
        const FilePath = join(fortlauncherFolderPath, "builds.json");
        if (!existsSync(FilePath)) {
          writeFileSync(FilePath, JSON.stringify([]));
        }
        const builds = JSON.parse(readFileSync(FilePath, "utf-8"));
        return builds;
      } catch (error) {
        console.error("Error reading build.json file!", error);
        return [];
      }
    });

    ipcMain.handle(
      "fortlauncher:getBuildVersion",
      async (_, buildString: string) => {
        return FortniteDetect(buildString);
      }
    );

    ipcMain.handle("fortlauncher:login~email", async (_, s) => {
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
