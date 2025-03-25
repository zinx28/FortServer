import { app } from "electron";
import { existsSync, mkdirSync, readFileSync, writeFileSync } from "fs";
import { join } from "path";
// copied from luna, i want to get this launcher to be functional as fast as i can (yoie)
interface BuildConfig {
  VersionID: string;
  buildPath: string;
  buildID: string;
  played: string;
}

export let TempBuildData: BuildConfig = {
  VersionID: "",
  buildPath: "",
  buildID: "",
  played: "",
};

export async function handleBuildConfig(
  buildID: string,
  VersionID: string,
  buildPath: string
): Promise<string> {
  const fortlauncherFolderPath = join(app.getPath("userData"), "FortLauncher");
  const FilePath = join(fortlauncherFolderPath, "builds.json");

  if (!existsSync(fortlauncherFolderPath)) {
    mkdirSync(fortlauncherFolderPath, { recursive: true });
  }

  let jsonArray: BuildConfig[];

  if (existsSync(FilePath)) {
    const jsonData = await readFileSync(FilePath, "utf-8");
    jsonArray = JSON.parse(jsonData) as BuildConfig[];
  } else {
    jsonArray = [];
    await writeFileSync(FilePath, JSON.stringify(jsonArray));
    console.log("Created File Path -> " + FilePath);
  }

  const existingEntry = jsonArray.find((item) => item.buildID === buildID);

  if (existingEntry) {
    console.log("BUILD ID IS " + existingEntry.buildID);
    console.log("BUILD NAME IS " + existingEntry.VersionID);

    existingEntry.buildPath = buildPath;
    existingEntry.played = new Date().toISOString();

    await writeFileSync(FilePath, JSON.stringify(jsonArray, null, 2));
    return "already~build";
  } else {
    console.log("BUILD ID IS " + buildID);

    const buildConfig: BuildConfig = {
      VersionID,
      buildPath,
      buildID,
      played: new Date().toISOString(),
    };

    jsonArray.unshift(buildConfig);

    await writeFileSync(FilePath, JSON.stringify(jsonArray, null, 2));
    return "saved!";
  }

  return "weird";
}
