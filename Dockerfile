FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Tetrio.Foxhole.Backend.Runtime/Tetrio.Foxhole.Backend.Runtime.csproj", "./"]
#COPY ["Tetrio.Network/Tetrio.Network.csproj", "./Network"]
#COPY ["Tetrio.ZenithDaily.Database/Tetrio.ZenithDaily.Database.csproj", "./Database"]
#COPY ["Tetrio.ZenithDaily.Challenge/Tetrio.ZenithDaily.Challenge.csproj", "./DailyChallenge"]
RUN dotnet restore "Tetrio.Foxhole.Backend.Runtime.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Tetrio.Foxhole.Backend.Runtime/Tetrio.Foxhole.Backend.Runtime.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Tetrio.Foxhole.Backend.Runtime/Tetrio.Foxhole.Backend.Runtime.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false -r linux-x64

FROM base AS final

USER root

#RUN apt-get update && \
#    apt-get install -y libfontconfig1 libfreetype6 fontconfig && \
#    apt-get clean

WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Tetrio.Foxhole.Backend.Runtime.dll"]