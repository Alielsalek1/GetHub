FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy solution and project files
COPY ["GetHub.sln", "."]
COPY ["services/catalogService/CatalogService.csproj", "services/catalogService/"]
COPY ["services/shared/Shared.csproj", "services/shared/"]

# Restore dependencies
RUN dotnet restore "services/catalogService/CatalogService.csproj"

# Copy source code
COPY services/catalogService/. ./services/catalogService/
COPY services/shared/. ./services/shared/

WORKDIR /src/services/catalogService

# Build and publish
RUN dotnet publish "CatalogService.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5083
EXPOSE 5083

ENTRYPOINT ["dotnet", "CatalogService.dll"]
