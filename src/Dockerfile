FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim AS build
WORKDIR /src

COPY ["./Common", "./Common"]
COPY ["./SecretsService", "./SecretsService"]
COPY ["./ProductSettings.cs", "./ProductSettings.cs"]

RUN dotnet restore "./SecretsService/SecretsService.csproj"

WORKDIR /src/SecretsService
RUN dotnet build "SecretsService.csproj" -c Release -o /app/build

FROM build as publish
RUN dotnet publish "SecretsService.csproj" -c Release -o /app/publish

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "Keebox.SecretsService.dll" ]