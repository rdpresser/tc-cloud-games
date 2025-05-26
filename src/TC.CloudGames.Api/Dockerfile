# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files separately for dependency caching
COPY ["src/TC.CloudGames.Api/TC.CloudGames.Api.csproj", "src/TC.CloudGames.Api/"]
COPY ["src/TC.CloudGames.Application/TC.CloudGames.Application.csproj", "src/TC.CloudGames.Application/"]
COPY ["src/TC.CloudGames.Domain/TC.CloudGames.Domain.csproj", "src/TC.CloudGames.Domain/"]
COPY ["src/TC.CloudGames.Infra.CrossCutting.Commons/TC.CloudGames.Infra.CrossCutting.Commons.csproj", "src/TC.CloudGames.Infra.CrossCutting.Commons/"]
COPY ["src/TC.CloudGames.Infra.CrossCutting.IoC/TC.CloudGames.Infra.CrossCutting.IoC.csproj", "src/TC.CloudGames.Infra.CrossCutting.IoC/"]
COPY ["src/TC.CloudGames.Infra.Data/TC.CloudGames.Infra.Data.csproj", "src/TC.CloudGames.Infra.Data/"]
RUN dotnet restore "./src/TC.CloudGames.Api/TC.CloudGames.Api.csproj"

# Copy everything after dependencies are restored
COPY . .
WORKDIR "/src/src/TC.CloudGames.Api"

# Run SonarScanner before build
RUN dotnet sonarscanner begin /k:"tc-cloudgames-local" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="sqp_c3c48b658840cb4ba8ae948682f254550baf4845"

# Build application
RUN dotnet build "./TC.CloudGames.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Run unit tests & collect code coverage
RUN dotnet test TC.CloudGames.sln /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./TestResults/coverage.xml

RUN dotnet test "./TC.CloudGames.Api.csproj" -c $BUILD_CONFIGURATION \
    /p:CollectCoverage=true \
    /p:CoverletOutputFormat=cobertura \
    /p:CoverletOutput=/app/TestResults/coverage.xml

# End SonarScanner analysis
RUN dotnet sonarscanner end /d:sonar.login="sqp_c3c48b658840cb4ba8ae948682f254550baf4845"

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TC.CloudGames.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime container
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TC.CloudGames.Api.dll"]
