FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SEVEN.MissionControl.Api/SEVEN.MissionControl.Api.csproj", "SEVEN.MissionControl.Api/"]
COPY ["SEVEN.Core/SEVEN.Core.csproj", "SEVEN.Core/"]
RUN dotnet restore "SEVEN.MissionControl.Api/SEVEN.MissionControl.Api.csproj"
COPY . .
WORKDIR "/src/SEVEN.MissionControl.Api"
RUN dotnet build "SEVEN.MissionControl.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SEVEN.MissionControl.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SEVEN.MissionControl.Api.dll"]
