# ===========================================
# FlashcardsApp API - Production Dockerfile
# ===========================================
# Multi-stage build for optimal image size and security

# ===========================================
# Stage 1: BUILD
# ===========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files for dependency caching
# Docker caches each layer - if .csproj files don't change, restore is cached
COPY FlashcardsApp.sln ./
COPY Directory.Build.props ./
COPY Directory.Packages.props ./

# Copy all .csproj files to their respective directories
COPY backend/FlashcardsApp.Api/FlashcardsApp.Api.csproj ./backend/FlashcardsApp.Api/
COPY backend/FlashcardsApp.BLL/FlashcardsApp.BLL.csproj ./backend/FlashcardsApp.BLL/
COPY backend/FlashcardsApp.DAL/FlashcardsApp.DAL.csproj ./backend/FlashcardsApp.DAL/
COPY backend/FlashcardsApp.Models/FlashcardsApp.Models.csproj ./backend/FlashcardsApp.Models/
COPY backend/FlashcardsApp.Services/FlashcardsApp.Services.csproj ./backend/FlashcardsApp.Services/
COPY backend/FlashcardsApp.Tools/FlashcardsApp.Tools.csproj ./backend/FlashcardsApp.Tools/

# Restore NuGet packages - this layer is cached if .csproj files don't change
RUN dotnet restore "backend/FlashcardsApp.Api/FlashcardsApp.Api.csproj"

# Copy the rest of the source code
COPY backend/. ./backend/

# Build and publish the application
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
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Create non-root user for security
# Running as root is a security risk in production
RUN groupadd -r appuser && useradd -r -g appuser appuser

WORKDIR /app

# Copy published application from build stage
COPY --from=build /app/publish ./

# Copy entrypoint script for running migrations before app starts
COPY backend/FlashcardsApp.Api/entrypoint.sh ./
RUN chmod +x entrypoint.sh

# Change ownership to non-root user
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose port (informational - actual port mapping in docker-compose)
EXPOSE 8080

# Health check - Docker will restart container if unhealthy
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Use entrypoint script to run migrations before starting app
ENTRYPOINT ["./entrypoint.sh"]