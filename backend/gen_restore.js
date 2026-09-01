const fs = require('fs');
const path = require('path');

const logPath = 'all_queries_utf8.txt';
const content = fs.readFileSync(logPath, 'utf8');
const lines = content.split('\n');

const scripts = [];

for (const line of lines) {
    if (!line) continue;
    try {
        const obj = JSON.parse(line);
        if (obj.tool_calls) {
            for (const tc of obj.tool_calls) {
                if (tc.name === 'default_api:run_command' && tc.arguments && tc.arguments.CommandLine) {
                    const cmd = tc.arguments.CommandLine;
                    if (cmd.includes('SNS.Application\\\\Projects\\\\Queries') || cmd.includes('SNS.Application/Projects/Queries')) {
                        scripts.push(cmd);
                    }
                }
            }
        }
    } catch(e) {}
}

fs.writeFileSync('restore.ps1', scripts.join('\n\n'));
console.log('Created restore.ps1 with ' + scripts.length + ' scripts.');
