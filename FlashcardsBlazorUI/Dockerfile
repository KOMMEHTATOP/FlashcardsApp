FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["FlashcardsBlazorUI/FlashcardsBlazorUI.csproj", "FlashcardsBlazorUI/"]
COPY ["FlashcardsAppContracts/FlashcardsAppContracts.csproj", "FlashcardsAppContracts/"]
RUN dotnet restore "FlashcardsBlazorUI/FlashcardsBlazorUI.csproj"

COPY ["FlashcardsBlazorUI/", "FlashcardsBlazorUI/"]
COPY ["FlashcardsAppContracts/", "FlashcardsAppContracts/"]

WORKDIR "/src/FlashcardsBlazorUI"
RUN dotnet build "FlashcardsBlazorUI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FlashcardsBlazorUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "FlashcardsBlazorUI.dll"]