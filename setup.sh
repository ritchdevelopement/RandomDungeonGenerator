#!/bin/sh

echo "Setting up development environment..."

# Git hooks
git config core.hooksPath .githooks
echo "[OK] Git hooks activated (.githooks/)"

# Check dotnet
if command -v dotnet > /dev/null 2>&1; then
    echo "[OK] dotnet SDK found: $(dotnet --version)"
else
    echo "[WARN] dotnet SDK not found. Install it to use the pre-commit formatter."
    echo "       https://dotnet.microsoft.com/download"
fi

echo ""
echo "Setup complete. You're ready to go!"
