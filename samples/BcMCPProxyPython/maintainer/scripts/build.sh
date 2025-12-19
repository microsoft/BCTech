#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR/../.."

echo "Installing build backend..."
python -m pip install --upgrade build >/dev/null

echo "Building bc-mcp-proxy package..."
python -m build

echo "Artifacts written to:"
ls -1 dist

