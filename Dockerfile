# ===========================================
# Stage 1: BUILD
# ===========================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY backend/FlashcardsApp.sln ./backend/
COPY backend/Directory.Build.props ./backend/
COPY backend/Directory.Packages.props ./backend/

COPY backend/FlashcardsApp.Api/FlashcardsApp.Api.csproj ./backend/FlashcardsApp.Api/
COPY backend/FlashcardsApp.BLL/FlashcardsApp.BLL.csproj ./backend/FlashcardsApp.BLL/
COPY backend/FlashcardsApp.DAL/FlashcardsApp.DAL.csproj ./backend/FlashcardsApp.DAL/
COPY backend/FlashcardsApp.Models/FlashcardsApp.Models.csproj ./backend/FlashcardsApp.Models/
COPY backend/FlashcardsApp.Services/FlashcardsApp.Services.csproj ./backend/FlashcardsApp.Services/
COPY backend/FlashcardsApp.Tools/FlashcardsApp.Tools.csproj ./backend/FlashcardsApp.Tools/

WORKDIR /src/backend
RUN dotnet restore "FlashcardsApp.Api/FlashcardsApp.Api.csproj"

COPY backend/. ./

WORKDIR /src/backend/FlashcardsApp.Api
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FlashcardsApp.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false \
    --no-restore

# ===========================================
# Stage 2: RUNTIME
# ===========================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

RUN groupadd -r appuser && useradd -r -g appuser appuser

WORKDIR /app

COPY --from=build /app/publish ./

COPY backend/FlashcardsApp.Api/entrypoint.sh ./
RUN sed -i 's/\r$//' ./entrypoint.sh
RUN chmod +x entrypoint.sh

RUN chown -R appuser:appuser /app

USER appuser

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["./entrypoint.sh"]