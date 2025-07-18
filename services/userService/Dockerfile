# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy only the files necessary for restore
COPY ["GetHub.sln", "."]
COPY ["services/userService/userService.csproj", "services/userService/"]
COPY ["shared/SharedKernel/SharedKernel.csproj", "shared/SharedKernel/"]

# Restore dependencies
RUN dotnet restore "services/userService/userService.csproj"

# Copy only required source files
COPY services/userService/. ./services/userService/
COPY shared/SharedKernel/. ./shared/SharedKernel/

WORKDIR /src/services/userService

RUN dotnet publish "userService.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# install MongoDB and prepare data directory
RUN apt-get update && apt-get install -y mongodb && mkdir -p /data/db
VOLUME /data/db

WORKDIR /app
COPY --from=build /app/publish .

# configure URLs and expose app and MongoDB ports
ENV ASPNETCORE_URLS=http://+:5082
EXPOSE 5082
EXPOSE 27017

# start MongoDB in background, then launch the ASP.NET app
ENTRYPOINT ["bash","-lc","mongod --dbpath /data/db --bind_ip_all & exec dotnet userService.dll"]