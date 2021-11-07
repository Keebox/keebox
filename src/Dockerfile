FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine  AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src

COPY ["./src/Common", "./src/Common"]
COPY ["./src/SecretsService", "./src/SecretsService"]
COPY ["./ProductSettings.cs", "./ProductSettings.cs"]

RUN dotnet restore "./src/SecretsService/SecretsService.csproj"

WORKDIR /src/src/SecretsService
RUN dotnet build "SecretsService.csproj" -c Release -o /app/build

FROM build as publish
RUN dotnet publish "SecretsService.csproj" -c Release -o /app/publish

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "Keebox.SecretsService.dll" ]