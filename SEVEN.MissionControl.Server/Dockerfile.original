FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["SEVEN.MissionControl.Server/SEVEN.MissionControl.Server.csproj", "SEVEN.MissionControl.Server/"]
COPY ["SEVEN.Core/SEVEN.Core.csproj", "SEVEN.Core/"]
RUN dotnet restore "SEVEN.MissionControl.Server/SEVEN.MissionControl.Server.csproj"
COPY . .
WORKDIR "/src/SEVEN.MissionControl.Server"
RUN dotnet build "SEVEN.MissionControl.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SEVEN.MissionControl.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SEVEN.MissionControl.Server.dll"]
