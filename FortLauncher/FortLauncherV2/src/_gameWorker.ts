// doesnt support alpha builds or latest atm
import { existsSync } from 'fs'
import path from 'path'

import dllinjector from '../resources/dllinjector.node'
import { parentPort, workerData } from 'worker_threads'
import { execSync, spawn } from 'child_process'

try {
  const { gameExePath, dllPath, user } = workerData
  console.log(gameExePath)
  const ShippingEAC = path.join(
    gameExePath,
    'FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping_BE.exe'
  )

  let EACProcess = null
  let FortniteLauncherProcess = null

  if (existsSync(ShippingEAC)) {
    EACProcess = spawn(ShippingEAC)
    console.log(EACProcess.pid)
    dllinjector.freezeProcess(EACProcess.pid)
  }

  const ForniteLauncher = path.join(
    gameExePath,
    'FortniteGame\\Binaries\\Win64\\FortniteLauncher.exe'
  )

  if (existsSync(ForniteLauncher)) {
    FortniteLauncherProcess = spawn(ForniteLauncher)
    dllinjector.freezeProcess(FortniteLauncherProcess.pid)
  }

  // should allow to launch on higher versions!
  var gameExecutablePath = path.join(
    gameExePath,
    'FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping_EAC_EOS.exe'
  )

  if (!existsSync(gameExecutablePath))
    gameExecutablePath = path.join(
      gameExePath,
      'FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe'
    )

  if (existsSync(gameExecutablePath)) {
    execSync('set OPENSSL_ia32cap=:~0x20000000')
    const gameProcess = spawn(
      gameExecutablePath,
      (
        '-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -skippatchcheck -noeac -fromfl=be -fltoken=h1cdhchd10150221h130eB56 -caldera=eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCJ9.eyJhY2NvdW50X2lkIjoiYmU5ZGE1YzJmYmVhNDQwN2IyZjQwZWJhYWQ4NTlhZDQiLCJnZW5lcmF0ZWQiOjE2Mzg3MTcyNzgsImNhbGRlcmFHdWlkIjoiMzgxMGI4NjMtMmE2NS00NDU3LTliNTgtNGRhYjNiNDgyYTg2IiwiYWNQcm92aWRlciI6IkVhc3lBbnRpQ2hlYXQiLCJub3RlcyI6IiIsImZhbGxiYWNrIjpmYWxzZX0.VAWQB67RTxhiWOxx7DBjnzDnXyyEnX7OljJm-j2d88G_WgwQ9wrE6lwMEHZHjBd1ISJdUO1UVUqkfLdU5nofBQ -frombe AUTH_TYPE=exchangecode -AUTH_LOGIN=unused -AUTH_PASSWORD=' +
        user.user?.AccessToken
      ).split(' '),
      { env: { OPENSSL_ia32cap: ':~0x20000000' } }
    )
    dllinjector.injectDll(gameProcess.pid, dllPath)

    gameProcess.on('close', (code: any) => {
      if (EACProcess) {
        console.log(`Closing EAC Process with PID: ${EACProcess.pid}`)
        EACProcess.kill()
      }

      if (FortniteLauncherProcess) {
        console.log(`Closing Launcher Process with PID: ${FortniteLauncherProcess.pid}`)
        FortniteLauncherProcess.kill()
      }
    })

    parentPort?.postMessage({ status: 'success' })
  } else parentPort?.postMessage({ status: 'error', message: 'missing path?' })
} catch (error) {
  parentPort?.postMessage({ status: 'error', message: error })
}
