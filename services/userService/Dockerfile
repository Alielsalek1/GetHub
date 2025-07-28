# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy only the files necessary for restore
COPY ["GetHub.sln", "."]
COPY ["services/userService/userService.csproj", "services/userService/"]
COPY ["shared/SharedKernel/SharedKernel.csproj", "shared/SharedKernel/"]

# Restore dependencies with optimizations
RUN dotnet restore "services/userService/userService.csproj" --verbosity normal

# Copy only required source files
COPY services/userService/. ./services/userService/
COPY shared/SharedKernel/. ./shared/SharedKernel/

WORKDIR /src/services/userService

RUN dotnet publish "userService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app
COPY --from=build /app/publish .

# Environment configuration
ENV ASPNETCORE_URLS=http://+:5082
EXPOSE 5082

ENTRYPOINT ["dotnet", "userService.dll"]
