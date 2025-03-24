import { openSync, readSync, statSync } from "fs";


// copied from luna, this was 1:1 copy of my c# version
// the searching under is from a different project "luna" that i worked on, this is only for backup!
function search(src: Buffer, pattern: Buffer): number[] {
    let indices: number[] = [];
    const maxSearchIndex = src.length - pattern.length;

    for (let i = 0; i <= maxSearchIndex; i++) {
        if (src[i] !== pattern[0]) continue;

        let found = true;
        for (let j = 1; j < pattern.length; j++) {
            if (src[i + j] !== pattern[j]) {
                found = false;
                break;
            }
        }

        if (found) {
            indices.push(i);
        }
    }

    return indices;
}

export async function getBuildVersion(exePath: string): Promise<string> {

    // TODO just allow certain fortnite builds in the launcher!
    // with file hash instead

    let result = '';
    const numThreads = require('os').cpus().length;
    let allMatchingPos: number[] = [];
    const pattern = Buffer.from('++Fortnite+Release-', 'utf16le');

    try {
        const fileSize = statSync(exePath).size;
        const chunkSize = Math.floor(fileSize / numThreads);

        const tasks = Array.from({ length: numThreads }, (_, i) => {
            return new Promise<void>((resolve, reject) => {
                const startPosition = i * chunkSize;
                const endPosition = i === numThreads - 1 ? fileSize : startPosition + chunkSize;
                const fd = openSync(exePath, 'r');
                const buffer = Buffer.alloc(endPosition - startPosition);

                try {
                    readSync(fd, buffer, 0, buffer.length, startPosition);
                    const matchingPositions = search(buffer, pattern);
        
                    allMatchingPos.push(...matchingPositions.map(pos => pos + startPosition));
                    resolve();
                } catch (err) {
                    reject(err);
                }
            });
        });

        await Promise.all(tasks);

        if (allMatchingPos.length > 0) {
            const fd = openSync(exePath, 'r'); // we cant keep opening this lol
            for (const pos of allMatchingPos) {
                const buffer = Buffer.alloc(100);
                readSync(fd, buffer, 0, buffer.length, pos);

                const chunkText = buffer.toString('utf16le');
               
                // higher builds cant grab this <3
                const match = chunkText.match(/\+\+Fortnite\+Release-(\d+(\.\d+){0,2}|Live|Next|Cert)-CL-\d+/i);

                if (match) {
                    result = match[0];
                    break;
                }
            }
        }

        console.log("VersionSearcher->", result);
        return result || "ERROR";

    } catch (ex) {
        console.error(ex);
        return "ERROR";
    }
}