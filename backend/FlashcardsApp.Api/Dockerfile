# Stage 1: Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Stage 2: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all project files for restore
COPY ["FlashcardsApp.Api/FlashcardsApp.Api.csproj", "FlashcardsApp.Api/"]
COPY ["FlashcardsApp.BLL/FlashcardsApp.BLL.csproj", "FlashcardsApp.BLL/"]
COPY ["FlashcardsApp.DAL/FlashcardsApp.DAL.csproj", "FlashcardsApp.DAL/"]
COPY ["FlashcardsApp.Models/FlashcardsApp.Models.csproj", "FlashcardsApp.Models/"]
COPY ["FlashcardsApp.Services/FlashcardsApp.Services.csproj", "FlashcardsApp.Services/"]

# Restore dependencies
RUN dotnet restore "FlashcardsApp.Api/FlashcardsApp.Api.csproj"

# Copy all source code
COPY . .

# Build the API project
WORKDIR "/src/FlashcardsApp.Api"
RUN dotnet build "FlashcardsApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FlashcardsApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "FlashcardsApp.Api.dll"]