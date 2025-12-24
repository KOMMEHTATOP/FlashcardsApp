#!/bin/bash
set -e

echo "==================================="
echo "FlashcardsApp API Starting..."
echo "==================================="
echo "Environment: $ASPNETCORE_ENVIRONMENT"
echo "==================================="

exec dotnet FlashcardsApp.Api.dll