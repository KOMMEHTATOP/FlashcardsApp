FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FlashcardsApp/FlashcardsApp.csproj", "FlashcardsApp/"]
COPY ["FlashcardsAppContracts/FlashcardsAppContracts.csproj", "FlashcardsAppContracts/"]
RUN dotnet restore "FlashcardsApp/FlashcardsApp.csproj"
COPY . .
WORKDIR "/src/FlashcardsApp"
RUN dotnet build "./FlashcardsApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FlashcardsApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlashcardsApp.dll"]