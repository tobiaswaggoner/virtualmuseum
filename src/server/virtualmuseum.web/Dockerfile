# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY virtualmuseum.web.api/*.csproj ./virtualmuseum.web.api/
COPY virtualmuseum.web.data/*.csproj ./virtualmuseum.web.data/
RUN dotnet restore

# copy everything else and build app
COPY virtualmuseum.web.data/. ./virtualmuseum.web.data/
COPY virtualmuseum.web.api/. ./virtualmuseum.web.api/

WORKDIR /source/virtualmuseum.web.api
RUN dotnet build -c release 

WORKDIR /source/virtualmuseum.web.api
RUN dotnet publish -c release -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
CMD ["dotnet", "virtualmuseum.web.api.dll"]