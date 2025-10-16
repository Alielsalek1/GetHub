# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy only the files necessary for restore
COPY ["GetHub.sln", "."]
COPY ["services/userService/userService.csproj", "services/userService/"]
COPY ["services/shared/Shared.csproj", "services/shared/"]

# Restore dependencies with optimizations
RUN dotnet restore "services/userService/userService.csproj" --verbosity normal

# Copy only required source files
COPY services/userService/. ./services/userService/
COPY services/shared/. ./services/shared/

WORKDIR /src/services/userService

RUN dotnet publish "userService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app
COPY --from=build /app/publish .

# Environment configuration
ENV ASPNETCORE_URLS=http://+:5082
EXPOSE 5082

ENTRYPOINT ["dotnet", "userService.dll"]
