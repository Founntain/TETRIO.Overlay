FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Tetrio.Overlay/Tetrio.Overlay.csproj", "./"]
#COPY ["Tetrio.Overlay.Network/Tetrio.Overlay.Network.csproj", "./Network"]
#COPY ["Tetrio.Overlay.Database/Tetrio.Overlay.Database.csproj", "./Database"]
#COPY ["Tetrio.Zenith.DailyChallenge/Tetrio.Zenith.DailyChallenge.csproj", "./DailyChallenge"]
RUN dotnet restore "Tetrio.Overlay.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Tetrio.Overlay/Tetrio.Overlay.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Tetrio.Overlay/Tetrio.Overlay.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false -r linux-x64

FROM base AS final

USER root

#RUN apt-get update && \
#    apt-get install -y libfontconfig1 libfreetype6 fontconfig && \
#    apt-get clean

WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Tetrio.Overlay.dll"]