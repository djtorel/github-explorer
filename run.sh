#!/usr/bin/env bash
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo "==> Building frontend..."
cd "$SCRIPT_DIR/src/frontend"
npm install
npm run build

echo "==> Starting backend..."
cd "$SCRIPT_DIR/src/GitHubExplorer.Api"
dotnet run "$@"
