// this was copied from a different project, if the exe doesnt launch after rerun the command and it will fix it
//out of 0.2/10 of the time it will happen, dw
const fs = require('fs');
const path = require('path');
const javascriptObfuscator = require('javascript-obfuscator');

const sourceDir = path.join(__dirname, 'out');

function obfuscateDirectory(directory) {
    const files = fs.readdirSync(directory);
    files.forEach(file => {
        const filePath = path.join(directory, file);
        const stats = fs.statSync(filePath);

        if (stats.isDirectory()) {
            obfuscateDirectory(filePath);
        } else if (filePath.endsWith('.js')) {
            obfuscateFile(filePath);
        }
    });
}

function obfuscateFile(filePath) {
    const content = fs.readFileSync(filePath, 'utf-8');
    const obfuscatedCode = javascriptObfuscator.obfuscate(content, {
        compact: true,
        controlFlowFlattening: true,
        controlFlowFlatteningThreshold: 1,
        deadCodeInjection: true,
        stringArray: true,
        stringArrayEncoding: ['base64'],
        stringArrayThreshold: 0.75
      }).getObfuscatedCode();
    
    const outputFilePath = path.join(sourceDir, path.relative(sourceDir, filePath));
    fs.mkdirSync(path.dirname(outputFilePath), { recursive: true });
    fs.writeFileSync(outputFilePath, obfuscatedCode);
    console.log(`Obfuscated: ${filePath}`);
}

obfuscateDirectory(sourceDir);