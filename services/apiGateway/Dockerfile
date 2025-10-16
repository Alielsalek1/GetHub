# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy only the files necessary for restore
COPY ["GetHub.sln", "."]
COPY ["services/apiGateway/ApiGateway.csproj", "services/apiGateway/"]
COPY ["services/shared/Shared.csproj", "services/shared/"]

# Restore dependencies with optimizations
RUN dotnet restore "services/apiGateway/ApiGateway.csproj" --verbosity minimal

# Copy only required source files
COPY services/apiGateway/. ./services/apiGateway/
COPY services/shared/. ./services/shared/

WORKDIR /src/services/apiGateway

# Publish with optimizations
RUN dotnet publish "ApiGateway.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app
COPY --from=build /app/publish .

# Environment configuration
ENV ASPNETCORE_URLS=http://+:5000

EXPOSE 5000

ENTRYPOINT ["dotnet", "ApiGateway.dll"]