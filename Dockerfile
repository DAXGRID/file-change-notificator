FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build-env
WORKDIR /app

COPY ./*sln ./

COPY ./src/FileChangeNotificator/*.csproj ./src/FileChangeNotificator/

RUN dotnet restore --packages ./packages

COPY . ./
WORKDIR /app/src/FileChangeNotificator
RUN dotnet publish -c Release -o out --packages ./packages

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:10.0-alpine
WORKDIR /app

COPY --from=build-env --chown=65534:65534 /app/src/FileChangeNotificator/out .

USER 65534

ENTRYPOINT ["dotnet", "FileChangeNotificator.dll"]
