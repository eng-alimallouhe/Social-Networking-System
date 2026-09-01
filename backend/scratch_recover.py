import json

log_path = r'C:\Users\Ali Mallouhe\.gemini\antigravity-ide\brain\8f9eea1d-2cd8-49f7-838a-a1cd10b4bb80\.system_generated\logs\transcript_full.jsonl'
with open(log_path, 'r', encoding='utf-8') as f:
    for line in f:
        try:
            data = json.loads(line)
            if 'tool_calls' in data:
                for tc in data['tool_calls']:
                    if tc.get('name') == 'default_api:write_to_file':
                        args = tc.get('arguments', {})
                        target = args.get('TargetFile', '').replace('\\\\', '/')
                        if 'SNS.Application/Projects/Queries' in target:
                            print(f'=== {target} ===')
                            print(args.get('CodeContent'))
                            print('='*40)
        except:
            pass
