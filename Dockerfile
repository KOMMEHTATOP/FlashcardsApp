# Base image для сборки (.NET SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем .sln и все .csproj для кеширования зависимостей
# ИСПРАВЛЕНО: Путь теперь относительно корня контекста сборки (.), то есть просто имя файла
COPY FlashcardsApp.sln . 
COPY backend/FlashcardsApp.Api/FlashcardsApp.Api.csproj ./backend/FlashcardsApp.Api/
COPY backend/FlashcardsApp.BLL/FlashcardsApp.BLL.csproj ./backend/FlashcardsApp.BLL/
COPY backend/FlashcardsApp.DAL/FlashcardsApp.DAL.csproj ./backend/FlashcardsApp.DAL/
COPY backend/FlashcardsApp.Models/FlashcardsApp.Models.csproj ./backend/FlashcardsApp.Models/
COPY backend/FlashcardsApp.Services/FlashcardsApp.Services.csproj ./backend/FlashcardsApp.Services/

# ИСПРАВЛЕНО: Пути теперь относительно корня контекста сборки (.), то есть просто имя файла
COPY Directory.Build.props .
COPY Directory.Packages.props .

# Восстанавливаем зависимости
RUN dotnet restore "backend/FlashcardsApp.Api/FlashcardsApp.Api.csproj"

# Копируем остальной исходный код из папки backend
COPY backend/. ./backend/

# Публикуем основной API проект
WORKDIR /src/backend/FlashcardsApp.Api/
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FlashcardsApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 3: Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Запуск приложения
ENTRYPOINT ["dotnet", "FlashcardsApp.Api.dll"]
