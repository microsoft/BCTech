#!/usr/bin/env bash
set -euo pipefail

REPOSITORY="${1:-pypi}"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR/../.."

if [ ! -d "dist" ]; then
  echo "dist/ folder not found. Run maintainer/scripts/build.sh first."
  exit 1
fi

echo "Installing twine..."
python -m pip install --upgrade twine >/dev/null

echo "Uploading artifacts to '${REPOSITORY}'..."
python -m twine upload --repository "${REPOSITORY}" dist/*

