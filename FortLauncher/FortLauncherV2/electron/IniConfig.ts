import { app } from 'electron'
import { readFileSync, writeFileSync, existsSync, mkdirSync } from 'fs'
import ini from 'ini'
import { join } from 'path'
const lunaFolderPath = join(app.getPath('userData'), 'FortLauncher')
const configFilePath = join(lunaFolderPath, 'config.ini')

const checkIfExisests = async () => {
  if (!existsSync(lunaFolderPath)) {
    mkdirSync(lunaFolderPath)
  }

  console.log(lunaFolderPath)

  if (!existsSync(configFilePath)) {
    const defaultConfig = {
      auth: {
        token: ''
      }
    }
    writeFileSync(configFilePath, ini.stringify(defaultConfig), 'utf-8')
  }
}

export const readTokenFromIni = () => {
  try {
    checkIfExisests()
    const config = ini.parse(readFileSync(configFilePath, 'utf-8'))
    return config.auth?.token || null
  } catch (error) {
    console.error('Error reading config file:', error)
    return null
  }
}

export function writeToConfig(sectionName: string, pathKey: string, newValue: string): void {
  try {
    checkIfExisests()

    const config = ini.parse(readFileSync(configFilePath, 'utf-8'))

    if (!config[sectionName]) {
      config[sectionName] = {}
    }

    config[sectionName][pathKey] = newValue;

    writeFileSync(configFilePath, ini.stringify(config), 'utf-8')
  } catch (error) {
    console.error('Error wrting config file:', error)
  }
}

export const saveTokenToIni = (token: string) => {
  try {
    checkIfExisests()

    const config = ini.parse(readFileSync(configFilePath, 'utf-8'))
    config.auth = config.auth || {}
    config.auth.token = token
    writeFileSync(configFilePath, ini.stringify(config), 'utf-8')
  } catch (error) {
    console.error('Error saving to config file:', error)
  }
}