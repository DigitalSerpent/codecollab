import sys
import json
import subprocess

data = json.loads(sys.argv[1])
language = data.get('language')
code = data.get('code')

if language == 'python':
    exec(code)
elif language == 'javascript' or language == 'js':
    subprocess.run(['node', '-e', code])
elif language == 'bash':
    subprocess.run(['bash', '-c', code])
elif language == 'ruby':
    subprocess.run(['ruby', '-e', code])
else:
    print(f"Язык {language} пока не поддерживается")