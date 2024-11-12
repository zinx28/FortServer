const fs = require('fs');
const axios = require('axios');

async function updateJsonFile() {
    const data = JSON.parse(fs.readFileSync('skins.json', 'utf8'));

    for (const item of data) {
        const itemId = item.item.split(':')[1];

        try {
            const response = await axios.get(`https://fortnite-api.com/v2/cosmetics/br/${itemId}`);
           // console.log( response.data.data.introduction);
            var season
            season = Number(response.data.data.introduction.backendValue);
                
   
          
            item.season = season;
        } catch (error) {
            console.error(`Failed to fetch data for item ${itemId}:`, error.message);
        }
    }


    fs.writeFileSync('test.json', JSON.stringify(data, null, 4), 'utf8');
    console.log('File updated with additional data!');
}

updateJsonFile();
