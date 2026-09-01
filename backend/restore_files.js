const fs = require('fs');

const logPath = 'all_queries_utf8.txt';
const content = fs.readFileSync(logPath, 'utf8');
const lines = content.split('\n');
const fileContents = {};

for (const line of lines) {
    if (!line) continue;
    try {
        const obj = JSON.parse(line);
        if (obj.tool_calls) {
            for (const tc of obj.tool_calls) {
                if (tc.name === 'default_api:write_to_file' && tc.arguments && tc.arguments.TargetFile) {
                    const target = tc.arguments.TargetFile;
                    if (target.includes('SNS.Application\\\\Projects\\\\Queries') || target.includes('SNS.Application/Projects/Queries')) {
                        fileContents[target] = tc.arguments.CodeContent;
                    }
                }
                if (tc.name === 'default_api:multi_replace_file_content' && tc.arguments && tc.arguments.TargetFile) {
                    const target = tc.arguments.TargetFile;
                    if (fileContents[target]) {
                        for (const chunk of tc.arguments.ReplacementChunks) {
                            fileContents[target] = fileContents[target].replace(chunk.TargetContent, chunk.ReplacementContent);
                        }
                    }
                }
            }
        }
    } catch(e) {}
}

const path = require('path');
for (const [filepath, content] of Object.entries(fileContents)) {
    const normPath = filepath.replace(/\\\\/g, '/');
    if (!normPath.includes('GetSkillsQuery')) { // Skip GetSkills
        console.log('Restoring: ' + normPath);
        const dir = path.dirname(normPath);
        fs.mkdirSync(dir, { recursive: true });
        fs.writeFileSync(normPath, content);
    }
}
