const fs = require('graceful-fs').promises;
const fetch = require('cross-fetch');

async function main() {
    const baseUrl = 'https://optimizer2.routesavvy.com/RSAPI.svc/';
    
    try {
        const args = process.argv.slice(2);
        if (args.length !== 2) {
            console.log('Usage: <input json request file> <output json result file>');
            console.log('Press any key to exit.');
            process.stdin.setRawMode(true);
            process.stdin.resume();
            process.stdin.on('data', process.exit.bind(process, 0));
            return;
        }

        const inFilePath = args[0];
        const outFilePath = args[1];

        const requestStr = await fs.readFile(inFilePath, 'utf-8');
        const endpoint = 'POSTOptimize';
        const requestUrl = `${baseUrl}${endpoint}`;

        const response = await fetch(requestUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: requestStr,
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const responseBody = await response.text();
        const jsonObject = JSON.parse(responseBody);
        const message = jsonObject.Message;

        if (message && message !== 'Success') {
            console.log(`An error occurred: ${message}`);
        } else {
            try {
                const outputJson = JSON.stringify(JSON.parse(responseBody), null, 2);
                await fs.writeFile(outFilePath, outputJson);
                console.log('Success');
            } catch (ex) {
                console.log(`An error occurred: ${ex.message}`);
            }
        }
    } catch (error) {
        console.error('Exception:', error.message);
    }
    
    console.log('Press any key to exit.');
    process.stdin.setRawMode(true);
    process.stdin.resume();
    process.stdin.on('data', process.exit.bind(process, 0));
}

main();