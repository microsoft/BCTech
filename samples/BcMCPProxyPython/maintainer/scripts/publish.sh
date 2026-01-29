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
echo ""
echo "When prompted for credentials:"
echo "  Username: __token__"
echo "  Password: <your PyPI API token starting with 'pypi-'>"
echo ""

if [ -n "${TWINE_PASSWORD:-}" ]; then
    echo "Using TWINE_PASSWORD environment variable..."
    python -m twine upload --repository "${REPOSITORY}" --username "__token__" --password "${TWINE_PASSWORD}" dist/*
else
python -m twine upload --repository "${REPOSITORY}" dist/*
fi

