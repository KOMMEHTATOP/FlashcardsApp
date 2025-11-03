#!/bin/bash
set -e

echo "==================================="
echo "FlashcardsApp API Starting..."
echo "==================================="
echo "Environment: $ASPNETCORE_ENVIRONMENT"
echo "==================================="

# Start the application
# 'exec' replaces bash process with dotnet process
# This ensures Docker signals (SIGTERM) are handled correctly
exec dotnet FlashcardsApp.Api.dll