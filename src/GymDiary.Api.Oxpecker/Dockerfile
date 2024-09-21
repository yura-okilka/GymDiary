FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/GymDiary.Api.Oxpecker/GymDiary.Api.Oxpecker.fsproj", "src/GymDiary.Api.Oxpecker/"]
RUN dotnet restore "src/GymDiary.Api.Oxpecker/GymDiary.Api.Oxpecker.fsproj"
COPY . .
WORKDIR "/src/src/GymDiary.Api.Oxpecker"
RUN dotnet build "GymDiary.Api.Oxpecker.fsproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "GymDiary.Api.Oxpecker.fsproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GymDiary.Api.Oxpecker.dll"]
