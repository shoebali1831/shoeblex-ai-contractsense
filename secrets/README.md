# Local Secrets Folder

Use this folder to keep local API keys and tokens on your machine.

Rules:
- Do not commit real keys.
- Keep actual values in `secrets/keys.local.env`.
- Use `secrets/keys.example.env` only as a template.
- Backend auto-loads `secrets/keys.local.env` at startup.

Example load command (zsh/bash):
```bash
set -a
source secrets/keys.local.env
set +a
```
