#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["KrisG.SickBox.WorkerService/KrisG.SickBox.WorkerService.csproj", "KrisG.SickBox.WorkerService/"]
COPY ["KrisG.SickBox.Core/KrisG.SickBox.Core.csproj", "KrisG.SickBox.Core/"]
COPY ["KrisG.KickassTorrents.Client/KrisG.KickassTorrents.Client.csproj", "KrisG.KickassTorrents.Client/"]
COPY ["KrisG.Utility/KrisG.Utility.csproj", "KrisG.Utility/"]
COPY ["KrisG.SickBeard.Client/KrisG.SickBeard.Client.csproj", "KrisG.SickBeard.Client/"]
COPY ["KrisG.IpTorrents.Client/KrisG.IpTorrents.Client.csproj", "KrisG.IpTorrents.Client/"]
COPY ["KrisG.SickBox.Core.Interfaces/KrisG.SickBox.Core.Interfaces.csproj", "KrisG.SickBox.Core.Interfaces/"]
RUN dotnet restore "KrisG.SickBox.WorkerService/KrisG.SickBox.WorkerService.csproj"
COPY . .
WORKDIR "/src/KrisG.SickBox.WorkerService"
RUN dotnet build "KrisG.SickBox.WorkerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KrisG.SickBox.WorkerService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KrisG.SickBox.WorkerService.dll"]
