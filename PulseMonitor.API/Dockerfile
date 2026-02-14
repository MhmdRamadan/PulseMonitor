FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY PulseMonitor.sln ./
COPY PulseMonitor.API/PulseMonitor.API.csproj PulseMonitor.API/
COPY PulseMonitor.Application/PulseMonitor.Application.csproj PulseMonitor.Application/
COPY PulseMonitor.Infrastructure/PulseMonitor.Infrastructure.csproj PulseMonitor.Infrastructure/
COPY PulseMonitor/PulseMonitor.Domain.csproj PulseMonitor/
RUN dotnet restore PulseMonitor.API/PulseMonitor.API.csproj
COPY . .
RUN dotnet build PulseMonitor.API/PulseMonitor.API.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish PulseMonitor.API/PulseMonitor.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PulseMonitor.API.dll"]
