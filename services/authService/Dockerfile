# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy only the files necessary for restore
COPY ["GetHub.sln", "."]
COPY ["services/authService/authService.csproj", "services/authService/"]
COPY ["shared/SharedKernel/SharedKernel.csproj", "shared/SharedKernel/"]

# Restore dependencies with optimizations
RUN dotnet restore "services/authService/authService.csproj" --verbosity minimal

# Copy only required source files
COPY services/authService/. ./services/authService/
COPY shared/SharedKernel/. ./shared/SharedKernel/

WORKDIR /src/services/authService

# Publish with optimizations
RUN dotnet publish "authService.csproj" -c Release -o /app/publish
# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app
COPY --from=build /app/publish .

# Environment configuration
ENV ASPNETCORE_URLS=http://+:5085

EXPOSE 5085

ENTRYPOINT ["dotnet", "authService.dll"]