#!/bin/bash
set -e

echo "Применение миграций..."
dotnet ef database update --no-build

echo "Запуск приложения..."
exec dotnet FlashcardsApp.dll