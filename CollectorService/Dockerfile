FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["CollectorService/CollectorService.csproj", "CollectorService/"]
RUN dotnet restore "CollectorService/CollectorService.csproj"
COPY . .
WORKDIR "/src/CollectorService"
RUN dotnet build "CollectorService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CollectorService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CollectorService.dll"]
