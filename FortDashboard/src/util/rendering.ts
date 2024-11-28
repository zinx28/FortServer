import ejs from 'ejs'
import * as path from 'path'
/*
    : file/path
    : data (data that ejs uses!)
*/
export async function renderEJS(PageName: string, data: Record<string, any>): Promise<string> {
    try {
      const html = await ejs.renderFile(path.join(__dirname, `../views/${PageName}`), data)
      return html
    } catch (err) {
      console.error('Error rendering:', err)
      return 'Error rendering the page :('
    }
  } 