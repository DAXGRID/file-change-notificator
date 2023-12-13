FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY ./*sln ./

COPY ./src/FileChangeNotificator/*.csproj ./src/FileChangeNotificator/

RUN dotnet restore --packages ./packages

COPY . ./
WORKDIR /app/src/FileChangeNotificator
RUN dotnet publish -c Release -o out --packages ./packages

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app

COPY --from=build-env /app/src/FileChangeNotificator/out .
ENTRYPOINT ["dotnet", "FileChangeNotificator.dll"]
