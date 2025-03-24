export function FortniteDetect(buildString: string): string {
    if (buildString.includes('3724489')) {
      buildString = '1.8'
    }
    else if (buildString.includes('3700114')) {
      buildString = '1.7.2'
    } else if (buildString.includes('3807424')) {
      buildString = '1.11'
    } else if (buildString.includes('3870737')) {
      buildString = '2.4.2'
    } else if (buildString.includes('3741772')) {
      buildString = '1.8.2'
    } else if (buildString.includes('3240987')) {
      buildString = 'Alpha'
    } else {
      if (buildString.includes('-')) {
        const buildAdding = buildString.split('-')
  
        if (buildAdding.length >= 2) {
          buildString = buildAdding.slice(1).join('-')
        } else {
          buildString = 'Unknown'
        }
      } else {
        buildString = 'Unknown?'
      }
    }
    return buildString
  }